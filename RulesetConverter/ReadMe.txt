RulesetConverter ver.1
2019 aug 26


RulesetConverter is a utility that converts data from an OxC ruleset file to a YAML configuration (template) file for MapViewII.

usage:
Input a .RUL file, specifying whether it is for UFO or TFTD. There is also a checkbox to add a Basepath to the output. A basepath is needed if your current MapView resource directory is not that of the MAPS, ROUTES, and TERRAIN folders from which tilesets are being configured. That is, you do not need to specify a basepath if you intend to configure MapView to use the basepath of the files that are being configured (hint: see MapViewII's Configurator on its Edit menu) or if the new tilesets are already in that basepath; on the other hand, if you want MapView to keep whatever basepath it's already using, and you don't want to copy the new tilesets to the appropriate folders of that basepath, the new tilesets need a custom basepath (specified by the add Basepath directory).

A basepath, by the way, is the parent directory of the MAPS, ROUTES, and/or TERRAIN folders. MapViewII leverages that standard aspect of the XCOM folder-hierarchy.

This converter outputs tileset configuration data to a file "MapTilesets.tpl", which it creates in its own directory if and only if tilesets were found in the ruleset file that was used as input. The file is actually a valid "MapTilesets.yml" file, that could be moved to MapViewII's /settings directory and used (after renaming the extension). Which may or may not be fine depending on the resource directory(s) that have been set by MapView's Configurator, but then you'd need to overwrite your current "MapTilesets.yml" file (if you have one, and it's very likely you do since it gets created when MapViewII is installed on first run). So, I suggest getting your ducks in order and *copying* the tileset-data from the output (the .TPL file) to your already existing "MapTilesets.yml" file - with or without basepaths added, as explained above.

Note that tilesets are stored in OxC ruleset files under nodes labeled "terrains". Also note that MapViewII doesn't call tilesets terrains. It calls terrains terrains and tilesets tilesets*


- kevL

* terrains are what you get in XCom TERRAIN directories; tilesets are a MAP file + its RMP file + its allocated terrains.
