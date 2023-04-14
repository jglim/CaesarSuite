using System;
using System.ComponentModel;
using System.Linq;

namespace Common
{
    public class EnumHelper
    {
        public static string GetTypeDescription(Enum enumeration)
        {
            var enumerationtype = enumeration.GetType();
            var attribute = enumerationtype.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            return ((DescriptionAttribute)attribute)?.Description ?? enumerationtype.ToString();
        }

        public static string GetMemberDescription(Enum enumeration)
        {
            string enumerationString = enumeration.ToString();
            var member = enumeration.GetType().GetMember(enumerationString).First();
            var attribute = member.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            return ((DescriptionAttribute)attribute)?.Description ?? enumerationString;
        }
    }
}
