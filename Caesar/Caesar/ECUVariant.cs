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
        
        private int MatchingPatternCount; // A
        private int MatchingPatternOffset;
        private int SubsectionB_Count; // B
        private int SubsectionB_Offset;
        private int ComParamsCount; // C
        private int ComParamsOffset;
        private int DiagServiceCode_Count; // D : DSC
        private int DiagServiceCode_Offset;
        private int DiagServicesCount; // E
        private int DiagServicesOffset;
        private int DTC_Count; // F
        private int DTC_Offset;
        private int EnvironmentCtx_Count; // G
        private int EnvironmentCtx_Offset;
        private int Xref_Count; // H
        private int Xref_Offset;
        private int VCDomainsCount; // I
        private int VCDomainsOffset;

        public string NegativeResponseName;
        public int UnkByte;

        public List<int> VCDomainPoolOffsets = new List<int>();
        public List<int> DiagServicesPoolOffsets = new List<int>();
        public List<Tuple<int, int, int>> DTCsPoolOffsetsWithBounds = new List<Tuple<int, int, int>>();
        public List<int> EnvironmentContextsPoolOffsets = new List<int>();

        public List<ECUVariantPattern> VariantPatterns = new List<ECUVariantPattern>();
        public int[] Xrefs = new int[] { };

        // these should be manually deserialized by creating references back to the parent ECU

        [System.Text.Json.Serialization.JsonIgnore]
        public List<VCDomain> VCDomains = new List<VCDomain>();
        [System.Text.Json.Serialization.JsonIgnore]
        public DiagService[] DiagServices = new DiagService[] { };
        [System.Text.Json.Serialization.JsonIgnore]
        public DTC[] DTCs = new DTC[] { };
        [System.Text.Json.Serialization.JsonIgnore]
        public DiagService[] EnvironmentContexts = new DiagService[] { };

        public long BaseAddress;
        [System.Text.Json.Serialization.JsonIgnore]
        public ECU ParentECU;

        [System.Text.Json.Serialization.JsonIgnore]
        private CTFLanguage Language;

        public void Restore(CTFLanguage language, ECU parentEcu) 
        {
            Language = language;
            ParentECU = parentEcu;

            CreateVCDomains(parentEcu, language);
            CreateDiagServices(parentEcu, language);
            CreateDTCs(parentEcu, language);
            CreateEnvironmentContexts(parentEcu, language);

            /*
            // no restoring required
            foreach (ECUVariantPattern vp in VariantPatterns) 
            {
                vp.Restore();
            }
            */
            // CreateComParameters(reader, parentEcu); // already serialized in json
        }

        public ECUVariant() { }

        public ECUVariant(BinaryReader reader, ECU parentEcu, CTFLanguage language, long baseAddress, int blockSize)
        {
            // int __usercall DIIFindVariantByECUID@<eax>(ECU_VARIANT *a1@<ebx>, _DWORD *a2, int a3, __int16 a4, int a5)

            BaseAddress = baseAddress;
            ParentECU = parentEcu;
            Language = language;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            byte[] variantBytes = reader.ReadBytes(blockSize);

            using (BinaryReader variantReader = new BinaryReader(new MemoryStream(variantBytes, 0, variantBytes.Length, false, true)))
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

            CreateVCDomains(parentEcu, language);
            CreateDiagServices(parentEcu, language);
            CreateVariantPatterns(reader);
            CreateComParameters(reader, parentEcu);
            CreateDTCs(parentEcu, language);
            CreateEnvironmentContexts(parentEcu, language);
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

        private void CreateComParameters(BinaryReader reader, ECU parentEcu)
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
                ComParameter param = new ComParameter(reader, comparamOffset, parentEcu.ECUInterfaces, Language);
                param.InsertIntoEcu(parentEcu);
            }
        }

        private void CreateVariantPatterns(BinaryReader reader) 
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

        private void CreateVCDomains(ECU parentEcu, CTFLanguage language)
        {
            VCDomains = new List<VCDomain>();
            foreach (int variantCodingDomainEntry in VCDomainPoolOffsets)
            {
                /*
                VCDomain vcDomain = new VCDomain(reader, parentEcu, language, variantCodingDomainEntry);
                VCDomains.Add(vcDomain);
                */
                VCDomains.Add(ParentECU.GlobalVCDs[variantCodingDomainEntry]);
            }
        }
        private void CreateDiagServices(ECU parentEcu, CTFLanguage language)
        {
            // unlike variant domains, storing references to the parent objects in the ecu is preferable since this is relatively larger
            //DiagServices = new List<DiagService>();

            DiagServices = new DiagService[DiagServicesPoolOffsets.Count];

            /*
            // computationally expensive, 40ish % runtime is spent here
            // easier to read, below optimization essentially accomplishes this in a shorter period

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
            */
            // optimization hack
            int poolSize = DiagServicesPoolOffsets.Count;
            for (int i = 0; i < poolSize; i++) 
            {
                if (i == DiagServicesPoolOffsets[i])
                {
                    DiagServices[i] = parentEcu.GlobalDiagServices[i];
                }
            }
            DiagServicesPoolOffsets.Sort();
            int lowestIndex = 0;
            int loopMax = parentEcu.GlobalDiagServices.Count;
            for (int i = 0; i < poolSize; i++)
            {
                if (DiagServices[i] != null) 
                {
                    continue;
                }
                for (int globalIndex = lowestIndex; globalIndex < loopMax; globalIndex++)
                {
                    if (parentEcu.GlobalDiagServices[globalIndex].PoolIndex == DiagServicesPoolOffsets[i])
                    {
                        DiagServices[i] = parentEcu.GlobalDiagServices[globalIndex];
                        lowestIndex = globalIndex;
                        break;
                    }
                }
            }
        }
        private void CreateDTCs(ECU parentEcu, CTFLanguage language)
        {
            int dtcPoolSize = DTCsPoolOffsetsWithBounds.Count;
            DTCs = new DTC[dtcPoolSize];

            for (int i = 0; i < dtcPoolSize; i++) 
            {
                if (i == DTCsPoolOffsetsWithBounds[i].Item1) 
                {
                    DTCs[i] = parentEcu.GlobalDTCs[i];
                    DTCs[i].XrefStart = DTCsPoolOffsetsWithBounds[i].Item2;
                    DTCs[i].XrefCount = DTCsPoolOffsetsWithBounds[i].Item3;
                }
            }
            DTCsPoolOffsetsWithBounds.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            int lowestIndex = 0;
            int loopMax = ParentECU.GlobalDTCs.Count;
            for (int i = 0; i < dtcPoolSize; i++) 
            {
                if (DTCs[i] != null)
                {
                    continue;
                }
                for (int globalIndex = lowestIndex; globalIndex < loopMax; globalIndex++)
                {
                    if (ParentECU.GlobalDTCs[globalIndex].PoolIndex == DTCsPoolOffsetsWithBounds[i].Item1) 
                    {
                        DTCs[i] = parentEcu.GlobalDTCs[globalIndex];
                        DTCs[i].XrefStart = DTCsPoolOffsetsWithBounds[i].Item2;
                        DTCs[i].XrefCount = DTCsPoolOffsetsWithBounds[i].Item3;
                        lowestIndex = globalIndex;
                        break;
                    }
                }
            }

            /*
            // same thing as above, just more readable and slower
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
            */
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
        private void CreateEnvironmentContexts(ECU parentEcu, CTFLanguage language)
        {
            int envPoolSize = EnvironmentContextsPoolOffsets.Count;
            EnvironmentContexts = new DiagService[envPoolSize];

            for (int i = 0; i < envPoolSize; i++)
            {
                if (i == EnvironmentContextsPoolOffsets[i])
                {
                    EnvironmentContexts[i] = parentEcu.GlobalEnvironmentContexts[i];
                }
            }
            EnvironmentContextsPoolOffsets.Sort();
            int lowestIndex = 0;
            int loopMax = parentEcu.GlobalEnvironmentContexts.Count;
            for (int i = 0; i < envPoolSize; i++)
            {
                if (EnvironmentContexts[i] != null)
                {
                    continue;
                }
                for (int globalIndex = lowestIndex; globalIndex < loopMax; globalIndex++)
                {
                    if (parentEcu.GlobalEnvironmentContexts[globalIndex].PoolIndex == EnvironmentContextsPoolOffsets[i])
                    {
                        EnvironmentContexts[i] = parentEcu.GlobalEnvironmentContexts[globalIndex];
                        lowestIndex = globalIndex;
                        break;
                    }
                }
            }
            /*
            // same thing, more readable, much slower
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
            */
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
