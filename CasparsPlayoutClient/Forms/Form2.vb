'Imports System.Threading

Public Class Form2

    Private sc As ServerController
    Private maxOffset(20) As Integer
    Delegate Sub SetTextCallback(ByVal state As Dictionary(Of Integer, Long))

    Public Sub New(ByRef controller As ServerController)
        MyBase.New()
        InitializeComponent()
        Me.sc = controller
        AddHandler sc.getTicker.frameTick, AddressOf receiveTick
    End Sub

    Public Sub receiveTick(ByVal sender As Object, ByVal e As frameTickEventArgs)
        On Error Resume Next
        If Me.lstFrame.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf updateLstFrame)
            Me.Invoke(d, New Object() {e.result})
        Else
            updateLstFrame(e.result)
        End If
    End Sub

    Private Sub updateLstFrame(ByVal state As Dictionary(Of Integer, Long))
        For Each channel In state.Keys
            If lstFrame.Items.Count >= channel Then
                If state.Item(channel) - lstFrame.Items.Item(channel - 1) > maxOffset(channel - 1) Then maxOffset(channel - 1) = state.Item(channel) - lstFrame.Items.Item(channel - 1)
                lstOffset.Items.Item(channel - 1) = (state.Item(channel) - lstFrame.Items(channel - 1) & " : " & maxOffset(channel - 1))
                lstFrame.Items.Item(channel - 1) = state.Item(channel)
            Else
                lstFrame.Items.Add(state.Item(channel))
                lstOffset.Items.Add(0 & " : " & maxOffset(channel - 1))
            End If
        Next

    End Sub

    Private Sub Form2_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler sc.getTicker.frameTick, AddressOf receiveTick
        MyBase.Finalize()
    End Sub
End Class