using System;
using System.Windows.Forms;


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
			{
				return tb_SpriteShade;
			}
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
								 + " - ie people who code their own executable and require extended values)."
								 + " This value is not saved. (default checked on)";
		}

		private void OnEnterSpriteShade(object sender, EventArgs e)
		{
			lbl_Description.Text = "SpriteShade is an inverse gamma-value for any sprites drawn in McdView."
								 + " It has nothing to do with palette-based sprite-shading in XCOM itself."
								 + " This value is not saved. (1..100, unity 33, default -1 off)";
		}
		#endregion Enter (options)


		#region Changed events
		/// <summary>
		/// @note Unsigned byte
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChanged38(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				int result;
				if (Int32.TryParse(tb38_leftrighthalf.Text, out result)
					&&     ((cb_Strict.Checked && result >  0 && result < 4)
						|| (!cb_Strict.Checked && result > -1 && result < 256)))
				{
					Records[SelId].Record.StartPhase = (byte)result;
				}
				else
					tb38_leftrighthalf.Text = "3";	// recurse w/ default.
			}
			else
				tb38_leftrighthalf.Text = String.Empty; // recurse.
		}
		private void OnEnter38(object sender, EventArgs e)
		{
			lbl_Description.Text = GetDescription38();
		}
		private string GetDescription38()
		{
			return "Always 3. Defined as \"printype\"; 3=whole, 2=right side,"
				+ " 1=left side. Since this is always 3 the game prints the"
				+ " entire object, while the assumption is that values of 2 or 1"
				+ " are possibly used for the map designer. But the actual game"
				+ " totally ignores this value.";
		}

		private void OnChanged53(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				int result;
				if (!Int32.TryParse(tb53_parttype.Text, out result)
					|| result < 0 || result > 3)
				{
					tb53_parttype.Text = "0";	// default.
					return;						// recurse.
				}
				Records[SelId].Record.StartPhase = (byte)result;
			}
			else
				tb53_parttype.Text = String.Empty; // recurse.
		}
		private void OnEnter53(object sender, EventArgs e)
		{
			lbl_Description.Text = GetDescription53();
		}
		private string GetDescription53()
		{
			return "0=floor, 1=westwall, 2=northwall, 3=content";
		}
		#endregion Changed events
	}
}
