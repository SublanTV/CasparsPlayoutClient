Public Class MainWindow

    Private sc As ServerController
    Private mediaLib As Library
    Dim WithEvents playlistView As PlaylistView
    Dim WithEvents libraryView As LibraryView
    Delegate Sub updateDelegate()

    Private Sub MainWindow_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'logger.addLogAction(New consoleLogger(3))
        sc = New ServerController
        'sc.open("casparcg", 5250)
        mediaLib = New Library(sc)
        AddPlaylist()
        AddLibrary()
        setMonitor()
    End Sub

    Private Sub AddLibrary()
        libraryView = New LibraryView(mediaLib)
        libraryView.Dock = DockStyle.Fill
        layoutCgLib.Panel2MinSize = libraryView.MinimumSize.Width
        layoutCgLib.SplitterDistance = layoutCgLib.Width - libraryView.MinimumSize.Width - layoutCgLib.SplitterWidth
        layoutCgLib.Panel2.Controls.Add(libraryView)
    End Sub

    Private Sub AddPlaylist()

        Dim pp As New PlaylistBlockItem("Paralelle Playlist", sc)
        Dim ps As New PlaylistBlockItem("Seq. Playlist", sc)

        pp.setParallel(True)
        sc.getPlaylistRoot.addItem(ps)
        sc.getPlaylistRoot.addItem(pp)

        playlistView = New PlaylistView(sc.getPlaylistRoot)
        playlistview.Dock = DockStyle.Fill
        playlistView.Parent = layoutPlaylistSplit.Panel1
    End Sub


    Private Sub setMonitor()
        lsvPlayingMedia.View = View.List   
        With lsvPlayingMedia.Columns
            .Add("Name")
            .Add("Channel")
            .Add("Layer")
            .Add("Laufzeit")
            .Add("Verbleibend")
            .Add("% gespielt")
        End With
        'AddHandler sc.getTicker.frameTick, AddressOf Updater_Tick
        'AddHandler sc.getTicker.frameTick, AddressOf Clock_Tick
    End Sub


    Private Sub connect() Handles cmbConnect.Click
        If Not sc.isConnected Then
            sc.open(txtAddress.Text, Integer.Parse(txtPort.Text))
            For i = 1 To sc.getChannels
                cbbClearChannel.Text = i
                cbbClearChannel.Items.Add(i)
            Next
            mediaLib.refreshLibrary()
            AddHandler sc.getTicker.frameTick, AddressOf onTick
            sc.startTicker()
            libraryView.cmbRefresh.PerformClick()
        Else
            MsgBox("Allready connected")
        End If
    End Sub

    Private Sub disconnect() Handles cmbDisconnect.Click
        If sc.isConnected Then
            sc.close()
            RemoveHandler sc.getTicker.frameTick, AddressOf onTick
            libraryView.cmbRefresh.PerformClick()
        End If
    End Sub

    Private Sub clearAll() Handles cmdClearAll.Click
        For i = 1 To sc.getChannels
            sc.getCommandConnection.sendCommand(CasparCGCommandFactory.getClear(i))
        Next
    End Sub

    Private Sub clearChannel() Handles cmbClearChannel.Click
        If cbbClearChannel.Text.Length > 0 AndAlso IsNumeric(cbbClearChannel.Text) AndAlso sc.containsChannel(Integer.Parse(cbbClearChannel.Text)) Then
            sc.getCommandConnection.sendCommand(CasparCGCommandFactory.getClear(Integer.Parse(cbbClearChannel.Text)))
        End If
    End Sub

    Private Sub clearLayer() Handles cmbClearLayer.Click
        If cbbClearChannel.Text.Length > 0 AndAlso IsNumeric(cbbClearChannel.Text) AndAlso sc.containsChannel(Integer.Parse(cbbClearChannel.Text)) Then
            sc.getCommandConnection.sendCommand(CasparCGCommandFactory.getClear(Integer.Parse(cbbClearChannel.Text), nudLayerClear.Value))
        End If
    End Sub

    Private Sub onTick(ByVal sender As Object, ByVal e As frameTickEventArgs)
        playlistView.onDataChanged()
        Updater_Tick()
    End Sub


    Private Sub Updater_Tick()
        If lsvPlayingMedia.InvokeRequired Then
            Dim d As New updateDelegate(AddressOf updateView)
            Me.Invoke(d, New Object() {})
        Else
            updateView()
        End If
    End Sub

    Private Sub updateView()
        'lsvPlayingMedia.Items.Clear()
        Dim childs = sc.getPlaylistRoot.getChildItems(True)
        lsvPlayingMedia.Items.Clear()
        For Each item In childs
            If (item.isPlayable) Then 'OrElse lsvPlayingMedia.Items.ContainsKey(item.toString) Then
                'If lsvPlayingMedia.Items.ContainsKey(item.toString) Then
                '    lsvPlayingMedia.Items.RemoveByKey(item.toString)
                'End If
                If item.isPlaying Then
                    Dim line As New ListViewItem(item.getName)
                    line.Name = item.toString
                    With line.SubItems
                        .Add(item.getChannel)
                        .Add(item.getLayer)
                        .Add(ServerController.getTimeStringOfMS(item.getDuration))
                        .Add(ServerController.getTimeStringOfMS(item.getRemaining))
                        .Add(item.getPlayed)
                    End With
                    lsvPlayingMedia.Items.Add(line)
                End If
            End If
        Next
    End Sub

End Class