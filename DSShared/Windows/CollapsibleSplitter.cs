#region author comment
/*
 *	http://www.codeproject.com/cs/miscctrl/collapsiblesplitter.asp
 *
	Windows Forms Collapsible Splitter Control for .Net
	(c)Copyright 2002-2003 NJF (furty74@yahoo.com). All rights reserved.

	Assembly Build Dependencies:
	CollapsibleSplitter.bmp

	Version 1.1 Changes:
	OnPaint is now overridden instead of being a handled event, and the entire splitter is now painted rather than just the collapser control
	The splitter rectangle is now correctly defined
	The Collapsed property was renamed to IsCollapsed, and the code changed so that no value needs to be set
	New visual styles added: Win9x, XP, DoubleDots and Lines

	Version 1.11 Changes:
	The OnMouseMove event handler was updated to address a flickering issue discovered by John O'Byrne

	Version 1.2 Changes:
	Added support for horizontal splitters

	Version 1.21 Changes:
	Added support for inclusion as a VS.Net ToolBox control
	Added a ToolBox bitmap
	Removed extraneous overrides
	Added summaries

	Version 1.22 Changes:
	Removed the ParentFolder from public properties - this is now set automatically in the OnHandleCreated event
	*Added expand/collapse animation code

	Version 1.3 Changes:
	Added an optional 3D border
	General code and comment cleaning
	Flagged assembly with the CLSCompliant attribute
	Added a simple designer class to filter unwanted properties
*/

/*
My Changes (well that was relevant..)
- added new Dock property
- changed VisualStyles to DotStyles
- changed drawing logic
- cached pens and brushes so they arent created over and over in drawing method
- turned some events (all?) into overrides
*/
#endregion

