using System;
using System.Drawing;


namespace XCom
{
	/// <summary>
	/// A container for storing data about an XCOM sprite or ScanG/LoFT icon.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable</c> crap.</remarks>
	public class XCImage
	{
		#region Methods (disposable)
		/// <summary>
		/// Disposes the Bitmaps 'Sprite' and 'SpriteToned' if applicable.
		/// </summary>
		public void Dispose()
		{
			var sprite = this as PckSprite; // dispose this in PckSprite - not so fast.
			if (sprite != null)
				DSShared.Logfile.Log("XCImage.Dispose() id= " + sprite.Id);
			else
				DSShared.Logfile.Log("XCImage.Dispose()");

			if (Sprite != null)
			{
				Sprite.Dispose();
				Sprite = null; // pointless. not necessarily ...
			}

			if (sprite != null && sprite.SpriteToned != null)
			{
				sprite.SpriteToned.Dispose();
				sprite.SpriteToned = null; // pointless. not necessarily ...
			}
		}
		#endregion Methods (disposable)


		#region Fields (static)
		public const  int SpriteWidth32  = 32; // for MapView, so I don't have to recode a bunch of crap there.
		public const  int SpriteHeight40 = 40; // for MapView, so I don't have to recode a bunch of crap there.

		// terrain & units 40px / bigobs 48px / scang 4px / loft 16px
		// NOTE: Bigobs, ScanG, and LoFT shall be supported only by PckView.

		public const  int SpriteHeight48 = 48; // for Bigobs in PckView

		public const  int ScanGside      =  4; // for ScanG icon dimensions in PckView
		public const  int LoFTside       = 16; // for LoFT  icon dimensions in PckView
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// A byte array containing the (uncompressed) sprite-pixels as
		/// palette-indices.
		/// </summary>
		/// <remarks>Byte arrays get initialized w/ "0" by default.</remarks>
		protected byte[] _bindata;
		#endregion Fields


		#region Properties (static)
		private static int _width = 32;
		/// <summary>
		/// The current width of a Pck sprite depends on the type of spriteset.
		/// </summary>
		public static int SpriteWidth
		{
			get { return _width; }
			set { _width = value; }
		}

		private static int _height = 40;
		/// <summary>
		/// The current height of a Pck sprite depends on the type of spriteset.
		/// </summary>
		public static int SpriteHeight
		{
			get { return _height; }
			set { _height = value; }
		}
		#endregion Properties (static)


		#region Properties
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
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Creates an <c>XCImage</c>.
		/// </summary>
		/// <param name="bindata">the uncompressed source data</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pal">the <c><see cref="Palette"/></c>. Pass in
		/// <c>null</c> to bypass creating the <c><see cref="Sprite"/></c> -
		/// <c><see cref="PckSprite(byte[], Palette, int, Spriteset, bool)">PckSprite()</see></c>
		/// has unravelled the compressed image-data and created the sprite
		/// already</param>
		/// <param name="id"></param>
		/// <remarks>Binary data must not be compressed.</remarks>
		internal XCImage(
				byte[] bindata,
				int width,
				int height,
				Palette pal,
				int id)
		{
			Id = id;

			_bindata = bindata;

			Pal = pal;

			if (Pal != null)								// NOTE: this is to check for a call by BitmapService.CreateSprite()
				Sprite = BitmapService.CreateSprite(		// which is called by
												width,		// BitmapService.CreateSpriteset() and
												height,		// several PckViewF contextmenu events
												_bindata,	// BUT: the call by PckSprite..cTor initializer needs to decode
												Pal.Table);	// the file-data first, then it creates its own 'Image'.
		}													// that's why i prefer pizza.

		/// <summary>
		/// cTor[1]. For clone.
		/// </summary>
		/// <remarks>See <c><see cref="PckSprite">PckSprite</see>.PckSprite()</c> and
		/// <c><see cref="PckSprite.Duplicate()">PckSprite.Duplicate()</see></c></remarks>
		protected XCImage()
		{}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Gets bindata.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Don't use a property getter - ca1819 - without changing the
		/// type to a collection.</remarks>
		public byte[] GetBindata()
		{
			return _bindata;
		}


		/// <summary>
		/// Checks if all bytes are the transparent id #0.
		/// </summary>
		/// <returns>true if all palette refs are tid</returns>
		public bool Istid()
		{
			byte[] bindata = GetBindata();
			for (int i = 0; i != bindata.Length; ++i)
			{
				if (_bindata[i] != Palette.Tid)
					return false;
			}
			return true;
		}
		#endregion Methods
	}
}
