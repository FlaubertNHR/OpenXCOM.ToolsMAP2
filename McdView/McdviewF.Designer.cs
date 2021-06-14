using System;
using System.ComponentModel;
using System.Windows.Forms;

using DSShared.Controls;


namespace McdView
{
	public sealed partial class McdviewF
	{
		#region Designer
		private IContainer components;

		private MainMenu mmMainMenu;
		private MenuItem miFileMenu;
		private MenuItem miCreate;
		private MenuItem miSeparator0;
		private MenuItem miSaveRecordsSprites;
		private MenuItem miSeparator4;
		private MenuItem miOpen;
		private MenuItem miSave;
		private MenuItem miSaveas;
		private MenuItem miReload;
		private MenuItem miSeparator1;
		private MenuItem miSaveSpriteset;
		private MenuItem miSeparator2;
		private MenuItem miQuit;
		private MenuItem miEditMenu;
		private MenuItem miCopier;
		private MenuItem miSeparator3;
		private MenuItem miZeroVals;
		private MenuItem miCheckVals;
		private MenuItem miResourcesMenu;
		private MenuItem miResourcesUfo;
		private MenuItem miResourcesTftd;
		private MenuItem miLoadMenu;
		private MenuItem miLoadScanGufo;
		private MenuItem miLoadLoFTufo;
		private MenuItem miLoadScanGtftd;
		private MenuItem miLoadLoFTtftd;
		private MenuItem miHelpMenu;
		private MenuItem miHelp;
		private MenuItem miAbout;

		private RecordLabel lbl20_scang;
		private RecordLabel lbl30_isslidingdoor;
		private RecordLabel lbl20;
		private RecordLabel lbl30;
		private RecordLabel lbl31;
		private RecordLabel lbl31_isblocklos;
		private RecordLabel lbl32;
		private RecordLabel lbl32_isdropthrou;
		private RecordLabel lbl33;
		private RecordLabel lbl33_isbigwall;
		private RecordLabel lbl34;
		private RecordLabel lbl34_isgravlift;
		private RecordLabel lbl35;
		private RecordLabel lbl35_ishingeddoor;
		private RecordLabel lbl36;
		private RecordLabel lbl36_isblockfire;
		private RecordLabel lbl37;
		private RecordLabel lbl37_isblocksmoke;
		private RecordLabel lbl38;
		private RecordLabel lbl38_;
		private RecordLabel lbl39;
		private RecordLabel lbl39_tuwalk;
		private RecordLabel lbl40;
		private RecordLabel lbl40_tuslide;
		private RecordLabel lbl41;
		private RecordLabel lbl41_tufly;
		private RecordLabel lbl42;
		private RecordLabel lbl42_armor;
		private RecordLabel lbl43;
		private RecordLabel lbl43_heblock;
		private RecordLabel lbl44;
		private RecordLabel lbl44_deathid;
		private RecordLabel lbl45;
		private RecordLabel lbl45_fireresist;
		private RecordLabel lbl46;
		private RecordLabel lbl46_alternateid;
		private RecordLabel lbl47;
		private RecordLabel lbl47_;
		private RecordLabel lbl48;
		private RecordLabel lbl48_terrainoffset;
		private RecordLabel lbl49;
		private RecordLabel lbl49_spriteoffset;
		private RecordLabel lbl50;
		private RecordLabel lbl50_;
		private RecordLabel lbl51;
		private RecordLabel lbl51_lightblock;
		private RecordLabel lbl52;
		private RecordLabel lbl52_footsound;
		private RecordLabel lbl53;
		private RecordLabel lbl53_parttype;
		private RecordLabel lbl54;
		private RecordLabel lbl54_hetype;
		private RecordLabel lbl55;
		private RecordLabel lbl55_hestrength;
		private RecordLabel lbl56;
		private RecordLabel lbl56_smokeblock;
		private RecordLabel lbl57;
		private RecordLabel lbl57_fuel;
		private RecordLabel lbl58;
		private RecordLabel lbl58_lightintensity;
		private RecordLabel lbl59;
		private RecordLabel lbl59_specialtype;
		private RecordLabel lbl60;
		private RecordLabel lbl60_isbaseobject;
		private RecordLabel lbl61;
		private RecordLabel lbl61_;
		private RecordLabel lbl19;
		private RecordLabel lbl19_loft11;
		private RecordLabel lbl18;
		private RecordLabel lbl18_loft10;
		private RecordLabel lbl17;
		private RecordLabel lbl17_loft09;
		private RecordLabel lbl16;
		private RecordLabel lbl16_loft08;
		private RecordLabel lbl15;
		private RecordLabel lbl15_loft07;
		private RecordLabel lbl14;
		private RecordLabel lbl14_loft06;
		private RecordLabel lbl13;
		private RecordLabel lbl13_loft05;
		private RecordLabel lbl12;
		private RecordLabel lbl12_loft04;
		private RecordLabel lbl11;
		private RecordLabel lbl11_loft03;
		private RecordLabel lbl10;
		private RecordLabel lbl10_loft02;
		private RecordLabel lbl09;
		private RecordLabel lbl09_loft01;
		private RecordLabel lbl08;
		private RecordLabel lbl08_loft00;
		private RecordLabel lbl00;
		private RecordLabel lbl00_phase0;
		private RecordLabel lbl01;
		private RecordLabel lbl01_phase1;
		private RecordLabel lbl02;
		private RecordLabel lbl02_phase2;
		private RecordLabel lbl03;
		private RecordLabel lbl03_phase3;
		private RecordLabel lbl07;
		private RecordLabel lbl07_phase7;
		private RecordLabel lbl06;
		private RecordLabel lbl06_phase6;
		private RecordLabel lbl05;
		private RecordLabel lbl05_phase5;
		private RecordLabel lbl04;
		private RecordLabel lbl04_phase4;
		private RecordLabel lbl22;
		private RecordLabel lbl22_;
		private RecordLabel lbl23;
		private RecordLabel lbl23_;
		private RecordLabel lbl24;
		private RecordLabel lbl24_;
		private RecordLabel lbl25;
		private RecordLabel lbl25_;
		private RecordLabel lbl26;
		private RecordLabel lbl26_;
		private RecordLabel lbl27;
		private RecordLabel lbl27_;
		private RecordLabel lbl28;
		private RecordLabel lbl28_;
		private RecordLabel lbl29;
		private RecordLabel lbl29_;
		private StatusStrip ss_Statusbar;
		private GroupBox gb_Unused;
		private GroupBox gb_Loft;
		internal GroupBox gb_Sprites;
		private GroupBox gb_Collection;
		private GroupBox gb_Overhead;
		private GroupBox gb_Tu;
		private GroupBox gb_Elevation;
		private GroupBox gb_Block;
		private GroupBox gb_Door;
		private GroupBox gb_Step;
		private GroupBox gb_Explode;
		private GroupBox gb_Health;
		private GroupBox gb_General;
		private Label lbl_SpriteShade;
		private TextBox tb_SpriteShade;
		private RecordTextbox tb20_scang1;
		private RecordTextbox tb44_deathid;
		private RecordTextbox tb42_armor;
		private RecordTextbox tb38_;
		private RecordTextbox tb61_;
		private RecordTextbox tb50_;
		private RecordTextbox tb47_;
		private RecordTextbox tb29_;
		private RecordTextbox tb28_;
		private RecordTextbox tb27_;
		private RecordTextbox tb26_;
		private RecordTextbox tb25_;
		private RecordTextbox tb24_;
		private RecordTextbox tb23_;
		private RecordTextbox tb22_;
		private RecordTextbox tb41_tufly;
		private RecordTextbox tb40_tuslide;
		private RecordTextbox tb39_tuwalk;
		private RecordTextbox tb49_spriteoffset;
		private RecordTextbox tb48_terrainoffset;
		private RecordTextbox tb51_lightblock;
		private RecordTextbox tb56_smokeblock;
		private RecordTextbox tb43_heblock;
		private RecordTextbox tb37_isblocksmoke;
		private RecordTextbox tb36_isblockfire;
		private RecordTextbox tb31_isblocklos;
		private RecordTextbox tb46_alternateid;
		private RecordTextbox tb35_ishingeddoor;
		private RecordTextbox tb30_isslidingdoor;
		private RecordTextbox tb32_isdropthrou;
		private RecordTextbox tb52_footsound;
		private RecordTextbox tb57_fuel;
		private RecordTextbox tb45_fireresist;
		private RecordTextbox tb55_hestrength;
		private RecordTextbox tb54_hetype;
		private RecordTextbox tb58_lightintensity;
		private RecordTextbox tb60_isbaseobject;
		private RecordTextbox tb59_specialtype;
		private RecordTextbox tb34_isgravlift;
		private RecordTextbox tb33_isbigwall;
		private RecordTextbox tb53_parttype;
		private RecordTextbox tb07_phase7;
		private RecordTextbox tb06_phase6;
		private RecordTextbox tb05_phase5;
		private RecordTextbox tb04_phase4;
		private RecordTextbox tb03_phase3;
		private RecordTextbox tb02_phase2;
		private RecordTextbox tb01_phase1;
		private RecordTextbox tb00_phase0;
		private RecordTextbox tb19_loft11;
		private RecordTextbox tb18_loft10;
		private RecordTextbox tb17_loft09;
		private RecordTextbox tb16_loft08;
		private RecordTextbox tb15_loft07;
		private RecordTextbox tb14_loft06;
		private RecordTextbox tb13_loft05;
		private RecordTextbox tb12_loft04;
		private RecordTextbox tb11_loft03;
		private RecordTextbox tb10_loft02;
		private RecordTextbox tb09_loft01;
		private RecordTextbox tb08_loft00;
		private RecordTextbox tb20_scang2;
		private BufferedPanel pnl_Sprites;
		private BufferedPanel pnl_ScanGic;
		private LoftPanel pnl_Loft08;
		private LoftPanel pnl_Loft09;
		private LoftPanel pnl_Loft10;
		private LoftPanel pnl_Loft11;
		private LoftPanel pnl_Loft12;
		private LoftPanel pnl_Loft13;
		private LoftPanel pnl_Loft14;
		private LoftPanel pnl_Loft15;
		private LoftPanel pnl_Loft16;
		private LoftPanel pnl_Loft17;
		private LoftPanel pnl_Loft18;
		private LoftPanel pnl_Loft19;
		private GroupBox gb_Description;
		private Label lbl_Description;
		private CheckBox cb_Strict;
		private Label lbl_Strict;
		private ToolStripStatusLabel tssl_Overval;
		private TrackBar bar_SpriteShade;
		private BufferedPanel pnl_IsoLoft;
		private TrackBar bar_IsoLoft;
		private BufferedPanel pnl_bg;
		private ToolStripStatusLabel tssl_Records;
		private ToolStripStatusLabel tssl_Sprites;
		private ToolStripStatusLabel tssl_Offset;
		private ToolStripStatusLabel tssl_OffsetLast;
		private ToolStripStatusLabel tssl_OffsetAftr;


