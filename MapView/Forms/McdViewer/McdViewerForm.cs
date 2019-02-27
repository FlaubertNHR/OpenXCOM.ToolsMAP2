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

//				short int ScanG; // A reference into the GEODATA\SCANG.DAT
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
//				unsigned char UFO_Door;	// If it's a UFO door it uses only Frame[0] until it is walked through, then
										// it animates once and becomes Alt_MCD. It changes back at the end of the turn

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"stop LOS:",
											record.StopLOS));
//				unsigned char Stop_LOS; // You cannot see through this tile.

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"no floor:",
											record.NoGround));
//				unsigned char No_Floor; // If 1, then a non-flying unit can't stand here

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"bigwall:",
											record.BigWall));
//				unsigned char Big_Wall; // It's an object (tile type 3), but it acts like a wall

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"gravlift:",
											record.GravLift));
//				unsigned char Gravlift;

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"standard door:",
											record.HumanDoor));
//				unsigned char Door; // It's a human style door--you walk through it and it changes to Alt_MCD

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"blocks fire:",
											record.BlockFire));
//				unsigned char Block_Fire; // If 1, fire won't go through the tile

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"blocks smoke:",
											record.BlockSmoke));
//				unsigned char Block_Smoke; // If 1, smoke won't go through the tile

				//rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[38]));
//				unsigned char u39;

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"tu walk:",
											record.TU_Walk));
//				unsigned char TU_Walk; // The number of TUs required to pass the tile while walking. 0xFF (255) means it's unpassable.

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"tu slide:",
											record.TU_Slide));
//				unsigned char TU_Slide; // sliding things include snakemen and silacoids

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"tu fly:",
											record.TU_Fly));
//				unsigned char TU_Fly; // remember, 0xFF means it's impassable!

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"armor:",
											record.Armor));
//				unsigned char Armour; // The higher this is the less likely it is that a weapon will destroy this tile when it's hit.

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"explosive block:",
											record.HE_Block));
//				unsigned char HE_Block; // How much of an explosion this tile will block

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"anti-flammability:",
											record.Flammable));
//				unsigned char Flammable; // How flammable it is (the higher the harder it is to set aflame)

				rtbInfo.SelectionColor = Color.Firebrick;
				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"death tile:",
											record.DieTile));
//				unsigned char Die_MCD; // If the terrain is destroyed, it is set to 0 and a tile of type Die_MCD is added

				rtbInfo.SelectionColor = Color.Firebrick;
				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"alternate tile:",
											record.Alt_MCD));
//				unsigned char Alt_MCD; // If "Door" or "UFO_Door" is on, then when a unit walks through it the door is set to 0 and a tile type Alt_MCD is added.

//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[47])); // unsigned char u48;

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"unit y-offset:",
											record.StandOffset));
//				signed char T_Level; // When a unit or object is standing on the tile, the unit is shifted by this amount

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"tile y-offset:",
											record.TileOffset));
//				unsigned char P_Level; // When the tile is drawn, this amount is subtracted from its y (so y position-P_Level is where it's drawn)

//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[50]));// unsigned char u51;

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"block light[0-10]:",
											record.LightBlock));
//				unsigned char Light_Block; // The amount of light it blocks, from 0 to 10

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"footstep sound:",
											record.Footstep));
//				unsigned char Footstep; // The Sound Effect set to choose from when footsteps are on the tile

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1} - {2}" + Environment.NewLine,
											"tile type:",
											(sbyte)record.PartType,
											Enum.GetName(typeof(PartType), record.PartType)));
//				info.TileType==0?"floor":info.TileType==1?"west wall":info.TileType==2?"north wall":info.TileType==3?"object":"Unknown"));
//				unsigned char Tile_Type;	// This is the type of tile it is meant to be -- 0=floor, 1=west wall, 2=north wall, 3=object.
											// When this type of tile is in the Die_As or Open_As flags, this value is added to the tile
											// coordinate to determine the byte in which the tile type should be written.

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1} - {2}" + Environment.NewLine,
											"explosive type:",
											record.HE_Type,
											(record.HE_Type == 0) ? "HE" : (record.HE_Type == 1) ? "smoke" : "unknown"));
//				unsigned char HE_Type; // 0=HE 1=Smoke

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"HE Strength:",
											record.HE_Strength));
//				unsigned char HE_Strength; // The strength of the explosion caused when it's destroyed. 0 means no explosion.

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"smoke block:",
											record.SmokeBlockage));
//				unsigned char Smoke_Blockage; // ? Not sure about this

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"fuel:",
											record.Fuel));
//				unsigned char Fuel; // The number of turns the tile will burn when set aflame

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"light:",
											record.LightSource));
//				unsigned char Light_Source; // The amount of light this tile produces

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1} - {2}" + Environment.NewLine,
											"special property:",
											(sbyte)record.TargetType,
											Enum.GetName(typeof(SpecialType), record.TargetType)));
//				unsigned char Target_Type; // The special properties of the tile

				rtbInfo.AppendText(string.Format(
											CultureInfo.InvariantCulture,
											"{0,-20}{1}" + Environment.NewLine,
											"base object:",
											(sbyte)record.BaseObject));
//				unsigned char u61;

//				rtb.AppendText(string.Format(CultureInfo.InvariantCulture, "Unknown data: {0}" + Environment.NewLine, info[61]));
//				unsigned char u62;


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
