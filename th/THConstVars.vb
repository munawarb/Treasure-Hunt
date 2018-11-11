Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace th
    <StandardModule> _
    Friend NotInheritable Class THConstVars
        ' Methods
        <DllImport("kernel32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)> _
        Private Shared Function GetTickCount() As Long
        End Function

        Public Shared Sub HandleError()
            Dim number As Long = Information.Err.Number
            Dim description As String = Information.Err.Description
            Dim erl As Long = Information.Err.Erl
            Dim source As String = Information.Err.Source
            If THConstVars.IsShuttingDown Then
                ProjectData.EndApp
            End If
            If (number <> &H5B) Then
                If ((number = 9) And ((((THF.F.px > THF.F.x) Or (THF.F.px < 1)) Or (THF.F.py > THF.F.y)) Or (THF.F.py < 1))) Then
                    If (THF.F.px < 1) Then
                        THF.F.px = 1
                    End If
                    If (THF.F.px > THF.F.x) Then
                        THF.F.px = THF.F.x
                    End If
                    If (THF.F.py < 1) Then
                        THF.F.py = 1
                    End If
                    If (THF.F.py > THF.F.y) Then
                        THF.F.py = THF.F.y
                    End If
                Else
                    THConstVars.CannotDoKeydown = True
                    Dim str3 As String = THConstVars.string_0
                    FileSystem.Reset
                    FileSystem.FileOpen(1, (Addendums.FilePath & "\error.log"), OpenMode.Output, OpenAccess.Default, OpenShare.Default, -1)
                    Select Case number
                        Case &H35
                            THConstVars.smethod_0(("The file " & str3 & " could not be found."))
                            FileSystem.WriteLine(1, New Object() { ("Could not find " & str3) })
                            Exit Select
                        Case &H1B0
                            THConstVars.smethod_0(("The file " & str3 & " could not be found."))
                            FileSystem.WriteLine(1, New Object() { ("Could not find " & str3) })
                            Exit Select
                        Case 5
                            THConstVars.smethod_0(("The file " & str3 & " is not of a valid format to be loaded into a 3d positional buffer."))
                            Exit Select
                        Case Else
                            THConstVars.smethod_0(String.Concat(New String() { "An error esception code ", Conversions.ToString(number), " has been raised. Please contact BPCPrograms SD for assistance. ERROR INFORMATION: ", description, "; ", source, " (", Conversions.ToString(erl), ")" }))
                            FileSystem.WriteLine(1, New Object() { String.Concat(New String() { "An error esception code ", Conversions.ToString(number), " has been raised. ERROR INFORMATION: Desc=", description, "; Source=", source, "; Erl=", Conversions.ToString(erl), "; Ver=", THF.F.AppVersion }) })
                            Exit Select
                    End Select
                    FileSystem.WriteLine(1, New Object() { ("error code: " & Conversions.ToString(number)) })
                    FileSystem.Reset
                    THF.F.ShutDown
                End If
            End If
        End Sub

        Public Shared Sub MWait(ByVal TimeToWait As Long)
            Dim num As Long = (THConstVars.GetTickCount + TimeToWait)
            Do While (THConstVars.GetTickCount <= num)
                Application.DoEvents
            Loop
        End Sub

        Public Shared Sub smethod_0(ByRef MSG As String)
            Interaction.MsgBox(MSG, MsgBoxStyle.Critical, "Program Error")
        End Sub


        ' Fields
        Public Const Easy As Single = 1!
        Public Const Moderate As Single = 2!
        Public Const ModeratelyImpossible As Single = 3!
        Public Const Impossible As Single = 4!
        Public Shared IsShuttingDown As Boolean
        Public Shared CannotDoKeydown As Boolean
        Public Shared Difficulty As Single
        Public Shared Keys As Keys
        Public Const K_Brutus As Single = 1!
        Public Const k_Brutus2 As Single = 2!
        Public Const K_Mouse As Single = 3!
        Public Shared string_0 As String
    End Class
End Namespace

