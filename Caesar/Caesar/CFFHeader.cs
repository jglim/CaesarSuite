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
        public int CaesarVersion;
        public int GpdVersion;
        public int EcuCount;
        public int EcuOffset;
        public int CtfOffset; // nCtfHeaderRpos
        public int StringPoolSize;
        private int DscOffset;
        private int DscCount;
        private int DscEntrySize;
        public string CbfVersionString;
        public string GpdVersionString;
        public string XmlString;

        [System.Text.Json.Serialization.JsonIgnore]
        public int CffHeaderSize;
        [System.Text.Json.Serialization.JsonIgnore]
        public long BaseAddress;

        public long DscBlockOffset;
        private int DscBlockSize;

        [System.Text.Json.Serialization.JsonIgnore]
        public byte[] DSCPool = new byte[] { };

        // DIIAddCBFFile

        public CFFHeader() 
        { }

        public CFFHeader(BinaryReader reader) 
        {
            reader.BaseStream.Seek(StubHeader.StubHeaderSize, SeekOrigin.Begin);
            CffHeaderSize = reader.ReadInt32();

            BaseAddress = reader.BaseStream.Position;

            ulong bitFlags = reader.ReadUInt16();

            CaesarVersion = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            GpdVersion = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            EcuCount = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            EcuOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            CtfOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            StringPoolSize = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            DscOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            DscCount = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            DscEntrySize = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);

            CbfVersionString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            GpdVersionString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            XmlString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);

            
            long dataBufferOffsetAfterStrings = StringPoolSize + CffHeaderSize + 0x414;
            if (DscCount > 0) 
            {
                DscBlockOffset = DscOffset + dataBufferOffsetAfterStrings;
                DscBlockSize = DscEntrySize * DscCount;
                reader.BaseStream.Seek(DscBlockOffset, SeekOrigin.Begin);
                DSCPool = reader.ReadBytes(DscBlockSize);
            }
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"{nameof(CaesarVersion)} : {CaesarVersion}");
            Console.WriteLine($"{nameof(GpdVersion)} : {GpdVersion}");
            Console.WriteLine($"{nameof(EcuCount)} : {EcuCount}");
            Console.WriteLine($"{nameof(EcuOffset)} : {EcuOffset} 0x{EcuOffset:X}");
            Console.WriteLine($"{nameof(CtfOffset)} : 0x{CtfOffset:X}");
            Console.WriteLine($"{nameof(StringPoolSize)} : {StringPoolSize} 0x{StringPoolSize:X}");
            
            Console.WriteLine($"{nameof(DscEntrySize)} : {DscEntrySize}");
            Console.WriteLine($"{nameof(CbfVersionString)} : {CbfVersionString}");
            Console.WriteLine($"{nameof(GpdVersionString)} : {GpdVersionString}");

            Console.WriteLine($"{nameof(DscOffset)} : {DscOffset} 0x{DscOffset:X}");
            Console.WriteLine($"{nameof(DscBlockOffset)} : {DscBlockOffset} 0x{DscBlockOffset:X}");
            Console.WriteLine($"{nameof(DscCount)} : {DscCount}");
            Console.WriteLine($"{nameof(DscBlockSize)} : {DscCount}");
        }
    }
}
