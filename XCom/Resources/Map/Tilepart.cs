using System;

using XCom.Interfaces;


namespace XCom
{
	public sealed class Tilepart
	{
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

		private SpriteCollection Spriteset
		{ get; set; }

		/// <summary>
		/// Gets the sprite-array used to animate this tile.
		/// TODO: This should never have happened; there should be no pointers
		/// to sprites in a 'Tilepart'. The sprites need to be retrieved
		/// directly from its 'SpriteCollection' by 'Record' (int)Phase*
		/// on-the-fly.
		/// 
		/// But unfortunately that incorrect mentality is deeply ingrained in
		/// the design of the code.
		/// </summary>
		public XCImage[] Sprites
		{ get; private set; }

		/// <summary>
		/// Gets a sprite at the specified animation frame.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public XCImage this[int id]
		{
			get { return Sprites[id]; }
			set { Sprites[id] = value; }
//			get { return Spriteset[getSpriteId(id)]; }
//			set { Spriteset[getSpriteId(id)] = value; }
		}
/*		private int getSpriteId(int id)
		{
			switch (id)
			{
				default: return Record.Sprite1; //case 0
				case 1:  return Record.Sprite2;
				case 2:  return Record.Sprite3;
				case 3:  return Record.Sprite4;
				case 4:  return Record.Sprite5;
				case 5:  return Record.Sprite6;
				case 6:  return Record.Sprite7;
				case 7:  return Record.Sprite8;
			}
		} */

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
		/// cTor[1]. Creates a standard Tilepart.
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
				Sprites = new XCImage[8];	// for MapView a part contains its own pointers to 8 sprites.
											// - animations and doors toggle basically
				InitSprites();
			}
		}

		/// <summary>
		/// cTor[2]. Creates a blank part that's ready to go in McdView.
		/// </summary>
		public Tilepart(int id)
		{
			TerId = id;
			Record = new McdRecord();
		}

		/// <summary>
		/// cTor[3]. Creates a blank part for CreateInsert().
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
		// 'MapView.XCMainWindow'     for options
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
				SpritesToPhases();
			}
			else
				SpritesToFirstPhase();
		}

		/// <summary>
		/// Sets this tilepart's sprites in accord with its record's
		/// sprite-phases.
		/// </summary>
		public void SpritesToPhases()
		{
			int spriteId;
			for (int i = 0; i != 8; ++i)
			{
				switch (i)
				{
					default: spriteId = Record.Sprite1; break; //case 0
					case 1:  spriteId = Record.Sprite2; break;
					case 2:  spriteId = Record.Sprite3; break;
					case 3:  spriteId = Record.Sprite4; break;
					case 4:  spriteId = Record.Sprite5; break;
					case 5:  spriteId = Record.Sprite6; break;
					case 6:  spriteId = Record.Sprite7; break;
					case 7:  spriteId = Record.Sprite8; break;
				}
				Sprites[i] = Spriteset[spriteId];
			}
		}

		/// <summary>
		/// Sets this tilepart's sprites to its record's first sprite-phase.
		/// </summary>
		private void SpritesToFirstPhase()
		{
			for (int i = 0; i != 8; ++i)
				Sprites[i] = Spriteset[Record.Sprite1];
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
						SpritesToPhases();
					}
					else
					{
						byte altr = Altr.Record.Sprite1;
						for (int i = 4; i != 8; ++i)
							Sprites[i] = Spriteset[altr];
					}
				}
				else
					SpritesToFirstPhase();
			}
		}

		/// <summary>
		/// Sets this tilepart's sprites to the first phase of its Altr part. Is
		/// for doors only.
		/// </summary>
		public void SpritesToAlternate()
		{
			if (Spriteset != null
				&& (Record.SlidingDoor || Record.HingedDoor))
			{
				byte altr = Altr.Record.Sprite1;
				for (int i = 0; i != 8; ++i)
					Sprites[i] = Spriteset[altr];
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
		#endregion Methods
	}
}
