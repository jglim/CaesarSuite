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
    public partial class API : Common.UnmanagedDisposable
    {
        private object sync = new object();
        private IntPtr pLibrary;
        internal API_Signature APISignature { get; }
        internal API(string FileName)
        {
            APISignature = new API_Signature();

            pLibrary = Kernal32.LoadLibrary(FileName);

            APISignature = AssignDelegates();
        }
        /// <summary>
        /// Opens a J2534 device using this API
        /// </summary>
        /// <param name="DeviceName">The name of the device or blank to open the first available</param>
        /// <returns>Device object</returns>
        public Device GetDevice(string DeviceName = "")
        {
            using (var hDeviceID = new HeapInt())
            using (var hDeviceName = new HeapString(DeviceName))
            {
                lock (sync)
                {
                    CheckResult(PTOpen(String.IsNullOrWhiteSpace(DeviceName) ? IntPtr.Zero : (IntPtr)hDeviceName, (IntPtr)hDeviceID));
                }
                var device = new Device(this, hDeviceName.ToString(), hDeviceID.Value, sync);
                OnDisposing += device.Dispose;
                return device;
            }
        }
        /// <summary>
        /// Resets the GetNextCarDAQ enumerator
        /// <para>NOTE: Drewtech ONLY</para>
        /// </summary>
        public void GetNextCarDAQ_RESET()
        {
            lock (sync)
            {
                CheckResult(PTGetNextCarDAQ(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero));
            }
        }
        /// <summary>
        /// Enumerates through connected Drewtech devices
        /// <para>NOTE: Drewtech ONLY</para>
        /// </summary>
        /// <returns>GetNextCarDAQResults</returns>
        public GetNextCarDAQResults GetNextCarDAQ()
        {
            using (var hNamePtr = new HeapIntPtr())
            using (var hAddrPtr = new HeapIntPtr())
            using (var hVerPtr = new HeapIntPtr())
            {
                lock (sync)
                {
                    CheckResult(PTGetNextCarDAQ((IntPtr)hNamePtr, (IntPtr)hVerPtr, (IntPtr)hAddrPtr));
                }
                if (hNamePtr.Ptr == IntPtr.Zero) return null;

                byte[] b = new byte[3];
                Marshal.Copy((IntPtr)hVerPtr, b, 0, 3);

                return new GetNextCarDAQResults(Name:    Marshal.PtrToStringAnsi((IntPtr)hNamePtr),
                                                Version: $"{b[2]}.{b[1]}.{b[0]}",
                                                Address: Marshal.PtrToStringAnsi((IntPtr)hAddrPtr));
            }
        }
        internal void CheckResult(ResultCode Result)
        {
            if (Result.IsNotOK()) throw new J2534Exception(Result, GetLastError());
        }

        internal string GetLastError()  //Should never be called outside of API_LOCK
        {
            ResultCode Result;
            using(var hErrorDescription = new HeapString(80))
            {
                lock (sync)
                {
                    Result = PTGetLastError((IntPtr)hErrorDescription);
                    if (Result == ResultCode.STATUS_NOERROR)
                        return hErrorDescription.ToString();
                    else
                        return $"GetLastError failed with result: {Result.ToString()}";
                }
            }
        }
        protected override void DisposeUnmanaged()
        {
            Kernal32.FreeLibrary(pLibrary);
        }
    }
}
