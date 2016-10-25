using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Global;
namespace fracture
{
    public partial class ToAccess : Form
    {
        string ExcelFilePath;
        string accessFilePath;
        RepositoryItemSearchLookUpEdit lookup1;
        public ToAccess()
        {
            InitializeComponent();
            treeList1.DoubleClick += treeList1_DoubleClick;
            treeList2.Click += treeList2_Click;
            GetTatableAndFieldName();
         
        }

        private void treeList2_Click(object sender, EventArgs e)
        {
            TreeListNode clickedNode = this.treeList2.FocusedNode;
            InitTreeView3(treeList3);
            object item = treeList2.FocusedNode;
            if (treeList2.DataSource == null)
                return;
            string sheetName = clickedNode.GetValue("sheetname").ToString();
            DataTable dt = OleDbHelper.ExcelSheetField(ExcelFilePath, sheetName);
            //this.treeList3.SelectImageList = imageCollection1;
            this.treeList3.StateImageList = imageCollection1;

            this.treeList3.DataSource = dt;
            foreach (TreeListNode N in treeList3.Nodes)
                N.StateImageIndex = 2;
            treeList3.Columns[0].Caption = "源字段";

            if (lookup1 != null)
            {
                dt.Columns[0].ColumnName = "源字段";
                lookup1.ValueMember = dt.Columns[0].ToString();
                lookup1.DisplayMember = dt.Columns[0].ToString();
                lookup1.DataSource = dt;
                gridView3.Columns["源字段"].ColumnEdit = lookup1;
            }


        }

        private void buttonEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }
        private void buttonEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Excel文件(*.xls,*.xlsx)|*.xls;*.xlsx";

