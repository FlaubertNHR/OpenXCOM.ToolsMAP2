using System;
using System.Windows.Forms;

using XCom;


namespace McdView
{
	internal partial class McdviewF
	{
		// Descriptions of MCD entries are at
		// https://www.ufopaedia.org/index.php/MCD

		#region Leave
		/// <summary>
		/// Handles mouseover leaving a Label or TextBox.
		/// @note Retains the current description if the associated TextBox has
		/// input-focus.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseLeave(object sender, EventArgs e)
		{
			tssl_Overvalue.Text = String.Empty;

			var tb = sender as TextBox;
			if (tb == null)
				tb = GetTextbox(sender);

			if (tb == null || !tb.Focused)
				lbl_Description.Text = String.Empty;
		}

		/// <summary>
		/// Gets the TextBox associated with a given Label.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		private TextBox GetTextbox(object label)
		{
			if (label == lbl_SpriteShade)                        return tb_SpriteShade;


			if (label == lbl0  || label == lbl0_phase0)          return tb0_phase0;
			if (label == lbl1  || label == lbl1_phase1)          return tb1_phase1;
			if (label == lbl2  || label == lbl2_phase2)          return tb2_phase2;
			if (label == lbl3  || label == lbl3_phase3)          return tb3_phase3;
			if (label == lbl4  || label == lbl4_phase4)          return tb4_phase4;
			if (label == lbl5  || label == lbl5_phase5)          return tb5_phase5;
			if (label == lbl6  || label == lbl6_phase6)          return tb6_phase6;
			if (label == lbl7  || label == lbl7_phase7)          return tb7_phase7;

			if (label == lbl8  || label == lbl8_loft00)          return tb8_loft00;
			if (label == lbl9  || label == lbl9_loft02)          return tb9_loft02;
			if (label == lbl10 || label == lbl10_loft04)         return tb10_loft04;
			if (label == lbl11 || label == lbl11_loft06)         return tb11_loft06;
			if (label == lbl12 || label == lbl12_loft08)         return tb12_loft08;
			if (label == lbl13 || label == lbl13_loft10)         return tb13_loft10;
			if (label == lbl14 || label == lbl14_loft12)         return tb14_loft12;
			if (label == lbl15 || label == lbl15_loft14)         return tb15_loft14;
			if (label == lbl16 || label == lbl16_loft16)         return tb16_loft16;
			if (label == lbl17 || label == lbl17_loft18)         return tb17_loft18;
			if (label == lbl18 || label == lbl18_loft20)         return tb18_loft20;
			if (label == lbl19 || label == lbl19_loft22)         return tb19_loft22;

//			if (label == lbl20 || label == lbl20_scang)          return tb20_scang1; // tb21_scang2

			if (label == lbl22 || label == lbl22_)               return tb22_; // internal RAM addresses ->
			if (label == lbl23 || label == lbl23_)               return tb23_;
			if (label == lbl24 || label == lbl24_)               return tb24_;
			if (label == lbl25 || label == lbl25_)               return tb25_;
			if (label == lbl26 || label == lbl26_)               return tb26_;
			if (label == lbl27 || label == lbl27_)               return tb27_;
			if (label == lbl28 || label == lbl28_)               return tb28_;
			if (label == lbl29 || label == lbl29_)               return tb29_;

			if (label == lbl30 || label == lbl30_isslidingdoor)  return tb30_isslidingdoor;
			if (label == lbl31 || label == lbl31_isblocklos)     return tb31_isblocklos;
			if (label == lbl32 || label == lbl32_isdropthrou)    return tb32_isdropthrou;
			if (label == lbl33 || label == lbl33_isbigwall)      return tb33_isbigwall;
			if (label == lbl34 || label == lbl34_isgravlift)     return tb34_isgravlift;
			if (label == lbl35 || label == lbl35_ishingeddoor)   return tb35_ishingeddoor;
			if (label == lbl36 || label == lbl36_isblockfire)    return tb36_isblockfire;
			if (label == lbl37 || label == lbl37_isblocksmoke)   return tb37_isblocksmoke;

			if (label == lbl38 || label == lbl38_)               return tb38_; // LeftRightHalf

			if (label == lbl39 || label == lbl39_tuwalk)         return tb39_tuwalk;
			if (label == lbl40 || label == lbl40_tuslide)        return tb40_tuslide;
			if (label == lbl41 || label == lbl41_tufly)          return tb41_tufly;
			if (label == lbl42 || label == lbl42_armor)          return tb42_armor;
			if (label == lbl43 || label == lbl43_heblock)        return tb43_heblock;
			if (label == lbl44 || label == lbl44_deathid)        return tb44_deathid;
			if (label == lbl45 || label == lbl45_fireresist)     return tb45_fireresist;
			if (label == lbl46 || label == lbl46_alternateid)    return tb46_alternateid;

			if (label == lbl47 || label == lbl47_)               return tb47_; // CloseDoors

			if (label == lbl48 || label == lbl48_terrainoffset)  return tb48_terrainoffset;
			if (label == lbl49 || label == lbl49_spriteoffset)   return tb49_spriteoffset;

			if (label == lbl50 || label == lbl50_)               return tb50_; // dTypeMod

			if (label == lbl51 || label == lbl51_lightblock)     return tb51_lightblock;
			if (label == lbl52 || label == lbl52_footsound)      return tb52_footsound;
			if (label == lbl53 || label == lbl53_parttype)       return tb53_parttype;
			if (label == lbl54 || label == lbl54_hetype)         return tb54_hetype;
			if (label == lbl55 || label == lbl55_hestrength)     return tb55_hestrength;
			if (label == lbl56 || label == lbl56_smokeblock)     return tb56_smokeblock;
			if (label == lbl57 || label == lbl57_fuel)           return tb57_fuel;
			if (label == lbl58 || label == lbl58_lightintensity) return tb58_lightintensity;
			if (label == lbl59 || label == lbl59_specialtype)    return tb59_specialtype;
			if (label == lbl60 || label == lbl60_isbaseobject)   return tb60_isbaseobject;

			if (label == lbl61 || label == lbl61_)               return tb61_; // VictoryPoints

			return null;
		}

