Public Class LibraryView

    Public Property Library As Library

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(ByVal library As Library)
        Me.Library = library
        InitializeComponent()
        refreshList()
    End Sub

    '
    '' Hilfsmethoden
    '
    Private Sub applyFilter() Handles ckbAudio.CheckedChanged, ckbMovie.CheckedChanged, ckbStill.CheckedChanged, ckbTemplate.CheckedChanged, txtFilter.TextChanged
        Dim filteredList As New List(Of CasparCGMedia)

        If ckbAudio.Checked Then
            filteredList.AddRange(Library.getItemsOfType(CasparCGMedia.MediaType.AUDIO))
        End If
        If ckbMovie.Checked Then
            filteredList.AddRange(Library.getItemsOfType(CasparCGMedia.MediaType.MOVIE))
        End If
        If ckbStill.Checked Then
            filteredList.AddRange(Library.getItemsOfType(CasparCGMedia.MediaType.STILL))
        End If
        If ckbTemplate.Checked Then
            filteredList.AddRange(Library.getItemsOfType(CasparCGMedia.MediaType.TEMPLATE))
        End If

        If txtFilter.Text.Length > 0 Then
            For Each item In filteredList

                '' ist das item im Filterergebnis?
                If item.getFullName.ToUpper Like txtFilter.Text.ToUpper & "*" OrElse item.getName.ToUpper Like txtFilter.Text.ToUpper & "*" Then
                    ' Gibt es schon ein entsprechendes control?
                    If layoutItemsFlow.Controls.ContainsKey(item.getFullName) Then
                        ' Also sichtbar machen
                        layoutItemsFlow.Controls.Item(layoutItemsFlow.Controls.IndexOfKey(item.getFullName)).Visible = True
                    Else
                        ' Noch kein Control da, also hinzufügen (Sollte eigentlich nicht vorkommen)
                        addMediaItem(item)
                    End If
                Else
                    ' Das item ist nicht im ergebnis, wenn es ein entsprechendes Control gibt wird es unsichtbar gemacht.
                    If layoutItemsFlow.Controls.ContainsKey(item.getFullName) Then
                        layoutItemsFlow.Controls.Item(layoutItemsFlow.Controls.IndexOfKey(item.getFullName)).Visible = False
                    End If
                End If
            Next
        Else
            For Each item As LibraryViewItem In layoutItemsFlow.Controls
                If filteredList.Contains(item.MediaItem) Then
                    ' Also sichtbar machen
                    item.Visible = True
                Else
                    ' Noch kein Control da, also hinzufügen (Sollte eigentlich nicht vorkommen)
                    item.Visible = False
                End If
            Next
        End If
    End Sub

    Private Sub refreshList()
        layoutItemsFlow.Controls.Clear()
        If Not IsNothing(Library) Then
            For Each item In Library.getItems
                addMediaItem(item)
            Next
        End If
    End Sub

    ''' <summary>
    ''' Adds a CasparCGMedia to the LibraryView
    ''' </summary>
    ''' <param name="mediaItem"></param>
    ''' <remarks></remarks>
    Private Sub addMediaItem(ByRef mediaItem As CasparCGMedia)
        Dim libItem As New LibraryViewItem(mediaItem)
        layoutItemsFlow.Controls.Add(libItem) 
        libItem.Width = libItem.Parent.ClientRectangle.Width - libItem.Parent.Margin.Horizontal
    End Sub

    '
    '' Ereignis verarbeitung
    '
    Private Sub cmbRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbRefresh.Click
        If Not IsNothing(Library) Then
            Library.refreshLibrary()
        End If
        refreshList()
        applyFilter()
    End Sub

    Private Sub LibraryView_ClientSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ClientSizeChanged, layoutItemsFlow.ClientSizeChanged
        For Each item As Control In layoutItemsFlow.Controls
            'item.Width = layoutItemsFlow.ClientRectangle.Width
            item.Width = item.Parent.ClientRectangle.Width - item.Parent.Margin.Horizontal
        Next
    End Sub

End Class
