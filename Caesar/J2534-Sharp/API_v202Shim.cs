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
using System.Runtime.InteropServices;

namespace SAE.J2534
{
    public partial class API
    {
        private string shim_DeviceName = String.Empty;
        private int shim_DeviceID = 0;
        private bool shim_IsOpen = false;
        private ResultCode Open_shim(IntPtr pDeviceName, IntPtr pDeviceID)
        {
            string DeviceName = pDeviceName == IntPtr.Zero ? String.Empty : Marshal.PtrToStringAnsi(pDeviceName);
            if (!shim_IsOpen)
            {
                shim_DeviceName = DeviceName;
                shim_IsOpen = true;
                return ResultCode.STATUS_NOERROR;
            }
            if (shim_IsOpen && (DeviceName == this.shim_DeviceName)) return ResultCode.DEVICE_IN_USE;

            return ResultCode.INVALID_DEVICE_ID;
        }

        private ResultCode Close_shim(int DeviceID)
        {
            if (!shim_IsOpen || DeviceID != shim_DeviceID) return ResultCode.INVALID_DEVICE_ID;
            return ResultCode.STATUS_NOERROR;
        }

        private ResultCode Connect_shim(int DeviceID, int ProtocolID, int ConnectFlags, int Baud, IntPtr ChannelID)
        {
            if (DeviceID != shim_DeviceID) return ResultCode.INVALID_DEVICE_ID;
            return PTConnectv202(ProtocolID, ConnectFlags, ChannelID);
        }

        private ResultCode SetVoltage_shim(int DeviceID, int Pin, int Voltage)
        {
            if (DeviceID != shim_DeviceID) return ResultCode.INVALID_DEVICE_ID;
            return PTSetProgrammingVoltagev202(Pin, Voltage);
        }

        private ResultCode ReadVersion_shim(int DeviceID, IntPtr pFirmwareVer, IntPtr pDllVer, IntPtr pAPIVer)
        {
            if (DeviceID != shim_DeviceID) return ResultCode.INVALID_DEVICE_ID;
            return PTReadVersionv202(pFirmwareVer, pDllVer, pAPIVer);
        }
    }
}
