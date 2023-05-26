using System;


namespace XCom
{
	/// <summary>
	/// A tile in the Tileset consisting of four
	/// <c><see cref="Tilepart">Tileparts</see></c> and a
	/// <c><see cref="RouteNode"/></c>.
	/// </summary>
	public sealed class MapTile
	{
		#region Fields (static)
		public const int QUADS = 4;
		#endregion Fields (static)


		#region Properties
		public Tilepart Floor   { get; set; }
		public Tilepart West    { get; set; }
		public Tilepart North   { get; set; }
		public Tilepart Content { get; set; }

		public Tilepart this[PartType slot]
		{
			get
			{
				switch (slot)
				{
					case PartType.Floor:   return Floor;
					case PartType.West:    return West;
					case PartType.North:   return North;
					case PartType.Content: return Content;
				}
				return null;
			}
			set
			{
				switch (slot)
				{
					case PartType.Floor:   Floor   = value; break;
					case PartType.West:    West    = value; break;
					case PartType.North:   North   = value; break;
					case PartType.Content: Content = value; break;
				}
			}
		}

		public RouteNode Node
		{ get; set; }

		/// <summary>
		/// This <c>MapTile</c> is flagged as <c>Occulted</c> if it has tiles
		/// with ground-parts above and to the south and east. Is used to
		/// optimize the draw-cycle.
		/// </summary>
		public bool Occulted
		{ get; set; }

		/// <summary>
		/// This <c>MapTile</c> is flagged as <c>Vacant</c> if it doesn't have
		/// any <c><see cref="Tilepart">Tileparts</see></c>. Is used to optimize
		/// the draw-cycle as well as by <c>MapInfoDialog</c> and
		/// <c>TileslotSubstitution</c>.
		/// </summary>
		public bool Vacant
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Creates a MapTile with given parts.
		/// </summary>
		/// <param name="floor"></param>
		/// <param name="west"></param>
		/// <param name="north"></param>
		/// <param name="content"></param>
		public MapTile(
				Tilepart floor,
				Tilepart west,
				Tilepart north,
				Tilepart content)
		{
			Floor   = floor;
			West    = west;
			North   = north;
			Content = content;

			Vacancy();
		}

		/// <summary>
		/// cTor[1]. Creates a blank MapTile.
		/// </summary>
		public MapTile()
		{
			Floor   =
			West    =
			North   =
			Content = null;

			Vacant = true;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Sets the tile as <c><see cref="Vacant"/></c> if it has no tileparts.
		/// </summary>
		public void Vacancy()
		{
			Vacant = Floor   == null
				  && West    == null
				  && North   == null
				  && Content == null;
		}
		#endregion Methods
	}
}
