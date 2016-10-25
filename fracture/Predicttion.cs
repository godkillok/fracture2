using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Global;
namespace fracture
{
    public partial class Predicttion : Form
    {
        DataTable dt_TableAndField;
        public Predicttion()
        {
            InitializeComponent();
            treeList1.DoubleClick += treeList1_DoubleClick;
            treelistview.InitTreeView(treeList1);
            // Handling the QueryControl event that will populate all automatically generated Documents
            treelistview.AddWellNodes(treeList1);
            SetImageIndex(imageCollection1, treeList1, null, 1, 0);
            //treeList1.CustomDrawNodeCell += treeList1_CustomDrawNodeCell;
        }

        private void treeList1_DoubleClick(object sender, EventArgs e)
        {

            TreeListNode clickedNode = this.treeList1.FocusedNode;
            if (clickedNode.ParentNode != null)
            {
                object item = treeList1.FocusedNode;
                string wellid = clickedNode.GetValue("WELLID").ToString();
                DataTable dt;
                //dt = GetProductData(wellid);
                //if (dt != null)
                //{
                //    //setgridcontrol(dt);
                //    //drawcandy(dt);
                //}
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

        private void btn_train_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btn_loadsample_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt = null;
           
            dt_TableAndField = new DataTable();
            string sheetName = "物理表汇总";
            string tablename = "T_WELL_SAMPLE";//采油井生产月报
            
            //string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
            //string excelpath = Application.StartupPath + "\\ACCESS.xlsx";   
            string TableAndField = string.Format("select 列显示名称 AS name ,库字段名称 as ID, 默认单位名称 as UNIT from [{0}$] where (库表名称='" + tablename + "')", sheetName);
            dt_TableAndField = OleDbHelper.ExcelToDataTable(sheetName, TableAndField);
            try
            {
                string sSql = "select ";
                for (int i = 0; i < dt_TableAndField.Rows.Count - 1; i++)
                {
                    sSql = sSql + string.Format("{0} AS {1}, ", dt_TableAndField.Rows[i][1], dt_TableAndField.Rows[i][0]);
                }
                sSql = sSql + string.Format("{0} AS {1} From {2}", dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][1], dt_TableAndField.Rows[dt_TableAndField.Rows.Count - 1][0], tablename);
                dt = OleDbHelper.getTable(sSql,  Globalname.DabaBasePath);

            }
            catch
            {

            }
        }


        private void btn_outpic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btnpredict_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}
