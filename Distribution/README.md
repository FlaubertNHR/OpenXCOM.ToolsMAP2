MapView stores settings in a subfolder of itself. If you do not have user-permissions to write to that folder - eg. Program Files - you may have to either enable them or install MapView to a different hierarchy on your hardrive - eg. C:\\

Automated builds for Mono only: https://lxnt.wtf/oxem/#/MapView by Stoddard.<br>
notes for Mono/Linux/Mac users
- automated Mono builds might not be stable; the prefered tactic is to build against Mono on your own computer (natively).
- MapView might need to run once and then be restarted for its window(s) to resize properly.
- if nontransparent black boxes appear all over the place, go to MainView's Options and turn on "UseMono".
- .NET on Mono tends to layout and render differently than in Windows, so pls look for usability rather than perfection.
- the Mono package does not contain the CHM helpfile or the keyboard_cheatsheet. They're available in the Windows archive and are recommended since they describe shortcuts, functionality, etc.


<br>
Distribution builds for Windows 32/64 is hosted on Google Drive.

Built against .NET 4.5.1

2020 December 7<br>
[MapView2_201207.7z](https://drive.google.com/file/d/1Uyc4uNVNDXYppSdy32AunuRaXhACyQJr/view?usp=sharing)

MapView.exe - 3.8.1.0
- fix exception in TopView when placing the null-part (eraser)
- close TilepartSubstition dialog on [Ctrl+U]
- tiptext tweaks in RouteView re. the auto-connector
- remove some redundant vars in the Designers

2020 December 6<br>
[MapView2_201206.7z](https://drive.google.com/file/d/1mKJIW7A8pgI61njIz09F6_9AcuFJFoxP/view?usp=sharing)

MapView.exe - 3.8.0.0 - code and UI tweaks generally

MainView
- addition to TilepartSubstition [Ctrl+u] specify a range of placed tileparts
  and shift their ids to different parts in the terrainset (useful when parts
  have been added to or removed from a terrain in either McdView or McdEdit
  externally). reworked dialog
- disallow placing a tilepart with an id greater than 253 (x-com Mapfiles still
  can't deal with it, sry)
- add Toner dropdown to the menubar (renders selected tiles in a monochrome
  colorscale - [F10] see options)
- add option "StartTopRoutePage" to start TopRouteView with a tab-page selected
  (the TopView panel or the RouteView panel)
- slightish optimizations to the draw routines: redraw the panel only when the
  cursor moves in and out of a tile instead of every pixel and eg, cache
  quadrant visibility flags from TopView for use by DrawTile() etc.
- fix drawing parts with a y-offset in their MCD record
- add option "LayerSelectionBorder" [F9] cycles through 3 ways to draw the
  selection border (0) at the grid level (1) at both the grid level and the
  level above (2) at the level above only (helpful when working with a bunch of
  big blocks of dirt eg.)
- add option "OneTileDraw" [Ctrl+F9] toggles whether or not to draw a selection
  border when only 1 tile is selected (helpful when large and obscuring sprites
  hide the tile you really want)
- add option "InvertMousewheel" to change levels in the opposite direction
- display cursor location in the panel
- draw temporary tilepart placeholders on the Map if there are any parts with
  ids that exceed the currently allocated terrainset. Saving the Mapfile shall
  irrevocably clear such rogue tileparts, so if you want to get legitimate
  terrain-parts back in those slots do it right away. A dialog will be shown
  when the tileset re/loads with vague instructions; vague because whatever went
  wrong and how to fix it is really in your court. The only thing that Mapview2
  detects is that there are placed tileparts with ids that exceed the bounds of
  the current terrainset.

TopView
- respect options Base1_xy and Base1_z in TestPartslots dialog
- close the TestPartslots dialog if it's open when options Base1_xy or Base1_z
  change (displayed data is no longer valid)
- properly close an old TestPartslots dialog if user recalls the test
- display cursor location and selected tile location in the panel
- stop redundant handling of a mousedown event ...

TopView.QuadrantPanel
- dispose old brushes when they change
- update quad-swatches when their color changes in options

RouteView
- respect options Base1_xy and Base1_z when displaying locations of invalid
  route-nodes
- refactor automatic node-connection routines
- faster location tracking and less redundant panel drawing
- add option "ReduceDraws" to further speed panel drawing (but the InfoOverlay
  will be drawn at stepped locations) (default false)
- when a node is deleted clear all linked nodes' link-Type and -Distance (as
  well as -Destination which was already done)
- add option "StartConnector" to start RouteView with your prefered
  auto-node-linker setting (default none)

MapInfoDialog
- add a Refresh button

TilesetEditor
- add button that lists Tilesets that are defined by Path+Map (info for global
  terrain allocation)

Configurator
- replacing tileset metadata (MapTilesets.yml) with the hardcoded manifest
  requires explicit user-confirmation

ScanG
- cycle single/multi/full-layer view on RMB click or [L] (was LMB double-click)
- reload ScanG.dat on [Enter] (was RMB double-click)
- close the viewer on [Esc] or [Ctrl+G]
- raise/lower the view-level with numeric keypad [Subtract] and [Add]
- keep view-level synched with the other viewers


McdView.exe - 3.4.2.0
- don't clear MapView's static spritesets when invoked by TileView
- set spriteshade from MainView options on load
- allow user to choose ScanG and/or LoFT for either/both UFO and TFTD resources

PckView.exe - 3.3.5.0
- select a sprite correctly when invoked by TileView
- set spriteshade from MainView options on load

DSShared.dll - 3.3.3.0
- better check for isOnScreen when instantiating the viewers

XCom.dll - 3.6.0.0
- integral support for MapView.exe, McdView.exe, and PckView.exe changes.

global:
- changed the y/x/z coordinate code-patterns to x/y/z
- force strings to be interpreted for usage, storage and/or display by the
  InvariantCulture rule (except for very few cases)
- store and restore the height of the Description area for each viewer's
  Options-panel
- bypass all close routines when the OS wants to shut down

- update CHM helpfile and keyboard_cheatsheet.txt with the added option keys

big thanks to Kato for advocating enhanced TilepartSubstitution


OnDetectBugs(EventArgs repro)
{
   report(kevL, repro);
}

GREAT TIME TO BACK UP ALL OF YOUR /MAPS, /ROUTES, and /TERRAIN (mcd/pck/tab)

just do it ... i know you know i want you to know i want you to !
I did a lot of things that can conceivably bork up. Treat as beta+

2020 February 22<br>
[MapView2_200222.7z](https://drive.google.com/file/d/1Agy0eLoc_glB07ij5wXOv34oqOiZ5NIl/view?usp=sharing)

MapView 3.6.1.0
- fix [Enter] not working on the search-box

MapView 3.7.0.0
- Options in MainView:
(1) Base1_xy - print position of rows and columns 1-based instead of (default) 0-based
(2) Base1_z - print position of levels 1-based instead of (default) 0-based
(3) IgnoreRecordsExceeded - a global setting to ignore any warnings that the count of MCD records has been exceeded for a tileset
- Menuitem in MainView|Edit:
Clear BypassRecordsExceeded flags - this will clear all BypassRecordsExceeded flags that have been set over all tilesets

McdView 3.4.1.0
- fixed a graphics glitch where the sprites of dead or alternate parts that have a y-offset were being drawn above the destination rectangle.

2019 November 16<br>
[MapView2_191116.7z](https://drive.google.com/file/d/1bM25j24XevtnMSjSe-_8YZlr6ms04O6a/view?usp=sharing)
- Tileslot Substitution [Ctrl+U] - opens a dialog box for replacing tileparts of a given setId with tileparts of a different setId across the currently loaded Map. Tileparts of a given setId can optionally be cleared from a Map.

2019 October 24<br>
[MapView2_191024.7z](https://drive.google.com/file/d/1v5X7QLBXRuOXEjjvzNghnxh9PzFsSVKt/view?usp=sharing)

- fixed PckView not starting. Thanks to luke83 for reporting and helping troubleshoot the probl.
- plus tweaks for Mono (if applicable). Thanks to osd_daedalus for reporting Mono issues and testing changes.

2019 October 2<br>
[MapView2_191002.7z](https://drive.google.com/file/d/1gpcPF3i2BjYFcaSwPItVBxG9YG2xNioh/view?usp=sharing)

MapView
- show error if configuration files failed to create.
- handle any errors while loading the UFO/TFTD targeters (CURSOR.PCK+TAB) better.
- force-quit if a targeter-sprite does not get loaded.
- use an explicit Descriptor.GroupType variable instead of current palette to determine whether a tileset is for UFO or TFTD.

PckView
- if a PCK vs TAB mismatched count error occurs print the counts in the error that pops.
- if a sprite-overflow happens don't assume that a Bigobs sprite was trying to load into a terrain/unit spriteset (just show a generic error).
- reset titlebar text if a spriteset fails to load (that is, don't assume that a spriteset will load when setting the titlebar text).

XCom
- set the GroupType (aka. GameType: UFO/TFTD) in the Descriptor of each tileset.
- fixed a bad typo that caused all tilesets to use the UFO battle-palette; this made editing TFTD tilesets problematic at best, since it also affected which terrains were trying to be accessed. Fixed ...
- slight refactor of TileGroup loading (based on above fix).
- use a bitwise int to store spritesets' failure-to-instantiate status, instead of three booleans.
- add spriteset fail-state: Taboffset overflow condition.

2019 August 31<br>
[MapView2_190831.7z](https://drive.google.com/file/d/1PNaG4crWloakkeydy-XOPnIeGeNxSsp0/view?usp=sharing)

This is a very large update.

IMPORTANT: The MapOptions.Cfg file in your /settings folder will be changed. So back that file up if you like your current MapViewII customizations and want to transfer them to a/the new MapOptions config (i would strongly suggest using the Options dialogs instead of dicking about with the textfiles).

IMPORTANT: The so-called backup mechanic for MapView, McdView, and PckView has also changed. This is not a comprehensive backup mechanic and was never intended to be that. It's intended only to give the user (including yours truly) that ONE chance on a misclick to go *oh shyte i shouldn't have done that* and have a chance of recovering the file you just overwrote(1). The problem was that the routines were inconsistent; sometimes you got a .bak file, sometimes it wrote to an MV_Backup subdirectory, sometimes you got nothing at all, etc - so I've implemented a plan: any and every time a file on the hardrive(2) is changed by MapView, McdView, or PckView, it is first copied to an MV_Backup subdirectory of its original location, with its original filename.

(1) note: This should also handle more hazardous failures like a power outage during file-write but let's not go there. Get yourself a UPS. (seriously.)

(2) note: Does not include image-output; exporting images does *not* perform backups. But saving MAP, RMP, MCD, PCK, and TAB files as well as configuration files should/will get their instant backups.

MainView
- RMB-drag scrolls the Map when scrollbars are visible
- Ctrl+F1..F4 toggles parttype visibility as per TopView F1..F4
- F1 opens the CHM Helpfile (was start animations)
- F2 toggles animations (was stop animations only)
- always draw targeter-cursor during keyboard navigation
- fixed multi-tile select by keyboard
- fixed exception on [Esc] and/or MouseWheel when no Mapfile is loaded
- moved ModifyMapSize from File menu to Edit menu
- reworked statusbar aesthetic
- allow keyboard navigation through the Maptree without loading any Maps until [Enter] is pressed
- expand/collapse parent nodes on the Maptree w/ [Enter]
- changed the hotkey for opening the Maptree's context menu from [Enter] to [Space]
- refactored the MenuManager (handles viewer-open/close from the Viewers menu)
- fixed CTD when taking full-level screenshots.
- tightened up Maptree handling (loading/writing the Maptree; allow a blank MapTree; don't write group or category padding-text to MapTilesets.yml if there are no tilesets in the group/category anyway).
- faster loading of Tilesets (much faster if you have many toplevel groups)
- added to Options: screenshot background color (default Transparent) and crop background (default false) and choice of PNG or GIF output (default PNG)
- added to Options: color toners for SelectedTiles (none,grayscale,redscale,greenscale,bluescale - default grayscale)
- rewrote the CollapsibleSplitter (the divider between the Maptree and the main panel)

TileView
- changed the null-sprite aka. eraser-sprite
- added mnemonics to menus
- reworked statusbar aesthetic
- prevent tab flicker

TopView
- tweaked the parttype null-sprites
- double-leftclick on a tile selects the tilepart of the currently selected quadrant (also [t])
- rightclick and double-rightclick (also [Enter] and [Shift+Del]) operate on either a tile or the quadrant-panel.
- display the currently selected tilepart's sprite in the quadrant-panel; click (also [q]) selects its proper quadrant-slot
- ignore the spaces between quadrant-types when clicking to select a type
- reset the quadrant-panel's sprites after loading or resizing a Map
- fixed multi-tile select by keyboard
- added mnemonics to menus
- added Test->Test parts in tileslots to check the types of all parts (floor, westwall, northwall, content) that are currently assigned to tiles against the type of tileslot they occupy. The result is shown in a dialog.

RouteView
- Tally button displays current spawn info for the tileset and its category
- clear previously selected tile's coordinate info after resizing a Map
- print mouseovered node's baseattack-weight in the overlay, iff non-zero
- add a Save button to the data area (doubles as a "routes changed" indicator)
- fixed: set MapLocation on LMB-dragnode so that keyboard-dragnode doesn't go dysfunctional.
- added mnemonics to menus and arranged menuitems

TopRouteView
- prevent tab flicker

TilesetEditor
- fixed up Tab-order (ie. keyboard navigation around the dialog)
- try to keep listboxes focused with a reasonable item selected when allocating/deallocating terrains
- disable the ACCEPT button until a Descriptor is valid.
- fixed the global descriptor changer (if a tileset with the same basepath exists in 2+ categories, both get their metadata changed)
- handle terrain-not-found errors less annoyingly
- added a checkbox to suppress the MCD RecordsExceeded warning (saved in MapTilesets.yml w/ each tileset)

ScanG
- fixed: don't draw the SE quadrant of large units (sic)
- added viewer to the zOrder list
- register its screen position on MainView quit
- moved item from File to Viewers menu
- switch icons and palette between UFO/TFTD resources when a Mapfile loads.
- enable/disable the ScanG menuitem depending on tileset and available resources

MapInfo
- moved item from File to Help menu
- added a Detail button that shows what MCD records are used/unused in the currently displayed Map's terrainset

Configurator
- use custom messagebox to display info/errors that need to print paths (prevents wrapping long paths)
- don't close the configurator when simply generating a MapTilesets template

General
- reworked load/save Options (MapOptions.cfg) incl/ fixes to Content-type blob updates
- rewrote Options interaction in toto (tighter integration w/ viewers' shortcuts, improved code refactorability, etc.)
- reworked load/save viewer locations and sizes (MapViewers.yml)
- try to maintain z-order on (1) open/close McdView or PckView (2) MinimizeAll/RestoreAll (3) option "AllowBringToFront" when MainView takes focus.
- changes to spriteset loading (determine tabword-length based on the tab-file data instead of failure to instantiate a 2-byte wordlength spriteset) [idea lifted from OXC code]
- rewrote tile-loading and the default-tileset creation routines
- reworked double-buffering calls to what I believe are the most simple/efficient yet effective
- re-select the current treenode on the Maptree if the Configurator forces a restart; plus use what seems to be a more stable restart routine.
- rewrote the Infobox (a dialog that replaces .net's stock MessageBox here and there)
- restrict keyboard shortcuts to exact key or key-combinations (eg. [Shift+Enter] no longer invokes an operation that's intended for [Enter] only)
- tighter file handling, IO exception handling, one-time safety backup handling, to-load-or-not-to-load handling, file corruption handling, file IO error message handling, handling handling and file-fondling in general.

McdView
- ensure that the CopyPanel is closed on exit, else it can stick open when invoked via TileView
- RMB-click on the IsoLoft panel reverts slider to show all layers
- fixed: write the ScanG ushort as little-endian regardless of computer architecture.
- allow loading of filetype via Explorer's file-associations
- properly dispose temporary LoFT and ScanG bitmaps
- allow the Isoloft viewer to track all the way down to 0 layers and use 3-step shading (instead of 2-step) while tracking layers.

ScanG chooser
- fixed: handle iconsets that have less than 35 icons (the first 35 icons are for units not terrains)

PckView
- fixed saving sprites that do not have a fully transparent top row(!)
- fixed File|Export Sprites ...
- reworked statusbar aesthetic
- minimize/restore the Sprite and Palette windows when the main window's state minimizes/restores
- bring Sprite and Palette windows to front when the main window takes focus
- apply MapView's gamma adjustment to sprites and swatches (can be toggled from the Palette menu, default is On)
- resolved conflict between contextmenu shortcut [p] (export sprite) and mnemonic [Alt+p] (open Palette menu)
- allow loading of filetype via Explorer's file-associations

SpriteEditor
- flag Changed if pixels changed
- reworked statusbar aesthetic
- added mnemonics to menus

Palette viewer
- properly dispose created brushes
- reworked statusbar aesthetic

Updated CHM helpfile and keyboard_cheatsheet.

ConfigConverter
- properly dispose the openfile dialog

RulesetConverter
- add.

IMPORTANT: This is a good time to backup your irreplaceable MAP, RMP, MCD, PCK, and TAB files(!)

Backing up your **\settings** folder is a good idea also. Especially **MapTilesets.yml** (... the other configuration files are easy to replace or regenerate) and, for the reasons given above, **MapOptions.Cfg**.

in fact, _if i were you_ I'd simply rename my current MapView2 installation ( eg. MapView2_ ) and install this version alongside it. Run it once to create the /settings directory, quit and *copy* in your **MapTilesets.yml** ... Then take it from there,

2019 June 24<br>
[MapView2_190624.7z](https://drive.google.com/file/d/1CKwhDQAJEBJdQI9lqwlCLSwv3L_wlcmT/view?usp=sharing)

- possible fix for intermittent exception related to MCD Info, caused by discrepancy between the way MCD records load in MapView and (external) McdView (after being invoked via TileView).

2019 June 9<br>
[MapView2_190609.7z](https://drive.google.com/file/d/1DLcpMzsX0c1Y1y_ggod_yDYLlkOW9xB6/view?usp=sharing)

- Cut/Paste/Delete respect TopView's Visible toggles

2019 May 12<br>
[MapView2_190512.7z](https://drive.google.com/file/d/1sBuSBipnK-kaOxWKMIjh-rDo_38YHw8N/view?usp=sharing)

- PckView upgrade. arrow-keys navigate sprites, [Delete] selects next sprite (allowing quick deletion of a range of sprites), shortcuts have been re-assigned, update CHM helpfile, confirmation on save if spriteset has changed, and the selected sprite's TabOffset is printed to the statusbar along with the offset of the 'next' sprite.

2019 May 10<br>
[MapView2_190510.7z](https://drive.google.com/file/d/1Ce3FK-DDHB8JpkQ1pRnoNG8sRR_1aVAh/view?usp=sharing)

- McdView's CopyPanel gets InsertAfterLast: optionally inserts sprites as well as death/alternate parts to the terrainset

2019 April 27<br>
[MapView2_190427.7z](https://drive.google.com/file/d/1xJv0KrPDdj8EDRxVG2lkmHxMAFLNII1t/view?usp=sharing)

- add CopyPanel to McdView + tweaks. The CopyPanel is a subsidiary window that can open an MCDset, from which records may be copied for insertion into McdView proper

2019 April 22<br>
[MapView2_190422.7z](https://drive.google.com/file/d/1SPjj31nwhw_65A7ph86PjWQmjE0FNxi8/view?usp=sharing)

- keyboard shortcuts: see keyboard_cheatsheet.txt or the CHM Helpfile
- several UI changes, insubstantial (mechanically) but noticeable
- the Maptree gets double-buffered to stop flicker (non-Mono build only)

2019 April 7<br>
[MapView2_190407b.7z](https://drive.google.com/file/d/1ZFZ2SmzzW1ZZ7QzWsqTPkLEEF9wD7j_V/view?usp=sharing)

- McdView: initial release/beta!
- several changes to MapView ...

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
