MapView stores settings in a subfolder of itself. If you do not have user-permissions to write to that folder - eg. Program Files - you may have to either enable them or install MapView to a different hierarchy on your hardrive - eg. C:\\

note for Mono users: MapView needs to run once and then be restarted for its window to resize properly.<br>
Automated builds for Mono only: https://lxnt.wtf/oxem/#/MapView by Stoddard.<br>
If nontransparent black boxes appear all over the place, go to MainView's Options and turn on "UseMono".

<br>
Distribution builds for Windows 32/64 is hosted on Google Drive.

Built against .NET 4.5.1

2019 February 26<br>
[MapView2_190226b.7z](https://drive.google.com/file/d/1OF7eIp8_LTYo_jHmTR-ksz6VjcvSkzME/view?usp=sharing)

- more integration of ScanG viewer; update when the Map changes; etc.
- ScanG view: double right-click to reload SCANG.DAT from disk
- File->ReloadTerrains: reload Map/Routes/Terrains from disk (without having to click to a different Map and back, or reload the app)
- MCD Info: fix TU_Slide/TU_Fly inversion
- extend MainView's SpriteShade setting to TileView and TopView sprites and refresh all sprites when user changes the shade

2019 February 25<br>
[MapView2_190225.7z](https://drive.google.com/file/d/16Nx0gzNblEpSenPds5YUVgI52WQlSZhJ/view?usp=sharing)

- PckView: support for SCANG.DAT files
- PNG output sets palette-id #0 transparent
- minor fixes and code improvements

2019 February 21<br>
[MapView2_190221.7z](https://drive.google.com/file/d/1lu_sXFIsHBue9uTfrq1m2MWi25qcPDjD/view?usp=sharing)

- overhead ScanG viewer: MainView->File->ScanG view
- mousewheel does levelup/down; doubleclick toggles between multilevel view and single-level view
- ScanG view requires a legitimate ScanG.Dat in the GEODATA folder of the Configurator's basepath; if ScanG.Dat is not found the viewer will simply display black on all levels
- check that the reference to ScanG.Dat in a tilepart's MCD-record doesn't exceed the entries in ScanG.Dat itself

2019 February 19<br>
[MapView2_190219.7z](https://drive.google.com/file/d/1IZ_IrFpHJRb8oTd59zFwcCe4N6NtHr77/view?usp=sharing)

- show an error dialog if the quantity of sprites in the PCK file does not match the quantity expected by its TAB file
- output MainView screenshots as PNG
- PckView: print selected and over IDs as 0-based
- update CHM helpfile
- MainView toolbar: enable/disable the levelup/leveldown and scale-in/scale-out buttons when they hit their min and max values
- TileView: add statusbar that shows the count of total tileparts in assigned terrains as well as info about a tilepart when it's mouseovered

2019 February 7<br>
[MapView2_190207.7z](https://drive.google.com/file/d/1C8mEQAeIBz1EFhcEuEjkg93gd2vbaUsi/view?usp=sharing)

- RouteView: export/import .RMP files (ie, use .RMP files as templates and/or replace route-nodes of a Map with nodes from a different Map, see RouteView's File menu)
- coordinate the RoutesChanged flag between RouteView and TopRouteView(Route)
- add a custom RouteCheck infobox (instead of the default .NET messagebox display)
- minor tweak-ups to RouteView
- when a node links to an out-of-bounds node, color the text of "Link" in red (clicking the Go button ought show the RouteCheck infobox with an option to delete the rogue destination node)
- change a few displayed text-strings

2019 February 5<br>
[MapView2_190205.7z](https://drive.google.com/file/d/1JVu3qtqoXleqdj4flFlf_qS9raJtD55Y/view?usp=sharing)

- fix for first run: the initial Configuration threw a KeyNotFoundException exception (by trying to show the current, nonexistent, configuration paths in the Configurator's textboxes)
- check duplicate Group labels as case insensitive (eg. "ufo_craft" and "Ufo_CraFt" are disallowed in the Maptree together)
- allow user to browse for a basepath when attempting to load a tileset that can't be found (this ought then allow other things like terrains to be further user-adjusted in the TilesetEditor)

2019 February 4<br>
[MapView2_190204.7z](https://drive.google.com/file/d/1qwrR_li1ckdvfeK60h4-UMGLzN53a8cB/view?usp=sharing)

- show x/y/z tile-positions using 1-based counting (instead of 0-based)
- show asterisk on MainView's titlebar if the Maptree changes
- show asterisk on MainView's statusbar if the Map changes
- show "routes changed" in RouteView if the Routes change
- TilesetEditor: fix ambiguity of the Maptree changed flag
- TilesetEditor: constrain tileset-labels to Categories (instead of globally)
- TilesetEditor: print count of tilesets with identical label/basepath
- TilesetEditor: button that applies current terrainset to all tilesets with identical label/basepath

2019 January 30<br>
[MapView2_190130.7z](https://drive.google.com/file/d/1-g_sk4aPzMsEBZT2203hFUpHY57xTQ_i/view?usp=sharing)

- see Important note @2019 January 27
- MainView, TopView, RouteView Options to highlight every 10th gridline
- print current selection size x/y in statusbar of MainView
- add to RouteView Edit: node up 1 level/node down 1 level
- a Mapfile save will not write an assigned tilepart if its value exceeds ID #253
- load tileset-configs from MapTilesets.yml even if the game-type (UFO/TFTD) does not have its resources configured
- maintain tileset-configs in MapTilesets.yml even if the game-type (UFO/TFTD) is not configured
- flag RoutesChanged if routes changed when resizing a Map
- ConfigConverter: ver.2, see its ReadMe.txt

2019 January 27<br>
[MapView2_190127.7z](https://drive.google.com/file/d/1tw-WsS04Qq-ClBe2AFHOGluLHiw52jyC/view?usp=sharing)

- IMPORTANT: Major change to Tileset configuration code. This is a good time to backup all resources (/MAPS, /ROUTES, /TERRAIN) as well as settings/MapResources.yml and settings/MapTilesets.yml. Your current configuration ought still work w/out any changes. But code has been added that supports terrains that are in different folders for a Map. This required general changes across the codebase -- so backup or risk Schrödinger's wrath.
- add warning on startup if UFO/TFTD resource-paths have been disabled in MapResources.yml but there are groups configured for UFO/TFTD in MapTilesets.yml since saving the MapTree would delete those groups
- fix potential inability to exit the TilesetEditor when a tileset has been created but user then wants to cancel it
- case insensitive search for available terrain files
- assign null-tileparts if MCD-records exceed 256 (on Save Mapfile)
- etc


2019 January 19<br>
[MapView2_190119.7z](https://drive.google.com/file/d/1RjjDJjg8V35ORAIQISwlxG_Dj1DuRIyx/view?usp=sharing)

- option to workaround the transparency issue in Mono
- options to set the selection border's color and width
- verify terrains even when Canceling the TilesetEditor
- reposition a couple of toolbar buttons
- provide more information when a Map's MCD records exceeds 256
- use PNG format for inputting/outputting sprites in PckView
- support terrain/unit/bigobs sprites in PckView (but only if they have a .Tab file)

<br><br>
Previous builds

Built against .NET 3.5

2019 January 7<br>
[MapView2_190107b.7z](https://drive.google.com/file/d/1DJ3sCI-izA3N4SFH_xJ4LaFWCo8DWUuw/view?usp=sharing)

- add UFO1A (small scout) to the default MapTilesets.YML ... note to those who want access to the small scout but don't want to bork their current MapTilesets config: generate a MapTilesets.TPL via the Configurator and copy **type:UFO1A** to their working MapTilesets.YML
- issue a warning if the quantity of allocated Terrain MCD-records exceeds 256. The warning, if applicable, is shown when the Map loads by selecting it in the MapTree or when a Map's descriptor is modified causing the Map to reload (eg. terrains have been added or removed)

2019 January 5<br>
[MapView2_190105.7z](https://drive.google.com/file/d/119IjWH4-Ec5W76sg229IgSGBArPfPtNU/view?usp=sharing)

- Bigob PCK+TAB support for PckView.

2018 December 18<br>
[MapView2_181218.7z](https://drive.google.com/file/d/19vCnjBQvfJbIH13KhwoCS-4ZG_CZFSFn/view?usp=sharing)
