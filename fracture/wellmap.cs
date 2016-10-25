using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System.Xml;
using System.IO;
using ILNumerics.Toolboxes;
using DevExpress.XtraCharts;
using System.Collections;
using System.ComponentModel;
using System.Data;
using DevExpress.Spreadsheet;
using System.Drawing.Imaging;
using System.Diagnostics;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraEditors.Repository;
using System.Text.RegularExpressions;
using Global;

namespace fracture
{
    public partial class wellmap : Form
    {
        public CategoryRow rowMain = new CategoryRow("Main");
        public CategoryRow contour = new CategoryRow("contour");
        public CategoryRow bubble = new CategoryRow("bubble");
        public DataTable dt_oilDynamicIndexnName;//动态指标名称
        public DataTable dt_waterDynamicIndexnName;//动态指标名称
        private DataTable dt_StaticndexnName;//静态指标名称
        DataTable dt_TableAndField;//字段名称汇总
        DataTable dt_line;//裂缝
       
        class BlankNumStruct
        {
            public int ID;
            public int Num;
        }

        class BlankStruct
        {
            public double X_axis;
            public double Y_axis;
            public int type;
        }


        public wellmap()
        {
            InitializeComponent();

            treelistview.InitTreeView(treeList1, true);
            // Handling the QueryControl event that will populate all automatically generated Documents
            treelistview.AddWellNodes(treeList1);
            SetImageIndex(imageCollection1, treeList1, null, 1, 0);

            treeList1.AfterCheckNode += treeList1_AfterCheckNode;
            treeList1.BeforeCheckNode += treeList1_BeforeCheckNode;
            initialvgridcontrol();
            barEditItem1.EditValue = DateTime.Now;
            // int YM = Int32.Parse(string.Format(barEditItem1.EditValue.ToString()));




        }

        private void initialvgridcontrol()
        {
            vGridControl1.ShowButtonMode = DevExpress.XtraVerticalGrid.ShowButtonModeEnum.ShowAlways;
            vGridControl1.LayoutStyle = LayoutViewStyle.SingleRecordView;
            vGridControl1.Rows.Add(rowMain);
            vGridControl1.Rows.Add(contour);
            vGridControl1.Rows.Add(bubble);

            rowMain.Properties.Caption = "全局设置";
            contour.Properties.Caption = "等值线/表名图设置";
            bubble.Properties.Caption = "泡泡图设置";

            // creating and modifying the Trademark row 
            EditorRow gird = new EditorRow("gird");
            gird.Properties.Caption = "网格数";
            gird.Properties.Value = "100";
            rowMain.ChildRows.Add(gird);

            EditorRow xmin = new EditorRow("xmin");
            xmin.Properties.Caption = "X轴最小值";
            xmin.Properties.Value = "";
            rowMain.ChildRows.Add(xmin);

            EditorRow xmax = new EditorRow("xmax");
            xmax.Properties.Caption = "X轴最大值";
            xmax.Properties.Value = "";
            rowMain.ChildRows.Add(xmax);


            EditorRow ymin = new EditorRow("ymin");
            ymin.Properties.Caption = "Y轴最小值";
            ymin.Properties.Value = "";
            rowMain.ChildRows.Add(ymin);

            EditorRow ymax = new EditorRow("ymax");
            ymax.Properties.Caption = "Y轴最大值";
            ymax.Properties.Value = "";
            rowMain.ChildRows.Add(ymax);


            EditorRow model = new EditorRow("model");
            DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit datasmoothedit = new RepositoryItemCheckedComboBoxEdit();
            //  RepositoryItemComboBox datasmoothedit = new RepositoryItemComboBox();
            string[] ff = new string[] { "泡泡图", "等值线", "表面图" };
            datasmoothedit.Items.AddRange(ff);
            // adding the Trademark row to the Main row's child collection 
            rowMain.ChildRows.Add(model);
            vGridControl1.RepositoryItems.Add(datasmoothedit);
            //Now you can define the repository item as an inplace editor of columns
            // populating the combo box with data
            model.Properties.Value = "表面图";
            model.Properties.RowEdit = datasmoothedit;
            model.Properties.Caption = "图形类型";


            EditorRow bubblesize = new EditorRow("bubblesize");
            bubblesize.Properties.Caption = "饼图大小";
            bubble.ChildRows.Add(bubblesize);
            bubblesize.Properties.Value = "10";
            {
                dt_oilDynamicIndexnName = new DataTable();
                RepositoryItemComboBox DynamicIndexnNameedit = new RepositoryItemComboBox();
                EditorRow oiledit = new EditorRow("oiledit");
                string tablename = "T_OW_M";//
                dt_oilDynamicIndexnName = FDynamicIndexnName(tablename);
                List<string> DynamicIndexnName2 = new List<string>();
                for (int i = 0; i < dt_oilDynamicIndexnName.Rows.Count; i++)
                {
                    if (dt_oilDynamicIndexnName.Rows[i]["UNIT"].ToString() != "" & !dt_oilDynamicIndexnName.Rows[i]["name"].ToString().Contains("年月"))
                        DynamicIndexnName2.Add(dt_oilDynamicIndexnName.Rows[i]["name"].ToString());
                }
                string[] DynamicIndexnNameArray = DynamicIndexnName2.ToArray();
                bubble.ChildRows.Add(oiledit);
                DynamicIndexnNameedit.Items.AddRange(DynamicIndexnNameArray);
                vGridControl1.RepositoryItems.Add(DynamicIndexnNameedit);
                //Now you can define the repository item as an inplace editor of columns
                // populating the combo box with data
                oiledit.Properties.Value = "月产油量";
                oiledit.Properties.RowEdit = DynamicIndexnNameedit;
                oiledit.Properties.Caption = "油井指标";
            }

            {
                dt_waterDynamicIndexnName = new DataTable();
                RepositoryItemComboBox waterDynamicIndexnNameedit = new RepositoryItemComboBox();
                EditorRow wateredit = new EditorRow("wateredit");
                string tablename = "T_INJWW_M";//

                dt_waterDynamicIndexnName = FDynamicIndexnName(tablename);
                List<string> DynamicIndexnName2 = new List<string>();
                for (int i = 0; i < dt_waterDynamicIndexnName.Rows.Count; i++)
                {
                    if (dt_waterDynamicIndexnName.Rows[i]["UNIT"].ToString() != "" & !dt_waterDynamicIndexnName.Rows[i]["name"].ToString().Contains("年月"))
                        DynamicIndexnName2.Add(dt_waterDynamicIndexnName.Rows[i]["name"].ToString());
                }
                string[] DynamicIndexnNameArray = DynamicIndexnName2.ToArray();
                bubble.ChildRows.Add(wateredit);
                waterDynamicIndexnNameedit.Items.AddRange(DynamicIndexnNameArray);
                vGridControl1.RepositoryItems.Add(waterDynamicIndexnNameedit);
                //Now you can define the repository item as an inplace editor of columns
                // populating the combo box with data
                wateredit.Properties.Value = "月注水量";
                wateredit.Properties.RowEdit = waterDynamicIndexnNameedit;
                wateredit.Properties.Caption = "注水井指标";

            }

            //datasmoothedit.Items.AddRange(DynamicIndexnName);


            {
                FStaticIndexnName();

                RepositoryItemComboBox waterDynamicIndexnNameedit = new RepositoryItemComboBox();
                EditorRow staticedit = new EditorRow("surfaceedit");



                List<string> DynamicIndexnName2 = new List<string>();
                for (int i = 0; i < dt_StaticndexnName.Rows.Count; i++)
                {
                    if (dt_StaticndexnName.Rows[i]["UNIT"].ToString() != "" & !dt_StaticndexnName.Rows[i]["name"].ToString().Contains("年月") & !dt_StaticndexnName.Rows[i]["name"].ToString().Contains("日期"))
                        DynamicIndexnName2.Add(dt_StaticndexnName.Rows[i]["name"].ToString());
                }
                string[] DynamicIndexnNameArray = DynamicIndexnName2.ToArray();
                contour.ChildRows.Add(staticedit);
                waterDynamicIndexnNameedit.Items.AddRange(DynamicIndexnNameArray);
                vGridControl1.RepositoryItems.Add(waterDynamicIndexnNameedit);
                //Now you can define the repository item as an inplace editor of columns
                // populating the combo box with data
                staticedit.Properties.Value = "渗透率";
                staticedit.Properties.RowEdit = waterDynamicIndexnNameedit;
                staticedit.Properties.Caption = "等值线图指标";

            }
        }
        ///// <summary>
        /// 
        /// </summary>
        /// <param name="tablename">采油井/注水生产月报</param>
        private DataTable FDynamicIndexnName(string tablename)
        {
            DataTable dt = null;
            try
            {

                
                //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";

            
                string sheetName = "物理表汇总";

                string TableAndField = string.Format("select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT from [{0}$] where (库表名称='" + tablename + "')", sheetName);
                dt = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);

                return dt;

            }
            catch
            {
                return dt;
            }
        }
        private void FStaticIndexnName()
        {

            try
            {
                string tablename = "T_OW_M";//采油井生产月报
                string tablename2 = "T_WELL_RESE_CHARACTER";//静态指标
                string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
                //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";

                if (dt_StaticndexnName == null)
                {
                    dt_StaticndexnName = new DataTable();
                    string sheetName = "物理表汇总";

                    string TableAndField = string.Format("select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT from [{0}$] where (库表名称='" + tablename + "')" +
                        "union select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT from [{0}$] where (库表名称='" + tablename2 + "')", sheetName);
                    dt_StaticndexnName = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);

                }

            }
            catch
            {

            }
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
        private double ratio = 1;
        private double ratio3 = 1;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageCollection1"></param>
        /// <param name="treeList1"></param>
        /// <param name="node"></param>
        /// <param name="nodeIndex"></param>
        /// <param name="parentIndex"></param>

