namespace fracture
{
    partial class fractureparameter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fractureparameter));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btncal = new DevExpress.XtraBars.BarButtonItem();
            this.btnoutpic = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageCategory1 = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.vGridControl1 = new DevExpress.XtraVerticalGrid.VGridControl();
            this.chartControl1 = new DevExpress.XtraCharts.ChartControl();
            this.document1 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(this.components);
            this.document3 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(this.components);
            this.chartControl2 = new DevExpress.XtraCharts.ChartControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vGridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.document1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.document3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl2)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.btncal,
            this.btnoutpic,
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonItem3});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 6;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.PageCategories.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
            this.ribbonPageCategory1});
            this.ribbonControl1.Size = new System.Drawing.Size(759, 145);
            // 
            // btncal
            // 
            this.btncal.Caption = "计算";
            this.btncal.Glyph = ((System.Drawing.Image)(resources.GetObject("btncal.Glyph")));
            this.btncal.Id = 1;
            this.btncal.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("btncal.LargeGlyph")));
            this.btncal.Name = "btncal";
            this.btncal.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btncal_ItemClick);
            // 
            // btnoutpic
            // 
            this.btnoutpic.Caption = "导出图片";
            this.btnoutpic.Id = 2;
            this.btnoutpic.LargeGlyph = global::fracture.Properties.Resources.存为图片位图_01;
            this.btnoutpic.Name = "btnoutpic";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "图形设计";
            this.barButtonItem1.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.Glyph")));
            this.barButtonItem1.Id = 3;
            this.barButtonItem1.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.LargeGlyph")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.OnlyInCustomizing;
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "保存模板";
            this.barButtonItem2.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.Glyph")));
            this.barButtonItem2.Id = 4;
            this.barButtonItem2.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.LargeGlyph")));
            this.barButtonItem2.Name = "barButtonItem2";
            this.barButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.OnlyInCustomizing;
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "导出Excel";
            this.barButtonItem3.Id = 5;
            this.barButtonItem3.LargeGlyph = global::fracture.Properties.Resources.excel;
            this.barButtonItem3.Name = "barButtonItem3";
            this.barButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.OnlyInCustomizing;
            // 
            // ribbonPageCategory1
            // 
            this.ribbonPageCategory1.Name = "ribbonPageCategory1";
            this.ribbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonPageCategory1.Text = "操作";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "裂缝参数计算";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btncal);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnoutpic);
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem3);
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem1);
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem2);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "操作";
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
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.dockPanel1.ID = new System.Guid("905b61df-f048-45dd-9028-84b5e4c6de90");
            this.dockPanel1.Location = new System.Drawing.Point(0, 145);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(202, 200);
            this.dockPanel1.Size = new System.Drawing.Size(202, 347);
            this.dockPanel1.Text = "参数输入";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.vGridControl1);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(194, 320);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // vGridControl1
            // 
            this.vGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vGridControl1.Location = new System.Drawing.Point(0, 0);
            this.vGridControl1.Name = "vGridControl1";
            this.vGridControl1.Size = new System.Drawing.Size(194, 320);
            this.vGridControl1.TabIndex = 0;
            // 
            // chartControl1
            // 
            this.chartControl1.Location = new System.Drawing.Point(337, 12);
            this.chartControl1.Name = "chartControl1";
            this.chartControl1.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartControl1.Size = new System.Drawing.Size(401, 273);
            this.chartControl1.TabIndex = 2;
            // 
            // document1
            // 
            this.document1.Caption = "结果图";
            this.document1.ControlName = "dockPanel2";
            this.document1.FloatLocation = new System.Drawing.Point(1804, 468);
            this.document1.FloatSize = new System.Drawing.Size(200, 200);
            this.document1.Properties.AllowClose = DevExpress.Utils.DefaultBoolean.True;
            this.document1.Properties.AllowFloat = DevExpress.Utils.DefaultBoolean.True;
            this.document1.Properties.AllowFloatOnDoubleClick = DevExpress.Utils.DefaultBoolean.True;
            // 
            // document3
            // 
            this.document3.Caption = "panelContainer1";
            this.document3.ControlName = "panelContainer1";
            this.document3.FloatLocation = new System.Drawing.Point(1793, 522);
            this.document3.FloatSize = new System.Drawing.Size(200, 200);
            // 
            // chartControl2
            // 
            this.chartControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl2.Location = new System.Drawing.Point(202, 145);
            this.chartControl2.Name = "chartControl2";
            this.chartControl2.PaletteName = "Violet II";
            this.chartControl2.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartControl2.Size = new System.Drawing.Size(557, 347);
            this.chartControl2.TabIndex = 3;
            // 
            // fractureparameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 492);
            this.Controls.Add(this.chartControl2);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "fractureparameter";
            this.Text = "Form5";
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vGridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.document1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.document3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private TGridControl tGridControl1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraVerticalGrid.VGridControl vGridControl1;
        private DevExpress.XtraBars.BarButtonItem btncal;
        private DevExpress.XtraBars.BarButtonItem btnoutpic;
        private DevExpress.XtraCharts.ChartControl chartControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPageCategory ribbonPageCategory1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.Document document1;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.Document document3;
        private DevExpress.XtraCharts.ChartControl chartControl2;
    }
}