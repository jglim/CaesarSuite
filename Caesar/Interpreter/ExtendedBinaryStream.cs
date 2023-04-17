using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CaesarInterpreter
{
    public class ExtendedBinaryStream
    {
        // using (BinaryReader reader = new BinaryReader(new MemoryStream(dscContainerBytes, 0, dscContainerBytes.Length, true, true)))
        MemoryStream ByteStream;
        public BinaryReader Reader;
        public BinaryWriter Writer;
        byte[] UnderlyingBuffer;

        public ExtendedBinaryStream(byte[] content)
        {
            UnderlyingBuffer = content;
            ByteStream = new MemoryStream(content, 0, content.Length, true, true);
            Reader = new BinaryReader(ByteStream);
            Writer = new BinaryWriter(ByteStream);
        }

        public byte[] GetUnderlyingBuffer() 
        {
            return UnderlyingBuffer;
        }

        public int Seek(int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            return (int)ByteStream.Seek((long)offset, origin);
        }

        public int Position { 
            get { return (int)ByteStream.Position; } 
            set { ByteStream.Position = value; }  
        }

        public void WriteI8(sbyte i8, bool advance = true)
        {
            long pos = ByteStream.Position;
            Writer.Write(i8);
            if (!advance) 
            {
                ByteStream.Position = pos;
            }
        }
        public void WriteU8(byte u8, bool advance = true)
        {
            long pos = ByteStream.Position;
            Writer.Write(u8);
            if (!advance)
            {
                ByteStream.Position = pos;
            }
        }
        public void WriteI16(short i16, bool advance = true)
        {
            long pos = ByteStream.Position;
            Writer.Write(i16);
            if (!advance)
            {
                ByteStream.Position = pos;
            }
        }
        public void WriteU16(ushort u16, bool advance = true)
        {
            long pos = ByteStream.Position;
            Writer.Write(u16);
            if (!advance)
            {
                ByteStream.Position = pos;
            }
        }
        public void WriteI32(int i32, bool advance = true)
        {
            long pos = ByteStream.Position;
            Writer.Write(i32);
            if (!advance)
            {
                ByteStream.Position = pos;
            }
        }
        public void WriteU32(uint u32, bool advance = true)
        {
            long pos = ByteStream.Position;
            Writer.Write(u32);
            if (!advance)
            {
                ByteStream.Position = pos;
            }
        }

        public void WriteFloat(float f32, bool advance = true)
        {
            long pos = ByteStream.Position;
            Writer.Write(f32);
            if (!advance)
            {
                ByteStream.Position = pos;
            }
        }

        public void WriteBytes(byte[] bytes, bool advance = true)
        {
            long pos = ByteStream.Position;
            Writer.Write(bytes);
            if (!advance)
            {
                ByteStream.Position = pos;
            }
        }

        public sbyte ReadI8()
        {
            return Reader.ReadSByte();
        }
        public byte ReadU8()
        {
            return Reader.ReadByte();
        }
        public short ReadI16()
        {
            return Reader.ReadInt16();
        }
        public ushort ReadU16()
        {
            return Reader.ReadUInt16();
        }
        public int ReadI32()
        {
            return Reader.ReadInt32();
        }
        public uint ReadU32()
        {
            return Reader.ReadUInt32();
        }

        public byte[] ReadBytes(int count) 
        {
            return Reader.ReadBytes(count);
        }

        public float ReadFloat() 
        {
            return Reader.ReadSingle();
        }

        public char PeekI8()
        {
            long pos = ByteStream.Position;
            var result = Reader.ReadChar();
            ByteStream.Position = pos;
            return result;
        }
        public byte PeekU8()
        {
            long pos = ByteStream.Position;
            var result = Reader.ReadByte();
            ByteStream.Position = pos;
            return result;
        }
        public short PeekI16()
        {
            long pos = ByteStream.Position;
            var result = Reader.ReadInt16();
            ByteStream.Position = pos;
            return result;
        }
        public ushort PeekU16()
        {
            long pos = ByteStream.Position;
            var result = Reader.ReadUInt16();
            ByteStream.Position = pos;
            return result;
        }
        public int PeekI32()
        {
            long pos = ByteStream.Position;
            var result = Reader.ReadInt32();
            ByteStream.Position = pos;
            return result;
        }
        public uint PeekU32()
        {
            long pos = ByteStream.Position;
            var result = Reader.ReadUInt32();
            ByteStream.Position = pos;
            return result;
        }

        public byte[] PeekBytes(int count)
        {
            long pos = ByteStream.Position;
            var result = Reader.ReadBytes(count);
            ByteStream.Position = pos;
            return result;
        }
    }
}
