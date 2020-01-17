Caspar's Playout Client
=======================

Caspar's Playout Client is a playout client program for the [CasparCG Server] [1] with focus on playout over CG.

This project is in an early development state and far away from beeing complete.  
Never the less, the key functions are ready to go.  

Some features are still missing and the GUI is only for testing and not the final one yet.

Table of contents
-----------------

* [License](#license)
* [System requirements](#system-requirements)
* [Features](#features)
* [Screenshots](#screenshots)
* [Quick Start](#quick-start)
* [FAQ](#faq)
* [Development](#development)



License
-------

This software is licensed under the [Gnu GPL v3] [2].


System requirements
--------------------

In order to run Caspar's Playout Client you need  
######REQUIREMENTS
* Windows XP or higher
* NET Framework 4 or higher
* CasparCG 2.* or higher 
* An extra channel without any consumers configured at your [CasparCG Server] [1] as the highest channel. This is not needed in 1 channel configurations.
   

######RECOMMENDATIONS
* Windows 7 or higher
* NET Framework 4 or higher
* CasparCG 2.0.4 Beta or higher or any CasparCG version with OSC support  
   

In addition to the requirements, I recommend
* that media filenames doesn't contain spaces
* that media filenames only contain ANSI chars
* to keep the number of files in your casparCG media folders as low as possible. The client accepts not more than 300 items.
* to use a wired network (no WLAN) to connect casparCG with Caspar's Playout Client
* a screen resolution of at least 1280x720 - the more, the better ;-)
* to run the server and the client on different computers
  

Features
--------

![Screenshot](/screenshots/cpc_MainWindow_playing.jpg "CasparsPlayoutClient mainwindow")

CasparsPlayoutClient offers a hierarchical playlist system allowing 
* simple parallel rundowns
* simple sequential rundowns
* complex blocks of combined playlists
* looping playlists
* autostart / manual start of playlists
* delayed start
* custom duration settings for media playlists
* feedback for every playlist (duration, position, remaining)
* single server multi channel support
* xml import/export of playlists
* xml import/export of the library allowing offline playlist editing
* searching in medialibrary
* drag and drop playlist editing
* custom playlist and media naming
* collapse/expand of subplaylists


Screenshots
-----------

The playlist

* The playlist tree:  
  ![The playlist tree](/screenshots/cpc_playlist.jpg "The playlist tree")
* A running playlist in expanded view:  
  ![A running playlist in expanded view](/screenshots/cpc_playlist_all_playing.jpg "A running playlist in expanded view")
* A running playlist in collapsed view:  
  ![A running playlist in collapsed view](/screenshots/cpc_playlist_all_playing_collapsed.jpg "A running playlist in collapsed view")
* A running playlist with manuel start waiting for start:  
  ![A running playlist with manuel start waiting for start](/screenshots/cpc_playlist_waiting.jpg "A running playlist with manuel start waiting for start")
* A delayed playlist counting down to start:  
  ![A delayed playlist counting down to start](/screenshots/cpc_playlist_delayed.jpg "A delayed playlist counting down to start")
* A running playlist with near end warning active:  
  ![A running playlist with near end warning active](/screenshots/cpc_playlist_nearEndWarn.jpg "A running playlist with near end warning active")


The medialibrary

* The media library view:  
  ![The media library view](/screenshots/cpc_medialib.jpg "The media library view")
* The media library with active filter:  
  ![The media library with active filter](/screenshots/cpc_medialib_filter.jpg "The media library with active filter")


Others
* The info bar:  
  ![The info bar](/screenshots/cpc_infoBar.jpg "The info bar")
* Help for most controls:  
  ![Help tip shown](/screenshots/cpc_tooltip.jpg "Help tip shown")
* The settings window:  
  ![Custumize Caspar's Playout Client to your needs!](/screenshots/cpc_Settings.jpg "TCustumize Caspar's Playout Client to your needs!")

Quick Start
-----------

The main window is splitted into 4 areas:  
![Areas of MainWindow](/screenshots/cpc_MainWindow.jpg "CasparsPlayoutClient: 4 areas of MainWindow")  
1. The playlist (left)  
2. The media library (right)  
3. The command bar (bottom)  
4. The Info and CG controll (middle, not yet implemented)  

* **Connect**  
	First connect to your [CasparCG Server][1] at the command bar (3).
	Now, the media library (2) is loaded and you should see your mediafiles known by [CasparCG Server][1] in the list.

* **Add and remove playlist items**  
	Click and drag one of the media files to the playlist (1) and drop it there. This adds the media file to the playlist.
	If you want to delete a playlist item, right click somewhere on it (but not over a textbox) and select ***Remove item*** form the context menu. This will delete the playlist with all sub items.
	For adding a block to the playlist, right click on the playlist you want the block to belong to and choose ***Add block***.
 
* **Change and modify playlist items**  
	You can change the name of each item to a propper one. They don't need to be unique nor do they have to match the file name on the [Server][1].
	Each media file item musst have a valid channel and layer set in order to be playable. To change the channel, use the number picker beside the name box and the most right one for the layer.
	Block items don't need correct channel and layer settings. But if you set them, newly added subitems will inerhit these values saving you time.  
	There are two types of playlist:
	* parallel playlists:  
		each item of the playlist will be started together
	* sequential playlists:  
		each item starts after the previous has stopped  

	And there are the following checkboxes:
	* P: Parrallel  
	 	choose parallel playing / sequential playing
	* A: Auto  
	 	choose auto start / waiting for manual play an the playlist
	* L: Loop  
	 	choose whether or not, after the whole playlist has been played, it should be started again  
	* C: Clear  
	 	choose whether or not, after the media has been played, it should be cleared from the layer 

* **Moving playlist items**  
	You can move playlist items via drag and drop. Dropping a playlist onto an other let the dropped playlist take the place of the underlaying.
	The underlaying will be moved below the dropped.
	If you want to move a playlist from one list to an other instead of reodering it, press **CTRL** while dragging the playlist.
 
* **Play / Stop / Abort**  
	There is only one control button for start / stop / abort for each playlist.
	Each playlist could be in one of the three states giving the button it's function:
	* stopped:
		pressing play will start playback this playlist and it's items
	* playing:
		pressing stop will stop the playlist and all it's items if they are not waiting
	* waiting:
		pressing play will start the waiting playlist
	* EVER:
		pressing the play / stop button while holding **CTRL** will abort the playlist no matter in what state it is


FAQ
---

`I have more than one channel, but the client doesn't show all of them?`  

The client needs an extra channel to probe the media and retrieve it's information. If you have more than one channel, it supposes the last channel as the probing channel.
Just add an extra channel at the end. You don't need to specifiy any consumers here.  

`I have some media in a parallel block, but only one of them is playing and beeing updated in the client?`  

Make sure to use different channels/layers if you're working with parallel playlists. Also make sure that no other client stops media that you have started with Caspar's Playout Client.
Don't use the clear command to stop media, always use the Start/Stop button of the client.

Development
-----------

CasparsPlayoutClient is no longer under development nor is it maintained.

[1]: https://github.com/CasparCG/Server "CasparCG Server"
[2]: http://www.gnu.org/licenses/gpl-3.0-standalone.html "Gnu General Public License Version 3"
