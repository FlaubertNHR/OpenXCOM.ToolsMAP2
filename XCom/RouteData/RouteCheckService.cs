using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DSShared;


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

		/// <summary>
		/// Holds the MainView option 'Base1_xy' for use here.
		/// </summary>
		private static bool _base1_xy;

		internal static bool Base1_xy
		{ get { return _base1_xy; } }

		public static void SetBase1_xy(bool base1)
		{
			_base1_xy = base1;
		}

		/// <summary>
		/// Holds the MainView option 'Base1_z' for use here.
		/// </summary>
		private static bool _base1_z;

		internal static bool Base1_z
		{ get { return _base1_z; } }

		public static void SetBase1_z(bool base1)
		{
			_base1_z = base1;
		}
		#endregion Properties (static)


		#region Methods (static)
		/// <summary>
		/// Opens a dialog to delete an invalid link-destination node.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="node">the node to delete</param>
		/// <returns>true if user chooses to delete out-of-bounds node</returns>
		public static DialogResult dialog_InvalidNode(MapFile file, RouteNode node)
		{
			using (var f = new RouteCheckInfobox())
			{
				string label = "Destination node is outside the Map's bounds."
							 + Environment.NewLine + Environment.NewLine
							 + "Do you want it deleted?";
				string text = "id " + node.Id + " : " + node.GetLocationString(file.Levs);
				f.SetTexts(label, text);

				return f.ShowDialog();
			}
		}


		/// <summary>
		/// Checks for and if found gives user a choice to delete nodes that are
		/// outside of a Map's x/y/z bounds.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="userinvoked">true if user-invoked</param>
		/// <returns>DialogResult.Yes if user opts to clear invalid nodes</returns>
		public static DialogResult CheckNodeBounds(MapFile file, bool userinvoked = false)
		{
			if ((_file = file) != null)
			{
				if ((_count = ListInvalidNodes()) != 0)
					return dialog_InvalidNodes();

				if (userinvoked)
				{
					using (var f = new Infobox(
											"Good stuff, Magister Ludi",
											"There are no Out of Bounds nodes detected."))
					{
						f.ShowDialog();
					}
				}
			}
			return DialogResult.No;
		}

		/// <summary>
		/// Fills the list with any invalid nodes.
		/// </summary>
		/// <returns>count of invalid nodes</returns>
		private static int ListInvalidNodes()
		{
			Invalids.Clear();

			foreach (RouteNode node in _file.Routes)
			{
				if (RouteNodes.OutsideMapBounds(
											node,
											_file.Cols,
											_file.Rows,
											_file.Levs))
				{
					Invalids.Add(node);
				}
			}
			return Invalids.Count;
		}

		/// <summary>
		/// Opens a dialog to delete the invalid nodes.
		/// @note Always update 'Base1_xy' and 'Base1_z' with user's current
		/// MainView options before calling this funct.
		/// </summary>
		/// <returns>DialogResult.Yes if user opts to clear invalid nodes</returns>
		private static DialogResult dialog_InvalidNodes()
		{
			using (var f = new RouteCheckInfobox())
			{
				bool singular = (_count == 1);
				string label = "There " + (singular ? "is " : "are ") + _count
					+ " route-node" + (singular ? "" : "s")
					+ " outside the bounds of the Map."
					+ Environment.NewLine + Environment.NewLine
					+ "Do you want " + (singular ? "it" : "them") + " deleted?";

				string text = String.Empty;
				int total = _file.Routes.Nodes.Count;
				byte loc;
				foreach (var node in Invalids)
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

					loc = (byte)(_file.Levs - node.Lev);
					if (!Base1_z) --loc;

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
