using System;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace MapView
{
	internal sealed class MapTreeInputBox
		:
			Form
	{
		/// <summary>
		/// The possible box-types.
		/// </summary>
		internal enum BoxType
		{
			AddGroup,
			AddCategory,
			EditGroup,
			EditCategory
		}


		#region Fields (static)
		private const string NewGroup    = "Add Group";
		private const string NewCategory = "Add Category";
		private const string RenGroup    = "Relabel Group";
		private const string RenCategory = "Relabel Category";
		#endregion Fields (static)


		#region Properties
		private BoxType InputBoxType
		{ get; set; }

		private string GroupLabel
		{ get; set; }

		/// <summary>
		/// Gets/Sets the text in the textbox.
		/// </summary>
		internal string Label
		{
			get { return tb_Input.Text; }
			set { tb_Input.Text = value; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Constructs an inputbox for adding/editing the group- or
		/// category-labels of MainView's Maptree.
		/// </summary>
		/// <param name="infoAbove">info about the add/edit</param>
		/// <param name="infoBelow">more info about the add/edit</param>
		/// <param name="boxType">type of box</param>
		/// <param name="labelGroup">label of the group if applicable</param>
		internal MapTreeInputBox(
				string infoAbove,
				string infoBelow,
				BoxType boxType,
				string labelGroup)
		{
			InitializeComponent();

			lbl_InfoTop   .Text = infoAbove;
			lbl_InfoBottom.Text = infoBelow;

			switch (InputBoxType = boxType)
			{
				case BoxType.AddGroup:
					Text = NewGroup;
					lbl_Parent.Text = "@root";
					break;

				case BoxType.EditGroup:
					Text = RenGroup;
					lbl_Parent.Text = "@root";
					break;

				case BoxType.AddCategory:
					Text = NewCategory;
					lbl_Parent.Text = "@" + labelGroup;
					break;

				case BoxType.EditCategory:
					Text = RenCategory;
					lbl_Parent.Text = "@" + labelGroup;
					break;
			}

			GroupLabel = labelGroup;

			tb_Input.Select();
		}
		#endregion cTor


		#region Events
		private void OnAcceptClick(object sender, EventArgs e)
		{
			Label = Label.Trim();

			switch (InputBoxType)
			{
				case BoxType.AddGroup:
				case BoxType.EditGroup:
					if (String.IsNullOrEmpty(Label))
					{
						ShowError("A group label has not been specified.");
						tb_Input.Select();
					}
					else if (!Label.StartsWith("ufo",  StringComparison.OrdinalIgnoreCase)
						&&   !Label.StartsWith("tftd", StringComparison.OrdinalIgnoreCase))
					{
						ShowError(Infobox.SplitString("The group label needs to start with"
								+ " UFO or TFTD (case insensitive)."));
						tb_Input.Select();
					}
					else if (Label.StartsWith("ufo", StringComparison.OrdinalIgnoreCase)
						&& String.IsNullOrEmpty(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryUfo)))
					{
						ShowError(Infobox.SplitString("UFO has not been configured. If you"
								+ " have UFO resources and want to set the configuration for UFO,"
								+ " run the Configurator from MainView's Edit menu."));
						tb_Input.Select();
					}
					else if (Label.StartsWith("tftd", StringComparison.OrdinalIgnoreCase)
						&& String.IsNullOrEmpty(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryTftd)))
					{
						ShowError(Infobox.SplitString("TFTD has not been configured. If you"
								+ " have TFTD resources and want to set the configuration for TFTD,"
								+ " run the Configurator from MainView's Edit menu."));
						tb_Input.Select();
					}
					else
					{
						bool bork = false;

						var groups = TileGroupManager.TileGroups;
						foreach (var labelGroup in groups.Keys)
						{
							if (String.Equals(labelGroup, Label, StringComparison.OrdinalIgnoreCase))
							{
								bork = true;
								break;
							}
						}

						if (bork)
						{
							ShowError("The group label already exists.");
							tb_Input.Select();
						}
						else
							DialogResult = DialogResult.OK;
					}
					break;

				case BoxType.AddCategory:
				case BoxType.EditCategory:
					if (String.IsNullOrEmpty(Label))
					{
						ShowError("A category label has not been specified.");
						tb_Input.Select();
					}
					else
					{
						bool bork = false;

						var tilegroup = TileGroupManager.TileGroups[GroupLabel];
						foreach (var labelCategory in tilegroup.Categories.Keys)
						{
							if (String.Equals(labelCategory, Label, StringComparison.OrdinalIgnoreCase))
							{
								bork = true;
								break;
							}
						}

						if (bork)
						{
							ShowError("The category label already exists.");
							tb_Input.Select();
						}
						else
							DialogResult = DialogResult.OK;
					}
					break;
			}
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Wrapper for <see cref="Infobox"/>.
		/// </summary>
		/// <param name="head">the error string to show</param>
		private void ShowError(string head)
		{
			using (var f = new Infobox(
									"Error",
									head,
									null,
									Infobox.BoxType.Error))
			{
				f.ShowDialog(this);
			}
		}
		#endregion Methods


		#region Designer
		private Button btn_Ok;
		private Button btn_Cancel;
		private Label lbl_InfoTop;
		private TextBox tb_Input;
		private Panel pnl_Bottom;
		private Panel pnl_Top;
		private Label lbl_InfoBottom;
		private Label lbl_Parent;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btn_Ok = new System.Windows.Forms.Button();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.lbl_InfoTop = new System.Windows.Forms.Label();
			this.tb_Input = new System.Windows.Forms.TextBox();
			this.pnl_Bottom = new System.Windows.Forms.Panel();
			this.pnl_Top = new System.Windows.Forms.Panel();
			this.lbl_Parent = new System.Windows.Forms.Label();
			this.lbl_InfoBottom = new System.Windows.Forms.Label();
			this.pnl_Bottom.SuspendLayout();
			this.pnl_Top.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_Ok
			// 
			this.btn_Ok.Location = new System.Drawing.Point(116, 0);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new System.Drawing.Size(80, 25);
			this.btn_Ok.TabIndex = 0;
			this.btn_Ok.Text = "Ok";
			this.btn_Ok.Click += new System.EventHandler(this.OnAcceptClick);
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(201, 0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(80, 25);
			this.btn_Cancel.TabIndex = 1;
			this.btn_Cancel.Text = "Cancel";
			// 
			// lbl_InfoTop
			// 
			this.lbl_InfoTop.Location = new System.Drawing.Point(5, 25);
			this.lbl_InfoTop.Name = "lbl_InfoTop";
			this.lbl_InfoTop.Padding = new System.Windows.Forms.Padding(10, 0, 8, 0);
			this.lbl_InfoTop.Size = new System.Drawing.Size(385, 50);
			this.lbl_InfoTop.TabIndex = 1;
			this.lbl_InfoTop.Text = "lblInfoTop";
			this.lbl_InfoTop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tb_Input
			// 
			this.tb_Input.Location = new System.Drawing.Point(20, 80);
			this.tb_Input.Name = "tb_Input";
			this.tb_Input.Size = new System.Drawing.Size(355, 19);
			this.tb_Input.TabIndex = 2;
			// 
			// pnl_Bottom
			// 
			this.pnl_Bottom.Controls.Add(this.btn_Ok);
			this.pnl_Bottom.Controls.Add(this.btn_Cancel);
			this.pnl_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnl_Bottom.Location = new System.Drawing.Point(0, 147);
			this.pnl_Bottom.Name = "pnl_Bottom";
			this.pnl_Bottom.Size = new System.Drawing.Size(394, 29);
			this.pnl_Bottom.TabIndex = 1;
			// 
			// pnl_Top
			// 
			this.pnl_Top.Controls.Add(this.lbl_Parent);
			this.pnl_Top.Controls.Add(this.lbl_InfoBottom);
			this.pnl_Top.Controls.Add(this.tb_Input);
			this.pnl_Top.Controls.Add(this.lbl_InfoTop);
			this.pnl_Top.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl_Top.Location = new System.Drawing.Point(0, 0);
			this.pnl_Top.Name = "pnl_Top";
			this.pnl_Top.Size = new System.Drawing.Size(394, 147);
			this.pnl_Top.TabIndex = 0;
			// 
			// lbl_Parent
			// 
			this.lbl_Parent.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_Parent.ForeColor = System.Drawing.SystemColors.Highlight;
			this.lbl_Parent.Location = new System.Drawing.Point(5, 5);
			this.lbl_Parent.Name = "lbl_Parent";
			this.lbl_Parent.Padding = new System.Windows.Forms.Padding(10, 0, 8, 0);
			this.lbl_Parent.Size = new System.Drawing.Size(385, 15);
			this.lbl_Parent.TabIndex = 0;
			this.lbl_Parent.Text = "lblParent";
			this.lbl_Parent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_InfoBottom
			// 
			this.lbl_InfoBottom.Location = new System.Drawing.Point(5, 105);
			this.lbl_InfoBottom.Name = "lbl_InfoBottom";
			this.lbl_InfoBottom.Padding = new System.Windows.Forms.Padding(10, 0, 8, 0);
			this.lbl_InfoBottom.Size = new System.Drawing.Size(385, 40);
			this.lbl_InfoBottom.TabIndex = 3;
			this.lbl_InfoBottom.Text = "lblInfoBottom";
			this.lbl_InfoBottom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// MapTreeInputBox
			// 
			this.AcceptButton = this.btn_Ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(394, 176);
			this.Controls.Add(this.pnl_Top);
			this.Controls.Add(this.pnl_Bottom);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 200);
			this.Name = "MapTreeInputBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.pnl_Bottom.ResumeLayout(false);
			this.pnl_Top.ResumeLayout(false);
			this.pnl_Top.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
