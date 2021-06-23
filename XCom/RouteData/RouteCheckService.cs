using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DSShared;


namespace XCom
{
	/// <summary>
	/// A static service that checks for out-of-bounds
	/// <c><see cref="RouteNode">RouteNodes</see></c>.
	/// </summary>
	public static class RouteCheckService
	{
		#region Properties (static)
		private static IList<RouteNode> _invalids = new List<RouteNode>();
		/// <summary>
		/// A list of invalid <c><see cref="RouteNode">RouteNodes</see></c>.
		/// </summary>
		public static IList<RouteNode> Invalids
		{
			get { return _invalids; }
		}

		/// <summary>
		/// Caches <c>MainViewOptionables.Base1_xy</c> for use here.
		/// </summary>
		private static bool _base1_xy;

		/// <summary>
		/// Caches <c>MainViewOptionables.Base1_xy</c> for use here.
		/// </summary>
		internal static bool Base1_xy
		{ get { return _base1_xy; } }

		/// <summary>
		/// Sets <c><see cref="Base1_xy"/></c>.
		/// </summary>
		/// <param name="base1"><c>MainViewOptionables.Base1_xy</c></param>
		public static void SetBase1_xy(bool base1)
		{
			_base1_xy = base1;
		}

		/// <summary>
		/// Caches <c>MainViewOptionables.Base1_z</c> for use here.
		/// </summary>
		private static bool _base1_z;

		/// <summary>
		/// Caches <c>MainViewOptionables.Base1_z</c> for use here.
		/// </summary>
		internal static bool Base1_z
		{ get { return _base1_z; } }

		/// <summary>
		/// Sets <c><see cref="Base1_z"/></c>.
		/// </summary>
		/// <param name="base1"><c>MainViewOptionables.Base1_z</c></param>
		public static void SetBase1_z(bool base1)
		{
			_base1_z = base1;
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Opens a dialog to delete an invalid link-destination
		/// <c><see cref="RouteNode"/></c>.
		/// </summary>
		/// <param name="file">a <c><see cref="MapFile"/></c></param>
		/// <param name="node">the <c>RouteNode</c> to delete</param>
		/// <returns>true if user chooses to delete the out-of-bounds
		/// <c>RouteNode</c></returns>
		public static DialogResult dialog_InvalidDestination(MapFile file, RouteNode node)
		{
			using (var rci = new RouteCheckInfobox())
			{
				string label = "Destination node is outside the Map's bounds."
							 + Environment.NewLine + Environment.NewLine
							 + "Do you want it deleted?";
				string text = "id " + node.Id + " : " + node.GetLocationString(file.Levs);
				rci.SetTexts(label, text);

				return rci.ShowDialog();
			}
		}


		/// <summary>
		/// Checks for and if found gives user a choice to delete
		/// <c><see cref="RouteNode">RouteNodes</see></c> that are outside the
		/// x/y/z bounds of a <c><see cref="MapFile"/></c>.
		/// </summary>
		/// <param name="file">a <c>MapFile</c></param>
		/// <param name="showsuccess"><c>true</c> if user-invoked</param>
		/// <returns><c>DialogResult.Yes</c> if user opts to delete invalid
		/// <c>RouteNodes</c></returns>
		public static DialogResult CheckNodeBounds(MapFile file, bool showsuccess = false)
		{
			if (file != null)
			{
				TallyInvalids(file);

				if (Invalids.Count != 0)
					return dialog_Invalids(file);

				if (showsuccess)
				using (var ib = new Infobox(
										"Good stuff, Magister Ludi",
										"There are no Out of Bounds nodes detected."))
				{
					ib.ShowDialog();
				}
			}
			return DialogResult.No;
		}

		/// <summary>
		/// Fills <c><see cref="Invalids"/></c> with any invalid
		/// <c><see cref="RouteNode">RouteNodes</see></c> in a given
		/// <c><see cref="MapFile"/></c>.
		/// <param name="file">a <c>MapFile</c></param>
		/// </summary>
		private static void TallyInvalids(MapFile file)
		{
			Invalids.Clear();

			int cols = file.Cols;
			int rows = file.Rows;
			int levs = file.Levs;

			foreach (RouteNode node in file.Routes)
			if (   node.Col < 0 || node.Col >= cols
				|| node.Row < 0 || node.Row >= rows
				|| node.Lev < 0 || node.Lev >= levs)
			{
				Invalids.Add(node);
			}
		}

		/// <summary>
		/// Checks if a given <c><see cref="RouteNode"/></c> is outside the
		/// boundaries of a given <c><see cref="MapFile"/></c>.
		/// </summary>
		/// <param name="node">a <c>RouteNode</c></param>
		/// <param name="file">a <c>MapFile</c></param>
		/// <returns></returns>
		public static bool OutsideBounds(
				RouteNode node,
				MapFile file)
		{
			return node.Col < 0 || node.Col >= file.Cols
				|| node.Row < 0 || node.Row >= file.Rows
				|| node.Lev < 0 || node.Lev >= file.Levs;
		}

		/// <summary>
		/// Opens a dialog to delete any invalid
		/// <c><see cref="RouteNode">RouteNodes</see></c> that were found.
		/// </summary>
		/// <param name="file">a <c><see cref="MapFile"/></c></param>
		/// <returns><c>DialogResult.Yes</c> if user opts to clear invalid
		/// <c>RouteNodes</c></returns>
		/// <remarks>Always update <c><see cref="Base1_xy"/></c> and
		/// <c><see cref="Base1_z"/></c> with user's current
		/// <c>MainViewOptionables</c> before calling this funct.</remarks>
		private static DialogResult dialog_Invalids(MapFile file)
		{
			using (var rci = new RouteCheckInfobox())
			{
				bool singular = (Invalids.Count == 1);
				string label = "There " + (singular ? "is " : "are ") + Invalids.Count
					+ " route-node" + (singular ? "" : "s")
					+ " outside the bounds of the Map."
					+ Environment.NewLine + Environment.NewLine
					+ "Do you want " + (singular ? "it" : "them") + " deleted?";

				string text = String.Empty;
				int total = file.Routes.Nodes.Count;
				byte loc;
				foreach (RouteNode node in Invalids)
				{
					text += "id ";

					if (total > 99)
					{
						if (node.Id < 10)
							text += "  ";
						else if (node.Id < 100)
							text += " ";
					}
					else if (total > 9)
					{
						if (node.Id < 10)
							text += " ";
					}
					text += node.Id + " :  c ";

					loc = node.Col;
					if (Base1_xy) ++loc;

					if (loc < 10)
						text += " ";

					text += loc + "  r ";

					loc = node.Row;
					if (Base1_z) ++loc;

					if (loc < 10)
						text += " ";

					text += loc + "  L ";

					loc = (byte)(file.Levs - node.Lev);
					if (!Base1_z) --loc;

					if (loc < 10)
						text += "  ";
					else if (loc < 100)
						text += " ";

					text += loc + Environment.NewLine;
				}

				rci.SetTexts(label, text);

				return rci.ShowDialog();
			}
		}
		#endregion Methods (static)
	}
}
