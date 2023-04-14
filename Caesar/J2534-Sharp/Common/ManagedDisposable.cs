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

namespace Common
{
    public abstract class ManagedDisposable : IDisposable
    {
        private BoolInterlock DisposalInterlock { get; } = new BoolInterlock();

        public delegate void OnDisposingPrototype();
        /// <summary>
        /// Event that fires immediately before object disposal occurs.  Intended for long lived dependant object cleanup.
        /// </summary>
        public event OnDisposingPrototype OnDisposing;

        /// <summary>
        /// Returns true if Dispose() has been called 
        /// </summary>
        public bool IsDisposed
        {
            get { return DisposalInterlock.IsLocked; }
        }

        /// <summary>
        /// Checks if Dispose() has been called, and throws an ObjectDisposedxException() if it has
        /// </summary>
        protected void CheckDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().ToString());
        }

        /// <summary>
        /// Disposal implementation for managed objects should go in this method.
        /// </summary>
        protected abstract void DisposeManaged();

        protected void Dispose(bool Disposing)
        {
            if (DisposalInterlock.Enter())
            {
                CallDisposeMethods(Disposing);
            }
        }

        protected virtual void CallDisposeMethods(bool Disposing)
        {
            if (Disposing)
            {
                OnDisposing?.Invoke();
                DisposeManaged();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
