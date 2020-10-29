using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    class StubHeader
    {
        public const int StubHeaderSize = 0x410;
        public static readonly byte[] FileHeader = Encoding.ASCII.GetBytes("CBF-TRANSLATOR-VERSION:04.00");

        public static void ReadHeader(byte[] header)
        {
            // file checksum first, but we are skipping that
            // last 4 bytes of cbf is for checksum

            if (!header.Take(FileHeader.Length).SequenceEqual(FileHeader))
            {
                Console.WriteLine("Unknown CBF version");
            }
            int cbfHeaderIdentifier = header[0x401];
            if (cbfHeaderIdentifier != 3)
            {
                Console.WriteLine($"Unrecognized magic 2 : {cbfHeaderIdentifier}");
            }
        }
    }
}
