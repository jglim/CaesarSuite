using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    // this was originally copied from the main Caesar library so that the interpreter can operate without a dependency to Caesar
    public class CaesarReaderStandalone
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;

        // slightly more complex because it jumps to the string position for reading
        public static string ReadBitflagStringWithReader(ref ulong bitFlags, BinaryReader reader, long virtualBase = 0)
        {
            if (CheckAndAdvanceBitflag(ref bitFlags))
            {
                // read the string's offset relative to our current block
                int stringOffset = reader.ReadInt32();
                // save our reading cursor
                long readerPosition = reader.BaseStream.Position;
                // seek to the specified offset, then read out the string
                reader.BaseStream.Seek(stringOffset + virtualBase, SeekOrigin.Begin);
                string result = ReadStringFromBinaryReader(reader);
                // restore our reading cursor
                reader.BaseStream.Seek(readerPosition, SeekOrigin.Begin);
                return result;
            }
            else
            {
                // Console.WriteLine("Bitflag was off for string");
                return "(flag disabled)";
            }
        }
        public static byte[] ReadBitflagDumpWithReader(ref ulong bitFlags, BinaryReader reader, int dumpSize, long virtualBase = 0)
        {
            if (CheckAndAdvanceBitflag(ref bitFlags))
            {
                // read the dump's offset relative to our current block
                int dumpOffset = reader.ReadInt32();
                // save our reading cursor
                long readerPosition = reader.BaseStream.Position;
                // seek to the specified offset, then read out the dump
                reader.BaseStream.Seek(dumpOffset + virtualBase, SeekOrigin.Begin);
                byte[] result = reader.ReadBytes(dumpSize);
                // restore our reading cursor
                reader.BaseStream.Seek(readerPosition, SeekOrigin.Begin);
                return result;
            }
            else
            {
                return new byte[] { };
            }
        }

        public static string ReadBitflagDumpWithReaderAsString(ref ulong bitFlags, BinaryReader reader, int dumpSize, long virtualBase = 0)
        {
            byte[] stringBytes = ReadBitflagDumpWithReader(ref bitFlags, reader, dumpSize, virtualBase);
            return DefaultEncoding.GetString(stringBytes); // lazy: no encoding is specified
        }

        public static string ReadStringFromBinaryReader(BinaryReader reader, Encoding encoding = null)
        {
            if (encoding is null)
            {
                encoding = DefaultEncoding;
            }

            // slightly better performance than the original below at the expense of compiling with unsafe
            long stringStartPosition = reader.BaseStream.Position;
            byte[] underlyingBuffer = ((MemoryStream)reader.BaseStream).GetBuffer();
            long cursor = stringStartPosition;
            while (underlyingBuffer[cursor++] != 0)
            {
            }
            int difference = (int)(cursor - stringStartPosition) - 1;
            byte[] stringBytes = new byte[difference];
            System.Buffer.BlockCopy(underlyingBuffer, (int)stringStartPosition, stringBytes, 0, difference);
            return encoding.GetString(stringBytes);

            /*
            // significant performance bottleneck: (original)
            // read out a string, stopping at the first null terminator
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
            {
                while (true)
                {
                    byte nextByte = reader.ReadByte();
                    if (nextByte == 0)
                    {
                        byte[] stringRaw = ((MemoryStream)writer.BaseStream).ToArray();
                        return encoding.GetString(stringRaw);
                    }
                    else
                    {
                        writer.Write(nextByte);
                    }
                }
            }
            */
        }

        public static bool CheckAndAdvanceBitflag(ref ulong bitFlag)
        {
            bool flagIsSet = (bitFlag & 1) > 0;
            bitFlag >>= 1;
            return flagIsSet;
        }
        public static float ReadBitflagFloat(ref ulong bitFlags, BinaryReader reader, float defaultResult = 0)
        {
            if (CheckAndAdvanceBitflag(ref bitFlags))
            {
                byte[] floatBytes = reader.ReadBytes(4);
                return BitConverter.ToSingle(floatBytes, 0);
            }
            return defaultResult;
        }
        public static int ReadBitflagInt32(ref ulong bitFlags, BinaryReader reader, int defaultResult = 0)
        {
            if (CheckAndAdvanceBitflag(ref bitFlags))
            {
                return reader.ReadInt32();
            }
            return defaultResult;
        }
        public static uint ReadBitflagUInt32(ref ulong bitFlags, BinaryReader reader, uint defaultResult = 0)
        {
            if (CheckAndAdvanceBitflag(ref bitFlags))
            {
                return reader.ReadUInt32();
            }
            return defaultResult;
        }
        public static short ReadBitflagInt16(ref ulong bitFlags, BinaryReader reader, short defaultResult = 0)
        {
            if (CheckAndAdvanceBitflag(ref bitFlags))
            {
                return reader.ReadInt16();
            }
            return defaultResult;
        }
        public static ushort ReadBitflagUInt16(ref ulong bitFlags, BinaryReader reader, ushort defaultResult = 0)
        {
            if (CheckAndAdvanceBitflag(ref bitFlags))
            {
                return reader.ReadUInt16();
            }
            return defaultResult;
        }
        public static int ReadBitflagInt8(ref ulong bitFlags, BinaryReader reader, int defaultResult = 0)
        {
            if (CheckAndAdvanceBitflag(ref bitFlags))
            {
                return reader.ReadChar();
            }
            return defaultResult;
        }
        public static byte ReadBitflagUInt8(ref ulong bitFlags, BinaryReader reader, byte defaultResult = 0)
        {
            if (CheckAndAdvanceBitflag(ref bitFlags))
            {
                return reader.ReadByte();
            }
            return defaultResult;
        }

        public static int ReadIntWithSize(BinaryReader reader, int size, long offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            if (size == 1)
            {
                return reader.ReadChar();
            }
            else if (size == 2)
            {
                return reader.ReadInt16();
            }
            else if (size == 4)
            {
                return reader.ReadInt32();
            }
            else
            {
                throw new NotImplementedException($"Requested an unknown integer size to read: {size}");
            }
        }
        public static uint ReadUIntWithSize(BinaryReader reader, int size, long offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            if (size == 1)
            {
                return reader.ReadByte();
            }
            else if (size == 2)
            {
                return reader.ReadUInt16();
            }
            else if (size == 4)
            {
                return reader.ReadUInt32();
            }
            else
            {
                throw new NotImplementedException($"Requested an unknown integer size to read: {size}");
            }
        }

    }
}
