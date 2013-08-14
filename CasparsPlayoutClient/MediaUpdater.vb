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
Imports Bespoke.Common
Imports Bespoke.Common.Osc
Imports System.Net


'
' Hier werden alle MediaUpdater und das entsprechende Interface definiert.
'

''' <summary>
''' Provides static methods for creationg MediaUpdate Classes
''' </summary>
''' <remarks></remarks>
Public Module MediaUpdaterFactory
    Function getMediaUpdater(ByRef updateConnection As CasparCGConnection, ByRef playlist As IPlaylistItem, ByRef controller As ServerControler) As AbstractMediaUpdater
        If updateConnection.isOSCSupported Then
            Return New OscMediaUpdater(updateConnection, playlist, controller)
        Else
            Return New InfoMediaUpdater(updateConnection, playlist, controller)
        End If
    End Function
End Module



''' <summary>
''' Abstract class for all MediaUpdater classes
''' </summary>
''' <remarks></remarks>
Public MustInherit Class AbstractMediaUpdater

    Friend controller As ServerControler
    Friend WithEvents ticker As FrameTicker
    Friend updateConnection As CasparCGConnection
    Friend channels As Integer
    Friend playlist As IPlaylistItem

    Public Sub New(ByRef updateConnection As CasparCGConnection, ByRef playlist As IPlaylistItem, ByRef controller As ServerControler)
        Me.controller = controller
        Me.updateConnection = updateConnection
        Me.channels = controller.getChannels
        Me.playlist = playlist

    End Sub

    Public MustOverride Sub stopUpdate()
    Public MustOverride Sub startUpdate()

End Class



Public Class OscMediaUpdater
    Inherits AbstractMediaUpdater

    Private oscPort As Integer = 5103
    Private WithEvents oscServer As OscServer

    Public Sub New(ByRef updateConnection As CasparCGConnection, ByRef playlist As IPlaylistItem, ByRef controller As ServerControler)
        MyBase.New(updateConnection, playlist, controller)

        ' OSC Server erstellen
        oscServer = New OscServer(TransportType.Udp, IPAddress.Loopback, oscPort)
        oscServer.FilterRegisteredMethods = False
        oscServer.ConsumeParsingExceptions = False

    End Sub

    Public Overrides Sub startUpdate()
        logger.debug("OscMediaUpdater: Started OSC Server")
        oscServer.Start()
    End Sub

    Public Overrides Sub stopUpdate()
        logger.debug("OscMediaUpdater: Stopped OSC Server")
        oscServer.Stop()
    End Sub

    Public Sub update(ByVal sender As Object, ByVal e As OscMessageReceivedEventArgs) Handles oscServer.MessageReceived

        Dim msg As OscMessage = e.Message
        ' Erst mal filtern
        Dim addressParts() As String = msg.Address.Split("/")

        'logger.log("OSC: Pause msg received " & msg.Address & ": " & msg.Data.First)

        If addressParts.Length > 7 AndAlso addressParts(1) = "channel" AndAlso addressParts(4) = "layer" AndAlso addressParts(6) = "file" AndAlso addressParts(7) = "frame" Then
            Dim c As Integer = Integer.Parse(addressParts(2))
            Dim l As Integer = Integer.Parse(addressParts(5))

            ' Passende Playlist finden
            For Each item In playlist.getPlayingChildItems(True, True)
                'logger.log("OSC: Checking Item " & item.getChannel & "-" & item.getLayer & item.getMedia.getName)
                If item.getChannel = c AndAlso item.getLayer = l Then
                    'logger.log("OSC: ReceiverItem found: " & item.getMedia.getName & " [" & item.getMedia.getInfo("frame-number") & "/" & item.getMedia.getInfo("frame-number") & "]")

                    '' BUGFIX CasparCG-FFMPEG Producer never reachses nb-frames
                    If Integer.Parse(msg.Data.Item(0)) >= Integer.Parse(msg.Data.Item(1)) - 10 Then
                        item.getMedia.setInfo("frame-number", Integer.Parse(item.getMedia.getInfo("frame-number")) + 1)
                    Else
                        item.getMedia.setInfo("frame-number", Integer.Parse(msg.Data.Item(0)))
                    End If
                    item.getMedia.setInfo("nb-frames", Integer.Parse(msg.Data.Item(1)))
                    If Integer.Parse(item.getMedia.getInfo("frame-number")) = Integer.Parse(msg.Data.Item(1)) AndAlso Not item.isLooping Then
                        item.stoppedPlaying()
                    End If
                    logger.debug("OscMediaUpdater: Frame Update msg received " & msg.Address & ": " & item.getMedia.getInfo("frame-number") & "/" & item.getMedia.getInfo("nb-frames"))
                    Exit Sub
                End If
            Next
        End If
    End Sub

