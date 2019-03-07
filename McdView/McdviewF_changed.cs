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
			if (label == lbl_SpriteShade)
				return tb_SpriteShade;

			if (label == lbl33 || label == lbl33_isbigwall)
				return tb33_isbigwall;

			if (label == lbl34 || label == lbl34_isgravlift)
				return tb34_isgravlift;

			if (label == lbl38 || label == lbl38_leftrighthalf)
				return tb38_leftrighthalf;

			if (label == lbl48 || label == lbl48_terrainoffset)
				return tb48_terrainoffset;

			if (label == lbl49 || label == lbl49_spriteoffset)
				return tb49_spriteoffset;

			if (label == lbl53 || label == lbl53_parttype)
				return tb53_parttype;

			if (label == lbl58 || label == lbl58_lightintensity)
				return tb58_lightintensity;

			if (label == lbl59 || label == lbl59_specialtype)
				return tb59_specialtype;

			if (label == lbl60 || label == lbl60_isbaseobject)
				return tb60_isbaseobject;

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
								 + " This value is not saved. (default checked)";
		}

		private void OnEnterSpriteShade(object sender, EventArgs e)
		{
			lbl_Description.Text = "SpriteShade is an inverse gamma-value for sprites drawn in this app."
								 + " It has nothing to do with palette-based sprite-shading in XCOM itself."
								 + " This value is not saved. (1..100, unity 33, default -1 off)";
		}
		#endregion Enter (options)


		#region Changed events
		// TODO: If value is outside of strict-bounds color its text red.

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
			lbl_Description.Text = "isBigwall is a true/false value that is relevant only to"
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
				lbl_Description.Text = text;
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
			lbl_Description.Text = "isGravLift is a true/false value that is relevant only to"
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
				lbl_Description.Text = text;
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
			lbl_Description.Text = "LeftRightHalf is always 3. It is supposed to decide whether to"
								 + " draw the left-half, the right-half, or both halves of a part's"
								 + " sprite but is not used."
								 + Environment.NewLine + Environment.NewLine
								 + "1 Left, 2 Right, 3 Full";
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
				lbl_Description.Text = text;
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
			lbl_Description.Text = "TerrainOffset is the distance in voxels between the height of"
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
								 + " total distance between levels on a Map is 24 voxels.";
		}
		private void OnMouseEnterTextbox48(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb48_terrainoffset.Text, out result))
			{
				lbl_Description.Text = result.ToString();
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
					&&     ((strict && result > -1 && result < 25)
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
			lbl_Description.Text = "SpriteOffset is the vertical distance in pixels between"
								 + " the position a part's sprite is usually drawn at and"
								 + " the position at which it will actually be drawn. Note"
								 + " that a positive value raises the sprite and that the"
								 + " standard value is 0.";
		}
		private void OnMouseEnterTextbox49(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb49_spriteoffset.Text, out result))
			{
				lbl_Description.Text = result.ToString();
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
			lbl_Description.Text = "PartType specifies the tile-slot in which a tile-part should"
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
				lbl_Description.Text = text;
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
			lbl_Description.Text = "LightIntensity controls how brightly a part lights up its surroundings."
								 + " A part with a value of 0 should not emit light although the exact"
								 + " effect depends on the XCOM build.";
		}
		private void OnMouseEnterTextbox58(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb58_lightintensity.Text, out result))
			{
				lbl_Description.Text = result.ToString();
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
			lbl_Description.Text = "SpecialType is the special property of a part. It determines where"
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
				lbl_Description.Text = text;
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
			lbl_Description.Text = "isBaseObject is a true/false value that is relevant only to"
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
				lbl_Description.Text = text;
			}
		}
		#endregion Changed events
	}
}
