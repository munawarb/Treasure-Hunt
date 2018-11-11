Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace th
    <StandardModule> _
    Friend NotInheritable Class THReg
        ' Methods
        Public Shared Function GetProductID() As String
            Dim str As String
            If (Strings.InStr(Strings.LCase(Interaction.Environ("SYSTEMROOT")), "winnt", CompareMethod.Binary) > 0) Then
                str = "SOFTWARE\\MICROSOFT\\Windows NT\\CurrentVersion"
            Else
                str = "SOFTWARE\\MICROSOFT\\WINDOWS\\CurrentVersion"
            End If
            Dim number As Short = &H7B
            Dim reverse As Boolean = False
            Dim str3 As String = (THReg.ValRegString(Interaction.GetSetting(Addendums.AppTitle, "Config", "Username", ""), number, reverse) & "-")
            Dim str4 As String = EllReg.ReadRegistry(-2147483646, str, "ProductId")
            Dim str5 As String = EllReg.ReadRegistry(-2147483646, str, "RegisteredOwner")
            If ((str4 = "") Or (str5 = "")) Then
                Interaction.MsgBox("There was an error with your product ID. The game cannot continue. The product ID does not exist.", MsgBoxStyle.Critical, "Error")
                ProjectData.EndApp
            End If
            number = &H1F5
            reverse = False
            Dim str6 As String = THReg.ValRegString((str4 & "-" & str5), number, reverse)
            Return (str3 & str6)
        End Function

        Public Shared Function GetProductReg(ByRef Optional ID As String = "") As String
            Dim expression As String = ID
            If (expression = "") Then
                expression = THReg.GetProductID
            End If
            Dim length As Short = CShort(Math.Round(Conversion.Int(CDbl((CDbl(Strings.Len(expression)) / 3)))))
            Dim str As String = Strings.Left(expression, length)
            Dim str2 As String = Strings.Mid(expression, length, length)
            Dim str3 As String = Strings.Right(expression, length)
            Dim number As Short = &H18F
            Dim reverse As Boolean = True
            expression = THReg.ValRegString(String.Concat(New String() { str, "-", str2, "-", str3 }), number, reverse)
            length = CShort(Math.Round(Conversion.Int(CDbl((CDbl(Strings.Len(expression)) / 3)))))
            str = Strings.Left(expression, length)
            str2 = Strings.Mid(expression, length, length)
            str3 = Strings.Right(expression, length)
            expression = (str2 & str & str3)
            If (Strings.Len(expression) < &H12) Then
                Do While (Strings.Len(expression) < &H12)
                    number = &H121
                    reverse = True
                    expression = THReg.ValRegString((expression & expression), number, reverse)
                    Application.DoEvents
                Loop
            End If
            Return expression
        End Function

        Private Shared Function GetRegChar(ByRef position As Short) As String
            Dim divisor As Long = CLng(position)
            Dim dividen As Long = &H1A
            position = CShort(divisor)
            If THReg.IsDiv(divisor, dividen) Then
                Return "a"
            End If
            dividen = CLng(position)
            divisor = &H19
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "b"
            End If
            dividen = CLng(position)
            divisor = &H18
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "c"
            End If
            dividen = CLng(position)
            divisor = &H17
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "d"
            End If
            dividen = CLng(position)
            divisor = &H16
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "e"
            End If
            dividen = CLng(position)
            divisor = &H15
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "f"
            End If
            dividen = CLng(position)
            divisor = 20
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "g"
            End If
            dividen = CLng(position)
            divisor = &H13
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "h"
            End If
            dividen = CLng(position)
            divisor = &H12
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "i"
            End If
            dividen = CLng(position)
            divisor = &H11
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "j"
            End If
            dividen = CLng(position)
            divisor = &H10
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "k"
            End If
            dividen = CLng(position)
            divisor = 15
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "l"
            End If
            dividen = CLng(position)
            divisor = 14
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "m"
            End If
            dividen = CLng(position)
            divisor = 13
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "n"
            End If
            dividen = CLng(position)
            divisor = 12
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "o"
            End If
            dividen = CLng(position)
            divisor = 11
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "p"
            End If
            dividen = CLng(position)
            divisor = 10
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "q"
            End If
            dividen = CLng(position)
            divisor = 9
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "r"
            End If
            dividen = CLng(position)
            divisor = 8
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "s"
            End If
            dividen = CLng(position)
            divisor = 7
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "t"
            End If
            dividen = CLng(position)
            divisor = 6
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "u"
            End If
            dividen = CLng(position)
            divisor = 5
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "v"
            End If
            dividen = CLng(position)
            divisor = 4
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "w"
            End If
            dividen = CLng(position)
            divisor = 3
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "x"
            End If
            dividen = CLng(position)
            divisor = 2
            position = CShort(dividen)
            If THReg.IsDiv(dividen, divisor) Then
                Return "y"
            End If
            Return "null"
        End Function

        Private Shared Function IsDiv(ByRef divisor As Long, ByRef dividen As Long) As Boolean
            Dim flag As Boolean
            If (Conversion.Int(CDbl((CDbl(divisor) / CDbl(dividen)))) = (CDbl(divisor) / CDbl(dividen))) Then
                flag = True
            End If
            Return flag
        End Function

        Private Shared Function RegGetAlpha(ByRef position As Short) As String
            Dim regChar As String = THReg.GetRegChar(position)
            If (regChar = "null") Then
                Dim num2 As Short
                Dim num4 As Short
                Dim str As String = Conversion.Str(CShort(position))
                str = Strings.Mid(str, 2, (Strings.Len(str) - 1))
                Dim strArray As String() = New String((Strings.Len(str) + 1)  - 1) {}
                Dim num As Short = CShort(Strings.Len(str))
                num2 = 1
                Do While (num2 <= num)
                    strArray(num2) = Strings.Mid(str, num2, 1)
                    num2 = CShort((num2 + 1))
                Loop
                Dim num3 As Short = CShort(Strings.Len(str))
                num2 = 1
                Do While (num2 <= num3)
                    num4 = CShort(Math.Round(CDbl((num4 + Conversion.Val(strArray(num2))))))
                    num2 = CShort((num2 + 1))
                Loop
                regChar = THReg.GetRegChar(num4)
                If (regChar = "null") Then
                    regChar = "z"
                End If
            End If
            Return regChar
        End Function

        Private Shared Function ValRegString(ByRef RString As String, ByRef number As Short, ByRef Optional reverse As Boolean = False) As String
            Dim num As Short
            Dim str As String
            Dim num2 As Short
            Dim str2 As String
            Dim num3 As Short
            Dim str3 As String
            If Not reverse Then
                Dim num4 As Short = CShort(Strings.Len(CStr(RString)))
                num2 = 1
                Do While (num2 <= num4)
                    str = Strings.Mid(RString, num2, 1)
                    If (num3 = 0) Then
                        num3 = CShort(Strings.Len(CStr(RString)))
                    Else
                        num3 = CShort((num3 - 1))
                    End If
                    If (num3 < num2) Then
                        num3 = CShort((num3 + 1))
                    End If
                    str2 = Strings.Mid(RString, num3, 1)
                    num = CShort(((Strings.Asc(str) + Strings.Asc(str2)) + CShort((number + number))))
                    str3 = (str3 & THReg.RegGetAlpha(num))
                    Application.DoEvents
                    num2 = CShort((num2 + 1))
                Loop
                Return str3
            End If
            num2 = CShort((Strings.Len(CStr(RString)) + 1))
            Do While (num2 > 1)
                num2 = CShort((num2 - 1))
                str = Strings.Mid(RString, num2, 1)
                If (num3 = 0) Then
                    num3 = 1
                Else
                    num3 = CShort((num3 + 1))
                End If
                If (num3 > num2) Then
                    num3 = CShort((num3 - 1))
                End If
                str2 = Strings.Mid(RString, num3, 1)
                num = CShort(((Strings.Asc(str) + Strings.Asc(str2)) + CShort((number + number))))
                str3 = (str3 & THReg.RegGetAlpha(num))
                Application.DoEvents
            Loop
            Return str3
        End Function

    End Class
End Namespace

