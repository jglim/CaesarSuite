using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.Protocol.Internal
{
    public class UDSNegativeResponse
    {
        public const byte PositiveResponse = 0x00;
        public const byte GeneralReject = 0x10;
        public const byte ServiceNotSupported = 0x11;
        public const byte SubfunctionNotSupported = 0x12;
        public const byte IncorrectMessageLengthOrInvalidFormat = 0x13;
        public const byte ResponseTooLong = 0x14;
        public const byte BusyRepeatRequest = 0x21;
        public const byte ConditionsNotCorrect = 0x22;
        public const byte RequestSequenceError = 0x24;
        public const byte NoResponseFromSubnetComponent = 0x25;
        public const byte FailurePreventsExecutionOfRequestedAction = 0x26;
        public const byte RequestOutOfRange = 0x31;
        public const byte SecurityAccessDenied = 0x33;
        public const byte InvalidKey = 0x35;
        public const byte ExceedNumberOfAttempts = 0x36;
        public const byte RequiredTimeDelayNotExpired = 0x37;
        public const byte UploadDownloadNotAccepted = 0x70;
        public const byte TransferDataSuspended = 0x71;
        public const byte GeneralProgrammingFailure = 0x72;
        public const byte WrongBlockSequenceCounter = 0x73;
        public const byte RequestCorrectlyReceived_ResponsePending = 0x78;
        public const byte SubfunctionNotSupportedInActiveSession = 0x7E;
        public const byte ServiceNotSupportedInActiveSession = 0x7F;
        public const byte RpmTooHigh = 0x81;
        public const byte RpmTooLow = 0x82;
        public const byte EngineRunning = 0x83;
        public const byte EngineNotRunning = 0x84;
        public const byte EngineRunTimeTooLow = 0x85;
        public const byte TemperatureTooHigh = 0x86;
        public const byte TemperatureTooLow = 0x87;
        public const byte VehicleSpeedTooHigh = 0x88;
        public const byte VehicleSpeedTooLow = 0x89;
        public const byte ThrottleOrPedalTooHigh = 0x8A;
        public const byte ThrottleOrPedalTooLow = 0X8B;
        public const byte TransmissionRangeNotInNeutral = 0X8C;
        public const byte TransmissionRangeNotInGear = 0x8D;
        public const byte BrakeSwitchesNotClosed = 0x8F;
        public const byte ShifterLeverNotInPark = 0x90;
        public const byte TorqueConverterClutchLocked = 0x91;
        public const byte VoltageTooHigh = 0x92;
        public const byte VoltageTooLow = 0x93;
    }
}
