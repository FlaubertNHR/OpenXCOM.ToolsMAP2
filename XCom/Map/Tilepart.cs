using System;
using System.Reflection;


namespace XCom
{
	public sealed class Tilepart
	{
		#region Fields (static)
		public const int PHASES = 8;

		private static SpriteCollection MonotoneSprites;
		#endregion Fields (static)


		#region Fields
		// TODO: The fact that the spriteset points to a "sprite" and this
		// tilepart points to a "sprite" causes a glitch when changing "that"
		// sprite. They ought be kept consistent since there is an awkward
		// sort of latency-effect happening on refresh.

		/// <summary>
		/// The spriteset of this Tilepart's sprites. It's used to animate the
		/// part or to render the part with its alternate part.
		/// </summary>
		private SpriteCollection _spriteset;
		#endregion Fields


		#region Properties
		/// <summary>
		/// The object that has information about the mechanics and appearance
		/// of this tilepart.
		/// </summary>
		public McdRecord Record
		{ get; private set; }

		public Tilepart Dead
		{ get; set; }

		public Tilepart Altr
		{ get; set; }

		/// <summary>
		/// Gets the sprite-array used to animate this tile.
		/// TODO: This should never have happened; there should be no pointers
		/// to sprites in a 'Tilepart'. The sprites need to be retrieved
		/// directly from its 'SpriteCollection' by 'Record' (int)Phase*
		/// on-the-fly.
		/// 
		/// But unfortunately that difficult perspective is deeply ingrained in
		/// the design of the code.
		/// </summary>
		private XCImage[] _sprites;

		/// <summary>
		/// Gets the sprite at a specified animation phase.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public XCImage this[int id]
		{
			get { return _sprites[id]; }
		}

		/// <summary>
		/// The ID of this tilepart that's unique to its terrain/MCD-record.
		/// </summary>
		public int TerId
		{ get; set; }

