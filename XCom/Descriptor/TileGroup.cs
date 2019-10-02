using System;
using System.Collections.Generic;


namespace XCom
{
	/// <summary>
	/// The class for a TileGroup contains its Categories and through those
	/// Categories all of its tilesets/Descriptors.
	/// </summary>
	public class TileGroup
	{
		#region Properties
		public string Label
		{ get; private set; }

		public GameType GroupType
		{ get; private set; }


		private readonly Dictionary<string, Dictionary<string, Descriptor>> _categories
				   = new Dictionary<string, Dictionary<string, Descriptor>>();
		/// <summary>
		/// Categories is a dictionary of category-labels mapped to a
		/// subdictionary of descriptor-labels (.MAP/.RMP filenames w/out
		/// extension) mapped to the Descriptors themselves.
		/// </summary>
		public Dictionary<string, Dictionary<string, Descriptor>> Categories
		{
			get { return _categories; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// @note If the prefix "tftd" is not found at the beginning of this
		/// TileGroup's label then default to UFO grouptype.
		/// </summary>
		/// <param name="labelGroup"></param>
		internal protected TileGroup(string labelGroup)
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
		/// Adds a category. Called by MainViewF.OnAddCategoryClick()
		/// NOTE: Check if the category already exists first.
		/// </summary>
		/// <param name="labelCategory">the label of the category to add</param>
		public void AddCategory(string labelCategory)
		{
			Categories[labelCategory] = new Dictionary<string, Descriptor>();
		}

		/// <summary>
		/// Deletes a category. Called by MainViewF.OnDeleteCategoryClick()
		/// </summary>
		/// <param name="labelCategory">the label of the category to delete</param>
		public void DeleteCategory(string labelCategory)
		{
			Categories.Remove(labelCategory);
		}

		/// <summary>
		/// Creates a new category and transfers ownership of all Descriptors
		/// from their previous Category to the specified new Category. Called
		/// by MainViewF.OnEditCategoryClick()
		/// NOTE: Check if the category already exists first.
		/// </summary>
		/// <param name="labelCategory">the new label for the category</param>
		/// <param name="labelCategoryPre">the old label of the category</param>
		public void EditCategory(string labelCategory, string labelCategoryPre)
		{
			AddCategory(labelCategory);

			foreach (var descriptor in Categories[labelCategoryPre].Values)
				Categories[labelCategory][descriptor.Label] = descriptor;

			DeleteCategory(labelCategoryPre); // hopefully this won't wipe all Values after transferring ownership.
		}

		/// <summary>
		/// Adds a tileset-descriptor. Called by
		/// - TilesetEditor.OnAcceptClick()
		/// NOTE: Check that the descriptor does *not* exist and category does
		/// exist first.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="labelCategory"></param>
		public void AddTileset(Descriptor descriptor, string labelCategory)
		{
			Categories[labelCategory][descriptor.Label] = descriptor;
		}

		/// <summary>
		/// Deletes a tileset-descriptor. Called by
		/// - MainViewF.OnDeleteTilesetClick()
		/// - TilesetEditor.OnAcceptClick()
		/// NOTE: Check that category and perhaps tileset exist first.
		/// </summary>
		/// <param name="labelTileset">the label of the tileset to delete</param>
		/// <param name="labelCategory">the label of the category of the tileset</param>
		public void DeleteTileset(string labelTileset, string labelCategory)
		{
			Categories[labelCategory].Remove(labelTileset);
		}
		#endregion Methods


		#region Methods (override)
		/// <summary>
		/// Overrides Object.ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}
		#endregion Methods (override)
	}
}
