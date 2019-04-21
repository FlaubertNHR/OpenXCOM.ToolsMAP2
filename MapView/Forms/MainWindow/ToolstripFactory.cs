using System;
using System.Collections.Generic;
using System.Windows.Forms;

using MapView.Properties;


namespace MapView.Forms.MainWindow
{
	internal sealed class ToolstripFactory
	{
		#region Fields
		private readonly MainViewUnderlay MainViewUnderlay;

		private readonly List<ToolStripButton> _editers = new List<ToolStripButton>(); // all edit-buttons except the pasters
		private readonly List<ToolStripButton> _pasters = new List<ToolStripButton>();
		private readonly List<ToolStripButton> _downers = new List<ToolStripButton>();
		private readonly List<ToolStripButton> _uppers  = new List<ToolStripButton>();

		// The instantiations of toolstrip-objects that are classvars are for
		// MainView, while the toolstrip-objects for TopView and
		// TopRouteView(Top) are instantiated in the classfuncts.

		private readonly ToolStripTextBox _tstbSearch     = new ToolStripTextBox();
		private readonly ToolStripButton  _tsbSearchClear = new ToolStripButton();

		private readonly ToolStripButton _tsbScale    = new ToolStripButton();
		private readonly ToolStripButton _tsbScaleOut = new ToolStripButton();
		private readonly ToolStripButton _tsbScaleIn  = new ToolStripButton();

		private readonly ToolStripButton _tsbDown     = new ToolStripButton();
		private readonly ToolStripButton _tsbUp       = new ToolStripButton();

		private readonly ToolStripButton _tsbCut      = new ToolStripButton();
		private readonly ToolStripButton _tsbCopy     = new ToolStripButton();
		private readonly ToolStripButton _tsbPaste    = new ToolStripButton();
		private readonly ToolStripButton _tsbDelete   = new ToolStripButton();

