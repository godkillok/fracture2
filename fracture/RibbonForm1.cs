using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraNavBar;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views.NativeMdi;
using DevExpress.XtraSplashScreen;
using System.IO;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.XtraSpreadsheet;
using DevExpress.Spreadsheet;
using Global;
namespace fracture
{
    public partial class RibbonForm1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        string filepath = "";
        bool openflag = false;
        Globalname alg = new Globalname();
        public RibbonForm1()
        {
            InitializeComponent();
            
            this.WindowState = FormWindowState.Maximized;
            UserInfo ui = new UserInfo();
            int n = 1;
            frmLogin myLogin = new frmLogin(ref ui, n);
            if (myLogin.ShowDialog() == DialogResult.OK)
            {
                openflag = true;
            }
        }


        void AddDocumentManager()
        {
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("提示");
            splashScreenManager1.SetWaitFormDescription("数据正在加载中...");
            this.documentManager1.MdiParent = this;
            this.IsMdiContainer = true;

        }
        void AddChildForm(Form childForm, string formname)
        {
            childForm.Text = formname;
            childForm.MdiParent = this;
            childForm.Show();
            splashScreenManager1.CloseWaitForm();

        }


        private void ribbon_Merge(object sender, DevExpress.XtraBars.Ribbon.RibbonMergeEventArgs e)
        {
            if (Ribbon.MergedCategories.Count > 0)
                ribbon.SelectedPage = ribbon.MergedCategories[0].Pages[0];

        }
        private void backstageViewButtonItem1_ItemClick(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\a.chm");
        }

        private void backstageViewButtonItem2_ItemClick(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e)
        {
            this.Close();
        }


        private void RibbonForm1_Load(object sender, EventArgs e)
        {

        }


        private void btnprepare_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form childForm = new ToAccess();
            string formname = btndatamanagement.Caption.ToString();
            AddDocumentManager();
            AddChildForm(childForm, formname);

        }
        private void barButtonPredict_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form childForm = new declinepred();
            string formname = btn_Predict.Caption.ToString();
            AddDocumentManager();
            AddChildForm(childForm, formname);
        }
        private void btnweight_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        
        }

        private void btnwellevl_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
         
            Form childForm = new wellmap();
            string formname = btnwellmap.Caption.ToString();
            AddDocumentManager();
            AddChildForm(childForm, formname);
        }

        private void btnreservoir_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form childForm = new PlugAgent();
            string formname = btnplugagent.Caption.ToString();
            AddDocumentManager();
            AddChildForm(childForm, formname);

        }

        private void btnnewproject_ItemClick(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            alg.newproject();
        }

        private void saveproj_ItemClick(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }



        private void btnopenproj_ItemClick(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //algorithm alg = new algorithm();
            alg.openproject();
       
        }

        private void btnabout_ItemClick(object sender, ItemClickEventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();

        }

        private void btnexit_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnnewcase_ItemClick(object sender, ItemClickEventArgs e)
        {
         
        }

        private void btnopencase_ItemClick(object sender, ItemClickEventArgs e)
        {


        }

    

        private void btnground_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form childForm = new fractureparameter();
            string formname = btnfracturepara.Caption.ToString();
            AddDocumentManager();
            AddChildForm(childForm, formname);
        }

        private void btnfront_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form childForm = new decline();
            string formname = btn_evaluate.Caption.ToString();
            AddDocumentManager();
            AddChildForm(childForm, formname);
        }

        private void btncase_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                System.Diagnostics.Process.Start(@Application.StartupPath.ToString() + "\\case");
            }
            catch
            {
                MessageBox.Show("请以管理员身份运行，否则无法打开案例");
            }
        }

        private void btnaccout_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmaccount myLogin = new frmaccount();
            if (myLogin.ShowDialog() == DialogResult.OK)
            {
                openflag = true;
            }
        }

        private void btnloggin_ItemClick(object sender, ItemClickEventArgs e)
        {
            UserInfo ui = new UserInfo();
            int n = 0;
            frmLogin myLogin = new frmLogin(ref ui, n);
            if (myLogin.ShowDialog() == DialogResult.OK)
            {
                openflag = true;
            }
        }

        private void RibbonForm1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check whether form is closed with dialog result
            if (this.DialogResult != DialogResult.Cancel &&
            this.DialogResult != DialogResult.OK)
                e.Cancel = true;
        }

        private void btnwellevlpred_ItemClick(object sender, ItemClickEventArgs e)
        {
            //if (openflag == false)
            //{
            //    MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
         
            //Form childForm = new frmwellevl(filepath);
            //string formname = btnwellevlpred.Caption.ToString();
            //AddDocumentManager();
            //AddChildForm(childForm, formname);
        }

        private void btnsurfacepred_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (filepath == "")
                return;

        }

        private void btnreservoirpred_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (filepath == "")
                return;
 

        }

        private void btnfrontpred_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (filepath == "")
                return;
     

        }
        private void RibbonForm1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void ribbon_Click(object sender, EventArgs e)
        {

        }

        private void backstageViewControl1_Click(object sender, EventArgs e)
        {

        }

        private void btnfrmhelp_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\help\\水平井裂缝堵水堵剂优化设计帮助文档.pdf");
            }
            catch
            {
                MessageBox.Show("请以管理员身份运行，否则无法打开帮助文档");
            }
        }

        private void btn_productioncurve_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form childForm = new productioncurve();
            string formname = btn_productioncurve.Caption.ToString();
            AddDocumentManager();
            AddChildForm(childForm, formname);
        }

        private void btn_datacheck_ItemClick(object sender, ItemClickEventArgs e)
        {
                if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

                Form childForm = new datacheck();
                string formname = btn_datacheck.Caption.ToString();
            AddDocumentManager();
            AddChildForm(childForm, formname);
 
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnWellHeadTemp_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openflag == false)
            {
                MessageBox.Show("请登陆！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            WellHeadPress childForm = new WellHeadPress();
            string formname = btnWellHeadTemp.Caption.ToString();
            AddDocumentManager();
            AddChildForm(childForm, formname);
        }


    }

}
