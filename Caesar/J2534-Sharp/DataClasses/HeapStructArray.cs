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
#endregion License
using System;
using System.Runtime.InteropServices;

namespace SAE.J2534
{
    internal abstract class HeapStructArray<T> : Common.UnmanagedDisposable
    {
        private readonly int elementSize;
        private readonly IntPtr firstElementPtr;
        public IntPtr Ptr { get; }
        private HeapStructArray(int Length)
        {
            elementSize = Marshal.SizeOf(typeof(T));
            //Create a blob big enough for all elements and two longs (NumOfItems and pItems)
            Ptr = Marshal.AllocHGlobal(Length * elementSize + 8);
            firstElementPtr = IntPtr.Add(Ptr, 8);
            this.Length = Length;
            //Write pItems.  To save complexity, the array immediately follows SConfigArray.
            Marshal.WriteIntPtr(Ptr, 4, firstElementPtr);
        }
        public HeapStructArray(T Element) : this(1)
        {
            setIndex(0, Element);
        }
        public HeapStructArray(T[] Elements) : this(Elements.Length)
        {
            //Write the array to the blob
            for (int i = 0; i < Elements.Length; i++)
                setIndex(i, Elements[i]);
        }
        public int Length
        {
            get { return Marshal.ReadInt32(Ptr); }
            private set { Marshal.WriteInt32(Ptr, value); }
        }
        public T this[int Index]
        {
            get
            {
                if (Index >= Length || Index < 0) throw new IndexOutOfRangeException("Index out of bounds HeapStructArrayPtr!");
                return getIndex(Index);
            }
            set
            {
                if (Index >= Length || Index < 0) throw new IndexOutOfRangeException("Index out of bounds HeapStructArrayPtr!");
                setIndex(Index, value);
            }
        }
        private T getIndex(int Index)
        {
            return Marshal.PtrToStructure<T>(IntPtr.Add(firstElementPtr, Index * elementSize));
        }
        private void setIndex(int Index, T Element)
        {
            Marshal.StructureToPtr<T>(Element, IntPtr.Add(firstElementPtr, Index * elementSize), false);
        }
        public T[] ToArray()
        {
            T[] result = new T[Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = getIndex(i);
            return result;
        }
        public static explicit operator IntPtr(HeapStructArray<T> HeapStructArray)
        {
            return HeapStructArray.Ptr;
        }
        protected override void DisposeUnmanaged()
        {
            Marshal.FreeHGlobal(Ptr);
        }
    }
}
