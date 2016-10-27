using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraCharts;
using System.Drawing.Imaging;
using DevExpress.XtraCharts.Designer;
using Global;
namespace fracture
{
    public partial class productioncurve : Form
    {
        DataTable dt_TableAndField;
        public productioncurve()
        {
            InitializeComponent();
            treeList1.DoubleClick += treeList1_DoubleClick;
            treelistview.InitTreeView(treeList1);
            // Handling the QueryControl event that will populate all automatically generated Documents
            treelistview.AddWellNodes(treeList1);
            SetImageIndex(imageCollection1, treeList1, null, 1, 0);
            //treeList1.CustomDrawNodeCell += treeList1_CustomDrawNodeCell;
        }
        void treeList1_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            if (e.Node.Id == 0)
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
        }
        private void treeList1_DoubleClick(object sender, EventArgs e)
        {
            TreeListNode clickedNode = this.treeList1.FocusedNode;
            if (clickedNode.ParentNode != null)
            {
                //object item = treeList1.FocusedNode;
                string wellid = clickedNode.GetValue("WELLID").ToString();
                DataTable dt;
                dt = GetProductData(wellid);
                if (dt != null)
                {
                    setgridcontrol(dt);
                    //drawcandy(dt);
                    loadTemp(dt);
                    dockPanel2.Show();
                }
                
            }
        }

        private void loadTemp(DataTable dt)
        { 	
            ChartControl chart = chartControl1 ;//= chart;//= new ChartControl(); 
                 chart.Series.Clear();
                 string excelpath = Application.StartupPath + "\\生产月报.xml";
                 chart.LoadFromFile(excelpath);
                 dt.Columns.Add("生产年月YYYYMM", Type.GetType("System.DateTime"));
                 for (int i = 0; i < dt.Rows.Count; i++)
                 {
                     dt.Rows[i]["生产年月YYYYMM"] = DateTime.ParseExact(dt.Rows[i]["生产年月"].ToString(), "yyyyMM", null); //string.Format("{0:yyyy-MM}", "]);//((DateTime)().ToString();
                 };
                 dt.Columns.Remove("生产年月");
                 chart.DataSource = dt;
        }

