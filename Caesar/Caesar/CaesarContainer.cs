using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        [Newtonsoft.Json.JsonIgnore]
        public byte[] FileBytes = new byte[] { };

        public uint FileChecksum;

        public CaesarContainer() { }

        // fixup serialization/deserialization:
        // language strings should be properties; resolve to actual string only when called
        public CaesarContainer(byte[] fileBytes)
        {
            FileBytes = fileBytes;
            // work from int __cdecl DIIAddCBFFile(char *fileName)
            using (BinaryReader reader = new BinaryReader(new MemoryStream(fileBytes)))
            {
                byte[] header = reader.ReadBytes(StubHeader.StubHeaderSize);
                StubHeader.ReadHeader(header);

                int cffHeaderSize = reader.ReadInt32();
                byte[] cffHeaderData = reader.ReadBytes(cffHeaderSize);

                VerifyChecksum(fileBytes, out uint checksum);
                FileChecksum = checksum;

                ReadCFFDefinition(reader);
                // language is the highest priority since all our strings come from it
                ReadCTF(reader);
                ReadECU(reader);
            }
        }

        public static string SerializeContainer(CaesarContainer container) 
        {
            return JsonConvert.SerializeObject(container);
        }

        public static CaesarContainer DeserializeContainer(string json) 
        {
            CaesarContainer container = JsonConvert.DeserializeObject<CaesarContainer>(json);
            // at this point, the container needs to restore its internal object references before it is fully usable
            CTFLanguage language = container.CaesarCTFHeader.CtfLanguages[0];
            foreach (ECU ecu in container.CaesarECUs) 
            {
                ecu.Restore(language, container);
            }

            return container;
        }

        public static CaesarContainer DeserializeCompressedContainer(byte[] containerBytes)
        {
            string json = Encoding.UTF8.GetString(Inflate(containerBytes));
            return DeserializeContainer(json);
        }
        public static byte[] SerializeCompressedContainer(CaesarContainer container)
        {
            return Deflate(Encoding.UTF8.GetBytes(SerializeContainer(container)));
        }


        private static byte[] Inflate(byte[] input)
        {
            using (MemoryStream ms = new MemoryStream(input))
            {
                using (MemoryStream msInner = new MemoryStream())
                {
                    using (DeflateStream z = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        z.CopyTo(msInner);
                    }
                    return msInner.ToArray();
                }
            }
        }
        private static byte[] Deflate(byte[] input)
        {
            using (MemoryStream compressedStream = new MemoryStream())
            {
                DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true);
                deflateStream.Write(input, 0, input.Length);
                deflateStream.Close();
                return compressedStream.ToArray();
            }
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

        public static string GetCaesarVersionString()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
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

        public string GetFileSize() 
        {
            return BytesToString(FileBytes.Length);
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = { " B", " KB", " MB", " GB", " TB", " PB", " EB" }; //Longs run out around EB
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 3);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public override bool Equals(object obj)
        {
            var container = obj as CaesarContainer;

            if (container == null) 
            {
                return false;
            }
            return this.FileChecksum == container.FileChecksum;
        }

        public override int GetHashCode()
        {
            return (int)FileChecksum;
        }

    }
}
