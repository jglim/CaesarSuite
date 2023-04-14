using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes
{
    public class LinkerTime
    {
        // https://rmauro.dev/add-build-time-to-your-csharp-assembly/
        public static DateTime GetLinkerTime()
        {
            return GetLinkerTime(Assembly.GetEntryAssembly());
        }

        public static DateTime GetLinkerTime(Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";
            const string dateFormat = "yyyy-MM-ddTHH:mm:ss:fffZ";

            var attribute = assembly
              .GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value[(index + BuildVersionMetadataPrefix.Length)..];

                    return DateTime.ParseExact(
                        value,
                      dateFormat,
                      CultureInfo.InvariantCulture);
                }
            }
            return default;
        }
    }
}
