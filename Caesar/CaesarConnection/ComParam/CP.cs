using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.ComParam
{
    public class CP
    {
        /// <summary>
        /// In the case of use of the value *PHYSICAL the ecu is stimulated with the address in CP_REQUEST_CANIDENTIFIER. A response of the ecu with the address in CP_RESPONSE_CANIDENTIFIER is expected. 
        /// 
        /// In the case of use of the value *FUNCTIONAL the ecu is stimulated with the address in CP_GLOBAL_REQUEST_CANIDENTIFIER. 
        /// Also handshake (TesterPresent) and the termination of the diagnosis are executed with this address (-> does not apply, if the comparameter is indicated at a diagservice TEMPORARY.) 
        /// On these services no response is expected. With this type the first controller, which is taken in diagnosis, intends the address for stimulation (a global address should be used, which applies to all ecu's at this bus). 
        /// If a ecu with a further global identifier, independently of the beforehand described, is to be taken in diagnosis, then *ALWAYS_TP is to be used. 
        /// 
        /// If the diagnosis is to be done with a gateway, then the values Gateway_* must be used.  
        /// 
        /// Values: FUNCTIONAL,1;PHYSICAL,2;FUNCTIONAL_ALWAYS_TP,5;GATEWAY_FUNCTIONAL,9;GATEWAY_PHYSICAL,10;GATEWAY_FUNCTIONAL_ALWAYS_TP,13
        /// </summary>
        public static readonly string CANECU_CLASS = "CP_CANECU_CLASS";

        /// <summary>
        /// Parameter is only used by trafo tool and specifies the protocol settings. The default value 0xFE should not be changed. It specifies the settings for UDS protocol. 
        /// </summary>
        public static readonly string ECUID_OPTION = "CP_ECUID_OPTION";

        /// <summary>
        /// Defines which part number of several logical blocks is evaluated. 
        /// </summary>
        public static readonly string PARTBLOCK = "CP_PARTBLOCK";

        /// <summary>
        /// Defines which version information of several logical blocks is evaluated. 
        /// </summary>
        public static readonly string SWVERSIONBLOCK = "CP_SWVERSIONBLOCK";

        /// <summary>
        /// Defines logical block of supplier info. 
        /// </summary>
        public static readonly string SWSUPPLIERBLOCK = "CP_SWSUPPLIERBLOCK";

        /// <summary>
        /// Defines session type requested during initialization automatically by protocol modul (programming session / extended session), if CPI_INITTYPE is set to DEFAULTINIT.
        /// Values: ST_EXTENDED,3;ST_PROGRAM,2;ST_DEFAULT,1
        /// </summary>
        public static readonly string INIT_SESSION_TYPE = "CP_INIT_SESSION_TYPE";

        /// <summary>
        /// Describes how many can messages are to be received between two (2) flowcontrol frames. Corresponds to BSmax at the flowcontrol frame of the tester. 
        /// </summary>
        public static readonly string BLOCKSIZE_SUG = "CP_BLOCKSIZE_SUG";

        /// <summary>
        /// Corresponds to the value of BSmax of the last ecu flowcontrol frame. 
        /// </summary>
        public static readonly string BLOCKSIZE_ECU = "CP_BLOCKSIZE_ECU";

        /// <summary>
        /// Switch to abort waiting for response.  ONCONTEXT Sends response to the master and waits for the next response until gpd signalizes the end.  
        /// If NormalTiming then parameter is set to ONTIMEOUT otherwise the pass is stopped.  NEVERCOME Sends response to master and waits for the next response.  
        /// ONACCOUNT Normal break condition for a service if the last response was received.  
        /// 
        /// Values: ONCONTEXT,0;NEVERCOME,1;ONACCOUNT,2;ONTIMEOUT,3
        /// </summary>
        public static readonly string BREAKCONDITION = "CP_BREAKCONDITION";

        /// <summary>
        /// Reaction for can controller busoff state. Default value is defined as NOTIFICATION | STOP (e.g.communication parameter definition, ch. Introduction) 
        /// Values: NOTIFICATION|REPEATBEFOREHANDLE|KEEP,195;NOTIFICATION|REPEATBEFOREHANDLE|WAIT,194;NOTIFICATION|REPEATBEFOREHANDLE|STAY,193;NOTIFICATION|REPEATBEFOREHANDLE|STOP,192;NOTIFICATION|KEEP,131;NOTIFICATION|WAIT,130;NOTIFICATION|STAY,129;NOTIFICATION|STOP,128;REPEATBEFOREHANDLE|KEEP,67;REPEATBEFOREHANDLE|WAIT,66;REPEATBEFOREHANDLE|STAY,65;REPEATBEFOREHANDLE|STOP,64;KEEP,3;WAIT,2;STAY,1;STOP,0
        /// </summary>
        public static readonly string CANBUSOFFREACT = "CP_CANBUSOFFREACT";

        /// <summary>
        /// Reaction for cancontroller errors where no special parameter exists. Default value is defined as NOTIFICATION | REPEATBEFOREHANDLE | STOP (e.g.communication parameter definition, ch. Introduction) 
        /// Values: NOTIFICATION|REPEATBEFOREHANDLE|KEEP,195;NOTIFICATION|REPEATBEFOREHANDLE|WAIT,194;NOTIFICATION|REPEATBEFOREHANDLE|STAY,193;NOTIFICATION|REPEATBEFOREHANDLE|STOP,192;NOTIFICATION|KEEP,131;NOTIFICATION|WAIT,130;NOTIFICATION|STAY,129;NOTIFICATION|STOP,128;REPEATBEFOREHANDLE|KEEP,67;REPEATBEFOREHANDLE|WAIT,66;REPEATBEFOREHANDLE|STAY,65;REPEATBEFOREHANDLE|STOP,64;KEEP,3;WAIT,2;STAY,1;STOP,0        /// </summary>
        public static readonly string CANDEFAULTREACT = "CP_CANDEFAULTREACT";

        /// <summary>
        /// Switches can monitoring on or off. 
        /// </summary>
        public static readonly string CANMONITORING = "CP_CANMONITORING";

        /// <summary>
        /// Flag for checking number of sending attempts. 
        /// </summary>
        public static readonly string CHECKROUTINECOUNTER = "CP_CHECKROUTINECOUNTER";

        /// <summary>
        /// Flag for checking the local id and length of a response. Error: WRONGRESPONSELOCALID, WRONGRESPONSELENGTH, WRONGRESPONSE 
        /// </summary>
        public static readonly string CHECKRESPONSE = "CP_CHECKRESPONSE";

        /// <summary>
        /// Reaction for errors where no special parameter exists. Default value is defined as NOTIFICATION | REPEATBEFOREHANDLE | STOP (e.g.communication parameter definition, ch. Introduction) 
        /// Values: NOTIFICATION|REPEATBEFOREHANDLE|KEEP,195;NOTIFICATION|REPEATBEFOREHANDLE|WAIT,194;NOTIFICATION|REPEATBEFOREHANDLE|STAY,193;NOTIFICATION|REPEATBEFOREHANDLE|STOP,192;NOTIFICATION|KEEP,131;NOTIFICATION|WAIT,130;NOTIFICATION|STAY,129;NOTIFICATION|STOP,128;REPEATBEFOREHANDLE|KEEP,67;REPEATBEFOREHANDLE|WAIT,66;REPEATBEFOREHANDLE|STAY,65;REPEATBEFOREHANDLE|STOP,64;KEEP,3;WAIT,2;STAY,1;STOP,0        /// </summary>
        public static readonly string DEFAULTREACT = "CP_DEFAULTREACT";

        /// <summary>
        /// Reaction on error while transfer data (upload, download) is running. Default value is defined as NOTIFICATION | STAY (e.g.communication parameter definition, ch. Introduction) 
        /// Values: NOTIFICATION|REPEATBEFOREHANDLE|KEEP,195;NOTIFICATION|REPEATBEFOREHANDLE|WAIT,194;NOTIFICATION|REPEATBEFOREHANDLE|STAY,193;NOTIFICATION|REPEATBEFOREHANDLE|STOP,192;NOTIFICATION|KEEP,131;NOTIFICATION|WAIT,130;NOTIFICATION|STAY,129;NOTIFICATION|STOP,128;REPEATBEFOREHANDLE|KEEP,67;REPEATBEFOREHANDLE|WAIT,66;REPEATBEFOREHANDLE|STAY,65;REPEATBEFOREHANDLE|STOP,64;KEEP,3;WAIT,2;STAY,1;STOP,0        /// </summary>
        public static readonly string DOWNLOADREACT = "CP_DOWNLOADREACT";

        /// <summary>
        /// Used length for can identifier. 
        /// Values: STANDARD(11bit)|EXTENDED(29bit)
        /// </summary>
        public static readonly string IDENTIFIER_LENGTH = "CP_IDENTIFIER_LENGTH";

        /// <summary>
        /// Flag for sending testerpresent or not. 
        /// </summary>
        public static readonly string NO_TESTERPRESENT = "CP_NO_TESTERPRESENT";

        /// <summary>
        /// TRUE: Timing values received from ECU (with Response on Diagnostic Session Control) are used by the tester. FALSE: The timing values received by the ECU are ignored by the tester. 
        /// </summary>
        public static readonly string USE_TIMING_RECEIVED_FROM_ECU = "CP_USE_TIMING_RECEIVED_FROM_ECU";

        /// <summary>
        /// Number of attempts to send a request in case of a transmission error (e.g. timeout). 
        /// </summary>
        public static readonly string REQREPCOUNT = "CP_REQREPCOUNT";

        /// <summary>
        /// This Parameter is used to check the response SID. Is set automatically by the gpd to request SID + 0x40. The application can set this parameter to other values if the ecu doesn't response with the expected value.
        /// </summary>
        public static readonly string RESPONSEMODE = "CP_RESPONSEMODE";

        /// <summary>
        /// Number of expected responses from the ecu. Is set to 1 by the gpd. 
        /// </summary>
        public static readonly string RESPTELCOUNT = "CP_RESPTELCOUNT";

        /// <summary>
        /// Flag for setting the default values of the comparameters after a diagnostic end. 
        /// </summary>
        public static readonly string SETCPDEFAULTS = "CP_SETCPDEFAULTS";

        /// <summary>
        /// Set to TRUE by the gpd after a positive response for StopCommunication or ResetECU. The communication with the ecu stopps. If set to TRUE by the application in the variant buffer then the communication with the ecu stopps before the next service. If set to TRUE by the application in the working buffer then the communication with the ecu stopps after the next service.
        /// </summary>
        public static readonly string STOPREQUEST = "CP_STOPREQUEST";

        /// <summary>
        /// Reaction on timeout P2 error (time between request and response). Default value is defined as NOTIFICATION | REPEATBEFOREHANDLE | STOP (e.g.communication parameter definition, ch. Introduction) 
        /// Values: NOTIFICATION|REPEATBEFOREHANDLE|KEEP,195;NOTIFICATION|REPEATBEFOREHANDLE|WAIT,194;NOTIFICATION|REPEATBEFOREHANDLE|STAY,193;NOTIFICATION|REPEATBEFOREHANDLE|STOP,192;NOTIFICATION|KEEP,131;NOTIFICATION|WAIT,130;NOTIFICATION|STAY,129;NOTIFICATION|STOP,128;REPEATBEFOREHANDLE|KEEP,67;REPEATBEFOREHANDLE|WAIT,66;REPEATBEFOREHANDLE|STAY,65;REPEATBEFOREHANDLE|STOP,64;KEEP,3;WAIT,2;STAY,1;STOP,0        /// </summary>
        public static readonly string TIMEOUTP2CANREACT = "CP_TIMEOUTP2CANREACT";

        /// <summary>
        /// Reaction on timeout B1,2req error (missing flowcontrol). Default value is defined as NOTIFICATION | REPEATBEFOREHANDLE | STOP (e.g.communication parameter definition, ch. Introduction) 
        /// Values: NOTIFICATION|REPEATBEFOREHANDLE|KEEP,195;NOTIFICATION|REPEATBEFOREHANDLE|WAIT,194;NOTIFICATION|REPEATBEFOREHANDLE|STAY,193;NOTIFICATION|REPEATBEFOREHANDLE|STOP,192;NOTIFICATION|KEEP,131;NOTIFICATION|WAIT,130;NOTIFICATION|STAY,129;NOTIFICATION|STOP,128;REPEATBEFOREHANDLE|KEEP,67;REPEATBEFOREHANDLE|WAIT,66;REPEATBEFOREHANDLE|STAY,65;REPEATBEFOREHANDLE|STOP,64;KEEP,3;WAIT,2;STAY,1;STOP,0        /// </summary>
        public static readonly string TIMEOUTB12REQREACT = "CP_TIMEOUTB12REQREACT";

        /// <summary>
        /// Reaction on timeout Cresp error (missing consecutive frame). Default value is defined as NOTIFICATION | REPEATBEFOREHANDLE | STOP (e.g.communication parameter definition, ch. Introduction) 
        /// Values: NOTIFICATION|REPEATBEFOREHANDLE|KEEP,195;NOTIFICATION|REPEATBEFOREHANDLE|WAIT,194;NOTIFICATION|REPEATBEFOREHANDLE|STAY,193;NOTIFICATION|REPEATBEFOREHANDLE|STOP,192;NOTIFICATION|KEEP,131;NOTIFICATION|WAIT,130;NOTIFICATION|STAY,129;NOTIFICATION|STOP,128;REPEATBEFOREHANDLE|KEEP,67;REPEATBEFOREHANDLE|WAIT,66;REPEATBEFOREHANDLE|STAY,65;REPEATBEFOREHANDLE|STOP,64;KEEP,3;WAIT,2;STAY,1;STOP,0        /// </summary>
        public static readonly string TIMEOUTCFREACT = "CP_TIMEOUTCFREACT";

        /// <summary>
        /// Number of Bytes used to specify the address of a memory (flash file). 
        /// </summary>
        public static readonly string MEM_ADDRESS_FORMAT = "CP_MEM_ADDRESS_FORMAT";

        /// <summary>
        /// Number of Bytes used to specify the size of a memory (flash file). 
        /// </summary>
        public static readonly string MEM_SIZE_FORMAT = "CP_MEM_SIZE_FORMAT";

        /// <summary>
        /// Specifies if the service is sent functional or physical. It doesn't say anything about the expected response behavior of the ECU. For physical requests, the CAN-Ids CP_REQUEST_CANIDENTIFIER and CP_RESPONSE_CANIDENTIFIER are applied. 
        /// For functional requests, the CAN-Id CP_FUNCTIONAL_REQUEST_CANIDENTIFIER is applied. CP_REQUESTTYPE is not used for initialization messages automatically generated by the protocol module. 
        /// 
        /// There is a special handling with additional parameters for all messages which are generated automatically by the protocol module during initialization and at the end of a diagnostic session. 
        /// In this case the decision (physical or functional addressing) is derived from the ECU-class (CP_CANECU_CLASS). The CAN-Id which is applied in this case is taken from CP_GLOBAL_REQUEST_CANIDENTIFIER.  
        /// Values: RT_PHYSICAL, RT_FUNCTIONAL, RT_FUNCTIONAL_WITH_RESPONSE 
        /// </summary>
        public static readonly string REQUESTTYPE = "CP_REQUESTTYPE";

        /// <summary>
        /// Target address of ecu, only used for extended addressing (CP_ADDRESSMODE=AM_EXTENDED) 
        /// </summary>
        public static readonly string ADDRESSEXTENSION = "CP_ADDRESSEXTENSION";

        /// <summary>
        ///  Address mode for ISO 15765 
        ///  Values: AM_NORMAL,AM_EXTENDED
        /// </summary>
        public static readonly string ADDRESSMODE = "CP_ADDRESSMODE";

        /// <summary>
        /// If TRUE then a block counter is inserted in TransferData request ($36) for automatic download. May not be changed for UDS. In UDS, the Block Sequence Counter is a mandatory parameter of service $36. 
        /// </summary>
        public static readonly string BLOCKSEQCOUNTER = "CP_BLOCKSEQCOUNTER";

        /// <summary>
        /// Defines if a positive response is expected upon an automatically generated diagnostic session control message. 
        /// If CP_RSP_DSC = RT_NONE (Default), a positive response is expected in case of physical addressed ECUs and no positive respone is expected in case of functionally addressed ECUs. 
        /// The addressing type for automatically generated DSC messages depends on the ECU class.  
        /// Values: RT_NONE,0;RT_NO_POS_RSP_REQ,1;RT_POS_RSP_REQ,2
        /// </summary>
        public static readonly string RSP_DSC = "CP_RSP_DSC";

        /// <summary>
        /// Defines if a positive response is expected for TP messages. If CP_RSP_TP = RT_NONE (Default), a positive response is expected in case of physical addressed ECUs and no positive respone is expected in case of functionally addressed ECUs. 
        /// The addressing type for Tester Present messages depends on the ECU class.
        /// Values: RT_NONE,0;RT_NO_POS_RSP_REQ,1;RT_POS_RSP_REQ,2 
        /// </summary>
        public static readonly string RSP_TP = "CP_RSP_TP";

        /// <summary>
        /// Defines if ignition is required
        /// </summary>
        public static readonly string IGNITION_REQUIRED = "CP_IGNITION_REQUIRED";

        /// <summary>
        /// Defines the data identifier of the partnumber used for variant detection. 
        /// </summary>
        public static readonly string PARTNUMBERID = "CP_PARTNUMBERID";

        /// <summary>
        /// Defines the data identifier (hardware version) used for variant detection. 
        /// </summary>
        public static readonly string HWVERSIONID = "CP_HWVERSIONID";

        /// <summary>
        /// Defines the data identifier (software version) used for variant detection. 
        /// </summary>
        public static readonly string SWVERSIONID = "CP_SWVERSIONID";

        /// <summary>
        /// Defines the data identifier (supplier info) used for variant detection. 
        /// </summary>
        public static readonly string SUPPLIERID = "CP_SUPPLIERID";

        /// <summary>
        /// Control code (bit mask) for different Part_B HW modules, used to specify settings for HW signals like CTRL1+2, STB1+2 and WKUP1+2. (for more details please refer to GPD Language Description - CAN Functions)
        /// </summary>
        public static readonly string CANCONTROLLER_STROBES = "CP_CANCONTROLLER_STROBES";

        /// <summary>
        /// Describes timing values of the can controller. If 0 then default values for the timing are used. (e.g. GPD Language Description - CAN Functions) 
        /// </summary>
        public static readonly string CANCONTROLLER_TIMING = "CP_CANCONTROLLER_TIMING";

        /// <summary>
        /// Default value for the time between the two DSC messages required during functional initialization sequence. 
        /// </summary>
        public static readonly string DSC_REPEAT_TIME = "CP_DSC_REPEAT_TIME";

        /// <summary>
        /// Maximum time between request and response. If the value of the timing record of the pos. resp. of DSC is adopted the value of CP_CANTRANSMIT has to be added. After a P2 timeout the request is repeated according to CP_ReqRepCount. (Max 5000)
        /// </summary>
        public static readonly string P2_TIMEOUT = "CP_P2_TIMEOUT";

        /// <summary>
        /// Used time before a request is repeated after a negative response 7F xx 21. 
        /// </summary>
        public static readonly string P2_EXT_TIMEOUT_7F_21 = "CP_P2_EXT_TIMEOUT_7F_21";

        /// <summary>
        /// Within this time a ecu has to send a response after it has requested more time via RC 7F 78. The GPD adds the time delay caused by the Network and stored in parameter CP_CAN_TRANSMIT. 
        /// </summary>
        public static readonly string P2_EXT_TIMEOUT_7F_78 = "CP_P2_EXT_TIMEOUT_7F_78";

        /// <summary>
        /// Time delay caused by the CAN network. Time covers complete transmission time for a signal from tester to ecu and backwards (Time= Ttx + Trx). 
        /// </summary>
        public static readonly string CAN_TRANSMIT = "CP_CAN_TRANSMIT";

        /// <summary>
        /// Used time before a next request is send after successful transmission  of the previous request, in case of physical addressing and no response required. 
        /// </summary>
        public static readonly string P3_TIME_NEXTREQ_PHYS = "CP_P3_TIME_NEXTREQ_PHYS";

        /// <summary>
        /// No documentation available
        /// </summary>
        public static readonly string P3_TIME_NEXTREQ_PHYS_WITH_RESPONSE = "CP_P3_TIME_NEXTREQ_PHYS_WITH_RESPONSE";

        /// <summary>
        /// Used time before a request is send after successful transmission  of the previous request in case of functional addressing (does not apply to functional TP request). 
        /// </summary>
        public static readonly string P3_TIME_NEXTREQ_FUNC = "CP_P3_TIME_NEXTREQ_FUNC";

        /// <summary>
        /// Time between a response and the next subsequent tester present message (if no other request is sent to this ECU) in case of physically addressed requests. 
        /// </summary>
        public static readonly string S3_TP_PHYS_TIMER = "CP_S3_TP_PHYS_TIMER";

        /// <summary>
        /// Time between two periodically transmitted tester present requests (functional request without positive response handling). 
        /// </summary>
        public static readonly string S3_TP_FUNC_TIMER = "CP_S3_TP_FUNC_TIMER";

        /// <summary>
        /// Specifies the segment size used during upload or download of flash files. 
        /// </summary>
        public static readonly string SEGMENTSIZE = "CP_SEGMENTSIZE";

        /// <summary>
        /// Minimum time for a flowcontrol from the ecu. 
        /// </summary>
        public static readonly string BS_MIN = "CP_BS_MIN";

        /// <summary>
        /// Maximum time for a flowcontrol from the ecu. 
        /// </summary>
        public static readonly string BS_MAX = "CP_BS_MAX";

        /// <summary>
        /// Time between two (2) consecutive frames from tester. Corresponds with the value STmin of the last flowcontrol of the ecu. 
        /// </summary>
        public static readonly string CS_SUG = "CP_CS_SUG";

        /// <summary>
        /// Time between firstframe from the ecu and the following flowcontrol of the tester. 
        /// </summary>
        public static readonly string BR_SUG = "CP_BR_SUG";

        /// <summary>
        /// Minimum inter frame time of consecutive frames of the ecu. 
        /// </summary>
        public static readonly string CS_MIN = "CP_CS_MIN";

        /// <summary>
        /// Expected cycle time for consecutive frames of a response. Transmitted in STmin of the tester flowcontrol. 
        /// </summary>
        public static readonly string STMIN_SUG = "CP_STMIN_SUG";

        /// <summary>
        /// Maximum inter frame time of consecutive frames of the ecu. 
        /// </summary>
        public static readonly string CS_MAX = "CP_CS_MAX";

        /// <summary>
        /// Number of used bytes in a can telegram. Shall always be 8 
        /// </summary>
        public static readonly string USED_CANDATA_LEN = "CP_USED_CANDATA_LEN";

        /// <summary>
        /// No documentation available
        /// </summary>
        public static readonly string MIRROR_MEMORY_CORRECTION = "CP_MIRROR_MEMORY_CORRECTION";

        /// <summary>
        /// Used baudrate for communication with the ecu. 
        /// </summary>
        public static readonly string BAUDRATE = "CP_BAUDRATE";

        /// <summary>
        /// Filter for monitoring: Bit=1 means "must match" identifier in CP_CANMONITORINGIDENTIFIER, Bit=0 means "dont care". 
        /// </summary>
        public static readonly string CANMONITORINGFILTER = "CP_CANMONITORINGFILTER";

        /// <summary>
        /// Describes identifier for can monitoring. 
        /// </summary>
        public static readonly string CANMONITORINGIDENTIFIER = "CP_CANMONITORINGIDENTIFIER";

        /// <summary>
        /// Can identifier for functional initialization, testerpresent and diagnostic end. Only active if Addressmode = functional 
        /// </summary>
        public static readonly string GLOBAL_REQUEST_CANIDENTIFIER = "CP_GLOBAL_REQUEST_CANIDENTIFIER";

        /// <summary>
        /// No documentation available
        /// </summary>
        public static readonly string TESTERPRESENT_MESSAGE = "CP_TESTERPRESENT_MESSAGE";

        /// <summary>
        /// Used CAN identifier for requests. 
        /// </summary>
        public static readonly string REQUEST_CANIDENTIFIER = "CP_REQUEST_CANIDENTIFIER";

        /// <summary>
        /// Used CAN identifier for responses. 
        /// </summary>
        public static readonly string RESPONSE_CANIDENTIFIER = "CP_RESPONSE_CANIDENTIFIER";

        /// <summary>
        /// Can identifier for functional requests. Only used if CP_REQUESTTYPE=RT_FUNCTIONAL 
        /// </summary>
        public static readonly string FUNCTIONAL_REQUEST_CANIDENTIFIER = "CP_FUNCTIONAL_REQUEST_CANIDENTIFIER";

        /// <summary>
        /// Physical Response CAN identifier required for ROE LITE feature (Response On Event). Specifies the CANID for periodical responses from ECU. 
        /// </summary>
        public static readonly string ROE_RESPONSE_CANIDENTIFIER = "CP_ROE_RESPONSE_CANIDENTIFIER";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_1 = "CP_SPARE_1";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_2 = "CP_SPARE_2";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_3 = "CP_SPARE_3";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_4 = "CP_SPARE_4";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_5 = "CP_SPARE_5";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_6 = "CP_SPARE_6";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_7 = "CP_SPARE_7";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_8 = "CP_SPARE_8";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_9 = "CP_SPARE_9";

        /// <summary>
        /// Spare parameter used during development process. 
        /// </summary>
        public static readonly string SPARE_10 = "CP_SPARE_10";

        /// <summary>
        /// Maximum time for a flowcontrol from the ecu.
        /// </summary>
        public static readonly string B12_REQ_MAX = "CP_B12_REQ_MAX";

        /// <summary>
        /// Minimum time for a flowcontrol from the ecu.
        /// </summary>
        public static readonly string B12_REQ_MIN = "CP_B12_REQ_MIN";

        /// <summary>
        /// Time between firstframe from the ecu and the following flowcontrol of the tester.
        /// </summary>
        public static readonly string B12_RESP_SUG = "CP_B12_RESP_SUG";

        /// <summary>
        /// Description of bus topology. Each bit represents a specific bus system.  
        /// </summary>
        public static readonly string BUSTOPOLOGY = "CP_BUSTOPOLOGY";

        /// <summary>
        /// Time between two (2) consecutive frames from tester. Corresponds with the value STmin of the last flowcontrol of the ecu.
        /// </summary>
        public static readonly string C_REQ_SUG = "CP_C_REQ_SUG";

        /// <summary>
        /// Minimum cycle time of consecutive frames of the ecu. Set by the gpd to CP_STMIN_SUG-2.
        /// </summary>
        public static readonly string C_RESP_MIN = "CP_C_RESP_MIN";

        /// <summary>
        /// Maximum cycle time of consecutive frames of the ecu. Set by the gpd to CP_STMIN_SUG+20.
        /// </summary>
        public static readonly string C_RESP_MAX = "CP_C_RESP_MAX";

        /// <summary>
        /// Flag for checking consecutive frames.
        /// </summary>
        public static readonly string CHECK_PCI_CF = "CP_CHECK_PCI_CF";

        /// <summary>
        /// Flag for accepting a singleframe instead of a firstframe.
        /// </summary>
        public static readonly string CHECK_PCI_FF_SF = "CP_CHECK_PCI_FF_SF";

        /// <summary>
        /// For OBD mode $03 it is the expected number of DTCs. Is read by mode $01, pid $01 and set by the gpd.
        /// </summary>
        public static readonly string DTCCOUNTOFFSET = "CP_DTCCOUNTOFFSET";

        /// <summary>
        /// Default value for the time between the two StartDiagnosticSession-DCXDiagnosticSession (10 92) in functional initialization sequence.
        /// </summary>
        public static readonly string GLOBAL_FIRST_R1_SUG = "CP_GLOBAL_FIRST_R1_SUG";

        /// <summary>
        /// Time between two functional testerpresent.
        /// </summary>
        public static readonly string GLOBAL_R1_SUG = "CP_GLOBAL_R1_SUG";

        /// <summary>
        /// Maximum time between request and response.
        /// </summary>
        public static readonly string P2_CAN_MAX = "CP_P2_CAN_MAX";

        /// <summary>
        /// Minimum time between request and response.
        /// </summary>
        public static readonly string P2_CAN_MIN = "CP_P2_CAN_MIN";

        /// <summary>
        /// Used time before a request is repeated after a negative response 7F xx 21.
        /// </summary>
        public static readonly string P3_7F_21 = "CP_P3_7F_21";

        /// <summary>
        /// Maximum time before a request is send after the last response. After that time the ecu stops diagnostic mode. Set after receiving a negative response with error code $78 (ReqCorrectlyRcvd_RspPending).
        /// </summary>
        public static readonly string P3_MAX = "CP_P3_MAX";

        /// <summary>
        /// Used time before a request is send after the last response or timeout.
        /// </summary>
        public static readonly string P3_SUG = "CP_P3_SUG";

        /// <summary>
        /// Time between response and the next tester present request. (UsedRequestTimeForPhysicalTesterPresent)
        /// </summary>
        public static readonly string R1_SUG = "CP_R1_SUG";

        /// <summary>
        /// Not used in CAN - Has to be PHYSICAL! 
        /// Values: FUNCTIONAL,1;PHYSICAL,2;NONE,0 : This enum is intentionally not defined
        /// </summary>
        public static readonly string RESTHEADERMODE = "CP_RESTHEADERMODE";

        /// <summary>
        /// Can identifier for "Response on Event" Lite single frame responses from ECU.
        /// </summary>
        public static readonly string ROE_CANIDENTIFIER = "CP_ROE_CANIDENTIFIER";

        /// <summary>
        /// Not used on CAN
        /// </summary>
        public static readonly string TRIGADDRESS = "CP_TRIGADDRESS";

        /// <summary>
        /// Description of supported local identifiers $9x for service ReadEcuIdentification ($1A). Each bit represents a specific local identifier. Bit 0 corresponds to $90 ... Bit 15 to $9F.  
        /// "1A 9x Support"
        /// </summary>
        public static readonly string USE_EXTENDED_ECU_IDENTIFICATION = "CP_USE_EXTENDED_ECU_IDENTIFICATION";

        /// <summary>
        /// Use Extended Requested Supported DTCs
        /// </summary>
        public static readonly string USE_EXTENDED_REQUESTED_SUPPORTED_DTCS = "CP_USE_EXTENDED_REQUESTED_SUPPORTED_DTCS";

        /// <summary>
        /// Number of bytes to download to the ecu. Set by the gpd from the values transmitted in RequestDownload request. Has to be set by the application if data are compressed. Only used for automatic or semiautomatic download.  (UncompressedDownloadMemorySize)
        /// </summary>
        public static readonly string I_DOWNLOADSIZE = "CPI_DOWNLOADSIZE";

        /// <summary>
        /// Describes the kind of communication end. For NOEXIT no StopCommunication request ($82) is generated. Testerpresent is also stopped.
        /// </summary>
        public static readonly string I_EXITTYPE = "CPI_EXITTYPE";

        /// <summary>
        /// SEMIAUTOMATIC, AUTOMATIC: Only usable with PAL functions PALFlashProg and PALUpload. Switches on the automatical up/download. Data bytes are transmitted over an internal buffer between master and slave. AUTOMATIC: After starting with RequestUpload/Download the data are transmitted/received with automatically generated TransferData requests between tester and ecu. Transmission will be stopped by a automatically generated TransferExit service. SEMIAUTOMATIC: The services RequestUp/Download, TransferData, TransferExit are started from the PAL functions. 
        /// MANUELL: All services are started by the application.
        /// Values: MANUELL,2;AUTOMATIC,1;SEMIAUTOMATIC,0
        /// </summary>
        public static readonly string I_GPDAUTODOWNLOAD = "CPI_GPDAUTODOWNLOAD";

        /// <summary>
        /// Decribes the kind of initialization. NOINIT: WakeUpPattern and Service StartCommunication are not generated. Communicaton starts with testerpresent ($3E). If the gpd receives a positive or negative response the communication is started. DEFAULTINIT: WakeUpPattern and service StartCommunication are generated.
        /// Values: DEFAULTINIT,1;NOINIT,0
        /// </summary>
        public static readonly string I_INITTYPE = "CPI_INITTYPE";

        /// <summary>
        /// Determine if ReadIdentification has to be done by CAESAR master during initialization sequence.  (UseReadIdentificationDuringInit)
        /// </summary>
        public static readonly string I_READIDBLOCK = "CPI_READIDBLOCK";

        /// <summary>
        /// Not used on CAN
        /// </summary>
        public static readonly string I_READTIMING = "CPI_READTIMING";

        /// <summary>
        /// Number of repetitions for negative response code BusyRepeatRequest (7F xx 21), ResponsePending (7F xx 78). The comparameter is read again from the variant buffer before each repetition and if the result is lower than the internal value then the new value is used. So it is possible to stop the repetition by the application. The repetition stops if the value becomes 0.  (Busy/PendingCounter)
        /// </summary>
        public static readonly string I_ROUTINECOUNTER = "CPI_ROUTINECOUNTER";

        /// <summary>
        /// Number of bytes to upload from the ecu. Set by the gpd from the values transmitted in RequestUpload request. Has to be set by the application if data are compressed. Only used for automatic or semiautomatic upload.  (UncompressedUploadMemorySize)
        /// </summary>
        public static readonly string I_UPLOADSIZE = "CPI_UPLOADSIZE";




        // ASAM-style parameters, normally not defined in CBF, and is instead mapped through ParameterMapping



        /// <summary>
        /// Used baudrate for communication with the ecu. 
        /// </summary>
        public static readonly string Baudrate = "CP_Baudrate"; // mapped to baudrate

        /// <summary>
        /// Maximum time between request and response.
        /// </summary>
        public static readonly string P2Max = "CP_P2Max";

        /// <summary>
        /// Used time before a request is send after the last response or timeout.
        /// </summary>
        public static readonly string P3Min = "CP_P3Min";

        /// <summary>
        /// Flag for sending testerpresent or not. 
        /// </summary>
        public static readonly string TesterPresentHandling = "CP_TesterPresentHandling"; // testerpresent 0: enable !0: disable 

        /// <summary>
        /// Used CAN identifier for requests.
        /// </summary>
        public static readonly string CanPhysReqId = "CP_CanPhysReqId"; // actual target tx addr e.g. 0x5B4

        /// <summary>
        /// Can identifier for functional requests. Only used if CP_REQUESTTYPE=RT_FUNCTIONAL 
        /// </summary>
        public static readonly string CanFuncReqId = "CP_CanFuncReqId"; // global addr e.g. 0x1C

        /// <summary>
        /// Used CAN identifier for responses. 
        /// </summary>
        public static readonly string CanRespUSDTId = "CP_CanRespUSDTId"; // actual target rx addr e.g. 0x4F4

        /// <summary>
        /// Specifies if the service is sent functional or physical. It doesn't say anything about the expected response behavior of the ECU. For physical requests, the CAN-Ids CP_REQUEST_CANIDENTIFIER and CP_RESPONSE_CANIDENTIFIER are applied. 
        /// For functional requests, the CAN-Id CP_FUNCTIONAL_REQUEST_CANIDENTIFIER is applied. CP_REQUESTTYPE is not used for initialization messages automatically generated by the protocol module. 
        /// 
        /// There is a special handling with additional parameters for all messages which are generated automatically by the protocol module during initialization and at the end of a diagnostic session. 
        /// In this case the decision (physical or functional addressing) is derived from the ECU-class (CP_CANECU_CLASS). The CAN-Id which is applied in this case is taken from CP_GLOBAL_REQUEST_CANIDENTIFIER.  
        /// Values: RT_PHYSICAL, RT_FUNCTIONAL, RT_FUNCTIONAL_WITH_RESPONSE 
        /// </summary>
        public static readonly string RequestAddrMode = "CP_RequestAddrMode";

        /// <summary>
        /// Time between response and the next tester present request. (UsedRequestTimeForPhysicalTesterPresent)
        /// </summary>
        public static readonly string TesterPresentTime = "CP_TesterPresentTime"; // tp interval, seconds

        /// <summary>
        /// Number of attempts to send a request in case of a transmission error (e.g. timeout). 
        /// </summary>
        public static readonly string RepeatReqCountApp = "CP_RepeatReqCountApp";

        /// <summary>
        /// Describes how many can messages are to be received between two (2) flowcontrol frames. Corresponds to BSmax at the flowcontrol frame of the tester. 
        /// </summary>
        public static readonly string BlockSize = "CP_BlockSize"; // Parameter.ISO15765_BS : 8

        /// <summary>
        /// Maximum time for a flowcontrol from the ecu.
        /// </summary>
        public static readonly string Bs = "CP_Bs"; // b12 req max

        /// <summary>
        /// Time between firstframe from the ecu and the following flowcontrol of the tester.
        /// </summary>
        public static readonly string Br = "CP_Br"; // b12 resp sug

        /// <summary>
        /// Maximum cycle time of consecutive frames of the ecu. Set by the gpd to CP_STMIN_SUG+20.
        /// </summary>
        public static readonly string Cr = "CP_Cr"; // c_resp_max

        /// <summary>
        /// Expected cycle time for consecutive frames of a response. Transmitted in STmin of the tester flowcontrol. 
        /// J2534: Parameter.ISO15765_STMIN
        /// </summary>
        public static readonly string StMin = "CP_StMin"; // ISO-TP separation time (STmin) : 40

        /// <summary>
        /// Time between two (2) consecutive frames from tester. Corresponds with the value STmin of the last flowcontrol of the ecu.
        /// J2534: Parameter.STMIN_TX
        /// </summary>
        public static readonly string StMinOverride = "CP_StMinOverride"; // ISO-TP transmit separation time : expecting 2 , kw2:CP_C_REQ_SUG, uds:CS_SUG

        /// <summary>
        /// Currently unused in this library, mapped to CP_LOGICAL_ADDRESS_GATEWAY
        /// DoIP. The internal diagnostic address of the gateway in the vehicle. This parameter is used to address a gateway if there is more than one gateway in the vehicle.  
        /// </summary>
        public static readonly string LogicalAddressGateway = "CP_LogicalAddressGateway"; // CP_LOGICAL_ADDRESS_GATEWAY

        /// <summary>
        /// Currently unused in this library, mapped to CP_LOGICAL_SOURCE_ADDRESS
        /// DoIP. The source address of the Tester. This is the logical address as defined by the OEM.
        /// </summary>
        public static readonly string LogicalSourceAddress = "CP_LogicalSourceAddress"; // unassigned

        /// <summary>
        /// Currently unused in this library, mapped to CP_LOGICAL_TARGET_ADDRESS
        /// DoIP. The target address of the ECU to communicate with. This is the logical address as defined by the OEM.
        /// </summary>
        public static readonly string LogicalTargetAddress = "CP_LogicalTargetAddress"; // unassigned
        
        /// <summary>
        /// Time delay caused by the CAN network. Time covers complete transmission time for a signal from tester to ecu and backwards (Time= Ttx + Trx). 
        /// </summary>
        public static readonly string CanTransmissionTime = "CP_CanTransmissionTime";

        /// <summary>
        /// Used time before a request is send after successful transmission  of the previous request in case of functional addressing (does not apply to functional TP request). 
        /// </summary>
        public static readonly string P3Func = "CP_P3Func";

        /// <summary>
        /// Used time before a next request is send after successful transmission  of the previous request, in case of physical addressing and no response required. 
        /// </summary>
        public static readonly string P3Phys = "CP_P3Phys";

        /// <summary>
        /// TRUE: Timing values received from ECU (with Response on Diagnostic Session Control) are used by the tester. FALSE: The timing values received by the ECU are ignored by the tester. 
        /// </summary>
        public static readonly string ModifyTiming = "CP_ModifyTiming";

    }
}

