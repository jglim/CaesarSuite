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
    public abstract class UnmanagedDisposable : ManagedDisposable
    {
        /// <summary>
        /// Disposal implementation for managed objects should go in this method.
        /// </summary>
        protected override void DisposeManaged() { }    //Optional implementation for the superclass

        /// <summary>
        /// Disposal implementation for unmanaged objects should go in this method.
        /// </summary>
        protected abstract void DisposeUnmanaged(); //Super class MUST implement this

        protected sealed override void CallDisposeMethods(bool Disposing)
        {
            base.CallDisposeMethods(Disposing); //Must dispose of managed objects first
            DisposeUnmanaged();
            if (Disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        ~UnmanagedDisposable()
        {
            try
            {
                Dispose(false);
            }
            catch (Exception exception)
            {
                //This is bad.  At least attempt to get a log message out if this happens.
                try
                {
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();
                    builder.AppendLine($"{DateTime.Now} - Exception in type '{GetType()}'");
                    builder.Append(exception.StackTrace);
                    builder.Append(exception.Message);
                    var inner_exception = exception.InnerException;
                    while (inner_exception != null)
                    {
                        builder.Append(inner_exception.Message);
                        inner_exception = inner_exception.InnerException;
                    }
                    System.IO.File.AppendAllText(@"FinalizerException.txt", builder.ToString());
                }
                catch { }   //Swallow any exceptions inside a finalizer
            }
        }
    }
}
