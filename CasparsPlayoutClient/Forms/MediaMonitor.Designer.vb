<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MediaMonitor
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.lsvPlayingMedia = New System.Windows.Forms.ListView()
        Me.tmUpdater = New System.Windows.Forms.Timer(Me.components)
        Me.lblClock = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lsvPlayingMedia
        '
        Me.lsvPlayingMedia.GridLines = True
        Me.lsvPlayingMedia.Location = New System.Drawing.Point(12, 60)
        Me.lsvPlayingMedia.Name = "lsvPlayingMedia"
        Me.lsvPlayingMedia.Size = New System.Drawing.Size(1011, 454)
        Me.lsvPlayingMedia.TabIndex = 0
        Me.lsvPlayingMedia.UseCompatibleStateImageBehavior = False
        '
        'tmUpdater
        '
        Me.tmUpdater.Interval = 500
        '
        'lblClock
        '
        Me.lblClock.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblClock.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblClock.Location = New System.Drawing.Point(446, 9)
        Me.lblClock.Name = "lblClock"
        Me.lblClock.Size = New System.Drawing.Size(140, 32)
        Me.lblClock.TabIndex = 1
        Me.lblClock.Text = "00:00:00.00"
        '
        'MediaMonitor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1035, 526)
        Me.Controls.Add(Me.lblClock)
        Me.Controls.Add(Me.lsvPlayingMedia)
        Me.Name = "MediaMonitor"
        Me.Text = "MediaMonitor"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lsvPlayingMedia As System.Windows.Forms.ListView
    Friend WithEvents tmUpdater As System.Windows.Forms.Timer
    Friend WithEvents lblClock As System.Windows.Forms.Label
End Class
