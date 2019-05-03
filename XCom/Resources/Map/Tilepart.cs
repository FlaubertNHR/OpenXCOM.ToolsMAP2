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

		public Tilepart Alternate
		{ get; set; }

		// TODO: The fact that the spriteset points to a "sprite" and this
		// tilepart points to a "sprite" causes a glitch when changing "that"
		// sprite. They ought be kept consistent since there is an awkward
		// sort of latency-effect happening on refresh.

		private SpriteCollection Spriteset
		{ get; set; }

		/// <summary>
		/// Gets the sprite-array used to animate this tile.
		/// </summary>
		public XCImage[] Sprites
		{ get; set; }

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
		/// allocated terrains. The value is set in MapFileChild..cTor.
		/// IMPORTANT: The 'SetId' is written to the Mapfile (as a byte).
		/// </summary>
		public int SetId
		{ get; internal set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="spriteset"></param>
		/// <param name="record"></param>
		public Tilepart(
				int id,
				SpriteCollection spriteset,
				McdRecord record)
		{
			TerId = id;
			SetId = -1;

			Spriteset = spriteset;

			Record = record;

			Sprites = new XCImage[8]; // every tile-part contains refs to 8 sprites.

			if (Spriteset != null)
				InitSprites();
		}
		#endregion cTor


		#region Methods
		// re. Animating Sprites
		// Basically this is how animations operate. For *any* animations to
		// happen the Animation option has to be turned on. Non-door sprites
		// always keep their array of sprites and will cycle because turning on
		// the Animation option starts a timer that does that (see
		// 'MapView.MainViewPanel').
		// 
		// UfoDoor sprites will animate when the Animation option is on and the
		// Doors option is turned on; but whether or not they animate is
		// controlled by setting their sprite-arrays to either the first image
		// or an array of images, like non-door records do.
		// 
		// HumanDoors, which also need the Animation option on to animate as
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
					if (Record.SlidingDoor || Alternate == null)
					{
						SpritesToPhases();
					}
					else
					{
						byte altr = Alternate.Record.Sprite1;
						for (int i = 4; i != 8; ++i)
							Sprites[i] = Spriteset[altr];
					}
				}
				else
					SpritesToFirstPhase();
			}
		}

		/// <summary>
		/// Sets this tilepart's sprites to the first phase of its Alternate
		/// part. Is for doors only.
		/// </summary>
		public void SpritesToAlternate()
		{
			if (Spriteset != null
				&& (Record.SlidingDoor || Record.HingedDoor))
			{
				byte altr = Alternate.Record.Sprite1;
				for (int i = 0; i != 8; ++i)
					Sprites[i] = Spriteset[altr];
			}
		}


		/// <summary>
		/// Returns a copy of this Tilepart with a deep-cloned Record.
		/// But any referred to anisprites and dead/alternate tileparts maintain
		/// their current objects.
		/// - classvars:
		///   Record
		///   Sprites
		///   TerId
		///   SetId = -1
		///   Spriteset
		///
		///   Dead
		///   Alternate
		/// </summary>
		/// <param name="spriteset">the spriteset to ref for the cloned part; if
		/// null use this part's spriteset</param>
		/// <returns></returns>
		public Tilepart Clone(SpriteCollection spriteset = null)
		{
			if (spriteset == null)
				spriteset = Spriteset;

			var part = new Tilepart(
								TerId,
								spriteset,
								Record.Clone());
			SpritesToPhases();

			part.Dead      = Dead;
			part.Alternate = Alternate;

			return part;
		}
		#endregion Methods
	}
}
