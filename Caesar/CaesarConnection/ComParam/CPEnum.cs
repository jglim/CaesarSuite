using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.ComParam
{
    public class CPEnum
    {
        public enum CANECU_CLASS
        {
            FUNCTIONAL = 1,
            PHYSICAL = 2,
            FUNCTIONAL_ALWAYS_TP = 5,
            GATEWAY_FUNCTIONAL = 9,
            GATEWAY_PHYSICAL = 10,
            GATEWAY_FUNCTIONAL_ALWAYS_TP = 13,
        }

        // this might be attached directly to a diagservice
        public enum REQUESTTYPE
        {
            RT_PHYSICAL = 0,
            RT_FUNCTIONAL = 1,
            RT_FUNCTIONAL_WITH_RESPONSE = 2,
        }

        public enum INIT_SESSION_TYPE 
        {
            ST_DEFAULT = 1,
            ST_PROGRAM = 2,
            ST_EXTENDED = 3,  
        }

        public enum BREAKCONDITION 
        {
            ONCONTEXT = 0, 
            NEVERCOME = 1,
            ONACCOUNT = 2,
            ONTIMEOUT = 3,
        }

        public enum IDENTIFIER_LENGTH 
        {
            STANDARD = 11,
            EXTENDED = 29,
        }

        public enum ADDRESSMODE 
        {
            AM_NORMAL = 0,
            AM_EXTENDED = 1,
        }

        public enum RSP 
        {
            RT_NONE = 0,
            RT_NO_POS_RSP_REQ = 1,
            RT_POS_RSP_REQ = 2,
        }

        public enum GPDAUTODOWNLOAD
        {
            SEMIAUTOMATIC = 0,
            AUTOMATIC = 1,
            MANUELL = 2,
        }

        public enum INITTYPE 
        {
            NOINIT = 0,
            DEFAULTINIT = 1, 
        }

        public enum EXCEPTION_RESPONSE
        {
            STOP = 0,
            STAY = 1,
            WAIT = 2,
            KEEP = 3,
            REPEATBEFOREHANDLE_STOP = 0x40,
            REPEATBEFOREHANDLE_STAY = 0x41,
            REPEATBEFOREHANDLE_WAIT = 0x42,
            REPEATBEFOREHANDLE_KEEP = 0x43,
            NOTIFICATION_STOP = 0x80,
            NOTIFICATION_STAY = 0x81,
            NOTIFICATION_WAIT = 0x82,
            NOTIFICATION_KEEP = 0x83,
            NOTIFICATION_REPEATBEFOREHANDLE_STOP = 0xC0,
            NOTIFICATION_REPEATBEFOREHANDLE_STAY = 0xC1,
            NOTIFICATION_REPEATBEFOREHANDLE_WAIT = 0xC2,
            NOTIFICATION_REPEATBEFOREHANDLE_KEEP = 0xC3,
        }
    }
}
