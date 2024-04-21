using System;
using System.Text;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;
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


		#region Properties (override)
		/// <summary>
		/// Prevents flicker by setting the 'WS_EX_COMPOSITED' flag.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}
		#endregion Properties (override)


		#region cTor
		/// <summary>
		/// cTor. Instantiates an MCD-info screen.
		/// </summary>
		internal McdInfoF()
		{
			InitializeComponent();

			rtbInfo.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
			rtbInfo.WordWrap = false;
			rtbInfo.ReadOnly = true;

			UpdateData();

			if (!RegistryInfo.RegisterProperties(this))
			{
				Left = 200;
				Top  = 100;
			}
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Cancels close and hides this dialog unless MapView is serious about
		/// quitting.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				if (!MainViewF.Quit)
				{
					ObserverManager.TileView.Control.OnMcdInfoClick(null, EventArgs.Empty);
					e.Cancel = true;
				}
				else
					RegistryInfo.UpdateRegistry(this);
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


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
			TileView tileView = ObserverManager.TileView.Control;

			Tilepart part; McdRecord record;
			if ((part = tileView.SelectedTilepart) != null
				&& (record = part.Record) != null)
			{
				Text = TITLE + " - " + tileView.GetTerrainLabel() + "  terId " + part.Id;

//				rtbInfo.SelectionColor = Color.Black;

				var sb = new StringBuilder();

				sb.AppendLine(record.stSprites);
				sb.AppendLine(record.stLoFTs);
				sb.AppendLine(record.stScanG);

//				sb.AppendLine(string.Format("Unknown data: {0}", info[22]));
//				sb.AppendLine(string.Format("Unknown data: {0}", info[23]));
//				sb.AppendLine(string.Format("Unknown data: {0}", info[24]));
//				sb.AppendLine(string.Format("Unknown data: {0}", info[25]));
//				sb.AppendLine(string.Format("Unknown data: {0}", info[26]));
//				sb.AppendLine(string.Format("Unknown data: {0}", info[27]));
//				sb.AppendLine(string.Format("Unknown data: {0}", info[28]));
//				sb.AppendLine(string.Format("Unknown data: {0}", info[29]));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"ufo door:",
										record.SlidingDoor));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"stop LOS:",
										record.StopLOS));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"no floor:",
										record.NotFloored));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"bigwall:",
										record.BigWall));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"gravlift:",
										record.GravLift));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"standard door:",
										record.HingedDoor));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"blocks fire:",
										record.BlockFire));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"blocks smoke:",
										record.BlockSmoke));

				// LeftRightHalf
//				sb.AppendLine(string.Format("Unknown data: {0}", info[38]));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"tu walk:",
										record.TU_Walk));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"tu slide:",
										record.TU_Slide));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"tu fly:",
										record.TU_Fly));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"armor:",
										record.Armor));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"explosive block:",
										record.HE_Block));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"anti-flammability:",
										record.FireResist));

//				rtbInfo.SelectionColor = Color.Firebrick;
				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"death tile:",
										record.DieTile));

//				rtbInfo.SelectionColor = Color.Firebrick;
				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"alternate tile:",
										record.Alt_MCD));

//				sb.AppendLine(string.Format("Unknown data: {0}", info[47]));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"unit y-offset:",
										record.TerrainOffset));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"sprite y-offset:",
										record.SpriteOffset));

//				sb.AppendLine(string.Format("Unknown data: {0}", info[50]));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"block light[0-10]:",
										record.LightBlock));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"footstep sound:",
										record.Footsound));

				sb.AppendLine(string.Format(
										"{0,-20}{1} - {2}",
										"part type:",
										(sbyte)record.PartType,
										Enum.GetName(typeof(PartType), record.PartType)));

				sb.AppendLine(string.Format(
										"{0,-20}{1} - {2}",
										"explosive type:",
										record.HE_Type,
										(record.HE_Type == 0) ? "HE" : (record.HE_Type == 1) ? "smoke" : "unknown"));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"HE Strength:",
										record.HE_Strength));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"smoke block:",
										record.SmokeBlockage));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"fuel:",
										record.Fuel));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"light:",
										record.LightSource));

				sb.AppendLine(string.Format(
										"{0,-20}{1} - {2}",
										"special property:",
										(byte)record.Special,
										Enum.GetName(typeof(SpecialType), record.Special)));

				sb.AppendLine(string.Format(
										"{0,-20}{1}",
										"base object:",
										record.BaseObject));

//				sb.AppendLine(string.Format("Unknown data: {0}", info[61]));


				sb.AppendLine();
//				rtbInfo.SelectionColor = Color.DarkGray;
				sb.AppendLine("byte data:");
//				rtbInfo.SelectionColor = Color.DarkGray;
				sb.AppendLine(record.ByteTable);

				rtbInfo.Text = sb.ToString();

				rtbInfo.SelectionStart  =
				rtbInfo.SelectionLength = 0;
			}
			else
			{
				Text = TITLE;
				rtbInfo.Text = String.Empty;
			}
		}
		#endregion Methods
	}
}
