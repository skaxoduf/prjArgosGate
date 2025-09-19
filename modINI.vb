''ini파일 읽기/쓰기 
Module modINI

    ''ini 읽기 
    Public Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" _
       (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Long


    ''ini 쓰기 
    Public Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" _
    (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Long

    ''시스템 디렉토리 
    Public Declare Function GetSystemDirectory Lib "kernel32" Alias "GetSystemDirectoryA" _
        (ByVal lpBuffer As String, ByVal nSize As Long) As Long



    ' INI파일 읽어오기
    Function GetIni(Section As String, Key As String, FileNm As String)

        Dim RetVal As String, Worked As Long

        RetVal = Space(255)  ''vb6.0은 String으로 담아와도 되던데 여기에선 오류나서 변경함 

        Worked = GetPrivateProfileString(Section, Key, "", RetVal, Len(RetVal), FileNm)

        If Worked = 0 Then
            GetIni = ""
        Else
            GetIni = Left(RetVal, InStr(RetVal, Chr(0)) - 1)
        End If


    End Function

    ' INI파일 저장하기
    Function PutIni(Section As String, Key As String, Writestr As String, FileNm As String)
        Dim Worked As Long

        Worked = WritePrivateProfileString(Section, Key, Writestr, FileNm)

        If Worked = 0 Then
            MsgBox("수정 실패!")
        End If

    End Function



End Module
