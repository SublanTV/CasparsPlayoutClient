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

Public Class PlaylistCommandItem
    Inherits AbstractPlaylistItem
    Implements IPlaylistItem

    Private timer As System.Timers.Timer
    Private stopWatch As New Stopwatch()

    Private command As ICommand


    Public Sub New(ByVal name As String, ByRef controler As ServerControler, ByVal command As ICommand, Optional ByVal channel As Integer = -1, Optional ByVal layer As Integer = -1)
        MyBase.New(name, PlaylistItemTypes.COMMAND, controler, channel, layer)
        Me.command = command
        setChannel(channel)
        setLayer(layer)
        timer = New Timers.Timer()
        timer.Enabled = False
        AddHandler timer.Elapsed, Sub() playNextItem()
    End Sub

    Public Function getCommand() As ICommand
        Return command
    End Function

    Public Overrides Sub abort()
        timer.Enabled = False
        stopWatch.Stop()
        stopWatch.Reset()
        waiting = False
        playing = False
    End Sub

    Public Overrides Sub start()
        ' Wait if autostart not checked
        If Not isAutoStarting() Then
            raiseWaitForNext(Me)
            waiting = True
        Else
            playNextItem()
        End If
    End Sub

    Public Overrides Sub playNextItem(Optional ByRef lastPlayed As IPlaylistItem = Nothing)
        ' Start in own thread to avoid long gaps in the playlist rundown
        If getDelay() > 0 AndAlso timer.Enabled = False Then
            stopWatch.Reset()
            timer.Interval = getDelay()
            timer.Enabled = True
            stopWatch.Start()
            waiting = True
            raiseWaitForNext(Me)
        Else
            stopWatch.Stop()
            timer.Enabled = False
            stopWatch.Reset()
            waiting = False
            playing = True

            Dim t As New Threading.Thread(AddressOf execute)
            playing = True
            t.Start()
        End If
    End Sub

    Public Overrides Sub stoppedPlaying()
        playing = False
        timer.Enabled = False
        raiseStopped(Me)
    End Sub

    Private Sub execute()
        raiseStarted(Me)
        command.execute(getControler.getCommandConnection)
        stoppedPlaying()
    End Sub

    Public Overrides Sub setChannel(channel As Integer)
        MyBase.setChannel(channel)
        If Not IsNothing(getCommand) Then
            Dim p As CommandParameter(Of Integer)
            If getCommand.getParameterNames.Contains("channel") AndAlso getChannel() > 0 Then
                p = DirectCast(getCommand.getParameter("channel"), CommandParameter(Of Integer))
                p.setValue(getChannel)
            End If
        End If
    End Sub

    Public Overrides Sub setLayer(layer As Integer)
        MyBase.setLayer(layer)
        If Not IsNothing(getCommand) Then
            Dim p As CommandParameter(Of Integer)
            If getCommand.getParameterNames.Contains("layer") AndAlso getLayer() > -1 Then
                p = DirectCast(getCommand.getParameter("layer"), CommandParameter(Of Integer))
                p.setValue(getLayer)
            End If
        End If
    End Sub

    Public Overrides Function getPosition() As Long
        If timer.Enabled Then
            Return stopWatch.ElapsedMilliseconds - timer.Interval
        Else
            Return MyBase.getPosition()
        End If
    End Function


    '' functions that have to be overriden because they shoudl have no effect at all
    Public Overloads Sub addItem(ByVal item As IPlaylistItem)
        '' Nothing to do. Movie Items dosen't have child items
    End Sub

    Public Overloads Sub setChildItems(ByRef items As List(Of IPlaylistItem))
        '' Nothing to do. Movie Items dosen't have child items
    End Sub


    '' EMPTY MEMBERS
    ''---------------

    ' Commands can't be halted, loaded or paused.

    Public Overrides Sub halt()
    End Sub

    Public Overrides Sub load()
    End Sub

    Public Overrides Sub pause(frames As Long)
    End Sub

    Public Overrides Sub unPause()
    End Sub
End Class
