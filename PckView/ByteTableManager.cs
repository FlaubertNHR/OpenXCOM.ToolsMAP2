﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace PckView
{
	/// <summary>
	/// Creates a texttable (copyable) that shows all the bytes in a sprite.
	/// TODO: Allow this to be also editable and saveable.
	/// </summary>
	internal static class ByteTableManager
	{
		#region Fields (static)
		private static Form _fBytes;
		private static RichTextBox _rtbBytes;

		private static XCImage _sprite;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Instantiates the byte-table as a Form with a RichTextBox docked
		/// inside.
		/// </summary>
		/// <param name="sprite">the sprite whose bytes to display</param>
		/// <param name="setType">the type of spriteset to deal with</param>
		/// <param name="callback">function pointer that unchecks the menuitem
		/// in PckViewF</param>
		internal static void LoadTable(
				XCImage sprite,
				SpritesetType setType,
				MethodInvoker callback)
		{
			_sprite = sprite;

			if (_fBytes == null)
			{
				_fBytes = new Form();
				_fBytes.Font       = new Font("Verdana", 7);
				_fBytes.Text       = "Byte Table";
				_fBytes.KeyPreview = true;
				_fBytes.KeyDown     += OnBytesKeyDown;
				_fBytes.FormClosing += (sender, e) => callback();
				_fBytes.FormClosing += OnBytesClosing;
				_fBytes.Load        += OnBytesLoad;

				_rtbBytes = new RichTextBox();
				_rtbBytes.Dock     = DockStyle.Fill;
				_rtbBytes.Font     = new Font("Courier New", 8);
				_rtbBytes.WordWrap = false;
				_rtbBytes.ReadOnly = true;

				_fBytes.Controls.Add(_rtbBytes);
			}
			_fBytes.ClientSize = SizeTable(setType);

			PrintTable();
			_fBytes.Show();
		}

		/// <summary>
		/// Loads new sprite information when the table is already open/visible.
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="setType"></param>
		internal static void ReloadTable(XCImage sprite, SpritesetType setType)
		{
			_sprite = sprite;

			if (_fBytes != null && _fBytes.Visible)
			{
				_fBytes.ClientSize = SizeTable(setType);
				PrintTable();
			}
		}

		/// <summary>
		/// Gets a decent size for the table.
		/// </summary>
		/// <param name="setType"></param>
		/// <returns></returns>
		private static Size SizeTable(SpritesetType setType)
		{
			switch (setType)
			{
//				case SpritesetType.non:
//				case SpritesetType.Pck:
				default:                   return new Size(949, 589);	// Tr w= 924 h= 574
				case SpritesetType.Bigobs: return new Size(949, 701);	// Tr w= 924 h= 686
				case SpritesetType.ScanG:  return new Size(165, 85);	// Tr w= 140 h= 70
				case SpritesetType.LoFT:   return new Size(501, 253);	// Tr w= 476 h= 238
			}
		}

		/// <summary>
		/// Prints the byte-table.
		/// </summary>
		private static void PrintTable()
		{
			string text = String.Empty;

			if (_sprite != null)
			{
				int wrapCount = 0;
				int row       = 0;

				int width = _sprite.GetSpriteWidth();

				byte[] bindata = _sprite.GetBindata();
				foreach (byte b in bindata)
				{
					if (wrapCount % width == 0)
					{
						if (++row < 10) text += " ";
						text += row + ":";
					}

					if      (b <  10) text += "  ";
					else if (b < 100) text +=  " ";

					text += " " + b;

					if (++wrapCount % width == 0)
						text += Environment.NewLine;
				}
			}
			_rtbBytes.Text = text;
		}

		/// <summary>
		/// Hides or closes the byte-table.
		/// </summary>
		internal static void HideTable()
		{
			if (_fBytes != null)
			{
				if (PckViewF.Quit)
					_fBytes.Close();
				else
					_fBytes.Hide();
			}
		}
		#endregion Methods (static)


		#region Events (static)
		/// <summary>
		/// Turns off <c>AutoWordSelection</c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks><c>AutoWordSelection</c> can't be turned off correctly in
		/// the Designer or the cTor.</remarks>
		private static void OnBytesLoad(object sender, EventArgs e)
		{
			_rtbBytes.AutoWordSelection = false;
		}

		/// <summary>
		/// Handles the <c>KeyDown</c> event. Hides or closes the form on
		/// <c>[Esc]</c> or <c>[F11]</c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnBytesKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
				case Keys.F11:
					e.Handled = e.SuppressKeyPress = true;
					_fBytes.Close();
					break;
			}
		}

		/// <summary>
		/// Handles the <c>FormClosing</c> event. Hides the form if PckView is
		/// not quitting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnBytesClosing(object sender, CancelEventArgs e)
		{
			if (e.Cancel = !PckViewF.Quit)
				_fBytes.Hide();
			else
			{
				_rtbBytes.Font.Dispose();
				_fBytes  .Font.Dispose();
			}
		}
		#endregion Events (static)
	}
}
