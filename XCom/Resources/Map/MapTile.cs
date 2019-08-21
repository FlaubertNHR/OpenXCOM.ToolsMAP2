using System;
using System.Collections.Generic;


namespace XCom
{
	public enum QuadrantType
	{
		None    = -1,
		Floor   =  0,
		West    =  1,
		North   =  2,
		Content =  3
	};


	/// <summary>
	/// A tile in the Tileset consisting of four parts.
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

		public Tilepart this[QuadrantType quad]
		{
			get
			{
				switch (quad)
				{
					case QuadrantType.Floor:   return Floor;
					case QuadrantType.West:    return West;
					case QuadrantType.North:   return North;
					case QuadrantType.Content: return Content;
				}
				return null;
			}
			set
			{
				switch (quad)
				{
					case QuadrantType.Floor:   Floor   = value; break;
					case QuadrantType.West:    West    = value; break;
					case QuadrantType.North:   North   = value; break;
					case QuadrantType.Content: Content = value; break;
				}
			}
		}

		/// <summary>
		/// @note This is used only by MainViewF.Screenshot().
		/// </summary>
		public Tilepart[] UsedParts
		{
			get
			{
				var parts = new List<Tilepart>();

				if (Floor   != null) parts.Add(Floor);
				if (West    != null) parts.Add(West);
				if (North   != null) parts.Add(North);
				if (Content != null) parts.Add(Content);

				return parts.ToArray();
			}
		}

		public RouteNode Node
		{ get; set; }

		/// <summary>
		/// A tile is flagged as occulted if it has tiles with ground-parts
		/// above and to the south and east. Is used to optimize the draw-cycle.
		/// </summary>
		public bool Occulted
		{ get; set; }

		/// <summary>
		/// a tile is flagged as vacant if it has no tileparts. Is used to
		/// optimize the draw-cycle as well as by MapInfoDialog.
		/// </summary>
		public bool Vacant
		{ get; private set; }

		/// <summary>
		/// Creates and returns a vacant tile.
		/// </summary>
		public static MapTile VacantTile
		{
			get
			{
				var tile = new MapTile(null,null,null,null);
				tile.Vacant = true;
				return tile;
			}
		}
		#endregion Properties


		#region cTor
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
		#endregion cTor


		#region Methods
		/// <summary>
		/// Sets the tile as Vacant if it has no tileparts.
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
