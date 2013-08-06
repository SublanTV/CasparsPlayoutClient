<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LibraryView
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LibraryView))
        Me.layoutHeaderItemSplit = New System.Windows.Forms.SplitContainer()
        Me.layoutHeaderTable = New System.Windows.Forms.TableLayoutPanel()
        Me.cmbRefresh = New System.Windows.Forms.Button()
        Me.layoutFilterPanel = New System.Windows.Forms.Panel()
        Me.txtFilter = New System.Windows.Forms.TextBox()
        Me.lblFilter = New System.Windows.Forms.Label()
        Me.layoutTypeFilterFlow = New System.Windows.Forms.FlowLayoutPanel()
        Me.ckbMovie = New System.Windows.Forms.CheckBox()
        Me.ckbAudio = New System.Windows.Forms.CheckBox()
        Me.ckbStill = New System.Windows.Forms.CheckBox()
        Me.ckbTemplate = New System.Windows.Forms.CheckBox()
        Me.layoutItemsFlow = New System.Windows.Forms.FlowLayoutPanel()
        CType(Me.layoutHeaderItemSplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.layoutHeaderItemSplit.Panel1.SuspendLayout()
        Me.layoutHeaderItemSplit.Panel2.SuspendLayout()
        Me.layoutHeaderItemSplit.SuspendLayout()
        Me.layoutHeaderTable.SuspendLayout()
        Me.layoutFilterPanel.SuspendLayout()
        Me.layoutTypeFilterFlow.SuspendLayout()
        Me.SuspendLayout()
        '
        'layoutHeaderItemSplit
        '
        Me.layoutHeaderItemSplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.layoutHeaderItemSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.layoutHeaderItemSplit.IsSplitterFixed = True
        Me.layoutHeaderItemSplit.Location = New System.Drawing.Point(0, 0)
        Me.layoutHeaderItemSplit.Margin = New System.Windows.Forms.Padding(0)
        Me.layoutHeaderItemSplit.MinimumSize = New System.Drawing.Size(275, 50)
        Me.layoutHeaderItemSplit.Name = "layoutHeaderItemSplit"
        Me.layoutHeaderItemSplit.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'layoutHeaderItemSplit.Panel1
        '
        Me.layoutHeaderItemSplit.Panel1.Controls.Add(Me.layoutHeaderTable)
        Me.layoutHeaderItemSplit.Panel1MinSize = 50
        '
        'layoutHeaderItemSplit.Panel2
        '
        Me.layoutHeaderItemSplit.Panel2.Controls.Add(Me.layoutItemsFlow)
        Me.layoutHeaderItemSplit.Panel2MinSize = 0
        Me.layoutHeaderItemSplit.Size = New System.Drawing.Size(285, 140)
        Me.layoutHeaderItemSplit.SplitterWidth = 1
        Me.layoutHeaderItemSplit.TabIndex = 0
        '
        'layoutHeaderTable
        '
        Me.layoutHeaderTable.ColumnCount = 5
        Me.layoutHeaderTable.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.layoutHeaderTable.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.layoutHeaderTable.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.layoutHeaderTable.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.layoutHeaderTable.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.layoutHeaderTable.Controls.Add(Me.cmbRefresh, 4, 0)
        Me.layoutHeaderTable.Controls.Add(Me.layoutFilterPanel, 0, 0)
        Me.layoutHeaderTable.Controls.Add(Me.layoutTypeFilterFlow, 0, 1)
        Me.layoutHeaderTable.Dock = System.Windows.Forms.DockStyle.Fill
        Me.layoutHeaderTable.Location = New System.Drawing.Point(0, 0)
        Me.layoutHeaderTable.Name = "layoutHeaderTable"
        Me.layoutHeaderTable.RowCount = 2
        Me.layoutHeaderTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.layoutHeaderTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.layoutHeaderTable.Size = New System.Drawing.Size(285, 50)
        Me.layoutHeaderTable.TabIndex = 0
        '
        'cmbRefresh
        '
        Me.cmbRefresh.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cmbRefresh.Image = CType(resources.GetObject("cmbRefresh.Image"), System.Drawing.Image)
        Me.cmbRefresh.Location = New System.Drawing.Point(247, 3)
        Me.cmbRefresh.Name = "cmbRefresh"
        Me.layoutHeaderTable.SetRowSpan(Me.cmbRefresh, 2)
        Me.cmbRefresh.Size = New System.Drawing.Size(35, 44)
        Me.cmbRefresh.TabIndex = 0
        Me.cmbRefresh.UseVisualStyleBackColor = True
        '
        'layoutFilterPanel
        '
        Me.layoutHeaderTable.SetColumnSpan(Me.layoutFilterPanel, 4)
        Me.layoutFilterPanel.Controls.Add(Me.txtFilter)
        Me.layoutFilterPanel.Controls.Add(Me.lblFilter)
        Me.layoutFilterPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.layoutFilterPanel.Location = New System.Drawing.Point(3, 3)
        Me.layoutFilterPanel.Name = "layoutFilterPanel"
        Me.layoutFilterPanel.Size = New System.Drawing.Size(238, 19)
        Me.layoutFilterPanel.TabIndex = 5
        '
        'txtFilter
        '
        Me.txtFilter.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFilter.Location = New System.Drawing.Point(32, 1)
        Me.txtFilter.Name = "txtFilter"
        Me.txtFilter.Size = New System.Drawing.Size(203, 20)
        Me.txtFilter.TabIndex = 1
        '
        'lblFilter
        '
        Me.lblFilter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblFilter.AutoSize = True
        Me.lblFilter.Location = New System.Drawing.Point(1, 4)
        Me.lblFilter.Name = "lblFilter"
        Me.lblFilter.Size = New System.Drawing.Size(32, 13)
        Me.lblFilter.TabIndex = 0
        Me.lblFilter.Text = "Filter:"
        '
        'layoutTypeFilterFlow
        '
        Me.layoutHeaderTable.SetColumnSpan(Me.layoutTypeFilterFlow, 4)
        Me.layoutTypeFilterFlow.Controls.Add(Me.ckbMovie)
        Me.layoutTypeFilterFlow.Controls.Add(Me.ckbAudio)
        Me.layoutTypeFilterFlow.Controls.Add(Me.ckbStill)
        Me.layoutTypeFilterFlow.Controls.Add(Me.ckbTemplate)
        Me.layoutTypeFilterFlow.Dock = System.Windows.Forms.DockStyle.Fill
        Me.layoutTypeFilterFlow.Location = New System.Drawing.Point(3, 28)
        Me.layoutTypeFilterFlow.Name = "layoutTypeFilterFlow"
        Me.layoutTypeFilterFlow.Size = New System.Drawing.Size(238, 19)
        Me.layoutTypeFilterFlow.TabIndex = 6
        '
        'ckbMovie
        '
        Me.ckbMovie.AutoSize = True
        Me.ckbMovie.Checked = True
        Me.ckbMovie.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ckbMovie.Location = New System.Drawing.Point(0, 0)
        Me.ckbMovie.Margin = New System.Windows.Forms.Padding(0)
        Me.ckbMovie.Name = "ckbMovie"
        Me.ckbMovie.Size = New System.Drawing.Size(55, 17)
        Me.ckbMovie.TabIndex = 1
        Me.ckbMovie.Text = "Movie"
        Me.ckbMovie.UseVisualStyleBackColor = True
        '
        'ckbAudio
        '
        Me.ckbAudio.AutoSize = True
        Me.ckbAudio.Location = New System.Drawing.Point(55, 0)
        Me.ckbAudio.Margin = New System.Windows.Forms.Padding(0)
        Me.ckbAudio.Name = "ckbAudio"
        Me.ckbAudio.Size = New System.Drawing.Size(53, 17)
        Me.ckbAudio.TabIndex = 2
        Me.ckbAudio.Text = "Audio"
        Me.ckbAudio.UseVisualStyleBackColor = True
        '
        'ckbStill
        '
        Me.ckbStill.AutoSize = True
        Me.ckbStill.Location = New System.Drawing.Point(108, 0)
        Me.ckbStill.Margin = New System.Windows.Forms.Padding(0)
        Me.ckbStill.Name = "ckbStill"
        Me.ckbStill.Size = New System.Drawing.Size(59, 17)
        Me.ckbStill.TabIndex = 3
        Me.ckbStill.Text = "Picture"
        Me.ckbStill.UseVisualStyleBackColor = True
        '
        'ckbTemplate
        '
        Me.ckbTemplate.AutoSize = True
        Me.ckbTemplate.Location = New System.Drawing.Point(167, 0)
        Me.ckbTemplate.Margin = New System.Windows.Forms.Padding(0)
        Me.ckbTemplate.Name = "ckbTemplate"
        Me.ckbTemplate.Size = New System.Drawing.Size(70, 17)
        Me.ckbTemplate.TabIndex = 4
        Me.ckbTemplate.Text = "Template"
        Me.ckbTemplate.UseVisualStyleBackColor = True
        '
        'layoutItemsFlow
        '
        Me.layoutItemsFlow.AutoScroll = True
        Me.layoutItemsFlow.AutoSize = True
        Me.layoutItemsFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.layoutItemsFlow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.layoutItemsFlow.Dock = System.Windows.Forms.DockStyle.Fill
        Me.layoutItemsFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.layoutItemsFlow.Location = New System.Drawing.Point(0, 0)
        Me.layoutItemsFlow.Name = "layoutItemsFlow"
        Me.layoutItemsFlow.Size = New System.Drawing.Size(285, 89)
        Me.layoutItemsFlow.TabIndex = 0
        Me.layoutItemsFlow.WrapContents = False
        '
        'LibraryView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.layoutHeaderItemSplit)
        Me.MinimumSize = New System.Drawing.Size(285, 50)
        Me.Name = "LibraryView"
        Me.Size = New System.Drawing.Size(285, 140)
        Me.layoutHeaderItemSplit.Panel1.ResumeLayout(False)
        Me.layoutHeaderItemSplit.Panel2.ResumeLayout(False)
        Me.layoutHeaderItemSplit.Panel2.PerformLayout()
        CType(Me.layoutHeaderItemSplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.layoutHeaderItemSplit.ResumeLayout(False)
        Me.layoutHeaderTable.ResumeLayout(False)
        Me.layoutFilterPanel.ResumeLayout(False)
        Me.layoutFilterPanel.PerformLayout()
        Me.layoutTypeFilterFlow.ResumeLayout(False)
        Me.layoutTypeFilterFlow.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents layoutHeaderItemSplit As System.Windows.Forms.SplitContainer
    Friend WithEvents layoutHeaderTable As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents cmbRefresh As System.Windows.Forms.Button
    Friend WithEvents ckbMovie As System.Windows.Forms.CheckBox
    Friend WithEvents ckbAudio As System.Windows.Forms.CheckBox
    Friend WithEvents ckbStill As System.Windows.Forms.CheckBox
    Friend WithEvents ckbTemplate As System.Windows.Forms.CheckBox
    Friend WithEvents lblFilter As System.Windows.Forms.Label
    Friend WithEvents layoutFilterPanel As System.Windows.Forms.Panel
    Friend WithEvents txtFilter As System.Windows.Forms.TextBox
    Friend WithEvents layoutTypeFilterFlow As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents layoutItemsFlow As System.Windows.Forms.FlowLayoutPanel

End Class
