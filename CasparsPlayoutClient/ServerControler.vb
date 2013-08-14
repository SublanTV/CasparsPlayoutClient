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

Imports System.Threading
Imports CasparCGNETConnector
Imports logger


Public Class ServerControler

    Public readyForUpdate As New Semaphore(1, 1)
    Private cmdConnection As CasparCGConnection
    'Private tickConnection As CasparCGConnection
    Private updateConnection As CasparCGConnection
    Private testConnection As CasparCGConnection
    Private tickThread As Thread
    Private serverAddress As String = "localhost"
    Private serverPort As Integer = 5250
    Private testChannel As Integer = 2
    Private framesPerTick As Integer = 1
    Private channels As Integer
    Private channelFPS() As Integer
    Private opened As Boolean
    Private WithEvents ticker As FrameTicker
    Private updater As AbstractMediaUpdater
    Private playlist As IPlaylistItem ' Die Root Playlist unter die alle anderen kommen

    Public Sub New()
        playlist = New PlaylistBlockItem("Playlist", Me, -1, -1)
        playlist.setParallel(True)
    End Sub

    Public Function open() As Boolean
        Return open(serverAddress, serverPort)
    End Function

    Public Sub close()
        logger.debug("ServerController.close: Close servercontroller...")
        opened = False

        If Not IsNothing(updater) Then updater.stopUpdate()
        If Not IsNothing(ticker) Then
            ticker.stopTicker()
        End If

        If Not IsNothing(cmdConnection) Then cmdConnection.close()
        If Not IsNothing(updateConnection) Then updateConnection.close()
        If Not IsNothing(testConnection) Then testConnection.close()
        'If Not IsNothing(tickConnection) Then tickConnection.close()
        channels = 0
        ReDim channelFPS(0)
    End Sub

    Public Function isOpen() As Boolean
        Return opened
    End Function

    Public Function isConnected() As Boolean
        Return Not IsNothing(updateConnection) AndAlso Not _
            IsNothing(cmdConnection) AndAlso Not IsNothing(testConnection) AndAlso _
            updateConnection.isConnected AndAlso _
            cmdConnection.isConnected AndAlso testConnection.isConnected
    End Function

    Public Function open(ByVal serverAddress As String, ByVal severPort As Integer) As Boolean
        Me.serverAddress = serverAddress
        Me.serverPort = serverPort
        cmdConnection = New CasparCGConnection(serverAddress, serverPort)
        updateConnection = New CasparCGConnection(serverAddress, serverPort)
        testConnection = New CasparCGConnection(serverAddress, serverPort)
        'tickConnection = New CasparCGConnection(serverAddress, serverPort)


        If testConnection.connect() AndAlso updateConnection.connect() AndAlso cmdConnection.connect() Then
            opened = True
            ' Channels des Servers bestimmen
            channels = testConnection.getServerChannels
            testChannel = channels
            If channels > 1 Then
                channels = channels - 1
            End If
            '' Test: removed -1 because the number should be the length, not the upper bound of the array
            ReDim channelFPS(channels)
            For c As Integer = 0 To channels - 1
                channelFPS(c) = getChannelFPS(c + 1)
            Next

            ' Tick  starten
            ticker = New frameTicker(Me, framesPerTick)

            ' updater starten
            updater = MediaUpdaterFactory.getMediaUpdater(updateConnection, playlist, Me)
            updater.startUpdate()
        Else
            opened = False
        End If
        Return opened
    End Function

    Public Function getPlaylistRoot() As IPlaylistItem
        Return playlist
    End Function

    Public Function getCommandConnection() As CasparCGConnection
        Return cmdConnection
    End Function

    Public Function getTestConnection() As CasparCGConnection
        Return testConnection
    End Function

    Public Function getTestChannel() As Integer
        Return testChannel
    End Function

    Public Function getChannels() As Integer
        Return channels
    End Function

    ''' <summary>
    ''' Returns the media duration in milliseconds if playing in native fps.
    ''' </summary>
    ''' <param name="media"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getOriginalMediaDuration(ByRef media As CasparCGMedia) As Long
        If isConnected() Then
            Select Case media.getMediaType
                Case CasparCGMedia.MediaType.COLOR, CasparCGMedia.MediaType.STILL, CasparCGMedia.MediaType.TEMPLATE
                    '' These mediatyps doesn't have any durations
                    Return 0
                Case Else
                    If media.getInfos.Count = 0 Then
                        '' no media info is loaded
                        '' load it now
                        media.fillMediaInfo(testConnection, testChannel)
                    End If
                    If media.containsInfo("file-nb-frames") AndAlso media.containsInfo("fps") AndAlso media.containsInfo("progressive") Then
                        Dim fps As Integer = Single.Parse(media.getInfo("fps")) * 100
                        Dim progressive = Boolean.Parse(media.getInfo("progressive"))
                        'If Not progressive Then
                        '    fps = fps / 2
                        'End If
                        Return getTimeInMS(media.getInfo("file-nb-frames"), fps)
                    End If
                    logger.err("ServerController.getOriginalMediaDuration: Could not get media duration of " & media.getFullName & "(" & media.getMediaType.ToString & ").")
            End Select
        End If
        Return 0
    End Function

    ''' <summary>
    ''' Returns the media duration in milliseconds if playing at a given channel.
    ''' </summary>
    ''' <param name="media"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getMediaDuration(ByRef media As CasparCGMedia, ByVal channel As Integer) As Long

        'Return getOriginalMediaDuration(media)

        ''---> Scheint wohl doch nötig zu sein! CD 07/13
        '' Scheint nicht nötig zu sein da die videos IMMER so lange spielen wie sie sollen
        '' zumindest wenn die metadaten stimmen - aber erst noch ausgiebig testen!
        If isConnected() Then
            Select Case media.getMediaType
                Case CasparCGMedia.MediaType.COLOR, CasparCGMedia.MediaType.STILL, CasparCGMedia.MediaType.TEMPLATE
                    '' These mediatyps doesn't have any durations
                    Return 0
                Case Else
                    If media.getInfos.Count = 0 Then
                        '' no media info is loaded
                        '' load it now
                        'media.parseXML(getMediaInfo(media))
                        media.fillMediaInfo(testConnection, testChannel)
                    End If
                    If media.containsInfo("nb-frames") Then
                        Return getTimeInMS(media.getInfo("nb-frames"), getFPS(channel))
                    End If
                    logger.err("ServerController.getMediaDuration: Could not get media duration of " & media.getFullName & "(" & media.getMediaType.ToString & ").")
            End Select
        End If
        Return 0
    End Function

    ''' <summary>
    ''' Returns whether or not, the given channel is configured at the connected CasparCGServer
    ''' </summary>
    ''' <param name="channel">the channel number to check for</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function containsChannel(ByVal channel As Integer) As Boolean
        Return channel <= channels AndAlso channel > 0
    End Function

    ''' <summary>
    ''' Returns the given number of milliseconds as a formated String "*hh:mm:ss.µµ" where h = hours, m = minutes, s = seconds and µ = milliseconds.
    ''' </summary>
    ''' <param name="milliseconds"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getTimeStringOfMS(ByVal milliseconds As Long) As String
        If Not milliseconds = 0 Then
            Dim neg As Boolean = False
            If milliseconds < 0 Then
                milliseconds = milliseconds * -1
                neg = True
            End If
            Dim h As String = Convert.ToUInt16(Math.Truncate(Math.Abs(milliseconds) / 3600000)).ToString("D2")
            Dim m As String = Convert.ToUInt16(Math.Truncate((Math.Abs(milliseconds) / 60000) Mod 60)).ToString("D2")
            Dim s As String = Convert.ToUInt16(Math.Truncate((Math.Abs(milliseconds) / 1000) Mod 60)).ToString("D2")
            Dim ms As String = Convert.ToUInt16(Math.Truncate(Math.Abs(milliseconds) Mod 1000)).ToString("D2").Substring(0, 2)
            If neg Then
                Return "-" & h & ":" & m & ":" & s & "." & ms
            Else
                Return h & ":" & m & ":" & s & "." & ms
            End If
        Else : Return "00:00:00.00"
        End If
    End Function


    ''' <summary>
    ''' Returns the time in milliseconds needed to play the given number of frames at a specified framerate.
    ''' </summary>
    ''' <param name="frames">the number of frames</param>
    ''' <param name="fps">the framerate multiplied by 100 to avoid floating numbers like 59.94.</param> 
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getTimeInMS(ByVal frames As Long, ByVal fps As Integer) As Long
        If fps = 0 Then fps = -100
        Return (frames * 1000) / (fps / 100)
    End Function

    Public Function getFPS(ByVal channel As Integer) As Integer
        If containsChannel(channel) Then
            Return channelFPS(channel - 1)
        End If
        Return 0
    End Function

    ''' <summary>
    ''' Returns the framerate of the spezified channel or 0 if the channel does not exist.
    ''' </summary>
    ''' <param name="channel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getChannelFPS(ByVal channel As Integer) As Integer
        If isConnected() Then
            If containsChannel(channel) Then
                Dim info = New InfoCommand(channel)
                Dim infoDoc As New MSXML2.DOMDocument
                If infoDoc.loadXML(info.execute(testConnection).getXMLData()) Then
                    If infoDoc.hasChildNodes Then
                        If Not IsNothing(infoDoc.selectSingleNode("channel")) AndAlso Not IsNothing(infoDoc.selectSingleNode("channel").selectSingleNode("video-mode")) Then
                            Select Case infoDoc.selectSingleNode("channel").selectSingleNode("video-mode").nodeTypedValue
                                Case "PAL"
                                    Return 2500
                                Case "NTSC"
                                    Return 2994
                                Case Else
                                    If infoDoc.selectSingleNode("channel").selectSingleNode("video-mode").nodeTypedValue.Contains("i") Then
                                        Return Integer.Parse(infoDoc.selectSingleNode("channel").selectSingleNode("video-mode").nodeTypedValue.Substring(infoDoc.selectSingleNode("channel").selectSingleNode("video-mode").nodeTypedValue.IndexOf("i") + 1)) / 2
                                    ElseIf infoDoc.selectSingleNode("channel").selectSingleNode("video-mode").nodeTypedValue.Contains("p") Then
                                        Return Integer.Parse(infoDoc.selectSingleNode("channel").selectSingleNode("video-mode").nodeTypedValue.Substring(infoDoc.selectSingleNode("channel").selectSingleNode("video-mode").nodeTypedValue.IndexOf("p") + 1))
                                    End If
                            End Select
                        End If
                    End If
                Else
                    logger.err("ServerController.getChannelFPS: Could not get channel fps. Error in server response: " & infoDoc.parseError.reason & " @" & vbNewLine & info.getResponse.getServerMessage)
                End If
            Else
                logger.err("ServerController.getChannelFPS: Could not get channel fps for channel " & channel & ". Channel does not exist.")
            End If
        End If
        Return -1
    End Function

    ''' <summary>
    ''' Returns a Dictionary of all media and templates on the server key by their names.
    ''' If withMediaInfo is true, all mediaItems will have filled mediaInfo which is default but need more time.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getMediaList() As Dictionary(Of String, CasparCGMedia)
        Dim media As New Dictionary(Of String, CasparCGMedia)
        Dim cmd As ICommand
        '' Catch the media list and create the media objects
        If isConnected() Then
            cmd = New ClsCommand()
            If cmd.execute(testConnection).isOK Then
                For Each line As String In cmd.getResponse.getData.Split(vbCrLf)
                    line = line.Trim()
                    If line <> "" AndAlso line.Split(" ").Length > 2 Then
                        Dim name = line.Substring(1, line.LastIndexOf("""") - 1).ToUpper
                        line = line.Remove(0, line.LastIndexOf("""") + 1)
                        line = line.Trim().Replace("""", "").Replace("  ", " ")
                        Dim values() = line.Split(" ")
                        Select Case values(0)
                            Case "MOVIE"
                                media.Add(name, New CasparCGMovie(name))
                                media.Item(name).setInfo("Duration", getTimeStringOfMS(getOriginalMediaDuration(media.Item(name))))
                                ' get Thumbnail
                                cmd = New ThumbnailRetrieveCommand(name)
                                If cmd.execute(testConnection).isOK Then
                                    media.Item(name).setBase64Thumb(cmd.getResponse.getData)
                                End If
                            Case "AUDIO"
                                media.Add(name, New CasparCGAudio(name))
                                'media.Item(name).setInfo("Duration", getTimeStringOfMS(getOriginalMediaDuration(media.Item(name))))
                            Case "STILL"
                                media.Add(name, New CasparCGStill(name))
                        End Select
                        'media.Item(name).parseXML(getMediaInfo(media.Item(name)))
                        media.Item(name).fillMediaInfo(testConnection, testChannel)
                    End If
                Next
            End If

            '' Catch the template list and create the template objects
            cmd = New TlsCommand()
            If cmd.execute(testConnection).isOK Then
                For Each line As String In cmd.getResponse.getData.Split(vbCrLf)
                    line = line.Trim.Replace(vbCr, "").Replace(vbLf, "")
                    If line <> "" AndAlso line.Split(" ").Length > 2 Then
                        Dim name = line.Substring(1, line.LastIndexOf("""") - 1).ToUpper
                        media.Add(name, New CasparCGTemplate(name))
                        'media.Item(name).parseXML(getMediaInfo(media.Item(name)))
                        media.Item(name).fillMediaInfo(testConnection, testChannel)
                    End If
                Next
            End If
        End If
        Return media
    End Function

    Public Shared Function getBase64ToImage(ByVal base64string As String) As System.Drawing.Image
        'Converts the base64 encoded msg to image data
        Return System.Drawing.Image.FromStream(New System.IO.MemoryStream(Convert.FromBase64String(base64string)))
    End Function

    Public Function getTicker() As FrameTicker
        Return ticker
    End Function

    Public Sub stopTicker()
        If Not IsNothing(ticker) Then
            ticker.stopTicker()
        End If
    End Sub

    Public Sub startTicker()
        If Not IsNothing(ticker) Then
            ticker.startTicker()
        End If
    End Sub

    Public Sub toggleTickerActive()
        ticker.toggleTicker()
    End Sub

End Class


Public Class frameTicker
    Private sysTimer As Timers.Timer
    Private frametime As Integer
    Private sc As ServerControler

    Public Sub New(ByVal controller As ServerControler, Optional ByVal frameInterval As Integer = 1)
        sc = controller

        For i As Integer = 1 To sc.getChannels
            If frametime > 1000 / (sc.getFPS(i) / 100) OrElse frametime < 1 Then frametime = 1000 / (sc.getFPS(i) / 100)
        Next

        If frameInterval > 1 Then
            sysTimer = New Timers.Timer(frametime * frameInterval)
        Else
            sysTimer = New Timers.Timer(frametime)
        End If
        sysTimer.Enabled = False
        AddHandler sysTimer.Elapsed, Sub() RaiseEvent frameTick()
    End Sub

    Public Event frameTick()

    Public Sub startTicker()
        sysTimer.Enabled = True
    End Sub

    Public Sub stopTicker()
        sysTimer.Enabled = False
    End Sub

    Public Sub toggleTicker()
        sysTimer.Enabled = Not sysTimer.Enabled
    End Sub

End Class



''
'' OLD TICKER - NOT USED ANYMORE
''

' ''' <summary>
' ''' A ticker class which raises events if a frame change is noticed in one of the casparCG channels. 
' ''' There is no waranty that a tick will be rissen for every framechange at all, but the frame number send should be close to the real one.
' ''' The period of ticks is close to "per frame" but will increase with every handler added for the frameTickEvent. 
' ''' Nevertheless, the precision of the frame number given by the event will not be affect much by increasing the eventHandler count.
' ''' During each poll request to the server for the current frame number, a short period of interpolated ticks will be rissen.
' ''' These are only calculated and not proofed to be in correct sync to the server. The length of this period could be configured.
' ''' Use bigger values if you have a poor network  conncetion. Default is 5 seconds (5000ms). It is possible that there will be no 
' ''' interpolated tick at all if the proccessing of the poll and raising the event takes longer than the given period.
' ''' In that case, you will only get received values form the server when they arrive.
' ''' It is very likley that the given frame number is behind the frame number at the servers channel by a few frames.
' ''' Since this delay may differ depending on your networkconnection and hardware, it should be, more or less, constant 
' ''' and will be minimized by a simple compensation technique.
' ''' 
' ''' To take load off your cpu, you could set a upper bound to the frequency at which frameTickEvents should be rissen.
' ''' As a default, it will be tried to raise one for every frame. But this is just a trie. Depending on your hardware and the number
' ''' of EventHandler, it may be only every few frames.
' ''' If you don't need high frequencies, use higher values to take load off your cpu.
' ''' Take in mind, that the bound will be related to the channel with the highest fps. So if you have a channel with p25 and one with p50 and
' ''' a frameInterval of 4, channel p50 will tick not more than every 4 frames but channel p25 could still tick every 2 frames.
' '''  
' ''' Start tick() in a new Thread and register handlers for the frameTick event. 
' ''' Keep in mind to use delegates since the event will likely to be rissen by a different thread.
' ''' </summary>
' ''' <remarks></remarks>
'Public Class FrameTicker
'    Private sc As ServerControler
'    Private con As CasparCGConnection
'    Public interpolationTime As Integer
'    Private frameInterval As Integer
'    Private channels As Integer
'    Private channelFameTime() As Integer
'    Private channelLastUpdate() As Long
'    Private channelFrame As New Dictionary(Of Integer, Long)
'    Private minFrameTime As Integer = Integer.MaxValue
'    Private sysTimer As Timers.Timer

'    Public Event frameTick(ByVal sender As Object, ByVal e As frameTickEventArgs)

'    ''' <summary>
'    ''' Creates a new ticker instance.
'    ''' </summary>
'    ''' <param name="con">the connection to poll the channels for the actual frame number</param>
'    ''' <param name="controller">the servercontroler</param>
'    ''' <param name="interpolationTime">the number of milliseconds between each servercall. In that time, the frame tick will be interpolated by a local timer which may differ from the servers real values.</param>
'    ''' <param name="frameInterval">the desired interval in which frameTickEvents should be rissen. This is just a desired value and will only give a upper bound but not a lower bound. Default is 1 tick per frame</param> 
'    ''' <remarks></remarks>
'    Public Sub New(ByRef con As CasparCGConnection, ByVal controller As ServerControler, Optional ByVal interpolationTime As Integer = 5000, Optional ByVal frameInterval As Integer = 1)
'        sc = controller
'        Me.con = con
'        Me.interpolationTime = interpolationTime
'        Me.frameInterval = frameInterval

'        channels = sc.getChannels 'Anzahl der Channels bestimmen

'        ReDim channelFameTime(channels)
'        ReDim channelLastUpdate(channels)
'        For i As Integer = 1 To channels
'            channelFameTime(i - 1) = 1000 / (sc.getFPS(i) / 100)
'            If minFrameTime > channelFameTime(i - 1) - 1 Then minFrameTime = channelFameTime(i - 1)
'            channelFrame.Add(i, 0)
'        Next

'        '' Test: Use timer instead of server syncronised frames as we don't need it anymore
'        sysTimer = New Timers.Timer(minFrameTime)
'        sysTimer.Enabled = False
'        AddHandler sysTimer.Elapsed, Sub() RaiseEvent frameTick(Me, Nothing)

'        logger.debug("frameTicker.New: Ticker init by thread " & Thread.CurrentThread.ManagedThreadId)
'    End Sub

'    Public Sub tick()
'        logger.debug("frameTicker.tick: Ticker thread " & Thread.CurrentThread.ManagedThreadId & " started")

'        If Not sysTimer.Enabled Then
'            sysTimer.Enabled = True
'        Else
'            sysTimer.Enabled = False
'        End If


'        '    Dim timer As New Stopwatch ' Timer to measure the time it takes to calc current frame / channel and inform listeners
'        '    Dim offsetTimer As New Stopwatch
'        '    Dim iterationStart As Long
'        '    Dim iterationEnd As Long
'        '    Dim interpolatingSince As Integer

'        '    Dim infoDoc As New MSXML2.DOMDocument
'        '    Dim frame As Long = 0
'        '    Dim ch = 0
'        '    Dim info As New InfoCommand(1, Integer.MaxValue)

'        '    Try
'        '        timer.Start()
'        '        offsetTimer.Start()
'        '        While True
'        '            ' Alle channels durchgehen
'        '            For channel As Integer = 1 To channels
'        '                ch = channel - 1
'        '                'werte einlesen
'        '                DirectCast(info.getParameter("channel"), CommandParameter(Of Integer)).setValue(channel)
'        '                offsetTimer.Restart()
'        '                infoDoc.loadXML(info.execute(con).getXMLData)
'        '                frame = infoDoc.firstChild.selectSingleNode("frame-number").nodeTypedValue + (offsetTimer.ElapsedMilliseconds / 2 / channelFameTime(ch))
'        '                offsetTimer.Stop()

'        '                ' Korrigierten Wert für die Frame number berechen aus dem rückgabewert des servers + der frames die in der bearbeitungszeit
'        '                ' vermeintlich vergangen sind. Wir gehen vereinfacht davon aus, das die hälfte der Zeit für den Rückweg 
'        '                ' vom Server zu uns gebaucht wurde da wir das nicht genau messen können.
'        '                If frame <> channelFrame.Item(channel) Then
'        '                    channelFrame.Item(channel) = frame
'        '                End If
'        '            Next
'        '            ' Event auslösen
'        '            RaiseEvent frameTick(Me, New frameTickEventArgs(channelFrame))
'        '            logger.debug("FrameTicker.tick: Raise frameTick()")

'        '            ' Jetzt ein paar frames nur rechnen und dann wieder mit dem Serverwert vergleichen
'        '            Dim hasChanged = False
'        '            interpolatingSince = timer.ElapsedMilliseconds
'        '            While timer.ElapsedMilliseconds - interpolatingSince < interpolationTime
'        '                iterationStart = timer.ElapsedMilliseconds
'        '                For channel As Integer = 0 To channels - 1
'        '                    ' Interpoliere frames indem wir die vergange zeit der der letzen aktualisierung betrachten.
'        '                    ' Ist sie größer als die frameTime müssen wir die frame ändern.
'        '                    If iterationStart - channelLastUpdate(channel) >= channelFameTime(channel) Then
'        '                        hasChanged = True
'        '                        channelFrame.Item(channel + 1) = channelFrame.Item(channel + 1) + ((iterationStart - channelLastUpdate(channel)) / channelFameTime(channel))
'        '                        channelLastUpdate(channel) = iterationStart
'        '                    End If
'        '                Next
'        '                '' Event auslösen
'        '                If hasChanged Then
'        '                    RaiseEvent frameTick(Me, New frameTickEventArgs(channelFrame))
'        '                    logger.debug("FrameTicker.tick: Raise frameTick()")
'        '                End If
'        '                hasChanged = False

'        '                ' Jetzt noch ein bisschen warten um die cpu zu entlasten. Mindestens bis die nächste frame möglich ist
'        '                ' oder soviele Frames wie im frameInterval angegeben
'        '                iterationEnd = timer.ElapsedMilliseconds
'        '                If iterationEnd - iterationStart < (minFrameTime * frameInterval) Then
'        '                    While timer.ElapsedMilliseconds < iterationEnd + (minFrameTime * frameInterval) - (iterationEnd - iterationStart)
'        '                        'Thread.Sleep((minFrameTime * frameInterval) - (iterationEnd - iterationStart))
'        '                    End While
'        '                End If
'        '            End While
'        '        End While
'        '    Catch e As ThreadInterruptedException
'        '    End Try
'    End Sub
'End Class

' ''' <summary>
' ''' The frameTick event result containing the channel which ticked and the actual framenumber.
' ''' </summary>
' ''' <remarks></remarks>
'Public Class frameTickEventArgs
'    Inherits EventArgs

'    Public Sub New(ByVal result As Dictionary(Of Integer, Long))
'        _result = result
'    End Sub

'    Private _result As Dictionary(Of Integer, Long)
'    Public ReadOnly Property result
'        Get
'            Return _result
'        End Get
'    End Property
'End Class




