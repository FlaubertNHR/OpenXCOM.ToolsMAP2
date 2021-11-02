using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using XCom;


namespace PckView
{
	internal sealed partial class SpriteEditorF
		:
			Form
	{
		#region Fields
		internal readonly PckViewF _f;
		internal readonly PaletteF _fpalette;

		internal int _scaler;
		#endregion Fields


		#region Properties (static)
		internal static EditMode Mode
		{ get; private set; }
		#endregion Properties (static)


		#region Properties
		internal SpritePanel SpritePanel
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal SpriteEditorF(PckViewF f)
		{
			InitializeComponent();

			ss_Status.Padding = new Padding(ss_Status.Padding.Left, // fixed ->
											ss_Status.Padding.Top,
											ss_Status.Padding.Left,
											ss_Status.Padding.Bottom);

			bar_Scale.MouseWheel += trackbar_OnMouseWheel;

			// WORKAROUND: See note in MainViewF cTor.
			MaximumSize = new Size(0,0); // fu.net

			_f = f;

			SpritePanel = new SpritePanel(this);
			Controls.Add(SpritePanel);
			SpritePanel.BringToFront();

			_fpalette = new PaletteF(this);
			_fpalette.FormClosing += OnPaletteFormClosing;

			if (!RegistryInfo.RegisterProperties(this))	// NOTE: Respect only left and top props;
			{											// let OnLoad() deter width and height.
				Left = _f.Left + 20;
				Top  = _f.Top  + 20;
			}

			ss_Status.Renderer = new CustomToolStripRenderer();
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Overrides the <c>Activated</c> handler. Brings
		/// <c><see cref="_fpalette"/></c> to top when this <c>SpriteEditorF</c>
		/// is activated.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			if (_f.Frontal && !PckViewF.BypassActivatedEvent && _fpalette.Visible)
			{
				PckViewF.BypassActivatedEvent = true;

				_fpalette.BringToFront();

				TopMost = true;
				TopMost = false;

				PckViewF.BypassActivatedEvent = false;
			}
		}

		/// <summary>
		/// Overrides the <c>KeyDown</c> handler.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires <c>KeyPreview</c> <c>true</c>.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
			{
				e.Handled = e.SuppressKeyPress = true;
				Close();
			}
		}

		/// <summary>
		/// Overrides the <c>FormClosing</c> handler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				if (!PckViewF.Quit)
				{
					e.Cancel = true;
					Hide();
				}
				else
				{
					RegistryInfo.UpdateRegistry(this);
					SpritePanel.Destroy();
				}
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Sets the *proper* <c>ClientSize</c>.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c>this</c></item>
		/// <item><c>null</c> - <c><see cref="size_click()">size_click()</see></c></item>
		/// <item><c>null</c> - <c><see cref="PckViewF.EnableInterface()">PckViewF.EnableInterface()</see></c></item>
		/// </list></param>
		/// <param name="e"></param>
		internal void OnLoad(object sender, EventArgs e)
		{
			int w;
			switch (_f.SetType)
			{
				default: // Pck,Bigobs
					w = GetScaled(Spriteset.SpriteWidth32 * 10);
					break;

				case SpritesetType.ScanG:
				case SpritesetType.LoFT:
					w = Spriteset.SpriteWidth32 * 10;
					break;
			}

			int h = GetScaled(_f.SpriteHeight * 10)
				  + bar_Scale  .Height
				  + la_EditMode.Height
				  + ss_Status  .Height;

			ClientSize = new Size(w + SpritePanel.Pad,
								  h + SpritePanel.Pad);
		}

		/// <summary>
		/// Sets the scale-factor of the sprite in the panel.
		/// </summary>
		/// <param name="sender"><c><see cref="bar_Scale"/></c></param>
		/// <param name="e"></param>
		private void trackbar_OnScroll(object sender, EventArgs e)
		{
			SpritePanel.ScaleFactor = bar_Scale.Value;
		}

		/// <summary>
		/// Reverses mousewheel direction on the trackbar.
		/// </summary>
		/// <param name="sender"><c><see cref="bar_Scale"/></c></param>
		/// <param name="e"></param>
		private void trackbar_OnMouseWheel(object sender, MouseEventArgs e)
		{
			(e as HandledMouseEventArgs).Handled = true;

			if (e.Delta > 0)
			{
				if (bar_Scale.Value != bar_Scale.Minimum)
					--bar_Scale.Value;
			}
			else if (e.Delta < 0)
			{
				if (bar_Scale.Value != bar_Scale.Maximum)
					++bar_Scale.Value;
			}
			SpritePanel.ScaleFactor = bar_Scale.Value;
		}

