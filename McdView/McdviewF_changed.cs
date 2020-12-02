using System;
using System.Windows.Forms;

using XCom;


namespace McdView
{
	public partial class McdviewF
	{
		// Descriptions of MCD entries are at
		// https://www.ufopaedia.org/index.php/MCD

		#region Leave
		/// <summary>
		/// Tags each LoftPanel with its corresponding RecordTextbox.
		/// @note The tagged TextBoxes are tagged with (string)panelid in the
		/// designer. Thus the loft-panels, loft-textboxes, and panelids are all
		/// synched respectively.
		/// </summary>
		private void TagLoftPanels()
		{
			pnl_Loft08.Tag = tb08_loft00;
			pnl_Loft09.Tag = tb09_loft01;
			pnl_Loft10.Tag = tb10_loft02;
			pnl_Loft11.Tag = tb11_loft03;
			pnl_Loft12.Tag = tb12_loft04;
			pnl_Loft13.Tag = tb13_loft05;
			pnl_Loft14.Tag = tb14_loft06;
			pnl_Loft15.Tag = tb15_loft07;
			pnl_Loft16.Tag = tb16_loft08;
			pnl_Loft17.Tag = tb17_loft09;
			pnl_Loft18.Tag = tb18_loft10;
			pnl_Loft19.Tag = tb19_loft11;
		}

		/// <summary>
		/// Checks if either of the ScanG textboxes has focus when mouse-leaving
		/// either of the ScanG labels.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		internal bool isScanG_tb_focused(object control)
		{
			return (control == lbl20 || control == lbl20_scang)
				&& (tb20_scang1.Focused || tb20_scang2.Focused);
		}

		/// <summary>
		/// Tags the labels with their respective textboxes. It's required for
		/// the RecordLabels' mouseleave event(s).
		/// </summary>
		private void TagLabels()
		{
			lbl00.Tag = lbl00_phase0.Tag = tb00_phase0;
			lbl01.Tag = lbl01_phase1.Tag = tb01_phase1;
			lbl02.Tag = lbl02_phase2.Tag = tb02_phase2;
			lbl03.Tag = lbl03_phase3.Tag = tb03_phase3;
			lbl04.Tag = lbl04_phase4.Tag = tb04_phase4;
			lbl05.Tag = lbl05_phase5.Tag = tb05_phase5;
			lbl06.Tag = lbl06_phase6.Tag = tb06_phase6;
			lbl07.Tag = lbl07_phase7.Tag = tb07_phase7;

			lbl08.Tag = lbl08_loft00.Tag = tb08_loft00;
			lbl09.Tag = lbl09_loft01.Tag = tb09_loft01;
			lbl10.Tag = lbl10_loft02.Tag = tb10_loft02;
			lbl11.Tag = lbl11_loft03.Tag = tb11_loft03;
			lbl12.Tag = lbl12_loft04.Tag = tb12_loft04;
			lbl13.Tag = lbl13_loft05.Tag = tb13_loft05;
			lbl14.Tag = lbl14_loft06.Tag = tb14_loft06;
			lbl15.Tag = lbl15_loft07.Tag = tb15_loft07;
			lbl16.Tag = lbl16_loft08.Tag = tb16_loft08;
			lbl17.Tag = lbl17_loft09.Tag = tb17_loft09;
			lbl18.Tag = lbl18_loft10.Tag = tb18_loft10;
			lbl19.Tag = lbl19_loft11.Tag = tb19_loft11;

//			lbl20.Tag = lbl20_scang.Tag = tb20_scang1; //|| tb20_scang2; NOTE: ScanG requires special handling.

			lbl22.Tag = lbl22_.Tag = tb22_; // internal RAM addresses ->
			lbl23.Tag = lbl23_.Tag = tb23_;
			lbl24.Tag = lbl24_.Tag = tb24_;
			lbl25.Tag = lbl25_.Tag = tb25_;
			lbl26.Tag = lbl26_.Tag = tb26_;
			lbl27.Tag = lbl27_.Tag = tb27_;
			lbl28.Tag = lbl28_.Tag = tb28_;
			lbl29.Tag = lbl29_.Tag = tb29_;

			lbl30.Tag = lbl30_isslidingdoor.Tag = tb30_isslidingdoor;
			lbl31.Tag = lbl31_isblocklos   .Tag = tb31_isblocklos;
			lbl32.Tag = lbl32_isdropthrou  .Tag = tb32_isdropthrou;
			lbl33.Tag = lbl33_isbigwall    .Tag = tb33_isbigwall;
			lbl34.Tag = lbl34_isgravlift   .Tag = tb34_isgravlift;
			lbl35.Tag = lbl35_ishingeddoor .Tag = tb35_ishingeddoor;
			lbl36.Tag = lbl36_isblockfire  .Tag = tb36_isblockfire;
			lbl37.Tag = lbl37_isblocksmoke .Tag = tb37_isblocksmoke;

			lbl38.Tag = lbl38_.Tag = tb38_; // LeftRightHalf

			lbl39.Tag = lbl39_tuwalk     .Tag = tb39_tuwalk;
			lbl40.Tag = lbl40_tuslide    .Tag = tb40_tuslide;
			lbl41.Tag = lbl41_tufly      .Tag = tb41_tufly;
			lbl42.Tag = lbl42_armor      .Tag = tb42_armor;
			lbl43.Tag = lbl43_heblock    .Tag = tb43_heblock;
			lbl44.Tag = lbl44_deathid    .Tag = tb44_deathid;
			lbl45.Tag = lbl45_fireresist .Tag = tb45_fireresist;
			lbl46.Tag = lbl46_alternateid.Tag = tb46_alternateid;

			lbl47.Tag = lbl47_.Tag = tb47_; // CloseDoors

			lbl48.Tag = lbl48_terrainoffset.Tag = tb48_terrainoffset;
			lbl49.Tag = lbl49_spriteoffset .Tag = tb49_spriteoffset;

			lbl50.Tag = lbl50_.Tag = tb50_; // dTypeMod

			lbl51.Tag = lbl51_lightblock    .Tag = tb51_lightblock;
			lbl52.Tag = lbl52_footsound     .Tag = tb52_footsound;
			lbl53.Tag = lbl53_parttype      .Tag = tb53_parttype;
			lbl54.Tag = lbl54_hetype        .Tag = tb54_hetype;
			lbl55.Tag = lbl55_hestrength    .Tag = tb55_hestrength;
			lbl56.Tag = lbl56_smokeblock    .Tag = tb56_smokeblock;
			lbl57.Tag = lbl57_fuel          .Tag = tb57_fuel;
			lbl58.Tag = lbl58_lightintensity.Tag = tb58_lightintensity;
			lbl59.Tag = lbl59_specialtype   .Tag = tb59_specialtype;
			lbl60.Tag = lbl60_isbaseobject  .Tag = tb60_isbaseobject;

			lbl61.Tag = lbl61_.Tag = tb61_; // VictoryPoints
		}
		#endregion Leave


		#region Changed events
		// TODO: If value is outside of strict-bounds color its text red.

		// TODO: OnEnter**() -> select text in TextBox when it gains focus
/*			var tb = sender as TextBox; // uh, mouseclick position overrules SelectAll()
			if (tb != null)
			{
				tb.SelectAll();
//				tb.SelectionStart = 0;
//				tb.SelectionLength = tb.Text.Length;
			} */


