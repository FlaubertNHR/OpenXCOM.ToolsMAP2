using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom.Interfaces;


namespace PckView
{
	/// <summary>
	/// Creates a texttable (copyable) that shows all the bytes in a sprite.
	/// TODO: Allow this to be also editable and saveable.
	/// </summary>
	internal static class SpriteBytesManager
	{
		#region Fields (static)
		private static Form _fBytes;
		private static RichTextBox _rtbBytes;

		private static XCImage _sprite;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Instantiates the bytes-table as a Form.
		/// </summary>
		/// <param name="sprite">the sprite whose bytes to display</param>
		/// <param name="callback">function pointer that unchecks the menuitem
		/// in PckViewForm</param>
		internal static void LoadBytesTable(
				XCImage sprite,
				MethodInvoker callback)
		{
			_sprite = sprite;

			if (_fBytes == null)
			{
				_fBytes = new Form();
				_fBytes.Size = new Size(960, 620);
				_fBytes.Font = new Font("Verdana", 7);
				_fBytes.Text = "Bytes Table";
				_fBytes.KeyPreview = true;
				_fBytes.KeyDown     += OnBytesKeyDown;
				_fBytes.FormClosing += (sender, e) => callback();
				_fBytes.FormClosing += OnBytesClosing;

				_rtbBytes = new RichTextBox();
				_rtbBytes.Dock = DockStyle.Fill;
				_rtbBytes.Font = new Font("Courier New", 8);
				_rtbBytes.WordWrap = false;
				_rtbBytes.ReadOnly = true;

				_fBytes.Controls.Add(_rtbBytes);
			}

			PrintBytesTable();
			_fBytes.Show();
		}

		/// <summary>
		/// Loads new sprite information when the table is already open/visible.
		/// </summary>
		/// <param name="sprite"></param>
		internal static void ReloadBytesTable(XCImage sprite)
		{
			_sprite = sprite;

			if (_fBytes != null && _fBytes.Visible)
			{
				if (_sprite != null)
				{
					PrintBytesTable();
				}
				else
					_rtbBytes.Clear();
			}
		}

		private static void PrintBytesTable()
		{
			string text = String.Empty;

			int wrapCount = 0;
			int row       = 0;

			foreach (byte b in _sprite.Bindata)
			{
				if (wrapCount % XCImage.SpriteWidth == 0)
				{
					if (++row < 10) text += " ";
					text += row + ":";
				}

				if (b < 10)
					text += "  ";
				else if (b < 100)
					text += " ";

				text += " " + b;

				if (++wrapCount % XCImage.SpriteWidth == 0)
					text += Environment.NewLine;
			}
			_rtbBytes.Text = text;
		}

		/// <summary>
		/// Hides or closes the bytes-table.
		/// </summary>
		internal static void HideBytesTable()
		{
			if (_fBytes != null)
			{
				if (PckViewForm.Quit)
					_fBytes.Close();
				else
					_fBytes.Hide();
			}
		}
		#endregion Methods (static)


		#region Events (static)
		private static void OnBytesKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				e.SuppressKeyPress = true;
				_fBytes.Close();
			}
		}

		private static void OnBytesClosing(object sender, CancelEventArgs e)
		{
			if (e.Cancel = !PckViewForm.Quit)
				_fBytes.Hide();
		}
		#endregion Events (static)
	}
}
