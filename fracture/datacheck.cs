using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.BandedGrid;
using Global;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using System.Collections;
namespace fracture
{
    public partial class datacheck : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        int currentIndex = 1;

        int currentCount = -1;

        int totalCount = -1;

        DataTable dt = null;
        DataTable dt_TableAndField = null;
        string tablename;
        public datacheck()
        {
            InitializeComponent();
            treeList1.DoubleClick += treeList1_DoubleClick;
            GetTatableAndFieldName();
            gridControl1.AllowDrop = true;
            gridControl1.UseEmbeddedNavigator = true;
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
                tablename = clickedNode.GetValue("KeyID").ToString();
                Query();
                //GetDataFromAccess(tablename);
            }
        }
        private void GetTitleFromExcel()
        {
            int start = (currentIndex - 1) * 100;
            //var left = recordCount - start >= 100 ? 100 : recordCount - start;


            //string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
            //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";
            string[] dataname;
            dt_TableAndField = new DataTable();
            string sheetName = "物理表汇总";

            string TableAndField = string.Format("select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT,主键 AS KEYID from [{0}$] where (库表名称='" + tablename + "')", sheetName);
            dt_TableAndField = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);

        }
        private DataTable GetDataFromAccess()
        {
            int start =  100;
            int skip = currentIndex* 100;

            DataTable query = new DataTable();
            string sSql = "SELECT COUNT(*) FROM " + tablename;
            query = OleDbHelper.getTable(sSql, Globalname.DabaBasePath);
            totalCount = int.Parse(query.Rows[0][0].ToString());

            if (totalCount < skip)
                start = start - (skip - totalCount);

            query = new DataTable();
            //          string sSql = "SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as KeyID,T_WELL_INFOR.WELL_NAME as Name FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID " +
            //"UNION SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name FROM T_DM_UNIT_CUR_INFOR";where [{0}$].库表名称 + tablename
            sSql = "select ";

            if ((currentIndex - 1) == 0)
            {
                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    sSql = sSql + string.Format("{0} AS {1}, ", dt_TableAndField.Rows[i][1], dt_TableAndField.Rows[i][0]);
                }
                sSql = sSql + string.Format("{0} AS {1} From (select top {3} * from {4}", dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][1], dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][0], start, skip, tablename);

                int flag = 0;
                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    if (bool.Parse(dt_TableAndField.Rows[i][3].ToString()) == true)
                    {
                        if (flag == 0)
                        {
                            sSql = sSql + string.Format(" order by {0}", dt_TableAndField.Rows[i][1]);
                            flag = flag + 1;
                        }
                        else
                        {
                            sSql = sSql + string.Format(" ,{0} ", dt_TableAndField.Rows[i][1]);
                        }
                    }
                }


                flag = 0;
                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    if (bool.Parse(dt_TableAndField.Rows[i][3].ToString()) == true)
                    {
                        if (flag == 0)
                        {
                            sSql = sSql + string.Format("  DESC) order by {0}", dt_TableAndField.Rows[i][1]);
                            flag = flag + 1;
                        }
                        else
                        {
                            sSql = sSql + string.Format(" ,{0} ", dt_TableAndField.Rows[i][1]);
                        }
                    }
                }
                sSql = sSql + string.Format(" desc ");

            }
            else
            {
                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    sSql = sSql + string.Format("{0} AS {1}, ", dt_TableAndField.Rows[i][1], dt_TableAndField.Rows[i][0]);
                }
                sSql = sSql + string.Format("{0} AS {1} From ( select top {2} * from (select top {3} * from {4}", dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][1], dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][0], start, skip, tablename);

                int flag = 0;
                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    if (bool.Parse(dt_TableAndField.Rows[i][3].ToString()) == true)
                    {
                        if (flag == 0)
                        {
                            sSql = sSql + string.Format(" order by {0}", dt_TableAndField.Rows[i][1]);
                            flag = flag + 1;
                        }
                        else
                        {
                            sSql = sSql + string.Format(" ,{0} ", dt_TableAndField.Rows[i][1]);
                        }
                    }
                }
                flag = 0;
                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    if (bool.Parse(dt_TableAndField.Rows[i][3].ToString()) == true)
                    {
                        if (flag == 0)
                        {
                            sSql = sSql + string.Format("desc ) order by {0} ", dt_TableAndField.Rows[i][1]);
                            flag = flag + 1;
                        }
                        else
                        {
                            sSql = sSql + string.Format(" ,{0} ", dt_TableAndField.Rows[i][1]);
                        }
                    }
                }

                flag = 0;
                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    if (bool.Parse(dt_TableAndField.Rows[i][3].ToString()) == true)
                    {
                        if (flag == 0)
                        {
                            sSql = sSql + string.Format(" ) order by {0}", dt_TableAndField.Rows[i][1]);
                            flag = flag + 1;
                        }
                        else
                        {
                            sSql = sSql + string.Format(" ,{0} ", dt_TableAndField.Rows[i][1]);
                        }
                    }
                }
                sSql = sSql + string.Format(" desc ");
            }
          
            query = OleDbHelper.getTable(sSql, Globalname.DabaBasePath);
            query.TableName = tablename;
            return query;
            //string[] varTableName;
            //varTableName = OleDbHelper.GetTableColumn(DabaBasePath, dataname[1]);

            // advBandedgridView1是表格上的默认视图，注意这里声明的是：BandedGridView


        }

        private void Query()
        {

            gridView1.TopRowChanged += gridView1_TopRowChanged;

            currentIndex = 1;
            GetTitleFromExcel();
            //dt = new DataTable();
            dt = GetDataFromAccess(); ;


            currentCount = dt.Rows.Count;



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

        private void gridView1_TopRowChanged(object sender, EventArgs e)
        {
            if (gridView1.IsRowVisible(currentCount - 10) == DevExpress.XtraGrid.Views.Grid.RowVisibleState.Visible)
            {
                gridView1.TopRowChanged -= gridView1_TopRowChanged;
                ++currentIndex;
                DataTable dataSource = null; ;
                dataSource = GetDataFromAccess();
                dt.Merge(dataSource);
                //StudentList.AddRange(dataSource);
                gridControl1.RefreshDataSource();
                currentCount = dt.Rows.Count;
                if (dt.Rows.Count % 100 == 0)
                {
                    gridView1.TopRowChanged += gridView1_TopRowChanged;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            gridView1.IndicatorWidth = 40;
            gridView1.PreviewIndent = 0;
            gridView1.CustomDrawRowIndicator += gridView1_CustomDrawRowIndicator;
            base.OnLoad(e);
        }

        void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
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
            OleDbHelper.UpdateDataTable(sSql, Globalname.DabaBasePath, dt);


        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
            OleDbHelper.UpdateDataTable(sSql, Globalname.DabaBasePath, dt);
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

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sqlstr;
            ArrayList list = new ArrayList(new string[] { "alter table  SYS_IMPORT_TASK alter column GROUPID varchar(50) ",
"alter table  SYS_IMPORT_TASK alter column TASKNAME varchar(50) ",
"alter table  SYS_IMPORT_TASK alter column CLIENTNAME varchar(50) ",
"alter table  SYS_IMPORT_TASK alter column TABLENAME varchar(50) ",
"alter table  SYS_IMPORT_TASK alter column TASKPATH varchar(50) ",
"alter table  SYS_IMPORT_TASK alter column TASKSTATUS varchar(50) ",
"alter table  SYS_IMPORT_TASK alter column IMPORTMESSAGE varchar(50) ",
"alter table  SYS_IMPORT_TASK alter column REMARK varchar(50) ",
"alter table  SYS_MAP_VALUES alter column NAME varchar(50) ",
"alter table  SYS_MAP_VALUES alter column SRC varchar(50) ",
"alter table  SYS_MAP_VALUES alter column DST varchar(50) ",
"alter table  SYS_MODULETABLE alter column MODUEL varchar(50) ",
"alter table  SYS_MODULETABLE alter column MODUELNAME varchar(50) ",
"alter table  SYS_MODULETABLE alter column FUNCTIONEN varchar(50) ",
"alter table  SYS_MODULETABLE alter column FUNCTIONCN varchar(50) ",
"alter table  SYS_MODULETABLE alter column TABLEEN varchar(50) ",
"alter table  SYS_MODULETABLE alter column TABLECN varchar(50) ",
"alter table  SYS_MODULETABLE alter column FIELDEN varchar(50) ",
"alter table  SYS_MODULETABLE alter column FIELDCN varchar(50) ",
"alter table  T_CORE_CAPILLARY alter column WELL_ID varchar(50) ",
"alter table  T_CORE_CAPILLARY alter column WELL_NAME varchar(50) ",
"alter table  T_CORE_CAPILLARY alter column WB_ID varchar(50) ",
"alter table  T_CORE_CAPILLARY alter column L_REMARK varchar(50) ",
"alter table  T_CORE_CAPILLARY alter column NUM varchar(50) ",
"alter table  T_CORE_CAPILLARY alter column TEST_DATA varchar(50) ",
"alter table  T_CORE_CAPILLARY alter column REMARK varchar(50) ",
"alter table  T_CORE_G_W_RELA_PER alter column WELL_ID varchar(50) ",
"alter table  T_CORE_G_W_RELA_PER alter column WELL_NAME varchar(50) ",
"alter table  T_CORE_G_W_RELA_PER alter column WB_ID varchar(50) ",
"alter table  T_CORE_G_W_RELA_PER alter column L_REMARK varchar(50) ",
"alter table  T_CORE_G_W_RELA_PER alter column NUM varchar(50) ",
"alter table  T_CORE_G_W_RELA_PER alter column TEST_DATA varchar(50) ",
"alter table  T_CORE_G_W_RELA_PER alter column REMARK varchar(50) ",
"alter table  T_CORE_O_G_RELA_PER alter column WELL_ID varchar(50) ",
"alter table  T_CORE_O_G_RELA_PER alter column WELL_NAME varchar(50) ",
"alter table  T_CORE_O_G_RELA_PER alter column WB_ID varchar(50) ",
"alter table  T_CORE_O_G_RELA_PER alter column L_REMARK varchar(50) ",
"alter table  T_CORE_O_G_RELA_PER alter column NUM varchar(50) ",
"alter table  T_CORE_O_G_RELA_PER alter column TEST_DATA varchar(50) ",
"alter table  T_CORE_O_G_RELA_PER alter column REMARK varchar(50) ",
"alter table  T_CORE_O_W_RELA_PER alter column WELL_ID varchar(50) ",
"alter table  T_CORE_O_W_RELA_PER alter column WELL_NAME varchar(50) ",
"alter table  T_CORE_O_W_RELA_PER alter column WB_ID varchar(50) ",
"alter table  T_CORE_O_W_RELA_PER alter column L_REMARK varchar(50) ",
"alter table  T_CORE_O_W_RELA_PER alter column NUM varchar(50) ",
"alter table  T_CORE_O_W_RELA_PER alter column TEST_DATA varchar(50) ",
"alter table  T_CORE_O_W_RELA_PER alter column REMARK varchar(50) ",
"alter table  T_CORE_ROCK_PARA alter column WELL_ID varchar(50) ",
"alter table  T_CORE_ROCK_PARA alter column WELL_NAME varchar(50) ",
"alter table  T_CORE_ROCK_PARA alter column WB_ID varchar(50) ",
"alter table  T_CORE_ROCK_PARA alter column L_REMARK varchar(50) ",
"alter table  T_CORE_ROCK_PARA alter column NUM varchar(50) ",
"alter table  T_CORE_ROCK_PARA alter column REMARK varchar(50) ",
"alter table  T_DB_CUR_INFOR alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_DB_CUR_INFOR alter column GEO_UNIT_ALIAS varchar(50) ",
"alter table  T_DB_CUR_INFOR alter column GEO_UNIT_CODE varchar(50) ",
"alter table  T_DB_CUR_INFOR alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_DB_CUR_INFOR alter column LEVEL_ID varchar(50) ",
"alter table  T_DB_CUR_INFOR alter column PARENT_GEO_UNIT_ID varchar(50) ",
"alter table  T_DB_CUR_INFOR alter column GEO_UNIT_TYPE varchar(50) ",
"alter table  T_DB_CUR_INFOR alter column REMARK varchar(50) ",
"alter table  T_DB_GEO_INFOR alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_DB_GEO_INFOR alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_DB_GEO_INFOR alter column REMARK varchar(50) ",
"alter table  T_DB_INFOR_CHA alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_DB_INFOR_CHA alter column CC varchar(50) ",
"alter table  T_DB_INFOR_CHA alter column BEFORE_RESULT varchar(50) ",
"alter table  T_DB_INFOR_CHA alter column REMARK varchar(50) ",
"alter table  T_DB_INFOR_CHA_CC alter column CATEGORY_NAME varchar(50) ",
"alter table  T_DB_INFOR_CHA_CC alter column CODE varchar(50) ",
"alter table  T_DB_INFOR_CHA_CC alter column REMARK varchar(50) ",
"alter table  T_DB_LC alter column LEVEL_NAME varchar(50) ",
"alter table  T_DB_LC alter column ID varchar(50) ",
"alter table  T_DB_LC alter column LEVEL_ALIAS varchar(50) ",
"alter table  T_DB_LC alter column REMARK varchar(50) ",
"alter table  T_DB_MEM alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_DB_MEM alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_DB_MEM alter column EVENT_TYPE varchar(50) ",
"alter table  T_DB_MEM alter column MATTER varchar(50) ",
"alter table  T_DB_MEM alter column REMARK varchar(50) ",
"alter table  T_DM_UNIT_CUR_INFOR alter column REMARK varchar(50) ",
"alter table  T_DM_UNIT_LC alter column LEVEL_NAME varchar(50) ",
"alter table  T_DM_UNIT_LC alter column ID varchar(50) ",
"alter table  T_DM_UNIT_LC alter column LEVEL_ALIAS varchar(50) ",
"alter table  T_DM_UNIT_LC alter column REMARK varchar(50) ",
"alter table  T_DM_UNIT_NAME_CHA_CC alter column CATEGORY_NAME varchar(50) ",
"alter table  T_DM_UNIT_NAME_CHA_CC alter column CODE varchar(50) ",
"alter table  T_DM_UNIT_NAME_CHA_CC alter column REMARK varchar(50) ",
"alter table  T_DZ_CUR_INFOR alter column L_NAME varchar(50) ",
"alter table  T_DZ_CUR_INFOR alter column L_ALIAS varchar(50) ",
"alter table  T_DZ_CUR_INFOR alter column L_CODE varchar(50) ",
"alter table  T_DZ_CUR_INFOR alter column L_ID varchar(50) ",
"alter table  T_DZ_CUR_INFOR alter column LEVEL_ID varchar(50) ",
"alter table  T_DZ_CUR_INFOR alter column SNUM varchar(50) ",
"alter table  T_DZ_CUR_INFOR alter column PARENT_L_ID varchar(50) ",
"alter table  T_DZ_CUR_INFOR alter column L_PROPERTY varchar(50) ",
"alter table  T_DZ_CUR_INFOR alter column REMARK varchar(50) ",
"alter table  T_DZ_INFOR_CHA alter column L_ID varchar(50) ",
"alter table  T_DZ_INFOR_CHA alter column CC varchar(50) ",
"alter table  T_DZ_INFOR_CHA alter column BEFORE_RESULT varchar(50) ",
"alter table  T_DZ_INFOR_CHA alter column REMARK varchar(50) ",
"alter table  T_DZ_INFOR_CHA_CC alter column CATEGORY_NAME varchar(50) ",
"alter table  T_DZ_INFOR_CHA_CC alter column CODE varchar(50) ",
"alter table  T_DZ_INFOR_CHA_CC alter column REMARK varchar(50) ",
"alter table  T_DZ_LC alter column LEVEL_NAME varchar(50) ",
"alter table  T_DZ_LC alter column ID varchar(50) ",
"alter table  T_DZ_LC alter column LEVEL_ALIAS varchar(50) ",
"alter table  T_DZ_LC alter column REMARK varchar(50) ",
"alter table  T_G_FDB_D alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_G_FDB_D alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_G_FDB_D alter column REMARK varchar(50) ",
"alter table  T_G_FDB_FLU alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_G_FDB_FLU alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_G_FDB_FLU alter column REMARK varchar(50) ",
"alter table  T_G_FDB_M alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_G_FDB_M alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_G_FDB_M alter column REMARK varchar(50) ",
"alter table  T_G_FDB_Y alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_G_FDB_Y alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_G_FDB_Y alter column REMARK varchar(50) ",
"alter table  T_G_RZ_D alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_G_RZ_D alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_G_RZ_D alter column L_ID varchar(50) ",
"alter table  T_G_RZ_D alter column L_NAME varchar(50) ",
"alter table  T_G_RZ_D alter column REMARK varchar(50) ",
"alter table  T_G_RZ_FLU alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_G_RZ_FLU alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_G_RZ_FLU alter column L_ID varchar(50) ",
"alter table  T_G_RZ_FLU alter column L_NAME varchar(50) ",
"alter table  T_G_RZ_FLU alter column REMARK varchar(50) ",
"alter table  T_G_RZ_M alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_G_RZ_M alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_G_RZ_M alter column L_ID varchar(50) ",
"alter table  T_G_RZ_M alter column L_NAME varchar(50) ",
"alter table  T_G_RZ_M alter column REMARK varchar(50) ",
"alter table  T_G_RZ_Y alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_G_RZ_Y alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_G_RZ_Y alter column L_ID varchar(50) ",
"alter table  T_G_RZ_Y alter column L_NAME varchar(50) ",
"alter table  T_G_RZ_Y alter column REMARK varchar(50) ",
"alter table  T_GEO_MODEL alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_GEO_MODEL alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_GEO_MODEL alter column MEMORY_ADDRESS varchar(50) ",
"alter table  T_GEO_MODEL alter column REMARK varchar(50) ",
"alter table  T_GW_D alter column WELL_ID varchar(50) ",
"alter table  T_GW_D alter column WELL_NAME varchar(50) ",
"alter table  T_GW_D alter column L_REMARK varchar(50) ",
"alter table  T_GW_D alter column PMC varchar(50) ",
"alter table  T_GW_D alter column REMARK varchar(50) ",
"alter table  T_GW_DELI alter column WELL_ID varchar(50) ",
"alter table  T_GW_DELI alter column WELL_NAME varchar(50) ",
"alter table  T_GW_DELI alter column TEST_DATA varchar(50) ",
"alter table  T_GW_DELI alter column REMARK varchar(50) ",
"alter table  T_GW_DELI_EXPLAIN alter column WELL_ID varchar(50) ",
"alter table  T_GW_DELI_EXPLAIN alter column WELL_NAME varchar(50) ",
"alter table  T_GW_DELI_EXPLAIN alter column L_REMARK varchar(50) ",
"alter table  T_GW_DELI_EXPLAIN alter column REMARK varchar(50) ",
"alter table  T_GW_L_M alter column WELL_ID varchar(50) ",
"alter table  T_GW_L_M alter column WELL_NAME varchar(50) ",
"alter table  T_GW_L_M alter column WB_ID varchar(50) ",
"alter table  T_GW_L_M alter column L_ID varchar(50) ",
"alter table  T_GW_L_M alter column PMC varchar(50) ",
"alter table  T_GW_L_M alter column REMARK varchar(50) ",
"alter table  T_GW_M alter column WELL_ID varchar(50) ",
"alter table  T_GW_M alter column WELL_NAME varchar(50) ",
"alter table  T_GW_M alter column L_REMARK varchar(50) ",
"alter table  T_GW_M alter column PMC varchar(50) ",
"alter table  T_GW_M alter column REMARK varchar(50) ",
"alter table  T_GW_PPT alter column WELL_ID varchar(50) ",
"alter table  T_GW_PPT alter column WELL_NAME varchar(50) ",
"alter table  T_GW_PPT alter column WB_ID varchar(50) ",
"alter table  T_GW_PPT alter column EXPLAIN_SNUM varchar(50) ",
"alter table  T_GW_PPT alter column L_REMARK varchar(50) ",
"alter table  T_GW_PPT alter column REMARK varchar(50) ",
"alter table  T_INJ_TRACER alter column WELL_ID varchar(50) ",
"alter table  T_INJ_TRACER alter column WELL_NAME varchar(50) ",
"alter table  T_INJ_TRACER alter column IWM varchar(50) ",
"alter table  T_INJ_TRACER alter column CODE_TRACER varchar(50) ",
"alter table  T_INJ_TRACER alter column REMARK varchar(50) ",
"alter table  T_INJGW_D alter column WELL_ID varchar(50) ",
"alter table  T_INJGW_D alter column WELL_NAME varchar(50) ",
"alter table  T_INJGW_D alter column L_REMARK varchar(50) ",
"alter table  T_INJGW_D alter column REMARK varchar(50) ",
"alter table  T_INJGW_IPT alter column WELL_ID varchar(50) ",
"alter table  T_INJGW_IPT alter column WELL_NAME varchar(50) ",
"alter table  T_INJGW_IPT alter column WB_ID varchar(50) ",
"alter table  T_INJGW_IPT alter column EXPLAIN_SNUM varchar(50) ",
"alter table  T_INJGW_IPT alter column L_REMARK varchar(50) ",
"alter table  T_INJGW_IPT alter column REMARK varchar(50) ",
"alter table  T_INJGW_L_M alter column WELL_ID varchar(50) ",
"alter table  T_INJGW_L_M alter column WELL_NAME varchar(50) ",
"alter table  T_INJGW_L_M alter column WB_ID varchar(50) ",
"alter table  T_INJGW_L_M alter column L_ID varchar(50) ",
"alter table  T_INJGW_L_M alter column REMARK varchar(50) ",
"alter table  T_INJGW_M alter column WELL_ID varchar(50) ",
"alter table  T_INJGW_M alter column WELL_NAME varchar(50) ",
"alter table  T_INJGW_M alter column L_REMARK varchar(50) ",
"alter table  T_INJGW_M alter column REMARK varchar(50) ",
"alter table  T_INJWW_D alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_D alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_D alter column L_REMARK varchar(50) ",
"alter table  T_INJWW_D alter column REMARK varchar(50) ",
"alter table  T_INJWW_FLU alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_FLU alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_FLU alter column REMARK varchar(50) ",
"alter table  T_INJWW_INJ alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_INJ alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_INJ alter column TEST_DATA varchar(50) ",
"alter table  T_INJWW_INJ alter column REMARK varchar(50) ",
"alter table  T_INJWW_IPT alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_IPT alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_IPT alter column WB_ID varchar(50) ",
"alter table  T_INJWW_IPT alter column L_REMARK varchar(50) ",
"alter table  T_INJWW_IPT alter column EXPLAIN_SNUM varchar(50) ",
"alter table  T_INJWW_IPT alter column REMARK varchar(50) ",
"alter table  T_INJWW_L_M alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_L_M alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_L_M alter column WB_ID varchar(50) ",
"alter table  T_INJWW_L_M alter column L_ID varchar(50) ",
"alter table  T_INJWW_L_M alter column REMARK varchar(50) ",
"alter table  T_INJWW_M alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_M alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_M alter column L_REMARK varchar(50) ",
"alter table  T_INJWW_M alter column REMARK varchar(50) ",
"alter table  T_INJWW_SEG_D alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_SEG_D alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_SEG_D alter column WB_ID varchar(50) ",
"alter table  T_INJWW_SEG_D alter column INJ_ZONE_SNUM varchar(50) ",
"alter table  T_INJWW_SEG_D alter column L_REMARK varchar(50) ",
"alter table  T_INJWW_SEG_D alter column REMARK varchar(50) ",
"alter table  T_INJWW_SEG_INJ alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_SEG_INJ alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_SEG_INJ alter column WB_ID varchar(50) ",
"alter table  T_INJWW_SEG_INJ alter column INJ_ZONE_SNUM varchar(50) ",
"alter table  T_INJWW_SEG_INJ alter column TEST_DATA varchar(50) ",
"alter table  T_INJWW_SEG_INJ alter column REMARK varchar(50) ",
"alter table  T_INJWW_SEG_PARA alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_SEG_PARA alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_SEG_PARA alter column WB_ID varchar(50) ",
"alter table  T_INJWW_SEG_PARA alter column INJ_ZONE_SNUM varchar(50) ",
"alter table  T_INJWW_SEG_PARA alter column L_REMARK varchar(50) ",
"alter table  T_INJWW_SEG_PARA alter column W_NOZZLE_TYPE varchar(50) ",
"alter table  T_INJWW_SEG_PARA alter column REMARK varchar(50) ",
"alter table  T_INJWW_SIPD alter column WELL_ID varchar(50) ",
"alter table  T_INJWW_SIPD alter column WELL_NAME varchar(50) ",
"alter table  T_INJWW_SIPD alter column TEST_DATA varchar(50) ",
"alter table  T_INJWW_SIPD alter column REMARK varchar(50) ",
"alter table  T_O_FDB_D alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_O_FDB_D alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_O_FDB_D alter column REMARK varchar(50) ",
"alter table  T_O_FDB_FLU alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_O_FDB_FLU alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_O_FDB_FLU alter column REMARK varchar(50) ",
"alter table  T_O_FDB_M alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_O_FDB_M alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_O_FDB_M alter column L_REMARK varchar(50) ",
"alter table  T_O_FDB_M alter column INJ_PATTERN varchar(50) ",
"alter table  T_O_FDB_M alter column REMARK varchar(50) ",
"alter table  T_O_FDB_Y alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_O_FDB_Y alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_O_FDB_Y alter column REMARK varchar(50) ",
"alter table  T_O_RZ_D alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_O_RZ_D alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_O_RZ_D alter column L_ID varchar(50) ",
"alter table  T_O_RZ_D alter column L_NAME varchar(50) ",
"alter table  T_O_RZ_D alter column REMARK varchar(50) ",
"alter table  T_O_RZ_FLU alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_O_RZ_FLU alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_O_RZ_FLU alter column L_ID varchar(50) ",
"alter table  T_O_RZ_FLU alter column L_NAME varchar(50) ",
"alter table  T_O_RZ_FLU alter column REMARK varchar(50) ",
"alter table  T_O_RZ_M alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_O_RZ_M alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_O_RZ_M alter column L_ID varchar(50) ",
"alter table  T_O_RZ_M alter column L_NAME varchar(50) ",
"alter table  T_O_RZ_M alter column INJ_PATTERN varchar(50) ",
"alter table  T_O_RZ_M alter column REMARK varchar(50) ",
"alter table  T_O_RZ_Y alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_O_RZ_Y alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_O_RZ_Y alter column L_ID varchar(50) ",
"alter table  T_O_RZ_Y alter column L_NAME varchar(50) ",
"alter table  T_O_RZ_Y alter column REMARK varchar(50) ",
"alter table  T_OW_D alter column WELL_ID varchar(50) ",
"alter table  T_OW_D alter column WELL_NAME varchar(50) ",
"alter table  T_OW_D alter column PMC varchar(50) ",
"alter table  T_OW_D alter column REMARK varchar(50) ",
"alter table  T_OW_EMULSIF alter column WELL_ID varchar(50) ",
"alter table  T_OW_EMULSIF alter column WELL_NAME varchar(50) ",
"alter table  T_OW_EMULSIF alter column WB_ID varchar(50) ",
"alter table  T_OW_EMULSIF alter column L_REMARK varchar(50) ",
"alter table  T_OW_EMULSIF alter column NUM varchar(50) ",
"alter table  T_OW_EMULSIF alter column TEST_DATA varchar(50) ",
"alter table  T_OW_EMULSIF alter column REMARK varchar(50) ",
"alter table  T_OW_GROUP_L_RELA alter column OW_ID varchar(50) ",
"alter table  T_OW_GROUP_L_RELA alter column OW_NAME varchar(50) ",
"alter table  T_OW_GROUP_L_RELA alter column OW_L_ID varchar(50) ",
"alter table  T_OW_GROUP_L_RELA alter column INJWW_ID varchar(50) ",
"alter table  T_OW_GROUP_L_RELA alter column INJWW_NAME varchar(50) ",
"alter table  T_OW_GROUP_L_RELA alter column INJW_L_ID varchar(50) ",
"alter table  T_OW_GROUP_L_RELA alter column REMARK varchar(50) ",
"alter table  T_OW_L_M alter column WELL_ID varchar(50) ",
"alter table  T_OW_L_M alter column WELL_NAME varchar(50) ",
"alter table  T_OW_L_M alter column L_ID varchar(50) ",
"alter table  T_OW_L_M alter column WB_ID varchar(50) ",
"alter table  T_OW_L_M alter column PMC varchar(50) ",
"alter table  T_OW_L_M alter column REMARK varchar(50) ",
"alter table  T_OW_M alter column WELL_ID varchar(50) ",
"alter table  T_OW_M alter column WELL_NAME varchar(50) ",
"alter table  T_OW_M alter column PROD_L_REMARK varchar(50) ",
"alter table  T_OW_M alter column PMC varchar(50) ",
"alter table  T_OW_M alter column REMARK varchar(50) ",
"alter table  T_OW_PPT alter column WELL_ID varchar(50) ",
"alter table  T_OW_PPT alter column WELL_NAME varchar(50) ",
"alter table  T_OW_PPT alter column WB_ID varchar(50) ",
"alter table  T_OW_PPT alter column EXPLAIN_SNUM varchar(50) ",
"alter table  T_OW_PPT alter column L_REMARK varchar(50) ",
"alter table  T_OW_PPT alter column REMARK varchar(50) ",
"alter table  T_OW_STEADY_WELL alter column WELL_ID varchar(50) ",
"alter table  T_OW_STEADY_WELL alter column WELL_NAME varchar(50) ",
"alter table  T_OW_STEADY_WELL alter column TEST_DATA varchar(50) ",
"alter table  T_OW_STEADY_WELL alter column REMARK varchar(50) ",
"alter table  T_OW_W_LAB alter column WELL_ID varchar(50) ",
"alter table  T_OW_W_LAB alter column WELL_NAME varchar(50) ",
"alter table  T_OW_W_LAB alter column SNUM varchar(50) ",
"alter table  T_OW_W_LAB alter column WB_ID varchar(50) ",
"alter table  T_OW_W_LAB alter column L_REMARK varchar(50) ",
"alter table  T_OW_W_LAB alter column W_TYPE varchar(50) ",
"alter table  T_OW_W_LAB alter column REMARK varchar(50) ",
"alter table  T_PERFOR_TC alter column PERFOR_METHOD_NAME varchar(50) ",
"alter table  T_PERFOR_TC alter column PERFOR_METHOD_CODE varchar(50) ",
"alter table  T_PERFOR_TC alter column ID varchar(50) ",
"alter table  T_PERFOR_TC alter column REMARK varchar(50) ",
"alter table  T_PROD_COMPOSING alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_PROD_COMPOSING alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_PROD_COMPOSING alter column REMARK varchar(50) ",
"alter table  T_PROD_TRACER alter column WELL_ID varchar(50) ",
"alter table  T_PROD_TRACER alter column WELL_NAME varchar(50) ",
"alter table  T_PROD_TRACER alter column CODE_TRACER varchar(50) ",
"alter table  T_PROD_TRACER alter column REMARK varchar(50) ",
"alter table  T_R_WELL_INITIALcount alter column well_id varchar(50) ",
"alter table  T_R_WELL_INITIALcount alter column well_name varchar(50) ",
"alter table  T_R_WELL_INITIALcount alter column PROD_L_REMARK varchar(50) ",
"alter table  T_R_WELL_INITIALcount alter column PMC varchar(50) ",
"alter table  T_R_WELL_INITIALcount alter column CODE varchar(50) ",
"alter table  T_R_WELL_INITIALcount alter column Remark varchar(50) ",
"alter table  T_RZ_BOUNDATY alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_RZ_BOUNDATY alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_RZ_BOUNDATY alter column L_ID varchar(50) ",
"alter table  T_RZ_BOUNDATY alter column L_NAME varchar(50) ",
"alter table  T_RZ_BOUNDATY alter column NUM varchar(50) ",
"alter table  T_RZ_BOUNDATY alter column TC varchar(50) ",
"alter table  T_RZ_BOUNDATY alter column BOUNDARY_DATA varchar(50) ",
"alter table  T_RZ_BOUNDATY alter column REMARK varchar(50) ",
"alter table  T_RZ_FAULT alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_RZ_FAULT alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_RZ_FAULT alter column L_ID varchar(50) ",
"alter table  T_RZ_FAULT alter column L_NAME varchar(50) ",
"alter table  T_RZ_FAULT alter column FAULT_NAME varchar(50) ",
"alter table  T_RZ_FAULT alter column FAULT_TC varchar(50) ",
"alter table  T_RZ_FAULT alter column FAULT_LINE_NUM varchar(50) ",
"alter table  T_RZ_FAULT alter column FAULT_LINE_DATA varchar(50) ",
"alter table  T_RZ_FAULT alter column REMARK varchar(50) ",
"alter table  T_RZ_PRO alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_RZ_PRO alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_RZ_PRO alter column L_ID varchar(50) ",
"alter table  T_RZ_PRO alter column L_NAME varchar(50) ",
"alter table  T_RZ_PRO alter column ROCK_TYPE varchar(50) ",
"alter table  T_RZ_PRO alter column WETTABILITY varchar(50) ",
"alter table  T_RZ_PRO alter column REMARK varchar(50) ",
"alter table  T_STIMU_C alter column STIMU_NAME varchar(50) ",
"alter table  T_STIMU_C alter column CODE varchar(50) ",
"alter table  T_STIMU_C alter column REMARK varchar(50) ",
"alter table  T_STIMU_COMPOSING alter column GEO_UNIT_ID varchar(50) ",
"alter table  T_STIMU_COMPOSING alter column GEO_UNIT_NAME varchar(50) ",
"alter table  T_STIMU_COMPOSING alter column STIMU_CODE varchar(50) ",
"alter table  T_STIMU_COMPOSING alter column REMARK varchar(50) ",
"alter table  T_TRACER alter column INJWW_ID varchar(50) ",
"alter table  T_TRACER alter column INJWW_NAME varchar(50) ",
"alter table  T_TRACER alter column OW_ID varchar(50) ",
"alter table  T_TRACER alter column OW_NAME varchar(50) ",
"alter table  T_TRACER alter column REMARK varchar(50) ",
"alter table  T_WELL_CASING_TC alter column TYPE_NAME varchar(50) ",
"alter table  T_WELL_CASING_TC alter column CODE varchar(50) ",
"alter table  T_WELL_CASING_TC alter column REMARK varchar(50) ",
"alter table  T_WELL_CC alter column WELL_CATEGORY_NAME varchar(50) ",
"alter table  T_WELL_CC alter column CODE varchar(50) ",
"alter table  T_WELL_CC alter column ID varchar(50) ",
"alter table  T_WELL_CC alter column REMARK varchar(50) ",
"alter table  T_WELL_FLU alter column WELL_ID varchar(50) ",
"alter table  T_WELL_FLU alter column WELL_NAME varchar(50) ",
"alter table  T_WELL_FLU alter column WB_ID varchar(50) ",
"alter table  T_WELL_FLU alter column NUM varchar(50) ",
"alter table  T_WELL_FLU alter column L_REMARK varchar(50) ",
"alter table  T_WELL_FLU alter column REMARK varchar(50) ",
"alter table  T_WELL_FLU_PVT alter column WELL_ID varchar(50) ",
"alter table  T_WELL_FLU_PVT alter column WELL_NAME varchar(50) ",
"alter table  T_WELL_FLU_PVT alter column L_REMARK varchar(50) ",
"alter table  T_WELL_FLU_PVT alter column NUM varchar(50) ",
"alter table  T_WELL_FLU_PVT alter column TEST_DATA varchar(50) ",
"alter table  T_WELL_FLU_PVT alter column REMARK varchar(50) ",
"alter table  T_WELL_G_EXPANSION alter column WELL_ID varchar(50) ",
"alter table  T_WELL_G_EXPANSION alter column WELL_NAME varchar(50) ",
"alter table  T_WELL_G_EXPANSION alter column WB_ID varchar(50) ",
"alter table  T_WELL_G_EXPANSION alter column L_REMARK varchar(50) ",
"alter table  T_WELL_G_EXPANSION alter column SAMPLING_DEPTH varchar(50) ",
"alter table  T_WELL_G_EXPANSION alter column NUM varchar(50) ",
"alter table  T_WELL_G_EXPANSION alter column TEST_DATA varchar(50) ",
"alter table  T_WELL_G_EXPANSION alter column REMARK varchar(50) ",
"alter table  T_WELL_INFOR_CHA alter column WELL_ID varchar(50) ",
"alter table  T_WELL_INFOR_CHA alter column CC varchar(50) ",
"alter table  T_WELL_INFOR_CHA alter column BEFORE_RESULT varchar(50) ",
"alter table  T_WELL_INFOR_CHA alter column REMARK varchar(50) ",
"alter table  T_WELL_INFOR_CHA_CC alter column CATEGORY_NAME varchar(50) ",
"alter table  T_WELL_INFOR_CHA_CC alter column CODE varchar(50) ",
"alter table  T_WELL_INFOR_CHA_CC alter column REMARK varchar(50) ",
"alter table  T_WELL_IWMC alter column IWM_NAME varchar(50) ",
"alter table  T_WELL_IWMC alter column CODE varchar(50) ",
"alter table  T_WELL_IWMC alter column ID varchar(50) ",
"alter table  T_WELL_IWMC alter column REMARK varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column WELL_ID varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column WELL_NAME varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column WB_ID varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column WB_L_SNUM varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column L_ID varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column LITHOLOGIC_TC varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column RESE_EXPLAIN_TC varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column PREC_FACIES_TC varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column PREC_SUBFACIES_TC varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column PREC_MICROFACIES_TC varchar(50) ",
"alter table  T_WELL_L_CUR_INFOR alter column REMARK varchar(50) ",
"alter table  T_WELL_L_LOCATION alter column WELL_ID varchar(50) ",
"alter table  T_WELL_L_LOCATION alter column WELL_NAME varchar(50) ",
"alter table  T_WELL_L_LOCATION alter column WB_ID varchar(50) ",
"alter table  T_WELL_L_LOCATION alter column WB_L_NAME varchar(50) ",
"alter table  T_WELL_L_LOCATION alter column WB_L_SNUM varchar(50) ",
"alter table  T_WELL_L_LOCATION alter column COORD_TYPE varchar(50) ",
"alter table  T_WELL_L_LOCATION alter column REMARK varchar(50) ",
"alter table  T_WELL_PMC alter column PM_NAME varchar(50) ",
"alter table  T_WELL_PMC alter column CODE varchar(50) ",
"alter table  T_WELL_PMC alter column ID varchar(50) ",
"alter table  T_WELL_PMC alter column REMARK varchar(50) ",
"alter table  T_WELL_PROD_MEM alter column WELL_ID varchar(50) ",
"alter table  T_WELL_PROD_MEM alter column WELL_NAME varchar(50) ",
"alter table  T_WELL_PROD_MEM alter column EVENT_TYPE varchar(50) ",
"alter table  T_WELL_PROD_MEM alter column MATTER varchar(50) ",
"alter table  T_WELL_PROD_MEM alter column REMARK varchar(50) ",
"alter table  T_WELL_RESE_CHARACTER alter column WELL_ID varchar(50) ",
"alter table  T_WELL_RESE_CHARACTER alter column WELL_NAME varchar(50) ",
"alter table  T_WELL_RESE_CHARACTER alter column L_REMARK varchar(50) ",
"alter table  T_WELL_RESE_CHARACTER alter column REMARK varchar(50) ",
"alter table  T_WELL_STIMU_REC alter column REMARK varchar(50) ",
"alter table  T_WELL_TC alter column WELL_TYPE_NAME varchar(50) ",
"alter table  T_WELL_TC alter column CODE varchar(50) ",
"alter table  T_WELL_TC alter column ID varchar(50) ",
"alter table  T_WELL_TC alter column REMARK varchar(50) ",
"alter table  V_FDB_D_MEM alter column GEO_UNIT_ID varchar(50) ",
"alter table  V_FDB_D_MEM alter column GEO_UNIT_NAME varchar(50) ",
"alter table  V_FDB_D_MEM alter column REMARK varchar(50) ",
"alter table  V_FDB_D_MEM alter column MATTER varchar(50) ",
"alter table  V_FDB_M_MEM alter column GEO_UNIT_ID varchar(50) ",
"alter table  V_FDB_M_MEM alter column GEO_UNIT_NAME varchar(50) ",
"alter table  V_FDB_M_MEM alter column L_REMARK varchar(50) ",
"alter table  V_FDB_M_MEM alter column INJ_PATTERN varchar(50) ",
"alter table  V_FDB_M_MEM alter column REMARK varchar(50) ",
"alter table  V_FDB_M_MEM alter column MATTER varchar(50) ",
"alter table  V_FDB_Y_MEM alter column GEO_UNIT_ID varchar(50) ",
"alter table  V_FDB_Y_MEM alter column GEO_UNIT_NAME varchar(50) ",
"alter table  V_FDB_Y_MEM alter column REMARK varchar(50) ",
"alter table  V_FDB_Y_MEM alter column MATTER varchar(50) ",
"alter table  V_OWD_INJG alter column WELL_ID varchar(50) ",
"alter table  V_OWD_INJG alter column REMARK varchar(50) ",
"alter table  V_OWD_INJG alter column INJRemark varchar(50) ",
"alter table  V_OWD_INJW alter column WELL_ID varchar(50) ",
"alter table  V_OWD_INJW alter column REMARK varchar(50) ",
"alter table  V_OWD_INJW alter column InjReMark varchar(50) ",
"alter table  V_OWM_INJG alter column WELL_ID varchar(50) ",
"alter table  V_OWM_INJG alter column WELL_NAME varchar(50) ",
"alter table  V_OWM_INJG alter column PROD_L_REMARK varchar(50) ",
"alter table  V_OWM_INJG alter column PMC varchar(50) ",
"alter table  V_OWM_INJG alter column REMARK varchar(50) ",
"alter table  V_OWM_INJG alter column injremark varchar(50) ",
"alter table  V_OWM_INJW alter column WELL_ID varchar(50) ",
"alter table  V_OWM_INJW alter column WELL_NAME varchar(50) ",
"alter table  V_OWM_INJW alter column PROD_L_REMARK varchar(50) ",
"alter table  V_OWM_INJW alter column PMC varchar(50) ",
"alter table  V_OWM_INJW alter column REMARK varchar(50) ",
"alter table  V_OWM_INJW alter column injremark varchar(50) ",
"alter table  VER alter column VER varchar(50) "
 });
            for (int i = 0; i < list.Count; i++)
            {
                sqlstr = list[i].ToString();
                OleDbHelper.EXSQL(sqlstr);
            }
        }
    }
}
