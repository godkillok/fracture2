using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraRichEdit.API.Native;
using System.Drawing.Printing;
using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.API;
using Global;
namespace fracture
{
    public partial class Stimu_decline : Form
    {
        XYDiagram Diagram;
        ChartControl myChart;
        DataTable dt_TableAndField;
        DataTable dt_TableAndField_Stimu;
        DateTime stimu_date;
        string stimu_code;
        string wellname;
        public bool selected_flag = false;
        List<double> getData;//输入的产油量
        List<double> decData;//拟合段的时间和产油量
        List<double[]> ployfitinfo;//拟合段信息汇总，包括：开始阶段，结束阶段，QI,DI,N,拟合相关系数
        List<DateTime> DataTime;//输入的时间
        //XYDiagram Diagram { get { return myChart.Diagram as XYDiagram; } }
        bool dragging = false;
        public Point firstPoint = new Point(0, 0);  //鼠标第一点 ，对应屏幕
        public Point secondPoint = new Point(0, 0);  //鼠标第二点 
        DiagramCoordinates seccoords;//对应图中坐标系的值
        DiagramCoordinates firstcoords;//对应图中坐标系的值
        ControlCoordinates maxcoords;//图中对应p屏幕坐标系的值
        ControlCoordinates mincoords;//图中对应p屏幕坐标系的值
        int seriesnum;//图中series的数目
        int phase_num;//添加的阶段数
        Rectangle selectionRectangle = Rectangle.Empty;
        DevExpress.XtraEditors.Repository.RepositoryItemComboBox ComboBoxproperties;
        DataSet result_dtset;
        List<arps> pfitinfo;
        public class Record
        {
            DateTime xaxis; double yaxis;
            public Record(DateTime xaxis, double yaxis)
            {
                this.xaxis = xaxis;
                this.yaxis = yaxis;
            }
            public DateTime Xaxis
            {
                get { return xaxis; }
                set { xaxis = value; }
            }

            public double Yaxis
            {
                get { return yaxis; }
                set { yaxis = value; }
            }
        }
        public class arps
        {
            private string name;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            private ArrayList xy;

            public ArrayList Xy
            {
                get { return xy; }
                set { xy = value; }
            }

            private int endpoint;

            public int Endpoint
            {
                get { return endpoint; }
                set { endpoint = value; }
            }
            private int beginpoint;

            public int Beginpoint
            {
                get { return beginpoint; }
                set { beginpoint = value; }
            }



            private DateTime enddate;


            public DateTime Enddate
            {
                get { return enddate; }
                set { enddate = value; }
            }

            private DateTime begindate;

            public DateTime Begindate
            {
                get { return begindate; }
                set { begindate = value; }
            }

            private double qi;

            public double Qi
            {
                get { return qi; }
                set { qi = value; }
            }



            private double ni;

            public double Ni
            {
                get { return ni; }
                set { ni = value; }
            }

            private double di;

