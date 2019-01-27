' NOTE: This form is inherited from the CaptureForm,
' so the VisualStudio Form Designer may not load this properly
' (at least until you build the project).
' If you want to make changes in the form layout - do it in the base CaptureForm.
' All changes in the CaptureForm will be reflected in all derived forms 
' (i.e. in the EnrollmentForm and in the VerificationForm)
Imports MySql.Data.MySqlClient

Public Class VerificationForm
    'Inherits CaptureForm
    Inherits SearchForm
    Public con As New MySqlConnection
    Public cmd As New MySqlCommand
    Public da As New MySqlDataAdapter
    Public dr As MySqlDataReader

  Private Template As DPFP.Template
  Private Verificator As DPFP.Verification.Verification

  Public Sub Verify(ByVal template As DPFP.Template)
    Me.Template = template
    ShowDialog()
  End Sub

  Protected Overrides Sub Init()
    MyBase.Init()
    MyBase.Text = "Fingerprint Verification"
    Verificator = New DPFP.Verification.Verification()
    UpdateStatus(0)
  End Sub

    Protected Overrides Sub Process(ByVal Sample As DPFP.Sample)
        con.Close()
        MyBase.Process(Sample)
        ' Process the sample and create a feature set for the enrollment purpose.
        Dim features As DPFP.FeatureSet = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification)

        'If Not Template Is Nothing Then
        ' Check quality of the sample and start verification if it's good
        If Not features Is Nothing Then
            ' Compare the feature set with our template

            con.ConnectionString = "Server=localhost;uid=root;password=;database=db_bio"
            cmd.CommandText = "SELECT * FROM tbl_bio"
            cmd.Connection = con
            Dim MemStream As IO.MemoryStream
            Dim fpBytes As Byte()
            Dim Idno As String
            Try
                con.Open()
                dr = cmd.ExecuteReader
                While dr.Read = True
                    fpBytes = dr.GetValue(2)
                    MemStream = New IO.MemoryStream(fpBytes)
                    Dim template As New DPFP.Template(MemStream)
                    'template.DeSerialize(MemStream)
                    Dim result As DPFP.Verification.Verification.Result = New DPFP.Verification.Verification.Result()
                    Verificator.Verify(features, template, result)
                    UpdateStatus(result.FARAchieved)
                    If result.Verified Then
                        Idno = dr.GetValue(0)
                        MakeReport("the fingerprint was verified.")
                    Else
                        MakeReport("the fingerprint was not verified.")
                    End If
                End While

                If (Idno.Length <> 0) Then
                    Dim TimeData = DateTime.Now().ToString("yyyy-MM-dd HH:mm:ss")
                    Dim Timenow = DateTime.Now().ToString("hh:mm:ss")
                    DisplayFullname(dr.GetValue(1).ToString.ToUpper())
                    DisplayTime(Timenow)
                    DisplayRemark("SUCCESSFUL")
                    savetimelogs(Idno, TimeData)

                Else
                    DisplayRemark("DENIED")
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message.ToString)
            Finally
                con.Close()
            End Try




            'Dim result As DPFP.Verification.Verification.Result = New DPFP.Verification.Verification.Result()
            'Verificator.Verify(features, Template, result)
            'UpdateStatus(result.FARAchieved)
            'If result.Verified Then
            '    MakeReport("the fingerprint was verified.")
            'Else
            '    MakeReport("the fingerprint was not verified.")
            'End If
        End If
        'Else
        'MessageBox.Show("No Template")
        'End If
    End Sub

    Sub savetimelogs(ByVal Idno, ByVal TimeData)
        Dim TimeFlag As String

        Dim query As String
        If (Button1.BackColor = Color.Green) Then
            TimeFlag = "IN"
        Else
            TimeFlag = "OUT"
        End If
        query = "INSERT INTO tbl_logs (refno,TimeData,TimeFlag) VALUES ('" + Idno + "','" + TimeData + "','" + TimeFlag + "')"
        With cmd
            .CommandText = query
            .Connection = con
        End With
        Try
            con.Open()
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
        End Try
    End Sub

    Protected Sub UpdateStatus(ByVal FAR As Integer)
        ' Show "False accept rate" value
        SetStatus(String.Format("False Accept Rate (FAR) = {0}", FAR))
    End Sub

End Class
