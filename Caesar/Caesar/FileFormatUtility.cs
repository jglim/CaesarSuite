using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Caesar
{
    public enum CaesarFileFormat
    {
        Unknown,
        CBF,
        CFF
    }

    public static class FileFormatUtility
    {
        private static readonly byte[] CbfHeader = Encoding.ASCII.GetBytes("CBF-TRANSLATOR-VERSION:04.00");
        
        /// <summary>
        /// Reads the magic bytes of the given file path to determine its format.
        /// </summary>
        public static CaesarFileFormat DetermineFormat(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return CaesarFileFormat.Unknown;
            }

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length < CbfHeader.Length) return CaesarFileFormat.Unknown;

                    byte[] headerBuffer = new byte[CbfHeader.Length];
                    int bytesRead = fs.Read(headerBuffer, 0, CbfHeader.Length);

                    if (bytesRead == CbfHeader.Length && headerBuffer.SequenceEqual(CbfHeader))
                    {
                        return CaesarFileFormat.CBF;
                    }
                    
                    // Note: CFF detection can be expanded here based on its specific magic bytes.
                    // For now, if the extension is .cff and it's not a CBF, we assume CFF.
                    if (filePath.EndsWith(".cff", StringComparison.OrdinalIgnoreCase))
                    {
                        return CaesarFileFormat.CFF;
                    }
                }
            }
            catch
            {
                return CaesarFileFormat.Unknown;
            }

            return CaesarFileFormat.Unknown;
        }
    }
}
