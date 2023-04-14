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
    }
}
