using System;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	public class TileGroupBase
	{
		#region Fields & Properties
		public string Label
		{ get; private set; }

//		/// <summary>
//		/// Descriptors is a dictionary of descriptor-labels (.MAP/.RMP
//		/// filenames w/out extension) mapped to Descriptors.
//		/// </summary>
		private readonly Dictionary<string, Descriptor> _descriptors
				   = new Dictionary<string, Descriptor>();
//		internal protected Dictionary<string, Descriptor> Descriptors
//		{
//			get { return _descriptors; }
//		}

		/// <summary>
		/// Categories is a dictionary of category-labels mapped to a
		/// subdictionary of descriptor-labels (.MAP/.RMP filenames w/out
		/// extension) mapped to the Descriptors themselves.
		/// </summary>
		private readonly Dictionary<string, Dictionary<string, Descriptor>> _categories
				   = new Dictionary<string, Dictionary<string, Descriptor>>();
		public Dictionary<string, Dictionary<string, Descriptor>> Categories
		{
			get { return _categories; }
		}

		/// <summary> old function -> TODO: REMOVE THIS AFTER PATHSEDITOR GETS PLUCKED.
		/// Used by PathsEditor to add/delete treenodes.
		/// </summary>
		public Descriptor this[string label]
		{
			get { return _descriptors[label]; }
			set
			{
				if (!_descriptors.ContainsKey(label)) // TODO: this needs to work through Categories *only*
					_descriptors[label] = value;
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="labelGroup"></param>
		internal protected TileGroupBase(string labelGroup)
		{
			Label = labelGroup;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Adds a category. Called by XCMainWindow.OnAddCategoryClick()
		/// NOTE: Check if the category already exists first.
		/// </summary>
		/// <param name="labelCategory">the label of the category to add</param>
		public void AddCategory(string labelCategory)
		{
			Categories[labelCategory] = new Dictionary<string, Descriptor>();
		}

		/// <summary>
		/// Deletes a category. Called by XCMainWindow.OnDeleteCategoryClick()
		/// </summary>
		/// <param name="labelCategory">the label of the category to delete</param>
		public void DeleteCategory(string labelCategory)
		{
			Categories.Remove(labelCategory);
		}

		/// <summary>
		/// Creates a new category and transfers ownership of all Descriptors
		/// from their previous Category to the specified new Category. Called
		/// by XCMainWindow.OnEditCategoryClick()
		/// NOTE: Check if the category already exists first.
		/// </summary>
		/// <param name="labelCategory">the new label for the category</param>
		/// <param name="labelCategoryPre">the old label of the category</param>
		public void EditCategory(string labelCategory, string labelCategoryPre)
		{
			AddCategory(labelCategory);

			foreach (var descriptor in Categories[labelCategoryPre].Values)
			{
				Categories[labelCategory][descriptor.Label] = descriptor;
			}
			DeleteCategory(labelCategoryPre); // hopefully this won't wipe all Values after transferring ownership.
		}

		/// <summary>
		/// Deletes a tileset. Called by XCMainWindow.OnDeleteTilesetClick()
		/// NOTE: Check that tileset and category exist first.
		/// </summary>
		/// <param name="labelTileset">the label of the tileset to delete</param>
		/// <param name="labelCategory">the label of the category of the tileset</param>
		public void DeleteTileset(string labelTileset, string labelCategory)
		{
			Categories[labelCategory].Remove(labelTileset);
		}

		public void AddTileset(Descriptor descriptor, string labelCategory)
		{
			Categories[labelCategory][descriptor.Label] = descriptor;
		}
		#endregion
	}
}