		/// <summary>
		/// Locks or enables the sprite for edits.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="la_EditMode"/></c></item>
		/// <item><c>null</c> - <c><see cref="PckViewF()">PckViewF.PckViewF()</see></c></item>
		/// </list></param>
		/// <param name="e"></param>
		internal void OnEditModeMouseClick(object sender, EventArgs e)
		{
			switch (Mode)
			{
				case EditMode.Locked:
					Mode = EditMode.Enabled;
					la_EditMode.Text = EditEnabled;
					la_EditMode.BackColor = Color.AliceBlue;
					break;

				case EditMode.Enabled:
					Mode = EditMode.Locked;
					la_EditMode.Text = EditLocked;
					la_EditMode.BackColor = Control.DefaultBackColor;
					break;
			}
		}


		/// <summary>
		/// Disables the Palette it when editing LoFTs.
		/// </summary>
		/// <param name="sender"><c><see cref="miPaletteMenu"/></c></param>
		/// <param name="e"></param>
		private void popup_Palette(object sender, EventArgs e)
		{
			miPalette.Enabled = _f.SetType != SpritesetType.LoFT;
		}

		/// <summary>
		/// Shows the palette-viewer or brings it to front if already shown.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="miPalette"/></c></item>
		/// <item><c>null</c> - <c><see cref="PckViewF"/>.OnSpriteEditorClick()</c></item>
		/// </list></param>
		/// <param name="e"></param>
		/// <remarks>Has no effect if a LoFTset is loaded.</remarks>
		internal void OnShowPaletteClick(object sender, EventArgs e)
		{
			if (_f.SetType != SpritesetType.LoFT) // don't allow the Palette to show if editing LoFTs
			{
				if (!miPalette.Checked)
				{
					miPalette.Checked = true;
					_fpalette.Show();
				}
				else
					_fpalette.BringToFront();
			}
		}

		/// <summary>
		/// <c><see cref="PaletteF"/> hides rather then closes.</c>
		/// </summary>
		/// <param name="sender"><c><see cref="_fpalette"/></c></param>
		/// <param name="e"></param>
		/// <remarks>This fires after the palette's <c>FormClosing</c> event.</remarks>
		private void OnPaletteFormClosing(object sender, CancelEventArgs e)
		{
			miPalette.Checked = false;
		}

		/// <summary>
		/// Overlays a dark grid on the sprite.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="miGridDark"/></c></item>
		/// <item><c>null</c> - <c><see cref="PckViewF()">PckViewF.PckViewF()</see></c></item>
		/// </list></param>
		/// <param name="e"></param>
		internal void OnGridDarkClick(object sender, EventArgs e)
		{
			if (miGridDark.Checked = !miGridDark.Checked)
			{
				miGridLight.Checked = false;
				SpritePanel.PenGrid = Pens.DarkGray;
			}
			else
				SpritePanel.PenGrid = null;
		}

		/// <summary>
		/// Overlays a light grid on the sprite.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="miGridLight"/></c></item>
		/// <item><c>null</c> - <c><see cref="PckViewF()">PckViewF.PckViewF()</see></c></item>
		/// </list></param>
		/// <param name="e"></param>
		internal void OnGridLightClick(object sender, EventArgs e)
		{
			if (miGridLight.Checked = !miGridLight.Checked)
			{
				miGridDark.Checked = false;
				SpritePanel.PenGrid = Pens.LightGray;
			}
			else
				SpritePanel.PenGrid = null;
		}

		/// <summary>
		/// Gets an <c>int</c> representing the current Grid menu checked.
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		/// <item><c>0</c> - no grid</item>
		/// <item><c>1</c> - dark grid</item>
		/// <item><c>2</c> - light grid</item>
		/// </list>
		/// </returns>
		internal int GetGridConfig()
		{
			if (miGridDark .Checked) return 1;
			if (miGridLight.Checked) return 2;
			return 0;
		}

		/// <summary>
		/// Enables or disables <c><see cref="miMagReset"/> when the
		/// Magnification menu opens.</c>
		/// </summary>
		/// <param name="sender"><c><see cref="miMagMenu"/></c></param>
		/// <param name="e"></param>
		private void popup_MagMenu(object sender, EventArgs e)
		{
			miMagReset.Enabled = !tsddb_Size_0.Checked;
		}

		/// <summary>
		/// Resets the magnification factor to <c>+0</c>.
		/// </summary>
		/// <param name="sender"><c><see cref="miMagReset"/></c></param>
		/// <param name="e"></param>
		private void OnResetMagnification(object sender, EventArgs e)
		{
			if (!tsddb_Size_0.Checked) // needed to reject [Ctrl+r]
				size_click(tsddb_Size_0, EventArgs.Empty);
		}

