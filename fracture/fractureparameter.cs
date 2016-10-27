using DevExpress.XtraCharts;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fracture
{

    public partial class fractureparameter : Form
    {
        public CategoryRow rowMain = new CategoryRow("Main");
        public fractureparameter()
        {
            InitializeComponent();
            initialvgridcontrol();
            // Handling the QueryControl event that will populate all automatically generated Documents
            //this.tabbedView1.QueryControl += tabbedView1_QueryControl;
            //tabbedView1.Controller.Activate(tabbedView1.Documents[0]);
            //tabbedView1.ActivateDocument(dockPanel3.FloatForm);
        }

        private void initialvgridcontrol()
        {
            vGridControl1.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
            vGridControl1.LayoutStyle = LayoutViewStyle.SingleRecordView;
            vGridControl1.Rows.Add(rowMain);
            // creating and modifying the Trademark row 
            EditorRow datasmooth = new EditorRow("model");
            datasmooth.Properties.Caption = "裂缝模型";
            RepositoryItemComboBox datasmoothedit = new RepositoryItemComboBox();
            datasmoothedit.Items.AddRange(new string[] { "PKN", "KGD" });
            // adding the Trademark row to the Main row's child collection 
            rowMain.ChildRows.Add(datasmooth);
            vGridControl1.RepositoryItems.Add(datasmoothedit);
            //Now you can define the repository item as an inplace editor of columns
            // populating the combo box with data
            datasmooth.Properties.Value = "KGD";
            datasmooth.Properties.RowEdit = datasmoothedit;

            addNewRow("tf", "施工时间", 2, "min");
            addNewRow("G", "岩石弹性模量", 15200, "MPa");
            addNewRow("Q", "施工排量", 2, "m3/min");
            addNewRow("v", "岩石泊松比", 0.291, "");
            addNewRow("mu", "压裂液粘度", 93, "mPa·s");
            addNewRow("h", "缝高", 29, "m");

        }

        private void addNewRow(string name, string cap, double defaultvalue, string unitcap)
        {
            EditorRow ni = new EditorRow(name);
            ni.Properties.Caption = cap;//"递减指数"
            // adding the Category row to the Model row's child collection 
            rowMain.ChildRows.Add(ni);
            ni.Properties.Value = defaultvalue;
            if (unitcap != "")
            {
                RepositoryItemButtonEdit rib = new RepositoryItemButtonEdit();//Button按钮
                //rib.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;//隐藏文字
                rib.Buttons[0].Caption = unitcap;
                rib.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;//按钮样式
                ni.Properties.RowEdit = rib;
            }

        }

        private void btncal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt = new DataTable();
            string model = vGridControl1.Rows["categoryMain"].ChildRows["rowmodel"].Properties.Value.ToString();
            double tf = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowtf"].Properties.Value);
            double G = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowG"].Properties.Value);
            double Q = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowQ"].Properties.Value);
            double v = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowv"].Properties.Value);
            double mu = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowmu"].Properties.Value);
            double h = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowh"].Properties.Value);
          //  double C = 0.0007;
          //  //#缝高 m
          //double  H=20;
          //  //# 粘度 pa*s
          //   mu=0.002;
          // double Qt=35;
          //  //# Q 为排量 m3/min
          //  Q=2.8;
          //  tf=Qt/Q;


            if (model == "PKN")
            {
                double g=algorithm.PKN(tf, G, Q, v, mu, h);
                MessageBox.Show(g.ToString());
            }
            else
            {
                dt=algorithm.KGD(tf, G, Q, v, mu, h);
                addNewPic(dt);
            }

      
        }

        private void addNewPic(DataTable dt)
        {
            // Create a chart.
            //
       
            ChartControl chart = chartControl2;//= chart;//= new ChartControl(); 
            chart.Series.Clear();
            // Create an empty Bar series and add it to the chart.
            Series series = new Series("裂缝半长", ViewType.Spline);
            chart.Series.Add(series);

            // Generate a data table and bind the series to it.
            series.DataSource = dt;

            // Specify data members to bind the series.
            series.ArgumentScaleType = ScaleType.Numerical;
            series.ArgumentDataMember = "time";
            series.ValueScaleType = ScaleType.Numerical;
            series.ValueDataMembers.AddRange(new string[] { "L" });
            ((PointSeriesView)series.View).Color = Color.FromArgb(125, Color.Green);
            // Create an empty Bar series and add it to the chart.

            Series series1 = new Series("缝宽", ViewType.Spline);
            chart.Series.Add(series1);

            // Generate a data table and bind the series to it.
            series1.DataSource = dt;

            // Specify data members to bind the series.
            series1.ArgumentScaleType = ScaleType.Numerical;
            series1.ArgumentDataMember = "time";
            series1.ValueScaleType = ScaleType.Numerical;
            series1.ValueDataMembers.AddRange(new string[] { "W0" });
            ((PointSeriesView)series1.View).Color = Color.FromArgb(125, Color.Red);

         

            chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;


            //SecondaryAxisX myAxisX = new SecondaryAxisX("watercutx");
           

            chart.CrosshairOptions.ShowArgumentLabels = true;
            chart.CrosshairOptions.ShowArgumentLine = true;
            chart.CrosshairOptions.ShowValueLabels = true;
            chart.CrosshairOptions.ShowValueLine = true;
            chart.CrosshairOptions.ValueLineColor = Color.DarkBlue;

            chart.CrosshairOptions.ArgumentLineColor = Color.DarkBlue;
            chart.CrosshairOptions.ShowCrosshairLabels = false;
            //series1.CrosshairLabelPattern = series1.Name + ":{V:F0}";
            //series2.CrosshairLabelPattern = series1.Name + ":{V:F0}";
            // chart.CrosshairOptions.sh = true;

            //series2.CrosshairOptions.ShowArgumentLabels  rosshairOptions.ShowValueLabels
            //  series2.CrosshairOptions.sho  CrosshairOptions.ShowValueLine, 
            //this.Controls.Add();

         

            XYDiagram diagram = (XYDiagram)chart.Diagram;
           diagram.Panes.Clear();
            diagram.SecondaryAxesY.Clear();
            diagram.SecondaryAxesX.Clear();

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
            diagram.AxisX.Title.Text = "时间（min）";
            diagram.AxisX.Title.TextColor = Color.Black;
            diagram.AxisX.Title.Antialiasing = true;
            diagram.AxisX.Title.Font = new Font("Microsoft YaHei", 10);
            diagram.AxisX.WholeRange.SideMarginsValue = 0;
            // Customize the appearance of the Y-axis title.
            diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisY.Title.Alignment = StringAlignment.Center;
            diagram.AxisY.Title.Text = "缝长（m）";
            diagram.AxisY.Title.TextColor = Color.Black;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("Microsoft YaHei", 10);
            XYDiagramPane pane = new XYDiagramPane("留白");
            diagram.Panes.Add(pane);
            XYDiagramSeriesViewBase view = (XYDiagramSeriesViewBase)chart.Series[0].View;
            int axesPosition = diagram.SecondaryAxesY.Add(new SecondaryAxisY());
            diagram.SecondaryAxesY[axesPosition].Alignment = AxisAlignment.Near;
            diagram.SecondaryAxesY[axesPosition].GridLines.Visible = true;
            diagram.SecondaryAxesY[axesPosition].GridLines.MinorVisible = true;
            view.AxisY = diagram.SecondaryAxesY[axesPosition];
            int axesPosition2 = diagram.SecondaryAxesX.Add(new SecondaryAxisX());
            diagram.SecondaryAxesX[axesPosition2].Alignment = AxisAlignment.Near;
            diagram.SecondaryAxesX[axesPosition2].GridLines.Visible = true;

            view.AxisX = diagram.SecondaryAxesX[axesPosition2];
            view.AxisX.WholeRange.SideMarginsValue = 0;
            diagram.SecondaryAxesX[axesPosition2].Title.Text = "时间（min）";
            diagram.SecondaryAxesY[axesPosition].Title.Text = "缝宽(mm)";
            diagram.SecondaryAxesX[axesPosition2].Title.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            diagram.SecondaryAxesY[axesPosition].Title.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            diagram.SecondaryAxesX[axesPosition2].Title.Visible = true;
            diagram.SecondaryAxesY[axesPosition].Title.Visible = true;
            diagram.SecondaryAxesX[axesPosition2].Title.Font = new Font("Microsoft YaHei", 10);
            diagram.SecondaryAxesY[axesPosition2].Title.Font = new Font("Microsoft YaHei", 10);
            view.Pane = pane;
        }

        // Assigning a required content for each auto generated Document
        void tabbedView1_QueryControl(object sender, DevExpress.XtraBars.Docking2010.Views.QueryControlEventArgs e)
        {

            
            if (e.Control == null)
                e.Control = new System.Windows.Forms.Control();
        }

    }
}
