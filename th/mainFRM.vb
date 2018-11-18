Imports DxVBLibA
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports QuartzTypeLib
Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Net
Imports System.Security.Authentication
Imports System.Text
Imports System.Globalization
Imports System.Collections.Generic
Imports BPCSharedComponent.Input
Imports SharpDX.DirectInput



Namespace th
    Public Class mainFRM
        Inherits Form
        ' Methods
        Public Sub New()
            AddHandler MyBase.Load, New EventHandler(AddressOf Me.mainFRM_Load)
            AddHandler MyBase.Closed, New EventHandler(AddressOf Me.mainFRM_Closed)
            Me.PassageSound = New DirectSoundSecondaryBuffer8(5 - 1) {}
            Me.DoorSound = New DirectSoundSecondaryBuffer8(5 - 1) {}
            'Me.grid = New Short(1  - 1, 1  - 1) {}
            'Me.BGrid = New Short(1  - 1, 1  - 1) {}
            'Me.CGrid = New Single(1  - 1, 1  - 1) {}
            Me.Weapons = New String(8 - 1) {}
            Me.C = New Single(6 - 1) {}
            Me.InitializeComponent()
        End Sub

        ' Used to hard-disable or re-enable the game loop.
        ' This is necessary when requesting input because the acquired keyboard object will throw when the input box pops up, causing the game to hang.
        ' It will also prevent the game from treating keys in this box as game actions.
        Private Sub disableGameLoop()
            Timer1.Enabled = False
        End Sub

        Private Sub enableGameLoop()
            Timer1.Enabled = True
        End Sub

        Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
            System.Diagnostics.Debug.WriteLine("timer: tick")
            updateFrame()
        End Sub

        Private Sub waitForSpaceOrEnter()
            Do While Not (DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Return) Or DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Space))
                Application.DoEvents()
            Loop
        End Sub

        Private Sub sayDepth()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Me.NStop = False
            Me.NNumber.Stop()
            Me.VoiceNumber(Me.WDepth)
            If Me.NStop Then
                Exit Sub
            End If
            Me.NumWait()
            If Me.NStop Then
                Exit Sub
            End If
            Me.NLS((DXSound.string_0 & "\ntfeet.wav"), False)
            Me.NStop = True
        End Sub

        Private Sub sayNumAlert()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Me.NStop = False
            Me.NNumber.Stop()
            Me.VoiceNumber(Me.NumAlert)
            Me.NStop = True
        End Sub

        Private Sub sayHealth()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Me.NStop = False
            Me.NNumber.Stop()
            If Me.UnlimitedHealth Then
                Me.NLS((DXSound.string_0 & "\nunlimited.wav"), False)
                Me.NStop = True
            Else
                Me.VoiceNumber(Me.h)
                Me.NStop = True
            End If
        End Sub

        Private Sub pauseOrUnpauseGame()
            If Me.IsInPauseState Then
                Me.UnmuteSounds()
                Me.IsInPauseState = False
                THConstVars.CannotDoKeydown = False
            ElseIf Not THConstVars.CannotDoKeydown Then
                Me.MuteSounds()
                Me.IsInPauseState = True
                THConstVars.CannotDoKeydown = True
                DXSound.PlaySound(Me.GetSound, True, False, False, 0, 0, "", False)
            End If
        End Sub

        Private Sub changeMusicVolume(action As VolumeAction)
            Select Case action
                Case VolumeAction.up
                    If ((Me.BackgroundSound.GetVolume + 200) > 0) Then
                        Me.BackgroundSound.SetVolume(0)
                    Else
                        Me.BackgroundSound.SetVolume(Me.BackgroundSound.GetVolume + 200)
                    End If
                    Exit Select
                Case VolumeAction.down
                    If ((Me.BackgroundSound.GetVolume - 200) < -10000) Then
                        Me.BackgroundSound.SetVolume(-10000)
                    Else
                        Me.BackgroundSound.SetVolume(Me.BackgroundSound.GetVolume - 200)
                    End If
                    Exit Select
                Case VolumeAction.mute
                    Me.BackgroundSound.SetVolume(-10000)
                    Exit Select
                Case VolumeAction.unmute
                    Me.BackgroundSound.SetVolume(0)
                    Exit Select
            End Select
            Me.DuelSound.SetVolume(Me.BackgroundSound.GetVolume)
            If Me.IsFightingLast Then
                Me.FinalDuelSound.SetVolume(Me.BackgroundSound.GetVolume)
            End If
            Interaction.SaveSetting(Addendums.AppTitle, "Config", "Vol", Conversions.ToString(Me.BackgroundSound.GetVolume))
            Me.V = (Me.BackgroundSound.GetVolume)
        End Sub

        Private Function canTakeStep() As Boolean
            If Not movedInLastTick Then
                Return True
            End If
            If If(Not inMindOfGuard(), Not (Footstep1Sound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING Or Footstep2Sound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING Or WallCrashSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING), CFootstepSound.GetStatus = Footstep1Sound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING) Then
                Return True
            End If
            Return False
        End Function

        Private Function canSwim(flagToCheck As Boolean) As Boolean
            ' If flagToCheck is false, this command has been issued for the first time and we can go ahead and execute it.
            If Not flagToCheck Then
                Return True
            End If
            If Not SwimSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING And Not WallCrashSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING Then
                Return True
            End If
            Return False
        End Function

        Public Sub AllThingReplace(ByRef X As Integer, ByRef Y As Integer, ByRef Thing As Integer)
            Me.ThingReplace(X, Y, Thing)
            Me.BThingReplace(X, Y, Thing)
        End Sub

        Private Sub Bomb()
            If (Me.bombs > 0) Then
                If Not Me.UnlimitedBullets Then
                    Me.bombs = ((Me.bombs - 1))
                End If
                Me.DisNeedle = 1
                needleTracker = 0
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = False
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.NeedleLaunchSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                Me.IsLaunchingNeedle = True
                THConstVars.CannotDoKeydown = True
            End If
        End Sub

        Public Sub BThingReplace(ByRef X As Integer, ByRef Y As Integer, ByRef Thing As Integer)
            Me.BGrid(X, Y) = Thing
        End Sub

        Private Sub BuildWalls()
            Dim num As Integer
            Dim num2 As Integer
            Dim x As Integer = Me.x
            num = 1
            Dim iTemp As Integer
            For iTemp = 131 To 399
                Me.Grid(iTemp, 131) = Me.Wall
                Me.Grid(iTemp, 129) = Me.Wall
            Next

            System.Diagnostics.Debug.WriteLine("in buildwalls")
            Do While (num <= x)
                Me.Grid(num, 1) = Me.Wall
                Me.Grid(num, CInt(Math.Round(Conversion.Int(CDbl((CDbl(Me.y) / 4)))))) = 0
                Me.Grid(num, CInt(Math.Round(CDbl((Conversion.Int(CDbl((CDbl(Me.y) / 4))) + 1))))) = Me.Wall
                Application.DoEvents()
                num = ((num + 1))
            Loop
            Dim num4 As Integer = Me.x
            num = 1
            Do While (num <= num4)
                System.Diagnostics.Debug.WriteLine("Integer second loop")
                Me.Grid(num, Me.y) = Me.Wall
                Application.DoEvents()
                num = ((num + 1))
            Loop
            num = 1
            Do
                Me.Grid(num, &H69) = Me.Wall
                Application.DoEvents()
                num = ((num + 1))
            Loop While (num <= 50)
            System.Diagnostics.Debug.WriteLine("after third loop")
            Me.Grid(30, &H65) = Me.ClosedDoor
            Me.Grid(2, &H69) = Me.ClosedDoor
            Me.Grid(50, &H66) = Me.Wall
            Me.Grid(50, &H67) = Me.Wall
            Me.Grid(50, &H68) = Me.Wall
            Me.Grid(&H2F, &H69) = Me.ClosedDoor
            num = &H69
            Do While True
                System.Diagnostics.Debug.WriteLine("Planting wall at 5," & num)
                Me.Grid(5, num) = Me.Wall
                If (num = 180) Then
                    num2 = 1
                    Do
                        Me.Grid(num2, num) = Me.Wall
                        num2 = ((num2 + 1))
                    Loop While (num2 <= 5)
                End If
                Application.DoEvents()
                num = ((num + 1))
                If (num > 180) Then
                    Me.Grid(5, &H86) = Me.ClosedDoor
                    Me.Grid(5, &HB2) = Me.ClosedDoor
                    num = 5
                    Do
                        Me.Grid(num, &HB0) = Me.Wall
                        Me.Grid(num, 180) = Me.Wall
                        Application.DoEvents()
                        num = ((num + 1))
                    Loop While (num <= &H2D)
                    System.Diagnostics.Debug.WriteLine("After 180 loop")
                    num = &H69
                    Exit Do
                End If
            Loop
Label_0269:
            System.Diagnostics.Debug.WriteLine("label Label_0269")
            Me.Grid(&H2D, num) = Me.Wall
            Me.Grid(&H30, num) = Me.Wall
            If (num = 180) Then
                Me.Grid(&H2E, num) = Me.Wall
                Me.Grid(&H2F, num) = Me.Wall
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 180) Then
                System.Diagnostics.Debug.WriteLine("num > 180")
                Me.Grid(&H2D, &H86) = Me.ClosedDoor
                Me.Grid(&H2D, &HB2) = Me.ClosedDoor
                Me.Grid(&H30, 130) = Me.ClosedDoor
                num = 5
                Do
                    Me.Grid(num, &H83) = Me.Wall
                    Me.Grid(num, &H87) = Me.Wall
                    Application.DoEvents()
                    num = ((num + 1))
                Loop While (num <= &H2D)
                num = &H87
                Do
                    Me.Grid(20, num) = Me.Wall
                    Me.Grid(&H18, num) = Me.Wall
                    Application.DoEvents()
                    num = ((num + 1))
                Loop While (num <= &HB0)
                num = &H15
                Do
                    Me.Grid(num, &HB0) = Me.PassageMarker
                    Me.Grid(num, &H87) = Me.PassageMarker
                    num = ((num + 1))
                Loop While (num <= &H17)
                num = &H31
                System.Diagnostics.Debug.WriteLine("Done num > 180 at label Label_0269")
            Else
                System.Diagnostics.Debug.WriteLine("num Not > 180, going to Label_0269")
                GoTo Label_0269
            End If
Label_03DE:
            System.Diagnostics.Debug.WriteLine("Label_03DE")
            Me.Grid(num, &H80) = Me.Wall
            Me.Grid(num, &H84) = Me.Wall
            If (num = 130) Then
                num2 = &H80
                Do
                    Me.Grid(num, num2) = Me.Wall
                    num2 = ((num2 + 1))
                Loop While (num2 <= &H84)
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 130) Then
                System.Diagnostics.Debug.WriteLine("num > 130")
                num = &H84
            Else
                System.Diagnostics.Debug.WriteLine("num Not > 130, going to Label_03DE")
                GoTo Label_03DE
            End If
            Do While True
                If (num = &H84) Then
                    num2 = &H7F
                    Do
                        Me.Grid(num2, num) = Me.PassageMarker
                        num2 = ((num2 + 1))
                    Loop While (num2 <= &H81)
                End If
                Me.Grid(&H7E, num) = Me.Wall
                Me.Grid(130, num) = Me.Wall
                If (num = &HAC) Then
                    num2 = &H7E
                    Do
                        Me.Grid(num2, num) = Me.Wall
                        num2 = ((num2 + 1))
                    Loop While (num2 <= 130)
                End If
                Application.DoEvents()
                num = ((num + 1))
                System.Diagnostics.Debug.WriteLine("Made it to If (num > &HAC) Then, with num = " & num)
                If (num > &HAC) Then
                    Me.Grid(130, 170) = Me.ClosedDoor
                    num = 130
                    Me.Grid(130, (num - 1)) = Me.Wall
                    Me.Grid(&H83, (num - 1)) = Me.Wall
                    Me.Grid(&H84, (num - 1)) = Me.Wall
                    Me.Grid(130, num) = Me.ClosedDoor
                    Exit Do
                End If
            Loop
            System.Diagnostics.Debug.WriteLine("After outer loop")
Label_05FF:
            Me.Grid(num, &HA8) = Me.Wall
            Me.Grid(num, &HAC) = Me.Wall
            If (num = 190) Then
                num2 = &HA9
                Do
                    Me.Grid(num, num2) = Me.Wall
                    num2 = ((num2 + 1))
                Loop While (num2 <= &HAB)
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 190) Then
                Me.Grid(&H84, &HA9) = Me.PassageMarker
                Me.Grid(&H84, 170) = Me.PassageMarker
                Me.Grid(&H84, &HAB) = Me.PassageMarker
                Me.Grid(&H83, &HAC) = Me.PassageMarker
                Me.Grid(&H83, &HA8) = Me.PassageMarker
                num = 130
            Else
                GoTo Label_05FF
            End If
Label_074E:
            If (num <> &H83) Then
                Me.Grid(num, 180) = Me.Wall
            End If
            Me.Grid(num, &HB6) = Me.Wall
            Select Case num
                Case 130
                    Me.Grid(num, &HB5) = Me.Wall
                    Exit Select
                Case 200
                    Me.Grid(num, &HB5) = Me.Wall
                    Exit Select
            End Select
            Application.DoEvents()
            num = ((num + 1))
            If (num > 200) Then
                Me.Grid(170, &HB6) = Me.ClosedDoor
                num = &HB6
            Else
                GoTo Label_074E
            End If
Label_07AB:
            Me.Grid(&HA9, num) = Me.Wall
            Me.Grid(&HAB, num) = Me.Wall
            If (num = 230) Then
                Me.Grid(170, num) = Me.Wall
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 230) Then
                Me.Grid(&HAB, 200) = Me.ClosedDoor
                num = &HAB
            Else
                GoTo Label_07AB
            End If
Label_0833:
            Me.Grid(num, &HC7) = Me.Wall
            Me.Grid(num, &HC9) = Me.Wall
            If (num = 220) Then
                Me.Grid(num, 200) = Me.Wall
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 220) Then
                Me.Grid(&HD7, &HC7) = Me.ClosedDoor
                num = &HA8
            Else
                GoTo Label_0833
            End If
Label_08D2:
            Me.Grid(&HD6, num) = Me.Wall
            Me.Grid(&HD9, num) = Me.Wall
            If (num = &HA8) Then
                Me.Grid(&HD7, num) = Me.Wall
                Me.Grid(&HD8, num) = Me.Wall
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > &HC7) Then
                Me.Grid(&HD6, 170) = Me.ClosedDoor
                num = 190
                Do
                    Me.Grid(num, &HA8) = Me.Wall
                    Me.Grid(num, &HAC) = Me.Wall
                    Application.DoEvents()
                    num = ((num + 1))
                Loop While (num <= &HD6)
                Me.Grid(190, 170) = Me.ClosedDoor
                Me.Grid(&HD9, 170) = Me.ClosedDoor
                num = &HD9
            Else
                GoTo Label_08D2
            End If
Label_0A07:
            Me.Grid(num, &HA8) = Me.Wall
            Me.Grid(num, &HAC) = Me.Wall
            If (num = 300) Then
                Me.Grid(num, &HA9) = Me.Wall
                Me.Grid(num, 170) = Me.Wall
                Me.Grid(num, &HAB) = Me.Wall
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 300) Then
                Me.Grid(&H83, 180) = Me.ClosedDoor
                Me.Grid(230, &HAC) = Me.LockedDoor
                num = &HAC
            Else
                GoTo Label_0A07
            End If
Label_0ABF:
            Me.Grid(220, num) = Me.Wall
            Me.Grid(270, num) = Me.Wall
            If (num = 230) Then
                num2 = 220
                Do
                    Me.Grid(num2, num) = Me.Wall
                    num2 = ((num2 + 1))
                Loop While (num2 <= 270)
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 230) Then
                Me.Grid(&HDD, &HAD) = Me.Reflector
                Me.Grid(&H10D, &HAD) = Me.Missile
                Me.Grid(&HDD, &HE5) = Me.Laser
                Me.Grid(&H10D, &HE5) = Me.controler
                Me.Grid(170, 230) = Me.ClosedDoor
                num = 230
            Else
                GoTo Label_0ABF
            End If
Label_0BB3:
            Me.Grid(&HA9, num) = Me.Wall
            Me.Grid(&HAB, num) = Me.Wall
            If (num = 330) Then
                Me.Grid(170, num) = Me.Wall
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 330) Then
                Me.Grid(&HAB, 300) = Me.ClosedDoor
                num = &HAB
            Else
                GoTo Label_0BB3
            End If
Label_0C6C:
            Me.Grid(num, &H12A) = Me.Wall
            Me.Grid(num, &H12E) = Me.Wall
            If (num = 400) Then
                Me.Grid(num, &H12B) = Me.Wall
                Me.Grid(num, 300) = Me.Wall
                Me.Grid(num, &H12D) = Me.Wall
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 400) Then
                Me.Grid(&HAB, 320) = Me.ClosedDoor
                num = &HAB
            Else
                GoTo Label_0C6C
            End If
