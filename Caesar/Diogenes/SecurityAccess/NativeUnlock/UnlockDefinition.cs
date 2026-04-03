using System.Collections.Generic;
using System.Linq;

namespace Diogenes.SecurityAccess.NativeUnlock
{
    public class UnlockDefinition
    {
        public string EcuName { get; set; }
        public List<string> Aliases { get; set; } = new List<string>();
        public int AccessLevel { get; set; }
        public int SeedLength { get; set; }
        public int KeyLength { get; set; }
        public string Provider { get; set; }
        public string Origin { get; set; }
        public List<UnlockParameter> Parameters { get; set; } = new List<UnlockParameter>();

        public bool MatchesEcuName(string ecuName)
        {
            if (string.IsNullOrWhiteSpace(ecuName))
            {
                return false;
            }

            string normalized = ecuName.Trim().ToUpperInvariant();
            if ((EcuName ?? string.Empty).Trim().ToUpperInvariant() == normalized)
            {
                return true;
            }

            return Aliases.Any(alias => (alias ?? string.Empty).Trim().ToUpperInvariant() == normalized);
        }

        public override string ToString()
        {
            return $"{EcuName} | L{AccessLevel} | Seed {SeedLength} | Key {KeyLength} | {Provider}";
        }
    }

    public class UnlockParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
    }
}
