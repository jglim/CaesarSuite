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
using System.Runtime.InteropServices;

namespace SAE.J2534
{
    internal class HeapSByteArray : Common.UnmanagedDisposable
    {
        public IntPtr Ptr { get; }
        public HeapSByteArray(byte Byte)
        {
            Ptr = Marshal.AllocHGlobal(9);
            Length = 1;
            Marshal.WriteIntPtr(Ptr, 4, IntPtr.Add(Ptr, 8));
            Marshal.WriteByte(IntPtr.Add(Ptr, 8), Byte);
        }
        public HeapSByteArray(byte[] SByteArray)
        {
            Ptr = Marshal.AllocHGlobal(SByteArray.Length + 8);
            Length = SByteArray.Length;
            Marshal.WriteIntPtr(Ptr, 4, IntPtr.Add(Ptr, 8));
            Marshal.Copy(SByteArray, 0, IntPtr.Add(Ptr, 8), SByteArray.Length);
        }
        public int Length
        {
            get { return Marshal.ReadInt32(Ptr); }
            private set { Marshal.WriteInt32(Ptr, value); }
        }

        public byte this[int Index]
        {
            get
            {
                if (Index >= Length || Index < 0) throw new IndexOutOfRangeException("Index is greater than array bound");
                return Marshal.ReadByte(IntPtr.Add(Ptr, Index + 8));                
            }
        }
        public byte[] ToSByteArray()
        {
            byte[] result = new byte[Length];
            Marshal.Copy(IntPtr.Add(Ptr, 8), result, 0, result.Length);
            return result;

        }
        public static explicit operator IntPtr(HeapSByteArray HeapSByteArray)
        {
            return HeapSByteArray.Ptr;
        }
        protected override void DisposeUnmanaged()
        {
            Marshal.FreeHGlobal(Ptr);
        }
    }
}
