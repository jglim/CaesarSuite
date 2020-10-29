using System;
using System.Text;

namespace Caesar
{
    /// <summary>
    /// Utilities for bit and byte operations.
    /// (Frequently copied-and-pasted across my projects)
    /// </summary>
    public class BitUtility
    {
        /// <summary>
        /// Sets all values in an array of bytes to a specific value
        /// </summary>
        /// <param name="value">Value to set byte array to</param>
        /// <param name="buf">Target byte array buffer</param>
        public static void Memset(byte value, byte[] buf)
        {
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = value;
            }
        }
        // Internally used by BytesFromHex
        private static byte[] StringToByteArrayFastest(string hex)
        {
            // see https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
            if (hex.Length % 2 == 1)
            {
                throw new Exception("The binary key cannot have an odd number of digits");
            }
            byte[] arr = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexValue(hex[i << 1]) << 4) + (GetHexValue(hex[(i << 1) + 1])));
            }
            return arr;
        }
        // Internally used by StringToByteArrayFastest
        private static int GetHexValue(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : 55);
        }
        /// <summary>
        /// Converts an array of bytes into its hex-string equivalent
        /// </summary>
        /// <param name="inBytes">Input byte array</param>
        /// <param name="spacedOut">Option to add spaces between individual bytes</param>
        /// <returns>Hex-string based on the input byte array</returns>
        public static string BytesToHex(byte[] inBytes, bool spacedOut = false)
        {
            return BitConverter.ToString(inBytes).Replace("-", spacedOut ? " " : "");
        }

        /// <summary>
        /// Converts an array of bytes into a printable hex-string
        /// </summary>
        /// <param name="hexString">Input hex-string to convert into a byte array</param>
        /// <returns>Byte array based on the input hex-string</returns>
        public static byte[] BytesFromHex(string hexString)
        {
            return StringToByteArrayFastest(hexString.Replace(" ", ""));
        }

        /// <summary>
        /// Resize a smaller array of bytes to a larger array. The padding bytes will be 0.
        /// </summary>
        /// <param name="inData">Input byte array</param>
        /// <param name="finalSize">New size for the input array</param>
        /// <returns>Resized byte array</returns>
        public static byte[] PadBytes(byte[] inData, int finalSize)
        {
            if (inData.Length > finalSize)
            {
                return inData;
            }
            byte[] result = new byte[finalSize];
            Buffer.BlockCopy(inData, 0, result, 0, inData.Length);
            return result;
        }


        // Caesar specific

        public static byte[] BitArrayToByteArray(byte[] inArray, bool littleEndian = true)
        {
            if (inArray.Length % 8 != 0)
            {
                throw new NotImplementedException("Bits must be byte-aligned");
            }

            byte[] result = new byte[inArray.Length / 8];
            byte workingByte = 0;
            int arrayIndex = 0;
            for (int i = 0; i < inArray.Length; i++)
            {
                if (littleEndian)
                {
                    workingByte >>= 1;
                    workingByte |= (inArray[i] == 0) ? (byte)0 : (byte)0x80;
                }
                else
                {
                    workingByte <<= 1;
                    workingByte |= inArray[i];
                }
                if (i % 8 == 7)
                {
                    result[arrayIndex] = workingByte;
                    arrayIndex++;
                }
            }
            return result;
        }

        public static byte[] ByteArrayToBitArray(byte[] inArray, bool littleEndian = true)
        {
            byte[] result = new byte[inArray.Length * 8];
            int arrayIndex = 0;
            foreach (byte inByte in inArray)
            {
                if (littleEndian)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        bool bitSet = ((1 << i) & inByte) > 0;
                        result[arrayIndex] = (bitSet ? (byte)1 : (byte)0);
                        arrayIndex++;
                    }
                }
                else
                {
                    for (int i = 7; i >= 0; i--)
                    {
                        bool bitSet = ((1 << i) & inByte) > 0;
                        result[arrayIndex] = (bitSet ? (byte)1 : (byte)0);
                        arrayIndex++;
                    }
                }
            }
            return result;
        }

        public static string BytesToBitString(byte[] inArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in inArray)
            {
                sb.Append(Convert.ToString(b, toBase: 2).PadLeft(8, '0') + " ");
            }
            return sb.ToString().TrimEnd();
        }
        public static string BytesToDecimalString(byte[] inArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in inArray)
            {
                sb.Append(Convert.ToString(b, toBase: 10).PadLeft(3, '0') + " ");
            }
            return sb.ToString().TrimEnd();
        }
        public static byte[] BytesFromDecimalString(string inData)
        {
            string[] chunks = inData.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            byte[] result = new byte[chunks.Length];
            for (int i = 0; i < chunks.Length; i++) 
            {
                result[i] = byte.Parse(chunks[i]);
            }
            return result;
        }

        public static string BitsToString(byte[] inArray) 
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < inArray.Length; i++)
            {
                sb.Append(inArray[i]);
                if (i % 8 == 7) 
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        public void BitRoundtripTest() 
        {
            byte[] bitarray = new byte[] {
                1,1,1,1, 1,1,1,1,
                1,0,1,0, 1,0,1,0,
                0,1,0,1, 0,1,0,1,
                0,0,0,0, 0,0,0,0,
                1,1,1,1, 0,0,0,0,
                0,0,0,0, 1,1,1,1,
            };
            byte[] testByteArray = BitUtility.BitArrayToByteArray(bitarray, false);
            Console.WriteLine($"test in 1: {BitUtility.BytesToBitString(testByteArray)}");
            Console.WriteLine($"test in 1: {BitUtility.BytesToHex(testByteArray)}");
            foreach (byte b in bitarray)
            {
                Console.Write(b);
            }
            Console.WriteLine();

            byte[] testBitArray = BitUtility.ByteArrayToBitArray(testByteArray, false);
            testByteArray = BitUtility.BitArrayToByteArray(testBitArray);
            Console.WriteLine($"test in 2: {BitUtility.BytesToBitString(testByteArray)}");
            Console.WriteLine($"test in 2: {BitUtility.BytesToHex(testByteArray)}");

            foreach (byte b in testBitArray)
            {
                Console.Write(b);
            }
            Console.WriteLine();

        }
    }
}
