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
        //Shared prototypes
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruConnect(int DeviceID, int ProtocolID, int ConnectFlags, int Baud, IntPtr ChannelID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruDisconnect(int channelId);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruReadMsgs(int ChannelID, IntPtr pUMsgArray, IntPtr NumMsgs, int Timeout);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruWriteMsgs(int ChannelID, IntPtr pUMsgArray, IntPtr NumMsgs, int Timeout);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruStartPeriodicMsg(int ChannelID, IntPtr Msg, IntPtr MsgID, int Interval);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruStopPeriodicMsg(int ChannelID, int MsgID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruStartMsgFilter
        (
            int ChannelID,
            int FilterType,
            IntPtr pMaskMsg,
            IntPtr PatternMsg,
            IntPtr pFlowControlMsg,
            IntPtr FilterID
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruStopMsgFilter(int ChannelID, int FilterID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruSetProgrammingVoltage(int DeviceID, int Pin, int Voltage);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruReadVersion(int DeviceID, IntPtr pFirmwareVer, IntPtr pDllVer, IntPtr pAPIVer);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruGetLastError(IntPtr pErr);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruIoctl(int HandleID, int IOCtlID, IntPtr Input, IntPtr Output);

        //v2.02 ONLY
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruConnectv202(int ProtocolID, int ConnectFlags, IntPtr ChannelID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruSetProgrammingVoltagev202(int Pin, int Voltage);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruReadVersionv202(IntPtr pFirmwareVer, IntPtr pDllVer, IntPtr pAPIVer);
        //v4.04
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruOpen(IntPtr pDeviceName, IntPtr DeviceID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruClose(int DeviceID);
        //v5.00 and undocumented drewtech calls
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruScanForDevices(ref int DeviceCount);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruGetNextDevice(IntPtr pSDevice);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruLogicalConnect(int PhysicalChannelID, int ProtocolID, int ConnectFlags, IntPtr ChannelDescriptor, ref int pChannelID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruLogicalDisconnect(int pChannelID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruSelect(IntPtr pSChannelSet, int SelectType, int Timeout);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruQueueMsgs(int ChannelID, IntPtr pMsgArray, ref int NumMsgs);
        //Drewtech ONLY
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ResultCode PassThruGetNextCarDAQ(IntPtr pName, IntPtr pVer, IntPtr pAddress);
    }
}
