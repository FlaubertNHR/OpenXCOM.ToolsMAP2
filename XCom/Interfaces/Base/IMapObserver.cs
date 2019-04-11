using System;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Parent of MapObserverControl0 and MapObserverControl1.
	/// </summary>
	public interface IMapObserver
	{
		MapFileBase MapBase
		{ set; get;}

		// NOTE: This is not even used by MapObserverControl1 - only by
		// MapObserverControl0 (for TopViewPanel and QuadrantsPanel).
		Dictionary<string, IMapObserver> Panels
		{ get; }

//		DSShared.Windows.RegistryInfo RegistryInfo
//		{ get; set; }


		void OnSelectLocationObserver(SelectLocationEventArgs args);
		void OnSelectLevelObserver(SelectLevelEventArgs args);
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

		private readonly MapTileBase _baseTile;
		public MapTileBase SelectedTile
		{
			get { return _baseTile; }
		}

		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="baseTile"></param>
		internal SelectLocationEventArgs(MapLocation location, MapTileBase baseTile)
		{
			_location = location;
			_baseTile = baseTile;
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