Label_0CFA:
            Me.Grid(num, &H13F) = Me.Wall
            Me.Grid(num, &H141) = Me.Wall
            If (num = 400) Then
                Me.Grid(num, 320) = Me.Wall
            End If
            Application.DoEvents()
            num = ((num + 1))
            If (num > 400) Then
                Me.Grid(330, &H141) = Me.LockedDoor
                num = &H141
                Do
                    Me.Grid(310, num) = Me.Wall
                    Application.DoEvents()
                    num = ((num + 1))
                Loop While (num <= 400)
                num = 180
                Do
                    Me.Grid(&H2A, num) = Me.Wall
                    Me.Grid(&H30, num) = Me.Wall
                    Application.DoEvents()
                    num = ((num + 1))
                Loop While (num <= 190)
                num = &H2A
                Do
                    Me.Grid(num, 180) = Me.Wall
                    Me.Grid(num, 190) = Me.Wall
                    Application.DoEvents()
                    num = ((num + 1))
                Loop While (num <= &H30)
                Me.Grid(&H2E, 180) = Me.LockedDoor
                Dim y As Integer = Me.y
                num = 1
                Do While (num <= y)
                    Me.Grid(1, num) = Me.Wall
                    Me.Grid(Me.x, num) = Me.Wall
                    Application.DoEvents()
                    num = ((num + 1))
                Loop
            Else
                GoTo Label_0CFA
            End If
        End Sub

        Private Sub BuyAmunition()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Try
                If (Me.Weapons(Me.WPos) <> "control") Then
                    disableGameLoop()
                    Me.NStop = False
                    Dim nWait As Boolean = False
                    Me.NLS((DXSound.string_0 & "\nbuyamunition1.wav"), nWait)
                    Dim inputStr As String = Interaction.InputBox("Amount:", "Buy Amunition", "", -1, -1)
                    If (inputStr = "") Then
                        Me.NStop = True
                    Else
                        Dim num2 As Long
                        Dim num As Integer = (Math.Round(Conversion.Val(inputStr)))
                        If ((num * 5) > Me.Points) Then
                            nWait = True
                            Me.NLS((DXSound.string_0 & "\nbuyamunition2.wav"), nWait)
                            If Me.NStop Then
                                Return
                            End If
                            num2 = num
                            Me.VoiceNumber(num2)
                            num = (num2)
                            Me.NumWait()

                            If Me.NStop Then
                                Return
                            End If
                            nWait = False
                            Me.NLS((DXSound.string_0 & "\nbuyamunition3.wav"), nWait)
                        Else
                            Me.Points = (Me.Points - (num * 5))
                            Select Case Strings.LCase(Me.Weapons(Me.WPos))
                                Case "gun"
                                    Me.Bullets = ((Me.Bullets + (num * 10)))
                                    Exit Select
                                Case "sword"
                                    Me.Swrd = ((Me.Swrd + num))
                                    Exit Select
                                Case "bombs"
                                    Me.bombs = ((Me.bombs + num))
                                    Exit Select
                                Case "laser"
                                    Me.ALaser = ((Me.ALaser + num))
                                    Exit Select
                                Case "gmissile"
                                    Me.short_2 = ((Me.short_2 + num))
                                    Exit Select
                                Case "reflector"
                                    Me.short_3 = ((Me.short_3 + num))
                                    Exit Select
                            End Select
                            nWait = True
                            Me.NLS((DXSound.string_0 & "\nbuyamunition4.wav"), nWait)
                            If Me.NStop Then
                                Return
                            End If
                            num2 = num
                            Me.VoiceNumber(num2)
                            num = (num2)
                            Me.NumWait()

                            If Me.NStop Then
                                Return
                            End If
                            nWait = False
                            Me.NLS((DXSound.string_0 & "\nbuyamunition5.wav"), nWait)
                        End If
                        Me.NStop = True
                    End If
                End If
            Finally
                enableGameLoop()
            End Try
        End Sub

        Private Sub ChangeSpeechRate(a As SpeechRate)
            Dim flag As Boolean
            If a = SpeechRate.slower Then
                Me.CurrentFreq = (Me.CurrentFreq - &H3E8)
                If (Me.CurrentFreq <= &H5A3C) Then
                    Me.CurrentFreq = &H5A3C
                    flag = False
                    Me.NLS((DXSound.string_0 & "\s_min.wav"), flag)
                ElseIf (Me.CurrentFreq = &HAC44) Then
                    flag = False
                    Me.NLS((DXSound.string_0 & "\s_def.wav"), flag)
                Else
                    flag = False
                    Me.NLS((DXSound.string_0 & "\s_slower.wav"), flag)
                End If
            End If
            If a = SpeechRate.quicker Then
                Me.CurrentFreq = (Me.CurrentFreq + &H3E8)
                If (Me.CurrentFreq >= &H1831C) Then
                    Me.CurrentFreq = &H1831C
                    flag = False
                    Me.NLS((DXSound.string_0 & "\s_max.wav"), flag)
                ElseIf (Me.CurrentFreq = &HAC44) Then
                    flag = False
                    Me.NLS((DXSound.string_0 & "\s_def.wav"), flag)
                Else
                    flag = False
                    Me.NLS((DXSound.string_0 & "\s_quicker.wav"), flag)
                End If
            End If
            Interaction.SaveSetting(Addendums.AppTitle, "Config", "SRate", Conversions.ToString(Me.CurrentFreq))
            Me.NStop = True
        End Sub

        Public Sub CharDied(ByRef Optional dPlayDie As Boolean = False)
            Dim str As String
            Dim flag4 As Boolean
            If (Me.h < 1) Then
                disableGameLoop()
                THConstVars.CannotDoKeydown = True
                Me.MuteSounds()

                If Not dPlayDie Then
                    Dim bCloseFirst As Boolean = True
                    Dim bLoopSound As Boolean = False
                    Dim performEffects As Boolean = False
                    Dim x As Integer = 0
                    Dim y As Integer = 0
                    str = ""
                    flag4 = False
                    DXSound.PlaySound(Me.CharDieSound, bCloseFirst, bLoopSound, performEffects, x, y, str, flag4)
                    Do While (Me.CharDieSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                        Application.DoEvents()
                    Loop
                    If Me.IsFightingLast Then
                        If ((Me.HasKilledBrutus And Me.HasKilledBrutus2) And Me.IsFightingLast) Then
                            Me.BombAlarmSound.Stop()
                            flag4 = True
                            performEffects = False
                            bLoopSound = False
                            y = 0
                            x = 0
                            str = ""
                            bCloseFirst = False
                            DXSound.PlaySound(Me.directSoundSecondaryBuffer8_6, flag4, performEffects, bLoopSound, y, x, str, bCloseFirst)
                            Do While (Me.directSoundSecondaryBuffer8_6.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                                Application.DoEvents()
                            Loop
                        ElseIf Me.IsFightingLast Then
                            flag4 = True
                            performEffects = False
                            bLoopSound = False
                            y = 0
                            x = 0
                            str = ""
                            bCloseFirst = False
                            DXSound.PlaySound(Me.directSoundSecondaryBuffer8_7, flag4, performEffects, bLoopSound, y, x, str, bCloseFirst)
                            Do While (Me.directSoundSecondaryBuffer8_7.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                                Application.DoEvents()
                            Loop
                        End If
                    End If
                    str = "r12.wav"
                    flag4 = True
                    DXSound.Radio(str, flag4)
                    If Me.IsFightingLast Then
                        Me.FinalDuelSound.Stop()
                    End If
                    Me.DuelSound.Stop()
                End If
                If ((Me.HasKilledBrutus And Me.HasKilledBrutus2) And Me.IsFightingLast) Then
                    Me.BombAlarmSound.Stop()
                    Me.TargetSound.Stop()

                    Do While (Me.BigBlastSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                        Application.DoEvents()
                    Loop
                End If
                THConstVars.CannotDoKeydown = False
                flag4 = False
                Me.MainMenu(flag4)
            ElseIf ((Me.h <= 30) And Not Me.WarnedOfHealth) Then
                str = "r16.wav"
                flag4 = False
                DXSound.Radio(str, flag4)
                Me.WarnedOfHealth = True
            ElseIf (Me.h > 30) Then
                Me.WarnedOfHealth = False
            End If
        End Sub

        Private Sub ControlPanelWork(ByRef Optional DoCheats As Boolean = False)
            Dim flag As Boolean
            Dim flag3 As Boolean
            Dim flag4 As Boolean
            Dim flag5 As Boolean
            Dim num2 As Integer
            Dim num3 As Integer
            Dim str3 As String
            Dim flag6 As Boolean
            If Me.HasShutDownCard Then
                THConstVars.CannotDoKeydown = True
                Me.HasShutDownCard = False
                flag3 = True
                flag4 = False
                flag5 = False
                num2 = 0
                num3 = 0
                str3 = ""
                flag6 = False
                DXSound.PlaySound(Me.WorkedPanelSound, flag3, flag4, flag5, num2, num3, str3, flag6)
                Me.IsWaitingForMachine = True
                Me.IsFightingLast = True
                str3 = "r8.wav"
                flag6 = True
                DXSound.Radio(str3, flag6)
                flag6 = True
                flag5 = False
                flag4 = False
                num3 = 0
                num2 = 0
                str3 = ""
                flag3 = False
                DXSound.PlaySound(Me.TeleportSound, flag6, flag5, flag4, num3, num2, str3, flag3)
                Me.BLastFightBeginSound = DXSound.LoadSound((DXSound.SoundPath & "\beginl2.wav"))
                Me.directSoundSecondaryBuffer8_7 = DXSound.LoadSound((DXSound.SoundPath & "\brutus4.wav"))
                Me.BigBlastSound = DXSound.LoadSound((DXSound.SoundPath & "\bigblast.wav"))
                Do While (Me.TeleportSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                    Application.DoEvents()
                Loop
                Me.px = 10
                Me.py = 110
                flag6 = True
                flag5 = False
                flag4 = False
                num3 = 0
                num2 = 0
                str3 = ""
                flag3 = False
                DXSound.PlaySound(Me.BLastFightBeginSound, flag6, flag5, flag4, num3, num2, str3, flag3)
                Do While (Me.BLastFightBeginSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                    Application.DoEvents()
                Loop
                Me.challs(1).TurnIntoMaster()
                Me.StartMusic()
                str3 = "r6.wav"
                flag6 = True
                DXSound.Radio(str3, flag6)
                THConstVars.CannotDoKeydown = False
                Return
            End If
            If (DoCheats And (Not Me.WorkedPanel Or Not Me.IsFull)) Then
                flag6 = True
                flag5 = False
                flag4 = False
                num3 = 0
                num2 = 0
                str3 = ""
                flag3 = False
                DXSound.PlaySound(Me.AccessDeniedSound, flag6, flag5, flag4, num3, num2, str3, flag3)
                Return
            End If
            If Not Me.WorkedPanel Then
                flag6 = False
                Me.NLS((DXSound.string_0 & "\npanel2.wav"), flag6)
                Dim str2 As String = Interaction.InputBox("You may begin typing into the panel. Press Escape if you wish not to enter any sequences into the panel.", "Control Panel Keypad", "", -1, -1)
                If (str2 = "") Then
                    Me.NNumber.Stop()
                Else
                    flag = True
                    If (Strings.LCase(str2) = Strings.LCase("kill all! die!")) Then
                        Me.WorkedPanel = True
                        flag6 = True
                        flag5 = False
                        flag4 = False
                        num3 = 0
                        num2 = 0
                        str3 = ""
                        flag3 = False
                        DXSound.PlaySound(Me.WorkedPanelSound, flag6, flag5, flag4, num3, num2, str3, flag3)
                        THConstVars.CannotDoKeydown = True
                        str3 = "r3.wav"
                        flag6 = True
                        DXSound.Radio(str3, flag6)
                        THConstVars.CannotDoKeydown = False
                        GoTo Label_02E2
                    End If
                    flag6 = True
                    flag5 = False
                    flag4 = False
                    num3 = 0
                    num2 = 0
                    str3 = ""
                    flag3 = False
                    DXSound.PlaySound(Me.AccessDeniedSound, flag6, flag5, flag4, num3, num2, str3, flag3)
                End If
                Return
            End If
Label_02E2:
            If DoCheats Then
                If (Me.IsFull And Me.WorkedPanel) Then
                    Dim flag2 As Boolean
                    flag6 = False
                    Me.NLS((DXSound.string_0 & "\npanel3.wav"), flag6)
                    Dim str As String = Interaction.InputBox("Cheat:", "Enter Cheat", "", -1, -1)
                    If (str = "") Then
                        Me.NNumber.Stop()
                        Return
                    End If
                    flag = True
                    Select Case Strings.LCase(str)
                        Case "i hate teleports!"
                            Me.DisableTeleports = True
                            flag2 = True
                            Exit Select
                        Case "i want all weapons!"
                            str3 = "laser"
                            num3 = 3
                            Me.SetWeapon(str3, num3)
                            str3 = "gmissile"
                            num3 = 4
                            Me.SetWeapon(str3, num3)
                            str3 = "reflector"
                            num3 = 5
                            Me.SetWeapon(str3, num3)
                            str3 = "control"
                            num3 = 6
                            Me.SetWeapon(str3, num3)
                            flag2 = True
                            Exit Select
                        Case "unlimit my health now!"
                            Me.UnlimitedHealth = True
                            Me.h = ((Me.h + 200))
                            flag2 = True
                            Exit Select
                        Case "unlimit my bullets now!"
                            Me.UnlimitedBullets = True
                            If (Me.Bullets = 0) Then
                                Me.Bullets = 1
                            End If
                            If (Me.bombs = 0) Then
                                Me.bombs = 1
                            End If
                            If (Me.short_2 = 0) Then
                                Me.short_2 = 1
                            End If
                            If (Me.ALaser = 0) Then
                                Me.ALaser = 1
                            End If
                            If (Me.Swrd = 0) Then
                                Me.Swrd = 1
                            End If
                            If (Me.short_3 = 0) Then
                                Me.short_3 = 1
                            End If
                            If (Me.AControl = 0) Then
                                Me.AControl = 1
                            End If
                            flag2 = True
                            Exit Select
                    End Select
                    If Not flag2 Then
                        flag6 = True
                        flag5 = False
                        flag4 = False
                        num3 = 0
                        num2 = 0
                        str3 = ""
                        flag3 = False
                        DXSound.PlaySound(Me.AccessDeniedSound, flag6, flag5, flag4, num3, num2, str3, flag3)
                        Return
                    End If
                    Me.NNumber.Stop()
                    flag6 = True
                    flag5 = False
                    flag4 = False
                    num3 = 0
                    num2 = 0
                    str3 = ""
                    flag3 = False
                    DXSound.PlaySound(Me.WorkedPanelSound, flag6, flag5, flag4, num3, num2, str3, flag3)
                Else
                    flag6 = True
                    flag5 = False
                    flag4 = False
                    num3 = 0
                    num2 = 0
                    str3 = ""
                    flag3 = False
                    DXSound.PlaySound(Me.AccessDeniedSound, flag6, flag5, flag4, num3, num2, str3, flag3)
                    Return
                End If
            End If
            If (flag AndAlso ((1.0! + Conversion.Int(CSng((2.0! * VBMath.Rnd)))) = 2.0!)) Then
                Me.TeleportsAreClosed = True
                flag6 = True
                flag5 = False
                flag4 = False
                num3 = 0
                num2 = 0
                str3 = ""
                flag3 = False
                DXSound.PlaySound(Me.AlarmSound, flag6, flag5, flag4, num3, num2, str3, flag3)
                Me.DetectRange = 3
                Me.HowMany = 15
                Dim num4 As Integer = (Information.UBound(Me.challs, 1))
                Dim i As Integer = 1
                Do While (i <= num4)
                    Me.GoFight(i)
                    i = ((i + 1))
                Loop
                Me.HowManyNum = 0
            End If
        End Sub

        Private Sub CountdownMachine()
            If (machineTracker * frameTime >= 1000) Then
                machineTracker = 0
                If Me.IsWaitingForMachine Then
                    Dim flag As Boolean
                    Dim flag2 As Boolean
                    Dim flag3 As Boolean
                    Dim num As Integer
                    Dim num2 As Integer
                    Dim str As String
                    Dim flag4 As Boolean
                    If (Me.CMachine <= 0) Then
                        Me.HasTurnedOffMachine = True
                        flag = True
                        flag2 = False
                        flag3 = False
                        num = 0
                        num2 = 0
                        str = ""
                        flag4 = False
                        DXSound.PlaySound(Me.MachineTurnOffSound, flag, flag2, flag3, num, num2, str, flag4)
                        Me.MachineSound.Stop()
                        str = "r9.wav"
                        flag4 = False
                        DXSound.Radio(str, flag4)
                        Me.IsWaitingForMachine = False
                    Else
                        Me.CMachine = ((Me.CMachine - 1))
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.BombBeepSound, flag4, flag3, flag2, num2, num, str, flag)
                    End If
                End If
            Else
                machineTracker += 1
            End If
        End Sub

        Private Sub DepthDecrease()
            If (Me.WDepth > 0) Then
                Me.WDepth = ((Me.WDepth - 1))
            End If
            If (Not IsResting And Me.WDepth = 0) Then
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = False
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.BreathSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                Me.WaterSound.Stop()
                Me.IsResting = True
                Me.DidNotSwim = 0
            End If
        End Sub

        Private Sub DepthIncrease()
            Me.IsResting = False
            If (Me.WDepth < maxDepth) Then
                Me.WDepth = ((Me.WDepth + 1))
            End If
            If (Me.WDepth = 1) Then
                Me.BreathSound.Stop()
                Dim z As Integer = 0
                DXSound.smethod_1(Me.JumpInWaterSound, True, False, Me.px, Me.py, z)
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = True
                Dim performEffects As Boolean = False
                z = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.WaterSound, bCloseFirst, bLoopSound, performEffects, z, y, dVolume, waitTillDone)
                Me.IsResting = False
            End If
        End Sub

        Private Sub Determine()
            Dim flag As Boolean
            Dim flag2 As Boolean
            Dim flag3 As Boolean
            Dim num As Integer
            Dim num2 As Integer
            Dim str As String
            Dim flag4 As Boolean
            If ((Me.Grid(Me.px, Me.py) <> Me.Water) AndAlso Me.IsInWater) Then
                Me.IsInWater = False
                Me.SubPX = 0
                Me.SubPY = 0
                flag = True
                flag2 = False
                flag3 = False
                num = 0
                num2 = 0
                str = ""
                flag4 = False
                DXSound.PlaySound(Me.BreathSound, flag, flag2, flag3, num, num2, str, flag4)
                Me.WaterSound.Stop()
                Me.WDepth = 0
                Me.DidNotSwim = 0
                Me.IsResting = False
                Me.h = Me.WH
                Me.JustCameFromWater = True
            End If
            Dim num4 As Integer = (Math.Round(Conversion.Val(Conversions.ToString(CInt(Me.Grid(Me.px, Me.py))))))
            If (num4 = Me.Treasure) Then
                If Me.HasKilledBrutus Then
                    THConstVars.CannotDoKeydown = True
                    flag4 = True
                    flag3 = False
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.VaultUpSound, flag4, flag3, flag2, num2, num, str, flag)
                    Do While (Me.VaultUpSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                        Application.DoEvents()
                    Loop
                    flag4 = True
                    flag3 = False
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.GetSound, flag4, flag3, flag2, num2, num, str, flag)
                    Me.HasShutDownCard = True
                    THConstVars.CannotDoKeydown = False
                    Return
                End If
                If (Me.HasKey And Me.WorkedPanel) Then
                    If Not Me.IsFightingLast Then
                        THConstVars.CannotDoKeydown = True
                        Me.IsFightingLast = True
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.TeleportSound, flag4, flag3, flag2, num2, num, str, flag)
                        Me.LastFightBeginSound = DXSound.LoadSound((DXSound.SoundPath & "\beginl.wav"))
                        Me.TalkToBrutusSound = DXSound.LoadSound((DXSound.SoundPath & "\talk2brutus.wav"))
                        Me.KillBrutusSound = DXSound.LoadSound((DXSound.SoundPath & "\brutusdietalk.wav"))
                        Me.directSoundSecondaryBuffer8_7 = DXSound.LoadSound((DXSound.SoundPath & "\brutus4.wav"))
                        Do While (Me.TeleportSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                            Application.DoEvents()
                        Loop
                        Me.px = 10
                        Me.py = 110
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.LastFightBeginSound, flag4, flag3, flag2, num2, num, str, flag)
                        Do While (Me.LastFightBeginSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                            Application.DoEvents()
                        Loop
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.TalkToBrutusSound, flag4, flag3, flag2, num2, num, str, flag)
                        Do While (Me.TalkToBrutusSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                            Application.DoEvents()
                        Loop
                        Me.challs(1).TurnIntoMaster()
                        Me.StartMusic()
                        str = "r7.wav"
                        flag4 = True
                        DXSound.Radio(str, flag4)
                        flag4 = False
                        Me.DoIfDemo(flag4)
                        THConstVars.CannotDoKeydown = False
                    Else
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.AccessDeniedSound, flag4, flag3, flag2, num2, num, str, flag)
                    End If
                Else
                    If (Not Me.WorkedPanel And Me.HasKey) Then
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.AccessDeniedSound, flag4, flag3, flag2, num2, num, str, flag)
                        Return
                    End If
                    flag4 = True
                    flag3 = False
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.TurnKnobSound, flag4, flag3, flag2, num2, num, str, flag)
                End If
            Else
                Select Case num4
                    Case 1
                        Me.snake()
                        Exit Sub
                    Case 2
                        Me.Sword()
                        Exit Sub
                    Case 3
                        Me.Bullets = ((Me.Bullets + 10))
                        Me.Grid(Me.px, Me.py) = 0
                        Exit Sub
                    Case 4
                        Me.h = ((Me.h + 5))
                        Me.Grid(Me.px, Me.py) = 0
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.PickUpHealthSound, flag4, flag3, flag2, num2, num, str, flag)
                        Exit Sub
                    Case 5
                        If Not Me.DisableTeleports Then
                            If Not Me.TeleportsAreClosed Then
                                Dim flag5 As Boolean
                                THConstVars.CannotDoKeydown = True
                                flag4 = True
                                flag3 = False
                                flag2 = False
                                num2 = 0
                                num = 0
                                str = ""
                                flag = False
                                DXSound.PlaySound(Me.TeleportSound, flag4, flag3, flag2, num2, num, str, flag)
                                Do While (Me.TeleportSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                                    Application.DoEvents()
                                Loop
                                Me.px = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((Me.x * VBMath.Rnd)))))))
                                Me.py = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((Me.y * VBMath.Rnd)))))))
                                Do While Not flag5
                                    If (((((Me.px > 1) And (Me.py > 1)) And (Me.px < (Me.x - 1))) And (Me.py < (Me.y - 1))) And (Me.py > Conversion.Int(CDbl((CDbl(Me.y) / 10))))) Then
                                        If ((((Me.Grid(Me.px, Me.py) <> Me.Wall) And (Me.Grid(Me.px, Me.py) <> Me.Water)) And (Me.Grid(Me.px, Me.py) <> Me.Mine)) And (Me.Grid(Me.px, Me.py) <> Me.MineGuard)) Then
                                            flag5 = True
                                        Else
                                            Me.px = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((Me.x * VBMath.Rnd)))))))
                                            Me.py = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((Me.y * VBMath.Rnd)))))))
                                        End If
                                    Else
                                        Me.px = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((Me.x * VBMath.Rnd)))))))
                                        Me.py = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((Me.y * VBMath.Rnd)))))))
                                    End If
                                Loop
                                THConstVars.CannotDoKeydown = False
                                Me.Determine()
                            ElseIf (Me.NumAlert = 0) Then
                                Me.TeleportsAreClosed = False
                                Me.Determine()
                            Else
                                flag4 = True
                                flag3 = False
                                flag2 = False
                                num2 = 0
                                num = 0
                                str = ""
                                flag = False
                                DXSound.PlaySound(Me.AccessDeniedSound, flag4, flag3, flag2, num2, num, str, flag)
                            End If
                        End If
                        Exit Sub
                End Select
                If (num4 = Me.Key) Then
                    If Me.WorkedPanel Then
                        Me.HasKey = True
                        Me.Grid(Me.px, Me.py) = 0
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.GetSound, flag4, flag3, flag2, num2, num, str, flag)
                        str = "r5.wav"
                        flag4 = False
                        DXSound.Radio(str, flag4)
                    Else
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.DisChestSound, flag4, flag3, flag2, num2, num, str, flag)
                        Me.HowMany = 15
                        Dim num5 As Integer = (Information.UBound(Me.challs, 1))
                        Dim i As Integer = 1
                        Do While (i <= num5)
                            Me.GoFight(i)
                            Application.DoEvents()
                            i = ((i + 1))
                        Loop
                        Me.HowManyNum = 0
                    End If
                ElseIf (num4 = Me.ControlPanel) Then
                    flag4 = False
                    Me.ControlPanelWork(flag4)
                ElseIf (num4 = Me.RBomb) Then
                    num2 = 10
                    num = 0
                    Me.ReflectHit(num2, num)
                    Me.Grid(Me.px, Me.py) = Me.BGrid(Me.px, Me.py)
                ElseIf (num4 = Me.Water) Then
                    If Not Me.IsFightingLast Then
                        If Not Me.IsInWater Then
                            Me.IsInWater = True
                            Me.BreathSound.Stop()
                            num2 = 0
                            DXSound.smethod_1(Me.JumpInWaterSound, True, False, Me.px, Me.py, num2)
                            flag4 = True
                            flag3 = True
                            flag2 = False
                            num2 = 0
                            num = 0
                            str = ""
                            flag = False
                            DXSound.PlaySound(Me.WaterSound, flag4, flag3, flag2, num2, num, str, flag)
                            Me.IsResting = False
                            Me.WH = Me.h
                            Me.WDepth = 1
                        End If
                    Else
                        Me.px = Me.TX
                        Me.py = Me.TY
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(Me.AccessDeniedSound, flag4, flag3, flag2, num2, num, str, flag)
                    End If
                ElseIf (num4 = Me.Laser) Then
                    str = "laser"
                    num2 = 0
                    Me.SetWeapon(str, num2)
                    flag4 = True
                    flag3 = False
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.PickUpSWeaponSound, flag4, flag3, flag2, num2, num, str, flag)
                    Me.ALaser = 200
                    num2 = 0
                    Me.AllThingReplace(Me.px, Me.py, num2)
                ElseIf (num4 = Me.Missile) Then
                    str = "gmissile"
                    num2 = 0
                    Me.SetWeapon(str, num2)
                    flag4 = True
                    flag3 = False
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.PickUpSWeaponSound, flag4, flag3, flag2, num2, num, str, flag)
                    Me.short_2 = 3
                    num2 = 0
                    Me.AllThingReplace(Me.px, Me.py, num2)
                ElseIf (num4 = Me.Reflector) Then
                    str = "reflector"
                    num2 = 0
                    Me.SetWeapon(str, num2)
                    flag4 = True
                    flag3 = False
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.PickUpSWeaponSound, flag4, flag3, flag2, num2, num, str, flag)
                    Me.short_3 = 5
                    num2 = 0
                    Me.AllThingReplace(Me.px, Me.py, num2)
                ElseIf (num4 = Me.controler) Then
                    str = "control"
                    num2 = 0
                    Me.SetWeapon(str, num2)
                    flag4 = True
                    flag3 = False
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.PickUpSWeaponSound, flag4, flag3, flag2, num2, num, str, flag)
                    Me.AControl = 100
                    num2 = 0
                    Me.AllThingReplace(Me.px, Me.py, num2)
                ElseIf (num4 = Me.Mine) Then
                    If Not Me.UnlimitedHealth Then
                        Me.h = 0
                    Else
                        num2 = 0
                        Me.AllThingReplace(Me.px, Me.py, num2)
                    End If
                    num2 = 0
                    DXSound.smethod_1(Me.GExplodeSound, True, False, Me.px, Me.py, num2)
                End If
            End If
            If Not Me.IsChall(Me.px, Me.py) Then
                Me.BGrid(Me.px, Me.py) = Me.Grid(Me.px, Me.py)
            End If
        End Sub

        Private Sub DisableInDemo()
            THF.F.Text = (Me.DefCaption & ": Demonstration Version")
        End Sub

        Protected Overrides Sub Dispose(ByVal Disposing As Boolean)
            If (Disposing AndAlso (Not Me.components Is Nothing)) Then
                Me.components.Dispose()
            End If
            MyBase.Dispose(Disposing)
        End Sub

        Public Sub DoIfDemo(ByRef Optional tdisplay As Boolean = False)
            If (Not Me.IsFull AndAlso Not tdisplay) Then
                Me.NNumber.Stop()
                Me.NStop = False
                Addendums.WriteToClipboard("http://www.bpcprograms.com")
                Dim nWait As Boolean = True
                Me.NLS((DXSound.string_0 & "\ndemo.wav"), nWait)
                Interaction.MsgBox(("Thank you for trying the demo version of " & Addendums.AppTitle & ". You can find out more about this game or purchase it at our website: http://www.bpcprograms.com."), MsgBoxStyle.Information, "Demo")
                Me.ShutDown()
            End If
        End Sub

        Private Sub DoIfEscapingFromBomb()
            If (bombTracker * frameTime >= 1000) Then
                bombTracker = 0
                If Me.IsEscapingFromBomb Then
                    Me.EBomb = ((Me.EBomb - 1))
                    Dim bCloseFirst As Boolean = True
                    Dim bLoopSound As Boolean = False
                    Dim performEffects As Boolean = False
                    Dim x As Integer = 0
                    Dim y As Integer = 0
                    Dim dVolume As String = ""
                    Dim waitTillDone As Boolean = False
                    DXSound.PlaySound(Me.BombBeepSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                    If (Me.EBomb <= 60) Then
                        waitTillDone = False
                        performEffects = True
                        bLoopSound = False
                        y = 0
                        x = 0
                        dVolume = ""
                        bCloseFirst = False
                        DXSound.PlaySound(Me.BombAlarmSound, waitTillDone, performEffects, bLoopSound, y, x, dVolume, bCloseFirst)
                    End If
                    If (Me.EBomb <= 0) Then
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        dVolume = ""
                        bCloseFirst = False
                        DXSound.PlaySound(Me.BigBlastSound, waitTillDone, performEffects, bLoopSound, y, x, dVolume, bCloseFirst)
                        Me.MuteSounds()
                        Me.h = 0
                    End If
                End If
            Else
                bombTracker += 1
            End If
        End Sub

        Private Sub DoIfInWater()
            If IsInWater Then
                If waterTracker * frameTime >= 1000 Then
                    waterTracker = 0
                    If Not Me.IsResting Then
                        Me.DidNotSwim = ((Me.DidNotSwim + 1))
                        If (Me.DidNotSwim >= 2) Then
                            Me.DepthIncrease()
                        End If
                        If (Me.WDepth < 10) Then
                            Me.SubHealth(10)
                        Else
                            Me.SubHealth(20)
                        End If
                    ElseIf (Me.h < Me.WH) Then ' If resting
                        Me.h = ((Me.h + 3))
                    End If
                Else ' if waterTracker < tick
                    waterTracker += 1
                End If
            End If
        End Sub

        Private Sub EMove_Tick()
            If Not HasDoneInit Then
                Exit Sub
            End If
            For i As Integer = 1 To Me.ChallAmount
                Me.challs(i).Activate()
            Next i
        End Sub

        Private Sub EnableInFull()
            Me.IsFull = True
            THF.F.Text = (Me.DefCaption & ": Registered to Open-Source")
        End Sub

        Private Sub EndControl()
            If Me.IsControling Then
                Dim wControl As Integer
                If (controlTracker * frameTime >= 1000) Then
                    controlTracker = 0
                    If ((Me.SControl >= 60) Or (Me.challs(CInt(Me.WControl)).A <= 0)) Then
                        Me.IsControling = False
                        Me.SControl = 0
                        Me.challs(CInt(Me.WControl)).IsBeingControled = False
                        If Not Me.challs(CInt(Me.WControl)).IsMaster Then
                            Me.challs(CInt(Me.WControl)).A = 0
                        Else
                            wControl = CInt(Me.WControl)
                            Me.challs(wControl).A = ((Me.challs(wControl).A - 5))
                        End If
                        Me.WControl = 0
                    Else
                        Me.SControl = ((Me.SControl + 1))
                    End If
                Else
                    wControl = 7
                    controlTracker += 1
                End If
            End If
        End Sub

        Private Sub EndReflector()
            If Me.DoingReflector Then
                If (Me.reflectorTime * frameTime >= 30000) Then
                    Me.DoingReflector = False
                    Me.ReflectorSound.Stop()
                Else
                    Me.reflectorTime += 1
                End If
            End If
        End Sub

        Private Sub ExitGame()
            Me.NStop = True
            Me.NNumber.Stop()
            Dim nWait As Boolean = False
            Me.NLS((DXSound.string_0 & "\exiting1.wav"), nWait)
            Dim num As Integer = (Interaction.MsgBox("Are you sure you want to exit Treasure Hunt?", (MsgBoxStyle.Question Or MsgBoxStyle.YesNo), "Question"))
            If (num = 6) Then
                Me.ShutDown()
            End If
        End Sub

        Private Sub file_load_click()
            Try
                disableGameLoop()
                System.Diagnostics.Debug.WriteLine("Entered load")
                Dim num20 As Integer
                Dim flag As Boolean
                Dim flag2 As Boolean
                Dim flag3 As Boolean
                Dim num2 As Integer
                Dim num3 As Integer
                Dim str As String
                Dim flag4 As Boolean
                Dim num As Integer = 2
                Me.FileName = ""
                If (Not Me.IsFull Or Me.IsFightingLast) Then
                    flag = True
                    flag2 = False
                    flag3 = False
                    num2 = 0
                    num3 = 0
                    str = ""
                    flag4 = False
                    DXSound.PlaySound(Me.AccessDeniedSound, flag, flag2, flag3, num2, num3, str, flag4)
                Else
                    Me.NNumber.Stop()
                    flag4 = False
                    ' NStop = False
                    Me.NLS((DXSound.string_0 & "\nloadgame1.wav"), flag4)
                    System.Diagnostics.Debug.WriteLine("in load Passed nls")
                    Me.FileName = InputBox("Slot (1-3):", "Load Game", "", -1, -1)
                    System.Diagnostics.Debug.WriteLine("load: inputbox returned")
                    If (Me.FileName <> "") Then
                        If ((Conversion.Val(Me.FileName) > 0) And (Conversion.Val(Me.FileName) < 4)) Then
                            Dim num5 As Integer
                            Dim num7 As Integer
                            FileSystem.FileOpen(1, (Addendums.FilePath & "\thsave" & Me.FileName & ".ths"), OpenMode.Input, OpenAccess.Default, OpenShare.Default, -1)
                            Me.NNumber.Stop()
                            flag4 = True
                            flag3 = True
                            flag2 = False
                            num3 = 0
                            num2 = 0
                            str = ""
                            flag = False
                            DXSound.PlaySound(Me.ClickSound, flag4, flag3, flag2, num3, num2, str, flag)
                            THConstVars.CannotDoKeydown = True
                            flag4 = True
                            flag3 = False
                            flag2 = False
                            num3 = 0
                            num2 = 0
                            str = ""
                            flag = False
                            DXSound.PlaySound(Me.directSoundSecondaryBuffer8_0, flag4, flag3, flag2, num3, num2, str, flag)
                            Do While (Me.directSoundSecondaryBuffer8_0.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                            Loop
                            FileSystem.Input(1, Me.IsFirstTimeLoading)
                            Dim x As Integer = Me.x
                            num5 = 1
                            Do While (num5 <= x)
                                Dim y As Integer = Me.y
                                num7 = 1
                                Do While (num7 <= y)
                                    FileSystem.Input(1, Me.CGrid(num5, num7))
                                    Application.DoEvents()
                                    num7 = ((num7 + 1))
                                Loop
                                num5 = ((num5 + 1))
                            Loop
                            FileSystem.Input(1, Me.IsControling)
                            FileSystem.Input(1, Me.SControl)
                            FileSystem.Input(1, Me.WControl)
                            FileSystem.Input(1, Me.AControl)
                            FileSystem.Input(1, Me.UnlimitedHealth)
                            FileSystem.Input(1, Me.UnlimitedBullets)
                            FileSystem.Input(1, Me.h)
                            FileSystem.Input(1, Me.x)
                            FileSystem.Input(1, Me.y)
                            FileSystem.Input(1, Me.Swrd)
                            FileSystem.Input(1, Me.Bullets)
                            FileSystem.Input(1, Me.Accuracy)
                            FileSystem.Input(1, Me.WinX)
                            FileSystem.Input(1, Me.WinY)
                            FileSystem.Input(1, Me.px)
                            FileSystem.Input(1, Me.py)
                            FileSystem.Input(1, Me.ch)
                            FileSystem.Input(1, Me.HasKey)
                            FileSystem.Input(1, Me.WorkedPanel)
                            FileSystem.Input(1, Me.Points)
                            FileSystem.Input(1, Me.bombs)
                            FileSystem.Input(1, Me.KeyX)
                            FileSystem.Input(1, Me.KeyY)
                            FileSystem.Input(1, Me.PanelX)
                            FileSystem.Input(1, Me.PanelY)
                            FileSystem.Input(1, Me.IsFightingLast)
                            FileSystem.Input(1, Me.HasKilledBrutus)
                            FileSystem.Input(1, Me.WH)
                            FileSystem.Input(1, Me.MineX)
                            FileSystem.Input(1, Me.MineY)
                            FileSystem.Input(1, Me.BlewUpMineControl)
                            FileSystem.Input(1, Me.ALaser)
                            FileSystem.Input(1, Me.short_2)
                            FileSystem.Input(1, Me.short_3)
                            FileSystem.Input(1, Me.IsDoingGMissile)
                            FileSystem.Input(1, Me.GSpeed)
                            FileSystem.Input(1, Me.GFront)
                            FileSystem.Input(1, Me.IsInWater)
                            FileSystem.Input(1, Me.WDepth)
                            FileSystem.Input(1, Me.SubPX)
                            FileSystem.Input(1, Me.SubPY)
                            FileSystem.Input(1, Me.DoingReflector)
                            FileSystem.Input(1, Me.GX)
                            FileSystem.Input(1, Me.GY)
                            FileSystem.Input(1, Me.RCount)
                            FileSystem.Input(1, Me.IsWaitingForMachine)
                            FileSystem.Input(1, Me.CMachine)
                            FileSystem.Input(1, Me.HasShutDownCard)
                            FileSystem.Input(1, Me.DisableTeleports)
                            FileSystem.Input(1, Me.NumAlert)
                            FileSystem.Input(1, Me.ChallNum)
                            FileSystem.Input(1, Me.ChallAmount)
                            FileSystem.Input(1, Me.WPos)
                            FileSystem.Input(1, Me.HasTurnedOffMachine)
                            FileSystem.Input(1, Me.HasBlownUpMachine)
                            FileSystem.Input(1, Me.MachineX)
                            FileSystem.Input(1, Me.MachineY)
                            FileSystem.Input(1, Me.ExitX)
                            FileSystem.Input(1, Me.ExitY)
                            FileSystem.Input(1, Me.EBomb)
                            FileSystem.Input(1, Me.IsEscapingFromBomb)
                            FileSystem.Input(1, Me.HasMainKey)
                            FileSystem.Input(1, Me.HasKilledMouse)
                            FileSystem.Input(1, Me.HasKilledBrutus2)
                            FileSystem.Input(1, Me.SGen)
                            FileSystem.Input(1, THConstVars.Difficulty)
                            Dim num8 As Integer = (Information.UBound(Me.Weapons, 1))
                            num5 = 0
                            Do While (num5 <= num8)
                                FileSystem.Input(1, Me.Weapons(num5))
                                num5 = ((num5 + 1))
                            Loop
                            Me.challs = New chall((Me.ChallAmount + 1) - 1) {}
                            Dim challAmount As Integer = Me.ChallAmount
                            num5 = 1
                            Do While (num5 <= challAmount)
                                Dim num10 As Integer
                                Dim flag5 As Boolean
                                Dim num11 As Integer
                                Dim num12 As Integer
                                Dim num13 As Integer
                                Dim num14 As Integer
                                Dim flag6 As Boolean
                                Dim flag7 As Boolean
                                Dim flag8 As Boolean
                                Dim flag9 As Boolean
                                Dim flag10 As Boolean
                                Dim flag11 As Boolean
                                Dim flag12 As Boolean
                                Me.challs(num5) = New chall
                                FileSystem.Input(1, num10)
                                FileSystem.Input(1, flag5)
                                FileSystem.Input(1, num11)
                                FileSystem.Input(1, num12)
                                FileSystem.Input(1, num13)
                                FileSystem.Input(1, num14)
                                FileSystem.Input(1, flag6)
                                FileSystem.Input(1, flag7)
                                FileSystem.Input(1, flag8)
                                FileSystem.Input(1, flag9)
                                FileSystem.Input(1, flag10)
                                FileSystem.Input(1, flag11)
                                FileSystem.Input(1, flag12)
                                Me.challs(num5).init(num11, num12, num5, num10, num13, num14, flag6)
                                Me.challs(num5).IsFighting = flag5
                                Me.challs(num5).HasKey = flag8
                                Me.challs(num5).HasFoundOnce = flag9
                                Me.challs(num5).IsBeingControled = flag10
                                Me.challs(num5).IsPoisoned = flag11
                                Me.challs(num5).NumOfNeedles = (-(flag12 > False))
                                If flag7 Then
                                    Me.challs(num5).TurnIntoMaster()
                                    Me.challs(num5).A = num10
                                End If
                                Application.DoEvents()
                                num5 = ((num5 + 1))
                            Loop
                            Dim num15 As Integer = Me.x
                            num5 = 1
                            Do While (num5 <= num15)
                                Dim y As Integer = Me.y
                                num7 = 1
                                Do While (num7 <= y)
                                    FileSystem.Input(1, Me.Grid(num5, num7))
                                    Application.DoEvents()
                                    num7 = ((num7 + 1))
                                Loop
                                num5 = ((num5 + 1))
                            Loop
                            Dim num17 As Integer = Me.x
                            num5 = 1
                            Do While (num5 <= num17)
                                Dim y As Integer = Me.y
                                num7 = 1
                                Do While (num7 <= y)
                                    Me.BGrid(num5, num7) = Me.Grid(num5, num7)
                                    num7 = ((num7 + 1))
                                Loop
                                num5 = ((num5 + 1))
                            Loop
                            FileSystem.Reset()

                            If Me.HasKilledBrutus Then
                                flag4 = False
                                Me.DoIfDemo(flag4)
                            End If
                            If (Me.IsWaitingForMachine Or Me.HasTurnedOffMachine) Then
                                Me.BigBlastSound = DXSound.LoadSound((DXSound.SoundPath & "\bigblast.wav"))
                            End If
                            Me.StopTimer()
                            Me.ClickSound.Stop()
                            flag4 = True
                            flag3 = False
                            flag2 = False
                            num3 = 0
                            num2 = 0
                            str = ""
                            flag = False
                            DXSound.PlaySound(Me.NLoadSuccessfulSound, flag4, flag3, flag2, num3, num2, str, flag)
                            Do While (Me.NLoadSuccessfulSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                                Application.DoEvents()
                            Loop
                            Me.StartMusic()
                            Me.UnmuteSounds()
                            THConstVars.CannotDoKeydown = False
                            Me.Determine()
                        Else
                            Me.NStop = False
                            Me.NNumber.Stop()
                            flag4 = True
                            Me.NLS((DXSound.string_0 & "\nnotslot.wav"), flag4)
                            Me.FileName = ""
                        End If
                    Else
                        Me.NNumber.Stop()
                    End If
                End If
                Me.NStop = False
                Me.FileName = ""
                Me.NNumber.Stop()
                Me.ClickSound.Stop()
            Finally
                FileSystem.Reset()
                enableGameLoop()
            End Try
        End Sub

        Private Sub file_save_click()
            Try
                disableGameLoop()
                Dim flag As Boolean
                Dim flag2 As Boolean
                Dim flag3 As Boolean
                Dim num3 As Integer
                Dim num4 As Integer
                Dim str As String
                Dim flag4 As Boolean
                Me.FileName = ""
                If (Not Me.IsFull Or Me.IsFightingLast) Then
                    flag = True
                    flag2 = False
                    flag3 = False
                    num3 = 0
                    num4 = 0
                    str = ""
                    flag4 = False
                    DXSound.PlaySound(Me.AccessDeniedSound, flag, flag2, flag3, num3, num4, str, flag4)
                Else
                    Me.NNumber.Stop()
                    Me.NStop = False
                    flag4 = False
                    Me.NLS((DXSound.string_0 & "\nsavegame1.wav"), flag4)
                    Me.FileName = Interaction.InputBox("slot (1-3):", "Save Game", "", -1, -1)
                    If (Me.FileName <> "") Then
                        If ((Conversion.Val(Me.FileName) > 0) And (Conversion.Val(Me.FileName) < 4)) Then
                            Dim num As Integer
                            Dim num2 As Integer
                            FileSystem.FileOpen(1, (Addendums.FilePath & "\thsave" & Me.FileName & ".ths"), OpenMode.Output, OpenAccess.Default, OpenShare.Default, -1)
                            Me.NNumber.Stop()
                            flag4 = False
                            Me.NLS((DXSound.string_0 & "\nsavegame2.wav"), flag4)
                            flag4 = True
                            flag3 = True
                            flag2 = False
                            num4 = 0
                            num3 = 0
                            str = ""
                            flag = False
                            DXSound.PlaySound(Me.ClickSound, flag4, flag3, flag2, num4, num3, str, flag)
                            THConstVars.CannotDoKeydown = True
                            FileSystem.WriteLine(1, New Object() {Me.IsFirstTimeLoading})
                            Dim x As Integer = Me.x
                            num = 1
                            Do While (num <= x)
                                Dim y As Integer = Me.y
                                num2 = 1
                                Do While (num2 <= y)
                                    FileSystem.WriteLine(1, New Object() {Me.CGrid(num, num2)})
                                    Application.DoEvents()
                                    num2 = ((num2 + 1))
                                Loop
                                num = ((num + 1))
                            Loop
                            FileSystem.WriteLine(1, New Object() {Me.IsControling, Me.SControl, Me.WControl, Me.AControl})
                            FileSystem.WriteLine(1, New Object() {Me.UnlimitedHealth, Me.UnlimitedBullets, Me.h, Me.x, Me.y, Me.Swrd, Me.Bullets, Me.Accuracy, Me.WinX, Me.WinY, Me.px, Me.py, Me.ch, Me.HasKey, Me.WorkedPanel, Me.Points, Me.bombs, Me.KeyX, Me.KeyY, Me.PanelX, Me.PanelY, Me.IsFightingLast, Me.HasKilledBrutus})
                            FileSystem.WriteLine(1, New Object() {Me.WH, Me.MineX, Me.MineY, Me.BlewUpMineControl, Me.ALaser, Me.short_2, Me.short_3, Me.IsDoingGMissile, Me.GSpeed, Me.GFront, Me.IsInWater, Me.WDepth, Me.SubPX, Me.SubPY, Me.DoingReflector, Me.GX, Me.GY, Me.RCount})
                            FileSystem.WriteLine(1, New Object() {Me.IsWaitingForMachine, Me.CMachine, Me.HasShutDownCard, Me.DisableTeleports, Me.NumAlert, Me.ChallNum, Me.ChallAmount, Me.WPos, Me.HasTurnedOffMachine, Me.HasBlownUpMachine, Me.MachineX, Me.MachineY, Me.ExitX, Me.ExitY, Me.EBomb, Me.IsEscapingFromBomb})
                            FileSystem.WriteLine(1, New Object() {Me.HasMainKey, Me.HasKilledMouse, Me.HasKilledBrutus2, Me.SGen, THConstVars.Difficulty})
                            Dim num7 As Integer = (Information.UBound(Me.Weapons, 1))
                            num = 0
                            Do While (num <= num7)
                                FileSystem.WriteLine(1, New Object() {Me.Weapons(num)})
                                num = ((num + 1))
                            Loop
                            Dim num8 As Integer = (Information.UBound(Me.challs, 1))
                            num = 1
                            Do While (num <= num8)
                                FileSystem.WriteLine(1, New Object() {Me.challs(num).A, Me.challs(num).IsFighting, Me.challs(num).x, Me.challs(num).y, Me.challs(num).CSeconds, Me.challs(num).HSeconds, Me.challs(num).IsDeadValid, Me.challs(num).IsMaster, Me.challs(num).HasKey, Me.challs(num).HasFoundOnce, Me.challs(num).IsBeingControled, Me.challs(num).IsPoisoned, Me.challs(num).NumOfNeedles})
                                Application.DoEvents()
                                num = ((num + 1))
                            Loop
                            Dim num9 As Integer = Me.x
                            num = 1
                            Do While (num <= num9)
                                Dim y As Integer = Me.y
                                num2 = 1
                                Do While (num2 <= y)
                                    FileSystem.WriteLine(1, New Object() {Me.BGrid(num, num2)})
                                    Application.DoEvents()
                                    num2 = ((num2 + 1))
                                Loop
                                num = ((num + 1))
                            Loop
                            FileSystem.FileClose(New Integer() {1})
                            Me.NNumber.Stop()
                            flag4 = False
                            Me.NLS((DXSound.string_0 & "\nsavegame3.wav"), flag4)
                            THConstVars.CannotDoKeydown = False
                            Me.ClickSound.Stop()
                            Me.NStop = True
                        Else
                            Me.NNumber.Stop()
                            flag4 = False
                            Me.NLS((DXSound.string_0 & "\nnotslot.wav"), flag4)
                            Me.NStop = True
                        End If
                    Else
                        Me.NNumber.Stop()
                        Me.NStop = True
                    End If
                End If
            Finally
                FileSystem.Reset()
                enableGameLoop()
            End Try
        End Sub

        Private Sub FindPassage(ByRef Optional OnlyDoPassage As Boolean = False)
            Dim x As Integer
            Dim y As Integer
            Dim num5 As Integer
            Dim num6 As Integer
            If Me.IsControling Then
                x = Me.challs(CInt(Me.WControl)).x
                y = Me.challs(CInt(Me.WControl)).y
            Else
                x = Me.px
                y = Me.py
            End If
            Dim passageWest As Boolean = Me.GetPassageWest
            Dim passageEast As Boolean = Me.GetPassageEast
            Dim passageSouth As Boolean = Me.GetPassageSouth
            Dim passageNorth As Boolean = Me.GetPassageNorth
            Dim passageCurrent As Boolean = Me.GetPassageCurrent
            Dim doorWest As Boolean = Me.GetDoorWest
            Dim doorEast As Boolean = Me.GetDoorEast
            Dim doorSouth As Boolean = Me.GetDoorSouth
            Dim doorNorth As Boolean = Me.GetDoorNorth
            If passageCurrent Then
                Dim cX As Integer = ((x - 1))
                Dim num4 As Integer = ((x + 1))
                If (Not Me.GetBlock(cX, y) And Not Me.GetBlock(num4, y)) Then
                    num5 = ((x - 1))
                    num6 = 0
                    DXSound.smethod_1(Me.PassageSound(1), False, True, num5, y, num6)
                    num6 = ((x + 1))
                    num5 = 0
                    DXSound.smethod_1(Me.PassageSound(2), False, True, num6, y, num5)
                    Return
                End If
                num6 = ((y - 1))
                num5 = ((y + 1))
                If (Not Me.GetBlock(x, num6) And Not Me.GetBlock(x, num5)) Then
                    num4 = ((y + 1))
                    cX = 0
                    DXSound.smethod_1(Me.PassageSound(1), False, True, x, num4, cX)
                    num6 = ((y - 1))
                    num5 = 0
                    DXSound.smethod_1(Me.PassageSound(2), False, True, x, num6, num5)
                    Return
                End If
            End If
            If passageWest Then
                num6 = ((x - 1))
                num5 = 0
                DXSound.smethod_1(Me.PassageSound(1), False, True, num6, y, num5)
            Else
                Me.PassageSound(1).Stop()
            End If
            If passageEast Then
                num6 = ((x + 1))
                num5 = 0
                DXSound.smethod_1(Me.PassageSound(2), False, True, num6, y, num5)
            Else
                Me.PassageSound(2).Stop()
            End If
            If passageSouth Then
                num6 = ((y + 1))
                num5 = 0
                DXSound.smethod_1(Me.PassageSound(3), False, True, x, num6, num5)
            Else
                Me.PassageSound(3).Stop()
            End If
            If passageNorth Then
                num6 = ((y - 1))
                num5 = 0
                DXSound.smethod_1(Me.PassageSound(4), False, True, x, num6, num5)
            Else
                Me.PassageSound(4).Stop()
            End If
            If Not OnlyDoPassage Then
                If doorWest Then
                    num6 = ((x - 1))
                    num5 = 0
                    DXSound.smethod_1(Me.DoorSound(1), False, True, num6, y, num5)
                Else
                    Me.DoorSound(1).Stop()
                End If
                If doorEast Then
                    num6 = ((x + 1))
                    num5 = 0
                    DXSound.smethod_1(Me.DoorSound(2), False, True, num6, y, num5)
                Else
                    Me.DoorSound(2).Stop()
                End If
                If doorSouth Then
                    num6 = ((y + 1))
                    num5 = 0
                    DXSound.smethod_1(Me.DoorSound(3), False, True, x, num6, num5)
                Else
                    Me.DoorSound(3).Stop()
                End If
                If doorNorth Then
                    num6 = ((y - 1))
                    num5 = 0
                    DXSound.smethod_1(Me.DoorSound(4), False, True, x, num6, num5)
                Else
                    Me.DoorSound(4).Stop()
                End If
            End If
        End Sub

        Private Sub FireRemoteControlMissile()
            If Not Me.IsDoingGMissile Then
                If (Me.short_2 > 0) Then
                    If Not Me.UnlimitedBullets Then
                        Me.short_2 = ((Me.short_2 - 1))
                    End If
                    Dim bCloseFirst As Boolean = True
                    Dim bLoopSound As Boolean = False
                    Dim performEffects As Boolean = False
                    Dim x As Integer = 0
                    Dim y As Integer = 0
                    Dim dVolume As String = ""
                    Dim waitTillDone As Boolean = False
                    DXSound.PlaySound(Me.GLaunchSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                    Me.GX = Me.px
                    Me.GY = Me.py
                    Me.GFront = 0
                    Me.GSpeed = 6
                    Me.IsDoingGMissile = True
                End If
            Else
                Me.IsDoingGMissile = False
                Me.GExplodeMissile()
            End If
        End Sub

        Private Sub GChangeSpeed(ByRef dir_Renamed As String)
            If (dir_Renamed = "u") Then
                If ((Me.GSpeed - 1) < 0) Then
                    Me.GSpeed = 0
                Else
                    Me.GSpeed = ((Me.GSpeed - 1))
                End If
            End If
            If (dir_Renamed = "d") Then
                If ((Me.GSpeed + 1) > 10) Then
                    Me.GSpeed = 10
                Else
                    Me.GSpeed = ((Me.GSpeed + 1))
                End If
            End If
            Me.GCount = 0
            Dim gSpeed As Long = Me.GSpeed
            Me.VoiceNumber(gSpeed)
            Me.GSpeed = (gSpeed)
        End Sub

        Private Function GenerateMenu(ByRef intro As String, ByRef menu_Renamed As String, ByRef Optional StartPos As Integer = 0) As Integer
            Dim hasSaid As Boolean = False
            Me.IsInMenu = True
            Dim array As String() = Strings.Split(menu_Renamed, "|", -1, CompareMethod.Binary)
            Dim num2 As Integer = (Information.UBound(array, 1))
            Me.NStop = False
            If (intro <> "") Then
                Me.NLS((DXSound.string_0 & "\" & intro), True)
            End If
            Me.MenuPos = StartPos
            If (Me.MenuPos < 0) Then
                Me.MenuPos = 0
            End If
            Do While (Not DXInput.isFirstPress(SharpDX.DirectInput.Key.Return))
                If DXInput.isFirstPress(SharpDX.DirectInput.Key.Up) Or DXInput.isFirstPress(SharpDX.DirectInput.Key.Left) Then
                    Me.MenuPos = ((Me.MenuPos - 1))
                    hasSaid = False
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.Down) Or DXInput.isFirstPress(SharpDX.DirectInput.Key.Right) Then
                    Me.MenuPos = ((Me.MenuPos + 1))
                    hasSaid = False
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.Home) Then
                    Me.MenuPos = 0
                    hasSaid = False
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.End) Then
                    Me.MenuPos = num2
                    hasSaid = False
                End If
                If (Me.MenuPos < 0) Then
                    Me.MenuPos = num2
                End If
                If (Me.MenuPos > num2) Then
                    Me.MenuPos = 0
                End If
                If Not hasSaid Then
                    Me.NLS((DXSound.string_0 & "\" & array(Me.MenuPos)), False)
                    hasSaid = True
                End If
                Application.DoEvents()
            Loop
            Me.IsInMenu = False
            Me.NStop = True
            Return ((Me.MenuPos + 1))
        End Function

        Private Sub GenHealth()
            If (Me.SGen * frameTime < 60 * 10 * 1000) Then
                Me.SGen = ((Me.SGen + 1))
            Else
                Me.SGen = 0
                Dim num As Integer = &H2B
                Do While True
                    Dim num2 As Integer = &HB5
                    Do
                        Me.Grid(num, num2) = 4
                        Me.BGrid(num, num2) = 4
                        num2 = ((num2 + 1))
                    Loop While (num2 <= &HBD)
                    num = ((num + 1))
                    If (num > &H2F) Then
                        Dim file As String = "r1.wav"
                        Dim rWait As Boolean = False
                        DXSound.Radio(file, rWait)
                        Return
                    End If
                Loop
            End If
        End Sub

        Public Function GetBGrid(ByRef x As Integer, ByRef y As Integer) As Integer
            Return (Math.Round(Conversion.Val(Conversions.ToString(CInt(Me.BGrid(x, y))))))
        End Function

        Private Function GetBlock(ByRef CX As Integer, ByRef CY As Integer) As Boolean
            Dim flag As Boolean
            If ((((CX < 1) Or (CX > Me.x)) Or (CY < 1)) Or (CY > Me.y)) Then
                Return True
            End If
            Dim num As Integer = Me.BGrid(CX, CY)
            If Not (((num <> Me.Wall) And (num <> Me.ClosedDoor)) And (num <> Me.LockedDoor)) Then
                flag = True
            End If
            Return flag
        End Function

        Public Function GetChall(ByVal ID As Long) As Object
            Return Me.challs(CInt(ID))
        End Function

        Private Function GetChallBlock(ByRef CX As Integer, ByRef CY As Integer) As Boolean
            Dim flag As Boolean
            If ((((CX < 1) Or (CX > Me.x)) Or (CY < 1)) Or (CY > Me.y)) Then
                Return True
            End If
            Dim num As Integer = Me.BGrid(CX, CY)
            If Not (((((num <> Me.Water) And (num <> Me.Wall)) And (num <> Me.ClosedDoor)) And (num <> Me.LockedDoor)) And (num <> Me.Mine)) Then
                flag = True
            End If
            Return flag
        End Function

        Private Function GetChallID(ByRef x As Integer, ByRef y As Integer) As Integer
            Dim num As Integer
            Dim challAmount As Integer = Me.ChallAmount
            Dim i As Integer = 1
            Do While (i <= challAmount)
                If ((Me.challs(i).x = x) And (Me.challs(i).y = y)) Then
                    Return i
                End If
                i = ((i + 1))
            Loop
            Return num
        End Function

        Private Function GetDoor(ByRef x As Integer, ByRef y As Integer) As Boolean
            Dim flag As Boolean
            If ((Me.BGrid(x, y) = Me.ClosedDoor) Or (Me.BGrid(x, y) = Me.LockedDoor)) Then
                flag = True
            End If
            Return flag
        End Function

        Private Function GetDoorBlock(ByRef x As Integer, ByRef y As Integer) As Boolean
            Dim flag As Boolean
            If (Me.BGrid(x, y) = Me.Wall) Then
                flag = True
            End If
            Return flag
        End Function

        Private Function GetDoorEast() As Boolean
            Dim x As Integer
            Dim y As Integer
            Dim flag As Boolean
            If Me.IsControling Then
                x = Me.challs(CInt(Me.WControl)).x
                y = Me.challs(CInt(Me.WControl)).y
            Else
                x = Me.px
                y = Me.py
            End If
            Dim num3 As Integer = 1
            Do While (((x + num3)) <= Me.x)
                If Me.GetDoorBlock(x, y) Then
                    Return flag
                End If
                x = ((x + num3))
                If Me.GetDoor(x, y) Then
                    Return True
                End If
                Application.DoEvents()
                num3 = ((num3 + 1))
                If (num3 > 5) Then
                    Return flag
                End If
            Loop
            Return flag
        End Function

        Private Function GetDoorNorth() As Boolean
            Dim x As Integer
            Dim y As Integer
            Dim flag As Boolean
            If Me.IsControling Then
                x = Me.challs(CInt(Me.WControl)).x
                y = Me.challs(CInt(Me.WControl)).y
            Else
                x = Me.px
                y = Me.py
            End If
            Dim num3 As Integer = 1
            Do While (((y - num3)) >= 1)
                If Me.GetDoorBlock(x, y) Then
                    Return flag
                End If
                y = ((y - num3))
                If Me.GetDoor(x, y) Then
                    Return True
                End If
                Application.DoEvents()
                num3 = ((num3 + 1))
                If (num3 > 5) Then
                    Return flag
                End If
            Loop
            Return flag
        End Function

        Private Function GetDoorSouth() As Boolean
            Dim x As Integer
            Dim y As Integer
            Dim flag As Boolean
            If Me.IsControling Then
                x = Me.challs(CInt(Me.WControl)).x
                y = Me.challs(CInt(Me.WControl)).y
            Else
                x = Me.px
                y = Me.py
            End If
            Dim num3 As Integer = 1
            Do While (((y + num3)) <= Me.y)
                If Me.GetDoorBlock(x, y) Then
                    Return flag
                End If
                y = ((y + num3))
                If Me.GetDoor(x, y) Then
                    Return True
                End If
                Application.DoEvents()
                num3 = ((num3 + 1))
                If (num3 > 5) Then
                    Return flag
                End If
            Loop
            Return flag
        End Function

        Private Function GetDoorWest() As Boolean
            Dim x As Integer
            Dim y As Integer
            Dim flag As Boolean
            If Me.IsControling Then
                x = Me.challs(CInt(Me.WControl)).x
                y = Me.challs(CInt(Me.WControl)).y
            Else
                x = Me.px
                y = Me.py
            End If
            Dim num3 As Integer = 1
            Do While (((x - num3)) >= 1)
                If Me.GetDoorBlock(x, y) Then
                    Return flag
                End If
                x = ((x - num3))
                If Me.GetDoor(x, y) Then
                    Return True
                End If
                Application.DoEvents()
                num3 = ((num3 + 1))
                If (num3 > 5) Then
                    Return flag
                End If
            Loop
            Return flag
        End Function

        Public Function GetGrid(ByRef x As Integer, ByRef y As Integer) As Integer
            Return (Math.Round(Conversion.Val(Conversions.ToString(CInt(Me.Grid(x, y))))))
        End Function

        Private Function GetPassage(ByRef x As Integer, ByRef y As Integer) As Boolean
            Dim flag As Boolean
            If ((Me.BGrid(x, y) = Me.PassageMarker) Or (Me.BGrid(x, y) = Me.OpenDoor)) Then
                flag = True
            End If
            Return flag
        End Function

        Private Function GetPassageCurrent() As Boolean
            If Not Me.IsControling Then
                Return Me.GetPassage(Me.px, Me.py)
            End If
            Return Me.GetPassage(Me.challs(CInt(Me.WControl)).x, Me.challs(CInt(Me.WControl)).y)
        End Function

        Private Function GetPassageEast() As Boolean
            Dim x As Integer
            Dim y As Integer
            Dim flag As Boolean
            If Me.IsControling Then
                x = Me.challs(CInt(Me.WControl)).x
                y = Me.challs(CInt(Me.WControl)).y
            Else
                x = Me.px
                y = Me.py
            End If
            Dim num3 As Integer = 1
            Do While (((x + num3)) <= Me.x)
                Dim cX As Integer = ((x + num3))
                If Me.GetBlock(cX, y) Then
                    Return flag
                End If
                cX = ((x + num3))
                If Me.GetPassage(cX, y) Then
                    Return True
                End If
                Application.DoEvents()
                num3 = ((num3 + 1))
                If (num3 > 5) Then
                    Return flag
                End If
            Loop
            Return flag
        End Function

        Private Function GetPassageNorth() As Boolean
            Dim x As Integer
            Dim y As Integer
            Dim flag As Boolean
            If Me.IsControling Then
                x = Me.challs(CInt(Me.WControl)).x
                y = Me.challs(CInt(Me.WControl)).y
            Else
                x = Me.px
                y = Me.py
            End If
            Dim num3 As Integer = 1
            Do While (((y - num3)) >= 1)
                Dim cY As Integer = ((y - num3))
                If Me.GetBlock(x, cY) Then
                    Return flag
                End If
                cY = ((y - num3))
                If Me.GetPassage(x, cY) Then
                    Return True
                End If
                Application.DoEvents()
                num3 = ((num3 + 1))
                If (num3 > 5) Then
                    Return flag
                End If
            Loop
            Return flag
        End Function

        Private Function GetPassageSouth() As Boolean
            Dim x As Integer
            Dim y As Integer
            Dim flag As Boolean
            If Me.IsControling Then
                x = Me.challs(CInt(Me.WControl)).x
                y = Me.challs(CInt(Me.WControl)).y
            Else
                x = Me.px
                y = Me.py
            End If
            Dim num3 As Integer = 1
            Do While (((y + num3)) <= Me.y)
                Dim cY As Integer = ((y + num3))
                If Me.GetBlock(x, cY) Then
                    Return flag
                End If
                cY = ((y + num3))
                If Me.GetPassage(x, cY) Then
                    Return True
                End If
                Application.DoEvents()
                num3 = ((num3 + 1))
                If (num3 > 5) Then
                    Return flag
                End If
            Loop
            Return flag
        End Function

        Private Function GetPassageWest() As Boolean
            Dim x As Integer
            Dim y As Integer
            Dim flag As Boolean
            If Me.IsControling Then
                x = Me.challs(CInt(Me.WControl)).x
                y = Me.challs(CInt(Me.WControl)).y
            Else
                x = Me.px
                y = Me.py
            End If
            Dim num3 As Integer = 1
            Do While (((x - num3)) >= 1)
                Dim cX As Integer = ((x - num3))
                If Me.GetBlock(cX, y) Then
                    Return flag
                End If
                cX = ((x - num3))
                If Me.GetPassage(cX, y) Then
                    Return True
                End If
                Application.DoEvents()
                num3 = ((num3 + 1))
                If (num3 > 5) Then
                    Return flag
                End If
            Loop
            Return flag
        End Function

        Public Sub GExplodeMissile()
            Dim num2 As Integer
            Me.IsDoingGMissile = False
            If (Me.BGrid(Me.GX, Me.GY) = Me.Water) Then
                num2 = 0
                DXSound.smethod_1(Me.JumpInWaterSound, True, False, Me.GX, Me.GY, num2)
                Me.Grid(Me.GX, Me.GY) = Me.Water
            Else
                Dim num3 As Integer
                num2 = 0
                DXSound.smethod_1(Me.GExplodeSound, True, False, Me.GX, Me.GY, num2)
                If (((Me.Grid(Me.GX, Me.GY) = Me.Wall) Or (Me.Grid(Me.GX, Me.GY) = Me.ClosedDoor)) Or (Me.Grid(Me.GX, Me.GY) = Me.LockedDoor)) Then
                    num2 = 0
                    DXSound.smethod_1(Me.DestroyWallSound, True, False, Me.GX, Me.GY, num2)
                    Me.Grid(Me.GX, Me.GY) = Me.PassageMarker
                    Me.BGrid(Me.GX, Me.GY) = Me.PassageMarker
                End If
                If (Me.BGrid(Me.GX, Me.GY) <> Me.Machine) Then
                    Dim num As Integer = 0
                    Do
                        If (((Me.GY + num)) <= Me.y) Then
                            num2 = ((Me.GY + num))
                            If Me.IsChall(Me.GX, num2) Then
                                num3 = 50
                                Me.HarmChall(num3, Me.GX, ((Me.GY + num)))
                            End If
                            num3 = ((Me.GY + num))
                            If Me.GetBlock(Me.GX, num3) Then
                                num2 = ((Me.GY + num))
                                Me.AllThingReplace(Me.GX, num2, Me.PassageMarker)
                                num3 = ((Me.GY + num))
                                num2 = 0
                                DXSound.smethod_1(Me.DestroyWallSound, False, False, Me.GX, num3, num2)
                            End If
                        End If
                        If (((Me.GY - num)) >= 1) Then
                            num3 = ((Me.GY - num))
                            If Me.IsChall(Me.GX, num3) Then
                                num2 = 50
                                Me.HarmChall(num2, Me.GX, ((Me.GY - num)))
                            End If
                            num3 = ((Me.GY - num))
                            If Me.GetBlock(Me.GX, num3) Then
                                num2 = ((Me.GY - num))
                                Me.AllThingReplace(Me.GX, num2, Me.PassageMarker)
                                num3 = ((Me.GY - num))
                                num2 = 0
                                DXSound.smethod_1(Me.DestroyWallSound, False, False, Me.GX, num3, num2)
                            End If
                        End If
                        If (((Me.GX + num)) <= Me.x) Then
                            num3 = ((Me.GX + num))
                            If Me.IsChall(num3, Me.GY) Then
                                num2 = 50
                                Me.HarmChall(num2, ((Me.GX + num)), Me.GY)
                            End If
                            num3 = ((Me.GX + num))
                            If Me.GetBlock(num3, Me.GY) Then
                                num2 = ((Me.GX + num))
                                Me.AllThingReplace(num2, Me.GY, Me.PassageMarker)
                                num3 = ((Me.GX + num))
                                num2 = 0
                                DXSound.smethod_1(Me.DestroyWallSound, False, False, num3, Me.GY, num2)
                            End If
                        End If
                        If (((Me.GX - num)) >= 1) Then
                            num3 = ((Me.GX - num))
                            If Me.IsChall(num3, Me.GY) Then
                                num2 = 50
                                Me.HarmChall(num2, ((Me.GX - num)), Me.GY)
                            End If
                            num3 = ((Me.GX - num))
                            If Me.GetBlock(num3, Me.GY) Then
                                num2 = ((Me.GX - num))
                                Me.AllThingReplace(num2, Me.GY, Me.PassageMarker)
                                num3 = ((Me.GX - num))
                                num2 = 0
                                DXSound.smethod_1(Me.DestroyWallSound, False, False, num3, Me.GY, num2)
                            End If
                        End If
                        Application.DoEvents()
                        num = ((num + 1))
                    Loop While (num <= 10)
                End If
                If ((Me.Grid(Me.GX, Me.GY) = Me.Machine) Or (Me.BGrid(Me.GX, Me.GY) = Me.Machine)) Then
                    If Not Me.HasTurnedOffMachine Then
                        Return
                    End If
                    THConstVars.CannotDoKeydown = True
                    Me.MachineExplodeSound = DXSound.LoadSound((DXSound.SoundPath & "\m3.wav"))
                    Dim bCloseFirst As Boolean = True
                    Dim bLoopSound As Boolean = False
                    Dim performEffects As Boolean = False
                    num3 = 0
                    num2 = 0
                    Dim dVolume As String = ""
                    Dim waitTillDone As Boolean = False
                    DXSound.PlaySound(Me.MachineExplodeSound, bCloseFirst, bLoopSound, performEffects, num3, num2, dVolume, waitTillDone)
                    Me.HasBlownUpMachine = True
                    waitTillDone = True
                    performEffects = False
                    bLoopSound = False
                    num3 = 0
                    num2 = 0
                    dVolume = ""
                    bCloseFirst = False
                    DXSound.PlaySound(Me.TeleportSound, waitTillDone, performEffects, bLoopSound, num3, num2, dVolume, bCloseFirst)
                    Me.directSoundSecondaryBuffer8_5 = DXSound.LoadSound((DXSound.SoundPath & "\mm1.wav"))
                    Me.px = 10
                    Me.py = 110
                    waitTillDone = True
                    performEffects = False
                    bLoopSound = False
                    num3 = 0
                    num2 = 0
                    dVolume = ""
                    bCloseFirst = False
                    DXSound.PlaySound(Me.directSoundSecondaryBuffer8_5, waitTillDone, performEffects, bLoopSound, num3, num2, dVolume, bCloseFirst)
                    Me.directSoundSecondaryBuffer8_6 = DXSound.LoadSound((DXSound.SoundPath & "\mm6.wav"))
                    Me.directSoundSecondaryBuffer8_1 = DXSound.LoadSound((DXSound.SoundPath & "\chall10.wav"))
                    Me.BigBlastSound = DXSound.LoadSound((DXSound.SoundPath & "\bigblast.wav"))
                    Me.BombAlarmSound = DXSound.LoadSound((DXSound.SoundPath & "\bombalarm.wav"))
                    Do While (Me.directSoundSecondaryBuffer8_5.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                        Application.DoEvents()
                    Loop
                    waitTillDone = True
                    performEffects = False
                    bLoopSound = False
                    num3 = 0
                    num2 = 0
                    dVolume = ""
                    bCloseFirst = False
                    DXSound.PlaySound(Me.directSoundSecondaryBuffer8_1, waitTillDone, performEffects, bLoopSound, num3, num2, dVolume, bCloseFirst)
                    Me.challs(1).TurnIntoMaster()

                    Do While (Me.directSoundSecondaryBuffer8_1.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                        Application.DoEvents()
                    Loop
                    Me.IsEscapingFromBomb = True
                    num3 = 0
                    Me.AllThingReplace(Me.GX, Me.GY, num3)
                    Me.IsFightingLast = True
                    Do While (Me.directSoundSecondaryBuffer8_5.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                        Application.DoEvents()
                    Loop
                    Me.StartMusic()
                    dVolume = "r10.wav"
                    waitTillDone = True
                    DXSound.Radio(dVolume, waitTillDone)
                    THConstVars.CannotDoKeydown = False
                End If
                If (((((Me.Grid(Me.GX, Me.GY) <> Me.RMissile) And (Me.Grid(Me.GX, Me.GY) <> Me.PassageMarker)) And (Me.Grid(Me.GX, Me.GY) <> Me.ClosedDoor)) And (Me.Grid(Me.GX, Me.GY) <> Me.OpenDoor)) And (Me.Grid(Me.GX, Me.GY) <> Me.Wall)) Then
                    num3 = 0
                    Me.AllThingReplace(Me.GX, Me.GY, num3)
                End If
            End If
        End Sub

        Public Sub GoFight(ByRef Optional i As Integer = 0)
            If ((Not i = 5) AndAlso ((Me.HowManyNum < Me.HowMany) AndAlso (Me.challs(i).A > 0))) Then
                Dim flag As Boolean
                Me.HowManyNum = ((Me.HowManyNum + 1))
                Dim thing As Integer = (Math.Round(Conversion.Val(Conversions.ToString(CInt(Me.BGrid(Me.challs(i).x, Me.challs(i).y))))))
                Me.ThingReplace(Me.challs(i).x, Me.challs(i).y, thing)
                Select Case (1.0! + Conversion.Int(CSng((4.0! * VBMath.Rnd))))
                    Case 1.0!
                        thing = ((Me.px - 1))
                        If Not Me.GetChallBlock(thing, Me.py) Then
                            Me.challs(i).x = ((Me.px - 1))
                            Me.challs(i).y = Me.py
                            flag = True
                        End If
                        Exit Select
                    Case 2.0!
                        thing = ((Me.px + 1))
                        If Not Me.GetChallBlock(thing, Me.py) Then
                            Me.challs(i).x = ((Me.px + 1))
                            Me.challs(i).y = Me.py
                            flag = True
                        End If
                        Exit Select
                    Case 3.0!
                        thing = ((Me.py - 1))
                        If Not Me.GetChallBlock(Me.px, thing) Then
                            Me.challs(i).x = Me.px
                            Me.challs(i).y = ((Me.py - 1))
                            flag = True
                        End If
                        Exit Select
                    Case 4.0!
                        thing = ((Me.py + 1))
                        If Not Me.GetChallBlock(Me.px, thing) Then
                            Me.challs(i).x = Me.px
                            Me.challs(i).y = ((Me.py + 1))
                            flag = True
                        End If
                        Exit Select
                End Select
                If Not flag Then
                    Me.challs(i).x = Me.px
                    Me.challs(i).y = Me.py
                End If
                Me.challs(i).Activate()
            End If
        End Sub

        Public Sub HarmChall(ByRef amount As Integer, ByVal Optional x As Integer = 0, ByVal Optional y As Integer = 0)
            If ((x = 0) And (y = 0)) Then
                x = Me.px
                y = Me.py
            End If
            Dim challAmount As Integer = Me.ChallAmount
            Dim i As Integer = 1
            Do While (i <= challAmount)
                If (((Me.challs(i).x = x) And (Me.challs(i).y = y)) And (i <> Me.WControl)) Then
                    Me.challs(i).ChallHit(amount)
                    If (Me.challs(i).A > 0) Then
                        Exit Do
                    End If
                End If
                i = ((i + 1))
            Loop
        End Sub

        Private Sub InitGame()
            Dim num As Integer
            Dim num2 As Integer
            Dim bCloseFirst As Boolean = True
            Dim bLoopSound As Boolean = False
            Dim performEffects As Boolean = False
            Dim x As Integer = 0
            Dim y As Integer = 0
            Dim dVolume As String = ""
            Dim waitTillDone As Boolean = False
            DXSound.PlaySound(Me.directSoundSecondaryBuffer8_0, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
            Me.WPos = 0
            Me.ChallNum = 1
            VBMath.Randomize()
            Me.h = 500
            Me.Swrd = 1
            Me.Bullets = 100
            Me.Accuracy = 0
            'Me.Grid = New Short((Me.x + 1) - 1, (Me.y + 1) - 1) {}
            'Me.BGrid = New Short((Me.x + 1) - 1, (Me.y + 1) - 1) {}
            'Me.CGrid = New Single((Me.x + 1) - 1, (Me.y + 1) - 1) {}
            Dim num5 As Integer = Me.x
            num = 1
            Do While (num <= num5)
                Dim num6 As Integer = (Math.Round(Conversion.Int(CDbl((CDbl(Me.y) / 4)))))
                num2 = 1
                Do While (num2 <= num6)
                    System.Diagnostics.Debug.WriteLine("inputting " & num & " " & num2)
                    Me.Grid(num, num2) = Me.Mine
                    Application.DoEvents()
                    num2 = ((num2 + 1))
                Loop
                num = ((num + 1))
            Loop
            Dim num7 As Integer = Me.x
            num = 1
            Do While (num <= num7)
                Dim num8 As Integer = (Math.Round(Conversion.Int(CDbl((CDbl(Me.y) / 10)))))
                num2 = 1
                Do While (num2 <= num8)
                    System.Diagnostics.Debug.WriteLine(num & "," & num2)
                    Me.Grid(num, num2) = 0
                    Application.DoEvents()
                    num2 = ((num2 + 1))
                Loop
                num = ((num + 1))
            Loop
            Me.Grid(5, 70) = Me.MineGuard
            Me.Grid(5, &H47) = Me.MineGuard
            Me.MachineX = 2
            Me.MachineY = 2
            Me.Grid(Me.MachineX, Me.MachineY) = Me.Machine
            Me.ExitX = Me.MachineX
            Me.ExitY = Me.MachineY
            Me.MineX = 140
            Me.MineY = 70
            Me.Grid(Me.MineX, Me.MineY) = Me.MineControl
            Me.BuildWalls()
            System.Diagnostics.Debug.WriteLine("Done building walls")
            Me.WinX = 200
            Me.WinY = 70
            Me.Grid(Me.WinX, Me.WinY) = Me.Treasure
            Me.KeyX = (Math.Round(Conversion.Int(CDbl((CDbl(Me.x) / 3)))))
            Me.KeyY = (Math.Round(Conversion.Int(CDbl(((CDbl(Me.y) / 10) / 2)))))
            Me.Grid(Me.KeyX, Me.KeyY) = Me.Key
            Me.PanelX = ((Me.x - 1))
            Me.PanelY = ((Me.y - 1))
            Me.Grid(Me.PanelX, Me.PanelY) = Me.ControlPanel
            Dim num9 As Integer = Me.x
            num = 1
            Do While (num <= num9)
                Dim num10 As Integer = Me.y
                num2 = 1
                Do While (num2 <= num10)
                    Me.BGrid(num, num2) = (Math.Round(Conversion.Val(Conversions.ToString(CInt(Me.Grid(num, num2))))))
                    Application.DoEvents()
                    num2 = ((num2 + 1))
                Loop
                num = ((num + 1))
            Loop
            Me.px = 3
            Me.py = &H67
            Me.SetChalls()
            Me.InitVars()

            Do While (Me.directSoundSecondaryBuffer8_0.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                Application.DoEvents()
            Loop
            Me.StartMusic()
        End Sub

        <DebuggerStepThrough>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
            Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
            Me.SuspendLayout()
            '
            'ProgressBar1
            '
            Me.ProgressBar1.Location = New System.Drawing.Point(172, 121)
            Me.ProgressBar1.Name = "ProgressBar1"
            Me.ProgressBar1.Size = New System.Drawing.Size(611, 41)
            Me.ProgressBar1.TabIndex = 0
            Me.ProgressBar1.Visible = False
            '
            'Timer1
            '
            Me.Timer1.Enabled = False
            Me.Timer1.Interval = frameTime
            '
            'mainFRM
            '
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.ClientSize = New System.Drawing.Size(924, 312)
            Me.Controls.Add(Me.ProgressBar1)
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Me.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Location = New System.Drawing.Point(11, 30)
            Me.Name = "mainFRM"
            Me.RightToLeft = System.Windows.Forms.RightToLeft.No
            Me.Text = "The Hunt"
            Me.ResumeLayout(False)

        End Sub

        Private Sub InitVars()
            Dim index As Integer = 0
            Do
                Me.Weapons(index) = ""
                index = ((index + 1))
            Loop While (index <= 6)
            Dim wName As String = "gun"
            Dim pos As Integer = 0
            Me.SetWeapon(wName, pos)
            wName = "sword"
            pos = 0
            Me.SetWeapon(wName, pos)
            wName = "bombs"
            pos = 0
            Me.SetWeapon(wName, pos)
            Me.UnlimitedHealth = False
            Me.UnlimitedBullets = False
            Me.ch = 0
            Me.HasKey = False
            Me.WorkedPanel = False
            Me.Points = 0
            Me.bombs = 3
            Me.IsFightingLast = False
            Me.HasKilledBrutus = False
            Me.BlewUpMineControl = False
            Me.ALaser = 0
            Me.short_2 = 0
            Me.short_3 = 0
            Me.IsDoingGMissile = False
            Me.IsInWater = False
            Me.WDepth = 0
            Me.SubPX = 0
            Me.SubPY = 0
            Me.DoingReflector = False
            Me.IsWaitingForMachine = False
            Me.CMachine = 180
            Me.HasShutDownCard = False
            Me.NumAlert = 0
            Me.HasTurnedOffMachine = False
            Me.HasBlownUpMachine = False
            Me.EBomb = 90
            Me.IsEscapingFromBomb = False
            Me.HasMainKey = False
            Me.HasKilledMouse = False
            Me.HasKilledBrutus2 = False
            Me.SGen = 0
            Me.IsControling = False
            Me.AControl = 0
            Me.WControl = 0
        End Sub

        Public Function IsChall(ByRef x As Integer, ByRef y As Integer) As Boolean
            Dim flag As Boolean
            Dim num As Integer = Me.Grid(x, y)
            If ((((num = Me.challenger) Or (num = Me.Mouse)) Or (num = Me.JamesBrutus)) Or (num = Me.ChallWithKey)) Then
                flag = True
            End If
            Return flag
        End Function

        Private Sub LoadAllSounds()
            Me.CurrentFreq = Conversions.ToInteger(Interaction.GetSetting(Addendums.AppTitle, "Config", "SRate", Conversions.ToString(&HAC44)))
            Me.CFootstepSound = DXSound.smethod_0((DXSound.SoundPath & "\challstep.wav"), 2.0!, 30)
            Me.NeedleLaunchSound = DXSound.LoadSound((DXSound.SoundPath & "\needlelaunch.wav"))
            Me.NeedleHitSound = DXSound.LoadSound((DXSound.SoundPath & "\needlehit.wav"))
            Me.CaughtSound = DXSound.smethod_0((DXSound.SoundPath & "\caught.wav"), 2.0!, 30)
            Me.LockDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\doorlock.wav"), 2.0!, 30)
            Me.UnlockDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\doorunlock.wav"), 2.0!, 30)
            Me.ShieldSound = DXSound.LoadSound((DXSound.SoundPath & "\shield.wav"))
            Me.EmptySound = DXSound.LoadSound((DXSound.SoundPath & "\empty.wav"))
            Me.TargetSound = DXSound.LoadSound((DXSound.SoundPath & "\target.wav"))
            Me.RadioSound = DXSound.LoadSound((DXSound.SoundPath & "\r2.wav"))
            Dim index As Integer = 1
            Do
                Me.PassageSound(index) = DXSound.smethod_0((DXSound.SoundPath & "\passageopening.wav"), 2.0!, 30)
                Me.DoorSound(index) = DXSound.smethod_0((DXSound.SoundPath & "\door.wav"), 2.0!, 30)
                Application.DoEvents()
                index = ((index + 1))
            Loop While (index <= 4)
            Me.ControlLaunchSound = DXSound.LoadSound((DXSound.SoundPath & "\wcontrol.wav"))
            Me.ControlHitSound = DXSound.LoadSound((DXSound.SoundPath & "\wcontrolhit.wav"))
            Me.OpenDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\opendoor.wav"), 3.0!, 30)
            Me.CloseDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\closedoor.wav"), 3.0!, 30)
            Me.directSoundSecondaryBuffer8_2 = DXSound.LoadSound((DXSound.SoundPath & "\fbeep.wav"))
            Me.MachineSound = DXSound.LoadSound((DXSound.SoundPath & "\m1.wav"))
            Me.MachineTurnOffSound = DXSound.LoadSound((DXSound.SoundPath & "\m2.wav"))
            Me.BombBeepSound = DXSound.LoadSound((DXSound.SoundPath & "\lastbeep.wav"))
            Me.directSoundSecondaryBuffer8_4 = DXSound.LoadSound((DXSound.SoundPath & "\rbeep.wav"))
            Me.ReflectorSound = DXSound.LoadSound((DXSound.SoundPath & "\reflector.wav"))
            Me.TurnKnobSound = DXSound.LoadSound((DXSound.SoundPath & "\knob.wav"))
            Me.GetSound = DXSound.LoadSound((DXSound.SoundPath & "\obtain.wav"))
            Me.NNumber = DXSound.LoadSound((DXSound.NumPath & "\0.wav"))
            Me.NLoadSuccessfulSound = DXSound.LoadSound((DXSound.string_0 & "\nloadsuccessful.wav"))
            Me.BackgroundSound = DXSound.LoadSound((DXSound.SoundPath & "\background.wav"))
            Me.BackgroundSound.SetVolume(Conversions.ToInteger(Interaction.GetSetting(Addendums.AppTitle, "Config", "Vol", Conversions.ToString(-1600))))
            Me.DisChestSound = DXSound.LoadSound((DXSound.SoundPath & "\chall6.wav"))
            Me.AccessDeniedSound = DXSound.LoadSound((DXSound.SoundPath & "\denied.wav"))
            Me.DestroyWallSound = DXSound.smethod_0((DXSound.SoundPath & "\destroywall.wav"), 2.0!, 30)
            Me.BeepSound = DXSound.LoadSound((DXSound.SoundPath & "\beep.wav"))
            Me.VaultUpSound = DXSound.LoadSound((DXSound.SoundPath & "\openvault.wav"))
            Me.BackToVaultSound = DXSound.LoadSound((DXSound.SoundPath & "\back2vault.wav"))
            Me.BreathSound = DXSound.LoadSound((DXSound.SoundPath & "\breath.wav"))
            Me.SwimSound = DXSound.LoadSound((DXSound.SoundPath & "\swim.wav"))
            Me.WaterSound = DXSound.LoadSound((DXSound.SoundPath & "\water.wav"))
            Me.JumpInWaterSound = DXSound.smethod_0((DXSound.SoundPath & "\jumpinwater.wav"), 2.0!, 30)
            Me.WorkedPanelSound = DXSound.LoadSound((DXSound.SoundPath & "\panel.wav"))
            Me.WallCrashSound = DXSound.LoadSound((DXSound.SoundPath & "\wallcrash.wav"))
            Me.PickUpHealthSound = DXSound.LoadSound((DXSound.SoundPath & "\energy.wav"))
            Me.AlarmSound = DXSound.LoadSound((DXSound.SoundPath & "\alarm.wav"))
            Me.TeleportSound = DXSound.LoadSound((DXSound.SoundPath & "\teleport.wav"))
            Me.CharHitSound = DXSound.smethod_0((DXSound.SoundPath & "\chargrunt.wav"), 2.0!, 30)
            Me.CharDieSound = DXSound.LoadSound((DXSound.SoundPath & "\chardie.wav"))
            Me.SwingSwordSound = DXSound.LoadSound((DXSound.SoundPath & "\swingsword.wav"))
            Me.GunSound = DXSound.LoadSound((DXSound.SoundPath & "\gun.wav"))
            Me.DuelSound = DXSound.LoadSound((DXSound.SoundPath & "\battle.wav"))
            Me.DuelSound.SetVolume(Conversions.ToInteger(Interaction.GetSetting(Addendums.AppTitle, "Config", "Vol", Conversions.ToString(-1600))))
            Me.Footstep1Sound = DXSound.LoadSound((DXSound.SoundPath & "\footstep.wav"))
            Me.Footstep2Sound = DXSound.LoadSound((DXSound.SoundPath & "\footstep2.wav"))
            Me.PickUpSWeaponSound = DXSound.LoadSound((DXSound.SoundPath & "\pweapon1.wav"))
            Me.LaserGunSound = DXSound.LoadSound((DXSound.SoundPath & "\sweapon2.wav"))
            Me.GLaunchSound = DXSound.LoadSound((DXSound.SoundPath & "\sweapon1.wav"))
            Me.GExplodeSound = DXSound.smethod_0((DXSound.SoundPath & "\explodemissile.wav"), 2.0!, 30)
            Me.directSoundSecondaryBuffer8_3 = DXSound.smethod_0((DXSound.SoundPath & "\gmove.wav"), 2.0!, 30)
        End Sub

        Private Sub LockUnlockDoor()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Dim num As Integer
            If ((Me.px - 1) > 0) Then
                num = ((Me.px - 1))
                Me.LOUD(num, Me.py)
            End If
            If ((Me.px + 1) < Me.x) Then
                num = ((Me.px + 1))
                Me.LOUD(num, Me.py)
            End If
            If ((Me.py - 1) > 0) Then
                num = ((Me.py - 1))
                Me.LOUD(Me.px, num)
            End If
            If ((Me.py + 1) < Me.y) Then
                num = ((Me.py + 1))
                Me.LOUD(Me.px, num)
            End If
        End Sub

        Private Sub LookInDir(a As WalkDirection)
            If keyDownDisabled() Then
                Exit Sub
            End If
            Dim num As Integer
            Dim num2 As Integer
            Dim flag As Boolean
            Dim gX As Integer
            Dim gY As Integer
            Dim num5 As Integer
            Dim flag2 As Boolean
            Dim num6 As Long
            If Me.IsDoingGMissile Then
                gX = Me.GX
                gY = Me.GY
            ElseIf Me.IsControling Then
                gX = Me.challs(CInt(Me.WControl)).x
                gY = Me.challs(CInt(Me.WControl)).y
            Else
                gX = Me.px
                gY = Me.py
            End If
            Me.NStop = False
            If a = WalkDirection.north Then
                Do While (num5 < 20)
                    num5 = ((num5 + 1))
                    If (((gY + num5)) > Me.y) Then
                        Me.NStop = True
                        Return
                    End If
                    num = Me.Grid(gX, ((gY + num5)))
                    num2 = Me.BGrid(gX, ((gY + num5)))
                    If ((num > 0) And (num <> Me.PassageMarker)) Then
                        flag2 = True
                        Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {num}, Nothing))), flag2)
                        If Not Me.NStop Then
                            If (((num2 <> 0) And (num2 <> num)) And (num2 <> Me.PassageMarker)) Then
                                flag2 = True
                                Me.NLS((DXSound.string_0 & "\and.wav"), flag2)
                                If Me.NStop Then
                                    Return
                                End If
                                flag2 = True
                                Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {num2}, Nothing))), flag2)
                                If Me.NStop Then
                                    Return
                                End If
                            End If
                            num6 = num5
                            Me.VoiceNumber(num6)
                            num5 = (num6)
                            If Not Me.NStop Then
                                Me.NumWait()

                                If Not Me.NStop Then
                                    flag2 = False
                                    Me.NLS((DXSound.string_0 & "\ntfeet.wav"), flag2)
                                    Me.NStop = True
                                End If
                            End If
                        End If
                        Return
                    End If
                    Application.DoEvents()
                Loop
                flag2 = False
                Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {0}, Nothing))), flag2)
                flag = True
            End If
            If a = WalkDirection.south Then
                Do While (num5 < 20)
                    num5 = ((num5 + 1))
                    If (((gY - num5)) < 1) Then
                        Me.NStop = True
                        Return
                    End If
                    num = Me.Grid(gX, ((gY - num5)))
                    num2 = Me.BGrid(gX, ((gY - num5)))
                    If ((num > 0) And (num <> Me.PassageMarker)) Then
                        flag2 = True
                        Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {num}, Nothing))), flag2)
                        If Not Me.NStop Then
                            If (((num2 <> 0) And (num2 <> num)) And (num2 <> Me.PassageMarker)) Then
                                flag2 = True
                                Me.NLS((DXSound.string_0 & "\and.wav"), flag2)
                                If Me.NStop Then
                                    Return
                                End If
                                flag2 = True
                                Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {num2}, Nothing))), flag2)
                                If Me.NStop Then
                                    Return
                                End If
                            End If
                            num6 = num5
                            Me.VoiceNumber(num6)
                            num5 = (num6)
                            If Not Me.NStop Then
                                Me.NumWait()

                                If Not Me.NStop Then
                                    flag2 = False
                                    Me.NLS((DXSound.string_0 & "\ntfeet.wav"), flag2)
                                    Me.NStop = True
                                End If
                            End If
                        End If
                        Return
                    End If
                    Application.DoEvents()
                Loop
                flag2 = False
                Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {0}, Nothing))), flag2)
                flag = True
            End If
            If a = WalkDirection.west Then
                Do While (num5 < 20)
                    num5 = ((num5 + 1))
                    If (((gX - num5)) < 1) Then
                        Me.NStop = True
                        Return
                    End If
                    num = Me.Grid(((gX - num5)), gY)
                    num2 = Me.BGrid(((gX - num5)), gY)
                    If ((num > 0) And (num <> Me.PassageMarker)) Then
                        flag2 = True
                        Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {num}, Nothing))), flag2)
                        If Not Me.NStop Then
                            If (((num2 <> 0) And (num2 <> num)) And (num2 <> Me.PassageMarker)) Then
                                flag2 = True
                                Me.NLS((DXSound.string_0 & "\and.wav"), flag2)
                                If Me.NStop Then
                                    Return
                                End If
                                flag2 = True
                                Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {num2}, Nothing))), flag2)
                                If Me.NStop Then
                                    Return
                                End If
                            End If
                            num6 = num5
                            Me.VoiceNumber(num6)
                            num5 = (num6)
                            If Not Me.NStop Then
                                Me.NumWait()

                                If Not Me.NStop Then
                                    flag2 = False
                                    Me.NLS((DXSound.string_0 & "\ntfeet.wav"), flag2)
                                    Me.NStop = True
                                End If
                            End If
                        End If
                        Return
                    End If
                    Application.DoEvents()
                Loop
                flag2 = False
                Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {0}, Nothing))), flag2)
                flag = True
            End If
            If a = WalkDirection.east Then
                Do While (num5 < 20)
                    num5 = ((num5 + 1))
                    If (((gX + num5)) > Me.x) Then
                        Me.NStop = True
                        Return
                    End If
                    num = Me.Grid(((gX + num5)), gY)
                    num2 = Me.BGrid(((gX + num5)), gY)
                    If ((num > 0) And (num <> Me.PassageMarker)) Then
                        flag2 = True
                        Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {num}, Nothing))), flag2)
                        If Not Me.NStop Then
                            If (((num2 <> 0) And (num2 <> num)) And (num2 <> Me.PassageMarker)) Then
                                flag2 = True
                                Me.NLS((DXSound.string_0 & "\and.wav"), flag2)
                                If Me.NStop Then
                                    Return
                                End If
                                flag2 = True
                                Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {num2}, Nothing))), flag2)
                                If Me.NStop Then
                                    Return
                                End If
                            End If
                            num6 = num5
                            Me.VoiceNumber(num6)
                            num5 = (num6)
                            If Not Me.NStop Then
                                Me.NumWait()

                                If Not Me.NStop Then
                                    flag2 = False
                                    Me.NLS((DXSound.string_0 & "\ntfeet.wav"), flag2)
                                    Me.NStop = True
                                End If
                            End If
                        End If
                        Return
                    End If
                    Application.DoEvents()
                Loop
                flag2 = False
                Me.NLS(Conversions.ToString(Operators.ConcatenateObject((DXSound.string_0 & "\"), NewLateBinding.LateIndexGet(Me.stuff, New Object() {0}, Nothing))), flag2)
                flag = True
            End If
            If flag Then
                Me.NStop = True
            End If
        End Sub

        Private Sub LOUD(ByRef x As Integer, ByRef y As Integer)
            If Me.HasMainKey Then
                Dim num As Integer
                If (Me.BGrid(x, y) = Me.ClosedDoor) Then
                    num = 0
                    DXSound.smethod_1(Me.LockDoorSound, True, False, x, y, num)
                    Me.AllThingReplace(x, y, Me.LockedDoor)
                ElseIf (Me.BGrid(x, y) = Me.LockedDoor) Then
                    num = 0
                    DXSound.smethod_1(Me.UnlockDoorSound, True, False, x, y, num)
                    Me.AllThingReplace(x, y, Me.ClosedDoor)
                End If
            End If
        End Sub

        <STAThread>
        Public Shared Sub Main()
            Application.Run(New mainFRM)
        End Sub

        Private Sub mainFRM_Closed(ByVal sender As Object, ByVal e As EventArgs)
            Me.ShutDown()
        End Sub

        Private Sub performActions()
            Dim Points As Long
            Dim num7 As Integer = 2
            If (Me.HasDoneInit) Then
                If (DXInput.IsShift()) Then
                    If DXInput.isFirstPress(SharpDX.DirectInput.Key.Up) Then
                        LookInDir(WalkDirection.north)
                        Exit Sub
                    ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.Down) Then
                        LookInDir(WalkDirection.south)
                        Exit Sub
                    ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.Right) Then
                        LookInDir(WalkDirection.east)
                        Exit Sub
                    ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.Left) Then
                        LookInDir(WalkDirection.west)
                        Exit Sub
                    ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.Return) Then
                        LockUnlockDoor()
                        Exit Sub
                    ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.PageUp) Then
                        changeMusicVolume(VolumeAction.mute)
                        Exit Sub
                    ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.PageDown) Then
                        changeMusicVolume(VolumeAction.unmute)
                        Exit Sub
                    End If
                    If DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Space) Then
                        swim(SwimDirection.south)
                        pressedActionButtonInLastTick = True
                        Exit Sub
                    Else
                        pressedActionButtonInLastTick = False
                    End If
                    Exit Sub
                Else ' If not shift. These keys can be pressed with either the shift key held or released, but the shift action will take priority.
                    If DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Space) Then
                        swim(SwimDirection.north)
                        Me.UseWeapon()
                        pressedActionButtonInLastTick = True
                        Exit Sub
                    Else
                        pressedActionButtonInLastTick = False
                    End If
                    If DXInput.isFirstPress(SharpDX.DirectInput.Key.Return) Then
                        OpenOrCloseDoor()
                        Exit Sub
                    End If
                    If DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.PageUp) Then
                        changeMusicVolume(VolumeAction.down)
                        Exit Sub
                    ElseIf DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.PageDown) Then
                        changeMusicVolume(VolumeAction.up)
                        Exit Sub
                    End If
                End If
                If DXInput.IsControl() Then
                    If DXInput.isFirstPress(SharpDX.DirectInput.Key.S) Then
                        Me.file_save_click()
                        Exit Sub
                    ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.L) Then
                        System.Diagnostics.Debug.WriteLine("load: Before loadclick")
                        Me.file_load_click()
                        System.Diagnostics.Debug.WriteLine("load: after loadclick")
                        Exit Sub
                    ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.Left) Then
                        Me.ChangeSpeechRate(SpeechRate.slower)
                        Exit Sub
                    ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.Right) Then
                        Me.ChangeSpeechRate(SpeechRate.quicker)
                        Exit Sub
                    End If
                    If Not Me.NStop Then
                        Me.NStop = True
                        Me.NNumber.Stop()
                    End If
                    Exit Sub
                Else ' if not ctrl, for keys that can be pressed with both ctrl and without
                    If DXInput.isFirstPress(SharpDX.DirectInput.Key.S) Then
                        Me.Stats()
                        Exit Sub
                    End If
                End If ' if ctrl
                ' Here, the keys will register no matter which modifiers are held because there is no modifier conflict
                If DXInput.isFirstPress(SharpDX.DirectInput.Key.Escape) Then
                    Me.MainMenu(True)
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.F) Then
                    pauseOrUnpauseGame()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.D1) Then
                    Me.SwitchToWeapon(0)
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.D2) Then
                    Me.SwitchToWeapon(1)
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.D3) Then
                    Me.SwitchToWeapon(2)
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.D4) Then
                    Me.SwitchToWeapon(3)
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.D5) Then
                    Me.SwitchToWeapon(4)
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.D6) Then
                    Me.SwitchToWeapon(5)
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.D7) Then
                    Me.SwitchToWeapon(6)
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.Z) Then
                    If keyDownDisabled() Then
                        Exit Sub
                    End If
                    Me.VoiceNumber(CLng(Math.Round(CDbl(THConstVars.Difficulty))))
                    Me.NumWait()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.L) Then
                    SayLocation()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.I) Then
                    Me.SayInventory()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.V) Then
                    Me.SayHasVisited()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.A) Then
                    Me.ReportAmunition(False)
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.B) Then
                    Me.BuyAmunition()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.T) Then
                    Me.position()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.P) Then
                    If keyDownDisabled() Then
                        Exit Sub
                    End If
                    If (Me.BGrid(Me.px, Me.py) = Me.ControlPanel) Then
                        Me.ControlPanelWork(True)
                    Else
                        Me.NStop = False
                        Me.NNumber.Stop()
                        Me.VoiceNumber(Me.Points)
                        Me.NStop = True
                    End If
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.H) Then
                    sayHealth()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.C) Then
                    sayNumAlert()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.D) Then
                    sayDepth()
                    Exit Sub
                ElseIf DXInput.isFirstPress(SharpDX.DirectInput.Key.R) Then
                    Me.ReturnToStartingPoint()
                    Exit Sub
                End If
                If DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Left) Then
                    swim(SwimDirection.west)
                    missileCommand(MissileAction.turnLeft)
                    MovePlayer(WalkDirection.west)
                    movedInLastTick = True
                ElseIf DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Right) Then
                    swim(SwimDirection.east)
                    missileCommand(MissileAction.turnRight)
                    MovePlayer(WalkDirection.east)
                    movedInLastTick = True
                ElseIf DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Up) Then
                    swim(SwimDirection.up)
                    missileCommand(MissileAction.speedUp)
                    MovePlayer(WalkDirection.north)
                    movedInLastTick = True
                ElseIf DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Down) Then
                    swim(SwimDirection.down)
                    missileCommand(MissileAction.slowDown)
                    MovePlayer(WalkDirection.south)
                    movedInLastTick = True
                Else
                    movedInLastTick = False
                End If
            End If ' if has done init
        End Sub

        Private Sub mainFRM_Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim num5 As Integer
            Try
                Dim num4 As Integer = 2
                Me.Show()
                Me.BringToFront()
                THF.init(Me)
                THF.F = Me
                Me.CA = 0
                FileSystem.FileOpen(1, (Addendums.FilePath & "\version.dat"), OpenMode.Input, OpenAccess.Default, OpenShare.Default, -1)
                FileSystem.Input(1, Me.AppVersion)
                FileSystem.Reset()
                Interaction.SaveSetting(Addendums.AppTitle, "Data", "Version", Me.AppVersion)
                Me.V = Conversions.ToInteger(Interaction.GetSetting(Addendums.AppTitle, "Config", "Vol", Conversions.ToString(-1600)))
                THF.F.Text = (Addendums.AppTitle & " version " & Me.AppVersion)
                Me.DefCaption = Me.Text
                Me.NStop = True
                THConstVars.CannotDoKeydown = True
                Dim winHandle As Long = Me.Handle.ToInt32
                DXSound.InitDX(winHandle)
                If (isUpdating()) Then
                    THConstVars.CannotDoKeydown = True
                    Exit Sub
                End If
                Me.NNumber = DXSound.LoadSound((DXSound.string_0 & "\reg1.wav"))
                Me.reg()
                Me.NNumber.Stop()
                Me.directSoundSecondaryBuffer8_0 = DXSound.LoadSound((DXSound.string_0 & "\nloading.wav"))
                Me.BPCLogoSound = DXSound.LoadSound((DXSound.SoundPath & "\logo.wav"))
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = False
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXInput.DInputInit(Me.Handle)

                DXSound.PlaySound(Me.BPCLogoSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                Me.IntroSound = modDirectShow.LoadMP3((DXSound.SoundPath & "\trailers\storyline.mp3"))
                Do While Me.BPCLogoSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING And Not (DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Return) Or DXInput.isKeyHeldDown(SharpDX.DirectInput.Key.Space))
                    Application.DoEvents()
                Loop
                Me.BPCLogoSound.Stop()
                Me.BPCLogoSound = Nothing
                waitTillDone = True
                performEffects = False
                bLoopSound = False
                y = 0
                x = 0
                dVolume = ""
                bCloseFirst = False
                DXSound.PlaySound(Me.directSoundSecondaryBuffer8_0, waitTillDone, performEffects, bLoopSound, y, x, dVolume, bCloseFirst)
                Do While (Me.directSoundSecondaryBuffer8_0.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                    Application.DoEvents()
                Loop
                Me.IntroSound.Run()
                Me.ClickSound = DXSound.LoadSound((DXSound.SoundPath & "\click.wav"))
                waitTillDone = True
                performEffects = True
                bLoopSound = False
                y = 0
                x = 0
                dVolume = ""
                bCloseFirst = False
                DXSound.PlaySound(Me.ClickSound, waitTillDone, performEffects, bLoopSound, y, x, dVolume, bCloseFirst)
                Me.LoadAllSounds()
                Dim prompt As String = String.Concat(New String() {"WELCOME! ", Addendums.AppTitle, " is written by Munawar Bijani; it has been ported to Windows by the work of Munawar Bijani and Ameer Armaly. Treasure Hunt version ", Me.AppVersion, ". ", Addendums.LegalCopyrights})
                Me.IStuff = 5
                Me.Treasure = ((Me.IStuff + 1))
                Me.Wall = ((Me.IStuff + 2))
                Me.Key = ((Me.IStuff + 3))
                Me.ControlPanel = ((Me.IStuff + 4))
                Me.challenger = ((Me.IStuff + 5))
                Me.RBomb = ((Me.IStuff + 6))
                Me.Water = ((Me.IStuff + 7))
                Me.Laser = ((Me.IStuff + 8))
                Me.Missile = ((Me.IStuff + 9))
                Me.RMissile = ((Me.IStuff + 10))
                Me.short_1 = ((Me.IStuff + 11))
                Me.Mine = ((Me.IStuff + 12))
                Me.MineControl = ((Me.IStuff + 13))
                Me.MineGuard = ((Me.IStuff + 14))
                Me.Reflector = ((Me.IStuff + 15))
                Me.Machine = ((Me.IStuff + &H10))
                Me.Mouse = ((Me.IStuff + &H11))
                Me.PassageMarker = ((Me.IStuff + &H12))
                Me.ClosedDoor = ((Me.IStuff + &H13))
                Me.OpenDoor = ((Me.IStuff + 20))
                Me.LockedDoor = ((Me.IStuff + &H15))
                Me.ChallWithKey = ((Me.IStuff + &H16))
                Me.JamesBrutus = ((Me.IStuff + &H17))
                Me.controler = ((Me.IStuff + &H18))
                Me.stuff = Strings.Split("nnothing.wav|nsnake.wav|nsword.wav|ncartrige.wav|nhealth.wav|nportal.wav|nvault.wav|nwall.wav|nchest.wav|npanel1.wav|nguard.wav|nbomb.wav|nwater.wav|nlaser.wav|nmissile.wav|nrmissile.wav|nglmissile.wav|nmine.wav|ncmine.wav|ngmine.wav|nreflector.wav|nmachine.wav|nmouse.wav|nnothing.wav|ncdoor.wav|nodoor.wav|ncldoor.wav|ngkey.wav|nbrutus.wav|ncontrol.wav", "|", -1, CompareMethod.Binary)
                'Me.Grid = New Short((Me.x + 1) - 1, (Me.y + 1) - 1) {}
                'Me.BGrid = New Short((Me.x + 1) - 1, (Me.y + 1) - 1) {}
                'Me.CGrid = New Single((Me.x + 1) - 1, (Me.y + 1) - 1) {}
                Me.ChallAmount = &H2710
                Me.Weapons = New String(7 - 1) {}
                Me.DetectRange = 1
                VBMath.Randomize()
                Me.ClickSound.Stop()
                waitTillDone = False
                Me.NLS((DXSound.string_0 & "\spaceenter.wav"), waitTillDone)
                waitForSpaceOrEnter()
                Me.IntroSound.Stop()
                Me.IntroSound = Nothing
                waitTillDone = False
                Me.MainMenu(waitTillDone)
                THConstVars.CannotDoKeydown = True
                Me.StopMusic()
                Me.MuteSounds()
                waitTillDone = False
                Me.NLS((DXSound.string_0 & "\intro.wav"), waitTillDone)
                Interaction.MsgBox(prompt, MsgBoxStyle.ApplicationModal, "Welcome!")
                Me.NNumber.Stop()
                If Not Me.IsFirstTimeLoading Then
                    modDirectShow.imediaControl_0 = modDirectShow.LoadMP3((DXSound.SoundPath & "\r18.mp3"))
                    modDirectShow.PlayMP3()
                    waitForSpaceOrEnter()
                    modDirectShow.StopMP3()
                    Me.IsFirstTimeLoading = True
                End If
                THConstVars.CannotDoKeydown = False
                Me.StartMusic()
                Me.UnmuteSounds()
                Me.HasDoneInit = True
            Catch err As Exception
                THConstVars.handleException(err, Timer1)
            End Try
        End Sub

        Private Sub MainMenu(ByRef Optional ResumeGame As Boolean = False)
            System.Diagnostics.Debug.WriteLine("In mainmenu")
            disableGameLoop()
            Dim str2 As String
            Dim num4 As Integer
            Dim str3 As String
            Me.MuteSounds()
            THConstVars.CannotDoKeydown = True
            Me.MenuSound = DXSound.LoadSound((DXSound.SoundPath & "\menus.wav"))
            Me.BackgroundSound.SetVolume(-10000)
            Me.DuelSound.SetVolume(-10000)
            If Me.IsFightingLast Then
                Me.FinalDuelSound.SetVolume(-10000)
            End If
            Me.MenuSound.SetVolume(0)
            Dim bCloseFirst As Boolean = True
            Dim bLoopSound As Boolean = True
            Dim performEffects As Boolean = False
            Dim x As Integer = 0
            Dim y As Integer = 0
            Dim dVolume As String = ""
            Dim waitTillDone As Boolean = False
            DXSound.PlaySound(Me.MenuSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
            If ResumeGame Then
                str2 = "menu_a1a.wav"
            Else
                str2 = "menu_a1.wav"
            End If
            str2 = (str2 & "|menu_a2.wav|menu_a6.wav|menu_a3.wav|menu_a5.wav|menu_a4.wav")
            Do While (num4 <> 1)
                Dim num5 As Integer
                Dim num6 As Integer
                Dim num7 As Integer
                Dim num8 As Integer
                dVolume = "menu.wav"
                y = 0
                Select Case Me.GenerateMenu(dVolume, str2, y)
                    Case 1
                        Exit Do
                    Case 2
                        If Not Me.IsFull Then
                            Exit Select
                        End If
                        Me.file_load_click()

                        If (Me.FileName <> "") Then
                            ResumeGame = True
                            Me.MenuSound.Stop()
                            Me.MenuSound = Nothing
                            THConstVars.CannotDoKeydown = False
                            Me.IsInMenu = False
                            Me.HasDoneInit = True
                            Exit Do
                        End If
                        Exit Select
                    Case 3
                        If Not Me.IsFull Then
                            GoTo Label_01A7
                        End If
                        Me.file_save_click()
                        GoTo Label_060A
                    Case 4
                        num5 = 0
                        GoTo Label_0505
                    Case 5
                        num7 = 0
                        GoTo Label_05FA
                    Case 6
                        Me.ExitGame()
                        GoTo Label_060A
                    Case Else
                        GoTo Label_060A
                End Select
                waitTillDone = True
                performEffects = False
                bLoopSound = False
                y = 0
                x = 0
                dVolume = ""
                bCloseFirst = False
                DXSound.PlaySound(Me.AccessDeniedSound, waitTillDone, performEffects, bLoopSound, y, x, dVolume, bCloseFirst)
                GoTo Label_060A
Label_01A7:
                waitTillDone = True
                performEffects = False
                bLoopSound = False
                y = 0
                x = 0
                dVolume = ""
                bCloseFirst = False
                DXSound.PlaySound(Me.AccessDeniedSound, waitTillDone, performEffects, bLoopSound, y, x, dVolume, bCloseFirst)
                GoTo Label_060A
Label_01E2:
                dVolume = ""
                str3 = "menu_b1.wav|menu_b2.wav|menu_b3.wav|menu_b4.wav|menu_b5.wav|menu_b6.wav|menu_b7.wav|menu_b8.wav|menu_b9.wav|menu_b10.wav|menu_b11.wav|menu_b12.wav|menu_b13.wav|menu_return.wav "
                y = ((num6 - 1))
                Select Case Me.GenerateMenu(dVolume, str3, y)
                    Case 1
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = False
                        DXSound.PlaySound(Me.PickUpHealthSound, waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 2
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = False
                        DXSound.PlaySound(Me.TargetSound, waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 3
                        y = 0
                        DXSound.smethod_1(Me.directSoundSecondaryBuffer8_3, True, False, Me.px, Me.py, y)
                        Exit Select
                    Case 4
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = False
                        DXSound.PlaySound(Me.directSoundSecondaryBuffer8_4, waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 5
                        y = 0
                        DXSound.smethod_1(Me.DoorSound(1), True, False, Me.px, Me.py, y)
                        Exit Select
                    Case 6
                        y = 0
                        DXSound.smethod_1(Me.PassageSound(1), True, False, Me.px, Me.py, y)
                        Exit Select
                    Case 7
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = True
                        DXSound.PlaySound(DXSound.LoadSound((DXSound.SoundPath & "\launch.wav")), waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 8
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = True
                        DXSound.PlaySound(DXSound.LoadSound((DXSound.SoundPath & "\challgun.wav")), waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 9
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = True
                        DXSound.PlaySound(DXSound.LoadSound((DXSound.SoundPath & "\caught.wav")), waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 10
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = True
                        DXSound.PlaySound(DXSound.LoadSound((DXSound.SoundPath & "\redalert.wav")), waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 11
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = False
                        DXSound.PlaySound(Me.GetSound, waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 12
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = False
                        DXSound.PlaySound(Me.AccessDeniedSound, waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 13
                        waitTillDone = True
                        performEffects = False
                        bLoopSound = False
                        y = 0
                        x = 0
                        str3 = ""
                        bCloseFirst = False
                        DXSound.PlaySound(Me.EmptySound, waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        Exit Select
                    Case 14
                        num5 = 1
                        Exit Select
                End Select
                Application.DoEvents()
Label_0505:
                If (num5 <> 1) Then
                    GoTo Label_01E2
                End If
                GoTo Label_060A
Label_051A:
                str3 = "menu.wav"
                dVolume = "menu_c1.wav|menu_c2.wav|menu_c3.wav|menu_c4.wav|menu_return.wav"
                y = ((num8 - 1))
                Select Case Me.GenerateMenu(str3, dVolume, y)
                    Case 1
                        Exit Select
                    Case 2
                        Exit Select
                    Case 3
                        Exit Select
                    Case 4
                        Exit Select
                    Case Else
                        Exit Select
                End Select
                waitTillDone = True
                performEffects = False
                bLoopSound = False
                y = 0
                x = 0
                str3 = ""
                bCloseFirst = False
                DXSound.PlaySound(Me.AccessDeniedSound, waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                GoTo Label_05F5
Label_05BA:
                waitTillDone = True
                performEffects = False
                bLoopSound = False
                y = 0
                x = 0
                str3 = ""
                bCloseFirst = False
                DXSound.PlaySound(Me.AccessDeniedSound, waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
Label_05F5:
                Application.DoEvents()
Label_05FA:
                If (num7 <> 1) Then
                    GoTo Label_051A
                End If
Label_060A:
                Application.DoEvents()
            Loop
            If Not ResumeGame Then
                Dim num9 As Single
                Do While (num9 <> 1.0!)
                    str3 = "menu.wav"
                    dVolume = "menu_d1.wav|menu_d2.wav|menu_d4.wav|menu_d3.wav"
                    y = 0
                    THConstVars.Difficulty = Me.GenerateMenu(str3, dVolume, y)
                    Me.UnlimitedHealth = True
                    If Not Me.IsFull Then
                        If (THConstVars.Difficulty <> 4.0!) Then
                            num9 = 1.0!
                        Else
                            waitTillDone = True
                            performEffects = False
                            bLoopSound = False
                            y = 0
                            x = 0
                            str3 = ""
                            bCloseFirst = False
                            DXSound.PlaySound(Me.AccessDeniedSound, waitTillDone, performEffects, bLoopSound, y, x, str3, bCloseFirst)
                        End If
                    Else
                        num9 = 1.0!
                    End If
                    Application.DoEvents()
                Loop
                Me.InitGame()
            End If
            THConstVars.CannotDoKeydown = False
            Me.MenuSound.Stop()
            Me.MenuSound = Nothing
            Me.UnmuteSounds()
            Me.StartMusic()
            Me.BackgroundSound.SetVolume(Me.V)
            Me.DuelSound.SetVolume(Me.V)
            If Me.IsFightingLast Then
                Me.FinalDuelSound.SetVolume(Me.V)
            End If
            enableGameLoop()
        End Sub

        Private Sub method_0()
            Dim bCloseFirst As Boolean = True
            Dim bLoopSound As Boolean = False
            Dim performEffects As Boolean = False
            Dim x As Integer = 0
            Dim y As Integer = 0
            Dim dVolume As String = ""
            Dim waitTillDone As Boolean = False
            DXSound.PlaySound(Me.NNumber, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
        End Sub

        Private Function isBlocked(x As Integer, y As Integer) As Boolean
            If x < 1 Or x > Me.x Or y < 1 Or y > Me.y Then
                Me.NLS((DXSound.string_0 & "\ncannot.wav"), False)
                Me.NStop = True
                Return True
            ElseIf Me.GetBlock(x, y) Then
                DXSound.PlaySound(Me.WallCrashSound, True, False, False, 0, 0, "", False)
                Return True
            End If
            Me.CGrid(x, y) = 1.0!
            Return False
        End Function

        Private Sub missileCommand(m As MissileAction)
            If keyDownDisabled() Then
                Exit Sub
            End If
            If Not IsDoingGMissile Then
                Exit Sub
            End If
            If movedInLastTick Then
                Exit Sub
            End If
            Dim g As Integer = GFront
            Dim s As Integer = GSpeed
            Select Case m
                Case MissileAction.turnRight
                    GFront = (GFront + 90) Mod 360
                    Exit Select
                Case MissileAction.turnLeft
                    GFront -= 90
                    If GFront < 0 Then GFront = 0
                    Exit Select
                Case MissileAction.speedUp
                    GSpeed += 1
                    If GSpeed > 10 Then GSpeed = 10
                    Exit Select
                Case MissileAction.slowDown
                    GSpeed -= 1
                    If GSpeed < 0 Then GSpeed = 0
            End Select
            If g <> GFront Then
                Dim dir As String = ""
                If GFront = 0 Then dir = "north"
                If GFront = 90 Then dir = "east"
                If GFront = 180 Then dir = "south"
                If GFront = 270 Then dir = "west"
                NStop = False
                Me.NLS((DXSound.string_0 & "\\n" & dir & ".wav"), False)
                NStop = True
            End If
            If s <> GSpeed Then
                Me.VoiceNumber(GSpeed)
            End If
        End Sub

        Private Sub swim(d As SwimDirection)
            If keyDownDisabled() Then
                Exit Sub
            End If
            If Not IsInWater Then
                Exit Sub
            End If
            Dim tx As Integer = Me.px
            Dim ty As Integer = Me.py
            Select Case d
                ' For up and down, we don't want rapid-fire. If we use canSwim here, we will get rapid-fire since
                ' the swimSound doesn't play
                Case SwimDirection.up
                    If movedInLastTick Then Exit Sub
                    DepthDecrease()
                    Exit Select
                Case SwimDirection.down
                    If movedInLastTick Then Exit Sub
                    DepthIncrease()
                    Exit Select
                Case SwimDirection.north
                    If WDepth = 0 Then Exit Sub
                    If Not canSwim(pressedActionButtonInLastTick) Then Exit Sub
                    Me.py = ((Me.py + 1))
                    Exit Select
                Case SwimDirection.south
                    If WDepth = 0 Then Exit Sub
                    If Not canSwim(pressedActionButtonInLastTick) Then Exit Sub
                    Me.py = ((Me.py - 1))
                    Exit Select
                Case SwimDirection.east
                    ' For east and west, since these are done using arrow keys we change movedInLastTick instead of pressedActionButtonInLastTick
                    If WDepth = 0 Then Exit Sub
                    If Not canSwim(movedInLastTick) Then Exit Sub
                    Me.px = ((Me.px + 1))
                    Exit Select
                Case SwimDirection.west
                    If WDepth = 0 Then Exit Sub
                    If Not canSwim(movedInLastTick) Then Exit Sub
                    Me.px = ((Me.px - 1))
                    Exit Select
            End Select
            If isBlocked(Me.px, Me.py) Then
                Me.px = tx
                Me.py = ty
            End If
            If px <> tx Or py <> ty Then DXSound.PlaySound(SwimSound, True, False, False, 0, 0, "", False)
            Determine()
            Me.DidNotSwim = 0
        End Sub

        Private Sub MovePlayer(w As WalkDirection)
            If keyDownDisabled() Then
                Exit Sub
            End If
            If IsInWater Or IsDoingGMissile Then
                Exit Sub
            End If
            If Not canTakeStep() Then
                Exit Sub
            End If
            Dim tx As Integer = px
            Dim ty As Integer = py
            Select Case w
                Case WalkDirection.north
                    If Not inMindOfGuard() Then
                        py += 1
                    Else
                        challs(WControl).ControlChall(chall.ControlAction.north)
                    End If
                    Exit Select
                Case WalkDirection.south
                    If Not inMindOfGuard() Then
                        py -= 1
                    Else
                        challs(WControl).ControlChall(chall.ControlAction.south)
                    End If
                    Exit Select
                Case WalkDirection.east
                    If Not inMindOfGuard() Then
                        px += 1
                    Else
                        challs(WControl).ControlChall(chall.ControlAction.east)
                    End If
                    Exit Select
                Case WalkDirection.west
                    If Not inMindOfGuard() Then
                        px -= 1
                    Else
                        challs(WControl).ControlChall(chall.ControlAction.west)
                    End If
                    Exit Select
            End Select
            If isBlocked(px, py) Then
                px = tx
                py = ty
            End If
            If px <> tx Or py <> ty Then
                If (Me.whichFootstep = 0) Then
                    DXSound.PlaySound(Me.Footstep1Sound, True, False, False, 0, 0, "", False)
                Else
                    DXSound.PlaySound(Me.Footstep2Sound, True, False, False, 0, 0, "", False)
                End If
                Me.whichFootstep = (whichFootstep + 1) Mod maxFootsteps
                Determine()
            End If
        End Sub

        Public Sub MuteSounds()
            Me.WaterSound.SetVolume(-10000)
            Me.TargetSound.SetVolume(-10000)
            Me.MachineSound.SetVolume(-10000)
            Me.ReflectorSound.SetVolume(-10000)
            If (Not Me.BombAlarmSound Is Nothing) Then
                Me.BombAlarmSound.Stop()
            End If
            Dim num As Single = 1.0!
            Do
                Me.PassageSound(CInt(Math.Round(CDbl(num)))).Stop()
                Me.DoorSound(CInt(Math.Round(CDbl(num)))).Stop()
                num += 1
            Loop While (num <= 4.0!)
        End Sub

        Private Sub NLS(ByRef sFilename As String, ByRef Optional NWait As Boolean = False)
            Dim dsbufferdesc As DSBUFFERDESC
            Me.NNumber.Stop()
            THConstVars.MWait(10)
            dsbufferdesc.lFlags = CONST_DSBCAPSFLAGS.DSBCAPS_CTRLFREQUENCY
            Me.NNumber = DXSound.objDS.CreateSoundBufferFromFile(sFilename, dsbufferdesc)
            Me.NNumber.SetFrequency(Me.CurrentFreq)
            Me.method_0()

            If NWait Then
                Me.NumWait()
            End If
        End Sub

        Private Sub NumWait()
            Dim enteredWhilePressingKey As Boolean = DXInput.isKeyHeldDown()
            Do While (Me.NNumber.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                If enteredWhilePressingKey Then
                    enteredWhilePressingKey = DXInput.isKeyHeldDown()
                End If
                If Not enteredWhilePressingKey And DXInput.isKeyHeldDown() Then
                    Me.NStop = True
                    Exit Do
                End If
                Application.DoEvents()
            Loop
        End Sub

        Private Sub OMove_Tick()
            Me.SetC()
            Me.DoIfInWater()
            Me.RemoteControlMissile()
            Me.EndReflector()
            Me.TooCloseToMachine()
            Me.DoIfEscapingFromBomb()
            Me.CountdownMachine()
            Me.GenHealth()
            Me.TargetBeep()
            Me.TSSRooms()
            Me.FindPassage(False)
            Me.EndControl()
            Me.ProjectControl()
            Me.ProjectNeedle()
            Me.StartMusic()
        End Sub

        Private Sub OOCD(ByRef x As Integer, ByRef y As Integer)
            Dim num As Integer
            If (Me.BGrid(x, y) = Me.ClosedDoor) Then
                num = 0
                DXSound.smethod_1(Me.OpenDoorSound, True, False, x, y, num)
                Me.AllThingReplace(x, y, Me.OpenDoor)
            ElseIf (Me.BGrid(x, y) = Me.OpenDoor) Then
                num = 0
                DXSound.smethod_1(Me.CloseDoorSound, True, False, x, y, num)
                Me.AllThingReplace(x, y, Me.ClosedDoor)
                If ((x = Me.px) And (y = Me.py)) Then
                    num = 100
                    Dim challID As Integer = 0
                    Me.ReflectHit(num, challID)
                End If
            End If
        End Sub

        Private Sub OpenOrCloseDoor()
            If keyDownDisabled() Then
                Exit Sub
            End If
            If inMindOfGuard() Then
                challs(WControl).ControlChall(chall.ControlAction.openOrCloseDoor)
                Exit Sub
            End If
            Dim num As Integer
            Me.OOCD(Me.px, Me.py)
            If ((Me.px - 1) > 0) Then
                num = ((Me.px - 1))
                Me.OOCD(num, Me.py)
            End If
            If ((Me.px + 1) < Me.x) Then
                num = ((Me.px + 1))
                Me.OOCD(num, Me.py)
            End If
            If ((Me.py - 1) > 0) Then
                num = ((Me.py - 1))
                Me.OOCD(Me.px, num)
            End If
            If ((Me.py + 1) < Me.y) Then
                num = ((Me.py + 1))
                Me.OOCD(Me.px, num)
            End If
        End Sub

        Private Sub position()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Dim str As String
            Dim gX As Integer
            Dim gY As Integer
            Dim flag As Boolean
            Me.NNumber.Stop()
            Me.NStop = False
            If Me.IsDoingGMissile Then
                gX = Me.GX
                gY = Me.GY
            Else
                gX = Me.px
                gY = Me.py
            End If
            If Me.IsDoingGMissile Then
                If Not Me.BlewUpMineControl Then
                    If (Me.MineY > gY) Then
                        str = "north"
                    End If
                    If (Me.MineY < gY) Then
                        str = "south"
                    End If
                    If (Me.MineX < gX) Then
                        str = (str & "West")
                    End If
                    If (Me.MineX > gX) Then
                        str = (str & "East")
                    End If
                    If (Not Me.NStop AndAlso (str <> "")) Then
                        flag = True
                        Me.NLS((DXSound.string_0 & "\npos1.wav"), flag)
                        If Not Me.NStop Then
                            flag = False
                            Me.NLS((DXSound.string_0 & "\n" & str & ".wav"), flag)
                            Me.NStop = True
                        End If
                    End If
                    Return
                End If
                If Not Me.HasBlownUpMachine Then
                    If (Me.MachineY > gY) Then
                        str = "north"
                    End If
                    If (Me.MachineY < gY) Then
                        str = "south"
                    End If
                    If (Me.MachineX < gX) Then
                        str = (str & "West")
                    End If
                    If (Me.MachineX > gX) Then
                        str = (str & "East")
                    End If
                    If (Not Me.NStop AndAlso (str <> "")) Then
                        flag = True
                        Me.NLS((DXSound.string_0 & "\npos5.wav"), flag)
                        If Not Me.NStop Then
                            flag = False
                            Me.NLS((DXSound.string_0 & "\n" & str & ".wav"), flag)
                            Me.NStop = True
                        End If
                    End If
                    Return
                End If
            End If
            If (Me.WinY > gY) Then
                str = "north"
            End If
            If (Me.WinY < gY) Then
                str = "south"
            End If
            If (Me.WinX < gX) Then
                str = (str & "West")
            End If
            If (Me.WinX > gX) Then
                str = (str & "East")
            End If
            If Not Me.NStop Then
                If (str <> "") Then
                    flag = True
                    Me.NLS((DXSound.string_0 & "\npos2.wav"), flag)
                    If Me.NStop Then
                        Return
                    End If
                    flag = True
                    Me.NLS((DXSound.string_0 & "\n" & str & ".wav"), flag)
                End If
                str = ""
                If Not Me.NStop Then
                    If Not Me.HasKey Then
                        If (Me.KeyY > gY) Then
                            str = "north"
                        End If
                        If (Me.KeyY < gY) Then
                            str = "south"
                        End If
                        If (Me.KeyX < gX) Then
                            str = (str & "West")
                        End If
                        If (Me.KeyX > gX) Then
                            str = (str & "East")
                        End If
                        If Me.NStop Then
                            Return
                        End If
                        If (str <> "") Then
                            flag = True
                            Me.NLS((DXSound.string_0 & "\npos3.wav"), flag)
                            If Me.NStop Then
                                Return
                            End If
                            flag = True
                            Me.NLS((DXSound.string_0 & "\n" & str & ".wav"), flag)
                            If Me.NStop Then
                                Return
                            End If
                        End If
                    End If
                    str = ""
                    If (Me.PanelY > gY) Then
                        str = "north"
                    End If
                    If (Me.PanelY < gY) Then
                        str = "south"
                    End If
                    If (Me.PanelX < gX) Then
                        str = (str & "West")
                    End If
                    If (Me.PanelX > gX) Then
                        str = (str & "East")
                    End If
                    If Not Me.NStop Then
                        If (str <> "") Then
                            flag = True
                            Me.NLS((DXSound.string_0 & "\npos4.wav"), flag)
                            If Me.NStop Then
                                Return
                            End If
                            flag = True
                            Me.NLS((DXSound.string_0 & "\n" & str & ".wav"), flag)
                        End If
                        If Not Me.NStop Then
                            str = ""
                            If (180 > gY) Then
                                str = "north"
                            End If
                            If (180 < gY) Then
                                str = "south"
                            End If
                            If (&H2E < gX) Then
                                str = (str & "West")
                            End If
                            If (&H2E > gX) Then
                                str = (str & "East")
                            End If
                            If Not Me.NStop Then
                                If (str <> "") Then
                                    flag = True
                                    Me.NLS((DXSound.string_0 & "\npos6.wav"), flag)
                                    If Me.NStop Then
                                        Return
                                    End If
                                    flag = True
                                    Me.NLS((DXSound.string_0 & "\n" & str & ".wav"), flag)
                                End If
                                Me.NStop = True
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub ProjectControl()
            If Me.IsLaunchingControl Then
                If (controlLaunchTracker * frameTime >= 200) Then
                    controlLaunchTracker = 0
                    If (((Me.py + Me.DisControl)) > Me.y) Then
                        THConstVars.CannotDoKeydown = False
                        Me.IsLaunchingControl = False
                    Else
                        Dim cY As Integer = ((Me.py + Me.DisControl))
                        If Me.GetBlock(Me.px, cY) Then
                            THConstVars.CannotDoKeydown = False
                            Me.IsLaunchingControl = False
                        ElseIf (Me.DisControl > 20) Then
                            THConstVars.CannotDoKeydown = False
                            Me.IsLaunchingControl = False
                        Else
                            cY = ((Me.py + Me.DisControl))
                            If Me.IsChall(Me.px, cY) Then
                                Me.IsLaunchingControl = False
                                Me.IsControling = True
                                cY = ((Me.py + Me.DisControl))
                                Me.WControl = Me.GetChallID(Me.px, cY)
                                Me.challs(CInt(Me.WControl)).GetReadyForControl()
                                Me.ControlLaunchSound.Stop()
                                Dim bCloseFirst As Boolean = True
                                Dim bLoopSound As Boolean = False
                                Dim performEffects As Boolean = False
                                cY = 0
                                Dim y As Integer = 0
                                Dim dVolume As String = ""
                                Dim waitTillDone As Boolean = False
                                DXSound.PlaySound(Me.ControlHitSound, bCloseFirst, bLoopSound, performEffects, cY, y, dVolume, waitTillDone)
                                THConstVars.CannotDoKeydown = False
                            Else
                                Me.DisControl = ((Me.DisControl + 1))
                            End If
                        End If
                    End If
                Else
                    controlLaunchTracker += 1
                End If
            End If
        End Sub

        Private Sub ProjectNeedle()
            If Me.IsLaunchingNeedle Then
                Dim num4 As Integer
                If (needleTracker * frameTime >= 100) Then
                    needleTracker = 0
                    If (Me.DisNeedle > 10) Then
                        Me.IsLaunchingNeedle = False
                        THConstVars.CannotDoKeydown = False
                    Else
                        Dim cY As Integer = ((Me.py + Me.DisNeedle))
                        If Me.GetBlock(Me.px, cY) Then
                            THConstVars.CannotDoKeydown = False
                            Me.IsLaunchingNeedle = False
                            Me.NeedleLaunchSound.Stop()
                        Else
                            cY = ((Me.py + Me.DisNeedle))
                            If Me.IsChall(Me.px, cY) Then
                                cY = ((Me.py + Me.DisNeedle))
                                Dim challID As Integer = Me.GetChallID(Me.px, cY)
                                Me.IsLaunchingNeedle = False
                                Me.NeedleLaunchSound.Stop()
                                Dim bCloseFirst As Boolean = True
                                Dim bLoopSound As Boolean = False
                                Dim performEffects As Boolean = False
                                cY = 0
                                Dim y As Integer = 0
                                Dim dVolume As String = ""
                                Dim waitTillDone As Boolean = False
                                DXSound.PlaySound(Me.NeedleHitSound, bCloseFirst, bLoopSound, performEffects, cY, y, dVolume, waitTillDone)
                                Me.challs(challID).IsPoisoned = True
                                If Not Me.challs(challID).IsMaster Then
                                    num4 = challID
                                    Me.challs(num4).NumOfNeedles = ((Me.challs(num4).NumOfNeedles + 1))
                                Else
                                    Me.challs(challID).NumOfNeedles = 1
                                End If
                                y = 5
                                Me.HarmChall(y, Me.px, ((Me.py + Me.DisNeedle)))
                                THConstVars.CannotDoKeydown = False
                            Else
                                Me.DisNeedle = ((Me.DisNeedle + 1))
                            End If
                        End If
                    End If
                Else
                    needleTracker += 1
                End If
            End If
        End Sub

        Public Sub ReflectHit(ByRef amount As Integer, ByRef Optional ChallID As Integer = 0)
            Dim num2 As Integer
            Dim flag4 As Boolean
            If Me.DoingReflector Then
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = False
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                num2 = 0
                Dim dVolume As String = ""
                flag4 = False
                DXSound.PlaySound(Me.ShieldSound, bCloseFirst, bLoopSound, performEffects, x, num2, dVolume, flag4)
            Else
                Me.SubHealth(amount)
                If Me.h > 0 Then
                    num2 = 0
                    DXSound.smethod_1(Me.CharHitSound, True, False, Me.px, Me.py, num2)
                End If
            End If
        End Sub

        Private Sub reg()
            Me.EnableInFull()
        End Sub

        Private Sub RemoteControlMissile()
            If Me.IsDoingGMissile Then
                If (Me.GCount * frameTime >= (Me.GSpeed * 1000)) Then
                    Me.GCount = 0
                    Me.Grid(Me.GX, Me.GY) = Me.BGrid(Me.GX, Me.GY)
                    If (Me.GFront = 0) Then
                        Me.GY = ((Me.GY + 1))
                    End If
                    If (Me.GFront = 90) Then
                        Me.GX = ((Me.GX + 1))
                    End If
                    If (Me.GFront = 180) Then
                        Me.GY = ((Me.GY - 1))
                    End If
                    If (Me.GFront = 270) Then
                        Me.GX = ((Me.GX - 1))
                    End If
                    If ((((Me.GX < 1) Or (Me.GX > Me.x)) Or (Me.GY < 1)) Or (Me.GY > Me.y)) Then
                        Me.IsDoingGMissile = False
                    ElseIf (((Me.Grid(Me.GX, Me.GY) = Me.Wall) Or (Me.Grid(Me.GX, Me.GY) = Me.ClosedDoor)) Or (Me.Grid(Me.GX, Me.GY) = Me.LockedDoor)) Then
                        Me.IsDoingGMissile = False
                        Me.GExplodeMissile()
                    Else
                        Dim z As Integer = 0
                        DXSound.smethod_1(Me.directSoundSecondaryBuffer8_3, True, False, Me.GX, Me.GY, z)
                        If (Me.Grid(Me.GX, Me.GY) = Me.MineGuard) Then
                            Me.GExplodeMissile()
                        Else
                            Dim str As String
                            Dim flag As Boolean
                            If (Me.Grid(Me.GX, Me.GY) = Me.MineControl) Then
                                Me.GExplodeMissile()
                                Me.BlewUpMineControl = True
                                Me.Grid(Me.GX, Me.GY) = Me.Water
                                Me.BGrid(Me.GX, Me.GY) = Me.Water
                                Dim x As Integer = Me.x
                                Dim i As Integer = 1
                                Do While (i <= x)
                                    Dim num4 As Integer = (Math.Round(Conversion.Int(CDbl((CDbl(Me.y) / 2)))))
                                    Dim j As Integer = 1
                                    Do While (j <= num4)
                                        If (((Me.BGrid(i, j) = Me.Mine) Or (Me.BGrid(i, j) = Me.MineGuard)) Or (Me.BGrid(i, j) = Me.MineControl)) Then
                                            Me.Grid(i, j) = Me.Water
                                            Me.BGrid(i, j) = Me.Water
                                        End If
                                        Application.DoEvents()
                                        j = ((j + 1))
                                    Loop
                                    i = ((i + 1))
                                Loop
                                str = "r4.wav"
                                flag = False
                                DXSound.Radio(str, flag)
                            ElseIf ((Me.Grid(Me.GX, Me.GY) = Me.Machine) Or (Me.BGrid(Me.GX, Me.GY) = Me.Machine)) Then
                                Me.GExplodeMissile()
                            Else
                                Me.Grid(Me.GX, Me.GY) = Me.short_1
                                flag = True
                                Dim bLoopSound As Boolean = False
                                Dim performEffects As Boolean = False
                                z = 0
                                Dim y As Integer = 0
                                str = ""
                                Dim waitTillDone As Boolean = False
                                DXSound.PlaySound(Me.directSoundSecondaryBuffer8_4, flag, bLoopSound, performEffects, z, y, str, waitTillDone)
                            End If
                        End If
                    End If
                Else
                    Me.GCount = ((Me.GCount + 1))
                End If
            End If
        End Sub

        Private Sub ReportAmunition(ByRef Optional DNStop As Boolean = False)
            If keyDownDisabled() Then
                Exit Sub
            End If
            Me.NStop = False
            Me.NNumber.Stop()

            If Me.UnlimitedBullets Then
                Dim nWait As Boolean = False
                Me.NLS((DXSound.string_0 & "\nunlimited.wav"), nWait)
            Else
                Dim bullets As Long
                Select Case Strings.LCase(Me.Weapons(Me.WPos))
                    Case "gun"
                        bullets = Me.Bullets
                        Me.VoiceNumber(bullets)
                        Me.Bullets = (bullets)
                        Exit Select
                    Case "sword"
                        bullets = Me.Swrd
                        Me.VoiceNumber(bullets)
                        Me.Swrd = (bullets)
                        Exit Select
                    Case "bombs"
                        bullets = Me.bombs
                        Me.VoiceNumber(bullets)
                        Me.bombs = (bullets)
                        Exit Select
                    Case "laser"
                        bullets = Me.ALaser
                        Me.VoiceNumber(bullets)
                        Me.ALaser = (bullets)
                        Exit Select
                    Case "gmissile"
                        bullets = Me.short_2
                        Me.VoiceNumber(bullets)
                        Me.short_2 = (bullets)
                        Exit Select
                    Case "reflector"
                        bullets = Me.short_3
                        Me.VoiceNumber(bullets)
                        Me.short_3 = (bullets)
                        Exit Select
                    Case "control"
                        bullets = Me.AControl
                        Me.VoiceNumber(bullets)
                        Me.AControl = (bullets)
                        Exit Select
                End Select
            End If
            If Not DNStop Then
                Me.NStop = True
            End If
        End Sub

        Private Sub ReturnToStartingPoint()
            If keyDownDisabled() Then
                Exit Sub
            End If
            If Not Me.IsFightingLast Then
                THConstVars.CannotDoKeydown = True
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = False
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.TeleportSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                Do While (Me.TeleportSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                    Application.DoEvents()
                Loop
                Me.px = 2
                Me.py = &H67
                THConstVars.CannotDoKeydown = False
            End If
        End Sub

        Private Sub SayHasVisited()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Dim flag As Boolean
            Me.NStop = True
            If (Me.CGrid(Me.px, Me.py) = 1.0!) Then
                flag = False
                Me.NLS((DXSound.string_0 & "\nyes.wav"), flag)
            Else
                flag = False
                Me.NLS((DXSound.string_0 & "\nno.wav"), flag)
            End If
        End Sub

        Private Sub SayInventory()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Me.NNumber.Stop()
            Me.NStop = False
            Dim nWait As Boolean = True
            Me.NLS((DXSound.string_0 & "\i.wav"), nWait)
            If Not Me.NStop Then
                Dim flag As Boolean
                If Me.HasKey Then
                    nWait = True
                    Me.NLS((DXSound.string_0 & "\i1.wav"), nWait)
                    flag = True
                End If
                If Not Me.NStop Then
                    If Me.HasMainKey Then
                        nWait = True
                        Me.NLS((DXSound.string_0 & "\i2.wav"), nWait)
                        flag = True
                    End If
                    If Not Me.NStop Then
                        If Me.HasShutDownCard Then
                            nWait = True
                            Me.NLS((DXSound.string_0 & "\i3.wav"), nWait)
                            flag = True
                        End If
                        If Not flag Then
                            nWait = False
                            Me.NLS((DXSound.string_0 & "\nnothing.wav"), nWait)
                        End If
                        Me.NStop = True
                    End If
                End If
            End If
        End Sub

        Private Sub SayLocation()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Me.NNumber.Stop()
            Me.NStop = False
            Dim px As Long = Me.px
            Me.VoiceNumber(px)
            Me.px = (px)
            If Not Me.NStop Then
                Me.NumWait()

                If Not Me.NStop Then
                    px = Me.py
                    Me.VoiceNumber(px)
                    Me.py = (px)
                    Me.NStop = True
                End If
            End If
        End Sub

        Private Function ScanForItem(ByRef Range As Integer, ByRef thing As Integer, ByRef Optional replaceIt As Integer = 0) As Boolean
            Dim flag As Boolean
            Dim flag2 As Boolean
            Dim flag3 As Boolean
            Dim num2 As Integer = Range
            Dim i As Integer = 0
            Do While (i <= num2)
                If ((((((Me.px - i)) >= 1) And (((Me.px + i)) <= Me.x)) And (((Me.py + i)) <= Me.y)) And (((Me.py - i)) >= 1)) Then
                    If (Me.Grid(((Me.px + i)), Me.py) = thing) Then
                        flag = True
                        If (Not replaceIt = 0) Then
                            Me.Grid(((Me.px + i)), Me.py) = replaceIt
                        End If
                    End If
                    If (Me.Grid(Me.px, ((Me.py + i))) = thing) Then
                        flag2 = True
                        If (Not replaceIt = 0) Then
                            Me.Grid(Me.px, ((Me.py + i))) = replaceIt
                        End If
                    End If
                    If (Me.Grid(((Me.px - i)), Me.py) = thing) Then
                        flag = True
                        If (Not replaceIt = 0) Then
                            Me.Grid(((Me.px - i)), Me.py) = replaceIt
                        End If
                    End If
                    If (Me.Grid(Me.px, ((Me.py - i))) = thing) Then
                        flag2 = True
                        If (Not replaceIt = 0) Then
                            Me.Grid(Me.px, ((Me.py - i))) = replaceIt
                        End If
                    End If
                End If
                i = ((i + 1))
            Loop
            If (flag And flag2) Then
                flag3 = True
            End If
            Return flag3
        End Function

        Private Sub SetC()
            Dim num As Integer
            If Me.IsControling Then
                num = 0
                DXSound.SetCoordinates(Me.challs(CInt(Me.WControl)).x, Me.challs(CInt(Me.WControl)).y, num)
            Else
                num = 0
                DXSound.SetCoordinates(Me.px, Me.py, num)
            End If
            Me.CharHitSound.GetDirectSound3DBuffer.SetPosition(CSng(Me.px), CSng(Me.py), 0!, CONST_DS3DAPPLYFLAGS.DS3D_IMMEDIATE)
        End Sub

        Private Sub SetChalls()
            Me.challs = New chall((Me.ChallAmount + 1) - 1) {}
            Dim challAmount As Integer = Me.ChallAmount
            Dim i As Integer = 1
            Do While (i <= challAmount)
                Dim flag As Boolean
                Me.challs(i) = New chall
                Do While Not flag
                    Dim px As Integer = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((Me.x * VBMath.Rnd)))))))
                    Dim py As Integer = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((Me.y * VBMath.Rnd)))))))
                    If ((((px < 3) Or (px > (Me.x - 2))) Or (py < 3)) Or (py > (Me.y - 2))) Then
                        flag = False
                    Else
                        Dim num4 As Integer = Me.BGrid(px, py)
                        If (((((((num4 <> Me.ClosedDoor) And (num4 <> Me.OpenDoor)) And (num4 <> Me.LockedDoor)) And (num4 <> Me.Water)) And (num4 <> Me.Wall)) And (num4 <> Me.Mine)) And (num4 <> Me.MineGuard)) Then
                            flag = True
                            Dim cHealth As Integer = 50
                            Dim s As Integer = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((2.0! * VBMath.Rnd)))))))
                            Dim t As Integer = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((3.0! * VBMath.Rnd)))))))
                            Dim dead As Boolean = False
                            Me.challs(i).init(px, py, i, cHealth, s, t, dead)
                        End If
                    End If
                Loop
                flag = False
                Application.DoEvents()
                i = ((i + 1))
            Loop
        End Sub

        Private Sub SetWeapon(ByRef WName As String, ByRef Optional pos As Integer = 0)
            If (pos = 0) Then
                Dim num As Integer
                Do While (Me.Weapons(num) <> "")
                    num = ((num + 1))
                    If (Me.Weapons(num) = WName) Then
                        Return
                    End If
                Loop
                Me.Weapons(num) = WName
            Else
                Me.Weapons(pos) = WName
            End If
        End Sub

        Private Sub shoot()
            Dim flag As Boolean
            Dim flag2 As Boolean
            Dim flag3 As Boolean
            Dim num2 As Integer
            Dim num3 As Integer
            Dim str As String
            Dim flag4 As Boolean
            If (Me.Bullets <= 0) Then
                flag = True
                flag2 = False
                flag3 = False
                num2 = 0
                num3 = 0
                str = ""
                flag4 = False
                DXSound.PlaySound(Me.EmptySound, flag, flag2, flag3, num2, num3, str, flag4)
            Else
                flag4 = True
                flag3 = False
                flag2 = False
                num3 = 0
                num2 = 0
                str = ""
                flag = False
                DXSound.PlaySound(Me.GunSound, flag4, flag3, flag2, num3, num2, str, flag)
                If Not Me.UnlimitedBullets Then
                    Me.Bullets = ((Me.Bullets - 1))
                End If
                If Me.IsChall(Me.px, Me.py) Then
                    Dim amount As Integer = (Math.Round(CDbl((11.0! + Conversion.Int(CSng((5.0! * VBMath.Rnd)))))))
                    Me.HarmChall(amount, 0, 0)
                End If
            End If
        End Sub

        Private Sub ShootLaser()
            Dim flag As Boolean
            Dim flag2 As Boolean
            Dim flag3 As Boolean
            Dim num2 As Integer
            Dim num3 As Integer
            Dim str As String
            Dim flag4 As Boolean
            If (Me.ALaser <= 0) Then
                flag = True
                flag2 = False
                flag3 = False
                num2 = 0
                num3 = 0
                str = ""
                flag4 = False
                DXSound.PlaySound(Me.EmptySound, flag, flag2, flag3, num2, num3, str, flag4)
            Else
                flag4 = True
                flag3 = False
                flag2 = False
                num3 = 0
                num2 = 0
                str = ""
                flag = False
                DXSound.PlaySound(Me.LaserGunSound, flag4, flag3, flag2, num3, num2, str, flag)
                If Not Me.UnlimitedBullets Then
                    Me.ALaser = ((Me.ALaser - 1))
                End If
                If Me.IsChall(Me.px, Me.py) Then
                    Dim amount As Integer = (Math.Round(CDbl((11.0! + Conversion.Int(CSng((100.0! * VBMath.Rnd)))))))
                    Me.HarmChall(amount, 0, 0)
                End If
            End If
        End Sub

        Public Sub ShutDown()
            THConstVars.IsShuttingDown = True
            If Me.HasDoneInit Then
                Dim challAmount As Integer = Me.ChallAmount
                Dim i As Integer = 1
                Do While (i <= challAmount)
                    Me.challs(i) = Nothing
                    i = ((i + 1))
                Loop
                DXSound.directSound3DListener8_0 = Nothing
                DXSound.objDS = Nothing
                DXSound.objDX = Nothing
            End If
            Information.Err.Clear()
            ProjectData.EndApp()
        End Sub

        Private Sub snake()
            If (Me.Swrd > 0) Then
                If Not Me.UnlimitedBullets Then
                    Me.Swrd = ((Me.Swrd - 1))
                End If
                Me.Grid(Me.px, Me.py) = 1
            Else
                Dim num As Integer = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((2.0! * VBMath.Rnd)))))))
                If (num = 2) Then
                    Dim amount As Integer = 5
                    Dim challID As Integer = 0
                    Me.ReflectHit(amount, challID)
                End If
            End If
        End Sub

        Public Sub StartMusic()
            Dim flag As Boolean
            Dim flag2 As Boolean
            Dim flag3 As Boolean
            Dim num As Integer
            Dim num2 As Integer
            Dim str As String
            Dim flag4 As Boolean
            If ((Me.NumAlert > 0) And Not Me.IsFightingLast) Then
                If (Me.DuelSound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_LOOPING) Then
                    Me.DuelSound.SetVolume(Me.V)
                    If (Me.CaughtSound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING) Then
                        flag = False
                        flag2 = True
                        flag3 = False
                        num = 0
                        num2 = 0
                        str = ""
                        flag4 = False
                        DXSound.PlaySound(Me.DuelSound, flag, flag2, flag3, num, num2, str, flag4)
                    End If
                End If
            Else
                Dim duelSound As DirectSoundSecondaryBuffer8 = Me.DuelSound
                duelSound.Stop()
                duelSound.SetCurrentPosition(0)
                duelSound = Nothing
            End If
            If ((Me.NumAlert = 0) And Not Me.IsFightingLast) Then
                If (Me.BackgroundSound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_LOOPING) Then
                    Me.BackgroundSound.SetVolume(Me.V)
                    flag4 = False
                    flag3 = True
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.BackgroundSound, flag4, flag3, flag2, num2, num, str, flag)
                End If
            ElseIf (Me.CaughtSound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING) Then
                Dim backgroundSound As DirectSoundSecondaryBuffer8 = Me.BackgroundSound
                backgroundSound.Stop()
                backgroundSound.SetCurrentPosition(0)
                backgroundSound = Nothing
            End If
            If Me.IsFightingLast Then
                Me.BackgroundSound.Stop()
                Me.DuelSound.Stop()
                Me.BackgroundSound.SetCurrentPosition(0)
                Me.DuelSound.SetCurrentPosition(0)
                If (Me.FinalDuelSound Is Nothing) Then
                    Me.FinalDuelSound = DXSound.LoadSound((DXSound.SoundPath & "\finalfight.wav"))
                End If
                If (Me.FinalDuelSound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_LOOPING) Then
                    Me.FinalDuelSound.SetVolume(Me.V)
                    flag4 = False
                    flag3 = True
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.FinalDuelSound, flag4, flag3, flag2, num2, num, str, flag)
                End If
            ElseIf (Not Me.FinalDuelSound Is Nothing) Then
                Me.FinalDuelSound.Stop()
                Me.FinalDuelSound = Nothing
            End If
        End Sub

        Private Sub updateFrame()
            Try
                System.Diagnostics.Debug.WriteLine("timer: before performaction")
                performActions()
                System.Diagnostics.Debug.WriteLine("timer: after performaction")
                If ((Not ((THConstVars.CannotDoKeydown And Not Me.IsLaunchingControl) And Not Me.IsLaunchingNeedle) AndAlso Not Me.IsInPauseState)) Then
                    System.Diagnostics.Debug.WriteLine("timer: ok to execute oMove_tick")
                    Me.OMove_Tick()
                End If
                frameTracker += 1
                Select Case THConstVars.Difficulty
                    Case 1.0!
                        If (frameTracker >= 100) Then
                            frameTracker = 0
                            Me.EMove_Tick()
                        End If
                        Exit Select
                    Case 2.0!
                        If (frameTracker >= 60) Then
                            frameTracker = 0
                            Me.EMove_Tick()
                        End If
                        Exit Select
                    Case 3.0!
                        If (frameTracker >= 40) Then
                            frameTracker = 0
                            Me.EMove_Tick()
                        End If
                        Exit Select
                    Case 4.0!
                        If (frameTracker >= 10) Then
                            frameTracker = 0
                            EMove_Tick()
                        End If
                        Exit Select
                End Select
                Me.CharDied(False) ' If char is dead, will play lose sound. Also responsible for low health notification
            Catch e As Exception
                THConstVars.handleException(e, Timer1)
            End Try
        End Sub

        Private Sub Stats()
            If keyDownDisabled() Then
                Exit Sub
            End If
            Me.NNumber.Stop()
            Me.NStop = False
            Dim nWait As Boolean = True
            Me.NLS((DXSound.string_0 & "\nstatus1.wav"), nWait)
            If Not Me.NStop Then
                Dim h As Long
                If Me.UnlimitedHealth Then
                    nWait = True
                    Me.NLS((DXSound.string_0 & "\nunlimited.wav"), nWait)
                    If Me.NStop Then
                        Return
                    End If
                Else
                    h = Me.h
                    Me.VoiceNumber(h)
                    Me.h = (h)
                    If Me.NStop Then
                        Return
                    End If
                End If
                nWait = True
                Me.NLS((DXSound.string_0 & "\nstatus2.wav"), nWait)
                If Not Me.NStop Then
                    h = Me.Points
                    Me.VoiceNumber(h)
                    Me.Points = CInt(h)
                    Me.NumWait()

                    If Not Me.NStop Then
                        nWait = True
                        Me.NLS((DXSound.string_0 & "\nstatus3.wav"), nWait)
                        Me.NumWait()

                        If Not Me.NStop Then
                            nWait = True
                            Me.ReportAmunition(nWait)
                            Me.NumWait()

                            If Not Me.NStop Then
                                nWait = True
                                Me.NLS((DXSound.string_0 & "\nstatus4.wav"), nWait)
                                If Not Me.NStop Then
                                    nWait = True
                                    Me.NLS((DXSound.string_0 & "\nstatus5.wav"), nWait)
                                    If Not Me.NStop Then
                                        h = Me.ch
                                        Me.VoiceNumber(h)
                                        Me.ch = CInt(h)
                                        Me.NumWait()

                                        If Not Me.NStop Then
                                            If (Me.ch = 1) Then
                                                nWait = True
                                                Me.NLS((DXSound.string_0 & "\nstatus6.wav"), nWait)
                                            Else
                                                nWait = True
                                                Me.NLS((DXSound.string_0 & "\nstatus6s.wav"), nWait)
                                            End If
                                            If Not Me.NStop Then
                                                Me.position()
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub StopMusic()
            Me.DuelSound.Stop()
            Me.BackgroundSound.Stop()

            If Me.IsFightingLast Then
                Me.FinalDuelSound.Stop()
            End If
        End Sub

        Private Sub StopTimer()
        End Sub

        Public Sub SubHealth(ByRef ASub As Integer)
            If Not Me.UnlimitedHealth Then
                Me.h = ((Me.h - ASub))
            End If
        End Sub

        Private Sub SwingSword()
            If (Me.Swrd > 0) Then
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = False
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.SwingSwordSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                If Me.IsChall(Me.px, Me.py) Then
                    Dim num2 As Integer = (Math.Round(CDbl(VBMath.Rnd)))
                    If (num2 <= Me.Accuracy) Then
                        Me.Accuracy = (Math.Round(CDbl((Me.Accuracy + ((11.0! + Conversion.Int(CSng((10.0! * VBMath.Rnd)))) / 1000.0!)))))
                        Dim amount As Integer = (Math.Round(CDbl(((11.0! + Conversion.Int(CSng((10.0! * VBMath.Rnd)))) * Me.Swrd))))
                        THConstVars.CannotDoKeydown = True
                        Do While (Me.SwingSwordSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                            Application.DoEvents()
                        Loop
                        THConstVars.CannotDoKeydown = False
                        Me.HarmChall(amount, 0, 0)
                    End If
                End If
            End If
        End Sub

        Private Function keyDownDisabled() As Boolean
            Return THConstVars.CannotDoKeydown Or Me.IsInMenu
        End Function

        Function inMindOfGuard() As Boolean
            Return IsControling And Strings.LCase(Me.Weapons(Me.WPos)) = "control"
        End Function

        Private Sub SwitchToWeapon(ByRef pos As Integer)
            If keyDownDisabled() Then
                Exit Sub
            End If
            Me.NStop = False
            Me.NNumber.Stop()
            Dim str As String = Strings.LCase(Me.Weapons(pos))
            If (Me.Weapons(pos) <> "") Then
                Dim flag As Boolean
                If (Me.Weapons(pos) <> "control") Then
                    Me.IsControling = False
                End If
                If (((Me.Weapons(pos) = "control") AndAlso (Me.WControl > 0)) AndAlso Me.challs(CInt(Me.WControl)).IsBeingControled) Then
                    Me.IsControling = True
                End If
                Select Case str
                    Case "gun"
                        flag = False
                        Me.NLS((DXSound.string_0 & "\nsgun.wav"), flag)
                        Exit Select
                    Case "sword"
                        flag = False
                        Me.NLS((DXSound.string_0 & "\nssword.wav"), flag)
                        Exit Select
                    Case "bombs"
                        flag = False
                        Me.NLS((DXSound.string_0 & "\nsneedle.wav"), flag)
                        Exit Select
                    Case "laser"
                        flag = False
                        Me.NLS((DXSound.string_0 & "\nslaser.wav"), flag)
                        Exit Select
                    Case "gmissile"
                        flag = False
                        Me.NLS((DXSound.string_0 & "\nsgmissile.wav"), flag)
                        Exit Select
                    Case "reflector"
                        flag = False
                        Me.NLS((DXSound.string_0 & "\nsreflector.wav"), flag)
                        Exit Select
                    Case "control"
                        flag = False
                        Me.NLS((DXSound.string_0 & "\nscontrol.wav"), flag)
                        Exit Select
                End Select
                Me.WPos = pos
                Me.NStop = True
            End If
        End Sub

        Private Sub Sword()
            Me.Grid(Me.px, Me.py) = 1
            Dim num As Integer = (Math.Round(CDbl((1.0! + Conversion.Int(CSng((2.0! * VBMath.Rnd)))))))
            If (num = 1) Then
                Me.h = ((Me.h + 5))
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = False
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.PickUpHealthSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
            Else
                Me.Swrd = ((Me.Swrd + 1))
            End If
        End Sub

        Private Sub TargetBeep()
            Dim flag As Boolean
            If Me.IsControling Then
                flag = Me.IsChall(Me.challs(CInt(Me.WControl)).x, Me.challs(CInt(Me.WControl)).y)
            Else
                flag = Me.IsChall(Me.px, Me.py)
            End If
            If flag Then
                Dim bCloseFirst As Boolean = False
                Dim bLoopSound As Boolean = True
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.TargetSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
            Else
                Me.TargetSound.Stop()
            End If
        End Sub

        Public Sub ThingReplace(ByRef x As Integer, ByRef y As Integer, ByRef thing As Integer)
            Me.Grid(x, y) = thing
        End Sub

        Private Sub TooCloseToMachine()
            If (tooCloseToMachineTracker * frameTime >= 1000) Then
                tooCloseToMachineTracker = 0
                If (Me.HasTurnedOffMachine AndAlso (((Me.MachineX + &H61) >= Me.px) And ((Me.MachineY + &H61) >= Me.py))) Then
                    THConstVars.CannotDoKeydown = True
                    Dim bCloseFirst As Boolean = True
                    Dim bLoopSound As Boolean = False
                    Dim performEffects As Boolean = False
                    Dim x As Integer = 0
                    Dim y As Integer = 0
                    Dim dVolume As String = ""
                    Dim waitTillDone As Boolean = False
                    DXSound.PlaySound(Me.AlarmSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                    waitTillDone = True
                    performEffects = False
                    bLoopSound = False
                    y = 0
                    x = 0
                    dVolume = ""
                    bCloseFirst = False
                    DXSound.PlaySound(Me.BigBlastSound, waitTillDone, performEffects, bLoopSound, y, x, dVolume, bCloseFirst)
                    Me.h = 0
                    Do While (Me.BigBlastSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                        Application.DoEvents()
                    Loop
                End If
            Else
                tooCloseToMachineTracker += 1
            End If
        End Sub

        Private Sub TSSRooms()
            Dim str As String
            Dim flag As Boolean
            If ((((Me.px > &H2A) And (Me.px < &H30)) And (Me.py > 180)) And (Me.py < 190)) Then
                If Not Me.HasBeenA Then
                    str = "r13.wav"
                    flag = False
                    DXSound.Radio(str, flag)
                    Me.HasBeenA = True
                End If
            Else
                Me.HasBeenA = False
            End If
            If ((((Me.px > 310) And (Me.px < 400)) And (Me.py > &H141)) And (Me.py < 400)) Then
                If Not Me.HasBeenB Then
                    str = "r14.wav"
                    flag = False
                    DXSound.Radio(str, flag)
                    Me.HasBeenB = True
                End If
            Else
                Me.HasBeenB = False
            End If
            If ((((Me.px > 220) And (Me.px < 270)) And (Me.py > &HAC)) And (Me.py < 230)) Then
                If Not Me.HasBeenC Then
                    str = "r15.wav"
                    flag = False
                    DXSound.Radio(str, flag)
                    Me.HasBeenC = True
                End If
            Else
                Me.HasBeenC = False
            End If
        End Sub

        Private Sub TurnGMissile(ByRef dir_Renamed As String)
            Dim flag As Boolean
            Me.NNumber.Stop()

            If (dir_Renamed = "l") Then
                If ((Me.GFront - 90) < 0) Then
                    Me.GFront = 270
                Else
                    Me.GFront = ((Me.GFront - 90))
                End If
            End If
            If (dir_Renamed = "r") Then
                If ((Me.GFront + 90) > 270) Then
                    Me.GFront = 0
                Else
                    Me.GFront = ((Me.GFront + 90))
                End If
            End If
            If (Me.GFront = 0) Then
                flag = False
                Me.NLS((DXSound.string_0 & "\nnorth.wav"), flag)
            End If
            If (Me.GFront = 90) Then
                flag = False
                Me.NLS((DXSound.string_0 & "\neast.wav"), flag)
            End If
            If (Me.GFront = 180) Then
                flag = False
                Me.NLS((DXSound.string_0 & "\nsouth.wav"), flag)
            End If
            If (Me.GFront = 270) Then
                flag = False
                Me.NLS((DXSound.string_0 & "\nwest.wav"), flag)
            End If
            Me.NStop = True
        End Sub

        Public Sub UnmuteSounds()
            Dim flag As Boolean
            Dim flag2 As Boolean
            Dim flag3 As Boolean
            Dim num As Integer
            Dim num2 As Integer
            Dim str As String
            Dim flag4 As Boolean
            Me.WaterSound.SetVolume(0)
            If (Me.IsInWater And (Me.WDepth > 0)) Then
                flag = False
                flag2 = True
                flag3 = False
                num = 0
                num2 = 0
                str = ""
                flag4 = False
                DXSound.PlaySound(Me.WaterSound, flag, flag2, flag3, num, num2, str, flag4)
            Else
                Me.WaterSound.Stop()
            End If
            Me.TargetSound.SetVolume(0)
            Me.MachineSound.SetVolume(0)
            If (Not Me.HasTurnedOffMachine And Not Me.HasBlownUpMachine) Then
                flag4 = False
                flag3 = True
                flag2 = False
                num2 = 0
                num = 0
                str = ""
                flag = False
                DXSound.PlaySound(Me.MachineSound, flag4, flag3, flag2, num2, num, str, flag)
            Else
                Me.MachineSound.Stop()
            End If
            Me.ReflectorSound.SetVolume(0)
            If Me.DoingReflector Then
                flag4 = False
                flag3 = True
                flag2 = False
                num2 = 0
                num = 0
                str = ""
                flag = False
                DXSound.PlaySound(Me.ReflectorSound, flag4, flag3, flag2, num2, num, str, flag)
            Else
                Me.ReflectorSound.Stop()
            End If
        End Sub

        Private Sub UseControler()
            If IsFightingLast Then
                Exit Sub
            End If
            Dim flag As Boolean
            Dim flag2 As Boolean
            Dim flag3 As Boolean
            Dim num As Integer
            Dim num2 As Integer
            Dim str As String
            Dim flag4 As Boolean
            If Not Me.IsFull Then
                flag = True
                flag2 = False
                flag3 = False
                num = 0
                num2 = 0
                str = ""
                flag4 = False
                DXSound.PlaySound(Me.AccessDeniedSound, flag, flag2, flag3, num, num2, str, flag4)
            ElseIf ((Me.AControl > 0) AndAlso Not Me.IsLaunchingControl) Then
                If Not Me.UnlimitedBullets Then
                    Me.AControl = ((Me.AControl - 1))
                End If
                Me.DisControl = 1
                controlLaunchTracker = 0
                THConstVars.CannotDoKeydown = True
                flag4 = True
                flag3 = False
                flag2 = False
                num2 = 0
                num = 0
                str = ""
                flag = False
                DXSound.PlaySound(Me.ControlLaunchSound, flag4, flag3, flag2, num2, num, str, flag)
                Me.IsLaunchingControl = True
            End If
        End Sub

        Private Sub UseReflector()
            If (Me.short_3 > 0) Then
                If Not Me.UnlimitedBullets Then
                    Me.short_3 = ((Me.short_3 - 1))
                End If
                Me.reflectorTime = 0
                Me.DoingReflector = True
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = True
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.ReflectorSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
            End If
        End Sub

        Private Sub UseWeapon()
            If keyDownDisabled() Then
                Exit Sub
            End If
            If Me.IsInWater And Me.WDepth > 0 Then
                Exit Sub
            End If
            Dim w As String = Strings.LCase(Me.Weapons(Me.WPos))
            ' If this is a fire-once weapon, and right now the player is not holding down the action button,
            ' or this is a rapid-fire weapon.
            If fireRates(w) = 0 And Not pressedActionButtonInLastTick Or fireRates(w) > 0 And (DateTime.Now - fireDate).TotalMilliseconds() >= fireRates(w) Then
                pressedActionButtonInLastTick = True
                Select Case w
                    Case "gun"
                        Me.shoot()
                        Exit Select
                    Case "sword"
                        Me.SwingSword()
                        Exit Select
                    Case "bombs"
                        Me.Bomb()
                        Exit Select
                    Case "laser"
                        Me.ShootLaser()
                        Exit Select
                    Case "gmissile"
                        Me.FireRemoteControlMissile()
                        Exit Select
                    Case "reflector"
                        Me.UseReflector()
                        Exit Select
                    Case "control"
                        If Not inMindOfGuard() Then
                            Me.UseControler()
                        Else
                            challs(WControl).ControlChall(chall.ControlAction.fireWeapon)
                        End If
                        Exit Select
                End Select
                fireDate = DateTime.Now
            End If
        End Sub

        Private Sub v1(ByRef string_2 As String)
            Dim nWait As Boolean = False
            Me.NLS((DXSound.NumPath & "\" & string_2 & ".wav"), nWait)
            If Not Me.NStop Then
                Me.NumWait()

                If Me.NStop Then
                End If
            End If
        End Sub

        Private Sub v2(ByRef string_2 As String)
            Dim flag As Boolean
            If ((Conversion.Val(CStr(string_2)) >= 1) And (Conversion.Val(CStr(string_2)) <= 20)) Then
                flag = False
                Me.NLS((DXSound.NumPath & "\" & Conversions.ToString(Conversion.Val(CStr(string_2))) & ".wav"), flag)
                If Not Me.NStop Then
                    Me.NumWait()

                    If Not Me.NStop Then
                    End If
                End If
            Else
                If (Strings.Mid(string_2, 1, 1) <> "0") Then
                    flag = False
                    Me.NLS((DXSound.NumPath & "\" & Strings.Mid(string_2, 1, 1) & "0.wav"), flag)
                    If Me.NStop Then
                        Return
                    End If
                    Me.NumWait()

                    If Me.NStop Then
                        Return
                    End If
                End If
                If (Strings.Mid(string_2, 2, 1) <> "0") Then
                    flag = False
                    Me.NLS((DXSound.NumPath & "\" & Strings.Mid(string_2, 2, 1) & ".wav"), flag)
                    If Not Me.NStop Then
                        Me.NumWait()

                        If Me.NStop Then
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub v3(ByRef string_2 As String)
            If (Conversion.Val(Strings.Mid(string_2, 1, 1)) > 0) Then
                Dim nWait As Boolean = False
                Me.NLS((DXSound.NumPath & "\" & Strings.Mid(string_2, 1, 1) & ".wav"), nWait)
                If Not Me.NStop Then
                    Me.NumWait()

                    If Not Me.NStop Then
                        nWait = False
                        Me.NLS((DXSound.NumPath & "\100.wav"), nWait)
                        If Not Me.NStop Then
                            Me.NumWait()

                            If Not Me.NStop Then
                                Me.v2(Strings.Mid(string_2, 2, 2))
                            End If
                        End If
                    End If
                End If
            Else
                Me.v2(Strings.Mid(string_2, 2, 2))
            End If
        End Sub

        Private Sub v4(ByRef string_2 As String)
            If (Conversion.Val(Strings.Mid(string_2, 1, 1)) >= 1) Then
                Dim nWait As Boolean = False
                Me.NLS((DXSound.NumPath & "\" & Strings.Mid(string_2, 1, 1) & ".wav"), nWait)
                If Not Me.NStop Then
                    Me.NumWait()

                    If Not Me.NStop Then
                        nWait = False
                        Me.NLS((DXSound.NumPath & "\1000.wav"), nWait)
                        If Not Me.NStop Then
                            Me.NumWait()

                            If Not Me.NStop Then
                                Me.v3(Strings.Mid(string_2, 2, 3))
                            End If
                        End If
                    End If
                End If
            Else
                Me.v3(Strings.Mid(string_2, 2, 3))
            End If
        End Sub

        Private Sub v5(ByRef string_2 As String)
            Me.v2(Strings.Mid(string_2, 1, 2))
            If Not Me.NStop Then
                Dim nWait As Boolean = False
                Me.NLS((DXSound.NumPath & "\1000.wav"), nWait)
                If Not Me.NStop Then
                    Me.NumWait()

                    If Not Me.NStop Then
                        Me.v3(Strings.Mid(string_2, 3, 3))
                    End If
                End If
            End If
        End Sub

        Private Sub v6(ByRef string_2 As String)
            Me.v3(Strings.Mid(string_2, 1, 3))
            If Not Me.NStop Then
                Me.NumWait()

                If Not Me.NStop Then
                    If (Strings.Mid(string_2, 1, 3) <> "000") Then
                        Dim nWait As Boolean = False
                        Me.NLS((DXSound.NumPath & "\1000.wav"), nWait)
                        If Me.NStop Then
                            Return
                        End If
                    End If
                    Me.NumWait()

                    If Not Me.NStop Then
                        Me.v3(Strings.Mid(string_2, 4, 3))
                    End If
                End If
            End If
        End Sub

        Private Sub v7(ByRef string_2 As String)
            If (Strings.Mid(string_2, 1, 1) <> "0") Then
                Dim nWait As Boolean = False
                Me.NLS((DXSound.NumPath & "\" & Strings.Mid(string_2, 1, 1) & ".wav"), nWait)
                If Me.NStop Then
                    Return
                End If
                Me.NumWait()

                If Me.NStop Then
                    Return
                End If
                nWait = False
                Me.NLS((DXSound.NumPath & "\1000000.wav"), nWait)
                If Me.NStop Then
                    Return
                End If
                Me.NumWait()

                If Me.NStop Then
                    Return
                End If
            End If
            If Not Me.NStop Then
                Me.v6(Strings.Mid(string_2, 2, 6))
            End If
        End Sub

        Private Sub v8(ByRef string_2 As String)
            Me.v2(Strings.Mid(string_2, 1, 2))
            If Not Me.NStop Then
                Me.NumWait()

                If Not Me.NStop Then
                    Dim nWait As Boolean = False
                    Me.NLS((DXSound.NumPath & "\1000000.wav"), nWait)
                    If Not Me.NStop Then
                        Me.NumWait()

                        If Not Me.NStop Then
                            Me.v6(Strings.Mid(string_2, 3, 6))
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub v9(ByRef string_2 As String)
            Me.v3(Strings.Mid(string_2, 1, 3))
            If Not Me.NStop Then
                Me.NumWait()

                If Not Me.NStop Then
                    Dim nWait As Boolean = False
                    Me.NLS((DXSound.NumPath & "\1000000.wav"), nWait)
                    If Not Me.NStop Then
                        Me.NumWait()

                        If Not Me.NStop Then
                            Me.v6(Strings.Mid(string_2, 4, 6))
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub VoiceNumber(ByRef number As Long)
            Me.NNumber.Stop()
            Dim str As String = Conversion.Str(CLng(number))
            str = Strings.Mid(str, 2, (Strings.Len(str) - 1))
            Select Case (Strings.Len(str))
                Case 1
                    Me.v1(str)
                    Exit Select
                Case 2
                    Me.v2(str)
                    Exit Select
                Case 3
                    Me.v3(str)
                    Exit Select
                Case 4
                    Me.v4(str)
                    Exit Select
                Case 5
                    Me.v5(str)
                    Exit Select
                Case 6
                    Me.v6(str)
                    Exit Select
                Case 7
                    Me.v7(str)
                    Exit Select
                Case 8
                    Me.v8(str)
                    Exit Select
                Case 9
                    Me.v9(str)
                    Exit Select
            End Select
        End Sub

        Public Sub Won()
            If Not Me.HasKilledMouse Then
                Me.HasKilledMouse = True
            Else
                THConstVars.CannotDoKeydown = True
                Me.MuteSounds()
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = False
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.TeleportSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                Me.WonSound = modDirectShow.LoadMP3((DXSound.SoundPath & "\won.mp3"))
                Do While (Me.TeleportSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                    Application.DoEvents()
                Loop
                Me.FinalDuelSound.Stop()
                dVolume = "r11.wav"
                waitTillDone = True
                DXSound.Radio(dVolume, waitTillDone)
                Me.WonSound.Run()
                Interaction.MsgBox("Congratulations, you win!", MsgBoxStyle.Exclamation, "End")
                Me.ShutDown()
            End If
        End Sub

        Public Function getPageContent(url As String) As String
            Try
                If (webClient Is Nothing) Then
                    webClient = New WebClient()
                End If
                Dim output() As Byte = webClient.DownloadData(url)
                Dim Str As StringBuilder = New StringBuilder()
                For Each b As Byte In output
                    Str.Append(ChrW(b))
                Next b
                Return Str.ToString()
            Catch e As System.Net.WebException
                Return "failed"
            End Try
        End Function

        Private Sub updateTo(from As String, toVersion As String, comments As String)
            Me.Invoke(Sub()
                          Me.Text = "Downloading update, please wait..."
                          Me.ProgressBar1.Visible = True
                      End Sub
                      )
            Me.MenuSound = DXSound.LoadSound((DXSound.SoundPath & "\menus.wav"))
            Me.MenuSound.SetVolume(0)
            DXSound.PlaySound(Me.MenuSound, True, True, False, 0, 0, "", False)
            downloadUpdate("https://github.com/munawarb/Treasure-Hunt/archive/master.zip", "master.zip")
        End Sub

        Private Sub downloadUpdate(url As String, localPath As String)
            If (webClient Is Nothing) Then
                webClient = New WebClient()
            End If
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            AddHandler webClient.DownloadFileCompleted, New System.ComponentModel.AsyncCompletedEventHandler(AddressOf downloadComplete)
            AddHandler webClient.DownloadProgressChanged, New DownloadProgressChangedEventHandler(AddressOf progressUpdated)
            webClient.DownloadFileAsync(New Uri(url), localPath)
        End Sub

        Private Sub progressUpdated(sender As Object, e As DownloadProgressChangedEventArgs)
            Dim progressPercentage As Integer = e.BytesReceived / totalSize * 100
            System.Diagnostics.Debug.WriteLine(progressPercentage)
            If (progressPercentage <> Me.lastProgress) Then
                Me.lastProgress = progressPercentage
                ProgressBar1.Invoke(Sub() ProgressBar1.Value = Me.lastProgress)
            End If
        End Sub

        Private Sub downloadComplete(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
            ProgressBar1.Visible = False
            Me.MenuSound.Stop()
            If (Not e.Error Is Nothing) Then
                downloadError = True
            End If
            completedDownload = True
            If (Not downloadError) Then
                MessageBox.Show("The update download is complete. Click OK to begin installing. Treasure Hunt will restart once the update is complete.", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                System.Diagnostics.Process.Start("Updater.exe", "th.exe")
            Else
                MessageBox.Show("The update failed to download. Click OK to close the game.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            THF.F.ShutDown()
        End Sub


        Private Function isUpdating() As Boolean
            Dim updatever As String = getPageContent("https://raw.githubusercontent.com/munawarb/Treasure-Hunt/master/bin/Debug/version.dat")
            If (updatever.Equals("failed")) Then
                Return False
            End If
            Dim newVersion As Single = Single.Parse(updatever, CultureInfo.InvariantCulture.NumberFormat)
            Dim oldVersion As Single = Single.Parse(Me.AppVersion, CultureInfo.InvariantCulture.NumberFormat)
            If (oldVersion < newVersion) Then
                Dim download As DialogResult = MessageBox.Show("Treasure Hunt version " & newVersion & " is available. You are running version " & oldVersion & ". Would you like to download version " & newVersion & " now?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If (download = DialogResult.Yes) Then
                    updateTo(Me.AppVersion, updatever, Nothing)
                    Return True
                Else
                    Return False
                End If
            End If
            Return False
        End Function


        ' Fields
        Private components As IContainer
        Private Const SupplyRoomX As Integer = &H2E
        Private Const SupplyRoomY As Integer = 180
        Private Const LookRange As Integer = 20
        Private Const Subs As Integer = 9
        Private Const KeyTime As Single = 1.0!
        Private Const MGun As String = "gun"
        Private Const MSword As String = "sword"
        Private Const MBomb As String = "bombs"
        Private Const MLaser As String = "laser"
        Private Const string_0 As String = "gmissile"
        Private Const string_1 As String = "reflector"
        Private Const MControl As String = "control"
        Private Const PRange As Integer = 5
        Private Const SubLength As Single = 5.0!
        Private ClickSound As DirectSoundSecondaryBuffer8
        Private directSoundSecondaryBuffer8_0 As DirectSoundSecondaryBuffer8
        Private NLoadSuccessfulSound As DirectSoundSecondaryBuffer8
        Private NNumber As DirectSoundSecondaryBuffer8
        Private Const Def_Freq As Integer = &HAC44
        Private Const Min_Freq As Integer = &H5A3C
        Private Const Max_Freq As Integer = &H1831C
        Private Const Background_Def_Vol As Integer = -1600
        Private CurrentFreq As Integer
        Public CaughtSound As DirectSoundSecondaryBuffer8
        Public CheatFileSound As DirectSoundSecondaryBuffer8
        Private IntDialogueSound As IMediaControl
        Public CFootstepSound As DirectSoundSecondaryBuffer8
        Private NeedleLaunchSound As DirectSoundSecondaryBuffer8
        Private NeedleHitSound As DirectSoundSecondaryBuffer8
        Private ControlLaunchSound As DirectSoundSecondaryBuffer8
        Private ControlHitSound As DirectSoundSecondaryBuffer8
        Private LockDoorSound As DirectSoundSecondaryBuffer8
        Private UnlockDoorSound As DirectSoundSecondaryBuffer8
        Private ShieldSound As DirectSoundSecondaryBuffer8
        Private EmptySound As DirectSoundSecondaryBuffer8
        Private MenuSound As DirectSoundSecondaryBuffer8
        Public TargetSound As DirectSoundSecondaryBuffer8
        Public RadioSound As DirectSoundSecondaryBuffer8
        Public directSoundSecondaryBuffer8_1 As DirectSoundSecondaryBuffer8
        Private MachineSound As DirectSoundSecondaryBuffer8
        Private MachineTurnOffSound As DirectSoundSecondaryBuffer8
        Private MachineExplodeSound As DirectSoundSecondaryBuffer8
        Private BLastFightBeginSound As DirectSoundSecondaryBuffer8
        Private PassageSound As DirectSoundSecondaryBuffer8()
        Private DoorSound As DirectSoundSecondaryBuffer8()
        Private OpenDoorSound As DirectSoundSecondaryBuffer8
        Private CloseDoorSound As DirectSoundSecondaryBuffer8
        Public directSoundSecondaryBuffer8_2 As DirectSoundSecondaryBuffer8
        Private PickUpSWeaponSound As DirectSoundSecondaryBuffer8
        Private LaserGunSound As DirectSoundSecondaryBuffer8
        Private GLaunchSound As DirectSoundSecondaryBuffer8
        Public GetSound As DirectSoundSecondaryBuffer8
        Private TurnKnobSound As DirectSoundSecondaryBuffer8
        Private directSoundSecondaryBuffer8_3 As DirectSoundSecondaryBuffer8
        Private ReflectorSound As DirectSoundSecondaryBuffer8
        Private directSoundSecondaryBuffer8_4 As DirectSoundSecondaryBuffer8
        Private BPCLogoSound As DirectSoundSecondaryBuffer8
        Private BombBeepSound As DirectSoundSecondaryBuffer8
        Private BigBlastSound As DirectSoundSecondaryBuffer8
        Private BombAlarmSound As DirectSoundSecondaryBuffer8
        Private directSoundSecondaryBuffer8_5 As DirectSoundSecondaryBuffer8
        Private WonSound As IMediaControl
        Private directSoundSecondaryBuffer8_6 As DirectSoundSecondaryBuffer8
        Private GExplodeSound As DirectSoundSecondaryBuffer8
        Private DisChestSound As DirectSoundSecondaryBuffer8
        Private AccessDeniedSound As DirectSoundSecondaryBuffer8
        Private DestroyWallSound As DirectSoundSecondaryBuffer8
        Public BeepSound As DirectSoundSecondaryBuffer8
        Private VaultUpSound As DirectSoundSecondaryBuffer8
        Public BackToVaultSound As DirectSoundSecondaryBuffer8
        Private BreathSound As DirectSoundSecondaryBuffer8
        Public SwimSound As DirectSoundSecondaryBuffer8
        Public WaterSound As DirectSoundSecondaryBuffer8
        Public JumpInWaterSound As DirectSoundSecondaryBuffer8
        Public TalkToBrutusSound As DirectSoundSecondaryBuffer8
        Public KillBrutusSound As DirectSoundSecondaryBuffer8
        Private RegSound As DirectSoundSecondaryBuffer8
        Private IntroSound As IMediaControl
        Private directSoundSecondaryBuffer8_7 As DirectSoundSecondaryBuffer8
        Private LastFightBeginSound As DirectSoundSecondaryBuffer8
        Private WallCrashSound As DirectSoundSecondaryBuffer8
        Private WorkedPanelSound As DirectSoundSecondaryBuffer8
        Private PickUpHealthSound As DirectSoundSecondaryBuffer8
        Private AlarmSound As DirectSoundSecondaryBuffer8
        Public FinalDuelSound As DirectSoundSecondaryBuffer8
        Public BackgroundSound As DirectSoundSecondaryBuffer8
        Public TeleportSound As DirectSoundSecondaryBuffer8
        Public CharHitSound As DirectSoundSecondaryBuffer8
        Private GunSound As DirectSoundSecondaryBuffer8
        Public DuelSound As DirectSoundSecondaryBuffer8
        Private Footstep1Sound As DirectSoundSecondaryBuffer8
        Private Footstep2Sound As DirectSoundSecondaryBuffer8
        Public CharDieSound As DirectSoundSecondaryBuffer8
        Private SwingSwordSound As DirectSoundSecondaryBuffer8
        Public Treasure As Integer
        Public Wall As Integer
        Public Key As Integer
        Public ControlPanel As Integer
        Public h As Integer
        Public x As Integer = 400
        Public y As Integer = 400
        Private Grid(0 To Me.x, 0 To Me.y) As Integer
        Private BGrid(0 To Me.x + 1, 0 To Me.y + 1) As Integer
        Private CGrid(0 To Me.x + 1, 0 To Me.y + 1) As Single
        Private Swrd As Integer
        Private Bullets As Integer
        Private Accuracy As Integer
        Private IStuff As Integer
        Private stuff As String()
        Private KeyX As Integer
        Private KeyY As Integer
        Private PanelX As Integer
        Private PanelY As Integer
        Private WinX As Integer
        Private WinY As Integer
        Public px As Integer
        Public py As Integer
        Public ch As Integer
        Private lvl As Integer
        Private HasKey As Boolean
        Private WorkedPanel As Boolean
        Private bombs As Integer
        Public Points As Integer
        Private challs As chall()
        Public NumAlert As Integer
        Public ChallNum As Integer
        Public challenger As Integer
        Public isdone As Boolean
        Public AllCurrentFights As Integer
        Public HasCalledMore As Boolean
        Private TeleportsAreClosed As Boolean
        Public DetectRange As Integer
        Public IsOnAlarm As Boolean
        Public UnlimitedHealth As Boolean
        Public UnlimitedBullets As Boolean
        Public AppVersion As String
        Public HowMany As Integer
        Public HowManyNum As Integer
        Public IsFightingLast As Boolean
        Public RBomb As Integer
        Public IsFull As Boolean
        Private temp As Object
        Public IsInWater As Boolean
        Public WH As Integer
        Public WDepth As Integer
        Public IsResting As Boolean
        Public DidNotSwim As Integer
        Public Water As Integer
        Private WarnedOfHealth As Boolean
        Private WTime As String
        Private short_0 As Integer
        Private SubPX As Integer
        Private SubPY As Integer
        Private TX As Integer
        Private TY As Integer
        Public HasKilledBrutus As Boolean
        Private Weapons As String()
        Private WPos As Integer
        Private Laser As Integer
        Private GFront As Integer
        Public GX As Integer
        Public GY As Integer
        Private IsDoingGMissile As Boolean
        Private Missile As Integer
        Public short_1 As Integer
        Public RMissile As Integer
        Private GSpeed As Integer
        Private GCount As Integer
        Public Mine As Integer
        Private MineControl As Integer
        Private MineGuard As Integer
        Private MineX As Integer
        Private MineY As Integer
        Private BlewUpMineControl As Boolean
        Private NStop As Boolean
        Private ALaser As Integer
        Private short_2 As Integer
        Private Reflector As Integer
        Private short_3 As Integer
        Public DoingReflector As Boolean
        Private RCount As Integer
        Private TheString As String
        Private IsInPauseState As Boolean
        Private HasDoneInit As Boolean
        Public IsEscapingFromBomb As Boolean
        Private HasShutDownCard As Boolean
        Private HasBlownUpMachine As Boolean
        Private HasTurnedOffMachine As Boolean
        Private Machine As Integer
        Private MachineX As Integer
        Private MachineY As Integer
        Public Mouse As Integer
        Private ExitX As Integer
        Private ExitY As Integer
        Private JustCameFromWater As Boolean
        Private test As Integer
        Private DisableTeleports As Boolean
        Private PassageMarker As Integer
        Public ClosedDoor As Integer
        Public OpenDoor As Integer
        Private IsWaitingForMachine As Boolean
        Public HasKilledMouse As Boolean
        Public HasKilledBrutus2 As Boolean
        Public HasMainKey As Boolean
        Public LockedDoor As Integer
        Private SGen As Integer
        Private EBomb As Integer
        Private CMachine As Integer
        Public ChallAmount As Integer
        Private MenuPos As Integer
        Private IsInMenu As Boolean
        Public DefCaption As String
        Private FileName As String
        Private HasBeenA As Boolean
        Private HasBeenB As Boolean
        Private HasBeenC As Boolean
        Private CA As Integer
        Private C As Single()
        Public ChallWithKey As Integer
        Public JamesBrutus As Integer
        Public IsControling As Boolean
        Public SControl As Integer
        Private AControl As Integer
        Public WControl As Long
        Private controler As Integer
        Private IsLaunchingControl As Boolean
        Private DisControl As Integer
        Private DisNeedle As Integer
        Private IsLaunchingNeedle As Boolean
        Private WNeedle As Integer
        Private IsFirstTimeLoading As Boolean
        Private V As Integer
        ' How many milliseconds between each fire of this weapon if the fire button is being held down.
        Private fireRates As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer) From {{"gun", 100}, {"sword", 50}, {"bombs", 0}, {"laser", 500}, {"gmissile", 0}, {"reflector", 0}, {"control", 0}}
        Private fireDate As DateTime = DateTime.Now
        Private whichFootstep As Integer
        Private maxFootsteps As Integer = 2
        Private frameTime As Integer = 10 ' Number of ms per frame
        Dim movedInLastTick As Boolean = False ' Controls how fast the player can move, and also prevents the rapid-fire of remote missile commands
        Dim pressedActionButtonInLastTick As Boolean = False
        Private pendingKeyUp As Boolean ' In case keyUp fires before keyDown completes
        Private inKeyDown As Boolean
        Friend WithEvents ProgressBar1 As ProgressBar
        Private webClient As WebClient
        Private downloadError As Boolean
        Private completedDownload As Boolean
        Private lastProgress As Integer
        Private totalSize As Integer = 99047691
        Friend WithEvents Timer1 As Timer
        Private reflectorTime As Integer
        Private frameTracker As Integer = 0
        Private maxDepth As Integer = 30
        Private waterTracker As Integer
        Private needleTracker As Integer
        Private bombTracker As Integer
        Private machineTracker As Integer
        Private controlTracker As Integer
        Private tooCloseToMachineTracker As Integer
        Private controlLaunchTracker As Integer
        ' The directions the player can swim in
        Enum SwimDirection
            north
            south
            east
            west
            up
            down
        End Enum
        ' All the durections the player can move in
        Enum WalkDirection
            north
            south
            east
            west
        End Enum
        ' All the things we can do with the background music
        Enum VolumeAction
            up
            down
            mute
            unmute
        End Enum
        ' Everything we can do with a remote controlled missile
        Enum MissileAction
            turnLeft
            turnRight
            speedUp
            slowDown
        End Enum
        ' How speech rate can be changed
        Enum SpeechRate
            quicker
            slower
            normal
        End Enum

    End Class
End Namespace

