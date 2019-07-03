using System;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	/// <summary>
	/// The derived class for TopPanel.
	/// @note This is not a Panel. It is a UserControl inside of a Panel.
	/// </summary>
	internal sealed class TopPanel
		:
			TopPanelParent // MapObserverControl_TopPanel, DoubleBufferedControl, IMapObserver
	{
		#region Fields
		private ColorTool _toolWest;
		private ColorTool _toolNorth;
		private ColorTool _toolContent;
		#endregion Fields


		#region Properties
		internal ToolStripMenuItem Floor
		{ get; set; }

		internal ToolStripMenuItem North
		{ get; set; }

		internal ToolStripMenuItem West
		{ get; set; }

		internal ToolStripMenuItem Content
		{ get; set; }

		internal QuadrantPanel QuadrantPanel
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// TopPanel cTor. Is NOT a panel.
		/// </summary>
		internal TopPanel()
		{
			MainViewOverlay.that.MouseDrag += PathSelectedLozenge;
		}
		#endregion cTor


		#region Methods
		internal void DrawTileBlobs(
				MapTileBase tilebase,
				Graphics graphics,
				int x, int y)
		{
			var tile = tilebase as XCMapTile;

			_toolWest    = _toolWest    ?? new ColorTool(TopPens   [TopView.WestColor]);
			_toolNorth   = _toolNorth   ?? new ColorTool(TopPens   [TopView.NorthColor]);
			_toolContent = _toolContent ?? new ColorTool(TopBrushes[TopView.ContentColor], _toolNorth.Pen.Width);

			if (Floor.Checked && tile.Floor != null)
				BlobService.DrawFloor(
									graphics,
									TopBrushes[TopView.FloorColor],
									x, y);

			if (Content.Checked && tile.Content != null)
				BlobService.DrawContent(
									graphics,
									_toolContent,
									x, y,
									tile.Content);

			if (West.Checked && tile.West != null)
				BlobService.DrawContent(
									graphics,
									_toolWest,
									x, y,
									tile.West);

			if (North.Checked && tile.North != null)
				BlobService.DrawContent(
									graphics,
									_toolNorth,
									x, y,
									tile.North);
		}
		#endregion Methods
	}
}
