using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Spreadsheet;
using DevExpress.XtraCharts;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using DevExpress.Spreadsheet.Charts;
using DevExpress.Spreadsheet.Drawings;
using System.Data;
using System.Linq;
using System.Text;

namespace fracture
{
    public partial class waterflooding : Form
    {
        XYDiagram Diagram;
        ChartControl myChart;
        int SecondaryAxesY_num = 0;//图中副坐标的数目
        int seriesnum;//图中series的数目
        int phase_num;//添加的阶段数
        int[] selectinfo;//拟合段的开始时间和拟合的时间的个数
        public bool selected_flag = false;
        List<double[]> getData;//输入的产油量
        List<double[]> fitinfo;//拟合段段信息
        List<double> jia_horizon;//甲型
        List<double> jia_vertical;//甲型
        List<double> yi_horizon;//yi型
        List<double> yi_vertical;//yi型
        List<double> bing_horizon;//丙型
        List<double> bing_vertical;//丙型
        List<double> ding_horizon;//丁型
        List<double> ding_vertical;//丁型
        List<double[]> ployfitinfo;//拟合段信息汇总，包括：拟合phase，曲线类型，开始点编号，结束点编号，斜率,截距,误差，NP
        //曲线类型等于0-3=甲乙丙丁；
        List<string> formulainfo;//拟合的公式
        List<DateTime> DataTime;//输入的时间
        //XYDiagram Diagram { get { return myChart.Diagram as XYDiagram; } }
        bool dragging = false;
        public Point firstPoint = new Point(0, 0);  //鼠标第一点 ，对应屏幕
        public Point secondPoint = new Point(0, 0);  //鼠标第二点 
        Point seccoords;//对应图中坐标系的值
        Point firstcoords;//对应图中坐标系的值
        int selection_num = 0;
        Point minminpoint;//屏幕坐标系的最小值
        Point maxmaxpoint;//屏幕坐标系的最大值
        List<ControlCoordinates> maxcoords;//图中对应p屏幕坐标系的值
        List<ControlCoordinates> mincoords;//图中对应p屏幕坐标系的值
        Color col = Color.Blue;//拟合线颜色
        List<Rectangle> selectionRectangle; //Rectangle.Empty;
        DevExpress.XtraEditors.Repository.RepositoryItemComboBox ComboBoxproperties;

        public waterflooding()
        {
            Stopwatch myWatch = Stopwatch.StartNew();
            InitializeComponent();
            intialspreadsheet();
            ribbonControl1.SelectedPage = ribbonControl1.PageCategories[0].Pages[0];
            spreadsheetControl1.Options.TabSelector.Visibility = DevExpress.XtraSpreadsheet.SpreadsheetElementVisibility.Hidden;
            ComboBoxproperties = comboBoxEdit1.Properties;

            //Disable editing 
            //添加下拉框的内容
            ComboBoxproperties.Items.AddRange(new string[] { "下拉选择递减段" });
            //Select the first item 
            comboBoxEdit1.SelectedIndex = 0;
            ComboBoxproperties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            myWatch.Stop();
            btnoutExcel.Enabled = false;
            //MessageBox.Show(myWatch.ElapsedMilliseconds.ToString());
        }
        void intialspreadsheet()
        {
            
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            IWorkbook workbook = spreadsheetControl1.Document;
            
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Font.Name = "Times New Roman";
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Font.Size = 11;
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Font.FontStyle = DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold;
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Borders.SetAllBorders(Color.Black, BorderLineStyle.Thin);
            workbook.Worksheets[0].Range["A1:Z1"].ColumnWidthInCharacters = 10;
             workbook.Worksheets[0].Columns[0].RowHeight = 70;

           

          worksheet.Rows[0].Fill.BackgroundColor = Color.FromArgb(125,217, 217, 217);//Color.Navy;     
            //worksheet.Rows[0].Fill.BackgroundColor = Color.FromArgb(125, Color.Blue);
            worksheet.Cells["A1"].Value = "时间";
            worksheet.Cells["B1"].Value = "年产油量";
            worksheet.Cells["C1"].Value = "年产水量";
        
            worksheet.Cells["D1"].Value = "累计产油量";
            worksheet.Cells["E1"].Value = "累计产水量";       
        }
        private void importdata(List<double[]> getData, List<DateTime> DataTime)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;

            // Perform the export.
            int i;
            i = 1;
            while (worksheet[i, 0].Value.IsEmpty == false)
            {
                double[] tempdata = new double[6];
                DataTime.Add(worksheet[i, 0].Value.DateTimeValue);
                tempdata[0] = worksheet[i, 1].Value.NumericValue;//产油
                tempdata[1] = worksheet[i, 2].Value.NumericValue;//产水
                tempdata[2] = worksheet[i, 3].Value.NumericValue;//累油
                tempdata[3] = worksheet[i, 4].Value.NumericValue;//累水
                tempdata[4] = worksheet[i, 5].Value.NumericValue;//累油
                tempdata[5] = worksheet[i, 6].Value.NumericValue;//累水
                getData.Add(tempdata);
                i++;
            }

        }
        public class Record
        {
            double xaxis; double yaxis;
            public Record(double xaxis, double yaxis)
            {
                this.xaxis = xaxis;
                this.yaxis = yaxis;
            }
            public double Xaxis
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
        public ArrayList list(List<double> xData, List<double> yData)
        {

            ArrayList list = new ArrayList();

            // Populate the list with records. 
            for (int i = 0; i < xData.Count; i++)
            {
                Record d1 = new Record(xData[i], yData[i]);
                list.Add(d1);
            }
            return list;
        }
        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            //List<double> getData = new List<double>();
            //List<DateTime> DataTime = new List<DateTime>();
            getData = new List<double[]>();
            DataTime = new List<DateTime>();
            ployfitinfo = new List<double[]>();  //拟合段信息汇总，包括：拟合phase，曲线类型，开始点编号，结束点编号，斜率,截距,误差，NP
            //曲线类型等于0-3=甲乙丙丁；
            formulainfo = new List<string>();
            jia_horizon = new List<double>();//甲型
            jia_vertical = new List<double>();//甲型
            yi_horizon = new List<double>();//yi型
            yi_vertical = new List<double>();//yi型
            bing_horizon = new List<double>();//丙型
            bing_vertical = new List<double>();//丙型
            ding_horizon = new List<double>();//丁型
            ding_vertical = new List<double>();//丁型
            col = Color.Blue;//拟合线颜色
            ComboBoxproperties.Items.Clear();
            importdata(getData, DataTime);//输入数据
            if (getData.Count == 0 | WatMethodchecked.CheckedItemsCount == 0)
                return;

