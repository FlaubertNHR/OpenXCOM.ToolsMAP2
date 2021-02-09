using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using DSShared.Controls;

using XCom;


namespace McdView
{
	internal sealed class LoftPanel_copier
		:
			BufferedPanel
	{
		#region Fields (static)
		private static McdviewF   _f;
		private static CopierF _fcopier;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Initializes '_f' and '_fcopier'.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="fcopier"></param>
		internal static void SetStaticVars(McdviewF f, CopierF fcopier)
		{
			_f = f; _fcopier = fcopier;
		}
		#endregion Methods (static)


		#region Events (override)
		/// <summary>
		/// Draws a LoFT icon in this LoftPanel_copier.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (McdviewF.isRunT // <- is set in McdviewF.cTor; prevents designer barf attacks.
				&& _fcopier.SelId != -1 && _f.LoFT != null)
			{
				var graphics = e.Graphics;
				graphics.PixelOffsetMode = PixelOffsetMode.Half;

				var tb = Tag as TextBox;

				int loftid = Int32.Parse(tb.Text);
				if (loftid < _f.LoFT.Length / 256)
				{
					graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

					loftid *= 256; // array id

					using (var loft = new Bitmap(16,16, PixelFormat.Format8bppIndexed))	// Format1bppIndexed <- uses only 1 BIT per pixel
					{																	// - causes probs setting the pixels below.
						var data = loft.LockBits(
											new Rectangle(0,0, loft.Width, loft.Height),
											ImageLockMode.WriteOnly,
											PixelFormat.Format8bppIndexed); // Format1bppIndexed
						var start = data.Scan0;

						unsafe
						{
							var pos = (byte*)start.ToPointer();

							byte palid;
							for (int row = 0; row != loft.Height; ++row)
							for (int col = 0; col != loft.Width;  ++col)
							{
								byte* pixel = pos + (row * data.Stride) + col;

								palid = Convert.ToByte(_f.LoFT[loftid + (row * loft.Width) + col]);
								*pixel = palid;
							}
						}
						loft.UnlockBits(data);

						ColorPalette pal = loft.Palette;
						pal.Entries[Palette.LoFTclear] = Color.Black;
						pal.Entries[Palette.LoFTSolid] = Color.White;
						loft.Palette = pal;

						graphics.DrawImage(
										loft,
										0,0,
										Width,Height);
					}
				}
				else
					graphics.FillRectangle(
										Colors.BrushInvalid,
										0,0,
										Width,Height);
			}
		}
		#endregion Events (override)
	}
}
