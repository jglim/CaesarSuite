using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Caesar
{
    public class SimEntry
    {
        public string Qualifier { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }

        public byte[] RawValue { get; set; }
    }

    public class SimFileParser
    {
        public Dictionary<string, SimEntry> Entries { get; private set; } = new Dictionary<string, SimEntry>(StringComparer.OrdinalIgnoreCase);

        public SimFileParser(string filePath)
        {
            if (File.Exists(filePath))
            {
                Parse(filePath);
            }
        }

        private void Parse(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            
            // First pass: create all entries
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("["))
                    continue;

                int eqIndex = trimmed.IndexOf('=');
                if (eqIndex <= 0) continue;

                string qualifier = trimmed.Substring(0, eqIndex).Trim();
                string rest = trimmed.Substring(eqIndex + 1).Trim();

                string[] parts = rest.Split(',');
                if (parts.Length < 3) continue;

                var newEntry = new SimEntry
                {
                    Qualifier = qualifier,
                    Status = parts[0].Trim(),
                    Type = parts[1].Trim(),
                    Value = parts[2].Trim(),
                    Unit = parts.Length > 3 ? parts[3].Trim() : "",
                    Description = parts.Length > 4 ? parts[4].Trim().Trim('@') : ""
                };

                ProcessRawValue(newEntry);
                Entries[qualifier] = newEntry;
            }

            // Second pass: handle _DUMP and _DIRECT overrides
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("["))
                    continue;

                int eqIndex = trimmed.IndexOf('=');
                if (eqIndex <= 0) continue;

                string qualifier = trimmed.Substring(0, eqIndex).Trim();
                string rest = trimmed.Substring(eqIndex + 1).Trim();

                if (qualifier.Contains("_DUMP"))
                {
                    string baseQualifier = qualifier.Replace("_DUMP", "");
                    if (Entries.TryGetValue(baseQualifier, out SimEntry entry))
                    {
                        entry.RawValue = BitUtility.BytesFromHex(rest.Replace(" ", "").Replace(",", ""));
                    }
                }
                else if (qualifier.EndsWith("DIRECT"))
                {
                    string baseQualifier = qualifier.Substring(0, qualifier.Length - 6);
                    if (Entries.TryGetValue(baseQualifier, out SimEntry entry))
                    {
                        entry.RawValue = BitUtility.BytesFromHex(rest.Replace(" ", "").Replace(",", ""));
                    }
                    else
                    {
                        // standalone DIRECT
                        Entries[qualifier] = new SimEntry { Qualifier = qualifier, RawValue = BitUtility.BytesFromHex(rest.Replace(" ", "").Replace(",", "")) };
                    }
                }
            }
        }

        private void ProcessRawValue(SimEntry entry)
        {
            try
            {
                switch (entry.Type)
                {
                    case "T_SBYTE":
                    case "T_UBYTE":
                        if (byte.TryParse(entry.Value, out byte b)) entry.RawValue = new byte[] { b };
                        break;
                    case "T_SWORD":
                    case "T_UWORD":
                        if (ushort.TryParse(entry.Value, out ushort u)) 
                        {
                            entry.RawValue = BitConverter.GetBytes(u);
                            if (BitConverter.IsLittleEndian) Array.Reverse(entry.RawValue);
                        }
                        break;
                    case "T_SLONG":
                    case "T_ULONG":
                        if (uint.TryParse(entry.Value, out uint ui))
                        {
                            entry.RawValue = BitConverter.GetBytes(ui);
                            if (BitConverter.IsLittleEndian) Array.Reverse(entry.RawValue);
                        }
                        break;
                    case "T_FLOAT":
                        if (float.TryParse(entry.Value, out float f))
                        {
                            entry.RawValue = BitConverter.GetBytes(f);
                            if (BitConverter.IsLittleEndian) Array.Reverse(entry.RawValue);
                        }
                        break;
                    case "T_STRING":
                        entry.RawValue = System.Text.Encoding.ASCII.GetBytes(entry.Value);
                        break;
                }
            }
            catch { }
        }
    }
}
