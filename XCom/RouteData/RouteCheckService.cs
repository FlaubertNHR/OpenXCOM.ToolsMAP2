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
		/// Caches <c>MainViewOptionables.Base1_z</c> for use here.
		/// </summary>
		private static bool _base1_z;

		/// <summary>
		/// Caches <c>MainViewOptionables.Base1_z</c> for use here.
		/// </summary>
		internal static bool Base1_z
		{ get { return _base1_z; } }

		/// <summary>
		/// Sets <c><see cref="Base1_xy"/></c> and <c><see cref="Base1_z"/></c>.
		/// </summary>
		/// <param name="base1_xy"><c>MainViewOptionables.Base1_xy</c></param>
		/// <param name="base1_z"><c>MainViewOptionables.Base1_z</c></param>
		/// <remarks>Used when printing the positions of out-of-bounds
		/// <c><see cref="RouteNode">RouteNodes</see></c>.</remarks>
		public static void SetBase1(bool base1_xy, bool base1_z)
		{
			_base1_xy = base1_xy;
			_base1_z  = base1_z;
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
										"Good stuff Magister Ludi",
										"There are no Out of Bounds nodes detected."))
				{
					ib.ShowDialog();
				}
			}
			return DialogResult.No;
		}

		/// <summary>
		/// Checks all <c><see cref="RouteNode">RouteNodes</see></c> in a
		/// specified <c><see cref="MapFile"/></c> and fills
		/// <c><see cref="Invalids"/></c> with any <c>RouteNodes</c> that are
		/// outside the x/y/z bounds of the <c>MapFile</c>.
		/// <param name="file">a <c>MapFile</c></param>
		/// </summary>
		private static void TallyInvalids(MapFile file)
		{
			Invalids.Clear();

			foreach (RouteNode node in file.Routes)
			{
				if (   node.Col < 0 || node.Col >= file.Cols
					|| node.Row < 0 || node.Row >= file.Rows
					|| node.Lev < 0 || node.Lev >= file.Levs)
				{
					Invalids.Add(node);
				}
			}
		}

		/// <summary>
		/// Checks if a given <c><see cref="RouteNode"/></c> is outside the
		/// x/y/z bounds of a specified <c><see cref="MapFile"/></c>.
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

				int[] pads = GetPads(file);

				string text = String.Empty;
				int val;
				foreach (RouteNode node in Invalids)
				{
					text += "id ";
					text += node.Id.ToString().PadLeft(pads[0]) + " :  c ";

					val = (int)node.Col;
					if (Base1_xy) ++val;
					text += val.ToString().PadLeft(pads[1]) + "  r ";

					val = (int)node.Row;
					if (Base1_z) ++val;
					text += val.ToString().PadLeft(pads[2]) + "  L ";

					val = file.Levs - node.Lev - 1;
					if (val < -127) val += 256; // cf. MapFile.MapResize()
					if (Base1_z) ++val;
					text += val.ToString().PadLeft(pads[3]) + Environment.NewLine;
				}

				rci.SetTexts(label, text);
				return rci.ShowDialog();
			}
		}

		/// <summary>
		/// Gets an array of <c>PadLeft()</c> values for
		/// <c><see cref="dialog_Invalids()">dialog_Invalids()</see></c>.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		private static int[] GetPads(MapFile file)
		{
			int[] pads = {0,0,0,0};

			int pad, val, pad0 = 3, pad1 = 3, pad2 = 3, pad3 = 4;
			foreach (RouteNode node in Invalids)
			{
				pad = 2; val = node.Id;
				while ((val /= 10) != 0) --pad;
				if (pad < pad0) pad0 = pad;

				pad += node.Id.ToString().Length;
				if (pads[0] < pad) pads[0] = pad;


				pad = 2; val = (int)node.Col;
				if (Base1_xy) ++val;
				while ((val /= 10) != 0) --pad;
				if (pad < pad1) pad1 = pad;

				pad += node.Col.ToString().Length;
				if (pads[1] < pad) pads[1] = pad;


				pad = 2; val = (int)node.Row;
				if (Base1_xy) ++val;
				while ((val /= 10) != 0) --pad;
				if (pad < pad2) pad2 = pad;

				pad += node.Row.ToString().Length;
				if (pads[2] < pad) pads[2] = pad;


				pad = 3; val = file.Levs - node.Lev - 1; // if node-level goes out of bounds 'val' can be less than 0 here ->
				if (val < -127) val += 256; // cf. MapFile.MapResize()
				if (Base1_z) ++val;

				bool n = val < 0;
				int val0 = Math.Abs(val);
				while ((val0 /= 10) != 0) --pad;
				if (n) --pad;
				if (pad < pad3) pad3 = pad;

				pad += val.ToString().Length;
				if (pads[3] < pad) pads[3] = pad;
			}

			pads[0] -= pad0; pads[1] -= pad1;
			pads[2] -= pad2; pads[3] -= pad3;

			return pads;
		}
		#endregion Methods (static)
	}
}