// TODO: Make this go away.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace DSShared.Windows
{
	/// <summary>
	/// Specifies how the outline of the collapsible splitter is drawn.
	/// </summary>
	public enum DotStyle
	{
		Mozilla,
		XP,
		Win9X,
		DoubleDots,
		Lines
	}

	/// <summary>
	/// Splitter states.
	/// </summary>
	public enum SplitterState
	{
		Collapsed,
		Expanding,
		Expanded,
		Collapsing
	}


	/// <summary>
	/// A custom collapsible splitter that can resize, hide and show associated
	/// form controls.
	/// </summary>
	[ToolboxBitmap(typeof(CollapsibleSplitter))]
	[DesignerAttribute(typeof(CollapsibleSplitterDesigner))]
	public sealed class CollapsibleSplitter
		:
			Splitter
	{
		#region Fields (static)
		private static Color _hotColor = CalculateColor(SystemColors.Highlight, SystemColors.Window, 70);

		private static Pen   _penHot             = new Pen(_hotColor);
		private static Brush _brushHot           = new SolidBrush(_hotColor);

		private static Brush _brushControlDark   = new SolidBrush(SystemColors.ControlDark);

		private static Pen _penControlDark       = new Pen(SystemColors.ControlDark);
		private static Pen _penControlDarkDark   = new Pen(SystemColors.ControlDarkDark);
		private static Pen _penControlLight      = new Pen(SystemColors.ControlLight);
		private static Pen _penControlLightLight = new Pen(SystemColors.ControlLightLight);

//		private const int OffsetX = 2;
		private const int OffsetY = 3;

		// width/height are for triangles pointing left/right
		private const int TriWidth  = 3;
		private const int TriHeight = 6;

		private const int TriDotPad = 5;
		#endregion Fields (static)


		#region Fields
		private Form _fparent;

		// animation controls introduced in version 1.22
		private Timer _timerAni;

		private int _controlWidth;
		private int _controlHeight;
		private int _fparentWidth;
		private int _fparentHeight;

		private bool _hot;

//		private Brush _brushBack;
		private Pen _penBack;
		#endregion Fields


		#region Properties
		private SplitterState CurrentState
		{ get; set; }

		private Rectangle HotArea
		{ get; set; }

		private DotStyle _dotStyle = DotStyle.Mozilla;
		/// <summary>
		/// Gets or sets the dot style.
		/// </summary>
		/// <value>The dot style.</value>
		[Category("Custom Appearance"),
		DefaultValue(DotStyle.Mozilla),
		Description("Dot style drawn on the hotzone")]
		public DotStyle DotStyle
		{
			get { return _dotStyle; }
			set
			{
				_dotStyle = value;
				Refresh();
			}
		}

		/// <summary>
		/// Gets or sets the background color for the control.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Drawing.Color"></see> that represents the background color of the control.
		/// The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor"></see> property.</returns>
		/// <PermissionSet>
		/// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// </PermissionSet>
		public new Color BackColor
		{
			get { return base.BackColor;}
			set
			{
				base.BackColor = value;
				_penBack = new Pen(value);
//				_brushBack = new SolidBrush(value);
			}
		}

		/// <summary>
		/// The initial state of the Splitter. Set to True if the control to
		/// hide is not visible by default.
		/// </summary>
		[Category("Collapsing Options")]
		[DefaultValue(false)]
		[Description("The initial state of the Splitter. Set to True if the control to hide is not visible by default")]
		public bool IsCollapsed
		{
			get { return (ControlToHide == null) || !ControlToHide.Visible; }
			set
			{
				if (ControlToHide != null && ControlToHide.Visible == value)
					ToggleSplitter();
			}
		}

		/// <summary>
		/// The System.Windows.Forms.Control that the splitter will collapse.
		/// </summary>
		[Category("Collapsing Options"), DefaultValue("(none)"),
		Description("The System.Windows.Forms.Control that the splitter will collapse")]
		public Control ControlToHide
		{ get; set; }

		/// <summary>
		/// Determines if the collapse and expanding actions will be animated.
		/// </summary>
		[Category("Collapsing Options"), DefaultValue(false),
		Description("Determines if the collapse and expanding actions will be animated")]
		public bool UseAnimations
		{ get; set; }

		private int _aniDelay = 20;
		/// <summary>
		/// The delay in milliseconds between animation steps.
		/// </summary>
		[Category("Collapsing Options"), DefaultValue(20),
		Description("The delay in millisenconds between animation steps")]
		public int AnimationDelay
		{
			get { return _aniDelay; }
			set { _aniDelay = value; }
		}

		private int _aniStep = 20;
		/// <summary>
		/// The amount of pixels moved in each animation step.
		/// </summary>
		[Category("Collapsing Options"), DefaultValue(20),
		Description("The amount of pixels moved in each animation step")]
		public int AnimationStep
		{
			get { return _aniStep; }
			set { _aniStep = value; }
		}

		/// <summary>
		/// When true the entire parent form will be expanded and collapsed,
		/// otherwise just the contol to expand will be changed.
		/// </summary>
		[Category("Collapsing Options"), DefaultValue(false),
		Description("When true the entire parent form will be expanded and collapsed, otherwise just the contol to expand will be changed")]
		public bool ExpandParent
		{ get; set; }

		// Border added in version 1.3
		private Border3DStyle borderStyle = Border3DStyle.Flat;
		/// <summary>
		/// An optional border style to paint on the control. Set to Flat for no
		/// border.
		/// </summary>
		[Category("Custom Appearance"),
		DefaultValue(Border3DStyle.Adjust),
		Description("An optional border style to paint on the control")]
		public Border3DStyle BorderStyle3D
		{
			get{ return borderStyle; }
			set
			{
//				switch (value)
//				{
//					case Border3DStyle.Etched:
//						OffsetX = 2;
//						break;
//					default:
//						OffsetX = 1;
//						break;
//				}
				borderStyle = value;
				Refresh();
			}
		}

		/// <summary>
		/// Gets or sets which <see cref="T:System.Windows.Forms.Splitter" />
		/// borders are docked to its parent control and determines how a
		/// <see cref="T:System.Windows.Forms.Splitter" /> is resized with its
		/// parent.
		/// </summary>
		/// <value></value>
		/// <returns>One of the <see cref="T:System.Windows.Forms.DockStyle" />
		/// values. The default is <see cref="F:System.Windows.Forms.DockStyle.Left" />.</returns>
		/// <exception cref="T:System.ArgumentException"><see cref="P:System.Windows.Forms.Splitter.Dock" />
		/// is not set to one of the valid <see cref="T:System.Windows.Forms.DockStyle" />
		/// values.</exception>
		/// <PermissionSet>
		///     <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///     <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
		///     <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		/// </PermissionSet>
		public new DockStyle Dock
		{
			get { return base.Dock; }
			set
			{
				switch (value)
				{

					case DockStyle.None:
					case DockStyle.Fill:
//						throw new Exception("CollapsibleSplitter cannot have a dock of " + value);
						break;

					case DockStyle.Left:
					case DockStyle.Right:
						Width = 10;
						break;

					case DockStyle.Top:
					case DockStyle.Bottom:
						Height = 10;
						break;
				}
				base.Dock = value;
			}
		}

		private int _hotMagnitude = 115;
		/// <summary>
		/// Gets or sets the size of the hotzone.
		/// </summary>
		/// <value>the size of california</value>
		[Browsable(true)]
		[DefaultValue(115)]
		[Description("The width/height of the 'hotzone' in pixels")]
		[Category("Custom Appearance")]
		public int HotMagnitude
		{
			get { return _hotMagnitude; }
			set
			{
				_hotMagnitude = value;
				OnResize(EventArgs.Empty);
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="T:DSShared.Windows.CollapsibleSplitter" /> class.
		/// </summary>
		public CollapsibleSplitter()
		{
			// Setup the animation timer control
			_timerAni = new Timer();
			_timerAni.Interval = _aniDelay;
			_timerAni.Tick += OnAnimationTimerTick;

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

//			_brushBack = new SolidBrush(BackColor);
			_penBack = new Pen(BackColor);
			HotArea = new Rectangle(
								0, (Height - _hotMagnitude) / 2,
								Width, _hotMagnitude);

			MinimumSize = new Size(5, 5);
		}
		#endregion cTor


		#region Methods (override)
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated" />
		/// event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains
		/// the event data.</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			_fparent = FindForm();

			if (ControlToHide != null)
				CurrentState = (ControlToHide.Visible) ? SplitterState.Expanded
													   : SplitterState.Collapsed;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" />
		/// event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" />
		/// that contains the event data.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!_hot && ControlToHide != null && ControlToHide.Visible)
				base.OnMouseDown(e); // if the hider control isn't hot, let the base resize action occur
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Resize" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains
		/// the event data.</param>
		protected override void OnResize(EventArgs e)
		{
			if (Dock == DockStyle.Left || Dock == DockStyle.Right)
				HotArea = new Rectangle(
									0, ((Height - _hotMagnitude) / 2),
									Width, _hotMagnitude);
			else
				HotArea = new Rectangle(
									(Width - _hotMagnitude) / 2, 0,
									_hotMagnitude, Height);

			Refresh();
		}

		// This method was updated in version 1.11 to fix a flickering problem
		// discovered by John O'Byrne.
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove" />
		/// event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" />
		/// that contains the event data.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (   e.X >= HotArea.X
				&& e.X <= HotArea.X + HotArea.Width
				&& e.Y >= HotArea.Y
				&& e.Y <= HotArea.Y + HotArea.Height)
			{
				if (!_hot)
				{
					_hot = true;
					Cursor = Cursors.Hand;
					Refresh();
				}
			}
			else
			{
				if (_hot)
				{
					_hot = false;
					Refresh();
				}

				Cursor = Cursors.Default;

				if (ControlToHide != null)
				{
					if (ControlToHide.Visible) // Changed in v1.2 to support Horizontal Splitters
					{
						switch (Dock)
						{
							case DockStyle.Left:
							case DockStyle.Right:
								Cursor = Cursors.VSplit;
								break;

							default:
								Cursor = Cursors.HSplit;
								break;
						}
					}
				}
			}
			base.OnMouseMove(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave" />
		/// event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains
		/// the event data.</param>
		protected override void OnMouseLeave(EventArgs e)
		{
			_hot = false; // ensure that the hot state is removed
			Refresh();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Click" />
		/// event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains
		/// the event data.</param>
		protected override void OnClick(EventArgs e)
		{
			switch (CurrentState)
			{
				case SplitterState.Collapsing:
				case SplitterState.Expanding:
					return;

				default:
					if (_hot && ControlToHide != null)
						ToggleSplitter();
					break;
			}
		}

		private void ToggleSplitter()
		{
			switch (CurrentState) // drop out if an animation is currently in progress for this control
			{
				case SplitterState.Collapsing:
				case SplitterState.Expanding:
					return;
			}


			_controlWidth  = ControlToHide.Width;
			_controlHeight = ControlToHide.Height;

			if (ControlToHide.Visible)
			{
				if (UseAnimations)
				{
					_timerAni.Enabled = true;
					CurrentState = SplitterState.Collapsing;

					if (_fparent != null)
					{
						switch (Dock)
						{
							case DockStyle.Left:
							case DockStyle.Right:
								_fparentWidth = _fparent.Width - _controlWidth;
								break;

							default:
								_fparentHeight = _fparent.Height - _controlHeight;
								break;
						}
					}
				}
				else // no animations so just toggle the visible state
				{
					CurrentState = SplitterState.Collapsed;

					ControlToHide.Visible = false;

					if (ExpandParent && _fparent != null)
					{
						switch (Dock)
						{
							case DockStyle.Left:
							case DockStyle.Right:
								_fparent.Width -= ControlToHide.Width;
								break;

							default:
								_fparent.Height -= ControlToHide.Height;
								break;
						}
					}
				}
			}
			else // control to hide is collapsed
			{
				if (UseAnimations)
				{
					_timerAni.Enabled = true;
					CurrentState = SplitterState.Expanding;

					ControlToHide.Visible = true;

					switch (Dock)
					{
						case DockStyle.Left:
						case DockStyle.Right:
							ControlToHide.Width = 0;

							if (_fparent != null)
								_fparentWidth = _fparent.Width + _controlWidth;
							break;

						default:
							ControlToHide.Height = 0;

							if (_fparent != null)
								_fparentHeight = _fparent.Height + _controlHeight;
							break;
					}
				}
				else // no animations so just toggle the visible state
				{
					CurrentState = SplitterState.Expanded;

					ControlToHide.Visible = true;

					if (ExpandParent && _fparent != null)
					{
						switch (Dock)
						{
							case DockStyle.Left:
							case DockStyle.Right:
								_fparent.Width += ControlToHide.Width;
								break;

							default:
								_fparent.Height += ControlToHide.Height;
								break;
						}
					}
				}
			}
			Refresh();
		}
		#endregion Methods (override)


		#region Implementation

		#region Animation Timer Tick
		private void OnAnimationTimerTick(object sender, EventArgs e)
		{
			switch (CurrentState)
			{
				case SplitterState.Collapsing:
					if (Dock == DockStyle.Left || Dock == DockStyle.Right) // vertical splitter
					{
						if (ControlToHide.Width > _aniStep)
						{
							if (ExpandParent
								&& _fparent.WindowState != FormWindowState.Maximized
								&& _fparent != null)
							{
								_fparent.Width -= _aniStep;
							}
							ControlToHide.Width -= _aniStep;
						}
						else
						{
							if (ExpandParent
								&& _fparent.WindowState != FormWindowState.Maximized
								&& _fparent != null)
							{
								_fparent.Width = _fparentWidth;
							}
							ControlToHide.Visible = false;
							_timerAni.Enabled = false;
							ControlToHide.Width = _controlWidth;
							CurrentState = SplitterState.Collapsed;
							Invalidate();
						}
					}
					else
					{
						if (ControlToHide.Height > _aniStep) // horizontal splitter
						{
							if (ExpandParent
								&& _fparent.WindowState != FormWindowState.Maximized
								&& _fparent != null)
							{
								_fparent.Height -= _aniStep;
							}
							ControlToHide.Height -= _aniStep;
						}
						else
						{
							if (ExpandParent
								&& _fparent.WindowState != FormWindowState.Maximized
								&& _fparent != null)
							{
								_fparent.Height = _fparentHeight;
							}
							ControlToHide.Visible = false;
							_timerAni.Enabled = false;
							ControlToHide.Height = _controlHeight;
							CurrentState = SplitterState.Collapsed;
							Invalidate();
						}
					}
					break;

				case SplitterState.Expanding:
					if (Dock == DockStyle.Left || Dock == DockStyle.Right) // vertical splitter
					{
						if (ControlToHide.Width < _controlWidth - _aniStep)
						{
							if (ExpandParent
								&& _fparent.WindowState != FormWindowState.Maximized
								&& _fparent != null)
							{
								_fparent.Width += _aniStep;
							}
							ControlToHide.Width += _aniStep;
						}
						else
						{
							if (ExpandParent
								&& _fparent.WindowState != FormWindowState.Maximized
								&& _fparent != null)
							{
								_fparent.Width = _fparentWidth;
							}
							ControlToHide.Width = _controlWidth;
							ControlToHide.Visible = true;
							_timerAni.Enabled = false;
							CurrentState = SplitterState.Expanded;
							Invalidate();
						}
					}
					else // horizontal splitter
					{
						if (ControlToHide.Height < _controlHeight - _aniStep)
						{
							if (ExpandParent
								&& _fparent.WindowState != FormWindowState.Maximized
								&& _fparent != null)
							{
								_fparent.Height += _aniStep;
							}
							ControlToHide.Height += _aniStep;
						}
						else
						{
							if (ExpandParent
								&& _fparent.WindowState != FormWindowState.Maximized
								&& _fparent != null)
							{
								_fparent.Height = _fparentHeight;
							}
							ControlToHide.Height = _controlHeight;
							ControlToHide.Visible = true;
							_timerAni.Enabled = false;
							CurrentState = SplitterState.Expanded;
							Invalidate();
						}
					}
					break;
			}
		}
		#endregion Animation Timer Tick


		// OnPaint is now an override rather than an event in version 1.1
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Paint" /> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" />
		/// that contains the event data.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			/********** Vertical splitter ********/
			if (Dock == DockStyle.Left || Dock == DockStyle.Right)
			{
				if (_hot)
					graphics.FillRectangle(_brushHot, HotArea);
//				else
//					g.FillRectangle(_brushBack, HotArea);

				graphics.DrawLine( // draw the top & bottom lines for the control image
								_penControlDark,
								HotArea.X,
								HotArea.Y,
								HotArea.X + HotArea.Width,
								HotArea.Y);
				graphics.DrawLine(
								_penControlDark,
								HotArea.X,
								HotArea.Y + HotArea.Height,
								HotArea.X + HotArea.Width,
								HotArea.Y + HotArea.Height);

				if (Enabled && ControlToHide != null)
				{
					var leftRight = new GraphicsPath(); // draw the arrows for the control image
					if (   (Dock == DockStyle.Right && ControlToHide.Visible)
						|| (Dock == DockStyle.Left && !ControlToHide.Visible))
					{
						leftRight.AddLine(
									(Width - TriWidth) / 2,
									HotArea.Y + OffsetY,
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + OffsetY + TriHeight / 2);
						leftRight.AddLine(
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + OffsetY + TriHeight / 2,
									(Width - TriWidth) / 2,
									HotArea.Y + OffsetY + TriHeight);
						leftRight.CloseFigure();

						leftRight.AddLine(
									(Width - TriWidth) / 2,
									HotArea.Y + HotArea.Height - OffsetY,
									(Width - TriWidth) / 2,
									HotArea.Y + HotArea.Height - OffsetY - TriHeight);
						leftRight.AddLine(
									(Width - TriWidth) / 2,
									HotArea.Y + HotArea.Height - OffsetY - TriHeight,
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + HotArea.Height - (OffsetY + TriHeight / 2));
						leftRight.CloseFigure();
					}
					else
					{
						leftRight.AddLine(
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + OffsetY,
									(Width - TriWidth) / 2,
									HotArea.Y + OffsetY + TriHeight / 2);
						leftRight.AddLine(
									(Width - TriWidth) / 2,
									HotArea.Y + OffsetY + TriHeight / 2,
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + OffsetY + TriHeight);
						leftRight.CloseFigure();

						leftRight.AddLine(
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + HotArea.Height - OffsetY,
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + HotArea.Height - OffsetY - TriHeight);
						leftRight.AddLine(
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + HotArea.Height - OffsetY - TriHeight,
									(Width - TriWidth) / 2,
									HotArea.Y + HotArea.Height - (OffsetY + TriHeight / 2));
						leftRight.CloseFigure();
					}
					graphics.FillPath(_brushControlDark,leftRight);
				}


				int x = Width / 2 - 1; // draw the dots for the control image using a loop
				int y = HotArea.Y + OffsetY + TriHeight + TriDotPad;
				int dotSpace = 3;

				switch (_dotStyle) // Visual Styles added in version 1.1
				{
					case DotStyle.Mozilla:
						while (y < HotArea.Y + HotArea.Height - (OffsetY + TriHeight + TriDotPad))
						{
							graphics.DrawLine( // light dot
											_penControlLightLight,
											x,
											y,
											x + 1,
											y + 1);
							graphics.DrawLine( // dark dot
											_penControlDarkDark,
											x + 1,
											y + 1,
											x + 2,
											y + 2);

							if (_hot) // overdraw the background color since 2px diagonal lines were actually drawn, not just dots
								graphics.DrawLine(
											_penHot,
											x + 2,
											y + 1,
											x + 2,
											y + 2);
							else
								graphics.DrawLine(
											_penBack,
											x + 2,
											y + 1,
											x + 2,
											y + 2);

							y += dotSpace;
						}
						break;

					case DotStyle.DoubleDots:
						while (y < HotArea.Y + HotArea.Height - (OffsetY + TriHeight + TriDotPad))
						{
							graphics.DrawRectangle( // light dot
												_penControlLightLight,
												x,
												y + 1,
												1,
												1);
							graphics.DrawRectangle( // dark dot
												_penControlDark,
												x - 1,
												y,
												1,
												1);
							y += dotSpace;
							graphics.DrawRectangle( // light dot
												_penControlLightLight,
												x + 2,
												y + 1,
												1,
												1);
							graphics.DrawRectangle( // dark dot
												_penControlDark,
												x + 1,
												y,
												1,
												1);
							y += dotSpace;
						}
						break;

					case DotStyle.Win9X:
						graphics.DrawLine(
										_penControlLightLight,
										x,
										y,
										x + 2,
										y);
						graphics.DrawLine(
										_penControlLightLight,
										x,
										y,
										x,
										HotArea.Y + HotArea.Height - (OffsetY + TriHeight + TriDotPad));
						graphics.DrawLine(
										_penControlDark,
										x + 2,
										y,
										x + 2,
										HotArea.Y + HotArea.Height - (OffsetY + TriHeight + TriDotPad));
						graphics.DrawLine(
									_penControlDark,
									x,
									HotArea.Y + HotArea.Height - (OffsetY + TriHeight + TriDotPad),
									x + 2,
									HotArea.Y + HotArea.Height - (OffsetY + TriHeight + TriDotPad));
						break;

					case DotStyle.XP:
						dotSpace = 5;
						while (y < HotArea.Y + HotArea.Height - (OffsetY + TriHeight + TriDotPad))
						{
							graphics.DrawRectangle( // light dot
											_penControlLight,
											x,
											y,
											2,
											2);
							graphics.DrawRectangle( // light light dot
											_penControlLightLight,
											x + 1,
											y + 1,
											1,
											1);
							graphics.DrawRectangle( // dark dark dot
											_penControlDarkDark,
											x,
											y,
											1,
											1);
							graphics.DrawLine( // dark fill
											_penControlDark,
											x,
											y,
											x,
											y + 1);
							graphics.DrawLine(
											_penControlDark,
											x,
											y,
											x + 1,
											y);

							y += dotSpace;
						}
						break;

					case DotStyle.Lines:
						dotSpace = 2;
						while (y < HotArea.Y + HotArea.Height - (OffsetY + TriHeight + TriDotPad))
						{
							graphics.DrawLine(
											_penControlDarkDark,
											x,
											y,
											x + 2,
											y);
							y += dotSpace;
						}
						break;
				}
			}
			else if (Dock == DockStyle.Top || Dock == DockStyle.Bottom) // Horizontal Splitter support added in v1.2
			{
				if (_hot) // draw the background color for our control image
					graphics.FillRectangle(new SolidBrush(_hotColor), HotArea);
				else
					graphics.FillRectangle(new SolidBrush(BackColor), HotArea);

				graphics.DrawLine( // draw the left & right lines for our control image
								new Pen(SystemColors.ControlDark, 1),
								HotArea.X,
								HotArea.Y + 1,
								HotArea.X,
								HotArea.Y + HotArea.Height - 2);
				graphics.DrawLine(
								new Pen(SystemColors.ControlDark, 1),
								HotArea.X + HotArea.Width,
								HotArea.Y + 1,
								HotArea.X + HotArea.Width,
								HotArea.Y + HotArea.Height - 2);

//				if (Enabled && ControlToHide != null)
//				{
					// draw the arrows for our control image
					// the ArrowPointArray is a point array that defines an arrow shaped polygon
//					g.FillPolygon(
//							new SolidBrush(SystemColors.ControlDarkDark),
//							ArrowPointArray(
//										HotArea.X + 3,
//										HotArea.Y + 2,
//										Dock,
//										ControlToHide.Visible));
//					g.FillPolygon(
//							new SolidBrush(SystemColors.ControlDarkDark),
//							ArrowPointArray(
//										HotArea.X + HotArea.Width - 9,
//										HotArea.Y + 2,
//										Dock,
//										ControlToHide.Visible));

					// draw the arrows for our control image
/*					GraphicsPath upDown = new GraphicsPath();
					if (   (Dock == DockStyle.Bottom && ControlToHide.Visible) // point down
						|| (Dock == DockStyle.Top   && !ControlToHide.Visible))
					{
						upDown.AddLine(
									HotArea.X + OffsetX,
									(Height + TriHeight) / 2,
									HotArea.X + OffsetX + TriWidth * 2,
									(Height + TriHeight) / 2);
						upDown.AddLine(
									HotArea.X + OffsetX + TriWidth * 2,
									(Height + TriHeight) / 2,
									HotArea.X + OffsetX + TriWidth,
									(Height + TriHeight) / 2 + TriHeight);
						upDown.CloseFigure();

						upDown.AddLine(
									(Width - TriWidth) / 2,
									HotArea.Y + HotArea.Height - OffsetY,
									(Width - TriWidth) / 2,
									HotArea.Y + HotArea.Height - OffsetY - TriHeight);
						upDown.AddLine(
									(Width - TriWidth) / 2,
									HotArea.Y + HotArea.Height - OffsetY - TriHeight,
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + HotArea.Height - (OffsetY + TriHeight / 2));
						upDown.CloseFigure();
					}
					else
					{
						upDown.AddLine(
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + OffsetY,
									(Width - TriWidth) / 2,
									HotArea.Y + OffsetY + TriHeight / 2);
						upDown.AddLine(
									(Width - TriWidth) / 2,
									HotArea.Y + OffsetY + TriHeight / 2,
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + OffsetY + TriHeight);
						upDown.CloseFigure();

						upDown.AddLine(
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + HotArea.Height - OffsetY,
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + HotArea.Height - OffsetY - TriHeight);
						upDown.AddLine(
									(Width - TriWidth) / 2 + TriWidth,
									HotArea.Y + HotArea.Height - OffsetY - TriHeight,
									(Width - TriWidth) / 2,
									HotArea.Y + HotArea.Height - (OffsetY + TriHeight / 2));
						upDown.CloseFigure();
					}
					g.FillPath(controlDarkBrush,upDown); */
//				}

				// draw the dots for our control image using a loop
				int x = HotArea.X + 14;
				int y = HotArea.Y + 3;

				// Visual Styles added in version 1.1
				switch (_dotStyle)
				{
					case DotStyle.Mozilla:
						for (int i = 0; i < 30; i++)
						{
							graphics.DrawLine( // light dot
											new Pen(SystemColors.ControlLightLight),
											x + (i * 3),
											y,
											x + 1 + (i * 3),
											y + 1);
							graphics.DrawLine( // dark dot
											new Pen(SystemColors.ControlDarkDark),
											x + 1 + (i * 3),
											y + 1,
											x + 2 + (i * 3),
											y + 2);

							if (_hot) // overdraw the background color since 2px diagonal lines were actually drawn, not just dots
								graphics.DrawLine(
											new Pen(_hotColor),
											x + 1 + (i * 3),
											y + 2,
											x + 2 + (i * 3),
											y + 2);
							else
								graphics.DrawLine(
											new Pen(BackColor),
											x + 1 + (i * 3),
											y + 2,
											x + 2 + (i * 3),
											y + 2);
						}
						break;

					case DotStyle.DoubleDots:
						for (int i = 0; i < 30; i++)
						{
							graphics.DrawRectangle( // light dot
												new Pen(SystemColors.ControlLightLight),
												x + 1 + (i * 3),
												y,
												1,
												1);
							graphics.DrawRectangle( // dark dot
												new Pen(SystemColors.ControlDark),
												x + (i * 3),
												y - 1,
												1,
												1);
							i++;
							graphics.DrawRectangle( // light dot
												new Pen(SystemColors.ControlLightLight),
												x + 1 + (i * 3),
												y + 2,
												1,
												1);
							graphics.DrawRectangle( // dark dot
												new Pen(SystemColors.ControlDark),
												x + (i * 3),
												y + 1,
												1,
												1);
						}
						break;

					case DotStyle.Win9X:
						graphics.DrawLine(
										new Pen(SystemColors.ControlLightLight),
										x,
										y,
										x,
										y + 2);
						graphics.DrawLine(
										new Pen(SystemColors.ControlLightLight),
										x,
										y,
										x + 88,
										y);
						graphics.DrawLine(
										new Pen(SystemColors.ControlDark),
										x,
										y + 2,
										x + 88,
										y + 2);
						graphics.DrawLine(
										new Pen(SystemColors.ControlDark),
										x + 88,
										y,
										x + 88,
										y + 2);
						break;

					case DotStyle.XP:
						for (int i = 0; i < 18; i++)
						{
							graphics.DrawRectangle( // light dot
											new Pen(SystemColors.ControlLight),
											x + (i * 5),
											y,
											2,
											2);
							graphics.DrawRectangle( // light light dot
											new Pen(SystemColors.ControlLightLight),
											x + 1 + (i * 5),
											y + 1,
											1,
											1);
							graphics.DrawRectangle( // dark dark dot
											new Pen(SystemColors.ControlDarkDark),
											x + (i * 5),
											y,
											1,
											1);
							graphics.DrawLine( // dark fill
											new Pen(SystemColors.ControlDark),
											x + (i * 5),
											y,
											x + (i * 5) + 1,
											y);
							graphics.DrawLine(
											new Pen(SystemColors.ControlDark),
											x + (i * 5),
											y,
											x + (i * 5),
											y + 1);
						}
						break;

					case DotStyle.Lines:
						for (int i = 0; i < 44; i++)
							graphics.DrawLine(
											new Pen(SystemColors.ControlDark),
											x + (i * 2),
											y,
											x + (i * 2),
											y + 2);
						break;
				}
			}

//			if (borderStyle != System.Windows.Forms.Border3DStyle.Flat)
			ControlPaint.DrawBorder3D(graphics, ClientRectangle, borderStyle);
		}

		// This creates a point array to draw a arrow-like polygon
//		private static GraphicsPath ArrowPointArray(int x, int y, DockStyle dock, bool visible)
//		{
//			Point[] point = new Point[3];
//
//			// decide which direction the arrow will point
//			if (   (dock == DockStyle.Right && visible)
//				|| (dock == DockStyle.Left && !visible))
//			{
//				// right arrow
//				point[0] = new Point(x, y);
//				point[1] = new Point(x + TriWidth, y + TriHeight / 2);
//				point[2] = new Point(x, y + TriHeight);
//			}
//			else if ((dock == DockStyle.Right && !visible)
//				||   (dock == DockStyle.Left   && visible))
//			{
//				// left arrow
//				point[0] = new Point(x + TriWidth, y);
//				point[1] = new Point(x, y + TriHeight / 2);
//				point[2] = new Point(x + TriWidth, y + TriHeight);
//			}
//			// Up/Down arrows added in v1.2
//			else if ((dock == DockStyle.Top     && visible)
//				||   (dock == DockStyle.Bottom && !visible))
//			{
//				// up arrow
//				point[0] = new Point(x + TriHeight / 2, y);
//				point[1] = new Point(x + TriHeight, y + TriWidth);
//				point[2] = new Point(x, y + TriWidth);
//			}
//			else if ((dock == DockStyle.Top   && !visible)
//				||   (dock == DockStyle.Bottom && visible))
//			{
//				// down arrow
//				point[0] = new Point(x,y);
//				point[1] = new Point(x + TriHeight, y);
//				point[2] = new Point(x + TriHeight / 2, y + TriWidth);
//			}
//
//			return point;
//		}

		// this method was borrowed from the RichUI Control library by Sajith M
		private static Color CalculateColor(Color front, Color back, int alpha)
		{
			// solid color obtained as a result of alpha-blending
			Color colorf = Color.FromArgb(255, front);
			Color colorb = Color.FromArgb(255, back);

			float fRed   = colorf.R * alpha / 255 + colorb.R * ((float)(255 - alpha) / 255);
			float fGreen = colorf.G * alpha / 255 + colorb.G * ((float)(255 - alpha) / 255);
			float fBlue  = colorf.B * alpha / 255 + colorb.B * ((float)(255 - alpha) / 255);

			return Color.FromArgb((int)fRed, (int)fGreen, (int)fBlue);
		}

		#endregion Implementation
	}


	/// <summary>
	/// A simple designer class for the CollapsibleSplitter control to remove
	/// unwanted properties at design time.
	/// </summary>
	internal sealed class CollapsibleSplitterDesigner
		:
			System.Windows.Forms.Design.ControlDesigner
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
		protected override void PreFilterProperties(System.Collections.IDictionary properties)
		{
			properties.Remove("BorderStyle");
		}
	}
}
