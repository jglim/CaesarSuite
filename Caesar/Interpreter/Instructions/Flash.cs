using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class Flash
    {

        public static void MLDoDownload(Interpreter ih)
        {
            // writes flash data given a specified block index
            switch (ih.Opcode)
            {
                case 0x3D1:
                    {
                        ih.Stack.Seek(-4);
                        int blockIndex = ih.Stack.PeekI32();

                        // since a block index is specified, set this as our active block
                        // download, getchecksum, getsignature do not specify a datablock, they assume that a working block is already "set"
                        ih.HostFlash.SetActiveBlockIndex(blockIndex);

                        // actually start transferring
                        ih.HostFlash.Download(blockIndex);

                        ih.ActiveStep.AddDescription($"MLDoDownload: {blockIndex:X8}");
                        break;
                    }
            }
        }

        public static void MLGetDatablockChecksum(Interpreter ih)
        {
            // checks if the CFF datablock specifies a checksum, if so, copy it into a buffer
            // doesn't specify a block index, so this needs to be tracked at MLDoDownload
            switch (ih.Opcode)
            {
                case 0x3D5:
                    {
                        ih.Stack.Seek(-4);
                        int crcLengthBuffer = ih.Stack.PeekI32(); // Writing pointer of lCRCLength to stack: 0x202E0000

                        byte[] checksum = ih.HostFlash.GetChecksum(); //new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

                        int csLength = checksum.Length;

                        // write cs length
                        InterpreterMemory.SetMemoryAtAddress(ih, crcLengthBuffer, BitConverter.GetBytes(csLength));

                        if (csLength > 0)
                        {
                            // write cs buffer
                            Buffer csContent = new Buffer(ih, $"Alloc by MLGetDatablockChecksum at {ih.Opcode:X4}");
                            csContent.ContentBytes = checksum;
                            ih.Stack.WriteI32(csContent.GetPointer());
                        }
                        else
                        {
                            ih.Stack.WriteI32(0); // null
                        }

                        ih.ActiveStep.AddDescription($"MLGetDatablockChecksum: length buffer {crcLengthBuffer:X8}, output length: {csLength}u");
                        break;
                    }
            }
        }

        public static void GetDatablockSecuritySignature(Interpreter ih)
        {
            // see MLGetDatablockChecksum, also requires block tracking
            switch (ih.Opcode)
            {
                case 0x3D7:
                    {
                        ih.Stack.Seek(-4);
                        int signatureLengthBuffer = ih.Stack.PeekI32(); // Writing pointer of lSignatureLength to stack: 0x202F0000

                        // 204: expects 0x80 bytes
                        byte[] signature = ih.HostFlash.GetSignature(); // new byte[0x80];

                        int sigLength = signature.Length;

                        // write cs length
                        InterpreterMemory.SetMemoryAtAddress(ih, signatureLengthBuffer, BitConverter.GetBytes(sigLength));

                        if (sigLength > 0)
                        {
                            // write cs buffer
                            Buffer content = new Buffer(ih, $"Alloc by GetDatablockSecuritySignature at {ih.Opcode:X4}");
                            content.ContentBytes = signature;
                            ih.Stack.WriteI32(content.GetPointer());
                        }
                        else
                        {
                            ih.Stack.WriteI32(0); // null
                        }

                        ih.ActiveStep.AddDescription($"GetDatablockSecuritySignature: length buffer {signatureLengthBuffer:X8}, output length: {sigLength}u");
                        break;
                    }
            }
        }

        public static void MLOpenSessionDataBlockByIndex(Interpreter ih)
        {
            // not very sure on why this is required
            // seems to open a handle to a flashdatablock for use later in DIGetDataBlockNumberOfSecurities
            // doesn't require the host to do anything
            switch (ih.Opcode)
            {
                case 0x3F3:
                    {
                        ih.Stack.Seek(-4);
                        int looksLikeAPtrToTrackedObj = ih.Stack.PeekI32(); // fixme: wtf ; param doesn't seem useful?
                        // ptr points to stack

                        ih.Stack.Seek(-4);
                        int blockIndex = ih.Stack.PeekI32();

                        FlashSessionDataBlock fdb = new FlashSessionDataBlock(ih);
                        fdb.BlockIndex = blockIndex;

                        int fdbPointer = fdb.GetPointer();

                        byte[] somebufVal = InterpreterMemory.GetMemoryAtAddress(ih, looksLikeAPtrToTrackedObj, 8);
                        int ptrValue = BitConverter.ToInt32(somebufVal, 0);
                        //var obj = InterpreterMemory.GetTrackedObjectAtAddress(ih, ptrValue);

                        ih.ActiveStep.AddDescription($"MLOpenSessionDataBlockByIndex: somebuffer: {looksLikeAPtrToTrackedObj:X8} {BitConverter.ToString(somebufVal)} blockindex: {blockIndex:X8}, fdbptr: {fdbPointer:X8}");

                        ih.Stack.WriteI32(fdbPointer);
                        // Console.ReadKey();
                        break;
                    }
            }
        }
        public static void DIGetDataBlockNumberOfSecurities(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3F6:
                    {
                        ih.Stack.Seek(-4);
                        int dataBlockPointer = ih.Stack.PeekI32();
                        FlashSessionDataBlock flashDataBlock = InterpreterMemory.GetTrackedObjectAtAddress(ih, dataBlockPointer) as FlashSessionDataBlock;
                        int securitiesCount = ih.HostFlash.GetNumberOfSecurities(flashDataBlock.BlockIndex); // 2;
                        
                        ih.Stack.WriteI32(securitiesCount);
                        ih.ActiveStep.AddDescription($"DIGetDataBlockNumberOfSecurities for block {flashDataBlock.BlockIndex}: {securitiesCount}");
                        break;
                    }
            }
        }
    }
}
