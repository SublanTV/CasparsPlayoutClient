<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
        Me.lstFrame = New System.Windows.Forms.ListBox()
        Me.lstOffset = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'lstFrame
        '
        Me.lstFrame.FormattingEnabled = True
        Me.lstFrame.Location = New System.Drawing.Point(12, 12)
        Me.lstFrame.Name = "lstFrame"
        Me.lstFrame.Size = New System.Drawing.Size(64, 121)
        Me.lstFrame.TabIndex = 3
        '
        'lstOffset
        '
        Me.lstOffset.FormattingEnabled = True
        Me.lstOffset.Location = New System.Drawing.Point(82, 12)
        Me.lstOffset.Name = "lstOffset"
        Me.lstOffset.Size = New System.Drawing.Size(49, 121)
        Me.lstOffset.TabIndex = 4
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(143, 144)
        Me.Controls.Add(Me.lstOffset)
        Me.Controls.Add(Me.lstFrame)
        Me.Name = "Form2"
        Me.Text = "Form2"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lstFrame As System.Windows.Forms.ListBox
    Friend WithEvents lstOffset As System.Windows.Forms.ListBox
End Class
