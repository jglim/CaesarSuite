using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Caesar
{
    public class CaesarContainer
    {
        public CFFHeader CaesarCFFHeader;
        public CTFHeader CaesarCTFHeader;
        public List<ECU> CaesarECUs = new List<ECU>();
        [System.Text.Json.Serialization.JsonIgnore]
        public byte[] FileBytes = new byte[] { };

        public uint FileChecksum;

        public CaesarContainer() { }

        // fixup serialization/deserialization:
        // language strings should be properties; resolve to actual string only when called
        public CaesarContainer(byte[] fileBytes)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            FileBytes = fileBytes;
            // work from int __cdecl DIIAddCBFFile(char *fileName)
            using (BinaryReader reader = new BinaryReader(new MemoryStream(fileBytes, 0, fileBytes.Length, false, true)))
            {
                byte[] header = reader.ReadBytes(StubHeader.StubHeaderSize);
                StubHeader.ReadHeader(header);

                int cffHeaderSize = reader.ReadInt32();
                byte[] cffHeaderData = reader.ReadBytes(cffHeaderSize);

                // expensive, probably an impediment for modders
                // VerifyChecksum(fileBytes, out uint checksum);
                FileChecksum = ReadFileChecksum(fileBytes);

                ReadCFFDefinition(reader);
                // language is the highest priority since all our strings come from it
                ReadCTF(reader);
                ReadECU(reader);
            }

            sw.Stop();
#if DEBUG
            Console.WriteLine($"Loaded {CaesarECUs[0].Qualifier} in {sw.ElapsedMilliseconds}ms");
#endif
        }

        public static bool VerifyChecksum(byte[] fileBytes, out uint checksum) 
        {
            uint computedChecksum = CaesarReader.ComputeFileChecksumLazy(fileBytes);
            uint providedChecksum = ReadFileChecksum(fileBytes);
            checksum = providedChecksum;
            if (computedChecksum != providedChecksum)
            {
                Console.WriteLine($"WARNING: Checksum mismatch : computed/provided: {computedChecksum:X8}/{providedChecksum:X8}");
                return false;
            }
            return true;
        }

        public static uint ReadFileChecksum(byte[] fileBytes) 
        {
            return BitConverter.ToUInt32(fileBytes, fileBytes.Length - 4);
        }

        public ECUVariant GetECUVariantByName(string name)
        {
            foreach (ECU ecu in CaesarECUs)
            {
                foreach (ECUVariant variant in ecu.ECUVariants)
                {
                    if (variant.Qualifier == name)
                    {
                        return variant;
                    }
                }
            }
            return null;
        }
        public ECU GetECUByName(string name)
        {
            foreach (ECU ecu in CaesarECUs)
            {
                if (ecu.Qualifier == name)
                {
                    return ecu;
                }
            }
            return null;
        }

        public string[] GetECUVariantNames() 
        {
            List<string> result = new List<string>();

            foreach (ECU ecu in CaesarECUs)
            {
                foreach (ECUVariant variant in ecu.ECUVariants)
                {
                    result.Add(variant.Qualifier);
                }
            }
            return result.ToArray();
        }

        public CTFLanguage GetLanguage() 
        {
            if (CaesarCTFHeader.CtfLanguages is null)
            {
                throw new NotImplementedException("stringtable not initialized");
            }

            if (CaesarCTFHeader.CtfLanguages.Count != 0)
            {
                return CaesarCTFHeader.CtfLanguages[0];
            }
            throw new NotImplementedException("no idea how to handle missing stringtable");
        }

        void ReadECU(BinaryReader fileReader) 
        {
            CaesarECUs = new List<ECU>();
            // read all ecu definitions
            long ecuTableOffset = CaesarCFFHeader.EcuOffset + CaesarCFFHeader.BaseAddress;
            
            for (int ecuIndex = 0; ecuIndex < CaesarCFFHeader.EcuCount; ecuIndex++)
            {
                // seek to an entry the ecu offsets table
                fileReader.BaseStream.Seek(ecuTableOffset + (ecuIndex * 4), SeekOrigin.Begin);
                // read the offset to the ecu entry, then seek to the actual address
                int offsetToActualEcuEntry = fileReader.ReadInt32();
                CaesarECUs.Add(new ECU(fileReader, GetLanguage(), CaesarCFFHeader, ecuTableOffset + offsetToActualEcuEntry, this));
            }
        }

        void ReadCTF(BinaryReader fileReader) 
        {
            // parse CTF language stuff
            // approx 0x1304 / 4 number of strings?
            if (CaesarCFFHeader.CtfOffset == 0)
            {
                throw new NotImplementedException("No idea how to handle nonexistent ctf header");
            }
            long ctfOffset = CaesarCFFHeader.BaseAddress + CaesarCFFHeader.CtfOffset;
            CaesarCTFHeader = new CTFHeader(fileReader, ctfOffset, CaesarCFFHeader.CffHeaderSize);
        }


        void ReadCFFDefinition(BinaryReader fileReader)
        {
            CaesarCFFHeader = new CFFHeader(fileReader);
            // CaesarCFFHeader.PrintDebug();

            if (CaesarCFFHeader.CaesarVersion < 400) 
            {
                throw new NotImplementedException($"Unhandled Caesar version: {CaesarCFFHeader.CaesarVersion}");
            }

            int caesarStringTableOffset = CaesarCFFHeader.CffHeaderSize + 0x410 + 4;
            int formEntryTable = caesarStringTableOffset + CaesarCFFHeader.StringPoolSize;
                
            // fixme: relook at this, might be the correct way to load fm?

            //Console.WriteLine($"{nameof(caesarStringTableOffset)} : 0x{caesarStringTableOffset:X}");
            //Console.WriteLine($"{nameof(afterStringTableOffset)} : 0x{afterStringTableOffset:X}");

            /*
            if (CaesarCFFHeader.FormEntries > 0)
            {
                int formOffsetTable = CaesarCFFHeader.unk2RelativeOffset + formEntryTable;
                int formOffsetTableSize = CaesarCFFHeader.FormEntrySize * CaesarCFFHeader.FormEntries;
                Console.WriteLine($"after string table block (*.fm) is present: {nameof(formEntryTable)} : 0x{formEntryTable:X}\n\n");
                Console.WriteLine($"{nameof(formOffsetTable)} : 0x{formOffsetTable:X}\n\n");
                Console.WriteLine($"{nameof(formOffsetTableSize)} : 0x{formOffsetTableSize:X}\n\n");
            }
            */
        }
    }
}
