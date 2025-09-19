Imports System.Configuration
Imports System.IO
Imports Microsoft.Data.SqlClient   '.net8.0에서는 이 네임스페이스 사용이 권장됨

Module modDBConn
    ' 프로그램 전체에서 사용할 DB 연결 정보 문자열
    Public ConnectionString As String
    Public Function GetConnection() As SqlConnection
        If String.IsNullOrEmpty(ConnectionString) Then
            MessageBox.Show("데이터베이스 연결 정보가 설정되지 않았습니다.", "설정 오류", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End If
        Try
            Dim conn As New SqlConnection(ConnectionString)
            conn.Open()
            Return conn
        Catch ex As SqlException
            MessageBox.Show("데이터베이스 연결 실패: " & vbCrLf & ex.Message, "DB 오류", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        Catch ex As Exception
            MessageBox.Show("알 수 없는 오류가 발생했습니다." & vbCrLf & ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function
    Public Sub CloseConnection(ByVal conn As SqlConnection)
        If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
            conn.Close()
            conn.Dispose()
        End If
    End Sub
    Public Sub SaveImageFromDb(imageData As Byte(), filePath As String)
        Try
            ' .NET에서는 File.WriteAllBytes 메서드 한 줄로
            ' Byte 배열 데이터를 파일에 쓰는 작업이 모두 완료됩니다.
            ' (VB6의 Open, Put, Close, FreeFile 역할 포함)
            File.WriteAllBytes(filePath, imageData)
        Catch ex As Exception
            ' 파일 저장 중 오류가 발생하면 메시지를 표시합니다.
            MessageBox.Show($"파일 저장에 실패했습니다.{Environment.NewLine}경로: {filePath}{Environment.NewLine}오류: {ex.Message}",
                            "파일 쓰기 오류", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' DB에 접속하여 특정 사용자가 현재 출입 가능한 상태인지 확인합니다.
    Public Function CheckUserAuthorizationFromDB(ByVal userID As Integer) As Boolean
        Using conn As SqlConnection = modDBConn.GetConnection()
            If conn Is Nothing Then Return False

            ' 일단 임시로 테스트용으로 성별체크하는걸로  
            Dim sql As String = "SELECT F_SEX FROM T_MEM WHERE F_IDX = @F_IDX AND F_COMPANY_CODE = @F_COMPANY_CODE   "
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@F_IDX", userID)
                cmd.Parameters.AddWithValue("@F_COMPANY_CODE", gCompanyCode) 'gCompanyCode
                Try
                    Dim sexValue As Object = cmd.ExecuteScalar()
                    If sexValue IsNot Nothing AndAlso Not Convert.IsDBNull(sexValue) Then
                        If sexValue.ToString() = "M" Then
                            Return True
                        End If
                        Return False
                    Else
                        Return False
                    End If
                Catch ex As Exception
                    MessageBox.Show($"2차 인증 DB Check Error: {ex.Message}")
                    Return False
                End Try
            End Using
        End Using
    End Function


End Module
