Imports System
Imports System.Runtime.CompilerServices

Namespace th
    Public Class THF
        ' Methods
        Public Shared Sub init(ByVal FRM As Object)
            THF.m_MainFRM = DirectCast(RuntimeHelpers.GetObjectValue(FRM), mainFRM)
        End Sub


        ' Properties
        Public Shared Property F As mainFRM
            Get
                Return THF.m_MainFRM
            End Get
            Set(ByVal value As mainFRM)
                THF.m_MainFRM = value
            End Set
        End Property


        ' Fields
        Private Shared m_MainFRM As mainFRM
    End Class
End Namespace

