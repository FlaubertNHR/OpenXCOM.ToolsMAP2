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
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// A byte array containing the (uncompressed) sprite-pixels as
		/// palette-indices.
		/// </summary>
		/// <remarks>Byte arrays get initialized w/ "0" by default.</remarks>
		private byte[] _bindata;
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

		/// <summary>
		/// The SetId is set in the <see cref="PckSprite"/> cTor if necessary.
		/// </summary>
		/// <remarks>SetId is used only by 'MapInfoDialog'.</remarks>
		public int SetId
		{ get; protected set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Creates an XCImage.
		/// </summary>
		/// <param name="bindata">the uncompressed source data</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pal">pass in null to *bypass* creating the 'Image'; ie,
		/// the PckSprite..cTor has already unravelled the compressed image-data
		/// instead</param>
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
			SetId = -1; // used only by MapInfo

			_bindata = bindata;

			Pal = pal;

			if (Pal != null)									// NOTE: this is to check for a call by BitmapService.CreateSprite()
				Sprite = BitmapService.CreateSprite(			// which is called by
												width,			// BitmapService.CreateSpriteset() and
												height,			// several PckViewF contextmenu events
												GetBindata(),	// BUT: the call by PckSprite..cTor initializer needs to decode
												Pal.Table);		// the file-data first, then it creates its own 'Image'.
		}														// that's why i prefer pizza.

		/// <summary>
		/// cTor[1]. For clone.
		/// </summary>
		/// <remarks>See <see cref="PckSprite">PckSprite..cTor</see> and
		/// <see cref="PckSprite.Duplicate">PckSprite.Duplicate</see></remarks>
		protected XCImage()
		{}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Sets bindata.
		/// </summary>
		/// <param name="bindata"></param>
		/// <remarks>Don't use a property setter - ca1044 - fxCop doesn't like
		/// writeonly properties.</remarks>
		protected void SetBindata(byte[] bindata)
		{
			_bindata = bindata;
		}

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
			for (int i = 0; i != GetBindata().Length; ++i)
			{
				if (GetBindata()[i] != Palette.Tid)
					return false;
			}
			return true;
		}
		#endregion Methods


		#region Methods (IDisposable)
		/// <summary>
		/// Disposes the Bitmaps 'Sprite' and 'SpriteToned' if applicable.
		/// </summary>
		public void Dispose()
		{
//			Sprite.Dispose();
//
//			var sprite = this as PckSprite;
//			if (sprite != null && sprite.SpriteToned != null)
//				sprite.SpriteToned.Dispose();

			// fxCop ca1063
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion Methods (IDisposable)


		#region fxCop ca1063
		private bool _disposed; // <- probably for multithreaded stuff

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
//				try
				{
					if (disposing)
					{
						if (Sprite != null)
						{
							Sprite.Dispose();
							Sprite = null; // pointless.
						}

						var sprite = this as PckSprite; // dispose this in PckSprite - not so fast.
						if (sprite != null && sprite.SpriteToned != null)
						{
							sprite.SpriteToned.Dispose();
							sprite.SpriteToned = null; // pointless.
						}
					}

//					if (nativeResource != IntPtr.Zero)
//					{
//						Marshal.FreeHGlobal(nativeResource);
//						nativeResource = IntPtr.Zero;
//					}
				}
//				finally
				{
					_disposed = true;
				}
			}
		}

		// NOTE: Leave out the finalizer altogether if this class doesn't own
		// unmanaged resources itself but leave the other methods exactly as
		// they are.
//		~XCImage()
//		{
//			Dispose(false);
//		}
		#endregion fxCop ca1063
	}
}
