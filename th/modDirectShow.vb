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
            Catch obj1 As Exception
                THConstVars.handleException(obj1, Nothing)
            End Try
            Return control
        End Function

        Public Shared Function PauseMP3() As Boolean
            Try
                modDirectShow.imediaControl_0.Stop()
            Catch obj1 As Exception
                THConstVars.handleException(obj1, Nothing)
            End Try
            Return True
        End Function

        Public Shared Function PlayMP3() As Boolean
            Try
                modDirectShow.imediaControl_0.Run()
            Catch obj1 As Exception
                THConstVars.handleException(obj1, Nothing)
            End Try
            Return True
        End Function

        Public Shared Function SetPlayBackBalance(ByRef Balance As Integer) As Boolean
            Try
                modDirectShow.DSAudio.Balance = Balance
            Catch obj1 As Exception
                THConstVars.handleException(obj1, Nothing)
            End Try
            Return True
        End Function

        Public Shared Function SetPlayBackSpeed(ByRef Speed As Single) As Boolean
            Try
                modDirectShow.imediaPosition_0.Rate = CDbl(Speed)
            Catch obj1 As Exception
                THConstVars.handleException(obj1, Nothing)
            End Try
            Return True
        End Function

        Public Shared Function SetPlayBackVolume(ByRef Volume As Integer) As Boolean
            Try
                modDirectShow.DSAudio.Volume = (Volume * 40)
            Catch obj1 As Exception
                THConstVars.handleException(obj1, Nothing)
            End Try
            Return True
        End Function

        Public Shared Function StopMP3() As Boolean
            Try
                modDirectShow.imediaControl_0.Stop()
                modDirectShow.imediaPosition_0.CurrentPosition = 0
            Catch obj1 As Exception
                THConstVars.handleException(obj1, Nothing)
            End Try
            Return True
        End Function

        Public Shared Function TerminateEngine() As Boolean
            Try
                Dim num As Integer = 2
                modDirectShow.DSAudio = Nothing
                modDirectShow.DSEvent = Nothing
                modDirectShow.imediaControl_0 = Nothing
                modDirectShow.imediaPosition_0 = Nothing
            Catch obj1 As Exception
                THConstVars.handleException(obj1, Nothing)
            End Try
            Return True
        End Function


        ' Fields
        Public Shared DSAudio As IBasicAudio
        Private Shared DSEvent As IMediaEvent
        Public Shared imediaControl_0 As IMediaControl
        Public Shared imediaPosition_0 As IMediaPosition
    End Class
End Namespace

