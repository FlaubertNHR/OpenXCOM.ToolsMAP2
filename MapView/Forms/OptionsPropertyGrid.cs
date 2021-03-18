using System;
using System.Windows.Forms;

using DSShared.Controls;


namespace MapView
{
	/// <summary>
	/// Derived class for CompositedPropertyGrid.
	/// </summary>
	public sealed class OptionsPropertyGrid
		:
			CompositedPropertyGrid
	{
		#region Properties
		internal Options Options
		{ private get; set; }
		#endregion Properties


		#region Events (override)
		/// <summary>
		/// Handles changing a value in a property panel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
		{
			base.OnPropertyValueChanged(e);

			string key = e.ChangedItem.PropertyDescriptor.Name;
			Options[key].SetValue(key, e.ChangedItem.Value);
		}
		#endregion Events (override)


//		#region Methods
//		internal void SetSelectedValue(object val)
//		{
//			//LogFile.WriteLine("SetSelectedValue() val= " + val);
//			if (SelectedGridItem != null && SelectedObject != null)
//				SelectedGridItem.PropertyDescriptor.SetValue(SelectedObject, val); // no fucking guff.
//		}
//		#endregion Methods
	}
}
