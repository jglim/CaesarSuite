﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class ComParameter
    {
        public int ComParamIndex;
        // this takes precedence over SubinterfaceIndex for KW2C3PE
        public int ParentInterfaceIndex;
        public int SubinterfaceIndex;
        public int Unk5;
        public int Unk_CTF;
        public int Phrase;
        private int DumpSize;
        public byte[] Dump;
       
        public int ComParamValue;
        public string ParamName = "";

        private long BaseAddress;
        CTFLanguage Language;

        public void Restore(CTFLanguage language) 
        {
            Language = language;
        }

        public ComParameter() { }

        // looks exactly like the definition in DIOpenDiagService (#T)
        public ComParameter(BinaryReader reader, long baseAddress, List<ECUInterface> parentEcuInterfaceList, CTFLanguage language) 
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt16();

            ComParamIndex = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            ParentInterfaceIndex = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            SubinterfaceIndex = CaesarReader.ReadBitflagInt16(ref bitflags, reader, 0);
            Unk5 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            Unk_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader); // no -1? ctf strings should have -1
            Phrase = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            DumpSize = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Dump = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, DumpSize, baseAddress);
            ComParamValue = 0;
            if (DumpSize == 4)
            {
                ComParamValue = BitConverter.ToInt32(Dump, 0);
            }
            else 
            {
                throw new Exception("Unhandled dump in comparam");
            }


            ECUInterface parentEcuInterface = parentEcuInterfaceList[ParentInterfaceIndex];

            if (ComParamIndex >= parentEcuInterface.ComParameterNames.Count)
            {
                throw new Exception("Invalid communication parameter : parent interface has no matching key");
                /*
                ParamName = "CP_UNKNOWN_MISSING_KEY";
                Console.WriteLine($"Warning: Tried to load a communication parameter without a parent (value: {ComParamValue}), parent: {parentEcuInterface.Qualifier}.");
                */
            }
            else
            {
                ParamName = parentEcuInterface.ComParameterNames[ComParamIndex];
            }
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"ComParam: id {ComParamIndex} ({ParamName}), v {ComParamValue} 0x{ComParamValue:X8} SI_Index:{SubinterfaceIndex} | parentIndex:{ParentInterfaceIndex} 5:{Unk5} DumpSize:{DumpSize} D: {BitUtility.BytesToHex(Dump)}");
            Console.WriteLine($"Pos 0x{BaseAddress:X}");
        }

        public void InsertIntoEcu(ECU parentEcu)
        {
            // KW2C3PE uses a different parent addressing style
            int parentIndex = ParentInterfaceIndex > 0 ? ParentInterfaceIndex : SubinterfaceIndex;
            if (ParentInterfaceIndex >= parentEcu.ECUInterfaceSubtypes.Count)
            {
                throw new Exception("ComParam: tried to assign to nonexistent interface");
            }
            else
            {
                // apparently it is possible to insert multiple of the same comparams..?
                var parentList = parentEcu.ECUInterfaceSubtypes[parentIndex].CommunicationParameters;
                if (parentList.Count(x => x.ParamName == ParamName) > 0)
                {
                    Console.WriteLine($"ComParam with existing key already exists, skipping insertion: {ParamName} new: {ComParamValue}");
                }
                else
                {
                    parentList.Add(this);
                }
            }
        }
    }
}
