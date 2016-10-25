namespace fracture
{
    partial class frmwellevl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmwellevl));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.spreadsheetControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btncal = new DevExpress.XtraBars.BarButtonItem();
            this.btnstart = new DevExpress.XtraBars.BarButtonItem();
            this.btnend = new DevExpress.XtraBars.BarButtonItem();
            this.btnplay = new DevExpress.XtraBars.BarButtonItem();
            this.btnfresh = new DevExpress.XtraBars.BarButtonItem();
            this.btnsavegif = new DevExpress.XtraBars.BarButtonItem();
            this.btnoutpic = new DevExpress.XtraBars.BarButtonItem();
            this.timeselect = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemMRUEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMRUEdit();
            this.ribbonPageCategory1 = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
            this.fileRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.FileRibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemComboBox2 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.chartControl1 = new DevExpress.XtraCharts.ChartControl();
            this.spreadsheetBarController1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetBarController();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMRUEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spreadsheetBarController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 144);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.spreadsheetControl1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.chartControl1);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(627, 326);
            this.splitContainerControl1.SplitterPosition = 269;
            this.splitContainerControl1.TabIndex = 2;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // spreadsheetControl1
            // 
            this.spreadsheetControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetControl1.Location = new System.Drawing.Point(0, 0);
            this.spreadsheetControl1.MenuManager = this.ribbonControl1;
            this.spreadsheetControl1.Name = "spreadsheetControl1";
            this.spreadsheetControl1.Size = new System.Drawing.Size(269, 326);
            this.spreadsheetControl1.TabIndex = 1;
            this.spreadsheetControl1.Text = "spreadsheetControl1";
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.btncal,
            this.btnstart,
            this.btnend,
            this.btnplay,
            this.btnfresh,
            this.btnsavegif,
            this.btnoutpic,
            this.timeselect});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 23;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.PageCategories.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
            this.ribbonPageCategory1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemComboBox1,
            this.repositoryItemComboBox2,
            this.repositoryItemMRUEdit1});
            this.ribbonControl1.Size = new System.Drawing.Size(827, 144);
            // 
            // btncal
            // 
            this.btncal.Caption = "计算";
            this.btncal.Glyph = ((System.Drawing.Image)(resources.GetObject("btncal.Glyph")));
            this.btncal.Id = 10;
            this.btncal.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("btncal.LargeGlyph")));
            this.btncal.Name = "btncal";
            this.btncal.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btncal_ItemClick);
            // 
            // btnstart
            // 
            this.btnstart.Caption = "开头";
            this.btnstart.Glyph = ((System.Drawing.Image)(resources.GetObject("btnstart.Glyph")));
            this.btnstart.Id = 11;
            this.btnstart.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("btnstart.LargeGlyph")));
            this.btnstart.Name = "btnstart";
            this.btnstart.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnstart_ItemClick);
            // 
            // btnend
            // 
            this.btnend.Caption = "结尾";
            this.btnend.Glyph = ((System.Drawing.Image)(resources.GetObject("btnend.Glyph")));
            this.btnend.Id = 12;
            this.btnend.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("btnend.LargeGlyph")));
            this.btnend.Name = "btnend";
            this.btnend.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnend_ItemClick);
            // 
            // btnplay
            // 
            this.btnplay.Caption = "播放";
            this.btnplay.Glyph = ((System.Drawing.Image)(resources.GetObject("btnplay.Glyph")));
            this.btnplay.Id = 13;
            this.btnplay.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("btnplay.LargeGlyph")));
            this.btnplay.Name = "btnplay";
            this.btnplay.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnplay_ItemClick);
            // 
            // btnfresh
            // 
            this.btnfresh.Caption = "刷新";
            this.btnfresh.Glyph = ((System.Drawing.Image)(resources.GetObject("btnfresh.Glyph")));
            this.btnfresh.Id = 18;
            this.btnfresh.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("btnfresh.LargeGlyph")));
            this.btnfresh.Name = "btnfresh";
            this.btnfresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnfresh_ItemClick);
            // 
            // btnsavegif
            // 
            this.btnsavegif.Caption = "保存GIF";
            this.btnsavegif.Glyph = ((System.Drawing.Image)(resources.GetObject("btnsavegif.Glyph")));
            this.btnsavegif.Id = 19;
            this.btnsavegif.LargeGlyph = global::fracture.Properties.Resources.存为GIF;
            this.btnsavegif.Name = "btnsavegif";
            this.btnsavegif.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnsavegif_ItemClick);
            // 
            // btnoutpic
            // 
            this.btnoutpic.Caption = "保存单幅图";
            this.btnoutpic.Id = 20;
            this.btnoutpic.LargeGlyph = global::fracture.Properties.Resources.存为图片位图_01;
            this.btnoutpic.Name = "btnoutpic";
            this.btnoutpic.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // timeselect
            // 
            this.timeselect.Caption = "筛选时间";
            this.timeselect.Edit = this.repositoryItemMRUEdit1;
            this.timeselect.Glyph = ((System.Drawing.Image)(resources.GetObject("timeselect.Glyph")));
            this.timeselect.Id = 21;
            this.timeselect.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("timeselect.LargeGlyph")));
            this.timeselect.Name = "timeselect";
            this.timeselect.RibbonStyle = ((DevExpress.XtraBars.Ribbon.RibbonItemStyles)(((DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText) 
            | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText)));
            this.timeselect.Width = 100;
            this.timeselect.EditValueChanged += new System.EventHandler(this.timeselect_EditValueChanged);
            this.timeselect.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.timeselect_ItemClick);
            // 
            // repositoryItemMRUEdit1
            // 
            this.repositoryItemMRUEdit1.AutoHeight = false;
            this.repositoryItemMRUEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemMRUEdit1.Name = "repositoryItemMRUEdit1";
            // 
            // ribbonPageCategory1
            // 
            this.ribbonPageCategory1.Name = "ribbonPageCategory1";
            this.ribbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.fileRibbonPage1});
            this.ribbonPageCategory1.Text = "操作";
            // 
            // fileRibbonPage1
            // 
            this.fileRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.fileRibbonPage1.Name = "fileRibbonPage1";
            this.fileRibbonPage1.Text = "操作";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btncal);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnstart);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnplay);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnend);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnfresh);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnsavegif);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnoutpic);
            this.ribbonPageGroup1.ItemLinks.Add(this.timeselect);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "计算";
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            this.repositoryItemComboBox1.Sorted = true;
            this.repositoryItemComboBox1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // repositoryItemComboBox2
            // 
            this.repositoryItemComboBox2.AutoHeight = false;
            this.repositoryItemComboBox2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox2.Name = "repositoryItemComboBox2";
            // 
            // chartControl1
            // 
            this.chartControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.chartControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl1.Location = new System.Drawing.Point(0, 0);
            this.chartControl1.Name = "chartControl1";
            this.chartControl1.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartControl1.Size = new System.Drawing.Size(346, 326);
            this.chartControl1.TabIndex = 0;
            this.chartControl1.CustomDrawSeriesPoint += new DevExpress.XtraCharts.CustomDrawSeriesPointEventHandler(this.chartControl1_CustomDrawSeriesPoint);
            // 
            // spreadsheetBarController1
            // 
            this.spreadsheetBarController1.Control = this.spreadsheetControl1;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel1});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Navigation.OfficeNavigationBar",
            "DevExpress.XtraBars.Navigation.TileNavPane"});
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add(this.dockPanel1_Container);
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dockPanel1.ID = new System.Guid("48438b6d-252e-4781-9d33-3314be389882");
            this.dockPanel1.Location = new System.Drawing.Point(627, 144);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockPanel1.Size = new System.Drawing.Size(200, 326);
            this.dockPanel1.Text = "井列表";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 39);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(192, 283);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // frmwellevl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 470);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "frmwellevl";
            this.Text = "Form7";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmwellevl_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMRUEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spreadsheetBarController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraSpreadsheet.SpreadsheetControl spreadsheetControl1;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem btncal;
        private DevExpress.XtraBars.BarButtonItem btnstart;
        private DevExpress.XtraBars.BarButtonItem btnend;
        private DevExpress.XtraBars.BarButtonItem btnplay;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private DevExpress.XtraSpreadsheet.UI.FileRibbonPage fileRibbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraCharts.ChartControl chartControl1;
        private DevExpress.XtraSpreadsheet.UI.SpreadsheetBarController spreadsheetBarController1;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox2;
        private DevExpress.XtraBars.BarButtonItem btnfresh;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraBars.BarButtonItem btnsavegif;
        private DevExpress.XtraBars.BarButtonItem btnoutpic;
        private DevExpress.XtraBars.Ribbon.RibbonPageCategory ribbonPageCategory1;
        private DevExpress.XtraBars.BarEditItem timeselect;
        private DevExpress.XtraEditors.Repository.RepositoryItemMRUEdit repositoryItemMRUEdit1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;

    }
}