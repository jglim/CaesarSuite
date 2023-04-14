using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class DTC
    {
        public enum DTCStatusByte : uint
        {
            TestFailedAtRequestTime = 0x01,
            TestFailedAtCurrentCycle = 0x02,
            PendingDTC = 0x04,
            ConfirmedDTC = 0x08,
            TestIncompleteSinceLastClear = 0x10,
            TestFailedSinceLastClear = 0x20,
            TestIncompleteAtCurrentCycle = 0x40,
            WarningIndicatorActive = 0x80,
        }

        // see : const char *__cdecl DIGetComfortErrorCode(DI_ECUINFO *ecuh, unsigned int dtcIndex)
        public string Qualifier;

        public int Description_CTF;
        public int Reference_CTF;

        public int XrefStart = -1;
        public int XrefCount = -1;

        private long BaseAddress;
        public int PoolIndex;

        [System.Text.Json.Serialization.JsonIgnore]
        public ECU ParentECU;
        [System.Text.Json.Serialization.JsonIgnore]
        CTFLanguage Language;

        [System.Text.Json.Serialization.JsonIgnore]
        public string Description { get { return Language.GetString(Description_CTF); } }

        public void Restore(CTFLanguage language, ECU parentEcu) 
        {
            ParentECU = parentEcu;
            Language = language;
        }

        public DTC() { }

        public DTC(BinaryReader reader, CTFLanguage language, long baseAddress, int poolIndex, ECU parentEcu)
        {
            ParentECU = parentEcu;
            PoolIndex = poolIndex;
            BaseAddress = baseAddress;
            Language = language;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitflags = reader.ReadUInt16();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            Description_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            Reference_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
#if DEBUG
            if (bitflags > 0) 
            {
                Console.WriteLine($"DTC {Qualifier} has additional unparsed fields : 0x{bitflags:X}");
            }
#endif
        }
        /*
        public string GetDescription() 
        {
            return Language.GetString(Description_CTF);
        }
        */
        public static DTC FindDTCById(string id, ECUVariant variant)
        {
            foreach (DTC dtc in variant.DTCs)
            {
                if (dtc.Qualifier.EndsWith(id))
                {
                    return dtc;
                }
            }
            return null;
        }
        public void PrintDebug() 
        {
            Console.WriteLine($"DTC: {Qualifier}: {Language.GetString(Description_CTF)} : {Language.GetString(Reference_CTF)}");
        }
    }
}