        private void drawcandy(DataTable dt)
        {
            // Create a chart.
           //

          

            ChartControl chart = chartControl1 ;//= chart;//= new ChartControl(); 
            chart.Series.Clear();
         
            chart.DataSource = dt;
            // Create an empty Bar series and add it to the chart.
            Series series = new Series("月产水量", ViewType.Line);
            chart.Series.Add(series);

            // Generate a data table and bind the series to it.
            series.DataSource = dt;

            // Specify data members to bind the series.
            series.ArgumentScaleType = ScaleType.DateTime;
            series.ArgumentDataMember = "生产年月YYYYMM";
            series.ValueScaleType = ScaleType.Numerical;
            series.ValueDataMembers.AddRange(new string[] { "月产水量" });
            ((PointSeriesView)series.View).Color = Color.FromArgb(125, Color.Green);
            // Create an empty Bar series and add it to the chart.
            Series series1 = new Series("月产油量", ViewType.Line);
            chart.Series.Add(series1);

            // Generate a data table and bind the series to it.
            series1.DataSource = dt;

            // Specify data members to bind the series.
            series1.ArgumentScaleType = ScaleType.DateTime;
            series1.ArgumentDataMember = "生产年月YYYYMM";
            series1.ValueScaleType = ScaleType.Numerical;
            series1.ValueDataMembers.AddRange(new string[] { "月产油量" });
            ((PointSeriesView)series1.View).Color = Color.FromArgb(125, Color.Red);
            Series series2 = new Series("含水", ViewType.Line);
            chart.Series.Add(series2);

            // Generate a data table and bind the series to it.
            series2.DataSource = dt;

            // Specify data members to bind the series.
            series2.ArgumentScaleType = ScaleType.DateTime;
            series2.ArgumentDataMember = "生产年月YYYYMM";
            series2.ValueScaleType = ScaleType.Numerical;
            series2.ValueDataMembers.AddRange(new string[] { "含水" });
            ((PointSeriesView)series2.View).Color = Color.FromArgb(125, Color.Blue);
            // Set some properties to get a nice-looking chart.
            //((SideBySideBarSeriesView)series.View).ColorEach = true;
            ((XYDiagram)chart.Diagram).AxisY.Visibility = DevExpress.Utils.DefaultBoolean.True;
            chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
       

            //SecondaryAxisX myAxisX = new SecondaryAxisX("watercutx");
            SecondaryAxisY myAxisY = new SecondaryAxisY("含水率");

           // ((XYDiagram)chartControl1.Diagram).SecondaryAxesX.Add(myAxisX);

            ((XYDiagram)chartControl1.Diagram).SecondaryAxesY.Add(myAxisY);

            // Assign the series2 to the created axes.
          //  ((LineSeriesView)series2.View).AxisX = myAxisX;
            ((LineSeriesView)series2.View).AxisY = myAxisY;

            // Customize the appearance of the secondary axes (optional).
   

            myAxisY.Title.Text = "含水率（%）";
            myAxisY.Title.Font = new Font("Microsoft YaHei", 10);
            myAxisY.Title.Visible = true;
            myAxisY.Title.TextColor = Color.Blue;
            myAxisY.Label.TextColor = Color.Blue;
            myAxisY.Color = Color.Blue;
            myAxisY.WholeRange.Auto = false;
            myAxisY.WholeRange.SetMinMaxValues(0, 100);
            myAxisY.VisualRange.Auto = false;
            myAxisY.VisualRange.SetMinMaxValues(0, 100);
            myAxisY.WholeRange.SideMarginsValue = 0;
    
            chart.CrosshairOptions.ShowArgumentLabels = true;
            chart.CrosshairOptions.ShowArgumentLine = true;
            chart.CrosshairOptions.ShowValueLabels = true;
            chart.CrosshairOptions.ShowValueLine = true;
            chart.CrosshairOptions.ValueLineColor =Color.DarkBlue;
            
            chart.CrosshairOptions.ArgumentLineColor = Color.DarkBlue;
            chart.CrosshairOptions.ShowCrosshairLabels = false;
            //series1.CrosshairLabelPattern = series1.Name + ":{V:F0}";
            //series2.CrosshairLabelPattern = series1.Name + ":{V:F0}";
           // chart.CrosshairOptions.sh = true;
           
            //series2.CrosshairOptions.ShowArgumentLabels  rosshairOptions.ShowValueLabels
            //  series2.CrosshairOptions.sho  CrosshairOptions.ShowValueLine, 
            //this.Controls.Add();

            DevExpress.XtraCharts.Legend legend = chart.Legend;

            // Display the chart control's legend.
            legend.Visibility = DevExpress.Utils.DefaultBoolean.True;

               // Define its margins and alignment relative to the diagram.
               legend.Margins.All = 8;


            // Define the layout of items within the legend.
            legend.Direction = LegendDirection.TopToBottom;
            legend.EquallySpacedItems = true;
            legend.HorizontalIndent = 8;
            legend.VerticalIndent = 8;
            legend.TextVisible = true;
            legend.TextOffset = 8;
            legend.MarkerVisible = true;
            legend.MarkerSize = new Size(10, 10);
            legend.Padding.All = 4;
            legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Left;
            legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Top;

            // Define the limits for the legend to occupy the chart's space.
            legend.MaxHorizontalPercentage = 50;
            legend.MaxVerticalPercentage = 50;

            // Customize the legend text properties.
            legend.Antialiasing = false;
            legend.Font = new Font("Arial", 9, FontStyle.Bold);
            legend.TextColor = Color.Black;

            XYDiagram diagram = (XYDiagram)chart.Diagram;
            ////diagram.Panes.Clear();
            //diagram.SecondaryAxesY.Clear();
            //diagram.SecondaryAxesX.Clear();

            // Enable the diagram's scrolling.
            diagram.EnableAxisXScrolling = false;
            diagram.EnableAxisYScrolling = false;

            // Customize the appearance of the axes' grid lines.
            diagram.AxisX.GridLines.Visible = true;
            diagram.AxisX.GridLines.MinorVisible = false;

            diagram.AxisY.GridLines.Visible = true;
            diagram.AxisY.GridLines.MinorVisible = true;

            //diagram.AxisY.Range.SetInternalMinMaxValues(1, 12);
            // Customize the appearance of the X-axis title.
            diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisX.Title.Alignment = StringAlignment.Center;
            diagram.AxisX.Title.Text = "生产时间";
            diagram.AxisX.Title.TextColor = Color.Black;
            diagram.AxisX.Title.Antialiasing = true;
            diagram.AxisX.Title.Font = new Font("Microsoft YaHei", 10);

            // Customize the appearance of the Y-axis title.
            diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisY.Title.Alignment = StringAlignment.Center;
            diagram.AxisY.Title.Text = "产量(方)";
            diagram.AxisY.Title.TextColor = Color.Black;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("Microsoft YaHei", 10);
          
        }

