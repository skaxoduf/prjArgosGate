Imports System.Security.Cryptography
Imports System.Text

Public Module Crypto
    ' 데이터를 암호화하는 함수
    ' DataProtectionScope.LocalMachine: 이 컴퓨터의 모든 사용자가 데이터를 해독할 수 있음
    ' (POS 프로그램처럼 컴퓨터에 고정된 프로그램에 적합)
    Public Function EncryptString(ByVal plainText As String) As String
        If String.IsNullOrEmpty(plainText) Then
            Return String.Empty
        End If

        ' 문자열을 바이트 배열로 변환
        Dim data As Byte() = Encoding.UTF8.GetBytes(plainText)

        ' DPAPI를 사용하여 암호화
        Dim encryptedData As Byte() = ProtectedData.Protect(data, Nothing, DataProtectionScope.LocalMachine)

        ' 암호화된 바이트 배열을 Base64 문자열로 변환하여 반환 (INI 파일에 저장하기 위함)
        Return Convert.ToBase64String(encryptedData)
    End Function

    ' 데이터를 복호화하는 함수
    Public Function DecryptString(ByVal encryptedText As String) As String
        If String.IsNullOrEmpty(encryptedText) Then
            Return String.Empty
        End If

        Try
            ' Base64 문자열을 암호화된 바이트 배열로 변환
            Dim encryptedData As Byte() = Convert.FromBase64String(encryptedText)

            ' DPAPI를 사용하여 복호화
            Dim data As Byte() = ProtectedData.Unprotect(encryptedData, Nothing, DataProtectionScope.LocalMachine)

            ' 복호화된 바이트 배열을 다시 문자열로 변환하여 반환
            Return Encoding.UTF8.GetString(data)
        Catch ex As Exception
            ' 복호화 실패 시 빈 문자열 반환
            Return String.Empty
        End Try
    End Function
End Module