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
using System.Collections.Generic;

namespace SAE.J2534
{
    public partial class API
    {
        /// <summary>
        /// Gets a list of discoverable devices
        /// <para>NOTE: Only supported by v5.00API or Drewtech</para>
        /// </summary>
        /// <returns></returns>
        public List<string> GetDeviceList()
        {
            List<string> result = new List<string>();
            //Need to make some special sauce to get from registry here
            if (APISignature.SAE_API == SAE_API.V500_SIGNATURE)
            {
                throw new System.NotImplementedException("J2534 v5 support is not complete!");
            }
            else if (APISignature.DREWTECH_API.HasFlag(DrewTech_API.GETNEXTCARDAQ))
            {
                GetNextCarDAQ_RESET();
                for (GetNextCarDAQResults DrewtechDevice = GetNextCarDAQ();
                     DrewtechDevice != null;
                     DrewtechDevice = GetNextCarDAQ())
                {
                    result.Add(DrewtechDevice.Name);
                }

            }
            return result;
        }
    }
}
