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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace SAE.J2534
{
    public class HeapMessageArray : Common.UnmanagedDisposable
    {
        private int protocolID;
        private int array_max_length;
        public IntPtr MessagePtr { get; }
        public IntPtr LengthPtr { get; }

        public IntPtr Ptr { get { return MessagePtr; } }

        public HeapMessageArray(Protocol ProtocolID, int Length)
        {
            if (Length < 1) throw new ArgumentException("Length must be at least 1 (HEAPMessageArray");
            array_max_length = Length;
            LengthPtr = Marshal.AllocHGlobal(CONST.J2534MESSAGESIZE * Length + 4);
            MessagePtr = IntPtr.Add(LengthPtr, 4);
            protocolID = (int)ProtocolID;
        }
        public int Length
        {
            get
            {
                return Marshal.ReadInt32(LengthPtr);
            }
            set
            {
                if (value > array_max_length) throw new IndexOutOfRangeException("Length is greater than array bound (HEAPMessageArray)");
                Marshal.WriteInt32(LengthPtr, value);
            }
        }
        public Message this[int index]
        {
            get
            {
                if (index >= array_max_length || index < 0) throw new IndexOutOfRangeException("Index out of range in J2534HeapMessageArray get[]");
                return getIndex(index);
            }
            set
            {
                if (index >= array_max_length || index < 0) throw new IndexOutOfRangeException("Index out of range in J2534HeapMessageArray set[]");
                setIndex(index, value);
            }
        }
        private Message getIndex(int index)
        {
            IntPtr pMessage = IntPtr.Add(MessagePtr, index * CONST.J2534MESSAGESIZE);
            return new Message(marshalHeapDataToArray(pMessage),
                                    (RxFlag)Marshal.ReadInt32(pMessage, 4),
                                    (uint)Marshal.ReadInt32(pMessage, 12));
        }
        private void setIndex(int index, Message Message)
        {
            IntPtr pMessage = IntPtr.Add(MessagePtr, index * CONST.J2534MESSAGESIZE);
            Marshal.WriteInt32(pMessage, protocolID);
            Marshal.WriteInt32(pMessage, 8, Message.FlagsAsInt);
            marshalIEnumerableToHeapData(pMessage, Message.Data);
        }
        private byte[] marshalHeapDataToArray(IntPtr pData)
        {
            int Length = Marshal.ReadInt32(pData, 16);
            byte[] data = new byte[Length];
            Marshal.Copy(IntPtr.Add(pData, 24), data, 0, Length);
            return data;
        }
        private void marshalIEnumerableToHeapData(IntPtr pMessage, IEnumerable<byte> Data)
        {
            if (Data is byte[])  //Byte[] is fastest
            {
                var DataAsArray = (byte[])Data;
                Marshal.WriteInt32(pMessage, 16, DataAsArray.Length);
                Marshal.Copy(DataAsArray, 0, IntPtr.Add(pMessage, 24), DataAsArray.Length);
            }
            else if (Data is ArraySegment<byte>)
            {
                var DataAsArraySegment = (ArraySegment<byte>)Data;
                Marshal.WriteInt32(pMessage, 16, DataAsArraySegment.Count);
                Marshal.Copy(DataAsArraySegment.Array, DataAsArraySegment.Offset, IntPtr.Add(pMessage, 24), DataAsArraySegment.Count);
            }
            else if (Data is IList<byte>)   //Collection with indexer is second best
            {
                var DataAsList = (IList<byte>)Data;
                int length = DataAsList.Count;
                IntPtr Ptr = IntPtr.Add(pMessage, 24);  //Offset to data array
                Marshal.WriteInt32(pMessage, 16, length);
                for (int indexer = 0; indexer < length; indexer++)
                {
                    Marshal.WriteByte(Ptr, indexer, DataAsList[indexer]);
                }
            }
            else if (Data is IEnumerable<byte>)    //Enumerator is third
            {
                IntPtr Ptr = IntPtr.Add(pMessage, 24);  //Offset to data array
                int index_count = 0;
                foreach (byte b in Data)
                {
                    Marshal.WriteByte(Ptr, index_count, b);
                    index_count++;
                }
                Marshal.WriteInt32(pMessage, 16, index_count);  //Set length
            }
        }
        public void FromMessage(Message Message)
        {
            Length = 1;
            setIndex(0, Message);
        }
        public Message ToMessage()
        {
            return getIndex(0);
        }
        public Message[] ToMessageArray()
        {
            int length = Length;
            Message[] result = new Message[length];
            for (int i = 0; i < length; i++)
                result[i] = getIndex(i);
            return result;
        }
        public void FromMessageArray(Message [] Messages)
        {
            Length = Messages.Length;
            for (int i = 0; i < Messages.Length; i++)
            {
                setIndex(i, Messages[i]);
            }
        }
        public void FromDataBytes(IEnumerable<byte> Data, TxFlag TxFlags = TxFlag.NONE)
        {
            Length = 1;
            Marshal.WriteInt32(MessagePtr, protocolID);
            Marshal.WriteInt32(MessagePtr, 8, (int)TxFlags);
            marshalIEnumerableToHeapData(MessagePtr, Data);
        }
        public void FromDataBytesArray(IEnumerable<byte>[] Data, TxFlag TxFlags = TxFlag.NONE)
        {
            Length = Data.Length;
            for (int i = 0; i < Data.Length; i++)
            {
                IntPtr pMessage = IntPtr.Add(MessagePtr, i * CONST.J2534MESSAGESIZE);
                Marshal.WriteInt32(pMessage, protocolID);
                Marshal.WriteInt32(pMessage, 8, (int)TxFlags);
                marshalIEnumerableToHeapData(pMessage, Data[i]);
            }
        }
        public static explicit operator IntPtr(HeapMessageArray HeapMessageArray)
        {
            return HeapMessageArray.Ptr;
        }
        protected override void DisposeUnmanaged()
        {
            Marshal.FreeHGlobal(LengthPtr);
        }
    }
}
