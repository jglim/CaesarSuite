#region License
/*Copyright(c) 2021, Brian Humlicek
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

namespace SAE.J2534
{
    public enum ProtocolInfo
    {
        MAX_RX_BUFFER_SIZE = 0x00000001,
        MAX_PASS_FILTER = 0x00000002,
        MAX_BLOCK_FILTER = 0x00000003,
        MAX_FILTER_MSG_LENGTH = 0x00000004,
        MAX_PERIODIC_MSGS = 0x00000005,
        MAX_PERIODIC_MSG_LENGTH = 0x00000006,
        DESIRED_DATA_RATE = 0x00000007,
        MAX_REPEAT_MESSAGING = 0x00000008,
        MAX_REPEAT_MESSAGING_LENGTH = 0x00000009,
        NETWORK_LINE_SUPPORTED = 0x0000000A,
        MAX_FUNCT_MSG_LOOKUP = 0x0000000B,
        PARITY_SUPPORTED = 0x0000000C,
        DATA_BITS_SUPPORTED = 0x0000000D,
        FIVE_BAUD_MOD_SUPPORTED = 0x0000000E,
        L_LINE_SUPPORTED = 0x0000000F,
        CAN_11_29_IDS_SUPPORTED = 0x00000010,
        CAN_MIXED_FORMAT_SUPPORTED = 0x00000011,
        MAX_FLOW_CONTROL_FILTER = 0x00000012,
        MAX_ISO15765_WFT_MAX = 0x00000013,
        MAX_AD_ACTIVE_CHANNELS = 0x00000014,
        MAX_AD_SAMPLE_RATE = 0x00000015,
        MAX_AD_SAMPLES_PER_READING = 0x00000016,
        AD_SAMPLE_RESOLUTION = 0x00000017,
        AD_INPUT_RANGE_LOW = 0x00000018,
        AD_INPUT_RANGE_HIGH = 0x00000019,
        RESOURCE_GROUP = 0x0000001A,
        TIMESTAMP_RESOLUTION = 0x0000001B
    }
}
