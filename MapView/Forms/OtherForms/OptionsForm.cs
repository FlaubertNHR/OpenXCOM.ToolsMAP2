using System;
using System.ComponentModel;
using System.Windows.Forms;

using DSShared.Windows;

using MapView.Forms.MapObservers.RouteViews;
using MapView.Forms.MapObservers.TileViews;
using MapView.Forms.MapObservers.TopViews;


namespace MapView
{
	/// <summary>
	/// The Options form.
	/// </summary>
	internal sealed class OptionsForm
		:
			Form
	{
		internal enum OptionableType
		{
			MainView,
			TileView,
			TopView,
			RouteView
		}

		#region Fields (static)
		private const string GridViewEdit = "GridViewEdit"; // ie. editfield
		#endregion Fields (static)


		#region cTor
		/// <summary>
		/// cTor. Constructs an OptionsForm.
		/// </summary>
		/// <param name="o">a class-object w/ Properties that are optionable</param>
		/// <param name="options">its Options</param>
		/// <param name="type">its optionable type</param>
		internal OptionsForm(
				object o,
				Options options,
				OptionableType type)
		{
			InitializeComponent();

			propertyGrid.Options = options;

			switch (type)
			{
				case OptionableType.MainView:
					propertyGrid.SelectedObject = o as MainViewOptionables;
					break;
				case OptionableType.TileView:
					propertyGrid.SelectedObject = o as TileViewOptionables;
					break;
				case OptionableType.TopView:
					propertyGrid.SelectedObject = o as TopViewOptionables;
					break;
				case OptionableType.RouteView:
					propertyGrid.SelectedObject = o as RouteViewOptionables;
					break;
			}

			RegistryInfo.RegisterProperties(this); // NOTE: 1 metric for all four types
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Handles command-key processing. Closes this form when either of
		/// [Esc] or [Ctrl+o] is pressed.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Control | Keys.O: // non-whiteman code-page users beware ... Mista Kurtz, he dead. GLORY TO THE LOST CAUSE!!! yeah whatever.
				case Keys.Escape:
					if (!FindFocusedControl().GetType().ToString().Contains(GridViewEdit))
					{
						Close();
						return true;
					}
					break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion (override)


		#region Events
		/// <summary>
		/// Handles this form's VisibleChanged event.
		/// @note The cached metric of an OptionsForm is updated every time the
		/// form is hidden, unlike other viewers that update their metrics only
		/// when MapView quits.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnVisibleChanged(object sender, EventArgs e)
		{
			if (!Visible)
				RegistryInfo.UpdateRegistry(this, true); // NOTE: 1 metric for all four types
		}
		#endregion Events


		#region Methods
		private Control FindFocusedControl()
		{
			Control control = null;

			var container = this as ContainerControl;
			while (container != null)
			{
				control = container.ActiveControl;
				container = control as ContainerControl;
			}
			return control;
		}
		#endregion Methods



		#region Designer
		private IContainer components = null;

		internal CompositedPropertyGrid propertyGrid;

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


		private void InitializeComponent()
		{
			this.propertyGrid = new CompositedPropertyGrid();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(592, 374);
			this.propertyGrid.TabIndex = 0;
			// 
			// OptionsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(592, 374);
			this.Controls.Add(this.propertyGrid);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new System.Drawing.Size(500, 300);
			this.Name = "OptionsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Custom PropertyGrid";
			this.VisibleChanged += new System.EventHandler(this.OnVisibleChanged);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}



	/// <summary>
	/// Derived class for PropertyGrid.
	/// </summary>
	internal sealed class CompositedPropertyGrid
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
		protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
		{
			base.OnPropertyValueChanged(e);

			string key = e.ChangedItem.PropertyDescriptor.Name;
			Option option = Options[key];
			option.doUpdate(key, (option.Value = e.ChangedItem.Value));
		}
		#endregion Events (override)


		#region Methods
		internal void SetSelectedValue(object val)
		{
			SelectedGridItem.PropertyDescriptor.SetValue(SelectedObject, val); // no fucking guff.
		}
		#endregion Methods
	}
}
