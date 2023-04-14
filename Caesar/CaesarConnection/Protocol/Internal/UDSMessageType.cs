using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.Protocol.Internal
{
    public class UDSMessageType
    {
        public const byte DiagnosticSessionControl = 0x10;
        public const byte ECUReset = 0x11;
        public const byte SecurityAccess = 0x27;
        public const byte CommunicationControl = 0x28;
        public const byte Authentication = 0x29;
        public const byte TesterPresent = 0x3E;
        public const byte AccessTimingParameters = 0x83;
        public const byte SecuredDataTransmission = 0x84;
        public const byte ControlDTCSettings = 0x85;
        public const byte ResponseOnEvent = 0x86;
        public const byte LinkControl = 0x87;
        public const byte ReadDataByIdentifier = 0x22;
        public const byte ReadMemoryByAddress = 0x23;
        public const byte ReadScalingDataByIdentifier = 0x24;
        public const byte ReadDataByIdentifierPeriodic = 0x2A;
        public const byte DynamicallyDefineDataIdentifier = 0x2C;
        public const byte WriteDataByIdentifier = 0x2E;
        public const byte WriteMemoryByAddress = 0x3D;
        public const byte ClearDiagnosticInformation = 0x14;
        public const byte ReadDTCInformation = 0x19;
        public const byte InputOutputControlByIdentifier = 0x2F;
        public const byte RoutineControl = 0x31;
        public const byte RequestDownload = 0x34;
        public const byte RequestUpload = 0x35;
        public const byte TransferData = 0x36;
        public const byte RequestTransferExit = 0x37;
        public const byte RequestFileTransfer = 0x38;
        
        public const byte NegativeResponse = 0x7F;
    }
}
