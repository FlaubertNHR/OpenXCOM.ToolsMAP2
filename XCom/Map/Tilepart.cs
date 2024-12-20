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
			//DSShared.Logfile.Log("Tilepart.DisposeCrippledSprites() static");
			if (CrippledSprites != null)
			{
				CrippledSprites.Dispose();
				CrippledSprites = null;
			}
		}


		#region Fields (static)
		/// <summary>
		/// The count of animation-phases aka the count of
		/// <c><see cref="_sprites"/></c> required.
		/// </summary>
		public const int PHASES = 8;

		private static Spriteset CrippledSprites;

		/// <summary>
		/// The ID for the West sprite in the MonotoneSpriteset.
		/// </summary>
		public const int Quad_WEST = 1; // cf. QuadrantDrawService.Quad_* ->
		/// <summary>
		/// The ID for the North sprite in the MonotoneSpriteset.
		/// </summary>
		public const int Quad_NORTH = 2;
		/// <summary>
		/// The ID for the Floor sprite in the MonotoneSpriteset.
		/// </summary>
		public const int Quad_FLOOR = 3;
		/// <summary>
		/// The ID for the Content sprite in the MonotoneSpriteset.
		/// </summary>
		public const int Quad_CONTENT = 4;

		/// <summary>
		/// The ID of a <c>Tilepart</c> across all loaded terrainsets.
		/// </summary>
		private static int _ordinal = -1;
		internal static void ResetOrdinal()
		{
			_ordinal = -1;
		}
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// The ID of the terrain of this <c>Tilepart</c> in
		/// <c><see cref="MapFile.Terrains">MapFile.Terrains</see></c>
		/// </summary>
		private int _terId = -1;
		#endregion Fields


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
					&& _terId != -1
					&& SpritesetManager.Spritesets.Count > _terId)
				{
					_spritealtr = SpritesetManager.Spritesets[_terId][_altr.Record.Sprite1];
				}
				else
					_spritealtr = null;
			}
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

		private int _setId = -1;
		/// <summary>
		/// The ID of this <c>Tilepart</c> that's unique to the Map across all
		/// allocated terrains. The value is usually set in
		/// <c><see cref="MapFileService.LoadDescriptor()">MapFileService.LoadDescriptor()</see></c>.
		/// </summary>
		/// <remarks><c>SetId</c> is written to the <c><see cref="MapFile"/></c>
		/// as a <c>byte</c>.</remarks>
		public int SetId
		{
			get { return _setId; }
			set { _setId = value; }
		}

		/// <summary>
		/// <c>true</c> if
		/// <c><see cref="McdRecord.HingedDoor">McdRecord.HingedDoor</see></c>
		/// or
		/// <c><see cref="McdRecord.SlidingDoor">McdRecord.SlidingDoor</see></c>
		/// is nonzero.
		/// </summary>
		public bool IsDoor
		{ get; private set; }
		#endregion Properties


		#region Indexers
		/// <summary>
		/// The sprite-array used to display and/or animate this <c>Tilepart</c>.
		/// </summary>
		private XCImage[] _sprites;

		/// <summary>
		/// Gets the sprite at a specified animation-phase.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public XCImage this[int id]
		{
			get { return _sprites[id]; }
		}
		#endregion Indexers


		#region cTor
		/// <summary>
		/// cTor[0]. Creates a standard <c>Tilepart</c>.
		/// </summary>
		/// <param name="id">the id of this <c>Tilepart</c> in its terrain</param>
		/// <param name="record">the <c><see cref="McdRecord"/></c> of this
		/// <c>Tilepart</c></param>
		/// <param name="terid">the ID of this <c>Tilepart's</c> terrain in
		/// <c><see cref="MapFile.Terrains">MapFile.Terrains</see></c> and to
		/// track this <c>Tilepart's</c> ID and sprites per
		/// <c><see cref="SetId"/></c> and <c><see cref="_sprites"/></c>
		/// respectively in MapView; <c>-1</c> if McdView is going to handle the
		/// sprites itself and this <c>Tilepart</c> is not part of a terrainset</param>
		internal Tilepart(
				int id,
				McdRecord record,
				int terid = -1)
		{
			//DSShared.Logfile.Log("Tilepart() terid= " + terid + " id= " + id);

			Id = id;

			Record = record;
			IsDoor = Record.SlidingDoor != 0
				  || Record.HingedDoor  != 0;

			if (terid != -1) // NOTA BENE: _terId shall be -1 and _sprites shall be null for McdView.
			{
				_terId = terid;
				SetId = ++_ordinal;

				//DSShared.Logfile.Log(". SetId= " + SetId);


				_sprites = new XCImage[PHASES]; // for MapView each tilepart contains its own pointers to 8 sprites.

//				Spriteset spriteset = SpritesetManager.Spritesets[_terId];
//				string info = ". valid sprites PRE= "
//						+ (spriteset[Record.Sprite1] != null) + ","
//						+ (spriteset[Record.Sprite2] != null) + ","
//						+ (spriteset[Record.Sprite3] != null) + ","
//						+ (spriteset[Record.Sprite4] != null) + ","
//						+ (spriteset[Record.Sprite5] != null) + ","
//						+ (spriteset[Record.Sprite6] != null) + ","
//						+ (spriteset[Record.Sprite7] != null) + ","
//						+ (spriteset[Record.Sprite8] != null);
//				DSShared.Logfile.Log(info);

				if (IsDoor)
					SetSprite1();
				else
					SetSprites();

//				info = ". valid sprites PST= "
//						+ (_sprites[0].Sprite != null) + ","
//						+ (_sprites[1].Sprite != null) + ","
//						+ (_sprites[2].Sprite != null) + ","
//						+ (_sprites[3].Sprite != null) + ","
//						+ (_sprites[4].Sprite != null) + ","
//						+ (_sprites[5].Sprite != null) + ","
//						+ (_sprites[6].Sprite != null) + ","
//						+ (_sprites[7].Sprite != null);
//				DSShared.Logfile.Log(info);
			}
		}

		/// <summary>
		/// cTor[1]. Creates a blank <c>Tilepart</c> that's ready to go in
		/// McdView (req'd: <c><see cref="Id"/></c>). Also used for crippled
		/// parts on Mapfile load (req'd: <c><see cref="SetId"/></c>).
		/// </summary>
		/// <param name="id"></param>
		/// <param name="setid"></param>
		public Tilepart(int id, int setid = -1)
		{
			Record = new McdRecord(null);

			Id = id;
			SetId = setid;
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
			Spriteset spriteset = SpritesetManager.Spritesets[_terId];

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
			XCImage sprite = SpritesetManager.Spritesets[_terId][Record.Sprite1];
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
			if (IsDoor)
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
			if (IsDoor)
			{
				if (!ani)
				{
					SetSprite1();
				}
				else if (Record.SlidingDoor != 0 || Altr == null)
				{
					SetSprites();
				}
				else
				{
					for (int i = 4; i != PHASES; ++i) // ie. flip between Record.Sprite1 and Altr.Record.Sprite1
						_sprites[i] = _spritealtr;
				}
			}
		}


		/// <summary>
		/// Returns a copy of this <c><see cref="Tilepart"/></c> with a
		/// deep-cloned <c><see cref="Record"/></c> for McdView. But any
		/// referred to sprites and dead/altr <c>Tileparts</c> keep pointers to
		/// their current objects.
		/// <br/><br/>
		/// - classvars
		/// <list type="bullet">
		/// <item><c><see cref="Record"/></c>   (ptr)</item>
		/// <item><c><see cref="_sprites"/></c> (ptr) -> not used in McdView</item>
		/// <item><c><see cref="Dead"/></c>     (ptr)</item>
		/// <item><c><see cref="Altr"/></c>     (ptr)</item>
		/// <item><c><see cref="Id"/></c>       (int)</item>
		/// <item><c><see cref="SetId"/></c>    (int)</item>
		/// <item><c><see cref="_terId"/></c>   (int)  -> not used in McdView</item>
		/// <item><c><see cref="IsDoor"/></c>   (bool) -> not used in McdView</item>
		/// </list>
		/// </summary>
		/// <returns>a new <c><see cref="Tilepart"/></c> for insertion in McdView</returns>
		/// <remarks><c><see cref="_sprites"/></c> shall be <c>null</c> and
		/// <c><see cref="_terId"/></c> shall be <c>-1</c></remarks>
		public Tilepart CreateInsert()
		{
			var part = new Tilepart();

			part.Record = Record.Duplicate();

			part.Dead = Dead; // NOTE: keep these pointers and use their Ids to determine the
			part.Altr = Altr; // part's 'DieTile' and 'Alt_MCD' fields after insertion. (aha!)

			part.Id    = Id;
			part.SetId = SetId;

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
		/// <br/><br/>
		/// This allows the user to invoke <c>MapView.TilepartSubstitution</c>
		/// to shift ids that are above the currently displayable/valid range
		/// down into acceptable values. It's useful only when records have been
		/// removed from <c><see cref="MapFile.Terrains">MapFile.Terrains</see></c>
		/// but there are still records with ids that are higher than any of the
		/// (previously/externally) removed ids.
		/// <br/><br/>
		/// See <c><see cref="MapFile"/>.CreateTile().</c>
		/// <br/><br/>
		/// <c><see cref="McdRecord"/></c> contains the MCD-entries in case you
		/// haven't figured that.
		/// </summary>
		/// <param name="quadrant">the <c><see cref="PartType"/></c> to show
		/// crippled</param>
		/// <remarks>This is strictly a one-way operation! All crippled parts
		/// shall go ~poof~ when the Mapfile is saved.</remarks>
		internal void Cripple(PartType quadrant)
		{
			// NOTE: Assigning 'PartType.Invalid' to the record's 'PartType'
			// shall force it to be listed in TopView's PartslotTest dialog and
			// disallow it from consideration as a valid highid in the
			// TilepartSubstitution dialog.
			Record.PartType = PartType.Invalid;

			if (CrippledSprites == null) // once created the CrippledSprites shall be disposed explicitly when MapView quits.
				CreateCrippledSprites();

			_sprites = new XCImage[PHASES];

			int spriteid;
			switch (quadrant)
			{
				case PartType.Floor:   spriteid = Quad_FLOOR;   break;
				case PartType.West:    spriteid = Quad_WEST;    break;
				case PartType.North:   spriteid = Quad_NORTH;   break;
				case PartType.Content: spriteid = Quad_CONTENT; break;

				default: return;
			}

			for (int i = 0; i != PHASES; ++i)
				_sprites[i] = CrippledSprites[spriteid];
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
				 sprite.Sprite = SpriteService.CreateSprite(
														Spriteset.SpriteWidth32,
														Spriteset.SpriteHeight40,
														bindata,
														sprite.Pal.Table);
			}
		}
		#endregion Methods (static)


		#region Methods (override)
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Tilepart _terId= " + _terId + " _setId= " + _setId;
		}

/*		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string cripples = String.Empty;
			for (int i = 0; i != CrippledSprites.Sprites.Count; ++i)
				cripples += i + ((CrippledSprites.Sprites[i] != null) ? ":true " : ":false ");

			return "Tilepart _terId= " + _terId + " id= " + Id + " _setId= " + SetId
				+ Environment.NewLine
				+ ". partslot= " + Record.PartType
				+ Environment.NewLine
				+ ". CrippledSprites valid= " + cripples
				+ Environment.NewLine
				+ ". valid sprites= "
					+ (_sprites[0].Sprite != null) + ","
					+ (_sprites[1].Sprite != null) + ","
					+ (_sprites[2].Sprite != null) + ","
					+ (_sprites[3].Sprite != null) + ","
					+ (_sprites[4].Sprite != null) + ","
					+ (_sprites[5].Sprite != null) + ","
					+ (_sprites[6].Sprite != null) + ","
					+ (_sprites[7].Sprite != null);
		} */
		#endregion Methods (override)
	}
}
