using Caesar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Diogenes.SecurityAccess.NativeUnlock
{
    public abstract class UnlockProvider
    {
        public abstract string ProviderName { get; }

        public abstract bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error);

        protected static byte GetParameterByte(IReadOnlyList<UnlockParameter> parameters, string key)
        {
            foreach (UnlockParameter row in parameters)
            {
                if ((row.Key == key) && (row.DataType == "Byte"))
                {
                    return byte.Parse(row.Value, System.Globalization.NumberStyles.HexNumber);
                }
            }

            throw new InvalidOperationException($"Missing byte parameter '{key}'.");
        }

        protected static int GetParameterInteger(IReadOnlyList<UnlockParameter> parameters, string key)
        {
            foreach (UnlockParameter row in parameters)
            {
                if ((row.Key == key) && (row.DataType == "Int32"))
                {
                    return int.Parse(row.Value, System.Globalization.NumberStyles.HexNumber);
                }
            }

            throw new InvalidOperationException($"Missing Int32 parameter '{key}'.");
        }

        protected static long GetParameterLong(IReadOnlyList<UnlockParameter> parameters, string key)
        {
            foreach (UnlockParameter row in parameters)
            {
                if ((row.Key == key) && (row.DataType == "Int64"))
                {
                    return long.Parse(row.Value, System.Globalization.NumberStyles.HexNumber);
                }
            }

            throw new InvalidOperationException($"Missing Int64 parameter '{key}'.");
        }

        protected static byte[] GetParameterByteArray(IReadOnlyList<UnlockParameter> parameters, string key)
        {
            foreach (UnlockParameter row in parameters)
            {
                if ((row.Key == key) && (row.DataType == "ByteArray"))
                {
                    return BitUtility.BytesFromHex(row.Value);
                }
            }

            throw new InvalidOperationException($"Missing ByteArray parameter '{key}'.");
        }

        protected enum Endian
        {
            Big,
            Little,
        }

        protected static uint BytesToUInt32(byte[] input, Endian endian, int offset = 0)
        {
            uint result = 0;
            if (endian == Endian.Big)
            {
                result |= (uint)input[offset++] << 24;
                result |= (uint)input[offset++] << 16;
                result |= (uint)input[offset++] << 8;
                result |= input[offset];
            }
            else
            {
                result |= input[offset++];
                result |= (uint)input[offset++] << 8;
                result |= (uint)input[offset++] << 16;
                result |= (uint)input[offset] << 24;
            }

            return result;
        }

        protected static void UInt32ToBytes(uint input, byte[] output, Endian endian)
        {
            if (endian == Endian.Big)
            {
                output[0] = (byte)(input >> 24);
                output[1] = (byte)(input >> 16);
                output[2] = (byte)(input >> 8);
                output[3] = (byte)input;
            }
            else
            {
                output[3] = (byte)(input >> 24);
                output[2] = (byte)(input >> 16);
                output[1] = (byte)(input >> 8);
                output[0] = (byte)input;
            }
        }

        protected static byte GetBit(byte input, int bitPosition)
        {
            return (byte)((input >> bitPosition) & 1);
        }

        protected static byte GetByte(uint input, int bytePosition)
        {
            return (byte)(input >> (8 * bytePosition));
        }

        protected static byte SetBit(byte input, int bitPosition)
        {
            return (byte)(input | (1 << bitPosition));
        }

        protected static uint SetByte(uint input, byte value, int bytePosition)
        {
            int bitPosition = 8 * bytePosition;
            input &= ~(uint)(0xFF << bitPosition);
            input |= (uint)(value << bitPosition);
            return input;
        }

        protected static byte[] ExpandByteArrayToNibbles(byte[] inputArray)
        {
            byte[] result = new byte[inputArray.Length * 2];
            for (int i = 0; i < inputArray.Length; i++)
            {
                result[i * 2] = (byte)((inputArray[i] >> 4) & 0xF);
                result[i * 2 + 1] = (byte)(inputArray[i] & 0xF);
            }

            return result;
        }

        protected static byte[] CollapseByteArrayFromNibbles(byte[] inputArray)
        {
            if ((inputArray.Length % 2) != 0)
            {
                throw new InvalidOperationException("Cannot collapse an odd-numbered nibble array.");
            }

            byte[] result = new byte[inputArray.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)((inputArray[i * 2] << 4) | inputArray[i * 2 + 1]);
            }

            return result;
        }

        protected static uint RotateLeft32(uint value, int count)
        {
            count &= 0x1F;
            return count == 0 ? value : (value << count) | (value >> (32 - count));
        }

        protected static uint RotateRight32(uint value, int count)
        {
            count &= 0x1F;
            return count == 0 ? value : (value >> count) | (value << (32 - count));
        }

        protected static int CountOnes(uint value)
        {
            int result = 0;
            while (value > 0)
            {
                result += (int)(value & 1);
                value >>= 1;
            }

            return result;
        }
    }

    internal sealed class KiAlgo1UnlockProvider : UnlockProvider
    {
        public override string ProviderName => "KIAlgo1";

        public override bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error)
        {
            payload = Array.Empty<byte>();
            error = string.Empty;

            if (seed.Length != 8 || definition.KeyLength != 7)
            {
                error = "KIAlgo1 expects an 8-byte seed and a 7-byte payload.";
                return false;
            }

            try
            {
                byte level = GetParameterByte(definition.Parameters, "Level");
                uint root = BytesToUInt32(GetParameterByteArray(definition.Parameters, "K"), Endian.Big);
                ulong interleavedSeed = ToUInt64BigEndian(seed);
                byte[] keyBytes = KeySolver.ComputeKeyFromInterleavedSeed(interleavedSeed, root);

                payload = new byte[]
                {
                    level,
                    keyBytes[0],
                    keyBytes[1],
                    keyBytes[2],
                    keyBytes[3],
                    0x00,
                    0x00
                };
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static ulong ToUInt64BigEndian(byte[] bytes)
        {
            ulong result = 0;
            foreach (byte value in bytes)
            {
                result = (result << 8) | value;
            }

            return result;
        }
    }

    internal sealed class Ki203UnlockProvider : UnlockProvider
    {
        public override string ProviderName => "KI203Algo";

        public override bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error)
        {
            payload = Array.Empty<byte>();
            error = string.Empty;

            if (seed.Length != 4 || definition.KeyLength != 4)
            {
                error = "KI203Algo expects a 4-byte seed and a 4-byte key.";
                return false;
            }

            try
            {
                uint root = BytesToUInt32(GetParameterByteArray(definition.Parameters, "K"), Endian.Big);
                int ones = CountOnes(root);

                uint value =
                    (uint)(seed[2] << 0) |
                    (uint)(seed[0] << 8) |
                    (uint)(seed[3] << 16) |
                    (uint)(seed[1] << 24);

                value = RotateLeft32(value, 3);
                value ^= root;
                value = RotateRight32(value, ones);

                payload = new[]
                {
                    GetByte(value, 0),
                    GetByte(value, 2),
                    GetByte(value, 3),
                    GetByte(value, 1)
                };
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }

    internal sealed class Ki221Algo1UnlockProvider : UnlockProvider
    {
        public override string ProviderName => "KI221Algo1";

        public override bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error)
        {
            payload = Array.Empty<byte>();
            error = string.Empty;

            if (seed.Length != 8 || definition.KeyLength != 7)
            {
                error = "KI221Algo1 expects an 8-byte seed and a 7-byte payload.";
                return false;
            }

            try
            {
                byte level = GetParameterByte(definition.Parameters, "Level");
                byte[] scratch = GetParameterByteArray(definition.Parameters, "K");
                uint rootKey = BytesToUInt32(scratch, Endian.Big);

                scratch[0] ^= seed[6];
                scratch[1] ^= seed[4];
                scratch[2] ^= seed[2];
                scratch[3] ^= seed[0];

                uint rs = BytesToUInt32(new byte[] { scratch[0], scratch[3], scratch[2], scratch[1] }, Endian.Little);
                uint intermediate = RotateLeft32(rs, 29) ^ rootKey;
                uint key = RotateLeft32(intermediate, 7);

                byte[] keyBytes = new byte[4];
                UInt32ToBytes(key, keyBytes, Endian.Big);
                payload = new byte[] { level, keyBytes[0], keyBytes[3], keyBytes[2], keyBytes[1], 0x00, 0x00 };
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }

    internal sealed class Ki221Algo2UnlockProvider : UnlockProvider
    {
        public override string ProviderName => "KI221Algo2";

        public override bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error)
        {
            payload = Array.Empty<byte>();
            error = string.Empty;

            if (seed.Length != 4 || definition.KeyLength != 4)
            {
                error = "KI221Algo2 expects a 4-byte seed and a 4-byte key.";
                return false;
            }

            try
            {
                uint xor = BytesToUInt32(GetParameterByteArray(definition.Parameters, "Xor"), Endian.Little);
                uint add = BytesToUInt32(GetParameterByteArray(definition.Parameters, "Add"), Endian.Little);
                uint seedValue = BytesToUInt32(seed, Endian.Big);

                uint key = unchecked((seedValue ^ xor) + add);
                payload = new byte[4];
                UInt32ToBytes(key, payload, Endian.Big);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }

    internal sealed class Ic172Algo1UnlockProvider : UnlockProvider
    {
        public override string ProviderName => "IC172Algo1";

        public override bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error)
        {
            payload = Array.Empty<byte>();
            error = string.Empty;

            if (seed.Length != 8 || definition.KeyLength != 8)
            {
                error = "IC172Algo1 expects an 8-byte seed and an 8-byte payload.";
                return false;
            }

            byte[] seedInput = new byte[] { seed[0], seed[2], seed[4], seed[6] };
            seedInput = ExpandByteArrayToNibbles(seedInput);

            List<byte[]> keyPool = new List<byte[]>
            {
                new byte[] { 0xEF, 0xCD, 0xAB, 0x89, 0x67, 0x45, 0x23, 0x01 },
                new byte[] { 0x45, 0x67, 0x01, 0x23, 0xCD, 0xEF, 0x89, 0xAB },
                new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF },
                new byte[] { 0x89, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67 },
                new byte[] { 0x54, 0x76, 0x10, 0x32, 0xDC, 0xFE, 0x98, 0xBA },
                new byte[] { 0xEF, 0xCD, 0xAB, 0x89, 0x67, 0x45, 0x23, 0x01 },
                new byte[] { 0x89, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67 },
                new byte[] { 0xBA, 0x98, 0xFE, 0xDC, 0x32, 0x10, 0x76, 0x54 }
            };

            byte[] transpositionTable = new byte[] { 5, 2, 7, 4, 1, 6, 3, 0 };
            byte[] intermediateKey = new byte[8];

            for (int i = 0; i < transpositionTable.Length; i++)
            {
                intermediateKey[i] = ExpandByteArrayToNibbles(keyPool[i])[seedInput[transpositionTable[i]]];
            }

            byte[] assembledKey = CollapseByteArrayFromNibbles(intermediateKey);
            payload = new byte[8];
            Array.ConstrainedCopy(assembledKey, 0, payload, 0, 4);
            Array.ConstrainedCopy(new byte[] { 0x55, 0x45, 0x43, 0x55 }, 0, payload, 4, 4);
            return true;
        }
    }

    internal sealed class Ic172Algo2UnlockProvider : UnlockProvider
    {
        public override string ProviderName => "IC172Algo2";

        public override bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error)
        {
            payload = Array.Empty<byte>();
            error = string.Empty;

            if (seed.Length != 4 || definition.KeyLength != 4)
            {
                error = "IC172Algo2 expects a 4-byte seed and a 4-byte key.";
                return false;
            }

            List<int[]> keyPool = new List<int[]>
            {
                new int[] { 0, -1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11, -12, -13, -14, -15 },
                new int[] { 251, 252, 253, 254, 255, 256, 257, 258, 243, 244, 245, 246, 247, 248, 249, 250 },
                new int[] { 0, 1, -2, -1, 4, 5, 2, 3, 8, 9, 6, 7, 12, 13, 10, 11 },
                new int[] { 73, 72, 75, 74, 69, 68, 71, 70, 81, 80, 83, 82, 77, 76, 79, 78 },
                new int[] { 0, -1, -2, -3, 4, 3, 2, 1, 8, 7, 6, 5, 12, 11, 10, 9 },
                new int[] { 203, 202, 205, 204, 207, 206, 209, 208, 195, 194, 197, 196, 199, 198, 201, 200 },
                new int[] { 0, 1, 2, 3, -4, -3, -2, -1, 8, 9, 10, 11, 4, 5, 6, 7 },
                new int[] { 185, 184, 183, 182, 181, 180, 179, 178, 193, 192, 191, 190, 189, 188, 187, 186 }
            };

            byte[] nibbles = ExpandByteArrayToNibbles(seed);
            int keyResult = 0;
            for (int i = 0; i < 8; i++)
            {
                keyResult += keyPool[i][nibbles[i]] << ((7 - i) * 4);
            }

            payload = new byte[4];
            UInt32ToBytes((uint)keyResult, payload, Endian.Big);
            return true;
        }
    }

    internal sealed class DaimlerStandardSecurityAlgoUnlockProvider : UnlockProvider
    {
        public override string ProviderName => "DaimlerStandardSecurityAlgo";

        public override bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error)
        {
            return TryGenerate(seed, definition, 1103515245L, 12345L, false, out payload, out error);
        }

        internal static bool TryGenerate(byte[] seed, UnlockDefinition definition, long kA, long kC, bool useRefGKeyName, out byte[] payload, out string error)
        {
            payload = Array.Empty<byte>();
            error = string.Empty;

            if (seed.Length != 8 || definition.KeyLength != 4)
            {
                error = "Daimler standard providers expect an 8-byte seed and a 4-byte key.";
                return false;
            }

            try
            {
                string keyName = useRefGKeyName ? "K_refG" : "K";
                uint cryptoKey = BytesToUInt32(GetParameterByteArray(definition.Parameters, keyName), Endian.Big);
                long seedA = BytesToUInt32(seed, Endian.Big, 0);
                long seedB = BytesToUInt32(seed, Endian.Big, 4);

                long intermediate1 = kA * seedA + kC;
                long intermediate2 = kA * seedB + kC;
                long seedKey = intermediate1 ^ intermediate2 ^ cryptoKey;

                payload = new byte[4];
                UInt32ToBytes((uint)seedKey, payload, Endian.Big);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }

    internal sealed class DaimlerStandardSecurityAlgoModUnlockProvider : UnlockProvider
    {
        public override string ProviderName => "DaimlerStandardSecurityAlgoMod";

        public override bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error)
        {
            try
            {
                long kA = GetParameterLong(definition.Parameters, "kA");
                long kC = GetParameterLong(definition.Parameters, "kC");
                return DaimlerStandardSecurityAlgoUnlockProvider.TryGenerate(seed, definition, kA, kC, false, out payload, out error);
            }
            catch (Exception ex)
            {
                payload = Array.Empty<byte>();
                error = ex.Message;
                return false;
            }
        }
    }

    internal sealed class DaimlerStandardSecurityAlgoRefGUnlockProvider : UnlockProvider
    {
        public override string ProviderName => "DaimlerStandardSecurityAlgoRefG";

        public override bool TryGeneratePayload(byte[] seed, UnlockDefinition definition, out byte[] payload, out string error)
        {
            const long kA0 = 3040238857L;
            const long kA1 = 4126034881L;
            const long kC0 = 2094854071L;
            const long kC1 = 3555108353L;

            payload = Array.Empty<byte>();
            error = string.Empty;

            if (seed.Length != 8 || definition.KeyLength != 4)
            {
                error = "DaimlerStandardSecurityAlgoRefG expects an 8-byte seed and a 4-byte key.";
                return false;
            }

            try
            {
                uint cryptoKey = BytesToUInt32(GetParameterByteArray(definition.Parameters, "K_refG"), Endian.Big);
                long seedA = BytesToUInt32(seed, Endian.Big, 0);
                long seedB = BytesToUInt32(seed, Endian.Big, 4);

                long intermediate1 = kA0 * seedA + kC0;
                long intermediate2 = kA1 * seedB + kC1;
                long seedKey = intermediate1 ^ intermediate2 ^ cryptoKey;

                payload = new byte[4];
                UInt32ToBytes((uint)seedKey, payload, Endian.Big);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