		/// <summary>
		/// Disposes resources used by the Form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The
		/// Forms designer might not be able to load this method if it was
		/// changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(McdviewF));
			this.mmMainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.miFileMenu = new System.Windows.Forms.MenuItem();
			this.miCreate = new System.Windows.Forms.MenuItem();
			this.miSeparator0 = new System.Windows.Forms.MenuItem();
			this.miSaveRecordsSprites = new System.Windows.Forms.MenuItem();
			this.miSeparator4 = new System.Windows.Forms.MenuItem();
			this.miOpen = new System.Windows.Forms.MenuItem();
			this.miSave = new System.Windows.Forms.MenuItem();
			this.miSaveas = new System.Windows.Forms.MenuItem();
			this.miReload = new System.Windows.Forms.MenuItem();
			this.miSeparator1 = new System.Windows.Forms.MenuItem();
			this.miSaveSpriteset = new System.Windows.Forms.MenuItem();
			this.miSeparator2 = new System.Windows.Forms.MenuItem();
			this.miQuit = new System.Windows.Forms.MenuItem();
			this.miEditMenu = new System.Windows.Forms.MenuItem();
			this.miCopier = new System.Windows.Forms.MenuItem();
			this.miSeparator3 = new System.Windows.Forms.MenuItem();
			this.miZeroVals = new System.Windows.Forms.MenuItem();
			this.miCheckVals = new System.Windows.Forms.MenuItem();
			this.miResourcesMenu = new System.Windows.Forms.MenuItem();
			this.miResourcesUfo = new System.Windows.Forms.MenuItem();
			this.miResourcesTftd = new System.Windows.Forms.MenuItem();
			this.miLoadMenu = new System.Windows.Forms.MenuItem();
			this.miLoadScanGufo = new System.Windows.Forms.MenuItem();
			this.miLoadLoFTufo = new System.Windows.Forms.MenuItem();
			this.miLoadScanGtftd = new System.Windows.Forms.MenuItem();
			this.miLoadLoFTtftd = new System.Windows.Forms.MenuItem();
			this.miHelpMenu = new System.Windows.Forms.MenuItem();
			this.miHelp = new System.Windows.Forms.MenuItem();
			this.miAbout = new System.Windows.Forms.MenuItem();
			this.lbl20_scang = new McdView.RecordLabel();
			this.lbl30_isslidingdoor = new McdView.RecordLabel();
			this.lbl20 = new McdView.RecordLabel();
			this.lbl30 = new McdView.RecordLabel();
			this.lbl31 = new McdView.RecordLabel();
			this.lbl31_isblocklos = new McdView.RecordLabel();
			this.lbl32 = new McdView.RecordLabel();
			this.lbl32_isdropthrou = new McdView.RecordLabel();
			this.lbl33 = new McdView.RecordLabel();
			this.lbl33_isbigwall = new McdView.RecordLabel();
			this.lbl34 = new McdView.RecordLabel();
			this.lbl34_isgravlift = new McdView.RecordLabel();
			this.lbl35 = new McdView.RecordLabel();
			this.lbl35_ishingeddoor = new McdView.RecordLabel();
			this.lbl36 = new McdView.RecordLabel();
			this.lbl36_isblockfire = new McdView.RecordLabel();
			this.lbl37 = new McdView.RecordLabel();
			this.lbl37_isblocksmoke = new McdView.RecordLabel();
			this.lbl38 = new McdView.RecordLabel();
			this.lbl38_ = new McdView.RecordLabel();
			this.lbl39 = new McdView.RecordLabel();
			this.lbl39_tuwalk = new McdView.RecordLabel();
			this.lbl40 = new McdView.RecordLabel();
			this.lbl40_tuslide = new McdView.RecordLabel();
			this.lbl41 = new McdView.RecordLabel();
			this.lbl41_tufly = new McdView.RecordLabel();
			this.lbl42 = new McdView.RecordLabel();
			this.lbl42_armor = new McdView.RecordLabel();
			this.lbl43 = new McdView.RecordLabel();
			this.lbl43_heblock = new McdView.RecordLabel();
			this.lbl44 = new McdView.RecordLabel();
			this.lbl44_deathid = new McdView.RecordLabel();
			this.lbl45 = new McdView.RecordLabel();
			this.lbl45_fireresist = new McdView.RecordLabel();
			this.lbl46 = new McdView.RecordLabel();
			this.lbl46_alternateid = new McdView.RecordLabel();
			this.lbl47 = new McdView.RecordLabel();
			this.lbl47_ = new McdView.RecordLabel();
			this.lbl48 = new McdView.RecordLabel();
			this.lbl48_terrainoffset = new McdView.RecordLabel();
			this.lbl49 = new McdView.RecordLabel();
			this.lbl49_spriteoffset = new McdView.RecordLabel();
			this.lbl50 = new McdView.RecordLabel();
			this.lbl50_ = new McdView.RecordLabel();
			this.lbl51 = new McdView.RecordLabel();
			this.lbl51_lightblock = new McdView.RecordLabel();
			this.lbl52 = new McdView.RecordLabel();
			this.lbl52_footsound = new McdView.RecordLabel();
			this.lbl53 = new McdView.RecordLabel();
			this.lbl53_parttype = new McdView.RecordLabel();
			this.lbl54 = new McdView.RecordLabel();
			this.lbl54_hetype = new McdView.RecordLabel();
			this.lbl55 = new McdView.RecordLabel();
			this.lbl55_hestrength = new McdView.RecordLabel();
			this.lbl56 = new McdView.RecordLabel();
			this.lbl56_smokeblock = new McdView.RecordLabel();
			this.lbl57 = new McdView.RecordLabel();
			this.lbl57_fuel = new McdView.RecordLabel();
			this.lbl58 = new McdView.RecordLabel();
			this.lbl58_lightintensity = new McdView.RecordLabel();
			this.lbl59 = new McdView.RecordLabel();
			this.lbl59_specialtype = new McdView.RecordLabel();
			this.lbl60 = new McdView.RecordLabel();
			this.lbl60_isbaseobject = new McdView.RecordLabel();
			this.lbl61 = new McdView.RecordLabel();
			this.lbl61_ = new McdView.RecordLabel();
			this.lbl19 = new McdView.RecordLabel();
			this.lbl19_loft11 = new McdView.RecordLabel();
			this.lbl18 = new McdView.RecordLabel();
			this.lbl18_loft10 = new McdView.RecordLabel();
			this.lbl17 = new McdView.RecordLabel();
			this.lbl17_loft09 = new McdView.RecordLabel();
			this.lbl16 = new McdView.RecordLabel();
			this.lbl16_loft08 = new McdView.RecordLabel();
			this.lbl15 = new McdView.RecordLabel();
			this.lbl15_loft07 = new McdView.RecordLabel();
			this.lbl14 = new McdView.RecordLabel();
			this.lbl14_loft06 = new McdView.RecordLabel();
			this.lbl13 = new McdView.RecordLabel();
			this.lbl13_loft05 = new McdView.RecordLabel();
			this.lbl12 = new McdView.RecordLabel();
			this.lbl12_loft04 = new McdView.RecordLabel();
			this.lbl11 = new McdView.RecordLabel();
			this.lbl11_loft03 = new McdView.RecordLabel();
			this.lbl10 = new McdView.RecordLabel();
			this.lbl10_loft02 = new McdView.RecordLabel();
			this.lbl09 = new McdView.RecordLabel();
			this.lbl09_loft01 = new McdView.RecordLabel();
			this.lbl08 = new McdView.RecordLabel();
			this.lbl08_loft00 = new McdView.RecordLabel();
			this.lbl00 = new McdView.RecordLabel();
			this.lbl00_phase0 = new McdView.RecordLabel();
			this.lbl01 = new McdView.RecordLabel();
			this.lbl01_phase1 = new McdView.RecordLabel();
			this.lbl02 = new McdView.RecordLabel();
			this.lbl02_phase2 = new McdView.RecordLabel();
			this.lbl03 = new McdView.RecordLabel();
			this.lbl03_phase3 = new McdView.RecordLabel();
			this.lbl07 = new McdView.RecordLabel();
			this.lbl07_phase7 = new McdView.RecordLabel();
			this.lbl06 = new McdView.RecordLabel();
			this.lbl06_phase6 = new McdView.RecordLabel();
			this.lbl05 = new McdView.RecordLabel();
			this.lbl05_phase5 = new McdView.RecordLabel();
			this.lbl04 = new McdView.RecordLabel();
			this.lbl04_phase4 = new McdView.RecordLabel();
			this.lbl22 = new McdView.RecordLabel();
			this.lbl22_ = new McdView.RecordLabel();
			this.lbl23 = new McdView.RecordLabel();
			this.lbl23_ = new McdView.RecordLabel();
			this.lbl24 = new McdView.RecordLabel();
			this.lbl24_ = new McdView.RecordLabel();
			this.lbl25 = new McdView.RecordLabel();
			this.lbl25_ = new McdView.RecordLabel();
			this.lbl26 = new McdView.RecordLabel();
			this.lbl26_ = new McdView.RecordLabel();
			this.lbl27 = new McdView.RecordLabel();
			this.lbl27_ = new McdView.RecordLabel();
			this.lbl28 = new McdView.RecordLabel();
			this.lbl28_ = new McdView.RecordLabel();
			this.lbl29 = new McdView.RecordLabel();
			this.lbl29_ = new McdView.RecordLabel();
			this.ss_Statusbar = new System.Windows.Forms.StatusStrip();
			this.tssl_Overval = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_Records = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_Sprites = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_Offset = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_OffsetLast = new System.Windows.Forms.ToolStripStatusLabel();
			this.tssl_OffsetAftr = new System.Windows.Forms.ToolStripStatusLabel();
			this.gb_Unused = new System.Windows.Forms.GroupBox();
			this.tb38_ = new McdView.RecordTextbox();
			this.tb61_ = new McdView.RecordTextbox();
			this.tb50_ = new McdView.RecordTextbox();
			this.tb47_ = new McdView.RecordTextbox();
			this.tb29_ = new McdView.RecordTextbox();
			this.tb28_ = new McdView.RecordTextbox();
			this.tb27_ = new McdView.RecordTextbox();
			this.tb26_ = new McdView.RecordTextbox();
			this.tb25_ = new McdView.RecordTextbox();
			this.tb24_ = new McdView.RecordTextbox();
			this.tb23_ = new McdView.RecordTextbox();
			this.tb22_ = new McdView.RecordTextbox();
			this.gb_Loft = new System.Windows.Forms.GroupBox();
			this.tb19_loft11 = new McdView.RecordTextbox();
			this.tb18_loft10 = new McdView.RecordTextbox();
			this.tb17_loft09 = new McdView.RecordTextbox();
			this.tb16_loft08 = new McdView.RecordTextbox();
			this.tb15_loft07 = new McdView.RecordTextbox();
			this.tb14_loft06 = new McdView.RecordTextbox();
			this.tb13_loft05 = new McdView.RecordTextbox();
			this.tb12_loft04 = new McdView.RecordTextbox();
			this.tb11_loft03 = new McdView.RecordTextbox();
			this.tb10_loft02 = new McdView.RecordTextbox();
			this.tb09_loft01 = new McdView.RecordTextbox();
			this.tb08_loft00 = new McdView.RecordTextbox();
			this.pnl_Loft08 = new McdView.LoftPanel();
			this.pnl_Loft09 = new McdView.LoftPanel();
			this.pnl_Loft10 = new McdView.LoftPanel();
			this.pnl_Loft11 = new McdView.LoftPanel();
			this.pnl_Loft12 = new McdView.LoftPanel();
			this.pnl_Loft13 = new McdView.LoftPanel();
			this.pnl_Loft14 = new McdView.LoftPanel();
			this.pnl_Loft15 = new McdView.LoftPanel();
			this.pnl_Loft16 = new McdView.LoftPanel();
			this.pnl_Loft17 = new McdView.LoftPanel();
			this.pnl_Loft18 = new McdView.LoftPanel();
			this.pnl_Loft19 = new McdView.LoftPanel();
			this.gb_Sprites = new System.Windows.Forms.GroupBox();
			this.tb07_phase7 = new McdView.RecordTextbox();
			this.tb06_phase6 = new McdView.RecordTextbox();
			this.tb05_phase5 = new McdView.RecordTextbox();
			this.tb04_phase4 = new McdView.RecordTextbox();
			this.tb03_phase3 = new McdView.RecordTextbox();
			this.tb02_phase2 = new McdView.RecordTextbox();
			this.tb01_phase1 = new McdView.RecordTextbox();
			this.tb00_phase0 = new McdView.RecordTextbox();
			this.pnl_Sprites = new DSShared.Controls.BufferedPanel();
			this.gb_Collection = new System.Windows.Forms.GroupBox();
			this.gb_Overhead = new System.Windows.Forms.GroupBox();
			this.tb20_scang2 = new McdView.RecordTextbox();
			this.tb20_scang1 = new McdView.RecordTextbox();
			this.pnl_ScanGic = new DSShared.Controls.BufferedPanel();
			this.gb_Tu = new System.Windows.Forms.GroupBox();
			this.tb41_tufly = new McdView.RecordTextbox();
			this.tb40_tuslide = new McdView.RecordTextbox();
			this.tb39_tuwalk = new McdView.RecordTextbox();
			this.gb_Elevation = new System.Windows.Forms.GroupBox();
			this.tb49_spriteoffset = new McdView.RecordTextbox();
			this.tb48_terrainoffset = new McdView.RecordTextbox();
			this.gb_Block = new System.Windows.Forms.GroupBox();
			this.tb51_lightblock = new McdView.RecordTextbox();
			this.tb56_smokeblock = new McdView.RecordTextbox();
			this.tb43_heblock = new McdView.RecordTextbox();
			this.tb37_isblocksmoke = new McdView.RecordTextbox();
			this.tb36_isblockfire = new McdView.RecordTextbox();
			this.tb31_isblocklos = new McdView.RecordTextbox();
			this.gb_Door = new System.Windows.Forms.GroupBox();
			this.tb46_alternateid = new McdView.RecordTextbox();
			this.tb35_ishingeddoor = new McdView.RecordTextbox();
			this.tb30_isslidingdoor = new McdView.RecordTextbox();
			this.gb_Step = new System.Windows.Forms.GroupBox();
			this.tb32_isdropthrou = new McdView.RecordTextbox();
			this.tb52_footsound = new McdView.RecordTextbox();
			this.gb_Explode = new System.Windows.Forms.GroupBox();
			this.tb57_fuel = new McdView.RecordTextbox();
			this.tb45_fireresist = new McdView.RecordTextbox();
			this.tb55_hestrength = new McdView.RecordTextbox();
			this.tb54_hetype = new McdView.RecordTextbox();
			this.gb_Health = new System.Windows.Forms.GroupBox();
			this.tb44_deathid = new McdView.RecordTextbox();
			this.tb42_armor = new McdView.RecordTextbox();
			this.gb_General = new System.Windows.Forms.GroupBox();
			this.tb58_lightintensity = new McdView.RecordTextbox();
			this.tb60_isbaseobject = new McdView.RecordTextbox();
			this.tb59_specialtype = new McdView.RecordTextbox();
			this.tb53_parttype = new McdView.RecordTextbox();
			this.tb34_isgravlift = new McdView.RecordTextbox();
			this.tb33_isbigwall = new McdView.RecordTextbox();
			this.lbl_SpriteShade = new System.Windows.Forms.Label();
			this.tb_SpriteShade = new System.Windows.Forms.TextBox();
			this.gb_Description = new System.Windows.Forms.GroupBox();
			this.lbl_Description = new System.Windows.Forms.Label();
			this.cb_Strict = new System.Windows.Forms.CheckBox();
			this.lbl_Strict = new System.Windows.Forms.Label();
			this.bar_SpriteShade = new System.Windows.Forms.TrackBar();
			this.pnl_IsoLoft = new DSShared.Controls.BufferedPanel();
			this.bar_IsoLoft = new System.Windows.Forms.TrackBar();
			this.pnl_bg = new DSShared.Controls.BufferedPanel();
			this.ss_Statusbar.SuspendLayout();
			this.gb_Unused.SuspendLayout();
			this.gb_Loft.SuspendLayout();
			this.gb_Sprites.SuspendLayout();
			this.gb_Overhead.SuspendLayout();
			this.gb_Tu.SuspendLayout();
			this.gb_Elevation.SuspendLayout();
			this.gb_Block.SuspendLayout();
			this.gb_Door.SuspendLayout();
			this.gb_Step.SuspendLayout();
			this.gb_Explode.SuspendLayout();
			this.gb_Health.SuspendLayout();
			this.gb_General.SuspendLayout();
			this.gb_Description.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bar_SpriteShade)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bar_IsoLoft)).BeginInit();
			this.pnl_bg.SuspendLayout();
			this.SuspendLayout();
			// 
			// mmMainMenu
			// 
			this.mmMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miFileMenu,
			this.miEditMenu,
			this.miResourcesMenu,
			this.miLoadMenu,
			this.miHelpMenu});
			// 
			// miFileMenu
			// 
			this.miFileMenu.Index = 0;
			this.miFileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miCreate,
			this.miSeparator0,
			this.miSaveRecordsSprites,
			this.miSeparator4,
			this.miOpen,
			this.miSave,
			this.miSaveas,
			this.miReload,
			this.miSeparator1,
			this.miSaveSpriteset,
			this.miSeparator2,
			this.miQuit});
			this.miFileMenu.Text = "&File";
			// 
			// miCreate
			// 
			this.miCreate.Index = 0;
			this.miCreate.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
			this.miCreate.Text = "C&reate MCD file ...";
			this.miCreate.Click += new System.EventHandler(this.OnClick_Create);
			// 
			// miSeparator0
			// 
			this.miSeparator0.Index = 1;
			this.miSeparator0.Text = "-";
			// 
			// miSaveRecordsSprites
			// 
			this.miSaveRecordsSprites.Enabled = false;
			this.miSaveRecordsSprites.Index = 2;
			this.miSaveRecordsSprites.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
			this.miSaveRecordsSprites.Text = "Save &MCD file and spriteset";
			this.miSaveRecordsSprites.Click += new System.EventHandler(this.OnClick_SaveRecordsSprites);
			// 
			// miSeparator4
			// 
			this.miSeparator4.Index = 3;
			this.miSeparator4.Text = "-";
			// 
			// miOpen
			// 
			this.miOpen.Index = 4;
			this.miOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.miOpen.Text = "&Open MCD file ...";
			this.miOpen.Click += new System.EventHandler(this.OnClick_Open);
			// 
			// miSave
			// 
			this.miSave.Enabled = false;
			this.miSave.Index = 5;
			this.miSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.miSave.Text = "&Save MCD file";
			this.miSave.Click += new System.EventHandler(this.OnClick_SaveRecords);
			// 
			// miSaveas
			// 
			this.miSaveas.Enabled = false;
			this.miSaveas.Index = 6;
			this.miSaveas.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
			this.miSaveas.Text = "Sav&e MCD file as ...";
			this.miSaveas.Click += new System.EventHandler(this.OnClick_SaveTerrainAs);
			// 
			// miReload
			// 
			this.miReload.Enabled = false;
			this.miReload.Index = 7;
			this.miReload.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			this.miReload.Text = "Re&load MCD file";
			this.miReload.Click += new System.EventHandler(this.OnClick_Reload);
			// 
			// miSeparator1
			// 
			this.miSeparator1.Index = 8;
			this.miSeparator1.Text = "-";
			// 
			// miSaveSpriteset
			// 
			this.miSaveSpriteset.Enabled = false;
			this.miSaveSpriteset.Index = 9;
			this.miSaveSpriteset.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.miSaveSpriteset.Text = "Save spr&iteset";
			this.miSaveSpriteset.Click += new System.EventHandler(this.OnClick_SaveSpriteset);
			// 
			// miSeparator2
			// 
			this.miSeparator2.Index = 10;
			this.miSeparator2.Text = "-";
			// 
			// miQuit
			// 
			this.miQuit.Index = 11;
			this.miQuit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
			this.miQuit.Text = "&Quit";
			this.miQuit.Click += new System.EventHandler(this.OnClick_Quit);
			// 
			// miEditMenu
			// 
			this.miEditMenu.Index = 1;
			this.miEditMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miCopier,
			this.miSeparator3,
			this.miZeroVals,
			this.miCheckVals});
			this.miEditMenu.Text = "&Edit";
			// 
			// miCopier
			// 
			this.miCopier.Index = 0;
			this.miCopier.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.miCopier.Text = "Open Co&pier panel ...";
			this.miCopier.Click += new System.EventHandler(this.OnClick_OpenCopier);
			// 
			// miSeparator3
			// 
			this.miSeparator3.Index = 1;
			this.miSeparator3.Text = "-";
			// 
			// miZeroVals
			// 
			this.miZeroVals.Enabled = false;
			this.miZeroVals.Index = 2;
			this.miZeroVals.Shortcut = System.Windows.Forms.Shortcut.Ctrl0;
			this.miZeroVals.Text = "&Zero record\'s values";
			this.miZeroVals.Click += new System.EventHandler(this.OnClick_ZeroVals);
			// 
			// miCheckVals
			// 
			this.miCheckVals.Enabled = false;
			this.miCheckVals.Index = 3;
			this.miCheckVals.Shortcut = System.Windows.Forms.Shortcut.CtrlK;
			this.miCheckVals.Text = "Chec&k record STRICT";
			this.miCheckVals.Click += new System.EventHandler(this.OnClick_CheckVals);
			// 
			// miResourcesMenu
			// 
			this.miResourcesMenu.Index = 2;
			this.miResourcesMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miResourcesUfo,
			this.miResourcesTftd});
			this.miResourcesMenu.Text = "&Resources";
			// 
			// miResourcesUfo
			// 
			this.miResourcesUfo.Checked = true;
			this.miResourcesUfo.Enabled = false;
			this.miResourcesUfo.Index = 0;
			this.miResourcesUfo.Shortcut = System.Windows.Forms.Shortcut.CtrlU;
			this.miResourcesUfo.Text = "&UFO";
			this.miResourcesUfo.Click += new System.EventHandler(this.OnClick_PaletteUfo);
			// 
			// miResourcesTftd
			// 
			this.miResourcesTftd.Enabled = false;
			this.miResourcesTftd.Index = 1;
			this.miResourcesTftd.Shortcut = System.Windows.Forms.Shortcut.CtrlT;
			this.miResourcesTftd.Text = "&TFTD";
			this.miResourcesTftd.Click += new System.EventHandler(this.OnClick_PaletteTftd);
			// 
			// miLoadMenu
			// 
			this.miLoadMenu.Index = 3;
			this.miLoadMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miLoadScanGufo,
			this.miLoadLoFTufo,
			this.miLoadScanGtftd,
			this.miLoadLoFTtftd});
			this.miLoadMenu.Text = "&Load";
			// 
			// miLoadScanGufo
			// 
			this.miLoadScanGufo.Index = 0;
			this.miLoadScanGufo.Text = "ufo - ScanG";
			this.miLoadScanGufo.Click += new System.EventHandler(this.OnClick_LoadScanGufo);
			// 
			// miLoadLoFTufo
			// 
			this.miLoadLoFTufo.Index = 1;
			this.miLoadLoFTufo.Text = "ufo - LoFT";
			this.miLoadLoFTufo.Click += new System.EventHandler(this.OnClick_LoadLoFTufo);
			// 
			// miLoadScanGtftd
			// 
			this.miLoadScanGtftd.Index = 2;
			this.miLoadScanGtftd.Text = "tftd - ScanG";
			this.miLoadScanGtftd.Click += new System.EventHandler(this.OnClick_LoadScanGtftd);
			// 
			// miLoadLoFTtftd
			// 
			this.miLoadLoFTtftd.Index = 3;
			this.miLoadLoFTtftd.Text = "tftd - LoFT";
			this.miLoadLoFTtftd.Click += new System.EventHandler(this.OnClick_LoadLoFTtftd);
			// 
			// miHelpMenu
			// 
			this.miHelpMenu.Index = 4;
			this.miHelpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miHelp,
			this.miAbout});
			this.miHelpMenu.Text = "&Help";
			// 
			// miHelp
			// 
			this.miHelp.Index = 0;
			this.miHelp.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
			this.miHelp.Text = "&Help";
			this.miHelp.Click += new System.EventHandler(this.OnClick_Help);
			// 
			// miAbout
			// 
			this.miAbout.Index = 1;
			this.miAbout.Shortcut = System.Windows.Forms.Shortcut.CtrlB;
			this.miAbout.Text = "A&bout";
			this.miAbout.Click += new System.EventHandler(this.OnClick_About);
			// 
			// lbl20_scang
			// 
			this.lbl20_scang.Location = new System.Drawing.Point(60, 15);
			this.lbl20_scang.Margin = new System.Windows.Forms.Padding(0);
			this.lbl20_scang.Name = "lbl20_scang";
			this.lbl20_scang.Size = new System.Drawing.Size(40, 15);
			this.lbl20_scang.TabIndex = 1;
			this.lbl20_scang.Text = "ScanG";
			this.lbl20_scang.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl20_scang.MouseEnter += new System.EventHandler(this.OnMouseEnterLabel20);
			// 
			// lbl30_isslidingdoor
			// 
			this.lbl30_isslidingdoor.Location = new System.Drawing.Point(40, 15);
			this.lbl30_isslidingdoor.Margin = new System.Windows.Forms.Padding(0);
			this.lbl30_isslidingdoor.Name = "lbl30_isslidingdoor";
			this.lbl30_isslidingdoor.Size = new System.Drawing.Size(85, 15);
			this.lbl30_isslidingdoor.TabIndex = 1;
			this.lbl30_isslidingdoor.Text = "isSlidingDoor";
			this.lbl30_isslidingdoor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl30_isslidingdoor.MouseEnter += new System.EventHandler(this.OnEnter30);
			// 
			// lbl20
			// 
			this.lbl20.Location = new System.Drawing.Point(10, 15);
			this.lbl20.Margin = new System.Windows.Forms.Padding(0);
			this.lbl20.Name = "lbl20";
			this.lbl20.Size = new System.Drawing.Size(50, 15);
			this.lbl20.TabIndex = 0;
			this.lbl20.Text = "#20|21";
			this.lbl20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl20.MouseEnter += new System.EventHandler(this.OnMouseEnterLabel20);
			// 
			// lbl30
			// 
			this.lbl30.Location = new System.Drawing.Point(10, 15);
			this.lbl30.Margin = new System.Windows.Forms.Padding(0);
			this.lbl30.Name = "lbl30";
			this.lbl30.Size = new System.Drawing.Size(30, 15);
			this.lbl30.TabIndex = 0;
			this.lbl30.Text = "#30";
			this.lbl30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl30.MouseEnter += new System.EventHandler(this.OnEnter30);
			// 
			// lbl31
			// 
			this.lbl31.Location = new System.Drawing.Point(10, 15);
			this.lbl31.Margin = new System.Windows.Forms.Padding(0);
			this.lbl31.Name = "lbl31";
			this.lbl31.Size = new System.Drawing.Size(30, 15);
			this.lbl31.TabIndex = 0;
			this.lbl31.Text = "#31";
			this.lbl31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl31.MouseEnter += new System.EventHandler(this.OnEnter31);
			// 
			// lbl31_isblocklos
			// 
			this.lbl31_isblocklos.Location = new System.Drawing.Point(40, 15);
			this.lbl31_isblocklos.Margin = new System.Windows.Forms.Padding(0);
			this.lbl31_isblocklos.Name = "lbl31_isblocklos";
			this.lbl31_isblocklos.Size = new System.Drawing.Size(85, 15);
			this.lbl31_isblocklos.TabIndex = 1;
			this.lbl31_isblocklos.Text = "isBlockLoS";
			this.lbl31_isblocklos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl31_isblocklos.MouseEnter += new System.EventHandler(this.OnEnter31);
			// 
			// lbl32
			// 
			this.lbl32.Location = new System.Drawing.Point(10, 35);
			this.lbl32.Margin = new System.Windows.Forms.Padding(0);
			this.lbl32.Name = "lbl32";
			this.lbl32.Size = new System.Drawing.Size(30, 15);
			this.lbl32.TabIndex = 3;
			this.lbl32.Text = "#32";
			this.lbl32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl32.MouseEnter += new System.EventHandler(this.OnEnter32);
			// 
			// lbl32_isdropthrou
			// 
			this.lbl32_isdropthrou.Location = new System.Drawing.Point(40, 35);
			this.lbl32_isdropthrou.Margin = new System.Windows.Forms.Padding(0);
			this.lbl32_isdropthrou.Name = "lbl32_isdropthrou";
			this.lbl32_isdropthrou.Size = new System.Drawing.Size(85, 15);
			this.lbl32_isdropthrou.TabIndex = 4;
			this.lbl32_isdropthrou.Text = "isDropThrou";
			this.lbl32_isdropthrou.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl32_isdropthrou.MouseEnter += new System.EventHandler(this.OnEnter32);
			// 
			// lbl33
			// 
			this.lbl33.Location = new System.Drawing.Point(10, 55);
			this.lbl33.Margin = new System.Windows.Forms.Padding(0);
			this.lbl33.Name = "lbl33";
			this.lbl33.Size = new System.Drawing.Size(30, 15);
			this.lbl33.TabIndex = 6;
			this.lbl33.Text = "#33";
			this.lbl33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl33.MouseEnter += new System.EventHandler(this.OnEnter33);
			// 
			// lbl33_isbigwall
			// 
			this.lbl33_isbigwall.Location = new System.Drawing.Point(40, 55);
			this.lbl33_isbigwall.Margin = new System.Windows.Forms.Padding(0);
			this.lbl33_isbigwall.Name = "lbl33_isbigwall";
			this.lbl33_isbigwall.Size = new System.Drawing.Size(85, 15);
			this.lbl33_isbigwall.TabIndex = 7;
			this.lbl33_isbigwall.Text = "isBigWall";
			this.lbl33_isbigwall.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl33_isbigwall.MouseEnter += new System.EventHandler(this.OnEnter33);
			// 
			// lbl34
			// 
			this.lbl34.Location = new System.Drawing.Point(10, 75);
			this.lbl34.Margin = new System.Windows.Forms.Padding(0);
			this.lbl34.Name = "lbl34";
			this.lbl34.Size = new System.Drawing.Size(30, 15);
			this.lbl34.TabIndex = 9;
			this.lbl34.Text = "#34";
			this.lbl34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl34.MouseEnter += new System.EventHandler(this.OnEnter34);
			// 
			// lbl34_isgravlift
			// 
			this.lbl34_isgravlift.Location = new System.Drawing.Point(40, 75);
			this.lbl34_isgravlift.Margin = new System.Windows.Forms.Padding(0);
			this.lbl34_isgravlift.Name = "lbl34_isgravlift";
			this.lbl34_isgravlift.Size = new System.Drawing.Size(85, 15);
			this.lbl34_isgravlift.TabIndex = 10;
			this.lbl34_isgravlift.Text = "isGravLift";
			this.lbl34_isgravlift.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl34_isgravlift.MouseEnter += new System.EventHandler(this.OnEnter34);
			// 
			// lbl35
			// 
			this.lbl35.Location = new System.Drawing.Point(10, 35);
			this.lbl35.Margin = new System.Windows.Forms.Padding(0);
			this.lbl35.Name = "lbl35";
			this.lbl35.Size = new System.Drawing.Size(30, 15);
			this.lbl35.TabIndex = 3;
			this.lbl35.Text = "#35";
			this.lbl35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl35.MouseEnter += new System.EventHandler(this.OnEnter35);
			// 
			// lbl35_ishingeddoor
			// 
			this.lbl35_ishingeddoor.Location = new System.Drawing.Point(40, 35);
			this.lbl35_ishingeddoor.Margin = new System.Windows.Forms.Padding(0);
			this.lbl35_ishingeddoor.Name = "lbl35_ishingeddoor";
			this.lbl35_ishingeddoor.Size = new System.Drawing.Size(85, 15);
			this.lbl35_ishingeddoor.TabIndex = 4;
			this.lbl35_ishingeddoor.Text = "isHingedDoor";
			this.lbl35_ishingeddoor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl35_ishingeddoor.MouseEnter += new System.EventHandler(this.OnEnter35);
			// 
			// lbl36
			// 
			this.lbl36.Location = new System.Drawing.Point(10, 35);
			this.lbl36.Margin = new System.Windows.Forms.Padding(0);
			this.lbl36.Name = "lbl36";
			this.lbl36.Size = new System.Drawing.Size(30, 15);
			this.lbl36.TabIndex = 3;
			this.lbl36.Text = "#36";
			this.lbl36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl36.MouseEnter += new System.EventHandler(this.OnEnter36);
			// 
			// lbl36_isblockfire
			// 
			this.lbl36_isblockfire.Location = new System.Drawing.Point(40, 35);
			this.lbl36_isblockfire.Margin = new System.Windows.Forms.Padding(0);
			this.lbl36_isblockfire.Name = "lbl36_isblockfire";
			this.lbl36_isblockfire.Size = new System.Drawing.Size(85, 15);
			this.lbl36_isblockfire.TabIndex = 4;
			this.lbl36_isblockfire.Text = "isBlockFire";
			this.lbl36_isblockfire.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl36_isblockfire.MouseEnter += new System.EventHandler(this.OnEnter36);
			// 
			// lbl37
			// 
			this.lbl37.Location = new System.Drawing.Point(10, 55);
			this.lbl37.Margin = new System.Windows.Forms.Padding(0);
			this.lbl37.Name = "lbl37";
			this.lbl37.Size = new System.Drawing.Size(30, 15);
			this.lbl37.TabIndex = 6;
			this.lbl37.Text = "#37";
			this.lbl37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl37.MouseEnter += new System.EventHandler(this.OnEnter37);
			// 
			// lbl37_isblocksmoke
			// 
			this.lbl37_isblocksmoke.Location = new System.Drawing.Point(40, 55);
			this.lbl37_isblocksmoke.Margin = new System.Windows.Forms.Padding(0);
			this.lbl37_isblocksmoke.Name = "lbl37_isblocksmoke";
			this.lbl37_isblocksmoke.Size = new System.Drawing.Size(85, 15);
			this.lbl37_isblocksmoke.TabIndex = 7;
			this.lbl37_isblocksmoke.Text = "isBlockSmoke";
			this.lbl37_isblocksmoke.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl37_isblocksmoke.MouseEnter += new System.EventHandler(this.OnEnter37);
			// 
			// lbl38
			// 
			this.lbl38.Location = new System.Drawing.Point(10, 175);
			this.lbl38.Margin = new System.Windows.Forms.Padding(0);
			this.lbl38.Name = "lbl38";
			this.lbl38.Size = new System.Drawing.Size(30, 15);
			this.lbl38.TabIndex = 24;
			this.lbl38.Text = "#38";
			this.lbl38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl38.MouseEnter += new System.EventHandler(this.OnEnter38);
			// 
			// lbl38_
			// 
			this.lbl38_.Location = new System.Drawing.Point(40, 175);
			this.lbl38_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl38_.Name = "lbl38_";
			this.lbl38_.Size = new System.Drawing.Size(85, 15);
			this.lbl38_.TabIndex = 25;
			this.lbl38_.Text = "LeftRightHalf";
			this.lbl38_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl38_.MouseEnter += new System.EventHandler(this.OnEnter38);
			// 
			// lbl39
			// 
			this.lbl39.Location = new System.Drawing.Point(10, 15);
			this.lbl39.Margin = new System.Windows.Forms.Padding(0);
			this.lbl39.Name = "lbl39";
			this.lbl39.Size = new System.Drawing.Size(30, 15);
			this.lbl39.TabIndex = 0;
			this.lbl39.Text = "#39";
			this.lbl39.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl39.MouseEnter += new System.EventHandler(this.OnEnter39);
			// 
			// lbl39_tuwalk
			// 
			this.lbl39_tuwalk.Location = new System.Drawing.Point(40, 15);
			this.lbl39_tuwalk.Margin = new System.Windows.Forms.Padding(0);
			this.lbl39_tuwalk.Name = "lbl39_tuwalk";
			this.lbl39_tuwalk.Size = new System.Drawing.Size(85, 15);
			this.lbl39_tuwalk.TabIndex = 1;
			this.lbl39_tuwalk.Text = "TuWalk";
			this.lbl39_tuwalk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl39_tuwalk.MouseEnter += new System.EventHandler(this.OnEnter39);
			// 
			// lbl40
			// 
			this.lbl40.Location = new System.Drawing.Point(10, 35);
			this.lbl40.Margin = new System.Windows.Forms.Padding(0);
			this.lbl40.Name = "lbl40";
			this.lbl40.Size = new System.Drawing.Size(30, 15);
			this.lbl40.TabIndex = 3;
			this.lbl40.Text = "#40";
			this.lbl40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl40.MouseEnter += new System.EventHandler(this.OnEnter40);
			// 
			// lbl40_tuslide
			// 
			this.lbl40_tuslide.Location = new System.Drawing.Point(40, 35);
			this.lbl40_tuslide.Margin = new System.Windows.Forms.Padding(0);
			this.lbl40_tuslide.Name = "lbl40_tuslide";
			this.lbl40_tuslide.Size = new System.Drawing.Size(85, 15);
			this.lbl40_tuslide.TabIndex = 4;
			this.lbl40_tuslide.Text = "TuSlide";
			this.lbl40_tuslide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl40_tuslide.MouseEnter += new System.EventHandler(this.OnEnter40);
			// 
			// lbl41
			// 
			this.lbl41.Location = new System.Drawing.Point(10, 55);
			this.lbl41.Margin = new System.Windows.Forms.Padding(0);
			this.lbl41.Name = "lbl41";
			this.lbl41.Size = new System.Drawing.Size(30, 15);
			this.lbl41.TabIndex = 6;
			this.lbl41.Text = "#41";
			this.lbl41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl41.MouseEnter += new System.EventHandler(this.OnEnter41);
			// 
			// lbl41_tufly
			// 
			this.lbl41_tufly.Location = new System.Drawing.Point(40, 55);
			this.lbl41_tufly.Margin = new System.Windows.Forms.Padding(0);
			this.lbl41_tufly.Name = "lbl41_tufly";
			this.lbl41_tufly.Size = new System.Drawing.Size(85, 15);
			this.lbl41_tufly.TabIndex = 7;
			this.lbl41_tufly.Text = "TuFly";
			this.lbl41_tufly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl41_tufly.MouseEnter += new System.EventHandler(this.OnEnter41);
			// 
			// lbl42
			// 
			this.lbl42.Location = new System.Drawing.Point(10, 15);
			this.lbl42.Margin = new System.Windows.Forms.Padding(0);
			this.lbl42.Name = "lbl42";
			this.lbl42.Size = new System.Drawing.Size(30, 15);
			this.lbl42.TabIndex = 0;
			this.lbl42.Text = "#42";
			this.lbl42.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl42.MouseEnter += new System.EventHandler(this.OnEnter42);
			// 
			// lbl42_armor
			// 
			this.lbl42_armor.Location = new System.Drawing.Point(40, 15);
			this.lbl42_armor.Margin = new System.Windows.Forms.Padding(0);
			this.lbl42_armor.Name = "lbl42_armor";
			this.lbl42_armor.Size = new System.Drawing.Size(85, 15);
			this.lbl42_armor.TabIndex = 1;
			this.lbl42_armor.Text = "Armor";
			this.lbl42_armor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl42_armor.MouseEnter += new System.EventHandler(this.OnEnter42);
			// 
			// lbl43
			// 
			this.lbl43.Location = new System.Drawing.Point(10, 75);
			this.lbl43.Margin = new System.Windows.Forms.Padding(0);
			this.lbl43.Name = "lbl43";
			this.lbl43.Size = new System.Drawing.Size(30, 15);
			this.lbl43.TabIndex = 9;
			this.lbl43.Text = "#43";
			this.lbl43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl43.MouseEnter += new System.EventHandler(this.OnEnter43);
			// 
			// lbl43_heblock
			// 
			this.lbl43_heblock.Location = new System.Drawing.Point(40, 75);
			this.lbl43_heblock.Margin = new System.Windows.Forms.Padding(0);
			this.lbl43_heblock.Name = "lbl43_heblock";
			this.lbl43_heblock.Size = new System.Drawing.Size(85, 15);
			this.lbl43_heblock.TabIndex = 10;
			this.lbl43_heblock.Text = "HeBlock";
			this.lbl43_heblock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl43_heblock.MouseEnter += new System.EventHandler(this.OnEnter43);
			// 
			// lbl44
			// 
			this.lbl44.Location = new System.Drawing.Point(10, 35);
			this.lbl44.Margin = new System.Windows.Forms.Padding(0);
			this.lbl44.Name = "lbl44";
			this.lbl44.Size = new System.Drawing.Size(30, 15);
			this.lbl44.TabIndex = 3;
			this.lbl44.Text = "#44";
			this.lbl44.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl44.MouseEnter += new System.EventHandler(this.OnEnter44);
			// 
			// lbl44_deathid
			// 
			this.lbl44_deathid.Location = new System.Drawing.Point(40, 35);
			this.lbl44_deathid.Margin = new System.Windows.Forms.Padding(0);
			this.lbl44_deathid.Name = "lbl44_deathid";
			this.lbl44_deathid.Size = new System.Drawing.Size(85, 15);
			this.lbl44_deathid.TabIndex = 4;
			this.lbl44_deathid.Text = "DeathId";
			this.lbl44_deathid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl44_deathid.MouseEnter += new System.EventHandler(this.OnEnter44);
			// 
			// lbl45
			// 
			this.lbl45.Location = new System.Drawing.Point(10, 55);
			this.lbl45.Margin = new System.Windows.Forms.Padding(0);
			this.lbl45.Name = "lbl45";
			this.lbl45.Size = new System.Drawing.Size(30, 15);
			this.lbl45.TabIndex = 6;
			this.lbl45.Text = "#45";
			this.lbl45.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl45.MouseEnter += new System.EventHandler(this.OnEnter45);
			// 
			// lbl45_fireresist
			// 
			this.lbl45_fireresist.Location = new System.Drawing.Point(40, 55);
			this.lbl45_fireresist.Margin = new System.Windows.Forms.Padding(0);
			this.lbl45_fireresist.Name = "lbl45_fireresist";
			this.lbl45_fireresist.Size = new System.Drawing.Size(85, 15);
			this.lbl45_fireresist.TabIndex = 7;
			this.lbl45_fireresist.Text = "FireResist";
			this.lbl45_fireresist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl45_fireresist.MouseEnter += new System.EventHandler(this.OnEnter45);
			// 
			// lbl46
			// 
			this.lbl46.Location = new System.Drawing.Point(10, 55);
			this.lbl46.Margin = new System.Windows.Forms.Padding(0);
			this.lbl46.Name = "lbl46";
			this.lbl46.Size = new System.Drawing.Size(30, 15);
			this.lbl46.TabIndex = 6;
			this.lbl46.Text = "#46";
			this.lbl46.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl46.MouseEnter += new System.EventHandler(this.OnEnter46);
			// 
			// lbl46_alternateid
			// 
			this.lbl46_alternateid.Location = new System.Drawing.Point(40, 55);
			this.lbl46_alternateid.Margin = new System.Windows.Forms.Padding(0);
			this.lbl46_alternateid.Name = "lbl46_alternateid";
			this.lbl46_alternateid.Size = new System.Drawing.Size(85, 15);
			this.lbl46_alternateid.TabIndex = 7;
			this.lbl46_alternateid.Text = "AlternateId";
			this.lbl46_alternateid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl46_alternateid.MouseEnter += new System.EventHandler(this.OnEnter46);
			// 
			// lbl47
			// 
			this.lbl47.Location = new System.Drawing.Point(10, 195);
			this.lbl47.Margin = new System.Windows.Forms.Padding(0);
			this.lbl47.Name = "lbl47";
			this.lbl47.Size = new System.Drawing.Size(30, 15);
			this.lbl47.TabIndex = 27;
			this.lbl47.Text = "#47";
			this.lbl47.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl47.MouseEnter += new System.EventHandler(this.OnEnter47);
			// 
			// lbl47_
			// 
			this.lbl47_.Location = new System.Drawing.Point(40, 195);
			this.lbl47_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl47_.Name = "lbl47_";
			this.lbl47_.Size = new System.Drawing.Size(85, 15);
			this.lbl47_.TabIndex = 28;
			this.lbl47_.Text = "CloseDoors";
			this.lbl47_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl47_.MouseEnter += new System.EventHandler(this.OnEnter47);
			// 
			// lbl48
			// 
			this.lbl48.Location = new System.Drawing.Point(10, 15);
			this.lbl48.Margin = new System.Windows.Forms.Padding(0);
			this.lbl48.Name = "lbl48";
			this.lbl48.Size = new System.Drawing.Size(30, 15);
			this.lbl48.TabIndex = 0;
			this.lbl48.Text = "#48";
			this.lbl48.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl48.MouseEnter += new System.EventHandler(this.OnEnter48);
			// 
			// lbl48_terrainoffset
			// 
			this.lbl48_terrainoffset.Location = new System.Drawing.Point(40, 15);
			this.lbl48_terrainoffset.Margin = new System.Windows.Forms.Padding(0);
			this.lbl48_terrainoffset.Name = "lbl48_terrainoffset";
			this.lbl48_terrainoffset.Size = new System.Drawing.Size(85, 15);
			this.lbl48_terrainoffset.TabIndex = 1;
			this.lbl48_terrainoffset.Text = "TerrainOffset";
			this.lbl48_terrainoffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl48_terrainoffset.MouseEnter += new System.EventHandler(this.OnEnter48);
			// 
			// lbl49
			// 
			this.lbl49.Location = new System.Drawing.Point(10, 35);
			this.lbl49.Margin = new System.Windows.Forms.Padding(0);
			this.lbl49.Name = "lbl49";
			this.lbl49.Size = new System.Drawing.Size(30, 15);
			this.lbl49.TabIndex = 3;
			this.lbl49.Text = "#49";
			this.lbl49.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl49.MouseEnter += new System.EventHandler(this.OnEnter49);
			// 
			// lbl49_spriteoffset
			// 
			this.lbl49_spriteoffset.Location = new System.Drawing.Point(40, 35);
			this.lbl49_spriteoffset.Margin = new System.Windows.Forms.Padding(0);
			this.lbl49_spriteoffset.Name = "lbl49_spriteoffset";
			this.lbl49_spriteoffset.Size = new System.Drawing.Size(85, 15);
			this.lbl49_spriteoffset.TabIndex = 4;
			this.lbl49_spriteoffset.Text = "SpriteOffset";
			this.lbl49_spriteoffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl49_spriteoffset.MouseEnter += new System.EventHandler(this.OnEnter49);
			// 
			// lbl50
			// 
			this.lbl50.Location = new System.Drawing.Point(10, 215);
			this.lbl50.Margin = new System.Windows.Forms.Padding(0);
			this.lbl50.Name = "lbl50";
			this.lbl50.Size = new System.Drawing.Size(30, 15);
			this.lbl50.TabIndex = 30;
			this.lbl50.Text = "#50";
			this.lbl50.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl50.MouseEnter += new System.EventHandler(this.OnEnter50);
			// 
			// lbl50_
			// 
			this.lbl50_.Location = new System.Drawing.Point(40, 215);
			this.lbl50_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl50_.Name = "lbl50_";
			this.lbl50_.Size = new System.Drawing.Size(85, 15);
			this.lbl50_.TabIndex = 31;
			this.lbl50_.Text = "dTypeMod";
			this.lbl50_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl50_.MouseEnter += new System.EventHandler(this.OnEnter50);
			// 
			// lbl51
			// 
			this.lbl51.Location = new System.Drawing.Point(10, 115);
			this.lbl51.Margin = new System.Windows.Forms.Padding(0);
			this.lbl51.Name = "lbl51";
			this.lbl51.Size = new System.Drawing.Size(30, 15);
			this.lbl51.TabIndex = 15;
			this.lbl51.Text = "#51";
			this.lbl51.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl51.MouseEnter += new System.EventHandler(this.OnEnter51);
			// 
			// lbl51_lightblock
			// 
			this.lbl51_lightblock.Location = new System.Drawing.Point(40, 115);
			this.lbl51_lightblock.Margin = new System.Windows.Forms.Padding(0);
			this.lbl51_lightblock.Name = "lbl51_lightblock";
			this.lbl51_lightblock.Size = new System.Drawing.Size(85, 15);
			this.lbl51_lightblock.TabIndex = 16;
			this.lbl51_lightblock.Text = "LightBlock";
			this.lbl51_lightblock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl51_lightblock.MouseEnter += new System.EventHandler(this.OnEnter51);
			// 
			// lbl52
			// 
			this.lbl52.Location = new System.Drawing.Point(10, 15);
			this.lbl52.Margin = new System.Windows.Forms.Padding(0);
			this.lbl52.Name = "lbl52";
			this.lbl52.Size = new System.Drawing.Size(30, 15);
			this.lbl52.TabIndex = 0;
			this.lbl52.Text = "#52";
			this.lbl52.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl52.MouseEnter += new System.EventHandler(this.OnEnter52);
			// 
			// lbl52_footsound
			// 
			this.lbl52_footsound.Location = new System.Drawing.Point(40, 15);
			this.lbl52_footsound.Margin = new System.Windows.Forms.Padding(0);
			this.lbl52_footsound.Name = "lbl52_footsound";
			this.lbl52_footsound.Size = new System.Drawing.Size(85, 15);
			this.lbl52_footsound.TabIndex = 1;
			this.lbl52_footsound.Text = "FootSound";
			this.lbl52_footsound.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl52_footsound.MouseEnter += new System.EventHandler(this.OnEnter52);
			// 
			// lbl53
			// 
			this.lbl53.Location = new System.Drawing.Point(10, 15);
			this.lbl53.Margin = new System.Windows.Forms.Padding(0);
			this.lbl53.Name = "lbl53";
			this.lbl53.Size = new System.Drawing.Size(30, 15);
			this.lbl53.TabIndex = 0;
			this.lbl53.Text = "#53";
			this.lbl53.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl53.MouseEnter += new System.EventHandler(this.OnEnter53);
			// 
			// lbl53_parttype
			// 
			this.lbl53_parttype.Location = new System.Drawing.Point(40, 15);
			this.lbl53_parttype.Margin = new System.Windows.Forms.Padding(0);
			this.lbl53_parttype.Name = "lbl53_parttype";
			this.lbl53_parttype.Size = new System.Drawing.Size(85, 15);
			this.lbl53_parttype.TabIndex = 1;
			this.lbl53_parttype.Text = "PartType";
			this.lbl53_parttype.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl53_parttype.MouseEnter += new System.EventHandler(this.OnEnter53);
			// 
			// lbl54
			// 
			this.lbl54.Location = new System.Drawing.Point(10, 15);
			this.lbl54.Margin = new System.Windows.Forms.Padding(0);
			this.lbl54.Name = "lbl54";
			this.lbl54.Size = new System.Drawing.Size(30, 15);
			this.lbl54.TabIndex = 0;
			this.lbl54.Text = "#54";
			this.lbl54.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl54.MouseEnter += new System.EventHandler(this.OnEnter54);
			// 
			// lbl54_hetype
			// 
			this.lbl54_hetype.Location = new System.Drawing.Point(40, 15);
			this.lbl54_hetype.Margin = new System.Windows.Forms.Padding(0);
			this.lbl54_hetype.Name = "lbl54_hetype";
			this.lbl54_hetype.Size = new System.Drawing.Size(85, 15);
			this.lbl54_hetype.TabIndex = 1;
			this.lbl54_hetype.Text = "HeType";
			this.lbl54_hetype.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl54_hetype.MouseEnter += new System.EventHandler(this.OnEnter54);
			// 
			// lbl55
			// 
			this.lbl55.Location = new System.Drawing.Point(10, 35);
			this.lbl55.Margin = new System.Windows.Forms.Padding(0);
			this.lbl55.Name = "lbl55";
			this.lbl55.Size = new System.Drawing.Size(30, 15);
			this.lbl55.TabIndex = 3;
			this.lbl55.Text = "#55";
			this.lbl55.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl55.MouseEnter += new System.EventHandler(this.OnEnter55);
			// 
			// lbl55_hestrength
			// 
			this.lbl55_hestrength.Location = new System.Drawing.Point(40, 35);
			this.lbl55_hestrength.Margin = new System.Windows.Forms.Padding(0);
			this.lbl55_hestrength.Name = "lbl55_hestrength";
			this.lbl55_hestrength.Size = new System.Drawing.Size(85, 15);
			this.lbl55_hestrength.TabIndex = 4;
			this.lbl55_hestrength.Text = "HeStrength";
			this.lbl55_hestrength.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl55_hestrength.MouseEnter += new System.EventHandler(this.OnEnter55);
			// 
			// lbl56
			// 
			this.lbl56.Location = new System.Drawing.Point(10, 95);
			this.lbl56.Margin = new System.Windows.Forms.Padding(0);
			this.lbl56.Name = "lbl56";
			this.lbl56.Size = new System.Drawing.Size(30, 15);
			this.lbl56.TabIndex = 12;
			this.lbl56.Text = "#56";
			this.lbl56.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl56.MouseEnter += new System.EventHandler(this.OnEnter56);
			// 
			// lbl56_smokeblock
			// 
			this.lbl56_smokeblock.Location = new System.Drawing.Point(40, 95);
			this.lbl56_smokeblock.Margin = new System.Windows.Forms.Padding(0);
			this.lbl56_smokeblock.Name = "lbl56_smokeblock";
			this.lbl56_smokeblock.Size = new System.Drawing.Size(85, 15);
			this.lbl56_smokeblock.TabIndex = 13;
			this.lbl56_smokeblock.Text = "SmokeBlock";
			this.lbl56_smokeblock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl56_smokeblock.MouseEnter += new System.EventHandler(this.OnEnter56);
			// 
			// lbl57
			// 
			this.lbl57.Location = new System.Drawing.Point(10, 75);
			this.lbl57.Margin = new System.Windows.Forms.Padding(0);
			this.lbl57.Name = "lbl57";
			this.lbl57.Size = new System.Drawing.Size(30, 15);
			this.lbl57.TabIndex = 9;
			this.lbl57.Text = "#57";
			this.lbl57.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl57.MouseEnter += new System.EventHandler(this.OnEnter57);
			// 
			// lbl57_fuel
			// 
			this.lbl57_fuel.Location = new System.Drawing.Point(40, 75);
			this.lbl57_fuel.Margin = new System.Windows.Forms.Padding(0);
			this.lbl57_fuel.Name = "lbl57_fuel";
			this.lbl57_fuel.Size = new System.Drawing.Size(85, 15);
			this.lbl57_fuel.TabIndex = 10;
			this.lbl57_fuel.Text = "Fuel";
			this.lbl57_fuel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl57_fuel.MouseEnter += new System.EventHandler(this.OnEnter57);
			// 
			// lbl58
			// 
			this.lbl58.Location = new System.Drawing.Point(10, 115);
			this.lbl58.Margin = new System.Windows.Forms.Padding(0);
			this.lbl58.Name = "lbl58";
			this.lbl58.Size = new System.Drawing.Size(30, 15);
			this.lbl58.TabIndex = 15;
			this.lbl58.Text = "#58";
			this.lbl58.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl58.MouseEnter += new System.EventHandler(this.OnEnter58);
			// 
			// lbl58_lightintensity
			// 
			this.lbl58_lightintensity.Location = new System.Drawing.Point(40, 115);
			this.lbl58_lightintensity.Margin = new System.Windows.Forms.Padding(0);
			this.lbl58_lightintensity.Name = "lbl58_lightintensity";
			this.lbl58_lightintensity.Size = new System.Drawing.Size(85, 15);
			this.lbl58_lightintensity.TabIndex = 16;
			this.lbl58_lightintensity.Text = "LightIntensity";
			this.lbl58_lightintensity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl58_lightintensity.MouseEnter += new System.EventHandler(this.OnEnter58);
			// 
			// lbl59
			// 
			this.lbl59.Location = new System.Drawing.Point(10, 35);
			this.lbl59.Margin = new System.Windows.Forms.Padding(0);
			this.lbl59.Name = "lbl59";
			this.lbl59.Size = new System.Drawing.Size(30, 15);
			this.lbl59.TabIndex = 3;
			this.lbl59.Text = "#59";
			this.lbl59.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl59.MouseEnter += new System.EventHandler(this.OnEnter59);
			// 
			// lbl59_specialtype
			// 
			this.lbl59_specialtype.Location = new System.Drawing.Point(40, 35);
			this.lbl59_specialtype.Margin = new System.Windows.Forms.Padding(0);
			this.lbl59_specialtype.Name = "lbl59_specialtype";
			this.lbl59_specialtype.Size = new System.Drawing.Size(85, 15);
			this.lbl59_specialtype.TabIndex = 4;
			this.lbl59_specialtype.Text = "SpecialType";
			this.lbl59_specialtype.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl59_specialtype.MouseEnter += new System.EventHandler(this.OnEnter59);
			// 
			// lbl60
			// 
			this.lbl60.Location = new System.Drawing.Point(10, 95);
			this.lbl60.Margin = new System.Windows.Forms.Padding(0);
			this.lbl60.Name = "lbl60";
			this.lbl60.Size = new System.Drawing.Size(30, 15);
			this.lbl60.TabIndex = 12;
			this.lbl60.Text = "#60";
			this.lbl60.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl60.MouseEnter += new System.EventHandler(this.OnEnter60);
			// 
			// lbl60_isbaseobject
			// 
			this.lbl60_isbaseobject.Location = new System.Drawing.Point(40, 95);
			this.lbl60_isbaseobject.Margin = new System.Windows.Forms.Padding(0);
			this.lbl60_isbaseobject.Name = "lbl60_isbaseobject";
			this.lbl60_isbaseobject.Size = new System.Drawing.Size(85, 15);
			this.lbl60_isbaseobject.TabIndex = 13;
			this.lbl60_isbaseobject.Text = "isBaseObject";
			this.lbl60_isbaseobject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl60_isbaseobject.MouseEnter += new System.EventHandler(this.OnEnter60);
			// 
			// lbl61
			// 
			this.lbl61.Location = new System.Drawing.Point(10, 235);
			this.lbl61.Margin = new System.Windows.Forms.Padding(0);
			this.lbl61.Name = "lbl61";
			this.lbl61.Size = new System.Drawing.Size(30, 15);
			this.lbl61.TabIndex = 33;
			this.lbl61.Text = "#61";
			this.lbl61.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl61.MouseEnter += new System.EventHandler(this.OnEnter61);
			// 
			// lbl61_
			// 
			this.lbl61_.Location = new System.Drawing.Point(40, 235);
			this.lbl61_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl61_.Name = "lbl61_";
			this.lbl61_.Size = new System.Drawing.Size(85, 15);
			this.lbl61_.TabIndex = 34;
			this.lbl61_.Text = "VictoryPoints";
			this.lbl61_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl61_.MouseEnter += new System.EventHandler(this.OnEnter61);
			// 
			// lbl19
			// 
			this.lbl19.Location = new System.Drawing.Point(5, 25);
			this.lbl19.Margin = new System.Windows.Forms.Padding(0);
			this.lbl19.Name = "lbl19";
			this.lbl19.Size = new System.Drawing.Size(30, 15);
			this.lbl19.TabIndex = 0;
			this.lbl19.Text = "#19";
			this.lbl19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl19.MouseEnter += new System.EventHandler(this.OnEnter19);
			// 
			// lbl19_loft11
			// 
			this.lbl19_loft11.Location = new System.Drawing.Point(35, 25);
			this.lbl19_loft11.Margin = new System.Windows.Forms.Padding(0);
			this.lbl19_loft11.Name = "lbl19_loft11";
			this.lbl19_loft11.Size = new System.Drawing.Size(45, 15);
			this.lbl19_loft11.TabIndex = 1;
			this.lbl19_loft11.Text = "loft 12";
			this.lbl19_loft11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl19_loft11.MouseEnter += new System.EventHandler(this.OnEnter19);
			// 
			// lbl18
			// 
			this.lbl18.Location = new System.Drawing.Point(5, 60);
			this.lbl18.Margin = new System.Windows.Forms.Padding(0);
			this.lbl18.Name = "lbl18";
			this.lbl18.Size = new System.Drawing.Size(30, 15);
			this.lbl18.TabIndex = 4;
			this.lbl18.Text = "#18";
			this.lbl18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl18.MouseEnter += new System.EventHandler(this.OnEnter18);
			// 
			// lbl18_loft10
			// 
			this.lbl18_loft10.Location = new System.Drawing.Point(35, 60);
			this.lbl18_loft10.Margin = new System.Windows.Forms.Padding(0);
			this.lbl18_loft10.Name = "lbl18_loft10";
			this.lbl18_loft10.Size = new System.Drawing.Size(45, 15);
			this.lbl18_loft10.TabIndex = 5;
			this.lbl18_loft10.Text = "loft 11";
			this.lbl18_loft10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl18_loft10.MouseEnter += new System.EventHandler(this.OnEnter18);
			// 
			// lbl17
			// 
			this.lbl17.Location = new System.Drawing.Point(5, 95);
			this.lbl17.Margin = new System.Windows.Forms.Padding(0);
			this.lbl17.Name = "lbl17";
			this.lbl17.Size = new System.Drawing.Size(30, 15);
			this.lbl17.TabIndex = 8;
			this.lbl17.Text = "#17";
			this.lbl17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl17.MouseEnter += new System.EventHandler(this.OnEnter17);
			// 
			// lbl17_loft09
			// 
			this.lbl17_loft09.Location = new System.Drawing.Point(35, 95);
			this.lbl17_loft09.Margin = new System.Windows.Forms.Padding(0);
			this.lbl17_loft09.Name = "lbl17_loft09";
			this.lbl17_loft09.Size = new System.Drawing.Size(45, 15);
			this.lbl17_loft09.TabIndex = 9;
			this.lbl17_loft09.Text = "loft 10";
			this.lbl17_loft09.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl17_loft09.MouseEnter += new System.EventHandler(this.OnEnter17);
			// 
			// lbl16
			// 
			this.lbl16.Location = new System.Drawing.Point(5, 130);
			this.lbl16.Margin = new System.Windows.Forms.Padding(0);
			this.lbl16.Name = "lbl16";
			this.lbl16.Size = new System.Drawing.Size(30, 15);
			this.lbl16.TabIndex = 12;
			this.lbl16.Text = "#16";
			this.lbl16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl16.MouseEnter += new System.EventHandler(this.OnEnter16);
			// 
			// lbl16_loft08
			// 
			this.lbl16_loft08.Location = new System.Drawing.Point(35, 130);
			this.lbl16_loft08.Margin = new System.Windows.Forms.Padding(0);
			this.lbl16_loft08.Name = "lbl16_loft08";
			this.lbl16_loft08.Size = new System.Drawing.Size(45, 15);
			this.lbl16_loft08.TabIndex = 13;
			this.lbl16_loft08.Text = "loft 9";
			this.lbl16_loft08.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl16_loft08.MouseEnter += new System.EventHandler(this.OnEnter16);
			// 
			// lbl15
			// 
			this.lbl15.Location = new System.Drawing.Point(5, 165);
			this.lbl15.Margin = new System.Windows.Forms.Padding(0);
			this.lbl15.Name = "lbl15";
			this.lbl15.Size = new System.Drawing.Size(30, 15);
			this.lbl15.TabIndex = 16;
			this.lbl15.Text = "#15";
			this.lbl15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl15.MouseEnter += new System.EventHandler(this.OnEnter15);
			// 
			// lbl15_loft07
			// 
			this.lbl15_loft07.Location = new System.Drawing.Point(35, 165);
			this.lbl15_loft07.Margin = new System.Windows.Forms.Padding(0);
			this.lbl15_loft07.Name = "lbl15_loft07";
			this.lbl15_loft07.Size = new System.Drawing.Size(45, 15);
			this.lbl15_loft07.TabIndex = 17;
			this.lbl15_loft07.Text = "loft 8";
			this.lbl15_loft07.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl15_loft07.MouseEnter += new System.EventHandler(this.OnEnter15);
			// 
			// lbl14
			// 
			this.lbl14.Location = new System.Drawing.Point(5, 200);
			this.lbl14.Margin = new System.Windows.Forms.Padding(0);
			this.lbl14.Name = "lbl14";
			this.lbl14.Size = new System.Drawing.Size(30, 15);
			this.lbl14.TabIndex = 20;
			this.lbl14.Text = "#14";
			this.lbl14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl14.MouseEnter += new System.EventHandler(this.OnEnter14);
			// 
			// lbl14_loft06
			// 
			this.lbl14_loft06.Location = new System.Drawing.Point(35, 200);
			this.lbl14_loft06.Margin = new System.Windows.Forms.Padding(0);
			this.lbl14_loft06.Name = "lbl14_loft06";
			this.lbl14_loft06.Size = new System.Drawing.Size(45, 15);
			this.lbl14_loft06.TabIndex = 21;
			this.lbl14_loft06.Text = "loft 7";
			this.lbl14_loft06.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl14_loft06.MouseEnter += new System.EventHandler(this.OnEnter14);
			// 
			// lbl13
			// 
			this.lbl13.Location = new System.Drawing.Point(5, 235);
			this.lbl13.Margin = new System.Windows.Forms.Padding(0);
			this.lbl13.Name = "lbl13";
			this.lbl13.Size = new System.Drawing.Size(30, 15);
			this.lbl13.TabIndex = 24;
			this.lbl13.Text = "#13";
			this.lbl13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl13.MouseEnter += new System.EventHandler(this.OnEnter13);
			// 
			// lbl13_loft05
			// 
			this.lbl13_loft05.Location = new System.Drawing.Point(35, 235);
			this.lbl13_loft05.Margin = new System.Windows.Forms.Padding(0);
			this.lbl13_loft05.Name = "lbl13_loft05";
			this.lbl13_loft05.Size = new System.Drawing.Size(45, 15);
			this.lbl13_loft05.TabIndex = 25;
			this.lbl13_loft05.Text = "loft 6";
			this.lbl13_loft05.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl13_loft05.MouseEnter += new System.EventHandler(this.OnEnter13);
			// 
			// lbl12
			// 
			this.lbl12.Location = new System.Drawing.Point(5, 270);
			this.lbl12.Margin = new System.Windows.Forms.Padding(0);
			this.lbl12.Name = "lbl12";
			this.lbl12.Size = new System.Drawing.Size(30, 15);
			this.lbl12.TabIndex = 28;
			this.lbl12.Text = "#12";
			this.lbl12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl12.MouseEnter += new System.EventHandler(this.OnEnter12);
			// 
			// lbl12_loft04
			// 
			this.lbl12_loft04.Location = new System.Drawing.Point(35, 270);
			this.lbl12_loft04.Margin = new System.Windows.Forms.Padding(0);
			this.lbl12_loft04.Name = "lbl12_loft04";
			this.lbl12_loft04.Size = new System.Drawing.Size(45, 15);
			this.lbl12_loft04.TabIndex = 29;
			this.lbl12_loft04.Text = "loft 5";
			this.lbl12_loft04.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl12_loft04.MouseEnter += new System.EventHandler(this.OnEnter12);
			// 
			// lbl11
			// 
			this.lbl11.Location = new System.Drawing.Point(5, 305);
			this.lbl11.Margin = new System.Windows.Forms.Padding(0);
			this.lbl11.Name = "lbl11";
			this.lbl11.Size = new System.Drawing.Size(30, 15);
			this.lbl11.TabIndex = 32;
			this.lbl11.Text = "#11";
			this.lbl11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl11.MouseEnter += new System.EventHandler(this.OnEnter11);
			// 
			// lbl11_loft03
			// 
			this.lbl11_loft03.Location = new System.Drawing.Point(35, 305);
			this.lbl11_loft03.Margin = new System.Windows.Forms.Padding(0);
			this.lbl11_loft03.Name = "lbl11_loft03";
			this.lbl11_loft03.Size = new System.Drawing.Size(45, 15);
			this.lbl11_loft03.TabIndex = 33;
			this.lbl11_loft03.Text = "loft 4";
			this.lbl11_loft03.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl11_loft03.MouseEnter += new System.EventHandler(this.OnEnter11);
			// 
			// lbl10
			// 
			this.lbl10.Location = new System.Drawing.Point(5, 340);
			this.lbl10.Margin = new System.Windows.Forms.Padding(0);
			this.lbl10.Name = "lbl10";
			this.lbl10.Size = new System.Drawing.Size(30, 15);
			this.lbl10.TabIndex = 36;
			this.lbl10.Text = "#10";
			this.lbl10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl10.MouseEnter += new System.EventHandler(this.OnEnter10);
			// 
			// lbl10_loft02
			// 
			this.lbl10_loft02.Location = new System.Drawing.Point(35, 340);
			this.lbl10_loft02.Margin = new System.Windows.Forms.Padding(0);
			this.lbl10_loft02.Name = "lbl10_loft02";
			this.lbl10_loft02.Size = new System.Drawing.Size(45, 15);
			this.lbl10_loft02.TabIndex = 37;
			this.lbl10_loft02.Text = "loft 3";
			this.lbl10_loft02.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl10_loft02.MouseEnter += new System.EventHandler(this.OnEnter10);
			// 
			// lbl09
			// 
			this.lbl09.Location = new System.Drawing.Point(5, 375);
			this.lbl09.Margin = new System.Windows.Forms.Padding(0);
			this.lbl09.Name = "lbl09";
			this.lbl09.Size = new System.Drawing.Size(30, 15);
			this.lbl09.TabIndex = 40;
			this.lbl09.Text = "#9";
			this.lbl09.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl09.MouseEnter += new System.EventHandler(this.OnEnter9);
			// 
			// lbl09_loft01
			// 
			this.lbl09_loft01.Location = new System.Drawing.Point(35, 375);
			this.lbl09_loft01.Margin = new System.Windows.Forms.Padding(0);
			this.lbl09_loft01.Name = "lbl09_loft01";
			this.lbl09_loft01.Size = new System.Drawing.Size(45, 15);
			this.lbl09_loft01.TabIndex = 41;
			this.lbl09_loft01.Text = "loft 2";
			this.lbl09_loft01.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl09_loft01.MouseEnter += new System.EventHandler(this.OnEnter9);
			// 
			// lbl08
			// 
			this.lbl08.Location = new System.Drawing.Point(5, 410);
			this.lbl08.Margin = new System.Windows.Forms.Padding(0);
			this.lbl08.Name = "lbl08";
			this.lbl08.Size = new System.Drawing.Size(30, 15);
			this.lbl08.TabIndex = 44;
			this.lbl08.Text = "#8";
			this.lbl08.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl08.MouseEnter += new System.EventHandler(this.OnEnter8);
			// 
			// lbl08_loft00
			// 
			this.lbl08_loft00.Location = new System.Drawing.Point(35, 410);
			this.lbl08_loft00.Margin = new System.Windows.Forms.Padding(0);
			this.lbl08_loft00.Name = "lbl08_loft00";
			this.lbl08_loft00.Size = new System.Drawing.Size(45, 15);
			this.lbl08_loft00.TabIndex = 45;
			this.lbl08_loft00.Text = "loft 1";
			this.lbl08_loft00.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl08_loft00.MouseEnter += new System.EventHandler(this.OnEnter8);
			// 
			// lbl00
			// 
			this.lbl00.Location = new System.Drawing.Point(10, 120);
			this.lbl00.Margin = new System.Windows.Forms.Padding(0);
			this.lbl00.Name = "lbl00";
			this.lbl00.Size = new System.Drawing.Size(20, 15);
			this.lbl00.TabIndex = 1;
			this.lbl00.Text = "#0";
			this.lbl00.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl00.MouseEnter += new System.EventHandler(this.OnEnter0);
			// 
			// lbl00_phase0
			// 
			this.lbl00_phase0.Location = new System.Drawing.Point(30, 120);
			this.lbl00_phase0.Margin = new System.Windows.Forms.Padding(0);
			this.lbl00_phase0.Name = "lbl00_phase0";
			this.lbl00_phase0.Size = new System.Drawing.Size(50, 15);
			this.lbl00_phase0.TabIndex = 2;
			this.lbl00_phase0.Text = "phase 1";
			this.lbl00_phase0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl00_phase0.MouseEnter += new System.EventHandler(this.OnEnter0);
			// 
			// lbl01
			// 
			this.lbl01.Location = new System.Drawing.Point(90, 120);
			this.lbl01.Margin = new System.Windows.Forms.Padding(0);
			this.lbl01.Name = "lbl01";
			this.lbl01.Size = new System.Drawing.Size(20, 15);
			this.lbl01.TabIndex = 4;
			this.lbl01.Text = "#1";
			this.lbl01.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl01.MouseEnter += new System.EventHandler(this.OnEnter1);
			// 
			// lbl01_phase1
			// 
			this.lbl01_phase1.Location = new System.Drawing.Point(110, 120);
			this.lbl01_phase1.Margin = new System.Windows.Forms.Padding(0);
			this.lbl01_phase1.Name = "lbl01_phase1";
			this.lbl01_phase1.Size = new System.Drawing.Size(50, 15);
			this.lbl01_phase1.TabIndex = 5;
			this.lbl01_phase1.Text = "phase 2";
			this.lbl01_phase1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl01_phase1.MouseEnter += new System.EventHandler(this.OnEnter1);
			// 
			// lbl02
			// 
			this.lbl02.Location = new System.Drawing.Point(170, 120);
			this.lbl02.Margin = new System.Windows.Forms.Padding(0);
			this.lbl02.Name = "lbl02";
			this.lbl02.Size = new System.Drawing.Size(20, 15);
			this.lbl02.TabIndex = 7;
			this.lbl02.Text = "#2";
			this.lbl02.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl02.MouseEnter += new System.EventHandler(this.OnEnter2);
			// 
			// lbl02_phase2
			// 
			this.lbl02_phase2.Location = new System.Drawing.Point(190, 120);
			this.lbl02_phase2.Margin = new System.Windows.Forms.Padding(0);
			this.lbl02_phase2.Name = "lbl02_phase2";
			this.lbl02_phase2.Size = new System.Drawing.Size(50, 15);
			this.lbl02_phase2.TabIndex = 8;
			this.lbl02_phase2.Text = "phase 3";
			this.lbl02_phase2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl02_phase2.MouseEnter += new System.EventHandler(this.OnEnter2);
			// 
			// lbl03
			// 
			this.lbl03.Location = new System.Drawing.Point(245, 120);
			this.lbl03.Margin = new System.Windows.Forms.Padding(0);
			this.lbl03.Name = "lbl03";
			this.lbl03.Size = new System.Drawing.Size(20, 15);
			this.lbl03.TabIndex = 10;
			this.lbl03.Text = "#3";
			this.lbl03.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl03.MouseEnter += new System.EventHandler(this.OnEnter3);
			// 
			// lbl03_phase3
			// 
			this.lbl03_phase3.Location = new System.Drawing.Point(265, 120);
			this.lbl03_phase3.Margin = new System.Windows.Forms.Padding(0);
			this.lbl03_phase3.Name = "lbl03_phase3";
			this.lbl03_phase3.Size = new System.Drawing.Size(50, 15);
			this.lbl03_phase3.TabIndex = 11;
			this.lbl03_phase3.Text = "phase 4";
			this.lbl03_phase3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl03_phase3.MouseEnter += new System.EventHandler(this.OnEnter3);
			// 
			// lbl07
			// 
			this.lbl07.Location = new System.Drawing.Point(560, 120);
			this.lbl07.Margin = new System.Windows.Forms.Padding(0);
			this.lbl07.Name = "lbl07";
			this.lbl07.Size = new System.Drawing.Size(20, 15);
			this.lbl07.TabIndex = 22;
			this.lbl07.Text = "#7";
			this.lbl07.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl07.MouseEnter += new System.EventHandler(this.OnEnter7);
			// 
			// lbl07_phase7
			// 
			this.lbl07_phase7.Location = new System.Drawing.Point(580, 120);
			this.lbl07_phase7.Margin = new System.Windows.Forms.Padding(0);
			this.lbl07_phase7.Name = "lbl07_phase7";
			this.lbl07_phase7.Size = new System.Drawing.Size(50, 15);
			this.lbl07_phase7.TabIndex = 23;
			this.lbl07_phase7.Text = "phase 8";
			this.lbl07_phase7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl07_phase7.MouseEnter += new System.EventHandler(this.OnEnter7);
			// 
			// lbl06
			// 
			this.lbl06.Location = new System.Drawing.Point(480, 120);
			this.lbl06.Margin = new System.Windows.Forms.Padding(0);
			this.lbl06.Name = "lbl06";
			this.lbl06.Size = new System.Drawing.Size(20, 15);
			this.lbl06.TabIndex = 19;
			this.lbl06.Text = "#6";
			this.lbl06.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl06.MouseEnter += new System.EventHandler(this.OnEnter6);
			// 
			// lbl06_phase6
			// 
			this.lbl06_phase6.Location = new System.Drawing.Point(500, 120);
			this.lbl06_phase6.Margin = new System.Windows.Forms.Padding(0);
			this.lbl06_phase6.Name = "lbl06_phase6";
			this.lbl06_phase6.Size = new System.Drawing.Size(50, 15);
			this.lbl06_phase6.TabIndex = 20;
			this.lbl06_phase6.Text = "phase 7";
			this.lbl06_phase6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl06_phase6.MouseEnter += new System.EventHandler(this.OnEnter6);
			// 
			// lbl05
			// 
			this.lbl05.Location = new System.Drawing.Point(400, 120);
			this.lbl05.Margin = new System.Windows.Forms.Padding(0);
			this.lbl05.Name = "lbl05";
			this.lbl05.Size = new System.Drawing.Size(20, 15);
			this.lbl05.TabIndex = 16;
			this.lbl05.Text = "#5";
			this.lbl05.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl05.MouseEnter += new System.EventHandler(this.OnEnter5);
			// 
			// lbl05_phase5
			// 
			this.lbl05_phase5.Location = new System.Drawing.Point(420, 120);
			this.lbl05_phase5.Margin = new System.Windows.Forms.Padding(0);
			this.lbl05_phase5.Name = "lbl05_phase5";
			this.lbl05_phase5.Size = new System.Drawing.Size(50, 15);
			this.lbl05_phase5.TabIndex = 17;
			this.lbl05_phase5.Text = "phase 6";
			this.lbl05_phase5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl05_phase5.MouseEnter += new System.EventHandler(this.OnEnter5);
			// 
			// lbl04
			// 
			this.lbl04.Location = new System.Drawing.Point(320, 120);
			this.lbl04.Margin = new System.Windows.Forms.Padding(0);
			this.lbl04.Name = "lbl04";
			this.lbl04.Size = new System.Drawing.Size(20, 15);
			this.lbl04.TabIndex = 13;
			this.lbl04.Text = "#4";
			this.lbl04.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl04.MouseEnter += new System.EventHandler(this.OnEnter4);
			// 
			// lbl04_phase4
			// 
			this.lbl04_phase4.Location = new System.Drawing.Point(340, 120);
			this.lbl04_phase4.Margin = new System.Windows.Forms.Padding(0);
			this.lbl04_phase4.Name = "lbl04_phase4";
			this.lbl04_phase4.Size = new System.Drawing.Size(50, 15);
			this.lbl04_phase4.TabIndex = 14;
			this.lbl04_phase4.Text = "phase 5";
			this.lbl04_phase4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl04_phase4.MouseEnter += new System.EventHandler(this.OnEnter4);
			// 
			// lbl22
			// 
			this.lbl22.Location = new System.Drawing.Point(10, 15);
			this.lbl22.Margin = new System.Windows.Forms.Padding(0);
			this.lbl22.Name = "lbl22";
			this.lbl22.Size = new System.Drawing.Size(30, 15);
			this.lbl22.TabIndex = 0;
			this.lbl22.Text = "#22";
			this.lbl22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl22.MouseEnter += new System.EventHandler(this.OnEnter22);
			// 
			// lbl22_
			// 
			this.lbl22_.Location = new System.Drawing.Point(40, 15);
			this.lbl22_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl22_.Name = "lbl22_";
			this.lbl22_.Size = new System.Drawing.Size(85, 15);
			this.lbl22_.TabIndex = 1;
			this.lbl22_.Text = "tab ram";
			this.lbl22_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl22_.MouseEnter += new System.EventHandler(this.OnEnter22);
			// 
			// lbl23
			// 
			this.lbl23.Location = new System.Drawing.Point(10, 35);
			this.lbl23.Margin = new System.Windows.Forms.Padding(0);
			this.lbl23.Name = "lbl23";
			this.lbl23.Size = new System.Drawing.Size(30, 15);
			this.lbl23.TabIndex = 3;
			this.lbl23.Text = "#23";
			this.lbl23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl23.MouseEnter += new System.EventHandler(this.OnEnter23);
			// 
			// lbl23_
			// 
			this.lbl23_.Location = new System.Drawing.Point(40, 35);
			this.lbl23_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl23_.Name = "lbl23_";
			this.lbl23_.Size = new System.Drawing.Size(85, 15);
			this.lbl23_.TabIndex = 4;
			this.lbl23_.Text = "tab ram";
			this.lbl23_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl23_.MouseEnter += new System.EventHandler(this.OnEnter23);
			// 
			// lbl24
			// 
			this.lbl24.Location = new System.Drawing.Point(10, 55);
			this.lbl24.Margin = new System.Windows.Forms.Padding(0);
			this.lbl24.Name = "lbl24";
			this.lbl24.Size = new System.Drawing.Size(30, 15);
			this.lbl24.TabIndex = 6;
			this.lbl24.Text = "#24";
			this.lbl24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl24.MouseEnter += new System.EventHandler(this.OnEnter24);
			// 
			// lbl24_
			// 
			this.lbl24_.Location = new System.Drawing.Point(40, 55);
			this.lbl24_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl24_.Name = "lbl24_";
			this.lbl24_.Size = new System.Drawing.Size(85, 15);
			this.lbl24_.TabIndex = 7;
			this.lbl24_.Text = "tab ram";
			this.lbl24_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl24_.MouseEnter += new System.EventHandler(this.OnEnter24);
			// 
			// lbl25
			// 
			this.lbl25.Location = new System.Drawing.Point(10, 75);
			this.lbl25.Margin = new System.Windows.Forms.Padding(0);
			this.lbl25.Name = "lbl25";
			this.lbl25.Size = new System.Drawing.Size(30, 15);
			this.lbl25.TabIndex = 9;
			this.lbl25.Text = "#25";
			this.lbl25.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl25.MouseEnter += new System.EventHandler(this.OnEnter25);
			// 
			// lbl25_
			// 
			this.lbl25_.Location = new System.Drawing.Point(40, 75);
			this.lbl25_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl25_.Name = "lbl25_";
			this.lbl25_.Size = new System.Drawing.Size(85, 15);
			this.lbl25_.TabIndex = 10;
			this.lbl25_.Text = "tab ram";
			this.lbl25_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl25_.MouseEnter += new System.EventHandler(this.OnEnter25);
			// 
			// lbl26
			// 
			this.lbl26.Location = new System.Drawing.Point(10, 95);
			this.lbl26.Margin = new System.Windows.Forms.Padding(0);
			this.lbl26.Name = "lbl26";
			this.lbl26.Size = new System.Drawing.Size(30, 15);
			this.lbl26.TabIndex = 12;
			this.lbl26.Text = "#26";
			this.lbl26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl26.MouseEnter += new System.EventHandler(this.OnEnter26);
			// 
			// lbl26_
			// 
			this.lbl26_.Location = new System.Drawing.Point(40, 95);
			this.lbl26_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl26_.Name = "lbl26_";
			this.lbl26_.Size = new System.Drawing.Size(85, 15);
			this.lbl26_.TabIndex = 13;
			this.lbl26_.Text = "pck ram";
			this.lbl26_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl26_.MouseEnter += new System.EventHandler(this.OnEnter26);
			// 
			// lbl27
			// 
			this.lbl27.Location = new System.Drawing.Point(10, 115);
			this.lbl27.Margin = new System.Windows.Forms.Padding(0);
			this.lbl27.Name = "lbl27";
			this.lbl27.Size = new System.Drawing.Size(30, 15);
			this.lbl27.TabIndex = 15;
			this.lbl27.Text = "#27";
			this.lbl27.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl27.MouseEnter += new System.EventHandler(this.OnEnter27);
			// 
			// lbl27_
			// 
			this.lbl27_.Location = new System.Drawing.Point(40, 115);
			this.lbl27_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl27_.Name = "lbl27_";
			this.lbl27_.Size = new System.Drawing.Size(85, 15);
			this.lbl27_.TabIndex = 16;
			this.lbl27_.Text = "pck ram";
			this.lbl27_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl27_.MouseEnter += new System.EventHandler(this.OnEnter27);
			// 
			// lbl28
			// 
			this.lbl28.Location = new System.Drawing.Point(10, 135);
			this.lbl28.Margin = new System.Windows.Forms.Padding(0);
			this.lbl28.Name = "lbl28";
			this.lbl28.Size = new System.Drawing.Size(30, 15);
			this.lbl28.TabIndex = 18;
			this.lbl28.Text = "#28";
			this.lbl28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl28.MouseEnter += new System.EventHandler(this.OnEnter28);
			// 
			// lbl28_
			// 
			this.lbl28_.Location = new System.Drawing.Point(40, 135);
			this.lbl28_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl28_.Name = "lbl28_";
			this.lbl28_.Size = new System.Drawing.Size(85, 15);
			this.lbl28_.TabIndex = 19;
			this.lbl28_.Text = "pck ram";
			this.lbl28_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl28_.MouseEnter += new System.EventHandler(this.OnEnter28);
			// 
			// lbl29
			// 
			this.lbl29.Location = new System.Drawing.Point(10, 155);
			this.lbl29.Margin = new System.Windows.Forms.Padding(0);
			this.lbl29.Name = "lbl29";
			this.lbl29.Size = new System.Drawing.Size(30, 15);
			this.lbl29.TabIndex = 21;
			this.lbl29.Text = "#29";
			this.lbl29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl29.MouseEnter += new System.EventHandler(this.OnEnter29);
			// 
			// lbl29_
			// 
			this.lbl29_.Location = new System.Drawing.Point(40, 155);
			this.lbl29_.Margin = new System.Windows.Forms.Padding(0);
			this.lbl29_.Name = "lbl29_";
			this.lbl29_.Size = new System.Drawing.Size(85, 15);
			this.lbl29_.TabIndex = 22;
			this.lbl29_.Text = "pck ram";
			this.lbl29_.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl29_.MouseEnter += new System.EventHandler(this.OnEnter29);
			// 
			// ss_Statusbar
			// 
			this.ss_Statusbar.Font = new System.Drawing.Font("Consolas", 7F);
			this.ss_Statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tssl_Overval,
			this.tssl_Records,
			this.tssl_Sprites,
			this.tssl_Offset,
			this.tssl_OffsetLast,
			this.tssl_OffsetAftr});
			this.ss_Statusbar.Location = new System.Drawing.Point(0, 772);
			this.ss_Statusbar.Name = "ss_Statusbar";
			this.ss_Statusbar.Size = new System.Drawing.Size(832, 22);
			this.ss_Statusbar.TabIndex = 3;
			this.ss_Statusbar.Text = "ss_Statusbar";
			// 
			// tssl_Overval
			// 
			this.tssl_Overval.AutoSize = false;
			this.tssl_Overval.Font = new System.Drawing.Font("Consolas", 8F);
			this.tssl_Overval.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.tssl_Overval.Name = "tssl_Overval";
			this.tssl_Overval.Size = new System.Drawing.Size(300, 22);
			this.tssl_Overval.Text = "over";
			this.tssl_Overval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_Records
			// 
			this.tssl_Records.AutoSize = false;
			this.tssl_Records.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_Records.Name = "tssl_Records";
			this.tssl_Records.Size = new System.Drawing.Size(95, 22);
			this.tssl_Records.Text = "records";
			this.tssl_Records.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_Sprites
			// 
			this.tssl_Sprites.AutoSize = false;
			this.tssl_Sprites.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_Sprites.Name = "tssl_Sprites";
			this.tssl_Sprites.Size = new System.Drawing.Size(95, 22);
			this.tssl_Sprites.Text = "sprites";
			this.tssl_Sprites.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_Offset
			// 
			this.tssl_Offset.AutoSize = false;
			this.tssl_Offset.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_Offset.Name = "tssl_Offset";
			this.tssl_Offset.Size = new System.Drawing.Size(34, 22);
			this.tssl_Offset.Text = "Offset";
			this.tssl_Offset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_OffsetLast
			// 
			this.tssl_OffsetLast.AutoSize = false;
			this.tssl_OffsetLast.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_OffsetLast.Name = "tssl_OffsetLast";
			this.tssl_OffsetLast.Size = new System.Drawing.Size(70, 22);
			this.tssl_OffsetLast.Text = "last";
			this.tssl_OffsetLast.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tssl_OffsetAftr
			// 
			this.tssl_OffsetAftr.AutoSize = false;
			this.tssl_OffsetAftr.Margin = new System.Windows.Forms.Padding(0);
			this.tssl_OffsetAftr.Name = "tssl_OffsetAftr";
			this.tssl_OffsetAftr.Size = new System.Drawing.Size(70, 22);
			this.tssl_OffsetAftr.Text = "after";
			this.tssl_OffsetAftr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gb_Unused
			// 
			this.gb_Unused.Controls.Add(this.lbl38);
			this.gb_Unused.Controls.Add(this.lbl22);
			this.gb_Unused.Controls.Add(this.lbl29);
			this.gb_Unused.Controls.Add(this.lbl23);
			this.gb_Unused.Controls.Add(this.lbl28);
			this.gb_Unused.Controls.Add(this.lbl24);
			this.gb_Unused.Controls.Add(this.lbl27);
			this.gb_Unused.Controls.Add(this.lbl25);
			this.gb_Unused.Controls.Add(this.lbl26);
			this.gb_Unused.Controls.Add(this.lbl47);
			this.gb_Unused.Controls.Add(this.lbl50);
			this.gb_Unused.Controls.Add(this.lbl61);
			this.gb_Unused.Controls.Add(this.lbl38_);
			this.gb_Unused.Controls.Add(this.lbl22_);
			this.gb_Unused.Controls.Add(this.lbl23_);
			this.gb_Unused.Controls.Add(this.lbl29_);
			this.gb_Unused.Controls.Add(this.lbl24_);
			this.gb_Unused.Controls.Add(this.lbl28_);
			this.gb_Unused.Controls.Add(this.lbl25_);
			this.gb_Unused.Controls.Add(this.lbl27_);
			this.gb_Unused.Controls.Add(this.lbl26_);
			this.gb_Unused.Controls.Add(this.lbl47_);
			this.gb_Unused.Controls.Add(this.lbl50_);
			this.gb_Unused.Controls.Add(this.lbl61_);
			this.gb_Unused.Controls.Add(this.tb38_);
			this.gb_Unused.Controls.Add(this.tb61_);
			this.gb_Unused.Controls.Add(this.tb50_);
			this.gb_Unused.Controls.Add(this.tb47_);
			this.gb_Unused.Controls.Add(this.tb29_);
			this.gb_Unused.Controls.Add(this.tb28_);
			this.gb_Unused.Controls.Add(this.tb27_);
			this.gb_Unused.Controls.Add(this.tb26_);
			this.gb_Unused.Controls.Add(this.tb25_);
			this.gb_Unused.Controls.Add(this.tb24_);
			this.gb_Unused.Controls.Add(this.tb23_);
			this.gb_Unused.Controls.Add(this.tb22_);
			this.gb_Unused.Location = new System.Drawing.Point(345, 105);
			this.gb_Unused.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Unused.Name = "gb_Unused";
			this.gb_Unused.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Unused.Size = new System.Drawing.Size(165, 255);
			this.gb_Unused.TabIndex = 9;
			this.gb_Unused.TabStop = false;
			this.gb_Unused.Text = " doh! ";
			// 
			// tb38_
			// 
			this.tb38_.Location = new System.Drawing.Point(125, 173);
			this.tb38_.Margin = new System.Windows.Forms.Padding(0);
			this.tb38_.Name = "tb38_";
			this.tb38_.Size = new System.Drawing.Size(35, 19);
			this.tb38_.TabIndex = 26;
			this.tb38_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb38_.WordWrap = false;
			this.tb38_.TextChanged += new System.EventHandler(this.OnChanged38);
			this.tb38_.Enter += new System.EventHandler(this.OnEnter38);
			this.tb38_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox38);
			// 
			// tb61_
			// 
			this.tb61_.Location = new System.Drawing.Point(125, 233);
			this.tb61_.Margin = new System.Windows.Forms.Padding(0);
			this.tb61_.Name = "tb61_";
			this.tb61_.Size = new System.Drawing.Size(35, 19);
			this.tb61_.TabIndex = 35;
			this.tb61_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb61_.WordWrap = false;
			this.tb61_.TextChanged += new System.EventHandler(this.OnChanged61);
			this.tb61_.Enter += new System.EventHandler(this.OnEnter61);
			this.tb61_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox61);
			// 
			// tb50_
			// 
			this.tb50_.Location = new System.Drawing.Point(125, 213);
			this.tb50_.Margin = new System.Windows.Forms.Padding(0);
			this.tb50_.Name = "tb50_";
			this.tb50_.Size = new System.Drawing.Size(35, 19);
			this.tb50_.TabIndex = 32;
			this.tb50_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb50_.WordWrap = false;
			this.tb50_.TextChanged += new System.EventHandler(this.OnChanged50);
			this.tb50_.Enter += new System.EventHandler(this.OnEnter50);
			this.tb50_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox50);
			// 
			// tb47_
			// 
			this.tb47_.Location = new System.Drawing.Point(125, 193);
			this.tb47_.Margin = new System.Windows.Forms.Padding(0);
			this.tb47_.Name = "tb47_";
			this.tb47_.Size = new System.Drawing.Size(35, 19);
			this.tb47_.TabIndex = 29;
			this.tb47_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb47_.WordWrap = false;
			this.tb47_.TextChanged += new System.EventHandler(this.OnChanged47);
			this.tb47_.Enter += new System.EventHandler(this.OnEnter47);
			this.tb47_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox47);
			// 
			// tb29_
			// 
			this.tb29_.Location = new System.Drawing.Point(125, 153);
			this.tb29_.Margin = new System.Windows.Forms.Padding(0);
			this.tb29_.Name = "tb29_";
			this.tb29_.Size = new System.Drawing.Size(35, 19);
			this.tb29_.TabIndex = 23;
			this.tb29_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb29_.WordWrap = false;
			this.tb29_.TextChanged += new System.EventHandler(this.OnChanged29);
			this.tb29_.Enter += new System.EventHandler(this.OnEnter29);
			this.tb29_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox29);
			// 
			// tb28_
			// 
			this.tb28_.Location = new System.Drawing.Point(125, 133);
			this.tb28_.Margin = new System.Windows.Forms.Padding(0);
			this.tb28_.Name = "tb28_";
			this.tb28_.Size = new System.Drawing.Size(35, 19);
			this.tb28_.TabIndex = 20;
			this.tb28_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb28_.WordWrap = false;
			this.tb28_.TextChanged += new System.EventHandler(this.OnChanged28);
			this.tb28_.Enter += new System.EventHandler(this.OnEnter28);
			this.tb28_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox28);
			// 
			// tb27_
			// 
			this.tb27_.Location = new System.Drawing.Point(125, 113);
			this.tb27_.Margin = new System.Windows.Forms.Padding(0);
			this.tb27_.Name = "tb27_";
			this.tb27_.Size = new System.Drawing.Size(35, 19);
			this.tb27_.TabIndex = 17;
			this.tb27_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb27_.WordWrap = false;
			this.tb27_.TextChanged += new System.EventHandler(this.OnChanged27);
			this.tb27_.Enter += new System.EventHandler(this.OnEnter27);
			this.tb27_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox27);
			// 
			// tb26_
			// 
			this.tb26_.Location = new System.Drawing.Point(125, 93);
			this.tb26_.Margin = new System.Windows.Forms.Padding(0);
			this.tb26_.Name = "tb26_";
			this.tb26_.Size = new System.Drawing.Size(35, 19);
			this.tb26_.TabIndex = 14;
			this.tb26_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb26_.WordWrap = false;
			this.tb26_.TextChanged += new System.EventHandler(this.OnChanged26);
			this.tb26_.Enter += new System.EventHandler(this.OnEnter26);
			this.tb26_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox26);
			// 
			// tb25_
			// 
			this.tb25_.Location = new System.Drawing.Point(125, 73);
			this.tb25_.Margin = new System.Windows.Forms.Padding(0);
			this.tb25_.Name = "tb25_";
			this.tb25_.Size = new System.Drawing.Size(35, 19);
			this.tb25_.TabIndex = 11;
			this.tb25_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb25_.WordWrap = false;
			this.tb25_.TextChanged += new System.EventHandler(this.OnChanged25);
			this.tb25_.Enter += new System.EventHandler(this.OnEnter25);
			this.tb25_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox25);
			// 
			// tb24_
			// 
			this.tb24_.Location = new System.Drawing.Point(125, 53);
			this.tb24_.Margin = new System.Windows.Forms.Padding(0);
			this.tb24_.Name = "tb24_";
			this.tb24_.Size = new System.Drawing.Size(35, 19);
			this.tb24_.TabIndex = 8;
			this.tb24_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb24_.WordWrap = false;
			this.tb24_.TextChanged += new System.EventHandler(this.OnChanged24);
			this.tb24_.Enter += new System.EventHandler(this.OnEnter24);
			this.tb24_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox24);
			// 
			// tb23_
			// 
			this.tb23_.Location = new System.Drawing.Point(125, 33);
			this.tb23_.Margin = new System.Windows.Forms.Padding(0);
			this.tb23_.Name = "tb23_";
			this.tb23_.Size = new System.Drawing.Size(35, 19);
			this.tb23_.TabIndex = 5;
			this.tb23_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb23_.WordWrap = false;
			this.tb23_.TextChanged += new System.EventHandler(this.OnChanged23);
			this.tb23_.Enter += new System.EventHandler(this.OnEnter23);
			this.tb23_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox23);
			// 
			// tb22_
			// 
			this.tb22_.Location = new System.Drawing.Point(125, 13);
			this.tb22_.Margin = new System.Windows.Forms.Padding(0);
			this.tb22_.Name = "tb22_";
			this.tb22_.Size = new System.Drawing.Size(35, 19);
			this.tb22_.TabIndex = 2;
			this.tb22_.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb22_.WordWrap = false;
			this.tb22_.TextChanged += new System.EventHandler(this.OnChanged22);
			this.tb22_.Enter += new System.EventHandler(this.OnEnter22);
			this.tb22_.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox22);
			// 
			// gb_Loft
			// 
			this.gb_Loft.Controls.Add(this.lbl19);
			this.gb_Loft.Controls.Add(this.lbl18);
			this.gb_Loft.Controls.Add(this.lbl17);
			this.gb_Loft.Controls.Add(this.lbl16);
			this.gb_Loft.Controls.Add(this.lbl15);
			this.gb_Loft.Controls.Add(this.lbl14);
			this.gb_Loft.Controls.Add(this.lbl13);
			this.gb_Loft.Controls.Add(this.lbl12);
			this.gb_Loft.Controls.Add(this.lbl11);
			this.gb_Loft.Controls.Add(this.lbl08);
			this.gb_Loft.Controls.Add(this.lbl10);
			this.gb_Loft.Controls.Add(this.lbl09);
			this.gb_Loft.Controls.Add(this.lbl08_loft00);
			this.gb_Loft.Controls.Add(this.lbl19_loft11);
			this.gb_Loft.Controls.Add(this.lbl18_loft10);
			this.gb_Loft.Controls.Add(this.lbl17_loft09);
			this.gb_Loft.Controls.Add(this.lbl16_loft08);
			this.gb_Loft.Controls.Add(this.lbl15_loft07);
			this.gb_Loft.Controls.Add(this.lbl14_loft06);
			this.gb_Loft.Controls.Add(this.lbl13_loft05);
			this.gb_Loft.Controls.Add(this.lbl12_loft04);
			this.gb_Loft.Controls.Add(this.lbl11_loft03);
			this.gb_Loft.Controls.Add(this.lbl10_loft02);
			this.gb_Loft.Controls.Add(this.lbl09_loft01);
			this.gb_Loft.Controls.Add(this.tb19_loft11);
			this.gb_Loft.Controls.Add(this.tb18_loft10);
			this.gb_Loft.Controls.Add(this.tb17_loft09);
			this.gb_Loft.Controls.Add(this.tb16_loft08);
			this.gb_Loft.Controls.Add(this.tb15_loft07);
			this.gb_Loft.Controls.Add(this.tb14_loft06);
			this.gb_Loft.Controls.Add(this.tb13_loft05);
			this.gb_Loft.Controls.Add(this.tb12_loft04);
			this.gb_Loft.Controls.Add(this.tb11_loft03);
			this.gb_Loft.Controls.Add(this.tb10_loft02);
			this.gb_Loft.Controls.Add(this.tb09_loft01);
			this.gb_Loft.Controls.Add(this.tb08_loft00);
			this.gb_Loft.Controls.Add(this.pnl_Loft08);
			this.gb_Loft.Controls.Add(this.pnl_Loft09);
			this.gb_Loft.Controls.Add(this.pnl_Loft10);
			this.gb_Loft.Controls.Add(this.pnl_Loft11);
			this.gb_Loft.Controls.Add(this.pnl_Loft12);
			this.gb_Loft.Controls.Add(this.pnl_Loft13);
			this.gb_Loft.Controls.Add(this.pnl_Loft14);
			this.gb_Loft.Controls.Add(this.pnl_Loft15);
			this.gb_Loft.Controls.Add(this.pnl_Loft16);
			this.gb_Loft.Controls.Add(this.pnl_Loft17);
			this.gb_Loft.Controls.Add(this.pnl_Loft18);
			this.gb_Loft.Controls.Add(this.pnl_Loft19);
			this.gb_Loft.Dock = System.Windows.Forms.DockStyle.Right;
			this.gb_Loft.Location = new System.Drawing.Point(672, 0);
			this.gb_Loft.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Loft.Name = "gb_Loft";
			this.gb_Loft.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Loft.Size = new System.Drawing.Size(160, 457);
			this.gb_Loft.TabIndex = 10;
			this.gb_Loft.TabStop = false;
			this.gb_Loft.Text = " LoFT ";
			this.gb_Loft.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint_LoFT_group);
			// 
			// tb19_loft11
			// 
			this.tb19_loft11.Location = new System.Drawing.Point(80, 23);
			this.tb19_loft11.Margin = new System.Windows.Forms.Padding(0);
			this.tb19_loft11.Name = "tb19_loft11";
			this.tb19_loft11.Size = new System.Drawing.Size(35, 19);
			this.tb19_loft11.TabIndex = 2;
			this.tb19_loft11.Tag = "11";
			this.tb19_loft11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb19_loft11.WordWrap = false;
			this.tb19_loft11.TextChanged += new System.EventHandler(this.OnChanged19);
			this.tb19_loft11.Enter += new System.EventHandler(this.OnEnter19);
			this.tb19_loft11.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox19);
			// 
			// tb18_loft10
			// 
			this.tb18_loft10.Location = new System.Drawing.Point(80, 58);
			this.tb18_loft10.Margin = new System.Windows.Forms.Padding(0);
			this.tb18_loft10.Name = "tb18_loft10";
			this.tb18_loft10.Size = new System.Drawing.Size(35, 19);
			this.tb18_loft10.TabIndex = 6;
			this.tb18_loft10.Tag = "10";
			this.tb18_loft10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb18_loft10.WordWrap = false;
			this.tb18_loft10.TextChanged += new System.EventHandler(this.OnChanged18);
			this.tb18_loft10.Enter += new System.EventHandler(this.OnEnter18);
			this.tb18_loft10.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox18);
			// 
			// tb17_loft09
			// 
			this.tb17_loft09.Location = new System.Drawing.Point(80, 93);
			this.tb17_loft09.Margin = new System.Windows.Forms.Padding(0);
			this.tb17_loft09.Name = "tb17_loft09";
			this.tb17_loft09.Size = new System.Drawing.Size(35, 19);
			this.tb17_loft09.TabIndex = 10;
			this.tb17_loft09.Tag = "9";
			this.tb17_loft09.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb17_loft09.WordWrap = false;
			this.tb17_loft09.TextChanged += new System.EventHandler(this.OnChanged17);
			this.tb17_loft09.Enter += new System.EventHandler(this.OnEnter17);
			this.tb17_loft09.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox17);
			// 
			// tb16_loft08
			// 
			this.tb16_loft08.Location = new System.Drawing.Point(80, 128);
			this.tb16_loft08.Margin = new System.Windows.Forms.Padding(0);
			this.tb16_loft08.Name = "tb16_loft08";
			this.tb16_loft08.Size = new System.Drawing.Size(35, 19);
			this.tb16_loft08.TabIndex = 14;
			this.tb16_loft08.Tag = "8";
			this.tb16_loft08.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb16_loft08.WordWrap = false;
			this.tb16_loft08.TextChanged += new System.EventHandler(this.OnChanged16);
			this.tb16_loft08.Enter += new System.EventHandler(this.OnEnter16);
			this.tb16_loft08.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox16);
			// 
			// tb15_loft07
			// 
			this.tb15_loft07.Location = new System.Drawing.Point(80, 163);
			this.tb15_loft07.Margin = new System.Windows.Forms.Padding(0);
			this.tb15_loft07.Name = "tb15_loft07";
			this.tb15_loft07.Size = new System.Drawing.Size(35, 19);
			this.tb15_loft07.TabIndex = 18;
			this.tb15_loft07.Tag = "7";
			this.tb15_loft07.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb15_loft07.WordWrap = false;
			this.tb15_loft07.TextChanged += new System.EventHandler(this.OnChanged15);
			this.tb15_loft07.Enter += new System.EventHandler(this.OnEnter15);
			this.tb15_loft07.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox15);
			// 
			// tb14_loft06
			// 
			this.tb14_loft06.Location = new System.Drawing.Point(80, 198);
			this.tb14_loft06.Margin = new System.Windows.Forms.Padding(0);
			this.tb14_loft06.Name = "tb14_loft06";
			this.tb14_loft06.Size = new System.Drawing.Size(35, 19);
			this.tb14_loft06.TabIndex = 22;
			this.tb14_loft06.Tag = "6";
			this.tb14_loft06.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb14_loft06.WordWrap = false;
			this.tb14_loft06.TextChanged += new System.EventHandler(this.OnChanged14);
			this.tb14_loft06.Enter += new System.EventHandler(this.OnEnter14);
			this.tb14_loft06.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox14);
			// 
			// tb13_loft05
			// 
			this.tb13_loft05.Location = new System.Drawing.Point(80, 233);
			this.tb13_loft05.Margin = new System.Windows.Forms.Padding(0);
			this.tb13_loft05.Name = "tb13_loft05";
			this.tb13_loft05.Size = new System.Drawing.Size(35, 19);
			this.tb13_loft05.TabIndex = 26;
			this.tb13_loft05.Tag = "5";
			this.tb13_loft05.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb13_loft05.WordWrap = false;
			this.tb13_loft05.TextChanged += new System.EventHandler(this.OnChanged13);
			this.tb13_loft05.Enter += new System.EventHandler(this.OnEnter13);
			this.tb13_loft05.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox13);
			// 
			// tb12_loft04
			// 
			this.tb12_loft04.Location = new System.Drawing.Point(80, 268);
			this.tb12_loft04.Margin = new System.Windows.Forms.Padding(0);
			this.tb12_loft04.Name = "tb12_loft04";
			this.tb12_loft04.Size = new System.Drawing.Size(35, 19);
			this.tb12_loft04.TabIndex = 30;
			this.tb12_loft04.Tag = "4";
			this.tb12_loft04.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb12_loft04.WordWrap = false;
			this.tb12_loft04.TextChanged += new System.EventHandler(this.OnChanged12);
			this.tb12_loft04.Enter += new System.EventHandler(this.OnEnter12);
			this.tb12_loft04.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox12);
			// 
			// tb11_loft03
			// 
			this.tb11_loft03.Location = new System.Drawing.Point(80, 303);
			this.tb11_loft03.Margin = new System.Windows.Forms.Padding(0);
			this.tb11_loft03.Name = "tb11_loft03";
			this.tb11_loft03.Size = new System.Drawing.Size(35, 19);
			this.tb11_loft03.TabIndex = 34;
			this.tb11_loft03.Tag = "3";
			this.tb11_loft03.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb11_loft03.WordWrap = false;
			this.tb11_loft03.TextChanged += new System.EventHandler(this.OnChanged11);
			this.tb11_loft03.Enter += new System.EventHandler(this.OnEnter11);
			this.tb11_loft03.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox11);
			// 
			// tb10_loft02
			// 
			this.tb10_loft02.Location = new System.Drawing.Point(80, 338);
			this.tb10_loft02.Margin = new System.Windows.Forms.Padding(0);
			this.tb10_loft02.Name = "tb10_loft02";
			this.tb10_loft02.Size = new System.Drawing.Size(35, 19);
			this.tb10_loft02.TabIndex = 38;
			this.tb10_loft02.Tag = "2";
			this.tb10_loft02.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb10_loft02.WordWrap = false;
			this.tb10_loft02.TextChanged += new System.EventHandler(this.OnChanged10);
			this.tb10_loft02.Enter += new System.EventHandler(this.OnEnter10);
			this.tb10_loft02.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox10);
			// 
			// tb09_loft01
			// 
			this.tb09_loft01.Location = new System.Drawing.Point(80, 373);
			this.tb09_loft01.Margin = new System.Windows.Forms.Padding(0);
			this.tb09_loft01.Name = "tb09_loft01";
			this.tb09_loft01.Size = new System.Drawing.Size(35, 19);
			this.tb09_loft01.TabIndex = 42;
			this.tb09_loft01.Tag = "1";
			this.tb09_loft01.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb09_loft01.WordWrap = false;
			this.tb09_loft01.TextChanged += new System.EventHandler(this.OnChanged9);
			this.tb09_loft01.Enter += new System.EventHandler(this.OnEnter9);
			this.tb09_loft01.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox9);
			// 
			// tb08_loft00
			// 
			this.tb08_loft00.Location = new System.Drawing.Point(80, 408);
			this.tb08_loft00.Margin = new System.Windows.Forms.Padding(0);
			this.tb08_loft00.Name = "tb08_loft00";
			this.tb08_loft00.Size = new System.Drawing.Size(35, 19);
			this.tb08_loft00.TabIndex = 46;
			this.tb08_loft00.Tag = "0";
			this.tb08_loft00.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb08_loft00.WordWrap = false;
			this.tb08_loft00.TextChanged += new System.EventHandler(this.OnChanged8);
			this.tb08_loft00.Enter += new System.EventHandler(this.OnEnter8);
			this.tb08_loft00.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox8);
			// 
			// pnl_Loft08
			// 
			this.pnl_Loft08.Location = new System.Drawing.Point(121, 400);
			this.pnl_Loft08.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft08.Name = "pnl_Loft08";
			this.pnl_Loft08.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft08.TabIndex = 47;
			// 
			// pnl_Loft09
			// 
			this.pnl_Loft09.Location = new System.Drawing.Point(121, 365);
			this.pnl_Loft09.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft09.Name = "pnl_Loft09";
			this.pnl_Loft09.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft09.TabIndex = 43;
			// 
			// pnl_Loft10
			// 
			this.pnl_Loft10.Location = new System.Drawing.Point(121, 330);
			this.pnl_Loft10.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft10.Name = "pnl_Loft10";
			this.pnl_Loft10.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft10.TabIndex = 39;
			// 
			// pnl_Loft11
			// 
			this.pnl_Loft11.Location = new System.Drawing.Point(121, 295);
			this.pnl_Loft11.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft11.Name = "pnl_Loft11";
			this.pnl_Loft11.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft11.TabIndex = 35;
			// 
			// pnl_Loft12
			// 
			this.pnl_Loft12.Location = new System.Drawing.Point(121, 260);
			this.pnl_Loft12.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft12.Name = "pnl_Loft12";
			this.pnl_Loft12.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft12.TabIndex = 31;
			// 
			// pnl_Loft13
			// 
			this.pnl_Loft13.Location = new System.Drawing.Point(121, 225);
			this.pnl_Loft13.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft13.Name = "pnl_Loft13";
			this.pnl_Loft13.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft13.TabIndex = 27;
			// 
			// pnl_Loft14
			// 
			this.pnl_Loft14.Location = new System.Drawing.Point(121, 190);
			this.pnl_Loft14.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft14.Name = "pnl_Loft14";
			this.pnl_Loft14.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft14.TabIndex = 23;
			// 
			// pnl_Loft15
			// 
			this.pnl_Loft15.Location = new System.Drawing.Point(121, 155);
			this.pnl_Loft15.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft15.Name = "pnl_Loft15";
			this.pnl_Loft15.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft15.TabIndex = 19;
			// 
			// pnl_Loft16
			// 
			this.pnl_Loft16.Location = new System.Drawing.Point(121, 120);
			this.pnl_Loft16.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft16.Name = "pnl_Loft16";
			this.pnl_Loft16.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft16.TabIndex = 15;
			// 
			// pnl_Loft17
			// 
			this.pnl_Loft17.Location = new System.Drawing.Point(121, 85);
			this.pnl_Loft17.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft17.Name = "pnl_Loft17";
			this.pnl_Loft17.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft17.TabIndex = 11;
			// 
			// pnl_Loft18
			// 
			this.pnl_Loft18.Location = new System.Drawing.Point(121, 50);
			this.pnl_Loft18.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft18.Name = "pnl_Loft18";
			this.pnl_Loft18.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft18.TabIndex = 7;
			// 
			// pnl_Loft19
			// 
			this.pnl_Loft19.Location = new System.Drawing.Point(121, 15);
			this.pnl_Loft19.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Loft19.Name = "pnl_Loft19";
			this.pnl_Loft19.Size = new System.Drawing.Size(32, 32);
			this.pnl_Loft19.TabIndex = 3;
			// 
			// gb_Sprites
			// 
			this.gb_Sprites.Controls.Add(this.lbl00);
			this.gb_Sprites.Controls.Add(this.lbl07);
			this.gb_Sprites.Controls.Add(this.lbl01);
			this.gb_Sprites.Controls.Add(this.lbl06);
			this.gb_Sprites.Controls.Add(this.lbl02);
			this.gb_Sprites.Controls.Add(this.lbl03);
			this.gb_Sprites.Controls.Add(this.lbl05);
			this.gb_Sprites.Controls.Add(this.lbl04);
			this.gb_Sprites.Controls.Add(this.lbl03_phase3);
			this.gb_Sprites.Controls.Add(this.lbl00_phase0);
			this.gb_Sprites.Controls.Add(this.lbl01_phase1);
			this.gb_Sprites.Controls.Add(this.lbl07_phase7);
			this.gb_Sprites.Controls.Add(this.lbl02_phase2);
			this.gb_Sprites.Controls.Add(this.lbl06_phase6);
			this.gb_Sprites.Controls.Add(this.lbl04_phase4);
			this.gb_Sprites.Controls.Add(this.lbl05_phase5);
			this.gb_Sprites.Controls.Add(this.tb07_phase7);
			this.gb_Sprites.Controls.Add(this.tb06_phase6);
			this.gb_Sprites.Controls.Add(this.tb05_phase5);
			this.gb_Sprites.Controls.Add(this.tb04_phase4);
			this.gb_Sprites.Controls.Add(this.tb03_phase3);
			this.gb_Sprites.Controls.Add(this.tb02_phase2);
			this.gb_Sprites.Controls.Add(this.tb01_phase1);
			this.gb_Sprites.Controls.Add(this.tb00_phase0);
			this.gb_Sprites.Controls.Add(this.pnl_Sprites);
			this.gb_Sprites.Dock = System.Windows.Forms.DockStyle.Top;
			this.gb_Sprites.Location = new System.Drawing.Point(0, 175);
			this.gb_Sprites.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Sprites.Name = "gb_Sprites";
			this.gb_Sprites.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Sprites.Size = new System.Drawing.Size(832, 140);
			this.gb_Sprites.TabIndex = 1;
			this.gb_Sprites.TabStop = false;
			this.gb_Sprites.Text = " Sprites ";
			// 
			// tb07_phase7
			// 
			this.tb07_phase7.Location = new System.Drawing.Point(580, 100);
			this.tb07_phase7.Margin = new System.Windows.Forms.Padding(0);
			this.tb07_phase7.Name = "tb07_phase7";
			this.tb07_phase7.Size = new System.Drawing.Size(35, 19);
			this.tb07_phase7.TabIndex = 24;
			this.tb07_phase7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb07_phase7.WordWrap = false;
			this.tb07_phase7.TextChanged += new System.EventHandler(this.OnChanged7);
			this.tb07_phase7.Enter += new System.EventHandler(this.OnEnter7);
			this.tb07_phase7.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox7);
			// 
			// tb06_phase6
			// 
			this.tb06_phase6.Location = new System.Drawing.Point(500, 100);
			this.tb06_phase6.Margin = new System.Windows.Forms.Padding(0);
			this.tb06_phase6.Name = "tb06_phase6";
			this.tb06_phase6.Size = new System.Drawing.Size(35, 19);
			this.tb06_phase6.TabIndex = 21;
			this.tb06_phase6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb06_phase6.WordWrap = false;
			this.tb06_phase6.TextChanged += new System.EventHandler(this.OnChanged6);
			this.tb06_phase6.Enter += new System.EventHandler(this.OnEnter6);
			this.tb06_phase6.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox6);
			// 
			// tb05_phase5
			// 
			this.tb05_phase5.Location = new System.Drawing.Point(420, 100);
			this.tb05_phase5.Margin = new System.Windows.Forms.Padding(0);
			this.tb05_phase5.Name = "tb05_phase5";
			this.tb05_phase5.Size = new System.Drawing.Size(35, 19);
			this.tb05_phase5.TabIndex = 18;
			this.tb05_phase5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb05_phase5.WordWrap = false;
			this.tb05_phase5.TextChanged += new System.EventHandler(this.OnChanged5);
			this.tb05_phase5.Enter += new System.EventHandler(this.OnEnter5);
			this.tb05_phase5.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox5);
			// 
			// tb04_phase4
			// 
			this.tb04_phase4.Location = new System.Drawing.Point(340, 100);
			this.tb04_phase4.Margin = new System.Windows.Forms.Padding(0);
			this.tb04_phase4.Name = "tb04_phase4";
			this.tb04_phase4.Size = new System.Drawing.Size(35, 19);
			this.tb04_phase4.TabIndex = 15;
			this.tb04_phase4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb04_phase4.WordWrap = false;
			this.tb04_phase4.TextChanged += new System.EventHandler(this.OnChanged4);
			this.tb04_phase4.Enter += new System.EventHandler(this.OnEnter4);
			this.tb04_phase4.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox4);
			// 
			// tb03_phase3
			// 
			this.tb03_phase3.Location = new System.Drawing.Point(265, 100);
			this.tb03_phase3.Margin = new System.Windows.Forms.Padding(0);
			this.tb03_phase3.Name = "tb03_phase3";
			this.tb03_phase3.Size = new System.Drawing.Size(35, 19);
			this.tb03_phase3.TabIndex = 12;
			this.tb03_phase3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb03_phase3.WordWrap = false;
			this.tb03_phase3.TextChanged += new System.EventHandler(this.OnChanged3);
			this.tb03_phase3.Enter += new System.EventHandler(this.OnEnter3);
			this.tb03_phase3.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox3);
			// 
			// tb02_phase2
			// 
			this.tb02_phase2.Location = new System.Drawing.Point(190, 100);
			this.tb02_phase2.Margin = new System.Windows.Forms.Padding(0);
			this.tb02_phase2.Name = "tb02_phase2";
			this.tb02_phase2.Size = new System.Drawing.Size(35, 19);
			this.tb02_phase2.TabIndex = 9;
			this.tb02_phase2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb02_phase2.WordWrap = false;
			this.tb02_phase2.TextChanged += new System.EventHandler(this.OnChanged2);
			this.tb02_phase2.Enter += new System.EventHandler(this.OnEnter2);
			this.tb02_phase2.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox2);
			// 
			// tb01_phase1
			// 
			this.tb01_phase1.Location = new System.Drawing.Point(110, 100);
			this.tb01_phase1.Margin = new System.Windows.Forms.Padding(0);
			this.tb01_phase1.Name = "tb01_phase1";
			this.tb01_phase1.Size = new System.Drawing.Size(35, 19);
			this.tb01_phase1.TabIndex = 6;
			this.tb01_phase1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb01_phase1.WordWrap = false;
			this.tb01_phase1.TextChanged += new System.EventHandler(this.OnChanged1);
			this.tb01_phase1.Enter += new System.EventHandler(this.OnEnter1);
			this.tb01_phase1.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox1);
			// 
			// tb00_phase0
			// 
			this.tb00_phase0.Location = new System.Drawing.Point(35, 100);
			this.tb00_phase0.Margin = new System.Windows.Forms.Padding(0);
			this.tb00_phase0.Name = "tb00_phase0";
			this.tb00_phase0.Size = new System.Drawing.Size(35, 19);
			this.tb00_phase0.TabIndex = 3;
			this.tb00_phase0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb00_phase0.WordWrap = false;
			this.tb00_phase0.TextChanged += new System.EventHandler(this.OnChanged0);
			this.tb00_phase0.Enter += new System.EventHandler(this.OnEnter0);
			this.tb00_phase0.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox0);
			// 
			// pnl_Sprites
			// 
			this.pnl_Sprites.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.pnl_Sprites.Location = new System.Drawing.Point(5, 15);
			this.pnl_Sprites.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Sprites.Name = "pnl_Sprites";
			this.pnl_Sprites.Size = new System.Drawing.Size(680, 80);
			this.pnl_Sprites.TabIndex = 0;
			this.pnl_Sprites.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint_Sprites);
			this.pnl_Sprites.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown_SpritePanel);
			// 
			// gb_Collection
			// 
			this.gb_Collection.Dock = System.Windows.Forms.DockStyle.Top;
			this.gb_Collection.Location = new System.Drawing.Point(0, 0);
			this.gb_Collection.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Collection.Name = "gb_Collection";
			this.gb_Collection.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Collection.Size = new System.Drawing.Size(832, 175);
			this.gb_Collection.TabIndex = 0;
			this.gb_Collection.TabStop = false;
			this.gb_Collection.Text = " RECORD COLLECTION ";
			// 
			// gb_Overhead
			// 
			this.gb_Overhead.Controls.Add(this.lbl20);
			this.gb_Overhead.Controls.Add(this.lbl20_scang);
			this.gb_Overhead.Controls.Add(this.tb20_scang2);
			this.gb_Overhead.Controls.Add(this.tb20_scang1);
			this.gb_Overhead.Controls.Add(this.pnl_ScanGic);
			this.gb_Overhead.Location = new System.Drawing.Point(5, 5);
			this.gb_Overhead.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Overhead.Name = "gb_Overhead";
			this.gb_Overhead.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Overhead.Size = new System.Drawing.Size(165, 55);
			this.gb_Overhead.TabIndex = 0;
			this.gb_Overhead.TabStop = false;
			this.gb_Overhead.Text = " Overhead ";
			this.gb_Overhead.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint_ScanG_group);
			// 
			// tb20_scang2
			// 
			this.tb20_scang2.Location = new System.Drawing.Point(60, 31);
			this.tb20_scang2.Margin = new System.Windows.Forms.Padding(0);
			this.tb20_scang2.Name = "tb20_scang2";
			this.tb20_scang2.Size = new System.Drawing.Size(35, 19);
			this.tb20_scang2.TabIndex = 3;
			this.tb20_scang2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb20_scang2.WordWrap = false;
			this.tb20_scang2.TextChanged += new System.EventHandler(this.OnChanged20r);
			this.tb20_scang2.Enter += new System.EventHandler(this.OnEnter20r);
			this.tb20_scang2.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox20r);
			// 
			// tb20_scang1
			// 
			this.tb20_scang1.Location = new System.Drawing.Point(20, 31);
			this.tb20_scang1.Margin = new System.Windows.Forms.Padding(0);
			this.tb20_scang1.Name = "tb20_scang1";
			this.tb20_scang1.Size = new System.Drawing.Size(35, 19);
			this.tb20_scang1.TabIndex = 2;
			this.tb20_scang1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb20_scang1.WordWrap = false;
			this.tb20_scang1.TextChanged += new System.EventHandler(this.OnChanged20);
			this.tb20_scang1.Enter += new System.EventHandler(this.OnEnter20);
			this.tb20_scang1.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox20);
			// 
			// pnl_ScanGic
			// 
			this.pnl_ScanGic.Location = new System.Drawing.Point(120, 15);
			this.pnl_ScanGic.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_ScanGic.Name = "pnl_ScanGic";
			this.pnl_ScanGic.Size = new System.Drawing.Size(32, 32);
			this.pnl_ScanGic.TabIndex = 4;
			this.pnl_ScanGic.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint_ScanG_panel);
			this.pnl_ScanGic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown_ScanGicon);
			// 
			// gb_Tu
			// 
			this.gb_Tu.Controls.Add(this.lbl39);
			this.gb_Tu.Controls.Add(this.lbl40);
			this.gb_Tu.Controls.Add(this.lbl41);
			this.gb_Tu.Controls.Add(this.lbl39_tuwalk);
			this.gb_Tu.Controls.Add(this.lbl40_tuslide);
			this.gb_Tu.Controls.Add(this.lbl41_tufly);
			this.gb_Tu.Controls.Add(this.tb41_tufly);
			this.gb_Tu.Controls.Add(this.tb40_tuslide);
			this.gb_Tu.Controls.Add(this.tb39_tuwalk);
			this.gb_Tu.Location = new System.Drawing.Point(175, 5);
			this.gb_Tu.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Tu.Name = "gb_Tu";
			this.gb_Tu.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Tu.Size = new System.Drawing.Size(165, 75);
			this.gb_Tu.TabIndex = 4;
			this.gb_Tu.TabStop = false;
			this.gb_Tu.Text = " TU ";
			// 
			// tb41_tufly
			// 
			this.tb41_tufly.Location = new System.Drawing.Point(125, 53);
			this.tb41_tufly.Margin = new System.Windows.Forms.Padding(0);
			this.tb41_tufly.Name = "tb41_tufly";
			this.tb41_tufly.Size = new System.Drawing.Size(35, 19);
			this.tb41_tufly.TabIndex = 8;
			this.tb41_tufly.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb41_tufly.WordWrap = false;
			this.tb41_tufly.TextChanged += new System.EventHandler(this.OnChanged41);
			this.tb41_tufly.Enter += new System.EventHandler(this.OnEnter41);
			this.tb41_tufly.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox41);
			// 
			// tb40_tuslide
			// 
			this.tb40_tuslide.Location = new System.Drawing.Point(125, 33);
			this.tb40_tuslide.Margin = new System.Windows.Forms.Padding(0);
			this.tb40_tuslide.Name = "tb40_tuslide";
			this.tb40_tuslide.Size = new System.Drawing.Size(35, 19);
			this.tb40_tuslide.TabIndex = 5;
			this.tb40_tuslide.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb40_tuslide.WordWrap = false;
			this.tb40_tuslide.TextChanged += new System.EventHandler(this.OnChanged40);
			this.tb40_tuslide.Enter += new System.EventHandler(this.OnEnter40);
			this.tb40_tuslide.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox40);
			// 
			// tb39_tuwalk
			// 
			this.tb39_tuwalk.Location = new System.Drawing.Point(125, 13);
			this.tb39_tuwalk.Margin = new System.Windows.Forms.Padding(0);
			this.tb39_tuwalk.Name = "tb39_tuwalk";
			this.tb39_tuwalk.Size = new System.Drawing.Size(35, 19);
			this.tb39_tuwalk.TabIndex = 2;
			this.tb39_tuwalk.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb39_tuwalk.WordWrap = false;
			this.tb39_tuwalk.TextChanged += new System.EventHandler(this.OnChanged39);
			this.tb39_tuwalk.Enter += new System.EventHandler(this.OnEnter39);
			this.tb39_tuwalk.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox39);
			// 
			// gb_Elevation
			// 
			this.gb_Elevation.Controls.Add(this.lbl49);
			this.gb_Elevation.Controls.Add(this.lbl48);
			this.gb_Elevation.Controls.Add(this.lbl49_spriteoffset);
			this.gb_Elevation.Controls.Add(this.lbl48_terrainoffset);
			this.gb_Elevation.Controls.Add(this.tb49_spriteoffset);
			this.gb_Elevation.Controls.Add(this.tb48_terrainoffset);
			this.gb_Elevation.Location = new System.Drawing.Point(175, 285);
			this.gb_Elevation.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Elevation.Name = "gb_Elevation";
			this.gb_Elevation.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Elevation.Size = new System.Drawing.Size(165, 55);
			this.gb_Elevation.TabIndex = 7;
			this.gb_Elevation.TabStop = false;
			this.gb_Elevation.Text = " Elevation ";
			// 
			// tb49_spriteoffset
			// 
			this.tb49_spriteoffset.Location = new System.Drawing.Point(125, 33);
			this.tb49_spriteoffset.Margin = new System.Windows.Forms.Padding(0);
			this.tb49_spriteoffset.Name = "tb49_spriteoffset";
			this.tb49_spriteoffset.Size = new System.Drawing.Size(35, 19);
			this.tb49_spriteoffset.TabIndex = 5;
			this.tb49_spriteoffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb49_spriteoffset.WordWrap = false;
			this.tb49_spriteoffset.TextChanged += new System.EventHandler(this.OnChanged49);
			this.tb49_spriteoffset.Enter += new System.EventHandler(this.OnEnter49);
			this.tb49_spriteoffset.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox49);
			// 
			// tb48_terrainoffset
			// 
			this.tb48_terrainoffset.Location = new System.Drawing.Point(125, 13);
			this.tb48_terrainoffset.Margin = new System.Windows.Forms.Padding(0);
			this.tb48_terrainoffset.Name = "tb48_terrainoffset";
			this.tb48_terrainoffset.Size = new System.Drawing.Size(35, 19);
			this.tb48_terrainoffset.TabIndex = 2;
			this.tb48_terrainoffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb48_terrainoffset.WordWrap = false;
			this.tb48_terrainoffset.TextChanged += new System.EventHandler(this.OnChanged48);
			this.tb48_terrainoffset.Enter += new System.EventHandler(this.OnEnter48);
			this.tb48_terrainoffset.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox48);
			// 
			// gb_Block
			// 
			this.gb_Block.Controls.Add(this.lbl31);
			this.gb_Block.Controls.Add(this.lbl36);
			this.gb_Block.Controls.Add(this.lbl37);
			this.gb_Block.Controls.Add(this.lbl43);
			this.gb_Block.Controls.Add(this.lbl51);
			this.gb_Block.Controls.Add(this.lbl56);
			this.gb_Block.Controls.Add(this.lbl31_isblocklos);
			this.gb_Block.Controls.Add(this.lbl36_isblockfire);
			this.gb_Block.Controls.Add(this.lbl37_isblocksmoke);
			this.gb_Block.Controls.Add(this.lbl43_heblock);
			this.gb_Block.Controls.Add(this.lbl51_lightblock);
			this.gb_Block.Controls.Add(this.lbl56_smokeblock);
			this.gb_Block.Controls.Add(this.tb51_lightblock);
			this.gb_Block.Controls.Add(this.tb56_smokeblock);
			this.gb_Block.Controls.Add(this.tb43_heblock);
			this.gb_Block.Controls.Add(this.tb37_isblocksmoke);
			this.gb_Block.Controls.Add(this.tb36_isblockfire);
			this.gb_Block.Controls.Add(this.tb31_isblocklos);
			this.gb_Block.Location = new System.Drawing.Point(175, 85);
			this.gb_Block.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Block.Name = "gb_Block";
			this.gb_Block.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Block.Size = new System.Drawing.Size(165, 135);
			this.gb_Block.TabIndex = 5;
			this.gb_Block.TabStop = false;
			this.gb_Block.Text = " Block ";
			// 
			// tb51_lightblock
			// 
			this.tb51_lightblock.Location = new System.Drawing.Point(125, 113);
			this.tb51_lightblock.Margin = new System.Windows.Forms.Padding(0);
			this.tb51_lightblock.Name = "tb51_lightblock";
			this.tb51_lightblock.Size = new System.Drawing.Size(35, 19);
			this.tb51_lightblock.TabIndex = 17;
			this.tb51_lightblock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb51_lightblock.WordWrap = false;
			this.tb51_lightblock.TextChanged += new System.EventHandler(this.OnChanged51);
			this.tb51_lightblock.Enter += new System.EventHandler(this.OnEnter51);
			this.tb51_lightblock.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox51);
			// 
			// tb56_smokeblock
			// 
			this.tb56_smokeblock.Location = new System.Drawing.Point(125, 93);
			this.tb56_smokeblock.Margin = new System.Windows.Forms.Padding(0);
			this.tb56_smokeblock.Name = "tb56_smokeblock";
			this.tb56_smokeblock.Size = new System.Drawing.Size(35, 19);
			this.tb56_smokeblock.TabIndex = 14;
			this.tb56_smokeblock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb56_smokeblock.WordWrap = false;
			this.tb56_smokeblock.TextChanged += new System.EventHandler(this.OnChanged56);
			this.tb56_smokeblock.Enter += new System.EventHandler(this.OnEnter56);
			this.tb56_smokeblock.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox56);
			// 
			// tb43_heblock
			// 
			this.tb43_heblock.Location = new System.Drawing.Point(125, 73);
			this.tb43_heblock.Margin = new System.Windows.Forms.Padding(0);
			this.tb43_heblock.Name = "tb43_heblock";
			this.tb43_heblock.Size = new System.Drawing.Size(35, 19);
			this.tb43_heblock.TabIndex = 11;
			this.tb43_heblock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb43_heblock.WordWrap = false;
			this.tb43_heblock.TextChanged += new System.EventHandler(this.OnChanged43);
			this.tb43_heblock.Enter += new System.EventHandler(this.OnEnter43);
			this.tb43_heblock.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox43);
			// 
			// tb37_isblocksmoke
			// 
			this.tb37_isblocksmoke.Location = new System.Drawing.Point(125, 53);
			this.tb37_isblocksmoke.Margin = new System.Windows.Forms.Padding(0);
			this.tb37_isblocksmoke.Name = "tb37_isblocksmoke";
			this.tb37_isblocksmoke.Size = new System.Drawing.Size(35, 19);
			this.tb37_isblocksmoke.TabIndex = 8;
			this.tb37_isblocksmoke.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb37_isblocksmoke.WordWrap = false;
			this.tb37_isblocksmoke.TextChanged += new System.EventHandler(this.OnChanged37);
			this.tb37_isblocksmoke.Enter += new System.EventHandler(this.OnEnter37);
			this.tb37_isblocksmoke.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox37);
			// 
			// tb36_isblockfire
			// 
			this.tb36_isblockfire.Location = new System.Drawing.Point(125, 33);
			this.tb36_isblockfire.Margin = new System.Windows.Forms.Padding(0);
			this.tb36_isblockfire.Name = "tb36_isblockfire";
			this.tb36_isblockfire.Size = new System.Drawing.Size(35, 19);
			this.tb36_isblockfire.TabIndex = 5;
			this.tb36_isblockfire.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb36_isblockfire.WordWrap = false;
			this.tb36_isblockfire.TextChanged += new System.EventHandler(this.OnChanged36);
			this.tb36_isblockfire.Enter += new System.EventHandler(this.OnEnter36);
			this.tb36_isblockfire.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox36);
			// 
			// tb31_isblocklos
			// 
			this.tb31_isblocklos.Location = new System.Drawing.Point(125, 13);
			this.tb31_isblocklos.Margin = new System.Windows.Forms.Padding(0);
			this.tb31_isblocklos.Name = "tb31_isblocklos";
			this.tb31_isblocklos.Size = new System.Drawing.Size(35, 19);
			this.tb31_isblocklos.TabIndex = 2;
			this.tb31_isblocklos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb31_isblocklos.WordWrap = false;
			this.tb31_isblocklos.TextChanged += new System.EventHandler(this.OnChanged31);
			this.tb31_isblocklos.Enter += new System.EventHandler(this.OnEnter31);
			this.tb31_isblocklos.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox31);
			// 
			// gb_Door
			// 
			this.gb_Door.Controls.Add(this.lbl30);
			this.gb_Door.Controls.Add(this.lbl35);
			this.gb_Door.Controls.Add(this.lbl46);
			this.gb_Door.Controls.Add(this.lbl30_isslidingdoor);
			this.gb_Door.Controls.Add(this.lbl35_ishingeddoor);
			this.gb_Door.Controls.Add(this.lbl46_alternateid);
			this.gb_Door.Controls.Add(this.tb46_alternateid);
			this.gb_Door.Controls.Add(this.tb35_ishingeddoor);
			this.gb_Door.Controls.Add(this.tb30_isslidingdoor);
			this.gb_Door.Location = new System.Drawing.Point(5, 265);
			this.gb_Door.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Door.Name = "gb_Door";
			this.gb_Door.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Door.Size = new System.Drawing.Size(165, 75);
			this.gb_Door.TabIndex = 3;
			this.gb_Door.TabStop = false;
			this.gb_Door.Text = " Door ";
			// 
			// tb46_alternateid
			// 
			this.tb46_alternateid.Location = new System.Drawing.Point(125, 53);
			this.tb46_alternateid.Margin = new System.Windows.Forms.Padding(0);
			this.tb46_alternateid.Name = "tb46_alternateid";
			this.tb46_alternateid.Size = new System.Drawing.Size(35, 19);
			this.tb46_alternateid.TabIndex = 8;
			this.tb46_alternateid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb46_alternateid.WordWrap = false;
			this.tb46_alternateid.TextChanged += new System.EventHandler(this.OnChanged46);
			this.tb46_alternateid.Enter += new System.EventHandler(this.OnEnter46);
			this.tb46_alternateid.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox46);
			// 
			// tb35_ishingeddoor
			// 
			this.tb35_ishingeddoor.Location = new System.Drawing.Point(125, 33);
			this.tb35_ishingeddoor.Margin = new System.Windows.Forms.Padding(0);
			this.tb35_ishingeddoor.Name = "tb35_ishingeddoor";
			this.tb35_ishingeddoor.Size = new System.Drawing.Size(35, 19);
			this.tb35_ishingeddoor.TabIndex = 5;
			this.tb35_ishingeddoor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb35_ishingeddoor.WordWrap = false;
			this.tb35_ishingeddoor.TextChanged += new System.EventHandler(this.OnChanged35);
			this.tb35_ishingeddoor.Enter += new System.EventHandler(this.OnEnter35);
			this.tb35_ishingeddoor.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox35);
			// 
			// tb30_isslidingdoor
			// 
			this.tb30_isslidingdoor.Location = new System.Drawing.Point(125, 13);
			this.tb30_isslidingdoor.Margin = new System.Windows.Forms.Padding(0);
			this.tb30_isslidingdoor.Name = "tb30_isslidingdoor";
			this.tb30_isslidingdoor.Size = new System.Drawing.Size(35, 19);
			this.tb30_isslidingdoor.TabIndex = 2;
			this.tb30_isslidingdoor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb30_isslidingdoor.WordWrap = false;
			this.tb30_isslidingdoor.TextChanged += new System.EventHandler(this.OnChanged30);
			this.tb30_isslidingdoor.Enter += new System.EventHandler(this.OnEnter30);
			this.tb30_isslidingdoor.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox30);
			// 
			// gb_Step
			// 
			this.gb_Step.Controls.Add(this.lbl32);
			this.gb_Step.Controls.Add(this.lbl52);
			this.gb_Step.Controls.Add(this.lbl52_footsound);
			this.gb_Step.Controls.Add(this.lbl32_isdropthrou);
			this.gb_Step.Controls.Add(this.tb32_isdropthrou);
			this.gb_Step.Controls.Add(this.tb52_footsound);
			this.gb_Step.Location = new System.Drawing.Point(175, 225);
			this.gb_Step.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Step.Name = "gb_Step";
			this.gb_Step.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Step.Size = new System.Drawing.Size(165, 55);
			this.gb_Step.TabIndex = 6;
			this.gb_Step.TabStop = false;
			this.gb_Step.Text = " Step ";
			// 
			// tb32_isdropthrou
			// 
			this.tb32_isdropthrou.Location = new System.Drawing.Point(125, 33);
			this.tb32_isdropthrou.Margin = new System.Windows.Forms.Padding(0);
			this.tb32_isdropthrou.Name = "tb32_isdropthrou";
			this.tb32_isdropthrou.Size = new System.Drawing.Size(35, 19);
			this.tb32_isdropthrou.TabIndex = 5;
			this.tb32_isdropthrou.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb32_isdropthrou.WordWrap = false;
			this.tb32_isdropthrou.TextChanged += new System.EventHandler(this.OnChanged32);
			this.tb32_isdropthrou.Enter += new System.EventHandler(this.OnEnter32);
			this.tb32_isdropthrou.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox32);
			// 
			// tb52_footsound
			// 
			this.tb52_footsound.Location = new System.Drawing.Point(125, 13);
			this.tb52_footsound.Margin = new System.Windows.Forms.Padding(0);
			this.tb52_footsound.Name = "tb52_footsound";
			this.tb52_footsound.Size = new System.Drawing.Size(35, 19);
			this.tb52_footsound.TabIndex = 2;
			this.tb52_footsound.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb52_footsound.WordWrap = false;
			this.tb52_footsound.TextChanged += new System.EventHandler(this.OnChanged52);
			this.tb52_footsound.Enter += new System.EventHandler(this.OnEnter52);
			this.tb52_footsound.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox52);
			// 
			// gb_Explode
			// 
			this.gb_Explode.Controls.Add(this.lbl54);
			this.gb_Explode.Controls.Add(this.lbl55);
			this.gb_Explode.Controls.Add(this.lbl45);
			this.gb_Explode.Controls.Add(this.lbl57);
			this.gb_Explode.Controls.Add(this.lbl54_hetype);
			this.gb_Explode.Controls.Add(this.lbl55_hestrength);
			this.gb_Explode.Controls.Add(this.lbl45_fireresist);
			this.gb_Explode.Controls.Add(this.lbl57_fuel);
			this.gb_Explode.Controls.Add(this.tb57_fuel);
			this.gb_Explode.Controls.Add(this.tb45_fireresist);
			this.gb_Explode.Controls.Add(this.tb55_hestrength);
			this.gb_Explode.Controls.Add(this.tb54_hetype);
			this.gb_Explode.Location = new System.Drawing.Point(345, 5);
			this.gb_Explode.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Explode.Name = "gb_Explode";
			this.gb_Explode.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Explode.Size = new System.Drawing.Size(165, 95);
			this.gb_Explode.TabIndex = 8;
			this.gb_Explode.TabStop = false;
			this.gb_Explode.Text = " Explode ";
			// 
			// tb57_fuel
			// 
			this.tb57_fuel.Location = new System.Drawing.Point(125, 73);
			this.tb57_fuel.Margin = new System.Windows.Forms.Padding(0);
			this.tb57_fuel.Name = "tb57_fuel";
			this.tb57_fuel.Size = new System.Drawing.Size(35, 19);
			this.tb57_fuel.TabIndex = 11;
			this.tb57_fuel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb57_fuel.WordWrap = false;
			this.tb57_fuel.TextChanged += new System.EventHandler(this.OnChanged57);
			this.tb57_fuel.Enter += new System.EventHandler(this.OnEnter57);
			this.tb57_fuel.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox57);
			// 
			// tb45_fireresist
			// 
			this.tb45_fireresist.Location = new System.Drawing.Point(125, 53);
			this.tb45_fireresist.Margin = new System.Windows.Forms.Padding(0);
			this.tb45_fireresist.Name = "tb45_fireresist";
			this.tb45_fireresist.Size = new System.Drawing.Size(35, 19);
			this.tb45_fireresist.TabIndex = 8;
			this.tb45_fireresist.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb45_fireresist.WordWrap = false;
			this.tb45_fireresist.TextChanged += new System.EventHandler(this.OnChanged45);
			this.tb45_fireresist.Enter += new System.EventHandler(this.OnEnter45);
			this.tb45_fireresist.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox45);
			// 
			// tb55_hestrength
			// 
			this.tb55_hestrength.Location = new System.Drawing.Point(125, 33);
			this.tb55_hestrength.Margin = new System.Windows.Forms.Padding(0);
			this.tb55_hestrength.Name = "tb55_hestrength";
			this.tb55_hestrength.Size = new System.Drawing.Size(35, 19);
			this.tb55_hestrength.TabIndex = 5;
			this.tb55_hestrength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb55_hestrength.WordWrap = false;
			this.tb55_hestrength.TextChanged += new System.EventHandler(this.OnChanged55);
			this.tb55_hestrength.Enter += new System.EventHandler(this.OnEnter55);
			this.tb55_hestrength.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox55);
			// 
			// tb54_hetype
			// 
			this.tb54_hetype.Location = new System.Drawing.Point(125, 13);
			this.tb54_hetype.Margin = new System.Windows.Forms.Padding(0);
			this.tb54_hetype.Name = "tb54_hetype";
			this.tb54_hetype.Size = new System.Drawing.Size(35, 19);
			this.tb54_hetype.TabIndex = 2;
			this.tb54_hetype.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb54_hetype.WordWrap = false;
			this.tb54_hetype.TextChanged += new System.EventHandler(this.OnChanged54);
			this.tb54_hetype.Enter += new System.EventHandler(this.OnEnter54);
			this.tb54_hetype.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox54);
			// 
			// gb_Health
			// 
			this.gb_Health.Controls.Add(this.lbl42);
			this.gb_Health.Controls.Add(this.lbl44);
			this.gb_Health.Controls.Add(this.lbl42_armor);
			this.gb_Health.Controls.Add(this.lbl44_deathid);
			this.gb_Health.Controls.Add(this.tb44_deathid);
			this.gb_Health.Controls.Add(this.tb42_armor);
			this.gb_Health.Location = new System.Drawing.Point(5, 205);
			this.gb_Health.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Health.Name = "gb_Health";
			this.gb_Health.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Health.Size = new System.Drawing.Size(165, 55);
			this.gb_Health.TabIndex = 2;
			this.gb_Health.TabStop = false;
			this.gb_Health.Text = " Health ";
			// 
			// tb44_deathid
			// 
			this.tb44_deathid.Location = new System.Drawing.Point(125, 33);
			this.tb44_deathid.Margin = new System.Windows.Forms.Padding(0);
			this.tb44_deathid.Name = "tb44_deathid";
			this.tb44_deathid.Size = new System.Drawing.Size(35, 19);
			this.tb44_deathid.TabIndex = 5;
			this.tb44_deathid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb44_deathid.WordWrap = false;
			this.tb44_deathid.TextChanged += new System.EventHandler(this.OnChanged44);
			this.tb44_deathid.Enter += new System.EventHandler(this.OnEnter44);
			this.tb44_deathid.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox44);
			// 
			// tb42_armor
			// 
			this.tb42_armor.Location = new System.Drawing.Point(125, 13);
			this.tb42_armor.Margin = new System.Windows.Forms.Padding(0);
			this.tb42_armor.Name = "tb42_armor";
			this.tb42_armor.Size = new System.Drawing.Size(35, 19);
			this.tb42_armor.TabIndex = 2;
			this.tb42_armor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb42_armor.WordWrap = false;
			this.tb42_armor.TextChanged += new System.EventHandler(this.OnChanged42);
			this.tb42_armor.Enter += new System.EventHandler(this.OnEnter42);
			this.tb42_armor.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox42);
			// 
			// gb_General
			// 
			this.gb_General.Controls.Add(this.lbl60);
			this.gb_General.Controls.Add(this.lbl53);
			this.gb_General.Controls.Add(this.lbl34);
			this.gb_General.Controls.Add(this.lbl59);
			this.gb_General.Controls.Add(this.lbl33);
			this.gb_General.Controls.Add(this.lbl58);
			this.gb_General.Controls.Add(this.lbl33_isbigwall);
			this.gb_General.Controls.Add(this.lbl60_isbaseobject);
			this.gb_General.Controls.Add(this.lbl53_parttype);
			this.gb_General.Controls.Add(this.lbl59_specialtype);
			this.gb_General.Controls.Add(this.lbl34_isgravlift);
			this.gb_General.Controls.Add(this.lbl58_lightintensity);
			this.gb_General.Controls.Add(this.tb58_lightintensity);
			this.gb_General.Controls.Add(this.tb60_isbaseobject);
			this.gb_General.Controls.Add(this.tb59_specialtype);
			this.gb_General.Controls.Add(this.tb53_parttype);
			this.gb_General.Controls.Add(this.tb34_isgravlift);
			this.gb_General.Controls.Add(this.tb33_isbigwall);
			this.gb_General.Location = new System.Drawing.Point(5, 65);
			this.gb_General.Margin = new System.Windows.Forms.Padding(0);
			this.gb_General.Name = "gb_General";
			this.gb_General.Padding = new System.Windows.Forms.Padding(0);
			this.gb_General.Size = new System.Drawing.Size(165, 135);
			this.gb_General.TabIndex = 1;
			this.gb_General.TabStop = false;
			this.gb_General.Text = " General ";
			// 
			// tb58_lightintensity
			// 
			this.tb58_lightintensity.Location = new System.Drawing.Point(125, 113);
			this.tb58_lightintensity.Margin = new System.Windows.Forms.Padding(0);
			this.tb58_lightintensity.Name = "tb58_lightintensity";
			this.tb58_lightintensity.Size = new System.Drawing.Size(35, 19);
			this.tb58_lightintensity.TabIndex = 17;
			this.tb58_lightintensity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb58_lightintensity.WordWrap = false;
			this.tb58_lightintensity.TextChanged += new System.EventHandler(this.OnChanged58);
			this.tb58_lightintensity.Enter += new System.EventHandler(this.OnEnter58);
			this.tb58_lightintensity.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox58);
			// 
			// tb60_isbaseobject
			// 
			this.tb60_isbaseobject.Location = new System.Drawing.Point(125, 93);
			this.tb60_isbaseobject.Margin = new System.Windows.Forms.Padding(0);
			this.tb60_isbaseobject.Name = "tb60_isbaseobject";
			this.tb60_isbaseobject.Size = new System.Drawing.Size(35, 19);
			this.tb60_isbaseobject.TabIndex = 14;
			this.tb60_isbaseobject.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb60_isbaseobject.WordWrap = false;
			this.tb60_isbaseobject.TextChanged += new System.EventHandler(this.OnChanged60);
			this.tb60_isbaseobject.Enter += new System.EventHandler(this.OnEnter60);
			this.tb60_isbaseobject.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox60);
			// 
			// tb59_specialtype
			// 
			this.tb59_specialtype.Location = new System.Drawing.Point(125, 33);
			this.tb59_specialtype.Margin = new System.Windows.Forms.Padding(0);
			this.tb59_specialtype.Name = "tb59_specialtype";
			this.tb59_specialtype.Size = new System.Drawing.Size(35, 19);
			this.tb59_specialtype.TabIndex = 5;
			this.tb59_specialtype.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb59_specialtype.WordWrap = false;
			this.tb59_specialtype.TextChanged += new System.EventHandler(this.OnChanged59);
			this.tb59_specialtype.Enter += new System.EventHandler(this.OnEnter59);
			this.tb59_specialtype.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox59);
			// 
			// tb53_parttype
			// 
			this.tb53_parttype.Location = new System.Drawing.Point(125, 13);
			this.tb53_parttype.Margin = new System.Windows.Forms.Padding(0);
			this.tb53_parttype.Name = "tb53_parttype";
			this.tb53_parttype.Size = new System.Drawing.Size(35, 19);
			this.tb53_parttype.TabIndex = 2;
			this.tb53_parttype.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb53_parttype.WordWrap = false;
			this.tb53_parttype.TextChanged += new System.EventHandler(this.OnChanged53);
			this.tb53_parttype.Enter += new System.EventHandler(this.OnEnter53);
			this.tb53_parttype.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox53);
			// 
			// tb34_isgravlift
			// 
			this.tb34_isgravlift.Location = new System.Drawing.Point(125, 73);
			this.tb34_isgravlift.Margin = new System.Windows.Forms.Padding(0);
			this.tb34_isgravlift.Name = "tb34_isgravlift";
			this.tb34_isgravlift.Size = new System.Drawing.Size(35, 19);
			this.tb34_isgravlift.TabIndex = 11;
			this.tb34_isgravlift.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb34_isgravlift.WordWrap = false;
			this.tb34_isgravlift.TextChanged += new System.EventHandler(this.OnChanged34);
			this.tb34_isgravlift.Enter += new System.EventHandler(this.OnEnter34);
			this.tb34_isgravlift.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox34);
			// 
			// tb33_isbigwall
			// 
			this.tb33_isbigwall.Location = new System.Drawing.Point(125, 53);
			this.tb33_isbigwall.Margin = new System.Windows.Forms.Padding(0);
			this.tb33_isbigwall.Name = "tb33_isbigwall";
			this.tb33_isbigwall.Size = new System.Drawing.Size(35, 19);
			this.tb33_isbigwall.TabIndex = 8;
			this.tb33_isbigwall.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb33_isbigwall.WordWrap = false;
			this.tb33_isbigwall.TextChanged += new System.EventHandler(this.OnChanged33);
			this.tb33_isbigwall.Enter += new System.EventHandler(this.OnEnter33);
			this.tb33_isbigwall.MouseEnter += new System.EventHandler(this.OnMouseEnterTextbox33);
			// 
			// lbl_SpriteShade
			// 
			this.lbl_SpriteShade.Location = new System.Drawing.Point(85, 345);
			this.lbl_SpriteShade.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_SpriteShade.Name = "lbl_SpriteShade";
			this.lbl_SpriteShade.Size = new System.Drawing.Size(75, 15);
			this.lbl_SpriteShade.TabIndex = 15;
			this.lbl_SpriteShade.Text = "SpriteShade";
			this.lbl_SpriteShade.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl_SpriteShade.Click += new System.EventHandler(this.OnClick_FocusShade);
			this.lbl_SpriteShade.MouseEnter += new System.EventHandler(this.OnEnterSpriteShade);
			this.lbl_SpriteShade.MouseLeave += new System.EventHandler(this.OnMouseLeaveSpriteShade);
			// 
			// tb_SpriteShade
			// 
			this.tb_SpriteShade.Location = new System.Drawing.Point(160, 343);
			this.tb_SpriteShade.Margin = new System.Windows.Forms.Padding(0);
			this.tb_SpriteShade.Name = "tb_SpriteShade";
			this.tb_SpriteShade.Size = new System.Drawing.Size(35, 19);
			this.tb_SpriteShade.TabIndex = 16;
			this.tb_SpriteShade.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_SpriteShade.WordWrap = false;
			this.tb_SpriteShade.TextChanged += new System.EventHandler(this.OnTextChanged_SpriteShade);
			this.tb_SpriteShade.Enter += new System.EventHandler(this.OnEnterSpriteShade);
			this.tb_SpriteShade.Leave += new System.EventHandler(this.OnLeave);
			this.tb_SpriteShade.MouseEnter += new System.EventHandler(this.OnEnterSpriteShade);
			this.tb_SpriteShade.MouseLeave += new System.EventHandler(this.OnMouseLeaveSpriteShade);
			// 
			// gb_Description
			// 
			this.gb_Description.Controls.Add(this.lbl_Description);
			this.gb_Description.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gb_Description.Location = new System.Drawing.Point(0, 365);
			this.gb_Description.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Description.Name = "gb_Description";
			this.gb_Description.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Description.Size = new System.Drawing.Size(672, 92);
			this.gb_Description.TabIndex = 18;
			this.gb_Description.TabStop = false;
			this.gb_Description.Text = " description ";
			// 
			// lbl_Description
			// 
			this.lbl_Description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl_Description.Location = new System.Drawing.Point(5, 15);
			this.lbl_Description.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Description.Name = "lbl_Description";
			this.lbl_Description.Size = new System.Drawing.Size(662, 71);
			this.lbl_Description.TabIndex = 0;
			this.lbl_Description.Click += new System.EventHandler(this.OnClick_FocusCollection);
			// 
			// cb_Strict
			// 
			this.cb_Strict.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
			this.cb_Strict.Checked = true;
			this.cb_Strict.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cb_Strict.Location = new System.Drawing.Point(60, 345);
			this.cb_Strict.Margin = new System.Windows.Forms.Padding(0);
			this.cb_Strict.Name = "cb_Strict";
			this.cb_Strict.Size = new System.Drawing.Size(15, 15);
			this.cb_Strict.TabIndex = 14;
			this.cb_Strict.UseVisualStyleBackColor = true;
			this.cb_Strict.CheckedChanged += new System.EventHandler(this.OnCheckChanged_Strict);
			this.cb_Strict.Enter += new System.EventHandler(this.OnEnterStrict);
			this.cb_Strict.Leave += new System.EventHandler(this.OnLeave);
			this.cb_Strict.MouseEnter += new System.EventHandler(this.OnEnterStrict);
			this.cb_Strict.MouseLeave += new System.EventHandler(this.OnMouseLeaveStrict);
			// 
			// lbl_Strict
			// 
			this.lbl_Strict.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_Strict.Location = new System.Drawing.Point(10, 345);
			this.lbl_Strict.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Strict.Name = "lbl_Strict";
			this.lbl_Strict.Size = new System.Drawing.Size(50, 15);
			this.lbl_Strict.TabIndex = 13;
			this.lbl_Strict.Text = "STRICT";
			this.lbl_Strict.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl_Strict.Click += new System.EventHandler(this.OnClick_FocusStrict);
			this.lbl_Strict.MouseEnter += new System.EventHandler(this.OnEnterStrict);
			this.lbl_Strict.MouseLeave += new System.EventHandler(this.OnMouseLeaveStrict);
			// 
			// bar_SpriteShade
			// 
			this.bar_SpriteShade.AutoSize = false;
			this.bar_SpriteShade.Location = new System.Drawing.Point(195, 345);
			this.bar_SpriteShade.Margin = new System.Windows.Forms.Padding(0);
			this.bar_SpriteShade.Maximum = 100;
			this.bar_SpriteShade.Name = "bar_SpriteShade";
			this.bar_SpriteShade.Size = new System.Drawing.Size(140, 16);
			this.bar_SpriteShade.TabIndex = 17;
			this.bar_SpriteShade.TickStyle = System.Windows.Forms.TickStyle.None;
			this.bar_SpriteShade.ValueChanged += new System.EventHandler(this.OnValueChanged_SpriteShade);
			// 
			// pnl_IsoLoft
			// 
			this.pnl_IsoLoft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pnl_IsoLoft.Location = new System.Drawing.Point(535, 15);
			this.pnl_IsoLoft.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_IsoLoft.Name = "pnl_IsoLoft";
			this.pnl_IsoLoft.Size = new System.Drawing.Size(130, 195);
			this.pnl_IsoLoft.TabIndex = 11;
			this.pnl_IsoLoft.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint_IsoLoft);
			this.pnl_IsoLoft.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick_IsoLoft);
			// 
			// bar_IsoLoft
			// 
			this.bar_IsoLoft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bar_IsoLoft.AutoSize = false;
			this.bar_IsoLoft.LargeChange = 6;
			this.bar_IsoLoft.Location = new System.Drawing.Point(520, 10);
			this.bar_IsoLoft.Margin = new System.Windows.Forms.Padding(0);
			this.bar_IsoLoft.Maximum = 24;
			this.bar_IsoLoft.Name = "bar_IsoLoft";
			this.bar_IsoLoft.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.bar_IsoLoft.Size = new System.Drawing.Size(16, 205);
			this.bar_IsoLoft.TabIndex = 12;
			this.bar_IsoLoft.TickStyle = System.Windows.Forms.TickStyle.None;
			this.bar_IsoLoft.Value = 24;
			this.bar_IsoLoft.ValueChanged += new System.EventHandler(this.OnValueChanged_IsoLoft);
			this.bar_IsoLoft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp_BarIsoLoft);
			// 
			// pnl_bg
			// 
			this.pnl_bg.Controls.Add(this.lbl_Strict);
			this.pnl_bg.Controls.Add(this.cb_Strict);
			this.pnl_bg.Controls.Add(this.lbl_SpriteShade);
			this.pnl_bg.Controls.Add(this.tb_SpriteShade);
			this.pnl_bg.Controls.Add(this.bar_SpriteShade);
			this.pnl_bg.Controls.Add(this.gb_Description);
			this.pnl_bg.Controls.Add(this.gb_Loft);
			this.pnl_bg.Controls.Add(this.gb_Overhead);
			this.pnl_bg.Controls.Add(this.gb_Tu);
			this.pnl_bg.Controls.Add(this.gb_Explode);
			this.pnl_bg.Controls.Add(this.gb_Block);
			this.pnl_bg.Controls.Add(this.gb_General);
			this.pnl_bg.Controls.Add(this.gb_Unused);
			this.pnl_bg.Controls.Add(this.gb_Elevation);
			this.pnl_bg.Controls.Add(this.gb_Door);
			this.pnl_bg.Controls.Add(this.gb_Health);
			this.pnl_bg.Controls.Add(this.gb_Step);
			this.pnl_bg.Controls.Add(this.pnl_IsoLoft);
			this.pnl_bg.Controls.Add(this.bar_IsoLoft);
			this.pnl_bg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl_bg.Location = new System.Drawing.Point(0, 315);
			this.pnl_bg.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_bg.Name = "pnl_bg";
			this.pnl_bg.Size = new System.Drawing.Size(832, 457);
			this.pnl_bg.TabIndex = 2;
			this.pnl_bg.Click += new System.EventHandler(this.OnClick_FocusCollection);
			// 
			// McdviewF
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(832, 794);
			this.Controls.Add(this.pnl_bg);
			this.Controls.Add(this.ss_Statusbar);
			this.Controls.Add(this.gb_Sprites);
			this.Controls.Add(this.gb_Collection);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximumSize = new System.Drawing.Size(840, 820);
			this.Menu = this.mmMainMenu;
			this.Name = "McdviewF";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "McdView";
			this.ss_Statusbar.ResumeLayout(false);
			this.ss_Statusbar.PerformLayout();
			this.gb_Unused.ResumeLayout(false);
			this.gb_Unused.PerformLayout();
			this.gb_Loft.ResumeLayout(false);
			this.gb_Loft.PerformLayout();
			this.gb_Sprites.ResumeLayout(false);
			this.gb_Sprites.PerformLayout();
			this.gb_Overhead.ResumeLayout(false);
			this.gb_Overhead.PerformLayout();
			this.gb_Tu.ResumeLayout(false);
			this.gb_Tu.PerformLayout();
			this.gb_Elevation.ResumeLayout(false);
			this.gb_Elevation.PerformLayout();
			this.gb_Block.ResumeLayout(false);
			this.gb_Block.PerformLayout();
			this.gb_Door.ResumeLayout(false);
			this.gb_Door.PerformLayout();
			this.gb_Step.ResumeLayout(false);
			this.gb_Step.PerformLayout();
			this.gb_Explode.ResumeLayout(false);
			this.gb_Explode.PerformLayout();
			this.gb_Health.ResumeLayout(false);
			this.gb_Health.PerformLayout();
			this.gb_General.ResumeLayout(false);
			this.gb_General.PerformLayout();
			this.gb_Description.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.bar_SpriteShade)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bar_IsoLoft)).EndInit();
			this.pnl_bg.ResumeLayout(false);
			this.pnl_bg.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
