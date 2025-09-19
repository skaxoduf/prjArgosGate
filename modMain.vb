Module modMain
    Public Sub Main()
        ' Splash 보여주기
        Dim splash As New frmSplash()
        splash.Show()
        Application.DoEvents()

        ' 1.5초 대기
        Threading.Thread.Sleep(1500)

        splash.Close()

        ' 메인폼 실행
        Application.Run(New Form1())
    End Sub
End Module
