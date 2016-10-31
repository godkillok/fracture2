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
using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;
using Global;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;
using System.Drawing.Imaging;
namespace fracture
{
    public partial class Pred : Form
    {
        private double learningRate = 0.1;
        private double momentum = 0.0;
        private double sigmoidAlphaValue = 2.0;
        private int neuronsInFirstLayer = 20;
        private int iterations = 1000;

        private Thread workerThread = null;
        private volatile bool needToStop = false;
        private Button button5;
        private ActivationNetwork network;
        private List<double[]> sclarer;

        private DataTable sample_factor=new DataTable();
        private DataTable sample_production=new DataTable();
        private DataTable candidy_factor = new DataTable();
        private DataSet ds = new DataSet();


        public class xy
        {
            DateTime xaxis;

            public DateTime Xaxis
            {
                get { return xaxis; }
                set { xaxis = value; }
            }
            double yaxis;

            public double Yaxis
            {
                get { return yaxis; }
                set { yaxis = value; }
            }
        
        }

        [Serializable()]
        private class network_scaler
        {
            public ActivationNetwork network;
            public List<double[]> sclarer;
        }
        network_scaler net_scaler = new network_scaler();
        // Constructor

        DataTable sample_data;
        DataTable pred_data;
        public Pred()
        {
            InitializeComponent();
           
            string strDestination = Application.StartupPath + "\\case\\demoproject\\project\\sample.xml";
            if (Globalname.localFilePath != "")
                strDestination = Globalname.localFilePath + "\\project\\sample.xml";
            if (File.Exists(strDestination))
            {
                ds.ReadXml(strDestination);
                sample_factor = ds.Tables["sample_factor"];
                sample_production = ds.Tables["sample_production"];
                candidy_factor = ds.Tables["candidy_factor"];

                gridControl2.DataSource = sample_factor;
                gridControl4.DataSource = sample_production;
                gridControl3.DataSource = candidy_factor;
            }
            else
            { 
            sample_factor.TableName = "sample_factor";
            sample_factor.Columns.Add(new DataColumn("井号", typeof(string)));
            sample_factor.Columns.Add(new DataColumn("措施类型", typeof(string)));
            sample_factor.Columns.Add(new DataColumn("措施时间", typeof(DateTime)));
            gridControl2.DataSource = sample_factor;
           

     
            sample_production.TableName = "sample_production";
            sample_production.Columns.Add(new DataColumn("井号", typeof(string)));
            sample_production.Columns.Add(new DataColumn("措施类型", typeof(string)));
            sample_production.Columns.Add(new DataColumn("措施时间", typeof(DateTime)));
            sample_production.Columns.Add(new DataColumn("增油年月", typeof(DateTime)));
            sample_production.Columns.Add(new DataColumn("增油量", typeof(double)));
            gridControl4.DataSource = sample_production;


       
            candidy_factor.TableName = "candidy_factor";
            candidy_factor.Columns.Add(new DataColumn("井号", typeof(string)));
            gridControl3.DataSource = candidy_factor;

            ds.DataSetName = "数据表";
            }
            treelistview.InitTreeView(treeList1, true);
            // Handling the QueryControl event that will populate all automatically generated Documents
            string sql = "";
            treelistview.AddWellNodes(treeList1);
            SetImageIndex(imageCollection1, treeList1, null, 1, 0);

            treeList1.AfterCheckNode += treeList1_AfterCheckNode;
            treeList1.BeforeCheckNode += treeList1_BeforeCheckNode;
           
        }
     