		/// <summary>
		/// Handles a TextBox or CheckBox losing input-focus.
		/// @note Clears the current description disregarding mouseover state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLeave(object sender, EventArgs e)
		{
			lbl_Description.Text = String.Empty;
		}

		/// <summary>
		/// Handles mouseover leaving the "STRICT" Label or CheckBox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseLeaveStrict(object sender, EventArgs e)
		{
			if (!cb_Strict.Focused)
				lbl_Description.Text = String.Empty;
		}
		#endregion Leave


		#region Enter (options)
		private void OnEnterStrict(object sender, EventArgs e)
		{
			lbl_Description.Text = "STRICT enforces valid values in XCOM."
								 + " Unchecked allows values outside what's expected (for expert experts only"
								 + " - ie people who code their own XCOM executable and require extended values)."
								 + " This value is not saved."
								 + Environment.NewLine + Environment.NewLine
								 + "default checked";
		}

		private void OnEnterSpriteShade(object sender, EventArgs e)
		{
			lbl_Description.Text = "SpriteShade is an inverse gamma-value only for sprites drawn in this app."
								 + " It has nothing to do with palette-based sprite-shading in XCOM itself."
								 + " This value is not saved."
								 + Environment.NewLine + Environment.NewLine
								 + "1..100, unity 33, default -1 off";
		}
		#endregion Enter (options)


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


		private string GetPhaseDescription()
		{
			return " Terrain sprites typically cycle through eight phases ("
				 + "sliding doors are static at phase 0 - see #30 isSlidingDoor).";
		}


