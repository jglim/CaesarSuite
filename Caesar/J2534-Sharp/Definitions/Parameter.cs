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

namespace SAE.J2534
{
    public enum Parameter
    {
        DATA_RATE = 0x01,
        LOOP_BACK = 0x03,
        NODE_ADDRESS = 0x04,
        NETWORK_LINE = 0x05,
        P1_MIN = 0x06,
        P1_MAX = 0x07,
        P2_MIN = 0x08,
        P2_MAX = 0x09,
        P3_MIN = 0x0A,
        P3_MAX = 0x0B,
        P4_MIN = 0x0C,
        P4_MAX = 0x0D,
        W0 = 0x19,
        W1 = 0x0E,
        W2 = 0x0F,
        W3 = 0x10,
        W4 = 0x11,
        W5 = 0x12,
        TIDLE = 0x13,
        TINIL = 0x14,
        TWUP = 0x15,
        PARITY = 0x16,
        BIT_SAMPLE_POINT = 0x17,
        SYNC_JUMP_WIDTH = 0x18,
        T1_MAX = 0x1A,
        T2_MAX = 0x1B,
        T3_MAX = 0x24,
        T4_MAX = 0x1C,
        T5_MAX = 0x1D,
        ISO15765_BS = 0x1E,
        ISO15765_STMIN = 0x1F,
        DATA_BITS = 0x20,
        FIVE_BAUD_MOD = 0x21,
        BS_TX = 0x22,
        STMIN_TX = 0x23,
        ISO15765_WFT_MAX = 0x25,

        //J2534-2
        CAN_MIXED_FORMAT = 0x00008000,
        J1962_PINS = 0x00008001,
        SW_CAN_HS_DATA_RATE = 0x00008010,
        SW_CAN_SPEEDCHANGE_ENABLE = 0x00008011,
        SW_CAN_RES_SWITCH = 0x00008012,
        ACTIVE_CHANNELS = 0x00008020,
        SAMPLE_RATE = 0x00008021,
        SAMPLES_PER_READING = 0x00008022,
        READINGS_PER_MSG = 0x00008023,
        AVERAGING_METHOD = 0x00008024,
        SAMPLE_RESOLUTION = 0x00008025,
        INPUT_RANGE_LOW = 0x00008026,
        INPUT_RANGE_HIGH = 0x00008027,
        UEB_T0_MIN = 0x00008028,
        UEB_T1_MAX = 0x00008029,
        UEB_T2_MAX = 0x0000802A,
        UEB_T3_MAX = 0x0000802B,
        UEB_T4_MIN = 0x0000802C,
        UEB_T5_MAX = 0x0000802D,
        UEB_T6_MAX = 0x0000802E,
        UEB_T7_MIN = 0x0000802F,
        UEB_T7_MAX = 0x00008030,
        UEB_T9_MIN = 0x00008031,
        J1939_PINS = 0x0000803D,
        J1708_PINS = 0x0000803E,
        J1939_T1 = 0x0000803F,
        J1939_T2 = 0x00008040,
        J1939_T3 = 0x00008041,
        J1939_T4 = 0x00008042,
        J1939_BRDCST_MIN_DELAY = 0x00008043,
        TP2_0_T_BR_INT = 0x00008044,
        TP2_0_T_E = 0x00008045,
        TP2_0_MNTC = 0x00008046,
        TP2_0_T_CTA = 0x00008047,
        TP2_0_MNCT = 0x00008048,
        TP2_0_MNTB = 0x00008049,
        TP2_0_MNT = 0x0000804A,
        TP2_0_T_WAIT = 0x0000804B,
        TP2_0_T1 = 0x0000804C,
        TP2_0_T3 = 0x0000804D,
        TP2_0_IDENTIFER = 0x0000804E,
        TP2_0_RXIDPASSIVE = 0x0000804F,
        FD_CAN_DATA_PHASE_RATE = 0x0000805C,
        FD_ISO15765_TX_DATA_LENGTH = 0x0000805D,
        HS_CAN_TERMINATION = 0x0000805E,
        N_CR_MAX = 0x0000805F,
        ISO15765_PAD_VALUE = 0x00008060,

        ISO15765_SIMULTANEOUS = 0x10000000, //Drewtech
        DT_ISO15765_PAD_BYTE = 0x10000001,  //Drewtech
        ADC_READINGS_PER_SECOND = 0x10000,  //Drewtech
        ADC_READINGS_PER_SAMPLE = 0x20000   //Drewtech
    }
}
