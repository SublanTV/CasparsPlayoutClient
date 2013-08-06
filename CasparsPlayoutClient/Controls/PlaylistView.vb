Public Class PlaylistView

    Private isInit As Boolean = False
    Private playlist As IPlaylistItem
    Private childs As List(Of PlaylistView)
    Private startCompact As Boolean
    Private waiting As Boolean = False
    Private Delegate Sub updateDelegate()
    Private cMenu As ContextMenuStrip

    Private Event changedPlaying()
    Public Event dataChanged()

    Public Sub New(ByRef playlist As IPlaylistItem, Optional ByVal startCompact As Boolean = False)
        Me.playlist = playlist
        Me.startCompact = startCompact
        childs = New List(Of PlaylistView)
        InitializeComponent()
        isInit = True
        Dim imgList As New ImageList
        imgList.ImageSize = New Size(24, 24)
        imgList.ColorDepth = 16
        imgList.Images.Add(Image.FromFile("img/Play-Green-Button-icon.png"))
        imgList.Images.Add(Image.FromFile("img/Stop-Red-Button-icon.png"))
        imgList.Images.Add(Image.FromFile("img/Play-Blue-Button-icon.png"))
        cmbToggleButton.ImageList = imgList
        init() 
        AddHandler playlist.waitForNext, AddressOf waitForNext
    End Sub

    Private Sub init()
        RaiseEvent dataChanged()

        layoutHeaderContentSplit_DoubleClick(Nothing, Nothing)

        '' ChildLayout füllen
        Select Case playlist.getItemType
            Case PlaylistItem.PlaylistItemTypes.MOVIE, PlaylistItem.PlaylistItemTypes.AUDIO, PlaylistItem.PlaylistItemTypes.STILL

            Case PlaylistItem.PlaylistItemTypes.TEMPLATE

            Case PlaylistItem.PlaylistItemTypes.BLOCK
                '' BlockItem, schauen ob childs geladen werden können
                For Each item In playlist.getChildItems(False)
                    addChild(item)
                Next
        End Select
        layoutHeaderContentSplit_DoubleClick(Nothing, Nothing)
        If startCompact Then layoutHeaderContentSplit_DoubleClick(Nothing, Nothing)


        '' ContexMenü hinzufügen
        cMenu = New ContextMenuStrip
        cMenu.Items.Add(New ToolStripMenuItem("Add Block", Nothing, New EventHandler(AddressOf addBlockItem)))
        cMenu.Items.Add(New ToolStripMenuItem("Remove item", Nothing, New EventHandler(AddressOf removeItem)))
        Me.ContextMenuStrip = cMenu
    End Sub

    Public Sub onDataChanged() Handles Me.dataChanged
        If Me.InvokeRequired Then
            Dim d As New updateDelegate(AddressOf Me.setData)
            Me.Invoke(d)
        Else
            setData()
        End If
        For Each child In childs
            child.onDataChanged()
        Next
    End Sub

    Private Sub setData()
        '' Werte eintragen
        With playlist
            If Not txtName.Focused Then Me.txtName.Text = .getName
            Me.nudChannel.Value = Math.Max(.getChannel, 0)
            Me.nudLayer.Value = Math.Max(.getLayer, -1)
            Me.txtPosition.Text = ServerController.getTimeStringOfMS(.getPosition)
            Me.txtDuration.Text = ServerController.getTimeStringOfMS(.getDuration)
            Me.txtRemaining.Text = ServerController.getTimeStringOfMS(.getRemaining)
            Me.txtDelay.Text = .getDelay
            Me.ckbAuto.Checked = .isAutoStarting
            Me.ckbParallel.Checked = .isParallel
            Me.ckbLoop.Checked = .isLooping
            Me.pbPlayed.Value = .getPlayed
        End With
        RaiseEvent changedPlaying()
    End Sub

    Private Sub addChild(ByRef childList As IPlaylistItem)
        Dim child As New PlaylistView(childList, startCompact)
        child.Parent = Me.layoutChild
        child.Show()
        childs.Add(child)
    End Sub

    Friend Sub atResize(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Resize
        For Each child In childs
            ' Let child use the whole width
            child.Width = Me.layoutChild.ClientRectangle.Width
        Next
    End Sub

    Private Sub layoutHeaderContentSplit_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblExpand.Click, layoutHeaderContentSplit.DoubleClick
        If layoutHeaderContentSplit.Panel2Collapsed Then
            lblExpand.Text = "-"
            If layoutChild.HasChildren Then
                Me.Height = layoutHeaderContentSplit.Panel1.Height + layoutChild.Height + (2 * layoutHeaderContentSplit.Panel2.Padding.Vertical) + (2 * layoutHeaderContentSplit.Panel2.Margin.Vertical) + 10
            Else
                Me.Height = layoutHeaderContentSplit.Panel1.Height + layoutHeaderContentSplit.Panel2MinSize + (2 * layoutHeaderContentSplit.Panel2.Padding.Vertical) + (2 * layoutHeaderContentSplit.Panel2.Margin.Vertical) + 10
            End If
        Else
            Me.Height = layoutHeaderContentSplit.Panel1MinSize
            lblExpand.Text = "+"
        End If
        layoutHeaderContentSplit.Panel2Collapsed = Not layoutHeaderContentSplit.Panel2Collapsed
    End Sub

    Private Sub cmbToggleButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToggleButton.Click
        If waiting Then
            waiting = False
            playlist.playNextItem()
        ElseIf playlist.isPlaying Then
            playlist.abort()
        Else
            ' Damit nicht gewartet wird falls der button manuel betätigt wurde aber auto nicht gesetzt ist
            If playlist.getController.containsChannel(playlist.getChannel) OrElse Not playlist.isPlayable Then
                playlist.playNextItem()
                playlist.start(True)
            Else
                MsgBox("Error, unknown channel.")
            End If
        End If
        RaiseEvent changedPlaying()
    End Sub

    Friend Sub onChangedPlayingState() Handles Me.changedPlaying
        If playlist.isPlaying And Not playlist.isWaiting Then
            txtName.BackColor = Color.Orange
            layoutContentSplit.Panel1.BackColor = Color.Orange
            'cmbToggleButton.Text = "o"
            cmbToggleButton.ImageIndex = 1
            'layoutInfos.Enabled = False
            layoutButton.Enabled = False
            nudChannel.Enabled = False
            nudLayer.Enabled = False
            txtName.ReadOnly = True
        ElseIf waiting Then
            txtName.BackColor = Color.LightBlue
            layoutContentSplit.Panel1.BackColor = Color.LightBlue
            cmbToggleButton.ImageIndex = 2
            layoutButton.Enabled = False
            nudChannel.Enabled = False
            nudLayer.Enabled = False
            txtName.ReadOnly = True
        Else
            txtName.BackColor = Color.LightGreen
            layoutContentSplit.Panel1.BackColor = Color.LightGreen
            'cmbToggleButton.Text = ">"
            cmbToggleButton.ImageIndex = 0
            'layoutInfos.Enabled = True
            layoutButton.Enabled = True
            nudChannel.Enabled = True
            nudLayer.Enabled = True
            txtName.ReadOnly = False
        End If
        For Each child In childs
            child.onChangedPlayingState()
        Next
    End Sub

    Private Sub ckbParallel_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ckbParallel.CheckedChanged
        If isInit Then
            playlist.setParallel(ckbParallel.Checked)
        End If
    End Sub

    Private Sub ckbAuto_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ckbAuto.CheckedChanged
        If isInit Then
            playlist.setAutoStart(ckbAuto.Checked)
        End If
    End Sub

    Private Sub ckbLoop_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ckbLoop.CheckedChanged
        If isInit Then
            playlist.setLooping(ckbLoop.Checked)
        End If
    End Sub

    Private Sub nudLayer_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudLayer.ValueChanged
        If isInit Then
            If nudLayer.Value < 0 Then
                nudLayer.BackColor = Color.Red
                playlist.setLayer(-1)
            Else
                nudLayer.BackColor = Color.White
                playlist.setLayer(nudLayer.Value)
            End If
        End If
    End Sub

    Private Sub nudChannel_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudChannel.ValueChanged
        If isInit Then
            If nudChannel.Value < 1 OrElse Not playlist.getController.containsChannel(nudChannel.Value) Then
                nudChannel.BackColor = Color.Red
                playlist.setChannel(-1)
            Else
                nudChannel.BackColor = Color.White
                playlist.setChannel(nudChannel.Value)
            End If
        End If
    End Sub

    Private Sub txtDelay_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDelay.Leave
        playlist.setDelay(TimeSpan.Parse(txtDelay.Text).TotalMilliseconds)
    End Sub

    Private Sub txtName_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtName.Leave
        playlist.setName(txtName.Text)
    End Sub

    Private Sub waitForNext()
        waiting = True
    End Sub


    ''
    '' Hinzufügen / entfernen von Blöcken
    ''

    Public Sub addBlockItem()
        Dim bi As New PlaylistBlockItem("BlockItem", playlist.getController)
        playlist.addItem(bi)
        addChild(bi)
    End Sub

    Public Sub removeItem()
        If Not IsNothing(playlist.getParent) Then
            playlist.getParent.removeChild(playlist)
            Me.Parent.Controls.Remove(Me)
        End If
    End Sub


    ''
    '' DragDrop verarbeiten
    '
    Private Overloads Sub handleDragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles Me.DragDrop
        If e.Data.GetDataPresent("CasparsPlayoutClient.CasparCGMovie") Then
            ''
            '' Neue MediaItems einfügen
            ''
            Dim media As CasparCGMedia = e.Data.GetData("CasparsPlayoutClient.CasparCGMovie")
            Dim child As IPlaylistItem
            child = New PlaylistMovieItem(media.getFullName, playlist.getController, media)
            playlist.addItem(child)
            addChild(child)
        ElseIf e.Data.GetDataPresent("CasparsPlayoutClient.PlaylistView") Then
            ''
            '' PlaylistItems verschieben
            ''
            Dim item As PlaylistView = e.Data.GetData("CasparsPlayoutClient.PlaylistView")
            If Not IsNothing(item.playlist.getParent) Then
                ' Playlist von seiner alten liste lösen
                item.playlist.getParent.removeChild(item.playlist)
                If Not IsNothing(playlist.getParent) Then
                    ' und an den platz dieser Playlist in dem Vater einfügen
                    playlist.getParent.insertChildAt(item.playlist, playlist)
                    'jetzt noch die Controls entsprechend verschieben.
                    item.Parent = Me.Parent
                    Me.Parent.Controls.SetChildIndex(item, Me.Parent.Controls.GetChildIndex(Me))
                Else
                    ' oder, wenn es auf den Freiraum der neuen liste 
                    playlist.addItem(item.playlist)
                    item.Parent = Me.layoutChild
                End If
            End If
        End If
    End Sub

    Private Overloads Sub handleDragEnter(ByVal sender As Object, ByVal e As DragEventArgs) Handles Me.dragEnter
        ' Check the format of the data being dropped. 
        If (e.Data.GetDataPresent("CasparsPlayoutClient.CasparCGMovie")) Then
            ' Display the copy cursor. 
            e.Effect = DragDropEffects.Copy
        ElseIf e.Data.GetDataPresent("CasparsPlayoutClient.PlaylistView") Then
            e.Effect = DragDropEffects.Move
        Else
            ' Display the no-drop cursor. 
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private MouseIsDown As Boolean = False
    Private Sub handleMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseDown, layoutButton.MouseDown, layoutContentSplit.MouseDown, layoutInfos.MouseDown, layoutName.MouseDown, layoutHeaderTable.MouseDown
        ' Set a flag to show that the mouse is down. 
        MouseIsDown = True
    End Sub
    Private Sub handleMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseMove, layoutButton.MouseMove, layoutContentSplit.MouseMove, layoutInfos.MouseMove, layoutName.MouseMove, layoutHeaderTable.MouseMove
        If MouseIsDown Then
            ' Initiate dragging. 
            DoDragDrop(Me, DragDropEffects.Move)
        End If
        MouseIsDown = False
    End Sub

End Class
