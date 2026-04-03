using System;
using System.Text;

namespace Caesar
{
    /// <summary>
    /// Seed-to-key solver reconstructed from the DSC VM function <c>SeedKeyBerechnung</c>
    /// found in a flash-programming DSC blob.
    ///
    /// The routine uses:
    /// 1. a 4-byte seed byte regrouping step
    /// 2. a rotation count derived from the regrouped seed
    /// 3. a feedback-driven 32-bit root rotation
    /// 4. two rounds of 16-bit rotate/add mixing
    /// 5. a final byte regrouping step to produce the ECU key
    ///
    /// The default root below is the literal extracted from the reference DSC blob
    /// used during reverse engineering.
    /// </summary>
    public static class KeySolver
    {
        public const uint DefaultRoot = 0xA09731F9;

        /// <summary>
        /// Extract the 4-byte seed word from the 8-byte interleaved seed format
        /// used by the published Rust reference implementation.
        /// </summary>
        public static uint ExtractSeedWord(ulong interleavedSeed)
        {
            return (uint)(
                ((interleavedSeed & 0xFF00_0000_0000_0000UL) >> 56) |
                ((interleavedSeed & 0x0000_FF00_0000_0000UL) >> 32) |
                ((interleavedSeed & 0x0000_0000_FF00_0000UL) >> 8) |
                ((interleavedSeed & 0x0000_0000_0000_FF00UL) << 16));
        }

        /// <summary>
        /// Compute the 4-byte key word from the 4-byte seed word.
        /// </summary>
        public static uint ComputeKeyWord(uint seedWord, uint root = DefaultRoot)
        {
            uint dwNewSeedwrd = RegroupSeedWord(seedWord);
            int numberOfRotations = (int)((dwNewSeedwrd & 0x7U) + 2U);

            uint dwSint = root;
            for (int i = 0; i < numberOfRotations; i++)
            {
                uint feedback =
                    ((dwSint >> 0) ^
                     (dwSint >> 7) ^
                     (dwSint >> 17) ^
                     (dwSint >> 26)) & 0x1U;

                uint stage = (dwSint & 0x7FFF_FFFFU) | (feedback << 31);
                dwSint = RotateRight32(stage, 1);
            }

            ushort wDummy16Hi = (ushort)(dwNewSeedwrd >> 16);
            ushort wDummy16Lo = (ushort)(dwNewSeedwrd & 0xFFFFU);

            for (int round = 0; round < 2; round++)
            {
                ushort rotatedMix = RotateLeft16((ushort)(wDummy16Hi ^ wDummy16Lo), numberOfRotations);
                ushort addend = (ushort)(round == 0 ? (dwSint & 0xFFFFU) : (dwSint >> 16));
                ushort next = unchecked((ushort)(rotatedMix + addend));

                wDummy16Hi = wDummy16Lo;
                wDummy16Lo = next;
            }

            uint combined = ((uint)wDummy16Hi << 16) | wDummy16Lo;
            return RegroupKeyWord(combined);
        }

        /// <summary>
        /// Compute the key bytes from the 4-byte seed bytes shown by the ECU.
        /// The input is interpreted in network order.
        /// </summary>
        public static byte[] ComputeKey(byte[] seedBytes, uint root = DefaultRoot)
        {
            if (seedBytes == null)
            {
                throw new ArgumentNullException(nameof(seedBytes));
            }

            if (seedBytes.Length != 4)
            {
                throw new ArgumentException("SeedKeyBerechnung expects exactly 4 seed bytes.", nameof(seedBytes));
            }

            uint seedWord =
                ((uint)seedBytes[0] << 24) |
                ((uint)seedBytes[1] << 16) |
                ((uint)seedBytes[2] << 8) |
                seedBytes[3];

            return ToBigEndianBytes(ComputeKeyWord(seedWord, root));
        }

        /// <summary>
        /// Compute the key bytes directly from the 8-byte interleaved seed format
        /// used by the published Rust reference implementation.
        /// </summary>
        public static byte[] ComputeKeyFromInterleavedSeed(ulong interleavedSeed, uint root)
        {
            return ToBigEndianBytes(ComputeKeyWord(ExtractSeedWord(interleavedSeed), root));
        }

