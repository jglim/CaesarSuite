using System;
using System.Collections.Generic;

namespace Caesar
{
    public class ECUConnectionProfile
    {
        public string Qualifier { get; set; } = string.Empty;
        public string InterfaceQualifier { get; set; } = string.Empty;
        public Dictionary<string, int> CommunicationParameters { get; set; } = new Dictionary<string, int>(StringComparer.Ordinal);

        public int? RequestCanIdentifier
        {
            get
            {
                return TryGetComParameterValue(ECUInterfaceSubtype.ParamName.CP_REQUEST_CANIDENTIFIER, out int value) ? value : (int?)null;
            }
        }

        public int? ResponseCanIdentifier
        {
            get
            {
                return TryGetComParameterValue(ECUInterfaceSubtype.ParamName.CP_RESPONSE_CANIDENTIFIER, out int value) ? value : (int?)null;
            }
        }

        public int? FunctionalRequestCanIdentifier
        {
            get
            {
                return TryGetComParameterValue(ECUInterfaceSubtype.ParamName.CP_FUNCTIONAL_REQUEST_CANIDENTIFIER, out int value) ? value : (int?)null;
            }
        }

        public int? GlobalRequestCanIdentifier
        {
            get
            {
                return TryGetComParameterValue(ECUInterfaceSubtype.ParamName.CP_GLOBAL_REQUEST_CANIDENTIFIER, out int value) ? value : (int?)null;
            }
        }

        public int? BaudRate
        {
            get
            {
                return TryGetComParameterValue(ECUInterfaceSubtype.ParamName.CP_BAUDRATE, out int value) ? value : (int?)null;
            }
        }

        public bool TryGetComParameterValue(string name, out int result)
        {
            return CommunicationParameters.TryGetValue(name, out result);
        }

        public bool TryGetComParameterValue(ECUInterfaceSubtype.ParamName name, out int result)
        {
            return TryGetComParameterValue(name.ToString(), out result);
        }
    }
}
