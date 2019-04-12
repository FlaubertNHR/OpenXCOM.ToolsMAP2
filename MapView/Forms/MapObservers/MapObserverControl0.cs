using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

//using DSShared.Windows;

using MapView.Forms.MainWindow;

using XCom.Interfaces.Base;


namespace MapView
{
	/// <summary>
	/// Inherited by TopView, TileView, RouteView.
	/// </summary>
	internal class MapObserverControl0
		:
			UserControl,
			IMapObserver
	{
		#region IMapObserver requirements
		private readonly Dictionary<string, IMapObserver> _panels =
					 new Dictionary<string, IMapObserver>();
		public Dictionary<string, IMapObserver> Panels
		{
			get { return _panels; }
		}

		private MapFileBase _base;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual MapFileBase MapBase
		{
			get { return _base; }
			set
			{
				_base = value;
				Refresh();
			}
		}

/*		private RegistryInfo _regInfo;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RegistryInfo RegistryInfo
		{
			get { return _regInfo; } // NOTE: not used. Only satisfies IMapObserver requirement.
			set
			{
				_regInfo = value;
//				value.RegistryLoadEvent += (sender, e) => OnExtraRegistrySettingsLoad(e);
//				value.RegistrySaveEvent += (sender, e) => OnExtraRegistrySettingsSave(e);
			}
		} */

		/// <summary>
		/// Satisfies IMapObserver.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnSelectLocationObserver(SelectLocationEventArgs args)
		{
			Refresh();
		}

		/// <summary>
		/// Satisfies IMapObserver.
		/// </summary>
		/// <param name="args"></param>
		public virtual void OnSelectLevelObserver(SelectLevelEventArgs args)
		{
			Refresh();
		}
		#endregion IMapObserver requirements


		#region Properties
		internal Options Options
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// Invoked by TopView, TileView, RouteView.
		/// </summary>
		public MapObserverControl0()
		{
			Options = new Options();
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Scrolls the z-axis for TopView and RouteView.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if      (e.Delta < 0) MapBase.LevelUp();
			else if (e.Delta > 0) MapBase.LevelDown();

			ViewerFormsManager.ToolFactory.SetLevelDownButtonsEnabled(MapBase.Level != MapBase.MapSize.Levs - 1);
			ViewerFormsManager.ToolFactory.SetLevelUpButtonsEnabled(  MapBase.Level != 0);
		}
		#endregion Events (override)


		#region Methods
		internal void ForceMousewheel(MouseEventArgs e)
		{
			OnMouseWheel(e);
		}
		#endregion Methods


		#region Methods (virtual)
		internal protected virtual void LoadControl0Options()
		{}

//		/// <summary>
//		/// Currently implemented only to load TopView's visible-quadrants menu.
//		/// </summary>
//		/// <param name="e"></param>
//		protected virtual void OnExtraRegistrySettingsLoad(RegistryEventArgs e)
//		{}
//		/// <summary>
//		/// Currently implemented only to save TopView's visible-quadrants menu.
//		/// </summary>
//		/// <param name="e"></param>
//		protected virtual void OnExtraRegistrySettingsSave(RegistryEventArgs e)
//		{}
		#endregion Methods (virtual)
	}
}
