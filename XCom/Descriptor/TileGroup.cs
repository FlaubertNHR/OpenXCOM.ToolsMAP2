using System;
using System.Collections.Generic;


namespace XCom
{
	/// <summary>
	/// The class for a <c>TileGroup</c> contains its
	/// <c><see cref="Categories"/></c> and through those <c>Categories</c> all
	/// of its <c><see cref="Descriptor">Descriptors</see></c> aka tilesets.
	/// </summary>
	public sealed class TileGroup
	{
		#region Properties
		public string Label
		{ get; private set; }

		public GameType GroupType
		{ get; private set; }


		private readonly Dictionary<string, Dictionary<string, Descriptor>> _categories
				   = new Dictionary<string, Dictionary<string, Descriptor>>();
		/// <summary>
		/// <c>Categories</c> is a dictionary of category-labels mapped to a
		/// subdictionary of descriptor-labels - .MAP/.RMP filenames w/out
		/// extension - mapped to the
		/// <c><see cref="Descriptor">Descriptors</see></c> themselves.
		/// </summary>
		public Dictionary<string, Dictionary<string, Descriptor>> Categories
		{
			get { return _categories; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="labelGroup"></param>
		/// <remarks>If the prefix "tftd" (case insensitive) is not found at the
		/// beginning of this <c>TileGroup's</c> label then default
		/// <c><see cref="GroupType"/></c> to
		/// <c><see cref="GameType.Ufo">GameType.Ufo</see></c>.</remarks>
		internal TileGroup(string labelGroup)
		{
			Label = labelGroup;

			if (Label.StartsWith("tftd", StringComparison.OrdinalIgnoreCase))
			{
				GroupType = GameType.Tftd;
			}
			else //if (labelGroup.StartsWith("ufo", StringComparison.OrdinalIgnoreCase))
			{
				GroupType = GameType.Ufo;
			}
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Adds a <c><see cref="Categories">Category</see></c>. Called by
		/// <list type="bullet">
		/// <item><c><see cref="EditCategory()">EditCategory()</see></c></item>
		/// <item><c><see cref="TileGroupManager.LoadTileGroups()">TileGroupManager.LoadTileGroups()</see></c></item>
		/// <item><c><see cref="TileGroupManager.EditTileGroup()">TileGroupManager.EditTileGroup()</see></c></item>
		/// <item><c>MainViewF.OnAddCategoryClick()</c></item>
		/// </list>
		/// </summary>
		/// <param name="labelCategory">the label of the category to add</param>
		/// <remarks>Check if the category already exists first.</remarks>
		public void AddCategory(string labelCategory)
		{
			Categories[labelCategory] = new Dictionary<string, Descriptor>();
		}

		/// <summary>
		/// Deletes a <c><see cref="Categories">Category</see></c>. Called by
		/// <list type="bullet">
		/// <item><c><see cref="EditCategory()">EditCategory()</see></c></item>
		/// <item><c>MainViewF.OnDeleteCategoryClick()</c></item>
		/// </list>
		/// </summary>
		/// <param name="labelCategory">the label of the <c>Category</c> to
		/// delete</param>
		public void DeleteCategory(string labelCategory)
		{
			Categories.Remove(labelCategory);
		}

		/// <summary>
		/// Creates a new <c><see cref="Categories">Category</see></c> and
		/// transfers ownership of all
		/// <c><see cref="Descriptor">Descriptors</see></c> from their previous
		/// <c>Category</c> to the specified new <c>Category</c>. Called by
		/// <list type="bullet">
		/// <item><c>MainViewF.OnEditCategoryClick()</c></item>
		/// </list>
		/// </summary>
		/// <param name="labelCategory">the new label for the <c>Category</c></param>
		/// <param name="labelCategory0">the old label of the <c>Category</c></param>
		/// <remarks>Check if the <c>Category</c> already exists first.</remarks>
		public void EditCategory(string labelCategory, string labelCategory0)
		{
			AddCategory(labelCategory);

			foreach (var descriptor in Categories[labelCategory0].Values)
				Categories[labelCategory][descriptor.Label] = descriptor;

			DeleteCategory(labelCategory0);
		}

		/// <summary>
		/// Adds a <c><see cref="Descriptor"/></c>. Called by
		/// <list type="bullet">
		/// <item><c><see cref="TileGroupManager.LoadTileGroups()">TileGroupManager.LoadTileGroups()</see></c></item>
		/// <item><c>TilesetEditor.OnAcceptClick()</c></item>
		/// <item><c>TilesetEditor.GlobalChangeLabels()</c></item>
		/// </list>
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="labelCategory"></param>
		/// <remarks>Check that the <c>Descriptor</c> does *not* exist and
		/// category does exist first.</remarks>
		public void AddTileset(Descriptor descriptor, string labelCategory)
		{
			Categories[labelCategory][descriptor.Label] = descriptor;
		}

		/// <summary>
		/// Deletes a <c><see cref="Descriptor"/></c>. Called by
		/// <list type="bullet">
		/// <item><c>MainViewF.OnDeleteTilesetClick()</c></item>
		/// <item><c>TilesetEditor.OnAcceptClick()</c></item>
		/// <item><c>TilesetEditor.GlobalChangeLabels()</c></item>
		/// </list>
		/// </summary>
		/// <param name="labelTileset">the label of the tileset to delete</param>
		/// <param name="labelCategory">the label of the
		/// <c><see cref="Categories">Category</see></c> of the tileset</param>
		/// <remarks>Check that <c>Category</c> and perhaps tileset exist
		/// first.</remarks>
		public void DeleteTileset(string labelTileset, string labelCategory)
		{
			Categories[labelCategory].Remove(labelTileset);
		}
		#endregion Methods


		#region Methods (override)
		/// <summary>
		/// Overrides <c>Object.ToString()</c>.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}
		#endregion Methods (override)
	}
}
