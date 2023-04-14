#region License
/*Copyright(c) 2018, Brian Humlicek
* https://github.com/BrianHumlicek
* 
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sub-license, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/
#endregion License
using System;

namespace SAE.J2534
{
    [Flags]
    internal enum SAE_API
    {
        //2.02 calls
        NONE = 0x00000000,
        CONNECT = 0x00000001,
        DISCONNECT = 0x00000002,
        READMSGS = 0x00000004,
        WRITEMSGS = 0x00000008,
        STARTPERIODICMSG = 0x00000010,
        STOPPERIODICMSG = 0x00000020,
        STARTMSGFILTER = 0x00000040,
        STOPMSGFILTER = 0x00000080,
        SETPROGRAMMINGVOLTAGE = 0x00000100,
        READVERSION = 0x00000200,
        GETLASTERROR = 0x00000400,
        IOCTL = 0x00000800,
        //4.04 calls
        OPEN = 0x00001000,
        CLOSE = 0x00002000,
        //5.00 calls
        SCANFORDEVICES = 0x00004000,
        GETNEXTDEVICE = 0x00008000,
        LOGICALCONNECT = 0x00010000,
        LOGICALDISCONNECT = 0x00020000,
        SELECT = 0x00040000,
        QUEUEMESSAGES = 0x00080000,
        //Signature matches
        V202_SIGNATURE = 0x0FFF,
        V404_SIGNATURE = 0x3FFF,
        V500_SIGNATURE = 0xFFFFF
    }
}
