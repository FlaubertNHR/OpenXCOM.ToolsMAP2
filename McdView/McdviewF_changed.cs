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

			if (label == lbl38 || label == lbl38_leftrighthalf)
				return tb38_leftrighthalf;

			if (label == lbl53 || label == lbl53_parttype)
				return tb53_parttype;

			if (label == lbl59 || label == lbl59_specialtype)
				return tb59_specialtype;

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
		/// #38 LeftRightHalf (byte)
		/// @note aka StartPhase
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged38(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				Changed = true;

				int result;
				if (Int32.TryParse(tb38_leftrighthalf.Text, out result)
					&&     ((strict && result == 3)
						|| (!strict && result > -1 && result < 256)))
				{
					Records[SelId].Record.StartPhase = (byte)result;
				}
				else
					tb38_leftrighthalf.Text = "3"; // recurse w/ default.
			}
			else
				tb38_leftrighthalf.Text = String.Empty; // recurse.
		}
		private void OnEnter38(object sender, EventArgs e)
		{
			lbl_Description.Text = "Always 3. Defined as \"printype\"; 3=whole, 2=right side,"
								 + " 1=left side. Since this is always 3 the game prints the"
								 + " entire object, while the assumption is that values of 2 or 1"
								 + " are possibly used for the map designer. But the actual game"
								 + " totally ignores this value.";
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
		/// #53 PartType (byte/PartType)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged53(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				Changed = true;

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
			lbl_Description.Text = "0 Floor, 1 Westwall, 2 Northwall, 3 Content";
		}
		private void OnMouseEnterTextbox53(object sender, EventArgs e)
		{
			int result;
			if (Int32.TryParse(tb53_parttype.Text, out result))
			{
				string text;
				switch (result)
				{
					case 0: text = "0 Floor";     break;
					case 1: text = "1 Westwall";  break;
					case 2: text = "2 Northwall"; break;
					case 3: text = "3 Content";   break;

					default: text = result.ToString(); break;
				}
				lbl_Description.Text = text;
			}
		}

		/// <summary>
		/// #59 SpecialType (sbyte/SpecialType)
		/// @note aka TargetType
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged59(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				Changed = true;

				int result;
				if (Int32.TryParse(tb59_specialtype.Text, out result)
					&&     ((strict && result > -1 && result < 15)
						|| (!strict && result > -129 && result < 128)))
				{
					Records[SelId].Record.TargetType = (SpecialType)result;	// NOTE: Assigning integers that are not
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
			lbl_Description.Text = "Special properties of the part. Determines where XCOM agents can"
								 + " enter and exit a Map, or what is salvaged at the end of tactical."
								 + " (UFO/TFTD)" + Environment.NewLine
								 + "0\u00A0None, 1\u00A0EntryPoint, 2\u00A0PowerSource/IonBeamAccelerators,"
								 + " 3\u00A0Navigation, 4\u00A0Construction, 5\u00A0Food/Cryogenics,"
								 + " 6\u00A0Reproduction/Cloning, 7\u00A0Entertainment/LearningArrays,"
								 + " 8\u00A0Surgery/Implanter, 9\u00A0Examination, 10\u00A0Alloys/Plastics,"
								 + " 11\u00A0Habitat/Reanimation, 12\u00A0RuinedAlloys/Plastics, 13\u00A0ExitPoint,"
								 + " 14\u00A0MustDestroyToSucceed"; //AlienBrain/T'lethPowerCylinders
			// TODO: Conform MapView2 to the proper descriptions.
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
		#endregion Changed events
	}
}
