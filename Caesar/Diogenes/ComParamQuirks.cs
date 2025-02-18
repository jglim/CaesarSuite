using CaesarConnection.ComParam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes
{
    public class ComParamQuirks
    {
        public static Dictionary<string, int> GetQuirksForTarget(string ecuQualifier, string ifaceQualifier) 
        {
            var quirks = new Dictionary<string, int>();

            if (ecuQualifier == "KI211") 
            {
                if (ifaceQualifier == "HSCAN_KW2C3PE_500")
                {
                    quirks.Add(CP.NONSTANDARD_FUNCTIONAL_INIT_TXCOUNT, 5);
                    quirks.Add(CP.TESTERPRESENT_MESSAGE, 0x3E02);
                    quirks.Add(CP.TesterPresentTime, 1000);
                }
            }

            return quirks;
        }
    }
}
