Public MustInherit Class PlaylistItem
    Implements IPlaylistItem

    Private _name As String
    Private layer As Integer
    Private channel As Integer
    Private delay As Long
    Private looping As Boolean
    Private autoStart As Boolean
    Private parallel As Boolean

    ' Die (Kinder)Items dieses Items
    Private items As List(Of IPlaylistItem)
    Private WithEvents controller As ServerController
    Private _Duration As Long ' Gesamtlaufzeit in Frames
    Private _Position As Long ' aktuelle Frame
    Private _Remaining As Long ' noch zu spielende Frames
    Private ItemType As PlaylistItem.PlaylistItemTypes ' Typ des Item
    Friend playing As Boolean
    Private paused As Boolean
    Private startThread As Threading.Thread
    Private pauseThread As Threading.Thread

    Enum PlaylistItemTypes
        BLOCK = -1
        STILL = 0
        MOVIE = 1
        AUDIO = 2
        TEMPLATE = 3
        COMMAND = 4
    End Enum

    ''' <summary>
    ''' Constructor for a generic PlaylistItem (Should be called by Subclasses)
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="itemType"></param>
    ''' <param name="controller"></param>
    ''' <param name="duration"></param>
    ''' <remarks></remarks>
    Protected Sub New(ByVal name As String, ByVal itemType As PlaylistItemTypes, ByRef controller As ServerController, Optional ByVal channel As Integer = -1, Optional ByVal layer As Integer = -1, Optional ByVal duration As Long = -1)
        Me._name = name
        Me.ItemType = itemType
        Me.controller = controller
        setChannel(channel)
        setLayer(layer)
        If duration > -1 Then
            setDuration(duration)
        End If
        items = New List(Of IPlaylistItem)
    End Sub

    Public Sub abort() Implements IPlaylistItem.abort
        If Not IsNothing(startThread) Then
            startThread.Abort()
            startThread = Nothing
        End If
        If Not IsNothing(pauseThread) Then
            pauseThread.Abort()
            pauseThread = Nothing
        End If
        For Each item In items
            item.abort()
        Next
        playing = False
    End Sub

    Public Overridable Sub stoppedPlaying() Implements IPlaylistItem.stoppedPlaying
        playing = False
        If Not IsNothing(startThread) Then
            startThread.Abort()
        End If
        logger.debug("PlaylistItem.stoppedPlaying: Stopped playing " & getName() & " (" & getChannel() & "-" & getLayer() & ")")
    End Sub

    Public Sub load() Implements IPlaylistItem.load
        '' Wird nur in den Medien Items implementiert
    End Sub

    '' ToDo:
    '' Alle Pause und delay funktionen über events realisieren und eventuell mit dem server Tick synchronisieren
    ''
    Public Overridable Sub pause(ByVal frames As Long) Implements IPlaylistItem.pause
        For Each item In items
            item.pause(-1)
        Next

        '' Wenn gesagt wird wie lange die pause geht, starte einen kleinen 
        '' wecker der nach dieser Zeit die Pause aufhebt.
        If frames >= 0 Then
            pauseThread = New Threading.Thread(AddressOf Me.unPause)
            pauseThread.Start(controller.getTimeInMS(frames, channel))
        End If
        paused = True
    End Sub

    Public Sub unPause(ByVal time As Long)
        Threading.Thread.Sleep(time)
        unPause()
    End Sub

    Public Overridable Sub unPause() Implements IPlaylistItem.unPause
        For Each item In items
            item.unPause()
        Next
        pauseThread = Nothing
        paused = False
    End Sub

    ''' <summary>
    ''' Start this item and all childItems. If noWait is given and false, start will wait till all items has been stopped and then end,
    ''' else it will start each item and then end imediatly
    ''' </summary>
    ''' <param name="noWait"></param>
    ''' <remarks></remarks>
    Public Overridable Sub start(Optional ByVal noWait As Boolean = False) Implements IPlaylistItem.start
        logger.log("PlaylistItem.start: " & getName() & ": received start at thread " & Threading.Thread.CurrentThread.ManagedThreadId)
        If noWait Then
            logger.log("PlaylistItem.start: " & getName() & ": noWait is true. Start new thread")
            If Not IsNothing(startThread) Then
                startThread.Abort()
            End If
            startThread = New Threading.Thread(AddressOf Me.start)
            startThread.Start()
        Else
            logger.debug("PlaylistItem.start: Start " & getName())
            playing = True

            ' alle Unteritems starten.
            ' Wenn parallel, dann wird nicht gewaret und alle starten
            ' semi gleichzeitig
            ' Sost startet das nächste erst wenn das vorherige fertig ist.
            For Each item In items
                item.start(isParallel)
            Next
        End If
        logger.debug("PlaylistItem.start: Start " & getName() & " has been completed")
    End Sub

    Public Overrides Function toString() As String Implements IPlaylistItem.toString
        Return getName() & "(" & getItemType.ToString & ") " & getChannel() & "-" & getLayer() & " playing: " & isPlaying.ToString
    End Function

    Public Function toXML() As String Implements IPlaylistItem.toXML
        Dim xml As String = "<item><name>" & _
            _name & "</name><type>" & _
            getItemType.ToString & "</type><layer>" & _
            layer.ToString & "</layer><channel>" & _
            channel.ToString & "</channel><autostarting>" & _
            isAutoStarting.ToString & "</autostarting><isParallel>" & _
            isParallel.ToString & "</isParallel><isLooping>" & _
            isLooping.ToString & "</isLooping><duration>" & _
            _Duration.ToString & "</duration><delay>" & _
            delay.ToString & "</delay> "
        For Each item As IPlaylistItem In items
            xml = xml & item.toXML
        Next
        Return xml & "</item>"
    End Function

    Public Sub loadXML(ByVal xml As String) Implements IPlaylistItem.loadXML
        '' ToDo
    End Sub


    '' GETTER:
    ''--------

    Public Function getChannel() As Integer Implements IPlaylistItem.getChannel
        Return channel
    End Function

    Public Function getDelay() As Long Implements IPlaylistItem.getDelay
        Return delay
    End Function

    Public Function getLayer() As Integer Implements IPlaylistItem.getLayer
        Return layer
    End Function

    Public Function getName() As String Implements IPlaylistItem.getName
        Return _name
    End Function

    Public Function isAutoStarting() As Boolean Implements IPlaylistItem.isAutoStarting
        Return autoStart
    End Function

    Public Function isLooping() As Boolean Implements IPlaylistItem.isLooping
        Return looping
    End Function

    Public Function isParallel() As Boolean Implements IPlaylistItem.isParallel
        Return parallel
    End Function

    Public Overridable Function getDuration() As Long Implements IPlaylistItem.getDuration
        Return _Duration
    End Function

    Public Function getItemType() As PlaylistItemTypes Implements IPlaylistItem.getItemType
        Return ItemType
    End Function

    Public Overridable Function getPosition() As Long Implements IPlaylistItem.getPosition
        Return _Position
    End Function

    Public Overridable Function getRemaining() As Long Implements IPlaylistItem.getRemaining
        Return getDuration() - getPosition()
    End Function

    Public Function getPlayingChildItems(Optional ByVal recursiv As Boolean = False, Optional ByVal onlyPlayable As Boolean = False) As IEnumerable(Of IPlaylistItem) Implements IPlaylistItem.getPlayingChildItems
        Dim activeItems As New List(Of IPlaylistItem)
        For Each item In getChildItems(recursiv)
            If item.isPlaying Then
                If onlyPlayable Then
                    If item.isPlayable Then
                        activeItems.Add(item)
                    End If
                Else
                    activeItems.Add(item)
                End If
            End If
        Next
        Return activeItems
    End Function

    Public Function getChildItems(Optional ByVal recursiv As Boolean = False) As List(Of IPlaylistItem) Implements IPlaylistItem.getChildItems
        Dim childItems As New List(Of IPlaylistItem)
        For Each item In items
            childItems.Add(item)
            If recursiv Then
                childItems.AddRange(item.getChildItems(recursiv))
            End If
        Next
        Return childItems
    End Function

    Public Overridable Function getPlayed() As Single Implements IPlaylistItem.getPlayed
        Return (1 / getDuration()) * getPosition()
    End Function

    Public Function isPlaying() As Boolean Implements IPlaylistItem.isPlaying
        Return playing
    End Function

    Public Function isPaused() As Boolean Implements IPlaylistItem.isPaused
        Return paused
    End Function

    Public Function isPlayable() As Boolean Implements IPlaylistItem.isPlayable
        Select Case getItemType()
            Case PlaylistItem.PlaylistItemTypes.AUDIO, PlaylistItem.PlaylistItemTypes.MOVIE, PlaylistItem.PlaylistItemTypes.STILL
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Function getController() As ServerController
        Return controller
    End Function

    Public Overridable Function getMedia() As CasparCGMedia Implements IPlaylistItem.getMedia
        Return Nothing
    End Function


    '' SETTER:
    ''---------

    Public Sub setName(ByVal Name As String)
        Me._name = Name
    End Sub

    Public Sub setChildItems(ByRef items As System.Collections.Generic.List(Of IPlaylistItem)) Implements IPlaylistItem.setChildItems
        ' einzeln und nicht als block hinzufügen damit die duration berechnet wird
        For Each item In items
            addItem(item)
        Next
    End Sub

    Public Sub setDuration(ByVal duration As Long) Implements IPlaylistItem.setDuration
        Me._Duration = duration
    End Sub

    Public Sub setPosition(ByVal position As Long) Implements IPlaylistItem.setPosition
        Me._Position = position
    End Sub

    Public Sub setRemaining(ByVal remaining As Long) Implements IPlaylistItem.setRemaining
        Me._Remaining = remaining
    End Sub

    Public Sub addItem(ByRef item As IPlaylistItem) Implements IPlaylistItem.addItem
        logger.log("PlaylistItem.addItem: " & getName() & "(" & getChannel() & "-" & getLayer() & "): Adding new Item " & item.getName)
        If Not IsNothing(item) Then
            If item.getChannel < 0 Then
                item.setChannel(getChannel)
            End If
            If item.getLayer < 0 Then
                item.setLayer(getLayer)
            End If

            items.Add(item)
            If isParallel() Then
                setDuration(Math.Max(getDuration, item.getDuration))
            Else
                setDuration(getDuration() + item.getDuration)
            End If
        End If
    End Sub

    Public Sub setAutoStart(ByVal autoStart As Boolean) Implements IPlaylistItem.setAutoStart
        Me.autoStart = autoStart
    End Sub

    Public Sub setChannel(ByVal channel As Integer) Implements IPlaylistItem.setChannel
        If channel <> -1 Then
            If Not controller.containsChannel(channel) Then
                logger.warn("PlaylistItem.setChannel: Playlist " & getName() & ": The channel " & channel & " is not configured at the given server. This could lead to errors during playlist playback.")
            End If
        End If
        Me.channel = channel
    End Sub

    Public Sub setDelay(ByVal delay As Long) Implements IPlaylistItem.setDelay
        Me.delay = delay
    End Sub

    Public Sub setLayer(ByVal layer As Integer) Implements IPlaylistItem.setLayer
        If layer > -2 Then
            Me.layer = layer
        Else
            logger.warn("PlaylistItem.setLayer: Playlist " & getName() & ": Can't set layer to " & layer & ". Leaving it unset which means it will be the standard layer")
            Me.layer = -1
        End If
    End Sub

    Public Sub setLooping(ByVal looping As Boolean) Implements IPlaylistItem.setLooping
        Me.looping = looping
    End Sub

    Public Sub setParallel(ByVal parallel As Boolean) Implements IPlaylistItem.setParallel
        Me.parallel = parallel
    End Sub

End Class