            public double Di
            {
                get { return di; }
                set { di = value; }
            }


        }
        public Stimu_decline()
        {

            Stopwatch myWatch = Stopwatch.StartNew();
            InitializeComponent();
            treeList1.DoubleClick += treeList1_DoubleClick;
            //spreadsheetControl1.Options.TabSelector.Visibility = DevExpress.XtraSpreadsheet.SpreadsheetElementVisibility.Hidden;
            ComboBoxproperties = comboBoxEdit1.Properties;
            btn_excel_output.Enabled = false;
            //Disable editing 
            //添加下拉框的内容
            ComboBoxproperties.Items.AddRange(new string[] { "下拉选择递减段" });
            //Select the first item 
            comboBoxEdit1.SelectedIndex = 0;
            ComboBoxproperties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            myWatch.Stop();
            //MessageBox.Show(myWatch.ElapsedMilliseconds.ToString());
            ribbonControl1.SelectedPage = ribbonControl1.PageCategories[0].Pages[0];
            dockPanel3.Show();

            treelistview.InitTreeView(treeList1);
            treelistview.AddWellStimuNodes(treeList1);
            SetImageIndex(imageCollection1, treeList1, null, 1, 0);
            snapControl1.Visible = false;

        }
        public static void SetImageIndex(DevExpress.Utils.ImageCollection imageCollection1, TreeList treeList1, TreeListNode node, int nodeIndex, int parentIndex)
        {
            treeList1.StateImageList = imageCollection1;

            SetImageIndex(treeList1, null, 1, 2, 0);
        }
        public static void SetImageIndex(TreeList tl, TreeListNode node, int oilnodeIndex, int waternodeindex, int parentIndex)
        {
            if (node == null)
            {
                foreach (TreeListNode N in tl.Nodes)
                    SetImageIndex(tl, N, oilnodeIndex, waternodeindex, parentIndex);
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
                    if (node.GetValue("wellCode").ToString() == "注水井")
                    {
                        node.StateImageIndex = waternodeindex;
                        node.ImageIndex = waternodeindex;
                    }
                    else
                    {
                        node.StateImageIndex = oilnodeIndex;
                        node.ImageIndex = oilnodeIndex;

                    }
                }

                foreach (TreeListNode N in node.Nodes)
                {
                    SetImageIndex(tl, N, oilnodeIndex, waternodeindex, parentIndex);
                }
            }
        }
        private void treeList1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                TreeListNode clickedNode = this.treeList1.FocusedNode;
                if (clickedNode.ParentNode != null)
                {
                    //object item = treeList1.FocusedNode;
                    string wellid = clickedNode.GetValue("ParentID").ToString();
                    DataTable dt;
                    dt = GetProductData(wellid);


                    if (dt != null)
                    {
                        stimu_date = DateTime.Parse(clickedNode.GetValue("wellCode").ToString());
                        stimu_code = clickedNode.GetValue("Name").ToString().Split('@')[0];
                        wellname = clickedNode.GetValue("ParentID").ToString();
                        setgridcontrol(dt);

                        //drawcandy(dt);
                        loadTemp(dt);
                        dockPanel3.Show();
                    }
                }
            }
            catch
            {

            }
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
            //dt_TableAndField是取单位和字段名的这儿就写死了


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
        }


        private void ge(List<DateTime> DataTime, List<double> getData, DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                DataTime.Add(DateTime.Parse(dr["生产日期"].ToString()));
                getData.Add(double.Parse(dr["日产油量"].ToString()));
            }

        }
        private void loadTemp(DataTable dt)
        {

            getData = new List<double>();
            DataTime = new List<DateTime>();
            pfitinfo = new List<arps>();
            ComboBoxproperties.Items.Clear();
            ComboBoxproperties.Items.AddRange(new string[] { "下拉选择递减段" });
            //Select the first item 
            comboBoxEdit1.SelectedIndex = 0;


            ge(DataTime, getData, dt);
            //getData = dt.Columns["日产油量"];

            if (dt == null)
                return;
            // Bind the chart to the list. 
            // ChartControl myChart = myChart;
            //ChartControl myChart = new ChartControl();
            myChart = chartControl1;
            myChart.Series.Clear();


            myChart.DataSource = dt;
            myChart.CrosshairOptions.ShowCrosshairLabels = false;
            myChart.CrosshairOptions.ShowArgumentLine = false;
            myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
            myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;


            // Create a series, and add it to the chart. 
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series("生产数据", ViewType.Point);
            myChart.Series.Add(series1);
            seriesnum = myChart.Series.Count;
            phase_num = 1;
            // Set the scale type for the series' arguments and values.
            series1.ArgumentScaleType = ScaleType.DateTime;
            series1.ValueScaleType = ScaleType.Numerical;


            // Adjust the series data members. 
            series1.ArgumentDataMember = "生产日期";
            series1.ValueDataMembers.AddRange(new string[] { "日产油量" });
            //myChart.DateTimeScaleOptions.MeasureUnit = Month
            // Access the view-type-specific options of the series. 
            ((PointSeriesView)series1.View).Color = Color.FromArgb(125, Color.Red);

            //series1.LegendTextPattern = "{A}";
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            //series1.Label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAllAroundPoint;
            DevExpress.XtraCharts.Legend legend = myChart.Legend;

            // Display the chart control's legend.
            legend.Visible = true;

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
            legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Bottom;

            // Define the limits for the legend to occupy the chart's space.
            legend.MaxHorizontalPercentage = 50;
            legend.MaxVerticalPercentage = 50;

            // Customize the legend text properties.
            legend.Antialiasing = false;
            legend.Font = new Font("Arial", 9, FontStyle.Bold);
            legend.TextColor = Color.Black;

            XYDiagram diagram = (XYDiagram)myChart.Diagram;
            Diagram = (XYDiagram)myChart.Diagram;

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
            diagram.AxisX.Title.Visible = true;
            diagram.AxisX.Title.Alignment = StringAlignment.Center;
            diagram.AxisX.Title.Text = "生产时间";
            diagram.AxisX.Title.TextColor = Color.Black;
            diagram.AxisX.Title.Antialiasing = true;
            diagram.AxisX.Title.Font = new Font("Microsoft YaHei", 10);

            // Customize the appearance of the Y-axis title.
            diagram.AxisY.Title.Visible = true;
            diagram.AxisY.Title.Alignment = StringAlignment.Center;
            diagram.AxisY.Title.Text = "产油量";
            diagram.AxisY.Title.TextColor = Color.Black;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("Microsoft YaHei", 10);
            //int maxaxisx = 1100;
            //int maxaxisy = 1100;
            //int minaxisx = 1000;
            //int minaxisy = 700;
            // Create a constant line.
            while (diagram.AxisX.ConstantLines.Count > 0)
                diagram.AxisX.ConstantLines.RemoveAt(diagram.AxisX.ConstantLines.Count - 1);


            ConstantLine constantLine1 = new ConstantLine("Constant Line 1");
            diagram.AxisX.ConstantLines.Add(constantLine1);

            // Define its axis value.
            constantLine1.AxisValue = stimu_date;

            // Customize the behavior of the constant line.
            constantLine1.Visible = true;
            constantLine1.ShowInLegend = false;
            constantLine1.LegendText = stimu_code;
            constantLine1.ShowBehind = false;
            constantLine1.Title.Text = "措施开始时间";
            constantLine1.Title.Alignment = ConstantLineTitleAlignment.Far;


        }

        private DataTable GetProductData(string wellid)
        {
            DataTable dt = null;
            try
            {
                string tablename = "T_OW_d";//采油井生产月报

                //string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
                //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";


                if (dt_TableAndField == null)
                {
                    dt_TableAndField = new DataTable();
                    string sheetName = "物理表汇总";

                    string TableAndField = string.Format("select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT from [{0}$] where (库表名称='" + tablename + "' and 库字段名称 in ('WELL_ID','WELL_NAME','PROD_DATE','D_O','NP')" + ")", sheetName);
                    dt_TableAndField = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);

                }

                string sSql = "select ";
                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    sSql = sSql + string.Format("{0} AS {1}, ", dt_TableAndField.Rows[i][1], dt_TableAndField.Rows[i][0]);
                }
                sSql = sSql + string.Format("{0} AS {1} From {2} where WELL_ID='{3}' order by PROD_DATE", dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][1], dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][0], tablename, wellid);
                dt = OleDbHelper.getTable(sSql, Globalname.DabaBasePath);

                return dt;
            }
            catch
            {
                return dt;
            }

        }

        public class Record3
        {
            DateTime xaxis; double yaxis1; double yaxis2;
            public Record3(DateTime xaxis, double yaxis1, double yaxis2)
            {
                this.xaxis = xaxis;
                this.yaxis1 = yaxis1;
                this.yaxis2 = yaxis2;
            }
            public DateTime Xaxis
            {
                get { return xaxis; }
                set { xaxis = value; }
            }

            public double Yaxis1
            {
                get { return yaxis1; }
                set { yaxis2 = value; }
            }
            public double Yaxis2
            {
                get { return yaxis2; }
                set { yaxis2 = value; }
            }
        }


        public ArrayList list(List<double> getData, List<DateTime> DataTime)
        {

            ArrayList list = new ArrayList();
            // Populate the list with records. 
            for (int i = 0; i < getData.Count; i++)
            {
                Record d1 = new Record(DataTime[i], getData[i]);
                list.Add(d1);
            }
            return list;
        }


        #region 图形拖动
        private void chartControl1_MouseMove(object sender, MouseEventArgs e)
        {

            if (Diagram == null)
                return;

            if (dragging && e.Button != MouseButtons.Right)
            {

                //获取新的右下角坐标   
                secondPoint = new Point(e.X, e.Y);
                if (e.X > maxcoords.Point.X)
                {
                    secondPoint.X = maxcoords.Point.X;
                }
                if (e.X < mincoords.Point.X)
                {
                    secondPoint.X = mincoords.Point.X;
                }
                if (e.Y < maxcoords.Point.Y)
                {
                    secondPoint.Y = maxcoords.Point.Y;
                }
                if (e.Y > mincoords.Point.Y)
                {
                    secondPoint.Y = mincoords.Point.Y;
                }
                int minX = Math.Min(firstPoint.X, secondPoint.X);
                int minY = maxcoords.Point.Y;
                int maxX = Math.Max(firstPoint.X, secondPoint.X);
                int maxY = mincoords.Point.Y;



                selectionRectangle = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }

        }
        private void chartControl1_CustomPaint(object sender, CustomPaintEventArgs e)
        {
            if (Diagram == null)
                return;
            Graphics g = e.Graphics;
            Brush solidcor = new SolidBrush(Color.FromArgb(205, Color.LightGoldenrodYellow));
            g.DrawRectangle(new Pen(Color.FromArgb(205, Color.Yellow)), selectionRectangle);
            g.FillRectangle(solidcor, selectionRectangle);
            //maxcoords = Diagram.DiagramToPoint((DateTime)Diagram.AxisX.WholeRange.MaxValue, (double)Diagram.AxisY.WholeRange.MaxValue);
            //mincoords = Diagram.DiagramToPoint((DateTime)Diagram.AxisX.WholeRange.MinValue, (double)Diagram.AxisY.WholeRange.MinValue);
            maxcoords = Diagram.DiagramToPoint((DateTime)Diagram.AxisX.WholeRange.MaxValue, (double)Diagram.AxisY.WholeRange.MaxValue + Diagram.AxisY.WholeRange.SideMarginsValue);
            mincoords = Diagram.DiagramToPoint((DateTime)Diagram.AxisX.WholeRange.MinValue, (double)Diagram.AxisY.WholeRange.MinValue);//- Diagram.AxisY.WholeRange.SideMarginsValue


            //maxcoords = Diagram.DiagramToPoint((double)Diagram.AxisX.VisualRange.MaxValue, (double)Diagram.AxisY.VisualRange.MaxValue);
            //mincoords = Diagram.DiagramToPoint((double)Diagram.AxisX.VisualRange.MinValue, (double)Diagram.AxisY.VisualRange.MinValue);
            //MessageBox.Show(Diagram.AxisX.VisualRange.MaxValue.ToString());    
        }
        private void chartControl1_MouseDown(object sender, MouseEventArgs e)
        {

            if (Diagram == null)
                return;
            if (e.Button != MouseButtons.Left)
                return;

            DiagramCoordinates coords = Diagram.PointToDiagram(e.Location);
            if (!coords.IsEmpty)
            {
                dragging = true;
                myChart.Capture = true;
                firstPoint = new Point(e.X, e.Y);

                if (e.X > maxcoords.Point.X)
                {
                    firstPoint.X = maxcoords.Point.X;
                }
                if (e.X < mincoords.Point.X)
                {
                    firstPoint.X = mincoords.Point.X;
                }
                if (e.Y < maxcoords.Point.Y)
                {
                    firstPoint.Y = maxcoords.Point.Y;
                }
                if (e.Y > mincoords.Point.Y)
                {
                    firstPoint.Y = mincoords.Point.Y;
                }
            }
        }
        private void chartControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Diagram == null)
                return;
            dragging = false;
            myChart.Capture = false;
            secondPoint = new Point(e.X, e.Y);
            int minX = Math.Min(firstPoint.X, secondPoint.X);
            int minY = Math.Min(firstPoint.Y, secondPoint.Y);
            int maxX = Math.Max(firstPoint.X, secondPoint.X);
            int maxY = Math.Max(firstPoint.Y, secondPoint.Y);
            if (e.X > maxcoords.Point.X)
            {
                secondPoint.X = maxcoords.Point.X;
            }
            if (e.X < mincoords.Point.X)
            {
                secondPoint.X = mincoords.Point.X;
            }

            if (e.Y < maxcoords.Point.Y)
            {
                secondPoint.Y = maxcoords.Point.Y;
            }
            if (e.Y > mincoords.Point.Y)
            {
                secondPoint.Y = mincoords.Point.Y;
            }

            seccoords = Diagram.PointToDiagram(secondPoint);
            firstcoords = Diagram.PointToDiagram(firstPoint);
            selectionRectangle = Rectangle.Empty;
            selected_flag = true;
            polyfit_decline();
            selected_flag = false;
        }
        #endregion 图形拖动
        private void polyfit_decline()
        {
            int[] selectinfo;//拟合段的开始时间和拟合的时间的个数
            algorithm alg = new algorithm();
            List<DateTime> resultTime = new List<DateTime>();
            selectinfo = new int[2];//拟合段的开始时间和拟合的时间的个数
            if (selected_flag == false)
                return;
            int i = 0;
            bool frist_enter = false;
            int countnum = 0;
            decData = new List<double>();//拟合段的时间和产油量
            if (seccoords.DateTimeArgument < firstcoords.DateTimeArgument)
            {
                for (i = 0; i < DataTime.Count; i++)
                {
                    if (DataTime[i] >= seccoords.DateTimeArgument & DataTime[i] <= firstcoords.DateTimeArgument & frist_enter == true)
                    {
                        frist_enter = true;
                        decData.Add(getData[i]);
                        resultTime.Add(DataTime[i]);
                        countnum = countnum + 1;
                        selectinfo[1] = i;
                    }
                    if (DataTime[i] >= seccoords.DateTimeArgument & DataTime[i] <= firstcoords.DateTimeArgument & frist_enter == false)
                    {
                        frist_enter = true;
                        decData.Add(getData[i]);
                        resultTime.Add(DataTime[i]);
                        selectinfo[0] = i;
                        countnum = countnum + 1;
                    }
                }
            }
            if (seccoords.DateTimeArgument > firstcoords.DateTimeArgument)
            {
                for (i = 0; i < DataTime.Count; i++)
                {
                    if (DataTime[i] <= seccoords.DateTimeArgument & DataTime[i] >= firstcoords.DateTimeArgument & frist_enter == true)
                    {
                        frist_enter = true;
                        decData.Add(getData[i]);
                        resultTime.Add(DataTime[i]);
                        countnum = countnum + 1;
                        selectinfo[1] = i;//结束的点
                    }
                    if (DataTime[i] <= seccoords.DateTimeArgument & DataTime[i] >= firstcoords.DateTimeArgument & frist_enter == false)
                    {
                        frist_enter = true;
                        decData.Add(getData[i]);
                        resultTime.Add(DataTime[i]);
                        selectinfo[0] = i;//开始的点
                        countnum = countnum + 1;
                    }
                }
            }
            //强制转化为开头小，结束大
            if (selectinfo[0] > selectinfo[1])
            {
                int tempselect = selectinfo[0];
                selectinfo[0] = selectinfo[1];
                selectinfo[1] = tempselect;
            }

            if (countnum < 2)//防止没有选到不合理的值进行拟合
                return;
            int math_predic_num = int.Parse(longgest_num.Text);

            int profit_predic_num = countnum + (getData.Count - selectinfo[1]);
            DateTime lastone = resultTime[resultTime.Count - 1];
            for (int iii = 1; iii <= int.Parse(longgest_num.Text); iii++)
            {
                if (DataTime[1].Year - DataTime[0].Year == 0)
                {
                    if (DataTime[1].Month - DataTime[0].Month == 0)
                    {
                        int span = DataTime[1].Day - DataTime[0].Day;
                        resultTime.Add(lastone.AddDays(iii * span));
                    }
                    else
                    {
                        int span = DataTime[1].Month - DataTime[0].Month;
                        resultTime.Add(lastone.AddMonths(iii * span));
                    }
                }
                else
                {
                    int span = DataTime[1].Year - DataTime[0].Year;
                    resultTime.Add(lastone.AddYears(iii * span));
                }

            }
            List<double> Result = new List<double>();
            double[] parameter_result = new double[3];

            if (decMethodchecked.SelectedIndex == 0)//指数递减
            {
                alg.ExponentialDec(decData, Result, parameter_result, profit_predic_num);
                string seriname = "";
                if (phase_num == 1)
                    seriname = "措施前拟合指数递减";
                if (phase_num == 2)
                    seriname = "措施后拟合指数递减";
                Color col = Color.FromArgb(125, Color.Blue);
                double correlative = correl(decData, Result);
                addseries(Result, resultTime, seriname, col, parameter_result, selectinfo, correlative);
            }

            if (decMethodchecked.SelectedIndex == 1)//双曲递减
            {
                //bool resolved;
                //List<double> HyperResult = new List<double>();
                //double[] Hyper_parameter_result = new double[3];
                alg.HyperbolicDec2(decData, Result, parameter_result, profit_predic_num);
                string seriname = "";
                if (phase_num == 1)
                    seriname = "措施前拟合双曲递减";
                if (phase_num == 2)
                    seriname = "措施后拟合双曲递减";
                Color col = Color.FromArgb(125, Color.Indigo);
                double correlative = correl(decData, Result);
                addseries(Result, resultTime, seriname, col, parameter_result, selectinfo, correlative);
            }

            if (decMethodchecked.SelectedIndex == 2)//调和递减
            {
                //List<double> HarmResult = new List<double>();
                //double[] Harm_parameter_result = new double[3];
                alg.HarmonicDec(decData, Result, parameter_result, profit_predic_num);
                string seriname = "";
                if (phase_num == 1)
                    seriname = "措施前拟合调和递减";
                if (phase_num == 2)
                    seriname = "措施后拟合调和递减";
                Color col = Color.FromArgb(125, Color.Green);
                double correlative = correl(decData, Result);
                addseries(Result, resultTime, seriname, col, parameter_result, selectinfo, correlative);
            }

        }
        private void addseries(List<double> Result, List<DateTime> resultTime, string seriname, Color col, double[] parameter_result, int[] selectinfo, double correlative)
        {

            //importdata(Result, resultTime);//输入数据
            ArrayList listDataSource = list(Result, resultTime);      //画图数据源

            if (resultTime.Count == 0 | Result.Count == 0)
                return;
            // Bind the chart to the list. 
            myChart.CrosshairOptions.ShowCrosshairLabels = true;
            myChart.CrosshairOptions.ShowArgumentLine = false;
            myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
            myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;
            //MessageBox.Show(myChart.Series.Count.ToString());
            int seiresi = seriesnum;
            int modelnum = 1;
            seiresi = seriesnum;
            //ConstantLine constantLine2 = new ConstantLine("Constant Line 2");
            XYDiagram diagram = (XYDiagram)myChart.Diagram;

            while (diagram.AxisX.ConstantLines.Count() > 1)
                diagram.AxisX.ConstantLines.RemoveAt(diagram.AxisX.ConstantLines.Count() - 1);

            int ik = 0;

            while (myChart.Series.Count >= seriesnum + modelnum)
            {
                if (comboBoxEdit1.SelectedIndex == 0)
                    return;
                if (ik > myChart.Series.Count - 1)
                    break;
                if (myChart.Series[ik].Name.ToString() == comboBoxEdit1.SelectedItem.ToString())
                {
                    myChart.Series.RemoveAt(ik);
                    ComboBoxproperties.Items.RemoveAt(comboBoxEdit1.SelectedIndex);
                    break;
                }
                ik++;
            }
            // Create a series, and add it to the chart. 
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series(seriname, ViewType.Line);
            //添加拟合段信息
            if (pfitinfo == null)
            {
                arps pfitinfo_temp = new arps();
                pfitinfo_temp.Name = seriname;
                pfitinfo_temp.Beginpoint = (int)selectinfo[0];
                pfitinfo_temp.Endpoint = (int)selectinfo[1];
                pfitinfo_temp.Ni = parameter_result[0];
                pfitinfo_temp.Qi = parameter_result[1];
                pfitinfo_temp.Di = parameter_result[2];
                pfitinfo.Add(pfitinfo_temp);
            }
            else
            {
                bool flag_find = false;
                for (int i = 0; i < pfitinfo.Count; i++)
                {
                    if (pfitinfo[i].Name == seriname)
                    {
                        pfitinfo[i].Beginpoint = (int)selectinfo[0];
                        pfitinfo[i].Endpoint = (int)selectinfo[1];
                        pfitinfo[i].Ni = parameter_result[0];
                        pfitinfo[i].Qi = parameter_result[1];
                        pfitinfo[i].Di = parameter_result[2];
                        flag_find = true;
                    }
                }
                if (flag_find == false)
                {
                    arps pfitinfo_temp = new arps();
                    pfitinfo_temp.Name = seriname;
                    pfitinfo_temp.Beginpoint = (int)selectinfo[0];
                    pfitinfo_temp.Endpoint = (int)selectinfo[1];
                    pfitinfo_temp.Ni = parameter_result[0];
                    pfitinfo_temp.Qi = parameter_result[1];
                    pfitinfo_temp.Di = parameter_result[2];
                    pfitinfo.Add(pfitinfo_temp);
                }

            }




            {
                //添加下拉框的内容
                ComboBoxproperties.Items.AddRange(new string[] { seriname });
                textQi.Text = pfitinfo[pfitinfo.Count - 1].Qi.ToString("f2");
                textDi.Text = pfitinfo[pfitinfo.Count - 1].Di.ToString("f5");
                textn.Text = pfitinfo[pfitinfo.Count - 1].Ni.ToString("f5");
            }
            myChart.Series.Add(series1);
            series1.DataSource = listDataSource;

            // Set the scale type for the series' arguments and values.
            series1.ArgumentScaleType = ScaleType.DateTime;
            series1.ValueScaleType = ScaleType.Numerical;
            series1.CrosshairHighlightPoints = DevExpress.Utils.DefaultBoolean.False;

            // Adjust the series data members. 
            series1.ArgumentDataMember = "Xaxis";
            series1.ValueDataMembers.AddRange(new string[] { "Yaxis" });
            //myChart.DateTimeScaleOptions.MeasureUnit = Month
            // Access the view-type-specific options of the series. 

            ((PointSeriesView)series1.View).Color = col;// Color.FromArgb(125, Color.Blue);

            //series1.LegendTextPattern = "{A}";
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            comboBoxEdit1.SelectedIndex = ComboBoxproperties.Items.Count - 1;
        }

        private void btnNull_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int seiresi = seriesnum;
            while (myChart.Series.Count > seriesnum)
            {
                myChart.Series.RemoveAt(seiresi);
            }
        }

        private void btnSelect_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Diagram == null)
                return;

        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (myChart.Series.Count != 2)
                return;
            seriesnum = myChart.Series.Count;
            phase_num = phase_num + 1;
        }

        private void btnmanul_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (pfitinfo == null)
            //    return;
            //if (pfitinfo.Count == 0)
            //    return;
            //if (comboBoxEdit1.SelectedIndex == 0)
            //    return;
            //List<double> result = new List<double>();
            //List<DateTime> resultTime = new List<DateTime>();
            //int math_predic_num = int.Parse(longgest_num.Text);
            //int i = 0;
            //while (i < pfitinfo.Count)
            //{
            //    if (comboBoxEdit1.SelectedItem == pfitinfo[i].Name)
            //    {
            //        int countnum = (int)(ployfitinfo[i - 1][1] - ployfitinfo[i - 1][0]) + 1;
            //        pfitinfo[i].Qi = double.Parse(textQi.Text);
            //        pfitinfo[i].Di = double.Parse(textDi.Text);
            //        pfitinfo[i].Ni= double.Parse(textn.Text);

            //        //重新计算拟合线
            //        double n = double.Parse(textn.Text); ;
            //        double Qi = double.Parse(textQi.Text); ;
            //        double Di = double.Parse(textDi.Text); ;
            //        //获得预测产油量
            //        if (comboBoxEdit1.SelectedItem.Contains("指数"))
            //        {
            //            for (int j = 0; j < countnum + math_predic_num; j++)
            //            {
            //                result.Add(Qi * System.Math.Exp((-Di) * j));
            //            }

            //        }

            //        else if (comboBoxEdit1.SelectedItem.Contains("双曲"))
            //        {
            //            for (int j = 0; j < countnum + math_predic_num; j++)
            //            {
            //                result.Add(Qi / (System.Math.Pow((1 + n * Di * (j)), 1 / n)));
            //            }

            //        }
            //        else if (comboBoxEdit1.SelectedItem.Contains("调和"))
            //        {
            //            for (int j = 0; j < countnum + math_predic_num; j++)
            //            {
            //                result.Add(Qi / ((Di) * j + 1));
            //            }

            //        }

            //        //获得时间
            //        for (int j = (int)ployfitinfo[i - 1][0]; j <= (int)ployfitinfo[i - 1][1]; j++)
            //            resultTime.Add(DataTime[j]);
            //        DateTime lastone = resultTime[resultTime.Count - 1];
            //        for (int iii = 1; iii <= int.Parse(longgest_num.Text); iii++)
            //        {
            //            if (DataTime[1].Year - DataTime[0].Year == 0)
            //            {
            //                if (DataTime[1].Month - DataTime[0].Month == 0)
            //                {
            //                    int span = DataTime[1].Day - DataTime[0].Day;
            //                    resultTime.Add(lastone.AddDays(iii * span));
            //                }
            //                else
            //                {
            //                    int span = DataTime[1].Month - DataTime[0].Month;
            //                    resultTime.Add(lastone.AddMonths(iii * span));
            //                }
            //            }
            //            else
            //            {
            //                int span = DataTime[1].Year - DataTime[0].Year;
            //                resultTime.Add(lastone.AddYears(iii * span));
            //            }

            //        }

            //        //重画图
            //        redrawseries(i, result, resultTime);
            //        break;

            //    }
            //    else
            //        i++;
            //}

        }
        private void redrawseries(int seiresi, List<double> Result, List<DateTime> resultTime)
        {

            //importdata(Result, resultTime);//输入数据
            ArrayList listDataSource = list(Result, resultTime);      //画图数据源

            if (resultTime.Count == 0 | Result.Count == 0)
                return;
            // Bind the chart to the list. 

            //MessageBox.Show(myChart.Series.Count.ToString());

            DevExpress.XtraCharts.Series series1 = myChart.Series[seiresi];
            //myChart.Series.Add(series1);
            series1.DataSource = listDataSource;


        }

        public ArrayList list(List<double> incre_result, List<double> result_aft, List<DateTime> resultTime_aft)
        {

            ArrayList list = new ArrayList();
            // Populate the list with records. 
            for (int i = 0; i < resultTime_aft.Count; i++)
            {
                Record3 d1 = new Record3(resultTime_aft[i], incre_result[i] + result_aft[i], result_aft[i]);
                list.Add(d1);
            }
            return list;
        }
        private void stimu_draw(DateTime faildate, List<double> incre_result, List<double> result_aft, List<DateTime> resultTime_aft)
        {
            XYDiagram diagram = (XYDiagram)myChart.Diagram;
            ConstantLine constantLine2 = new ConstantLine("Constant Line 2");
            diagram.AxisX.ConstantLines.Add(constantLine2);

            // Define its axis value.
            constantLine2.AxisValue = faildate;

            // Customize the behavior of the constant line.
            constantLine2.Visible = true;
            constantLine2.ShowInLegend = false;
            constantLine2.LegendText = "措施失效";
            constantLine2.ShowBehind = false;
            constantLine2.Title.Text = "措施失效";
            constantLine2.Title.Alignment = ConstantLineTitleAlignment.Far;

            ArrayList listDataSource = list(incre_result, result_aft, resultTime_aft);      //画图数据源

            myChart.CrosshairOptions.ShowCrosshairLabels = false;
            myChart.CrosshairOptions.ShowArgumentLine = false;
            myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
            myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;

            //添加下拉框的内容
            if (!ComboBoxproperties.Items.Contains("增油面积"))
                ComboBoxproperties.Items.AddRange(new string[] { "增油面积" });



            // Create a series, and add it to the chart. 
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series("增油面积", ViewType.RangeArea);
            myChart.Series.Add(series1);
            series1.DataSource = listDataSource;


            // Set the scale type for the series' arguments and values.
            series1.ArgumentScaleType = ScaleType.DateTime;
            series1.ValueScaleType = ScaleType.Numerical;


            // Adjust the series data members. 
            series1.ArgumentDataMember = "Xaxis";
            series1.ValueDataMembers.AddRange(new string[] { "Yaxis1", "Yaxis2" });
            //myChart.DateTimeScaleOptions.MeasureUnit = Month
            // Access the view-type-specific options of the series. 
            ((PointSeriesView)series1.View).Color = Color.FromArgb(125, Color.Orange);

            //series1.LegendTextPattern = "{A}";
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            //series1.Label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAllAroundPoint;
            DevExpress.XtraCharts.Legend legend = myChart.Legend;

            ((XYDiagram)myChart.Diagram).AxisX.WholeRange.MaxValue = resultTime_aft.Max().AddMonths(1);


        }
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (pfitinfo == null)
                return;
            int i = 1;
            if (comboBoxEdit1.SelectedItem.ToString().Contains("措施后"))
            {
                phase_num = 2;
            }
            else
            {
                phase_num = 1;
            }
            while (i < pfitinfo.Count)
            {
              
                if (comboBoxEdit1.SelectedItem.ToString() == pfitinfo[i].Name)
                {
                    textQi.Text = pfitinfo[i].Qi.ToString("f2");
                    textDi.Text = pfitinfo[i].Di.ToString("f5");
                    textn.Text = pfitinfo[i].Ni.ToString("f5");
                    break;
                }
                else
                    i++;
            }
        }


        private void btncal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (pfitinfo == null)
                return;
            if (pfitinfo.Count == 0)
                return;
            if (comboBoxEdit1.SelectedIndex == 0)
                return;
            myChart = chartControl1;
            for (int ik = 0; ik < myChart.Series.Count; ik++)
            {
                if (myChart.Series[ik].Name == "增油面积")
                    myChart.Series.RemoveAt(ik);

            }

            List<double> result_bef = new List<double>();
            List<double> result_aft = new List<double>();
            List<double> his_result = new List<double>();
            List<double> increase_result = new List<double>();
            List<DateTime> resultTime = new List<DateTime>();
            List<DateTime> resultTime_aft = new List<DateTime>();
            int math_predic_num = int.Parse(longgest_num.Text);

            //i-1表示措施前拟合线 i表示措施后拟合线 
            for (int i = 0; i < pfitinfo.Count; i++)
                if (comboBoxEdit1.SelectedItem.ToString() == pfitinfo[i].Name.ToString())
                {
                    pfitinfo[i].Qi = double.Parse(textQi.Text);
                    pfitinfo[i].Di = double.Parse(textDi.Text);
                    pfitinfo[i].Ni = double.Parse(textn.Text);
                    break;
                }
            double n_bef = 0;
            double Qi_bef = 0;
            double Di_bef = 0;
            int lastindex_bef = 0;
            int firstindex_bef = 0;
            //重新计算拟合线,措施前的线
            for (int i = 0; i < pfitinfo.Count; i++)
                if (pfitinfo[i].Name.Contains("措施前"))
                {
                    Qi_bef = pfitinfo[i].Qi;
                    Di_bef = pfitinfo[i].Di;
                    n_bef = pfitinfo[i].Ni;
                    lastindex_bef = pfitinfo[i].Endpoint;
                    firstindex_bef = pfitinfo[i].Beginpoint;
                    break;
                }
            if (lastindex_bef > getData.Count())
                return;

            //获得时间
            double n_pred = 0;
            double Qi_pred = 0;
            double Di_pred = 0;

            int countnum_pred = 0;
            //重新计算拟合线,措施后的线
            for (int i = 0; i < pfitinfo.Count; i++)
                if (pfitinfo[i].Name.Contains("措施后"))
                {
                    n_pred = pfitinfo[i].Ni;
                    Qi_pred = pfitinfo[i].Qi;
                    Di_pred = pfitinfo[i].Di;

                    countnum_pred = (int)(pfitinfo[i].Endpoint - pfitinfo[i].Beginpoint) + 1;
                    break;
                }



            DateTime fail_date = new DateTime(2001, 3, 23);
            int fail_last_time = int.Parse(lasttime.Text);



            {
                # region
                int j = firstindex_bef;
                int j_pred = 0;
                int longest = 0;//最长有效期
                int time_before_stimu = 0;
                int failflag = 0;//连续
                int i = 1;
                int ij = 0;
                for (i = 0; i < ComboBoxproperties.Items.Count; i++)//看看措施前用啥拟合的
                {
                    if (ComboBoxproperties.Items[i].ToString().Contains("措施前"))
                        break;
                }
                for (ij = 0; ij < ComboBoxproperties.Items.Count; ij++)//看看措施前用啥拟合的
                {
                    if (ComboBoxproperties.Items[ij].ToString().Contains("措施后"))
                        break;
                }
                while (failflag < fail_last_time)
                {
                    double tempq = 0;
                    double tempq_pred = 0;
                    DateTime temp_date = new DateTime(2001, 3, 23);

                    if (ComboBoxproperties.Items[i].ToString().Contains("指数"))
                    {
                        tempq = Qi_bef * System.Math.Exp((-Di_bef) * (j - firstindex_bef));
                    }
                    else if (ComboBoxproperties.Items[i].ToString().Contains("双曲"))
                    {
                        tempq = Qi_bef / (System.Math.Pow((1 + n_bef * Di_bef * (j - firstindex_bef)), 1 / n_bef));
                    }
                    else if (ComboBoxproperties.Items[i].ToString().Contains("调和"))
                    {
                        tempq = Qi_bef / ((Di_bef) * (j - firstindex_bef) + 1);
                    }

                    if (DataTime[1].Year - DataTime[0].Year == 0)
                    {
                        if (DataTime[1].Month - DataTime[0].Month == 0)
                        {
                            int span = DataTime[1].Day - DataTime[0].Day;
                            temp_date = (DataTime[0].AddDays(j * span));
                        }
                        else
                        {
                            int span = DataTime[1].Month - DataTime[0].Month;
                            temp_date = (DataTime[0].AddMonths(j * span));
                        }
                    }
                    else
                    {
                        int span = DataTime[1].Year - DataTime[0].Year;
                        temp_date = (DataTime[0].AddYears(j * span));
                    }
                    resultTime.Add(temp_date);


                    if (temp_date <= stimu_date)
                    {
                        time_before_stimu = j;
                        result_bef.Add(tempq);
                    }

                    if (temp_date > stimu_date)
                    {
                        longest++;
                        if (longest > int.Parse(longgest_num.Text))
                        {
                            resultTime.RemoveAt(resultTime.Count() - 1);
                            break;
                        }


                        result_aft.Add(tempq);
                        resultTime_aft.Add(temp_date);
                        fail_date = temp_date;

                        if (j < getData.Count)
                        {
                            his_result.Add(getData[j]);
                        }
                        else
                        {//措施后
                            if (Qi_pred != 0)
                            {
                                if (ComboBoxproperties.Items[ij].ToString().Contains("指数"))
                                {
                                    tempq_pred = Qi_pred * System.Math.Exp((-Di_pred) * (countnum_pred + j_pred));
                                }
                                else if (ComboBoxproperties.Items[ij].ToString().Contains("双曲"))
                                {
                                    tempq_pred = Qi_pred / (System.Math.Pow((1 + n_pred * Di_pred * (countnum_pred + j_pred)), 1 / n_pred));
                                }
                                else if (ComboBoxproperties.Items[ij].ToString().Contains("调和"))
                                {
                                    tempq_pred = Qi_pred / ((Di_pred) * (countnum_pred + j_pred) + 1);
                                }
                                j_pred++;
                                his_result.Add(tempq_pred);
                            }
                        }

                        if (result_aft.Count() == his_result.Count())
                        {
                            if ((his_result[result_aft.Count() - 1] - result_aft[result_aft.Count() - 1]) > 0)
                            {
                                increase_result.Add(his_result[result_aft.Count() - 1] - result_aft[result_aft.Count() - 1]);
                                failflag = 0;
                            }
                            else
                            {
                                increase_result.Add(0);
                                failflag++;
                            }
                        }
                        else
                        {
                            result_aft.RemoveAt(result_aft.Count() - 1);
                            resultTime_aft.RemoveAt(resultTime_aft.Count() - 1);
                            break;
                        }

                    }
                    j++;
                }
                if (failflag > 0)
                {
                    if (DataTime[1].Year - DataTime[0].Year == 0)
                    {
                        if (DataTime[1].Month - DataTime[0].Month == 0)
                        {
                            int span = DataTime[1].Day - DataTime[0].Day;
                            fail_date = (fail_date.AddDays(-fail_last_time * span));
                        }
                        else
                        {
                            int span = DataTime[1].Month - DataTime[0].Month;
                            fail_date = (fail_date.AddMonths(-fail_last_time * span));
                        }
                    }
                    else
                    {
                        int span = DataTime[1].Year - DataTime[0].Year;
                        fail_date = (fail_date.AddYears(-fail_last_time * span));
                    }
                }
                while (failflag > 0)
                {
                    increase_result.RemoveAt(increase_result.Count() - 1);
                    his_result.RemoveAt(his_result.Count() - 1);
                    result_aft.RemoveAt(result_aft.Count() - 1);
                    resultTime.RemoveAt(resultTime.Count() - 1);
                    resultTime_aft.RemoveAt(resultTime_aft.Count() - 1);
                    failflag--;
                }
                result_bef.AddRange(result_aft);
                redrawseries(1, result_bef, resultTime);

                for (int ik = 0; ik < myChart.Series.Count; ik++)
                {
                    if (myChart.Series[ik].Name.Contains("措施后"))
                        redrawseries(ik, his_result, resultTime_aft);
                }
                fail_date = resultTime_aft.Last();
                stimu_draw(fail_date, increase_result, result_aft, resultTime_aft);
                # endregion
            }


            //写结果

            writeresult(fail_date, his_result, increase_result, result_aft, resultTime_aft);
            btn_excel_output.Enabled = true;
        }

        private void writeresult(DateTime fail_date, List<double> his_result, List<double> increase_result, List<double> result_aft, List<DateTime> resultTime_aft)
        {
            // The following line opens a specified document in a Snap application. 
            snapControl1.LoadDocument(Application.StartupPath + "\\Stimulation_Evaluation.snx", SnapDocumentFormat.Snap);
            DataTable dt = new DataTable();
            dt.TableName = "增油月报2";
            dt.Columns.Add("井号", Type.GetType("System.String"));
            dt.Columns.Add("措施时间", Type.GetType("System.String"));
            dt.Columns.Add("增油年月", Type.GetType("System.Int32"));
            dt.Columns.Add("增油量", Type.GetType("System.Double"));

            dt.Columns.Add("措施前预测产油量", Type.GetType("System.Double"));
            dt.Columns.Add("措施后产油量", Type.GetType("System.Double"));
            for (int i = 0; i < his_result.Count(); i++)
            {
                dt.Rows.Add(new object[] { wellname, stimu_date.ToString("yyyy/MM/dd"), Int32.Parse(resultTime_aft[i].ToString("yyyyMM")), 
                    increase_result[i], his_result[i], result_aft[i] });
            }
            var result = (from x in dt.AsEnumerable()
                          group x by x.Field<int>("增油年月") into g
                          select new

                     {
                         wellname = g.Select(r => r.Field<string>("井号")).First(),
                         //time = g.Select(r => r.Field<string>("措施时间")).First(),
                         datatime = g.Key,
                         incresese = g.Sum(r => r.Field<double>("增油量")),
                         before = g.Sum(r => r.Field<double>("措施前预测产油量")),
                         after = g.Sum(r => r.Field<double>("措施后产油量"))
                     });
            DataTable dt1 = new DataTable();
            dt1.TableName = "增油月报";
            dt1.Columns.Add("井ID", Type.GetType("System.String"));
            dt1.Columns.Add("井号名称", Type.GetType("System.String"));
            dt1.Columns.Add("措施代码", Type.GetType("System.String"));
            dt1.Columns.Add("措施开始日期", Type.GetType("System.DateTime"));
            dt1.Columns.Add("增油年月", Type.GetType("System.Int32"));
            dt1.Columns.Add("月增油量", Type.GetType("System.Double"));
            dt1.Columns.Add("措施前预测产油量", Type.GetType("System.Double"));
            dt1.Columns.Add("措施后产油量", Type.GetType("System.Double"));

            DataRow dr;
            foreach (var re in result)
            {
                dr = dt1.NewRow();
                dr["井ID"] = re.wellname;
                dr["井号名称"] = re.wellname;
                dr["措施代码"] = stimu_code;
                dr["措施开始日期"] = stimu_date;
                dr["增油年月"] = re.datatime;
                dr["月增油量"] = re.incresese;
                dr["措施前预测产油量"] = re.before;
                dr["措施后产油量"] = re.after;
                dt1.Rows.Add(dr);
            }


            DataTable dt2 = new DataTable();
            dt2.TableName = "增油汇总表";
            dt2.Columns.Add("井号", Type.GetType("System.String"));
            dt2.Columns.Add("措施时间", Type.GetType("System.String"));
            dt2.Columns.Add("失效时间", Type.GetType("System.String"));
            dt2.Columns.Add("有效期", Type.GetType("System.String"));
            dt2.Columns.Add("累增油量", Type.GetType("System.String"));
            dt2.Rows.Add(new object[] { wellname, stimu_date.ToString("yyyy/MM/dd"), fail_date.ToString("yyyy/MM/dd"), 
                (fail_date - stimu_date).Days.ToString(), increase_result.Sum().ToString("0.00")});


            result_dtset = new DataSet();
            result_dtset.Tables.Add(dt1);
            result_dtset.Tables.Add(dt2);

            //DataSet xmlDataSet = new DataSet();
            //xmlDataSet.ReadXml("C:\\Users\\Public\\Documents\\DevExpress Demos 14.1\\Components\\Data\\Cars.xml");
            snapControl1.DataSource = result_dtset;
            snapControl1.Document.Fields.Update();

            snapControl1.Visible = true;
            dockPanel5.Show();
        }


        private void btnOutpic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            ImageFormat format = ImageFormat.Bmp;
            //myChart.ExportToImage(fileName, format);

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导出图片";
            fileDialog.Filter = "Bitmaps|*.BMP|JPEG Files|*.JPEG|Windows Metafile Format|*.WMF|PNG Files|*.PNG|Enhanced MetaFile|*.EMF";
            DialogResult dialogResult = fileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    System.IO.File.Delete(fileDialog.FileName);
                }
                //获得文件路径
                string localFilePath = fileDialog.FileName.ToString();
                //去除文件后缀名
                string fileNameWithoutSuffix = localFilePath.Substring(0, localFilePath.LastIndexOf("."));
                //后缀名
                string aLastName = localFilePath.Substring(localFilePath.LastIndexOf(".") + 1, (localFilePath.Length - localFilePath.LastIndexOf(".") - 1));   //扩展名
                switch (aLastName)
                {
                    case "BMP": format = ImageFormat.Bmp; break;
                    case "WMF": format = ImageFormat.Wmf; break;
                    case "JPEG": format = ImageFormat.Jpeg; break;
                    case "PNG": format = ImageFormat.Png; break;
                    case "EMF": format = ImageFormat.Emf; break;
                }

                myChart.ExportToImage(localFilePath, format);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void barCheckItem2_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barCheckItem2.Checked == true)
                dockPanel1.Show();
            else
                dockPanel1.Hide();
        }



        private void btnhelp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\help\\递减分析.pdf");
        }


        private void btn_upload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt_TableAndField = new DataTable();
            string sheetName = "物理表汇总";
            string tablename = "R_OIL_STIMU_M";
            string TableAndField = string.Format("select 库表名称 AS TABLE_ID, 表显示名称 AS 目的表名, 列显示名称 AS 目的字段 ,库字段名称 as ID, 默认单位名称 as 单位名称, 主键  as 是否必须 from [{0}$] where (库表名称='" + tablename + "')", sheetName);
            dt_TableAndField = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);
            dt_TableAndField.Columns.Add("源表格", typeof(string));
            dt_TableAndField.Columns.Add("源字段", typeof(string));
            dt_TableAndField.Columns.Add("ID_字符类型", typeof(string));

            int n = dt_TableAndField.Columns.IndexOf("源表格");

            string varTableName = dt_TableAndField.Rows[0]["TABLE_ID"].ToString();
            dt_TableAndField = OleDbHelper.GetTableColumn(Globalname.DabaBasePath, varTableName, dt_TableAndField);



            for (int i = 0; i < dt_TableAndField.Rows.Count; i++)
            {
                dt_TableAndField.Rows[i]["源表格"] = result_dtset.Tables["增油月报"].TableName;

                for (int j = 0; j < result_dtset.Tables["增油月报"].Columns.Count; j++)
                {
                    if (dt_TableAndField.Rows[i]["目的字段"].ToString() == result_dtset.Tables["增油月报"].Columns[j].ColumnName.ToString())
                    {
                        dt_TableAndField.Rows[i]["源字段"] = result_dtset.Tables["增油月报"].Columns[j].ColumnName.ToString();
                        break;
                    }
                }
            }

            //dt_TableAndField.Rows[0]["源表格"] = result_dtset.Tables["增油汇总表"].TableName;



            sheetName = dt_TableAndField.Rows[0][n].ToString();//
            if (sheetName == "")
                return;
            OleDbHelper.InsertDataTable2(result_dtset.Tables["增油月报"], Globalname.DabaBasePath, dt_TableAndField);
        }

        #region 算法
        /// <summary>
        /// 相关系数,或者拟合度，要求两个集合数量必须相同
        /// </summary>
        /// <param name="decdata">数组一</param>
        /// <param name="decresult">数组二</param>
        /// <returns></returns>
        private double correl(List<double> decdata, List<double> decresult)
        {
            int realdatanum = decdata.Count;

            //数组一
            double avg1 = average(decdata, realdatanum);
            //数组二
            double avg2 = average(decresult, realdatanum);

            double sumfenzi = 0;
            double sumfenmu_x = 0;
            double sumfenmu_y = 0;
            for (int i = 0; i < realdatanum; i++)
            {
                sumfenzi = sumfenzi + ((decdata[i] - avg1)) * ((decresult[i] - avg2));
                sumfenmu_x = sumfenmu_x + (decdata[i] - avg1) * (decdata[i] - avg1);
                sumfenmu_y = sumfenmu_y + (decresult[i] - avg2) * (decresult[i] - avg2);
            }
            double sumfenmu = System.Math.Pow(sumfenmu_x, 0.5) * System.Math.Pow(sumfenmu_y, 0.5);
            double cor = sumfenzi / sumfenmu;
            cor = cor * cor;
            return cor;
        }
        /// <summary>
        /// 求出数据平均值,并保留三位小数
        /// </summary>
        /// <param name="Valist">数据集合</param>
        /// <returns></returns>
        private double average(List<double> Valist, int realdatanum)
        {
            double sum = 0;
            for (int i = 0; i < realdatanum; i++)
            {
                sum = sum + Valist[i];
            }
            double revl = sum / realdatanum;
            return revl;
        }

        #endregion 算法





    }
}