/*

// Completely unimplemented ComParams, should be mostly kline-era stuff

CP_CYCLETIME
CP_CAN_ID
CP_REPEATCOUNTER
CP_DLC_DATALENGTH_TO_SEND
CP_STROBES
CP_MSGCOUNTER_1_STARTBIT
CP_MSGCOUNTER_1_BITLENGTH
CP_PARITY_1_STARTBIT
CP_PARITY_1_BITLENGTH
CP_PARITY_1_TARGETBIT
CP_PARITY_1_TYPE
CP_PARITY_2_STARTBIT
CP_PARITY_2_BITLENGTH
CP_PARITY_2_TARGETBIT
CP_PARITY_2_TYPE
CP_PARITY_3_STARTBIT
CP_PARITY_3_BITLENGTH
CP_PARITY_3_TARGETBIT
CP_PARITY_3_TYPE
CP_PARITY_4_STARTBIT
CP_PARITY_4_BITLENGTH
CP_PARITY_4_TARGETBIT
CP_PARITY_4_TYPE
CP_CRC_1_STARTBYTE
CP_CRC_1_BYTELENGTH
CP_CRC_1_TARGETBYTE
CP_PARITY
CP_DATABITS
CP_STOPBITS
CP_INTERBYTETIMEMAX
CP_NEWBAUDRATE
CP_ECUSPECIFICBAUDRATE
CP_CANIDENTIFIER
CP_WAKEUP_CANIDENTIFIER
CP_TIME_MAX
CP_P3_CL15_WAKEUP
CP_BYTEPOS
CP_BITPOS
CP_ONSTATE
CP_U_BITMASK_1
CP_U_BITMASK_2
CP_U_VALUE_1
CP_U_VALUE_2
CP_INITTYPE
CP_KEYWORD1
CP_KEYWORD2
CPI_CANMEMBERTABLEFROMECU
CPI_CANGATEWAYORSINGLEECU
CP_DISABLE_INS_FASTINIT
CP_DISABLE_INS_DIAGNOSTIC
CP_COM_OVER_INS
CP_INSTREQTARGETBYTE
CP_INSTRESPSOURCEBYTE
CP_INSTRESPONSEMASTER
CP_INSTHEADERMODE
CP_REQTARGETBYTE
CP_REQSOURCEBYTE
CP_RESPSOURCEBYTE
CP_RESPONSEMASTER
CP_RESPTARGETBYTE
CP_CHECK1STHEADER
CP_CHECK2NDHEADER
CP_CHECK3RDHEADER
CP_CHECKCHECKSUM
CP_CHECKINVERTADDRESS
CP_INVERTADDRESSPARITYCUT
CP_SECONDKEYPARITYCUT
CP_NEG_TESTERPRESENT
CP_COMSTATEREACT
CP_TIMEOUTW1REACT
CP_TIMEOUTW2REACT
CP_TIMEOUTW3REACT
CP_TIMEOUTW4REACT
CP_ADDRESSREACT
CP_BUILDINGREACT
CP_CHECKSUMREACT
CP_COLLISIONREACT
CP_TIMEOUTP1REACT
CP_TIMEOUTP2REACT
CP_TIMEOUTP3REACT
CP_TELFRAMEREACT
CP_BYTEFRAMEREACT
CP_BYTERECEIVEREACT
CP_RECEIVESYNCREACT
CP_5BAUD_BITTIME
CP_W5_SUG
CP_W1_MIN
CP_W1_MAX
CP_W2_MIN
CP_W2_MAX
CP_W3_MIN
CP_W3_MAX
CP_W4_MIN
CP_W4_MAX
CP_W4_SUG
CP_TH_STOP_SUG
CP_TH_TIMEOUT_SUG
CP_TWUP_SUG
CP_TINIL_SUG
CP_P1_MAX
CP_P2_MIN
CP_DEF_MBNFZ_P2_MIN
CP_DEF_KW2000_P2_MIN
CP_P2_MAX
CP_DEF_MBNFZ_P2_MAX
CP_DEF_KW2000_P2_MAX
CP_P3_MIN
CP_DEF_MBNFZ_P3_MIN
CP_DEF_KW2000_P3_MIN
CP_DEF_MBNFZ_P3_MAX
CP_DEF_KW2000_P3_MAX
CP_DEF_MBNFZ_P3_SUG
CP_DEF_KW2000_P3_SUG
CP_DEF_MBNFZ_R1_SUG
CP_DEF_KW2000_R1_SUG
CP_P4_SUG
CP_DEF_MBNFZ_P4_SUG
CP_PALFKT
CP_NETWORKLAYER
CP_NEW_NETWORKLAYER
CP_DEF_KW2000_P4_SUG
CP_TINC_SUG
CP_NO_DOWNLOADSIZE
CPI_INFOMESSAGE
CPI_MESSAGELOCK
CPI_RECEIVEFLUSH
CPI_ERRORLIST
CPI_MONITORINGINSESSION
CPI_MONITORINGBETWEENSESSION
CPI_MONITORINGTESTERPRESENT
CP_ERRORCODE21_REPEAT
CP_ERRORCODE23_REPEAT
CP_MODE22COUNT
CP_TWUP_SUG_US
CP_TINIL_SUG_US
CP_IGNORE_WAKEUP_TIMEOUT
CP_DELAY_AFTER_STARTCOM
CP_DELAY_TIME
CP_REQUEST_CANIDENTIFIER_15765
CP_RESPONSE_CANIDENTIFIER_15765
CP_D_REQ_SUG
CP_D_RESP_MIN
CP_D_RESP_MAX
CP_CHECKRESPONSEADDRESS
CP_COMMANDREACT
CP_CTG_ADDRESS
CP_ECU_CANIDENTIFIER
CP_INS_ADDRESS
CP_INS_NOT_PRESENT_RESET_ECU_REACTION
CP_INS_PRESENT_RESET_ECU_REACTION
CP_INS_PRESENT_RESET_INS_REACTION
CP_TESTERPRESENT_TYPE
CP_SINGLEWIREMODE_ALLOWED
CP_STUFFBYTE
CP_T1_MIN
CP_T1_MAX
CP_T3_MIN
CP_T3_SUG
CP_T3_MAX
CP_T4_AFTER_TIMEOUTT3
CP_T4_SUG
CP_T4_MAX
CP_TIMEOUTT1_ECU_REACT
CP_TIMEOUTT1_INS_REACT
CP_TIMEOUTT3REACT
CP_TRANSFERMODE_BYTEPOS
CP_TRANSFERMODE_BITPOS
CP_USE_STUFFBYTE
CP_WORKING_MODE
CP_TRIGHEADERMODE
CP_LENGTHINFO
CP_RESETBAUDRATE
CP_RESETTIMING
CP_DATASEGMENTATION
CP_MONITORING
CP_TESTERPRESENTADDRESS
CP_OBDREQTYPBYTE
CP_OBDRESPLENGTH
CP_OBDRESPTYPBYTE
CP_ROUTINEEXITBYTE
CP_MASTERSLAVEMODE
CP_TIMEOUTT1REACT
CP_TIMEOUTT2REACT
CP_TIMEOUTT4REACT
CP_TIMEOUTINCREACT
CP_TIMEOUTP6REACT
CP_T0_SUG
CP_T2_MAX
CP_T4_MIN
CP_T5_SUG
CP_P6_MIN
CP_P6_MAX
CP_HANDSHAKE
CP_MODE21_HANDSHAKE
CP_T5_MIN_GR10KBD
CP_T6_MIN_GR10KBD
CP_T0_MIN
CP_T0_MAX
CP_T1_SUG
CP_T2_MIN
CP_T2_SUG
CP_T5_MIN
CP_T5_MAX
CP_T6_MIN
CP_T6_MAX
CP_T6_SUG
CP_T40_MAX
CP_T40_SUG
CP_TIMEOUTT0REACT
CP_TIMEOUTT5REACT
CP_TIMEOUTT6REACT
CP_TIMEOUTT40REACT
CP_BYTE_PAUSE_RECEIVE
CP_BYTE_PAUSE_SEND
CPI_Massetastung
CPI_T_LOWLINE
CPI_16BITCHECKSUM 
 
*/