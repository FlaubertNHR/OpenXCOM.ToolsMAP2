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

			if (label == lbl22 || label == lbl22_)               return tb22_;
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
			if (label == lbl38 || label == lbl38_leftrighthalf)  return tb38_leftrighthalf;
			if (label == lbl39 || label == lbl39_tuwalk)         return tb39_tuwalk;
			if (label == lbl40 || label == lbl40_tuslide)        return tb40_tuslide;
			if (label == lbl41 || label == lbl41_tufly)          return tb41_tufly;
			if (label == lbl42 || label == lbl42_armor)          return tb42_armor;
			if (label == lbl43 || label == lbl43_heblock)        return tb43_heblock;
			if (label == lbl44 || label == lbl44_deathid)        return tb44_deathid;
			if (label == lbl45 || label == lbl45_fireresist)     return tb45_fireresist;
			if (label == lbl46 || label == lbl46_alternateid)    return tb46_alternateid;

			if (label == lbl47 || label == lbl47_)               return tb47_;

			if (label == lbl48 || label == lbl48_terrainoffset)  return tb48_terrainoffset;
			if (label == lbl49 || label == lbl49_spriteoffset)   return tb49_spriteoffset;

			if (label == lbl50 || label == lbl50_)               return tb50_;

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

			if (label == lbl61 || label == lbl61_)               return tb61_;

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
			lbl_Description.Text = "Checked enforces valid values in XCOM."
								 + " Unchecked allows values outside what's expected (for expert experts only"
								 + " - ie people who code their own XCOM executable and require extended values)."
								 + " This value is not saved."
								 + Environment.NewLine + Environment.NewLine
								 + "default checked";
		}

		private void OnEnterSpriteShade(object sender, EventArgs e)
		{
			lbl_Description.Text = "SpriteShade is an inverse gamma-value for sprites drawn in this app."
								 + " It has nothing to do with palette-based sprite-shading in XCOM itself."
								 + " This value is not saved."
								 + Environment.NewLine + Environment.NewLine
								 + "1..100, unity 33, default -1 off";
		}
		#endregion Enter (options)


		#region Changed events
		// TODO: If value is outside of strict-bounds color its text red.

		/// <summary>
		/// #30 isSlidingDoor (bool)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged30(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb30_isslidingdoor.Text, out result)
					&&     ((strict && result > -1 && result < 2)
						|| (!strict && result > -1 && result < 2)))
				{
					Records[SelId].Record.SlidingDoor = Convert.ToBoolean(result);
				}
				else
					tb30_isslidingdoor.Text = "0"; // recurse w/ default.
			}
			else
				tb30_isslidingdoor.Text = String.Empty; // recurse.
		}
		private void OnEnter30(object sender, EventArgs e)
		{
			lbl_Description.Text = "isSlidingDoor (bool) is a true/false value that is relevant only to"
								 + " westwall and northwall parts. Such a part will cycle through its"
								 + " first four animation sprites when opened, then the part will be"
								 + " replaced by the part designated as its AlternateId; at the end"
								 + " of the turn it will revert to the orginal part. Note that"
								 + " specifying a part as both isSlidingDoor and isHingedDoor might have"
								 + " an unpredictable effect. See #46 AlternateId."
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
		/// #33 isBigWall (bool)
		/// TODO: Allow the bigwall value to be stored as a byte.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged33(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb33_isbigwall.Text, out result)
					&&     ((strict && result > -1 && result < 2)
						|| (!strict && result > -1 && result < 2)))
				{
					Records[SelId].Record.BigWall = Convert.ToBoolean(result);
				}
				else
					tb33_isbigwall.Text = "0"; // recurse w/ default.
			}
			else
				tb33_isbigwall.Text = String.Empty; // recurse.
		}
		private void OnEnter33(object sender, EventArgs e)
		{
/*			var tb = sender as TextBox; // uh, mouseclick position overrules SelectAll()
			if (tb != null)
			{
				tb.SelectAll();
//				tb.SelectionStart = 0;
//				tb.SelectionLength = tb.Text.Length;
			} */

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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb34_isgravlift.Text, out result)
					&&     ((strict && result > -1 && result < 2)
						|| (!strict && result > -1 && result < 2)))
				{
					Records[SelId].Record.GravLift = Convert.ToBoolean(result);
				}
				else
					tb34_isgravlift.Text = "0"; // recurse w/ default.
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb35_ishingeddoor.Text, out result)
					&&     ((strict && result > -1 && result < 2)
						|| (!strict && result > -1 && result < 2)))
				{
					Records[SelId].Record.HingedDoor = Convert.ToBoolean(result);
				}
				else
					tb35_ishingeddoor.Text = "0"; // recurse w/ default.
			}
			else
				tb35_ishingeddoor.Text = String.Empty; // recurse.
		}
		private void OnEnter35(object sender, EventArgs e)
		{
			lbl_Description.Text = "isHingedDoor (bool) is a true/false value that is relevant only to"
								 + " westwall and northwall parts. Such a part will be replaced by the"
								 + " part designated as its AlternateId when opened. Note that"
								 + " specifying a part as both isHingedDoor and isSlidingDoor might have"
								 + " an unpredictable effect. See #46 AlternateId."
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
		/// #38 LeftRightHalf (byte)
		/// @note aka StartPhase
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged38(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb38_leftrighthalf.Text, out result)
					&&     ((strict && result == 3)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.LeftRightHalf = (byte)result;
				}
				else
					tb38_leftrighthalf.Text = "3"; // recurse w/ default.
			}
			else
				tb38_leftrighthalf.Text = String.Empty; // recurse.
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
			if (Int32.TryParse(tb38_leftrighthalf.Text, out result))
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb39_tuwalk.Text, out result)
					&&     ((strict && result > -1 && result < 256)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.TU_Walk = (byte)result;
				}
				else
					tb39_tuwalk.Text = "4"; // recurse w/ default.
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
				tssl_Overvalue.Text = "TuWalk: " + result;
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb40_tuslide.Text, out result)
					&&     ((strict && result > -1 && result < 256)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.TU_Slide = (byte)result;
				}
				else
					tb40_tuslide.Text = "4"; // recurse w/ default.
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
				tssl_Overvalue.Text = "TuSlide: " + result;
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb41_tufly.Text, out result)
					&&     ((strict && result > -1 && result < 256)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.TU_Fly = (byte)result;
				}
				else
					tb41_tufly.Text = "4"; // recurse w/ default.
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
				tssl_Overvalue.Text = "TuFly: " + result;
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb42_armor.Text, out result)
					&&     ((strict && result > -1 && result < 256)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.Armor = (byte)result;
				}
				else
					tb42_armor.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "Armor: " + result;
				OnEnter42(null, EventArgs.Empty);
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb44_deathid.Text, out result)
					&&     ((strict && result > -1 && result < 256 && result < Records.Length)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.DieTile = (byte)result;

					if (result != 0 && result < Records.Length)
						Records[SelId].Dead = Records[result];
					else
						Records[SelId].Dead = null;

					RecordPanel.Invalidate();
				}
				else
					tb44_deathid.Text = "0"; // recurse w/ default.
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
								 + "0.." + (Records != null ? (Records.Length - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox44(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb44_deathid.Text, out result))
			{
				tssl_Overvalue.Text = "DeathId: " + result;
				OnEnter44(null, EventArgs.Empty);
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb46_alternateid.Text, out result)
					&&     ((strict && result > -1 && result < 256 && result < Records.Length)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.Alt_MCD = (byte)result;

					if (result != 0 && result < Records.Length)
						Records[SelId].Alternate = Records[result];
					else
						Records[SelId].Alternate = null;

					RecordPanel.Invalidate();
				}
				else
					tb46_alternateid.Text = "0"; // recurse w/ default.
			}
			else
				tb46_alternateid.Text = String.Empty; // recurse.
		}
		private void OnEnter46(object sender, EventArgs e)
		{
			lbl_Description.Text = "AlternateId (ubyte) is the ID of the part that will replace a"
								 + " part that is a door and has been opened. The alternate-part"
								 + " should be in the MCD file with the original part. A value of"
								 + " 0 indicates that if a part is opened it will not be replaced"
								 + " by an alternate-part; that is, a value of 0 does not"
								 + " reference ID #0. See #30 isSlidingDoor and #35 isHingedDoor."
								 + Environment.NewLine + Environment.NewLine
								 + "0.." + (Records != null ? (Records.Length - 1).ToString() : "255");
		}
		private void OnMouseEnterTextbox46(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb46_alternateid.Text, out result))
			{
				tssl_Overvalue.Text = "AlternateId: " + result;
				OnEnter46(null, EventArgs.Empty);
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb48_terrainoffset.Text, out result)
					&&     ((strict && result > - 25 && result < 1)
						|| (!strict && result > -129 && result < 128)))
				{
					Records[SelId].Record.StandOffset = (sbyte)result;
				}
				else
					tb48_terrainoffset.Text = "0"; // recurse w/ default.
			}
			else
				tb48_terrainoffset.Text = String.Empty; // recurse.
		}
		private void OnEnter48(object sender, EventArgs e)
		{
			lbl_Description.Text = "TerrainOffset (sbyte) is the distance in voxels between the height of"
								 + " a tile's lowest voxel-level and the height that objects such"
								 + " as units and items (possibly including smoke and fire graphics)"
								 + " should appear at and be considered to have their bottom voxel"
								 + " positioned at (if applicable). Note that a negative value"
								 + " raises the object and that the standard value of 0 places the"
								 + " object at the floor-level of a tile. This variable has"
								 + " relevance only for floor and content parts; the TerrainOffset"
								 + " of westwall or northwall parts is not evaluated. Also note that"
								 + " for a unit to step from one tile to another their respective"
								 + " terrain-heights can be no greater than 8 voxels and that the"
								 + " total distance between levels on a Map is 24 voxels."
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb49_spriteoffset.Text, out result)
					&&     ((strict && result > -1 && result < 41)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.TileOffset = (byte)result;
					RecordPanel.Invalidate();
					pnl_Sprites.Invalidate();
				}
				else
					tb49_spriteoffset.Text = "0"; // recurse w/ default.
			}
			else
				tb49_spriteoffset.Text = String.Empty; // recurse.
		}
		private void OnEnter49(object sender, EventArgs e)
		{
			lbl_Description.Text = "SpriteOffset (ubyte) is the vertical distance in pixels between"
								 + " the position a part's sprite is usually drawn at and"
								 + " the position at which it will actually be drawn. Note"
								 + " that a positive value raises the sprite and that the"
								 + " standard value is 0."
								 + Environment.NewLine + Environment.NewLine
								 + "0..40";
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
		/// #53 PartType (byte/PartType)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged53(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb53_parttype.Text, out result)
					&&     ((strict && result > -1 && result < 4)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.PartType = (PartType)result;	// NOTE: Assigning integers that are not
				}														// explicitly defined in the enum is allowed.
				else
					tb53_parttype.Text = "0"; // recurse w/ default.
			}
			else
				tb53_parttype.Text = String.Empty; // recurse.
		}
		private void OnEnter53(object sender, EventArgs e)
		{
			lbl_Description.Text = "PartType (ubyte) specifies the tile-slot in which a tile-part should"
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
		/// #58 LightIntensity (byte)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged58(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb58_lightintensity.Text, out result)
					&&     ((strict && result > -1 && result < 256)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.LightSource = (byte)result;
				}
				else
					tb58_lightintensity.Text = "0"; // recurse w/ default.
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
				tssl_Overvalue.Text = "LightIntensity: " + result;
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb59_specialtype.Text, out result)
					&&     ((strict && result > -1 && result < 15)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.Special = (SpecialType)result;	// NOTE: Assigning integers that are not
				}															// explicitly defined in the enum is allowed.
				else
					tb59_specialtype.Text = "0"; // recurse w/ default.
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
				Changed |= !InitFields;

				int result;
				if (Int32.TryParse(tb60_isbaseobject.Text, out result)
					&&     ((strict && result > -1 && result < 2)
						|| (!strict && result > -1 && result < 2)))
				{
					Records[SelId].Record.BaseObject = Convert.ToBoolean(result);
				}
				else
					tb60_isbaseobject.Text = "0"; // recurse w/ default.
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
		#endregion Changed events
	}
}
