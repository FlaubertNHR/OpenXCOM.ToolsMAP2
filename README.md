# MapView II Extended (WIP)

based on Mapview2 by kevL (which was based on the original MapView by Ben Ratzlaff)

.NET 4.8 is required.

Added support for new MAP2 format, which stores tileset info using 16bits instead of 8bits.
This way there is no virtual limit (well, 65534!) for tiles used on maps (MAP format has a limit of 253)

- Allows loading MAP/MAP2 files -if there are 2 MAP/MAP2 with the same name, MAP one will be loaded-
- MAP2 files are shown in blue in browser tree.
- Saves map in the same format it was loaded (either MAP/MAP2)
- Exports maps in the desired format, either MAP or MAP2.
- No "more-than-253-tileset" warning if editing a MAP2 file.
- No banning of using tiles with ID > 253 if editing a MAP2 file.
- Avoid/warn exporting to MAP file if a tile with ID > 253 is used in a the map file



