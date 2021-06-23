using System;


namespace XCom
{
	public sealed class Tilepart
	{
		/// <summary>
		/// Disposes the <c><see cref="CrippledSprites"/></c>.
		/// </summary>
		public static void DisposeCrippledSprites()
		{
			DSShared.Logfile.Log("Tilepart.DisposeCrippledSprites() static");
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


		#region Properties
		/// <summary>
		/// The <c><see cref="McdRecord"/></c> that has information about the
		/// properties of this <c>Tilepart</c>.
		/// </summary>
		public McdRecord Record
		{ get; private set; }

		/// <summary>
		/// This <c>Tilepart's</c>
		/// <c><see cref="McdRecord.DieTile">McdRecord.DieTile</see></c>.
		/// </summary>
		public Tilepart Dead
		{ get; set; }

		private Tilepart _altr;
		/// <summary>
		/// This <c>Tilepart's</c>
		/// <c><see cref="McdRecord.Alt_MCD">McdRecord.Alt_MCD</see></c>.
		/// </summary>
		public Tilepart Altr
		{
			get { return _altr; }
			set
			{
				if ((_altr = value) != null
					&& SpritesetManager.Spritesets.Count > TerId)
				{
					_spritealtr = SpritesetManager.Spritesets[TerId][_altr.Record.Sprite1];
				}
				else
					_spritealtr = null;
			}
		}

		/// <summary>
		/// The sprite-array used to display and/or animate this <c>Tilepart</c>.
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
		/// A door's <c><see cref="Altr"/></c> sprite.
		/// </summary>
		private XCImage _spritealtr;

		/// <summary>
		/// The ID of this <c>Tilepart</c> in its terrain.
		/// </summary>
		public int Id
		{ get; set; }

		/// <summary>
		/// The ID of this <c>Tilepart</c> that's unique to the Map across all
		/// allocated terrains. The value is usually set in
		/// <c><see cref="MapFile()">MapFile()</see></c>.
		/// </summary>
		/// <remarks><c>SetId</c> is written to the Mapfile as a byte.</remarks>
		public int SetId
		{ get; internal set; }

		/// <summary>
		/// The ID of the terrain of this <c>Tilepart</c> in
		/// <c><see cref="MapFile.Terrains">MapFile.Terrains</see></c>
		/// </summary>
		public int TerId
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Creates a standard <c>Tilepart</c>.
		/// </summary>
		/// <param name="id">the id of this <c>Tilepart</c> in its recordset</param>
		/// <param name="record">the <c><see cref="McdRecord"/></c> of this
		/// <c>Tilepart</c></param>
		/// <param name="terid">the id of this <c>Tilepart's</c> terrain in
		/// <c><see cref="MapFile.Terrains">MapFile.Terrains</see></c></param>
		/// <param name="setsprites"><c>true</c> to reference this
		/// <c>Tilepart's</c> sprites per <c><see cref="_sprites"/></c>,
		/// <c>false</c> if McdView is going to handle the sprites itself</param>
		public Tilepart(
				int id,
				McdRecord record,
				int terid = 0,
				bool setsprites = true)
		{
			Record = record;

			Id    = id;
			SetId = -1;
			TerId = terid;

			if (setsprites) // NOTA BENE: '_sprites' shall be null for McdView.
			{
				_sprites = new XCImage[PHASES]; // for MapView each part contains its own pointers to 8 sprites.

				if (!Record.SlidingDoor && !Record.HingedDoor)
				{
					SetSprites();
				}
				else
					SetSprite1();
			}
		}

		/// <summary>
		/// cTor[1]. Creates a blank <c>Tilepart</c> that's ready to go in
		/// McdView (req'd: <c><see cref="Id"/></c>). Also used for crippled
		/// parts on Mapfile load (req'd: <c><see cref="SetId"/></c>).
		/// </summary>
		public Tilepart(int id)
		{
			Record = new McdRecord(null);

			Id = SetId = id;
//			TerId = 0; // default
		}

		/// <summary>
		/// cTor[2]. Creates a blank <c>Tilepart</c> for
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
		/// Sets this <c>Tilepart's</c> <c><see cref="_sprites"/></c>.
		/// </summary>
		private void SetSprites()
		{
			Spriteset spriteset = SpritesetManager.Spritesets[TerId];

			_sprites[0] = spriteset[Record.Sprite1];
			_sprites[1] = spriteset[Record.Sprite2];
			_sprites[2] = spriteset[Record.Sprite3];
			_sprites[3] = spriteset[Record.Sprite4];
			_sprites[4] = spriteset[Record.Sprite5];
			_sprites[5] = spriteset[Record.Sprite6];
			_sprites[6] = spriteset[Record.Sprite7];
			_sprites[7] = spriteset[Record.Sprite8];
		}

		/// <summary>
		/// Sets this <c>Tilepart's</c> <c><see cref="_sprites"/></c> to
		/// <c><see cref="McdRecord.Sprite1">McdRecord.Sprite1</see></c>.
		/// </summary>
		private void SetSprite1()
		{
			XCImage sprite = SpritesetManager.Spritesets[TerId][Record.Sprite1];
			for (int i = 0; i != PHASES; ++i)
				_sprites[i] = sprite;
		}

		/// <summary>
		/// Sets this <c>Tilepart's</c> <c><see cref="_sprites"/></c> to the
		/// first phase of its <c><see cref="Altr"/></c> part.
		/// </summary>
		/// <remarks>This is for doors only.</remarks>
		public void SetSprite1_altr()
		{
			if (Record.SlidingDoor || Record.HingedDoor)
			{
				for (int i = 0; i != PHASES; ++i)
					_sprites[i] = _spritealtr;
			}
		}

		/// <summary>
		/// Toggles this <c>Tilepart's</c> array of sprites if it's a door-part.
		/// </summary>
		/// <param name="ani"><c>true</c> to animate through the 8 phases;
		/// <c>false</c> to display only
		/// <c><see cref="McdRecord.Sprite1">McdRecord.Sprite1</see></c>.</param>
		/// <remarks>This is for doors only.</remarks>
		public void ToggleDoorSprites(bool ani)
		{
			if (Record.SlidingDoor || Record.HingedDoor)
			{
				if (ani)
				{
					if (Record.SlidingDoor || Altr == null)
					{
						SetSprites();
					}
					else
					{
						for (int i = 4; i != PHASES; ++i) // ie. flip between Record.Sprite1 and Altr.Record.Sprite1
							_sprites[i] = _spritealtr;
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
		/// <item><c><see cref="Dead"/></c>       (ptr)</item>
		/// <item><c><see cref="Altr"/></c>       (ptr)</item>
		/// <item><c><see cref="Id"/></c>         (int)</item>
		/// <item><c><see cref="SetId"/></c>      (int)</item>
		/// <item><c><see cref="TerId"/></c>      (int)</item>
		/// </list>
		/// </summary>
		/// <returns>a new <c><see cref="Tilepart"/></c> for insertion in McdView</returns>
		/// <remarks><c><see cref="_sprites"/></c> shall be null.</remarks>
		// and <c><see cref="_spriteset"/></c>
		public Tilepart CreateInsert()
		{
			var part = new Tilepart();

			part.Record = Record.Duplicate();

			part.Dead = Dead;	// NOTE: keep these pointers and use their Ids to
			part.Altr = Altr;	// determine the part's 'DieTile' and 'Alt_MCD' fields
								// after insertion. (aha!)
			part.Id    = Id;
			part.SetId = SetId;
			part.TerId = TerId;

			return part;
		}


		/// <summary>
		/// When a <c><see cref="MapFile"/></c> contains
		/// <c><see cref="MapFile.Parts"/></c> with ids that are beyond the
		/// count of parts in its current terrainset do not null those parts. To
		/// cripple a part instead is to create a new part and assign it a
		/// default <c><see cref="McdRecord"/></c> and one of the
		/// <c><see cref="CrippledSprites"/></c> (based on its quadslot) but to
		/// transfer the old <c><see cref="SetId"/></c> to the new
		/// <c><see cref="Tilepart"/></c>.
		/// 
		/// 
		/// This allows the user to invoke <c>MapView.TilepartSubstitution</c>
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
		/// shall go ~poof~ when the Mapfile is saved.</remarks>
		internal void Cripple(PartType slot)
		{
			// TODO: stop the part from being selected in TileView when the slot
			// in QuadrantControl is double-clicked

			// NOTE: Assigning 'PartType.Invalid' to the record's 'PartType'
			// shall force it to be listed in TopView's TestPartslots dialog and
			// disallow it from consideration as a valid highid in the
			// TilepartSubstitution dialog.
			Record.PartType = PartType.Invalid;

			if (CrippledSprites == null) // once created the CrippledSprites shall be disposed explicitly when MapView quits.
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
		/// Creates the sprites for <c><see cref="CrippledSprites"/></c>.
		/// </summary>
		/// <remarks>These sprites could be broken out and put in Resources but
		/// it's kinda cute this way too.</remarks>
		private static void CreateCrippledSprites()
		{
			CrippledSprites = EmbeddedService.CreateMonotoneSpriteset("Monotone_crippled");

			byte[] bindata;
			foreach (XCImage sprite in CrippledSprites.Sprites) // change nontransparent pixels to a color ->
			{
				bindata = sprite.GetBindata();
				for (int i = 0; i != bindata.Length; ++i)
				{
					if (bindata[i] != Palette.Tid)
						bindata[i] = (byte)96; // colorid Palette.UfoBattle : light brown/yellowy
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
