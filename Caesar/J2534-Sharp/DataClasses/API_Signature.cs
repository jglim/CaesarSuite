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
using System.Text;

namespace SAE.J2534
{
    internal class API_Signature
    {
        public API_Signature()
        {
            SAE_API = SAE_API.NONE;
            DREWTECH_API = DrewTech_API.NONE;
        }
        //This indicates what SAE standard API functions have been loaded
        public SAE_API SAE_API { get; set; }
        //This indicates if any drewtech specific API functions have been loaded
        public DrewTech_API DREWTECH_API { get; set; }
        public override string ToString()
        {
            StringBuilder API_String = new StringBuilder();
            if (DREWTECH_API != DrewTech_API.NONE)
                API_String.Append("DREWTECH ");
            else if (SAE_API != SAE_API.NONE)
                API_String.Append("SAE ");
            else
                API_String.Append("NO J2534 API DETECTED");

            switch (SAE_API)
            {
                case SAE_API.V202_SIGNATURE:
                    API_String.Append("J2534 v2.02");
                    break;
                case SAE_API.V404_SIGNATURE:
                    API_String.Append("J2534 v4.04");
                    break;
                case SAE_API.V500_SIGNATURE:
                    API_String.Append("J2534 v5.00");
                    break;
                default:
                    API_String.Append("UNKNOWN API");
                    break;
            }
            return API_String.ToString();
        }
    }
}
