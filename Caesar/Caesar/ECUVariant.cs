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
        public int DiagServiceCode_Count; // D : DSC
        public int DiagServiceCode_Offset;
        public int DiagServicesCount; // E
        public int DiagServicesOffset;
        public int DTC_Count; // F
        public int DTC_Offset;
        public int EnvironmentCtx_Count; // G
        public int EnvironmentCtx_Offset;
        public int Xref_Count; // H
        public int Xref_Offset;
        public int VCDomainsCount; // I
        public int VCDomainsOffset;

        public string NegativeResponseName;
        public int UnkByte;

        public List<int> VCDomainPoolOffsets = new List<int>();
        public List<int> DiagServicesPoolOffsets = new List<int>();
        public List<Tuple<int, int, int>> DTCsPoolOffsetsWithBounds = new List<Tuple<int, int, int>>();
        public List<int> EnvironmentContextsPoolOffsets = new List<int>();

        public List<VCDomain> VCDomains = new List<VCDomain>();
        public List<ECUVariantPattern> VariantPatterns = new List<ECUVariantPattern>();
        public DiagService[] DiagServices = new DiagService[] { };
        public DTC[] DTCs = new DTC[] { };
        public DiagService[] EnvironmentContexts = new DiagService[] { };
        //public EnvironmentContext[] EnvironmentContexts = new EnvironmentContext[] { };
        public int[] Xrefs = new int[] { };

        public long BaseAddress;
        public ECU ParentECU;

        public ECUVariant(BinaryReader reader, ECU parentEcu, CTFLanguage language, long baseAddress, int blockSize)
        {
            // int __usercall DIIFindVariantByECUID@<eax>(ECU_VARIANT *a1@<ebx>, _DWORD *a2, int a3, __int16 a4, int a5)

            BaseAddress = baseAddress;
            ParentECU = parentEcu;
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
                DiagServiceCode_Count = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 8 
                DiagServiceCode_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 9 
                DiagServicesCount = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 10 
                DiagServicesOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 11 
                DTC_Count = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 12 
                DTC_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 13 
                EnvironmentCtx_Count = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 14
                EnvironmentCtx_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 15 
                Xref_Count = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 16
                Xref_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 17 

                VCDomainsCount = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 18 
                VCDomainsOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 19 

                NegativeResponseName = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                UnkByte = CaesarReader.ReadBitflagInt8(ref bitFlags, variantReader);  // 20 byte

                // vcdomain
                VCDomainPoolOffsets = new List<int>();
                variantReader.BaseStream.Seek(VCDomainsOffset, SeekOrigin.Begin);
                for (int variantCodingIndex = 0; variantCodingIndex < VCDomainsCount; variantCodingIndex++)
                {
                    VCDomainPoolOffsets.Add(variantReader.ReadInt32());
                }
                // diagnostic services
                DiagServicesPoolOffsets = new List<int>();
                variantReader.BaseStream.Seek(DiagServicesOffset, SeekOrigin.Begin);
                for (int diagIndex = 0; diagIndex < DiagServicesCount; diagIndex++)
                {
                    DiagServicesPoolOffsets.Add(variantReader.ReadInt32());
                }
                // DTCs
                //DTCsPoolOffsets = new List<int>();
                DTCsPoolOffsetsWithBounds = new List<Tuple<int, int, int>>();
                variantReader.BaseStream.Seek(DTC_Offset, SeekOrigin.Begin);
                for (int dtcIndex = 0; dtcIndex < DTC_Count; dtcIndex++)
                {
                    int actualIndex = variantReader.ReadInt32();
                    int xrefStart = variantReader.ReadInt32(); 
                    int xrefCount = variantReader.ReadInt32(); // stitch with table H : int __cdecl DIECUGetNumberOfEnvForAllErrors(DI_ECUINFO *ecuh, int a2, int a3)
                    //DTCsPoolOffsets.Add(actualIndex); // todo: depreciate this
                    DTCsPoolOffsetsWithBounds.Add(new Tuple<int, int, int>(actualIndex, xrefStart, xrefCount));
                }
                // EnvCtxs
                EnvironmentContextsPoolOffsets = new List<int>();
                variantReader.BaseStream.Seek(EnvironmentCtx_Offset, SeekOrigin.Begin);
                for (int envIndex = 0; envIndex < EnvironmentCtx_Count; envIndex++)
                {
                    EnvironmentContextsPoolOffsets.Add(variantReader.ReadInt32());
                }
            }

            CreateVCDomains(reader, parentEcu, language);
            CreateDiagServices(reader, parentEcu, language);
            CreateVariantPatterns(reader);
            CreateComParameters(reader, parentEcu);
            CreateDTCs(reader, parentEcu, language);
            CreateEnvironmentContexts(reader, parentEcu, language);
            CreateXrefs(reader, parentEcu, language);
            //PrintDebug();
        }

        // this function is parked here since the values are drawn from EnvironmentContexts // Xref_Count and Xref_Offset;
        public List<DiagService> GetEnvironmentContextsForDTC(DTC inDtc)
        {
            List<DiagService> ctxList = new List<DiagService>();

            for (int i = inDtc.XrefStart; i < (inDtc.XrefStart + inDtc.XrefCount); i++) 
            {
                foreach (DiagService envToTest in EnvironmentContexts) 
                {
                    int xref = Xrefs[i];
                    if (envToTest.PoolIndex == xref)
                    {
                        ctxList.Add(envToTest);
                        break;
                    }
                }
            }
            return ctxList;
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
        private void CreateDTCs(BinaryReader reader, ECU parentEcu, CTFLanguage language)
        {
            DTCs = new DTC[DTCsPoolOffsetsWithBounds.Count];

            foreach (DTC dtc in parentEcu.GlobalDTCs)
            {
                for (int i = 0; i < DTCsPoolOffsetsWithBounds.Count; i++)
                {
                    if (dtc.PoolIndex == DTCsPoolOffsetsWithBounds[i].Item1)
                    {
                        // this is only valid on the assumption that DTC instances are unique (e.g. not shared from a base variant)
                        dtc.XrefStart = DTCsPoolOffsetsWithBounds[i].Item2;
                        dtc.XrefCount = DTCsPoolOffsetsWithBounds[i].Item3;
                        DTCs[i] = dtc;
                    }
                }
            }
        }
        private void CreateXrefs(BinaryReader reader, ECU parentEcu, CTFLanguage language)
        {
            Xrefs = new int[Xref_Count];
            reader.BaseStream.Seek(BaseAddress + Xref_Offset, SeekOrigin.Begin);
            for (int i = 0; i < Xref_Count; i++)
            {
                Xrefs[i] = reader.ReadInt32();
            }
        }
        private void CreateEnvironmentContexts(BinaryReader reader, ECU parentEcu, CTFLanguage language)
        {
            /*
            EnvironmentContexts = new EnvironmentContext[EnvironmentContextsPoolOffsets.Count];

            foreach (EnvironmentContext env in parentEcu.GlobalEnvironmentContexts)
            {
                for (int i = 0; i < EnvironmentContextsPoolOffsets.Count; i++)
                {
                    if (env.PoolIndex == EnvironmentContextsPoolOffsets[i])
                    {
                        EnvironmentContexts[i] = env;
                    }
                }
            }
            */
            EnvironmentContexts = new DiagService[EnvironmentContextsPoolOffsets.Count];

            foreach (DiagService env in parentEcu.GlobalEnvironmentContexts)
            {
                for (int i = 0; i < EnvironmentContextsPoolOffsets.Count; i++)
                {
                    if (env.PoolIndex == EnvironmentContextsPoolOffsets[i])
                    {
                        EnvironmentContexts[i] = env;
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
            Console.WriteLine($"{nameof(DiagServiceCode_Count)} : {DiagServiceCode_Count}");
            Console.WriteLine($"{nameof(DiagServiceCode_Offset)} : {DiagServiceCode_Offset}");
            Console.WriteLine($"{nameof(DiagServicesCount)} : {DiagServicesCount}");
            Console.WriteLine($"{nameof(DiagServicesOffset)} : {DiagServicesOffset}");
            Console.WriteLine($"{nameof(DTC_Count)} : {DTC_Count}");
            Console.WriteLine($"{nameof(DTC_Offset)} : {DTC_Offset}");
            Console.WriteLine($"{nameof(EnvironmentCtx_Count)} : {EnvironmentCtx_Count}");
            Console.WriteLine($"{nameof(EnvironmentCtx_Offset)} : {EnvironmentCtx_Offset}");
            Console.WriteLine($"{nameof(Xref_Count)} : {Xref_Count}");
            Console.WriteLine($"{nameof(Xref_Offset)} : {Xref_Offset}");
            Console.WriteLine($"{nameof(VCDomainsCount)} : {VCDomainsCount}");
            Console.WriteLine($"{nameof(VCDomainsOffset)} : {VCDomainsOffset}");

        }
    }
}
