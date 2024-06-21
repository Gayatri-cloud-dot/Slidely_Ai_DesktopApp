Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json

Public Class CreateSubmissionForm

    Private stopwatchRunning As Boolean = False
    Private elapsedTime As TimeSpan = TimeSpan.Zero

    Private Sub CreateSubmissionForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Interval = 1000 ' Set timer interval to 1 second
        AddHandler Timer1.Tick, AddressOf Timer1_Tick
    End Sub

    Private Sub btnStopwatch_Click(sender As Object, e As EventArgs) Handles btnStopwatch.Click
        If stopwatchRunning Then
            Timer1.Stop()
            btnStopwatch.Text = "Start"
        Else
            Timer1.Start()
            btnStopwatch.Text = "Stop"
        End If
        stopwatchRunning = Not stopwatchRunning
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs)
        elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1))
        lblStopwatch.Text = elapsedTime.ToString("hh\:mm\:ss")
    End Sub

    Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        ' Collect data from textboxes
        Dim name As String = txtName.Text
        Dim email As String = txtEmail.Text
        Dim phone As String = txtPhone.Text
        Dim github As String = txtGithub.Text
        Dim stopwatchTime As String = lblStopwatch.Text

        ' Validate input fields
        If String.IsNullOrEmpty(name) OrElse String.IsNullOrEmpty(email) OrElse String.IsNullOrEmpty(phone) OrElse String.IsNullOrEmpty(github) OrElse String.IsNullOrEmpty(stopwatchTime) Then
            MessageBox.Show("All fields are required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If Not IsValidName(name) Then
            MessageBox.Show("Name must contain alphabetic characters only", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If Not IsValidEmail(email) Then
            MessageBox.Show("Invalid email format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If Not IsValidPhoneNumber(phone) Then
            MessageBox.Show("Phone number must be 10 digits long", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If Not IsValidGithubLink(github) Then
            MessageBox.Show("Invalid GitHub link. Example link: https://github.com/user", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Create a submission object
        Dim submission = New With {
            Key .name = name,
            Key .email = email,
            Key .phone = phone,
            Key .github_link = github,
            Key .stopwatch_time = stopwatchTime
        }

        ' Serialize the submission object to JSON
        Dim json As String = JsonConvert.SerializeObject(submission)

        ' Create an HTTP client and send the request
        Using client As New HttpClient()
            Dim content = New StringContent(json, Encoding.UTF8, "application/json")
            Dim response = Await client.PostAsync("http://localhost:8080/submit", content)

            If response.IsSuccessStatusCode Then
                MessageBox.Show("Submission successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("Submission failed: " & response.ReasonPhrase, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Using

        ' Optionally, reset the form
        txtName.Text = ""
        txtEmail.Text = ""
        txtPhone.Text = ""
        txtGithub.Text = ""
        Timer1.Stop()
        elapsedTime = TimeSpan.Zero
        lblStopwatch.Text = "00:00:00"
        btnStopwatch.Text = "Start"
        stopwatchRunning = False
    End Sub

    ' Helper method to validate email format
    Private Function IsValidEmail(email As String) As Boolean
        ' Basic email validation regex
        Dim emailRegex As New System.Text.RegularExpressions.Regex("^[^\s@]+@[^\s@]+\.[^\s@]+$")
        Return emailRegex.IsMatch(email)
    End Function

    ' Helper method to validate phone number format (exactly 10 digits)
    Private Function IsValidPhoneNumber(phone As String) As Boolean
        ' Validate phone number format (exactly 10 digits)
        Dim phoneRegex As New System.Text.RegularExpressions.Regex("^\d{10}$")
        Return phoneRegex.IsMatch(phone)
    End Function

    ' Helper method to validate GitHub link format
    Private Function IsValidGithubLink(githubLink As String) As Boolean
        ' Validate GitHub link regex
        Dim githubRegex As New System.Text.RegularExpressions.Regex("^https:\/\/github\.com\/[a-zA-Z0-9-]+(\/[a-zA-Z0-9-]+)*\/?$")
        Return githubRegex.IsMatch(githubLink)
    End Function

    ' Helper method to validate name (only alphabetic characters)
    Private Function IsValidName(name As String) As Boolean
        ' Validate name regex (only alphabetic characters)
        Dim nameRegex As New System.Text.RegularExpressions.Regex("^[a-zA-Z\s]+$")
        Return nameRegex.IsMatch(name)
    End Function

    Private Sub lblStopwatch_Click(sender As Object, e As EventArgs) Handles lblStopwatch.Click

    End Sub
End Class
