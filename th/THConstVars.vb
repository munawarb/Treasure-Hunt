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

        Public Shared Sub handleException(e As Exception, timer As System.Windows.Forms.Timer)
            If Not (timer Is Nothing) Then
                timer.Enabled = False
            End If
            FileSystem.Reset()
            FileSystem.FileOpen(1, (Addendums.FilePath & "\error.log"), OpenMode.Output, OpenAccess.Default, OpenShare.Default, -1)
            FileSystem.WriteLine(1, New Object() {e.Message})
            FileSystem.Write(1, New Object() {e.StackTrace})
            FileSystem.Reset()
            Interaction.MsgBox("An error of type " & e.Message & " has occurred. Treasure Hunt can't continue, and a log has been generated.", MsgBoxStyle.Critical, "Error")
            THF.F.ShutDown()
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