		/// <summary>
		/// Sets the magnification factor.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c>tsddb_Size_*</c></item>
		/// <item><c>tsddb_Size_*</c> - <c><see cref="SetScale()">SetScale()</see></c></item>
		/// <item><c><see cref="tsddb_Size_0"/></c> - <c><see cref="OnResetMagnification()">OnResetMagnification()</see></c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void size_click(object sender, EventArgs e)
		{
			var it = sender as ToolStripMenuItem;
			if (!it.Checked)
			{
				tsddb_Size.Text = it.Text;

				foreach (var tsmi in tsddb_Size.DropDownItems)
					(tsmi as ToolStripMenuItem).Checked = false;

				if (it == tsddb_Size_0)
				{
					tsddb_Size_0.Checked = true;
					_scaler = 0;
				}
				else if (it == tsddb_Size_1)
				{
					tsddb_Size_1.Checked = true;
					_scaler = 1;
				}
				else if (it == tsddb_Size_2)
				{
					tsddb_Size_2.Checked = true;
					_scaler = 2;
				}
				else if (it == tsddb_Size_3)
				{
					tsddb_Size_3.Checked = true;
					_scaler = 3;
				}
				else if (it == tsddb_Size_4)
				{
					tsddb_Size_4.Checked = true;
					_scaler = 4;
				}
				else if (it == tsddb_Size_5)
				{
					tsddb_Size_5.Checked = true;
					_scaler = 5;
				}
				else if (it == tsddb_Size_6)
				{
					tsddb_Size_6.Checked = true;
					_scaler = 6;
				}
				else if (it == tsddb_Size_7)
				{
					tsddb_Size_7.Checked = true;
					_scaler = 7;
				}
				else if (it == tsddb_Size_8)
				{
					tsddb_Size_8.Checked = true;
					_scaler = 8;
				}
				else if (it == tsddb_Size_9)
				{
					tsddb_Size_9.Checked = true;
					_scaler = 9;
				}
				else // it == tsddb_Size_10
				{
					tsddb_Size_10.Checked = true;
					_scaler = 10;
				}

				OnLoad(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Sets the magnification factor from the configfile.
		/// </summary>
		/// <param name="scale"></param>
		internal void SetScale(int scale)
		{
			ToolStripMenuItem it;
			switch (scale)
			{
				default: it = tsddb_Size_0;  break; // case 0
				case  1: it = tsddb_Size_1;  break;
				case  2: it = tsddb_Size_2;  break;
				case  3: it = tsddb_Size_3;  break;
				case  4: it = tsddb_Size_4;  break;
				case  5: it = tsddb_Size_5;  break;
				case  6: it = tsddb_Size_6;  break;
				case  7: it = tsddb_Size_7;  break;
				case  8: it = tsddb_Size_8;  break;
				case  9: it = tsddb_Size_9;  break;
				case 10: it = tsddb_Size_10; break;
			}
			size_click(it, EventArgs.Empty);
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Prints color to the statusbar.
		/// </summary>
		/// <param name="color"></param>
		internal void PrintPixelColor(string color)
		{
			tssl_ColorInfo.Text = color;
		}

		/// <summary>
		/// Closes the palette-form when PckView closes. This is required only
		/// when PckView opens via MapView.
		/// </summary>
		internal void ClosePalette()
		{
			_fpalette.Close();
		}

		/// <summary>
		/// Gets the length of anything that needs to be scaled by the
		/// magnification factor.
		/// </summary>
		/// <param name="length">a length that needs to be scaled</param>
		/// <returns></returns>
		internal int GetScaled(int length)
		{
			return length * (10 + _scaler) / 10;
		}

		/// <summary>
		/// Checks if the trackbar is at its minimum value.
		/// </summary>
		/// <returns><c>true</c> if trackbar is not at its minimum value</returns>
		internal bool isMinTrackVal()
		{
			return bar_Scale.Value == bar_Scale.Minimum;
		}
		#endregion Methods


		/// <summary>
		/// <c>string</c> of <c><see cref="EditMode.Locked">EditMode.Locked</see></c>.
		/// </summary>
		internal const string EditLocked  = "Locked";

		/// <summary>
		/// <c>string</c> of <c><see cref="EditMode.Enabled">EditMode.Enabled</see></c>.
		/// </summary>
		internal const string EditEnabled = "Enabled";
	}


	/// <summary>
	/// <c>enum</c> for
	/// <c><see cref="SpriteEditorF.Mode">SpriteEditorF.Mode</see></c>.
	/// </summary>
	internal enum EditMode
	{
		/// <summary>
		/// Sprite cannot be edited.
		/// </summary>
		/// <remarks>Keep this identifier synched with
		/// <c><see cref="SpriteEditorF.EditLocked">SpriteEditorF.EditLocked</see></c>
		/// since it is used as a <c>string</c> by the user-config file.</remarks>
		Locked,

		/// <summary>
		/// Sprite can be edited.
		/// </summary>
		/// <remarks>Keep this identifier synched with
		/// <c><see cref="SpriteEditorF.EditLocked">SpriteEditorF.EditEnabled</see></c>
		/// since it is used as a <c>string</c> by the user-config file.</remarks>
		Enabled
	}
}
