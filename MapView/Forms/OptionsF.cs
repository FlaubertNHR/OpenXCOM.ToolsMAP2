using System;
using System.Reflection;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;
using MapView.Forms.Observers;


namespace MapView
{
	/// <summary>
	/// The viewer-type.
	/// <list type="bullet">
	/// <item>MainView</item>
	/// <item>TileView</item>
	/// <item>TopView</item>
	/// <item>RouteView</item>
	/// </list>
	/// </summary>
	internal enum OptionableType
	{
		MainView,	// 0
		TileView,	// 1
		TopView,	// 2
		RouteView	// 3
	}


	/// <summary>
	/// <c><see cref="MainViewF._foptions">MainViewF</see></c>,
	/// <c><see cref="TileView._foptions">TileView</see></c>,
	/// <c><see cref="TopView._foptions">TopView</see></c>,
	/// <c><see cref="RouteView._foptions">RouteView</see></c> each get their
	/// own <c>OptionsF</c>.
	/// </summary>
	/// <remarks><c>TopView</c> and <c>RouteView</c> share their respective
	/// <c>OptionsF</c> with
	/// <c><see cref="TopRouteViewForm.ControlTop">TopRouteViewForm.ControlTop</see></c> and
	/// <c><see cref="TopRouteViewForm.ControlRoute">TopRouteViewForm.ControlRoute</see></c>.</remarks>
	internal sealed class OptionsF
		:
			Form
	{
		#region Fields (static)
		private const string GridViewEdit = "GridViewEdit";	// the currently edited field
		private const string DocComment   = "DocComment";	// the Description area
		private const string userSized    = "userSized";	// tells .net that the Description area has been/can be resized
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// The viewer that this belongs to as an <c><see cref="OptionableType"/></c>.
		/// </summary>
		private OptionableType _oType;

		/// <summary>
		/// The Description area control - used to get/set each viewers'
		/// 'DescriptionHeight' option.
		/// </summary>
		/// <remarks>.net appears to handle heights that are too large etc okay.</remarks>
		private Control _desc;

		/// <summary>
		/// <c>true</c> bypasses eventhandlers during instantiation.
		/// </summary>
		private bool _init;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor. Constructs this <c>OptionsF</c>.
		/// </summary>
		/// <param name="o">a class-object w/ Properties that are optionable</param>
		/// <param name="options">its Options</param>
		/// <param name="oType">its optionable type</param>
		internal OptionsF(
				object o,
				Options options,
				OptionableType oType)
		{
			_init = true;
			InitializeComponent();

			foreach (Control control in propertyGrid.Controls)
			if (control.GetType().Name == DocComment)
			{
				_desc = control;
				break;
			}
			_desc.SizeChanged += OnDescriptionSizeChanged;

			_desc.GetType().BaseType.GetField(userSized, BindingFlags.Instance
													   | BindingFlags.NonPublic).SetValue(_desc, true);

			propertyGrid.Options = options;

			switch (_oType = oType)
			{
				case OptionableType.MainView:
					propertyGrid.SelectedObject = o as MainViewOptionables;
					_desc.Height = MainViewF.Optionables.DescriptionHeight;
					break;
				case OptionableType.TileView:
					propertyGrid.SelectedObject = o as TileViewOptionables;
					_desc.Height = TileView.Optionables.DescriptionHeight;
					break;
				case OptionableType.TopView:
					propertyGrid.SelectedObject = o as TopViewOptionables;
					_desc.Height = TopView.Optionables.DescriptionHeight;
					break;
				case OptionableType.RouteView:
					propertyGrid.SelectedObject = o as RouteViewOptionables;
					_desc.Height = RouteView.Optionables.DescriptionHeight;
					break;
			}

			RegistryInfo.RegisterProperties(this); // NOTE: 1 metric for all four types
			_init = false;
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Handles command-key processing. Closes this <c>OptionsF</c> when
		/// either of <c>[Esc]</c> or <c>[Ctrl+o]</c> is pressed.
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
					if (!GetActiveControl().GetType().ToString().Contains(GridViewEdit)) // lalala i can't hear you
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
		/// Handles the SizeChanged event of the Description area.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDescriptionSizeChanged(object sender, EventArgs e)
		{
			if (!_init && WindowState == FormWindowState.Normal)
			{
				switch (_oType)
				{
					case OptionableType.MainView:  MainViewF.Optionables.DescriptionHeight = _desc.Height; break;
					case OptionableType.TileView:  TileView .Optionables.DescriptionHeight = _desc.Height; break;
					case OptionableType.TopView:   TopView  .Optionables.DescriptionHeight = _desc.Height; break;
					case OptionableType.RouteView: RouteView.Optionables.DescriptionHeight = _desc.Height; break;
				}
			}
		}

		/// <summary>
		/// Handles this form's VisibleChanged event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>The cached metric of an <c>OptionsF</c> is updated every
		/// time the form is hidden, unlike other viewers that update their
		/// metrics only when MapView quits.</remarks>
		private void OnVisibleChanged(object sender, EventArgs e)
		{
			if (!Visible)
				RegistryInfo.UpdateRegistry(this, true); // NOTE: 1 metric for all four types
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Finds the focused control in this container.
		/// </summary>
		/// <returns>the active control</returns>
		private Control GetActiveControl()
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
		internal OptionsPropertyGrid propertyGrid;

		private void InitializeComponent()
		{
			this.propertyGrid = new MapView.OptionsPropertyGrid();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Margin = new System.Windows.Forms.Padding(0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(592, 374);
			this.propertyGrid.TabIndex = 0;
			// 
			// OptionsF
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(592, 374);
			this.Controls.Add(this.propertyGrid);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new System.Drawing.Size(500, 300);
			this.Name = "OptionsF";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Custom PropertyGrid";
			this.VisibleChanged += new System.EventHandler(this.OnVisibleChanged);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
