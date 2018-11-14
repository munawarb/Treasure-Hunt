Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.InteropServices

Namespace th
    <StandardModule> _
    Friend NotInheritable Class EllReg
        ' Methods
        Public Shared Function DeleteSubkey(ByVal Group As Integer, ByVal Section As String) As String
            Dim num4 As Integer
            Dim str2 As String = ""
            Try
                Dim num2 As Integer
                Dim num3 As Integer
Label_0001:
                ProjectData.ClearProjectError
                Dim num As Integer = 1
Label_0008:
                num2 = 2
                Dim lpSubKey As String = ChrW(0)
                EllReg.RegOpenKeyExA(Group, lpSubKey, 0, -1, num3)
Label_001D:
                num2 = 3
                EllReg.RegDeleteKeyA(num3, Section)
Label_0028:
                num2 = 4
                EllReg.RegCloseKey(num3)
                GoTo Label_0098
Label_0033:
                num4 = 0
                Select Case (num4 + 1)
                    Case 1
                        GoTo Label_0001
                    Case 2
                        GoTo Label_0008
                    Case 3
                        GoTo Label_001D
                    Case 4
                        GoTo Label_0028
                    Case 5
                        GoTo Label_0098
                    Case Else
                        GoTo Label_008D
                End Select
Label_0059:
                num4 = num2
                Select Case num
                    Case 0
                        GoTo Label_008D
                    Case 1
                        GoTo Label_0033
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_0059
            End Try
        Label_008D:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_0098:
            If (num4 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return str2
        End Function

        Public Shared Function DeleteValue(ByVal Group As Integer, ByVal Section As String, ByVal Key As String) As String
            Dim num4 As Integer
            Dim str As String = ""
            Try
                Dim num2 As Integer
                Dim num3 As Integer
Label_0001:
                ProjectData.ClearProjectError
                Dim num As Integer = 1
Label_0008:
                num2 = 2
                EllReg.RegOpenKeyA(Group, Section, num3)
Label_0015:
                num2 = 3
                EllReg.RegDeleteValueA(num3, Key)
Label_0020:
                num2 = 4
                EllReg.RegCloseKey(num3)
                GoTo Label_008C
Label_002B:
                num4 = 0
                Select Case (num4 + 1)
                    Case 1
                        GoTo Label_0001
                    Case 2
                        GoTo Label_0008
                    Case 3
                        GoTo Label_0015
                    Case 4
                        GoTo Label_0020
                    Case 5
                        GoTo Label_008C
                    Case Else
                        GoTo Label_0081
                End Select
Label_004F:
                num4 = num2
                Select Case num
                    Case 0
                        GoTo Label_0081
                    Case 1
                        GoTo Label_002B
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_004F
            End Try
        Label_0081:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_008C:
            If (num4 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return str
        End Function

        Public Shared Function ReadRegistry(ByVal Group As Integer, ByVal Section As String, ByVal Key As String) As String
            Dim str As String = ""
            Dim num9 As Integer
            Try
                Dim num2 As Integer
                Dim num3 As Integer
                Dim num10 As Integer
Label_0001:
                ProjectData.ClearProjectError
                Dim num8 As Integer = 1
Label_0009:
                num10 = 2
                Dim num4 As Integer = EllReg.RegOpenKeyA(Group, Section, num3)
Label_0017:
                num10 = 3
                Dim expression As String = Strings.Space(&H800)
Label_0026:
                num10 = 4
                Dim lpcbData As Integer = Strings.Len(expression)
Label_0032:
                num10 = 5
                num4 = EllReg.RegQueryValueExA(num3, Key, 0, num2, expression, lpcbData)
Label_0045:
                num10 = 6
                If Not ((num4 = 0) And (Information.Err.Number = 0)) Then
                    GoTo Label_0170
                End If
Label_005F:
                num10 = 7
                If (num2 <> 4) Then
                    GoTo Label_00D5
                End If
Label_0066:
                num10 = 8
                Dim num6 As Double = (((Strings.Asc(Strings.Mid(expression, 1, 1)) + (&H100 * Strings.Asc(Strings.Mid(expression, 2, 1)))) + (&H10000 * Strings.Asc(Strings.Mid(expression, 3, 1)))) + (16777216 * Strings.Asc(Strings.Mid(expression, 4, 1))))
Label_00BE:
                num10 = 9
                expression = Strings.Format(num6, "000")
Label_00D5:
                num10 = 11
                If (num2 <> 3) Then
                    GoTo Label_0159
                End If
Label_00DD:
                num10 = 12
                Dim str4 As String = ""
Label_00E8:
                num10 = 13
                Dim num7 As Short = CShort(lpcbData)
                Dim start As Short = 1
Label_00F3:
                If (start > num7) Then
                    GoTo Label_014F
                End If
Label_00F8:
                num10 = 14
                Dim str3 As String = Conversion.Hex(Strings.Asc(Strings.Mid(expression, start, 1)))
Label_0111:
                num10 = 15
                If (Strings.Len(str3) <> 1) Then
                    GoTo Label_013E
                End If
Label_011F:
                num10 = &H10
                str3 = ("0" & str3)
                GoTo Label_013E
Label_0133:
                num10 = &H13
                start = CShort((start + 1))
                GoTo Label_00F3
Label_013E:
                num10 = &H12
                str4 = (str4 & str3)
                GoTo Label_0133
Label_014F:
                num10 = 20
                expression = str4
                GoTo Label_017F
Label_0159:
                num10 = &H16
Label_015D:
                num10 = &H17
                expression = Strings.Left(expression, (lpcbData - 1))
                GoTo Label_017F
Label_0170:
                num10 = &H1A
Label_0174:
                num10 = &H1B
                expression = "Not Found"
Label_017F:
                num10 = &H1D
                num4 = EllReg.RegCloseKey(num3)
Label_018A:
                num10 = 30
                str = expression
                GoTo Label_0267
Label_0197:
                num9 = 0
                Select Case (num9 + 1)
                    Case 1
                        GoTo Label_0001
                    Case 2
                        GoTo Label_0009
                    Case 3
                        GoTo Label_0017
                    Case 4
                        GoTo Label_0026
                    Case 5
                        GoTo Label_0032
                    Case 6
                        GoTo Label_0045
                    Case 7
                        GoTo Label_005F
                    Case 8
                        GoTo Label_0066
                    Case 9
                        GoTo Label_00BE
                    Case 10, 11
                        GoTo Label_00D5
                    Case 12
                        GoTo Label_00DD
                    Case 13
                        GoTo Label_00E8
                    Case 14
                        GoTo Label_00F8
                    Case 15
                        GoTo Label_0111
                    Case &H10
                        GoTo Label_011F
                    Case &H11, &H12
                        GoTo Label_013E
                    Case &H13
                        GoTo Label_0133
                    Case 20
                        GoTo Label_014F
                    Case &H15, &H18, &H19, &H1C, &H1D
                        GoTo Label_017F
                    Case &H16
                        GoTo Label_0159
                    Case &H17
                        GoTo Label_015D
                    Case &H1A
                        GoTo Label_0170
                    Case &H1B
                        GoTo Label_0174
                    Case 30
                        GoTo Label_018A
                    Case &H1F
                        GoTo Label_0267
                    Case Else
                        GoTo Label_025C
                End Select
Label_0225:
                num9 = num10
                Select Case num8
                    Case 0
                        GoTo Label_025C
                    Case 1
                        GoTo Label_0197
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                GoTo Label_0225
            End Try
Label_025C:
            Throw ProjectData.CreateProjectError(-2146828237)
Label_0267:
            If (num9 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return str
        End Function

        Public Shared Function ReadRegistryGetAll(ByVal Group As Integer, ByVal Section As String, ByRef Idx As Integer) As Object
            Dim expression As String = ""
            Dim obj2 As Object
            Dim num8 As Integer
            Dim str2 As String = ""
            Try
                Dim num As Integer
                Dim num2 As Integer
                Dim num9 As Integer
Label_0001:
                ProjectData.ClearProjectError()
                Dim num7 As Integer = 1
Label_0009:
                num9 = 2
                Dim num3 As Integer = EllReg.RegOpenKeyA(Group, Section, num2)
Label_0017:
                num9 = 3
                expression = Strings.Space(&H800)
Label_0026:
                num9 = 4
                str2 = Strings.Space(&H800)
Label_0035:
                num9 = 5
                Dim lpcbData As Integer = Strings.Len(expression)
Label_0040:
                num9 = 6
                Dim lpcbValueName As Integer = Strings.Len(str2)
Label_004C:
                num9 = 7
                num3 = EllReg.RegEnumValueA(num2, Idx, str2, lpcbValueName, 0, num, expression, lpcbData)
Label_0063:
                num9 = 8
                If Not ((num3 = 0) And (Information.Err.Number = 0)) Then
                    GoTo Label_0116
                End If
Label_007D:
                num9 = 9
                If (num <> 4) Then
                    GoTo Label_00F5
                End If
Label_0085:
                num9 = 10
                Dim num6 As Double = (((Strings.Asc(Strings.Mid(expression, 1, 1)) + (&H100 * Strings.Asc(Strings.Mid(expression, 2, 1)))) + (&H10000 * Strings.Asc(Strings.Mid(expression, 3, 1)))) + (16777216 * Strings.Asc(Strings.Mid(expression, 4, 1))))
Label_00DE:
                num9 = 11
                expression = Strings.Format(num6, "000")
Label_00F5:
                num9 = 13
                expression = Strings.Left(expression, (lpcbData - 1))
Label_0105:
                num9 = 14
                str2 = Strings.Left(str2, lpcbValueName)
                GoTo Label_0125
Label_0116:
                num9 = &H10
Label_011A:
                num9 = &H11
                expression = "Not Found"
Label_0125:
                num9 = &H13
                num3 = EllReg.RegCloseKey(num2)
Label_0130:
                num9 = 20
                obj2 = Strings.Split(String.Concat(New String() {Conversions.ToString(num), "|", str2, "|", expression}), "|", -1, CompareMethod.Binary)
                GoTo Label_0226
Label_017E:
                num8 = 0
                Select Case (num8 + 1)
                    Case 1
                        GoTo Label_0001
                    Case 2
                        GoTo Label_0009
                    Case 3
                        GoTo Label_0017
                    Case 4
                        GoTo Label_0026
                    Case 5
                        GoTo Label_0035
                    Case 6
                        GoTo Label_0040
                    Case 7
                        GoTo Label_004C
                    Case 8
                        GoTo Label_0063
                    Case 9
                        GoTo Label_007D
                    Case 10
                        GoTo Label_0085
                    Case 11
                        GoTo Label_00DE
                    Case 12, 13
                        GoTo Label_00F5
                    Case 14
                        GoTo Label_0105
                    Case 15, &H12, &H13
                        GoTo Label_0125
                    Case &H10
                        GoTo Label_0116
                    Case &H11
                        GoTo Label_011A
                    Case 20
                        GoTo Label_0130
                    Case &H15
                        GoTo Label_0226
                    Case Else
                        GoTo Label_021B
                End Select
Label_01E4:
                num8 = num9
                Select Case num7
                    Case 0
                        GoTo Label_021B
                    Case 1
                        GoTo Label_017E
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                GoTo Label_01E4
            End Try
Label_021B:
            Throw ProjectData.CreateProjectError(-2146828237)
Label_0226:
            If (num8 <> 0) Then
                ProjectData.ClearProjectError()
            End If
            Return obj2
        End Function

        Public Shared Function ReadRegistryGetSubkey(ByVal Group As Integer, ByVal Section As String, ByRef Idx As Integer) As String
            Dim str2 As String = ""
            Dim num6 As Integer
            Try
                Dim num2 As Integer
                Dim num3 As Integer
Label_0001:
                ProjectData.ClearProjectError
                Dim num As Integer = 1
Label_0008:
                num2 = 2
                Dim num4 As Integer = EllReg.RegOpenKeyA(Group, Section, num3)
Label_0015:
                num2 = 3
                Dim expression As String = Strings.Space(&H800)
Label_0023:
                num2 = 4
                Dim cbName As Integer = Strings.Len(expression)
Label_002E:
                num2 = 5
                num4 = EllReg.RegEnumKeyA(num3, Idx, expression, cbName)
Label_003D:
                num2 = 6
                If Not ((num4 = 0) And (Information.Err.Number = 0)) Then
                    GoTo Label_006F
                End If
Label_0053:
                num2 = 7
                expression = Strings.Left(expression, (Strings.InStr(expression, ChrW(0), CompareMethod.Binary) - 1))
                GoTo Label_007C
Label_006F:
                num2 = 9
Label_0072:
                num2 = 10
                expression = "Not Found"
Label_007C:
                num2 = 12
                num4 = EllReg.RegCloseKey(num3)
Label_0086:
                num2 = 13
                str2 = expression
                GoTo Label_011B
Label_0092:
                num6 = 0
                Select Case (num6 + 1)
                    Case 1
                        GoTo Label_0001
                    Case 2
                        GoTo Label_0008
                    Case 3
                        GoTo Label_0015
                    Case 4
                        GoTo Label_0023
                    Case 5
                        GoTo Label_002E
                    Case 6
                        GoTo Label_003D
                    Case 7
                        GoTo Label_0053
                    Case 8, 11, 12
                        GoTo Label_007C
                    Case 9
                        GoTo Label_006F
                    Case 10
                        GoTo Label_0072
                    Case 13
                        GoTo Label_0086
                    Case 14
                        GoTo Label_011B
                    Case Else
                        GoTo Label_0110
                End Select
Label_00DC:
                num6 = num2
                Select Case num
                    Case 0
                        GoTo Label_0110
                    Case 1
                        GoTo Label_0092
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                GoTo Label_00DC
            End Try
Label_0110:
            Throw ProjectData.CreateProjectError(-2146828237)
Label_011B:
            If (num6 <> 0) Then
                ProjectData.ClearProjectError
            End If
            Return str2
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegCloseKey(ByVal hKey As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegCreateKeyA(ByVal hKey As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpSubKey As String, ByRef phkResult As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegDeleteKeyA(ByVal hKey As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpSubKey As String) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegDeleteValueA(ByVal hKey As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpValueName As String) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegEnumKeyA(ByVal hKey As Integer, ByVal dwIndex As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpName As String, ByVal cbName As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegEnumValueA(ByVal hKey As Integer, ByVal dwIndex As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpValueName As String, ByRef lpcbValueName As Integer, ByVal lpReserved As Integer, ByRef lpType As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpData As String, ByRef lpcbData As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegFlushKey(ByVal hKey As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegOpenKeyA(ByVal hKey As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpSubKey As String, ByRef phkResult As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegOpenKeyExA(ByVal hKey As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpSubKey As String, ByVal ulOptions As Integer, ByVal samDesired As Integer, ByRef phkResult As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegQueryValueExA(ByVal hKey As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpValueName As String, ByVal lpReserved As Integer, ByRef lpType As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpData As String, ByRef lpcbData As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegSetValueExA(ByVal hKey As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpValueName As String, ByVal Reserved As Integer, ByVal dwType As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpValue As String, ByVal cbData As Integer) As Integer
        End Function

        <DllImport("advapi32.dll", EntryPoint:="RegSetValueExA", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Private Shared Function RegSetValueExA_1(ByVal hKey As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpValueName As String, ByVal Reserved As Integer, ByVal dwType As Integer, ByRef lpValue As Integer, ByVal cbData As Integer) As Integer
        End Function

        Public Shared Sub WriteRegistry(ByVal Group As Integer, ByVal Section As String, ByVal Key As String, ByVal ValType As InTypes, ByVal Value As String)
            Dim num6 As Integer
            Dim expression As String = ""
            Try
                Dim num2 As Integer
                Dim num3 As Integer
Label_0001:
                ProjectData.ClearProjectError
                Dim num As Integer = 1
Label_0008:
                num2 = 2
                EllReg.RegCreateKeyA(Group, Section, num3)
Label_0015:
                num2 = 3
                If (ValType <> InTypes.ValDWord) Then
                    GoTo Label_003D
                End If
Label_001B:
                num2 = 4
                Dim lpValue As Integer = Conversions.ToInteger(Value)
Label_0025:
                num2 = 5
                Dim cbData As Integer = 4
Label_002A:
                num2 = 6
                EllReg.RegSetValueExA_1(num3, Key, 0, CInt(ValType), lpValue, cbData)
                GoTo Label_007C
Label_003D:
                num2 = 8
Label_003F:
                num2 = 9
                If (ValType <> InTypes.ValString) Then
                    GoTo Label_0057
                End If
Label_0046:
                num2 = 10
                Value = (Value & ChrW(0))
Label_0057:
                num2 = 12
                expression = Value
Label_005E:
                num2 = 13
                cbData = Strings.Len(expression)
Label_006A:
                num2 = 14
                EllReg.RegSetValueExA(num3, Key, 0, 1, expression, cbData)
Label_007C:
                num2 = &H10
                EllReg.RegFlushKey(num3)
Label_0086:
                num2 = &H11
                EllReg.RegCloseKey(num3)
                GoTo Label_012E
Label_0095:
                num6 = 0
                Select Case (num6 + 1)
                    Case 1
                        GoTo Label_0001
                    Case 2
                        GoTo Label_0008
                    Case 3
                        GoTo Label_0015
                    Case 4
                        GoTo Label_001B
                    Case 5
                        GoTo Label_0025
                    Case 6
                        GoTo Label_002A
                    Case 7, 15, &H10
                        GoTo Label_007C
                    Case 8
                        GoTo Label_003D
                    Case 9
                        GoTo Label_003F
                    Case 10
                        GoTo Label_0046
                    Case 11, 12
                        GoTo Label_0057
                    Case 13
                        GoTo Label_005E
                    Case 14
                        GoTo Label_006A
                    Case &H11
                        GoTo Label_0086
                    Case &H12
                        GoTo Label_012E
                    Case Else
                        GoTo Label_0123
                End Select
Label_00EF:
                num6 = num2
                Select Case num
                    Case 0
                        GoTo Label_0123
                    Case 1
                        GoTo Label_0095
                End Select
            Catch obj1 As Exception
                ProjectData.SetProjectError(DirectCast(obj1, Exception))
                goto Label_00EF
            End Try
        Label_0123:
            Throw ProjectData.CreateProjectError(-2146828237)
        Label_012E:
            If (num6 <> 0) Then
                ProjectData.ClearProjectError
            End If
        End Sub


        ' Fields
        Public Const READ_CONTROL As Integer = &H20000
        Public Const SYNCHRONIZE As Integer = &H100000
        Public Const STANDARD_RIGHTS_ALL As Integer = &H1F0000
        Public Const STANDARD_RIGHTS_READ As Integer = &H20000
        Public Const STANDARD_RIGHTS_WRITE As Integer = &H20000
        Public Const KEY_QUERY_VALUE As Short = 1
        Public Const KEY_SET_VALUE As Short = 2
        Public Const KEY_CREATE_SUB_KEY As Short = 4
        Public Const KEY_ENUMERATE_SUB_KEYS As Short = 8
        Public Const KEY_NOTIFY As Short = &H10
        Public Const KEY_CREATE_LINK As Short = &H20
        Public Const KEY_ALL_ACCESS As Boolean = True
        Public Const KEY_READ As Boolean = True
        Public Const KEY_EXECUTE As Boolean = True
        Public Const KEY_WRITE As Boolean = True
        Public Const REG_NONE As Integer = 0
        Public Const REG_SZ As Integer = 1
        Public Const REG_EXPAND_SZ As Integer = 2
        Public Const REG_BINARY As Integer = 3
        Public Const REG_DWORD As Integer = 4
        Public Const REG_LINK As Integer = 6
        Public Const REG_MULTI_SZ As Integer = 7
        Public Const REG_RESOURCE_LIST As Integer = 8
        Public Const HKEY_CLASSES_ROOT As Integer = -2147483648
        Public Const HKEY_CURRENT_USER As Integer = -2147483647
        Public Const HKEY_LOCAL_MACHINE As Integer = -2147483646
        Public Const HKEY_USERS As Integer = -2147483645
        Public Const HKEY_PERFORMANCE_DATA As Integer = -2147483644
        Public Const HKEY_CURRENT_CONFIG As Integer = -2147483643
        Public Const HKEY_DYN_DATA As Integer = -2147483642
        Private Const ERROR_NONE As Short = 0
        Private Const ERROR_BADDB As Short = 1
        Private Const ERROR_BADKEY As Short = 2
        Private Const ERROR_CANTOPEN As Short = 3
        Private Const ERROR_CANTREAD As Short = 4
        Private Const ERROR_CANTWRITE As Short = 5
        Private Const ERROR_OUTOFMEMORY As Short = 6
        Private Const ERROR_INVALID_PARAMETER As Short = 7
        Private Const ERROR_ACCESS_DENIED As Short = 8
        Private Const ERROR_INVALID_PARAMETERS As Short = &H57
        Private Const ERROR_NO_MORE_ITEMS As Short = &H103

        ' Nested Types
        Public Enum InTypes
            ' Fields
            ValNull = 0
            ValString = 1
            const_2 = 2
            ValBinary = 3
            ValDWord = 4
            ValLink = 6
            ValMultiString = 7
            ValResList = 8
        End Enum
    End Class
End Namespace

