Public Class KeyEvent
    Private Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Long) As Integer

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' I or i
        If GetAsyncKeyState(73) Or GetAsyncKeyState(105) Then
            Button1.BackColor = Color.Red
            ' O or o
        ElseIf GetAsyncKeyState(79) Or GetAsyncKeyState(111) Then
            Button1.BackColor = Color.Green
        End If
    End Sub
End Class