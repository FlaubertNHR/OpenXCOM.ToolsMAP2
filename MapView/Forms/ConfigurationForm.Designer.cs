using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace MapView
{
	internal sealed partial class ConfigurationForm
	{
		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components;

		private Label labelUfo;
		private Label labelTftd;
		private Button btnFindUfo;
		private Button btnFindTftd;
		private TextBox tbUfo;
		private TextBox tbTftd;
		private Button btnOk;
		private Panel pUfo;
		private Panel pTftd;
		private Button btnCancel;
		private Label lblInfo;
		private CheckBox cbResources;
		private GroupBox gbResources;
		private GroupBox gbOptions;
		private RadioButton rbTilesetsTpl;
		private RadioButton rbTilesets;
		private Label lblResources;
		private Label lblTilesets;
		private CheckBox cbTilesets;
		private ToolTip toolTip1;


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


		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tbUfo = new System.Windows.Forms.TextBox();
			this.tbTftd = new System.Windows.Forms.TextBox();
			this.labelUfo = new System.Windows.Forms.Label();
			this.labelTftd = new System.Windows.Forms.Label();
			this.btnFindUfo = new System.Windows.Forms.Button();
			this.btnFindTftd = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.pUfo = new System.Windows.Forms.Panel();
			this.pTftd = new System.Windows.Forms.Panel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblInfo = new System.Windows.Forms.Label();
			this.cbResources = new System.Windows.Forms.CheckBox();
			this.gbResources = new System.Windows.Forms.GroupBox();
			this.gbOptions = new System.Windows.Forms.GroupBox();
			this.cbTilesets = new System.Windows.Forms.CheckBox();
			this.lblTilesets = new System.Windows.Forms.Label();
			this.rbTilesetsTpl = new System.Windows.Forms.RadioButton();
			this.rbTilesets = new System.Windows.Forms.RadioButton();
			this.lblResources = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pUfo.SuspendLayout();
			this.pTftd.SuspendLayout();
			this.gbResources.SuspendLayout();
			this.gbOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbUfo
			// 
			this.tbUfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbUfo.Location = new System.Drawing.Point(40, 2);
			this.tbUfo.Margin = new System.Windows.Forms.Padding(0);
			this.tbUfo.Name = "tbUfo";
			this.tbUfo.Size = new System.Drawing.Size(474, 19);
			this.tbUfo.TabIndex = 1;
			this.toolTip1.SetToolTip(this.tbUfo, "UFO installation folder");
			// 
			// tbTftd
			// 
			this.tbTftd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbTftd.Location = new System.Drawing.Point(40, 2);
			this.tbTftd.Margin = new System.Windows.Forms.Padding(0);
			this.tbTftd.Name = "tbTftd";
			this.tbTftd.Size = new System.Drawing.Size(474, 19);
			this.tbTftd.TabIndex = 1;
			this.toolTip1.SetToolTip(this.tbTftd, "TFTD installation folder");
			// 
			// labelUfo
			// 
			this.labelUfo.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelUfo.Location = new System.Drawing.Point(5, 5);
			this.labelUfo.Margin = new System.Windows.Forms.Padding(0);
			this.labelUfo.Name = "labelUfo";
			this.labelUfo.Size = new System.Drawing.Size(35, 15);
			this.labelUfo.TabIndex = 0;
			this.labelUfo.Text = "UFO";
			this.labelUfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelUfo, "UFO installation folder");
			// 
			// labelTftd
			// 
			this.labelTftd.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTftd.Location = new System.Drawing.Point(5, 5);
			this.labelTftd.Margin = new System.Windows.Forms.Padding(0);
			this.labelTftd.Name = "labelTftd";
			this.labelTftd.Size = new System.Drawing.Size(35, 15);
			this.labelTftd.TabIndex = 0;
			this.labelTftd.Text = "TFTD";
			this.labelTftd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.labelTftd, "TFTD installation folder");
			// 
			// btnFindUfo
			// 
			this.btnFindUfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindUfo.Location = new System.Drawing.Point(515, 0);
			this.btnFindUfo.Margin = new System.Windows.Forms.Padding(0);
			this.btnFindUfo.Name = "btnFindUfo";
			this.btnFindUfo.Size = new System.Drawing.Size(30, 22);
			this.btnFindUfo.TabIndex = 2;
			this.btnFindUfo.Text = "...";
			this.toolTip1.SetToolTip(this.btnFindUfo, "Browse for UFO installation folder");
			this.btnFindUfo.Click += new System.EventHandler(this.OnFindUfoClick);
			// 
			// btnFindTftd
			// 
			this.btnFindTftd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFindTftd.Location = new System.Drawing.Point(515, 0);
			this.btnFindTftd.Margin = new System.Windows.Forms.Padding(0);
			this.btnFindTftd.Name = "btnFindTftd";
			this.btnFindTftd.Size = new System.Drawing.Size(30, 22);
			this.btnFindTftd.TabIndex = 2;
			this.btnFindTftd.Text = "...";
			this.toolTip1.SetToolTip(this.btnFindTftd, "Browse for TFTD installation folder");
			this.btnFindTftd.Click += new System.EventHandler(this.OnFindTftdClick);
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(100, 245);
			this.btnOk.Margin = new System.Windows.Forms.Padding(0);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(165, 25);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "E=mc^2";
			this.btnOk.Click += new System.EventHandler(this.OnAcceptClick);
			// 
			// pUfo
			// 
			this.pUfo.Controls.Add(this.tbUfo);
			this.pUfo.Controls.Add(this.labelUfo);
			this.pUfo.Controls.Add(this.btnFindUfo);
			this.pUfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.pUfo.Location = new System.Drawing.Point(3, 50);
			this.pUfo.Margin = new System.Windows.Forms.Padding(0);
			this.pUfo.Name = "pUfo";
			this.pUfo.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this.pUfo.Size = new System.Drawing.Size(546, 25);
			this.pUfo.TabIndex = 1;
			// 
			// pTftd
			// 
			this.pTftd.Controls.Add(this.tbTftd);
			this.pTftd.Controls.Add(this.labelTftd);
			this.pTftd.Controls.Add(this.btnFindTftd);
			this.pTftd.Dock = System.Windows.Forms.DockStyle.Top;
			this.pTftd.Location = new System.Drawing.Point(3, 75);
			this.pTftd.Margin = new System.Windows.Forms.Padding(0);
			this.pTftd.Name = "pTftd";
			this.pTftd.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this.pTftd.Size = new System.Drawing.Size(546, 25);
			this.pTftd.TabIndex = 2;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(290, 245);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(165, 25);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// lblInfo
			// 
			this.lblInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblInfo.Location = new System.Drawing.Point(3, 15);
			this.lblInfo.Margin = new System.Windows.Forms.Padding(0);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Padding = new System.Windows.Forms.Padding(5);
			this.lblInfo.Size = new System.Drawing.Size(546, 35);
			this.lblInfo.TabIndex = 0;
			this.lblInfo.Text = "Enter the paths to either or both the UFO and TFTD resource (installation) folder" +
	"s. These need to be the respective parent folder(s) of the MAPS, ROUTES, and TER" +
	"RAIN subfolders.";
			// 
			// cbResources
			// 
			this.cbResources.Checked = true;
			this.cbResources.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbResources.Location = new System.Drawing.Point(20, 30);
			this.cbResources.Margin = new System.Windows.Forms.Padding(0);
			this.cbResources.Name = "cbResources";
			this.cbResources.Size = new System.Drawing.Size(525, 20);
			this.cbResources.TabIndex = 1;
			this.cbResources.Text = "create or replace the XCOM resource paths file [MapResources.yml]";
			this.toolTip1.SetToolTip(this.cbResources, "Create paths to stock UFO/TFTD installations");
			this.cbResources.UseVisualStyleBackColor = true;
			this.cbResources.CheckedChanged += new System.EventHandler(this.OnResourcesCheckedChanged);
			// 
			// gbResources
			// 
			this.gbResources.Controls.Add(this.pTftd);
			this.gbResources.Controls.Add(this.pUfo);
			this.gbResources.Controls.Add(this.lblInfo);
			this.gbResources.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbResources.Location = new System.Drawing.Point(0, 135);
			this.gbResources.Margin = new System.Windows.Forms.Padding(0);
			this.gbResources.Name = "gbResources";
			this.gbResources.Size = new System.Drawing.Size(552, 105);
			this.gbResources.TabIndex = 1;
			this.gbResources.TabStop = false;
			this.gbResources.Text = "XCOM Resource folders";
			// 
			// gbOptions
			// 
			this.gbOptions.Controls.Add(this.cbTilesets);
			this.gbOptions.Controls.Add(this.lblTilesets);
			this.gbOptions.Controls.Add(this.rbTilesetsTpl);
			this.gbOptions.Controls.Add(this.rbTilesets);
			this.gbOptions.Controls.Add(this.cbResources);
			this.gbOptions.Controls.Add(this.lblResources);
			this.gbOptions.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbOptions.Location = new System.Drawing.Point(0, 0);
			this.gbOptions.Margin = new System.Windows.Forms.Padding(0);
			this.gbOptions.Name = "gbOptions";
			this.gbOptions.Size = new System.Drawing.Size(552, 135);
			this.gbOptions.TabIndex = 0;
			this.gbOptions.TabStop = false;
			this.gbOptions.Text = "Options";
			// 
			// cbTilesets
			// 
			this.cbTilesets.Checked = true;
			this.cbTilesets.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbTilesets.Location = new System.Drawing.Point(20, 70);
			this.cbTilesets.Margin = new System.Windows.Forms.Padding(0);
			this.cbTilesets.Name = "cbTilesets";
			this.cbTilesets.Size = new System.Drawing.Size(525, 20);
			this.cbTilesets.TabIndex = 3;
			this.cbTilesets.Text = "create or replace a tileset configuration file [MapTilesets.yml | tpl]";
			this.toolTip1.SetToolTip(this.cbTilesets, "Create default tileset configuration for MapView");
			this.cbTilesets.UseVisualStyleBackColor = true;
			this.cbTilesets.CheckedChanged += new System.EventHandler(this.OnTilesetsCheckedChanged);
			// 
			// lblTilesets
			// 
			this.lblTilesets.Location = new System.Drawing.Point(5, 55);
			this.lblTilesets.Margin = new System.Windows.Forms.Padding(0);
			this.lblTilesets.Name = "lblTilesets";
			this.lblTilesets.Size = new System.Drawing.Size(540, 15);
			this.lblTilesets.TabIndex = 2;
			this.lblTilesets.Text = "Tileset configuration";
			this.toolTip1.SetToolTip(this.lblTilesets, "Configuration file in the settings subfolder");
			// 
			// rbTilesetsTpl
			// 
			this.rbTilesetsTpl.Location = new System.Drawing.Point(35, 110);
			this.rbTilesetsTpl.Margin = new System.Windows.Forms.Padding(0);
			this.rbTilesetsTpl.Name = "rbTilesetsTpl";
			this.rbTilesetsTpl.Size = new System.Drawing.Size(510, 20);
			this.rbTilesetsTpl.TabIndex = 5;
			this.rbTilesetsTpl.TabStop = true;
			this.rbTilesetsTpl.Text = "generate a template file for the stock UFO and TFTD tilesets [*.tpl]";
			this.toolTip1.SetToolTip(this.rbTilesetsTpl, "Creates a generic template with stock tilesets");
			this.rbTilesetsTpl.UseVisualStyleBackColor = true;
			// 
			// rbTilesets
			// 
			this.rbTilesets.Checked = true;
			this.rbTilesets.Location = new System.Drawing.Point(35, 90);
			this.rbTilesets.Margin = new System.Windows.Forms.Padding(0);
			this.rbTilesets.Name = "rbTilesets";
			this.rbTilesets.Size = new System.Drawing.Size(510, 20);
			this.rbTilesets.TabIndex = 4;
			this.rbTilesets.TabStop = true;
			this.rbTilesets.Text = "create or replace your metadata file for tilesets [*.yml]";
			this.toolTip1.SetToolTip(this.rbTilesets, "WARNING : This will replace any custom tileset configuration");
			this.rbTilesets.UseVisualStyleBackColor = true;
			// 
			// lblResources
			// 
			this.lblResources.Location = new System.Drawing.Point(5, 15);
			this.lblResources.Margin = new System.Windows.Forms.Padding(0);
			this.lblResources.Name = "lblResources";
			this.lblResources.Size = new System.Drawing.Size(540, 15);
			this.lblResources.TabIndex = 0;
			this.lblResources.Text = "XCOM Resource paths";
			this.toolTip1.SetToolTip(this.lblResources, "Configuration file in the settings subfolder");
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 10000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			this.toolTip1.UseAnimation = false;
			this.toolTip1.UseFading = false;
			// 
			// ConfigurationForm
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(552, 274);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.gbResources);
			this.Controls.Add(this.gbOptions);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(560, 300);
			this.MinimumSize = new System.Drawing.Size(560, 300);
			this.Name = "ConfigurationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Configuration options";
			this.pUfo.ResumeLayout(false);
			this.pUfo.PerformLayout();
			this.pTftd.ResumeLayout(false);
			this.pTftd.PerformLayout();
			this.gbResources.ResumeLayout(false);
			this.gbOptions.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