		/// <summary>
		/// The ID of this tilepart that's unique to the Map across all
		/// allocated terrains. The value is set in MapFile..cTor.
		/// IMPORTANT: The 'SetId' is written to the Mapfile (as a byte).
		/// </summary>
		public int SetId
		{ get; internal set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Creates a standard Tilepart.
		/// </summary>
		/// <param name="id">the id of this part in its recordset</param>
		/// <param name="record">this part's MCD-record as an object</param>
		/// <param name="spriteset">the spriteset from which to get this part's
		/// sprite-phases; null for McdView - sprites shall be retrieved
		/// directly from the spriteset itself</param>
		public Tilepart(
				int id,
				McdRecord record,
				SpriteCollection spriteset = null)
		{
			Record = record;

			TerId = id;
			SetId = -1;

			if ((_spriteset = spriteset) != null) // nota bene: 'Spriteset' and 'Sprites' shall be null for McdView.
			{
				InitSprites();
			}
		}

		/// <summary>
		/// cTor[1]. Creates a blank part that's ready to go in McdView
		/// (req'd: 'TerId'). Also used for crippled parts on Mapfile load
		/// (req'd: 'SetId').
		/// </summary>
		public Tilepart(int id)
		{
			Record = new McdRecord(null);

			TerId =
			SetId = id;
		}

		/// <summary>
		/// cTor[2]. Creates a blank part for CreateInsert().
		/// </summary>
		private Tilepart()
		{}
		#endregion cTor


		#region Methods
		// re. Animating Sprites
		// Basically this is how animations operate. For *any* animations to
		// happen the Animation option has to be turned on. Non-door sprites
		// always keep their array of sprites and will cycle because turning on
		// the Animation option starts a timer that does that.
		// 'MapView.MainViewUnderlay' for timer
		// 'MapView.MainViewF'        for options
		// 
		// SlidingDoor sprites will animate when the Animation option is on and
		// the Doors option is turned on; but whether or not they animate is
		// controlled by setting their sprite-arrays to either the first image
		// or an array of images, like non-door records do.
		// 
		// HingedDoors, which also need the Animation option on to animate as
		// well as the Doors option on, will cycle by flipping their sprite
		// back and forth between their first sprite and their Alt-tile's first
		// sprite; they stop animating by setting the entire array to their
		// first sprite only.

		/// <summary>
		/// Initializes this tilepart's array of sprites.
		/// </summary>
		private void InitSprites()
		{
			_sprites = new XCImage[PHASES]; // for MapView a part contains its own pointers to 8 sprites.

			if (!Record.SlidingDoor && !Record.HingedDoor)
			{
				SetSprites();
			}
			else
				SetSprite1();
		}

		/// <summary>
		/// Sets this tilepart's sprites in accord with its record's
		/// sprite-phases.
		/// </summary>
		private void SetSprites()
		{
			_sprites[0] = _spriteset[Record.Sprite1];
			_sprites[1] = _spriteset[Record.Sprite2];
			_sprites[2] = _spriteset[Record.Sprite3];
			_sprites[3] = _spriteset[Record.Sprite4];
			_sprites[4] = _spriteset[Record.Sprite5];
			_sprites[5] = _spriteset[Record.Sprite6];
			_sprites[6] = _spriteset[Record.Sprite7];
			_sprites[7] = _spriteset[Record.Sprite8];
		}

		/// <summary>
		/// Sets this tilepart's sprites to its record's first sprite-phase.
		/// </summary>
		private void SetSprite1()
		{
			for (int i = 0; i != PHASES; ++i)
				_sprites[i] = _spriteset[Record.Sprite1];
		}

		/// <summary>
		/// Sets this tilepart's sprites to the first phase of its Altr part. Is
		/// for doors only.
		/// </summary>
		public void SetSprite1_alt()
		{
			if (_spriteset != null
				&& (Record.SlidingDoor || Record.HingedDoor))
			{
				byte altr = Altr.Record.Sprite1;
				for (int i = 0; i != PHASES; ++i)
					_sprites[i] = _spriteset[altr];
			}
		}

		/// <summary>
		/// Toggles this tilepart's array of sprites if it's a door-part.
		/// </summary>
		/// <param name="animate">true to animate</param>
		public void ToggleDoorSprites(bool animate)
		{
			if (_spriteset != null
				&& (Record.SlidingDoor || Record.HingedDoor))
			{
				if (animate)
				{
					if (Record.SlidingDoor || Altr == null)
					{
						SetSprites();
					}
					else
					{
						byte altr = Altr.Record.Sprite1;
						for (int i = 4; i != PHASES; ++i) // ie. flip between Sprite1 and Altr.Sprite1
							_sprites[i] = _spriteset[altr];
					}
				}
				else
					SetSprite1();
			}
		}


		/// <summary>
		/// Returns a copy of this Tilepart with a deep-cloned Record for
		/// McdView. But any referred to sprites and dead/altr tileparts
		/// keep pointers to their current objects.
		/// @note Sprites and the Spriteset shall be null.
		/// - classvars:
		///   Record	(ptr)
		///   Sprites	(ptr) -> not used in McdView
		///   Spriteset	(ptr) -> not used in McdView
		///   Dead		(ptr)
		///   Altr		(ptr)
		///
		///   TerId		(int)
		///   SetId		(int)
		/// </summary>
		/// <returns></returns>
		public Tilepart CreateInsert()
		{
			var part = new Tilepart();

			part.TerId = TerId;
			part.SetId = SetId;

			part.Record = Record.Duplicate();

			part.Dead = Dead;	// NOTE: keep these pointers and use their TerIds to
			part.Altr = Altr;	// determine the part's 'DieTile' and 'Alt_MCD' fields
								// after insertion. (aha!)
			return part;
		}


		/// <summary>
		/// When a Mapfile contains tilepart-ids that are beyond the count of
		/// parts in its current terrainset do not null those parts. To cripple
		/// a part instead is to create a new part and assign it a default
		/// record and one of the MonotoneSprites (based on its quadslot) but to
		/// transfer the old SetId to the new SetId.
		/// 
		/// This allows the user to invoke TileslotSubstitution to shift ids
		/// above the currently displayable range down into accepted values.
		/// It's useful only when records have been removed from the Mapfile's
		/// current terrains but there are still used records with ids that are
		/// higher than any of the (previously/externally) removed records' ids.
		/// 
		/// Records are MCD-entries in case you haven't figured that.
		/// 
		/// IMPORTANT: This is strictly a one-way operation!
		/// 
		/// See <see cref="MapFile"/> CreateTile()
		/// 
		/// IMPORTANT: All crippled parts shall go ~poof~ when the Mapfile is
		/// saved.
		/// </summary>
		/// <param name="slot"></param>
		internal void Cripple(PartType slot)
		{
			// TODO: stop the 'tile' from being selected in TileView
			// when the slot in QuadrantPanel is double-clicked

			// NOTE: Assigning "-1" to the record's 'PartType' shall force it to
			// be listed in TopView's TestPartslots dialog. And discount it from
			// consideration as a valid highid in the TilepartSubstitution
			// dialog.
			Record.PartType = PartType.Invalid;

			LoadMonotoneSprites();

			_sprites = new XCImage[PHASES];

			switch (slot)
			{
				case PartType.Floor:
					for (int i = 0; i != PHASES; ++i)
						_sprites[i] = MonotoneSprites[3];
					break;

				case PartType.West:
					for (int i = 0; i != PHASES; ++i)
						_sprites[i] = MonotoneSprites[1];
					break;

				case PartType.North:
					for (int i = 0; i != PHASES; ++i)
						_sprites[i] = MonotoneSprites[2];
					break;

				case PartType.Content:
					for (int i = 0; i != PHASES; ++i)
						_sprites[i] = MonotoneSprites[4];
					break;
			}
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Loads the sprites for TopView's blank quads and TileView's eraser.
		/// @note These sprites could be broken out and put in Resources but
		/// it's kinda cute this way too.
		/// @note See also MainViewF.LoadMonotoneSprites().
		/// </summary>
		private static void LoadMonotoneSprites()
		{
			if (MonotoneSprites == null)
			{
				var ass = Assembly.GetExecutingAssembly();
				using (var strPck = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE_D.PCK"))
				using (var strTab = ass.GetManifestResourceStream("XCom._Embedded.MONOTONE_D.TAB"))
				{
					var bytesPck = new byte[strPck.Length];
					var bytesTab = new byte[strTab.Length];

					strPck.Read(bytesPck, 0, (int)strPck.Length);
					strTab.Read(bytesTab, 0, (int)strTab.Length);

					MonotoneSprites = new SpriteCollection(
														"Monotone_D",
														Palette.UfoBattle,
														SpritesetsManager.TAB_WORD_LENGTH_2,
														bytesPck,
														bytesTab);

					foreach (var sprite in MonotoneSprites.Sprites) // change nontransparent pixels to color ->
					{
						for (int i = 0; i != sprite.Bindata.Length; ++i)
						{
							if (sprite.Bindata[i] != Palette.Tid)
								sprite.Bindata[i] = (byte)(96); // light brown/yellowy
						}
						sprite.Sprite = BitmapService.CreateColored(
																XCImage.SpriteWidth32,
																XCImage.SpriteHeight40,
																sprite.Bindata,
																sprite.Pal.Table);
					}
				}
			}
		}
		#endregion Methods (static)
	}
}

/*		internal void Cripple()
		{
//			Record = null;
//			Dead   = null;
//			Altr   = null;

			Spriteset = null;

			var bindata = new byte[XCImage.SpriteWidth32
								 * XCImage.SpriteHeight40]; // inits to 0

			for (int i = 0; i != bindata.Length; ++i)
			{
				if (   i % XCImage.SpriteWidth32 == 0
					|| i % XCImage.SpriteWidth32 - (XCImage.SpriteWidth32 - 1) == 0)
				{
					bindata[i] = 15; // color-id
				}
			}

			var image = new PckImage();
			image.Bindata = bindata;
			image.Id = -1;
			image.Pal = Palette.UfoBattle;
//			image.SetId = -1;

			image.Sprite = BitmapService.CreateColored(
													XCImage.SpriteWidth32,
													XCImage.SpriteHeight40,
													image.Bindata,
													image.Pal.Table);

			image.SpriteToned = BitmapService.CreateColored(
													XCImage.SpriteWidth32,
													XCImage.SpriteHeight40,
													image.Bindata,
													image.Pal.GrayScaled.Table);

			Sprites = new XCImage[PHASECOUNT];
			for (int i = 0; i != PHASECOUNT; ++i)
				Sprites[i] = image;

//			TerId = -1;
		} */
