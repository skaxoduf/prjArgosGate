Public Class frmSplash

    Private Async Sub frmSplash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 1.5초 대기
        Await Task.Delay(1500)

        ' Form1 보여주기
        Dim mainForm As New Form1()
        mainForm.Show()

        ' 현재 Splash 닫기
        Me.Hide()

    End Sub

End Class
