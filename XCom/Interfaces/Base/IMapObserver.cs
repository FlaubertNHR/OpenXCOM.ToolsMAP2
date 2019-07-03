using System;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Interface for 'MapObserverControl' and 'MapObserverControl_TopPanels'.
	/// </summary>
	public interface IMapObserver
	{
		MapFileBase MapBase
		{ set; get;}

		Dictionary<string, IMapObserver> ObserverPanels
		{ get; }

		void OnSelectLocationObserver(SelectLocationEventArgs args);
		void OnSelectLevelObserver(   SelectLevelEventArgs    args);
	}


	/// <summary>
	/// EventArgs with a MapLocation and MapTile for when a LocationSelected
	/// event fires.
	/// </summary>
	public sealed class SelectLocationEventArgs
		:
			EventArgs
	{
		private readonly MapLocation _location;
		public MapLocation Location
		{
			get { return _location; }
		}

		private readonly MapTileBase _tile;
		public MapTileBase Tile
		{
			get { return _tile; }
		}

		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="tile"></param>
		internal SelectLocationEventArgs(MapLocation location, MapTileBase tile)
		{
			_location = location;
			_tile     = tile;
		}
	}

	/// <summary>
	/// EventArgs for when a LevelChanged event fires.
	/// </summary>
	public sealed class SelectLevelEventArgs
		:
			EventArgs
	{
		private readonly int _level;
		public int Level
		{
			get { return _level; }
		}

		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="level">the new level</param>
		internal SelectLevelEventArgs(int level)
		{
			_level = level;
		}
	}
}
