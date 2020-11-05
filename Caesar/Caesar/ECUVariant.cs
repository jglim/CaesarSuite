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
        public string variantName;
        public int variantName_T;
        public int variantLongName_T;
        public string variantIdkStr1;
        public string variantIdkStr2;
        public int variantIdk1;
        public int ecuMatchingPatternCount;
        public int ecuMatchingPatternOffset;
        public int variantSubsectionB;
        public int variantSubsectionB_Offset;
        public int comparamsCount_C;
        public int comparamsOffset_C;
        public int variantSubsectionD;
        public int variantSubsectionD_Offset;
        public int variantSubsectionE_DiagSvc;
        public int variantSubsectionE_Offset_DiagSvc;
        public int variantSubsectionF;
        public int variantSubsectionF_Offset;
        public int variantSubsectionG;
        public int variantSubsectionG_Offset;
        public int variantSubsectionH;
        public int variantSubsectionH_Offset;
        public int variantVCodingDomainsCount;
        public int variantVCodingDomainsOffset;
        public string negativeResponseName;
        public int variantIdkByte;
        public List<int> variantCodingDomainOffsets = new List<int>();
        public List<int> diagSvcOffsets = new List<int>();

        public List<VCDomain> VCDomains = new List<VCDomain>();
        public List<DiagService> DiagServices = new List<DiagService>();
        public List<ECUVariantPattern> VariantPatterns = new List<ECUVariantPattern>();

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

                variantName = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                variantName_T = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader, -1);
                variantLongName_T = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader, -1);
                variantIdkStr1 = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                variantIdkStr2 = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);

                variantIdk1 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 1 
                ecuMatchingPatternCount = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 2 
                ecuMatchingPatternOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 3 
                variantSubsectionB = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 4 
                variantSubsectionB_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 5 
                comparamsCount_C = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 6 
                comparamsOffset_C = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader); // 7 
                variantSubsectionD = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 8 
                variantSubsectionD_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 9 
                variantSubsectionE_DiagSvc = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 10 
                variantSubsectionE_Offset_DiagSvc = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 11 
                variantSubsectionF = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 12 
                variantSubsectionF_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 13 
                variantSubsectionG = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 14 
                variantSubsectionG_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 15 
                variantSubsectionH = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 16 
                variantSubsectionH_Offset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 17 

                variantVCodingDomainsCount = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 18 
                variantVCodingDomainsOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 19 

                negativeResponseName = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                variantIdkByte = CaesarReader.ReadBitflagInt8(ref bitFlags, variantReader);  // 20 byte

                variantCodingDomainOffsets = new List<int>();
                variantReader.BaseStream.Seek(variantVCodingDomainsOffset, SeekOrigin.Begin);
                for (int variantCodingIndex = 0; variantCodingIndex < variantVCodingDomainsCount; variantCodingIndex++)
                {
                    variantCodingDomainOffsets.Add(variantReader.ReadInt32());
                }

                diagSvcOffsets = new List<int>();
                variantReader.BaseStream.Seek(variantSubsectionE_Offset_DiagSvc, SeekOrigin.Begin);
                for (int diagIndex = 0; diagIndex < variantSubsectionE_DiagSvc; diagIndex++)
                {
                    diagSvcOffsets.Add(variantReader.ReadInt32());
                }


            }

            //PrintDebug();
            CreateVCDomains(reader, parentEcu, language);
            CreateDiagServices(reader, parentEcu, language);
            CreateVariantPatterns(reader);
            CreateComParameters(reader, parentEcu);
        }

        public void CreateComParameters(BinaryReader reader, ECU parentEcu) 
        {
            // this is unusual as it doesn't use the usual caesar-style bitflag reads
            // for reasons unknown the comparam is attached to the basevariant
            long comparamBaseAddress = BaseAddress + comparamsOffset_C;
            reader.BaseStream.Seek(comparamBaseAddress, SeekOrigin.Begin);
            List<long> comparameterOffsets = new List<long>();
            for (int comIndex = 0; comIndex < comparamsCount_C; comIndex++) 
            {
                comparameterOffsets.Add(reader.ReadInt32() + comparamBaseAddress);
            }


            if (parentEcu.ECUInterfaces.Count == 0)
            {
                throw new Exception("Invalid communication parameter : no parent interface");
            }
            ECUInterface parentEcuInterface = parentEcu.ECUInterfaces[0];


            foreach (long comparamOffset in comparameterOffsets) 
            {
                ComParameter param = new ComParameter(reader, comparamOffset, parentEcuInterface);
                if (param.subinterfaceIndex >= parentEcu.ECUInterfaceSubtypes.Count)
                {
                    throw new Exception("ComParam: tried to assign to nonexistent interface");
                }
                else
                {
                    parentEcu.ECUInterfaceSubtypes[param.subinterfaceIndex].CommunicationParameters.Add(param);
                }
            }
        }

        public void CreateVariantPatterns(BinaryReader reader) 
        {
            long tableOffset = BaseAddress + ecuMatchingPatternOffset;
            reader.BaseStream.Seek(tableOffset, SeekOrigin.Begin);

            VariantPatterns.Clear();
            for (int patternIndex = 0; patternIndex < ecuMatchingPatternCount; patternIndex++) 
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
                if (domain.vcdName == name)
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
                if (diag.qualifierName == name)
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
                result.Add(domain.vcdName);
            }
            return result.ToArray();
        }

        private void CreateVCDomains(BinaryReader reader, ECU parentEcu, CTFLanguage language)
        {
            VCDomains = new List<VCDomain>();
            foreach (int variantCodingDomainEntry in variantCodingDomainOffsets)
            {
                VCDomain vcDomain = new VCDomain(reader, parentEcu, language, variantCodingDomainEntry);
                VCDomains.Add(vcDomain);
            }
        }
        private void CreateDiagServices(BinaryReader reader, ECU parentEcu, CTFLanguage language)
        {
            // unlike variant domains, storing references to the parent objects in the ecu is preferable since this is relatively larger
            DiagServices = new List<DiagService>();
            foreach (int diagIndex in diagSvcOffsets)
            {
                foreach (DiagService diagSvc in parentEcu.GlobalDiagServices)
                {
                    if (diagSvc.PoolIndex == diagIndex) 
                    {
                        DiagServices.Add(diagSvc);
                    }
                }
            }

        }

        public void PrintDebug() 
        {
            Console.WriteLine($"---------------- {BaseAddress:X} ----------------");
            Console.WriteLine($"{nameof(variantName)} : {variantName}");
            Console.WriteLine($"{nameof(variantName_T)} : {variantName_T}");
            Console.WriteLine($"{nameof(variantLongName_T)} : {variantLongName_T}");
            Console.WriteLine($"{nameof(variantIdkStr1)} : {variantIdkStr1}");
            Console.WriteLine($"{nameof(variantIdkStr2)} : {variantIdkStr2}");
            Console.WriteLine($"{nameof(variantVCodingDomainsCount)} : {variantVCodingDomainsCount}");
            Console.WriteLine($"{nameof(variantVCodingDomainsOffset)} : {variantVCodingDomainsOffset}");
            Console.WriteLine($"{nameof(negativeResponseName)} : {negativeResponseName}");

            Console.WriteLine($"{nameof(variantIdk1)} : {variantIdk1}");
            Console.WriteLine($"{nameof(ecuMatchingPatternCount)} : {ecuMatchingPatternCount}");
            Console.WriteLine($"{nameof(ecuMatchingPatternOffset)} : {ecuMatchingPatternOffset}");
            Console.WriteLine($"{nameof(variantSubsectionB)} : {variantSubsectionB}");
            Console.WriteLine($"{nameof(variantSubsectionB_Offset)} : {variantSubsectionB_Offset}");
            Console.WriteLine($"{nameof(comparamsCount_C)} : {comparamsCount_C}");
            Console.WriteLine($"{nameof(comparamsOffset_C)} : {comparamsOffset_C}");
            Console.WriteLine($"{nameof(variantSubsectionD)} : {variantSubsectionD}");
            Console.WriteLine($"{nameof(variantSubsectionD_Offset)} : {variantSubsectionD_Offset}");
            Console.WriteLine($"{nameof(variantSubsectionE_DiagSvc)} : {variantSubsectionE_DiagSvc}");
            Console.WriteLine($"{nameof(variantSubsectionE_Offset_DiagSvc)} : {variantSubsectionE_Offset_DiagSvc}");
            Console.WriteLine($"{nameof(variantSubsectionF)} : {variantSubsectionF}");
            Console.WriteLine($"{nameof(variantSubsectionF_Offset)} : {variantSubsectionF_Offset}");
            Console.WriteLine($"{nameof(variantSubsectionG)} : {variantSubsectionG}");
            Console.WriteLine($"{nameof(variantSubsectionG_Offset)} : {variantSubsectionG_Offset}");
            Console.WriteLine($"{nameof(variantSubsectionH)} : {variantSubsectionH}");
            Console.WriteLine($"{nameof(variantSubsectionH_Offset)} : {variantSubsectionH_Offset}");
            Console.WriteLine($"{nameof(variantVCodingDomainsCount)} : {variantVCodingDomainsCount}");
            Console.WriteLine($"{nameof(variantVCodingDomainsOffset)} : {variantVCodingDomainsOffset}");

        }
    }
}