            ComboBoxproperties.Items.AddRange(new string[] { "下拉选择递减段" });
            int num_time = getData.Count;
            //Select the first item 
            comboBoxEdit1.SelectedIndex = 0;

            myChart = chartControl1;
            myChart.Series.Clear();
            if (WatMethodchecked.GetItemChecked(0) == true)// '甲型水驱曲线
            {

                for (int i = 0; i < num_time; i++)
                {
                    jia_horizon.Add(getData[i][0]);
                    jia_vertical.Add(getData[i][1]);
                }

                ArrayList listDataSource = list(jia_horizon, jia_vertical);      //画图数据源
                DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series("注入量1 FV", ViewType.Line);
                myChart.Series.Add(series1);
                series1.DataSource = listDataSource;
                //Set the scale type for the series' arguments and values.
                series1.ArgumentScaleType = ScaleType.Numerical;
                series1.ValueScaleType = ScaleType.Numerical;


                // Adjust the series data members. 
                series1.ArgumentDataMember = "Xaxis";
                series1.ValueDataMembers.AddRange(new string[] { "Yaxis" });
                //myChart.DateTimeScaleOptions.MeasureUnit = Month
                // Access the view-type-specific options of the series. 
                ((PointSeriesView)series1.View).Color = Color.FromArgb(125, Color.LightPink);

                //series1.LegendTextPattern = "{A}";
                series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
                series1.Label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAllAroundPoint;
            }
            if (WatMethodchecked.GetItemChecked(1) == true)//'乙型水驱曲线
            {

                for (int i = 0; i < num_time; i++)
                {
                    yi_horizon.Add(getData[i][2]);
                    yi_vertical.Add(getData[i][3]);
                }
                ArrayList listDataSource = list(yi_horizon, yi_vertical);      //画图数据源
                DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series("注入量1.5 FV", ViewType.Line);
                myChart.Series.Add(series2);
                series2.DataSource = listDataSource;
                //Set the scale type for the series' arguments and values.
                series2.ArgumentScaleType = ScaleType.Numerical;
                series2.ValueScaleType = ScaleType.Numerical;


                // Adjust the series data members. 
                series2.ArgumentDataMember = "Xaxis";
                series2.ValueDataMembers.AddRange(new string[] { "Yaxis" });
                //myChart.DateTimeScaleOptions.MeasureUnit = Month
                // Access the view-type-specific options of the series. 
                ((PointSeriesView)series2.View).Color = Color.FromArgb(125, Color.Orange);

                //series2.LegendTextPattern = "{A}";
                series2.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
                series2.Label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAllAroundPoint;

            }

            if (WatMethodchecked.GetItemChecked(2) == true)//  '丙型水驱曲线
            {
                for (int i = 0; i < num_time; i++)
                {
                    bing_horizon.Add(getData[i][4]);
                    bing_vertical.Add(getData[i][5]);
                }
                ArrayList listDataSource = list(bing_horizon, bing_vertical);      //画图数据源
                DevExpress.XtraCharts.Series series3 = new DevExpress.XtraCharts.Series("注入量2 FV", ViewType.Line);
                myChart.Series.Add(series3);
                series3.DataSource = listDataSource;
                //Set the scale type for the series' arguments and values.
                series3.ArgumentScaleType = ScaleType.Numerical;
                series3.ValueScaleType = ScaleType.Numerical;


                // Adjust the series data members. 
                series3.ArgumentDataMember = "Xaxis";
                series3.ValueDataMembers.AddRange(new string[] { "Yaxis" });
                //myChart.DateTimeScaleOptions.MeasureUnit = Month
                // Access the view-type-specific options of the series. 
                ((PointSeriesView)series3.View).Color = Color.FromArgb(125, Color.Tomato);

                //series3.LegendTextPattern = "{A}";
                series3.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
                series3.Label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAllAroundPoint;

            }
         
            // Bind the chart to the list. 
            // ChartControl myChart = myChart;
            //ChartControl myChart = new ChartControl();
            DevExpress.XtraCharts.ChartTitle chartTitle1 = new DevExpress.XtraCharts.ChartTitle();
            chartTitle1.Text="不同堵剂注入量下的交联剂在裂缝中的浓度分布";
            chartTitle1.Font = new Font("微软雅黑", 10, FontStyle.Bold);
            chartControl1.Titles.AddRange(new DevExpress.XtraCharts.ChartTitle[] {
chartTitle1});
          
            myChart.CrosshairOptions.ShowCrosshairLabels = true;
            myChart.CrosshairOptions.ShowArgumentLine = false;
            myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
            myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;
            // Create a series, and add it to the chart. 
            phase_num = 1;
            DevExpress.XtraCharts.Legend legend = myChart.Legend;

            // Display the chart control's legend.
            legend.Visible = true;

            // Define its margins and alignment relative to the diagram.
            legend.Margins.All = 8;


            // Define the layout of items within the legend.

            legend.EquallySpacedItems = true;
            legend.HorizontalIndent = 8;
            legend.VerticalIndent = 8;
            legend.TextVisible = true;
            legend.TextOffset = 8;
            legend.MarkerVisible = true;
            legend.MarkerSize = new Size(10, 10);
            legend.Padding.All = 4;



            // Define the limits for the legend to occupy the chart's space.
            legend.MaxHorizontalPercentage = 50;
            legend.MaxVerticalPercentage = 50;

            // Customize the legend text properties.
            legend.Antialiasing = false;
            legend.Font = new Font("Tahoma", 9, FontStyle.Bold);
            legend.TextColor = Color.Black;

            XYDiagram diagram = (XYDiagram)myChart.Diagram;
            Diagram = (XYDiagram)myChart.Diagram;

