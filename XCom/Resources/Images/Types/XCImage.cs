using System;
using System.Drawing;


namespace XCom.Interfaces
{
	public class XCImage
	{
		#region Properties
		public byte[] Bindata
		{ get; protected set; }

		public int TerrainId
		{ get; set; }

		public Bitmap Image // TODO: change to 'Sprite' ... expect a designer to fu.
		{ get; set; }

		public Bitmap SpriteGray
		{ get; protected set; }

		private Palette _palette;
		public Palette Pal
		{
			get { return _palette; }
			set
			{
				_palette = value;

				if (Image != null)
					Image.Palette = _palette.ColorTable;
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// Creates an XCImage.
		/// NOTE: Entries must not be compressed.
		/// </summary>
		/// <param name="bindata">the uncompressed source data</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pal">pass in null to *bypass* creating the 'Image'; ie,
		/// the PckImage..cTor has already unravelled the compressed image-data
		/// instead</param>
		/// <param name="terrainId"></param>
		internal XCImage(
				byte[] bindata,
				int width,
				int height,
				Palette pal,
				int terrainId)
		{
			TerrainId = terrainId;

			Bindata = bindata;
			Pal     = pal;

			if (Pal != null)										// NOTE: this is to check for a call by BitmapService.LoadSprite()
				Image = BitmapService.MakeBitmapTrue(				// which is called by
												width,				// BitmapService.LoadSpriteset() and
												height,				// PckViewForm.OnSpriteAddClick()
												Bindata,			// BUT: the call by PckImage..cTor initializer needs to decode
												Pal.ColorTable);	// the file-data first, then it creates its own 'Image'.
		}															// that's why i prefer pizza.
		#endregion
	}
}

//		private XCImage(Bitmap image, int id)
//		{
//			Image = image;
//			FileId = id;
//		}
//		public XCImage()
//			:
//				this(
//					new byte[]{},
//					0, 0,
//					null,
//					-1)
//		{}

//		#region Methods
//		/// <summary>
//		/// Clones this image for use by PckView.
//		/// </summary>
//		/// <returns>pointer to a new XCImage or null</returns>
//		public XCImage Clone()
//		{
//			if (Bindata != null)
//			{
//				var bindata = new byte[Bindata.Length];
//				for (int i = 0; i != bindata.Length; ++i)
//					bindata[i] = Bindata[i];
//
//				return new XCImage(
//								bindata,
//								Image.Width,
//								Image.Height,
//								Pal,
//								FileId);
//			}
//
//			// TODO: this arbitrary Clone() method should probably be disallowed:
//			return (Image != null) ? new XCImage(Image.Clone() as Bitmap, FileId)
//								   : null;
//		}

//		internal void HQ2X()
//		{
//			Image = Bmp.HQ2X(/*Image*/);
//		}
//		#endregion
