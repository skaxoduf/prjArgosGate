Public Module UCBioAPIConstants

    ' Constant for DeviceID
    Public Const UCBioAPI_DEVICE_ID_AUTO_DETECT As Integer = 255

    ' Window Style
    Public Const UCBioAPI_WINDOW_STYLE_POPUP As Integer = 0
    Public Const UCBioAPI_WINDOW_STYLE_INVISIBLE As Integer = 1
    Public Const UCBioAPI_WINDOW_STYLE_CONTINUOUS As Integer = 2
    Public Const UCBioAPI_WINDOW_STYLE_NO_FPIMG As Integer = 65536
    Public Const UCBioAPI_WINDOW_STYLE_TOPMOST As Integer = 131072
    Public Const UCBioAPI_WINDOW_STYLE_NO_WELCOME As Integer = 262144
    Public Const UCBioAPI_WINDOW_STYLE_NO_TOPMOST As Integer = 524288

    ' UCBioAPI_FIR_SECURITY_LEVEL
    Public Const UCBioAPI_FIR_SECURITY_LEVEL_LOWEST As Integer = 1
    Public Const UCBioAPI_FIR_SECURITY_LEVEL_LOWER As Integer = 2
    Public Const UCBioAPI_FIR_SECURITY_LEVEL_LOW As Integer = 3
    Public Const UCBioAPI_FIR_SECURITY_LEVEL_BELOW_NORMAL As Integer = 4
    Public Const UCBioAPI_FIR_SECURITY_LEVEL_NORMAL As Integer = 5
    Public Const UCBioAPI_FIR_SECURITY_LEVEL_ABOVE_NORMAL As Integer = 6
    Public Const UCBioAPI_FIR_SECURITY_LEVEL_HIGH As Integer = 7
    Public Const UCBioAPI_FIR_SECURITY_LEVEL_HIGHER As Integer = 8
    Public Const UCBioAPI_FIR_SECURITY_LEVEL_HIGHEST As Integer = 9

    'Error Code
    Public Const UCBioAPIERROR_NONE As Integer = 0
    Public Const UCBioAPIERROR_INVALID_HANDLE As Integer = 1
    Public Const UCBioAPIERROR_INVALID_POINTER As Integer = 2
    Public Const UCBioAPIERROR_INVALID_TYPE As Integer = 3
    Public Const UCBioAPIERROR_FUNCTION_FAIL As Integer = 4
    Public Const UCBioAPIERROR_STRUCTTYPE_NOT_MATCHED As Integer = 5
    Public Const UCBioAPIERROR_ALREADY_PROCESSED As Integer = 6
    Public Const UCBioAPIERROR_EXTRACTION_OPEN_FAIL As Integer = 7
    Public Const UCBioAPIERROR_VERIFICATION_OPEN_FAIL As Integer = 8
    Public Const UCBioAPIERROR_DATA_PROCESS_FAIL As Integer = 9
    Public Const UCBioAPIERROR_MUST_BE_PROCESSED_DATA As Integer = 10
    Public Const UCBioAPIERROR_INTERNAL_CHECKSUM_FAIL As Integer = 11
    Public Const UCBioAPIERROR_ENCRYPTED_DATA_ERROR As Integer = 12
    Public Const UCBioAPIERROR_UNKNOWN_FORMAT As Integer = 13
    Public Const UCBioAPIERROR_UNKNOWN_VERSION As Integer = 14
    Public Const UCBioAPIERROR_VALIDITY_FAIL As Integer = 15
    Public Const UCBioAPIERROR_INVALID_TEMPLATESIZE As Integer = 16
    Public Const UCBioAPIERROR_INVALID_TEMPLATE As Integer = 17
    Public Const UCBioAPIERROR_EXPIRED_VERSION As Integer = 18
    Public Const UCBioAPIERROR_INVALID_SAMPLESPERFINGER As Integer = 19
    Public Const UCBioAPIERROR_UNKNOWN_INPUTFORMAT As Integer = 20
    Public Const UCBioAPIERROR_INVALID_PARAMETER As Integer = 21
    Public Const UCBioAPIERROR_FUNCTION_NOT_SUPPORTED As Integer = 22
    Public Const UCBioAPIERROR_INIT_MAXFINGERSFORENROLL As Integer = 1
    Public Const UCBioAPIERROR_INIT_NECESSARYENROLLNUM As Integer = 2
    Public Const UCBioAPIERROR_INIT_SAMPLESPERFINGER As Integer = 3
    Public Const UCBioAPIERROR_INIT_SECULEVELFORENROLL As Integer = 4
    Public Const UCBioAPIERROR_INIT_SECULEVELFORVERIFY As Integer = 5
    Public Const UCBioAPIERROR_INIT_SECULEVELFORIDENTIFY As Integer = 6
    Public Const UCBioAPIERROR_INIT_RESERVED1 As Integer = 7
    Public Const UCBioAPIERROR_INIT_RESERVED2 As Integer = 8
    Public Const UCBioAPIERROR_INIT_TEMPLATE_FORMAT As Integer = 9
    Public Const UCBioAPIERROR_INIT_LIVEDETECTLEVEL As Integer = 10
    Public Const UCBioAPIERROR_DEVICE_OPEN_FAIL As Integer = 1
    Public Const UCBioAPIERROR_DEVICE_INVALID_DEVICE_ID As Integer = 2
    Public Const UCBioAPIERROR_DEVICE_WRONG_DEVICE_ID As Integer = 3
    Public Const UCBioAPIERROR_DEVICE_ALREADY_OPENED As Integer = 4
    Public Const UCBioAPIERROR_DEVICE_NOT_OPENED As Integer = 5
    Public Const UCBioAPIERROR_DEVICE_BRIGHTNESS As Integer = 6
    Public Const UCBioAPIERROR_DEVICE_CONTRAST As Integer = 7
    Public Const UCBioAPIERROR_DEVICE_GAIN As Integer = 8
    Public Const UCBioAPIERROR_USER_CANCEL As Integer = 1
    Public Const UCBioAPIERROR_USER_BACK As Integer = 2
    Public Const UCBioAPIERROR_CAPTURE_TIMEOUT As Integer = 3
    Public Const UCBioAPIERROR_CAPTURE_FAKE_SUSPICIOUS As Integer = 4
    Public Const UCBioAPIERROR_ENROLL_EVENT_PLACE As Integer = 5
    Public Const UCBioAPIERROR_ENROLL_EVENT_HOLD As Integer = 6
    Public Const UCBioAPIERROR_ENROLL_EVENT_REMOVE As Integer = 7
    Public Const UCBioAPIERROR_ENROLL_EVENT_PLACE_AGAIN As Integer = 8
    Public Const UCBioAPIERROR_ENROLL_EVENT_PROCESS As Integer = 9
    Public Const UCBioAPIERROR_ENROLL_EVENT_MATCH_FAILED As Integer = 10
    Public Const UCBioAPIERROR_FASTSEARCH_INIT_FAIL As Integer = 1
    Public Const UCBioAPIERROR_FASTSEARCH_SAVE_DB As Integer = 2
    Public Const UCBioAPIERROR_FASTSEARCH_LOAD_DB As Integer = 3
    Public Const UCBioAPIERROR_FASTSEARCH_UNKNOWN_VER As Integer = 4
    Public Const UCBioAPIERROR_FASTSEARCH_IDENTIFY_FAIL As Integer = 5
    Public Const UCBioAPIERROR_FASTSEARCH_DUPLICATED_ID As Integer = 6
    Public Const UCBioAPIERROR_FASTSEARCH_IDENTIFY_STOP As Integer = 7
    Public Const UCBioAPIERROR_FASTSEARCH_NOUSER_EXIST As Integer = 8
    Public Const UCBioAPIERROR_OPTIONAL_UUID_FAIL As Integer = 1
    Public Const UCBioAPIERROR_OPTIONAL_PIN1_FAIL As Integer = 2
    Public Const UCBioAPIERROR_OPTIONAL_PIN2_FAIL As Integer = 3
    Public Const UCBioAPIERROR_OPTIONAL_SITEID_FAIL As Integer = 4
    Public Const UCBioAPIERROR_OPTIONAL_EXPIRE_DATE_FAIL As Integer = 5
    Public Const UCBioAPIERROR_SC_FUNCTION_FAILED As Integer = 1
    Public Const UCBioAPIERROR_SC_NOT_SUPPORTED_DEVICE As Integer = 2
    Public Const UCBioAPIERROR_SC_NOT_SUPPORTED_FIRMWARE As Integer = 3

End Module