        private void btn_setting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btn_train_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             sample_data = gridControl2.DataSource as DataTable;
             sample_production = gridControl2.DataSource as DataTable;
            SearchSolution(sample_factor);
        }
        /// <summary>
        /// Save the network
        /// </summary>
        /// <param name="Net">The network to save</param>
        private static void SaveNet(network_scaler Net, string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, Net);
            fs.Close();
        }
        /// <summary>
        /// Load a network
        /// </summary>
        /// <param name="FilePath">The path to the binary network file</param>
        /// <returns></returns>
        private static network_scaler LoadNet(string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            network_scaler net = (network_scaler)formatter.Deserialize(fs);
            fs.Close();
            return net;
        }
        // Worker thread
        void SearchSolution(DataTable factor)
        {
            // number of learning samples
            int samples = factor.Rows.Count;
            int features = factor.Columns.Count - 3;
            // data transformation factor         

            // prepare learning data
            double[][] input = new double[samples][];
            double[][] output = new double[samples][];

            for (int i = 0; i < samples; i++)
            {
                input[i] = new double[features];
                output[i] = new double[3];

                for (int j = 0; j < features; j++)
                {
                    input[i][j] = double.Parse(factor.Rows[i][j + 2].ToString());
                }

                for (int j = 0; j < 3; j++)
                {
                    output[i][j] = double.Parse(factor.Rows[i][j].ToString());
                }

            }


            //建立网络的方法在就这里，假如要建立32*13*5（输入层，隐层，输出层，）的网络则可以这样设置参数：
            ///ActivationNetwork network = new ActivationNetwork( SigmoidFunction( 2 ), 32,32,13,5)。
            // create multi-layer neural network
            network = new ActivationNetwork(
                new BipolarSigmoidFunction(sigmoidAlphaValue),
                2, 10, 1);

            // initialize input and output values


            // create teacher
            BackPropagationLearning teacher = new BackPropagationLearning(network);
            // set learning rate and momentum
            // 设置BP算法的学习率与冲量系数
            teacher.LearningRate = learningRate;
            teacher.Momentum = momentum;

            // iterations
            int iteration = 1;

            // solution array
            double[,] solution = new double[50, 3];
            double[] networkInput = new double[2];
            double[,] solutiondata = new double[50, 2];
            // calculate X values to be used with solution function

            double error1 = 0;
            double error2 = 1;
            // loop
            while (!needToStop)
            {
                // run epoch of learning procedure
                error1 = teacher.RunEpoch(input, output) / samples;

                //// calculate error
                double learningError = 0.0;
                //for (int j = 0, k = samples; j < k; j++)
                //{
                //    System.Diagnostics.Debug.WriteLine(network.Compute(input[j])[0].ToString());
                //    learningError += Math.Abs(data[j, 2] - (network.Compute(input[j])[0]));
                //}

                // increase current iteration
                iteration++;
                if (((iterations != 0) && (iteration > iterations)) || error1 < 0.00001)
                    break;

            }
        }

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

        private void btn_test_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pred(sample_factor);
        }

        private void btn_sampleout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "文本文件|*.pdi";


            net_scaler.network = network;
            int length = sample_factor.Columns.Count - 3;
            List<double[]> scl = new List<double[]>();
            for (int i = 0; i < length; i++)
            {
                double[] scale = new double[2];
                scale[0] = i;
                scale[1] = i + 1;
                scl.Add(scale);
            }
            net_scaler.sclarer = scl;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveNet(net_scaler, sfd.FileName);
            }

        }


        private void btn_samplein_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog file1 = new OpenFileDialog();
            file1.Filter = "文本文件|*.pdi";
            if (file1.ShowDialog() == DialogResult.OK)
            {
                net_scaler = LoadNet(file1.FileName);
                network = net_scaler.network;
                sclarer = net_scaler.sclarer;
            }
        }

        private double[][] pred(DataTable sample_factor)
        {
            // number of learning samples
            int samples = sample_factor.Rows.Count;
            int features = sample_factor.Columns.Count - 3;
            // data transformation factor         

            // prepare learning data
            double[][] input = new double[samples][];
            double[][] result = new double[samples][];

            for (int i = 0; i < samples; i++)
            {
                input[i] = new double[features];
                result[i] = new double[3];
                for (int j = 0; j < features; j++)
                {
                    input[i][j] = double.Parse(sample_factor.Rows[i][j + 2].ToString());
                }
            }

            double learningError = 0;
            for (int j = 0, k = samples; j < k; j++)
            {

                for (int m = 0; m < 3; m++)

                    result[j][m] = network.Compute(input[j])[m];
            }
            return result;
        }
        private void btn_pred_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             pred_data = gridControl3.DataSource as DataTable;
           
            pred(candidy_factor);

        }

        private void gridView4_DoubleClick(object sender, System.EventArgs e)
        {


        }
        private void gridView3_DoubleClick(object sender, System.EventArgs e)
        {
            
        }
        private void btn_pic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

                // myChart.ExportToImage(localFilePath, format);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_pics_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }


        private void toolbar_export_Click(object sender, EventArgs e)
        {
            string localFilePath = FileOperate.saveopenfile();
            if (localFilePath != null)
            {
                //FileOperate.ExportToExcel(gridView1, localFilePath);
            }
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
        private void toolbar_copy_Click(object sender, EventArgs e)
        {
            //gridView1.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            //gridView1.CopyToClipboard();
        }

        private void toolbar_delete_Click(object sender, EventArgs e)
        {
            //gridView1.DeleteSelectedRows();
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

        // Initial plot setup, modify this as needed
        void AddRow(string data)
        {
            //if (data == string.Empty) return;
            //gridView1.AddNewRow();
            //string[] rowData = data.Split(new char[] { '\r', '\x09' });
            //int rowHandle = gridView1.GetRowHandle(gridView1.DataRowCount);

            //for (int i = 0; i < rowData.Length; i++)
            //{
            //    if (i >= gridView1.Columns.Count) break;
            //    if (gridView1.IsNewItemRow(rowHandle))
            //    {
            //        this.gridView1.SetRowCellValue(rowHandle, gridView1.Columns[i], rowData[i]);
            //    }
            //}
            //DataTable dt = gridControl1.DataSource as DataTable;
            //DataTable dtf = dt;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OleDbHelper olg = new OleDbHelper();
            ds.Tables.Add(sample_factor);
            ds.Tables.Add(sample_production);
            ds.Tables.Add(candidy_factor);


            string localFilePath1 = "";
            string localFilePath2 = "";
            string localFilePath3 = "";
            if (Globalname.localFilePath == "")
            {
                localFilePath1 = Application.StartupPath + "\\case\\demoproject\\project\\sample.xml";
                //localFilePath2 = Application.StartupPath + "\\test\\project\\sample_production.xml";
                //localFilePath3 = Application.StartupPath + "\\test\\project\\candidy_factor.xml";
            }
            else
            {
                localFilePath1 = Globalname.localFilePath + "\\project\\sample.xml";
                //localFilePath2 = Globalname.localFilePath + "\\project\\sample_production.xml";
                //localFilePath3 = Globalname.localFilePath + "\\project\\candidy_factor.xml";

            }
            string strPath = Path.GetDirectoryName(localFilePath1);
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            //olg.ConvertDataTableToXML(sample_factor, localFilePath1);
            //olg.ConvertDataTableToXML(sample_production, localFilePath2);
            //olg.ConvertDataTableToXML(candidy_factor, localFilePath3);
            ds.WriteXml(localFilePath1);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        // Assigning a required content for each auto generated Document

    }
}
