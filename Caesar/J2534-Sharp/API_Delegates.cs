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
/* 
 * Copyright (c) 2010, Michael Kelly
 * michael.e.kelly@gmail.com
 * http://michael-kelly.com/
 * 
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the organization nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */
#endregion License
using System;
using System.Runtime.InteropServices;

namespace SAE.J2534
{
    public partial class API
    {
        internal PassThruConnect PTConnect = delegate (int DeviceID, int ProtocolID, int ConnectFlags, int Baud, IntPtr ChannelID) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruDisconnect PTDisconnect = delegate (int channelId) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruReadMsgs PTReadMsgs = delegate (int ChannelID, IntPtr pMsgArray, IntPtr NumMsgs, int Timeout) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruWriteMsgs PTWriteMsgs = delegate (int ChannelID, IntPtr pMsgArray, IntPtr NumMsgs, int Timeout) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruStartPeriodicMsg PTStartPeriodicMsg = delegate (int ChannelID, IntPtr pMsg, IntPtr MsgID, int Interval) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruStopPeriodicMsg PTStopPeriodicMsg = delegate (int ChannelID, int MsgID) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruStartMsgFilter PTStartMsgFilter = delegate (
            int ChannelID,
            int FilterType,
            IntPtr pMaskMsg,
            IntPtr pPatternMsg,
            IntPtr pFlowControlMsg,
            IntPtr FilterID
        ) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruStopMsgFilter PTStopMsgFilter = delegate (int ChannelID, int FilterID) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruSetProgrammingVoltage PTSetProgrammingVoltage = delegate (int DeviceID, int Pin, int Voltage) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruReadVersion PTReadVersion = delegate (int DeviceID, IntPtr pFirmwareVer, IntPtr pDllVer, IntPtr pAPIVer) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruGetLastError PTGetLastError = delegate (IntPtr pErr) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruIoctl PTIoctl = delegate (int HandleID, int IoctlID, IntPtr Input, IntPtr Output) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        //v2.02
        internal PassThruConnectv202 PTConnectv202 = delegate (int ProtocolID, int ConnectFlags, IntPtr ChannelID) { return ResultCode.FUNCTION_NOT_ASSIGNED; } ;
        internal PassThruSetProgrammingVoltagev202 PTSetProgrammingVoltagev202 = delegate (int Pin, int Voltage) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruReadVersionv202 PTReadVersionv202 = delegate (IntPtr pFirmwareVer, IntPtr pDllVer, IntPtr pAPIVer) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        //v4.04
        internal PassThruOpen PTOpen = delegate (IntPtr pDeviceName, IntPtr DeviceID) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruClose PTClose = delegate (int DeviceID) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        //v5.00 and undocumented Drewtech calls
        internal PassThruScanForDevices PTScanForDevices = delegate (ref int DeviceCount) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruGetNextDevice PTGetNextDevice = delegate (IntPtr pSDevice) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruLogicalConnect PTLogicalConnect = delegate (int PhysicalChannelID, int ProtocolID, int ConnectFlags, IntPtr ChannelDescriptor, ref int pChannelID) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruLogicalDisconnect PTLogicalDisconnect = delegate (int pChannelID) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruSelect PTSelect = delegate (IntPtr pSChannelSet, int SelectType, int Timeout) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        internal PassThruQueueMsgs PTQueueMsgs = delegate (int ChannelID, IntPtr pMsgArray, ref int NumMsgs) { return ResultCode.FUNCTION_NOT_ASSIGNED; };
        //Drewtech ONLY
        internal PassThruGetNextCarDAQ PTGetNextCarDAQ = delegate (IntPtr pName, IntPtr pVer, IntPtr pAddress) { return ResultCode.FUNCTION_NOT_ASSIGNED; };

