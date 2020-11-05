using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class CFFHeader
    {
        public int caesarVersion;
        public int gpdVersion;
        public int numberOfEcus;
        public int offsetsToEcuOffsets;
        public int nCtfHeaderRpos;
        public int sizeOfStringPool;
        public int unk2RelativeOffset;
        public int FormEntries;
        public int FormEntrySize;
        public string cbfVersionString;
        public string gpdVersionString;
        public string diogenesXmlString;

        public int cffHeaderSize;
        public long BaseAddress;
        
        public CFFHeader(BinaryReader reader) 
        {
            reader.BaseStream.Seek(StubHeader.StubHeaderSize, SeekOrigin.Begin);
            cffHeaderSize = reader.ReadInt32();

            BaseAddress = reader.BaseStream.Position;

            ulong bitFlags = reader.ReadUInt16();

            caesarVersion = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            gpdVersion = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            numberOfEcus = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            offsetsToEcuOffsets = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            nCtfHeaderRpos = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            sizeOfStringPool = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            unk2RelativeOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            FormEntries = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            FormEntrySize = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);

            cbfVersionString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            gpdVersionString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            diogenesXmlString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"{nameof(caesarVersion)} : {caesarVersion}");
            Console.WriteLine($"{nameof(gpdVersion)} : {gpdVersion}");
            Console.WriteLine($"{nameof(numberOfEcus)} : {numberOfEcus}");
            Console.WriteLine($"{nameof(offsetsToEcuOffsets)} : {offsetsToEcuOffsets} 0x{offsetsToEcuOffsets:X}");
            Console.WriteLine($"{nameof(nCtfHeaderRpos)} : 0x{nCtfHeaderRpos:X}");
            Console.WriteLine($"{nameof(sizeOfStringPool)} : {sizeOfStringPool} 0x{sizeOfStringPool:X}");
            Console.WriteLine($"{nameof(unk2RelativeOffset)} : {unk2RelativeOffset} 0x{unk2RelativeOffset:X}");
            Console.WriteLine($"{nameof(FormEntries)} : {FormEntries}");
            Console.WriteLine($"{nameof(FormEntrySize)} : {FormEntrySize}");
            Console.WriteLine($"{nameof(cbfVersionString)} : {cbfVersionString}");
            Console.WriteLine($"{nameof(gpdVersionString)} : {gpdVersionString}");
        }
    }
}
