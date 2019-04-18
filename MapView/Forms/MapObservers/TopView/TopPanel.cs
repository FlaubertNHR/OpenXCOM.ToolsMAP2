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
			TopPanelParent
	{
		#region Fields & Properties
		private ColorTools _toolWest;
		private ColorTools _toolNorth;
		private ColorTools _toolContent;

		internal ToolStripMenuItem Ground
		{ get; set; }

		internal ToolStripMenuItem North
		{ get; set; }

		internal ToolStripMenuItem West
		{ get; set; }

		internal ToolStripMenuItem Content
		{ get; set; }

		internal QuadrantPanel QuadrantsPanel
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// TopPanel cTor. Is NOT a panel.
		/// </summary>
		internal TopPanel()
		{
			MainViewUnderlay.Instance.MainViewOverlay.MouseDragEvent += PathSelectedLozenge;
		}
		#endregion


		#region Methods
		internal void DrawTileBlobs(
				MapTileBase tile,
				Graphics graphics,
				int x, int y)
		{
			var mapTile = (XCMapTile)tile;

			_toolWest    = _toolWest    ?? new ColorTools(TopPens[TopView.WestColor]);
			_toolNorth   = _toolNorth   ?? new ColorTools(TopPens[TopView.NorthColor]);
			_toolContent = _toolContent ?? new ColorTools(TopBrushes[TopView.ContentColor], _toolNorth.Pen.Width);

			if (Ground.Checked && mapTile.Floor != null)
				BlobService.DrawFloor(
									graphics,
									TopBrushes[TopView.FloorColor],
									x, y);

			if (Content.Checked && mapTile.Content != null)
				BlobService.DrawContent(
									graphics,
									_toolContent,
									x, y,
									mapTile.Content);

			if (West.Checked && mapTile.West != null)
				BlobService.DrawContent(
									graphics,
									_toolWest,
									x, y,
									mapTile.West);

			if (North.Checked && mapTile.North != null)
				BlobService.DrawContent(
									graphics,
									_toolNorth,
									x, y,
									mapTile.North);
		}
		#endregion
	}
}
