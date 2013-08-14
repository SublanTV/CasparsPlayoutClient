'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'' Author: Christopher Diekkamp
'' Email: christopher@development.diekkamp.de
'' GitHub: https://github.com/mcdikki
'' 
'' This software is licensed under the 
'' GNU General Public License Version 3 (GPLv3).
'' See http://www.gnu.org/licenses/gpl-3.0-standalone.html 
'' for a copy of the license.
''
'' You are free to copy, use and modify this software.
'' Please let me know of any changes and improvements you made to it.
''
'' Thank you!
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Imports CasparCGNETConnector
Imports logger

Public Class MainWindow

    Private sc As ServerControler
    Private mediaLib As Library
    Private WithEvents playlistView As PlaylistView
    Private WithEvents libraryView As LibraryView
    Private Delegate Sub updateDelegate()
    Private timer As Timers.Timer

    Private Sub MainWindow_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        logger.addLogAction(New consoleLogger(3))
        'logger.addLogAction(New fileLogger(3, "c:\daten\cpc2.log", True, False))
        sc = New ServerControler
        mediaLib = New Library(sc)
        AddPlaylist()
        AddLibrary()
        initInfo()
    End Sub

    Private Sub AddLibrary()
        libraryView = New LibraryView(mediaLib)
        libraryView.Dock = DockStyle.Fill
        layoutCgLib.Panel2MinSize = libraryView.MinimumSize.Width
        layoutCgLib.SplitterDistance = layoutCgLib.Width - libraryView.MinimumSize.Width - layoutCgLib.SplitterWidth
        layoutCgLib.Panel2.Controls.Add(libraryView)
    End Sub

    Private Sub AddPlaylist()
        playlistView = New PlaylistView(sc.getPlaylistRoot)
        playlistView.Dock = DockStyle.Fill
        playlistView.Parent = layoutPlaylistSplit.Panel1
    End Sub

    Private Sub connect() Handles cmbConnect.Click
        If Not sc.isConnected Then
            If sc.open(txtAddress.Text, Integer.Parse(txtPort.Text)) Then
                cmbConnect.Enabled = False
                For i = 1 To sc.getChannels
                    cbbClearChannel.Text = i
                    cbbClearChannel.Items.Add(i)
                Next
                AddHandler sc.getTicker.frameTick, AddressOf onTick
                sc.startTicker()
                libraryView.cmbRefresh.PerformClick()
                cmbDisconnect.Enabled = True
            Else
                cmbConnect.Enabled = True
                cmbDisconnect.Enabled = False
                MsgBox("Error: Could not connect to " & txtAddress.Text & ":" & txtPort.Text, vbCritical + vbOKOnly, "Error - not connected")
            End If
        Else
            MsgBox("Allready connected")
        End If
    End Sub

    Private Sub initInfo()
        timer = New Timers.Timer(100)
        timer.Enabled = True
        AddHandler timer.Elapsed, AddressOf updateClock
        AddHandler timer.Elapsed, AddressOf updateDate
        AddHandler timer.Elapsed, AddressOf updateStatus
        updateClock()
        updateDate()
        updateStatus()
    End Sub

    Private Sub updateClock()
        If lblClock.InvokeRequired Then
            Dim d = New updateDelegate(AddressOf updateClock)
            lblClock.Invoke(d)
        Else
            lblClock.Text = Now.TimeOfDay.ToString.Substring(0, 8)
        End If
    End Sub

    Private Sub updateDate()
        If lblDate.InvokeRequired Then
            Dim d = New updateDelegate(AddressOf updateDate)
            lblDate.Invoke(d)
        Else
            lblDate.Text = Now.Date.ToShortDateString
        End If
    End Sub

    Private Sub updateStatus()
        If lblStatus.InvokeRequired Then
            Dim d = New updateDelegate(AddressOf updateStatus)
            lblStatus.Invoke(d)
        Else
            If sc.isConnected Then
                lblStatus.ForeColor = Color.Lime
                If sc.getPlaylistRoot.isPlaying OrElse sc.getPlaylistRoot.isWaiting Then
                    lblStatus.Text = "Running"
                Else
                    lblStatus.Text = "Idle"
                End If
            Else
                lblStatus.Text = "Stopped"
                lblStatus.ForeColor = Color.Red
            End If
        End If
    End Sub

    Private Sub disconnect() Handles cmbDisconnect.Click
        If sc.isConnected Then
            libraryView.Library.abortUpdate()
            cmbDisconnect.Enabled = False
            sc.close()
            RemoveHandler sc.getTicker.frameTick, AddressOf onTick
            libraryView.Library.refreshLibrary()
            playlistView.onDataChanged()
            cmbConnect.Enabled = True
        End If
    End Sub

    Private Sub clearAll() Handles cmdClearAll.Click
        Dim cmd As New ClearCommand()
        Dim p = CTypeDynamic(cmd.getParameter("channel"), cmd.getParameter("channel").getGenericParameterType)
        For i = 1 To sc.getChannels
            p.setValue(i)
            cmd.execute(sc.getCommandConnection)
        Next
    End Sub

    Private Sub clearChannel() Handles cmbClearChannel.Click
        If cbbClearChannel.Text.Length > 0 AndAlso IsNumeric(cbbClearChannel.Text) AndAlso sc.containsChannel(Integer.Parse(cbbClearChannel.Text)) Then
            Dim cmd As New ClearCommand(Integer.Parse(cbbClearChannel.Text))
            cmd.execute(sc.getCommandConnection)
        End If
    End Sub

    Private Sub clearLayer() Handles cmbClearLayer.Click
        If cbbClearChannel.Text.Length > 0 AndAlso IsNumeric(cbbClearChannel.Text) AndAlso sc.containsChannel(Integer.Parse(cbbClearChannel.Text)) Then
            Dim cmd As New ClearCommand(Integer.Parse(cbbClearChannel.Text), nudLayerClear.Value)
            cmd.execute(sc.getCommandConnection)
        End If
    End Sub

    Private Sub onTick()
        playlistView.onDataChanged()
    End Sub


End Class