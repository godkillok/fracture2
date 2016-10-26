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
using Global;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace fracture
{
    public partial class WellHeadTemp : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CategoryRow rowMain = new CategoryRow("Main");
        [Serializable]
        public class Temp
        {
            double pi;
            //原始地层压力
            public double Pi
            {
                get { return pi; }
                set { pi = value; }
            }
            double h;
            //井深度
            public double H
            {
                get { return h; }
                set { h = value; }
            }

            double wf;
            //缝宽
            public double Wf
            {
                get { return wf; }
                set { wf = value; }
            }
            double v;
            //注入速度
            public double V
            {
                get { return v; }
                set { v = value; }
            }
            double rou;
            //密度
            public double Rou
            {
                get { return rou; }
                set { rou = value; }
            }
            double radiu;
            //封堵半价
            public double Radiu
            {
                get { return radiu; }
                set { radiu = value; }
            }
            string type;
            //leixing 
            public string Type
            {
                get { return type; }
                set { type = value; }
            }
        }
        Temp temp_para = new Temp();
        public WellHeadTemp()
        {
            InitializeComponent();
            string strDestination = Application.StartupPath+ "\\case\\demoproject\\project\\WellHeadPressure.xml";
            if (Globalname.localFilePath != "")
                strDestination = Globalname.localFilePath + "\\project\\WellHeadPressure.xml";
            temp_para = LoadNet(strDestination);
            initialvgridcontrol();
            //  snapControl1.Visible = false;
            snapControl1.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            snapControl1.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            snapControl1.LoadDocument(Application.StartupPath + "\\PlugAgent.snx", SnapDocumentFormat.Snap);
            ribbonControl1.Visible = true;
            snapControl1.Document.Sections[0].Page.PaperKind = PaperKind.A4;
           // snapControl1.ActiveViewType = RichEditViewType.Simple;
        }
        private static Temp LoadNet(string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            Temp net = (Temp)formatter.Deserialize(fs);
            fs.Close();
            return net;
        }
        private void initialvgridcontrol()
        {
            vGridControl1.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
            vGridControl1.LayoutStyle = LayoutViewStyle.SingleRecordView;
            vGridControl1.Rows.Add(rowMain);
            if (temp_para.Type!=null)
            {
                addNewRow("pi", "原始地层压力",temp_para.Pi, "MPa");
                addNewRow("h", "井深", temp_para.H, "m");
                addNewRow("wf", "缝宽", temp_para.Wf, "mm");
                addNewRow("v", "注入速度", temp_para.V, "m^3/min");
                addNewRow("rou", "密度", temp_para.Rou, "kg/m^3");
                addNewRow("radiu", "封堵半径", temp_para.Radiu, "m");
            }
            else
            {
                addNewRow("pi", "原始地层压力", 7.01, "MPa");
                addNewRow("h", "井深", 29.1, "m");
                addNewRow("wf", "缝宽", 28.7, "mm");
                addNewRow("v", "注入速度", 35.2, "m^3/min");
                addNewRow("rou", "密度", 58.2, "kg/m^3");
                addNewRow("radiu", "封堵半径", 28.7, "m");
            }

        
            EditorRow datasmooth = new EditorRow("Type");
            datasmooth.Properties.Caption = "堵剂类型";
            RepositoryItemComboBox datasmoothedit = new RepositoryItemComboBox();
            datasmoothedit.Items.AddRange(new string[] { "触变型", "聚合物", "SP-1" });
            // adding the Trademark row to the Main row's child collection 
            rowMain.ChildRows.Add(datasmooth);
            vGridControl1.RepositoryItems.Add(datasmoothedit);
            //Now you can define the repository item as an inplace editor of columns
            // populating the combo box with data
            if (temp_para != null)
            {
                datasmooth.Properties.Value = temp_para.Type;
            }
            else
            datasmooth.Properties.Value = "聚合物";
            datasmooth.Properties.RowEdit = datasmoothedit;
            temp_para.Type = vGridControl1.Rows["categoryMain"].ChildRows["rowType"].Properties.Value.ToString();
            temp_para.Pi = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowpi"].Properties.Value);
            temp_para.H = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowh"].Properties.Value);
            temp_para.Wf = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowwf"].Properties.Value);
            temp_para.V = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowv"].Properties.Value);
            temp_para.Rou = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowrou"].Properties.Value);
            temp_para.Radiu = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowradiu"].Properties.Value);

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
        private void initialexcel(double vgel)
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
   

            dt.Rows.Add(new object[] { "原始地层压力", "MPa", temp_para.Pi.ToString(), "井深", "m",  temp_para.H.ToString() });
            dt.Rows.Add(new object[] { "缝宽", "mm", temp_para.Wf.ToString(), "注入速度", "m^3/min", temp_para.V .ToString() });
            dt.Rows.Add(new object[] { "密度", "kg/m^3", temp_para.Rou.ToString(), "封堵半径", "m", temp_para.Radiu.ToString() });
            dt.Rows.Add(new object[] { "类型", "", temp_para.Type.ToString() });
            DataTable dt2 = new DataTable();
            //dt.Rows.Add(new object[] { "计算结果", "", "", "" });
            //dt.Rows.Add(new object[] { "堵剂的最终用量", " m3", "", "" });
            dt2.Columns.Add("输出参数", Type.GetType("System.String"));
            dt2.Columns.Add("单位", Type.GetType("System.String"));
            dt2.Columns.Add("参数值", Type.GetType("System.String"));

            dt2.Rows.Add(new object[] { "井口压力", "MPa", vgel.ToString() });

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
            snapControl1.LoadDocument(Application.StartupPath + "\\Config\\WellHeadPressure.snx", SnapDocumentFormat.Snap);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            temp_para.Type = vGridControl1.Rows["categoryMain"].ChildRows["rowType"].Properties.Value.ToString();
            temp_para.Pi = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowpi"].Properties.Value);
            temp_para.H = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowh"].Properties.Value);
            temp_para.Wf = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowwf"].Properties.Value);
            temp_para.V = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowv"].Properties.Value);
            temp_para.Rou = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowrou"].Properties.Value);
            temp_para.Radiu = Convert.ToDouble(vGridControl1.Rows["categoryMain"].ChildRows["rowradiu"].Properties.Value);

         double wellheadtemp=temp_para.Pi+delat()*temp_para.Radiu+temp_para.Rou*9.8*temp_para.H;
            
            initialexcel(wellheadtemp);
            //   GenerateLayout(snapControl1.Document); 

        }

       private double  delat()
       {
           double delatp = 0;
           switch (temp_para.Type)
           {
               //    触变凝胶堵剂注入体积一般较小
               case "触变型":
                   {
                       delatp =Math.Pow(10,1.4637-0.0931*temp_para.Wf)*temp_para.V+51.704*Math.Pow(temp_para.Wf,-0.2535);
                       break;
                   }
               //  聚合物凝胶堵剂可适当延长注入时间
               default:
                   {
                       delatp =Math.Pow(10,1.4676-0.0542*temp_para.Wf)*temp_para.V+63.96*Math.Pow(temp_para.Wf,-0.2037);
                       break;
                   }
           }
           return delatp;
       
       }

       private void btnsave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
       {

           string strDestination = Application.StartupPath + "\\case\\demoproject\\project\\WellHeadPressure.xml";
           if (Globalname.localFilePath != "")
               strDestination = Globalname.localFilePath + "\\project\\WellHeadPressure.xml";

           string strPath = Path.GetDirectoryName(strDestination);
           if (!Directory.Exists(strPath))
           {
               Directory.CreateDirectory(Path.GetDirectoryName(strPath));
           }
           SaveNet(temp_para, strDestination);
       }
       private static void SaveNet(Temp Net, string FilePath)
       {
           FileStream fs = new FileStream(FilePath, FileMode.Create);
           BinaryFormatter formatter = new BinaryFormatter();
           formatter.Serialize(fs, Net);
           fs.Close();
       }

    }
}