        private void treeList1_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);

        }

        private void treeList1_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
        }

        /// <summary>
        /// 设置子节点的状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedChildNodes(DevExpress.XtraTreeList.Nodes.TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckState = check;
                SetCheckedChildNodes(node.Nodes[i], check);
            }
        }

        /// <summary>
        /// 设置父节点的状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedParentNodes(DevExpress.XtraTreeList.Nodes.TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool b = false;
                CheckState state;
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                    if (!check.Equals(state))
                    {
                        b = !b;
                        break;
                    }
                }
                node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
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

        // Initial plot setup, modify this as needed


        private void ilPanel1_BeginRenderFrame(object sender, ILRenderEventArgs e)
        {
            ilPanel1.Scene.First<ILTriangles>().AutoNormals = false;

            if (ilPanel1.Size.Height == 0)
                return;
            double ratio2 = (double)ilPanel1.Size.Width / ilPanel1.Size.Height;

            foreach (var lp in ilPanel1.Scene.Find<ILGroup>("pieshape"))
            {
                Matrix4 change = new Matrix4(lp.Transform.M11, lp.Transform.M12, lp.Transform.M13, lp.Transform.M14,
                                lp.Transform.M21, lp.Transform.M11 * (float)(ratio2 * ratio3), lp.Transform.M23, lp.Transform.M24,
                                lp.Transform.M31, lp.Transform.M32, lp.Transform.M33, lp.Transform.M34,
                                lp.Transform.M41, lp.Transform.M42, lp.Transform.M43, lp.Transform.M44);
                lp.Transform = change;
            }
            ratio = (double)ilPanel1.Size.Width / ilPanel1.Size.Height;
        }


        # region 学习学习
        private class Computation : ILMath
        {
            /// <summary>
            /// Create some test data for plotting
            /// </summary>
            /// <param name="ang">end angle for a spiral</param>
            /// <param name="resolution">number of points to plot</param>
            /// <returns>3d data matrix for plotting, points in columns</returns>
            public static ILRetArray<float> CreateData(int ang, int resolution)
            {
                using (ILScope.Enter())
                {
                    ILArray<float> A = linspace<float>(0, ang * pi, resolution);
                    ILArray<float> Pos = zeros<float>(3, A.S[1]);
                    Pos["0;:"] = sin(A);
                    Pos["1;:"] = cos(A);
                    Pos["2;:"] = A;
                    return Pos;
                }
            }

        }
        # endregion 学习学习



        /// <summary>
        /// 导出图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnoutpic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImageFormat format = ImageFormat.Bmp;
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
                //using (FileStream fs = new FileStream(@"test.svg", FileMode.Create))
                //{
                //    new ILSVGDriver(fs, scene: ilPanel1.Scene).Render();
                //}
                var drv = new ILGDIDriver(ilPanel1.Width, ilPanel1.Height, ilPanel1.Scene);
                drv.Render();
                drv.BackBuffer.Bitmap.Save(localFilePath, format);
                // MessageBox.Show("Image saved to: " + path);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wellid">井ID列表</param>
        /// <param name="YM">年月</param>
        /// <param name="tablename">取数据的表</param>
        /// <param name="flag">油水井标志</param>
        /// <returns></returns>
        private DataTable GetDynamicIndexnData(List<string> wellid, string tablename, string filedname)
        {
            int YM = Int32.Parse(Convert.ToDateTime(barEditItem1.EditValue).ToString("yyyyMM"));

            DataTable dt = null;
            ;//字段名称
            string filedID;//字段id


            try
            {

                //string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
                //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";
                

                string sheetName = "物理表汇总";

                string TableAndField = string.Format("select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT from [{0}$] where (库表名称='" + tablename + "'and 列显示名称='" + filedname + "') ", sheetName);
                dt = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);
                //
                filedID = dt.Rows[0]["ID"].ToString();
                dt = null;

                string sSql = "";

                //                sSql = sSql + string.Format(@"iif(isnull(T0.{0}),0,T0.{0})  AS {1}, T0.Year_month as yearmonth , T1.well_name as wellname FROM 
                //(
                //select  T0.{0}, T0.Year_month , T0.well_ID FROM {2} T0 where T0.Year_month={3} 
                //) T0 
                //right join 
                //(
                //select T1.well_ID,T1.well_NAME FROM  T_WELL_INFOR T1 where T1.WELL_ID IN (", filedID, filedname, tablename, YM);
                //                for (int i = 0; i < wellid.Count - 1; i++)
                //                {
                //                    sSql = sSql + string.Format("'{0}',", wellid[i]);
                //                }
                //                sSql = sSql + string.Format(@"'{0}') 
                //) T1
                //on Left(T0.WELL_ID,255)= Left(T1.WELL_ID,255) 
                //order by T1.well_name
                //", wellid[wellid.Count - 1]);

                sSql = string.Format(@" 
select  T0.{0} AS {1}, T0.Year_month as yearmonthh , T0.well_name as WELLNAME FROM {2} T0 where T0.Year_month={3} and T0.WELL_ID IN (", filedID, filedname, tablename, YM);
                for (int i = 0; i < wellid.Count - 1; i++)
                {
                    sSql = sSql + string.Format("'{0}',", wellid[i]);
                }
                sSql = sSql + string.Format(@"'{0}') 
", wellid[wellid.Count - 1]);
                dt = OleDbHelper.getTable(sSql,  Globalname.DabaBasePath);

                return dt;


            }
            catch
            {
                return dt;
            }
        }

        private DataTable GetwellLocation(List<string> wellid)
        {

            DataTable dt = null;


            try
            {

                //string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
                //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";



                string tablename = "T_WELL_INFOR";

                string sSql = "select ";

                sSql += string.Format("well_NAME AS WELLNAME,well_CC AS WELLCC,well_x AS X_axis, well_y AS Y_axis FROM {0} where  WELL_ID IN ( ", tablename);
                for (int i = 0; i < wellid.Count - 1; i++)
                {
                    sSql = sSql + string.Format("'{0}',", wellid[i]);
                }
                sSql = sSql + string.Format(@"'{0}') 
                           order by {1}.well_CC ,{1}.well_name", wellid[wellid.Count - 1], tablename);
                dt = OleDbHelper.getTable(sSql,  Globalname.DabaBasePath);

                return dt;


            }
            catch
            {
                return dt;
            }
        }
        private DataTable GetSurfaceIndexnData(List<string> wellid)
        {
            string filedname = vGridControl1.Rows["categorycontour"].ChildRows["rowsurfaceedit"].Properties.Value.ToString();
            DataTable dt_StaticndexnName2 = null;
            DataTable dt = null;
            string filedID;
            string tablename3;
            string sSql = "";
            int YM = Int32.Parse(Convert.ToDateTime(barEditItem1.EditValue).ToString("yyyyMM"));
            //try
            {
                string tablename = "T_OW_M";//采油井生产月报
                string tablename2 = "T_WELL_RESE_CHARACTER";//静态指标
               // string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
                //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";

                if (dt_StaticndexnName2 == null)
                {
                    dt_StaticndexnName = new DataTable();
                    string sheetName = "物理表汇总";

                    string TableAndField = string.Format("select T0.name as name , T0.ID as ID ,T0.tablename as tablename FROM ( select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT  ,库表名称 as tablename from [{0}$] where (库表名称='" + tablename + "')" +
                        "union select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT,库表名称  as tablename from [{0}$] where (库表名称='" + tablename2 + "')) T0 where (T0.name='" + filedname + "')", sheetName);
                    dt_StaticndexnName2 = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);
                    filedID = dt_StaticndexnName2.Rows[0]["ID"].ToString();
                    tablename3 = dt_StaticndexnName2.Rows[0]["tablename"].ToString();
                    if (tablename3 == tablename2)
                    {
                        sSql = "select ";
                        sSql += string.Format("well_NAME AS WELLNAME,{0} AS {1} FROM {2} where  WELL_ID IN ( ", filedID, filedname, tablename3);
                        for (int i = 0; i < wellid.Count - 1; i++)
                        {
                            sSql = sSql + string.Format("'{0}',", wellid[i]);
                        }
                        sSql = sSql + string.Format(@"'{0}')", wellid[wellid.Count - 1], tablename);

                    }
                    else
                    {
                        sSql = string.Format(@" 
select  T0.{0} AS {1}, T0.Year_month as yearmonthh , T0.well_name as WELLNAME FROM {2} T0 where T0.Year_month={3} and T0.WELL_ID IN (", filedID, filedname, tablename3, YM);
                        for (int i = 0; i < wellid.Count - 1; i++)
                        {
                            sSql = sSql + string.Format("'{0}',", wellid[i]);
                        }
                        sSql = sSql + string.Format(@"'{0}') 
", wellid[wellid.Count - 1]);


                    }
                    dt = OleDbHelper.getTable(sSql,  Globalname.DabaBasePath);
                }
                return dt;
            }
            //catch
            //{
            //    return dt_StaticndexnName2;
            //}
        }


        private void draw(DataTable dt_line)
        {
            //Call the operation:
            GetCheckedNodesOperation op = new GetCheckedNodesOperation();
            treeList1.NodesIterator.DoOperation(op);
            string model = vGridControl1.Rows["categoryMain"].ChildRows["rowmodel"].Properties.Value.ToString();


            string filedname_oil = vGridControl1.Rows["categorybubble"].ChildRows["rowoiledit"].Properties.Value.ToString();

            string filedname_water = vGridControl1.Rows["categorybubble"].ChildRows["rowwateredit"].Properties.Value.ToString();
            string filedname_surface = vGridControl1.Rows["categorycontour"].ChildRows["rowsurfaceedit"].Properties.Value.ToString();
            List<string> wellid = new List<string>();
            TreeListNode clickedNode = this.treeList1.FocusedNode;
            for (int i = 0; i < op.CheckedNodes.Count; i++)
            {
                if (op.CheckedNodes[i].HasChildren == false)
                {
                    wellid.Add(op.CheckedNodes[i].GetValue("WELLID").ToString());
                }
            }
            if (wellid.Count == 0)
                return;
            //油井
            string tablename = "T_OW_M";
            DataTable dt_oil = GetDynamicIndexnData(wellid, tablename, filedname_oil);
            //水井
            tablename = "T_INJWW_M";
            DataTable dt_water = GetDynamicIndexnData(wellid, tablename, filedname_water);
            //井位
            DataTable dt_wellloction = GetwellLocation(wellid);
            //画等值线的指标
            DataTable dt_surface_index = GetSurfaceIndexnData(wellid);
            dt_wellloction.Columns.Add("Production", Type.GetType("System.Double"));
            dt_wellloction.Columns.Add("surface_index", Type.GetType("System.Double"));
            foreach (DataRow dr in dt_wellloction.Rows)
                foreach (DataRow dr2 in dt_oil.Rows)
                {
                    if (dr["wellname"].ToString() == dr2["wellname"].ToString())
                    {
                        dr["Production"] = dr2[filedname_oil];
                    }
                }

            foreach (DataRow dr in dt_wellloction.Rows)
                foreach (DataRow dr2 in dt_water.Rows)
                {
                    if (dr["wellname"].ToString() == dr2["wellname"].ToString())
                    {
                        dr["Production"] = dr2[filedname_water];
                    }
                }
            foreach (DataRow dr in dt_wellloction.Rows)
                foreach (DataRow dr2 in dt_surface_index.Rows)
                {
                    if (dr["wellname"].ToString() == dr2["wellname"].ToString())
                    {
                        dr["surface_index"] = dr2[filedname_surface];
                    }
                }




            //dt_wellloction.TableName = "MSGInfoName";
            //MemoryStream streamSaveUldByte = new MemoryStream();
            //dt_wellloction.WriteXml(streamSaveUldByte, XmlWriteMode.WriteSchema);
            //DataTable dt_wellloction_temp = null;
            //dt_wellloction_temp = dt_wellloction.Copy();
            double[,] welllocation = new double[dt_wellloction.Rows.Count, 2];

            string oilcode = "采油井";
            int oilwellnum =
     (from order in dt_wellloction.AsEnumerable()
      where order.Field<string>("WELLCC") == oilcode
      select order).Count();

            int temp_num = 0;//如果只有一口井时会出现错误，所以如果只有一口井，就自动加上一口井
            if (oilwellnum == 1)
            {
                temp_num = 1;
            }
            float[,] welllocation_oil = new float[oilwellnum + temp_num, 3];
            temp_num = 0;
            if (dt_wellloction.Rows.Count - oilwellnum == 1)
            {
                temp_num = 1;
            }
            float[,] welllocation_water = new float[dt_wellloction.Rows.Count - oilwellnum + temp_num, 3];
            float[,] production = new float[dt_wellloction.Rows.Count, 1];
            float[,] surface_index = new float[dt_wellloction.Rows.Count, 1];
            int oilk = 0;
            int waterk = 0;

            for (int i = 0; i < dt_wellloction.Rows.Count; i++)
            {
                welllocation[i, 0] = Convert.ToDouble(dt_wellloction.Rows[i]["X_axis"]);
                welllocation[i, 1] = Convert.ToDouble(dt_wellloction.Rows[i]["Y_axis"]);
                if (dt_wellloction.Rows[i]["production"].ToString() == "")
                    production[i, 0] = 0.00000000000001F;
                else
                    production[i, 0] = Convert.ToSingle(dt_wellloction.Rows[i]["production"]);
                if (dt_wellloction.Rows[i]["surface_index"].ToString() == "")
                    surface_index[i, 0] = 0.00000000000001F;
                else
                    surface_index[i, 0] = Convert.ToSingle(dt_wellloction.Rows[i]["surface_index"]);
                if (Convert.ToString(dt_wellloction.Rows[i]["WELLCC"]) == "采油井")
                {

                    welllocation_oil[oilk, 0] = Convert.ToSingle(welllocation[i, 0]);
                    welllocation_oil[oilk, 1] = Convert.ToSingle(welllocation[i, 1]);
                    welllocation_oil[oilk++, 2] = 1;
                }
                else
                {
                    welllocation_water[waterk, 0] = Convert.ToSingle(welllocation[i, 0]);
                    welllocation_water[waterk, 1] = Convert.ToSingle(welllocation[i, 1]);
                    welllocation_water[waterk++, 2] = 1;
                }
            }

            if (oilwellnum == 1)//如果只有一口井时会出现错误，所以如果只有一口井，就自动加上一口井,并且是水井的坐标，到时候画的时候，水井坐标就会覆盖这个假的水井
            {
                welllocation_oil[oilk, 0] = welllocation_water[waterk - 1, 0];
                welllocation_oil[oilk, 1] = welllocation_water[waterk - 1, 1];
                welllocation_oil[oilk, 2] = 0;
            }


            if (dt_wellloction.Rows.Count - oilwellnum == 1)//如果只有一口井时会出现错误，所以如果只有一口井，就自动加上一口井
            {
                welllocation_water[waterk, 0] = welllocation_oil[oilk - 1, 0];
                welllocation_water[waterk, 1] = welllocation_oil[oilk - 1, 1];
                welllocation_water[waterk++, 2] = 0;
            }

            //var dt_wellloction_var = from t in dt_wellloction.AsEnumerable() select new { X_axis = t.Field<double>("X_axis"), Y_axis = t.Field<double>("Y_axis") };
            //double[,] ff=
            //  List<double> ffg=dt_wellloction_var.ToList().ForEach(p=>p.X_axis);
            //List<double [,]> ff=dt_wellloction_var.ToArray<double[,]>();
            ILArray<double> welllocation_plot = welllocation;
            //double[,] scatteredPositions2 = new[,] { {149.6352,157.0626},
            //{	238.4967	,	168.7518	},
            //{	277.6083	,	226.7472	},
            //{	200.8128	,	060.4062	},
            //{	097.0929	,	178.2291	},
            //{	175.2507	,	269.0529	},
            //{	142.4604	,	094.7187	},
            //{	092.7549	,	158.1384	},
            //{	192.47688	,	151.09428	},
            //};

            //ILArray<double> ymax2 = scatteredPositions[];

            double ymax = welllocation_plot[1, ILMath.full].Max();
            double ymin = welllocation_plot[1, ILMath.full].Min();
            double xmax = welllocation_plot[0, ILMath.full].Max();
            double xmin = welllocation_plot[0, ILMath.full].Min();
            if ((xmax - xmin) == 0 || (ymax - ymin) == 0)
                return;
            double xlenth = Math.Abs(xmax - xmin);
            double ylenth = Math.Abs(ymax - ymin);
            ymax = (vGridControl1.Rows["categoryMain"].ChildRows["rowymax"].Properties.Value.ToString() != "" ? Convert.ToInt32(vGridControl1.Rows["categoryMain"].ChildRows["rowymax"].Properties.Value.ToString()) : (int)(ymax + 0.1 * ylenth));
            ymin = (vGridControl1.Rows["categoryMain"].ChildRows["rowymin"].Properties.Value.ToString() != "" ? Convert.ToInt32(vGridControl1.Rows["categoryMain"].ChildRows["rowymin"].Properties.Value.ToString()) : (int)(ymin - 0.1 * ylenth));
            xmax = (vGridControl1.Rows["categoryMain"].ChildRows["rowxmax"].Properties.Value.ToString() != "" ? Convert.ToInt32(vGridControl1.Rows["categoryMain"].ChildRows["rowxmax"].Properties.Value.ToString()) : (int)(xmax + 0.1 * xlenth));
            xmin = (vGridControl1.Rows["categoryMain"].ChildRows["rowxmin"].Properties.Value.ToString() != "" ? Convert.ToInt32(vGridControl1.Rows["categoryMain"].ChildRows["rowxmin"].Properties.Value.ToString()) : (int)(xmin - 0.1 * xlenth));
            int step = (vGridControl1.Rows["categoryMain"].ChildRows["rowgird"].Properties.Value.ToString() != "" ? Convert.ToInt32(vGridControl1.Rows["categoryMain"].ChildRows["rowgird"].Properties.Value.ToString()) : 100);

            double stepx = Math.Abs(xmax - xmin) / (step - 1);
            double stepy = Math.Abs(ymax - ymin) / (step - 1);
            if (stepx == 0)
                stepx = 0.1;
            if (stepy == 0)
                stepy = 0.1;
            // generate a grid with true values based on the custom function for surface plotting 
            ILArray<double> Y = 1, X = ILMath.meshgrid(ILMath.vec<double>(xmin, stepx, xmin + stepx * step), ILMath.vec<double>(ymin, stepy, ymin + stepy * step), Y);
            //  ILArray<double> Y = 1, X = ILMath.meshgrid(ILMath.vec<double>(0, stepx, 300), ILMath.vec<double>(0, stepx, 300), Y);
            ILArray<double> Grid = X.C;



            // pick only some values as source for interpolation
            // ILArray<double> scatteredPositions = ILMath.rand(2, 10) * 6000;

            //ILArray<double> scatteredValues = myFunc(scatteredPositions["0;:"], scatteredPositions["1;:"]);
            // ILArray<double> scatteredValues = ILMath.rand(1, scatteredPositions.Length) * 2;
            ILArray<double> scatteredValues = production;
            ILArray<double> surface_scatteredValues = surface_index;
            // let's plot it! 
            var scene = new ILScene();
            #region 定义了ILPlotCube（这个画图的基础），以及刻度的一系列
            var pc = scene.Add(new ILPlotCube(twoDMode: true)
            {

                Axes =
                {
                    YAxis =
                    {
                        GridMajor =
                        {
                            Visible = false
                        },
                        Label =
                        {
                            Visible = false
                        }
                    },
                    XAxis =
                    {
                        GridMajor =
                        {
                            Visible = false
                        },
                        Label =
                        {
                            Visible = false
                        },

                    },

                },


            });

            pc.First<ILPlotCube>().Axes.XAxis.Ticks.MaxNumberDigitsShowFull = 10;
            pc.First<ILPlotCube>().Axes.YAxis.Ticks.MaxNumberDigitsShowFull = 10;
            pc.First<ILPlotCube>().Axes.XAxis.Ticks.TickLength = -0.5f;
            pc.First<ILPlotCube>().Axes.YAxis.Ticks.TickLength = -0.5f;

            // fetch the default label for the axis
            var lab = pc.Axes.XAxis.Ticks.DefaultLabel;
            // configure the labels font
            lab.Font = new System.Drawing.Font("微软雅黑", 9,FontStyle.Bold);
            var labY = pc.Axes.YAxis.Ticks.DefaultLabel;
            // configure the labels font
            labY.Font = new System.Drawing.Font("微软雅黑", 9, FontStyle.Bold);
            #endregion







            # region 井点位置，图形样式


            ILArray<float> interpValues4Plot = ILMath.tosingle(welllocation_plot.Concat(scatteredValues, 0));
            ILArray<float> welllocation_oil_plot = welllocation_oil;
            ILArray<float> welllocation_water_plot = welllocation_water;
            #region 白化

            //列名 曲线编号；X；Y；类型
            //类型 0 外边界；1内边界；2为断层


            if (dt_line != null)
            {

                int linenum =
(from order in dt_line.AsEnumerable()
 group order by order.Field<int>("num")).Count();
                List<BlankNumStruct> list_num =
 (from order in dt_line.AsEnumerable()
  group order by order.Field<int>("num") into g
  select new BlankNumStruct
  {
      ID = g.Key,
      Num = g.Count()

  }).ToList<BlankNumStruct>();


                for (int k = 0; k < linenum; k++)
                {
                    List<BlankStruct> list_blank =
 (from order in dt_line.AsEnumerable()
  where order.Field<int>("num") == list_num[k].ID
  select new BlankStruct
  {
      X_axis = order.Field<double>("X_axis"),
      Y_axis = order.Field<double>("Y_axis"),
      type = order.Field<int>("type")
  }).ToList<BlankStruct>();
                    int nvert = list_blank.Count;
                    if (list_blank[0].X_axis != list_blank[nvert - 1].X_axis & list_blank[0].Y_axis != list_blank[nvert - 1].Y_axis & list_blank[0].type < 3)//如果首尾的坐标不是一样，就强制一样(断层线就算了)
                        nvert = nvert + 1;

                    float[] vertx = new float[nvert];
                    float[] verty = new float[nvert];
                    if (list_blank[0].type >= 3)
                        nvert = nvert + 1;
                    for (int i = 0; i < nvert - 1; i++)
                    {
                        vertx[i] = Convert.ToSingle(list_blank[i].X_axis);
                        verty[i] = Convert.ToSingle(list_blank[i].Y_axis);
                    }
                    if (list_blank[0].X_axis != list_blank[nvert - 2].X_axis & list_blank[0].Y_axis != list_blank[nvert - 2].Y_axis & list_blank[0].type < 3)
                    {
                        vertx[nvert - 1] = Convert.ToSingle(list_blank[0].X_axis);
                        verty[nvert - 1] = Convert.ToSingle(list_blank[0].Y_axis);
                        nvert = nvert - 1;
                    }
                    if (list_blank[0].type < 2)
                    {
                        for (int i = 0; i < welllocation_plot.Size[1]; i++)
                        {
                            // ILArray<float> interpValues4Plot = ILMath.tosingle(welllocation_plot.Concat(scatteredValues, 0));
                            float testx = Convert.ToSingle(welllocation_plot.GetValue(0, i));
                            float testy = Convert.ToSingle(welllocation_plot.GetValue(1, i));

                            bool init = inpolygon(nvert, vertx, verty, testx, testy);//true在里面
                            switch (list_blank[0].type)
                            {
                                case 0:
                                    {
                                        if (!init)
                                        {
                                            interpValues4Plot[2, i] = float.NaN;
                                        }
                                        break;
                                    };
                                case 1:
                                    {
                                        if (init)
                                        {
                                            interpValues4Plot[2, i] = float.NaN;
                                        }
                                        break;
                                    };
                            }

                        }

                        for (int i = 0; i < welllocation_oil_plot.Size[1]; i++)
                        {
                            // ILArray<float> interpValues4Plot = ILMath.tosingle(welllocation_plot.Concat(scatteredValues, 0));
                            float testx = Convert.ToSingle(welllocation_oil_plot.GetValue(0, i));
                            float testy = Convert.ToSingle(welllocation_oil_plot.GetValue(1, i));

                            bool init = inpolygon(nvert, vertx, verty, testx, testy);//true在里面
                            switch (list_blank[0].type)
                            {
                                case 0:
                                    {
                                        if (!init)
                                        {
                                            welllocation_oil_plot[2, i] = float.NaN;
                                        }
                                        break;
                                    };
                                case 1:
                                    {
                                        if (init)
                                        {
                                            welllocation_oil_plot[2, i] = float.NaN;
                                        }
                                        break;
                                    };
                            }
                        }

                        for (int i = 0; i < welllocation_water_plot.Size[1]; i++)
                        {
                            // ILArray<float> interpValues4Plot = ILMath.tosingle(welllocation_plot.Concat(scatteredValues, 0));
                            float testx = Convert.ToSingle(welllocation_water_plot.GetValue(0, i));
                            float testy = Convert.ToSingle(welllocation_water_plot.GetValue(1, i));

                            bool init = inpolygon(nvert, vertx, verty, testx, testy);//true在里面
                            switch (list_blank[0].type)
                            {
                                case 0:
                                    {
                                        if (!init)
                                        {
                                            welllocation_water_plot[2, i] = float.NaN;
                                        }
                                        break;
                                    };
                                case 1:
                                    {
                                        if (init)
                                        {
                                            welllocation_water_plot[2, i] = float.NaN;
                                        }
                                        break;
                                    };
                            }

                        }
                    }



                }
            }
            #endregion 白化
            //目前只有直井的，水平井应该也同样处理
            pc.Add(new ILLinePlot(welllocation_oil_plot, Tag = "oil", lineColor: Color.Empty)
            {

                Marker =
                {
                    Style = MarkerStyle.Circle,
                    Fill = { Color = Color.OrangeRed },
                    Border =
                    {
                        Color = Color.Red,
                        Width = 2
                    },
                    Size = 5
                }
            });



            pc.Add(new ILLinePlot(welllocation_water_plot, Tag = "WATER", lineColor: Color.Empty)
            {
                Marker =
                {
                    Style = MarkerStyle.Circle,
                    Fill = { Color = Color.DarkBlue },
                    Border =
                    {
                        Color = Color.DarkBlue,
                        Width = 2
                    },

                    Size = 5,
                }
            }

            );

            # endregion 井点位置，图形样式

            #region 修改图形title
            //定义了图形的title，这个图形title没有直接提供给方法修改font之类的东西，于是定义了一个ILScreenObject
            //当然其实通过访问最底层的方式，修改的
            string UNINNAME="";
            if (op.CheckedNodes[0].ParentNode == null)
            {
             UNINNAME=op.CheckedNodes[0].GetValue("Name").ToString();
            }
            pc.Add(new ILScreenObject(width: 0, height: 0)
            {
                // position the anchor point to the center of the panel
                Location = new PointF(0.5f, 0.06f),

                Children = {
               new ILLabel(UNINNAME+"井位图")
                 {
                   Position= new Vector3(0.2,0.1,0.1),//(interpValues4Plot),
                     Color = Color.Black,
                 Fringe={Color = Color.Black,Width=0},
                 Font=new Font("微软雅黑",15, FontStyle.Bold)
                 }
            }

            }
            );

            # endregion 修改图形title
            if (model.Contains("表面图") | model.Contains("等值线"))
            {
                #region 插值方法
                // we compute interpolated values for all grid points used for plotting the true function also
                ILArray<double> interpPositions = X[":"].T.Concat(Y[":"].T, 0);
                // now interpPositions are layed out as follows: 
                // x1, x2, x3, x4, ...
                // y1, y2, y3, y4, ...
                ILArray<double> err = 1;

                ILArray<double> interpValues = Interpolation.kriging(surface_scatteredValues, welllocation_plot, interpPositions, error: err);
                // ILArray<double> interpValues2 = Interpolation.interp3s(scatteredValues, scatteredPositions, interpPositions);
                // bring the values back into shape for plotting
                interpValues.a = ILMath.reshape(interpValues, X.S);
                err.a = ILMath.reshape(err, X.S);
                # endregion 插值方法

                #region 白化

                //列名 曲线编号；X；Y；类型
                //类型 0 外边界；1内边界；2为断层

                //                double[,] blank = new[,] {{	211.509	,	231.913	},
                //{	234.489	,	221.624	},
                //{	249.066	,	190.387	},
                //{	248.723	,	152.336	},
                //{	224.886	,	125.691	},
                //{	137.252	,	61.5071	},
                //{	109.299	,	73.92	},
                //{	96.7797	,	99.9406	},
                //{	98.4947	,	130.873	},
                //{	106.383	,	174.028	},
                //{	123.704	,	202	},
                //{	144.97	,	213.509	},
                //{	175.152	,	224.133	},
                //{	195.56	,	229.591	},
                //{	211.509	,	231.913	}};

                if (dt_line != null)
                {

                    int linenum =
   (from order in dt_line.AsEnumerable()
    group order by order.Field<int>("num")).Count();
                    List<BlankNumStruct> list_num =
     (from order in dt_line.AsEnumerable()
      group order by order.Field<int>("num") into g
      select new BlankNumStruct
      {
          ID = g.Key,
          Num = g.Count()

      }).ToList<BlankNumStruct>();


                    for (int k = 0; k < linenum; k++)
                    {
                        List<BlankStruct> list_blank =
     (from order in dt_line.AsEnumerable()
      where order.Field<int>("num") == list_num[k].ID
      select new BlankStruct
      {
          X_axis = order.Field<double>("X_axis"),
          Y_axis = order.Field<double>("Y_axis"),
          type = order.Field<int>("type")
      }).ToList<BlankStruct>();
                        //int nvert = list_blank.Count;
                        //if (list_blank[0].X_axis != list_blank[nvert - 1].X_axis & list_blank[0].Y_axis != list_blank[nvert - 1].Y_axis & list_blank[0].type < 3)//如果首尾的坐标不是一样，就强制一样(断层线就算了)
                        //    nvert = nvert + 1;

                        //float[] vertx = new float[nvert];
                        //float[] verty = new float[nvert];
                        ////define random points with XYZ coordinates as a [3*10] array
                        //ILArray<float> Points = ILMath.tosingle(ILMath.rand(2, nvert));
                  
                        //// call the splinepath function
                        //ILArray<float> spl = Interpolation.splinepath(Points);

                        //if (list_blank[0].type >= 3)
                        //    nvert = nvert + 1;
                        //for (int i = 0; i < nvert - 1; i++)
                        //{
                        //    vertx[i] = Convert.ToSingle(list_blank[i].X_axis);
                        //    verty[i] = Convert.ToSingle(list_blank[i].Y_axis);
                        //}
                        //if (list_blank[0].X_axis != list_blank[nvert - 2].X_axis & list_blank[0].Y_axis != list_blank[nvert - 2].Y_axis & list_blank[0].type < 3)
                        //{
                        //    vertx[nvert - 1] = Convert.ToSingle(list_blank[0].X_axis);
                        //    verty[nvert - 1] = Convert.ToSingle(list_blank[0].Y_axis);
                        //    nvert = nvert - 1;
                        //}

                        int nvert = list_blank.Count;
                        if (list_blank[0].X_axis != list_blank[nvert - 1].X_axis & list_blank[0].Y_axis != list_blank[nvert - 1].Y_axis & list_blank[0].type < 3)//如果首尾的坐标不是一样，就强制一样(断层线就算了)
                            nvert = nvert + 1;

                      
                        //define random points with XYZ coordinates as a [3*10] array
                        ILArray<float> Points = ILMath.tosingle(ILMath.rand(2, nvert));

                        // call the splinepath function
                       

                        if (list_blank[0].type >= 3)
                            nvert = nvert + 1;
                        for (int i = 0; i < nvert - 1; i++)
                        {
                            Points[0,i] = Convert.ToSingle(list_blank[i].X_axis);
                            Points[1,i] = Convert.ToSingle(list_blank[i].Y_axis);
                        }
                        if (list_blank[0].X_axis != list_blank[nvert - 2].X_axis & list_blank[0].Y_axis != list_blank[nvert - 2].Y_axis & list_blank[0].type < 3)
                        {
                            Points[0,nvert - 1] = Convert.ToSingle(list_blank[0].X_axis);
                            Points[1,nvert - 1] = Convert.ToSingle(list_blank[0].Y_axis);
                            nvert = nvert - 1;
                        }
                        ILArray<float> spl = Interpolation.splinepath(Points);
                        nvert = spl.S[1];
                        float[] vertx = new float[spl.S[1]];
                        float[] verty = new float[spl.S[1]];
                        for (int i = 0; i < spl.S[1]; i++)
                        {
                            vertx[i] = spl.GetValue(0,i);
                            verty[i] = spl.GetValue(1, i);
                        }
                     
                        if (list_blank[0].type < 2)
                        {
                            for (int i = 0; i < X.Size[0]; i++)
                                for (int j = 0; j < X.Size[1]; j++)
                                {
                                    float testx = Convert.ToSingle(X.GetValue(i, j));
                                    float testy = Convert.ToSingle(Y.GetValue(i, j));

                                    bool init = inpolygon(nvert, vertx, verty, testx, testy);//true在里面
                                    switch (list_blank[0].type)
                                    {
                                        case 0:
                                            {
                                                if (!init)
                                                {
                                                    interpValues[i, j] = double.NaN;
                                                }
                                                break;
                                            };
                                        case 1:
                                            {
                                                if (init)
                                                {
                                                    interpValues[i, j] = double.NaN;
                                                }
                                                break;
                                            };
                                    }
                                }
                        }

                        else
                        {
                            ILArray<float> fracture = ILMath.tosingle(ILMath.zeros(3, vertx.Length));
                            fracture["0;:"] = vertx;
                            fracture["1;:"] = verty;
                            fracture["2;:"] = Convert.ToSingle(interpValues.Max());
                            pc.Add(new ILLinePlot(ILMath.tosingle(fracture), Tag = "fracture", lineColor: Color.Black, lineWidth: 4));

                        }

                    }
                }
                #endregion 白化
                if (model.Contains("表面图"))
                {
                    # region 定义了surface
                    var surf1 = pc.Add(new ILSurface(interpValues, X, Y, colormap: Colormaps.ILNumerics) //Colormaps.Jet
                    {

                        Wireframe = { Visible = false, Width = 2, Color = null }, // 'Color' controls the solid color. 'null' activates individual color mapped colors. 网格线
                        Children = {
                    new ILColorbar()
                {
                     Location = new PointF(0.99f, 0.3f),
                },
                }
                    });
                    # endregion 定义了surface
                }
                if (model.Contains("等值线"))
                {
                    #region 定义了等值线图
                    var contour = pc.Add(new ILContourPlot(ILMath.tosingle(interpValues), colormap: Colormaps.Gray, tag: "contour"));
                    {
                        //因为等值线这个是没有提供X,Y,Z来画图的方法，因此需要对X,Y进行变换，变换主要改变的是transform这个矩阵的值，这个矩阵在整个图形过程中都是会被记住的
                        var size2 = new Vector3(Math.Abs(X.Max() - X.Min()) / Math.Abs(interpValues.S[0]), Math.Abs(Y.Max() - Y.Min()) / Math.Abs(interpValues.S[1]), interpValues.Max());
                        var size = new Vector3(X.Min(), Y.Min(), interpValues.Max());
                        //这两个方法就是提供来改变这个矩阵，当然可以直接操作矩阵，这两行代码不能顺便改顺序
                        contour.Scale(size2);
                        contour.Translate(size);
                    }
                    //这两个方法是用来寻找需要访问的对象，对本代码无意义，对学习有意义
                    var lp = pc.Find<ILContourPlot>("contour");
                    var lp2 = pc.First<ILGroup>("LabelsGroup");
                    //改变等值线的label的字体，没有直接提供方法，需要去访问最底层的方法才能改变
                    foreach (var lp3 in pc.Find<ILGroup>("LabelsGroup"))
                    {
                        lp3.First<ILLabel>("0").Fringe.Width = 0;
                        lp3.First<ILLabel>("0").Fringe.Color = Color.Black;
                        lp3.First<ILLabel>("0").Font = new Font("微软雅黑", 9, FontStyle.Bold);
                    }
                    #endregion end等值线图
                }
            }
            # region 定义了画图的区域之类的大问题
            //    ILArray<float> A3 = ILMath.tosingle(ILSpecialData.terrain);
            // ILArray<float> interpValues4count = ILMath.tosingle(interpValues);
            ratio = (double)ilPanel1.Size.Width / ilPanel1.Size.Height;
            //ratio = (double)pc.ScreenRect.Size.Width / pc.ScreenRect.Size.Height;
            ratio = ratio * Math.Abs(Y.Max() - Y.Min()) / Math.Abs(X.Max() - X.Min());
            //double ffg = X.Max();
            //double ffg2 = Y.Max();
            //double fg2 = ffg2 / ffg;

            pc.DataScreenRect = new RectangleF(0.05f, 0.1f, 0.88f, 0.82f);
            ratio3 = pc.DataScreenRect.Width / pc.DataScreenRect.Height;
            ratio = ratio * ratio3;
            ratio3 = ratio3 * Math.Abs(Y.Max() - Y.Min()) / Math.Abs(X.Max() - X.Min());
            //给显示图形的区域外面套一层边框，要不然 这个会比较难看，这个也是这个控件的bug
            ILArray<double> lineborderX = 1;
            lineborderX[0] = X.Min(); ;
            lineborderX[1] = X.Min();
            lineborderX[2] = X.Max();
            lineborderX[3] = X.Max();
            lineborderX[4] = X.Min(); ;
            ILArray<double> lineborderY = 1;
            lineborderY[0] = Y.Min();
            lineborderY[1] = Y.Max();
            lineborderY[2] = Y.Max();
            lineborderY[3] = Y.Min();
            lineborderY[4] = Y.Min();
            pc.Add(new ILLinePlot(lineborderX, lineborderY, lineColor: Color.FromArgb(166, 166, 166)));


            float min = 0;
            float max = 0;
            interpValues4Plot[2, ILMath.full].GetLimits(out min, out max);
            if (Math.Abs(max) < Math.Abs(min))
            {
                max = Math.Abs(min);
            }
            else
            { max = Math.Abs(max); }

            #endregion 定义了画图的区域之类的大问题

            #region 饼转图的比例尺

            float scale = 10;//这个定义的饼状图的饼的最大尺寸，这个和x，Y，
            max = max / scale;

            #endregion 饼状图的比例尺
            string[] wellname = dt_wellloction.AsEnumerable().Select(d => d.Field<string>("wellname")).ToArray();
            if (model.Contains("泡泡图"))
            {
                #region 画饼状图
                //这个饼状图是没有坐标的，也是需要定义transform来实现在井位上投影的，吐血。。




                for (int i = 0; i < oilwellnum; i++) //采油井的饼状图
                {
                    var gr = pc.Add(new ILGroup(target: RenderTarget.Screen2DFar, tag: "pieshape")
                    {
                        Children = {
                                     new ILCircle()
                   {
                       Fill={ Color = Color.FromArgb(225,255,127,80)},
                      Border={ Color = Color.FromArgb(255,255,127,80)},
   
                   }             
                   }

                    }

              );
                    var size2 = new Vector3(interpValues4Plot.GetValue(0, i) / (Math.Abs(interpValues4Plot.GetValue(2, i)) / max), (1 / ratio) * interpValues4Plot.GetValue(1, i) / (Math.Abs(interpValues4Plot.GetValue(2, i)) / max), -1);
                    gr.Translate(size2); // circles are placed in 3D! 
                    var size = new Vector3(Math.Abs(interpValues4Plot.GetValue(2, i)) / max, ratio * Math.Abs(interpValues4Plot.GetValue(2, i)) / max, 1);
                    gr.Scale(size);

                }

                for (int i = oilwellnum; i < wellname.LongLength; i++) //注水井的饼状图
                {
                    var gr = pc.Add(new ILGroup(target: RenderTarget.Screen2DFar, tag: "pieshape")
                    {
                        Children = {
                                     new ILCircle()
                   {

                      // Fill={Color = Color.FromArgb(225,255,127,80)},
                      //Border={ Color = Color.FromArgb(255,255,127,80)},
                     Fill={Color = Color.FromArgb( 225,0,191,209)},
                    Border={ Color = Color.FromArgb(225,0,191,209)},
                   }             
                   }

                    }

              );
                    var size2 = new Vector3(interpValues4Plot.GetValue(0, i) / (Math.Abs(interpValues4Plot.GetValue(2, i)) / max), (1 / ratio) * interpValues4Plot.GetValue(1, i) / (Math.Abs(interpValues4Plot.GetValue(2, i)) / max), -1);
                    gr.Translate(size2); // circles are placed in 3D! 
                    var size = new Vector3(Math.Abs(interpValues4Plot.GetValue(2, i)) / max, ratio * Math.Abs(interpValues4Plot.GetValue(2, i)) / max, 1);
                    gr.Scale(size);

                }
                #endregion 画饼状图
            }
            #region 饼状图的标注，井位名称
            //，请主要这个标注的z坐标的值要大于饼状图，否则会被盖住，这个也是需要transform的
            for (int i = 0; i < wellname.Length; i++)
            {
                var gr = pc.Add(new ILGroup(target: RenderTarget.Screen2DFar, tag: "pie_text")
                {
                    Children = {
 
//井位
                 new ILLabel(wellname[i])
                 {Position= new Vector3(0,0,0),//(interpValues4Plot),
                   Anchor = new PointF(0.5f, 1.1f),
                     Color = Color.Black,
                 Fringe={Color = Color.Black,Width=0},
                 Font=new Font("微软雅黑",9)
                 }, 
                   }
                });
                var size2 = new Vector3(interpValues4Plot.GetValue(0, i) / (Math.Abs(interpValues4Plot.GetValue(2, i)) / max), (1 / ratio) * interpValues4Plot.GetValue(1, i) / (Math.Abs(interpValues4Plot.GetValue(2, i)) / max), 1);
                gr.Translate(size2); // circles are placed in 3D! 
                var size = new Vector3(Math.Abs(interpValues4Plot.GetValue(2, i)) / max, ratio * Math.Abs(interpValues4Plot.GetValue(2, i)) / max, 1);
                gr.Scale(size);
            }
            #endregion 饼状图的标注，井位名称

            #region 图例
            //定义了图形的title，这个图形title没有直接提供给方法修改font之类的东西，于是定义了一个ILScreenObject
            //当然其实通过访问最底层的方式，修改的
            pc.Add(new ILScreenObject(width: 100, height: 100)
            {
                // position the anchor point to the center of the panel
                Location = new PointF(0.115f, 0.81f),

                Children = {
               new ILLabel("图例")
                 {
                   Position= new Vector3(0.5,0.1,0.1),//(interpValues4Plot),
                     Color = Color.Black,
                 Fringe={Color = Color.Black,Width=0},
                 Font=new Font("微软雅黑",9)
                 },
            new ILCircle()
                 {
               Fill={ Color = Color.DarkBlue},
                  Border={ Color = Color.DarkBlue},
                  Transform = Matrix4.Translation(.2f,.4f,1) * 
                        Matrix4.ScaleTransform(0.05f,0.05f,1)

                 },
               new ILLabel("注水井")
                 {
                      Position= new Vector3(0.6,0.4,0.1),
              Color = Color.Black,
                 Fringe={Color = Color.Black,Width=0},
                 Font=new Font("微软雅黑",9)
                 },
              new ILCircle()
                 {
               Fill={ Color = Color.Red},
              Border={ Color = Color.Red},
                   Transform = Matrix4.Translation(.2f,.8f,1) * 
                        Matrix4.ScaleTransform(0.05f,0.05f,1)
                 },
                   new ILLabel("采油井")
                 {
              Position= new Vector3(0.6,0.8,0.1),
              Color = Color.Black,
              Fringe={Color = Color.Black,Width=0},
              Font=new Font("微软雅黑",9)
                 },
            }
            }
            );
            #endregion 图例
            pc.DataScreenRect = new RectangleF(0.05f, 0.1f, 0.88f, 0.82f);

            ilPanel1.Scene = scene;
            // don't forget to prepare the scene for rendering + trigger a redraw!


            ilPanel1.BeginRenderFrame += ilPanel1_BeginRenderFrame;
            ilPanel1.Configure();
            ilPanel1.Refresh();


        }

        private void btn_draw_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (dt_line == null)
                dt_line = new DataTable();
            draw(dt_line);

        }
        bool inpolygon(int nvert, float[] vertx, float[] verty, float testx, float testy)
        {
            int i, j = 0;
            bool c = false;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((verty[i] >= testy) != (verty[j] >= testy)) &&
                 (testx <= (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
                    c = !c;
            }
            return c;
        }
        private void tabPane1_Click(object sender, EventArgs e)
        {

        }

        private void vGridControl2_Click(object sender, EventArgs e)
        {

        }

        private void btn_blank_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //列名 曲线编号；X；Y；类型
            //类型 0 外边界；1内边界；2为断层

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导入边界文件";
            fileDialog.Filter = "边界文件|*.sn";
            DialogResult dialogResult = fileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    //获得文件路径
                    string localFilePath = fileDialog.FileName.ToString();

                    int linenum = File.ReadAllLines(localFilePath).Length;
                    dt_line = new DataTable("blank");
                    dt_line.Columns.Add("num", Type.GetType("System.Int32"));
                    dt_line.Columns.Add("X_axis", Type.GetType("System.Double"));
                    dt_line.Columns.Add("Y_axis", Type.GetType("System.Double"));
                    dt_line.Columns.Add("type", Type.GetType("System.Int32"));
                    int i = 0;
                    foreach (string line in File.ReadAllLines(localFilePath))
                    {
                        if (i > 0)
                        {
                            string[] parts = line.Split();
                            DataRow dr = dt_line.NewRow();
                            dr["num"] = int.Parse(parts[0]);
                            dr["X_axis"] = double.Parse(parts[1]);
                            dr["Y_axis"] = double.Parse(parts[2]);
                            dr["type"] = int.Parse(parts[3]);
                            dt_line.Rows.Add(dr);
                            dt_line.AcceptChanges();
                        }
                        i++;
                    }

                    draw(dt_line);

                }

            }
        }
    }
}
