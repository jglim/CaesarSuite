using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class FlashSecurity
    {
        public int MethodValueType;
        public int MethodSize;
        public byte[] MethodValue;

        public int SignatureValueType;
        public int SignatureSize;
        public byte[] SignatureValue;

        public int ChecksumValueType;
        public int ChecksumSize;
        public byte[] ChecksumValue;

        public int EcuKeyValueType;
        public int EcuKeySize;
        public byte[] EcuKeyValue;

        public long BaseAddress;

        public FlashSecurity(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitFlags = reader.ReadUInt16();

            MethodValueType = CaesarReader.ReadBitflagInt16(ref bitFlags, reader);
            MethodSize = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            MethodValue = CaesarReader.ReadBitflagDumpWithReader(ref bitFlags, reader, MethodSize, baseAddress);

            SignatureValueType = CaesarReader.ReadBitflagInt16(ref bitFlags, reader);
            SignatureSize = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            SignatureValue = CaesarReader.ReadBitflagDumpWithReader(ref bitFlags, reader, SignatureSize, baseAddress);

            ChecksumValueType = CaesarReader.ReadBitflagInt16(ref bitFlags, reader);
            ChecksumSize = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            ChecksumValue = CaesarReader.ReadBitflagDumpWithReader(ref bitFlags, reader, ChecksumSize, baseAddress);

            EcuKeyValueType = CaesarReader.ReadBitflagInt16(ref bitFlags, reader);
            EcuKeySize = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            EcuKeyValue = CaesarReader.ReadBitflagDumpWithReader(ref bitFlags, reader, EcuKeySize, baseAddress);
        }

        public void PrintDebug()
        {
            Console.WriteLine($"{nameof(MethodValueType)} : 0x{MethodValueType:X}");
            Console.WriteLine($"{nameof(MethodSize)} : 0x{MethodSize:X}");
            Console.WriteLine($"{nameof(MethodValue)} : {BitUtility.BytesToHex(MethodValue)}");

            Console.WriteLine($"{nameof(SignatureValueType)} : 0x{SignatureValueType:X}");
            Console.WriteLine($"{nameof(SignatureSize)} : 0x{SignatureSize:X}");
            Console.WriteLine($"{nameof(SignatureValue)} : {BitUtility.BytesToHex(SignatureValue)}");

            Console.WriteLine($"{nameof(ChecksumValueType)} : 0x{ChecksumValueType:X}");
            Console.WriteLine($"{nameof(ChecksumSize)} : 0x{ChecksumSize:X}");
            Console.WriteLine($"{nameof(ChecksumValue)} : {BitUtility.BytesToHex(ChecksumValue)}");

            Console.WriteLine($"{nameof(EcuKeyValueType)} : 0x{EcuKeyValueType:X}");
            Console.WriteLine($"{nameof(EcuKeySize)} : 0x{EcuKeySize:X}");
            Console.WriteLine($"{nameof(EcuKeyValue)} : {BitUtility.BytesToHex(EcuKeyValue)}");
        }
    }
}
