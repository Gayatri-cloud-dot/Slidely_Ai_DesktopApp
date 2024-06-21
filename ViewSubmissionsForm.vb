Imports System.Net
Imports System.Net.Http
Imports Newtonsoft.Json.Linq
Imports System.Diagnostics

Public Class ViewSubmissionsForm
    Private currentIndex As Integer = 0

    ' Constructor
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Load the first submission when the form loads
        LoadSubmission(currentIndex)
    End Sub

    ' Load a submission from the backend
    Private Async Sub LoadSubmission(index As Integer)
        Dim client As New HttpClient()
        Try
            Dim response As HttpResponseMessage = Await client.GetAsync($"http://localhost:8080/read?index={index}")
            If response.StatusCode = HttpStatusCode.NotFound Then
                MsgBox("Submission not found.")
                Return
            End If
            response.EnsureSuccessStatusCode()
            Dim responseBody As String = Await response.Content.ReadAsStringAsync()
            Dim submission As JObject = JObject.Parse(responseBody)("submission")
            DisplaySubmission(submission)
        Catch ex As Exception
            MsgBox("Error loading submission: " & ex.Message)
        End Try
    End Sub

    ' Display the current submission
    Private Sub DisplaySubmission(submission As JObject)
        If submission IsNot Nothing Then
            txtName.Text = submission("name").ToString()
            txtEmail.Text = submission("email").ToString()
            txtPhone.Text = submission("phone").ToString()
            txtGithub.Text = submission("github_link").ToString()
            lblTime.Text = $"Time taken: {submission("stopwatch_time").ToString()}"
        End If
    End Sub

    ' Event handler for the Previous button
    Private Sub btnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        If currentIndex > 0 Then
            currentIndex -= 1
            LoadSubmission(currentIndex)
        Else
            MsgBox("No previous submission.")
        End If
    End Sub

    ' Event handler for the Next button
    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        currentIndex += 1
        LoadSubmission(currentIndex)
    End Sub

    Private Sub txtGithub_TextChanged(sender As Object, e As EventArgs) Handles txtGithub.TextChanged

    End Sub
End Class