            if (dlg.ShowDialog() == DialogResult.OK)
            {

                ExcelFilePath = dlg.FileName;

                this.buttonEdit1.Text = ExcelFilePath;
                GetExcelSheetName(ExcelFilePath);

            }

        }
        private void GetExcelSheetName(string filePath)
        {
            InitTreeView2(treeList2);
            DataTable sheetName = OleDbHelper.ExcelSheetName(filePath);
            // List<string> sheetNamelist = sheetName.ToList();
            this.treeList2.DataSource = sheetName;
            this.treeList2.StateImageList = imageCollection1;
            foreach (TreeListNode N in treeList2.Nodes)
                N.StateImageIndex = 0;

            treeList2.Columns[0].Caption = "原表格";
        }
        public static void InitTreeView(DevExpress.XtraTreeList.TreeList treeView)
        {

            treeView.OptionsView.ShowColumns = false;
            treeView.OptionsView.ShowIndicator = false;
            treeView.OptionsView.ShowVertLines = false;
            treeView.OptionsView.ShowHorzLines = false;
            treeView.OptionsBehavior.Editable = false;
            treeView.OptionsView.ShowCheckBoxes = false;
            treeView.OptionsBehavior.EnableFiltering = true;
            treeView.OptionsView.ShowAutoFilterRow = false;
            treeView.OptionsFilter.FilterMode = FilterMode.Extended;

            //treeView.OptionsBehavior.DragNodes = true;
            treeView.OptionsBehavior.AllowIndeterminateCheckState = true;
            // treeView.OptionsSelection.EnableAppearanceFocusedCell = false;
            treeView.OptionsFind.HighlightFindResults = true;
            //treeView.ShowCloseButton = ceShowCloseButton.Checked;
            //TreeList.OptionsFind.ShowClearButton = ceShowClearButton.Checked;
            //TreeList.OptionsFind.ShowFindButton = ceShowFindButton.Checked;
            treeView.ShowFindPanel();
            treeView.OptionsFind.ShowCloseButton = false;

            treeView.OptionsFind.ShowFindButton = false;
            treeView.OptionsFind.ShowClearButton = false;

        }
        public static void InitTreeView2(DevExpress.XtraTreeList.TreeList treeView)
        {
            treeView.ClearNodes();
            //treeView.OptionsView.ShowColumns = false;
            treeView.OptionsView.ShowIndicator = false;
            treeView.OptionsView.ShowVertLines = false;
            treeView.OptionsView.ShowHorzLines = false;
            treeView.OptionsBehavior.Editable = false;
            treeView.OptionsView.ShowCheckBoxes = false;
            treeView.OptionsBehavior.EnableFiltering = false;
            treeView.OptionsView.ShowAutoFilterRow = false;

        }

        public static void InitTreeView3(DevExpress.XtraTreeList.TreeList treeView)
        {
            treeView.ClearNodes();

            //treeView.OptionsView.ShowColumns = false;
            treeView.OptionsView.ShowIndicator = false;
            treeView.OptionsView.ShowVertLines = false;
            treeView.OptionsView.ShowHorzLines = false;
            treeView.OptionsBehavior.Editable = false;
            treeView.OptionsView.ShowCheckBoxes = true;
            treeView.OptionsBehavior.EnableFiltering = false;
            treeView.OptionsView.ShowAutoFilterRow = false;

        }
        private void GetTatableAndFieldName()
        {

            DataTable dt_TableAndCate = new DataTable();
            //string pathName = Application.StartupPath + "\\ACCESS.xlsx";

            string sheetName = "数据表描述";
            string columname = "数据表类型,表显示名称,库表名称 ";
            string sSql = "SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as KeyID,T_WELL_INFOR.WELL_NAME as Name FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID " +
"UNION SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name FROM T_DM_UNIT_CUR_INFOR"; //where [{0}$].库表名称=[{1}$].库表名称 

            string TableAndCate = string.Format("select 数据表类型ID AS ParentID ,表显示名称 as Name,库表名称 as KeyID from [{0}$]  UNION select  上级数据表类型 as ParentID, 数据表类型 as Name,数据表类型ID as KeyID from [{0}$]", sheetName);

            dt_TableAndCate = OleDbHelper.ExcelToDataTable(sheetName, TableAndCate);


            InitTreeView(treeList1);
            treeList1.Nodes.Clear();
            treeList1.DataSource = dt_TableAndCate;
            treeList1.ParentFieldName = "ParentID";

            treeList1.KeyFieldName = "KeyID";
            treeList1.Columns["Name"].Caption = "表显示名称";
            this.treeList1.Nodes[0].Expanded = true; // 只显示1级目录

        }

        private void treeList1_DoubleClick(object sender, EventArgs e)
        {
            TreeListNode clickedNode = this.treeList1.FocusedNode;
            if (clickedNode.ParentNode != null)
            {
                object item = treeList1.FocusedNode;
                string tablename = clickedNode.GetValue("KeyID").ToString();
                GetDataFromAccess(tablename);
            }
        }
        private void GetDataFromAccess(string tablename)
        {
            DataTable dt = null;
         
            //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";
            string[] dataname;
            DataTable dt_TableAndField = new DataTable();
            string sheetName = "物理表汇总";
            
            string TableAndField = string.Format("select 库表名称 AS TABLE_ID, 表显示名称 AS 目的表名, 列显示名称 AS 目的字段 ,库字段名称 as ID, 默认单位名称 as 单位名称, 主键  as 是否必须 from [{0}$] where (库表名称='" + tablename + "')", sheetName);
            dt_TableAndField = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);
            DataTable query = new DataTable();

            dt_TableAndField.Columns.Add("源字段", typeof(string));

            gridView3.OptionsView.ShowColumnHeaders = true;                         //因为有Band列了，所以把ColumnHeader隐藏
            gridView3.ColumnPanelRowHeight = 50;
            gridView3.OptionsView.AllowHtmlDrawHeaders = true;
            //gridView3.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            //表头及行内容居中显示
            gridView3.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView3.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            //添加列标题
            dt_TableAndField.Columns["源字段"].SetOrdinal(3);
            dt_TableAndField.Columns.Add("源表格", typeof(string));
            DataTable tr3 = treeList3.DataSource as DataTable;
            if (tr3 != null)
            {
                for (int i = 0; i < dt_TableAndField.Rows.Count; i++)
                {
                    dt_TableAndField.Rows[i]["源表格"] = treeList3.DataSource.ToString();
                }
            }
            gridControl3.DataSource = dt_TableAndField;// dt;
            gridControl3.MainView.PopulateColumns();
            gridView3.Columns[0].Visible = false;//目的表ID
            gridView3.Columns[4].Visible = false;//目的字段ID
            gridView3.Columns[7].Visible = false;//原表格
        

            if (lookup1 == null)
                lookup1 = (RepositoryItemSearchLookUpEdit)gridControl1.RepositoryItems.Add("SearchLookUpEdit");



            if (tr3 != null)
            {
                treeList3.Visible = true;
                tr3.Columns[0].ColumnName = "源字段";
                lookup1.ValueMember = tr3.Columns[0].ToString();
                lookup1.DisplayMember = tr3.Columns[0].ToString();
                lookup1.DataSource = tr3;
                gridView3.Columns["源字段"].ColumnEdit = lookup1;

            }

            gridView3.OptionsView.ColumnAutoWidth = true;
            gridView3.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveHorzScroll;
            gridView3.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always; // or ScrollVisibility.Auto doesn't work neither; or remove this line, doesn't work 
            gridView3.OptionsView.RowAutoHeight = true;
            gridView3.OptionsCustomization.AllowFilter = true;
            gridView3.BestFitColumns(true);
            gridView3.OptionsCustomization.AllowColumnMoving = false;
            gridView3.OptionsSelection.MultiSelect = true;
            gridView3.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;

        }

        private void dockPanel8_Click(object sender, EventArgs e)
        {

        }

        private void btn_smartmatch_Click(object sender, EventArgs e)
        {
            if (treeList3.DataSource == null)
                return;
            if (gridView3.DataSource == null)
                return;
            DataTable tr3 = treeList3.DataSource as DataTable;
            DataTable gv3 = gridControl3.DataSource as DataTable;
            int m = gv3.Columns.IndexOf("目的字段");
            int n = gv3.Columns.IndexOf("源字段");
            for (int i = 0; i < gv3.Rows.Count; i++)
                for (int j = 0; j < tr3.Rows.Count; j++)
                {
                    string stringtr3 = tr3.Rows[j].ItemArray.GetValue(0).ToString();
                    string stringgv3 = gv3.Rows[i][m].ToString();
                    if (stringtr3.IndexOf(stringgv3) >= 0)
                        gv3.Rows[i][n] = tr3.Rows[j].ItemArray.GetValue(0).ToString();

                }

            gridControl3.DataSource = gv3;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {

        }

        private void btn_run_Click(object sender, EventArgs e)
        {

            DataTable gv3 = gridControl3.DataSource as DataTable;
            int n = gv3.Columns.IndexOf("源表格");
            string sheetName = gv3.Rows[0][n].ToString();//
           
            //accessFilePath = Application.StartupPath + "\\Database.mdb";
            string ExcelSql = string.Format("select * from [{0}]", sheetName);
          
            //+ 
            //INSERT INTO Persons (LastName, Address) VALUES ('Wilson', 'Champs-Elysees')
            OleDbHelper.InsertDataTable2(ExcelFilePath, sheetName, ExcelSql,  Globalname.DabaBasePath, gv3);
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            string title = "导出配置文件";
            string filter = "配置文件(*.xml)|*.xml";
            string localFilePath = FileOperate.saveopenfile(title, filter);
            if (localFilePath != null)
            {
                FileOperate.ExportTo(gridView3, new ExportXmlProvider(localFilePath));
            }
        }

        private void btn_import_Click(object sender, EventArgs e)
        {
            string title = "导入配置文件";
            string filter = "配置文件(*.xml)|*.xml";
            string localFilePath = FileOperate.saveopenfile(title, filter);
            if (localFilePath != null)
            {
                //gridControl3.MainView(localFilePath);

                // FileOperate.ExportTo(gridView3, new ExportXmlProvider(localFilePath));
            }

        }

        private void buttonEdit1_EditValueChanged_1(object sender, EventArgs e)
        {

        }

        private void ToAccess_Load(object sender, EventArgs e)
        {

        }

        private void treeList2_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {

        }





    }
}
