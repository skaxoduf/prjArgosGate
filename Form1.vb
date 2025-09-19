Imports System.ComponentModel
Imports System.IO
Imports System.IO.Ports
Imports System.Windows.Forms
Imports Microsoft.Web.WebView2.Core
Imports Microsoft.Web.WebView2.WinForms
Imports System.Text.RegularExpressions
Imports System.Reflection.Metadata
Imports System.Text.Json
Imports System.Threading
Imports System.Diagnostics
Imports System.Security.Permissions
Imports System.Runtime.InteropServices
Imports System.Security.Policy
Imports Microsoft.Data.SqlClient
Imports UCBioBSPCOMLib
Imports UCSAPICOMLib
Imports Windows.Win32.UI.Input



Public Class Form1


    Private gFormGb As String
    Private lastSyncTimestamp As DateTime = DateTime.MinValue
    Private WithEvents syncTimer As New System.Windows.Forms.Timer()

    ' 지문 관련 선언 
    Public WithEvents objUCSAPICOM As New UCSAPI()
    Public objTerminalUserData As ITerminalUserData
    Public objServerUserData As IServerUserData
    Public objAccessLogData As IAccessLogData
    Public objAccessControlData As IAccessControlData
    Public objServerAuthentication As IServerAuthentication
    Public objTerminalOption As ITerminalOption

    'UCBioBSP Object
    Public WithEvents objUCBioBSP As New UCBioBSP()
    Public objDevice As IDevice
    Public objExtraction As IExtraction
    Public objMatching As IMatching
    Public objFPData As IFPData
    Public objFPImage As IFPImage
    Public objFastSearch As IFastSearch

    Public szTextEnrolledFIR As String
    Private binaryEnrolledFIR() As Byte
    Private sFingerAuthUseYN As Boolean = True   ' 지문인증을 여기서 사용할건지 여부 플래그 
    Private sTestYN As Boolean = True   ' TEST 환경인지 플래그, 테스트 : True,  배포 : False

    'UCBioBSP Object-스마트카드
    Private objSmartCard As ISmartCard   ' RF카드용 선언 
    '지문 품질값 51~75 적절한, 76~100 우수한, 정상적인 매칭을 위해서는 76 이상에 지문사용필요함
    ' VB6의 Long (32비트)은 VB.NET의 Integer (32비트)에 해당함..
    Private gBioAPI_QUALITY As Integer


    ' 애니메이션을 위한 타이머 객체
    Private WithEvents TitleAnimationTimer As New System.Windows.Forms.Timer()

    ' 현재 점(.)의 개수를 저장할 변수
    Private dotCount As Integer = 1
    ' 원래 제목을 저장할 변수
    Private baseTitle As String = "Argos APT GateDemon 실행 중"

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        frmSplash.Close()
    End Sub
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load


        ' 타이틀 애니메이션 타이머 간격 설정 (400ms = 0.4초)
        TitleAnimationTimer.Interval = 400
        TitleAnimationTimer.Start()

        WebView21.Visible = False  ' 웹뷰는 숨긴다.
        pnlCSMain.Visible = True   ' 데몬 프로그램 패널을 보여준다.
        Await subFormLoad()


        '지문 dll 로드(지문등록을 위해 dll 로드 진행)
        subFingerLoad()

        If sFingerAuthUseYN = True Then ' 지문인증사용여부가 True이면 지문서버 시작
            Finger_Server_Start()
        End If


    End Sub
    Private Async Function subFormLoad() As Task

        ' 현재 모니터 해상도 가져오기 (웹뷰를 최대화 하지않음...)
        'Dim screenWidth As Integer = Screen.PrimaryScreen.Bounds.Width
        'Dim screenHeight As Integer = Screen.PrimaryScreen.Bounds.Height

        If Config_Load() = False Then
            gFormGb = "C"
        Else
            gFormGb = "W"
        End If

        ' 디비정보 읽어오기 (사용안함, 웹에서 Json으로 받아오기때문에)
        'If Config_Load2() = False Then
        ' MessageBox.Show("시스템 정보 확인 필요!", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ' End If

        ' 시리얼 포트번호가 있으면 시리얼에 연결한다.
        If IsNumeric(gSeralPortNo) = True Then
            modFunc.ConnectSerialPort("COM" + gSeralPortNo, 9600)
            AddHandler modFunc.serialPort.DataReceived, AddressOf HandleSerialData
        End If

        ' 폼 크기 설정 (웹뷰를 최대화 하지않음...)
        'Me.Width = screenWidth
        'Me.Height = screenHeight
        'Me.WindowState = FormWindowState.Maximized 

        WebView21.Left = 0
        WebView21.Top = 0

        ' CS 최초 로딩될때는 WebView로 웹포스를 호출해주고 
        Dim url As String = "http://julist.webpos.co.kr/login/"
        Await WebView21.EnsureCoreWebView2Async(Nothing)

        'WebView21.Width = Me.ClientSize.Width
        'WebView21.Height = Me.ClientSize.Height
        WebView21.Source = New Uri(url)

        ' CS 웹뷰는 폼 로딩될때 자바스크립트로부터 수신받을 준비를 한다.
        RemoveHandler WebView21.WebMessageReceived, AddressOf WebView21_WebMessageReceived
        AddHandler WebView21.WebMessageReceived, AddressOf WebView21_WebMessageReceived

        ' 폼 로딩될때 CS 웹뷰는 자바스크립트에다가 아무 액션도 하지 않는다.
        ' 테스트시 주석해제 , 배포시 주석처리
        'RemoveHandler WebView21.NavigationCompleted, AddressOf WebView_NavigationCompleted
        'AddHandler WebView21.NavigationCompleted, AddressOf WebView_NavigationCompleted

    End Function
    Private Async Sub WebView21_WebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs)
        '자바스크립트에서 받는부분 
        Try
            Dim receivedJson As String = e.WebMessageAsJson

            ' JSON이 문자열로 감싸져 있으면, 파싱하여 JSON 객체로 변환
            If receivedJson.StartsWith("""") AndAlso receivedJson.EndsWith("""") Then
                receivedJson = JsonDocument.Parse(receivedJson.Trim(""""c)).RootElement.GetRawText()
            End If

            Dim doc As JsonDocument = JsonDocument.Parse(receivedJson)
            Dim data As JsonElement = doc.RootElement

            If data.TryGetProperty("call", Nothing) Then
                Dim methodName = data.GetProperty("call").GetString()

                Select Case methodName

                    Case "Get_DBInfo"   ' 로그인 메인 페이지의 fnWebCsDbInfoSetter 함수
                        If data.TryGetProperty("dbInfo", Nothing) Then
                            Dim dbInfoJson As String = data.GetProperty("dbInfo").GetRawText()
                            Await Get_DBInfo(dbInfoJson)    '웹으로부터 디비접속정보를 Json 문자열로 받아서 전역변수에 담는 함수

                            If sFingerAuthUseYN = True Then  ' 지문인증사용여부가 True일 경우에만 
                                ' 디비접속정보를 가져왔다면 지문 테이블에서 데이타 가져와서  objFastSearch 모듈에 지문 탬플릿 데이타 등록을 진행한다.
                                ' 사용자 지문인증을 위한 유니온 고속검색엔진dll에 지문탬플릿을 로드하는 작업 
                                LoadAllFingerprintsFromDB()

                                ' 지문인증을 위해 30초마다 갱신 체크하는 시간 변수 초기화
                                lastSyncTimestamp = DateTime.Now.AddSeconds(-5)
                                syncTimer.Interval = 30000   ' 30초
                                syncTimer.Start()
                            End If
                        End If

                    Case Else
                        MessageBox.Show("알 수 없는 call 메서드: " & methodName)
                        WriteLog("알 수 없는 call 메서드: " & methodName, LOG_TO_FILE, LOG_FILE_NAME)
                End Select
            End If

        Catch ex As Exception
            MessageBox.Show("메시지 수신 중 오류 발생: " & ex.Message & vbCrLf & "받은 데이터: " & e.WebMessageAsJson,
                            "오류", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Function Config_Load() As Boolean

        Config_Load = True
        Try
            gAppPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), INI_FILENAME)
            gPosNo = GetIni("Settings", "PosNo", gAppPath)
            gCompanyCode = GetIni("Settings", "CompanyCode", gAppPath)
            gSeralPortNo = GetIni("Settings", "SerialPortNo", gAppPath)

            If IsNumeric(gPosNo) = False Or gCompanyCode = "" Then
                'MessageBox.Show("포스번호가 잘못되었습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Config_Load = False
            End If
        Catch ex As Exception
            Config_Load = False
        End Try

    End Function
    Private Function Config_Load2() As Boolean
        ' 웹에서 json으로 디비접속정보를 받기때문에 이 함수는 사용안함. 혹시몰라 임시로 남겨둠..

        Config_Load2 = True
        Try
            ' 폼 로딩시 디비정보 전역함수에 저장
            Dim systemPath As String = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
            gAppPath2 = System.IO.Path.Combine(systemPath, INI_DB_FILENAME)

            Dim server As String = DecryptString(GetIni("DATABASE", "Server", gAppPath2))
            Dim database As String = DecryptString(GetIni("DATABASE", "DBName", gAppPath2))
            Dim user As String = DecryptString(GetIni("DATABASE", "UserID", gAppPath2))
            Dim pass As String = DecryptString(GetIni("DATABASE", "Password", gAppPath2))


            If String.IsNullOrEmpty(server) Then
                server = "175.117.144.57,11433"
                database = "WEB_POS"
                user = "sa"
                pass = "julist1101@nate.com"
                PutIni("DATABASE", "Server", EncryptString(server), gAppPath2)
                PutIni("DATABASE", "DBName", EncryptString(database), gAppPath2)
                PutIni("DATABASE", "UserID", EncryptString(user), gAppPath2)
                PutIni("DATABASE", "Password", EncryptString(pass), gAppPath2)
            End If

            modDBConn.ConnectionString = $"Data Source={server};Initial Catalog={database};User ID={user};Password={pass};TrustServerCertificate=True"

            Config_Load2 = True
        Catch ex As Exception
            Config_Load2 = False
        End Try

    End Function
    Public Async Function Get_DBInfo(ByVal jsonStr As String) As Task
        Try
            Using doc As JsonDocument = JsonDocument.Parse(jsonStr)
                Dim root As JsonElement = doc.RootElement

                gServer = GetJsonString(root, "server")
                gDatabase = GetJsonString(root, "database")
                gUser = GetJsonString(root, "user")
                gPass = GetJsonString(root, "pass")

                '전역변수에 담긴 디비정보로 디비접속문자열 생성
                modDBConn.ConnectionString = $"Data Source={gServer};Initial Catalog={gDatabase};User ID={gUser};Password={gPass};TrustServerCertificate=True"

                ' MessageBox.Show($"DB 정보 수신 완료:" & vbCrLf &
                '                 $"Server: {gServer}" & vbCrLf &
                '                 $"Database: {gDatabase}" & vbCrLf &
                '                 $"User: {gUser}", "DB 정보 설정 완료")

            End Using
        Catch ex As Exception
            MessageBox.Show("DB 정보 처리 중 오류 발생: " & ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function
    Private Sub subFingerLoad()

        Try
            '// Create UCSCOM object
            objUCSAPICOM = New UCSAPI()
            objTerminalUserData = objUCSAPICOM.TerminalUserData
            objServerUserData = objUCSAPICOM.ServerUserData
            objAccessLogData = objUCSAPICOM.AccessLogData
            objAccessControlData = objUCSAPICOM.AccessControlData
            objServerAuthentication = objUCSAPICOM.ServerAuthentication
            objTerminalOption = objUCSAPICOM.TerminalOption

            '// Create UCBioBSP object
            objUCBioBSP = New UCBioBSP()
            objDevice = objUCBioBSP.Device
            objExtraction = objUCBioBSP.Extraction
            objMatching = objUCBioBSP.Matching
            objFPData = objUCBioBSP.FPData
            objFPImage = objUCBioBSP.FPImage
            objFastSearch = objUCBioBSP.FastSearch '//지문인증용
            objSmartCard = objUCBioBSP.SmartCard '//RF카드 인식용
            objDevice.Enumerate()  ' 현재 pc에 연결된 지문장치 목록을 가져옴
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    ' 지문데이타를 DB에서 가져와서 objFastSearch 에 등록하는 함수
    Private Sub LoadAllFingerprintsFromDB()

        Dim logMessage As String

        logMessage = $"[{DateTime.Now:HH:mm:ss}] 사용자 지문 데이타 로딩 중...."
        WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)

        Using conn As SqlConnection = modDBConn.GetConnection()
            If conn Is Nothing Then Return

            Dim sql As String = "SELECT F_MEM_IDX, F_FINGER FROM T_MEM_PHOTO WHERE F_FINGER IS NOT NULL AND DATALENGTH(F_FINGER) > 0  " &
                                " AND F_COMPANY_CODE = @F_COMPANY_CODE "
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@F_COMPANY_CODE", gCompanyCode)  'gCompanyCode

                Try
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim img As Object = reader("F_FINGER")
                            Dim nUserID As Integer

                            If Not Convert.IsDBNull(img) Then
                                Dim lbytTemp As Byte() = DirectCast(img, Byte())
                                nUserID = If(Convert.IsDBNull(reader("F_MEM_IDX")), 0, Convert.ToInt32(reader("F_MEM_IDX")))
                                If nUserID > 0 Then
                                    ' 1. SDK가 인식할 수 있도록 데이터 파싱 및 파일 저장 (기존 로직 유지)
                                    ' 이 부분은 디버깅용도로 사용되므로 실제사용시에는 필요없기때문에 주석처리한다.
                                    'objFPData.Export(lbytTemp, 400)

                                    'If objUCBioBSP.ErrorCode = 0 Then
                                    '    Dim nFingerCnt As Integer = objFPData.TotalFingerCount
                                    '    Dim nSampleNum As Integer = objFPData.SampleNumber
                                    '    For f As Integer = 0 To nFingerCnt - 1
                                    '        Dim nFingerID As Integer = objFPData.FingerID(f)
                                    '        For s As Integer = 0 To nSampleNum - 1
                                    '            Dim biTemplate As Byte() = objFPData.FPSampleData(nFingerID, s)
                                    '            Dim szFileName As String = Path.Combine(Application.StartupPath, nUserID.ToString() & ".uct")
                                    '            SaveImageFromDb(biTemplate, szFileName)
                                    '        Next
                                    '    Next
                                    'Else
                                    '    ' Export 오류 발생 시 콘솔에 로그를 남깁니다.
                                    '    Console.WriteLine("FPData.Export Error: " & objUCBioBSP.ErrorDescription)
                                    'End If

                                    ' 2. 고속 검색 엔진에 지문 정보 등록 (이 부분이 중요....지문검색엔진 모듈객체에 등록을 해야 인증을 할때 얘를 갖다가 비교를 한다.)
                                    objFastSearch.RemoveUser(nUserID) ' 기존 정보가 있다면 삭제
                                    objFastSearch.AddFIR(lbytTemp, nUserID) ' 새 정보 추가

                                    logMessage = $"[{DateTime.Now:HH:mm:ss}] " + "지문 DLL 로드 성공 : " + Convert.ToString(nUserID)
                                    WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)

                                    If objUCBioBSP.ErrorCode <> 0 Then ' UCBioAPIERROR_NONE = 0
                                        logMessage = $"[{DateTime.Now:HH:mm:ss}] " + objUCBioBSP.ErrorDescription & " [" & objUCBioBSP.ErrorCode & "]"
                                        WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
                                        Return
                                    End If
                                End If
                            End If
                        End While
                    End Using
                Catch ex As Exception
                    logMessage = $"[{DateTime.Now:HH:mm:ss}] " + "데이터 처리 중 오류 발생: " & ex.Message
                    WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
                End Try
            End Using
        End Using ' 이 지점에서 conn 객체는 자동으로 Close 및 Dispose 됩니다.

        logMessage = $"[{DateTime.Now:HH:mm:ss}] 지문 인증서버 작동 중...."
        WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)

    End Sub
    Private Sub Finger_Server_Start()
        Dim logMessage As String
        Try
            objUCSAPICOM.ServerStart(20, 9870)   ' 9870은 지문인증장비 기본포트값인데 실제 지문인증장비에서 설정된 포트값과 같아야함
            If objUCSAPICOM.ErrorCode <> 0 Then
                ' MessageBox.Show("지문장비 초기화 중 오류 발생: " & objUCSAPICOM.ErrorCode, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error)
                logMessage = $"[{DateTime.Now:HH:mm:ss}] 지문 인증서버 시작 중 오류 발생"
                WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
            Else
                logMessage = $"[{DateTime.Now:HH:mm:ss}] 지문 인증서버 시작 중...."
                WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
            End If
        Catch ex As Exception
            logMessage = $"[{ex.Message}]"
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
        End Try
    End Sub
    Private Sub Finger_Server_Stop()
        Dim logMessage As String
        Try
            objUCSAPICOM.ServerStop()
            logMessage = $"[{DateTime.Now:HH:mm:ss}] 지문 인증서버 종료 중...."
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
        Catch ex As Exception
            logMessage = $"[{ex.Message}]"
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
        End Try
    End Sub
    Private Sub objUCSAPICOM_EventVerifyFinger1toN(TerminalID As Integer, AuthMode As Integer, InputIDLength As Integer, SecurityLevel As Integer, AntipassbackLevel As Integer, FingerData As Object) Handles objUCSAPICOM.EventVerifyFinger1toN


        '//ErrorCode 설명
        '// 769:미등록사용자, 770:매칭실패, 771:권한없음, 772:지문Capture 실패
        '// 773:인증실패, 774:패스백
        '// 775:권한없음(네트워크 문제로 서버로부터 응답없음)
        '// 776:권한없음(서버가 Busy 상태로 인증을 수행 할수 없음)
        '// 777:얼굴이 인지되지 않았습니다.

        Try

            '// --- 지문 단일 인증 (1:N 매칭) 로직 시작 ---

            ' Variant(Object)로 받은 지문 데이터를 Byte 배열로 명시적 형변환
            Dim fingerDataBytes As Byte() = DirectCast(FingerData, Byte())
            Dim logMessage As String = $"FIR 변환 시작"
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)

            ' FIR(지문 템플릿)로 변환
            objFPData.Import(1, 1, 2, 400, 400, fingerDataBytes, 0)

            Dim biInputFingerData As Byte() = DirectCast(objFPData.FIR, Byte())
            logMessage = $"FIR 변환 완료"
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)

            ' 메모리에 로드된 지문 정보와 비교하여 사용자 검색
            objFastSearch.MaxSearchTime = 0 ' 0 = 검색제한시간 : 무제한
            logMessage = $"사용자 검색 시작"
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)

            objFastSearch.IdentifyUser(biInputFingerData, UCBioAPI_FIR_SECURITY_LEVEL_NORMAL)
            logMessage = $"사용자 검색 완료"
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)

            Dim isAuthorized As Integer
            Dim isAcessibility As Integer = 1
            Dim isVistor As Integer = 0
            Dim isUserID As Integer
            Dim sErrorCode As Integer

            If objUCBioBSP.ErrorCode = 0 Then
                ' 매칭 성공
                Dim objMatchedFpInfo As ITemplateInfo = objFastSearch.MatchedFpInfo

                If objUCBioBSP.ErrorCode = 0 Then
                    isUserID = objMatchedFpInfo.UserID
                    isAuthorized = 1 ' 인증 성공
                    sErrorCode = 0

                    logMessage = $"1차 인증 성공: UserID({isUserID}), FingerID({objMatchedFpInfo.FingerID}) 찾음"
                    WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)

                    ' 여기에서 비지니스 로직을 수행하거나 아니면 웹으로 인증결과만 넘겨주고 웹의 방문창을 띄우게 하거나...
                    ' -------------------비즈니스 로직(권한 확인) 시작-----------------------
                    If CheckUserAuthorizationFromDB(isUserID) Then
                        isAuthorized = 1 ' 인증 성공
                        sErrorCode = 0
                        logMessage = $"2차 인증 성공: 사용자 ID({isUserID})는 출입 권한이 있습니다."
                        WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
                    Else
                        isAuthorized = 0 ' 인증 실패
                        sErrorCode = 771 ' ErrorCode: 권한 없음
                        logMessage = $"2차 인증 실패: 사용자 ID({isUserID})는 출입 권한이 없습니다."
                        WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
                    End If
                    '------------------- 비즈니스 로직(권한 확인) 종료------------------------------
                Else
                    ' 매칭은 성공했으나, 매칭된 정보(UserID)를 가져오는 데 실패한 경우
                    isUserID = 0
                    isAuthorized = 0 ' 인증 실패
                    sErrorCode = objUCBioBSP.ErrorCode   ' 769 ' 미등록 사용자 또는 정보 조회 실패
                    logMessage = $"매칭 후 정보 조회 실패"
                    WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
                End If
            Else
                ' 매칭 실패
                isUserID = 0
                isAuthorized = 0 ' 인증 실패
                sErrorCode = objUCBioBSP.ErrorCode
                logMessage = $"매칭 실패: {objUCBioBSP.ErrorDescription} [{sErrorCode}]"
                WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
            End If

            ' --- 인증 결과 전송 ---
            logMessage = $"인증 결과 전송"
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
            Dim txtEventTime As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

            ' 인증 타입 설정 (단일 인증)
            objServerAuthentication.SetAuthType(0, 1, 0, 0, 1, 0)
            ' 터미널로 최종 인증 결과 전송 -- 전송되면 접점신호가 발생한다.
            objServerAuthentication.SendAuthResultToTerminal(TerminalID, isUserID, isAcessibility, isVistor, isAuthorized, txtEventTime, sErrorCode)

            logMessage = ""
            logMessage &= $"{Environment.NewLine}<--EventVerifyFinger1toN"
            logMessage &= $"{Environment.NewLine}      +ErrorCode: {objUCSAPICOM.ErrorCode}"
            logMessage &= $"{Environment.NewLine}      +TerminalID: {TerminalID}"
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)


            'Dim sMsg As String
            'If isAuthorized = 1 Then
            '    sMsg = "오늘도 즐거운 하루 보내세요!!"
            'Else
            '    sMsg = "등록된 지문이 아니다!!!!!!!"
            'End If
            'objUCSAPICOM.SendPrivateMessageToTerminal(0, TerminalID, Len(sMsg), sMsg, 5)  ' 5초간 메시지 표시

        Catch ex As Exception
            Dim logMessage As String = $"프로그램 오류 발생: {ex.Message}"
            WriteLog(logMessage, LOG_TO_FILE, LOG_FILE_NAME)
        End Try


    End Sub

    ' 시리얼 포트에서 데이터가 수신될 때 호출될 이벤트 핸들러
    Private Sub HandleSerialData(sender As Object, e As SerialDataReceivedEventArgs)

        Me.Invoke(Sub()
                      Try
                          ' 데이터가 완전히 도착할 시간을 확보하기 위해 대기시간 
                          Thread.Sleep(50) '50ms

                          Dim receivedData As String = modFunc.serialPort.ReadExisting()

                          ' 받은 바코드 값 처리
                          Dim barcodeValue As String = receivedData.Trim()
                          If Not String.IsNullOrEmpty(barcodeValue) Then
                              TextBox1.Text = $"읽은 바코드: {barcodeValue}"
                          End If
                      Catch ex As Exception
                          MessageBox.Show("바코드 데이터 수신 오류: " & ex.Message)
                      End Try
                  End Sub)
    End Sub
    ' Json 문자열 값 꺼내오는 함수 
    Private Function GetJsonString(root As JsonElement, propName As String, Optional asRawText As Boolean = False, Optional defaultValue As String = "") As String
        Dim value As String = defaultValue
        Dim temp As JsonElement

        If root.TryGetProperty(propName, temp) Then
            If asRawText Then
                value = temp.GetRawText()
            ElseIf temp.ValueKind = JsonValueKind.String Then
                value = temp.GetString()
            End If
        End If
        Return value

    End Function
    Private Sub syncTimer_Tick(sender As Object, e As EventArgs) Handles syncTimer.Tick
        SyncNewFingerprints()
    End Sub
    '지문 갱신데이타 존재여부 확인 
    Public Sub SyncNewFingerprints()

        Using conn As SqlConnection = modDBConn.GetConnection()
            If conn Is Nothing Then Return

            ' 새로운 데이터가 있는지 먼저 존재여부만 판단..
            Dim hasNewData As Boolean = False
            Dim checkSql As String = "IF EXISTS (SELECT 1 FROM T_MEM_PHOTO " &
                                 "WHERE F_COMPANY_CODE = @F_COMPANY_CODE AND F_UDATE > @LastSyncTime " &
                                 "AND F_FINGER IS NOT NULL AND DATALENGTH(F_FINGER) > 0) " &
                                 "SELECT 1 ELSE SELECT 0"

            Using checkCmd As New SqlCommand(checkSql, conn)
                checkCmd.Parameters.AddWithValue("@F_COMPANY_CODE", gCompanyCode)
                checkCmd.Parameters.AddWithValue("@LastSyncTime", lastSyncTimestamp)
                If CInt(checkCmd.ExecuteScalar()) = 1 Then
                    hasNewData = True
                End If
            End Using

            '새로운 데이터가 없으면, 원래 로직을 실행하지 않고 즉시 종료.
            If Not hasNewData Then Return

            Dim sql As String = "SELECT F_MEM_IDX, F_FINGER FROM T_MEM_PHOTO " &
                            "WHERE F_COMPANY_CODE = @F_COMPANY_CODE AND F_UDATE > @LastSyncTime " &
                            "AND F_FINGER IS NOT NULL AND DATALENGTH(F_FINGER) > 0"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@F_COMPANY_CODE", gCompanyCode)
                cmd.Parameters.AddWithValue("@LastSyncTime", lastSyncTimestamp)

                Try
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim img As Object = reader("F_FINGER")
                            Dim nUserID As Integer
                            If Not Convert.IsDBNull(img) Then
                                Dim lbytTemp As Byte() = DirectCast(img, Byte())
                                nUserID = If(Convert.IsDBNull(reader("F_MEM_IDX")), 0, Convert.ToInt32(reader("F_MEM_IDX")))
                                If nUserID > 0 Then
                                    objFastSearch.RemoveUser(nUserID) ' 기존 정보가 있다면 삭제
                                    objFastSearch.AddFIR(lbytTemp, nUserID) ' 새 정보 추가
                                    If objUCBioBSP.ErrorCode <> 0 Then
                                        WriteLog(objUCBioBSP.ErrorDescription & " [" & objUCBioBSP.ErrorCode & "]", LOG_TO_FILE, LOG_FILE_NAME)
                                        Return
                                    Else
                                        WriteLog("신규 지문 갱신 완료 : " + Convert.ToString(nUserID), LOG_TO_FILE, LOG_FILE_NAME)
                                    End If
                                End If
                            End If
                        End While
                    End Using
                    lastSyncTimestamp = DateTime.Now.AddSeconds(-5)
                Catch ex As Exception
                    WriteLog("지문 정보 갱신 중 오류 발생: " & ex.Message, LOG_TO_FILE, LOG_FILE_NAME)
                End Try
            End Using
        End Using
    End Sub
    ''' <summary>
    ''' 로그를 텍스트 박스에 표시하고, 선택적으로 파일에 날짜별로 기록합니다.
    ''' </summary>
    ''' <param name="message">기록할 로그 메시지입니다.</param>
    ''' <param name="writeToFile">로그를 파일에 기록할지 여부 (True/False)입니다.</param>
    ''' <param name="baseFileName">로그 파일의 기본 이름입니다. (예: "FingerAuth.log")</param>
    Private Sub WriteLog(message As String, Optional writeToFile As Boolean = False, Optional baseFileName As String = "AppLog.log")
        ' 1. 타임스탬프가 포함된 전체 로그 메시지 생성
        Dim logEntry As String = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}"

        ' 2. 화면의 텍스트 박스에 로그 추가 (UI 스레드 접근 처리 포함)
        If txtFingerDataLog.InvokeRequired Then
            txtFingerDataLog.Invoke(New Action(Sub()
                                                   txtFingerDataLog.AppendText(logEntry & Environment.NewLine)
                                               End Sub))
        Else
            txtFingerDataLog.AppendText(logEntry & Environment.NewLine)
        End If

        ' 3. 파일에 로그 기록 (writeToFile이 True일 경우)
        If writeToFile Then
            Try
                ' --- 파일명에 날짜를 추가하는 로직 (수정된 부분) ---
                ' (1) 기본 파일명에서 이름과 확장자를 분리합니다.
                '     예: "FingerAuth.log" -> "FingerAuth", ".log"
                Dim fileNameWithoutExt As String = IO.Path.GetFileNameWithoutExtension(baseFileName)
                Dim fileExtension As String = IO.Path.GetExtension(baseFileName)

                ' (2) 오늘 날짜를 "yyyy-MM-dd" 형식의 문자열로 만듭니다.
                Dim currentDate As String = DateTime.Now.ToString("yyyy-MM-dd")

                ' (3) "파일명_날짜.확장자" 형식으로 새로운 파일명을 조합합니다.
                '     예: "FingerAuth_2025-09-19.log"
                Dim datedFileName As String = $"{fileNameWithoutExt}_{currentDate}{fileExtension}"

                Dim logDirectory As String = My.Application.Info.DirectoryPath
                Dim logFilePath As String = IO.Path.Combine(logDirectory, datedFileName)

                Using writer As New IO.StreamWriter(logFilePath, True)
                    writer.WriteLine(logEntry)
                End Using

            Catch ex As Exception
                ' 파일 쓰기 실패 시 화면에만 오류 로그를 남깁니다.
                Dim errorLog As String = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] FILE LOGGING ERROR: {ex.Message}"
                If txtFingerDataLog.InvokeRequired Then
                    txtFingerDataLog.Invoke(New Action(Sub()
                                                           txtFingerDataLog.AppendText(errorLog & Environment.NewLine)
                                                       End Sub))
                Else
                    txtFingerDataLog.AppendText(errorLog & Environment.NewLine)
                End If
            End Try
        End If
    End Sub

    Private Sub TitleAnimationTimer_Tick(sender As Object, e As EventArgs) Handles TitleAnimationTimer.Tick

        ' 점(.)의 개수를 1씩 증가시킵니다.
        dotCount += 1

        ' 점의 개수가 3개를 넘어가면 다시 1개로 리셋합니다.
        If dotCount > 10 Then
            dotCount = 1
        End If

        ' 기본 텍스트와 현재 점(.) 개수를 조합하여 제목을 설정합니다.
        ' 뒤에 공백을 추가하여 글자 수 변경 시 제목이 흔들리는 현상을 방지합니다.
        Me.Text = baseTitle & New String("."c, dotCount) & New String(" "c, 10 - dotCount)

    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        TitleAnimationTimer.Stop()
    End Sub
End Class
