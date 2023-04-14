using CaesarConnection.ComParam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.Protocol
{
    // KW2C3PE inherits from UDS
    // UDS is the successor of KW2c3PE and inherits a lot of similarities.
    // In this implementation, we inherit from UDS and apply the differences such as different messages for session change / ecuid
    public class KW2C3PE : UDS
    {
        public override byte[] GetMessageForSessionNormal()
        {
            return new byte[] { 0x10, 0x81 };
        }

        public override byte[] GetMessageForSessionDiagnostic()
        {
            return new byte[] { 0x10, 0x92 };
        }

        public override int GetEcuVariant()
        {
            
            byte[] variant = Send(new byte[] { 0x1A, 0x86 });
            if (variant.Take(2).SequenceEqual(new byte[] { 0x5A, 0x86 }))
            {
                return (variant[12] << 8) | variant[13];
            }

            variant = Send(new byte[] { 0x1A, 0x87 });
            if (variant.Take(2).SequenceEqual(new byte[] { 0x5A, 0x87 }))
            {
                return (variant[4] << 8) | variant[5];
            }
            return 0;
        }

        public override List<ParameterMapping> GetDefaultProtocolComParamMaps()
        {
            return new List<ParameterMapping>()
            {
                new ParameterMapping { Source = CP.Baudrate, Destination = CP.BAUDRATE, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.P2Max, Destination = CP.P2_CAN_MAX, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.P3Min, Destination = CP.P3_SUG, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.TesterPresentHandling, Destination = CP.NO_TESTERPRESENT, ConversionFactor = 1M }, // CPTesterPresentHandlingMappingTable
                new ParameterMapping { Source = CP.CanPhysReqId, Destination = CP.REQUEST_CANIDENTIFIER, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.CanFuncReqId, Destination = CP.FUNCTIONAL_REQUEST_CANIDENTIFIER, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.CanRespUSDTId, Destination = CP.RESPONSE_CANIDENTIFIER, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.RequestAddrMode, Destination = CP.REQUESTTYPE, ConversionFactor = 1M }, // CPRequestAddrModeMappingTable
                new ParameterMapping { Source = CP.TesterPresentTime, Destination = CP.R1_SUG, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.RepeatReqCountApp, Destination = CP.REQREPCOUNT, ConversionFactor = 1M }, // second param is 1
                new ParameterMapping { Source = CP.BlockSize, Destination = CP.BLOCKSIZE_SUG, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.Bs, Destination = CP.B12_REQ_MAX, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.Br, Destination = CP.B12_RESP_SUG, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.Cr, Destination = CP.C_RESP_MAX, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.StMin, Destination = CP.STMIN_SUG, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.StMinOverride, Destination = CP.C_REQ_SUG, ConversionFactor = 1M },
                
                // not defined: CP_LOGICAL_ADDRESS_GATEWAY, CP_LOGICAL_SOURCE_ADDRESS, CP_LOGICAL_TARGET_ADDRESS
                // new ParameterMapping { Source = CP.LogicalAddressGateway, Destination = CP.LogicalAddressGateway, ConversionFactor = 1M },
                // new ParameterMapping { Source = CP.LogicalSourceAddress, Destination = CP.LogicalSourceAddress, ConversionFactor = 1M },
                // new ParameterMapping { Source = CP.LogicalTargetAddress, Destination = CP.LogicalTargetAddress, ConversionFactor = 1M },

                // this CP is an unofficial change by jg; uds adds this value on top of some timeouts to compensate for network delays
                // as the quality of j2534 devices largely depend on their vendor, having this extra time can help with unintended timeouts
                new ParameterMapping { Source = CP.CanTransmissionTime, Destination = CP.CAN_TRANSMIT, ConversionFactor = 0.001M },
            };
        }

        public override Dictionary<string, decimal> GetDefaultComParamValues()
        {
            return new Dictionary<string, decimal>()
            {
                { CP.B12_REQ_MAX, 150 },
                { CP.B12_REQ_MIN, 0 },
                { CP.B12_RESP_SUG, 0 },

                { CP.BLOCKSEQCOUNTER, 0 },
                { CP.BLOCKSIZE_ECU, 8 },
                { CP.BLOCKSIZE_SUG, 8 },
                { CP.BREAKCONDITION, 2 },
                { CP.BUSTOPOLOGY, 0 },
                { CP.C_REQ_SUG, 0 },
                { CP.C_RESP_MIN, 0 },
                { CP.C_RESP_MAX, 150 },

                //{ CP.BS_MAX, 150 }, // absent?
                //{ CP.CAN_TRANSMIT, 130 }, // absent?

                { CP.CANBUSOFFREACT, 128 },
                { CP.CANCONTROLLER_STROBES, 0xFFF },
                { CP.CANDEFAULTREACT, 192 },
                { CP.CANECU_CLASS, 2 },
                { CP.CANMONITORING, 1 },

                { CP.CHECK_PCI_CF, 1 },
                { CP.CHECK_PCI_FF_SF, 1 },

                { CP.CHECKROUTINECOUNTER, 1 },
                //{ CP.CS_MAX, 150 }, // absent?
                //{ CP.CS_SUG, 20 }, // absent?
                { CP.DEFAULTREACT, 192 },
                { CP.DOWNLOADREACT, 192 }, // i think it should be 192 and not 129?

                { CP.DTCCOUNTOFFSET, 0},                
                //{ CP.DSC_REPEAT_TIME, 130 }, // absent?

                { CP.ECUID_OPTION, 0x86 }, // 0x86

                { CP.FUNCTIONAL_REQUEST_CANIDENTIFIER, 0x1C },
                { CP.GLOBAL_FIRST_R1_SUG, 130 }, // delay after 1092 msg?
                { CP.GLOBAL_R1_SUG, 1000 }, // tp interval?
                { CP.GLOBAL_REQUEST_CANIDENTIFIER, 0x1C },
                { CP.IDENTIFIER_LENGTH, 11 }, // 11 bit can (as opposed to 29 bit can)

                //{ CP.INIT_SESSION_TYPE, 3 }, // absent? uds 10 03<-
                //{ CP.MIRROR_MEMORY_CORRECTION, 4 }, // absent?

                // ASAM CP_InitializationSettings? uses 2 for KW2C3PE, 1 for KW2000PE

                
                // entire chunk here seems to be uds specific
                //{ CP.P2_EXT_TIMEOUT_7F_21, 200 },
                //{ CP.P2_EXT_TIMEOUT_7F_78, 2000 },
                //{ CP.P2_TIMEOUT, 20 },
                //{ CP.P3_TIME_NEXTREQ_PHYS, 150 },
                //{ CP.P3_TIME_NEXTREQ_FUNC, 150 },
                //{ CP.PARTBLOCK, 1 },
                
                          
                { CP.P2_CAN_MAX, 150 },
                { CP.P2_CAN_MIN, 0 },
                { CP.P3_7F_21, 20 },
                { CP.P3_MAX, 5000 },
                { CP.P3_SUG, 0 },
                { CP.R1_SUG, 2500 },

                { CP.REQREPCOUNT, 3 },
                { CP.RESPTELCOUNT, 1 },

                { CP.RESTHEADERMODE, 2 },
                { CP.ROE_CANIDENTIFIER, 0 },

                //{ CP.S3_TP_PHYS_TIMER, 2000 }, // absent
                //{ CP.S3_TP_FUNC_TIMER, 2000 }, // absent

                { CP.SETCPDEFAULTS, 1 },

                //{ CP.SWSUPPLIERBLOCK, 1 },
                //{ CP.SWVERSIONBLOCK, 1 },

                { CP.TESTERPRESENT_MESSAGE, 0x3E01 },
                { CP.TIMEOUTP2CANREACT, 192 },
                { CP.TIMEOUTB12REQREACT, 192 },
                { CP.TIMEOUTCFREACT, 192 },

                { CP.TRIGADDRESS, 0 },
                { CP.USE_EXTENDED_ECU_IDENTIFICATION, 0 },
                { CP.USE_EXTENDED_REQUESTED_SUPPORTED_DTCS, 0 },

                // { CP.USE_TIMING_RECEIVED_FROM_ECU, 1 }, // absent
                { CP.USED_CANDATA_LEN, 8 },

                { CP.I_DOWNLOADSIZE, 1 },
                { CP.I_EXITTYPE, 1 },
                { CP.I_GPDAUTODOWNLOAD, 2 },
                { CP.I_INITTYPE, 1 },
                { CP.I_READIDBLOCK, 1 },
                { CP.I_READTIMING, 0 },
                { CP.I_ROUTINECOUNTER, 30 },
                { CP.I_UPLOADSIZE, 0 },

                { CP.CHECKRESPONSE, 0 },
                { CP.NO_TESTERPRESENT, 0 },
                { CP.RESPONSEMODE, 0 },
                { CP.STOPREQUEST, 0 },
                { CP.MEM_ADDRESS_FORMAT, 0 },
                { CP.MEM_SIZE_FORMAT, 0 },
                { CP.REQUESTTYPE, 0 },
                { CP.ADDRESSEXTENSION, 0 },
                { CP.ADDRESSMODE, 0 },
                { CP.RSP_DSC, 0 },
                { CP.RSP_TP, 0 },
                { CP.IGNITION_REQUIRED, 0 },
                { CP.PARTNUMBERID, 0 },
                { CP.HWVERSIONID, 0 },
                { CP.SWVERSIONID, 0 },
                { CP.SUPPLIERID, 0 },
                { CP.CANCONTROLLER_TIMING, 0 },
                { CP.P3_TIME_NEXTREQ_PHYS_WITH_RESPONSE, 0 },
                { CP.SEGMENTSIZE, 0 },
                { CP.BS_MIN, 0 },
                { CP.BR_SUG, 0 },
                { CP.CS_MIN, 0 },
                { CP.STMIN_SUG, 0 },
                { CP.BAUDRATE, 0 },
                { CP.CANMONITORINGFILTER, 0 },
                { CP.CANMONITORINGIDENTIFIER, 0 },
                { CP.REQUEST_CANIDENTIFIER, 0 },
                { CP.RESPONSE_CANIDENTIFIER, 0 },
                { CP.ROE_RESPONSE_CANIDENTIFIER, 0 },


                //{ CP.SPARE_1, 0 },
                //{ CP.SPARE_2, 0 },
                //{ CP.SPARE_3, 0 },
                //{ CP.SPARE_4, 0 },
                //{ CP.SPARE_5, 0 },
                //{ CP.SPARE_6, 0 },
                //{ CP.SPARE_7, 0 },
                //{ CP.SPARE_8, 0 },
                //{ CP.SPARE_9, 0 },
                //{ CP.SPARE_10, 0 },


                // this value is unofficially added by jg to reduce false timeouts on j2534 devices
                // uds has this ~130ms period to reduce false timeouts
                // this value will be remapped to CanTransmissionTime (ASAM)
                { CP.CAN_TRANSMIT, 130 },
            };
        }
    }
}
