Imports Microsoft.VisualBasic.CompilerServices
Imports QuartzTypeLib
Imports System

Namespace th
    <StandardModule> _
    Friend NotInheritable Class modDirectShow
        ' Methods
        Public Shared Function LoadMP3(ByRef FileName As String) As IMediaControl
            Dim control As IMediaControl
            Dim num2 As Integer
            Try 
                ProjectData.ClearProjectError
                Dim num As Integer = 2
                modDirectShow.imediaControl_0 = New FilgraphManagerClass
                modDirectShow.imediaControl_0.RenderFile(FileName)
                modDirectShow.DSAudio = DirectCast(modDirectShow.imediaControl_0, IBasicAudio)
                modDirectShow.DSAudio.Balance = 0
                control = modDirectShow.imediaControl_0
                modDirectShow.DSEvent = DirectCast(modDirectShow.imediaControl_0, IMediaEvent)
                modDirectShow.imediaPosition_0 = DirectCast(modDirectShow.imediaControl_0, IMediaPosition)
                modDirectShow.imediaPosition_0.Rate = 1
                modDirectShow.imediaPosition_0.CurrentPosition = 0
                goto Label_00CC
            Label_0084:
                THConstVars.HandleError
                goto Label_00CC
            Label_008B:
                num2 = -1
                Select Case num
                    Case 0, 1
                        goto Label_00C1
                    Case 2
                        goto Label_0084
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_008B
            End Try
        Label_00C1:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_00CC:
            If (num2 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return control
        End Function

        Public Shared Function PauseMP3() As Boolean
            Dim flag As Boolean
            Dim num2 As Integer
            Try 
                ProjectData.ClearProjectError
                Dim num As Integer = 2
                modDirectShow.imediaControl_0.Stop
                flag = True
                goto Label_0060
            Label_0016:
                flag = False
                THConstVars.HandleError
                goto Label_0060
            Label_001F:
                num2 = -1
                Select Case num
                    Case 0, 1
                        goto Label_0055
                    Case 2
                        goto Label_0016
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_001F
            End Try
        Label_0055:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_0060:
            If (num2 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return flag
        End Function

        Public Shared Function PlayMP3() As Boolean
            Dim flag As Boolean
            Dim num2 As Integer
            Try 
                ProjectData.ClearProjectError
                Dim num As Integer = 2
                modDirectShow.imediaControl_0.Run
                flag = True
                goto Label_0060
            Label_0016:
                flag = False
                THConstVars.HandleError
                goto Label_0060
            Label_001F:
                num2 = -1
                Select Case num
                    Case 0, 1
                        goto Label_0055
                    Case 2
                        goto Label_0016
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_001F
            End Try
        Label_0055:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_0060:
            If (num2 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return flag
        End Function

        Public Shared Function SetPlayBackBalance(ByRef Balance As Integer) As Boolean
            Dim flag As Boolean
            Dim num2 As Integer
            Try 
                ProjectData.ClearProjectError
                Dim num As Integer = 2
                modDirectShow.DSAudio.Balance = Balance
                flag = True
                goto Label_0062
            Label_0018:
                flag = False
                THConstVars.HandleError
                goto Label_0062
            Label_0021:
                num2 = -1
                Select Case num
                    Case 0, 1
                        goto Label_0057
                    Case 2
                        goto Label_0018
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_0021
            End Try
        Label_0057:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_0062:
            If (num2 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return flag
        End Function

        Public Shared Function SetPlayBackSpeed(ByRef Speed As Single) As Boolean
            Dim flag As Boolean
            Dim num2 As Integer
            Try 
                ProjectData.ClearProjectError
                Dim num As Integer = 2
                modDirectShow.imediaPosition_0.Rate = CDbl(Speed)
                flag = True
                goto Label_0063
            Label_0019:
                flag = False
                THConstVars.HandleError
                goto Label_0063
            Label_0022:
                num2 = -1
                Select Case num
                    Case 0, 1
                        goto Label_0058
                    Case 2
                        goto Label_0019
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_0022
            End Try
        Label_0058:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_0063:
            If (num2 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return flag
        End Function

        Public Shared Function SetPlayBackVolume(ByRef Volume As Integer) As Boolean
            Dim flag As Boolean
            Dim num2 As Integer
            Try 
                ProjectData.ClearProjectError
                Dim num As Integer = 2
                modDirectShow.DSAudio.Volume = (Volume * 40)
                flag = True
                goto Label_0065
            Label_001B:
                flag = False
                THConstVars.HandleError
                goto Label_0065
            Label_0024:
                num2 = -1
                Select Case num
                    Case 0, 1
                        goto Label_005A
                    Case 2
                        goto Label_001B
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_0024
            End Try
        Label_005A:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_0065:
            If (num2 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return flag
        End Function

        Public Shared Function StopMP3() As Boolean
            Dim flag As Boolean
            Dim num2 As Integer
            Try 
                ProjectData.ClearProjectError
                Dim num As Integer = 2
                modDirectShow.imediaControl_0.Stop
                modDirectShow.imediaPosition_0.CurrentPosition = 0
                flag = True
                goto Label_0073
            Label_0029:
                flag = False
                THConstVars.HandleError
                goto Label_0073
            Label_0032:
                num2 = -1
                Select Case num
                    Case 0, 1
                        goto Label_0068
                    Case 2
                        goto Label_0029
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_0032
            End Try
        Label_0068:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_0073:
            If (num2 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return flag
        End Function

        Public Shared Function TerminateEngine() As Boolean
            Dim flag As Boolean
            Dim num2 As Integer
            Try 
                ProjectData.ClearProjectError
                Dim num As Integer = 2
                modDirectShow.DSAudio = Nothing
                modDirectShow.DSEvent = Nothing
                modDirectShow.imediaControl_0 = Nothing
                modDirectShow.imediaPosition_0 = Nothing
                flag = True
                goto Label_006E
            Label_0024:
                flag = False
                THConstVars.HandleError
                goto Label_006E
            Label_002D:
                num2 = -1
                Select Case num
                    Case 0, 1
                        goto Label_0063
                    Case 2
                        goto Label_0024
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_002D
            End Try
        Label_0063:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_006E:
            If (num2 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return flag
        End Function


        ' Fields
        Public Shared DSAudio As IBasicAudio
        Private Shared DSEvent As IMediaEvent
        Public Shared imediaControl_0 As IMediaControl
        Public Shared imediaPosition_0 As IMediaPosition
    End Class
End Namespace

