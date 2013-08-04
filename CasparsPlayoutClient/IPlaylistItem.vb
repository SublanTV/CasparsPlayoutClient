Public Interface IPlaylistItem

    Function getName() As String ' Name des Items
    Function getDelay() As Long ' Verzögerung bis zum start dieses Items in Frames
    Function getLayer() As Integer ' Layer des Items
    Function getChannel() As Integer ' Server Channel des Items
    Function isPlayable() As Boolean
    Function isLooping() As Boolean
    Function isAutoStarting() As Boolean
    Function isParallel() As Boolean


    Function getDuration() As Long ' Gesamtlaufzeit in ms
    Function getPosition() As Long ' aktuelle ms
    Function getRemaining() As Long ' noch zu spielende ms
    Function getItemType() As PlaylistItem.PlaylistItemTypes ' Typ des Item
    Function isPlaying() As Boolean
    Function isPaused() As Boolean
    Function getPlayed() As Single '% des Items gespielt
    Function getChildItems(Optional ByVal recursiv As Boolean = False) As List(Of IPlaylistItem) ' alle Items in diesem Item
    Function getPlayingChildItems(Optional ByVal recursiv As Boolean = False, Optional ByVal onlyPlayable As Boolean = False) As IEnumerable(Of IPlaylistItem) ' alle activen, spielenden Items in diesem Item
    Function getMedia() As CasparCGMedia
    'Function getLayerUser(Optional ByVal recursiv As Boolean = False) As Dictionary(Of Integer, Integer)
    Function toXML() As String
    Function toString() As String

    Sub setLayer(ByVal layer As Integer)
    Sub setChannel(ByVal channel As Integer)
    Sub setLooping(ByVal looping As Boolean)
    Sub setAutoStart(ByVal autoStart As Boolean)
    Sub setParallel(ByVal parallel As Boolean)
    Sub setDelay(ByVal delay As Long)
    Sub setDuration(ByVal duration As Long) ' Gesamtlaufzeit in Frames
    Sub setPosition(ByVal position As Long) ' aktuelle Frame
    Sub setRemaining(ByVal remaining As Long) ' noch zu spielende Frames
    Sub setChildItems(ByRef items As List(Of IPlaylistItem))
    Sub addItem(ByRef item As IPlaylistItem)
    Sub loadXML(ByVal xml As String) ' Erstellt ein IPlaylistItem aus einer xml definition)

    Sub load() ' lädt wenn möglich item schon im Hintergrund (ACMP loadbg)
    Sub start(Optional ByVal noWait As Boolean = False)
    Sub abort() ' stopt item (ACMP stop)
    Sub stoppedPlaying() ' informs item that it has stopped playing
    Sub pause(ByVal frames As Long) ' pausiert das Spielen des Items for frames Frames oder bis zum manuellen start bei 0 (ACMP pause)
    Sub unPause()

End Interface
