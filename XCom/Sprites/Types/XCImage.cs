using System;
using System.Drawing;


namespace XCom
{
	/// <summary>
	/// A container for basic data about an XCOM sprite or ScanG/LoFT icon.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable</c>
	/// crap.</remarks>
	public class XCImage
	{
		#region Methods (disposable)
		/// <summary>
		/// Disposes the <c>Bitmaps</c> <c><see cref="Sprite"/></c> and
		/// <c><see cref="PckSprite.SpriteToned">PckSprite.SpriteToned</see></c>
		/// if applicable.
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
		public const int SpriteWidth32  = 32; // for MapView, so I don't have to recode a bunch of crap there.
		public const int SpriteHeight40 = 40; // for MapView, so I don't have to recode a bunch of crap there.

		// terrain & units 40px / bigobs 48px / scang 4px / loft 16px
		// NOTE: Bigobs, ScanG, and LoFT shall be supported only by PckView.

		public const int SpriteHeight48 = 48; // for Bigobs in PckView

		public const int ScanGside      =  4; // for ScanG icon dimensions in PckView
		public const int LoFTside       = 16; // for LoFT  icon dimensions in PckView
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// A byte array containing the (uncompressed) sprite-pixels as
		/// palette-indices.
		/// </summary>
		/// <remarks>Byte arrays get initialized w/ <c>0</c> by default.</remarks>
		protected byte[] _bindata;

		protected int _width;
		protected int _height;
		#endregion Fields


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


		#region Methods
		/// <summary>
		/// Gets <c><see cref="_bindata"/></c>.
		/// </summary>
		/// <returns></returns>
		public byte[] GetBindata()
		{
			return _bindata;
		}

		/// <summary>
		/// Gets <c><see cref="_width"/></c>.
		/// </summary>
		/// <returns></returns>
		public int GetSpriteWidth()
		{
			return _width;
		}

		/// <summary>
		/// Checks if all bytes are the transparent id <c>0</c>.
		/// </summary>
		/// <returns><c>true</c> if all palette-indices are
		/// <c><see cref="Palette.Tid">Palette.Tid</see></c></returns>
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
