namespace fracture
{
    partial class datacheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(datacheck));
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.treeList1 = new DevExpress.XtraTreeList.TreeList();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.copy = new DevExpress.XtraBars.BarButtonItem();
            this.save = new DevExpress.XtraBars.BarButtonItem();
            this.btn_outexcel = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.delete = new DevExpress.XtraBars.BarButtonItem();
            this.insert = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageCategory1 = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.tool_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolbar_export = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbar_copy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbar_delete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBar_insert = new System.Windows.Forms.ToolStripMenuItem();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            this.tool_menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
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
            this.dockPanel1.ID = new System.Guid("e13a97ec-6062-4ac0-88ed-22d3acc95b01");
            this.dockPanel1.Location = new System.Drawing.Point(0, 147);
            this.dockPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockPanel1.Size = new System.Drawing.Size(200, 365);
            this.dockPanel1.Text = "数据库表";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.treeList1);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel1_Container.Margin = new System.Windows.Forms.Padding(4);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(192, 338);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // treeList1
            // 
            this.treeList1.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList1.Location = new System.Drawing.Point(0, 0);
            this.treeList1.Margin = new System.Windows.Forms.Padding(4);
            this.treeList1.Name = "treeList1";
            this.treeList1.Size = new System.Drawing.Size(192, 338);
            this.treeList1.TabIndex = 0;
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.copy,
            this.save,
            this.btn_outexcel,
            this.barButtonItem1,
            this.delete,
            this.insert});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.Margin = new System.Windows.Forms.Padding(4);
            this.ribbonControl1.MaxItemId = 16;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.PageCategories.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
            this.ribbonPageCategory1});
            this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
            this.ribbonControl1.Size = new System.Drawing.Size(866, 147);
            // 
            // copy
            // 
            this.copy.Caption = "复制选中行";
            this.copy.Glyph = ((System.Drawing.Image)(resources.GetObject("copy.Glyph")));
            this.copy.Id = 3;
            this.copy.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("copy.LargeGlyph")));
            this.copy.Name = "copy";
            this.copy.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.copy_ItemClick);
            // 
            // save
            // 
            this.save.Caption = "提交数据";
            this.save.Id = 7;
            this.save.Name = "save";
            // 
            // btn_outexcel
            // 
            this.btn_outexcel.Caption = "导出excel";
            this.btn_outexcel.Id = 12;
            this.btn_outexcel.LargeGlyph = global::fracture.Properties.Resources._19导出到excel;
            this.btn_outexcel.Name = "btn_outexcel";
            this.btn_outexcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btn_outexcel_ItemClick);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "保存";
            this.barButtonItem1.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.Glyph")));
            this.barButtonItem1.Id = 13;
            this.barButtonItem1.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.LargeGlyph")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // delete
            // 
            this.delete.Caption = "删除选中行";
            this.delete.Glyph = ((System.Drawing.Image)(resources.GetObject("delete.Glyph")));
            this.delete.Id = 14;
            this.delete.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("delete.LargeGlyph")));
            this.delete.Name = "delete";
            this.delete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.delete_ItemClick);
            // 
            // insert
            // 
            this.insert.Caption = "插入复制行";
            this.insert.Glyph = ((System.Drawing.Image)(resources.GetObject("insert.Glyph")));
            this.insert.Id = 15;
            this.insert.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("insert.LargeGlyph")));
            this.insert.Name = "insert";
            this.insert.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.insert_ItemClick);
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
            this.ribbonPage1.Text = "操作";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.copy);
            this.ribbonPageGroup1.ItemLinks.Add(this.btn_outexcel);
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem1);
            this.ribbonPageGroup1.ItemLinks.Add(this.delete);
            this.ribbonPageGroup1.ItemLinks.Add(this.insert);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "操作";
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // gridControl1
            // 
            this.gridControl1.ContextMenuStrip = this.tool_menu;
            this.gridControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridControl1.Location = new System.Drawing.Point(200, 147);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridControl1.MenuManager = this.ribbonControl1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(666, 365);
            this.gridControl1.TabIndex = 5;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // tool_menu
            // 
            this.tool_menu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tool_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbar_export,
            this.toolbar_copy,
            this.toolbar_delete,
            this.toolBar_insert});
            this.tool_menu.Name = "tool_menu";
            this.tool_menu.Size = new System.Drawing.Size(153, 114);
            // 
            // toolbar_export
            // 
            this.toolbar_export.Name = "toolbar_export";
            this.toolbar_export.Size = new System.Drawing.Size(152, 22);
            this.toolbar_export.Text = "导出到Excel";
            this.toolbar_export.Click += new System.EventHandler(this.toolbar_export_Click);
            // 
            // toolbar_copy
            // 
            this.toolbar_copy.Name = "toolbar_copy";
            this.toolbar_copy.Size = new System.Drawing.Size(152, 22);
            this.toolbar_copy.Text = "复制选中行";
            this.toolbar_copy.Click += new System.EventHandler(this.toolbar_copy_Click);
            // 
            // toolbar_delete
            // 
            this.toolbar_delete.Name = "toolbar_delete";
            this.toolbar_delete.Size = new System.Drawing.Size(152, 22);
            this.toolbar_delete.Text = "删除选中行";
            this.toolbar_delete.Click += new System.EventHandler(this.toolbar_delete_Click);
            // 
            // toolBar_insert
            // 
            this.toolBar_insert.Name = "toolBar_insert";
            this.toolBar_insert.Size = new System.Drawing.Size(152, 22);
            this.toolBar_insert.Text = "黏贴行";
            this.toolBar_insert.Click += new System.EventHandler(this.toolBar_insert_Click);
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            // 
            // datacheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 512);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.ribbonControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "datacheck";
            this.Ribbon = this.ribbonControl1;
            this.Text = "Form3";
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            this.tool_menu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraTreeList.TreeList treeList1;
        private DevExpress.XtraBars.BarButtonItem copy;
        private DevExpress.XtraBars.BarButtonItem save;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraBars.BarButtonItem btn_outexcel;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem delete;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraBars.BarButtonItem insert;
        private System.Windows.Forms.ContextMenuStrip tool_menu;
        private System.Windows.Forms.ToolStripMenuItem toolbar_export;
        private System.Windows.Forms.ToolStripMenuItem toolbar_copy;
        private System.Windows.Forms.ToolStripMenuItem toolbar_delete;
        private System.Windows.Forms.ToolStripMenuItem toolBar_insert;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraBars.Ribbon.RibbonPageCategory ribbonPageCategory1;
    }
}