using System;
using System.Collections.Generic;


namespace RulesetConverter
{
	#region Structs
	/// <summary>
	/// The <c>Tileset</c> struct is the basic stuff of a tileset.
	/// </summary>
	struct Tileset
	{
		internal string Label
		{ get; private set; }

		internal string Group
		{ get; private set; }

		internal string Category
		{ get; private set; }

		internal List<string> Terrains
		{ get; private set; }


		internal Tileset(
				string label,
				string @group,
				string category,
				List<string> terrains)
			:
				this()
		{
			Label    = label;
			Group    = @group;
			Category = category;
			Terrains = terrains;
		}
	}
	#endregion Structs
}