        private void setgridcontrol(DataTable dt)
        {

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
        private DataTable GetProductData(string wellid)
        {
            DataTable dt = null;
           
           
            try
            {
                string tablename = "T_OW_M";//采油井生产月报

                //string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
                //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";
                string[] dataname;
                if (dt_TableAndField==null)
                { 
                dt_TableAndField = new DataTable();
                string sheetName = "物理表汇总";

                string TableAndField = string.Format("select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT from [{0}$] where (库表名称='" + tablename + "')", sheetName);
                dt_TableAndField = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);
             
                }
                //          string sSql = "SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as KeyID,T_WELL_INFOR.WELL_NAME as Name FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID " +
                //"UNION SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name FROM T_DM_UNIT_CUR_INFOR";where [{0}$].库表名称 + tablename
                string sSql = "select ";


                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    sSql = sSql + string.Format("{0} AS {1}, ", dt_TableAndField.Rows[i][1], dt_TableAndField.Rows[i][0]);
                }
                sSql = sSql + string.Format("{0} AS {1} From {2} where WELL_ID='{3}'", dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][1], dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][0], tablename,wellid);
                dt = OleDbHelper.getTable(sSql,  Globalname.DabaBasePath);

                return dt;
            }
            catch
            {
                return dt;
            }

        }
        public static void SetImageIndex(DevExpress.Utils.ImageCollection imageCollection1, TreeList treeList1, TreeListNode node, int nodeIndex, int parentIndex)
        {
            treeList1.StateImageList = imageCollection1;

            SetImageIndex(treeList1, null, 1, 0);
        }
        public static void SetImageIndex(TreeList tl, TreeListNode node, int nodeIndex, int parentIndex)
        {



            if (node == null)
            {
                foreach (TreeListNode N in tl.Nodes)
                    SetImageIndex(tl, N, nodeIndex, parentIndex);
            }
            else
            {
                if (node.HasChildren || node.ParentNode == null)
                {
                    //node.SelectImageIndex = parentIndex; 
                    node.StateImageIndex = parentIndex;
                    node.ImageIndex = parentIndex;
                }
                else
                {
                    //node.SelectImageIndex = nodeIndex; 
                    node.StateImageIndex = nodeIndex;
                    node.ImageIndex = nodeIndex;
                }

                foreach (TreeListNode N in node.Nodes)
                {
                    SetImageIndex(tl, N, nodeIndex, parentIndex);
                }
            }
        }

        private void btn_outpic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FileOperate.savechart(chartControl1);
        }

     

        private void btn_excel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string localFilePath = FileOperate.saveopenfile();
            if (localFilePath != null)
            {
                FileOperate.ExportToExcel(gridView1, localFilePath);
            }
        }

        private void btn_design_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ChartDesigner designer = new ChartDesigner(chartControl1);
            designer.ShowDialog();
        }

      

     

    }
}
