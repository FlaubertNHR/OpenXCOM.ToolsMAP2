using System;
using System.Drawing;


namespace XCom
{
	/// <summary>
	/// A container for storing data about an XCOM image.
	/// </summary>
	public class XCImage
		:
			IDisposable
	{
		#region Fields (static)
		public const  int SpriteWidth32  = 32; // for MapView, so I don't have to recode a bunch of crap there.
		public const  int SpriteHeight40 = 40; // for MapView, so I don't have to recode a bunch of crap there.

		// terrain & units 40px / bigobs 48px / scang 4px / loft 16px
		// NOTE: Bigobs, ScanG, and LoFT shall be supported only by PckView.

		public const  int SpriteHeight48 = 48; // for Bigobs in PckView

		public const  int ScanGside      =  4; // for ScanG icon dimensions in PckView
		public const  int LoFTside       = 16; // for LoFT  icon dimensions in PckView

		public static int SpriteWidth    = 32; // these two change depending on the type of spriteset ->
		public static int SpriteHeight   = 40; // their initial values are only defaults
		#endregion Fields (static)


		#region Properties
		/// <summary>
		/// A byte array containing the (uncompressed) sprite-pixels as
		/// palette-indices.
		/// @note Byte arrays get initialized w/ "0" by default.
		/// </summary>
		public byte[] Bindata
		{ get; internal protected set; }

		public int Id
		{ get; set; }

		public Bitmap Sprite
		{ get; set; }

		private Palette _palette;
		public Palette Pal
		{
			get { return _palette; }
			set
			{
				_palette = value;

				if (Sprite != null)
					Sprite.Palette = _palette.Table;
			}
		}

		/// <summary>
		/// SetId is used only by 'MapInfoDialog'.
		/// @note It is set in the PckSprite cTor if necessary.
		/// </summary>
		public int SetId
		{ get; internal protected set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Creates an XCImage.
		/// @note Binary data must not be compressed.
		/// </summary>
		/// <param name="bindata">the uncompressed source data</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pal">pass in null to *bypass* creating the 'Image'; ie,
		/// the PckSprite..cTor has already unravelled the compressed image-data
		/// instead</param>
		/// <param name="id"></param>
		internal XCImage(
				byte[] bindata,
				int width,
				int height,
				Palette pal,
				int id)
		{
			Id = id;
			SetId = -1; // used only by MapInfo

			Bindata = bindata;

			Pal = pal;

			if (Pal != null)								// NOTE: this is to check for a call by BitmapService.CreateSprite()
				Sprite = BitmapService.CreateSprite(		// which is called by
												width,		// BitmapService.CreateSpriteset() and
												height,		// several PckViewF contextmenu events
												Bindata,	// BUT: the call by PckSprite..cTor initializer needs to decode
												Pal.Table);	// the file-data first, then it creates its own 'Image'.
		}													// that's why i prefer pizza.

		/// <summary>
		/// cTor[1]. For clone. See PckSprite..cTor[1] and .Duplicate().
		/// </summary>
		protected XCImage()
		{}
		#endregion cTor


		#region Methods (IDisposable)
		/// <summary>
		/// Disposes the Bitmap 'Sprite'.
		/// </summary>
		public void Dispose()
		{
			Sprite.Dispose();
		}
		#endregion Methods (IDisposable)
	}
}
