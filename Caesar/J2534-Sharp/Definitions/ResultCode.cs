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
using System.ComponentModel;

namespace SAE.J2534
{
    public enum ResultCode
    {
        [Description("Function completed successfully.")]
        STATUS_NOERROR = 0x00,
        [Description("Function option is not supported.")]
        NOT_SUPPORTED = 0x01,
        [Description("Channel Identifier or handle was not recognized.")]
        INVALID_CHANNEL_ID = 0x02,
        [Description("Protocol identifier was not recognized.")]
        INVALID_PROTOCOL_ID = 0x03,
        [Description("NULL pointer presented as a function parameter address.")]
        NULL_PARAMETER = 0x04,
        [Description("IOCTL GET_CONFIG/SET_CONFIG parameter value is not recognized.")]
        INVALID_IOCTL_VALUE = 0x05,
        [Description("Flags bit field(s) contain(s) an invalid value.")]
        INVALID_FLAGS = 0x06,
        [Description("Unspecified error, use PassThruGetLastError for obtaining error text string.")]
        FAILED = 0x07,
        [Description("The PassThru device is not connected to the PC.")]
        DEVICE_NOT_CONNECTED = 0x08,
        [Description("The PassThru device was unable to read the specified number of messages from the vehicle network within the specified time.")]
        TIMEOUT = 0x09,
        [Description("Message contains a min/max Length, ExtraData support or J1850PWM specific source address conflict violation.")]
        INVALID_MSG = 0x0A,
        [Description("The time interval value is outside the allowed range.")]
        INVALID_TIME_INTERVAL = 0x0B,
        [Description("The limit (ten) of filter/periodic messages has been exceed. for the protocol associated with the communications channel.")]
        EXCEEDED_LIMIT = 0x0C,
        [Description("The message identifier or handle was not recognized.")]
        INVALID_MSG_ID = 0x0D,
        [Description("The specified PassThru device is already in use.")]
        DEVICE_IN_USE = 0x0E,
        [Description("IOCTL identifier is not recognized.")]
        INVALID_IOCTL_ID = 0x0F,
        [Description("The PassThru device could not read any messages from the vehicle network.")]
        BUFFER_EMPTY = 0x10,
        [Description("The PassThru device could not queue any more transmit messages destined for the vehicle network.")]
        BUFFER_FULL = 0x11,
        [Description("The PassThru device experienced a buffer overflow and receive messages were lost.")]
        BUFFER_OVERFLOW = 0x12,
        [Description("An unknown pin number specified for the J1962 connector, or the resource is already in use.")]
        PIN_INVALID = 0x13,
        [Description("An existing communications channel is currently using the specified network protocol.")]
        CHANNEL_IN_USE = 0x14,
        [Description("The specified protocol type within the message structure is different from the protocol associated with the communications channel when it was opened.")]
        MSG_PROTOCOL_ID = 0x15,
        [Description("Filter identifier is not recognized.")]
        INVALID_FILTER_ID = 0x16,
        [Description("No ISO15765 flow control filter matches the header of the outgoing message.")]
        NO_FLOW_CONTROL = 0x17,
        [Description("An existing filter already matches this header or node identifier.")]
        NOT_UNIQUE = 0x18,
        [Description("Unable to honor requested baud-rate within required tolerances.")]
        INVALID_BAUDRATE = 0x19,
        [Description("PassThru device identifier was not recognized.")]
        INVALID_DEVICE_ID = 0x1A,
        [Description("Invalid IOCTL Parameter ID (either in the reserved range or not appropriate for the current channel)")]
        ERR_INVALID_IOCTL_PARAM_ID = 0x1E,
        [Description("Programming voltage is currently being applied to another pin")]
        ERR_VOLTAGE_IN_USE = 0x1F,
        [Description("Pin number specified is currently in use(another protocol is using the pin)")]
        ERR_PIN_IN_USE = 0x20,
        [Description("The API call was not mapped to a function in the PassThru DLL.")]
        FUNCTION_NOT_ASSIGNED = 0x7EADBEEF  //non-standard flag used by the wrapper to indicate no function assigned
    }
    public static class ResultCodeExtensions
    {
        public static bool IsOK(this ResultCode code)
        {
            return code == ResultCode.STATUS_NOERROR;
        }
        public static bool IsNotOK(this ResultCode code)
        {
            return code != ResultCode.STATUS_NOERROR;
        }
        public static string GetDescription(this ResultCode code)
        {
            return Common.EnumHelper.GetMemberDescription(code);
        }
    }
}
