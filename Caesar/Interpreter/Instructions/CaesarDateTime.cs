using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class CaesarDateTime
    {
        public static void GetDateTimeComponent(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3B6:
                    {
                        ushort val = (ushort)DateTime.Now.Second;
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetSec: {val}");
                        break;
                    }
                case 0x3B7:
                    {
                        ushort val = (ushort)DateTime.Now.Minute;
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetMinc: {val}");
                        break;
                    }
                case 0x3B8:
                    {
                        ushort val = (ushort)DateTime.Now.Hour;
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetHour: {val}");
                        break;
                    }
                case 0x3B9:
                    {
                        ushort val = (ushort)DateTime.Now.Day;
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetDayOfMonth: {val}");
                        break;
                    }
                case 0x3BA:
                    {
                        ushort val = (ushort)(DateTime.Now.Month - 1); // expects jan to be 0, dec to be 11
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetMonth: {val}");
                        break;
                    }
                case 0x3BB:
                    {
                        ushort val = (ushort)(DateTime.Now.Year - 1900); // tm_year relative to 1900
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetYear: {val}");
                        break;
                    }
                case 0x3BC:
                    {
                        ushort val = (ushort)DateTime.Now.DayOfWeek; // no idea if zero indexed for original impl
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetDayOfWeek: {val}");
                        break;
                    }
                case 0x3BD:
                    {
                        ushort val = (ushort)DateTime.Now.DayOfYear; // no idea if zero indexed
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetDayOfYear: {val}");
                        break;
                    }
                case 0x3BE:
                    {
                        ushort val = DateTime.Now.IsDaylightSavingTime() ? (ushort)1 : (ushort)0;
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"IsDaylightSavingsTime: {val}");
                        break;
                    }
                case 0x3BF:
                    {
                        throw new NotImplementedException("unimplemented GetWeekOfYear");
                        /*
                        ushort val = (ushort)1;
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetWeekOfYear: {val}");
                        break;
                        */
                    }
                case 0x3C0:
                    {
                        throw new NotImplementedException("unimplemented GetYearMonth");
                        /*
                        ushort val = (ushort)DateTime.Now.Day;
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetYearMonth: {val}");
                        break;
                        */
                    }
                case 0x3C1:
                    {
                        throw new NotImplementedException("unimplemented GetDayOfDecade");
                        // expected to fault if year is before 1990 or after 2030
                        /*
                        ushort val = 0;
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"GetDayOfDecade: {val}");
                        break;
                        */
                    }
            }
        }
    }
}
