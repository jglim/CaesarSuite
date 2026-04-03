using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Caesar
{
    /// <summary>
    /// Disassembler for the Mercedes DSC (Diagnostic Script Container) VM bytecode.
    /// The DSC VM is a stack-based virtual machine used in CBF diagnostic files.
    /// Opcode table reverse-engineered from empirical analysis of representative DSC blobs.
    /// </summary>
    public class DSCDisassembler
    {
        private enum FlowControlKind
        {
            None,
            ConditionalBranch,
            UnconditionalBranch,
            Return,
        }

        private readonly byte[] _code;
        private int _pc;
        private readonly StringBuilder _output = new StringBuilder();
        private readonly Dictionary<int, string> _globalVarNames;
        private readonly List<(int EntryPoint, string Name)> _functionsByEntryPoint;
        private FlowControlKind _lastFlowControl;
        private int? _lastFlowTarget;

        public DSCDisassembler(
            byte[] dscBlobBytes,
            Dictionary<int, string> globalVarNames = null,
            IEnumerable<(int EntryPoint, string Name)> functionsByEntryPoint = null)
        {
            _code = dscBlobBytes;
            _globalVarNames = globalVarNames ?? new Dictionary<int, string>();
            _functionsByEntryPoint = (functionsByEntryPoint ?? Enumerable.Empty<(int EntryPoint, string Name)>())
                .Where(x => x.EntryPoint >= 0)
                .OrderBy(x => x.EntryPoint)
                .ToList();
        }

        private byte ReadByte() => _code[_pc++];
        private ushort ReadUInt16() { var v = BitConverter.ToUInt16(_code, _pc); _pc += 2; return v; }
        private short ReadInt16() { var v = BitConverter.ToInt16(_code, _pc); _pc += 2; return v; }
        private uint ReadUInt32() { var v = BitConverter.ToUInt32(_code, _pc); _pc += 4; return v; }
        private int ReadInt32() { var v = BitConverter.ToInt32(_code, _pc); _pc += 4; return v; }

        private void SetFlowControl(FlowControlKind kind, int? target = null)
        {
            _lastFlowControl = kind;
            _lastFlowTarget = target;
        }

        private bool TryReadStorageOperand(out string target)
        {
            target = null;
            if (_pc >= _code.Length)
            {
                return false;
            }

            byte mode = _code[_pc];
            switch (mode)
            {
                case 0x01:
                {
                    if (_pc + 1 >= _code.Length) return false;
                    _pc++;
                    byte index = ReadByte();
                    target = $"local[{index}]";
                    return true;
                }

                case 0x03:
                {
                    if (_pc + 1 >= _code.Length) return false;
                    _pc++;
                    byte index = ReadByte();
                    target = $"&local[{index}]";
                    return true;
                }

                case 0x07:
                {
                    if (_pc + 1 >= _code.Length) return false;
                    _pc++;
                    byte index = ReadByte();
                    target = $"buf[0x{index:X2}]";
                    return true;
                }

                case 0x15:
                {
                    if (_pc + 1 >= _code.Length) return false;
                    _pc++;
                    byte index = ReadByte();
                    target = $"ref32[0x{index:X2}]";
                    return true;
                }

                case 0x3F:
                    _pc++;
                    target = "slot_3F";
                    return true;

                default:
                    return false;
            }
        }

        private string VarName(int idx)
        {
            return _globalVarNames.TryGetValue(idx, out var name) ? $"gv[0x{idx:X2}:{name}]" : $"gv[0x{idx:X2}]";
        }

        private static string EscapeAscii(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }

        private bool TryReadAsciiStringAt(int address, out string value)
        {
            value = null;
            if (address < 0 || address >= _code.Length)
            {
                return false;
            }

            int end = address;
            while (end < _code.Length && _code[end] != 0)
            {
                byte b = _code[end];
                if (b < 0x20 || b > 0x7E)
                {
                    return false;
                }

                end++;
            }

            if (end <= address || end >= _code.Length)
            {
                return false;
            }

            string candidate = Encoding.ASCII.GetString(_code, address, end - address);
            if (candidate.Length < 2)
            {
                return false;
            }

            value = candidate;
            return true;
        }

        private bool IsProbableAsciiDataStart(int address)
        {
            if (!TryReadAsciiStringAt(address, out string value))
            {
                return false;
            }

            if (value.Length < 8)
            {
                return false;
            }

            return value.Any(char.IsLetter);
        }

        private string FormatAbsoluteReference(int address)
        {
            if (_functionsByEntryPoint.Count == 0)
            {
                return $"0x{address:X4}";
            }

            (int EntryPoint, string Name)? containingFunction = null;
            foreach (var fn in _functionsByEntryPoint)
            {
                if (fn.EntryPoint > address)
                {
                    break;
                }

                containingFunction = fn;
            }

            if (!containingFunction.HasValue)
            {
                if (TryReadAsciiStringAt(address, out string ascii))
                {
                    return $"0x{address:X4} \"{EscapeAscii(ascii)}\"";
                }

                return $"0x{address:X4}";
            }

            int delta = address - containingFunction.Value.EntryPoint;
            if (delta == 0)
            {
                return $"0x{address:X4} ({containingFunction.Value.Name})";
            }

            return $"0x{address:X4} ({containingFunction.Value.Name}+0x{delta:X})";
        }

        /// <summary>
        /// Disassemble a function starting at the given entry point.
        /// The first 2 bytes at EP are the stack allocation size.
        /// </summary>
        public string DisassembleFunction(int entryPoint, int maxBytes = 2048)
        {
            _output.Clear();
            _pc = entryPoint;

            int stackAlloc = ReadInt16();
            _output.AppendLine($"; Function @ 0x{entryPoint:X4}, stack_alloc = {stackAlloc}");
            _output.AppendLine();

            int endAddr = Math.Min(entryPoint + maxBytes, _code.Length);
            int codeStart = _pc;
            var visitedInstructions = new HashSet<int>();
            var pendingBlocks = new SortedSet<int>();
            pendingBlocks.Add(codeStart);
            bool firstBlock = true;

            while (pendingBlocks.Count > 0)
            {
                int blockStart = pendingBlocks.Min;
                pendingBlocks.Remove(blockStart);
                if (blockStart < codeStart || blockStart >= endAddr || visitedInstructions.Contains(blockStart))
                {
                    continue;
                }

                if (!firstBlock)
                {
                    _output.AppendLine($"; Block @ 0x{blockStart:X4}");
                }

                firstBlock = false;
                _pc = blockStart;

                while (_pc < endAddr)
                {
                    if (IsProbableAsciiDataStart(_pc))
                    {
                        break;
                    }

                    int instrAddr = _pc;
                    if (!visitedInstructions.Add(instrAddr))
                    {
                        break;
                    }

                    try
                    {
                        string disasm = DecodeInstruction();
                        if (disasm == null) break; // BB = RETURN

                        // Format: address: hex bytes  mnemonic
                        int instrLen = _pc - instrAddr;
                        string hexBytes = string.Join("", _code.AsSpan(instrAddr, instrLen).ToArray().Select(b => b.ToString("X2")));
                        if (hexBytes.Length > 20) hexBytes = hexBytes.Substring(0, 20) + "..";

                        _output.AppendLine($"  {instrAddr:X4}: {hexBytes,-24} {disasm}");

                        switch (_lastFlowControl)
                        {
                            case FlowControlKind.UnconditionalBranch:
                                if (_lastFlowTarget.HasValue && _lastFlowTarget.Value >= codeStart && _lastFlowTarget.Value < endAddr)
                                {
                                    pendingBlocks.Add(_lastFlowTarget.Value);
                                }
                                goto EndBlock;

                            case FlowControlKind.ConditionalBranch:
                                if (_lastFlowTarget.HasValue && _lastFlowTarget.Value >= codeStart && _lastFlowTarget.Value < endAddr)
                                {
                                    pendingBlocks.Add(_lastFlowTarget.Value);
                                }
                                if (_pc < endAddr)
                                {
                                    pendingBlocks.Add(_pc);
                                }
                                goto EndBlock;

                            case FlowControlKind.Return:
                                goto EndBlock;
                        }
                    }
                    catch (Exception ex)
                    {
                        _output.AppendLine($"  {instrAddr:X4}: ??? (decode error: {ex.Message})");
                        goto EndBlock;
                    }
                }

            EndBlock:
                _output.AppendLine();
            }

            return _output.ToString();
        }

        /// <summary>
        /// Disassemble from current PC, returning mnemonic string. Advances _pc.
        /// </summary>
        private string DecodeInstruction()
        {
            SetFlowControl(FlowControlKind.None);
            byte op = ReadByte();

            switch (op)
            {
                // === IMMEDIATE PUSH ===
                case 0x41: { byte v = ReadByte(); return $"PUSH_I8    {v} (0x{v:X2})"; }
                case 0x42: { ushort v = ReadUInt16(); return $"PUSH_I16   {v} (0x{v:X4})"; }
                case 0x43: { uint v = ReadUInt32(); return $"PUSH_I32   {v} (0x{v:X8})"; }
                case 0x44: { byte v = ReadByte(); return $"PUSH_STR   \"{ReadStringAt(v)}\""; }

                // === LOCAL VARIABLE ACCESS ===
                case 0x01: { byte v = ReadByte(); return $"STORE_LOC  local[{v}]"; }
                case 0x02: { byte v = ReadByte(); return $"LOAD_LOC   local[{v}]"; }
                case 0x03: { byte v = ReadByte(); return $"LOAD_LOCADDR &local[{v}]"; }
                case 0x04: { byte v = ReadByte(); return $"LOAD_LOCW  local_w[{v}]"; }
                case 0x07: { byte v = ReadByte(); return $"LOAD_BUF   buf[0x{v:X2}] (indexed access)"; }
                case 0x15: { byte v = ReadByte(); return $"LOAD_REF32 ref32[0x{v:X2}]"; }
                case 0x47: return "WRITE_IND  (empirical indirect write)";

                // === PARAMETER ACCESS ===
                case 0x13: { byte v = ReadByte(); return $"LOAD_PARAM param[{v}]"; }

                // === GLOBAL VARIABLE LOAD (by type size) ===
                case 0x19: { byte v = ReadByte(); return $"LOAD_GV8   {VarName(v)}"; }
                case 0x1A: { byte v = ReadByte(); return $"LOAD_GV16  {VarName(v)}"; }
                case 0x1B: { byte v = ReadByte(); return $"LOAD_GV32  {VarName(v)}"; }
                case 0x1C: { byte v = ReadByte(); return $"LOAD_GVPTR {VarName(v)}"; }

                case 0x1F: { byte hi = ReadByte(); byte lo = ReadByte(); return $"LOAD_GVA   {VarName(hi)}[{lo}]"; }

                // === GLOBAL VARIABLE STORE (part of FE group, handled below) ===

                // === STACK OPERATIONS ===
                case 0x11: { byte v = ReadByte(); return $"PUSH_ZERO  (type={v})"; }
                case 0x22: { byte v = ReadByte(); return $"STORE_IDX  [{v}]"; }

                // === CONTROL FLOW ===
                case 0xA3:
                {
                    byte sub = ReadByte();
                    switch (sub)
                    {
                        case 0x0D: { byte len = ReadByte(); return $"MEMCPY_B   len={len}"; }
                        case 0x22: { byte idx = ReadByte(); return $"MEMCPY_IDX [{idx}]"; }
                        default: return $"A3_OP      sub=0x{sub:X2}";
                    }
                }
                case 0xA4:
                {
                    byte off = ReadByte();
                    int target = _pc + off - 1;
                    SetFlowControl(FlowControlKind.UnconditionalBranch, target);
                    return $"JMP_FWD_U  +{off} (-> 0x{target:X4})";
                }
                case 0xA6:
                {
                    byte off = ReadByte();
                    int target = _pc - off - 1;
                    SetFlowControl(FlowControlKind.UnconditionalBranch, target);
                    return $"JMP_BACK_U -{off} (-> 0x{target:X4})";
                }
                case 0xA8: { byte off = ReadByte(); return $"SKIP_FWD   +{off}"; }
                case 0xA9: { byte off = ReadByte(); return $"SKIP_FWD2  +{off}"; }
                case 0xAA:
                {
                    byte off = ReadByte();
                    int target = _pc + off - 1;
                    SetFlowControl(FlowControlKind.ConditionalBranch, target);
                    return $"JMP_FWD    +{off} (-> 0x{target:X4})";
                }
                case 0xAB:
                {
                    byte off = ReadByte();
                    int target = _pc - off - 1;
                    SetFlowControl(FlowControlKind.ConditionalBranch, target);
                    return $"JMP_BACK   -{off} (-> 0x{target:X4})";
                }
                case 0xAC:
                {
                    byte off = ReadByte();
                    int target = _pc + off - 1;
                    SetFlowControl(FlowControlKind.ConditionalBranch, target);
                    return $"JZ_FWD     +{off} (-> 0x{target:X4})";
                }
                case 0xAD:
                {
                    byte off = ReadByte();
                    int target = _pc + off - 1;
                    SetFlowControl(FlowControlKind.ConditionalBranch, target);
                    return $"JNZ_FWD    +{off} (-> 0x{target:X4})";
                }

                // === FUNCTION CALLS ===
                case 0xB5: return "CALL_SYS   (system callback)";
                case 0xB6: { byte n = ReadByte(); return $"POP_N      {n}"; }
                case 0xB7: return "???_B7";
                case 0xB8: { byte fn = ReadByte(); return $"CALL_FN    ordinal={fn}"; }
                case 0xB9: { byte a = ReadByte(); byte b = ReadByte(); return $"CALL_EXT   ({a},{b})"; }
                case 0xBA: return "CALL_DONE";
                case 0xBB:
                    SetFlowControl(FlowControlKind.Return);
                    return "RETURN";

                // === ARITHMETIC / LOGIC (single byte) ===
                case 0xB3: return "NOT        (logical negate TOS)";
                case 0x79:
                {
                    if (TryReadStorageOperand(out string target))
                    {
                        return $"STORE_RES  {target}";
                    }

                    return "STORE_RES  (store result)";
                }
                case 0x7A: return "INC        (increment)";
                case 0x75: { byte v = ReadByte(); return $"CONST_ADD  +{v}"; }
                case 0x6C: return "STRLEN     (string length)";

                // === ARRAY/MEMORY ACCESS ===
                case 0x83:
                {
                    byte sub = ReadByte();
                    switch (sub)
                    {
                        case 0x01: { byte idx = ReadByte(); return $"ARR_LOAD8  local[{idx}]"; }
                        case 0x03: { byte idx = ReadByte(); return $"ARR_LOAD_A local[{idx}]"; }
                        case 0x07: { byte idx = ReadByte(); return $"ARR_LOAD7  [{idx}]"; }
                        case 0x0D: { byte idx = ReadByte(); return $"ARR_STORE  [{idx}]"; }
                        case 0x1F:
                        {
                            byte hi = ReadByte();
                            byte lo = ReadByte();
                            return $"GVARR_W    {VarName(hi)}[{lo}]";
                        }
                        case 0x22:
                        {
                            byte hi = ReadByte();
                            byte lo = ReadByte();
                            return $"GVARR_R    {VarName(hi)}[{lo}]";
                        }
                        default: return $"MEM_OP     sub=0x{sub:X2}";
                    }
                }

                case 0x8A: return "DUP        (duplicate TOS)";
                case 0x9B: return "BIT_OR2    (empirical merge/or)";
                case 0x9F: return "BIT_XOR2   (empirical xor/feedback)";
                case 0xBD: return "READ_IND   (empirical indirect read)";

                // === EXTENDED OPCODE GROUP FE xx ===
                case 0xFE:
                {
                    byte sub = ReadByte();
                    switch (sub)
                    {
                        case 0x4E: return "BIT_AND_R  (empirical mask/and)";
                        case 0x51: return "AND_REF8   (empirical deref-and with byte mask)";
                        case 0x56: return "AND_REF32  (empirical deref-and with dword mask)";
                        case 0x66: return "OR_REF8    (empirical deref-or with byte mask)";
                        case 0x67: return "ADD";
                        case 0x68: return "SUB";
                        case 0x69: return "MUL";
                        case 0x6A: return "DIV";
                        case 0x6B: return "OR_REF32   (empirical deref-or with dword mask)";
                        case 0x80: return "AND";
                        case 0x81: return "OR";
                        case 0x82: return "XOR";
                        case 0x83: return "SHL";
                        case 0x84: return "SHR";
                        case 0x90: return "CMP_EQ";
                        case 0x91: return "CMP_NE";
                        case 0x92: return "CMP_LT";
                        case 0x93: return "CMP_GT";
                        case 0x94: return "CMP_LE";
                        case 0x95:
                        {
                            if (TryReadStorageOperand(out string target))
                            {
                                return $"STORE_REF  {target}";
                            }

                            return "STORE_REF  (empirical store via typed address)";
                        }
                        case 0x96:
                        {
                            if (TryReadStorageOperand(out string target))
                            {
                                return $"STORE_REF32 {target}";
                            }

                            return "STORE_REF32 (empirical dword store via address)";
                        }
                        case 0xD1: return "TEST_ZERO  (TOS == 0?)";
                        case 0xD4: return "TEST_NEG   (TOS < 0?)";
                        case 0xD5: return "TEST_POS   (TOS > 0?)";
                        case 0xE4: return "DEREF      (pointer deref)";
                        case 0xE6: return "DISCARD    (pop and discard)";
                        case 0xEC: return "SWAP       (swap TOS)";
                        case 0xED: return "CMP_GE";
                        case 0xEF: return "BIT_OR     (bitwise OR)";
                        default: return $"FE_{sub:X2}";
                    }
                }

                // === SYSTEM OPCODE GROUP FF xx ===
                case 0xFF:
                {
                    byte sub = ReadByte();
                    switch (sub)
                    {
                        case 0x0E:
                        {
                            ushort addr = ReadUInt16();
                            return $"REF_ABS    {FormatAbsoluteReference(addr)}";
                        }
                        case 0x15: return "BIT_SHL    (bitwise shift left)";
                        case 0x16: return "ADD        (empirical integer add)";
                        case 0x17: return "BIT_AND    (bitwise AND)";
                        case 0x2B: return "ARR_SET    (array element store)";
                        case 0x7F: return "SHL        (empirical shift left)";
                        case 0x94: return "SHR        (empirical shift right)";
                        case 0xA8: return "JZ_REL     (jump if zero)";
                        case 0xA9: return "CMP_NE2    (empirical binary compare not-equal)";
                        case 0xAA: return "CMP_EQ2    (empirical binary compare equal)";
                        case 0xCF: return "LOOP_END   (decrement & loop)";
                        case 0xD4: return "CMP_GE2    (empirical binary compare greater/equal)";
                        case 0xFD: return "CMP_STRICT (strict compare)";
                        default: return $"FF_{sub:X2}";
                    }
                }

                // === SYSTEM CALLBACK GROUP FD xx ===
                case 0xFD:
                {
                    byte sub = ReadByte();
                    switch (sub)
                    {
                        case 0x53: return "SYS_ALLOC  (allocate buffer)";
                        case 0x5E: return "SYS_SEND   (send diagnostic request)";
                        case 0x5F: return "SYS_FMT    (empirical format/extract helper)";
                        case 0xD5: return "SYS_RECV   (receive response)";
                        case 0xDC: return "SYS_ERROR  (error handler)";
                        default: return $"FD_{sub:X2}";
                    }
                }

                // === UNKNOWN/MISC ===
                default:
                    return $"UNK_{op:X2}";
            }
        }

        private string ReadStringAt(int offset)
        {
            if (offset >= _code.Length) return "???";
            int end = offset;
            while (end < _code.Length && _code[end] != 0) end++;
            return Encoding.ASCII.GetString(_code, offset, end - offset);
        }

        /// <summary>
        /// Disassemble all functions in a DSC blob, using the header to find entry points.
        /// </summary>
        public static string DisassembleBlob(byte[] dscBlob, Action<string> log = null)
        {
            var output = new StringBuilder();
            log = log ?? ((msg) => { });

            const int fnTableEntrySize = 50;
            using (var reader = new BinaryReader(new MemoryStream(dscBlob, 0, dscBlob.Length, true, true)))
            {
                reader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                int fnTableOffset = reader.ReadInt32();
                int numberOfFunctions = reader.ReadInt16();
                int dscOffsetA = reader.ReadInt32();
                int caesarHash = reader.ReadInt16();

                reader.BaseStream.Seek(0x1E, SeekOrigin.Begin);
                int globalVarAllocSize = reader.ReadInt16();

                reader.BaseStream.Seek(0x22, SeekOrigin.Begin);
                int globalVariablesBufferPtr = reader.ReadInt32();
                int globalVariablesCount = reader.ReadInt16();

                // Build global variable name map
                var varNames = new Dictionary<int, string>();
                for (int gvIndex = 0; gvIndex < globalVariablesCount; gvIndex++)
                {
                    reader.BaseStream.Seek(globalVariablesBufferPtr + (gvIndex * 12), SeekOrigin.Begin);
                    int varNameOff = reader.ReadInt32();
                    reader.ReadInt16(); // baseType
                    reader.ReadInt16(); // derivedType
                    reader.ReadInt16(); // arraySize
                    int posInBuffer = reader.ReadInt16();

                    reader.BaseStream.Seek(varNameOff, SeekOrigin.Begin);
                    string name = CaesarReader.ReadStringFromBinaryReader(reader, CaesarReader.DefaultEncoding);

                    // Map position to name (positions are byte offsets, but global var index is what we use)
                    varNames[gvIndex] = name;
                }

                var functions = new List<(int EntryPoint, string Name, int Id)>();

                for (int fnIndex = 0; fnIndex < numberOfFunctions; fnIndex++)
                {
                    long fnBaseAddress = fnTableEntrySize * fnIndex + fnTableOffset;
                    reader.BaseStream.Seek(fnBaseAddress, SeekOrigin.Begin);

                    int fnIdentifier = reader.ReadInt16();
                    int fnNameOffset = reader.ReadInt32();
                    int fnEntryPoint = reader.ReadInt32();

                    reader.BaseStream.Seek(fnNameOffset, SeekOrigin.Begin);
                    string fnName = CaesarReader.ReadStringFromBinaryReader(reader);
                    functions.Add((fnEntryPoint, fnName, fnIdentifier));
                }

                var disasm = new DSCDisassembler(
                    dscBlob,
                    varNames,
                    functions.Select(x => (x.EntryPoint, x.Name)));

                // Disassemble each function
                foreach (var fn in functions)
                {
                    // Skip functions with invalid EPs
                    if (fn.EntryPoint < 0 || fn.EntryPoint > dscBlob.Length - 2) continue;

                    output.AppendLine($"===== {fn.Name} (ordinal={fn.Id}, EP=0x{fn.EntryPoint:X}) =====");
                    try
                    {
                        string result = disasm.DisassembleFunction(fn.EntryPoint);
                        output.AppendLine(result);
                    }
                    catch (Exception ex)
                    {
                        output.AppendLine($"  ERROR: {ex.Message}");
                    }
                    output.AppendLine();
                }
            }

            return output.ToString();
        }
    }
}
