
Public Class ScreenLock
    Sub KillExplorer()
        Dim taskKill As ProcessStartInfo = New ProcessStartInfo("taskkill", "/F /IM explorer.exe")
        taskKill.WindowStyle = ProcessWindowStyle.Hidden
        Dim Process As Process = New Process()
        Process.StartInfo = taskKill
        Process.Start()
        Process.WaitForExit()
    End Sub

    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            Const CS_NOCLOSE As Integer = &H200
            cp.ClassStyle = cp.ClassStyle Or CS_NOCLOSE
            Return cp
        End Get
    End Property


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' TopMost = True
        For Each selProcess As Process In Process.GetProcesses
            If selProcess.ProcessName = "taskmgr" Then
                selProcess.Kill()
                Exit For
            End If
        Next
        For Each selProcess As Process In Process.GetProcesses
            If selProcess.ProcessName = "explorer" Then
                'selProcess.Kill()
                Exit For
            End If
        Next
    End Sub

    Private Sub ScreenLock_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.a  Then
            Button1.BackColor = Color.Green
        End If
    End Sub

    Private Sub ScreenLock_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
        Dim T As New Threading.Thread(AddressOf Block)
        T.Start()
    End Sub

    Sub Block()

    End Sub
End Class