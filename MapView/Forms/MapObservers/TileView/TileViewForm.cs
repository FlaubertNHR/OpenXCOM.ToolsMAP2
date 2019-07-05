using System;
using System.Windows.Forms;

using MapView.Forms.MainWindow;


namespace MapView.Forms.MapObservers.TileViews
{
	internal sealed class TileViewForm
		:
			Form,
			IMapObserverProvider
	{
		#region Properties
		/// <summary>
		/// Gets TileViewControl as a child of MapObserverControl.
		/// </summary>
		internal TileView Control
		{
			get { return TileViewControl; }
		}

		/// <summary>
		/// Satisfies IMapObserverProvider.
		/// </summary>
		public MapObserverControl ObserverControl
		{
			get { return TileViewControl; }
		}
		#endregion Properties


		#region cTor
		internal TileViewForm()
		{
			InitializeComponent();
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Fires when the form is activated. Maintains the position of this
		/// form in the z-order List and focuses the selected panel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);

			TileViewControl.GetSelectedPanel().Focus();

//			base.OnActivated(e);
		}
		/// <summary>
		/// Handles a so-called command-key at the form level. Stops keys that
		/// shall be used for navigating the tiles from doing anything stupid
		/// instead.
		/// - passes the arrow-keys to the TileView control's current panel's
		///   Navigate() funct
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
					if (Control.GetSelectedPanel().Focused)
					{
						Control.GetSelectedPanel().Navigate(keyData);
						return true;
					}
					break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Handles KeyDown events at the form level.
		/// - [Esc] focuses the current panel
		/// - opens/closes Options on [Ctrl+o] event
		/// - checks for and if so processes a viewer F-key
		/// - passes edit-keys to the TileView control's current panel's
		///   Navigate() funct
		/// @note Requires 'KeyPreview' true.
		/// @note See also TopViewForm, RouteViewForm, TopRouteViewForm
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				e.SuppressKeyPress = true;
				Control.GetSelectedPanel().Focus();
			}
			else if (e.KeyCode == Keys.O
				&& (e.Modifiers & Keys.Control) == Keys.Control)
			{
				e.SuppressKeyPress = true;
				Control.OnOptionsClick(Control.GetOptionsButton(), EventArgs.Empty);
			}
			else if (!MainMenusManager.ViewerKeyDown(e)
				&& Control.GetSelectedPanel().Focused)
			{
				switch (e.KeyCode)
				{
					case Keys.Home:
					case Keys.End:
					case Keys.PageUp:
					case Keys.PageDown:
						e.SuppressKeyPress = true;
						Control.GetSelectedPanel().Navigate(e.KeyData);
						break;
				}
			}
			base.OnKeyDown(e);
		}
		#endregion Events (override)


		#region Designer
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


/*		The #develop designer is going to delete this:

			TileViewControl = new MapView.Forms.MapObservers.TileViews.TileView();

		so copy it back into InitializeComponent() */

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			TileViewControl = new MapView.Forms.MapObservers.TileViews.TileView();
			this.SuspendLayout();
			// 
			// TileViewControl
			// 
			this.TileViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TileViewControl.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TileViewControl.Location = new System.Drawing.Point(0, 0);
			this.TileViewControl.Name = "TileViewControl";
			this.TileViewControl.Size = new System.Drawing.Size(632, 454);
			this.TileViewControl.TabIndex = 0;
			// 
			// TileViewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 454);
			this.Controls.Add(this.TileViewControl);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.KeyPreview = true;
			this.Name = "TileViewForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "TileView";
			this.ResumeLayout(false);

		}

		private System.ComponentModel.IContainer components = null;

		private TileView TileViewControl;
		#endregion Designer
	}
}
