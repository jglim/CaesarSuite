using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class ECUVariant
    {
        public string Qualifier;
        public int Name_CTF;
        public int Description_CTF;
        public string UnkStr1;
        public string UnkStr2;
        public int Unk1;
        
        public int MatchingPatternCount; // A
        public int MatchingPatternOffset;
        public int SubsectionB_Count; // B
        public int SubsectionB_Offset;
        public int ComParamsCount; // C
        public int ComParamsOffset;
        public int SubsectionD_Count; // D
        public int SubsectionD_Offset;
        public int DiagServicesCount; // E
        public int DiagServicesOffset;
        public int SubsectionF_Count; // F
        public int SubsectionF_Offset;
        public int SubsectionG_Count; // G
        public int SubsectionG_Offset;
        public int SubsectionH_Count; // H
        public int SubsectionH_Offset;
        public int VCDomainsCount; // I
        public int VCDomainsOffset;

        public string NegativeResponseName;
        public int UnkByte;

        public List<int> VCDomainPoolOffsets = new List<int>();
        public List<int> DiagServicesPoolOffsets = new List<int>();

        public List<VCDomain> VCDomains = new List<VCDomain>();
        public List<ECUVariantPattern> VariantPatterns = new List<ECUVariantPattern>();
        public DiagService[] DiagServices = new DiagService[] { };

        public long BaseAddress;

        public ECUVariant(BinaryReader reader, ECU parentEcu, CTFLanguage language, long baseAddress, int blockSize)
        {
            // int __usercall DIIFindVariantByECUID@<eax>(ECU_VARIANT *a1@<ebx>, _DWORD *a2, int a3, __int16 a4, int a5)

            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            byte[] variantBytes = reader.ReadBytes(blockSize);

            using (BinaryReader variantReader = new BinaryReader(new MemoryStream(variantBytes)))
            {
                ulong bitFlags = variantReader.ReadUInt32();
                int skip = variantReader.ReadInt32();

                Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                Name_CTF = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader, -1);
                Description_CTF = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader, -1);
                UnkStr1 = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                UnkStr2 = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);

                Unk1 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 1 
                MatchingPatternCount = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 2 
                MatchingPatternOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 3 
                SubsectionB_Count = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 4 
                SubsectionB_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 5 
                ComParamsCount = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 6 
                ComParamsOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader); // 7 
                SubsectionD_Count = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 8 
                SubsectionD_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 9 
                DiagServicesCount = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 10 
                DiagServicesOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 11 
                SubsectionF_Count = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 12 
                SubsectionF_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 13 
                SubsectionG_Count = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 14 
                SubsectionG_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 15 
                SubsectionH_Count = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 16 
                SubsectionH_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 17 

                VCDomainsCount = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 18 
                VCDomainsOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 19 

                NegativeResponseName = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                UnkByte = CaesarReader.ReadBitflagInt8(ref bitFlags, variantReader);  // 20 byte

                VCDomainPoolOffsets = new List<int>();
                variantReader.BaseStream.Seek(VCDomainsOffset, SeekOrigin.Begin);
                for (int variantCodingIndex = 0; variantCodingIndex < VCDomainsCount; variantCodingIndex++)
                {
                    VCDomainPoolOffsets.Add(variantReader.ReadInt32());
                }

                DiagServicesPoolOffsets = new List<int>();
                variantReader.BaseStream.Seek(DiagServicesOffset, SeekOrigin.Begin);
                for (int diagIndex = 0; diagIndex < DiagServicesCount; diagIndex++)
                {
                    DiagServicesPoolOffsets.Add(variantReader.ReadInt32());
                }


            }

            CreateVCDomains(reader, parentEcu, language);
            CreateDiagServices(reader, parentEcu, language);
            CreateVariantPatterns(reader);
            CreateComParameters(reader, parentEcu);
        }

        public void CreateComParameters(BinaryReader reader, ECU parentEcu) 
        {
            // this is unusual as it doesn't use the usual caesar-style bitflag reads
            // for reasons unknown the comparam is attached to the basevariant
            long comparamBaseAddress = BaseAddress + ComParamsOffset;
            // Console.WriteLine($"Comparam base: 0x{comparamBaseAddress:X} : number of comparams: {ComParamsCount} ");
            reader.BaseStream.Seek(comparamBaseAddress, SeekOrigin.Begin);
            List<long> comparameterOffsets = new List<long>();
            for (int comIndex = 0; comIndex < ComParamsCount; comIndex++) 
            {
                comparameterOffsets.Add(reader.ReadInt32() + comparamBaseAddress);
            }

            if (parentEcu.ECUInterfaces.Count == 0)
            {
                throw new Exception("Invalid communication parameter : no parent interface");
            }

            foreach (long comparamOffset in comparameterOffsets) 
            {
                ComParameter param = new ComParameter(reader, comparamOffset, parentEcu.ECUInterfaces);

                // KW2C3PE uses a different parent addressing style
                int parentIndex = param.ParentInterfaceIndex > 0 ? param.ParentInterfaceIndex : param.SubinterfaceIndex;

                if (param.ParentInterfaceIndex >= parentEcu.ECUInterfaceSubtypes.Count)
                {
                    throw new Exception("ComParam: tried to assign to nonexistent interface");
                }
                else
                {
                    parentEcu.ECUInterfaceSubtypes[parentIndex].CommunicationParameters.Add(param);
                }
            }
        }

        public void CreateVariantPatterns(BinaryReader reader) 
        {
            long tableOffset = BaseAddress + MatchingPatternOffset;
            reader.BaseStream.Seek(tableOffset, SeekOrigin.Begin);

            VariantPatterns.Clear();
            for (int patternIndex = 0; patternIndex < MatchingPatternCount; patternIndex++) 
            {
                reader.BaseStream.Seek(tableOffset + (patternIndex * 4), SeekOrigin.Begin);
                int patternOffset = reader.ReadInt32();
                long patternAddress = patternOffset + tableOffset;

                ECUVariantPattern pattern = new ECUVariantPattern(reader, patternAddress);
                VariantPatterns.Add(pattern);
            }
        }

        public VCDomain GetVCDomainByName(string name)
        {
            foreach (VCDomain domain in VCDomains)
            {
                if (domain.Qualifier == name)
                {
                    return domain;
                }
            }
            return null;
        }
        public DiagService GetDiagServiceByName(string name)
        {
            foreach (DiagService diag in DiagServices)
            {
                if (diag.Qualifier == name)
                {
                    return diag;
                }
            }
            return null;
        }
        public string[] GetVCDomainNames()
        {
            List<string> result = new List<string>();
            foreach (VCDomain domain in VCDomains)
            {
                result.Add(domain.Qualifier);
            }
            return result.ToArray();
        }

        private void CreateVCDomains(BinaryReader reader, ECU parentEcu, CTFLanguage language)
        {
            VCDomains = new List<VCDomain>();
            foreach (int variantCodingDomainEntry in VCDomainPoolOffsets)
            {
                VCDomain vcDomain = new VCDomain(reader, parentEcu, language, variantCodingDomainEntry);
                VCDomains.Add(vcDomain);
            }
        }
        private void CreateDiagServices(BinaryReader reader, ECU parentEcu, CTFLanguage language)
        {
            // unlike variant domains, storing references to the parent objects in the ecu is preferable since this is relatively larger
            //DiagServices = new List<DiagService>();
            
            // computationally expensive, 40ish % runtime is spent here
            DiagServices = new DiagService[DiagServicesPoolOffsets.Count];

            foreach (DiagService diagSvc in parentEcu.GlobalDiagServices)
            {
                for (int i = 0; i < DiagServicesPoolOffsets.Count; i++)
                {
                    if (diagSvc.PoolIndex == DiagServicesPoolOffsets[i])
                    {
                        DiagServices[i] = diagSvc;
                    }
                }
            }
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"---------------- {BaseAddress:X} ----------------");
            Console.WriteLine($"{nameof(Qualifier)} : {Qualifier}");
            Console.WriteLine($"{nameof(Name_CTF)} : {Name_CTF}");
            Console.WriteLine($"{nameof(Description_CTF)} : {Description_CTF}");
            Console.WriteLine($"{nameof(UnkStr1)} : {UnkStr1}");
            Console.WriteLine($"{nameof(UnkStr2)} : {UnkStr2}");
            Console.WriteLine($"{nameof(VCDomainsCount)} : {VCDomainsCount}");
            Console.WriteLine($"{nameof(VCDomainsOffset)} : {VCDomainsOffset}");
            Console.WriteLine($"{nameof(NegativeResponseName)} : {NegativeResponseName}");

            Console.WriteLine($"{nameof(Unk1)} : {Unk1}");
            Console.WriteLine($"{nameof(MatchingPatternCount)} : {MatchingPatternCount}");
            Console.WriteLine($"{nameof(MatchingPatternOffset)} : {MatchingPatternOffset}");
            Console.WriteLine($"{nameof(SubsectionB_Count)} : {SubsectionB_Count}");
            Console.WriteLine($"{nameof(SubsectionB_Offset)} : {SubsectionB_Offset}");
            Console.WriteLine($"{nameof(ComParamsCount)} : {ComParamsCount}");
            Console.WriteLine($"{nameof(ComParamsOffset)} : {ComParamsOffset}");
            Console.WriteLine($"{nameof(SubsectionD_Count)} : {SubsectionD_Count}");
            Console.WriteLine($"{nameof(SubsectionD_Offset)} : {SubsectionD_Offset}");
            Console.WriteLine($"{nameof(DiagServicesCount)} : {DiagServicesCount}");
            Console.WriteLine($"{nameof(DiagServicesOffset)} : {DiagServicesOffset}");
            Console.WriteLine($"{nameof(SubsectionF_Count)} : {SubsectionF_Count}");
            Console.WriteLine($"{nameof(SubsectionF_Offset)} : {SubsectionF_Offset}");
            Console.WriteLine($"{nameof(SubsectionG_Count)} : {SubsectionG_Count}");
            Console.WriteLine($"{nameof(SubsectionG_Offset)} : {SubsectionG_Offset}");
            Console.WriteLine($"{nameof(SubsectionH_Count)} : {SubsectionH_Count}");
            Console.WriteLine($"{nameof(SubsectionH_Offset)} : {SubsectionH_Offset}");
            Console.WriteLine($"{nameof(VCDomainsCount)} : {VCDomainsCount}");
            Console.WriteLine($"{nameof(VCDomainsOffset)} : {VCDomainsOffset}");

        }
    }
}
