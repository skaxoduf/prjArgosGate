<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        WebView21 = New Microsoft.Web.WebView2.WinForms.WebView2()
        pnlCSMain = New Panel()
        txtFingerDataLog = New TextBox()
        TextBox1 = New TextBox()
        Panel1 = New Panel()
        Button1 = New Button()
        btnExit = New Button()
        CType(WebView21, ComponentModel.ISupportInitialize).BeginInit()
        pnlCSMain.SuspendLayout()
        Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' WebView21
        ' 
        WebView21.AllowExternalDrop = True
        WebView21.CreationProperties = Nothing
        WebView21.DefaultBackgroundColor = Color.White
        WebView21.Location = New Point(825, 32)
        WebView21.Name = "WebView21"
        WebView21.Size = New Size(90, 37)
        WebView21.TabIndex = 1
        WebView21.ZoomFactor = 1R
        ' 
        ' pnlCSMain
        ' 
        pnlCSMain.BackColor = Color.Black
        pnlCSMain.Controls.Add(WebView21)
        pnlCSMain.Controls.Add(txtFingerDataLog)
        pnlCSMain.Controls.Add(TextBox1)
        pnlCSMain.Location = New Point(12, 80)
        pnlCSMain.Name = "pnlCSMain"
        pnlCSMain.Size = New Size(894, 437)
        pnlCSMain.TabIndex = 2
        ' 
        ' txtFingerDataLog
        ' 
        txtFingerDataLog.BackColor = Color.FromArgb(CByte(45), CByte(45), CByte(48))
        txtFingerDataLog.Font = New Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(129))
        txtFingerDataLog.ForeColor = Color.FromArgb(CByte(241), CByte(241), CByte(241))
        txtFingerDataLog.Location = New Point(6, 3)
        txtFingerDataLog.Multiline = True
        txtFingerDataLog.Name = "txtFingerDataLog"
        txtFingerDataLog.ScrollBars = ScrollBars.Vertical
        txtFingerDataLog.Size = New Size(882, 431)
        txtFingerDataLog.TabIndex = 1
        ' 
        ' TextBox1
        ' 
        TextBox1.Location = New Point(792, -10)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(102, 23)
        TextBox1.TabIndex = 3
        TextBox1.Visible = False
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(Button1)
        Panel1.Controls.Add(btnExit)
        Panel1.Location = New Point(12, 12)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(894, 62)
        Panel1.TabIndex = 4
        ' 
        ' Button1
        ' 
        Button1.BackColor = Color.MidnightBlue
        Button1.Font = New Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(129))
        Button1.ForeColor = Color.White
        Button1.Location = New Point(6, 7)
        Button1.Name = "Button1"
        Button1.Size = New Size(106, 48)
        Button1.TabIndex = 1
        Button1.Text = "환경설정"
        Button1.UseVisualStyleBackColor = False
        ' 
        ' btnExit
        ' 
        btnExit.BackColor = Color.MidnightBlue
        btnExit.Font = New Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(129))
        btnExit.ForeColor = Color.White
        btnExit.Location = New Point(782, 7)
        btnExit.Name = "btnExit"
        btnExit.Size = New Size(106, 48)
        btnExit.TabIndex = 1
        btnExit.Text = "종 료"
        btnExit.UseVisualStyleBackColor = False
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(45), CByte(45), CByte(48))
        ClientSize = New Size(918, 529)
        Controls.Add(Panel1)
        Controls.Add(pnlCSMain)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Argos APT GateDemon"
        CType(WebView21, ComponentModel.ISupportInitialize).EndInit()
        pnlCSMain.ResumeLayout(False)
        pnlCSMain.PerformLayout()
        Panel1.ResumeLayout(False)
        ResumeLayout(False)
    End Sub
    Friend WithEvents WebView21 As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents pnlCSMain As Panel
    Friend WithEvents txtFingerDataLog As TextBox
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents btnExit As Button
    Friend WithEvents Button1 As Button

End Class
