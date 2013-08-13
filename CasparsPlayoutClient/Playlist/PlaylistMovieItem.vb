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

Public Class PlaylistMovieItem
    Inherits AbstractPlaylistItem
    Implements IPlaylistItem

    Private media As CasparCGMovie
    Private timer As System.Timers.Timer

    ''' <summary>
    ''' Create a PlaylistMovieItem. If a duration is given and smaller than the original duration of the file, the file will only be played for that long.
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="controller"></param>
    ''' <param name="movie"></param>
    ''' <param name="duration"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String, ByRef controller As ServerControler, ByVal movie As CasparCGMovie, Optional ByVal channel As Integer = -1, Optional ByVal layer As Integer = -1, Optional ByVal duration As Long = -1)
        MyBase.New(name, PlaylistItemTypes.MOVIE, controller, channel, layer, duration)
        If Not IsNothing(movie) Then
            If movie.getInfos.Count = 0 Then
                movie.fillMediaInfo(getControler.getTestConnection, getControler.getTestChannel)
            End If
            If movie.containsInfo("nb-frames") AndAlso (duration > getControler.getOriginalMediaDuration(movie) OrElse duration = -1) Then
                setDuration(getControler.getMediaDuration(movie, channel))
            End If
            media = movie
            timer = New Timers.Timer()
            timer.Enabled = False
            AddHandler timer.Elapsed, Sub() playNextItem()
        Else
            logger.critical("PlaylistMovieItem.new: ERROR: Given movie was nothing - Stopping now")
            Throw New Exception("NOTHING not allowed")
        End If
    End Sub


    '' Methoden die überschrieben werden müssen weil sie andere oder mehr functionen haben
    ''-------------------------------------------------------------------------------------
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
        waiting = False
        playing = True

        '' CMD an ServerController schicken
        logger.log("PlaylistMovieItem.start: Starte " & getChannel() & "-" & getLayer() & ": " & getMedia.toString)

        If getDelay() > 0 AndAlso timer.Enabled = False Then
            timer.Interval = getDelay()
            timer.Enabled = True
        Else
            timer.Enabled = False
            If getControler.containsChannel(getChannel) AndAlso getLayer() > -1 Then
                Dim cmd As ICommand = New PlayCommand(getChannel, getLayer, getMedia, isLooping, False)

                'Dim result = getController.getCommandConnection.sendCommand(CasparCGCommandFactory.getPlay(getChannel, getLayer, getMedia, isLooping, False))
                If cmd.execute(getControler.getCommandConnection).isOK Then
                    raiseStarted(Me)
                    While Not getControler.readyForUpdate.WaitOne()
                        logger.warn("PlaylistMovieItem.start: " & getName() & ": Could not get handle to update my status")
                    End While
                    playing = True
                    getControler.readyForUpdate.Release()
                    ' InfoMediaUpdater needs an empty to detect end of file due to BUG: frame-number never reaches nb-frames
                    If Not getControler.getCommandConnection.isOSCSupported() Then
                        cmd = New LoadbgCommand(getChannel, getLayer, "empty", True)
                        cmd.execute(getControler.getCommandConnection)
                    End If
                    logger.log("PlaylistMovieItem.start: ...gestartet " & getChannel() & "-" & getLayer() & ": " & getMedia.toString)
                Else
                    playing = False
                    raiseCanceled(Me)
                    logger.err("PlaylistMovieItem.start: Could not start " & media.getFullName & ". ServerMessage was: " & cmd.getResponse.getServerMessage)
                End If
            Else
                logger.err("PlaylistMovieItem.start: Error playing " & getName() & ". The channel " & getChannel() & " does not exist on the server. Aborting start.")
            End If
        End If
    End Sub

    Public Overrides Sub abort()
        '' CMD an ServerController schicken
        Dim cmd As New StopCommand(getChannel, getLayer)
        cmd.execute(getControler.getCommandConnection)
        setPosition(0)
        waiting = False
        playing = False
        raiseAborted(Me)
    End Sub

    Public Overrides Sub halt()
        Dim cmd As New StopCommand(getChannel, getLayer)
        cmd.execute(getControler.getCommandConnection)
        stoppedPlaying()
    End Sub


    Public Overrides Sub stoppedPlaying()
        playing = False
        setPosition(0)
        raiseStopped(Me)
    End Sub

    Public Overrides Sub pause(ByVal frames As Long)
        '' cmd an ServerController schicken
        Dim cmd As New PauseCommand(getChannel, getLayer)
        cmd.execute(getControler.getCommandConnection)
        raisePaused(Me)
    End Sub

    Public Overrides Sub unPause()
        '' cms an ServerController schicken

        ' check if a media is loaded to bg, than we need to play me with seek!!
        ' because otherwise it would start the bg clip
        Dim cmd As New PlayCommand(getChannel(), getLayer())
        cmd.execute(getControler.getCommandConnection)
        raiseStarted(Me)
    End Sub

    Public Overrides Sub load()
        ''ToDo
    End Sub

    Public Overrides Function getMedia() As CasparCGMedia
        Return media
    End Function

    Public Overrides Function getPosition() As Long
        If getMedia.containsInfo("frame-number") AndAlso isPlaying() Then 'OrElse hasPlayingParent()) Then
            Return ServerControler.getTimeInMS(Long.Parse(getMedia.getInfo("frame-number")), getFPS())
        Else
            Return 0
        End If
    End Function

    Public Overrides Sub setPosition(ByVal position As Long)
        If Not isPlaying() AndAlso getMedia.containsInfo("frame-number") AndAlso getMedia.containsInfo("nb-frames") AndAlso Integer.Parse(getMedia.getInfo("nb-frames")) <= position Then
            getMedia.setInfo("frame-number", position)
        ElseIf Not isPlaying() Then
            '' warum habe ich das gemacht? Warum nicht immer die position übernehmen?
            'getMedia.setInfo("frame-number", "0")
            getMedia.setInfo("frame-number", position)
        End If
    End Sub

    Public Overrides Sub setChannel(ByVal channel As Integer)
        MyBase.setChannel(channel)
        If getControler.containsChannel(channel) AndAlso Not IsNothing(getMedia) Then
            setDuration(getControler.getMediaDuration(getMedia, channel))
        End If
    End Sub


    '' Methoden die Überschrieben werden müssen weil sie leer sind
    ''-------------------------------------------------------------
    Public Overloads Sub addItem(ByVal item As IPlaylistItem)
        '' Nothing to do. Movie Items dosen't have child items
    End Sub

    Public Overloads Sub setChildItems(ByRef items As List(Of IPlaylistItem))
        '' Nothing to do. Movie Items dosen't have child items
    End Sub
End Class