		/// <summary>
		/// Checks the text of a TextBox for validity. This changes the text if
		/// it's not valid, which causes the OnChanged handler to recurse. It
		/// trims the string and disallows any superfluous preceeding zeros.
		/// Non-numeric text will be checked afterward in the OnChanged handler
		/// itself.
		/// </summary>
		/// <param name="tb">a TextBox to check the text of</param>
		/// <returns>true if the textbox's text is valid</returns>
		private bool TryParseText(Control tb)
		{
			string text = tb.Text.Trim();
			if (text.Length > 1)
			{
				while (text.StartsWith("0", StringComparison.Ordinal))
					text = text.Substring(1);
			}

			if (text != tb.Text)
			{
				tb.Text = text; // recurse
				return false;
			}
			return true;
		}


		/// <summary>
		/// Gets a generic description for the aniphase fields.
		/// </summary>
		/// <returns></returns>
		private string GetPhaseDescription()
		{
			return " Terrain sprites typically cycle through eight phases"
				 + " (sliding doors are static at phase 1 - see #30 isSlidingDoor).";
		}

		/// <summary>
		/// #0 phase 1 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged0(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb00_phase0)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb00_phase0.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite1 = (byte)result;

						PartsPanel .Invalidate();
						pnl_Sprites.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb00_phase0.Text = "0"; // recurse w/ default.
					else if (strict && Spriteset != null)
						tb00_phase0.Text = (Spriteset.Count - 1).ToString();
					else
						tb00_phase0.Text = "255";
				}
			}
			else
				tb00_phase0.Text = String.Empty; // recurse.
		}
		private void OnEnter0(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 1 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox0(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb00_phase0.Text, out result))
			{
				tssl_Overval.Text = "phase 1: " + result;
				OnEnter0(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #1 phase 2 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged1(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb01_phase1)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb01_phase1.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite2 = (byte)result;

						PartsPanel .Invalidate();
						pnl_Sprites.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb01_phase1.Text = "0"; // recurse w/ default.
					else if (strict && Spriteset != null)
						tb01_phase1.Text = (Spriteset.Count - 1).ToString();
					else
						tb01_phase1.Text = "255";
				}
			}
			else
				tb01_phase1.Text = String.Empty; // recurse.
		}
		private void OnEnter1(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 2 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox1(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb01_phase1.Text, out result))
			{
				tssl_Overval.Text = "phase 2: " + result;
				OnEnter1(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #2 phase 3 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged2(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb02_phase2)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb02_phase2.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite3 = (byte)result;

						PartsPanel .Invalidate();
						pnl_Sprites.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb02_phase2.Text = "0"; // recurse w/ default.
					else if (strict && Spriteset != null)
						tb02_phase2.Text = (Spriteset.Count - 1).ToString();
					else
						tb02_phase2.Text = "255";
				}
			}
			else
				tb02_phase2.Text = String.Empty; // recurse.
		}
		private void OnEnter2(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 3 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox2(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb02_phase2.Text, out result))
			{
				tssl_Overval.Text = "phase 3: " + result;
				OnEnter2(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #3 phase 4 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged3(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb03_phase3)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb03_phase3.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite4 = (byte)result;

						PartsPanel .Invalidate();
						pnl_Sprites.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb03_phase3.Text = "0"; // recurse w/ default.
					else if (strict && Spriteset != null)
						tb03_phase3.Text = (Spriteset.Count - 1).ToString();
					else
						tb03_phase3.Text = "255";
				}
			}
			else
				tb03_phase3.Text = String.Empty; // recurse.
		}
		private void OnEnter3(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 4 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox3(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb03_phase3.Text, out result))
			{
				tssl_Overval.Text = "phase 4: " + result;
				OnEnter3(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #4 phase 5 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged4(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb04_phase4)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb04_phase4.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite5 = (byte)result;

						PartsPanel .Invalidate();
						pnl_Sprites.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb04_phase4.Text = "0"; // recurse w/ default.
					else if (strict && Spriteset != null)
						tb04_phase4.Text = (Spriteset.Count - 1).ToString();
					else
						tb04_phase4.Text = "255";
				}
			}
			else
				tb04_phase4.Text = String.Empty; // recurse.
		}
		private void OnEnter4(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 5 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox4(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb04_phase4.Text, out result))
			{
				tssl_Overval.Text = "phase 5: " + result;
				OnEnter4(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #5 phase 6 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged5(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb05_phase5)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb05_phase5.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite6 = (byte)result;

						PartsPanel .Invalidate();
						pnl_Sprites.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb05_phase5.Text = "0"; // recurse w/ default.
					else if (strict && Spriteset != null)
						tb05_phase5.Text = (Spriteset.Count - 1).ToString();
					else
						tb05_phase5.Text = "255";
				}
			}
			else
				tb05_phase5.Text = String.Empty; // recurse.
		}
		private void OnEnter5(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 6 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox5(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb05_phase5.Text, out result))
			{
				tssl_Overval.Text = "phase 6: " + result;
				OnEnter5(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #6 phase 7 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged6(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb06_phase6)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb06_phase6.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite7 = (byte)result;

						PartsPanel .Invalidate();
						pnl_Sprites.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb06_phase6.Text = "0"; // recurse w/ default.
					else if (strict && Spriteset != null)
						tb06_phase6.Text = (Spriteset.Count - 1).ToString();
					else
						tb06_phase6.Text = "255";
				}
			}
			else
				tb06_phase6.Text = String.Empty; // recurse.
		}
		private void OnEnter6(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 7 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox6(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb06_phase6.Text, out result))
			{
				tssl_Overval.Text = "phase 7: " + result;
				OnEnter6(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #7 phase 8 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged7(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb07_phase7)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb07_phase7.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite8 = (byte)result;

						PartsPanel .Invalidate();
						pnl_Sprites.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb07_phase7.Text = "0"; // recurse w/ default.
					else if (strict && Spriteset != null)
						tb07_phase7.Text = (Spriteset.Count - 1).ToString();
					else
						tb07_phase7.Text = "255";
				}
			}
			else
				tb07_phase7.Text = String.Empty; // recurse.
		}
		private void OnEnter7(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 8 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox7(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb07_phase7.Text, out result))
			{
				tssl_Overval.Text = "phase 8: " + result;
				OnEnter7(null, EventArgs.Empty);
			}
		}

		private string GetLoftDescription()
		{
			return " Line of Fire Templates (LoFTs) are representations of solid voxel-space"
				 + " (white in the graphical icons to the right). A 3d-tile in XCOM has"
				 + " x/y/z dimensions of 16/16/24 voxels, and each of the twelve"
				 + " templates is double-layered to stack to the total 24-voxel height.";
		}

		/// <summary>
		/// #8 loft 1 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged8(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb08_loft00)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb08_loft00.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft1 = (byte)result;

						pnl_Loft08 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb08_loft00.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb08_loft00.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb08_loft00.Text = "255";
				}
			}
			else
				tb08_loft00.Text = String.Empty; // recurse.
		}
		private void OnEnter8(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 1 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox8(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb08_loft00.Text, out result))
			{
				tssl_Overval.Text = "loft 1: " + result;
				OnEnter8(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #9 loft 2 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged9(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb09_loft01)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb09_loft01.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft2 = (byte)result;

						pnl_Loft09 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb09_loft01.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb09_loft01.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb09_loft01.Text = "255";
				}
			}
			else
				tb09_loft01.Text = String.Empty; // recurse.
		}
		private void OnEnter9(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 2 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox9(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb09_loft01.Text, out result))
			{
				tssl_Overval.Text = "loft 2: " + result;
				OnEnter9(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #10 loft 3 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged10(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb10_loft02)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb10_loft02.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft3 = (byte)result;

						pnl_Loft10 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb10_loft02.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb10_loft02.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb10_loft02.Text = "255";
				}
			}
			else
				tb10_loft02.Text = String.Empty; // recurse.
		}
		private void OnEnter10(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 3 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox10(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb10_loft02.Text, out result))
			{
				tssl_Overval.Text = "loft 3: " + result;
				OnEnter10(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #11 loft 4 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged11(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb11_loft03)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb11_loft03.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft4 = (byte)result;

						pnl_Loft11 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb11_loft03.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb11_loft03.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb11_loft03.Text = "255";
				}
			}
			else
				tb11_loft03.Text = String.Empty; // recurse.
		}
		private void OnEnter11(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 4 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox11(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb11_loft03.Text, out result))
			{
				tssl_Overval.Text = "loft 4: " + result;
				OnEnter11(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #12 loft 5 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged12(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb12_loft04)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb12_loft04.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft5 = (byte)result;

						pnl_Loft12 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb12_loft04.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb12_loft04.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb12_loft04.Text = "255";
				}
			}
			else
				tb12_loft04.Text = String.Empty; // recurse.
		}
		private void OnEnter12(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 5 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox12(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb12_loft04.Text, out result))
			{
				tssl_Overval.Text = "loft 5: " + result;
				OnEnter12(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #13 loft 6 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged13(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb13_loft05)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb13_loft05.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft6 = (byte)result;

						pnl_Loft13 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb13_loft05.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb13_loft05.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb13_loft05.Text = "255";
				}
			}
			else
				tb13_loft05.Text = String.Empty; // recurse.
		}
		private void OnEnter13(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 6 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox13(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb13_loft05.Text, out result))
			{
				tssl_Overval.Text = "loft 6: " + result;
				OnEnter13(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #14 loft 7 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged14(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb14_loft06)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb14_loft06.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft7 = (byte)result;

						pnl_Loft14 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb14_loft06.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb14_loft06.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb14_loft06.Text = "255";
				}
			}
			else
				tb14_loft06.Text = String.Empty; // recurse.
		}
		private void OnEnter14(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 7 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox14(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb14_loft06.Text, out result))
			{
				tssl_Overval.Text = "loft 7: " + result;
				OnEnter14(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #15 loft 8 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged15(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb15_loft07)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb15_loft07.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft8 = (byte)result;

						pnl_Loft15 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb15_loft07.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb15_loft07.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb15_loft07.Text = "255";
				}
			}
			else
				tb15_loft07.Text = String.Empty; // recurse.
		}
		private void OnEnter15(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 8 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox15(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb15_loft07.Text, out result))
			{
				tssl_Overval.Text = "loft 8: " + result;
				OnEnter15(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #16 loft 9 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged16(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb16_loft08)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb16_loft08.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft9 = (byte)result;

						pnl_Loft16 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb16_loft08.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb16_loft08.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb16_loft08.Text = "255";
				}
			}
			else
				tb16_loft08.Text = String.Empty; // recurse.
		}
		private void OnEnter16(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 9 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox16(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb16_loft08.Text, out result))
			{
				tssl_Overval.Text = "loft 9: " + result;
				OnEnter16(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #17 loft 10 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged17(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb17_loft09)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb17_loft09.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft10 = (byte)result;

						pnl_Loft17 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb17_loft09.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb17_loft09.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb17_loft09.Text = "255";
				}
			}
			else
				tb17_loft09.Text = String.Empty; // recurse.
		}
		private void OnEnter17(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 10 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox17(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb17_loft09.Text, out result))
			{
				tssl_Overval.Text = "loft 10: " + result;
				OnEnter17(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #18 loft 11 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged18(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb18_loft10)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb18_loft10.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft11 = (byte)result;

						pnl_Loft18 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb18_loft10.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb18_loft10.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb18_loft10.Text = "255";
				}
			}
			else
				tb18_loft10.Text = String.Empty; // recurse.
		}
		private void OnEnter18(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 11 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox18(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb18_loft10.Text, out result))
			{
				tssl_Overval.Text = "loft 11: " + result;
				OnEnter18(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #19 loft 12 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged19(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb19_loft11)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb19_loft11.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft12 = (byte)result;

						pnl_Loft19 .Invalidate();
						pnl_IsoLoft.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb19_loft11.Text = "0"; // recurse w/ default.
					else if (strict && LoFT != null)
						tb19_loft11.Text = (LoFT.Length / 256 - 1).ToString();
					else
						tb19_loft11.Text = "255";
				}
			}
			else
				tb19_loft11.Text = String.Empty; // recurse.
		}
		private void OnEnter19(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 12 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox19(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb19_loft11.Text, out result))
			{
				tssl_Overval.Text = "loft 12: " + result;
				OnEnter19(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #20|21 ScanG - this is a special mouseover handler for the ScanG labels.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseEnterLabel20(object sender, EventArgs e)
		{
			lbl_Description.Text = "ScanG (unsigned short) is a reference"
								 + " to an icon in SCANG.DAT that represents a part on"
								 + " the overhead Minimap during tactical.";
		}

		/// <summary>
		/// #20|21 ScanG (little endian unsigned short) + 35
		/// @note This is the value in the MCD file with 35 added = the ID in
		/// SCANG.DAT.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged20(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb20_scang1)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb20_scang1.Text, out result)
						&& result > 34 && result < 65571
						&& (ScanG == null
							|| result == ScanGicon.UNITICON_Max // req'd after choosing an icon per ScanGiconF but the iconset has less than 35 icons
							|| !strict || result < ScanG.Length / ScanGicon.Length_ScanG))
					{
						Parts[SelId].Record.ScanG         = (ushort)(result);
						Parts[SelId].Record.ScanG_reduced = (ushort)(result - ScanGicon.UNITICON_Max);

						tb20_scang2.Text = (result - ScanGicon.UNITICON_Max).ToString();
						pnl_ScanGic.Invalidate(); // uh how does the IsoLoFT panel refresh - it seems to okay ...

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < ScanGicon.UNITICON_Max)
						tb20_scang1.Text = ScanGicon.UNITICON_Max.ToString(); // recurse w/ default.
					else if (strict && ScanG != null)
						tb20_scang1.Text = (ScanG.Length / ScanGicon.Length_ScanG - 1).ToString();
					else
						tb20_scang1.Text = "65570";
				}
			}
			else
				tb20_scang1.Text = String.Empty; // recurse.
		}
		private void OnEnter20(object sender, EventArgs e)
		{
			string top = String.Empty;
			if (ScanG != null)
			{
				int id = ScanG.Length / ScanGicon.Length_ScanG - 1;
				if (id > ScanGicon.UNITICON_Max)
					top = ".." + id;
			}
			else
				top = ".." + 65570;

			lbl_Description.Text = "ScanG (unsigned short) + 35"
								 + Environment.NewLine + Environment.NewLine
								 + "35" + top;
		}
		private void OnMouseEnterTextbox20(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb20_scang1.Text, out result))
			{
				tssl_Overval.Text = "ScanG: " + result;
				OnEnter20(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #20|21 ScanG_reduced (little endian unsigned short)
		/// @note This is the unadjusted value in the MCD file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged20r(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb20_scang2)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb20_scang2.Text, out result)
						&& result > -1 && result < 65536
						&& (ScanG == null
							|| result == 0 // safety.
							|| !strict || result < ScanG.Length / ScanGicon.Length_ScanG - ScanGicon.UNITICON_Max))
					{
						Parts[SelId].Record.ScanG         = (ushort)(result + ScanGicon.UNITICON_Max);
						Parts[SelId].Record.ScanG_reduced = (ushort)(result);

						tb20_scang1.Text = (result + ScanGicon.UNITICON_Max).ToString();
						pnl_ScanGic.Invalidate(); // uh how does the IsoLoFT panel refresh - it seems to okay ...

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb20_scang2.Text = "0"; // recurse w/ default.
					else if (strict && ScanG != null)
						tb20_scang2.Text = (ScanG.Length / ScanGicon.Length_ScanG - 36).ToString();
					else
						tb20_scang2.Text = UInt16.MaxValue.ToString();
				}
			}
			else
				tb20_scang2.Text = String.Empty; // recurse.
		}
		private void OnEnter20r(object sender, EventArgs e)
		{
			string top = String.Empty;
			if (ScanG != null)
			{
				int id = ScanG.Length / ScanGicon.Length_ScanG - 36;
				if (id > 0)
					top = ".." + id;
			}
			else
				top = ".." + UInt16.MaxValue;

			lbl_Description.Text = "ScanG_reduced (unsigned short)"
								 + Environment.NewLine + Environment.NewLine
								 + "0" + top;
		}
		private void OnMouseEnterTextbox20r(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb20_scang2.Text, out result))
			{
				tssl_Overval.Text = "ScanG_reduced: " + result;
				OnEnter20r(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #22 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged22(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb22_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb22_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown22 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb22_.Text = "0"; // recurse w/ default.
					else
						tb22_.Text = "255";
				}
			}
			else
				tb22_.Text = String.Empty; // recurse.
		}
		private void OnEnter22(object sender, EventArgs e)
		{
			lbl_Description.Text = "22 (ubyte)"
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox22(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb22_.Text, out result))
			{
				tssl_Overval.Text = "22: " + result;
				OnEnter22(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #23 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged23(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb23_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb23_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown23 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb23_.Text = "0"; // recurse w/ default.
					else
						tb23_.Text = "255";
				}
			}
			else
				tb23_.Text = String.Empty; // recurse.
		}
		private void OnEnter23(object sender, EventArgs e)
		{
			lbl_Description.Text = "23 (ubyte)"
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox23(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb23_.Text, out result))
			{
				tssl_Overval.Text = "23: " + result;
				OnEnter23(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #24 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged24(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb24_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb24_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown24 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb24_.Text = "0"; // recurse w/ default.
					else
						tb24_.Text = "255";
				}
			}
			else
				tb24_.Text = String.Empty; // recurse.
		}
		private void OnEnter24(object sender, EventArgs e)
		{
			lbl_Description.Text = "24 (ubyte)"
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox24(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb24_.Text, out result))
			{
				tssl_Overval.Text = "24: " + result;
				OnEnter24(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #25 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged25(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb25_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb25_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown25 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb25_.Text = "0"; // recurse w/ default.
					else
						tb25_.Text = "255";
				}
			}
			else
				tb25_.Text = String.Empty; // recurse.
		}
		private void OnEnter25(object sender, EventArgs e)
		{
			lbl_Description.Text = "25 (ubyte)"
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox25(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb25_.Text, out result))
			{
				tssl_Overval.Text = "25: " + result;
				OnEnter25(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #26 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged26(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb26_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb26_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown26 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb26_.Text = "0"; // recurse w/ default.
					else
						tb26_.Text = "255";
				}
			}
			else
				tb26_.Text = String.Empty; // recurse.
		}
		private void OnEnter26(object sender, EventArgs e)
		{
			lbl_Description.Text = "26 (ubyte)"
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox26(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb26_.Text, out result))
			{
				tssl_Overval.Text = "26: " + result;
				OnEnter26(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #27 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged27(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb27_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb27_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown27 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb27_.Text = "0"; // recurse w/ default.
					else
						tb27_.Text = "255";
				}
			}
			else
				tb27_.Text = String.Empty; // recurse.
		}
		private void OnEnter27(object sender, EventArgs e)
		{
			lbl_Description.Text = "27 (ubyte)"
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox27(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb27_.Text, out result))
			{
				tssl_Overval.Text = "27: " + result;
				OnEnter27(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #28 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged28(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb28_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb28_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown28 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb28_.Text = "0"; // recurse w/ default.
					else
						tb28_.Text = "255";
				}
			}
			else
				tb28_.Text = String.Empty; // recurse.
		}
		private void OnEnter28(object sender, EventArgs e)
		{
			lbl_Description.Text = "28 (ubyte)"
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox28(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb28_.Text, out result))
			{
				tssl_Overval.Text = "28: " + result;
				OnEnter28(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #29 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged29(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb29_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb29_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown29 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb29_.Text = "0"; // recurse w/ default.
					else
						tb29_.Text = "255";
				}
			}
			else
				tb29_.Text = String.Empty; // recurse.
		}
		private void OnEnter29(object sender, EventArgs e)
		{
			lbl_Description.Text = "29 (ubyte)"
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox29(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb29_.Text, out result))
			{
				tssl_Overval.Text = "29: " + result;
				OnEnter29(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #30 isSlidingDoor (bool)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged30(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb30_isslidingdoor)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb30_isslidingdoor.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.SlidingDoor = Convert.ToBoolean(result);

						if (strict
							&& Parts[SelId].Record.SlidingDoor
							&& Parts[SelId].Record.HingedDoor)
						{
							tb35_ishingeddoor.Text = "0";
						}

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb30_isslidingdoor.Text = "0"; // recurse w/ default.
					else
						tb30_isslidingdoor.Text = "1";
				}
			}
			else
				tb30_isslidingdoor.Text = String.Empty; // recurse.
		}
		private void OnEnter30(object sender, EventArgs e)
		{
			lbl_Description.Text = "isSlidingDoor (bool) is a true/false value that is relevant only to"
								 + " westwall and northwall parts. Such a part will iterate through the"
								 + " phases of its animation sprites while opening (see #0..7 phase 1..8);"
								 + " only the first four phases are iterated if the part is designated"
								 + " as an EntryPoint (see #59 SpecialType) otherwise all eight phases"
								 + " are iterated. A sliding door displays its phase 8 sprite while open"
								 + " (although it could be replaced by its #46 AlternateId depending on the"
								 + " XCOM build), then at the end of the turn it closes and reverts to phase 1."
								 + " Note that specifying a part as both isSlidingDoor and #35 isHingedDoor"
								 + " could have an unpredictable effect."
								 + Environment.NewLine + Environment.NewLine
								 + "0 False, 1 True";
		}
		private void OnMouseEnterTextbox30(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb30_isslidingdoor.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 False"; break;
					case 1: text = "1 True";  break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "isSlidingDoor: " + text;
				OnEnter30(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #31 isBlockLoS (bool)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged31(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb31_isblocklos)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb31_isblocklos.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.StopLOS = Convert.ToBoolean(result);

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb31_isblocklos.Text = "0"; // recurse w/ default.
					else
						tb31_isblocklos.Text = "1";
				}
			}
			else
				tb31_isblocklos.Text = String.Empty; // recurse.
		}
		private void OnEnter31(object sender, EventArgs e)
		{
			lbl_Description.Text = "isBlockLoS (bool) is a true/false value that signifies"
								 + " whether or not a part stops Line of Sight."
								 + Environment.NewLine + Environment.NewLine
								 + "0 False, 1 True";
		}
		private void OnMouseEnterTextbox31(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb31_isblocklos.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 False"; break;
					case 1: text = "1 True";  break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "isBlockLoS: " + text;
				OnEnter31(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #32 isDropThrou (bool)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged32(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb32_isdropthrou)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb32_isdropthrou.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.NotFloored = Convert.ToBoolean(result);

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb32_isdropthrou.Text = "0"; // recurse w/ default.
					else
						tb32_isdropthrou.Text = "1";
				}
			}
			else
				tb32_isdropthrou.Text = String.Empty; // recurse.
		}
		private void OnEnter32(object sender, EventArgs e)
		{
			lbl_Description.Text = "isDropThrou (bool) is a true/false value that is relevant only to"
								 + " floor parts. A non-flying unit will drop through such a part if true."
								 + Environment.NewLine + Environment.NewLine
								 + "0 False, 1 True";
		}
		private void OnMouseEnterTextbox32(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb32_isdropthrou.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 False"; break;
					case 1: text = "1 True";  break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "isDropThrou: " + text;
				OnEnter32(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #33 isBigWall (bool)
		/// TODO: Allow the bigwall value to be stored as a byte.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged33(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb33_isbigwall)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb33_isbigwall.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.BigWall = Convert.ToBoolean(result);

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb33_isbigwall.Text = "0"; // recurse w/ default.
					else
						tb33_isbigwall.Text = "1";
				}
			}
			else
				tb33_isbigwall.Text = String.Empty; // recurse.
		}
		private void OnEnter33(object sender, EventArgs e)
		{
			lbl_Description.Text = "isBigWall (bool) is a true/false value that is relevant only to"
								 + " content parts. A tile with such a part placed in it is"
								 + " not navigable; a unit is not even allowed to clip through"
								 + " such a tile's corner by walking past it diagonally."
								 + Environment.NewLine + Environment.NewLine
								 + "0 False, 1 True";
		}
		private void OnMouseEnterTextbox33(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb33_isbigwall.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 False"; break;
					case 1: text = "1 True";  break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "isBigWall: " + text;
				OnEnter33(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #34 isGravLift (bool)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged34(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb34_isgravlift)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb34_isgravlift.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.GravLift = Convert.ToBoolean(result);

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb34_isgravlift.Text = "0"; // recurse w/ default.
					else
						tb34_isgravlift.Text = "1";
				}
			}
			else
				tb34_isgravlift.Text = String.Empty; // recurse.
		}
		private void OnEnter34(object sender, EventArgs e)
		{
			lbl_Description.Text = "isGravLift (bool) is a true/false value that is relevant only to"
								 + " floor parts. A unit on a tile with such a part can use"
								 + " it as an elevator to a tile that's a level higher or"
								 + " lower as long as that tile also has a GravLift floor part."
								 + Environment.NewLine + Environment.NewLine
								 + "0 False, 1 True";
		}
		private void OnMouseEnterTextbox34(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb34_isgravlift.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 False"; break;
					case 1: text = "1 True";  break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "isGravLift: " + text;
				OnEnter34(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #35 isHingedDoor (bool)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged35(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb35_ishingeddoor)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb35_ishingeddoor.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.HingedDoor = Convert.ToBoolean(result);

						if (strict
							&& Parts[SelId].Record.HingedDoor
							&& Parts[SelId].Record.SlidingDoor)
						{
							tb30_isslidingdoor.Text = "0";
						}

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb35_ishingeddoor.Text = "0"; // recurse w/ default.
					else
						tb35_ishingeddoor.Text = "1";
				}
			}
			else
				tb35_ishingeddoor.Text = String.Empty; // recurse.
		}
		private void OnEnter35(object sender, EventArgs e)
		{
			lbl_Description.Text = "isHingedDoor (bool) is a true/false value that is relevant only to"
								 + " westwall and northwall parts. Such a part will be replaced by the"
								 + " part designated as its AlternateId when opened. See #46 AlternateId."
								 + " Note that specifying a part as both isHingedDoor and #30 isSlidingDoor"
								 + " could have an unpredictable effect."
								 + Environment.NewLine + Environment.NewLine
								 + "0 False, 1 True";
		}
		private void OnMouseEnterTextbox35(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb35_ishingeddoor.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 False"; break;
					case 1: text = "1 True";  break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "isHingedDoor: " + text;
				OnEnter35(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #36 isBlockFire (bool)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged36(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb36_isblockfire)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb36_isblockfire.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.BlockFire = Convert.ToBoolean(result);

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb36_isblockfire.Text = "0"; // recurse w/ default.
					else
						tb36_isblockfire.Text = "1";
				}
			}
			else
				tb36_isblockfire.Text = String.Empty; // recurse.
		}
		private void OnEnter36(object sender, EventArgs e)
		{
			lbl_Description.Text = "isBlockFire (bool) is a true/false value that signifies"
								 + " whether or not a part stops fire spreading."
								 + Environment.NewLine + Environment.NewLine
								 + "0 False, 1 True";
		}
		private void OnMouseEnterTextbox36(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb36_isblockfire.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 False"; break;
					case 1: text = "1 True";  break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "isBlockFire: " + text;
				OnEnter36(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #37 isBlockSmoke (bool)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged37(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb37_isblocksmoke)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb37_isblocksmoke.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.BlockSmoke = Convert.ToBoolean(result);

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb37_isblocksmoke.Text = "0"; // recurse w/ default.
					else
						tb37_isblocksmoke.Text = "1";
				}
			}
			else
				tb37_isblocksmoke.Text = String.Empty; // recurse.
		}
		private void OnEnter37(object sender, EventArgs e)
		{
			lbl_Description.Text = "isBlockSmoke (bool) is a true/false value that signifies"
								 + " whether or not a part stops smoke spreading."
								 + Environment.NewLine + Environment.NewLine
								 + "0 False, 1 True";
		}
		private void OnMouseEnterTextbox37(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb37_isblocksmoke.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 False"; break;
					case 1: text = "1 True";  break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "isBlockSmoke: " + text;
				OnEnter37(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #38 LeftRightHalf (byte)
		/// @note aka StartPhase
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged38(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb38_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb38_.Text, out result)
						&&     ((strict && result == 3)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.LeftRightHalf = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (!strict && result < 1)
						tb38_.Text = "0"; // recurse w/ default.
					else if (strict)
						tb38_.Text = "3";
					else
						tb38_.Text = "255";
				}
			}
			else
				tb38_.Text = String.Empty; // recurse.
		}
		private void OnEnter38(object sender, EventArgs e)
		{
			lbl_Description.Text = "LeftRightHalf (ubyte) is always 3. It is supposed to decide whether to"
								 + " draw the left-half, the right-half, or both halves of a part's"
								 + " sprite but is not used."
								 + Environment.NewLine + Environment.NewLine
								 + "3 Full (1 Left, 2 Right)";
		}
		private void OnMouseEnterTextbox38(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb38_.Text, out result))
			{
				string text;
				switch (result)
				{
					case 1: text = "1 Lefthalf";  break;
					case 2: text = "2 Righthalf"; break;
					case 3: text = "3 Full";      break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "LeftRightHalf: " + text;
				OnEnter38(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #39 TuWalk (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged39(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb39_tuwalk)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb39_tuwalk.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.TU_Walk = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1) //|| Parts[SelId].Record.PartType == PartType.Floor)
						tb39_tuwalk.Text = "0"; // recurse w/ default.
					else
						tb39_tuwalk.Text = "255";
				}
			}
			else
				tb39_tuwalk.Text = String.Empty; // recurse.
		}
		private void OnEnter39(object sender, EventArgs e)
		{
			lbl_Description.Text = "TuWalk (ubyte) is the turnunits required to navigate a part"
								 + " by a walking unit. A value of 255 means the part is non-navigable."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox39(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb39_tuwalk.Text, out result))
			{
				tssl_Overval.Text = "TuWalk: " + result + (result == 255 ? " impassable" : String.Empty);
				OnEnter39(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #40 TuSlide (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged40(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb40_tuslide)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb40_tuslide.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.TU_Slide = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1) //|| Parts[SelId].Record.PartType == PartType.Floor)
						tb40_tuslide.Text = "0"; // recurse w/ default.
					else
						tb40_tuslide.Text = "255";
				}
			}
			else
				tb40_tuslide.Text = String.Empty; // recurse.
		}
		private void OnEnter40(object sender, EventArgs e)
		{
			lbl_Description.Text = "TuSlide (ubyte) is the turnunits required to navigate a part"
								 + " by a sliding unit. A value of 255 means the part is non-navigable."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox40(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb40_tuslide.Text, out result))
			{
				tssl_Overval.Text = "TuSlide: " + result + (result == 255 ? " impassable" : String.Empty);
				OnEnter40(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #41 TuFly (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged41(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb41_tufly)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb41_tufly.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.TU_Fly = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1) //|| Parts[SelId].Record.PartType == PartType.Floor)
						tb41_tufly.Text = "0"; // recurse w/ default.
					else
						tb41_tufly.Text = "255";
				}
			}
			else
				tb41_tufly.Text = String.Empty; // recurse.
		}
		private void OnEnter41(object sender, EventArgs e)
		{
			lbl_Description.Text = "TuFly (ubyte) is the turnunits required to navigate a part"
								 + " by a flying unit. A value of 255 means the part is non-navigable."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox41(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb41_tufly.Text, out result))
			{
				tssl_Overval.Text = "TuFly: " + result + (result == 255 ? " impassable" : String.Empty);
				OnEnter41(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #42 Armor (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged42(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb42_armor)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb42_armor.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Armor = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb42_armor.Text = "0"; // recurse w/ default.
					else
						tb42_armor.Text = "255";
				}
			}
			else
				tb42_armor.Text = String.Empty; // recurse.
		}
		private void OnEnter42(object sender, EventArgs e)
		{
			lbl_Description.Text = "Armor (ubyte) is the damage that a part must sustain in a single"
								 + " hit in order to be destroyed. Note that part-damage in XCOM is"
								 + " not cumulative; it's all or nothing. A value of 255 typically"
								 + " designates a part as indestructible although it depends on the"
								 + " XCOM build. See #44 DeathId."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox42(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb42_armor.Text, out result))
			{
				tssl_Overval.Text = "Armor: " + result + (result == 255 ? " indestructible" : String.Empty);
				OnEnter42(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #43 HeBlock (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged43(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb43_heblock)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb43_heblock.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.HE_Block = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb43_heblock.Text = "0"; // recurse w/ default.
					else
						tb43_heblock.Text = "255";
				}
			}
			else
				tb43_heblock.Text = String.Empty; // recurse.
		}
		private void OnEnter43(object sender, EventArgs e)
		{
			lbl_Description.Text = "HeBlock (ubyte) is the reduction of damage that occurs as"
								 + " an explosion passes through a tile with this part."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox43(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb43_heblock.Text, out result))
			{
				tssl_Overval.Text = "HeBlock: " + result;
				OnEnter43(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #44 DeathId (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged44(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb44_deathid)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb44_deathid.Text, out result)
						&&     ((strict && result > -1 && result < 256 && result < Parts.Length) // NOTE: 'Parts' shall not be null here.
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.DieTile = (byte)result;

						if (result != 0 && result < Parts.Length)
							Parts[SelId].Dead = Parts[result];
						else
							Parts[SelId].Dead = null;

						PartsPanel.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb44_deathid.Text = "0"; // recurse w/ default.
					else if (strict)
						tb44_deathid.Text = (Parts.Length - 1).ToString();
					else
						tb44_deathid.Text = "255";
				}
			}
			else
				tb44_deathid.Text = String.Empty; // recurse.
		}
		private void OnEnter44(object sender, EventArgs e)
		{
			lbl_Description.Text = "DeathId (ubyte) is the ID of the part that will replace a"
								 + " part when it gets destroyed. The death-part should be in the"
								 + " MCD file with the original part. A value of 0 indicates"
								 + " that if a part is destroyed it will not be replaced by a"
								 + " death-part; that is, a value of 0 does not reference ID #0."
								 + " See #42 Armor."
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Parts != null ? (Parts.Length - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox44(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb44_deathid.Text, out result))
			{
				tssl_Overval.Text = "DeathId: " + result + (result == 0 ? " none" : String.Empty);
				OnEnter44(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #45 FireResist (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged45(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb45_fireresist)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb45_fireresist.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.FireResist = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb45_fireresist.Text = "0"; // recurse w/ default.
					else
						tb45_fireresist.Text = "255";
				}
			}
			else
				tb45_fireresist.Text = String.Empty; // recurse.
		}
		private void OnEnter45(object sender, EventArgs e)
		{
			lbl_Description.Text = "FireResist (ubyte) is a part's resistance to catching fire."
								 + " A value of 255 means that the part is impervious to fire"
								 + " although this could depend on the XCOM build; in any case"
								 + " a value of 255 makes a part highly unlikely to burn. See"
								 + " also #57 Fuel."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox45(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb45_fireresist.Text, out result))
			{
				tssl_Overval.Text = "FireResist: " + result + (result == 255 ? " impervious" : String.Empty);
				OnEnter45(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #46 AlternateId (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged46(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb46_alternateid)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb46_alternateid.Text, out result)
						&&     ((strict && result > -1 && result < 256 && result < Parts.Length) // NOTE: 'Parts' shall not be null here.
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Alt_MCD = (byte)result;

						if (result != 0 && result < Parts.Length)
							Parts[SelId].Altr = Parts[result];
						else
							Parts[SelId].Altr = null;

						PartsPanel.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb46_alternateid.Text = "0"; // recurse w/ default.
					else if (strict)
						tb46_alternateid.Text = (Parts.Length - 1).ToString();
					else
						tb46_alternateid.Text = "255";
				}
			}
			else
				tb46_alternateid.Text = String.Empty; // recurse.
		}
		private void OnEnter46(object sender, EventArgs e)
		{
			lbl_Description.Text = "AlternateId (ubyte) is the ID of the part that will replace a"
								 + " part that is a hinged door and has been opened. The alternate-part"
								 + " should be in the MCD file with the original part. A value of"
								 + " 0 indicates that if a part is opened it will not be replaced"
								 + " by an alternate-part; that is, a value of 0 does not"
								 + " reference ID #0. See #35 isHingedDoor."
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Parts != null ? (Parts.Length - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox46(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb46_alternateid.Text, out result))
			{
				tssl_Overval.Text = "AlternateId: " + result + (result == 0 ? " none" : String.Empty);
				OnEnter46(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #47 CloseDoors (byte)
		/// @note Is probably a boolean.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged47(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb47_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb47_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown47 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb47_.Text = "0"; // recurse w/ default.
					else
						tb47_.Text = "255";
				}
			}
			else
				tb47_.Text = String.Empty; // recurse.
		}
		private void OnEnter47(object sender, EventArgs e)
		{
			lbl_Description.Text = "CloseDoors (ubyte) is a flag that MicroProse uses to check if"
								 + " sliding doors are open and should be closed at the end of a turn."
								 + " It is a runtime variable and is not used by newer executables."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox47(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb47_.Text, out result))
			{
				tssl_Overval.Text = "CloseDoors: " + result;
				OnEnter47(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #48 TerrainOffset (sbyte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged48(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb48_terrainoffset)) // else recurse // TODO: check 0 after "-" sign
				{
					int result;
					if (Int32.TryParse(tb48_terrainoffset.Text, out result)
						&&     ((strict && result > - 25 && result < 1)
							|| (!strict && result > -129 && result < 128)))
					{
						Parts[SelId].Record.StandOffset = (sbyte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result == 0) // ie. failed to parse
						tb48_terrainoffset.Text = "0"; // recurse w/ default.
					else if (strict)
					{
						if      (result < -24) tb48_terrainoffset.Text = "-24";
						else if (result >   0) tb48_terrainoffset.Text =   "0";
					}
					else if (result < -128) tb48_terrainoffset.Text = "-128";
					else                    tb48_terrainoffset.Text =  "127";
				}
			}
			else
				tb48_terrainoffset.Text = String.Empty; // recurse.
		}
		private void OnEnter48(object sender, EventArgs e)
		{
			lbl_Description.Text = "TerrainOffset (sbyte) is the distance in voxels between the height"
								 + " of a tile's lowest voxel-level and the height that objects such"
								 + " as units and items (possibly including smoke and fire graphics)"
								 + " should appear at and be considered to have their bottom voxel"
								 + " positioned at (if applicable). Note that a negative value raises"
								 + " the object and that the standard value of 0 positions the object"
								 + " at the floor-level of a tile. This variable has relevance only"
								 + " for floor and content parts; the TerrainOffset of westwall or"
								 + " northwall parts is not evaluated. Also note that for a unit to"
								 + " step from one tile to another their respective terrain-heights"
								 + " can be no greater than 8 voxels and that the total distance"
								 + " between levels is 24 voxels."
								 + Environment.NewLine + Environment.NewLine
								 + "-24..0";
		}
		private void OnMouseEnterTextbox48(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb48_terrainoffset.Text, out result))
			{
				tssl_Overval.Text = "TerrainOffset: " + result;
				OnEnter48(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #49 SpriteOffset (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged49(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb49_spriteoffset)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb49_spriteoffset.Text, out result)
						&&     ((strict && result > -1 && result < 25)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.TileOffset = (byte)result;
						PartsPanel .Invalidate();
						pnl_Sprites.Invalidate();

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb49_spriteoffset.Text = "0"; // recurse w/ default.
					else if (strict)
						tb49_spriteoffset.Text = "24";
					else
						tb49_spriteoffset.Text = "255";
				}
			}
			else
				tb49_spriteoffset.Text = String.Empty; // recurse.
		}
		private void OnEnter49(object sender, EventArgs e)
		{
			lbl_Description.Text = "SpriteOffset (ubyte) is the vertical distance in pixels between"
								 + " the position that a part's sprite is usually drawn at and the"
								 + " position at which it will actually be drawn. Note that a positive"
								 + " value raises the sprite and that the standard value is 0. A value"
								 + " of 24 raises the sprite 1 vertical level."
								 + Environment.NewLine + Environment.NewLine
								 + "0..24";
		}
		private void OnMouseEnterTextbox49(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb49_spriteoffset.Text, out result))
			{
				tssl_Overval.Text = "SpriteOffset: " + result;
				OnEnter49(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #50 dTypeMod (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged50(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb50_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb50_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown50 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb50_.Text = "0"; // recurse w/ default.
					else
						tb50_.Text = "255";
				}
			}
			else
				tb50_.Text = String.Empty; // recurse.
		}
		private void OnEnter50(object sender, EventArgs e)
		{
			lbl_Description.Text = "dTypeMod (ubyte) was perhaps intended to be a received damage-type"
								 + " modifier (eg. soft target, hard target, etc.) but is not used."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox50(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb50_.Text, out result))
			{
				tssl_Overval.Text = "dTypeMod: " + result;
				OnEnter50(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #51 LightBlock (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged51(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb51_lightblock)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb51_lightblock.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.LightBlock = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb51_lightblock.Text = "0"; // recurse w/ default.
					else
						tb51_lightblock.Text = "255";
				}
			}
			else
				tb51_lightblock.Text = String.Empty; // recurse.
		}
		private void OnEnter51(object sender, EventArgs e)
		{
			lbl_Description.Text = "LightBlock (ubyte) is the reduction of light that occurs as"
								 + " light passes through a tile with this part. This value"
								 + " is not used although it could depend on the XCOM build."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox51(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb51_lightblock.Text, out result))
			{
				tssl_Overval.Text = "LightBlock: " + result;
				OnEnter51(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #52 FootSound (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged52(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb52_footsound)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb52_footsound.Text, out result)
						&&     ((strict && result > -1 && result < 7)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Footstep = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb52_footsound.Text = "0"; // recurse w/ default.
					else if (strict)
						tb52_footsound.Text = "6";
					else
						tb52_footsound.Text = "255";
				}
			}
			else
				tb52_footsound.Text = String.Empty; // recurse.
		}
		private void OnEnter52(object sender, EventArgs e)
		{
			lbl_Description.Text = "FootSound (ubyte) specifies the footstep-sound that units make"
								 + " when walking on a part. This value has relevance to floor and"
								 + " content parts only. Note that the sounds and sound-types will"
								 + " differ depending on your XCOM version/edition and/or executable."
								 + " The listed descriptions should be considered as approximations."
								 + Environment.NewLine + Environment.NewLine
								 + "0 None, 1 Metal, 2 Wood/Stone, 3 Dirt, 4 Mud, 5 Sand, 6 Snow"; // TODO: <- Investigate.
		}
		private void OnMouseEnterTextbox52(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb52_footsound.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 None";       break;
					case 1: text = "1 Metal";      break;
					case 2: text = "2 Wood/Stone"; break;
					case 3: text = "3 Dirt";       break; // TODO: <- Investigate.
					case 4: text = "4 Mud";        break;
					case 5: text = "5 Sand";       break;
					case 6: text = "6 Snow";       break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "FootSound: " + text;
				OnEnter52(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #53 PartType (byte/PartType)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged53(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb53_parttype)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb53_parttype.Text, out result)
						&&     ((strict && result > -1 && result < 4)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.PartType = (PartType)result;	// NOTE: Assigning integers that are not
																			// explicitly defined in the enum is allowed.
						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb53_parttype.Text = "0"; // recurse w/ default.
					else if (strict)
						tb53_parttype.Text = "3";
					else
						tb53_parttype.Text = "255";
				}
			}
			else
				tb53_parttype.Text = String.Empty; // recurse.
		}
		private void OnEnter53(object sender, EventArgs e)
		{
			lbl_Description.Text = "PartType (ubyte) specifies the tile-slot in which a part should"
								 + " be placed."
								 + Environment.NewLine + Environment.NewLine
								 + "0 Floor, 1 Westwall, 2 Northwall, 3 Content";
		}
		private void OnMouseEnterTextbox53(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb53_parttype.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 FLOOR";   break;
					case 1: text = "1 WEST";    break;
					case 2: text = "2 NORTH";   break;
					case 3: text = "3 CONTENT"; break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "PartType: " + text;
				OnEnter53(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #54 HeType (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged54(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb54_hetype)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb54_hetype.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.HE_Type = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb54_hetype.Text = "0"; // recurse w/ default.
					else if (strict)
						tb54_hetype.Text = "1";
					else
						tb54_hetype.Text = "255";
				}
			}
			else
				tb54_hetype.Text = String.Empty; // recurse.
		}
		private void OnEnter54(object sender, EventArgs e)
		{
			lbl_Description.Text = "HeType (ubyte) is the type of explosion that occurs when this"
								 + " part gets destroyed. No explosion will occur if #55 HeStrength"
								 + " has a value of 0."
								 + Environment.NewLine + Environment.NewLine
								 + "0 HighExplosive, 1 Smoke";
		}
		private void OnMouseEnterTextbox54(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb54_hetype.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 HighExplosive"; break;
					case 1: text = "1 Smoke";         break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "HeType: " + text;
				OnEnter54(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #55 HeStrength (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged55(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb55_hestrength)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb55_hestrength.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.HE_Strength = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb55_hestrength.Text = "0"; // recurse w/ default.
					else
						tb55_hestrength.Text = "255";
				}
			}
			else
				tb55_hestrength.Text = String.Empty; // recurse.
		}
		private void OnEnter55(object sender, EventArgs e)
		{
			lbl_Description.Text = "HeStrength (ubyte) is the power of an explosion that occurs when"
								 + " a part is destroyed. The type of explosion (high explosive or"
								 + " smoke) is determined by #54 HeType."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox55(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb55_hestrength.Text, out result))
			{
				tssl_Overval.Text = "HeStrength: " + result;
				OnEnter55(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #56 SmokeBlock (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged56(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb56_smokeblock)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb56_smokeblock.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.SmokeBlockage = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb56_smokeblock.Text = "0"; // recurse w/ default.
					else
						tb56_smokeblock.Text = "255";
				}
			}
			else
				tb56_smokeblock.Text = String.Empty; // recurse.
		}
		private void OnEnter56(object sender, EventArgs e)
		{
			lbl_Description.Text = "SmokeBlock (ubyte) is the reduction of smoke that occurs as"
								 + " an explosion passes through a tile with this part. This value"
								 + " is not used; see #37 isBlockSmoke instead."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox56(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb56_smokeblock.Text, out result))
			{
				tssl_Overval.Text = "SmokeBlock: " + result;
				OnEnter56(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #57 Fuel (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged57(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb57_fuel)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb57_fuel.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Fuel = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb57_fuel.Text = "0"; // recurse w/ default.
					else
						tb57_fuel.Text = "255";
				}
			}
			else
				tb57_fuel.Text = String.Empty; // recurse.
		}
		private void OnEnter57(object sender, EventArgs e)
		{
			lbl_Description.Text = "Fuel (ubyte) is the number of turns that a part will"
								 + " burn once it catches fire. See #45 FireResist."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox57(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb57_fuel.Text, out result))
			{
				tssl_Overval.Text = "Fuel: " + result;
				OnEnter57(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #58 LightIntensity (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged58(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb58_lightintensity)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb58_lightintensity.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.LightSource = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb58_lightintensity.Text = "0"; // recurse w/ default.
					else
						tb58_lightintensity.Text = "255";
				}
			}
			else
				tb58_lightintensity.Text = String.Empty; // recurse.
		}
		private void OnEnter58(object sender, EventArgs e)
		{
			lbl_Description.Text = "LightIntensity (ubyte) controls how brightly a part lights up its surroundings."
								 + " A part with a value of 0 should not emit light although the exact"
								 + " effect depends on the XCOM build."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox58(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb58_lightintensity.Text, out result))
			{
				tssl_Overval.Text = "LightIntensity: " + result + (result == 0 ? " none" : String.Empty);
				OnEnter58(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #59 SpecialType (byte/SpecialType)
		/// @note aka TargetType
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged59(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb59_specialtype)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb59_specialtype.Text, out result)
						&&     ((strict && result > -1 && result < 15)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Special = (SpecialType)result;	// NOTE: Assigning integers that are not
																			// explicitly defined in the enum is allowed.
						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb59_specialtype.Text = "0"; // recurse w/ default.
					else if (strict)
						tb59_specialtype.Text = "14";
					else
						tb59_specialtype.Text = "255";
				}
			}
			else
				tb59_specialtype.Text = String.Empty; // recurse.
		}
		private void OnEnter59(object sender, EventArgs e)
		{
			// NOTE: "\u00A0" is a UTF nonbreaking-space char.
			lbl_Description.Text = "SpecialType (ubyte) is the special property of a part. It determines where"
								 + " XCOM agents can enter and exit a Map, or what is salvaged at the"
								 + " end of tactical, or if the part must be destroyed for XCOM to"
								 + " succeed at tactical. (UFO/TFTD)"
								 + Environment.NewLine + Environment.NewLine
								 + "0\u00A0None, 1\u00A0EntryPoint, 2\u00A0PowerSource/IonBeamAccelerators,"
								 + " 3\u00A0Navigation, 4\u00A0Construction, 5\u00A0Food/Cryogenics,"
								 + " 6\u00A0Reproduction/Cloning, 7\u00A0Entertainment/LearningArrays,"
								 + " 8\u00A0Surgery/Implanter, 9\u00A0Examination, 10\u00A0Alloys/Plastics,"
								 + " 11\u00A0Habitat/Reanimation, 12\u00A0RuinedAlloys/Plastics, 13\u00A0ExitPoint,"
								 + " 14\u00A0MustDestroyToSucceed"; //AlienBrain/T'lethPowerCylinders
		}
		private void OnMouseEnterTextbox59(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb59_specialtype.Text, out result))
			{
				string text;
				switch (result)
				{
					case  0: text =  "0 None";                            break;
					case  1: text =  "1 EntryPoint";                      break;
					case  2: text =  "2 PowerSource/IonBeamAccelerators"; break;
					case  3: text =  "3 Navigation";                      break;
					case  4: text =  "4 Construction";                    break;
					case  5: text =  "5 Food/Cryogenics";                 break;
					case  6: text =  "6 Reproduction/Cloning";            break;
					case  7: text =  "7 Entertainment/LearningArrays";    break;
					case  8: text =  "8 Surgery/Implanter";               break;
					case  9: text =  "9 Examination";                     break;
					case 10: text = "10 Alloys/Plastics";                 break;
					case 11: text = "11 Habitat/Reanimation";             break;
					case 12: text = "12 RuinedAlloys/Plastics";           break;
					case 13: text = "13 ExitPoint";                       break;
					case 14: text = "14 MustDestroyToSucceed";            break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "SpecialType: " + text;
				OnEnter59(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #60 isBaseObject (bool)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged60(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb60_isbaseobject)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb60_isbaseobject.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.BaseObject = Convert.ToBoolean(result);

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb60_isbaseobject.Text = "0"; // recurse w/ default.
					else
						tb60_isbaseobject.Text = "1";
				}
			}
			else
				tb60_isbaseobject.Text = String.Empty; // recurse.
		}
		private void OnEnter60(object sender, EventArgs e)
		{
			lbl_Description.Text = "isBaseObject (bool) is a true/false value that is relevant only to"
								 + " content parts. aLiens will shoot these parts during base defense"
								 + " tacticals as they try to destroy XCOM facilities."
								 + Environment.NewLine + Environment.NewLine
								 + "0 False, 1 True";
		}
		private void OnMouseEnterTextbox60(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb60_isbaseobject.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 False"; break;
					case 1: text = "1 True";  break;

					default: text = result.ToString(); break;
				}
				tssl_Overval.Text = "isBaseObject: " + text;
				OnEnter60(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #61 VictoryPoints (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged61(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb61_)) // else recurse
				{
					int result;
					if (Int32.TryParse(tb61_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown61 = (byte)result;

						if (!InitFields)
							Changed = CacheLoad.Changed(Parts);
					}
					else if (result < 1)
						tb61_.Text = "0"; // recurse w/ default.
					else
						tb61_.Text = "255";
				}
			}
			else
				tb61_.Text = String.Empty; // recurse.
		}
		private void OnEnter61(object sender, EventArgs e)
		{
			lbl_Description.Text = "VictoryPoints (ubyte) was perhaps intended to be points awarded"
								 + " for destroying a part but is not used."
								 + Environment.NewLine + Environment.NewLine
								 + "0..255";
		}
		private void OnMouseEnterTextbox61(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb61_.Text, out result))
			{
				tssl_Overval.Text = "VictoryPoints: " + result;
				OnEnter61(null, EventArgs.Empty);
			}
		}
		#endregion Changed events
	}
}
