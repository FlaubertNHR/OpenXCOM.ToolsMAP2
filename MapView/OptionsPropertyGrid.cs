using System;
using System.Windows.Forms;


namespace MapView
{
	/// <summary>
	/// Derived class of a <c>PropertyGrid</c>.
	/// </summary>
	public sealed class OptionsPropertyGrid
		:
			PropertyGrid
	{
		#region Properties (override)
		/// <summary>
		/// Prevents flicker.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000; // enable 'WS_EX_COMPOSITED'
				return cp;
			}
		}
		#endregion Properties (override)


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
//			//Logfile.Log("SetSelectedValue() val= " + val);
//			if (SelectedGridItem != null && SelectedObject != null)
//				SelectedGridItem.PropertyDescriptor.SetValue(SelectedObject, val); // no fucking guff.
//		}
//		#endregion Methods
	}
}
