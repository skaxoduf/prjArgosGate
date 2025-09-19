Imports System.IO.Ports

Module modFunc

    Public WithEvents serialPort As New SerialPort()

    ' 시리얼포트 연결 함수
    Public Sub ConnectSerialPort(portName As String, baudRate As Integer)
        ' 만약 포트가 이미 열려있다면 닫기
        If serialPort.IsOpen Then
            serialPort.Close()
        End If

        Try
            ' 포트 설정
            serialPort.PortName = portName
            serialPort.BaudRate = baudRate
            serialPort.DataBits = 8
            serialPort.Parity = Parity.None
            serialPort.StopBits = StopBits.One
            serialPort.ReadBufferSize = 8192

            ' 포트 열기
            serialPort.Open()
            'MessageBox.Show($"{portName} 포트가 {baudRate} 통신 속도로 성공적으로 열렸습니다.", "연결 성공")
        Catch ex As Exception
            MessageBox.Show($"포트 연결 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ' 시리얼 포트를 닫는 함수
    Public Sub DisconnectSerialPort()
        If serialPort.IsOpen Then
            serialPort.Close()
            'MessageBox.Show("시리얼 포트가 닫혔습니다.", "연결 종료")
        End If
    End Sub

End Module
