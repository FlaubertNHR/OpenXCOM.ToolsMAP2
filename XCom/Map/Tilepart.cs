using System;
using System.Reflection;


namespace XCom
{
	public sealed class Tilepart
	{
		#region Fields (static)
		private const int PHASECOUNT = 8;

		private static SpriteCollection MonotoneSprites;
		#endregion Fields (static)


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

		// TODO: The fact that the spriteset points to a "sprite" and this
		// tilepart points to a "sprite" causes a glitch when changing "that"
		// sprite. They ought be kept consistent since there is an awkward
		// sort of latency-effect happening on refresh.

		/// <summary>
		/// WHY THE FUCK DOES EVERY TILEPART STORE THE ENTIRE SPRITESET.
		/// Yes, this is The spriteset. Each tilepart has a pointer to it ...
		/// psst if you really want the spritesets get it in ResourceInfo.
		/// </summary>
		private SpriteCollection Spriteset
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
		public XCImage[] Sprites
		{ get; private set; }

		/// <summary>
		/// Gets the sprite at a specified animation phase.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public XCImage this[int id]
		{
			get { return Sprites[id]; }
			set { Sprites[id] = value; }
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

			if ((Spriteset = spriteset) != null) // nota bene: 'Spriteset' and 'Sprites' shall be null for McdView.
			{
				Sprites = new XCImage[PHASECOUNT];	// for MapView a part contains its own pointers to 8 sprites.
													// - animations and doors toggle basically
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
			if (!Record.SlidingDoor && !Record.HingedDoor)
			{
				SetSpritesArray();
			}
			else
				SetSprite1();
		}

		/// <summary>
		/// Sets this tilepart's sprites in accord with its record's
		/// sprite-phases.
		/// </summary>
		private void SetSpritesArray()
		{
			Sprites[0] = Spriteset[Record.Sprite1];
			Sprites[1] = Spriteset[Record.Sprite2];
			Sprites[2] = Spriteset[Record.Sprite3];
			Sprites[3] = Spriteset[Record.Sprite4];
			Sprites[4] = Spriteset[Record.Sprite5];
			Sprites[5] = Spriteset[Record.Sprite6];
			Sprites[6] = Spriteset[Record.Sprite7];
			Sprites[7] = Spriteset[Record.Sprite8];
		}

		/// <summary>
		/// Sets this tilepart's sprites to its record's first sprite-phase.
		/// </summary>
		private void SetSprite1()
		{
			for (int i = 0; i != PHASECOUNT; ++i)
				Sprites[i] = Spriteset[Record.Sprite1];
		}

		/// <summary>
		/// Sets this tilepart's sprites to the first phase of its Altr part. Is
		/// for doors only.
		/// </summary>
		public void SetSprite1_alt()
		{
			if (Spriteset != null
				&& (Record.SlidingDoor || Record.HingedDoor))
			{
				byte altr = Altr.Record.Sprite1;
				for (int i = 0; i != PHASECOUNT; ++i)
					Sprites[i] = Spriteset[altr];
			}
		}

		/// <summary>
		/// Toggles this tilepart's array of sprites if it's a door-part.
		/// </summary>
		/// <param name="animate">true to animate</param>
		public void ToggleDoorSprites(bool animate)
		{
			if (Spriteset != null
				&& (Record.SlidingDoor || Record.HingedDoor))
			{
				if (animate)
				{
					if (Record.SlidingDoor || Altr == null)
					{
						SetSpritesArray();
					}
					else
					{
						byte altr = Altr.Record.Sprite1;
						for (int i = 4; i != PHASECOUNT; ++i) // ie. flip between Sprite1 and Altr.Sprite1
							Sprites[i] = Spriteset[altr];
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

			Sprites = new XCImage[PHASECOUNT];

			switch (slot)
			{
				case PartType.Floor:
					for (int i = 0; i != PHASECOUNT; ++i)
						Sprites[i] = MonotoneSprites[3];
					break;

				case PartType.West:
					for (int i = 0; i != PHASECOUNT; ++i)
						Sprites[i] = MonotoneSprites[1];
					break;

				case PartType.North:
					for (int i = 0; i != PHASECOUNT; ++i)
						Sprites[i] = MonotoneSprites[2];
					break;

				case PartType.Content:
					for (int i = 0; i != PHASECOUNT; ++i)
						Sprites[i] = MonotoneSprites[4];
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
														ResourceInfo.TAB_WORD_LENGTH_2,
														bytesPck,
														bytesTab);

					foreach (var sprite in MonotoneSprites.Sprites) // change nontransparent pixels to color ->
					{
						for (int i = 0; i != sprite.Bindata.Length; ++i)
						{
							if (sprite.Bindata[i] != 0)
								sprite.Bindata[i] = (byte)(96); // light brown/yellowy
						}
						sprite.Sprite = BitmapService.CreateColored(
																XCImage.SpriteWidth32,
																XCImage.SpriteHeight40,
																sprite.Bindata,
																sprite.Pal.ColorTable);
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
													image.Pal.ColorTable);

			image.SpriteT = BitmapService.CreateColored(
													XCImage.SpriteWidth32,
													XCImage.SpriteHeight40,
													image.Bindata,
													image.Pal.GrayScaled.ColorTable);

			Sprites = new XCImage[PHASECOUNT];
			for (int i = 0; i != PHASECOUNT; ++i)
				Sprites[i] = image;

//			TerId = -1;
		} */
