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
using System.Threading;

namespace Common
{
    public class BoolInterlock
    {
        private int state = States.Unlocked;

        public bool IsUnlocked
        {
            get { return state == States.Unlocked; }
        }

        public bool IsLocked
        {
            get { return state == States.Locked; }
        }

        public bool Enter()
        {
            //Set state to Locked, and return the original state
            return Interlocked.Exchange(ref state, States.Locked) == States.Unlocked;
        }
        public void Exit()
        {
            state = States.Unlocked;
        }
        //Can't be an Enum due to being passed by ref in the Exchange() method
        private static class States
        {
            public const int Unlocked = 0;
            public const int Locked = 1;
        }
    }
}
