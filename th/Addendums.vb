Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Reflection
Imports System.Windows.Forms

Namespace th
    Public Class Addendums
        ' Methods
        Public Shared Sub WriteToClipboard(ByVal [Text] As String)
            Dim data As New DataObject
            data.SetData(DataFormats.Text, [Text])
            Clipboard.SetDataObject(data)
        End Sub


        ' Properties
        Public Shared ReadOnly Property FilePath As String
            Get
                Dim str3 As String = ""
                Dim array As String() = Strings.Split(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly.Location).FileName, "\", -1, CompareMethod.Binary)
                Dim num2 As Integer = ((Information.UBound(array, 1) - 1))
                Dim i As Integer = 0
                Do While (i <= num2)
                    If (i = 0) Then
                        str3 = array(i)
                    Else
                        str3 = (str3 & "\" & array(i))
                    End If
                    i = ((i + 1))
                Loop
                Return str3
            End Get
        End Property

        Public Shared ReadOnly Property FileVersion As String
            Get
                Return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly.Location).FileVersion
            End Get
        End Property

        Public Shared ReadOnly Property LegalCopyrights As String
            Get
                Return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly.Location).LegalCopyright
            End Get
        End Property

        Public Shared ReadOnly Property AppTitle As String
            Get
                Return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly.Location).ProductName
            End Get
        End Property

    End Class
End Namespace

