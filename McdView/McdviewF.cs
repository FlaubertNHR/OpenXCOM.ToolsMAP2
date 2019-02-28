using System;
using System.IO;
using System.Windows.Forms;

using XCom;
using XCom.Resources.Map;


namespace McdView
{
	/// <summary>
	/// 
	/// </summary>
	internal partial class McdviewF
		:
			Form
	{
		#region Fields
		private Tilepart[] Records;

		private SpriteCollectionPanel SpriteCollectionPanel;
		#endregion Fields


		#region Properties
		private SpriteCollection _spriteset;
		internal SpriteCollection SpriteCollection
		{
			get { return _spriteset; }
			set
			{
				SpriteCollectionPanel.SpriteCollection = (_spriteset = value);
			}
		}
		#endregion Properties


		#region cTor
		internal McdviewF()
		{
			InitializeComponent();

			SpriteCollectionPanel = new SpriteCollectionPanel(this);
			gb_Collection.Controls.Add(SpriteCollectionPanel);
		}
		#endregion cTor


		#region Menuitems
		private void OnOpenClick(object sender, EventArgs e)
		{
			// TODO: Check changed.

			using (var ofd = new OpenFileDialog())
			{
				ofd.Title      = "Open an MCD file";
				ofd.DefaultExt = "MCD";
				ofd.Filter     = "MCD files (*.MCD)|*.MCD|All files (*.*)|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					ResourceInfo.ReloadSprites = true;

					string pfeMcd = ofd.FileName;
					string terrain = Path.GetFileNameWithoutExtension(pfeMcd);

					using (var bs = new BufferedStream(File.OpenRead(pfeMcd)))
					{
						Records = new Tilepart[(int)bs.Length / TilepartFactory.Length]; // TODO: Error if this don't work out right.

						Palette pal;
						if (miPaletteUfo.Checked)
							pal = Palette.UfoBattle;
						else
							pal = Palette.TftdBattle;

						// NOTE: The spriteset is also maintained by a pointer
						// to it that's stored in each tilepart.
						SpriteCollection = ResourceInfo.LoadSpriteset(
																	terrain,
																	Path.GetDirectoryName(pfeMcd),
																	2,
																	pal);

						for (int id = 0; id != Records.Length; ++id)
						{
							var bindata = new byte[TilepartFactory.Length];
							bs.Read(bindata, 0, TilepartFactory.Length);
							McdRecord record = McdRecordFactory.CreateRecord(bindata);

							Records[id] = new Tilepart(id, SpriteCollection, record);
						}

						for (int id = 0; id != Records.Length; ++id)
						{
							Records[id].Dead      = TilepartFactory.GetDeadPart(     terrain, id, Records[id].Record, Records);
							Records[id].Alternate = TilepartFactory.GetAlternatePart(terrain, id, Records[id].Record, Records);
						}
					}
					ResourceInfo.ReloadSprites = false;
				}
			}
		}


		private void OnPaletteUfoClick(object sender, EventArgs e)
		{
			if (!miPaletteUfo.Checked)
			{
				miPaletteUfo .Checked = true;
				miPaletteTftd.Checked = false;

				_spriteset.Pal = Palette.UfoBattle;
				// TODO: refresh all sprites
			}
		}

		private void OnPaletteTftdClick(object sender, EventArgs e)
		{
			if (!miPaletteTftd.Checked)
			{
				miPaletteTftd.Checked = true;
				miPaletteUfo .Checked = false;

				_spriteset.Pal = Palette.TftdBattle;
				// TODO: refresh all sprites
			}
		}
		#endregion Menuitems


		#region Events
		#endregion Events
	}
}