		private readonly ToolStripButton _tsbFill     = new ToolStripButton();
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="panel"></param>
		internal ToolstripFactory(MainViewUnderlay panel)
		{
			MainViewUnderlay = panel;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Adds a textfield for search to the specified toolstrip.
		/// NOTE: Appears only in MainView.
		/// </summary>
		/// <param name="toolStrip"></param>
		internal void CreateToolstripSearchObjects(ToolStrip toolStrip)
		{
			var tsItems = new ToolStripItem[]
			{
				_tstbSearch,
				_tsbSearchClear
			};
			toolStrip.Items.AddRange(tsItems);

			// Search textfield
			_tstbSearch.Name             = "tstbSearch";
			_tstbSearch.Text             = "search";
			_tstbSearch.KeyPress        += OnSearchKeyPress;

			// SearchClear btn
			_tsbSearchClear.Name         = "tsbSearchClear";
			_tsbSearchClear.ToolTipText  = "reset hilight";
			_tsbSearchClear.DisplayStyle = ToolStripItemDisplayStyle.Image;
			_tsbSearchClear.Image        = Resources.delete;
			_tsbSearchClear.Click       += OnClearHighlightClick;
		}

		/// <summary>
		/// Focuses the Search textbox.
		/// </summary>
		internal void FocusSearch()
		{
			_tstbSearch.Focus();
		}

		/// <summary>
		/// Gets the Search textbox's current text.
		/// </summary>
		/// <returns></returns>
		internal string GetSearchText()
		{
			return _tstbSearch.Text;
		}


		/// <summary>
		/// Adds buttons for zooming the map-scale to the specified toolstrip.
		/// NOTE: Appears only in MainView.
		/// </summary>
		/// <param name="toolStrip"></param>
		internal void CreateToolstripScaleObjects(ToolStrip toolStrip)
		{
			var tsItems = new ToolStripItem[]
			{
				new ToolStripSeparator(),
				_tsbScale,
				_tsbScaleOut,
				_tsbScaleIn
			};
			toolStrip.Items.AddRange(tsItems);

			// AutoZoom btn
			_tsbScale.Name            = "tsbScale";
			_tsbScale.ToolTipText     = "autoscale";
			_tsbScale.DisplayStyle    = ToolStripItemDisplayStyle.Image;
			_tsbScale.Image           = Resources.scale;
			_tsbScale.Click          += XCMainWindow.that.OnScaleClick;
			_tsbScale.Checked         = true;
			_tsbScale.Enabled         = false;

			// ZoomOut btn
			_tsbScaleOut.Name         = "tsbScaleOut";
			_tsbScaleOut.ToolTipText  = "scale Out";
			_tsbScaleOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
			_tsbScaleOut.Image        = Resources.scaleOut;
			_tsbScaleOut.Click       += XCMainWindow.that.OnScaleOutClick;
			_tsbScaleOut.Enabled      = false;

			// ZoomIn btn
			_tsbScaleIn.Name          = "tsbScaleIn";
			_tsbScaleIn.ToolTipText   = "scale In";
			_tsbScaleIn.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			_tsbScaleIn.Image         = Resources.scaleIn;
			_tsbScaleIn.Click        += XCMainWindow.that.OnScaleInClick;
			_tsbScaleIn.Enabled       = false;
		}

		/// <summary>
		/// Disables the auto-scale button when user switches to glide.
		/// </summary>
		internal void DisableScaleChecked()
		{
			_tsbScale.Checked = false;
		}

		/// <summary>
		/// Toggles the auto-scale button checked/unchecked.
		/// </summary>
		/// <returns>true if checked</returns>
		internal bool ToggleScaleChecked()
		{
			return (_tsbScale.Checked = !_tsbScale.Checked);
		}

		/// <summary>
		/// Enables MainView's auto-scale button.
		/// </summary>
		internal void EnableScaleButton()
		{
			_tsbScale.Enabled = true;
		}

		internal void SetScaleOutButtonEnabled(bool enabled)
		{
			_tsbScaleOut.Enabled = enabled;
		}

		internal void SetScaleInButtonEnabled(bool enabled)
		{
			_tsbScaleIn.Enabled = enabled;
		}


		/// <summary>
		/// Adds buttons for Up,Down,Cut,Copy,Paste,Delete and Fill to the
		/// specified toolstrip in MainView as well as TopView and
		/// TopRouteView(Top).
		/// </summary>
		/// <param name="toolStrip">a toolstrip to put the buttons in</param>
		/// <param name="tertiary">false for MainView's toolstrip, true for
		/// TopView's and TopRouteView's toolstrips</param>
		internal void CreateToolstripEditorObjects(
				ToolStrip toolStrip,
				bool tertiary = false)
		{
			ToolStripButton tsbDown;	// NOTE: Down/Up are not really editor-objects ...
			ToolStripButton tsbUp;		// but they appear in TopView and TopRouteView(Top)
										// as well as MainView, with the editor-objects.
			ToolStripButton tsbCut;
			ToolStripButton tsbCopy;
			ToolStripButton tsbPaste;
			ToolStripButton tsbDelete;

			ToolStripButton tsbFill;

			if (tertiary)
			{
				tsbDown   = new ToolStripButton();
				tsbUp     = new ToolStripButton();

				tsbCut    = new ToolStripButton();
				tsbCopy   = new ToolStripButton();
				tsbPaste  = new ToolStripButton();
				tsbDelete = new ToolStripButton();

				tsbFill   = new ToolStripButton();
			}
			else
			{
				tsbDown   = _tsbDown;
				tsbUp     = _tsbUp;

				tsbCut    = _tsbCut;
				tsbCopy   = _tsbCopy;
				tsbPaste  = _tsbPaste;
				tsbDelete = _tsbDelete;

				tsbFill   = _tsbFill;
			}

			var tsItems = new ToolStripItem[]
			{
				new ToolStripSeparator(), // NOTE: c#/.NET cant figure out how to use 1 separator 4 times.
				tsbDown,
				tsbUp,

				new ToolStripSeparator(),
				tsbCut,
				tsbCopy,
				tsbPaste,
				tsbDelete,

				new ToolStripSeparator(),
				tsbFill,

				new ToolStripSeparator()
			};
			toolStrip.Items.AddRange(tsItems);

			// LevelDown btn
			tsbDown.Name          = "tsbDown";
			tsbDown.ToolTipText   = "level down";
			tsbDown.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			tsbDown.Image         = Resources.down;
			tsbDown.Click        += OnDownClick;
			tsbDown.Enabled       = false;

			_downers.Add(tsbDown);

			// LevelUp btn
			tsbUp.Name             = "tsbUp";
			tsbUp.ToolTipText      = "level up";
			tsbUp.DisplayStyle     = ToolStripItemDisplayStyle.Image;
			tsbUp.Image            = Resources.up;
			tsbUp.Click           += OnUpClick;
			tsbUp.Enabled          = false;

			_uppers.Add(tsbUp);

			// Cut btn
			tsbCut.Name            = "tsbCut";
			tsbCut.ToolTipText     = "cut";
			tsbCut.DisplayStyle    = ToolStripItemDisplayStyle.Image;
			tsbCut.Image           = Resources.cut;
			tsbCut.Click          += MainViewUnderlay.OnCut;
			tsbCut.Enabled         = false;
//			tsbCut.Click          += (sender, e) => // -> example of ... lambda usage
//									{
//										EnablePasteButton();
//										MainViewUnderlay.OnCut(sender, e);
//									};

			_editers.Add(tsbCut);

			// Copy btn
			tsbCopy.Name           = "tsbCopy";
			tsbCopy.ToolTipText    = "copy";
			tsbCopy.DisplayStyle   = ToolStripItemDisplayStyle.Image;
			tsbCopy.Image          = Resources.copy;
			tsbCopy.Click         += MainViewUnderlay.OnCopy;
			tsbCopy.Enabled        = false;
//			tsbCopy.Click         += (sender, e) => // -> example of ... lambda usage
//									{
//										EnablePasteButton();
//										MainViewUnderlay.OnCopy(sender, e);
//									};

			_editers.Add(tsbCopy);

			// Paste btn
			tsbPaste.Name          = "tsbPaste";
			tsbPaste.ToolTipText   = "paste";
			tsbPaste.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			tsbPaste.Image         = Resources.paste;
			tsbPaste.Click        += MainViewUnderlay.OnPaste;
			tsbPaste.Enabled       = false;

			_pasters.Add(tsbPaste);

			// Delete btn
			tsbDelete.Name         = "tsbDelete";
			tsbDelete.ToolTipText  = "delete";
			tsbDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
			tsbDelete.Image        = Resources.delete;
			tsbDelete.Click       += MainViewUnderlay.OnDelete;
			tsbDelete.Enabled      = false;

			_editers.Add(tsbDelete);

			// Fill btn
			tsbFill.Name           = "tsbFill";
			tsbFill.ToolTipText    = "fill";
			tsbFill.DisplayStyle   = ToolStripItemDisplayStyle.Image;
			tsbFill.Image          = Resources.fill;
			tsbFill.Click         += MainViewUnderlay.OnFill;
			tsbFill.Enabled        = false;

			_editers.Add(tsbFill);
		}

		internal void SetLevelDownButtonsEnabled(bool enabled)
		{
			foreach (var tsb in _downers)
				tsb.Enabled = enabled;
		}

		internal void SetLevelUpButtonsEnabled(bool enabled)
		{
			foreach (var tsb in _uppers)
				tsb.Enabled = enabled;
		}

		internal void SetEditButtonsEnabled(bool enabled)
		{
			foreach (var tsb in _editers)
				tsb.Enabled = enabled;
		}

		internal void SetPasteButtonsEnabled(bool enabled = true)
		{
			foreach (var tsb in _pasters)
				tsb.Enabled = enabled;
		}
		#endregion Methods


		#region Events (level)
		/// <summary>
		/// Handles a level-down click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDownClick(object sender, EventArgs e)
		{
			var @base = MainViewUnderlay.MapBase;
			if (@base != null)
			{
				@base.LevelDown();

				SetLevelDownButtonsEnabled(@base.Level != @base.MapSize.Levs - 1);
				SetLevelUpButtonsEnabled(  @base.Level != 0);
			}
		}

		/// <summary>
		/// Handles a level-up click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnUpClick(object sender, EventArgs e)
		{
			var @base = MainViewUnderlay.MapBase;
			if (@base != null)
			{
				@base.LevelUp();

				SetLevelDownButtonsEnabled(@base.Level != @base.MapSize.Levs - 1);
				SetLevelUpButtonsEnabled(  @base.Level != 0);
			}
		}
		#endregion Events (level)


		#region Events (search)
		/// <summary>
		/// Handler for pressing the Enter-key when the search-textbox is focused.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnSearchKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				XCMainWindow.that.Search(_tstbSearch.Text);
				e.Handled = true;
			}
		}

		/// <summary>
		/// Clears the searched, found, and highlighted Treenode.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnClearHighlightClick(object sender, EventArgs e)
		{
			XCMainWindow.that.ClearSearched();
			_tstbSearch.Focus();
		}
		#endregion Events (search)
	}
}
