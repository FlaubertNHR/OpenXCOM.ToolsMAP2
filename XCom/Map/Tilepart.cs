using System;


namespace XCom
{
	public sealed class Tilepart
	{
		/// <summary>
		/// Disposes the crippled spriteset.
		/// </summary>
		public static void DisposeCrippledSprites()
		{
			DSShared.LogFile.WriteLine("Tilepart.DisposeCrippledSprites() static");
			if (CrippledSprites != null)
			{
				CrippledSprites.Dispose();
				CrippledSprites = null;
			}
		}


		#region Fields (static)
		public const int PHASES = 8;

		private static Spriteset CrippledSprites;

		private const int MonoTONE_WEST    = 1; // cf. QuadrantDrawService.MonoTONE_* ->
		private const int MonoTONE_NORTH   = 2;
		private const int MonoTONE_FLOOR   = 3;
		private const int MonoTONE_CONTENT = 4;
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// The spriteset of this Tilepart's sprites. It's used to animate the
		/// part or to render the part with its alternate part.
		/// </summary>
		private Spriteset _spriteset;
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
		/// The sprite-array used to animate this tile.
		/// </summary>
		/// <remarks>TODO: Instead of storing the sprite-references in
		/// <c><see cref="Tilepart"/></c> reference the sprites directly in
		/// <c><see cref="_spriteset"/></c> by <c><see cref="Record"/></c>.</remarks>
		/// <code>
		/// byte    id1    = Record.Sprite1;
		/// XCImage sprite = spriteset.Sprites[id1];
		/// </code>
		/// <remarks>But unfortunately the current perspective is deeply
		/// ingrained in the design of the code.</remarks>
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
				Spriteset spriteset = null)
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
		/// cTor[2]. Creates a blank tilepart for
		/// <c><see cref="CreateInsert()">CreateInsert()</see></c>.
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
		// controlled by setting their sprite-arrays to either the first sprite
		// or an array of sprites, like non-door records do.
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
		/// Returns a copy of this <c><see cref="Tilepart"/></c> with a
		/// deep-cloned <c><see cref="Record"/></c> for McdView. But any
		/// referred to sprites and dead/altr tileparts keep pointers to their
		/// current objects.
		/// 
		/// 
		/// - classvars
		/// <list type="bullet">
		/// <item><c><see cref="Record"/></c>     (ptr)</item>
		/// <item><c><see cref="_sprites"/></c>   (ptr) -> not used in McdView</item>
		/// <item><c><see cref="_spriteset"/></c> (ptr) -> not used in McdView</item>
		/// <item><c><see cref="Dead"/></c>       (ptr)</item>
		/// <item><c><see cref="Altr"/></c>       (ptr)</item>
		/// <item><c><see cref="TerId"/></c>      (int)</item>
		/// <item><c><see cref="SetId"/></c>      (int)</item>
		/// </list>
		/// </summary>
		/// <returns>a new <c><see cref="Tilepart"/></c> for insertion in McdView</returns>
		/// <remarks><c><see cref="_sprites"/></c> and <c><see cref="_spriteset"/></c>
		/// shall be null.</remarks>
		public Tilepart CreateInsert()
		{
			var part = new Tilepart();

			part.Record = Record.Duplicate();

			part.Dead = Dead;	// NOTE: keep these pointers and use their TerIds to
			part.Altr = Altr;	// determine the part's 'DieTile' and 'Alt_MCD' fields
								// after insertion. (aha!)
			part.TerId = TerId;
			part.SetId = SetId;

			return part;
		}


		/// <summary>
		/// When a <c><see cref="MapFile"/></c> contains <c><see cref="MapFile.Parts"/></c>
		/// with ids that are beyond the count of parts in its current
		/// terrainset do not null those parts. To cripple a part instead is to
		/// create a new part and assign it a default <c><see cref="McdRecord"/></c>
		/// and one of the <c><see cref="CrippledSprites"/></c> (based on its
		/// quadslot) but to transfer the old <c><see cref="SetId"/></c> to the
		/// new <c><see cref="Tilepart"/></c>.
		/// 
		/// 
		/// This allows the user to invoke <c>MapView.TileslotSubstitution</c>
		/// to shift ids that are above the currently displayable/valid range
		/// down into acceptable values. It's useful only when records have been
		/// removed from <c><see cref="MapFile.Terrains">MapFile.Terrains</see></c>
		/// but there are still records with ids that are higher than any of the
		/// (previously/externally) removed ids.
		/// 
		/// 
		/// See <c><see cref="MapFile"/>.CreateTile().</c>
		/// 
		/// 
		/// <c><see cref="McdRecord"/></c> contains the MCD-entries in case you
		/// haven't figured that.
		/// </summary>
		/// <param name="slot">the <c><see cref="PartType"/></c> to show
		/// crippled</param>
		/// <remarks>This is strictly a one-way operation! All crippled parts
		/// shall go ~poof~ when the Map is saved. TODO: Dispose and null
		/// <c><see cref="CrippledSprites"/></c>.</remarks>
		internal void Cripple(PartType slot)
		{
			// TODO: stop the part from being selected in TileView when the slot
			// in QuadrantControl is double-clicked

			// NOTE: Assigning "-1" to the record's 'PartType' shall force it to
			// be listed in TopView's TestPartslots dialog. And discount it from
			// consideration as a valid highid in the TilepartSubstitution
			// dialog.
			Record.PartType = PartType.Invalid;

			if (CrippledSprites == null)
				CreateCrippledSprites();

			_sprites = new XCImage[PHASES];

			switch (slot)
			{
				case PartType.Floor:
					for (int i = 0; i != PHASES; ++i)
						_sprites[i] = CrippledSprites[MonoTONE_FLOOR];
					break;

				case PartType.West:
					for (int i = 0; i != PHASES; ++i)
						_sprites[i] = CrippledSprites[MonoTONE_WEST];
					break;

				case PartType.North:
					for (int i = 0; i != PHASES; ++i)
						_sprites[i] = CrippledSprites[MonoTONE_NORTH];
					break;

				case PartType.Content:
					for (int i = 0; i != PHASES; ++i)
						_sprites[i] = CrippledSprites[MonoTONE_CONTENT];
					break;
			}
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Creates the sprites for crippled tileparts.
		/// </summary>
		/// <remarks>These sprites could be broken out and put in Resources but
		/// it's kinda cute this way too.</remarks>
		private static void CreateCrippledSprites()
		{
			CrippledSprites = EmbeddedService.CreateMonotoneSpriteset("Monotone_crippled");

			byte[] bindata;
			foreach (XCImage sprite in CrippledSprites.Sprites) // change nontransparent pixels to color ->
			{
				bindata = sprite.GetBindata();
				for (int i = 0; i != bindata.Length; ++i)
				{
					if (bindata[i] != Palette.Tid)
						bindata[i] = (byte)96; // light brown/yellowy - is Palette.UfoBattle
				}

				(sprite as PckSprite).SpriteToned =
				 sprite.Sprite = BitmapService.CreateSprite(
														XCImage.SpriteWidth32,
														XCImage.SpriteHeight40,
														bindata,
														sprite.Pal.Table);
			}
		}
		#endregion Methods (static)
	}
}