        /// <summary>
        /// Emit the important intermediate values using the same conceptual names
        /// as the DSC debug strings.
        /// </summary>
        public static string Explain(uint seedWord, uint root = DefaultRoot)
        {
            uint dwNewSeedwrd = RegroupSeedWord(seedWord);
            int numberOfRotations = (int)((dwNewSeedwrd & 0x7U) + 2U);

            uint dwSint = root;
            for (int i = 0; i < numberOfRotations; i++)
            {
                uint feedback =
                    ((dwSint >> 0) ^
                     (dwSint >> 7) ^
                     (dwSint >> 17) ^
                     (dwSint >> 26)) & 0x1U;

                uint stage = (dwSint & 0x7FFF_FFFFU) | (feedback << 31);
                dwSint = RotateRight32(stage, 1);
            }

            ushort wDummy16Hi = (ushort)(dwNewSeedwrd >> 16);
            ushort wDummy16Lo = (ushort)(dwNewSeedwrd & 0xFFFFU);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"SeedKeyBerechnung");
            sb.AppendLine($"  Seed before regrouping: 0x{seedWord:X8}");
            sb.AppendLine($"  Root constant:          0x{root:X8}");
            sb.AppendLine($"  dwNewSeedwrd:           0x{dwNewSeedwrd:X8}");
            sb.AppendLine($"  Number of Rotation:     {numberOfRotations}");
            sb.AppendLine($"  dwSint:                 0x{dwSint:X8}");
            sb.AppendLine($"  wDummy16Hi:             0x{wDummy16Hi:X4}");
            sb.AppendLine($"  wDummy16Lo:             0x{wDummy16Lo:X4}");

            for (int round = 0; round < 2; round++)
            {
                ushort rotatedMix = RotateLeft16((ushort)(wDummy16Hi ^ wDummy16Lo), numberOfRotations);
                ushort addend = (ushort)(round == 0 ? (dwSint & 0xFFFFU) : (dwSint >> 16));
                ushort next = unchecked((ushort)(rotatedMix + addend));

                sb.AppendLine($"  Round {round}:");
                sb.AppendLine($"    mixed/rotated:        0x{rotatedMix:X4}");
                sb.AppendLine($"    addend:               0x{addend:X4}");
                sb.AppendLine($"    next:                 0x{next:X4}");

                wDummy16Hi = wDummy16Lo;
                wDummy16Lo = next;
            }

            uint combined = ((uint)wDummy16Hi << 16) | wDummy16Lo;
            uint keyWord = RegroupKeyWord(combined);
            sb.AppendLine($"  Combined 16-bit state:  0x{combined:X8}");
            sb.AppendLine($"  Key word:               0x{keyWord:X8}");
            sb.AppendLine($"  Key bytes:              {BitUtility.BytesToHex(ToBigEndianBytes(keyWord), true)}");
            return sb.ToString().TrimEnd();
        }

        private static uint RegroupSeedWord(uint seedWord)
        {
            return
                ((seedWord & 0x0000_00FFU) << 16) |
                ((seedWord & 0x0000_FF00U) >> 8) |
                ((seedWord & 0x00FF_0000U) << 8) |
                ((seedWord & 0xFF00_0000U) >> 16);
        }

        private static uint RegroupKeyWord(uint combinedWord)
        {
            return
                ((combinedWord & 0x0000_00FFU) << 24) |
                ((combinedWord & 0x0000_FF00U) << 8) |
                ((combinedWord & 0x00FF_0000U) >> 16) |
                ((combinedWord & 0xFF00_0000U) >> 16);
        }

        private static ushort RotateLeft16(ushort value, int count)
        {
            count &= 0x0F;
            if (count == 0)
            {
                return value;
            }

            return (ushort)((value << count) | (value >> (16 - count)));
        }

        private static uint RotateRight32(uint value, int count)
        {
            count &= 0x1F;
            if (count == 0)
            {
                return value;
            }

            return (value >> count) | (value << (32 - count));
        }

        private static byte[] ToBigEndianBytes(uint value)
        {
            return new[]
            {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value
            };
        }
    }
}
