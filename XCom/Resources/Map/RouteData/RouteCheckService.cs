using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace XCom
{
	public static class RouteCheckService
	{
		#region Fields (static)
		private static MapFile _file;

		private static int _count;
		#endregion Fields (static)


		#region Properties (static)
		private static List<RouteNode> _invalids = new List<RouteNode>();
		public static List<RouteNode> Invalids
		{
			get { return _invalids; }
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Opens a dialog to delete an invalid link-destination node.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="node">the node to delete</param>
		/// <returns>true if user chooses to delete out-of-bounds node</returns>
		public static DialogResult ShowInvalid(MapFile file, RouteNode node)
		{
			using (var f = new RouteCheckInfobox())
			{
				string label = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"Destination node is outside the Map's bounds."
											+ "{0}{0}Do you want it deleted?",
										Environment.NewLine);
				string text = "id " + node.Index + " : " + node.GetLocationString(file.MapSize.Levs);
				f.SetTexts(label, text);

				return f.ShowDialog();
			}
		}


		/// <summary>
		/// Checks for and if found gives user a choice to delete nodes that are
		/// outside of a Map's x/y/z bounds.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="ludi">true if user-invoked</param>
		/// <returns>DialogResult.Yes if user opts to clear invalid nodes</returns>
		public static DialogResult CheckNodeBounds(MapFile file, bool ludi = false)
		{
			Invalids.Clear();

			if ((_file = file) != null)
			{
				if ((_count = DeterInvalidNodes()) != 0)
					return ShowInvalids();

				if (ludi)
					MessageBox.Show(
								"There are no Out of Bounds nodes detected.",
								" Good stuff, Magister Ludi",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information,
								MessageBoxDefaultButton.Button1,
								0);
			}
			return DialogResult.No;
		}

		/// <summary>
		/// Fills the list with any invalid nodes.
		/// </summary>
		/// <returns>count of invalid nodes</returns>
		private static int DeterInvalidNodes()
		{
			foreach (RouteNode node in _file.Routes)
			{
				if (RouteNodeCollection.IsNodeOutsideMapBounds(
															node,
															_file.MapSize.Cols,
															_file.MapSize.Rows,
															_file.MapSize.Levs))
				{
					Invalids.Add(node);
				}
			}
			return Invalids.Count;
		}

		/// <summary>
		/// Opens a dialog to delete the invalid nodes.
		/// </summary>
		/// <returns>DialogResult.Yes if user opts to clear invalid nodes</returns>
		private static DialogResult ShowInvalids()
		{
			using (var f = new RouteCheckInfobox())
			{
				bool singular = (_count == 1);
				string label = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"There {0} " + _count + " route-node{1} outside"
											+ " the bounds of the Map.{3}{3}Do you want {2} deleted?",
										singular ? "is" : "are",
										singular ? ""   : "s",
										singular ? "it" : "them",
										Environment.NewLine);

				string text = String.Empty;
				int total = _file.Routes.Count;
				byte loc;
				foreach (var node in Invalids)
				{
					text += "id ";

					if (total > 99)
					{
						if (node.Index < 10)
							text += "  ";
						else if (node.Index < 100)
							text += " ";
					}
					else if (total > 9)
					{
						if (node.Index < 10)
							text += " ";
					}
					text += node.Index + " :  c ";

					loc = (byte)(node.Col + 1);
					if (loc < 10)
						text += " ";

					text += loc + "  r ";

					loc = (byte)(node.Row + 1);
					if (loc < 10)
						text += " ";

					text += loc + "  L ";

					loc = (byte)(_file.MapSize.Levs - node.Lev);
					if (loc < 10)
						text += "  ";
					else if (loc < 100)
						text += " ";

					text += loc + Environment.NewLine;
				}

				f.SetTexts(label, text);

				return f.ShowDialog();
			}
		}
		#endregion Methods (static)
	}
}
