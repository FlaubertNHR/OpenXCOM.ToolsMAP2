﻿using System;
using System.Collections.Generic;


namespace XCom
{
	/// <summary>
	/// A Tileset is a tileset. It's comprised of references to loaded MAP/RMP
	/// data as well as required terrain-data loaded from PCK/TAB/MCD files.
	/// However, a Tileset is used only for loading groups- and terrains-
	/// configuration from MapConfig; the data is then sorted by the
	/// TilesetManager and is stored as Descriptor's and Terrain's for use in
	/// the viewers/editors.
	/// NOTE: I'm just working as best I can with a big wad of spaghetti.
	/// </summary>
	internal sealed class Tileset
	{
		#region Fields
		internal string Type
		{ get; private set; }

		internal string Group
		{ get; private set; }

		internal string Category
		{ get; private set; }

		internal List<string> Terrains
		{ get; private set; }

		internal string BasePath
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="gruop"></param>
		/// <param name="category"></param>
		/// <param name="terrains"></param>
		/// <param name="basepath"></param>
		internal Tileset(
				string type,
				string gruop, // sic.
				string category,
				List<string> terrains,
				string basepath)
		{
			Type     = type;
			Group    = gruop;
			Category = category;
			Terrains = terrains;
			BasePath = basepath;
		}
		#endregion
	}
}
