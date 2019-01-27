Imports System.IO
Imports MySql.Data.MySqlClient

Delegate Sub FunctionCall(ByVal param)

Public Class MainForm
    Public con As New MySqlConnection
    Public cmd As New MySqlCommand
    Public da As New MySqlDataAdapter
    Public dr As MySqlDataReader

    Private Sub EnrollButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EnrollButton.Click
        Dim Enroller As New EnrollmentForm()
        AddHandler Enroller.OnTemplate, AddressOf OnTemplate
        Enroller.ShowDialog()
    End Sub

    Private Sub VerifyButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VerifyButton.Click
        Dim Verifier As New VerificationForm()
        Verifier.Verify(Template)
    End Sub

    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        Dim save As New SaveFileDialog()
        save.Filter = "Fingerprint Template File (*.fpt)|*.fpt"
        If save.ShowDialog() = Windows.Forms.DialogResult.OK Then
            ' Write template into the file stream
            Using fs As IO.FileStream = IO.File.Open(save.FileName, IO.FileMode.Create, IO.FileAccess.Write)
                Template.Serialize(fs)
            End Using
        End If

    End Sub

    Private Sub LoadButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadButton.Click
        Dim open As New OpenFileDialog()
        open.Filter = "Fingerprint Template File (*.fpt)|*.fpt"
        If open.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Using fs As IO.FileStream = IO.File.OpenRead(open.FileName)
                Dim template As New DPFP.Template(fs)
                OnTemplate(template)
            End Using
        End If
    End Sub

   

    Private Sub OnTemplate(ByVal template)
        Invoke(New FunctionCall(AddressOf _OnTemplate), template)
    End Sub

    Private Sub _OnTemplate(ByVal template)
        Me.Template = template
        VerifyButton.Enabled = (Not template Is Nothing)
        SaveButton.Enabled = (Not template Is Nothing)
        If Not template Is Nothing Then
            MessageBox.Show("The fingerprint template is ready for fingerprint verification.", "Fingerprint Enrollment")
            Button1.Enabled = True
        Else
            MessageBox.Show("The fingerprint template is not valid. Repeat fingerprint enrollment.", "Fingerprint Enrollment")
        End If
    End Sub


    Private Template As DPFP.Template

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        
        If Not Template Is Nothing And TextBox1.Text <> "" Then
            ' Dim Verifier As New VerificationForm()
            'Verifier.Verify(Template)

            Dim fingerprintData As MemoryStream = New MemoryStream
            Template.Serialize(fingerprintData)
            fingerprintData.Position = 0
            Dim br As BinaryReader = New BinaryReader(fingerprintData)
            Dim bytes() As Byte = br.ReadBytes(CType(fingerprintData.Length, Int32))

            con.ConnectionString = "Server=localhost;uid=root;password=;database=db_bio"
            With cmd
                .CommandText = "INSERT INTO tbl_bio (full_name,fpData) VALUES (@fname,@fp)"
                .Connection = con
                .Parameters.AddWithValue("@fname", TextBox1.Text)
                .Parameters.AddWithValue("@fp", bytes)
            End With
            Try
                con.Open()
                cmd.ExecuteNonQuery()
                cmd.Parameters.Clear()

            Catch ex As Exception
                MessageBox.Show(ex.Message.ToString)
            Finally
                con.Close()
            End Try


            'Dim SQLconnect As New SQLite.SQLiteConnection()
            'Dim SQLcommand As SQLiteCommand
            'SQLconnect.ConnectionString = "data source=C:\Users\mypc\Documents\MatrixDbase.s3db"
            'SQLconnect.Open()
            'SQLcommand = SQLconnect.CreateCommand

            'SQLcommand.CommandText = "INSERT INTO Longin (id, Fname, Lname, Pic) VALUES ('" & TextBox1.Text & "', '" & TextBox2.Text & "', '" & TextBox3.Text & "', @Pic)"

            'SQLcommand.Parameters.AddWithValue("@Pic", bytes)
            ''Runs Query
            'SQLcommand.ExecuteNonQuery()
            'SQLcommand.Dispose()
            'SQLconnect.Close()

        Else
            If TextBox1.Text = "" Then
                MessageBox.Show("PROVIDE FULLNAME")
            End If
            MessageBox.Show("ENROLL FINGERPRINT TEMPLATE FIRST")

        End If



    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        Dim Verifier As New VerificationForm()
        Verifier.Verify(Template)
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Dim Verifier As New VerificationForm()
        Verifier.Verify(Template)
    End Sub
End Class
