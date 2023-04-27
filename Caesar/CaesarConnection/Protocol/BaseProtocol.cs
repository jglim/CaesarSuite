using CaesarConnection.ComParam;
using CaesarConnection.VCI;
using CaesarConnection.VCI.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.Protocol
{
    public class BaseProtocol : IDisposable
    {
        public string PhysicalProtocol;
        public string DiagnosticProtocol;
        public BaseDevice Device;
        public BaseChannel Channel;
        public ParameterSet ComParameters;

        public enum DataFormatCompression : int 
        {
            NoCompression = 0,
            // no idea how to handle the rest
        }
        public enum DataFormatEncryption : int
        {
            NoEncryption = 0,
            // no idea how to handle the rest
        }

        public virtual Dictionary<string, decimal> GetDefaultComParamValues()
        {
            return new Dictionary<string, decimal>();
        }
        public virtual List<ParameterMapping> GetDefaultProtocolComParamMaps()
        {
            return new List<ParameterMapping>();
        }

        public static BaseProtocol Create(string physicalProtocol, string diagProtocol, BaseDevice device, Dictionary<string, int> cpTuple, System.IO.TextWriter traceWriter)
        {
            // check if the selected device supports the current protocol, throws an exception if not
            device.EnsurePhysicalProtocolCompatibility(physicalProtocol);

            BaseProtocol protocol = null;
            switch (diagProtocol)
            {
                case "KW2C3PE":
                    protocol = new KW2C3PE();
                    break;
                case "UDS":
                    protocol = new UDS();
                    break;
                default:
                    throw new Exception($"Unknown protocol type (i.e. not UDS or KW2C3PE): {diagProtocol}");
            }

            // create a new comparam pool
            var cp = new ParameterSet(protocol.GetDefaultComParamValues(), protocol.GetDefaultProtocolComParamMaps());
            foreach (var param in cpTuple)
            {
                cp.SetParameter(param.Key, param.Value);
            }

            protocol.PhysicalProtocol = physicalProtocol;
            protocol.DiagnosticProtocol = diagProtocol;
            protocol.Device = device;
            protocol.ComParameters = cp;

            // create the physical channel
            protocol.Channel = device.CreateChannel(cp, traceWriter);

            // initialize protocol, stuff like polling loops can be configured now
            // good time to set up diag session too
            // PALDetermineID equivalent could be optionally called after
            protocol.Initialize();

            return protocol;
        }
        public virtual void Initialize()
        {
        }

        public virtual void Dispose() 
        {
            Device.ReleaseChannel(Channel);
        }


        public CPEnum.CANECU_CLASS GetEcuClass()
        {
            return (CPEnum.CANECU_CLASS)(int)ComParameters.GetParameterOrDefault(CP.CANECU_CLASS, 2);
        }
        public bool EcuIsGateway()
        {
            var ecu = GetEcuClass();
            return (ecu == CPEnum.CANECU_CLASS.GATEWAY_FUNCTIONAL) || (ecu == CPEnum.CANECU_CLASS.GATEWAY_FUNCTIONAL_ALWAYS_TP) || (ecu == CPEnum.CANECU_CLASS.GATEWAY_PHYSICAL);
        }
        public bool EcuIsFunctional()
        {
            var ecu = GetEcuClass();
            return (ecu == CPEnum.CANECU_CLASS.FUNCTIONAL) || (ecu == CPEnum.CANECU_CLASS.FUNCTIONAL_ALWAYS_TP) || (ecu == CPEnum.CANECU_CLASS.GATEWAY_FUNCTIONAL) || (ecu == CPEnum.CANECU_CLASS.GATEWAY_FUNCTIONAL_ALWAYS_TP);
        }
        public bool EcuAlwaysRequiresTP()
        {
            var ecu = GetEcuClass();
            return (ecu == CPEnum.CANECU_CLASS.FUNCTIONAL_ALWAYS_TP) || (ecu == CPEnum.CANECU_CLASS.GATEWAY_FUNCTIONAL_ALWAYS_TP);
        }
        public bool EcuIsPhysical()
        {
            var ecu = GetEcuClass();
            return (ecu == CPEnum.CANECU_CLASS.PHYSICAL) || (ecu == CPEnum.CANECU_CLASS.GATEWAY_PHYSICAL);
        }

        public virtual byte[] Send(byte[] message, bool responseRequired = true, Envelope.RequestType recipientType = Envelope.RequestType.Physical, bool throwExceptionOnError = false) 
        {
            return new byte[] { };
        }

        public virtual int GetEcuVariant() 
        {
            throw new Exception($"Variant detection is unimplemented for this protocol");
        }

        public virtual Target.SoftwareBlock[] GetSoftwareBlocks() 
        {
            // this shouldn't be fatal..?
            return new Target.SoftwareBlock[] { };
        }

        public virtual void TransferBlock(long address, Span<byte> payload) 
        {
            throw new Exception($"Block transfer is unimplemented for this protocol");
        }

        public static byte CreateDataFormatIdentifier(DataFormatCompression compression, DataFormatEncryption encryption) 
        {
            // one nibble each
            int compInt = (int)compression & 0xF;
            int encInt = (int)encryption & 0xF;
            int result = (compInt << 4) | encInt;
            return (byte)result;
        }

        public static byte CreateAddressAndLengthFormatIdentifier(int addressSizeInBytes, int memorySizeInBytes)
        {
            // fixme: duplicate function in memory editor
            if ((addressSizeInBytes < 1) || (addressSizeInBytes > 5))
            {
                throw new ArgumentOutOfRangeException("Invalid address size for ALFID");
            }
            if ((memorySizeInBytes < 1) || (memorySizeInBytes > 5))
            {
                throw new ArgumentOutOfRangeException("Invalid memory size for ALFID");
            }
            return (byte)((memorySizeInBytes << 4) | addressSizeInBytes);
        }

        // converts a long into a big-endian byte array
        public List<byte> LongToBytesBigEndian(long inValue, int constrainedSize = -1)
        {
            // fixme: duplicate function in memory editor
            List<byte> result = new List<byte>();
            if (constrainedSize == -1)
            {
                while (inValue > 0)
                {
                    byte row = (byte)(inValue & 0xFF);
                    result.Insert(0, row);
                    inValue >>= 8;
                }
            }
            else
            {
                for (int i = 0; i < constrainedSize; i++)
                {
                    byte row = (byte)(inValue & 0xFF);
                    result.Insert(0, row);
                    inValue >>= 8;
                }
            }
            return result;
        }

        public long BytesBigEndianToLong(Span<byte> inValue) 
        {
            long result = 0;
            for (int i = 0; i < inValue.Length; i++)
            {
                result <<= 8;
                result |= inValue[i];
            }
            return result;
        }

    }
}