        private API_Signature AssignDelegates()
        {
            if (pLibrary == IntPtr.Zero)
                return APISignature;

            IntPtr pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruOpen");
            if (pFunction != IntPtr.Zero)
            {
                PTOpen = Marshal.GetDelegateForFunctionPointer<PassThruOpen>(pFunction);
                APISignature.SAE_API |= SAE_API.OPEN;
            }
            else
                PTOpen = Open_shim;

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruClose");
            if (pFunction != IntPtr.Zero)
            {
                PTClose = Marshal.GetDelegateForFunctionPointer<PassThruClose>(pFunction);
                APISignature.SAE_API |= SAE_API.CLOSE;
            }
            else
                PTClose = Close_shim;

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruConnect");
            if (pFunction != IntPtr.Zero)
            {
                //If the API is v4.04 (because it has 'PassThruOpen')
                if (APISignature.SAE_API.HasFlag(SAE_API.OPEN))
                    //Make 'Connect' work directly with the library function
                    PTConnect = Marshal.GetDelegateForFunctionPointer<PassThruConnect>(pFunction);
                else
                {
                    //Otherwise, use the v202 prototype and wrap it with the v404 call
                    PTConnectv202 = Marshal.GetDelegateForFunctionPointer<PassThruConnectv202>(pFunction);
                    PTConnect = Connect_shim;
                }
                APISignature.SAE_API |= SAE_API.CONNECT;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruDisconnect");
            if (pFunction != IntPtr.Zero)
            {
                PTDisconnect = Marshal.GetDelegateForFunctionPointer<PassThruDisconnect>(pFunction);
                APISignature.SAE_API |= SAE_API.DISCONNECT;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruReadMsgs");
            if (pFunction != IntPtr.Zero)
            {
                PTReadMsgs = Marshal.GetDelegateForFunctionPointer<PassThruReadMsgs>(pFunction);
                APISignature.SAE_API |= SAE_API.READMSGS;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruWriteMsgs");
            if (pFunction != IntPtr.Zero)
            {
                PTWriteMsgs = Marshal.GetDelegateForFunctionPointer<PassThruWriteMsgs>(pFunction);
                APISignature.SAE_API |= SAE_API.WRITEMSGS;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruStartPeriodicMsg");
            if (pFunction != IntPtr.Zero)
            {
                PTStartPeriodicMsg = Marshal.GetDelegateForFunctionPointer<PassThruStartPeriodicMsg>(pFunction);
                APISignature.SAE_API |= SAE_API.STARTPERIODICMSG;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruStopPeriodicMsg");
            if (pFunction != IntPtr.Zero)
            {
                PTStopPeriodicMsg = Marshal.GetDelegateForFunctionPointer<PassThruStopPeriodicMsg>(pFunction);
                APISignature.SAE_API |= SAE_API.STOPPERIODICMSG;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruStartMsgFilter");
            if (pFunction != IntPtr.Zero)
            {
                PTStartMsgFilter = Marshal.GetDelegateForFunctionPointer<PassThruStartMsgFilter>(pFunction);
                APISignature.SAE_API |= SAE_API.STARTMSGFILTER;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruStopMsgFilter");
            if (pFunction != IntPtr.Zero)
            {
                PTStopMsgFilter = Marshal.GetDelegateForFunctionPointer<PassThruStopMsgFilter>(pFunction);
                APISignature.SAE_API |= SAE_API.STOPMSGFILTER;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruSetProgrammingVoltage");
            if (pFunction != IntPtr.Zero)
            {
                //If the API is v4.04 (because it has 'PassThruOpen')
                if (APISignature.SAE_API.HasFlag(SAE_API.OPEN))
                    //Make 'Connect' work directly with the library function
                    PTSetProgrammingVoltage = Marshal.GetDelegateForFunctionPointer<PassThruSetProgrammingVoltage>(pFunction);
                else
                {
                    //Otherwise, use the v202 prototype and wrap it with the v404 call
                    PTSetProgrammingVoltagev202 = Marshal.GetDelegateForFunctionPointer<PassThruSetProgrammingVoltagev202>(pFunction);
                    PTSetProgrammingVoltage = SetVoltage_shim;
                }
                APISignature.SAE_API |= SAE_API.SETPROGRAMMINGVOLTAGE;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruReadVersion");
            if (pFunction != IntPtr.Zero)
            {
                //If the API is v4.04 (because it has 'PassThruOpen')
                if (APISignature.SAE_API.HasFlag(SAE_API.OPEN))
                    //Make 'Connect' work directly with the library function
                    PTReadVersion = Marshal.GetDelegateForFunctionPointer<PassThruReadVersion>(pFunction);
                else
                {
                    //Otherwise, use the v202 prototype and wrap it with the v404 call
                    PTReadVersionv202 = Marshal.GetDelegateForFunctionPointer<PassThruReadVersionv202>(pFunction);
                    PTReadVersion = ReadVersion_shim;
                }
                APISignature.SAE_API |= SAE_API.READVERSION;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruGetLastError");
            if (pFunction != IntPtr.Zero)
            {
                PTGetLastError = Marshal.GetDelegateForFunctionPointer<PassThruGetLastError>(pFunction);
                APISignature.SAE_API |= SAE_API.GETLASTERROR;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruIoctl");
            if (pFunction != IntPtr.Zero)
            {
                PTIoctl = Marshal.GetDelegateForFunctionPointer<PassThruIoctl>(pFunction);
                APISignature.SAE_API |= SAE_API.IOCTL;
            }

            //v5.00
            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruScanForDevices");
            if (pFunction != IntPtr.Zero)
            {
                PTScanForDevices = Marshal.GetDelegateForFunctionPointer<PassThruScanForDevices>(pFunction);
                APISignature.SAE_API |= SAE_API.SCANFORDEVICES;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruGetNextDevice");
            if (pFunction != IntPtr.Zero)
            {
                PTGetNextDevice = Marshal.GetDelegateForFunctionPointer<PassThruGetNextDevice>(pFunction);
                APISignature.SAE_API |= SAE_API.GETNEXTDEVICE;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruLogicalConnect");
            if (pFunction != IntPtr.Zero)
            {
                PTLogicalConnect = Marshal.GetDelegateForFunctionPointer<PassThruLogicalConnect>(pFunction);
                APISignature.SAE_API |= SAE_API.LOGICALCONNECT;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruLogicalDisconnect");
            if (pFunction != IntPtr.Zero)
            {
                PTLogicalDisconnect = Marshal.GetDelegateForFunctionPointer<PassThruLogicalDisconnect>(pFunction);
                APISignature.SAE_API |= SAE_API.LOGICALDISCONNECT;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruSelect");
            if (pFunction != IntPtr.Zero)
            {
                PTSelect = Marshal.GetDelegateForFunctionPointer<PassThruSelect>(pFunction);
                APISignature.SAE_API |= SAE_API.SELECT;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruQueueMsgs");
            if (pFunction != IntPtr.Zero)
            {
                PTQueueMsgs = Marshal.GetDelegateForFunctionPointer<PassThruQueueMsgs>(pFunction);
                APISignature.SAE_API |= SAE_API.QUEUEMESSAGES;
            }

            pFunction = Kernal32.GetProcAddress(pLibrary, "PassThruGetNextCarDAQ");
            if (pFunction != IntPtr.Zero)
            {
                PTGetNextCarDAQ = Marshal.GetDelegateForFunctionPointer<PassThruGetNextCarDAQ>(pFunction);
                APISignature.DREWTECH_API |= DrewTech_API.GETNEXTCARDAQ;
            }

            return APISignature;
        }
    }
}
