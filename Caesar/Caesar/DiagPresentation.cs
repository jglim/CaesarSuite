using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Caesar
{
    public class DiagPresentation
    {
        public string Qualifier;
        public int Description_CTF;
        public int ScaleTableOffset;
        public int ScaleCount;
        public int BasicEnumOffset;
        public int BasicEnumCount;
        public int Unk7;
        public int Unk8;
        public int Unk9;
        public int UnkA;
        public int UnkB;
        public int UnkC;
        public int UnkD;
        public int UnkE;
        public int UnkF;
        public int DisplayedUnit_CTF;
        public int Unk11;
        public int Unk12;
        public int EnumMaxValue;
        public int Unk14;
        public int Unk15;
        public int Description2_CTF;
        public int Unk17;
        public int Unk18;
        public int Unk19;
        public int TypeLength_1A;
        public int InternalDataType; // discovered by @prj : #37
        public int Type_1C;
        public int Unk1d;
        public int SignBit; // discovered by @prj : #37
        public int ByteOrder; // discovered by @prj : #37 ; Unset = HiLo, 1 = LoHi
        public int Unk20;

        public int TypeLengthBytesMaybe_21;
        public int Unk22;
        public int Unk23;
        public int Unk24;
        public int Unk25;
        public int Unk26;
        // public string DescriptionString;
        // public string DisplayedUnitString;
        // public string DescriptionString2;

        private long BaseAddress;
        public int PresentationIndex;



        [System.Text.Json.Serialization.JsonIgnore]
        public string DescriptionString { get { return Language.GetString(Description_CTF); } }
        [System.Text.Json.Serialization.JsonIgnore]
        public string DisplayedUnitString { get { return Language.GetString(DisplayedUnit_CTF); } }
        [System.Text.Json.Serialization.JsonIgnore]
        public string DescriptionString2 { get { return Language.GetString(Description2_CTF); } }

        [System.Text.Json.Serialization.JsonIgnore]
        public CTFLanguage Language;

        public List<Scale> Scales = new List<Scale>();
        public List<BasicEnum> BasicEnums = new List<BasicEnum>();

        // trying to split InterpretData into (1) type ident, (2) type setters, (3) type getters
        public enum PresentationTypes 
        {
            PresScaledEnum,
            PresBasicEnum,
            PresInt,
            PresUInt,
            PresScaledDecimal,
            PresIEEEFloat,
            PresString,
            PresBytes,
        }

        public void Restore(CTFLanguage language) 
        {
            Language = language;
            foreach (Scale s in Scales) 
            {
                s.Restore(language);
            }
        }

        public DiagPresentation() { }

        // 0x05 [6,   4,4,4,4,  4,4,4,4,  4,4,4,4,  2,2,2,4,      4,4,4,4,   4,4,4,4,   4,4,1,1,  1,1,1,4,     4,4,2,4,   4,4],

        public DiagPresentation(BinaryReader reader, long baseAddress, int presentationsIndex, CTFLanguage language) 
        {
            BaseAddress = baseAddress;
            PresentationIndex = presentationsIndex;
            Language = language;

            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt32();
            
            ulong extendedBitflags = reader.ReadUInt16(); // skip 2 bytes

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, BaseAddress);

            Description_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);

            ScaleTableOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            ScaleCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            BasicEnumOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            BasicEnumCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            Unk7 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Unk8 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            Unk9 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            UnkA = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            UnkB = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            UnkC = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            UnkD = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            UnkE = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            UnkF = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            DisplayedUnit_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);

            Unk11 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Unk12 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            EnumMaxValue = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Unk14 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);

            Unk15 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Description2_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            Unk17 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            Unk18 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            Unk19 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            TypeLength_1A = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            InternalDataType = CaesarReader.ReadBitflagInt8(ref bitflags, reader, -1);
            Type_1C = CaesarReader.ReadBitflagInt8(ref bitflags, reader, -1);

            Unk1d = CaesarReader.ReadBitflagInt8(ref bitflags, reader);
            SignBit = CaesarReader.ReadBitflagInt8(ref bitflags, reader);
            ByteOrder = CaesarReader.ReadBitflagInt8(ref bitflags, reader);
            Unk20 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            bitflags = extendedBitflags;

            TypeLengthBytesMaybe_21 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Unk22 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            Unk23 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            Unk24 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            Unk25 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Unk26 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);


            long scaleTableBase = BaseAddress + ScaleTableOffset;
            Scales = new List<Scale>();
            for (int i = 0; i < ScaleCount; i++)
            {
                reader.BaseStream.Seek(scaleTableBase + (i * 4), SeekOrigin.Begin);
                int entryRelativeOffset = reader.ReadInt32();

                Scale scale = new Scale(reader, scaleTableBase + entryRelativeOffset, language);
                Scales.Add(scale);
            }

            long basicEnumBase = BaseAddress + BasicEnumOffset;
            BasicEnums = new List<BasicEnum>();
            for (int i = 0; i < BasicEnumCount; i++)
            {
                reader.BaseStream.Seek(basicEnumBase + (i * 8), SeekOrigin.Begin);
                int index = reader.ReadInt32();
                int field2 = reader.ReadInt32();

                BasicEnum basicEnum = new BasicEnum(reader, basicEnumBase, index, field2, language);
                BasicEnums.Add(basicEnum);
            }
        }

        public string Interpret(BitArray valueToInterpret) 
        {
            string response = "";
            var dataType = GetDataType();
            switch (dataType) 
            {
                case PresentationTypes.PresBasicEnum: 
                    {
                        // check conversion, might be faulty
                        int map = BitArrayExtension.PromoteToInt32(valueToInterpret, true);
                        foreach (var entry in BasicEnums) 
                        {
                            if (entry.EnumValue == map) 
                            {
                                response += $"{entry.EnumName}";
                                break;
                            }
                        }
                        break;
                    }
                case PresentationTypes.PresBytes: 
                    {
                        byte[] val = BitArrayExtension.ToBytes(valueToInterpret);
                        response += BitUtility.BytesToHex(val, true);
                        break;
                    }
                case PresentationTypes.PresInt:
                    {
                        response += BitArrayExtension.PromoteToInt32(valueToInterpret, true).ToString();
                        break;
                    }
                case PresentationTypes.PresUInt:
                    {
                        response += BitArrayExtension.PromoteToUInt32(valueToInterpret, true).ToString();
                        break;
                    }
                case PresentationTypes.PresScaledDecimal: 
                    {
                        decimal inputAsDecimal = BitArrayExtension.PromoteToInt32(valueToInterpret, true);
                        var scale = Scales[0];
                        // apply scale transform on raw number
                        decimal scaledValue = inputAsDecimal;
                        scaledValue *= (decimal)scale.MultiplyFactor;
                        scaledValue += (decimal)scale.AddConstOffset;

                        // scaled decimals don't require bounds check, they _have_ to fit
                        response += $"{Language.GetString(scale.EnumDescription)}: {scaledValue} {DisplayedUnitString}";

                        break;
                    }
                case PresentationTypes.PresString:
                    {
                        // fixme: need to differentiate between ascii and unicode
                        response += Encoding.ASCII.GetString(BitArrayExtension.ToBytes(valueToInterpret));
                        break;
                    }
                case PresentationTypes.PresScaledEnum:
                    {
                        bool scaleFound = false;
                        decimal inputAsDecimal = BitArrayExtension.PromoteToInt32(valueToInterpret, true);

                        // search through all scales in presentation
                        foreach (var scale in Scales)
                        {
                            // apply scale transform on raw number
                            decimal scaledValue = inputAsDecimal;
                            if (scale.MultiplyFactor != 0)
                            {
                                scaledValue *= (decimal)scale.MultiplyFactor;
                                scaledValue += (decimal)scale.AddConstOffset;
                            }

                            // check if scale fits within enum bounds
                            int scaledValueInt = (int)scaledValue;
                            if ((scaledValueInt >= scale.EnumLowBound) && (scaledValueInt <= scale.EnumUpBound))
                            {
                                scaleFound = true;
                                response += $"{Language.GetString(scale.EnumDescription)}";
                            }
                        }

                        if (!scaleFound)
                        {
                            response += $"(no matching scale found)";
                            // dump all scales

                            foreach (var scale in Scales)
                            {
                                decimal scaledValue = inputAsDecimal;
                                scaledValue *= (decimal)scale.MultiplyFactor;
                                scaledValue += (decimal)scale.AddConstOffset;

                                Console.WriteLine($"{Language.GetString(scale.EnumDescription)} Mul: {scale.MultiplyFactor} Add: {scale.AddConstOffset} Raw: {inputAsDecimal} Scaled: {scaledValue}, Enum bounds: [{scale.EnumLowBound}/{scale.EnumUpBound}]");
                            }
                        }
                        break;
                    }
                default: 
                    {
                        response += $"No handler defined for type {dataType}";
                        break;
                    }
            }
            return response;
        }

        // trying to deprecate this
        public string InterpretData(byte[] inBytes, DiagPreparation inPreparation, bool describe = true)
        {
            // might be relevant: DMPrepareSingleDatum, DMPresentSingleDatum

            bool isDebugBuild = false;
#if DEBUG
            isDebugBuild = true;
#endif

            string descriptionPrefix = describe ? $"{DescriptionString}: " : "";
            byte[] workingBytes = inBytes.Skip(inPreparation.BitPosition / 8).Take(TypeLength_1A).ToArray();

            bool isEnumType = (SignBit == 0) && ((Type_1C == 1) || (ScaleCount > 1));

            // hack: sometimes hybrid types (regularly parsed as an scaled value if within bounds) are misinterpreted as pure enums
            // this is a temporary fix for kilometerstand until there's a better way to ascertain its type
            // this also won't work on other similar cases without a unit string e.g. error instance counter (Häufigkeitszähler)
            if (DisplayedUnitString == "km")
            {
                isEnumType = false;
            }

            if (workingBytes.Length != TypeLength_1A)
            {
                return $"InBytes [{BitUtility.BytesToHex(workingBytes)}] length mismatch (expecting {TypeLength_1A})";
            }

            // handle booleans first since they're the edge case where they can cross byte boundaries
            if (inPreparation.SizeInBits == 1)
            {
                int bytesToSkip = (int)(inPreparation.BitPosition / 8);
                int bitsToSkip = inPreparation.BitPosition % 8;
                byte selectedByte = inBytes[bytesToSkip];

                int selectedBit = (selectedByte >> bitsToSkip) & 1;
                if (isEnumType && (Scales.Count > selectedBit))
                {
                    return $"{descriptionPrefix}{Language.GetString(Scales[selectedBit].EnumDescription)} {DisplayedUnitString}";
                }
                else
                {
                    return $"{descriptionPrefix}{selectedBit} {DisplayedUnitString}";
                }
            }

            // everything else should be aligned to byte boundaries
            if (inPreparation.BitPosition % 8 != 0)
            {
                return "BitOffset was outside byte boundary (skipped)";
            }
            int dataType = GetRawDataType();
            int rawIntInterpretation = 0;

            string humanReadableType = $"UnhandledType:{dataType}";
            string parsedValue = BitUtility.BytesToHex(workingBytes, true);
            if (dataType == 20)
            {
                // parse as a regular int (BE)
                for (int i = 0; i < workingBytes.Length; i++)
                {
                    rawIntInterpretation <<= 8;
                    rawIntInterpretation |= workingBytes[i];
                }

                humanReadableType = "IntegerType";

                parsedValue = rawIntInterpretation.ToString();
                if (dataType == 20)
                {
                    humanReadableType = "ScaledType";

                    double valueToScale = rawIntInterpretation;

                    // if there's only one scale, use it as-is
                    // if there's more than one, use the first scale as an interim solution;
                    // the results of stacking scales does not make sense
                    // there might be a better, non-hardcoded (0) solution to this, and perhaps with a sig-fig specifier

                    valueToScale *= Scales[0].MultiplyFactor;
                    valueToScale += Scales[0].AddConstOffset;

                    parsedValue = valueToScale.ToString("0.000000");
                }
            }
            else if (dataType == 6)
            {
                // type 6 refers to either internal presentation types 8 (ieee754 float) or 5 (unsigned int?)
                // these values are tagged with an exclamation [!] i (jglim) am not sure if they will work correctly yet
                // specifically, i am not sure if the big endian float parsing is done correctly
                uint rawUIntInterpretation = 0;
                for (int i = 0; i < 4; i++)
                {
                    rawUIntInterpretation <<= 8;
                    rawUIntInterpretation |= workingBytes[i];
                }

                if (InternalDataType == 8)
                {
                    // interpret as big-endian float, https://github.com/jglim/CaesarSuite/issues/37
                    parsedValue = BitUtility.ToFloat(rawUIntInterpretation).ToString("");
                    humanReadableType = "Float [!]";
                }
                else if (InternalDataType == 5)
                {
                    // haven't seen this one around, will parse as a regular int (BE) for now
                    humanReadableType = "UnsignedIntegerType [!]";
                    parsedValue = rawUIntInterpretation.ToString();
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

            if (isEnumType)
            {
                // discovered by @VladLupashevskyi in https://github.com/jglim/CaesarSuite/issues/27
                // if an enum is specified, the inclusive upper bound and lower bound will be defined in the scale object

                bool useNewInterpretation = false;
                foreach (Scale scale in Scales)
                {
                    if ((scale.EnumUpBound > 0) || (scale.EnumLowBound > 0))
                    {
                        useNewInterpretation = true;
                        break;
                    }
                }

                if (useNewInterpretation)
                {
                    foreach (Scale scale in Scales)
                    {
                        if ((rawIntInterpretation >= scale.EnumLowBound) && (rawIntInterpretation <= scale.EnumUpBound))
                        {
                            return $"{descriptionPrefix}{Language.GetString(scale.EnumDescription)} {DisplayedUnitString}";
                        }
                    }
                }
                else
                {
                    // original implementation, probably incorrect
                    if (rawIntInterpretation < Scales.Count)
                    {
                        return $"{descriptionPrefix}{Language.GetString(Scales[rawIntInterpretation].EnumDescription)} {DisplayedUnitString}";
                    }
                }
                return $"{descriptionPrefix}(Enum not found) {DisplayedUnitString}";
                // this bit below for troubleshooting problematic presentations
                /*
                if (rawIntInterpretation < Scales.Count)
                {
                    return $"{descriptionPrefix}{Language.GetString(Scales[rawIntInterpretation].EnumDescription)} {DisplayedUnitString}";
                }
                else 
                {
                    // seems like an enum-like value broke
                    return $"{descriptionPrefix}{Language.GetString(Scales[0].EnumDescription)} {DisplayedUnitString} [!]";
                }
                */
            }
            else
            {
                if (isDebugBuild)
                {
                    return $"{descriptionPrefix}{parsedValue} {DisplayedUnitString} ({humanReadableType})";
                }
                else
                {
                    return $"{descriptionPrefix}{parsedValue} {DisplayedUnitString}";
                }
            }
        }
        public PresentationTypes GetDataType()
        {
            // might be relevant: DMPrepareSingleDatum, DMPresentSingleDatum

            if (BasicEnumCount > 0) 
            {
                return PresentationTypes.PresBasicEnum;
            }

            /*
            // can we determine booleans without peeking at the parent prep?
            // handle booleans first since they're the edge case where they can cross byte boundaries
            if (inPreparation.SizeInBits == 1)
            {
                return PresentationTypes.PresBoolean;
            }
            */

            int rawDataType = GetRawDataType();

            if ((rawDataType == 20) && (Scales.Count == 1))
            {
                // jg : added a scales.count == 1 constraint so that scaled enums don't get stuck here
                return PresentationTypes.PresScaledDecimal;
            }
            else if (rawDataType == 6)
            {
                if (InternalDataType == 8)
                {
                    // interpret as big-endian float, https://github.com/jglim/CaesarSuite/issues/37 from @prj
                    return PresentationTypes.PresIEEEFloat;
                }
                else if (InternalDataType == 5)
                {
                    // ic204: rt_art .. segment bit (bool), PREP_Anzahl_2Byte (unsigned)
                    return PresentationTypes.PresUInt;
                }
                // no idea how to handle edge cases, defaulting to uint here
                throw new Exception($"Presentation: unhandled type {rawDataType}/{InternalDataType} for {Qualifier}");
            }
            else if (rawDataType == 18)
            {
                return PresentationTypes.PresBytes;
            }
            else if ((rawDataType == 17) || (rawDataType == 22))
            {
                // 17 regular ascii
                // 22 unicode
                return PresentationTypes.PresString;
            }

            // enums evaluated last
            bool isEnumType = (SignBit == 0) && ((Type_1C == 1) || (ScaleCount > 1));

            // hack: sometimes hybrid types (regularly parsed as an scaled value if within bounds) are misinterpreted as pure enums
            // this is a temporary fix for kilometerstand until there's a better way to ascertain its type
            // this also won't work on other similar cases without a unit string e.g. error instance counter (Häufigkeitszähler)
            if (DisplayedUnitString == "km")
            {
                isEnumType = false;
            }

            if (isEnumType)
            {
                // discovered by @VladLupashevskyi in https://github.com/jglim/CaesarSuite/issues/27
                // if an enum is specified, the inclusive upper bound and lower bound will be defined in the scale object
                return PresentationTypes.PresScaledEnum;
            }

            throw new Exception($"Could not assign a type to presentation {Qualifier}");
        }


        public int GetRawDataType() 
        {
            // see DIDiagServiceRealPresType
            int result = -1;
            if (Unk14 != -1) 
            {
                return 20;
            }

            // does the value have scale structures attached to it? 
            // supposed to parse scale struct and check if we can return 20
            if (ScaleTableOffset != -1)
            {
                return 20; // scaled value
            }
            else
            {
                if (BasicEnumOffset != -1)
                {
                    return 18; // hexdump raw
                }
                if (Unk17 != -1)
                {
                    return 18; // hexdump raw
                }
                if (Unk19 != -1)
                {
                    return 18; // hexdump raw
                }
                if (Unk22 != -1)
                {
                    return 18; // hexdump raw
                }
                if (InternalDataType != -1)
                {
                    if (InternalDataType == 6)
                    {
                        return 17; // ascii dump
                    }
                    else if (InternalDataType == 7)
                    {
                        return 22; // unicode string
                    }
                    else if (InternalDataType == 8)
                    {
                        result = 6; // IEEE754 float, discovered by @prj in https://github.com/jglim/CaesarSuite/issues/37
                    }
                    else if (InternalDataType == 5) 
                    {
                        // UNSIGNED integer (i haven't seen a const for uint around, sticking it into a regular int for now)
                        // this will be an issue for 32-bit+ uints
                        // see DT_STO_Zaehler_Programmierversuche_Reprogramming and DT_STO_ID_Aktive_Diagnose_Information_Version
                        result = 6; 
                    }
                }
                else 
                {
                    if ((TypeLength_1A == -1) || (Type_1C == -1)) 
                    {
                        Console.WriteLine("typelength and type must be valid");
                        // might be good to throw an exception here
                    }
                    if ((SignBit == 1) || (SignBit == 2))
                    {
                        result = 5; // ?? haven't seen this one around
                    }
                    else 
                    {
                        result = 2; // ?? haven't seen this one around
                    }
                }
                return result;
            }
        }

        public void PrintDebug()
        {
            Console.WriteLine("Presentation: ");
            Console.WriteLine($"{nameof(Qualifier)}: {Qualifier}");


            //Console.WriteLine($"{nameof(Description_CTF)}: {Description_CTF}");
            Console.WriteLine($"{nameof(ScaleTableOffset)}: {ScaleTableOffset}");
            Console.WriteLine($"{nameof(ScaleCount)}: {ScaleCount}");

            Console.WriteLine($"{nameof(BasicEnumOffset)}: {BasicEnumOffset}");
            Console.WriteLine($"{nameof(BasicEnumCount)}: {BasicEnumCount}");
            Console.WriteLine($"{nameof(Unk7)}: {Unk7}");
            Console.WriteLine($"{nameof(Unk8)}: {Unk8}");

            Console.WriteLine($"{nameof(Unk9)}: {Unk9}");
            Console.WriteLine($"{nameof(UnkA)}: {UnkA}");
            Console.WriteLine($"{nameof(UnkB)}: {UnkB}");
            Console.WriteLine($"{nameof(UnkC)}: {UnkC}");

            Console.WriteLine($"{nameof(UnkD)}: {UnkD}");
            Console.WriteLine($"{nameof(UnkE)}: {UnkE}");
            Console.WriteLine($"{nameof(UnkF)}: {UnkF}");
            //Console.WriteLine($"{nameof(DisplayedUnit_CTF)}: {DisplayedUnit_CTF}");

            Console.WriteLine($"{nameof(Unk11)}: {Unk11}");
            Console.WriteLine($"{nameof(Unk12)}: {Unk12}");
            Console.WriteLine($"{nameof(EnumMaxValue)}: {EnumMaxValue}");
            Console.WriteLine($"{nameof(Unk14)}: {Unk14}");

            Console.WriteLine($"{nameof(Unk15)}: {Unk15}");
            // Console.WriteLine($"{nameof(Description2_CTF)}: {Description2_CTF}");
            Console.WriteLine($"{nameof(Unk17)}: {Unk17}");
            Console.WriteLine($"{nameof(Unk18)}: {Unk18}");

            Console.WriteLine($"{nameof(Unk19)}: {Unk19}");
            Console.WriteLine($"{nameof(InternalDataType)}: {InternalDataType}");

            Console.WriteLine($"{nameof(Unk1d)}: {Unk1d}");
            Console.WriteLine($"{nameof(SignBit)}: {SignBit}");
            Console.WriteLine($"{nameof(ByteOrder)}: {ByteOrder}");
            Console.WriteLine($"{nameof(Unk20)}: {Unk20}");

            Console.WriteLine($"{nameof(TypeLengthBytesMaybe_21)}: {TypeLengthBytesMaybe_21}");
            Console.WriteLine($"{nameof(Unk22)}: {Unk22}");
            Console.WriteLine($"{nameof(Unk23)}: {Unk23}");
            Console.WriteLine($"{nameof(Unk24)}: {Unk24}");

            Console.WriteLine($"{nameof(Unk25)}: {Unk25}");
            Console.WriteLine($"{nameof(Unk26)}: {Unk26}");
            /**/


            Console.WriteLine($"{nameof(DescriptionString)}: {DescriptionString}");
            Console.WriteLine($"{nameof(DisplayedUnitString)}: {DisplayedUnitString}");
            Console.WriteLine($"{nameof(DescriptionString2)}: {DescriptionString2}");
            Console.WriteLine($"Type: {GetRawDataType()}");
            Console.WriteLine($"{nameof(Type_1C)}: {Type_1C}");
            Console.WriteLine($"{nameof(TypeLength_1A)}: {TypeLength_1A}");
            Console.WriteLine($"ScaleOffset: 0x{(ScaleTableOffset + BaseAddress):X}, base of pres @ 0x{BaseAddress:X}");

            foreach (Scale s in Scales)
            {
                Console.WriteLine("Scale: ");
                s.PrintDebug();
            }

            Console.WriteLine("Presentation end");
        }

        public string CopyMinDebug()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PRES: ");
            sb.Append($" {nameof(BasicEnumOffset)}: {BasicEnumOffset}");
            sb.Append($" {nameof(BasicEnumCount)}: {BasicEnumCount}");
            sb.Append($" {nameof(Unk7)}: {Unk7}");
            sb.Append($" {nameof(Unk8)}: {Unk8}");
            sb.Append($" {nameof(Unk9)}: {Unk9}");
            sb.Append($" {nameof(UnkA)}: {UnkA}");
            sb.Append($" {nameof(UnkB)}: {UnkB}");
            sb.Append($" {nameof(UnkC)}: {UnkC}");
            sb.Append($" {nameof(UnkD)}: {UnkD}");
            sb.Append($" {nameof(UnkE)}: {UnkE}");
            sb.Append($" {nameof(UnkF)}: {UnkF}");
            sb.Append($" {nameof(Unk11)}: {Unk11}");
            sb.Append($" {nameof(Unk12)}: {Unk12}");
            sb.Append($" {nameof(EnumMaxValue)}: {EnumMaxValue}");
            sb.Append($" {nameof(Unk14)}: {Unk14}");
            sb.Append($" {nameof(Unk15)}: {Unk15}");
            sb.Append($" {nameof(Unk17)}: {Unk17}");
            sb.Append($" {nameof(Unk18)}: {Unk18}");
            sb.Append($" {nameof(Unk19)}: {Unk19}");
            sb.Append($" {nameof(InternalDataType)}: {InternalDataType}");
            sb.Append($" {nameof(Unk1d)}: {Unk1d}");
            sb.Append($" {nameof(SignBit)}: {SignBit}");
            sb.Append($" {nameof(ByteOrder)}: {ByteOrder}");
            sb.Append($" {nameof(Unk20)}: {Unk20}");
            sb.Append($" {nameof(TypeLengthBytesMaybe_21)}: {TypeLengthBytesMaybe_21}");
            sb.Append($" {nameof(Unk22)}: {Unk22}");
            sb.Append($" {nameof(Unk23)}: {Unk23}");
            sb.Append($" {nameof(Unk24)}: {Unk24}");
            sb.Append($" {nameof(Unk25)}: {Unk25}");
            sb.Append($" {nameof(Unk26)}: {Unk26}");
            sb.Append($" {nameof(BaseAddress)}: 0x{BaseAddress:X8}");
            sb.Append($" {nameof(Type_1C)}: {Type_1C}");
            sb.Append($" {nameof(TypeLength_1A)}: {TypeLength_1A}");
            sb.Append($" Type: {GetRawDataType()}");
            sb.Append($" {nameof(ScaleTableOffset)}: {ScaleTableOffset}");
            sb.Append($" {nameof(Qualifier)}: {Qualifier}"); sb.Append($" {nameof(ScaleCount)}: {ScaleCount}");
            if (ScaleCount > 0)
            {
                sb.Append($" {Language.GetString(Scales[0].EnumDescription)}");
            }
            return sb.ToString();
        }


    }
}
