using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class DiagPresentation
    {
        public string Qualifier;
        public int Description_CTF;
        public int ScaleTableOffset;
        public int ScaleCountMaybe;
        public int unk5;
        public int unk6;
        public int unk7;
        public int unk8;
        public int unk9;
        public int unka;
        public int unkb;
        public int unkc;
        public int unkd;
        public int unke;
        public int unkf;
        public int DisplayedUnit_CTF;
        public int unk11;
        public int unk12;
        public int unk13;
        public int unk14;
        public int unk15;
        public int Description2_CTF;
        public int unk17;
        public int unk18;
        public int unk19;
        public int TypeLength_1a;
        public int unk1b;
        public int Type_1c;
        public int unk1d;
        public int unk1e;
        public int unk1f;
        public int unk20;

        public int unk21;
        public int unk22;
        public int unk23;
        public int unk24;
        public int unk25;
        public int unk26;
        public string DescriptionString;
        public string DisplayedUnitString;
        public string DescriptionString2;

        public long BaseAddress;
        public int PresentationIndex;
        public List<Scale> Scales = new List<Scale>();

        // 0x05 [6,   4,4,4,4,  4,4,4,4,  4,4,4,4,  2,2,2,4,      4,4,4,4,   4,4,4,4,   4,4,1,1,  1,1,1,4,     4,4,2,4,   4,4],

        public DiagPresentation(BinaryReader reader, long baseAddress, int presentationsIndex, CTFLanguage language) 
        {
            BaseAddress = baseAddress;
            PresentationIndex = presentationsIndex;

            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt32();
            
            ulong extendedBitflags = reader.ReadUInt16(); // skip 2 bytes

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, BaseAddress);

            Description_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            ScaleTableOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            ScaleCountMaybe = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            unk5 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            unk6 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unk7 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unk8 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            unk9 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unka = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unkb = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unkc = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            unkd = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            unke = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            unkf = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            DisplayedUnit_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);

            unk11 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unk12 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unk13 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unk14 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);

            unk15 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Description2_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            unk17 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            unk18 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            unk19 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            TypeLength_1a = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            unk1b = CaesarReader.ReadBitflagInt8(ref bitflags, reader, -1);
            Type_1c = CaesarReader.ReadBitflagInt8(ref bitflags, reader, -1);

            unk1d = CaesarReader.ReadBitflagInt8(ref bitflags, reader);
            unk1e = CaesarReader.ReadBitflagInt8(ref bitflags, reader);
            unk1f = CaesarReader.ReadBitflagInt8(ref bitflags, reader);
            unk20 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            bitflags = extendedBitflags;

            unk21 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unk22 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            unk23 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            unk24 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            unk25 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            unk26 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);


            DescriptionString = language.GetString(Description_CTF);
            DisplayedUnitString = language.GetString(DisplayedUnit_CTF);
            DescriptionString2 = language.GetString(Description2_CTF);


            long scaleTableBase = BaseAddress + ScaleTableOffset;
            Scales = new List<Scale>();
            for (int i = 0; i < ScaleCountMaybe; i++) 
            {
                reader.BaseStream.Seek(scaleTableBase + (i * 4), SeekOrigin.Begin);
                int entryRelativeOffset = reader.ReadInt32();

                Scale scale = new Scale(reader, scaleTableBase + entryRelativeOffset);
                Scales.Add(scale);
            }
        }

        public string InterpretData(byte[] inBytes, DiagPreparation inPreparation)
        {
            // might be relevant: DMPrepareSingleDatum, DMPresentSingleDatum

            byte[] workingBytes = inBytes.Skip(inPreparation.BitPosition / 8).Take(TypeLength_1a).ToArray();
            if (workingBytes.Length != TypeLength_1a)
            {
                return $"InBytes [{BitUtility.BytesToHex(workingBytes)}] length mismatch (expecting {TypeLength_1a})";
            }

            // handle booleans first since they're the edge case where they can cross byte boundaries
            if (inPreparation.SizeInBits == 1)
            {
                int bytesToSkip = (int)(inPreparation.BitPosition / 8);
                int bitsToSkip = inPreparation.BitPosition % 8;
                byte selectedByte = inBytes[bytesToSkip];

                int selectedBit = (selectedByte >> bitsToSkip) & 1;

                return $"{DescriptionString}: {selectedBit} {DisplayedUnitString} (BitType)";
            }

            // everything else should be aligned to byte boundaries
            if (inPreparation.BitPosition % 8 != 0)
            {
                return "BitOffset was outside byte boundary (skipped)";
            }

            int dataType = GetDataType();
            string humanReadableType = $"UnhandledType:{dataType}";
            string parsedValue = BitUtility.BytesToHex(workingBytes, true);
            if ((dataType == 6 || (dataType == 20)))
            {
                // parse as a regular int (BE)
                int result = 0;

                for (int i = 0; i < workingBytes.Length; i++) 
                {
                    result <<= 8;
                    result |= workingBytes[i];
                }

                humanReadableType = "IntegerType";

                parsedValue = result.ToString();
                if (dataType == 20) 
                {
                    humanReadableType = "ScaledType";

                    double valueToScale = result;
                    foreach (Scale scale in Scales) 
                    {
                        valueToScale *= scale.MultiplyFactor;
                        valueToScale += scale.AddConstOffset;
                    }

                    parsedValue = valueToScale.ToString("0.000000");
                }
            }
            else if (dataType == 18)
            {
                humanReadableType = "HexdumpType";
            }
            else if (dataType == 17)
            {
                humanReadableType = "StringType";
                parsedValue = Encoding.UTF8.GetString(workingBytes);
            }

            return $"{DescriptionString}: {parsedValue} {DisplayedUnitString} ({humanReadableType})";
        }

        public int GetDataType() 
        {
            // see DIDiagServiceRealPresType

            int result = -1;
            if (unk14 != -1) 
            {
                return 20;
            }
            if (ScaleTableOffset != -1)
            {
                // scale value
                // integer
                return (ScaleTableOffset == -1) ? 6 : 20;
                // supposed to parse scale struct and check if we can return 20
            }
            else
            {
                if (unk5 != -1)
                {
                    // hexdump raw
                    return 18;
                }
                if (unk17 != -1)
                {
                    // hexdump raw
                    return 18;
                }
                if (unk19 != -1)
                {
                    // hexdump raw
                    return 18;
                }
                if (unk22 != -1)
                {
                    // hexdump raw
                    return 18;
                }
                if (unk1b != -1)
                {
                    if (unk1b == 6)
                    {
                        // ascii dump
                        return 17;
                    }
                    else if (unk1b == 7)
                    {
                        return 22;
                    }
                    else if (unk1b == 8)
                    {
                        // integer
                        result = 6;
                    }
                }
                else 
                {
                    if ((TypeLength_1a == -1) || (Type_1c == -1)) 
                    {
                        Console.WriteLine("typelength and type must be valid");
                        // might be good to throw an exception here
                    }
                    if ((unk1e == 1) || (unk1e == 2))
                    {
                        result = 5;
                    }
                    else 
                    {
                        result = 2;
                    }
                }
                return result;
            }
        }

        public void PrintDebug() 
        {
            Console.WriteLine("Presentation: ");
            Console.WriteLine($"{nameof(Qualifier)}: {Qualifier}");

            /*
            //Console.WriteLine($"{nameof(Description_CTF)}: {Description_CTF}");
            Console.WriteLine($"{nameof(ScaleTableOffset)}: {ScaleTableOffset}");
            Console.WriteLine($"{nameof(ScaleCountMaybe)}: {ScaleCountMaybe}");

            Console.WriteLine($"{nameof(unk5)}: {unk5}");
            Console.WriteLine($"{nameof(unk6)}: {unk6}");
            Console.WriteLine($"{nameof(unk7)}: {unk7}");
            Console.WriteLine($"{nameof(unk8)}: {unk8}");

            Console.WriteLine($"{nameof(unk9)}: {unk9}");
            Console.WriteLine($"{nameof(unka)}: {unka}");
            Console.WriteLine($"{nameof(unkb)}: {unkb}");
            Console.WriteLine($"{nameof(unkc)}: {unkc}");

            Console.WriteLine($"{nameof(unkd)}: {unkd}");
            Console.WriteLine($"{nameof(unke)}: {unke}");
            Console.WriteLine($"{nameof(unkf)}: {unkf}");
            //Console.WriteLine($"{nameof(DisplayedUnit_CTF)}: {DisplayedUnit_CTF}");

            Console.WriteLine($"{nameof(unk11)}: {unk11}");
            Console.WriteLine($"{nameof(unk12)}: {unk12}");
            Console.WriteLine($"{nameof(unk13)}: {unk13}");
            Console.WriteLine($"{nameof(unk14)}: {unk14}");

            Console.WriteLine($"{nameof(unk15)}: {unk15}");
            // Console.WriteLine($"{nameof(Description2_CTF)}: {Description2_CTF}");
            Console.WriteLine($"{nameof(unk17)}: {unk17}");
            Console.WriteLine($"{nameof(unk18)}: {unk18}");

            Console.WriteLine($"{nameof(unk19)}: {unk19}");
            Console.WriteLine($"{nameof(unk1b)}: {unk1b}");

            Console.WriteLine($"{nameof(unk1d)}: {unk1d}");
            Console.WriteLine($"{nameof(unk1e)}: {unk1e}");
            Console.WriteLine($"{nameof(unk1f)}: {unk1f}");
            Console.WriteLine($"{nameof(unk20)}: {unk20}");

            Console.WriteLine($"{nameof(unk21)}: {unk21}");
            Console.WriteLine($"{nameof(unk22)}: {unk22}");
            Console.WriteLine($"{nameof(unk23)}: {unk23}");
            Console.WriteLine($"{nameof(unk24)}: {unk24}");

            Console.WriteLine($"{nameof(unk25)}: {unk25}");
            Console.WriteLine($"{nameof(unk26)}: {unk26}");
            */


            Console.WriteLine($"{nameof(DescriptionString)}: {DescriptionString}");
            Console.WriteLine($"{nameof(DisplayedUnitString)}: {DisplayedUnitString}");
            Console.WriteLine($"{nameof(DescriptionString2)}: {DescriptionString2}");
            Console.WriteLine($"Type: {GetDataType()}");
            Console.WriteLine($"{nameof(Type_1c)}: {Type_1c}");
            Console.WriteLine($"{nameof(TypeLength_1a)}: {TypeLength_1a}");
            Console.WriteLine($"ScaleOffset: 0x{(ScaleTableOffset + BaseAddress):X}, base of pres @ 0x{BaseAddress:X}");

            foreach (Scale s in Scales) 
            {
                Console.WriteLine("Scale: ");
                s.PrintDebug();
            }

            Console.WriteLine("Presentation end");
        }


    }
}
