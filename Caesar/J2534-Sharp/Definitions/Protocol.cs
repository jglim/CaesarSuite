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
using System.ComponentModel;

namespace SAE.J2534
{
    public enum Protocol
    {
        [Description("SAE J1850 VPW")]
        J1850VPW = 0x01,
        [Description("SAE J1850 PWM")]
        J1850PWM = 0x02,
        [Description("ISO 9141")]
        ISO9141 = 0x03,
        [Description("ISO 14229")]
        ISO14230 = 0x04,
        [Description("CAN")]
        CAN = 0x05,
        [Description("ISO 15765")]
        ISO15765 = 0x06,
        [Description("SCI-A Engine")]
        SCI_A_ENGINE = 0x07,
        [Description("SCI-A Transmission")]
        SCI_A_TRANS = 0x08,
        [Description("SCI-B Engine")]
        SCI_B_ENGINE = 0x09,
        [Description("SCI-B Transmission")]
        SCI_B_TRANS = 0x0A,

        //J2534-2 Protocol definitions
        J1850VPW_PS = 0x00008000,
        J1850PWM_PS = 0x00008001,
        ISO9141_PS = 0x00008002,
        ISO14230_PS = 0x00008003,
        CAN_PS = 0x00008004,
        ISO15765_PS = 0x00008005,
        J2610_PS = 0x00008006,
        SW_ISO15765_PS = 0x00008007,
        SW_CAN_PS = 0x00008008,
        GM_UART_PS = 0x00008009,
        UART_ECHO_BYTE_PS = 0x0000800A,
        HONDA_DIAGH_PS = 0x0000800B,
        J1939_PS = 0x0000800C,
        J1708_PS = 0x0000800D,
        TP2_0_PS = 0x0000800E,
        FT_CAN_PS = 0x0000800F,
        FT_ISO15765_PS = 0x00008010,
        FD_CAN_PS = 0x00008011,
        FD_ISO15765_PS = 0x00008012,
        CAN_CH1 = 0x00009000,
        J1850VPW_CH1 = 0x00009080,
        J1850PWM_CH1 = 0x00009100,
        ISO9141_CH1 = 0x00009180,
        ISO14230_CH1 = 0x00009200,
        ISO15765_CH1 = 0x00009280,
        SW_CAN_CAN_CH1 = 0x00009300,
        SW_CAN_ISO15765_CH1 = 0x00009380,
        J2610_CH1 = 0x00009400,
        FT_CAN_CH1 = 0x00009480,
        FT_ISO15765_CH1 = 0x00009500,
        GM_UART_CH1 = 0x00009580,
        ECHO_BYTE_CH1 = 0x00009600,
        HONDA_DIAGH_CH1 = 0x00009680,
        J1939_CH1 = 0x00009700,
        J1708_CH1 = 0x00009780,
        TP2_0_CH1 = 0x00009800,
        FD_CAN_CH1 = 0x00009880,
        FD_ISO15765_CH1 = 0x00009900,
        ANALOG_IN_1 = 0x0000C000,
    }
    public static class ProtocolExtensions
    {
        public static string GetDescription(this Protocol protocol)
        {
            return Common.EnumHelper.GetMemberDescription(protocol);
        }
    }
}
