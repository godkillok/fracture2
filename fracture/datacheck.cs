using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.BandedGrid;
using Global;
namespace fracture
{
    public partial class datacheck : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public datacheck()
        {
            InitializeComponent();
            treeList1.DoubleClick += treeList1_DoubleClick;
            GetTatableAndFieldName();
       
            //gridControl1.MouseUp +=gridControl1_MouseUp;
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
          
            //string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
            //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";
            string[] dataname;
            DataTable dt_TableAndField = new DataTable();
            string sheetName = "物理表汇总";

            string TableAndField = string.Format("select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT from [{0}$] where (库表名称='" + tablename + "')", sheetName);
            dt_TableAndField = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);
            DataTable query = new DataTable();

            //          string sSql = "SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as KeyID,T_WELL_INFOR.WELL_NAME as Name FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID " +
            //"UNION SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name FROM T_DM_UNIT_CUR_INFOR";where [{0}$].库表名称 + tablename
            string sSql = "select ";
            

            for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
            {
                sSql = sSql + string.Format("{0} AS {1}, ", dt_TableAndField.Rows[i][1], dt_TableAndField.Rows[i][0]);
            }
            sSql = sSql + string.Format("{0} AS {1} From " + tablename, dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][1], dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][0]);
            dt = OleDbHelper.getTable(sSql,  Globalname.DabaBasePath);
            //string[] varTableName;
            //varTableName = OleDbHelper.GetTableColumn(DabaBasePath, dataname[1]);
      
            // advBandedgridView1是表格上的默认视图，注意这里声明的是：BandedGridView
        
          

           gridView1.OptionsView.ShowColumnHeaders = true;                         //因为有Band列了，所以把ColumnHeader隐藏
           gridView1.ColumnPanelRowHeight = 50;
           gridView1.OptionsView.AllowHtmlDrawHeaders = true;
           //gridView1.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
           //表头及行内容居中显示
           gridView1.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
           gridView1.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
     
            //添加列标题
            //添加列标题

            gridControl1.DataSource = dt;// dt;
            gridControl1.MainView.PopulateColumns();

       
            for (int i = 0; i < dt_TableAndField.Rows.Count; i++)
            {
                if (dt_TableAndField.Rows[i][2].ToString() != "")
                {
                    gridView1.Columns[i].Caption = dt_TableAndField.Rows[i][0].ToString() + "\r\n" + dt_TableAndField.Rows[i][2].ToString();
                }
                else
                {
                    gridView1.Columns[i].Caption = dt_TableAndField.Rows[i][0].ToString();

                }
                
                //view.Bands[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;//这是合并表头居中显示
                //view.Bands[i].View.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            }

        

      
            //bandFile
            // Specify the data source for the grid control. 
       
            //gridView1.PopulateColumns();
          gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveHorzScroll;
            gridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always; // or ScrollVisibility.Auto doesn't work neither; or remove this line, doesn't work 
            gridView1.OptionsView.RowAutoHeight = true;
            gridView1.OptionsCustomization.AllowFilter = true;
           gridView1.BestFitColumns(true);
            gridView1.OptionsCustomization.AllowColumnMoving = false;

            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
     
        }

        private void btn_outexcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string localFilePath = FileOperate.saveopenfile();
            if (localFilePath != null)
            {
                FileOperate.ExportToExcel(gridView1, localFilePath);
            }
        }

        private void btnsave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt = gridControl1.DataSource as DataTable;
            string tablename = dt.TableName;
            //string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
            //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";
            string[] dataname;
            DataTable dt_TableAndField = new DataTable();
            string sheetName = "物理表汇总";
           
            string TableAndField = string.Format("select 列显示名称 AS name ,库字段名称 as ID from [{0}$] where (库表名称='" + tablename + "')", sheetName);
            dt_TableAndField = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);
            DataTable query = new DataTable();
           
            //          string sSql = "SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as KeyID,T_WELL_INFOR.WELL_NAME as Name FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID " +
            //"UNION SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name FROM T_DM_UNIT_CUR_INFOR";where [{0}$].库表名称 + tablename
            string sSql = "select ";


            for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
            {
                sSql = sSql + string.Format("{0} AS {1}, ", dt_TableAndField.Rows[i][1], dt_TableAndField.Rows[i][0]);
            }
            sSql = sSql + string.Format("{0} AS {1} From " + tablename, dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][1], dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][0]);
            OleDbHelper.UpdateDataTable(sSql,  Globalname.DabaBasePath, dt);


        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gridControl1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void copy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            gridView1.CopyToClipboard();


        }

        private void delete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.DeleteSelectedRows();

        }



        private void gridControl1_Click(object sender, EventArgs e)
        {

        }
        string ClipboardData
        {
            get
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData == null) return "";

                if (iData.GetDataPresent(DataFormats.Text))
                    return (string)iData.GetData(DataFormats.Text);
                return "";
            }
            set { Clipboard.SetDataObject(value); }
        }

        private void insert_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string[] data = ClipboardData.Split('\n');
            if (data.Length < 1) return;
            foreach (string row in data)
            {
                AddRow(row);
            }
        }

        void AddRow(string data)
        {
            if (data == string.Empty) return;
            gridView1.AddNewRow();
            string[] rowData = data.Split(new char[] { '\r', '\x09' });
            int rowHandle = gridView1.GetRowHandle(gridView1.DataRowCount);

            for (int i = 0; i < rowData.Length; i++)
            {
                if (i >= gridView1.Columns.Count) break;
                if (gridView1.IsNewItemRow(rowHandle))
                {
                    this.gridView1.SetRowCellValue(rowHandle, gridView1.Columns[i], rowData[i]);
                }
            }
            DataTable dt = gridControl1.DataSource as DataTable;
            DataTable dtf = dt;
        }

        private void toolbar_export_Click(object sender, EventArgs e)
        {
            string localFilePath = FileOperate.saveopenfile();
            if (localFilePath != null)
            {
                FileOperate.ExportToExcel(gridView1, localFilePath);
            }
        }

        private void toolbar_copy_Click(object sender, EventArgs e)
        {
            gridView1.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            gridView1.CopyToClipboard();
        }

        private void toolbar_delete_Click(object sender, EventArgs e)
        {
            gridView1.DeleteSelectedRows();
        }

        private void toolBar_insert_Click(object sender, EventArgs e)
        {
            string[] data = ClipboardData.Split('\n');
            if (data.Length < 1) return;
            foreach (string row in data)
            {
                AddRow(row);
            }
        }
    }
}
