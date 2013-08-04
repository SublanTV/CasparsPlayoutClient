Public Class PlaylistMovieItem
    Inherits PlaylistItem
    Implements IPlaylistItem

    Private media As CasparCGMovie

    ''' <summary>
    ''' Create a PlaylistMovieItem. If a duration is given and smaller than the original duration of the file, the file will only be played for that long.
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="controller"></param>
    ''' <param name="movie"></param>
    ''' <param name="duration"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String, ByRef controller As ServerController, ByVal movie As CasparCGMovie, Optional ByVal channel As Integer = -1, Optional ByVal layer As Integer = -1, Optional ByVal duration As Long = -1)
        MyBase.New(name, PlaylistItemTypes.MOVIE, controller, channel, layer, duration)
        If Not IsNothing(movie) Then
            If movie.getInfos.Count = 0 Then
                movie.parseXML(getController.getMediaInfo(movie))
            End If
            If movie.containsInfo("nb-frames") AndAlso (duration > Long.Parse(movie.getInfo("nb-frames")) OrElse duration = -1) Then
                setDuration(Long.Parse(movie.getInfo("nb-frames")))
            End If
            media = movie
        Else
            logger.critical("PlaylistMovieItem.new: ERROR: Given movie was nothing - Stopping now")
            Throw New Exception("NOTHING not allowed")
        End If
    End Sub


    '' Methoden die überschrieben werden müssen weil sie andere oder mehr functionen haben
    ''-------------------------------------------------------------------------------------
    Public Overrides Sub start(Optional ByVal noWait As Boolean = False)
        '' CMD an ServerController schicken
        logger.log("PlaylistMovieItem.start: Starte " & getChannel() & "-" & getLayer() & ": " & getMedia.toString)
        Dim result = getController.getCommandConnection.sendCommand(CasparCGCommandFactory.getPlay(getChannel, getLayer, getMedia, isLooping, , getDuration))
        If result.isOK Then
            While Not getController.readyForUpdate.WaitOne()
                logger.warn("PlaylistMovieItem.start: " & getName() & ": Could not get handel to update my status")
            End While
            playing = True
            getController.readyForUpdate.Release()
            getController.getCommandConnection.sendAsyncCommand(CasparCGCommandFactory.getLoadbg(getChannel, getLayer, "empty", True))
            logger.log("PlaylistMovieItem.start: ...gestartet " & getChannel() & "-" & getLayer() & ": " & getMedia.toString)
        Else
            logger.err("PlaylistMovieItem.start: Could not start " & media.getFullName & ". ServerMessage was: " & result.getServerMessage)
        End If

        While isPlaying() AndAlso Not noWait
            'getController.update()
            Threading.Thread.Sleep(1)
        End While
    End Sub

    Public Overloads Sub abort()
        '' CMD an ServerController schicken
        getController.getCommandConnection.sendCommand(CasparCGCommandFactory.getStop(getChannel, getLayer))
        '' Der rest wird von der Elternklasse erledigt
        MyBase.abort()
    End Sub

    Public Overrides Sub pause(ByVal frames As Long)
        '' cmd an ServerController schicken
        getController.getCommandConnection.sendCommand(CasparCGCommandFactory.getPause(getChannel, getLayer))
        '' pause wird über die Elternklasse realisiert
        MyBase.pause(frames)
    End Sub

    Public Overrides Sub unPause()
        '' cms an ServerController schicken
        getController.getCommandConnection.sendCommand(CasparCGCommandFactory.getPlay(getChannel, getLayer))
        MyBase.unPause()
    End Sub

    Public Overrides Function getMedia() As CasparCGMedia
        Return media
    End Function

    Public Overrides Function getPosition() As Long
        If getMedia.containsInfo("nb-frames") AndAlso getMedia.containsInfo("frame-number") Then
            Return Long.Parse(getMedia.getInfo("frame-number"))
        Else
            Return 0
        End If
    End Function

    '' Methoden die Überschrieben werden müssen weil sie leer sind
    ''-------------------------------------------------------------
    Public Overloads Sub addItem(ByVal item As IPlaylistItem)
        '' Nothing to do. Movie Items dosen't have child items
    End Sub

    Public Overloads Sub setChildItems(ByRef items As List(Of IPlaylistItem))
        '' Nothing to do. Movie Items dosen't have child items
    End Sub
End Class
