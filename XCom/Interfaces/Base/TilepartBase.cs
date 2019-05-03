using System;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Provides all the necessary information to animate a tilepart. No it
	/// doesn't.
	/// </summary>
	public class TilepartBase
	{
		#region Properties
		/// <summary>
		/// The object that has information about the IG mechanics of this tile.
		/// </summary>
		public McdRecord Record
		{ get; protected set; }


		// TODO: The fact that the spriteset points to a "sprite" and this
		// tilepart points to a "sprite" causes a glitch when changing "that"
		// sprite. They ought be kept consistent since there is an awkward
		// sort of latency-effect happening on refresh.

		/// <summary>
		/// Gets the sprite-array used to animate this tile.
		/// </summary>
		public XCImage[] Sprites
		{ get; protected set; }

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


		protected SpriteCollection Spriteset
		{ get; private set; }

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
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// Instantiates a blank tile.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="spriteset"></param>
		protected TilepartBase(
				int id,
				SpriteCollection spriteset)
		{
			TerId = id;
			SetId = -1;

			Spriteset = spriteset;
		}
		#endregion cTor
	}
}
