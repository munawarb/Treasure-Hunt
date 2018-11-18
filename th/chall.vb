Imports DxVBLibA
Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace th
    Friend Class chall
        ' Methods
        Public Sub Activate()
            Dim flag4 As Boolean
            If (THF.F.IsFightingLast Or (THF.F.NumAlert = 0)) Then
                THF.F.DuelSound.Stop
                THF.F.DuelSound.SetCurrentPosition(0)
                If Not THF.F.IsFightingLast Then
                    Dim bCloseFirst As Boolean = False
                    Dim bLoopSound As Boolean = True
                    Dim performEffects As Boolean = False
                    Dim x As Integer = 0
                    Dim y As Integer = 0
                    Dim dVolume As String = ""
                    flag4 = False
                    DXSound.PlaySound(THF.F.BackgroundSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, flag4)
                End If
            End If
            If (Me.A > 0) Then
                If (Me.HasLoadedSounds AndAlso (Me.IsDoingBomb And (Me.BombLaunchSound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING))) Then
                    Me.ThrowBomb
                End If
                Me.DoIfBomb
                Me.DoIfGRMissile
                Me.DoIfGLMissile
                Me.DoIfClosedDoor
                Me.DoIfPoisoned
                If Not Me.IsBeingControled Then
                    Me.ShowChall
                End If
                If Not Me.IsBeingControled Then
                    If ((Me.x = THF.F.px) And (Me.y = THF.F.py)) Then
                        THF.F.ChallNum = Me.PosNum
                    End If
                    If Me.ScanForChar(THF.F.DetectRange) Then
                        If (Me.TCount >= Me.HSeconds) Then
                            Me.TCount = 0
                            Me.hit
                        Else
                            Me.TCount = ((Me.TCount + 1))
                        End If
                    ElseIf (Me.SCount >= Me.CSeconds) Then
                        Me.SCount = 0
                        If (Me.method_0 = 2) Then
                            Me.hit
                        Else
                            Me.Move
                        End If
                    Else
                        Me.SCount = ((Me.SCount + 1))
                    End If
                End If
            Else
                Me.ChallDied
                If Not Me.HasUnloadedSounds Then
                    Me.UnloadSounds
                End If
            End If
        End Sub

        Public Sub ChallDied()
            If Not Me.IsDeadValid Then
                Dim f As mainFRM
                Dim flag As Boolean
                Dim flag2 As Boolean
                Dim flag3 As Boolean
                Dim num As Integer
                Dim num2 As Integer
                Dim str As String
                Dim flag4 As Boolean
                Me.IsBeingControled = False
                If (Me.IsFighting And Not Me.IsMaster) Then
                    f = THF.F
                    f.NumAlert -= 1
                    If (THF.F.NumAlert = 0) Then
                        THF.F.HasCalledMore = False
                        If Not THF.F.IsFightingLast Then
                            THF.F.DetectRange = 1
                            flag = True
                            flag2 = True
                            flag3 = False
                            num = 0
                            num2 = 0
                            str = ""
                            flag4 = False
                            DXSound.PlaySound(THF.F.BackgroundSound, flag, flag2, flag3, num, num2, str, flag4)
                            THF.F.DuelSound.Stop
                            THF.F.DuelSound.SetCurrentPosition(0)
                        End If
                    End If
                End If
                If Not Me.IsMaster Then
                    If (Me.ChallDieSound Is Nothing) Then
                        Me.ChallDieSound = DXSound.smethod_0((DXSound.SoundPath & "\challdie.wav"), 2.0!, 30)
                    End If
                    num2 = 0
                    DXSound.smethod_1(Me.ChallDieSound, True, False, Me.x, Me.y, num2)
                Else
                    THConstVars.CannotDoKeydown = True
                    THF.F.TargetSound.Stop
                    flag4 = True
                    flag3 = False
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(Me.ChallDieSound, flag4, flag3, flag2, num2, num, str, flag)
                End If
                If (Me.method_0 = 2) Then
                    THConstVars.CannotDoKeydown = True
                    Do While (Me.ChallDieSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                        Application.DoEvents
                    Loop
                    If Not THF.F.HasKilledBrutus Then
                        THF.F.HasKilledBrutus = True
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(THF.F.KillBrutusSound, flag4, flag3, flag2, num2, num, str, flag)
                        Do While (THF.F.KillBrutusSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                            Application.DoEvents
                        Loop
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num2 = 0
                        num = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(THF.F.BackToVaultSound, flag4, flag3, flag2, num2, num, str, flag)
                        Do While (THF.F.BackToVaultSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                            Application.DoEvents
                        Loop
                        Me.ReturnToGame
                        flag4 = False
                        THF.F.DoIfDemo(flag4)
                        Return
                    End If
                    If (Not THF.F.HasKilledBrutus2 And THF.F.HasKilledBrutus) Then
                        THF.F.HasKilledBrutus2 = True
                        Me.ReturnToGame
                        Return
                    End If
                    If (Not THF.F.HasKilledMouse And THF.F.HasKilledBrutus2) Then
                        THF.F.HasKilledMouse = True
                        THF.F.Won
                    End If
                End If
                Me.IsDeadValid = True
                f = THF.F
                f.Points = CInt(Math.Round(CDbl((f.Points + (1! + Conversion.Int(CSng((10! * VBMath.Rnd))))))))
                f = THF.F
                f.ch += 1
                If ((THF.F.ch = 250) AndAlso THF.F.IsFull) Then
                    FileSystem.FileOpen(1, (Addendums.FilePath & "\cheats.txt"), OpenMode.Output, OpenAccess.Default, OpenShare.Default, -1)
                    FileSystem.PrintLine(1, New Object() { "----------" })
                    FileSystem.PrintLine(1, New Object() { ("Cheats for Treasure Hunt " & THF.F.AppVersion & ".") })
                    FileSystem.PrintLine(1, New Object() { "NOTE: All cheats must be entered by pressing P at the control panel." })
                    FileSystem.PrintLine(1, New Object() { "Unlimited bullets: unlimit my bullets now!" })
                    FileSystem.PrintLine(1, New Object() { "Unlimited health: unlimit my health now!" })
                    FileSystem.PrintLine(1, New Object() { "Get all special weapons: I want all weapons!" })
                    FileSystem.FileClose(New Integer() { 1 })
                    THConstVars.CannotDoKeydown = True
                    THF.F.MuteSounds
                    THF.F.CheatFileSound = DXSound.LoadSound((DXSound.SoundPath & "\cheatfile.wav"))
                    flag4 = True
                    flag3 = False
                    flag2 = False
                    num2 = 0
                    num = 0
                    str = ""
                    flag = False
                    DXSound.PlaySound(THF.F.CheatFileSound, flag4, flag3, flag2, num2, num, str, flag)
                    Do While (THF.F.CheatFileSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                        Application.DoEvents
                    Loop
                    THF.F.CheatFileSound = Nothing
                    THF.F.UnmuteSounds
                    THConstVars.CannotDoKeydown = False
                End If
                Me.IsFighting = False
                THF.F.ThingReplace(Me.x, Me.y, THF.F.GetBGrid(Me.x, Me.y))
                If Me.HasKey Then
                    THF.F.HasMainKey = True
                    str = "r2.wav"
                    flag4 = False
                    DXSound.Radio(str, flag4)
                End If
            End If
        End Sub

        Public Sub ChallHit(ByRef subtract As Integer)
            If (Me.A > 0) Then
                Me.LoadSounds
                Me.A = ((Me.A - subtract))
                If (Me.A <= 0) Then
                    Me.ChallDied
                Else
                    If (Me.challHitSound Is Nothing) Then
                        Me.challHitSound = DXSound.smethod_0((DXSound.SoundPath & "\challgrunt.wav"), 2.0!, 30)
                    End If
                    Dim z As Integer = 0
                    DXSound.smethod_1(Me.challHitSound, True, False, Me.x, Me.y, z)
                End If
            End If
        End Sub

        Public Sub ControlChall(ByVal shift As Long, ByVal keyCode As Long)
            If Not Me.HasLoadedSounds Then
                Me.LoadSounds
            End If
            If (Me.A <= 0) Then
                Me.ChallDied
            Else
                If (shift <> &H10000) Then
                    Dim flag As Boolean
                    Dim num As Integer
                    THF.F.ThingReplace(Me.x, Me.y, THF.F.GetBGrid(Me.x, Me.y))
                    If (keyCode = &H25) Then
                        num = ((Me.x - 1))
                        If Not Me.GetBlock(num, Me.y) Then
                            Me.x = ((Me.x - 1))
                            flag = True
                        End If
                    End If
                    If (keyCode = &H27) Then
                        num = ((Me.x + 1))
                        If Not Me.GetBlock(num, Me.y) Then
                            Me.x = ((Me.x + 1))
                            flag = True
                        End If
                    End If
                    If (keyCode = &H26) Then
                        num = ((Me.y + 1))
                        If Not Me.GetBlock(Me.x, num) Then
                            Me.y = ((Me.y + 1))
                            flag = True
                        End If
                    End If
                    If (keyCode = 40) Then
                        num = ((Me.y - 1))
                        If Not Me.GetBlock(Me.x, num) Then
                            Me.y = ((Me.y - 1))
                            flag = True
                        End If
                    End If
                    If flag Then
                        num = 0
                        DXSound.smethod_1(THF.F.CFootstepSound, True, False, Me.x, Me.y, num)
                        Return
                    End If
                End If
                If (keyCode = &H20) Then
                    Me.hit
                End If
                If (keyCode = &H52) Then
                    Me.x = THF.F.px
                    Me.y = THF.F.py
                End If
                If (keyCode = 13) Then
                    Me.DoDoor(Me.x, Me.y)
                End If
            End If
        End Sub

        Private Sub DetectCharForHit()
            If Me.ScanForChar(THF.F.DetectRange) Then
                Me.hit
            End If
        End Sub

        Private Sub DoDoor(ByRef x As Integer, ByRef y As Integer)
            Dim num2 As Integer
            Dim num3 As Integer
            Select Case Me.GetDoor(((x - 1)), y)
                Case 1!
                    If Not Me.LDoor Then
                        Me.OpenDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\opendoor.wav"), 2!, 7)
                    End If
                    Me.LDoor = True
                    num2 = ((x - 1))
                    num3 = 0
                    DXSound.smethod_1(Me.OpenDoorSound, True, False, num2, y, num3)
                    Me.XDir = 0
                    num3 = ((x - 1))
                    THF.F.AllThingReplace(num3, y, THF.F.OpenDoor)
                    If Not Me.IsBeingControled Then
                        x = ((x - 1))
                    End If
                    Exit Select
                Case 2!
                    If Not Me.LCDoor Then
                        Me.LockedDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\knob.wav"), 2!, 7)
                    End If
                    Me.LCDoor = True
                    num3 = 0
                    DXSound.smethod_1(Me.LockedDoorSound, True, False, x, y, num3)
                    Exit Select
            End Select
            Select Case Me.GetDoor(((x + 1)), y)
                Case 1!
                    If Not Me.LDoor Then
                        Me.OpenDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\opendoor.wav"), 2!, 7)
                    End If
                    Me.LDoor = True
                    num3 = ((x + 1))
                    num2 = 0
                    DXSound.smethod_1(Me.OpenDoorSound, True, False, num3, y, num2)
                    Me.XDir = 1
                    num3 = ((x + 1))
                    THF.F.AllThingReplace(num3, y, THF.F.OpenDoor)
                    If Not Me.IsBeingControled Then
                        x = ((x + 1))
                    End If
                    Exit Select
                Case 2!
                    If Not Me.LCDoor Then
                        Me.LockedDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\knob.wav"), 2!, 7)
                    End If
                    Me.LCDoor = True
                    num3 = 0
                    DXSound.smethod_1(Me.LockedDoorSound, True, False, x, y, num3)
                    Exit Select
            End Select
            Select Case Me.GetDoor(x, ((y + 1)))
                Case 1!
                    If Not Me.LDoor Then
                        Me.OpenDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\opendoor.wav"), 2!, 7)
                    End If
                    Me.LDoor = True
                    num3 = ((y + 1))
                    num2 = 0
                    DXSound.smethod_1(Me.OpenDoorSound, True, False, x, num3, num2)
                    Me.MDir = 0
                    num3 = ((y + 1))
                    THF.F.AllThingReplace(x, num3, THF.F.OpenDoor)
                    If Not Me.IsBeingControled Then
                        y = ((y + 1))
                    End If
                    Exit Select
                Case 2!
                    If Not Me.LCDoor Then
                        Me.LockedDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\knob.wav"), 2!, 7)
                    End If
                    Me.LCDoor = True
                    num3 = 0
                    DXSound.smethod_1(Me.LockedDoorSound, True, False, x, y, num3)
                    Exit Select
            End Select
            Select Case Me.GetDoor(x, ((y - 1)))
                Case 1!
                    If Not Me.LDoor Then
                        Me.OpenDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\opendoor.wav"), 2!, 7)
                    End If
                    Me.LDoor = True
                    num3 = ((y - 1))
                    num2 = 0
                    DXSound.smethod_1(Me.OpenDoorSound, True, False, x, num3, num2)
                    Me.MDir = 1
                    num3 = ((y - 1))
                    THF.F.AllThingReplace(x, num3, THF.F.OpenDoor)
                    If Not Me.IsBeingControled Then
                        y = ((y - 1))
                    End If
                    Exit Select
                Case 2!
                    If Not Me.LCDoor Then
                        Me.LockedDoorSound = DXSound.smethod_0((DXSound.SoundPath & "\knob.wav"), 2!, 7)
                    End If
                    Me.LCDoor = True
                    num3 = 0
                    DXSound.smethod_1(Me.LockedDoorSound, True, False, x, y, num3)
                    Exit Select
            End Select
        End Sub

        Private Sub DoIfBomb()
            If (THF.F.GetGrid(Me.x, Me.y) = THF.F.RBomb) Then
                Me.LoadSounds
                Me.A = (Math.Round(CDbl((Me.A - (1! + Conversion.Int(CSng((50! * VBMath.Rnd))))))))
                If (Me.A <= 0) Then
                    Me.ChallDied
                    Return
                End If
                If (Me.challHitSound Is Nothing) Then
                    Me.challHitSound = DXSound.smethod_0((DXSound.SoundPath & "\challgrunt.wav"), 2.0!, 30)
                End If
                Dim z As Integer = 0
                DXSound.smethod_1(Me.challHitSound, True, False, Me.x, Me.y, z)
                Me.ShowChall
            End If
            THF.F.ThingReplace(Me.x, Me.y, THF.F.GetBGrid(Me.x, Me.y))
        End Sub

        Private Sub DoIfClosedDoor()
            If ((THF.F.GetBGrid(Me.x, Me.y) = THF.F.ClosedDoor) Or (THF.F.GetBGrid(Me.x, Me.y) = THF.F.LockedDoor)) Then
                Me.LoadSounds
                Me.A = 0
                Me.ChallDied
                THF.F.ThingReplace(Me.x, Me.y, THF.F.GetBGrid(Me.x, Me.y))
            End If
        End Sub

        Private Sub DoIfGLMissile()
            If ((Not THF.F.HasKilledBrutus2 AndAlso ((Me.x = THF.F.GX) And (Me.y = THF.F.GY))) AndAlso ((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))) = 2!)) Then
                Me.LoadSounds()
                If (Me.challGunSound Is Nothing) Then
                    Me.challGunSound = DXSound.smethod_0((DXSound.SoundPath & "\challgun.wav"), 2.0!, 30)
                End If
                Dim bCloseFirst As Boolean = True
                Dim bLoopSound As Boolean = False
                Dim performEffects As Boolean = False
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim dVolume As String = ""
                Dim waitTillDone As Boolean = False
                DXSound.PlaySound(Me.challGunSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
                THF.F.GExplodeMissile
                Me.A = 0
                Me.ChallDied
            End If
        End Sub

        Private Sub DoIfGRMissile()
            If (THF.F.GetGrid(Me.x, Me.y) = THF.F.RMissile) Then
                Me.LoadSounds
                Dim subtract As Integer = (Math.Round(CDbl((1! + Conversion.Int(CSng((80! * VBMath.Rnd)))))))
                Me.ChallHit(subtract)
                THF.F.ThingReplace(Me.x, Me.y, THF.F.GetBGrid(Me.x, Me.y))
            End If
        End Sub

        Private Sub DoIfPoisoned()
            If Me.IsPoisoned Then
                If Me.IsMaster Then
                    Me.A = ((Me.A - 5))
                Else
                    Me.A = (Math.Round(CDbl((Me.A - ((1! + Conversion.Int(CSng((10! * VBMath.Rnd)))) * (1! + Conversion.Int(CSng((Me.NumOfNeedles * VBMath.Rnd)))))))))
                End If
            End If
        End Sub

        Private Function GetBlock(ByRef x As Integer, ByRef y As Integer) As Boolean
            Dim flag As Boolean
            If ((((x < 1) Or (x > THF.F.x)) Or (y < 1)) Or (y > THF.F.y)) Then
                Return True
            End If
            Dim bGrid As Integer = THF.F.GetBGrid(x, y)
            If Not (((((bGrid <> THF.F.Mine) And (bGrid <> THF.F.Water)) And (bGrid <> THF.F.ClosedDoor)) And (bGrid <> THF.F.LockedDoor)) And (bGrid <> THF.F.Wall)) Then
                flag = True
            End If
            Return flag
        End Function

        Private Function GetDoor(ByVal x As Integer, ByVal y As Integer) As Single
            Dim num As Single
            If (THF.F.GetBGrid(x, y) = THF.F.OpenDoor) Then
                Return 0!
            End If
            If (THF.F.GetBGrid(x, y) = THF.F.ClosedDoor) Then
                Return 1!
            End If
            If (THF.F.GetBGrid(x, y) = THF.F.LockedDoor) Then
                num = 2!
            End If
            Return num
        End Function

        Public Sub GetReadyForControl()
            If Not Me.HasLoadedSounds Then
                Me.LoadSounds
            End If
            If (Me.IsFighting And Not Me.IsMaster) Then
                Me.IsFighting = False
                Dim f As mainFRM = THF.F
                f.NumAlert -= 1
            End If
            Me.IsBeingControled = True
        End Sub

        Private Sub GetWhere(ByRef TheX As Integer, ByRef TheY As Integer)
            If (THF.F.px > Me.x) Then
                TheX = -1
            End If
            If (THF.F.px = Me.x) Then
                TheX = 0
            End If
            If (THF.F.px < Me.x) Then
                TheX = 1
            End If
            If (THF.F.py > Me.y) Then
                TheY = -1
            End If
            If (THF.F.py = Me.y) Then
                TheY = 0
            End If
            If (THF.F.py < Me.y) Then
                TheY = 1
            End If
        End Sub

        Private Sub hit()
            Dim num4 As Integer
            Dim str As String
            Me.LoadSounds
            If Not Me.IsBeingControled Then
                If (Me.method_0 = 2) Then
                    If (((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))) = 2!) Or (Me.LCount > 1)) Then
                        Me.LCount = 1
                        Me.MoveWhileFight
                    Else
                        Me.LCount = ((Me.LCount + 1))
                    End If
                End If
                If Not ((Me.method_0 = 0) Or (Me.method_0 = 1)) Then
                    If (Me.method_0 = 2) Then
                        If Not Me.ScanForChar(Me.LRange) Then
                            Me.x = THF.F.px
                            Me.y = THF.F.py
                            If ((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))) = 2!) Then
                                num4 = 0
                                DXSound.smethod_1(Me.BAttemptEscape2Sound, True, False, Me.x, Me.y, num4)
                            Else
                                num4 = 0
                                DXSound.smethod_1(Me.BAttemptEscape1Sound, True, False, Me.x, Me.y, num4)
                            End If
                        End If
                        Me.IsFighting = True
                    End If
                ElseIf Not Me.IsFighting Then
                    Dim num3 As Integer
                    If ((THConstVars.Difficulty <> 4!) And Not Me.HasFoundOnce) Then
                        num3 = (Math.Round(CDbl((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))))))
                    Else
                        num3 = 2
                    End If
                    If (num3 = 1) Then
                        If (Me.RedAlertSound Is Nothing) Then
                            Me.RedAlertSound = DXSound.smethod_0((DXSound.SoundPath & "\redalert.wav"), 2.0!, 30)
                        End If
                        num4 = 0
                        DXSound.smethod_1(Me.RedAlertSound, True, False, Me.x, Me.y, num4)
                    Else
                        If Not Me.IsDeadValid Then
                            Dim f As mainFRM = THF.F
                            f.NumAlert += 1
                        End If
                        If Not Me.IsDeadValid Then
                            Me.IsFighting = True
                        End If
                        If (THF.F.CaughtSound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING) Then
                            num4 = 0
                            DXSound.smethod_1(THF.F.CaughtSound, True, False, Me.x, Me.y, num4)
                        End If
                        If (Not THF.F.HasCalledMore AndAlso (((((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))) = 2!) Or (THConstVars.Difficulty = 3!)) Or (THConstVars.Difficulty = 4!)) Or Me.HasFoundOnce)) Then
                            If ((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))) = 2!) Then
                                str = "r17.wav"
                                Dim rWait As Boolean = False
                                DXSound.Radio(str, rWait)
                            End If
                            THF.F.HowMany = (Math.Round(CDbl((2! * THConstVars.Difficulty))))
                            If Not Me.HasFoundOnce Then
                                Select Case (1! + Conversion.Int(CSng((4! * VBMath.Rnd))))
                                    Case 1!
                                        Me.Caught1Sound = DXSound.smethod_0((DXSound.SoundPath & "\caught1.wav"), 2!, 30)
                                        num4 = 0
                                        DXSound.smethod_1(Me.Caught1Sound, True, False, Me.x, Me.y, num4)
                                        Exit Select
                                    Case 2!
                                        Me.Caught2Sound = DXSound.smethod_0((DXSound.SoundPath & "\caught2.wav"), 2!, 30)
                                        num4 = 0
                                        DXSound.smethod_1(Me.Caught2Sound, True, False, Me.x, Me.y, num4)
                                        Exit Select
                                    Case 3!
                                        Me.Caught3Sound = DXSound.smethod_0((DXSound.SoundPath & "\caught3.wav"), 2!, 30)
                                        num4 = 0
                                        DXSound.smethod_1(Me.Caught3Sound, True, False, Me.x, Me.y, num4)
                                        Exit Select
                                    Case 4!
                                        Me.Caught4Sound = DXSound.smethod_0((DXSound.SoundPath & "\caught4.wav"), 2!, 30)
                                        num4 = 0
                                        DXSound.smethod_1(Me.Caught4Sound, True, False, Me.x, Me.y, num4)
                                        Exit Select
                                End Select
                            Else
                                Me.Caught5Sound = DXSound.smethod_0((DXSound.SoundPath & "\caught5.wav"), 2!, 30)
                                num4 = 0
                                DXSound.smethod_1(Me.Caught5Sound, True, False, Me.x, Me.y, num4)
                            End If
                            THF.F.HasCalledMore = True
                            Dim challAmount As Integer = THF.F.ChallAmount
                            Dim i As Integer = 1
                            Do While (i <= challAmount)
                                THF.F.GoFight(i)
                                Application.DoEvents
                                i = ((i + 1))
                            Loop
                            THF.F.HowManyNum = 0
                        End If
                    End If
                End If
            End If
            If (Me.IsFighting Or Me.IsBeingControled) Then
                Dim num As Integer
                THF.F.ChallNum = Me.PosNum
                If (Not Me.IsBeingControled AndAlso Not Me.IsDoingBomb) Then
                    If (((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))) = 1!) Or (Me.BCount > 1)) Then
                        Me.BCount = 1
                        str = ""
                        DXSound.LocaleNotify(Me.x, Me.y, str)
                        Me.ThrowBomb
                        Return
                    End If
                    Me.BCount = ((Me.BCount + 1))
                End If
                str = ""
                DXSound.LocaleNotify(Me.x, Me.y, str)
                If (Me.challGunSound Is Nothing) Then
                    Me.challGunSound = DXSound.smethod_0((DXSound.SoundPath & "\challgun.wav"), 2.0!, 30)
                End If
                num4 = 0
                DXSound.smethod_1(Me.challGunSound, True, False, Me.x, Me.y, num4)
                If (Me.method_0 = 2) Then
                    If (THF.F.HasKilledBrutus And THF.F.HasKilledBrutus2) Then
                        num = (Math.Round(CDbl((Conversion.Int(CSng((VBMath.Rnd(1!) * 20!))) + 1!))))
                    Else
                        num = (Math.Round(CDbl((Conversion.Int(CSng((VBMath.Rnd(1!) * 150!))) + 1!))))
                    End If
                Else
                    num = (Math.Round(CDbl((Conversion.Int(CSng((VBMath.Rnd(1!) * 50!))) + 1!))))
                End If
                If Not Me.IsBeingControled Then
                    THF.F.ReflectHit(num, Me.PosNum)
                Else
                    THF.F.HarmChall(num, Me.x, Me.y)
                End If
            End If
        End Sub

        Public Sub init(ByRef px As Integer, ByRef py As Integer, ByRef p As Integer, ByRef CHealth As Integer, ByRef s As Integer, ByRef t As Integer, ByRef Optional dead As Boolean = False)
            Me.CSeconds = s
            Me.HSeconds = t
            Me.IsDeadValid = dead
            Me.A = CHealth
            Me.x = px
            Me.y = py
            If (p = 5) Then
                Me.HasKey = True
                Me.x = 40
                Me.y = &HB2
            End If
            Me.PosNum = p
            Me.ShowChall
        End Sub

        Public Sub LoadSounds()
            If Not Me.HasLoadedSounds Then
                THConstVars.CannotDoKeydown = True
                Me.BombLaunchSound = DXSound.smethod_0((DXSound.SoundPath & "\launch.wav"), 2!, 30)
                THConstVars.CannotDoKeydown = False
                Me.HasLoadedSounds = True
                Me.HasUnloadedSounds = False
            End If
        End Sub

        Private Function method_0() As Integer
            Dim num As Integer
            If Not THF.F.IsFightingLast Then
                Return 0
            End If
            If (Not Me.IsMaster And THF.F.IsFightingLast) Then
                Return 1
            End If
            If (Me.IsMaster And THF.F.IsFightingLast) Then
                num = 2
            End If
            Return num
        End Function

        Private Sub Move()
            Dim num As Integer
            Dim num2 As Integer
            If Me.IsFighting Then
                If (Me.run >= (3! * THConstVars.Difficulty)) Then
                    Dim f As mainFRM = THF.F
                    f.NumAlert -= 1
                    Me.HasFoundOnce = True
                    If ((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))) = 2!) Then
                        Me.LostSound = DXSound.smethod_0((DXSound.SoundPath & "\lost.wav"), 2!, 30)
                        num = 0
                        DXSound.smethod_1(Me.LostSound, True, False, Me.x, Me.y, num)
                    End If
                    If (THF.F.NumAlert = 0) Then
                        If Not THF.F.IsFightingLast Then
                            THF.F.DetectRange = 1
                            Dim bCloseFirst As Boolean = True
                            Dim bLoopSound As Boolean = True
                            Dim performEffects As Boolean = False
                            num = 0
                            num2 = 0
                            Dim dVolume As String = ""
                            Dim waitTillDone As Boolean = False
                            DXSound.PlaySound(THF.F.BackgroundSound, bCloseFirst, bLoopSound, performEffects, num, num2, dVolume, waitTillDone)
                            THF.F.DuelSound.Stop
                            THF.F.DuelSound.SetCurrentPosition(0)
                        End If
                        THF.F.HasCalledMore = False
                    End If
                    Me.IsFighting = False
                    Me.run = 0
                Else
                    Me.run = ((Me.run + 1))
                End If
            End If
            THF.F.ThingReplace(Me.x, Me.y, THF.F.GetBGrid(Me.x, Me.y))
            Me.DoDoor(Me.x, Me.y)
            If ((Me.PosNum <> 5) And ((((THConstVars.Difficulty = 4!) Or ((THF.F.NumAlert > 0) And ((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))) = 2!))) Or Me.HasFoundOnce) Or ((1! + Conversion.Int(CSng((2! * VBMath.Rnd)))) = 2!))) Then
                If (Me.x < (THF.F.x - 1)) Then
                    num2 = ((Me.x + 1))
                    If Me.GetBlock(num2, Me.y) Then
                        Me.XDir = 0
                    End If
                Else
                    Me.XDir = 0
                End If
                If (Me.x > 2) Then
                    num2 = ((Me.x - 1))
                    If Me.GetBlock(num2, Me.y) Then
                        Me.XDir = 1
                    End If
                Else
                    Me.XDir = 1
                End If
                num2 = ((Me.x + 1))
                num = ((Me.x - 1))
                If (Not Me.GetBlock(num2, Me.y) And Not Me.GetBlock(num, Me.y)) Then
                    If (Me.XDir = 0!) Then
                        Me.x = ((Me.x - 1))
                    End If
                    If (Me.XDir = 1!) Then
                        Me.x = ((Me.x + 1))
                    End If
                End If
            End If
            If (Me.y < (THF.F.y - 1)) Then
                num2 = ((Me.y + 1))
                If Me.GetBlock(Me.x, num2) Then
                    Me.MDir = 1
                End If
            Else
                Me.MDir = 1
            End If
            If (Me.y > 2) Then
                num2 = ((Me.y - 1))
                If Me.GetBlock(Me.x, num2) Then
                    Me.MDir = 0
                End If
            Else
                Me.MDir = 0
            End If
            num2 = ((Me.y + 1))
            num = ((Me.y - 1))
            If Not (Me.GetBlock(Me.x, num2) And Me.GetBlock(Me.x, num)) Then
                If (Me.MDir = 1!) Then
                    Me.y = ((Me.y - 1))
                Else
                    Me.y = ((Me.y + 1))
                End If
            End If
            Me.ShowChall
        End Sub

        Private Sub MoveWhileFight()
            Dim num As Integer
            THF.F.ThingReplace(Me.x, Me.y, THF.F.GetBGrid(Me.x, Me.y))
            Select Case (Math.Round(CDbl((1! + Conversion.Int(CSng((4! * VBMath.Rnd)))))))
                Case 1
                    If ((THF.F.px + 1) <= THF.F.x) Then
                        num = ((THF.F.px + 1))
                        If Not Me.GetBlock(num, Me.y) Then
                            Me.x = ((THF.F.px + 1))
                        End If
                    End If
                    Exit Select
                Case 2
                    If ((THF.F.px - 1) >= 1) Then
                        num = ((THF.F.px - 1))
                        If Not Me.GetBlock(num, Me.y) Then
                            Me.x = ((THF.F.px - 1))
                        End If
                    End If
                    Exit Select
                Case 3
                    If ((THF.F.py + 1) <= THF.F.y) Then
                        num = ((THF.F.py + 1))
                        If Not Me.GetBlock(Me.x, num) Then
                            Me.y = ((THF.F.py + 1))
                        End If
                    End If
                    Exit Select
                Case 4
                    If ((THF.F.py - 1) >= 1) Then
                        num = ((THF.F.py - 1))
                        If Not Me.GetBlock(Me.x, num) Then
                            Me.y = ((THF.F.py - 1))
                        End If
                    End If
                    Exit Select
            End Select
            Me.ShowChall
            num = 0
            DXSound.smethod_1(Me.directSoundSecondaryBuffer8_0, True, False, Me.x, Me.y, num)
        End Sub

        Private Sub ReturnToGame()
            Me.IsFighting = False
            Me.IsDeadValid = True
            Dim bCloseFirst As Boolean = True
            Dim bLoopSound As Boolean = False
            Dim performEffects As Boolean = False
            Dim x As Integer = 0
            Dim y As Integer = 0
            Dim dVolume As String = ""
            Dim waitTillDone As Boolean = False
            DXSound.PlaySound(THF.F.TeleportSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
            Do While (THF.F.TeleportSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                Application.DoEvents
            Loop
            THF.F.px = 3
            THF.F.py = 130
            THF.F.IsFightingLast = False
            THConstVars.CannotDoKeydown = False
            Me.IsMaster = False
        End Sub

        Private Function ScanForChar(ByRef Range As Integer) As Boolean
            Dim flag As Boolean
            If ((THF.F.WDepth < 10) AndAlso ((Math.Abs(((THF.F.px - Me.x))) <= Range) And (Math.Abs(((THF.F.py - Me.y))) <= Range))) Then
                flag = True
            End If
            Return flag
        End Function

        Private Sub SetSoundPosition(ByRef TheSound As DirectSoundSecondaryBuffer8, ByRef DVolume As Boolean)
            Dim num As Integer
            Dim num2 As Integer
            Dim num3 As Integer
            Dim flag As Boolean
            Dim flag2 As Boolean
            Dim flag3 As Boolean
            Dim num4 As Integer
            Dim num5 As Integer
            Dim str As String
            Dim flag4 As Boolean
            Me.GetWhere(num2, num3)
            If (Math.Abs(((THF.F.px - Me.x))) > 10) Then
                num = &H2710
            Else
                num = (Math.Abs(((THF.F.px - Me.x))) * &H3E8)
            End If
            Select Case num2
                Case -1
                    TheSound.SetPan((0 - num))
                    Exit Select
                Case 0
                    TheSound.SetPan(0)
                    Exit Select
                Case 1
                    TheSound.SetPan(num)
                    Exit Select
            End Select
            If (Math.Abs(((THF.F.py - Me.y))) > Math.Abs(((THF.F.px - Me.x)))) Then
                If (Math.Abs(((THF.F.py - Me.y))) > 20) Then
                    num = &H2710
                Else
                    num = (Math.Abs(((THF.F.py - Me.y))) * 100)
                End If
            End If
            If ((Math.Abs(((THF.F.px - Me.x))) > Math.Abs(((THF.F.py - Me.y)))) Or (Math.Abs(((THF.F.px - Me.x))) = Math.Abs(((THF.F.py - Me.y))))) Then
                If (Math.Abs(((THF.F.px - Me.x))) > 20) Then
                    num = &H2710
                Else
                    num = (Math.Abs(((THF.F.px - Me.x))) * 100)
                End If
            End If
            TheSound.SetVolume((0 - num))
            If (num3 = 1) Then
                THF.F.BeepSound.SetPan(TheSound.GetPan)
                If DVolume Then
                    THF.F.BeepSound.SetVolume((0 - num))
                Else
                    THF.F.BeepSound.SetVolume(0)
                End If
                flag = True
                flag2 = False
                flag3 = False
                num4 = 0
                num5 = 0
                str = ""
                flag4 = False
                DXSound.PlaySound(THF.F.BeepSound, flag, flag2, flag3, num4, num5, str, flag4)
            End If
            If (num3 = -1) Then
                THF.F.directSoundSecondaryBuffer8_2.SetPan(TheSound.GetPan)
                If DVolume Then
                    THF.F.directSoundSecondaryBuffer8_2.SetVolume((0 - num))
                Else
                    THF.F.directSoundSecondaryBuffer8_2.SetVolume(0)
                End If
                flag4 = True
                flag3 = False
                flag2 = False
                num5 = 0
                num4 = 0
                str = ""
                flag = False
                DXSound.PlaySound(THF.F.directSoundSecondaryBuffer8_2, flag4, flag3, flag2, num5, num4, str, flag)
            End If
        End Sub

        Private Sub ShowChall()
            If Not Me.IsDeadValid Then
                If ((Me.IsMaster And THF.F.HasKilledBrutus) And THF.F.HasKilledBrutus2) Then
                    THF.F.ThingReplace(Me.x, Me.y, THF.F.Mouse)
                ElseIf (Me.IsMaster And Not THF.F.HasKilledBrutus2) Then
                    THF.F.ThingReplace(Me.x, Me.y, THF.F.JamesBrutus)
                ElseIf Not Me.IsMaster Then
                    If (Me.PosNum = 5) Then
                        THF.F.ThingReplace(Me.x, Me.y, THF.F.ChallWithKey)
                    Else
                        THF.F.ThingReplace(Me.x, Me.y, THF.F.challenger)
                    End If
                End If
            End If
        End Sub

        Private Sub ThrowBomb()
            If Not (((Me.IsMaster And THF.F.HasKilledBrutus) And THF.F.HasKilledBrutus2) And Not THF.F.HasKilledMouse) Then
                Dim num3 As Integer
                If Not Me.IsDoingBomb Then
                    If (Me.BombLaunchSound Is Nothing) Then
                        Me.BombLaunchSound = DXSound.smethod_0((DXSound.SoundPath & "\launch.wav"), 2.0!, 30)
                    End If
                    num3 = 0
                    DXSound.smethod_1(Me.BombLaunchSound, True, False, Me.x, Me.y, num3)
                    Me.IsDoingBomb = True
                End If
                If (Me.BombLaunchSound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING) Then
                    Dim num As Integer
                    Dim num2 As Integer
                    If (Me.BombDetSound Is Nothing) Then
                        Me.BombDetSound = DXSound.smethod_0((DXSound.SoundPath & "\explode.wav"), 2.0!, 30)
                    End If
                    num3 = 0
                    DXSound.smethod_1(Me.BombDetSound, True, False, Me.x, Me.y, num3)
                    Me.IsDoingBomb = False
                    If (Me.method_0 = 2) Then
                        num2 = 1
                        num = (Math.Round(CDbl((1! + Conversion.Int(CSng((200! * VBMath.Rnd)))))))
                    Else
                        num2 = 3
                        num = (Math.Round(CDbl((1! + Conversion.Int(CSng((30! * VBMath.Rnd)))))))
                    End If
                    If Me.ScanForChar(num2) Then
                        THF.F.ReflectHit(num, Me.PosNum)
                    End If
                End If
            End If
        End Sub

        Public Sub TurnIntoMaster()
            If Not Me.HasLoadedSounds Then
                Me.LoadSounds
            End If
            THConstVars.CannotDoKeydown = True
            If ((THF.F.HasKilledBrutus2 And THF.F.HasKilledBrutus) And Not THF.F.HasKilledMouse) Then
                Me.challGunSound = DXSound.smethod_0((DXSound.SoundPath & "\mm5.wav"), 2!, 30)
                Me.challHitSound = DXSound.smethod_0((DXSound.SoundPath & "\mm3.wav"), 2!, 30)
                Me.ChallDieSound = DXSound.LoadSound((DXSound.SoundPath & "\mm4.wav"))
                Me.BAttemptEscape1Sound = DXSound.smethod_0((DXSound.SoundPath & "\mm2.wav"), 2!, 30)
                Me.BAttemptEscape2Sound = DXSound.smethod_0((DXSound.SoundPath & "\mm2.wav"), 2!, 30)
                Me.BombLaunchSound = DXSound.smethod_0((DXSound.SoundPath & "\launchlast.wav"), 2!, 30)
                Me.BombDetSound = DXSound.smethod_0((DXSound.SoundPath & "\explodelast.wav"), 2!, 30)
                Me.directSoundSecondaryBuffer8_0 = DXSound.smethod_0((DXSound.SoundPath & "\mm2.wav"), 2!, 30)
                Me.CSeconds = 0
                Me.HSeconds = 0
                Me.A = &H1D4C
                Me.LRange = 1
            End If
            If (Not THF.F.HasKilledBrutus Or Not THF.F.HasKilledBrutus2) Then
                Me.challGunSound = DXSound.smethod_0((DXSound.SoundPath & "\challgunl.wav"), 2!, 30)
                Me.challHitSound = DXSound.smethod_0((DXSound.SoundPath & "\challgruntl.wav"), 2!, 30)
                If Not THF.F.HasKilledBrutus Then
                    Me.ChallDieSound = DXSound.LoadSound((DXSound.SoundPath & "\challdiel.wav"))
                Else
                    Me.ChallDieSound = DXSound.LoadSound((DXSound.SoundPath & "\challdiel2.wav"))
                End If
                Me.BombLaunchSound = DXSound.smethod_0((DXSound.SoundPath & "\launchlast.wav"), 2!, 30)
                Me.BombDetSound = DXSound.smethod_0((DXSound.SoundPath & "\explodelast.wav"), 2!, 30)
                Me.BAttemptEscape1Sound = DXSound.smethod_0((DXSound.SoundPath & "\brutus1.wav"), 2!, 30)
                Me.BAttemptEscape2Sound = DXSound.smethod_0((DXSound.SoundPath & "\brutus3.wav"), 2!, 30)
                Me.directSoundSecondaryBuffer8_0 = DXSound.smethod_0((DXSound.SoundPath & "\brutus2.wav"), 2!, 30)
                Me.CSeconds = 1
                Me.HSeconds = 1
                Me.A = &H1B58
                Me.LRange = 5
            End If
            Me.IsMaster = True
            THF.F.ThingReplace(Me.x, Me.y, THF.F.GetBGrid(Me.x, Me.y))
            Me.x = THF.F.px
            Me.y = THF.F.py
            Me.IsDeadValid = False
            If Me.IsFighting Then
                Dim f As mainFRM = THF.F
                f.NumAlert -= 1
                Me.IsFighting = False
            End If
        End Sub

        Private Sub UnloadSound(ByRef Sound As DirectSoundSecondaryBuffer8)
            If ((Not Sound Is Nothing) AndAlso ((Sound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING) And (Sound.GetStatus <> CONST_DSBSTATUSFLAGS.DSBSTATUS_LOOPING))) Then
                Sound = Nothing
            End If
        End Sub

        Public Sub UnloadSounds()
            If ((Me.PosNum <> 1) AndAlso (((Not Me.IsFighting And Not Me.HasUnloadedSounds) And Me.HasLoadedSounds) And (THF.F.NumAlert <= 0))) Then
                Me.UnloadSound(Me.BombLaunchSound)
                Me.UnloadSound(Me.BombDetSound)
                Me.UnloadSound(Me.LostSound)
                Me.UnloadSound(Me.RedAlertSound)
                Me.UnloadSound(Me.Caught1Sound)
                Me.UnloadSound(Me.Caught2Sound)
                Me.UnloadSound(Me.Caught3Sound)
                Me.UnloadSound(Me.Caught4Sound)
                Me.UnloadSound(Me.Caught5Sound)
                Me.UnloadSound(Me.challGunSound)
                Me.UnloadSound(Me.challHitSound)
                Me.UnloadSound(Me.ChallDieSound)
                Me.UnloadSound(Me.OpenDoorSound)
                Me.UnloadSound(Me.LockedDoorSound)
                Me.HasUnloadedSounds = True
                Me.HasLoadedSounds = False
            End If
        End Sub


        ' Fields
        Private Const Open_Door As Single = 0!
        Private Const Closed_Door As Single = 1!
        Private Const Locked_Door As Single = 2!
        Private Const North As Single = 0!
        Private Const South As Single = 1!
        Private Const West As Single = 0!
        Private Const east As Single = 1!
        Public NumOfNeedles As Integer
        Public IsPoisoned As Boolean
        Public IsBeingControled As Boolean
        Private LDoor As Boolean
        Private LCDoor As Boolean
        Private ChallDieSound As DirectSoundSecondaryBuffer8
        Private GExplodeSound As DirectSoundSecondaryBuffer8
        Private OpenDoorSound As DirectSoundSecondaryBuffer8
        Private LockedDoorSound As DirectSoundSecondaryBuffer8
        Public challHitSound As DirectSoundSecondaryBuffer8
        Public IsFighting As Boolean
        Public A As Integer
        Public x As Integer = 400
        Public y As Integer = 400
        Private up As Integer
        Private down As Integer
        Private MDir As Integer
        Private XDir As Integer
        Private countdown As Integer
        Private LCount As Integer
        Private BCount As Integer
        Private PosNum As Integer
        Public IsDeadValid As Boolean
        Private run As Integer
        Public CSeconds As Integer
        Private SCount As Integer
        Public HSeconds As Integer
        Private TCount As Integer
        Public IsMaster As Boolean
        Private RedAlertSound As DirectSoundSecondaryBuffer8
        Private BombLaunchSound As DirectSoundSecondaryBuffer8
        Private BombDetSound As DirectSoundSecondaryBuffer8
        Private challGunSound As DirectSoundSecondaryBuffer8
        Private HasLoadedSounds As Boolean
        Private BAttemptEscape1Sound As DirectSoundSecondaryBuffer8
        Private BAttemptEscape2Sound As DirectSoundSecondaryBuffer8
        Private directSoundSecondaryBuffer8_0 As DirectSoundSecondaryBuffer8
        Private LostSound As DirectSoundSecondaryBuffer8
        Private Caught1Sound As DirectSoundSecondaryBuffer8
        Private Caught2Sound As DirectSoundSecondaryBuffer8
        Private Caught3Sound As DirectSoundSecondaryBuffer8
        Private Caught4Sound As DirectSoundSecondaryBuffer8
        Private Caught5Sound As DirectSoundSecondaryBuffer8
        Private HasUnloadedSounds As Boolean
        Private IsDoingBomb As Boolean
        Private LRange As Integer
        Public HasKey As Boolean
        Public HasFoundOnce As Boolean
    End Class
End Namespace

