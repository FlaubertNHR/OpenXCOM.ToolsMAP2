using System;
using System.Drawing;
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
		private SpriteCollectionPanel SpriteCollectionPanel;
		#endregion Fields


		#region Properties
		private Tilepart[] _records;
		private Tilepart[] Records
		{
			get { return _records; }
			set
			{
				SpriteCollectionPanel.Records = (_records = value);
			}
		}

		private SpriteCollection Spriteset
		{ get; set; }


		internal bool _spriteShadeEnabled;

		private int _spriteShadeInt = 12;// 33; // unity (default)
		private int SpriteShadeInt
		{
			get { return _spriteShadeInt; }
			set
			{
				if (_spriteShadeEnabled = ((_spriteShadeInt = value) != -1))
					SpriteShadeFloat = ((float)_spriteShadeInt * 0.03f);

				SpriteCollectionPanel.Invalidate();
				// TODO: refresh anisprites
			}
		}
		internal float SpriteShadeFloat
		{ get; private set; }
		#endregion Properties


		#region cTor
		internal McdviewF()
		{
			InitializeComponent();

			MaximumSize = new Size(0,0);

			SpriteCollectionPanel = new SpriteCollectionPanel(this);
			gb_Collection.Controls.Add(SpriteCollectionPanel);
			SpriteCollectionPanel.Width = gb_Collection.Width - 10;

			tb_SpriteShade.Text = SpriteShadeInt.ToString();
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
						Spriteset = ResourceInfo.LoadSpriteset(
															terrain,
															Path.GetDirectoryName(pfeMcd),
															2,
															pal);

						for (int id = 0; id != Records.Length; ++id)
						{
							var bindata = new byte[TilepartFactory.Length];
							bs.Read(bindata, 0, TilepartFactory.Length);
							McdRecord record = McdRecordFactory.CreateRecord(bindata);

							Records[id] = new Tilepart(id, Spriteset, record);
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

				Spriteset.Pal = Palette.UfoBattle;

				SpriteCollectionPanel.Invalidate();
				// TODO: refresh anisprites
			}
		}

		private void OnPaletteTftdClick(object sender, EventArgs e)
		{
			if (!miPaletteTftd.Checked)
			{
				miPaletteTftd.Checked = true;
				miPaletteUfo .Checked = false;

				Spriteset.Pal = Palette.TftdBattle;

				SpriteCollectionPanel.Invalidate();
				// TODO: refresh anisprites
			}
		}
		#endregion Menuitems


		#region Events
		private void OnTextChanged_SpriteShade(object sender, EventArgs e)
		{
			// TODO: "SpriteShade does NOT get saved."
			int result;
			if (Int32.TryParse(tb_SpriteShade.Text, out result))
			{
				if      (result <  -1) tb_SpriteShade.Text =  "-1"; // recurse
				else if (result ==  0) tb_SpriteShade.Text =  "-1"; // recurse
				else if (result > 100) tb_SpriteShade.Text = "100"; // recurse
				else
					SpriteShadeInt = result;
			}
			else
				tb_SpriteShade.Text = "-1"; // recurse
		}
		#endregion Events
	}
}
