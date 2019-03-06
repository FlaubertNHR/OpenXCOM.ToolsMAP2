using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using XCom;


namespace MapView.Forms.McdViewer
{
	internal sealed partial class McdViewerForm
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor. Instantiates an MCD-info screen.
		/// </summary>
		internal McdViewerForm()
		{
			InitializeComponent();

			rtbInfo.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
			rtbInfo.WordWrap = false;
			rtbInfo.ReadOnly = true;
		}
		#endregion


		#region EventCalls
		/// <summary>
		/// Closes the screen on an Escape keydown event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}
		#endregion


		#region Methods
		/// <summary>
		/// Updates the displayed data whenever the selected tile changes.
		/// </summary>
		/// <param name="record"></param>
		internal void UpdateData(McdRecord record)
		{
			rtbInfo.Text = String.Empty;

			if ((bsInfo.DataSource = record) != null)
			{
				rtbInfo.SelectionColor = Color.Black;
				rtbInfo.AppendText(record.Images);
				rtbInfo.AppendText(record.LoftReference);
				rtbInfo.AppendText(record.ScanGReference);

//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[22])); // unsigned char u23;
//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[23])); // unsigned char u24;
//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[24])); // unsigned char u25;
//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[25])); // unsigned char u26;
//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[26])); // unsigned char u27;
//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[27])); // unsigned char u28;
//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[28])); // unsigned char u29;
//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[29])); // unsigned char u30;

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"ufo door:",
											record.UfoDoor));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"stop LOS:",
											record.StopLOS));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"no floor:",
											record.NoGround));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"bigwall:",
											record.BigWall));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"gravlift:",
											record.GravLift));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"standard door:",
											record.HumanDoor));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"blocks fire:",
											record.BlockFire));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"blocks smoke:",
											record.BlockSmoke));

				// LeftRightHalf
//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[38]));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"tu walk:",
											record.TU_Walk));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"tu slide:",
											record.TU_Slide));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"tu fly:",
											record.TU_Fly));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"armor:",
											record.Armor));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"explosive block:",
											record.HE_Block));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"anti-flammability:",
											record.Flammable));

				rtbInfo.SelectionColor = Color.Firebrick;
				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"death tile:",
											record.DieTile));

				rtbInfo.SelectionColor = Color.Firebrick;
				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"alternate tile:",
											record.Alt_MCD));

//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[47]));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"unit y-offset:",
											record.StandOffset));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"tile y-offset:",
											record.TileOffset));

//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[50]));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"block light[0-10]:",
											record.LightBlock));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"footstep sound:",
											record.Footstep));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1} - {2}" + Environment.NewLine,
											"part type:",
											(sbyte)record.PartType,
											Enum.GetName(typeof(PartType), record.PartType)));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1} - {2}" + Environment.NewLine,
											"explosive type:",
											record.HE_Type,
											(record.HE_Type == 0) ? "HE" : (record.HE_Type == 1) ? "smoke" : "unknown"));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"HE Strength:",
											record.HE_Strength));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"smoke block:",
											record.SmokeBlockage));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"fuel:",
											record.Fuel));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"light:",
											record.LightSource));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1} - {2}" + Environment.NewLine,
											"special property:",
											(sbyte)record.Special,
											Enum.GetName(typeof(SpecialType), record.Special)));

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"base object:",
											(sbyte)record.BaseObject));

//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[61]));


				rtbInfo.AppendText(Environment.NewLine);
				rtbInfo.SelectionColor = Color.DarkGray;
				rtbInfo.AppendText("byte data:" + Environment.NewLine);
				rtbInfo.SelectionColor = Color.DarkGray;
				rtbInfo.AppendText(record.ByteTable + Environment.NewLine);
			}

			rtbInfo.SelectionStart  =
			rtbInfo.SelectionLength = 0;
		}
		#endregion
	}
}
