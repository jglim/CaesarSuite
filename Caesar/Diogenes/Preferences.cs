using Caesar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes
{
    public class Preferences
    {
        public enum PreferenceKey 
        {
            AllowVC,
            EnableSCNZero,
            EnableFingerprintClone,
            FingerprintValue,
        }

        private static Dictionary<int, string> UserPreferences = null;

        public static void InitializePreferences()
        {
            // default values for first run / no config file
            UserPreferences = new Dictionary<int, string>();
            UserPreferences.Add((int)PreferenceKey.AllowVC, "false");
            UserPreferences.Add((int)PreferenceKey.EnableSCNZero, "true");
            UserPreferences.Add((int)PreferenceKey.FingerprintValue, "1");
            UserPreferences.Add((int)PreferenceKey.EnableFingerprintClone, "true");
            LoadAndDeserializePreferences();
        }

        public static string GetValue(PreferenceKey key)
        {
            int keyIndex = (int)key;
            if (UserPreferences is null)
            {
                InitializePreferences();
            }

            if (UserPreferences.ContainsKey(keyIndex))
            {
                return UserPreferences[keyIndex];
            }
            else
            {
                return "";
            }
        }
        public static void SetValue(PreferenceKey key, string newValue, bool save = true)
        {
            int keyIndex = (int)key;
            if (UserPreferences is null)
            {
                InitializePreferences();
            }

            if (UserPreferences.ContainsKey(keyIndex))
            {
                UserPreferences[keyIndex] = newValue;
            }
            else
            {
                UserPreferences.Add(keyIndex, newValue);
            }
            if (save)
            {
                SerializeAndSavePreferences();
            }
        }

        private static string GetPreferencesFilePath()
        {
            return Path.Combine(GetPreferencesDirectory(), "Preferences.ini");
        }
        private static string GetPreferencesDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Diogenes");
        }

        public static void SerializeAndSavePreferences() 
        {
            string prefsDirectory = GetPreferencesDirectory();
            if (!Directory.Exists(prefsDirectory)) 
            {
                Directory.CreateDirectory(prefsDirectory);
            }
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, string> row in UserPreferences) 
            {
                // assume that windows clients will mess up the line endings, so use \r\n first
                sb.Append($"{row.Key}={BitUtility.BytesToHex(Encoding.UTF8.GetBytes(row.Value))}\r\n");
            }
            File.WriteAllText(GetPreferencesFilePath(), sb.ToString());
        }

        public static bool LoadAndDeserializePreferences() 
        {
            string prefsPath = GetPreferencesFilePath();
            if (!File.Exists(prefsPath))
            {
                return false;
            }
            string[] iniRows = File.ReadAllText(prefsPath).Replace("\r", "").Split(new string[] {"\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string iniRow in iniRows) 
            {
                string[] keyValue = iniRow.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                PreferenceKey key = (PreferenceKey)int.Parse(keyValue[0]);
                string value = Encoding.UTF8.GetString(BitUtility.BytesFromHex(keyValue[1]));
                SetValue(key, value, false);
            }
            return true;
        }

    }
}
