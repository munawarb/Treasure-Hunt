Imports DxVBLibA
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace th
    <StandardModule> _
    Friend NotInheritable Class DXSound
        ' Methods
        Private Shared Sub GetWhere(ByRef TheX As Short, ByRef TheY As Short, ByRef x As Short, ByRef y As Short)
            Dim px As Short
            Dim py As Short
            If THF.F.IsControling Then
                px = Conversions.ToShort(NewLateBinding.LateGet(THF.F.GetChall(THF.F.WControl), Nothing, "x", New Object(0  - 1) {}, Nothing, Nothing, Nothing))
                py = Conversions.ToShort(NewLateBinding.LateGet(THF.F.GetChall(THF.F.WControl), Nothing, "y", New Object(0  - 1) {}, Nothing, Nothing, Nothing))
            Else
                px = THF.F.px
                py = THF.F.py
            End If
            If (px > x) Then
                TheX = -1
            End If
            If (px = x) Then
                TheX = 0
            End If
            If (px < x) Then
                TheX = 1
            End If
            If (py > y) Then
                TheY = -1
            End If
            If (py = y) Then
                TheY = 0
            End If
            If (py < y) Then
                TheY = 1
            End If
        End Sub

        Public Shared Sub InitDX(ByRef WinHandle As Long)
            DXSound.SoundPath = (Addendums.FilePath & "\sounds")
            DXSound.string_0 = (DXSound.SoundPath & "\narratives")
            DXSound.NumPath = (DXSound.string_0 & "\nums")
            DXSound.objDX = New DirectX8Class
            DXSound.objDS = DXSound.objDX.DirectSoundCreate("")
            DXSound.objDS.SetCooperativeLevel(CInt(WinHandle), CONST_DSSCLFLAGS.DSSCL_PRIORITY)
        End Sub

        Public Shared Function LoadSound(ByVal FileName As String) As DirectSoundSecondaryBuffer8
            Dim dsbufferdesc As DSBUFFERDESC = New DSBUFFERDESC()

            THConstVars.string_0 = FileName
            dsbufferdesc.lFlags = (CONST_DSBCAPSFLAGS.DSBCAPS_GLOBALFOCUS Or (CONST_DSBCAPSFLAGS.DSBCAPS_CTRLVOLUME Or CONST_DSBCAPSFLAGS.DSBCAPS_CTRLPAN))
            Return DXSound.objDS.CreateSoundBufferFromFile(FileName, dsbufferdesc)
        End Function

        Public Shared Sub LocaleNotify(ByRef x As Short, ByRef y As Short, ByRef Optional DVolume As String = "")
            Dim num As Short
            Dim num2 As Short
            DXSound.GetWhere(num, num2, x, y)
            If (DVolume <> "dont") Then
                Dim flag As Boolean
                Dim flag2 As Boolean
                Dim flag3 As Boolean
                Dim num3 As Short
                Dim num4 As Short
                Dim str As String
                Dim flag4 As Boolean
                Select Case num2
                    Case -1
                        flag = True
                        flag2 = False
                        flag3 = False
                        num3 = 0
                        num4 = 0
                        str = ""
                        flag4 = False
                        DXSound.PlaySound(THF.F.BeepSound, flag, flag2, flag3, num3, num4, str, flag4)
                        Exit Select
                    Case 1
                        flag4 = True
                        flag3 = False
                        flag2 = False
                        num4 = 0
                        num3 = 0
                        str = ""
                        flag = False
                        DXSound.PlaySound(THF.F.directSoundSecondaryBuffer8_2, flag4, flag3, flag2, num4, num3, str, flag)
                        Exit Select
                End Select
            End If
        End Sub

        Private Shared Sub PlayAroundCenter(ByRef Sound As DirectSoundSecondaryBuffer8, ByVal x As Short, ByVal y As Short, ByRef position As Single, ByVal Optional Distance As Long = 1)
            Dim num2 As Short
            Dim num3 As Short
            Select Case position
                Case 0!
                    num2 = CShort((x - Distance))
                    num3 = 0
                    DXSound.smethod_1(Sound, True, False, num2, y, num3)
                    Exit Select
                Case 1!
                    num3 = CShort((x + Distance))
                    num2 = 0
                    DXSound.smethod_1(Sound, True, False, num3, y, num2)
                    Exit Select
                Case 2!
                    num3 = CShort((y + Distance))
                    num2 = 0
                    DXSound.smethod_1(Sound, True, False, x, num3, num2)
                    Exit Select
                Case 3!
                    num3 = CShort((y - Distance))
                    num2 = 0
                    DXSound.smethod_1(Sound, True, False, x, num3, num2)
                    Exit Select
                Case Else
                    num3 = 0
                    DXSound.smethod_1(Sound, True, False, x, y, num3)
                    Exit Select
            End Select
        End Sub

        Public Shared Sub PlaySound(ByRef Sound As DirectSoundSecondaryBuffer8, ByRef bCloseFirst As Boolean, ByRef bLoopSound As Boolean, ByRef Optional PerformEffects As Boolean = False, ByRef Optional x As Short = 0, ByRef Optional y As Short = 0, ByRef Optional DVolume As String = "", ByRef Optional WaitTillDone As Boolean = False)
            If bCloseFirst Then
                Sound.Stop
                Sound.SetCurrentPosition(0)
            End If
            If bLoopSound Then
                Sound.Play(CONST_DSBPLAYFLAGS.DSBPLAY_LOOPING)
            Else
                Sound.Play(CONST_DSBPLAYFLAGS.DSBPLAY_DEFAULT)
            End If
            If WaitTillDone Then
                Do While (Sound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                    Application.DoEvents
                Loop
            End If
        End Sub

        Public Shared Sub Radio(ByRef file As String, ByRef Optional RWait As Boolean = False)
            THF.F.RadioSound.Stop
            THF.F.RadioSound = DXSound.LoadSound((DXSound.SoundPath & "\" & file))
            Dim bCloseFirst As Boolean = True
            Dim bLoopSound As Boolean = False
            Dim performEffects As Boolean = False
            Dim x As Short = 0
            Dim y As Short = 0
            Dim dVolume As String = ""
            Dim waitTillDone As Boolean = False
            DXSound.PlaySound(THF.F.RadioSound, bCloseFirst, bLoopSound, performEffects, x, y, dVolume, waitTillDone)
            If RWait Then
                THConstVars.CannotDoKeydown = True
                Do While (THF.F.RadioSound.GetStatus = CONST_DSBSTATUSFLAGS.DSBSTATUS_PLAYING)
                    Application.DoEvents
                Loop
            End If
        End Sub

        Public Shared Sub ResetSoundCard(ByRef WinHandle As Object)
            THConstVars.CannotDoKeydown = True
            DXSound.objDX = Nothing
            DXSound.objDS = Nothing
            DXSound.objDX = New DirectX8Class
            DXSound.objDS = DXSound.objDX.DirectSoundCreate("")
            DXSound.objDS.SetCooperativeLevel(Conversions.ToInteger(WinHandle), CONST_DSSCLFLAGS.DSSCL_PRIORITY)
            THConstVars.CannotDoKeydown = False
        End Sub

        Public Shared Sub SetCoordinates(ByRef x As Short, ByRef y As Short, ByRef Optional z As Short = 0)
            DXSound.directSound3DListener8_0.SetPosition(CSng(x), CSng(y), CSng(z), CONST_DS3DAPPLYFLAGS.DS3D_IMMEDIATE)
        End Sub

        Private Shared Sub SetSoundPosition1(ByRef TheSound As DirectSoundSecondaryBuffer8, ByRef x As Short, ByRef y As Short, ByRef DVolume As String)
            Dim num As Integer
            Dim num2 As Short
            Dim num3 As Short
            DXSound.GetWhere(num2, num3, x, y)
            If (Math.Abs(CShort((THF.F.px - x))) > 10) Then
                num = &H2710
            Else
                num = (Math.Abs(CShort((THF.F.px - x))) * &H3E8)
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
            If (Math.Abs(CShort((THF.F.py - y))) > Math.Abs(CShort((THF.F.px - x)))) Then
                If (Math.Abs(CShort((THF.F.py - y))) > 20) Then
                    num = &H2710
                Else
                    num = (Math.Abs(CShort((THF.F.py - y))) * 100)
                End If
            End If
            If ((Math.Abs(CShort((THF.F.px - x))) > Math.Abs(CShort((THF.F.py - y)))) Or (Math.Abs(CShort((THF.F.px - x))) = Math.Abs(CShort((THF.F.py - y))))) Then
                If (Math.Abs(CShort((THF.F.px - x))) > 20) Then
                    num = &H2710
                Else
                    num = (Math.Abs(CShort((THF.F.px - x))) * 100)
                End If
            End If
            TheSound.SetVolume((0 - num))
            If (DVolume <> "dont") Then
                Dim flag As Boolean
                Dim flag2 As Boolean
                Dim flag3 As Boolean
                Dim num4 As Short
                Dim num5 As Short
                Dim str As String
                Dim flag4 As Boolean
                Select Case num3
                    Case 1
                        THF.F.BeepSound.SetPan(TheSound.GetPan)
                        If (DVolume = "vol") Then
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
                        Exit Select
                    Case -1
                        THF.F.directSoundSecondaryBuffer8_2.SetPan(TheSound.GetPan)
                        If (DVolume = "vol") Then
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
                        Exit Select
                End Select
            End If
        End Sub

        Public Shared Function smethod_0(ByVal FileName As String, ByVal Optional float_0 As Single = 2!, ByVal Optional MaxDistance As Long = 30) As DirectSoundSecondaryBuffer8
            Dim dsbufferdesc As DSBUFFERDESC = New DSBUFFERDESC()

            Dim dsbufferdesc2 As DSBUFFERDESC = New DSBUFFERDESC()
            THConstVars.string_0 = FileName
            dsbufferdesc.lFlags = (CONST_DSBCAPSFLAGS.DSBCAPS_MUTE3DATMAXDISTANCE Or (CONST_DSBCAPSFLAGS.DSBCAPS_CTRLVOLUME Or CONST_DSBCAPSFLAGS.DSBCAPS_CTRL3D))
            Dim buffer As DirectSoundSecondaryBuffer8 = DXSound.objDS.CreateSoundBufferFromFile(FileName, dsbufferdesc)
            Dim buffer2 As DirectSound3DBuffer8 = buffer.GetDirectSound3DBuffer
            dsbufferdesc2.lFlags = (CONST_DSBCAPSFLAGS.DSBCAPS_CTRL3D Or CONST_DSBCAPSFLAGS.DSBCAPS_PRIMARYBUFFER)
            DXSound.directSound3DListener8_0 = DXSound.objDS.CreatePrimarySoundBuffer(dsbufferdesc2).GetDirectSound3DListener
            DXSound.directSound3DListener8_0.SetOrientation(0!, 0!, 1!, 0!, 1!, 0!, CONST_DS3DAPPLYFLAGS.DS3D_IMMEDIATE)
            DXSound.directSound3DListener8_0.SetDistanceFactor(0.3048!, CONST_DS3DAPPLYFLAGS.DS3D_IMMEDIATE)
            buffer2.SetMinDistance(float_0, CONST_DS3DAPPLYFLAGS.DS3D_IMMEDIATE)
            buffer2.SetMaxDistance(CSng(MaxDistance), CONST_DS3DAPPLYFLAGS.DS3D_IMMEDIATE)
            THConstVars.MWait(10)
            Return buffer
        End Function

        Public Shared Sub smethod_1(ByRef Sound As DirectSoundSecondaryBuffer8, ByVal bCloseFirst As Boolean, ByVal bLoopSound As Boolean, ByRef x As Short, ByRef y As Short, ByRef Optional z As Short = 0)
            Dim buffer As DirectSound3DBuffer8 = Sound.GetDirectSound3DBuffer
            If bCloseFirst Then
                Sound.Stop
                Sound.SetCurrentPosition(0)
            End If
            buffer.SetPosition(CSng(x), CSng(y), CSng(z), CONST_DS3DAPPLYFLAGS.DS3D_IMMEDIATE)
            If bLoopSound Then
                Sound.Play(CONST_DSBPLAYFLAGS.DSBPLAY_LOOPING)
            Else
                Sound.Play(CONST_DSBPLAYFLAGS.DSBPLAY_DEFAULT)
            End If
        End Sub


        ' Fields
        Public Const Pos_West As Single = 0!
        Public Const Pos_East As Single = 1!
        Public Const Pos_North As Single = 2!
        Public Const pos_South As Single = 3!
        Public Const Pos_Same As Single = 4!
        Public Shared objDX As DirectX8 = New DirectX8Class
        Public Shared objDS As DirectSound8
        Public Shared directSound3DListener8_0 As DirectSound3DListener8
        Public Shared SoundPath As String
        Public Shared string_0 As String
        Public Shared NumPath As String
    End Class
End Namespace

