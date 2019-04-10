using System;
using System.Collections.Generic;
using System.ComponentModel;

using DSShared.Windows;

using XCom.Interfaces.Base;


namespace MapView
{
	/// <summary>
	/// Inherited by TopViewPanelParent, TopViewPanel, QuadrantPanel.
	/// </summary>
	internal class MapObserverControl1
		:
			DoubleBufferedControl,
			IMapObserver
	{
		#region IMapObserver requirements
		// NOTE: This is not used.
		private readonly Dictionary<string, IMapObserver> _panels =
					 new Dictionary<string, IMapObserver>();
		[Browsable(false)]
		public Dictionary<string, IMapObserver> Panels
		{
			get { return _panels; }
		}

		private MapFileBase _mapBase;
		[Browsable(false), DefaultValue(null)]
		public virtual MapFileBase MapBase
		{
			get { return _mapBase; }
			set
			{
				_mapBase = value;
				Refresh();
			}
		}

		/// <summary>
		/// Satisfies IMapObserver. Used by QuadrantPanel but disabled in
		/// TopViewPanelParent.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLocationSelectedObserver(LocationSelectedEventArgs args)
		{}

		/// <summary>
		/// Satisfies IMapObserver. Used by QuadrantPanel and does not exist in
		/// TopViewPanelParent.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnLevelChangedObserver(LevelChangedEventArgs args)
		{}

//		/// <summary>
//		/// This is here only to satisfy IMapObserver requirements.
//		/// </summary>
//		public RegistryInfo RegistryInfo
//		{ get; set; }

//		/// <summary>
//		/// This is here only to satisfy IMapObserver requirements.
//		/// </summary>
//		private RegistryInfo _regInfo;
//		/// <summary>
//		/// This stuff is here only to satisfy IMapObserver requirements.
//		/// </summary>
//		[Browsable(false), DefaultValue(null)]
//		public RegistryInfo RegistryInfo
//		{
//			get { return _regInfo; }
//			set
//			{
//				_regInfo = value;
//				value.RegistryLoadEvent += (sender, e) => OnRegistrySettingsLoad(e);
//				value.RegistrySaveEvent += (sender, e) => OnRegistrySettingsSave(e);
//			}
//		}
		#endregion


//		protected virtual void OnRegistrySettingsLoad(RegistryEventArgs e){}
//		protected virtual void OnRegistrySettingsSave(RegistryEventArgs e){}
	}
}
