using DevExpress.Snap.Core.API;
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
using DevExpress.XtraRichEdit.API.Native;
using System.Drawing.Printing;
using DevExpress.XtraRichEdit;
namespace fracture
{
    public partial class PlugAgent : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CategoryRow rowMain = new CategoryRow("Main");
        public PlugAgent()
        {
            InitializeComponent();
            initialvgridcontrol();
            //  snapControl1.Visible = false;
            snapControl1.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            snapControl1.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            snapControl1.LoadDocument(Application.StartupPath + "\\PlugAgent.snx", SnapDocumentFormat.Snap);
            ribbonControl1.Visible = true;
            snapControl1.Document.Sections[0].Page.PaperKind = PaperKind.A4;
           // snapControl1.ActiveViewType = RichEditViewType.Simple;
        }

        private void initialvgridcontrol()
        {
            vGridControl1.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
            vGridControl1.LayoutStyle = LayoutViewStyle.SingleRecordView;
            vGridControl1.Rows.Add(rowMain);


            addNewRow("widthf", "裂缝宽度", 7.01, "mm");
            addNewRow("heightf", "裂缝高度", 29.1, "m");
            addNewRow("lengthf", "半缝长", 28.7, "m");
            addNewRow("porsand", "平均砂比", 35.2, "%");
            addNewRow("vs", "压裂液体积", 58.2, "m");
            addNewRow("lengthm", "高渗基质长", 28.7, "m");
            addNewRow("widthm", "高渗基质宽", 25.7, "m");
            addNewRow("heightm", "高渗基质高", 29.1, "m");
            addNewRow("lengthgel", "堵剂充填范围", 5, "m");
            addNewRow("pormbef", "原地层基质孔隙度", 7.16, "%");
            addNewRow("wgel", "封堵宽度", 5, "m");
            addNewRow("wm", "基质宽度", 28.2, "m");
            addNewRow("a", "滤失系数a", 0.83, "");
            addNewRow("b", "滤失系数b", 0.8, "");
            addNewRow("c", "滤失系数c", 0.61, "");

            EditorRow datasmooth = new EditorRow("Type");
            datasmooth.Properties.Caption = "堵剂类型";
            RepositoryItemComboBox datasmoothedit = new RepositoryItemComboBox();
            datasmoothedit.Items.AddRange(new string[] { "触变型", "聚合物", "SP-1" });
            // adding the Trademark row to the Main row's child collection 
            rowMain.ChildRows.Add(datasmooth);
            vGridControl1.RepositoryItems.Add(datasmoothedit);
            //Now you can define the repository item as an inplace editor of columns
            // populating the combo box with data
            datasmooth.Properties.Value = "聚合物";
            datasmooth.Properties.RowEdit = datasmoothedit;


            addNewRow("ts", "初凝时间", 8, "h");
            addNewRow("tf", "终凝时间", 20, "h");


        }
        private void addNewRow(string name, string cap, double defaultvalue, string unitcap)
        {
            EditorRow ni = new EditorRow(name);
            ni.Properties.Caption = cap;//"递减指数"

            rowMain.ChildRows.Add(ni);
            ni.Properties.Value = defaultvalue;
            if (unitcap != "")
            {
                RepositoryItemButtonEdit rib = new RepositoryItemButtonEdit();//Button按钮
                rib.Buttons[0].Caption = unitcap;
                rib.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;//按钮样式
                ni.Properties.RowEdit = rib;
            }

        }
        private void initialexcel(double widthf, double heightf, double lengthf, double porsand,
           double vs, double lengthm, double widthm, double heightm, double lengthgel, double pormbef,
           double wgel, double wm, double a, double b, double c, string type, double ts, double tf, double vgel)
        {
            // The following line opens a specified document in a Snap application. 
            snapControl1.LoadDocument(Application.StartupPath + "\\PlugAgent.snx", SnapDocumentFormat.Snap);
            DataTable dt = new DataTable();
            dt.Columns.Add("输入参数1", Type.GetType("System.String"));
            dt.Columns.Add("单位1", Type.GetType("System.String"));
            dt.Columns.Add("参数值1", Type.GetType("System.String"));
            dt.Columns.Add("输入参数2", Type.GetType("System.String"));
            dt.Columns.Add("单位2", Type.GetType("System.String"));
            dt.Columns.Add("参数值2", Type.GetType("System.String"));

            //dt.Rows.Add(new object[] { "输入参数", "", "", "" });
            //dt.Rows.Add(new object[] { "输入参数", "单位", "输入参数", "单位" });
            dt.Rows.Add(new object[] { "裂缝宽度", "mm", widthf.ToString(), "原地层基质孔隙度", "%", pormbef.ToString() });
            dt.Rows.Add(new object[] { "裂缝高度", "m", heightf.ToString(), "封堵宽度", "m", wgel.ToString() });
            dt.Rows.Add(new object[] { "半缝长", "m", lengthf.ToString(), "基质宽度", "m", wm.ToString() });
            dt.Rows.Add(new object[] { "平均砂比", "%", porsand.ToString(), "滤失系数a", "", a.ToString() });
            dt.Rows.Add(new object[] { "压裂液体积", "m", vs.ToString(), "滤失系数b", "", b.ToString() });
            dt.Rows.Add(new object[] { "高渗基质长", "m", lengthm.ToString(), "滤失系数c", "", c.ToString() });
            dt.Rows.Add(new object[] { "高渗基质宽", "m", widthm.ToString(), "堵剂类型", "", type });
            dt.Rows.Add(new object[] { "高渗基质高", "m", heightm.ToString(), "初凝时间", "h", ts.ToString() });
            dt.Rows.Add(new object[] { "堵剂充填范围", "m", lengthgel.ToString(), "终凝时间", "h", tf.ToString() });
            DataTable dt2 = new DataTable();
            //dt.Rows.Add(new object[] { "计算结果", "", "", "" });
            //dt.Rows.Add(new object[] { "堵剂的最终用量", " m3", "", "" });
            dt2.Columns.Add("输出参数", Type.GetType("System.String"));
            dt2.Columns.Add("单位", Type.GetType("System.String"));
            dt2.Columns.Add("参数值", Type.GetType("System.String"));

            dt2.Rows.Add(new object[] { "堵剂用量", "m3", vgel.ToString() });

            DataSet dtset = new DataSet();
            dtset.Tables.Add(dt);
            dtset.Tables.Add(dt2);

            //DataSet xmlDataSet = new DataSet();
            //xmlDataSet.ReadXml("C:\\Users\\Public\\Documents\\DevExpress Demos 14.1\\Components\\Data\\Cars.xml");
            snapControl1.DataSource = dtset;
            snapControl1.Document.Fields.Update();
     
            snapControl1.Visible = true;

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            // snapControl1.SaveDocument(Application.StartupPath + "PlugAgent.snx", SnapDocumentFormat.Snap);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           // snapControl1.Options.Layout.SimpleView.=DevExpress.XtraRichEdit.RichEditViewType.Simple;//;// SimpleView;
            snapControl1.LoadDocument(Application.StartupPath + "\\PlugAgent.snx", SnapDocumentFormat.Snap);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string Type = vGridControl1.Rows["categoryMain"].ChildRows["rowType"].Properties.Value.ToString();
            double widthf = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowwidthf"].Properties.Value);
            double heightf = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowheightf"].Properties.Value);
            double lengthf = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowlengthf"].Properties.Value);
            double porsand = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowporsand"].Properties.Value);
            double vs = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowvs"].Properties.Value);
            double lengthm = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowlengthm"].Properties.Value);
            double widthm = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowwidthm"].Properties.Value);
            double heightm = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowheightm"].Properties.Value);
            double lengthgel = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowlengthgel"].Properties.Value);
            double pormbef = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowpormbef"].Properties.Value);
            double wgel = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowwgel"].Properties.Value);
            double wm = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowwm"].Properties.Value);
            double a = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowa"].Properties.Value);
            double b = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowb"].Properties.Value);
            double c = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowc"].Properties.Value);
            double ts = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowts"].Properties.Value);
            double tf = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowtf"].Properties.Value);
            pormbef = pormbef / 100;
            porsand = porsand / 100;
            double vgel = algorithm.gel(widthf, heightf, lengthf, porsand, vs, lengthm, lengthgel, pormbef, widthm, heightm, wgel, wm);
            vgel = algorithm.gel(a, b, c, vgel);
            vgel = algorithm.gel(ts, tf, vgel, Type);
            initialexcel(widthf, heightf, lengthf, porsand, vs, lengthm, widthm, heightm, lengthgel, pormbef, wgel, wm, a, b, c, Type, ts, tf, vgel);
            //   GenerateLayout(snapControl1.Document); 

        }


    }
}
