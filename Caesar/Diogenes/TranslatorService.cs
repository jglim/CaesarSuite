using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using Caesar;

namespace Diogenes
{
    public static class TranslatorService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly Dictionary<string, string> _translationCache = new Dictionary<string, string>();

        public static async Task TranslateECUAsync(ECU ecu, Action<string, int, int> progressCallback = null)
        {
            if (ecu.Language == null || ecu.Language.StringEntries == null)
            {
                return;
            }

            // Collect all unique German strings to minimize network requests.
            var uniqueStrings = new HashSet<string>();

            foreach(string str in ecu.Language.StringEntries)
            {
                if (HasTranslatableLetters(str))
                {
                    uniqueStrings.Add(str);
                }
            }

            int total = uniqueStrings.Count;
            int counter = 0;

            // Translate unique strings
            foreach (var str in uniqueStrings)
            {
                counter++;
                if (!_translationCache.ContainsKey(str))
                {
                    progressCallback?.Invoke($"Translating: {(str.Length > 20 ? str.Substring(0, 20) + "..." : str)}", counter, total);
                    string translated = await TranslateTextAsync(str);
                    _translationCache[str] = translated;
                }
            }

            progressCallback?.Invoke("Applying translations to dictionary...", total, total);

            // Apply translations back to ECU components internally
            for (int i = 0; i < ecu.Language.StringEntries.Count; i++)
            {
                string original = ecu.Language.StringEntries[i];
                if (!string.IsNullOrWhiteSpace(original) && _translationCache.TryGetValue(original, out string translatedString))
                {
                    ecu.Language.StringEntries[i] = translatedString;
                }
            }
        }

        public static async Task<string> TranslateTextAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            if (_translationCache.TryGetValue(text, out string cachedTranslation))
            {
                return cachedTranslation;
            }

            try
            {
                string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=en&dt=t&q={WebUtility.UrlEncode(text)}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    string translated = ExtractTranslatedText(jsonResult);
                    if (!string.IsNullOrWhiteSpace(translated))
                    {
                        _translationCache[text] = translated;
                        return translated;
                    }
                }
            }
            catch
            {
                // Fallback silently if offline or rate limited
            }
            return text;
        }

        private static bool HasTranslatableLetters(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            foreach (char character in text)
            {
                if (char.IsLetter(character))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ExtractTranslatedText(string jsonResult)
        {
            using (JsonDocument document = JsonDocument.Parse(jsonResult))
            {
                if ((document.RootElement.ValueKind != JsonValueKind.Array) || (document.RootElement.GetArrayLength() == 0))
                {
                    return string.Empty;
                }

                JsonElement segments = document.RootElement[0];
                if (segments.ValueKind != JsonValueKind.Array)
                {
                    return string.Empty;
                }

                List<string> translatedSegments = new List<string>();
                foreach (JsonElement segment in segments.EnumerateArray())
                {
                    if ((segment.ValueKind != JsonValueKind.Array) || (segment.GetArrayLength() == 0))
                    {
                        continue;
                    }

                    JsonElement translatedSegment = segment[0];
                    if ((translatedSegment.ValueKind == JsonValueKind.String) && !string.IsNullOrWhiteSpace(translatedSegment.GetString()))
                    {
                        translatedSegments.Add(translatedSegment.GetString());
                    }
                }

                return string.Concat(translatedSegments);
            }
        }
    }
}
