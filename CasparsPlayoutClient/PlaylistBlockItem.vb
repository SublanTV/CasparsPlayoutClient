Imports System.Threading

Public Class PlaylistBlockItem
    Inherits PlaylistItem
    Implements IPlaylistItem

    Private lastPlaying As Integer

    Public Sub New(ByVal name As String, ByRef controller As ServerController, Optional ByVal channel As Integer = -1, Optional ByVal layer As Integer = -1)
        MyBase.New(name, PlaylistItemTypes.BLOCK, controller, channel, layer, 0)
    End Sub

    Public Overloads Function isPlaying()
        For Each child In getChildItems()
            If child.isPlaying Then Return True
        Next
        Return False
    End Function

    'Private Sub receiveTick(ByVal sender As Object, ByVal e As frameTickEventArgs)
    '    '' Verarbeitet tickEvents
    '    ''
    '    '' wenn seq. dann schauen ob das zuletzt aktive child
    '    '' nicht mehr aktiv ist und das nächste starten

    '    If Not isParallel() Then
    '        '' Wenn das zuletzt gespielte nicht mehr aktiv ist
    '        '' das nächste starten falls möglich
    '        If lastPlaying > -1 Then
    '            If getChildItems.Count > lastPlaying + 1 AndAlso Not getChildItems().Item(lastPlaying).isPlaying Then
    '                getChildItems().Item(lastPlaying + 1).start()
    '            End If
    '        End If
    '    End If

    '    If Not isPlaying() Then
    '        abort()
    '    End If

    'End Sub

    'Public Overrides Sub stoppedPlaying()
    '    lastPlaying = getChildItems().Count
    '    MyBase.stoppedPlaying()
    'End Sub

    Public Overrides Sub start(Optional ByVal noWait As Boolean = False)
        MyBase.start(noWait)

        While isPlaying() AndAlso Not isParallel()
            Thread.Sleep(1)
        End While
        'AddHandler getController.getTicker.frameTick, AddressOf receiveTick
    End Sub

    'Public Overloads Sub abort()
    '    MyBase.abort()
    '    'RemoveHandler getController.getTicker.frameTick, AddressOf receiveTick
    'End Sub
End Class
