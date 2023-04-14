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
    public enum Baud
    {
        [Description("10.4kbps")]
        ISO9141 = 10400,
        [Description("10.4kbps")]
        ISO9141_10400 = 10400,
        [Description("10kbps")]
        ISO9141_10000 = 10000,

        [Description("10.4kbps")]
        ISO14230 = 10400,
        [Description("10.4kbps")]
        ISO14230_10400 = 10400,
        [Description("10kbps")]
        ISO14230_10000 = 10000,

        [Description("41.6kbps")]
        J1850PWM = 41600,
        [Description("41.6kbps")]
        J1850PWM_41600 = 41600,
        [Description("83.2kbps")]
        J1850PWM_83200 = 83200,

        [Description("10.4kbps")]
        J1850VPW = 10400,
        [Description("10.4kbps")]
        J1850VPW_10400 = 10400,
        [Description("41.6kbps")]
        J1850VPW_41600 = 41600,

        [Description("500kbps")]
        CAN = 500000,
        [Description("125kbps")]
        CAN_125000 = 125000,
        [Description("250kbps")]
        CAN_250000 = 250000,
        [Description("500kbps")]
        CAN_500000 = 500000,

        [Description("500kbps")]
        ISO15765 = 500000,
        [Description("125kbps")]
        ISO15765_125000 = 125000,
        [Description("250kbps")]
        ISO15765_250000 = 250000,
        [Description("500kbps")]
        ISO15765_500000 = 500000,

        [Description("7812bps")]
        SCI_7812 = 7812,
        [Description("62.5kbps")]
        SCI_62500 = 62500
    }
    public static class BaudExtensions
    {
        public static string GetDescription(this Baud baud)
        {
            return Common.EnumHelper.GetMemberDescription(baud);
        }
    }
}
