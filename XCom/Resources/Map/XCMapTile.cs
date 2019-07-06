using System;
using System.Collections.Generic;

using XCom.Interfaces.Base;


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


	public sealed class XCMapTile
		:
			MapTileBase
	{
		#region Properties
		private Tilepart _floor;
		public Tilepart Floor
		{
			get { return _floor; }
			set { SetQuadrantPart(QuadrantType.Floor, value); }
		}

		private Tilepart _west;
		public Tilepart West
		{
			get { return _west; }
			set { SetQuadrantPart(QuadrantType.West, value); }
		}

		private Tilepart _north;
		public Tilepart North
		{
			get { return _north; }
			set { SetQuadrantPart(QuadrantType.North, value); }
		}

		private Tilepart _content;
		public Tilepart Content
		{
			get { return _content; }
			set { SetQuadrantPart(QuadrantType.Content, value); }
		}

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
			set { SetQuadrantPart(quad, value); }
		}

		/// <summary>
		/// @note This is used only by MapFileBase.SaveGifFile().
		/// </summary>
		public override Tilepart[] UsedParts
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
		/// Used only by 'MapInfoOutputBox'.
		/// </summary>
		public bool Vacant
		{ get; private set; }

		/// <summary>
		/// Creates and returns a vacant tile.
		/// </summary>
		public static XCMapTile VacantTile
		{
			get
			{
				var tile = new XCMapTile(null, null, null, null);
				tile.Vacant = true;
				return tile;
			}
		}
		#endregion Properties


		#region cTor
		public XCMapTile(
				Tilepart floor,
				Tilepart west,
				Tilepart north,
				Tilepart content)
		{
			_floor   = floor; // NOTE: Don't even try ... don't even think about it.
			_west    = west;
			_north   = north;
			_content = content;

			Vacancy();
		}
		#endregion cTor


		#region Methods
		private void SetQuadrantPart(QuadrantType quad, Tilepart part)
		{
			switch (quad)
			{
				case QuadrantType.Floor:   _floor   = part; break;
				case QuadrantType.West:    _west    = part; break;
				case QuadrantType.North:   _north   = part; break;
				case QuadrantType.Content: _content = part; break;
			}
		}

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
