Imports Microsoft
Imports Microsoft.Win32
Imports Microsoft.Win32.Registry

Public Class Main_Screen

    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message()
                Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ":" & ex.Message.ToString

                Display_Message1.Timer1.Interval = 1000
                Display_Message1.ShowDialog()
                Dim dir As System.IO.DirectoryInfo = New System.IO.DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                dir = Nothing
                Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & identifier_msg & ":" & ex.ToString)
                filewriter.Flush()
                filewriter.Close()
                filewriter = Nothing
            End If
            ex = Nothing
            identifier_msg = Nothing
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Me.Text = Me.Text & " " & My.Application.Info.Version.Major & My.Application.Info.Version.Minor & My.Application.Info.Version.Build & "." & My.Application.Info.Version.Revision
            For Each skn As String In Registry.ClassesRoot.GetSubKeyNames()
                'If skn.StartsWith("*") Or skn.StartsWith(".") Then
                ListBox1.Items.Add(skn)
                'End If
            Next
            If ListBox1.Items.Count > 0 Then
                ListBox1.SelectedIndex = 0
            End If
            If ListBox2.Items.Count > 0 Then
                ListBox2.SelectedIndex = 0
            End If
        Catch ex As Exception
            Error_Handler(ex, "Load")
        End Try
    End Sub
    Private Sub listbox1reload()
        Try
            If ListBox1.SelectedIndex <> -1 Then
                ListBox2.Items.Clear()
                TextBox1.Text = ""
                TextBox2.Text = ""
                TextBox3.Text = ""

                ToolStripStatusLabel1.Text = Registry.ClassesRoot.Name & "\" & ListBox1.Items.Item(ListBox1.SelectedIndex)
                Dim regkey As RegistryKey = Registry.ClassesRoot.OpenSubKey(ListBox1.Items.Item(ListBox1.SelectedIndex), False)
                For Each skn As String In regkey.GetSubKeyNames()
                    If skn.ToLower = "shell" Then
                        regkey = regkey.OpenSubKey(skn, False)
                        For Each skn2 As String In regkey.GetSubKeyNames
                            ListBox2.Items.Add(skn2)
                        Next
                    End If

                Next

                regkey.Close()

                If ListBox2.Items.Count > 0 Then
                    ListBox2.SelectedIndex = 0
                
                End If
                ButtonEnabler()
            End If
        Catch ex As Exception
            Error_Handler(ex, "ListBox1_SelectedIndexChanged")
        End Try
    End Sub
    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        listbox1reload()
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox2.SelectedIndexChanged
        Try
            If ListBox2.SelectedIndex <> -1 Then

                Dim regkey As RegistryKey = Registry.ClassesRoot.OpenSubKey(ListBox1.Items.Item(ListBox1.SelectedIndex) & "\shell\" & ListBox2.Items.Item(ListBox2.SelectedIndex), False)
                TextBox3.Text = ListBox2.Items.Item(ListBox2.SelectedIndex)
                For Each skn2 As String In regkey.GetValueNames
                    TextBox2.Text = regkey.GetValue(skn2)
                    Exit For
                Next
                For Each skn As String In regkey.GetSubKeyNames
                    If skn.ToLower = "command" Then
                        regkey = regkey.OpenSubKey(skn, False)
                        For Each skn2 As String In regkey.GetValueNames
                            TextBox1.Text = regkey.GetValue(skn2)
                            Exit For
                        Next



                    End If
                Next
                regkey.Close()
                'If ListBox2.Items.IndexOf(TextBox3.Text) = -1 And TextBox3.Text.Length > 0 Then
                '    Button1.Enabled = True
                'Else
                '    Button1.Enabled = False
                'End If
                ButtonEnabler()
            End If
        Catch ex As Exception
            Error_Handler(ex, "ListBox2_SelectedIndexChanged")
        End Try
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.TextChanged

        'If TextBox1.Text.Length > 0 And TextBox2.Text.Length > 0 And TextBox3.Text.Length > 0 Then
        '    If ListBox2.Items.IndexOf(TextBox3.Text) = -1 Then
        '        Button1.Enabled = True
        '    Else
        '        Button1.Enabled = False
        '    End If


        '    Button3.Enabled = True
        'Else
        '    Button1.Enabled = False
        '    Button3.Enabled = False
        'End If
        ButtonEnabler()
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        'If TextBox1.Text.Length > 0 And TextBox2.Text.Length > 0 And TextBox3.Text.Length > 0 Then
        '    Button1.Enabled = True
        '    Button3.Enabled = True
        'Else
        '    Button1.Enabled = False
        '    Button3.Enabled = False
        'End If
        ButtonEnabler()
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        'If TextBox1.Text.Length > 0 And TextBox2.Text.Length > 0 And TextBox3.Text.Length > 0 Then
        '    Button1.Enabled = True
        '    Button3.Enabled = True
        'Else
        '    Button1.Enabled = False
        '    Button3.Enabled = False
        'End If
        ButtonEnabler()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Try

        
            If ListBox1.Items.Count > 0 And ListBox2.Items.Count > 0 Then
                If ListBox2.SelectedIndex <> -1 And ListBox1.SelectedIndex <> -1 Then
                    If MsgBox("You are about to remove the following registry entry: " & vbCrLf & vbCrLf & Registry.ClassesRoot.Name & "\" & ListBox1.Items.Item(ListBox1.SelectedIndex) & "\shell\" & ListBox2.Items.Item(ListBox2.SelectedIndex) & vbCrLf & vbCrLf & "Is this correct?", MsgBoxStyle.OkCancel, "Remove Registry entry") = MsgBoxResult.Ok Then
                        Dim regkey As RegistryKey = Registry.ClassesRoot.OpenSubKey(ListBox1.Items.Item(ListBox1.SelectedIndex) & "\shell", True)
                        regkey.DeleteSubKeyTree(ListBox2.Items.Item(ListBox2.SelectedIndex))
                        regkey.Close()
                        listbox1reload()

                    End If

                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Remove Entry")
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            If TextBox1.Text.Length > 0 And TextBox2.Text.Length > 0 And TextBox3.Text.Length > 0 Then
                If MsgBox("You are about to create the following registry entry: " & vbCrLf & vbCrLf & Registry.ClassesRoot.Name & "\" & ListBox1.Items.Item(ListBox1.SelectedIndex) & "\shell\" & TextBox3.Text & vbCrLf & vbCrLf & "Is this correct?", MsgBoxStyle.OkCancel, "Create Registry entry") = MsgBoxResult.Ok Then
                    Dim regkey As RegistryKey = Registry.ClassesRoot.OpenSubKey(ListBox1.Items.Item(ListBox1.SelectedIndex) & "\shell", True)
                    If regkey Is Nothing Then
                        regkey = Registry.ClassesRoot.OpenSubKey(ListBox1.Items.Item(ListBox1.SelectedIndex), True)
                        regkey = regkey.CreateSubKey("shell")
                        regkey = Registry.ClassesRoot.OpenSubKey(ListBox1.Items.Item(ListBox1.SelectedIndex) & "\shell", True)
                    End If
                    regkey = regkey.CreateSubKey(TextBox3.Text)
                    regkey.SetValue(Nothing, TextBox2.Text, RegistryValueKind.String)
                    regkey = regkey.CreateSubKey("Command")
                    regkey.SetValue(Nothing, TextBox1.Text, RegistryValueKind.String)
                    regkey.Close()
                    listbox1reload()
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Create Entry")
        End Try
    End Sub

    Private Sub ButtonEnabler()
        Try

    
            If ListBox2.Items.Count > 0 Then
                If ListBox2.SelectedIndex <> -1 And ListBox2.Items.IndexOf(TextBox3.Text) <> -1 Then
                    Button2.Enabled = True
                End If
                If TextBox1.Text.Length > 0 And TextBox2.Text.Length > 0 And TextBox3.Text.Length > 0 And ListBox2.Items.IndexOf(TextBox3.Text) = -1 Then
                    Button1.Enabled = True
                    Button3.Enabled = False
                Else
                    Button1.Enabled = False
                    If TextBox1.Text.Length > 0 And TextBox2.Text.Length > 0 And TextBox3.Text.Length > 0 Then
                        Button3.Enabled = True
                    Else
                        Button3.Enabled = False
                    End If

                End If

            Else
                If TextBox1.Text.Length > 0 And TextBox2.Text.Length > 0 And TextBox3.Text.Length > 0 Then
                    Button1.Enabled = True
                Else
                    Button1.Enabled = False
                End If
                Button2.Enabled = False
                Button3.Enabled = False
            End If
        Catch ex As Exception
            Error_Handler(ex, "ButtonEnabler")
        End Try
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Try
            If TextBox1.Text.Length > 0 And TextBox2.Text.Length > 0 And TextBox3.Text.Length > 0 Then
                If MsgBox("You are about to change the following registry entry: " & vbCrLf & vbCrLf & Registry.ClassesRoot.Name & "\" & ListBox1.Items.Item(ListBox1.SelectedIndex) & "\shell\" & ListBox2.Items.Item(ListBox2.SelectedIndex) & vbCrLf & vbCrLf & "Is this correct?", MsgBoxStyle.OkCancel, "Change Registry entry") = MsgBoxResult.Ok Then
                    Dim regkey As RegistryKey = Registry.ClassesRoot.OpenSubKey(ListBox1.Items.Item(ListBox1.SelectedIndex) & "\shell\" & ListBox2.Items.Item(ListBox2.SelectedIndex), True)
                    regkey.SetValue(Nothing, TextBox2.Text, RegistryValueKind.String)
                    regkey = regkey.OpenSubKey("Command", True)
                    regkey.SetValue(Nothing, TextBox1.Text, RegistryValueKind.String)
                    regkey.Close()
                    listbox1reload()
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Edit Entry")
        End Try
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Try
            OpenFileDialog1.Title = ("Select the executable associated with this menu option")
            OpenFileDialog1.Filter = "Executables|*.exe|All files|*.*"
            If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                TextBox1.Text = OpenFileDialog1.FileName
            End If
        Catch ex As Exception
            Error_Handler(ex, "Command Browse")
        End Try
    End Sub
End Class
