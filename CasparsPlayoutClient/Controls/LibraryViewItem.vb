Public Class LibraryViewItem

    Public Property MediaItem As CasparCGMedia

    Public Sub New(ByVal mediaItem As CasparCGMedia)
        Me.MediaItem = mediaItem
        InitializeComponent()
        init()
    End Sub

    Private Sub init()
        If Not IsNothing(MediaItem) Then
            Name = MediaItem.getFullName
            toolTip.SetToolTip(Me, MediaItem.getFullName)
            toolTip.SetToolTip(Me.lblName, MediaItem.getFullName)
            toolTip.SetToolTip(Me.cmbAdd, "Add " & MediaItem.getFullName & " to playlist")
            lblName.Text = MediaItem.getName
            lblType.Text = MediaItem.getMediaType.ToString
            If MediaItem.containsInfo("Duration") Then
                lblDuration.Text = MediaItem.getInfo("Duration")
            Else
                lblDuration.Visible = False
            End If
        End If
    End Sub

    Private Sub lblExpand_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblExpand.Click
        Dim metadata As String = "Metadata"
        For Each info In MediaItem.getInfos.Keys
            metadata = metadata & vbNewLine & info & ": " & MediaItem.getInfo(info)
        Next
        MsgBox(metadata, vbOKOnly, "Metadata for " & MediaItem.getName)
    End Sub


    '' DragDrop verarbeiten

    Private MouseIsDown As Boolean = False
    Private Sub handleMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseDown, layoutHeaderInfoPanel.MouseDown, layoutHeaderTable.MouseDown, cmbAdd.MouseDown, lblDuration.MouseDown, lblExpand.MouseDown, lblName.MouseDown, lblType.MouseDown
        ' Set a flag to show that the mouse is down. 
        MouseIsDown = True
    End Sub
    Private Sub handleMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseMove, layoutHeaderInfoPanel.MouseMove, layoutHeaderTable.MouseMove, cmbAdd.MouseMove, lblDuration.MouseMove, lblExpand.MouseMove, lblName.MouseMove, lblType.MouseMove
        If MouseIsDown Then
            ' Initiate dragging. 
            DoDragDrop(MediaItem, DragDropEffects.Copy)
        End If
        MouseIsDown = False
    End Sub

End Class
