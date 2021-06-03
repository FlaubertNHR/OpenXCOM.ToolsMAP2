using System;
using System.Drawing;
using System.Windows.Forms;

using MapView.Forms.Observers;

using XCom;


namespace MapView
{
	internal sealed partial class McdInfoF
		:
			Form
	{
		#region Fields (static)
		private const string TITLE = "MCD Info";
		#endregion Fields (static)


		#region Fields
		TileView _tileView;
		#endregion Fields


		#region Properties (override)
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000; // enable 'WS_EX_COMPOSITED'
				return cp;
			}
		}
		#endregion Properties (override)


		#region cTor
		/// <summary>
		/// cTor. Instantiates an MCD-info screen.
		/// </summary>
		/// <param name="tileView"></param>
		internal McdInfoF(TileView tileView)
		{
			InitializeComponent();

			_tileView = tileView;

			rtbInfo.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
			rtbInfo.WordWrap = false;
			rtbInfo.ReadOnly = true;
		}
		#endregion cTor


		#region Events
		/// <summary>
		/// Stops arbitrary beeps when trying to type in a readonly RichTextBox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyDown_rtb(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Up: // these keys are allowed w/ or w/out modifiers ->
				case Keys.Down:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Home:
				case Keys.End:
				case Keys.Left:
				case Keys.Right:
					return;

				default: // these are not ->
					e.SuppressKeyPress = true;
					break;
			}
		}

		/// <summary>
		/// Closes the screen on [Esc] or [i] keyup event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyUp_rtb(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
				case Keys.I:
					Close();
					break;
			}
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Updates the displayed data whenever the selected tile changes.
		/// </summary>
		internal void UpdateData()
		{
			rtbInfo.Text = String.Empty;

			Tilepart part; McdRecord record;
			if ((part = _tileView.SelectedTilepart) != null
				&& (record = part.Record) != null)
			{
				Text = TITLE + " - " + _tileView.GetTerrainLabel() + "  terId " + part.TerId;

				rtbInfo.SelectionColor = Color.Black;

				// TODO: StringBuilder ->

				rtbInfo.AppendText(record.stSprites);
				rtbInfo.AppendText(record.stLoFTs);
				rtbInfo.AppendText(record.stScanG);

//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[22]));
//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[23]));
//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[24]));
//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[25]));
//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[26]));
//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[27]));
//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[28]));
//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[29]));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"ufo door:",
											record.SlidingDoor));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"stop LOS:",
											record.StopLOS));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"no floor:",
											record.NotFloored));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"bigwall:",
											record.BigWall));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"gravlift:",
											record.GravLift));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"standard door:",
											record.HingedDoor));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"blocks fire:",
											record.BlockFire));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"blocks smoke:",
											record.BlockSmoke));

				// LeftRightHalf
//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[38]));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"tu walk:",
											record.TU_Walk));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"tu slide:",
											record.TU_Slide));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"tu fly:",
											record.TU_Fly));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"armor:",
											record.Armor));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"explosive block:",
											record.HE_Block));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"anti-flammability:",
											record.FireResist));

				rtbInfo.SelectionColor = Color.Firebrick;
				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"death tile:",
											record.DieTile));

				rtbInfo.SelectionColor = Color.Firebrick;
				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"alternate tile:",
											record.Alt_MCD));

//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[47]));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"unit y-offset:",
											record.StandOffset));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"tile y-offset:",
											record.TileOffset));

//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[50]));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"block light[0-10]:",
											record.LightBlock));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"footstep sound:",
											record.Footstep));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1} - {2}" + Environment.NewLine,
											"part type:",
											(sbyte)record.PartType,
											Enum.GetName(typeof(PartType), record.PartType)));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1} - {2}" + Environment.NewLine,
											"explosive type:",
											record.HE_Type,
											(record.HE_Type == 0) ? "HE" : (record.HE_Type == 1) ? "smoke" : "unknown"));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"HE Strength:",
											record.HE_Strength));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"smoke block:",
											record.SmokeBlockage));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"fuel:",
											record.Fuel));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"light:",
											record.LightSource));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1} - {2}" + Environment.NewLine,
											"special property:",
											(byte)record.Special,
											Enum.GetName(typeof(SpecialType), record.Special)));

				rtbInfo.AppendText(string.Format(
											"{0,-20}{1}" + Environment.NewLine,
											"base object:",
											record.BaseObject));

//				rtb.AppendText(string.Format("Unknown data: {0}" + Environment.NewLine, info[61]));


				rtbInfo.AppendText(Environment.NewLine);
				rtbInfo.SelectionColor = Color.DarkGray;
				rtbInfo.AppendText("byte data:" + Environment.NewLine);
				rtbInfo.SelectionColor = Color.DarkGray;
				rtbInfo.AppendText(record.ByteTable + Environment.NewLine);


				rtbInfo.SelectionStart  =
				rtbInfo.SelectionLength = 0;
			}
			else
				Text = TITLE;
		}
		#endregion Methods
	}
}
