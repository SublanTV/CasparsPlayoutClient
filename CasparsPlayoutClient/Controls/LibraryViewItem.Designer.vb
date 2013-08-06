<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LibraryViewItem
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.layoutHeaderTable = New System.Windows.Forms.TableLayoutPanel()
        Me.cmbAdd = New System.Windows.Forms.Button()
        Me.lblName = New System.Windows.Forms.Label()
        Me.layoutHeaderInfoPanel = New System.Windows.Forms.Panel()
        Me.lblExpand = New System.Windows.Forms.Label()
        Me.lblType = New System.Windows.Forms.Label()
        Me.lblDuration = New System.Windows.Forms.Label()
        Me.toolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.layoutHeaderTable.SuspendLayout()
        Me.layoutHeaderInfoPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'layoutHeaderTable
        '
        Me.layoutHeaderTable.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.layoutHeaderTable.ColumnCount = 2
        Me.layoutHeaderTable.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.layoutHeaderTable.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.layoutHeaderTable.Controls.Add(Me.cmbAdd, 1, 0)
        Me.layoutHeaderTable.Controls.Add(Me.lblName, 0, 0)
        Me.layoutHeaderTable.Controls.Add(Me.layoutHeaderInfoPanel, 0, 1)
        Me.layoutHeaderTable.Location = New System.Drawing.Point(0, 0)
        Me.layoutHeaderTable.Margin = New System.Windows.Forms.Padding(0)
        Me.layoutHeaderTable.MinimumSize = New System.Drawing.Size(145, 30)
        Me.layoutHeaderTable.Name = "layoutHeaderTable"
        Me.layoutHeaderTable.RowCount = 2
        Me.layoutHeaderTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.layoutHeaderTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.layoutHeaderTable.Size = New System.Drawing.Size(145, 30)
        Me.layoutHeaderTable.TabIndex = 1
        '
        'cmbAdd
        '
        Me.cmbAdd.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cmbAdd.Location = New System.Drawing.Point(105, 0)
        Me.cmbAdd.Margin = New System.Windows.Forms.Padding(0)
        Me.cmbAdd.Name = "cmbAdd"
        Me.layoutHeaderTable.SetRowSpan(Me.cmbAdd, 2)
        Me.cmbAdd.Size = New System.Drawing.Size(40, 30)
        Me.cmbAdd.TabIndex = 0
        Me.cmbAdd.Text = "Add"
        Me.cmbAdd.UseVisualStyleBackColor = True
        '
        'lblName
        '
        Me.lblName.AutoEllipsis = True
        Me.lblName.AutoSize = True
        Me.lblName.BackColor = System.Drawing.Color.Transparent
        Me.lblName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblName.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblName.Location = New System.Drawing.Point(0, 0)
        Me.lblName.Margin = New System.Windows.Forms.Padding(0)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(105, 15)
        Me.lblName.TabIndex = 1
        Me.lblName.Text = "Name"
        Me.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'layoutHeaderInfoPanel
        '
        Me.layoutHeaderInfoPanel.AutoSize = True
        Me.layoutHeaderInfoPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.layoutHeaderInfoPanel.Controls.Add(Me.lblExpand)
        Me.layoutHeaderInfoPanel.Controls.Add(Me.lblType)
        Me.layoutHeaderInfoPanel.Controls.Add(Me.lblDuration)
        Me.layoutHeaderInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.layoutHeaderInfoPanel.Location = New System.Drawing.Point(0, 15)
        Me.layoutHeaderInfoPanel.Margin = New System.Windows.Forms.Padding(0)
        Me.layoutHeaderInfoPanel.Name = "layoutHeaderInfoPanel"
        Me.layoutHeaderInfoPanel.Size = New System.Drawing.Size(105, 15)
        Me.layoutHeaderInfoPanel.TabIndex = 2
        '
        'lblExpand
        '
        Me.lblExpand.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblExpand.AutoEllipsis = True
        Me.lblExpand.AutoSize = True
        Me.lblExpand.BackColor = System.Drawing.Color.Transparent
        Me.lblExpand.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblExpand.Location = New System.Drawing.Point(3, 0)
        Me.lblExpand.Margin = New System.Windows.Forms.Padding(0)
        Me.lblExpand.Name = "lblExpand"
        Me.lblExpand.Size = New System.Drawing.Size(15, 16)
        Me.lblExpand.TabIndex = 2
        Me.lblExpand.Text = "+"
        Me.lblExpand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.toolTip.SetToolTip(Me.lblExpand, "Show all Metadata to this media")
        '
        'lblType
        '
        Me.lblType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblType.AutoEllipsis = True
        Me.lblType.AutoSize = True
        Me.lblType.BackColor = System.Drawing.Color.Transparent
        Me.lblType.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblType.Location = New System.Drawing.Point(58, 5)
        Me.lblType.Margin = New System.Windows.Forms.Padding(0)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(37, 9)
        Me.lblType.TabIndex = 1
        Me.lblType.Text = "Template"
        '
        'lblDuration
        '
        Me.lblDuration.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblDuration.AutoEllipsis = True
        Me.lblDuration.AutoSize = True
        Me.lblDuration.BackColor = System.Drawing.Color.Transparent
        Me.lblDuration.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDuration.Location = New System.Drawing.Point(12, 5)
        Me.lblDuration.Margin = New System.Windows.Forms.Padding(0)
        Me.lblDuration.Name = "lblDuration"
        Me.lblDuration.Size = New System.Drawing.Size(43, 9)
        Me.lblDuration.TabIndex = 0
        Me.lblDuration.Text = "00:00:00.00"
        '
        'LibraryViewItem
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.layoutHeaderTable)
        Me.MinimumSize = New System.Drawing.Size(145, 32)
        Me.Name = "LibraryViewItem"
        Me.Size = New System.Drawing.Size(143, 30)
        Me.layoutHeaderTable.ResumeLayout(False)
        Me.layoutHeaderTable.PerformLayout()
        Me.layoutHeaderInfoPanel.ResumeLayout(False)
        Me.layoutHeaderInfoPanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents layoutHeaderTable As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents cmbAdd As System.Windows.Forms.Button
    Friend WithEvents lblName As System.Windows.Forms.Label
    Friend WithEvents layoutHeaderInfoPanel As System.Windows.Forms.Panel
    Friend WithEvents lblExpand As System.Windows.Forms.Label
    Friend WithEvents lblType As System.Windows.Forms.Label
    Friend WithEvents lblDuration As System.Windows.Forms.Label
    Friend WithEvents toolTip As System.Windows.Forms.ToolTip

End Class
