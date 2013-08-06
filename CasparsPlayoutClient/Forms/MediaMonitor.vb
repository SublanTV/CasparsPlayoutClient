Public Class MediaMonitor

    Private sc As ServerController
    Delegate Sub updateDelegate()
    Delegate Sub clockDelegate(ByVal state As Dictionary(Of Integer, Long))

    Public Sub New(ByRef controller As ServerController)
        MyBase.New()
        InitializeComponent()
        Me.sc = controller
    End Sub

    Private Sub MediaMonitor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lsvPlayingMedia.View = View.Details
        With lsvPlayingMedia.Columns
            .Add("Name")
            .Add("Channel")
            .Add("Layer")
            .Add("Laufzeit")
            .Add("Verbleibend")
            .Add("% gespielt")
        End With
        AddHandler sc.getTicker.frameTick, AddressOf Updater_Tick
        AddHandler sc.getTicker.frameTick, AddressOf Clock_Tick
    End Sub

    Private Sub Clock_Tick(ByVal sender As Object, ByVal e As frameTickEventArgs)
        If lblClock.InvokeRequired Then
            Dim d As New clockDelegate(AddressOf updateClock)
            Me.Invoke(d, New Object() {e.result})
        Else
            updateClock(e.result)
        End If
    End Sub

    Private Sub updateClock(ByVal state As Dictionary(Of Integer, Long))
        If state.ContainsKey(1) Then
            lblClock.Text = state.Item(1)
        End If
    End Sub

    Private Sub Updater_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If lsvPlayingMedia.InvokeRequired Then
            Dim d As New updateDelegate(AddressOf updateView)
            Me.Invoke(d, New Object() {})
        Else
            updateView()
        End If
    End Sub

    Private Sub updateView()
        'lsvPlayingMedia.Items.Clear()
        For Each item In sc.getPlaylistRoot.getChildItems(True)
            If (item.isPlayable AndAlso item.isPlaying) Then 'OrElse lsvPlayingMedia.Items.ContainsKey(item.toString) Then
                Dim line As New ListViewItem(item.getName)
                line.Name = item.toString
                With line.SubItems
                    .Add(item.getChannel)
                    .Add(item.getLayer)
                    .Add(ServerController.getTimeStringOfMS(item.getDuration))
                    .Add(ServerController.getTimeStringOfMS(item.getRemaining))
                    .Add(item.getPlayed)
                End With
                If lsvPlayingMedia.Items.ContainsKey(item.toString) Then
                    lsvPlayingMedia.Items(item.toString).Remove()
                End If
                lsvPlayingMedia.Items.Add(line)
            End If
        Next
    End Sub
End Class