		/// <summary>
		/// Checks the text of a TextBox for validity. This changes the text if
		/// it's not valid, which causes the OnChanged handler to recurse. It
		/// trims the string and disallows any superfluous preceeding zeros.
		/// Non-numeric text will be checked afterward in the OnChanged handler
		/// itself.
		/// </summary>
		/// <param name="tb">a TextBox to check the text of</param>
		/// <returns>tru if valid</returns>
		private bool TryParseText(Control tb)
		{
			string text = tb.Text.Trim();
			if (text.Length > 1)
			{
				while (text.StartsWith("0", StringComparison.InvariantCulture))
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
		/// #0 phase 0 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged0(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb0_phase0)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb0_phase0.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite1 = (byte)result;
						if (Spriteset != null && result < Spriteset.Count)
						{
							Parts[SelId].Anisprites[0] = Spriteset[result];
						}
						else
							Parts[SelId].Anisprites[0] = null;

						PartsPanel.Invalidate();
						pnl_Sprites.Invalidate();
					}
					else
						tb0_phase0.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb0_phase0.Text = String.Empty; // recurse.
		}
		private void OnEnter0(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 0 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox0(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb0_phase0.Text, out result))
			{
				tssl_Overvalue.Text = "phase 0: " + result;
				OnEnter0(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #1 phase 1 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged1(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb1_phase1)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb1_phase1.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite2 = (byte)result;
						if (Spriteset != null && result < Spriteset.Count)
						{
							Parts[SelId].Anisprites[1] = Spriteset[result];
						}
						else
							Parts[SelId].Anisprites[1] = null;

						PartsPanel.Invalidate();
						pnl_Sprites.Invalidate();
					}
					else
						tb1_phase1.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb1_phase1.Text = String.Empty; // recurse.
		}
		private void OnEnter1(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 1 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox1(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb1_phase1.Text, out result))
			{
				tssl_Overvalue.Text = "phase 1: " + result;
				OnEnter1(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #2 phase 2 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged2(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb2_phase2)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb2_phase2.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite3 = (byte)result;
						if (Spriteset != null && result < Spriteset.Count)
						{
							Parts[SelId].Anisprites[2] = Spriteset[result];
						}
						else
							Parts[SelId].Anisprites[2] = null;

						PartsPanel.Invalidate();
						pnl_Sprites.Invalidate();
					}
					else
						tb2_phase2.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb2_phase2.Text = String.Empty; // recurse.
		}
		private void OnEnter2(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 2 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox2(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb2_phase2.Text, out result))
			{
				tssl_Overvalue.Text = "phase 2: " + result;
				OnEnter2(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #3 phase 3 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged3(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb3_phase3)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb3_phase3.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite4 = (byte)result;
						if (Spriteset != null && result < Spriteset.Count)
						{
							Parts[SelId].Anisprites[3] = Spriteset[result];
						}
						else
							Parts[SelId].Anisprites[3] = null;

						PartsPanel.Invalidate();
						pnl_Sprites.Invalidate();
					}
					else
						tb3_phase3.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb3_phase3.Text = String.Empty; // recurse.
		}
		private void OnEnter3(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 3 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox3(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb3_phase3.Text, out result))
			{
				tssl_Overvalue.Text = "phase 3: " + result;
				OnEnter3(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #4 phase 4 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged4(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb4_phase4)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb4_phase4.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite5 = (byte)result;
						if (Spriteset != null && result < Spriteset.Count)
						{
							Parts[SelId].Anisprites[4] = Spriteset[result];
						}
						else
							Parts[SelId].Anisprites[4] = null;

						PartsPanel.Invalidate();
						pnl_Sprites.Invalidate();
					}
					else
						tb4_phase4.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb4_phase4.Text = String.Empty; // recurse.
		}
		private void OnEnter4(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 4 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox4(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb4_phase4.Text, out result))
			{
				tssl_Overvalue.Text = "phase 4: " + result;
				OnEnter4(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #5 phase 5 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged5(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb5_phase5)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb5_phase5.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite6 = (byte)result;
						if (Spriteset != null && result < Spriteset.Count)
						{
							Parts[SelId].Anisprites[5] = Spriteset[result];
						}
						else
							Parts[SelId].Anisprites[5] = null;

						PartsPanel.Invalidate();
						pnl_Sprites.Invalidate();
					}
					else
						tb5_phase5.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb5_phase5.Text = String.Empty; // recurse.
		}
		private void OnEnter5(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 5 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox5(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb5_phase5.Text, out result))
			{
				tssl_Overvalue.Text = "phase 5: " + result;
				OnEnter5(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #6 phase 6 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged6(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb6_phase6)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb6_phase6.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite7 = (byte)result;
						if (Spriteset != null && result < Spriteset.Count)
						{
							Parts[SelId].Anisprites[6] = Spriteset[result];
						}
						else
							Parts[SelId].Anisprites[6] = null;

						PartsPanel.Invalidate();
						pnl_Sprites.Invalidate();
					}
					else
						tb6_phase6.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb6_phase6.Text = String.Empty; // recurse.
		}
		private void OnEnter6(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 6 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox6(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb6_phase6.Text, out result))
			{
				tssl_Overvalue.Text = "phase 6: " + result;
				OnEnter6(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #7 phase 7 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged7(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb7_phase7)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb7_phase7.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (Spriteset == null || result < Spriteset.Count))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Sprite8 = (byte)result;
						if (Spriteset != null && result < Spriteset.Count)
						{
							Parts[SelId].Anisprites[7] = Spriteset[result];
						}
						else
							Parts[SelId].Anisprites[7] = null;

						PartsPanel.Invalidate();
						pnl_Sprites.Invalidate();
					}
					else
						tb7_phase7.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb7_phase7.Text = String.Empty; // recurse.
		}
		private void OnEnter7(object sender, EventArgs e)
		{
			lbl_Description.Text = "phase 7 (ubyte)" + GetPhaseDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Spriteset != null ? (Spriteset.Count - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox7(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb7_phase7.Text, out result))
			{
				tssl_Overvalue.Text = "phase 7: " + result;
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
		/// #8 loft 00 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged8(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb8_loft00)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb8_loft00.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft1 = (byte)result;
						pnl_Loft08.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb8_loft00.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb8_loft00.Text = String.Empty; // recurse.
		}
		private void OnEnter8(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 00 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox8(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb8_loft00.Text, out result))
			{
				tssl_Overvalue.Text = "loft 00: " + result;
				OnEnter8(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #9 loft 02 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged9(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb9_loft02)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb9_loft02.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft2 = (byte)result;
						pnl_Loft09.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb9_loft02.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb9_loft02.Text = String.Empty; // recurse.
		}
		private void OnEnter9(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 02 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox9(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb9_loft02.Text, out result))
			{
				tssl_Overvalue.Text = "loft 02: " + result;
				OnEnter9(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #10 loft 04 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged10(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb10_loft04)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb10_loft04.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft3 = (byte)result;
						pnl_Loft10.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb10_loft04.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb10_loft04.Text = String.Empty; // recurse.
		}
		private void OnEnter10(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 04 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox10(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb10_loft04.Text, out result))
			{
				tssl_Overvalue.Text = "loft 04: " + result;
				OnEnter10(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #11 loft 06 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged11(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb11_loft06)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb11_loft06.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft4 = (byte)result;
						pnl_Loft11.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb11_loft06.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb11_loft06.Text = String.Empty; // recurse.
		}
		private void OnEnter11(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 06 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox11(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb11_loft06.Text, out result))
			{
				tssl_Overvalue.Text = "loft 06: " + result;
				OnEnter11(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #12 loft 08 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged12(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb12_loft08)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb12_loft08.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft5 = (byte)result;
						pnl_Loft12.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb12_loft08.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb12_loft08.Text = String.Empty; // recurse.
		}
		private void OnEnter12(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 08 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox12(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb12_loft08.Text, out result))
			{
				tssl_Overvalue.Text = "loft 08: " + result;
				OnEnter12(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #13 loft 10 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged13(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb13_loft10)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb13_loft10.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft6 = (byte)result;
						pnl_Loft13.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb13_loft10.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb13_loft10.Text = String.Empty; // recurse.
		}
		private void OnEnter13(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 10 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox13(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb13_loft10.Text, out result))
			{
				tssl_Overvalue.Text = "loft 10: " + result;
				OnEnter13(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #14 loft 12 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged14(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb14_loft12)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb14_loft12.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft7 = (byte)result;
						pnl_Loft14.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb14_loft12.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb14_loft12.Text = String.Empty; // recurse.
		}
		private void OnEnter14(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 12 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox14(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb14_loft12.Text, out result))
			{
				tssl_Overvalue.Text = "loft 12: " + result;
				OnEnter14(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #15 loft 14 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged15(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb15_loft14)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb15_loft14.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft8 = (byte)result;
						pnl_Loft15.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb15_loft14.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb15_loft14.Text = String.Empty; // recurse.
		}
		private void OnEnter15(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 14 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox15(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb15_loft14.Text, out result))
			{
				tssl_Overvalue.Text = "loft 14: " + result;
				OnEnter15(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #16 loft 16 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged16(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb16_loft16)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb16_loft16.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft9 = (byte)result;
						pnl_Loft16.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb16_loft16.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb16_loft16.Text = String.Empty; // recurse.
		}
		private void OnEnter16(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 16 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox16(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb16_loft16.Text, out result))
			{
				tssl_Overvalue.Text = "loft 16: " + result;
				OnEnter16(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #17 loft 18 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged17(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb17_loft18)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb17_loft18.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft10 = (byte)result;
						pnl_Loft17.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb17_loft18.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb17_loft18.Text = String.Empty; // recurse.
		}
		private void OnEnter17(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 18 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox17(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb17_loft18.Text, out result))
			{
				tssl_Overvalue.Text = "loft 18: " + result;
				OnEnter17(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #18 loft 20 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged18(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb18_loft20)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb18_loft20.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft11 = (byte)result;
						pnl_Loft18.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb18_loft20.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb18_loft20.Text = String.Empty; // recurse.
		}
		private void OnEnter18(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 20 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox18(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb18_loft20.Text, out result))
			{
				tssl_Overvalue.Text = "loft 20: " + result;
				OnEnter18(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// #19 loft 22 (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged19(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				if (TryParseText(tb19_loft22)) // else recurse
				{
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb19_loft22.Text, out result)
						&&     ((strict && result > -1 && result < 256 && (LoFT == null || result < LoFT.Length / 256))
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Loft12 = (byte)result;
						pnl_Loft19.Invalidate();
						pnl_IsoLoft.Invalidate();
					}
					else
						tb19_loft22.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb19_loft22.Text = String.Empty; // recurse.
		}
		private void OnEnter19(object sender, EventArgs e)
		{
			lbl_Description.Text = "loft 22 (ubyte)" + GetLoftDescription()
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (LoFT != null ? (LoFT.Length / 256 - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox19(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb19_loft22.Text, out result))
			{
				tssl_Overvalue.Text = "loft 22: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb20_scang1.Text, out result)
						&&     ((strict && result > 34 && result < 65571 && (ScanG == null || result < ScanG.Length / 16))
							|| (!strict && result > 34 && result < 65571)))
					{
						Parts[SelId].Record.ScanG         = (ushort)(result);
						Parts[SelId].Record.ScanG_reduced = (ushort)(result - 35);

						tb20_scang2.Text = (result - 35).ToString();
						pnl_ScanGic.Invalidate();
					}
					else
						tb20_scang1.Text = "35"; // recurse w/ default.
				}
			}
			else
				tb20_scang1.Text = String.Empty; // recurse.
		}
		private void OnEnter20(object sender, EventArgs e)
		{
			lbl_Description.Text = "ScanG (unsigned short) + 35"
								 + Environment.NewLine + Environment.NewLine
								 + "35.." + (ScanG != null ? (ScanG.Length / 16 - 1).ToString() : "65570");
		}
		private void OnMouseEnterTextbox20(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb20_scang1.Text, out result))
			{
				tssl_Overvalue.Text = "ScanG: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb20_scang2.Text, out result)
						&&     ((strict && result > -1 && result < 65536 && (ScanG == null || result < ScanG.Length / 16 - 35))
							|| (!strict && result > -1 && result < 65536)))
					{
						//LogFile.WriteLine("\nOnChanged20r() text2= " + tb20_scang2.Text + " result= " + result);
						Parts[SelId].Record.ScanG         = (ushort)(result + 35);
						Parts[SelId].Record.ScanG_reduced = (ushort)(result);
						//LogFile.WriteLine(". ScanG= "   + Parts[SelId].Record.ScanG);
						//LogFile.WriteLine(". ScanG_r= " + Parts[SelId].Record.ScanG_reduced);

						tb20_scang1.Text = (result + 35).ToString();
						//LogFile.WriteLine(". . text1= " + tb20_scang1.Text);
						pnl_ScanGic.Invalidate();
					}
					else
						tb20_scang2.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb20_scang2.Text = String.Empty; // recurse.
		}
		private void OnEnter20r(object sender, EventArgs e)
		{
			lbl_Description.Text = "ScanG_reduced (unsigned short)"
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (ScanG != null ? (ScanG.Length / 16 - 36).ToString() : "65535");
		}
		private void OnMouseEnterTextbox20r(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb20_scang2.Text, out result))
			{
				tssl_Overvalue.Text = "ScanG_reduced: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb22_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown22 = (byte)result;
					}
					else
						tb22_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "22: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb23_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown23 = (byte)result;
					}
					else
						tb23_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "23: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb24_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown24 = (byte)result;
					}
					else
						tb24_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "24: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb25_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown25 = (byte)result;
					}
					else
						tb25_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "25: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb26_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown26 = (byte)result;
					}
					else
						tb26_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "26: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb27_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown27 = (byte)result;
					}
					else
						tb27_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "27: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb28_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown28 = (byte)result;
					}
					else
						tb28_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "28: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb29_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown29 = (byte)result;
					}
					else
						tb29_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "29: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb30_isslidingdoor.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.SlidingDoor = Convert.ToBoolean(result);
					}
					else
						tb30_isslidingdoor.Text = "0"; // recurse w/ default.
				}
			}
			else
				tb30_isslidingdoor.Text = String.Empty; // recurse.
		}
		private void OnEnter30(object sender, EventArgs e)
		{
			lbl_Description.Text = "isSlidingDoor (bool) is a true/false value that is relevant only to"
								 + " westwall and northwall parts. Such a part will iterate through the"
								 + " phases of its animation sprites while opening (see #0..7 phase 0..7);"
								 + " only the first four phases are iterated if the part is designated"
								 + " as an EntryPoint (see #59 SpecialType) otherwise all eight phases"
								 + " are iterated. A sliding door displays its phase 7 sprite while open"
								 + " (although it could be replaced by its #46 AlternateId depending on the XCOM build),"
								 + " then at the end of the turn it closes and reverts to phase 0."
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
				tssl_Overvalue.Text = "isSlidingDoor: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb31_isblocklos.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.StopLOS = Convert.ToBoolean(result);
					}
					else
						tb31_isblocklos.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "isBlockLoS: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb32_isdropthrou.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.NotFloored = Convert.ToBoolean(result);
					}
					else
						tb32_isdropthrou.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "isDropThrou: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb33_isbigwall.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.BigWall = Convert.ToBoolean(result);
					}
					else
						tb33_isbigwall.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "isBigWall: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb34_isgravlift.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.GravLift = Convert.ToBoolean(result);
					}
					else
						tb34_isgravlift.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "isGravLift: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb35_ishingeddoor.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.HingedDoor = Convert.ToBoolean(result);
					}
					else
						tb35_ishingeddoor.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "isHingedDoor: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb36_isblockfire.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.BlockFire = Convert.ToBoolean(result);
					}
					else
						tb36_isblockfire.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "isBlockFire: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb37_isblocksmoke.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.BlockSmoke = Convert.ToBoolean(result);
					}
					else
						tb37_isblocksmoke.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "isBlockSmoke: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb38_.Text, out result)
						&&     ((strict && result == 3)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.LeftRightHalf = (byte)result;
					}
					else
						tb38_.Text = "3"; // recurse w/ default.
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
				tssl_Overvalue.Text = "LeftRightHalf: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb39_tuwalk.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.TU_Walk = (byte)result;
					}
					else if (Parts[SelId].Record.PartType == PartType.Floor)
					{
						tb39_tuwalk.Text = "4"; // recurse w/ default.
					}
					else
						tb39_tuwalk.Text = "255"; // recurse w/ default.
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
				tssl_Overvalue.Text = "TuWalk: " + result + (result == 255 ? " impassable" : String.Empty);
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb40_tuslide.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.TU_Slide = (byte)result;
					}
					else if (Parts[SelId].Record.PartType == PartType.Floor)
					{
						tb40_tuslide.Text = "4"; // recurse w/ default.
					}
					else
						tb40_tuslide.Text = "255"; // recurse w/ default.
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
				tssl_Overvalue.Text = "TuSlide: " + result + (result == 255 ? " impassable" : String.Empty);
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb41_tufly.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.TU_Fly = (byte)result;
					}
					else if (Parts[SelId].Record.PartType == PartType.Floor)
					{
						tb41_tufly.Text = "4"; // recurse w/ default.
					}
					else
						tb41_tufly.Text = "255"; // recurse w/ default.
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
				tssl_Overvalue.Text = "TuFly: " + result + (result == 255 ? " impassable" : String.Empty);
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb42_armor.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Armor = (byte)result;
					}
					else
						tb42_armor.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "Armor: " + result + (result == 255 ? " indestructible" : String.Empty);
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb43_heblock.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.HE_Block = (byte)result;
					}
					else
						tb43_heblock.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "HeBlock: " + result;
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
					Changed |= !InitFields;

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
					}
					else
						tb44_deathid.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "DeathId: " + result + (result == 0 ? " none" : String.Empty);
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb45_fireresist.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Flammable = (byte)result;
					}
					else
						tb45_fireresist.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "FireResist: " + result + (result == 255 ? " impervious" : String.Empty);
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb46_alternateid.Text, out result)
						&&     ((strict && result > -1 && result < 256 && result < Parts.Length) // NOTE: 'Parts' shall not be null here.
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Alt_MCD = (byte)result;

						if (result != 0 && result < Parts.Length)
							Parts[SelId].Alternate = Parts[result];
						else
							Parts[SelId].Alternate = null;

						PartsPanel.Invalidate();
					}
					else
						tb46_alternateid.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "AlternateId: " + result + (result == 0 ? " none" : String.Empty);
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb47_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown47 = (byte)result;
					}
					else
						tb47_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "CloseDoors: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb48_terrainoffset.Text, out result)
						&&     ((strict && result > - 25 && result < 1)
							|| (!strict && result > -129 && result < 128)))
					{
						Parts[SelId].Record.StandOffset = (sbyte)result;
					}
					else
						tb48_terrainoffset.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "TerrainOffset: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb49_spriteoffset.Text, out result)
						&&     ((strict && result > -1 && result < 25)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.TileOffset = (byte)result;
						PartsPanel.Invalidate();
						pnl_Sprites.Invalidate();
					}
					else
						tb49_spriteoffset.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "SpriteOffset: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb50_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown50 = (byte)result;
					}
					else
						tb50_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "dTypeMod: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb51_lightblock.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.LightBlock = (byte)result;
					}
					else
						tb51_lightblock.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "LightBlock: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb52_footsound.Text, out result)
						&&     ((strict && result > -1 && result < 7)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Footstep = (byte)result;
					}
					else
						tb52_footsound.Text = "0"; // recurse w/ default.
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
								 + " differ depending on your XCOM version/edition."
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
				tssl_Overvalue.Text = "FootSound: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb53_parttype.Text, out result)
						&&     ((strict && result > -1 && result < 4)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.PartType = (PartType)result;	// NOTE: Assigning integers that are not
					}														// explicitly defined in the enum is allowed.
					else
						tb53_parttype.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "PartType: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb54_hetype.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.HE_Type = (byte)result;
					}
					else
						tb54_hetype.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "HeType: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb55_hestrength.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.HE_Strength = (byte)result;
					}
					else
						tb55_hestrength.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "HeStrength: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb56_smokeblock.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.SmokeBlockage = (byte)result;
					}
					else
						tb56_smokeblock.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "SmokeBlock: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb57_fuel.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Fuel = (byte)result;
					}
					else
						tb57_fuel.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "Fuel: " + result;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb58_lightintensity.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.LightSource = (byte)result;
					}
					else
						tb58_lightintensity.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "LightIntensity: " + result + (result == 0 ? " none" : String.Empty);
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb59_specialtype.Text, out result)
						&&     ((strict && result > -1 && result < 15)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Special = (SpecialType)result;	// NOTE: Assigning integers that are not
					}															// explicitly defined in the enum is allowed.
					else
						tb59_specialtype.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "SpecialType: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb60_isbaseobject.Text, out result)
						&&     ((strict && result > -1 && result < 2)
							|| (!strict && result > -1 && result < 2)))
					{
						Parts[SelId].Record.BaseObject = Convert.ToBoolean(result);
					}
					else
						tb60_isbaseobject.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "isBaseObject: " + text;
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
					Changed |= !InitFields;

					int result;
					if (Int32.TryParse(tb61_.Text, out result)
						&&     ((strict && result > -1 && result < 256)
							|| (!strict && result > -1 && result < 256)))
					{
						Parts[SelId].Record.Unknown61 = (byte)result;
					}
					else
						tb61_.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "VictoryPoints: " + result;
				OnEnter61(null, EventArgs.Empty);
			}
		}
		#endregion Changed events
	}
}
