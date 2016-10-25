using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fracture
{
    public partial class Form6 : Form
    {
        public CategoryRow rowMain = new CategoryRow("Main");
        public Form6()
        {
            InitializeComponent();
            //gridcontroltool gridcontroltool1 = new gridcontroltool();
            //gridcontroltool1.InitView(gridView1);
            //this.gridControl1.ContextMenuStrip = gridcontroltool1.tool_menu;
            // adding a category row to the root level 

            vGridControl1.Rows.Add(rowMain);
            vGridControl1.LayoutStyle = LayoutViewStyle.SingleRecordView;
            vGridControl1.CellValueChanged += vGridControl1_CellValueChanged;
            // creating and modifying the Trademark row 
            EditorRow datasmooth = new EditorRow("数据平滑");
            datasmooth.Properties.Caption = "数据平滑";
            RepositoryItemComboBox datasmoothedit = new RepositoryItemComboBox();
            datasmoothedit.Items.AddRange(new string[] { "不处理", "移动窗口平滑", "五次三点平滑" });
            // adding the Trademark row to the Main row's child collection 
            rowMain.ChildRows.Add(datasmooth);
            vGridControl1.RepositoryItems.Add(datasmoothedit);
            //Now you can define the repository item as an inplace editor of columns
            // populating the combo box with data
            datasmooth.Properties.Value = "不处理";
            datasmooth.Properties.RowEdit = datasmoothedit;


            for (int j = 1; j < 3; j++)
            {
                int i = 1;
                // creating and modifying the Model row 


                DataTable dt_TableAndCate = new DataTable();
                //string pathName = Application.StartupPath + "\\ACCESS.xlsx";

                string sheetName = "数据表描述";
                string columname = "数据表类型,表显示名称,库表名称 ";
                string sSql = "SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as KeyID,T_WELL_INFOR.WELL_NAME as Name FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID " +
    "UNION SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name FROM T_DM_UNIT_CUR_INFOR"; //where [{0}$].库表名称=[{1}$].库表名称 

                string TableAndCate = string.Format("select 数据表类型ID AS ParentID ,表显示名称 as Name,库表名称 as KeyID from [{0}$]  UNION select  上级数据表类型 as ParentID, 数据表类型 as Name,数据表类型ID as KeyID from [{0}$]", sheetName);

                dt_TableAndCate = OleDbHelper.ExcelToDataTable(sheetName, TableAndCate);
                //UserControl1  fg=new UserControl1();

                //   fg.DataSource = dt_TableAndCate;

                tgridControl11.SetGridDataView(dt_TableAndCate);
                tgridControl11.gridViewoption();
                tgridControl11.gridView1.OptionsCustomization.AllowColumnMoving = false;
                //tgridControl11.gridView1.OptionsView.ColumnAutoWidth = false;
                //gridView1.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveHorzScroll;
                //gridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always; // or ScrollVisibility.Auto doesn't work neither; or remove this line, doesn't work 
                //gridView1.OptionsView.RowAutoHeight = true;
                //gridView1.BestFitColumns(true);
                //gridView1.OptionsCustomization.AllowColumnMoving = false;

                //gridView1.OptionsSelection.MultiSelect = true;
                //gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                vGridControl1.ShowButtonMode = ShowButtonModeEnum.ShowAlways;

            }


        }

        private void vGridControl1_CellValueChanged(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            MessageBox.Show(e.Value.ToString());
        }

        private void dockPanel1_Click(object sender, EventArgs e)
        {

        }

        private void dockPanel1_Container_Paint(object sender, PaintEventArgs e)
        {

        }

        private void vGridControl1_Click(object sender, EventArgs e)
        {
            //DataTable dt = tgirdControl11.DataSource as DataTable;
            //DataTable dt2 = dt;

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DataTable dt = tgirdControl11.DataSource as DataTable;
            //DataTable dt2 = dt;
        }

        private void tgirdControl11_Load(object sender, EventArgs e)
        {

        }

        private void tgirdControl11_Load_1(object sender, EventArgs e)
        {

        }

        private void vGridControl1_Click_1(object sender, EventArgs e)
        {

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int phasei = 1;
            aprs_fit.deletefitphase(vGridControl1, rowMain, phasei);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            aprs_fit.addfitphase(vGridControl1, rowMain, 1, "指数递减", new DateTime(1990, 1, 1), new DateTime(1990, 1, 1), 2, 1, 1);
        }

        private void vGridControl1_Click_2(object sender, EventArgs e)
        {

        }
    }

    public class aprs_fit
    {
        public string AprsType;//
        public DateTime BeginDate;//
        public DateTime EndDate;//
        public double Qi;
        public double ni;
        public double di;

        public static void addfitphase(DevExpress.XtraVerticalGrid.VGridControl vGridControl1, CategoryRow rowMain, int phasei,
            string AprsType, DateTime BeginDate, DateTime EndDate, double QiValue, double DiValue, double niValue)
        {
            CategoryRow fitphase = new CategoryRow("拟合阶段" + phasei.ToString());
            rowMain.ChildRows.Add(fitphase);

            // creating and modifying the Category row 
            EditorRow rowCategory2 = new EditorRow("拟合模型");
            rowCategory2.Properties.Caption = "拟合模型";
            // adding the Category row to the Model row's child collection 
            fitphase.ChildRows.Add(rowCategory2);
            RepositoryItemComboBox riCombo = new RepositoryItemComboBox();
            riCombo.Items.AddRange(new string[] { "指数递减", "双曲递减", "调和递减" });
            //Add a repository item to the repository items of grid control
            vGridControl1.RepositoryItems.Add(riCombo);
            //Now you can define the repository item as an inplace editor of columns
            // populating the combo box with data
            rowCategory2.Properties.Value = "指数递减";
            rowCategory2.Properties.RowEdit = riCombo;
            // vGridControl1.SetCellValue(rowCategory2, vGridControl1.FocusedRecord, "customText");

            // creating and modifying the Category row 
            EditorRow startdate = new EditorRow("开始时间");
            startdate.Properties.Caption = "开始时间";
            // adding the Category row to the Model row's child collection 
            RepositoryItemDateEdit ridate = vGridControl1.RepositoryItems.Add("DateEdit") as RepositoryItemDateEdit;
            fitphase.ChildRows.Add(startdate);
            startdate.Properties.Value = BeginDate;
            vGridControl1.RepositoryItems.Add(ridate);
            //Now you can define the repository item as an inplace editor of columns
            // populating the combo box with data
            startdate.Properties.RowEdit = ridate;

            EditorRow enddate = new EditorRow("结束时间");
            enddate.Properties.Caption = "结束时间";
            // adding the Category row to the Model row's child collection 
            fitphase.ChildRows.Add(enddate);
            enddate.Properties.Value = 232;
            enddate.Properties.Value = EndDate;
            enddate.Properties.RowEdit = ridate;


            DateTime discount = Convert.ToDateTime(enddate.Properties.Value);
       
            //Create a repository item corresponding to a combo box editor to the persistent repository

            EditorRow Qi = new EditorRow("初始递减产量");
            Qi.Properties.Caption = "初始递减产量";
            // adding the Category row to the Model row's child collection 
            fitphase.ChildRows.Add(Qi);
            Qi.Properties.Value = string.Format("{0}   Km", QiValue);
            //double discoun2t = Convert.ToDouble(Qi.Properties.Value);
            //double dd = discoun2t;
            //Qi.Properties.ShowUnboundExpressionMenu = true;

            EditorRow di = new EditorRow("初始递减率");
            di.Properties.Caption = "初始递减率";
            // adding the Category row to the Model row's child collection 
            fitphase.ChildRows.Add(di);
            di.Properties.Value = DiValue;


            EditorRow ni = new EditorRow("递减指数");
            ni.Properties.Caption = "递减指数";
            // adding the Category row to the Model row's child collection 
            fitphase.ChildRows.Add(ni);
            ni.Properties.Value = niValue;
            RepositoryItemButtonEdit rib = new RepositoryItemButtonEdit();//Button按钮

            //rib.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;//隐藏文字
            rib.Buttons[0].Caption = "Km";
            rib.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;//按钮样式
            ni.Properties.RowEdit = rib;
        }

        public static void deletefitphase(DevExpress.XtraVerticalGrid.VGridControl vGridControl1, CategoryRow rowMain, int removeid)
        {
            rowMain.ChildRows.RemoveAt(removeid);
        }
    }
}
