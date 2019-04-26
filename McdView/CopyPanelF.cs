using System;
using System.Windows.Forms;
using System.Drawing;

using XCom;


namespace McdView
{
	/// <summary>
	/// A form with a panel that enables the user to copy MCD records from a
	/// different MCD-set than the one that's currently loaded in McdView to the
	/// internal copy-buffer of McdView for pasting into the currently loaded
	/// MCD-set.
	/// </summary>
	internal partial class CopyPanelF
		:
			Form
	{
		#region Fields (static)
		internal static Point Loc = new Point(-1,-1);
		#endregion Fields (static)


		#region Fields
		private readonly McdviewF _f;

		internal string Label;
		#endregion Fields


		#region Properties
		internal TerrainPanel_copy PartsPanel
		{ get; private set; }

		private Tilepart[] _parts;
		/// <summary>
		/// An array of 'Tileparts'. Each entry's record is referenced w/ 'Record'.
		/// </summary>
		internal Tilepart[] Parts
		{
			get { return _parts; }
			set
			{
				PartsPanel.Parts = (_parts = value);
			}
		}

		private SpriteCollection _spriteset;
		internal SpriteCollection Spriteset
		{
			get { return _spriteset; }
			set
			{
				string text = "Copy panel - ";

				if ((PartsPanel.Spriteset = (_spriteset = value)) != null)
					text += _spriteset.Label;
				else
					text += "spriteset invalid";

				Text = text;
				PartsPanel.Select();
			}
		}

		private int _selId = -1;
		/// <summary>
		/// The currently selected 'Parts' ID.
		/// </summary>
		internal int SelId
		{
			get { return _selId; }
			set
			{
				if (_selId != value)
				{
					if ((_selId = value) != -1)
					{
//						PopulateTextFields();
						PartsPanel.ScrollToPart();
					}
					else
					{
//						ClearTextFields();
					}

					PartsPanel.Invalidate();
				}

				if (PartsPanel.SubIds.Remove(_selId)) // safety. The SelId shall never be in the SubIds.
					PartsPanel.Invalidate();
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="f"></param>
		internal CopyPanelF(McdviewF f)
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_f = f;

			Location = new Point(
							_f.Location.X + 20,
							_f.Location.Y + 20);

			PartsPanel = new TerrainPanel_copy(_f, this);
			gb_Collection.Controls.Add(PartsPanel);
			PartsPanel.Width = gb_Collection.Width - 10;

			McdviewF.SetDoubleBuffered(PartsPanel);

			PartsPanel.Select();

		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Closes (and disposes) this CopyPanelF object.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Loc = new Point(Location.X, Location.Y);
			_f.CloseCopyPanel();

			base.OnFormClosing(e);
		}

		/// <summary>
		/// Positions this CopyPanelF wrt/ McdviewF.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			if (Loc.X == -1)
				Location = new Point(_f.Location.X + 20, _f.Location.Y + 20);
			else
				Location = new Point(Loc.X, Loc.Y);

			base.OnLoad(e);
		}

		/// <summary>
		/// yah whatever. I dislike .NET keyboard gobbledy-gook.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.O))
			{
				_f.OpenCopyPanel();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion Events (override)


		#region Events
		private void OnClick_Open(object sender, EventArgs e)
		{
			_f.OpenCopyPanel();
		}
		#endregion Events
	}
}
