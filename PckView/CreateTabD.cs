using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace PckView
{
	/// <summary>
	/// A dialog for creating a Tabfile from scratch based on a Pckfile
	/// (xcom compressed image data).
	/// </summary>
	internal sealed partial class CreateTabD
		:
			Form
	{
		#region Fields (static)
		private static string _lastfile;
		private static string _lastdir;

		private const string WARNING = "This is not guaranteed to be valid.";
		private const string SUCCESS = "Tabfile was written to the directory of the Pckfile.";

		private const string ERROR_TITLE = "Pckfile is bonky";
		#endregion Fields (static)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal CreateTabD()
		{
			InitializeComponent();

			tb_input.BackColor = Color.GhostWhite;

			if (!String.IsNullOrEmpty(_lastfile))
				tb_input.Text = _lastfile;

			bu_open.Select();
		}
		#endregion cTor


		#region eventhandlers
		/// <summary>
		/// Inserts a path to a Pckfile to <c><see cref="tb_input"/></c>.
		/// </summary>
		/// <param name="sender"><c><see cref="bu_open"/></c></param>
		/// <param name="e"></param>
		private void OnOpenPckfileClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title  = "Select a PCK file";
				ofd.Filter = FileDialogStrings.GetFilterPck();

				ofd.RestoreDirectory = true;

				if (Directory.Exists(_lastdir))
					ofd.InitialDirectory = _lastdir;

				if (!String.IsNullOrEmpty(_lastfile))
					ofd.FileName = _lastfile;
				else
					ofd.FileName = "*.PCK";


				if (ofd.ShowDialog(this) == DialogResult.OK)
					tb_input.Text = ofd.FileName;

				_lastdir = Path.GetDirectoryName(ofd.FileName);
			}
		}

		/// <summary>
		/// Creates a Tabfile based on a Pckfile.
		/// </summary>
		/// <param name="sender"><c><see cref="bu_create"/></c></param>
		/// <param name="e"></param>
		private void OnCreateClick(object sender, EventArgs e)
		{
			byte[] bytes = FileService.ReadFile(tb_input.Text);
			if (bytes != null)
			{
				if (bytes.Length < 2)
				{
					using (var f = new Infobox(
											ERROR_TITLE,
											Infobox.SplitString("input file requires at least 2 bytes: one for"
															  + " the Min count of transparent rows in the first"
															  + " sprite and another for an End_of_Sprite marker."),
											"00 FF",
											InfoboxType.Error))
					{
						f.ShowDialog(this);
						return;
					}
				}

				if (bytes[bytes.Length - 1] != PckSprite.MarkerEos)
				{
					using (var f = new Infobox(
											ERROR_TITLE,
											"final sprite does not end with a stop-byte",
											"offset " + getoffset((uint)bytes.Length - 1),
											InfoboxType.Error))
					{
						f.ShowDialog(this);
						return;
					}
				}

				var offsets = new List<uint> { (uint)0 };		// first offset is always 0

				for (uint i = 1; i != bytes.Length; ++i)
				{
					if (i == offsets[offsets.Count - 1])		// start-byte
					{
						if (i == bytes.Length - 1 && bytes[i] == PckSprite.MarkerEos)
						{
							using (var f = new Infobox(
													ERROR_TITLE,
													"final sprite does not end with a valid stop-byte",
													"offset " + getoffset(i),
													InfoboxType.Error))
							{
								f.ShowDialog(this);
								return;
							}
						}
					}
					else if (bytes[i] == PckSprite.MarkerRle)	// rle-byte
					{
						if (i == bytes.Length - 2)
						{
							using (var f = new Infobox(
													ERROR_TITLE,
													"final sprite does not end with a valid stop-byte",
													"offset " + getoffset(i),
													InfoboxType.Error))
							{
								f.ShowDialog(this);
								return;
							}
						}

						++i;
					}
					else if (bytes[i] == PckSprite.MarkerEos)	// stop-byte
					{
						if (i != bytes.Length - 1)
						{
							uint offset = i + 1;

							if (rb_2byte.Checked && offset > UInt16.MaxValue)
							{
								string head = Infobox.SplitString("The size (in bytes) of the encoded Pckfile"
																+ " is too large for the offsets of its sprites"
																+ " to be written in a 2-byte Tabfile.");
								using (var f = new Infobox(
														ERROR_TITLE,
														head,
														"offset " + getoffset(i),
														InfoboxType.Error))
								{
									f.ShowDialog(this);
									return;
								}
							}

							offsets.Add(offset);
						}
					}
					// else										// palid-byte
				}


				string dir = Path.GetDirectoryName(tb_input.Text);
				string pfe = Path.Combine(
										dir,
										Path.GetFileNameWithoutExtension(tb_input.Text) + GlobalsXC.TabExt);

				string pfeT;
				if (File.Exists(pfe))
					pfeT = pfe + GlobalsXC.TEMPExt;
				else
					pfeT = pfe;


				bool fail = true;

				using (var fsTab = FileService.CreateFile(pfeT))
				if (fsTab != null)
				using (var bwTab = new BinaryWriter(fsTab))
				{
					fail = false;

					if (rb_2byte.Checked)
					{
						foreach (var offset in offsets)
							bwTab.Write((ushort)offset);
					}
					else // rb_4byte.Checked
					{
						foreach (var offset in offsets)
							bwTab.Write(offset);
					}
				}

				if (!fail)
				{
					la_warn.ForeColor = Color.LimeGreen;
					la_warn.Text = SUCCESS;

					if (pfeT != pfe)
						FileService.ReplaceFile(pfe);
				}
			}
		}

		/// <summary>
		/// helper for <c><see cref="OnCreateClick()">OnCreateClick()</see></c>
		/// </summary>
		/// <returns></returns>
		private string getoffset(uint offset)
		{
			string format;
			if (offset <= UInt16.MaxValue) format = "X4";
			else                           format = "X8";

			return "0x" + offset.ToString(format);
		}

		/// <summary>
		/// Caches the current pfe and dis/enables the Create button.
		/// </summary>
		/// <param name="sender"><c><see cref="tb_input"/></c></param>
		/// <param name="e"></param>
		private void OnInputTextchanged(object sender, EventArgs e)
		{
			bu_create.Enabled = File.Exists(_lastfile = tb_input.Text);

			la_warn.ForeColor = Color.DarkSalmon;
			la_warn.Text = WARNING;
		}
		#endregion eventhandlers
	}
}
