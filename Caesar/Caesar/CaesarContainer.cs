using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class CaesarContainer
    {
        public CFFHeader CaesarCFFHeader;
        public CTFHeader CaesarCTFHeader;
        public List<ECU> CaesarECUs = new List<ECU>();
        public CaesarContainer(byte[] fileBytes)
        {
            // work from int __cdecl DIIAddCBFFile(char *fileName)
            using (BinaryReader reader = new BinaryReader(new MemoryStream(fileBytes)))
            {
                byte[] header = reader.ReadBytes(StubHeader.StubHeaderSize);
                StubHeader.ReadHeader(header);

                int cffHeaderSize = reader.ReadInt32();
                byte[] cffHeaderData = reader.ReadBytes(cffHeaderSize);

                ReadCFFDefinition(reader);
                // language is the highest priority since all our strings come from it
                ReadCTF(reader);
                ReadECU(reader);
            }

        }

        public static string GetCaesarVersionString() 
        {
            return "1.0.0";
        }

        public ECUVariant GetECUVariantByName(string name) 
        {
            foreach (ECU ecu in CaesarECUs) 
            {
                foreach (ECUVariant variant in ecu.ECUVariants) 
                {
                    if (variant.variantName == name) 
                    {
                        return variant;
                    }
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
                    result.Add(variant.variantName);
                }
            }
            return result.ToArray();
        }

        public CTFLanguage GetLanguage() 
        {
            if (CaesarCTFHeader.CTFLanguages is null)
            {
                throw new NotImplementedException("stringtable not initialized");
            }

            if (CaesarCTFHeader.CTFLanguages.Count != 0)
            {
                return CaesarCTFHeader.CTFLanguages[0];
            }
            throw new NotImplementedException("no idea how to handle missing stringtable");
        }

        void ReadECU(BinaryReader fileReader) 
        {
            CaesarECUs = new List<ECU>();
            // read all ecu definitions
            long ecuTableOffset = CaesarCFFHeader.offsetsToEcuOffsets + CaesarCFFHeader.BaseAddress;
            
            for (int ecuIndex = 0; ecuIndex < CaesarCFFHeader.numberOfEcus; ecuIndex++)
            {
                // seek to an entry the ecu offsets table
                fileReader.BaseStream.Seek(ecuTableOffset + (ecuIndex * 4), SeekOrigin.Begin);
                // read the offset to the ecu entry, then seek to the actual address
                int offsetToActualEcuEntry = fileReader.ReadInt32();
                CaesarECUs.Add(new ECU(fileReader, GetLanguage(), CaesarCFFHeader, ecuTableOffset + offsetToActualEcuEntry));
            }

        }

        void ReadCTF(BinaryReader fileReader) 
        {
            // parse CTF language stuff
            // approx 0x1304 / 4 number of strings?
            if (CaesarCFFHeader.nCtfHeaderRpos == 0)
            {
                throw new NotImplementedException("No idea how to handle nonexistent ctf header");
            }
            // Console.WriteLine($"ctf header relative to definitions: {nameof(CaesarCFFHeader.nCtfHeaderRpos)} : 0x{CaesarCFFHeader.nCtfHeaderRpos:X}");

            long ctfOffset = CaesarCFFHeader.BaseAddress + CaesarCFFHeader.nCtfHeaderRpos;
            CaesarCTFHeader = new CTFHeader(fileReader, ctfOffset, CaesarCFFHeader);
        }


        void ReadCFFDefinition(BinaryReader fileReader)
        {

            CaesarCFFHeader = new CFFHeader(fileReader);
            // CaesarCFFHeader.PrintDebug();

            if (CaesarCFFHeader.caesarVersion < 400) 
            {
                throw new NotImplementedException($"Unhandled Caesar version: {CaesarCFFHeader.caesarVersion}");
            }

            int caesarStringTableOffset = CaesarCFFHeader.cffHeaderSize + 0x410 + 4;
            int formEntryTable = caesarStringTableOffset + CaesarCFFHeader.sizeOfStringPool;
                
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
