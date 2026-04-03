using Caesar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Diogenes.SecurityAccess.NativeUnlock
{
    public static class NativeUnlockService
    {
        private const string DefinitionFileName = "NativeUnlockDefinitions.json";

        private static readonly IReadOnlyList<UnlockProvider> Providers = new UnlockProvider[]
        {
            new KiAlgo1UnlockProvider(),
            new Ki203UnlockProvider(),
            new Ki221Algo1UnlockProvider(),
            new Ki221Algo2UnlockProvider(),
            new Ic172Algo1UnlockProvider(),
            new Ic172Algo2UnlockProvider(),
            new DaimlerStandardSecurityAlgoUnlockProvider(),
            new DaimlerStandardSecurityAlgoModUnlockProvider(),
            new DaimlerStandardSecurityAlgoRefGUnlockProvider(),
        };

        private static readonly Lazy<List<UnlockDefinition>> CachedDefinitions = new Lazy<List<UnlockDefinition>>(LoadDefinitions);

        public static IReadOnlyList<UnlockProvider> GetProviders()
        {
            return Providers;
        }

        public static IReadOnlyList<UnlockDefinition> GetDefinitions()
        {
            return CachedDefinitions.Value;
        }

        public static IEnumerable<UnlockDefinition> GetAvailableDefinitions(string filter = null)
        {
            IEnumerable<UnlockDefinition> query = GetDefinitions().Where(definition =>
                Providers.Any(provider => provider.ProviderName == definition.Provider));

            if (!string.IsNullOrWhiteSpace(filter))
            {
                string normalizedFilter = filter.Trim().ToUpperInvariant();
                query = query.Where(definition =>
                    (definition.EcuName ?? string.Empty).ToUpperInvariant().Contains(normalizedFilter) ||
                    (definition.Origin ?? string.Empty).ToUpperInvariant().Contains(normalizedFilter) ||
                    definition.Aliases.Any(alias => (alias ?? string.Empty).ToUpperInvariant().Contains(normalizedFilter)));
            }

            return query
                .OrderBy(definition => definition.EcuName)
                .ThenBy(definition => definition.AccessLevel)
                .ThenBy(definition => definition.Origin);
        }

        public static IEnumerable<UnlockDefinition> FindDefinitions(string ecuName)
        {
            return GetAvailableDefinitions().Where(definition => definition.MatchesEcuName(ecuName));
        }

        public static bool TryGeneratePayload(UnlockDefinition definition, byte[] seed, out byte[] payload, out string error)
        {
            payload = Array.Empty<byte>();
            error = string.Empty;

            if (definition is null)
            {
                error = "No unlock definition selected.";
                return false;
            }

            if (seed is null)
            {
                error = "Seed cannot be null.";
                return false;
            }

            UnlockProvider provider = Providers.FirstOrDefault(candidate => candidate.ProviderName == definition.Provider);
            if (provider is null)
            {
                error = $"Provider '{definition.Provider}' is not available in this build.";
                return false;
            }

            if (seed.Length != definition.SeedLength)
            {
                error = $"Definition expects a {definition.SeedLength}-byte seed. Current seed is {seed.Length} bytes.";
                return false;
            }

            return provider.TryGeneratePayload(seed, definition, out payload, out error);
        }

        public static bool TryGeneratePayload(string ecuName, int accessLevel, byte[] seed, out byte[] payload, out UnlockDefinition definition, out string error)
        {
            payload = Array.Empty<byte>();
            error = string.Empty;
            definition = GetAvailableDefinitions().FirstOrDefault(candidate =>
                candidate.AccessLevel == accessLevel &&
                candidate.MatchesEcuName(ecuName));

            if (definition is null)
            {
                error = $"No native unlock definition matches ECU '{ecuName}' at access level {accessLevel}.";
                return false;
            }

            return TryGeneratePayload(definition, seed, out payload, out error);
        }

        public static byte[] BuildSeedRequest(UnlockDefinition definition)
        {
            return new byte[] { 0x27, (byte)definition.AccessLevel };
        }

        public static byte[] BuildUnlockRequest(UnlockDefinition definition, byte[] payload)
        {
            List<byte> request = new List<byte>(2 + payload.Length)
            {
                0x27,
                (byte)(definition.AccessLevel + 1)
            };
            request.AddRange(payload);
            return request.ToArray();
        }

        public static string DescribeDefinition(UnlockDefinition definition)
        {
            if (definition is null)
            {
                return string.Empty;
            }

            string aliases = definition.Aliases.Count == 0 ? "(none)" : string.Join(", ", definition.Aliases);
            string parameters = definition.Parameters.Count == 0
                ? "(none)"
                : string.Join(Environment.NewLine, definition.Parameters.Select(parameter => $"  {parameter.Key} [{parameter.DataType}] = {parameter.Value}"));

            return
                $"ECU: {definition.EcuName}{Environment.NewLine}" +
                $"Origin: {definition.Origin}{Environment.NewLine}" +
                $"Access level: {definition.AccessLevel}{Environment.NewLine}" +
                $"Seed length: {definition.SeedLength}{Environment.NewLine}" +
                $"Payload length: {definition.KeyLength}{Environment.NewLine}" +
                $"Provider: {definition.Provider}{Environment.NewLine}" +
                $"Aliases: {aliases}{Environment.NewLine}" +
                $"Parameters:{Environment.NewLine}{parameters}";
        }

        private static List<UnlockDefinition> LoadDefinitions()
        {
            string definitionPath = Path.Combine(AppContext.BaseDirectory, DefinitionFileName);
            if (!File.Exists(definitionPath))
            {
                return new List<UnlockDefinition>();
            }

            string json = File.ReadAllText(definitionPath);
            List<UnlockDefinition> definitions = JsonSerializer.Deserialize<List<UnlockDefinition>>(json) ?? new List<UnlockDefinition>();
            foreach (UnlockDefinition definition in definitions)
            {
                definition.Aliases ??= new List<string>();
                definition.Parameters ??= new List<UnlockParameter>();
            }

            return definitions;
        }
    }
}
