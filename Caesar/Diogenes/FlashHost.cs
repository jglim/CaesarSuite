using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes
{
    public class FlashHost : CaesarInterpreter.Host.IFlash
    {
        int ActiveBlockIndex = -1;
        FlashItem FlashData = null;

        public Action FlashProgressChanged = null;

        public int FlashProgress = 0;
        public int SegmentsDownloaded = 0;
        public int SegmentsTotal = 0;
        public string SegmentName = "";
        public string BlockName = "";

        public FlashHost(FlashItem fi)
        {
            FlashData = fi;
        }

        public void Download(int blockIndex)
        {
            // i think we are supposed to check whether CPI_GPDAUTODOWNLOAD is set to 1 (AUTOMATIC) before doing this
            // no idea if non-automatic options will even call the download function

            var header = FlashData.Container.CaesarFlashHeader;
            var datablock = header.DataBlocks[blockIndex];
            var segments = datablock.FlashSegments;
            var bytes = new Span<byte>(FlashData.Container.FileBytes);

            long fileCursor = 0; // flashdata is a contiguous span for each datablock; the segments don't specify a data offset

            SegmentsDownloaded = 0;
            SegmentsTotal = segments.Count;
            FlashProgress = 0;
            SegmentName = "";
            BlockName = datablock.Qualifier;
            FlashProgressChanged?.Invoke();


            foreach (var segment in segments)
            {
                long offset = datablock.FlashData + header.CffHeaderSize + header.LanguageBlockLength + fileCursor + 0x414;
                fileCursor += segment.SegmentLength;
                Span<byte> segmentBytes = bytes.Slice((int)offset, segment.SegmentLength);
                DiogenesSharedContext.Singleton.Channel.TransferBlock(segment.FromAddress, segmentBytes);

                // Console.WriteLine($"Segment: {segment.SegmentName} : FromAddress 0x{segment.FromAddress:X8} , SegmentLength 0x{segment.SegmentLength:X8}");
                SegmentsDownloaded++;
                SegmentName = segment.SegmentName;
                FlashProgress = (int)(100.0 * SegmentsDownloaded / segments.Count);

                FlashProgressChanged?.Invoke();
            }
        }


        public byte[] GetChecksum()
        {
            foreach (var security in FlashData.Container.CaesarFlashHeader.DataBlocks[ActiveBlockIndex].FlashSecurities)
            {
                if (security.ChecksumSize > 0)
                {
                    return security.ChecksumValue;
                }
            }
            return new byte[] { };
        }

        public byte[] GetSignature()
        {
            foreach (var security in FlashData.Container.CaesarFlashHeader.DataBlocks[ActiveBlockIndex].FlashSecurities)
            {
                if (security.SignatureSize > 0)
                {
                    return security.SignatureValue;
                }
            }
            return new byte[] { };
            // return new byte[0x80];
        }

        public void SetActiveBlockIndex(int blockIndex)
        {
            ActiveBlockIndex = blockIndex;
        }

        public int GetNumberOfSecurities(int blockIndex)
        {
            return FlashData.Container.CaesarFlashHeader.DataBlocks[blockIndex].FlashSecurities.Count;
        }

    }
}
