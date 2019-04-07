using System;

using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class Tilepart
		:
			TilepartBase
	{
		#region Fields & Properties
		private readonly SpriteCollection _spriteset;

		public Tilepart Dead
		{ get; set; }

		public Tilepart Alternate
		{ get; set; }
		#endregion


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
			:
				base(id)
		{
			_spriteset = spriteset;
			Record = record;

			Sprites = new XCImage[8]; // every tile-part contains refs to 8 sprites.
			InitializeSprites();
		}
		#endregion


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
		private void InitializeSprites()
		{
			if (_spriteset != null)
			{
				if (Record.SlidingDoor || Record.HingedDoor)
				{
					for (int i = 0; i != 8; ++i)
						Sprites[i] = _spriteset[Record.Sprite1];
				}
				else
				{
					Sprites[0] = _spriteset[Record.Sprite1];
					Sprites[1] = _spriteset[Record.Sprite2];
					Sprites[2] = _spriteset[Record.Sprite3];
					Sprites[3] = _spriteset[Record.Sprite4];
					Sprites[4] = _spriteset[Record.Sprite5];
					Sprites[5] = _spriteset[Record.Sprite6];
					Sprites[6] = _spriteset[Record.Sprite7];
					Sprites[7] = _spriteset[Record.Sprite8];
				}
			}
		}


		/// <summary>
		/// Toggles this tilepart's array of sprites if it's a door-part.
		/// </summary>
		/// <param name="animate">true to animate</param>
		public void SetDoorSprites(bool animate)
		{
			if (_spriteset != null)
			{
				if (Record.SlidingDoor || Record.HingedDoor)
				{
					if (animate)
					{
						if (Record.SlidingDoor || Alternate == null)
						{
							Sprites[0] = _spriteset[Record.Sprite1];
							Sprites[1] = _spriteset[Record.Sprite2];
							Sprites[2] = _spriteset[Record.Sprite3];
							Sprites[3] = _spriteset[Record.Sprite4];
							Sprites[4] = _spriteset[Record.Sprite5];
							Sprites[5] = _spriteset[Record.Sprite6];
							Sprites[6] = _spriteset[Record.Sprite7];
							Sprites[7] = _spriteset[Record.Sprite8];
						}
						else
						{
							byte alt = Alternate.Record.Sprite1;
							for (int i = 4; i != 8; ++i)
								Sprites[i] = _spriteset[alt];
						}
					}
					else
					{
						for (int i = 0; i != 8; ++i)
							Sprites[i] = _spriteset[Record.Sprite1];
					}
				}
			}
		}

		public void SetDoorToAlternateSprite()
		{
			if (_spriteset != null)
			{
				if (Record.SlidingDoor || Record.HingedDoor)
				{
					byte alt = Alternate.Record.Sprite1;
					for (int i = 0; i != 8; ++i)
						Sprites[i] = _spriteset[alt];
				}
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
		///
		///   Dead
		///   Alternate
		///   _spriteset
		/// </summary>
		/// <returns></returns>
		public Tilepart Clone()
		{
			var part = new Tilepart(
								TerId,
								_spriteset,
								Record.Clone());
			part.Dead      = Dead;
			part.Alternate = Alternate;

			return part;
		}
		#endregion
	}
}
