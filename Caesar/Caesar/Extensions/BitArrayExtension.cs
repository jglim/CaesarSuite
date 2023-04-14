using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections
{
    public static class BitArrayExtension
    {
        public static byte[] ToBytes(BitArray array)
        {
            bool misaligned = (array.Length % 8) != 0;
            int resultSize = (array.Length / 8);
            if (misaligned)
            {
                resultSize++;
            }
            byte[] result = new byte[resultSize];
            array.CopyTo(result, 0);
            return result;
        }

        public static byte[] GetBytes(BitArray array, int bitOffset, int byteCount)
        {
            return ToBytes(Slice(array, bitOffset, byteCount * 8));
        }

        public static BitArray Slice(BitArray array, int bitOffset, int bitLength) 
        {
            BitArray result = new BitArray(bitLength);
            for (int i = 0; i < bitLength; i++) 
            {
                result[i] = array[bitOffset + i];
            }
            return result;
        }

        // create an int from a bitarray of lengths 0-32
        public static int PromoteToInt32(BitArray array, bool bigEndian = false) 
        {
            if (array.Length == 1) 
            {
                return array[0] ? 1 : 0;
            }
            var byteSpan = new Span<byte>(ToBytes(array)); // might not need a span?

            switch (byteSpan.Length) 
            {
                case 1:
                    return byteSpan[0];
                case 2:
                    return bigEndian ? BinaryPrimitives.ReadInt16BigEndian(byteSpan) : BinaryPrimitives.ReadInt16LittleEndian(byteSpan);
                case 3:
                    // throw new Exception("Abonrmal condition: attempting to promote 3-byte dumps to an integer"); // wtf apparently this exists
                    {
                        byte[] buffer = new byte[4];
                        int offset = bigEndian ? 1 : 0;
                        for (int i = 0; i < byteSpan.Length; i++) 
                        {
                            buffer[i + offset] = byteSpan[i];
                        }
                        return bigEndian ? BinaryPrimitives.ReadInt32BigEndian(buffer) : BinaryPrimitives.ReadInt32LittleEndian(buffer);
                    }
                case 4:
                    return bigEndian ? BinaryPrimitives.ReadInt32BigEndian(byteSpan) : BinaryPrimitives.ReadInt32LittleEndian(byteSpan);
                default:
                    throw new Exception($"Abonrmal condition: attempting to promote {byteSpan.Length}-byte dumps to an integer");
            }
        }
        // create an uint from a bitarray of lengths 0-32
        public static uint PromoteToUInt32(BitArray array, bool bigEndian = false)
        {
            if (array.Length == 1)
            {
                return array[0] ? 1u : 0;
            }
            var byteSpan = new Span<byte>(ToBytes(array));

            switch (byteSpan.Length)
            {
                case 1:
                    return byteSpan[0];
                case 2:
                    return bigEndian ? BinaryPrimitives.ReadUInt16BigEndian(byteSpan) : BinaryPrimitives.ReadUInt16LittleEndian(byteSpan);
                case 3:
                    {
                        byte[] buffer = new byte[4];
                        int offset = bigEndian ? 1 : 0;
                        for (int i = 0; i < byteSpan.Length; i++)
                        {
                            buffer[i + offset] = byteSpan[i];
                        }
                        return bigEndian ? BinaryPrimitives.ReadUInt32BigEndian(buffer) : BinaryPrimitives.ReadUInt32LittleEndian(buffer);
                    }
                case 4:
                    return bigEndian ? BinaryPrimitives.ReadUInt32BigEndian(byteSpan) : BinaryPrimitives.ReadUInt32LittleEndian(byteSpan);
                default:
                    throw new Exception($"Abonrmal condition: attempting to promote {byteSpan.Length}-byte dumps to an integer");
            }
        }

        // 32 bit signed int to bitarray
        public static BitArray ToBitArray(int val, bool bigEndian = false, int bitArraySize = 32)
        {
            byte[] buffer = new byte[4];

            //BinaryPrimitives.WriteInt32BigEndian(buffer, val);
            // 1:
            // 00 00 00 01

            // fill byte[4] with with lsb first, regardless of endian preference
            // 1:
            // 01 00 00 00
            BinaryPrimitives.WriteInt32LittleEndian(buffer, val);

            return IntegerBytesToBitArray(buffer, bitArraySize, bigEndian);
        }

        private static BitArray IntegerBytesToBitArray(byte[] buffer, int bitArraySize, bool bigEndian) 
        {
            List<BitArray> valuesToAssemble = new List<BitArray>();

            // convert bytes to bits, preserve least significant bits if truncation occurs
            int bitsRemaining = bitArraySize;
            for (int i = 0; i < buffer.Length; i++)
            {
                valuesToAssemble.Add(ByteToBitArray(buffer[i], (bitsRemaining >= 8) ? 8 : bitsRemaining));
                bitsRemaining -= 8;
                if (bitsRemaining <= 0)
                {
                    break;
                }
            }

            // reverse array if BE
            if (bigEndian)
            {
                valuesToAssemble.Reverse();
            }

            // reassemble as an unified bitarray
            BitArray result = new BitArray(bitArraySize);
            int cursor = 0;
            foreach (var byteAsBits in valuesToAssemble)
            {
                for (int i = 0; i < byteAsBits.Length; i++)
                {
                    result[cursor] = byteAsBits[i];
                    cursor++;
                }
            }
            return result;

            /*
            big endian:
             
            255:
            0000000000000000 00000000 11111111
            256:
            0000000000000000 10000000 00000000
            257:
            0000000000000000 10000000 10000000
            384:
            0000000000000000 10000000 00000001

            default bit order: lsb starts from left to right, bit[0] = least significant bit, bit[7] = most significant bit

            if cropped to 12 bits, same values are..
            255:
            0000 11111111
            256:
            1000 00000000
            257:
            1000 10000000
            384:
            1000 00000001

            */
        }

        public static BitArray ByteToBitArray(byte val, int bitArrayLength = 8) 
        {
            // "standard" bit endian
            BitArray result = new BitArray(bitArrayLength);
            for (int i = 0; i < bitArrayLength; i++) 
            {
                result[i] = (val & 1) > 0;
                val >>= 1;
            }
            return result;
        }

        // 32 bit unsigned int to bitarray
        public static BitArray ToBitArray(uint val, bool bigEndian = false, int bitArraySize = 32)
        {
            byte[] buffer = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, val);
            return IntegerBytesToBitArray(buffer, bitArraySize, bigEndian);
        }

        // copies values in a bitarray to another bitarray, will clamp length mismatches
        public static void CopyBitsClamped(BitArray destinationToWrite, BitArray valueToCopy, bool rightToLeft = true)
        {
            int copyLength = Math.Min(destinationToWrite.Length, valueToCopy.Length);
            for (int i = 0; i < copyLength; i++)
            {
                if (rightToLeft)
                {
                    destinationToWrite[destinationToWrite.Length - 1 - i] = valueToCopy[valueToCopy.Length - 1 - i];
                }
                else
                {
                    destinationToWrite[i] = valueToCopy[i];
                }
            }
            // Console.WriteLine($"CopyBits: {BitArrayExtension.ToBitString(destinationToWrite)} valueToCopy: {ToBitString(valueToCopy)}");
        }

        public static string ToBitString(BitArray array) 
        {
            StringBuilder sb = new StringBuilder(array.Length);
            for (int i = 0; i < array.Length; i++) 
            {
                sb.Append(array[i] ? "1" : "0");
            }
            return sb.ToString();
        }

        public static float ToFloat(BitArray array, bool bigEndian = false) 
        {
            Span<byte> buffer = new Span<byte>(ToBytes(array));
            if (buffer.Length != 4) 
            {
                throw new Exception($"Could not convert {ToBitString(array)} to an IEEE float as the size ({buffer.Length}) is != 4");
            }
            return bigEndian ? BinaryPrimitives.ReadSingleBigEndian(buffer) : BinaryPrimitives.ReadSingleLittleEndian(buffer);
        }

        public static BitArray ToBitArray(float val, bool bigEndian = false)
        {
            byte[] result = new byte[4];
            if (bigEndian)
            {
                BinaryPrimitives.WriteSingleBigEndian(result, val);
            }
            else
            {
                BinaryPrimitives.WriteSingleLittleEndian(result, val);
            }
            return new BitArray(result);
        }
    }
}