            //define axis for multiple pane
            if (myChart.Series.Count > 0)
            {
                // Obtain a diagram and clear its collection of panes.
                //XYDiagram diagram = (XYDiagram)myChart.Diagram;
                diagram.Panes.Clear();
                diagram.SecondaryAxesY.Clear();
                diagram.SecondaryAxesX.Clear();
                // Create a pane for each series.
                switch (myChart.Series[0].Name)
                {
                    case "注入量1 FV":
                        {
                            diagram.AxisX.Title.Text = "距离裂缝入口";
                            diagram.AxisY.Title.Text = "归一化浓度";
                            diagram.AxisX.Title.Font = new Font("Tahoma", 9, FontStyle.Bold);
                            diagram.AxisY.Title.Font = new Font("Tahoma", 9, FontStyle.Bold);
                            break;
                        }
                    case "注入量1.5 FV":
                        {
                            diagram.AxisX.Title.Text = "距离裂缝入口";
                            diagram.AxisY.Title.Text = "归一化浓度";
                            break;
                        }
                    case "注入量2 FV":
                        {
                            diagram.AxisX.Title.Text = "距离裂缝入口";
                            diagram.AxisY.Title.Text = "归一化浓度";
                            break;
                        }
                    case "丁型水驱曲线":
                        {
                            diagram.AxisX.Title.Text = "Wp";
                            diagram.AxisY.Title.Text = "Lp/Np";
                            break;
                        }
                }
                if (myChart.Series.Count == 1)
                {
                    legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Left;
                    legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Bottom;
                    legend.Direction = LegendDirection.TopToBottom;

                }
                else
                {
                    legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Left;
                    legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.TopOutside;
                    legend.Direction = LegendDirection.LeftToRight;
                    legend.MaxHorizontalPercentage = 100;
                }

                for (int i = 1; i < myChart.Series.Count; i++)
                {
                    XYDiagramPane pane = new XYDiagramPane("留白");
                    diagram.Panes.Add(pane);

                    XYDiagramSeriesViewBase view = (XYDiagramSeriesViewBase)myChart.Series[i].View;

                    int axesPosition = diagram.SecondaryAxesY.Add(new SecondaryAxisY());
                    diagram.SecondaryAxesY[axesPosition].Alignment = AxisAlignment.Near;
                    diagram.SecondaryAxesY[axesPosition].GridLines.Visible = true;
                    diagram.SecondaryAxesY[axesPosition].GridLines.MinorVisible = true;
                    view.AxisY = diagram.SecondaryAxesY[axesPosition];
                    int axesPosition2 = diagram.SecondaryAxesX.Add(new SecondaryAxisX());
                    diagram.SecondaryAxesX[axesPosition2].Alignment = AxisAlignment.Near;
                    diagram.SecondaryAxesX[axesPosition2].GridLines.Visible = true;

                    view.AxisX = diagram.SecondaryAxesX[axesPosition2];


                    switch (myChart.Series[i].Name)
                    {
                        case "注入量1 FV":
                            {

                                //diagram.SecondaryAxesX[axesPosition2].Title.Text = "Np";
                                //diagram.SecondaryAxesY[axesPosition].Title.Text = "Lg(Wp)";
                                //diagram.AxisX.Title.Text = "Np";
                                //diagram.AxisY.Title.Text = "Lg(Wp)";
                                break;
                            }
                        case "注入量1.5 FV":
                            {
                                diagram.SecondaryAxesX[axesPosition2].Title.Text = "距离裂缝入口";
                                diagram.SecondaryAxesY[axesPosition].Title.Text = "归一化浓度";
                                diagram.SecondaryAxesX[axesPosition2].Title.Font = new Font("Tahoma", 9, FontStyle.Bold);
                                diagram.SecondaryAxesY[axesPosition].Title.Font = new Font("Tahoma", 9, FontStyle.Bold);
                                break;
                            }
                        case "注入量2 FV":
                            {
                                diagram.SecondaryAxesX[axesPosition2].Title.Text = "距离裂缝入口";
                                diagram.SecondaryAxesY[axesPosition].Title.Text = "归一化浓度";
                                diagram.SecondaryAxesX[axesPosition2].Title.Font = new Font("Tahoma", 9, FontStyle.Bold);
                                diagram.SecondaryAxesY[axesPosition].Title.Font = new Font("Tahoma", 9, FontStyle.Bold);
                                break;
                            }
                    }
                    diagram.SecondaryAxesX[axesPosition2].Title.Visible = true;
                    diagram.SecondaryAxesY[axesPosition].Title.Visible = true;
                    diagram.SecondaryAxesX[axesPosition2].Title.Font = new Font("Tahoma", 9);
                    diagram.SecondaryAxesY[axesPosition2].Title.Font = new Font("Tahoma", 9);
                    view.Pane = pane;
                }
            }
            SecondaryAxesY_num = diagram.SecondaryAxesY.Count;//图中副坐标的数目
            seriesnum = myChart.Series.Count;//图中series的数目
            diagram.DefaultPane.SizeMode = PaneSizeMode.UseWeight;
            diagram.DefaultPane.Weight = 1.2;

            // Enable the diagram's scrolling.
            diagram.EnableAxisXScrolling = true;
            diagram.EnableAxisYScrolling = true;

            // Customize the appearance of the axes' grid lines.
            diagram.AxisX.GridLines.Visible = true;
            diagram.AxisX.GridLines.MinorVisible = false;

            diagram.AxisY.GridLines.Visible = true;
            diagram.AxisY.GridLines.MinorVisible = true;

            //diagram.AxisY.Range.SetInternalMinMaxValues(1, 12);
            // Customize the appearance of the X-axis title.
            diagram.AxisX.Title.Visible = true;
            diagram.AxisX.Title.Alignment = StringAlignment.Center;

            diagram.AxisX.Title.TextColor = Color.Black;
            diagram.AxisX.Title.Antialiasing = true;
            diagram.AxisX.Title.Font = new Font("Tahoma", 9);

            // Customize the appearance of the Y-axis title.
            diagram.AxisY.Title.Visible = true;
            diagram.AxisY.Title.Alignment = StringAlignment.Center;