End Class


Public Class InfoMediaUpdater
    Inherits AbstractMediaUpdater

    ' Global um häufiges Alloc in updateMedia zu verhindern
    Private infoDoc As New MSXML2.DOMDocument
    Dim xml As String
    Private foregroundProducer As MSXML2.IXMLDOMElement

    Private activeItems() As Dictionary(Of Integer, Dictionary(Of String, IPlaylistItem))
    Private layer As Integer
    Private mediaName As String
    Private updateHandler As New FrameTicker.frameTickEventHandler(AddressOf updateMedia)

    Public Sub New(ByRef updateConnection As CasparCGConnection, ByRef playlist As IPlaylistItem, ByRef controller As ServerControler)
        MyBase.New(updateConnection, playlist, controller)

        ReDim activeItems(channels)
        For i = 0 To channels - 1
            activeItems(i) = New Dictionary(Of Integer, Dictionary(Of String, IPlaylistItem))
        Next
    End Sub

    Public Overrides Sub startUpdate()
        AddHandler controller.getTicker.frameTick, updateHandler
    End Sub

    Public Overrides Sub stopUpdate()
        RemoveHandler controller.getTicker.frameTick, updateHandler
    End Sub

    ''' <summary>
    ''' Updates all playing media items in the playlist
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub updateMedia()
        ''
        '' reads in alle channels as xml
        '' and checks the state of each layer
        '' if a media is found, the corresponding 
        '' IPlaylistItem will be searched within the active 
        '' Items and updated. If no one of the active Items 
        '' is not playing anymore, it will be set to stopped.
        ''

        ' Damit nicht zu viele updates gleichzeitig laufen, 
        ' muss jedes update exlusiv updaten. Kann es das in einer milliseconde
        ' nicht erreichen, verwirft es das update für diesen Tick

        Dim info As New InfoCommand()
        If controller.readyForUpdate.WaitOne(1) AndAlso controller.isOpen Then
            '' Listen und variablen vorbereiten
            xml = ""
            mediaName = ""
            For Each item In playlist.getPlayingChildItems(True, True)
                If activeItems(item.getChannel - 1).ContainsKey(item.getLayer) Then
                    activeItems(item.getChannel - 1).Item(item.getLayer).Add(item.getMedia().getName, item)
                Else
                    activeItems(item.getChannel - 1).Add(item.getLayer, New Dictionary(Of String, IPlaylistItem))
                    activeItems(item.getChannel - 1).Item(item.getLayer).Add(item.getMedia.getName, item)
                End If
            Next

            For c = 0 To channels - 1
                DirectCast(info.getParameter("channel"), CommandParameter(Of Integer)).setValue(c + 1)
                If infoDoc.loadXML(info.execute(updateConnection).getXMLData) Then

                    '' Über alle layer iter.
                    For Each layerNode As MSXML2.IXMLDOMElement In infoDoc.getElementsByTagName("layer")
                        layer = Integer.Parse(layerNode.selectSingleNode("index").nodeTypedValue())

                        ' Ich brauche das layer nur zu beachten, wenn es auch aktive Items auf diesem layer gibt
                        If activeItems(c).ContainsKey(layer) Then

                            '' Zum richtigen Producer navigieren
                            foregroundProducer = layerNode.selectSingleNode("foreground").selectSingleNode("producer")
                            Do Until IsNothing(foregroundProducer) _
                                OrElse foregroundProducer.selectSingleNode("type").nodeTypedValue.Equals("ffmpeg-producer") _
                                OrElse foregroundProducer.selectSingleNode("type").nodeTypedValue.Equals("image-producer") _
                                OrElse foregroundProducer.selectSingleNode("type").nodeTypedValue.Equals("color-producer")

                                Select Case foregroundProducer.selectSingleNode("type").nodeTypedValue
                                    Case "transition-producer"
                                        foregroundProducer = foregroundProducer.selectSingleNode("destination").selectSingleNode("producer")
                                    Case "separated-producer"
                                        foregroundProducer = foregroundProducer.selectSingleNode("fill").selectSingleNode("producer")
                                    Case Else
                                        foregroundProducer = Nothing
                                End Select
                            Loop

                            ' Name und XML aus dem Producer holen und Pfad und extension wegschneiden
                            If Not IsNothing(foregroundProducer) Then
                                If foregroundProducer.selectSingleNode("type").nodeTypedValue.Equals("color-producer") Then
                                    mediaName = foregroundProducer.selectSingleNode("color").nodeTypedValue
                                Else
                                    mediaName = foregroundProducer.selectSingleNode("filename").nodeTypedValue
                                    '' CASPARCG BUG WORKAROUND für doppelte // bei image-producern
                                    mediaName = mediaName.Replace("\\", "\")
                                    mediaName = mediaName.Substring(mediaName.LastIndexOf("\") + 1, mediaName.LastIndexOf(".") - (mediaName.LastIndexOf("\") + 1)).ToUpper
                                End If
                                xml = foregroundProducer.xml
                                If activeItems(c).Item(layer).ContainsKey(mediaName) Then
                                    '' Daten updaten
                                    activeItems(c).Item(layer).Item(mediaName).getMedia.parseXML(xml)
                                    ''danach aus liste entfernen
                                    activeItems(c).Item(layer).Remove(mediaName)
                                End If
                            End If
                            If activeItems(c).Item(layer).Count = 0 Then Exit For
                        End If
                    Next
                    ' Alle Items in diesem Channel die jetzt noch in der liste sind, sind nicht mehr auf dem Server gestartet 
                    ' und werden daher als gestoppt markiert
                    For Each layer As Integer In activeItems(c).Keys
                        For Each item As IPlaylistItem In activeItems(c).Item(layer).Values

                            '' BUGFIX CasparCG won't ever reach nb-frames with frame-number, so we fake it till this is fixed
                            '' --> This BUGFIX made some other Probs.
                            '' if you have two items wiht same filename, both will be updated as if they where playing
                            'If item.getMedia.containsInfo("nb-frames") AndAlso item.getMedia.containsInfo("frame-number") Then
                            '    If Long.Parse(item.getMedia.getInfo("nb-frames")) > Long.Parse(item.getMedia.getInfo("frame-number")) Then
                            '        item.getMedia.setInfo("frame-number", item.getMedia.getInfo("nb-frames"))
                            '    End If
                            'End If
                            item.stoppedPlaying()
                        Next
                    Next
                    activeItems(c).Clear()
                Else
                    logger.err("mediaUpdater.UpdateMedia: Could not update media at channel " & c + 1 & ". Unable to load xml data. " & infoDoc.parseError.reason)
                End If
            Next
        End If
        controller.readyForUpdate.Release()
    End Sub
End Class


''
'' Alte OSC Implementation. Sackgasse!
''

'Public Class OscMediaUpdater
'    Inherits AbstractMediaUpdater

'    Private oscPort As Integer = 5103
'    Private WithEvents oscServer As OscServer

'    Private readyForUpdate As New Threading.Semaphore(1, 1)

'    Private currentFileInfos() As Dictionary(Of Integer, fileInfo)

'    Public Sub New(ByRef updateConnection As CasparCGConnection, ByRef playlist As IPlaylistItem, ByRef controller As ServerController)
'        MyBase.New(updateConnection, playlist, controller)

'        ' Init Vars
'        ReDim currentFileInfos(channels)
'        For i = 0 To channels - 1
'            currentFileInfos(i) = New Dictionary(Of Integer, fileInfo)
'        Next

'        ' OSC Server erstellen
'        oscServer = New OscServer(TransportType.Udp, IPAddress.Loopback, oscPort)
'        oscServer.FilterRegisteredMethods = False
'        oscServer.ConsumeParsingExceptions = False

'    End Sub

'    Public Overrides Sub startUpdate()
'        oscServer.Start()
'        AddHandler controller.getTicker.frameTick, AddressOf updateMedia
'    End Sub

'    Public Overrides Sub stopUpdate()
'        oscServer.Stop()
'    End Sub


'    Public Sub update(ByVal sender As Object, ByVal e As OscMessageReceivedEventArgs)

'        Dim msg As OscMessage = e.Message
'        ' Erst mal filtern
'        Dim addressParts() As String = msg.Address.Split("/")

'        If addressParts.Length > 7 AndAlso addressParts(1) = "channel" AndAlso addressParts(4) = "layer" Then
'            Dim c As Integer = Integer.Parse(addressParts(2)) - 1
'            Dim l As Integer = Integer.Parse(addressParts(5))

'            Dim currentFileInfo As fileInfo
'            If Not currentFileInfos(c).ContainsKey(l) Then
'                currentFileInfo = New fileInfo()
'                currentFileInfo.channel = c
'                currentFileInfo.layer = l
'                currentFileInfos(c).Add(l, currentFileInfo)
'            Else
'                currentFileInfo = currentFileInfos(c).Item(l)
'            End If

'            'If readyForUpdate.WaitOne(40) Then
'            Select Case addressParts(6)
'                Case "file"
'                    Select Case addressParts(7)
'                        Case "frame"
'                            currentFileInfo.setFrames(Integer.Parse(msg.Data.Item(0)), Integer.Parse(msg.Data.Item(1)))
'                        Case "path"
'                            currentFileInfo.setPath(msg.Data.First.ToString.ToUpper)
'                        Case "time"
'                            currentFileInfo.setTime(Double.Parse(msg.Data.Item(0)), Double.Parse(msg.Data.Item(1)))
'                        Case "fps"
'                            currentFileInfo.setFps(Integer.Parse(msg.Data.First))
'                        Case Else
'                            logger.warn("OSC: Unknown subaddress of /file/ found: " & addressParts(7))
'                    End Select
'                Case "color"
'                Case "host"
'            End Select
'            'readyForUpdate.Release()
'            'End If
'            'ElseIf addressParts.Length = 7 AndAlso addressParts(6) = "paused" Then
'            '    logger.log("OSC: Pause msg received for " & addressParts(2) & "-" & addressParts(5) & ": " & msg.Data.First)
'        End If
'    End Sub

'    Public Sub updateMedia()
'        ' Damit nicht zu viele updates gleichzeitig laufen, 
'        ' muss jedes update exlusiv updaten. Kann es das in einer millisekunde
'        ' nicht erreichen, verwirft es das update für diesen Tick
'        If controller.readyForUpdate.WaitOne(1) AndAlso controller.isOpen Then

'            logger.debug("OscMediaUpdater.updateMedia: Start Update")

'            '' Listen und variablen vorbereiten
'            For Each item In playlist.getPlayingChildItems(True, True)
'                logger.log("OscMediaUpdater.updateMedia: Add active Item " & item.getChannel & "-" & item.getLayer & ": " & item.getMedia.getName & " {" & item.getMedia.getUuid & "}")
'                If activeItems(item.getChannel - 1).ContainsKey(item.getLayer) Then
'                    activeItems(item.getChannel - 1).Item(item.getLayer).Add(item.getMedia().getName, item)
'                Else
'                    activeItems(item.getChannel - 1).Add(item.getLayer, New Dictionary(Of String, IPlaylistItem))
'                    activeItems(item.getChannel - 1).Item(item.getLayer).Add(item.getMedia.getName, item)
'                End If
'            Next

'            ' Alle currentFileInfos durchlaufen und updates in die Playlists übernehmen
'            For c = 0 To channels - 1 Step 1
'                'If readyForUpdate.WaitOne(40) Then
'                For Each fileInfo In currentFileInfos(c).Values
'                    If fileInfo.isComplete Then
'                        logger.log("OscMediaUpdater.updateMedia: Processing " & c + 1 & "-" & fileInfo.layer & ": " & fileInfo.path)
'                        If activeItems(c).ContainsKey(fileInfo.layer) AndAlso activeItems(c).Item(fileInfo.layer).ContainsKey(fileInfo.path) Then
'                            logger.log("OscMediaUpdater.updateMedia: Update " & activeItems(c).Item(fileInfo.layer).Item(fileInfo.path).getName & " {" & activeItems(c).Item(fileInfo.layer).Item(fileInfo.path).getMedia.getUuid & "} [frames: " & fileInfo.frame_nb & "/" & fileInfo.nb_frames & "]")
'                            ''
'                            '' testig BUGFIX:
'                            ''
'                            activeItems(c).Item(fileInfo.layer).Item(fileInfo.path).getMedia().setUpdated()
'                            '' Daten updaten
'                            activeItems(c).Item(fileInfo.layer).Item(fileInfo.path).getMedia.setInfo("nb-frames", fileInfo.nb_frames)
'                            activeItems(c).Item(fileInfo.layer).Item(fileInfo.path).getMedia.setInfo("frame-number", fileInfo.frame_nb)
'                            activeItems(c).Item(fileInfo.layer).Item(fileInfo.path).getMedia.setInfo("fps", fileInfo.fps)
'                            activeItems(c).Item(fileInfo.layer).Item(fileInfo.path).getMedia.setInfo("current-time", fileInfo.time)
'                            activeItems(c).Item(fileInfo.layer).Item(fileInfo.path).getMedia.setInfo("duration", fileInfo.duration)
'                            ''danach aus liste entfernen
'                            activeItems(c).Item(fileInfo.layer).Remove(fileInfo.path)
'                        Else
'                            logger.log("OscMediaUpdater.updateMedia: Not an active item: " & c + 1 & "-" & fileInfo.layer & ": " & fileInfo.path)
'                        End If
'                        fileInfo.reset()
'                    End If
'                Next
'                'readyForUpdate.Release()
'                'End If


'                ' Alle Items in diesem Channel die jetzt noch in der liste sind, sind nicht mehr auf dem Server gestartet 
'                ' und werden daher als gestoppt markiert
'                For Each layer As Integer In activeItems(c).Keys
'                    For Each item As IPlaylistItem In activeItems(c).Item(layer).Values
'                        '' BUGFIX CasparCG won't ever reach nb-frames with frame-number, so we fake it till this is fixed
'                        '' --> This BUGFIX made some other Probs.
'                        '' if you have two items wiht same filename, both will be updated as if they where playing
'                        'If item.getMedia.containsInfo("nb-frames") AndAlso item.getMedia.containsInfo("frame-number") Then
'                        '    If Long.Parse(item.getMedia.getInfo("nb-frames")) > Long.Parse(item.getMedia.getInfo("frame-number")) Then
'                        '        item.getMedia.setInfo("frame-number", item.getMedia.getInfo("nb-frames"))
'                        '    End If
'                        'End If

'                        ''
'                        '' Test BUGFIX: freshly started items will be removed because osc mesg arriving to slow.
'                        '' Only if at least one osc mesg. arrived the item will be stopped.
'                        ''
'                        If item.getMedia.hasBeenUpdated Then
'                            item.stoppedPlaying()
'                        End If
'                    Next
'                Next
'                activeItems(c).Clear()
'            Next
'            controller.readyForUpdate.Release()
'        End If
'    End Sub

'    Private Class fileInfo
'        Public path As String = ""
'        Public fps As Integer
'        Public frame_nb As Integer
'        Public nb_frames As Integer
'        Public time As Double
'        Public duration As Double
'        Public channel As Integer
'        Public layer As Integer

'        Private completed(4) As Boolean

'        Public Function isComplete() As Boolean
'            For i = 0 To 3 Step 1
'                If Not completed(i) Then Return False
'            Next
'            Return True
'        End Function

'        Public Sub setPath(path As String)
'            logger.debug("Set path to: " & path)
'            completed(0) = True
'            Me.path = path

'            ' Nötigenfalls endung wegschneiden
'            If Me.path.Contains(".") Then
'                Me.path = Me.path.Substring(0, Me.path.LastIndexOf("."))
'            End If
'        End Sub

'        Public Sub setFps(fps As Integer)
'            logger.debug("Set fps to: " & fps)
'            completed(1) = True
'            Me.fps = fps
'        End Sub

'        Public Sub setFrames(frame_nb As Integer, nb_frames As Integer)
'            logger.debug("Set frames to: " & frame_nb & "/" & nb_frames)
'            completed(2) = True
'            Me.frame_nb = frame_nb
'            Me.nb_frames = nb_frames
'        End Sub

'        Public Sub setTime(time As Double, duration As Double)
'            logger.debug("Set times to: " & time & "/" & duration)
'            completed(3) = True
'            Me.time = time
'            Me.duration = duration
'        End Sub

'        Public Sub reset()
'            logger.debug("Reset fileInfo")
'            For i = 0 To 3 Step 1
'                completed(i) = False
'            Next
'            path = ""
'            fps = 0
'            frame_nb = 0
'            nb_frames = 0
'            time = 0
'            duration = 0
'        End Sub
'    End Class
'End Class