            diagram.AxisY.Title.TextColor = Color.Black;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("Tahoma", 9);
        }
        private void chartControl1_CustomPaint(object sender, CustomPaintEventArgs e)
        {
            if (Diagram == null)
                return;

            if (selectionRectangle != null)
                if (selectionRectangle.Count == SecondaryAxesY_num + 1)
                {
                    for (int ipane = 0; ipane < SecondaryAxesY_num + 1; ipane++)
                    {
                        Graphics g = e.Graphics;
                        Brush solidcor = new SolidBrush(Color.FromArgb(205, Color.LightGoldenrodYellow));
                        g.DrawRectangle(new Pen(Color.FromArgb(205, Color.Yellow)), selectionRectangle[ipane]);
                        g.FillRectangle(solidcor, selectionRectangle[ipane]);
                        //Rectangle selectionRectangle_temp = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                        //selectionRectangle.Add(selectionRectangle_temp);
                    }
                }
            maxcoords = new List<ControlCoordinates>();
            mincoords = new List<ControlCoordinates>();
            if (SecondaryAxesY_num == 0)
            {
                maxcoords.Add(Diagram.DiagramToPoint((double)Diagram.AxisX.WholeRange.MaxValue + Diagram.AxisX.WholeRange.SideMarginsValue, (double)Diagram.AxisY.WholeRange.MaxValue + Diagram.AxisY.WholeRange.SideMarginsValue));
                mincoords.Add(Diagram.DiagramToPoint((double)Diagram.AxisX.WholeRange.MinValue - Diagram.AxisX.WholeRange.SideMarginsValue, (double)Diagram.AxisY.WholeRange.MinValue - Diagram.AxisY.WholeRange.SideMarginsValue));

            }
            else if (SecondaryAxesY_num > 0)
            {
                maxcoords.Add(Diagram.DiagramToPoint((double)Diagram.AxisX.WholeRange.MaxValue + Diagram.AxisX.WholeRange.SideMarginsValue, (double)Diagram.AxisY.WholeRange.MaxValue + Diagram.AxisY.WholeRange.SideMarginsValue));
                mincoords.Add(Diagram.DiagramToPoint((double)Diagram.AxisX.WholeRange.MinValue - Diagram.AxisX.WholeRange.SideMarginsValue, (double)Diagram.AxisY.WholeRange.MinValue - Diagram.AxisY.WholeRange.SideMarginsValue));
                for (int i = 0; i < SecondaryAxesY_num; i++)
                {
                    maxcoords.Add(Diagram.DiagramToPoint((double)Diagram.SecondaryAxesX[i].WholeRange.MaxValue + Diagram.SecondaryAxesX[i].WholeRange.SideMarginsValue, (double)Diagram.SecondaryAxesY[i].WholeRange.MaxValue + Diagram.SecondaryAxesY[i].WholeRange.SideMarginsValue, Diagram.SecondaryAxesX[i], Diagram.SecondaryAxesY[i], Diagram.Panes[i]));
                    mincoords.Add(Diagram.DiagramToPoint((double)Diagram.SecondaryAxesX[i].WholeRange.MinValue - Diagram.SecondaryAxesX[i].WholeRange.SideMarginsValue, (double)Diagram.SecondaryAxesY[i].WholeRange.MinValue, Diagram.SecondaryAxesX[i], Diagram.SecondaryAxesY[i], Diagram.Panes[i]));
                }
            }
            if (mincoords != null)
            {
                minminpoint = new Point(0, 0);
                maxmaxpoint = new Point(0, 0);
                minminpoint.X = mincoords[0].Point.X;
                minminpoint.Y = mincoords[0].Point.X;
                maxmaxpoint.X = maxcoords[0].Point.X;
                maxmaxpoint.Y = maxcoords[0].Point.X;
                for (int i = 0; i < mincoords.Count; i++)
                {
                    if (minminpoint.X > mincoords[i].Point.X)
                        minminpoint.X = mincoords[i].Point.X;
                    if (minminpoint.Y < mincoords[i].Point.Y)
                        minminpoint.Y = mincoords[i].Point.Y;
                    if (maxmaxpoint.X < maxcoords[i].Point.X)
                        maxmaxpoint.X = maxcoords[i].Point.X;
                    if (maxmaxpoint.Y > maxcoords[i].Point.Y)
                        maxmaxpoint.Y = maxcoords[i].Point.Y;
                }
            }

        }
        private void chartControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Diagram == null)
                return;
            if (e.Button != MouseButtons.Left)
                return;

            dragging = true;
            myChart.Capture = true;
            firstPoint = new Point(e.X, e.Y);

            if (e.Y < maxmaxpoint.Y)
            {
                firstPoint.Y = maxmaxpoint.Y;
            }
            if (e.Y > minminpoint.Y)
            {
                firstPoint.Y = minminpoint.Y;
            }
            if (e.X > maxmaxpoint.X)
            {
                firstPoint.X = maxmaxpoint.X;
            }
            if (e.X < minminpoint.X)
            {
                firstPoint.X = minminpoint.X;
            }
        }
        private void chartControl1_MouseMove(object sender, MouseEventArgs e)
        {

            if (Diagram == null)
                return;

            if (dragging && e.Button != MouseButtons.Right)
            {

                //获取新的右下角坐标   
                secondPoint = new Point(e.X, e.Y);
                if (e.Y < maxmaxpoint.Y)
                {
                    secondPoint.Y = maxmaxpoint.Y;
                }
                if (e.Y > minminpoint.Y)
                {
                    secondPoint.Y = minminpoint.Y;
                }
                if (e.X > maxmaxpoint.X)
                {
                    secondPoint.X = maxmaxpoint.X;
                }
                if (e.X < minminpoint.X)
                {
                    secondPoint.X = minminpoint.X;
                }

                int minX = Math.Min(firstPoint.X, secondPoint.X);
                int minY = maxcoords[0].Point.Y;
                int maxX = Math.Max(firstPoint.X, secondPoint.X);
                int maxY = mincoords[0].Point.Y;
                firstcoords = new Point(minX, minY);
                seccoords = new Point(maxX, maxY);
                for (int i = 0; i < SecondaryAxesY_num + 1; i++)
                {

                    if ((firstPoint.Y >= maxcoords[i].Point.Y) & (firstPoint.Y <= mincoords[i].Point.Y))
                    {
                        selection_num = i;

                        break;
                    }

                }
                selectionRectangle = new List<Rectangle>();
                for (int ipane = 0; ipane < SecondaryAxesY_num + 1; ipane++)
                {
                    minY = maxcoords[ipane].Point.Y;
                    maxY = mincoords[ipane].Point.Y;
                    Rectangle selectionRectangle_temp = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                    selectionRectangle.Add(selectionRectangle_temp);
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
            //int minX = Math.Min(firstPoint.X, secondPoint.X);
            //int minY = Math.Min(firstPoint.Y, secondPoint.Y);
            //int maxX = Math.Max(firstPoint.X, secondPoint.X);
            //int maxY = Math.Max(firstPoint.Y, secondPoint.Y);
            if (e.Y < maxmaxpoint.Y)
            {
                secondPoint.Y = maxmaxpoint.Y;
            }
            if (e.Y > minminpoint.Y)
            {
                secondPoint.Y = minminpoint.Y;
            }
            if (e.X > maxmaxpoint.X)
            {
                secondPoint.X = maxmaxpoint.X;
            }
            if (e.X < minminpoint.X)
            {
                secondPoint.X = minminpoint.X;
            }
            selected_flag = true;
            polyfit_decline();
            selected_flag = false;
            for (int ipane = 0; ipane < SecondaryAxesY_num + 1; ipane++)
            {
                selectionRectangle[ipane] = Rectangle.Empty;
            }

        }
        private void polyfit_decline()
        {

            algorithm alg = new algorithm();

            selectinfo = new int[2];//拟合段的开始时间和拟合的时间的个数
            if (selected_flag == false)
                return;

            bool frist_enter = false;
            int countnum = 0;
            fitinfo = new List<double[]>();//拟合段的横纵坐标

            if (selection_num == 0)
            {
                for (int i = 0; i < myChart.Series[0].Points.Count; i++)
                {
                    Point temp = new Point(0, 0);
                    temp = Diagram.DiagramToPoint((double)myChart.Series[0].Points[i].NumericalArgument, (double)myChart.Series[0].Points[i].Values[0]).Point;
                    if (temp.X >= firstcoords.X & temp.X <= seccoords.X & frist_enter == false)
                    {
                        //double[] fitdata_temp = new double[2];
                        //fitdata_temp[0] = (double)myChart.Series[0].Points[0].NumericalArgument;
                        //fitdata_temp[1] = (double)myChart.Series[0].Points[0].NumericalArgument;
                        frist_enter = true;
                        //fitdata.Add(fitdata_temp);
                        selectinfo[0] = i;
                        countnum = countnum + 1;
                    }
                    else if (temp.X >= firstcoords.X & temp.X <= seccoords.X & frist_enter == true)
                    {
                        frist_enter = true;
                        //double[] fitdata_temp = new double[2];
                        //fitdata_temp[0] = (double)myChart.Series[0].Points[0].NumericalArgument;
                        //fitdata_temp[1] = (double)myChart.Series[0].Points[0].NumericalArgument;
                        //fitdata.Add(fitdata_temp);
                        countnum = countnum + 1;
                        selectinfo[1] = i;
                    }
                }
            }
            else if (selection_num > 0)
            {
                for (int i = 0; i < myChart.Series[selection_num].Points.Count; i++)
                {
                    Point temp = new Point(0, 0);

                    temp = Diagram.DiagramToPoint((double)myChart.Series[selection_num].Points[i].NumericalArgument, (double)myChart.Series[selection_num].Points[i].Values[0], Diagram.SecondaryAxesX[selection_num - 1], Diagram.SecondaryAxesY[selection_num - 1], Diagram.Panes[selection_num - 1]).Point;
                    if (temp.X >= firstcoords.X & temp.X <= seccoords.X & frist_enter == false)
                    {
                        //double[] fitdata_temp = new double[2];
                        //fitdata_temp[0] = (double)myChart.Series[0].Points[0].NumericalArgument;
                        //fitdata_temp[1] = (double)myChart.Series[0].Points[0].NumericalArgument;
                        frist_enter = true;
                        //fitdata.Add(fitdata_temp);
                        selectinfo[0] = i;
                        countnum = countnum + 1;
                    }
                    else if (temp.X >= firstcoords.X & temp.X <= seccoords.X & frist_enter == true)
                    {
                        frist_enter = true;
                        //double[] fitdata_temp = new double[2];
                        //fitdata_temp[0] = (double)myChart.Series[0].Points[0].NumericalArgument;
                        ////fitdata_temp[1] = (double)myChart.Series[0].Points[0].NumericalArgument;
                        //fitdata.Add(fitdata_temp);
                        countnum = countnum + 1;
                        selectinfo[1] = i;
                    }
                }

            }

            if (countnum < 2)//防止没有选到不合理的值进行拟合
                return;
            for (int i = 0; i < SecondaryAxesY_num + 1; i++)
            {
                int k = 0;
                double[] fitdatax = new double[countnum];
                double[] fitdatay = new double[countnum];
                double[] predic_datay = new double[countnum];
                List<double> drawx = new List<double>();
                List<double> drawy = new List<double>();
                for (int j = selectinfo[0]; j < selectinfo[1] + 1; j++)
                {
                    fitdatax[k] = myChart.Series[i].Points[j].NumericalArgument;
                    fitdatay[k] = myChart.Series[i].Points[j].Values[0];
                    k++;
                }

                double[,] bcresult = new double[2, 1];
                bcresult = alg.polyfit(fitdatax, fitdatay);// ' x, y, 参数列表 y=bx+c
                double capital_b = bcresult[1, 0];//斜率
                double capital_a = bcresult[0, 0];//截距
                k = 0;
                for (int j = selectinfo[0]; j < selectinfo[1] + 1; j++)
                {
                    //fitdatax[k] = myChart.Series[i].Points[j].NumericalArgument;
                    predic_datay[k] = capital_b * fitdatax[k] + capital_a;
                    k++;
                }
                double sqrcor = alg.correl(predic_datay, fitdatay);//计算误差
                for (k = 0; k < predic_datay.GetLength(0); k++)
                {
                    drawx.Add(fitdatax[k]);
                    drawy.Add(predic_datay[k]);
                }
                addseries(drawx, drawy, i, col);

                double fw = double.Parse(txt_fw.Text);
                string S = "";
                double np = 0;
                int waterflooding_type = 0;
                switch (myChart.Series[i].Name)
                {          // ''可采储量的计算，参考文献《常用水驱特征曲线在八面河油田中的应用》
                    case "甲型水驱曲线":
                        {
                            S = "Lg Wp=" + capital_a.ToString("f2") + "+" + capital_b.ToString() + "Np";
                            np = (System.Math.Log(fw / (1 - fw)) / System.Math.Log(10) - (capital_a + System.Math.Log(2.303 * capital_b) / System.Math.Log(10))) / capital_b;
                            waterflooding_type = 0;
                            break;
                        }
                    case "乙型水驱曲线":
                        {
                            S = "Lg Lp=" + capital_a.ToString("f2") + "+" + capital_b.ToString() + "Np";
                            np = (System.Math.Log(1 / (1 - fw)) / System.Math.Log(10) - (capital_a + System.Math.Log(2.303 * capital_b) / System.Math.Log(10))) / capital_b;
                            waterflooding_type = 1;
                            break;
                        }
                    case "丙型水驱曲线":
                        {
                            S = "Lp/Np=" + capital_a.ToString("f2") + "+" + capital_b.ToString() + "Lp";
                            np = (1 - System.Math.Pow(capital_a * (1 - fw), 0.5)) / capital_b;
                            waterflooding_type = 2;
                            break;
                        }
                    case "丁型水驱曲线":
                        {
                            S = "Lp/Np=" + capital_a.ToString("f2") + "+" + capital_b.ToString() + "Wp";
                            np = (1 - System.Math.Pow((capital_a - 1) * (1 - fw) / fw, 0.5)) / capital_b;
                            waterflooding_type = 3;
                            break;
                        }
                }
                //拟合段信息汇总，包括：拟合phase，曲线类型，开始点编号，结束点编号，斜率,截距,误差，NP
                //曲线类型等于0-3=甲乙丙丁；
                double[] polyfitinfo_temp = new double[8];
                polyfitinfo_temp[0] = phase_num;
                polyfitinfo_temp[1] = waterflooding_type;
                //polyfitinfo_temp[2] = phase_num;
                polyfitinfo_temp[2] = selectinfo[0];
                polyfitinfo_temp[3] = selectinfo[1];
                polyfitinfo_temp[4] = capital_b;
                polyfitinfo_temp[5] = capital_a;
                polyfitinfo_temp[6] = sqrcor;
                polyfitinfo_temp[7] = np;
                while (myChart.Series.Count >= seriesnum + SecondaryAxesY_num + 1)
                {
                    if (ployfitinfo.Count >= (myChart.Series.Count - (SecondaryAxesY_num + 1)))
                    {
                        ployfitinfo.RemoveAt(myChart.Series.Count - (SecondaryAxesY_num + 1));
                        formulainfo.RemoveAt(myChart.Series.Count - (SecondaryAxesY_num + 1));
                    }
                    else
                        break;
                }
                ployfitinfo.Add(polyfitinfo_temp);
                formulainfo.Add(S);
            }
        }
        private void addseries(List<double> drawx, List<double> drawy, int seriesi, Color col)
        {

            ArrayList listDataSource = list(drawx, drawy);      //画图数据源

            if (drawy.Count == 0 | drawx.Count == 0)
                return;
            // Bind the chart to the list. 
            myChart.CrosshairOptions.ShowCrosshairLabels = false;
            myChart.CrosshairOptions.ShowArgumentLine = false;
            myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
            myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;
            //MessageBox.Show(myChart.Series.Count.ToString());
            int seiresk = seriesnum;
            while (myChart.Series.Count >= seriesnum + SecondaryAxesY_num + 1)
            {
                if (myChart.Series.Count > seiresk)
                {
                    myChart.Series.RemoveAt(seiresk);
                    ployfitinfo.RemoveAt(myChart.Series.Count - (SecondaryAxesY_num + 1));
                    formulainfo.RemoveAt(myChart.Series.Count - (SecondaryAxesY_num + 1));
                }
                else
                    break;
            }

            if (phase_num > comboBoxEdit1.Properties.Items.Count-1)
            {
                ComboBoxproperties.Items.Add("第"+phase_num.ToString()+"阶段");
            }
            // Create a series, and add it to the chart. 
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series("ff", ViewType.Line);

            myChart.Series.Add(series1);
            series1.DataSource = listDataSource;

            // Set the scale type for the series' arguments and values.
            series1.ArgumentScaleType = ScaleType.Numerical;
            series1.ValueScaleType = ScaleType.Numerical;
            series1.ShowInLegend = false;
            if (seriesi > 0)
            {
                XYDiagramSeriesViewBase view = (XYDiagramSeriesViewBase)myChart.Series[myChart.Series.Count - 1].View;
                view.Pane = Diagram.Panes[seriesi - 1];
                view.AxisY = Diagram.SecondaryAxesY[seriesi - 1];
                view.AxisX = Diagram.SecondaryAxesX[seriesi - 1];
            }
            series1.CrosshairHighlightPoints = DevExpress.Utils.DefaultBoolean.False;
            // Adjust the series data members. 
            series1.ArgumentDataMember = "Xaxis";
            series1.ValueDataMembers.AddRange(new string[] { "Yaxis" });
            //myChart.DateTimeScaleOptions.MeasureUnit = Month
            // Access the view-type-specific options of the series. 

            ((PointSeriesView)series1.View).Color = col;// Color.FromArgb(125, Color.Blue);

            //series1.LegendTextPattern = "{A}";
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            comboBoxEdit1.SelectedIndex = 1;
        }

        private void btnSelect_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Diagram == null)
                return;
        }

        private void btnNull_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int seiresi = seriesnum;
            while (myChart.Series.Count > seriesnum)
            {
                myChart.Series.RemoveAt(seiresi);
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
            algorithm alg = new algorithm();
            seriesnum = myChart.Series.Count;
            phase_num = phase_num + 1;
            col = alg.GetRandomColor();

        }
        
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ployfitinfo == null)
                return;
            int i = 1;
            while (i < ployfitinfo.Count + 1)
            {
                if (comboBoxEdit1.SelectedIndex == i)
                {
                    //textQi.Text = ployfitinfo[i - 1][3].ToString();
                    //textDi.Text = ployfitinfo[i - 1][4].ToString();
                    //textn.Text = ployfitinfo[i - 1][2].ToString();
                    break;
                }
                else
                    i++;
            }
        }

        private void btncal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (ployfitinfo == null)
                return;
            if (ployfitinfo.Count ==0)
                return;
            if (ployfitinfo.Count<comboBoxEdit1.SelectedIndex-1)
                  return;
            splashScreenManager1.ShowWaitForm();

            spreadsheetControl1.Visible = false;
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            int k=1;
            int kq = 5;
            int j=5;
           

            intialspreadsheet();
            for (int i = kq; i <= kq+26; i++)
                worksheet.Columns[i].ClearContents();
            int curve_num=0;
            for (int i = 0; i < ployfitinfo.Count; i++)
            {
                if (ployfitinfo[i][0] == comboBoxEdit1.SelectedIndex)
                {
                    if (ployfitinfo[i][1] == 0)// '甲型水驱曲线
                    {
                        curve_num = curve_num + 1;
                    }
                    if (ployfitinfo[i][1] == 1)// 'yi型水驱曲线
                    {
                        curve_num = curve_num + 1;
                    }
                    if (ployfitinfo[i][1] == 2)// 'bing型水驱曲线
                    {
                        curve_num = curve_num + 1;
                    }
                    if (ployfitinfo[i][1] == 3)// 'ding型水驱曲线
                    {
                        curve_num = curve_num + 1;
                    }
                }
            }
            for (int i=0;i<ployfitinfo.Count;i++)
            {
                if (ployfitinfo[i][0] == comboBoxEdit1.SelectedIndex)
                {
                    worksheet.Cells[k, 5+curve_num*3].Value = WatMethodchecked.Items[(int)ployfitinfo[i][1]].Description.ToString();
                    worksheet.Cells[k + 1, 5+curve_num*3].Value = "拟合方程";
                    worksheet.Cells[k + 2, 5+curve_num*3].Value = "拟合的相关系数";
                    worksheet.Cells[k + 3, 5 + curve_num * 3].Value = "Np";
                    // A rectangular range whose left column index is 0, top row index is 0, 
                    // right column index is 3 and bottom row index is 2. This is the A1:D3 cell range.
                    worksheet.MergeCells(worksheet.Range.FromLTRB(5 + curve_num * 3, k, 6 + curve_num * 3, k));

                    worksheet.Cells[k + 1, 6 + curve_num * 3].Value = formulainfo[i];
                    worksheet.Cells[k + 2, 6 + curve_num * 3].Value = ployfitinfo[i][6];
                    worksheet.Cells[k + 3, 6 + curve_num * 3].Value = ployfitinfo[i][7];
                    worksheet.Cells[k + 2, 6 + curve_num * 3].NumberFormat = "0.00";
                    worksheet.Cells[k + 3, 6 + curve_num * 3].NumberFormat = "0.00";
                    k = k + 4;
                    selectinfo[0] =(int) ployfitinfo[i][2];
                    selectinfo[1] = (int)ployfitinfo[i][3];
                    int countnum = selectinfo[1] + 1 - selectinfo[0];
                    double capital_b = ployfitinfo[i][4];//斜率
                    double capital_a = ployfitinfo[i][5];//截距
                    if (ployfitinfo[i][1]==0)// '甲型水驱曲线
                    {
                        int num_time = jia_horizon.Count;
                        //double[] predic_datay = new double[countnum];

                        for (int ii = 0; ii < num_time; ii++)
                        {
                            worksheet.Cells[ii+1, j].Value=jia_horizon[ii];
                            worksheet.Cells[ii + 1, j+1].Value = jia_vertical[ii];
                        }
                        //拟合段信息汇总，包括：拟合phase，曲线类型，开始点编号，结束点编号，斜率,截距,误差，NP
                        for (int jj = selectinfo[0]; jj < selectinfo[1] + 1; jj++)
                        {
                            //predic_datay[jj - (int)ployfitinfo[i][2]] = capital_b * jia_horizon[jj] + capital_a;
                            worksheet.Cells[jj + 1, j + 2].Value = capital_b * jia_horizon[jj] + capital_a;
                        }
                        worksheet.Cells[0, j].Value = "Np";
                        
                        worksheet.Cells[0, j + 1].Value = "Lg(Wp)";
                        worksheet.Columns[j].NumberFormat = "0.00";
                        worksheet.Columns[j+1].NumberFormat = "0.00";
                        worksheet.Cells[0, j + 2].Value = "预测" + worksheet.Cells[0, j + 1].Value;
                        worksheet.Columns[j + 2].NumberFormat = "0.00";
                    }
                    else if (ployfitinfo[i][1] == 1)//'乙型水驱曲线
                    {
                        int num_time = yi_horizon.Count;
                        for (int ii = 0; ii < num_time; ii++)
                        {
                            worksheet.Cells[ii + 1, j].Value = yi_horizon[ii];
                            worksheet.Cells[ii + 1, j + 1].Value = yi_vertical[ii];
                        }
                        for (int jj = selectinfo[0]; jj < selectinfo[1] + 1; jj++)
                        {
                            worksheet.Cells[jj + 1, j + 2].Value = capital_b * yi_horizon[jj] + capital_a;
                        }
                     worksheet.Cells[0, j].Value  = "Np";
                     worksheet.Cells[0, j + 1].Value = "Lg(Lp)";
                     worksheet.Columns[j].NumberFormat = "0.00";
                     worksheet.Columns[j + 1].NumberFormat = "0.00";
                     worksheet.Cells[0, j + 2].Value = "预测" + worksheet.Cells[0, j + 1].Value;
                     worksheet.Columns[j + 2].NumberFormat = "0.00";
                    }

                    else if (ployfitinfo[i][1] == 2)//  '丙型水驱曲线
                    {
                        int num_time = bing_horizon.Count;
                        for (int ii = 0; ii< num_time; ii++)
                        {
                            worksheet.Cells[ii + 1, j].Value = bing_horizon[ii];
                            worksheet.Cells[ii + 1, j + 1].Value = bing_vertical[ii];
                        }
                        for (int jj = selectinfo[0]; jj < selectinfo[1] + 1; jj++)
                        {
                            worksheet.Cells[jj + 1, j + 2].Value = capital_b * bing_horizon[jj] + capital_a;
                        }
                     worksheet.Cells[0, j].Value =  "Lp";
                     worksheet.Cells[0, j + 1].Value = "Lp/Np";
                     worksheet.Columns[j].NumberFormat = "0.00";
                     worksheet.Columns[j + 1].NumberFormat = "0.00";
                     worksheet.Cells[0, j + 2].Value = "预测" + worksheet.Cells[0, j + 1].Value;
                     worksheet.Columns[j + 2].NumberFormat = "0.00";
                    }
                    else if (ployfitinfo[i][1] == 3)// '丁型水驱曲线
                    {
                        int num_time = ding_horizon.Count;
                        for (int ii = 0; ii < num_time; ii++)
                        {
                            worksheet.Cells[ii + 1, j].Value = ding_horizon[ii];
                            worksheet.Cells[ii + 1, j + 1].Value = ding_vertical[ii];
                        }
                        for (int jj = selectinfo[0]; jj < selectinfo[1] + 1; jj++)
                        {
                            worksheet.Cells[jj + 1, j + 2].Value = capital_b * ding_horizon[jj] + capital_a;
                        }
                     worksheet.Cells[0, j].Value =   "Wp";
                     worksheet.Cells[0, j + 1].Value = "Lp/Np";
                  
                     worksheet.Cells[0, j + 2].Value = "预测" + worksheet.Cells[0, j + 1].Value;
                     worksheet.Columns[j].NumberFormat = "0.00";
                     worksheet.Columns[j + 1].NumberFormat = "0.00";
                     worksheet.Columns[j + 2].NumberFormat = "0.00";
                    }
                    j = j + 3;
                }
             
            }
            
            //worksheet.Columns[5].Fill.BackgroundColor = Color.FromArgb(217, 217, 217);//Color.Navy;
            splashScreenManager1.CloseWaitForm();
            btnoutExcel.Enabled = true;
            spreadsheetControl1.Visible = true;
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
      
        private void barCheckItem1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (barCheckItem1.Checked)
            {
                //dockPanel1.Show();
            }
            else
            {
                //dockPanel1.Hide();
            }
        }

        private void barCheckItem2_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (barCheckItem2.Checked)
            //{
            //    dockPanel2.Show();

            //}
            //else
            //{
            //    dockPanel2.Hide();
            //}
        }

        private void btnoutExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            spreadsheetControl1.Visible = false;
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            IWorkbook workbook = spreadsheetControl1.Document;
            int i=7;
            int startcol = 5;
            while (worksheet[0, i].Value.IsEmpty == false)
            {
                i = i + 1;
            }
            int waterflooding_curve_num = (int)((i - startcol) / 3);
            for (i = 0; i < waterflooding_curve_num; i++)
            {
                //Create a chart and specify its location.
                Chart chart = worksheet.Charts.Add(ChartType.ScatterMarkers);
                int topcell = 4;
                int leftcell = 5;
                chart.TopLeftCell = worksheet.Cells[topcell, leftcell];
                chart.BottomRightCell = worksheet.Cells[topcell + 20, leftcell + 7];

                ///画图数据原
                DevExpress.Spreadsheet.Range range1;
                DevExpress.Spreadsheet.Range range2;

                range1 = worksheet.Range.FromLTRB(startcol + i * 3, 1, startcol + i * 3, getData.Count);
                range2 = worksheet.Range.FromLTRB(startcol + i * 3 + 1, 1, startcol + i * 3 + 1, getData.Count);
                // Add chart series using worksheet ranges as the data sources.
                chart.Series.Add(worksheet["F1"], range1, range2);//原始序列

                // A rectangular range whose left column index is 0, top row index is 0, 
                // right column index is 3 and bottom row index is 2. This is the A1:D3 cell range.
                range2 = worksheet.Range.FromLTRB(startcol + i * 3 +2, 1, startcol + i * 3 +2, getData.Count);
                // Add chart series using worksheet ranges as the data sources.
                chart.Series.Add(worksheet["F1"], range1, range2);//结果序列



                DevExpress.Spreadsheet.Charts.Axis axis = chart.PrimaryAxes[0];
                axis.Scaling.AutoMax = true;
                //axis.Scaling.Max = worksheet.Cells[resulty.Count, 5].Value.NumericValue;
                //axis.Scaling.AutoMin = false;
                //axis.Scaling.Min = worksheet.Cells[1, 0].Value.NumericValue;
                axis.Title.SetValue(worksheet.Cells[0,startcol + i * 3].Value.ToString());
                chart.PrimaryAxes[1].Title.SetValue(worksheet.Cells[0, startcol + i * 3+1].Value.ToString());

                chart.Legend.Visible = false;
               

                chart.Title.Visible = true;
                chart.Title.SetValue(worksheet.Cells[4*i+1,5].Value.ToString());
                chart.Title.Font.Size = 11;
                chart.Series[0].Marker.Symbol = MarkerStyle.Circle;
                chart.Series[0].Marker.Size = 5;
                //chart.Series[0].Outline.SetNoFill();
                //chart.Series[0].ChangeType(ChartType.ScatterMarkers);
                chart.Series[1].ChangeType(ChartType.ScatterSmooth);
                chart.Series[1].Outline.SetSolidFill(Color.Red);
                chart.Series[0].Marker.Outline.SetSolidFill(Color.Orange);
                chart.Series[0].Marker.Fill.SetSolidFill(Color.FromArgb(125, Color.Orange));
                chart.PlotArea.Outline.SetSolidFill(Color.Black);

            }


            DocumentFormat format = DocumentFormat.Xlsx;
            //myChart.ExportToImage(fileName, format);

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导出结果图表";
            fileDialog.Filter = "EXCEL|*.xlsx|EXCEL 97-03|*.xls";
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
                    case "xlsx": format = DocumentFormat.Xlsx; break;
                    case "xls": format = DocumentFormat.Xls; break;
                }
                workbook.SaveDocument(localFilePath, format);
                while (worksheet.Charts.Count > 0)
                { worksheet.Charts[0].Delete(); }


                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            while (worksheet.Charts.Count > 0)
            { worksheet.Charts[0].Delete(); }
            spreadsheetControl1.Visible = true;
        }

        private void btnhelp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\help\\水驱曲线.pdf");
        }

        private void spreadsheetCommandBarButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            IWorkbook workbook = spreadsheetControl1.Document;
            DocumentFormat format = DocumentFormat.Xlsx;
            //myChart.ExportToImage(fileName, format);

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导出结果图表";
            fileDialog.Filter = "EXCEL|*.xlsx|EXCEL 97-03|*.xls";
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
                    case "xlsx": format = DocumentFormat.Xlsx; break;
                    case "xls": format = DocumentFormat.Xls; break;
                }
                workbook.SaveDocument(localFilePath, format);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


    }
}
