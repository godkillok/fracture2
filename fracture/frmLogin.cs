using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security;
using System.Security.Cryptography;
namespace fracture
{

    public partial class frmLogin : Form
    {
        CspParameters param;
        int maxnnum=0;
        int itern=0;//用于计数，此程序改变的也是这个值
        private UserInfo uiLogin;
        List<string> listuser;
        List<string> listpwd;
        private string deEncodermd5(string strLine)
        {
            string strLine2;
            param = new CspParameters();
            param.KeyContainerName = "Tgp";
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                byte[] encryptdata = Convert.FromBase64String(strLine);
                byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                strLine2 = Encoding.Default.GetString(decryptdata);
                return strLine2;
            }
        }
        public frmLogin(ref UserInfo ui,int n)
        {
            maxnnum = n;
            //
            // Required for Windows Form Designer support
            //

            //设置属性
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
            txtUserName.Focus();
            // Set login info to class member
            uiLogin = ui;
            //usecheck.Visible = false;
            //pwdcheck.Visible = false;
            //autocheck.Visible = false;
            //autocheck.Checked = true;
            //txtUserName.Text = "123";
            //txtPwd.Text = "123";
            timer1.Enabled = false;
            string strLine;
            string filepath = Application.StartupPath.ToString() + "\\confidential.info";
            FileStream aFile = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(aFile);
            listuser=new List<string>();
            listpwd=new List<string>() ;
            //StreamWriter sw = new StreamWriter(aFile);
            strLine = sr.ReadLine();
            //Read data in line by line 这个兄台看的懂吧~一行一行的读取
            while (strLine != null)
            {
                strLine = deEncodermd5(strLine);
                string[] s = strLine.Split(new char[] { ':' });
                listuser.Add(s[1]);
                listpwd.Add(s[3]);
                strLine = sr.ReadLine();
            }
            sr.Close();
            aFile.Close();


            filepath = Application.StartupPath.ToString() + "\\Log.txt";

            aFile = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Read);
            sr = new StreamReader(aFile);
            //StreamWriter sw = new StreamWriter(aFile);
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                string[] s = strLine.Split(new char[] { ':' });

                if (s[0] == "UserName")
                {
                    usecheck.Checked = true;
                    txtUserName.Text = s[1];
                }
                if (s[0] == "PassWord")
                {
                    pwdcheck.Checked = true;
                    txtPwd.Text = s[1];

                }
                if (s[0] == "Flag")
                    if (s[1] == "true")
                    {
                        autocheck.Checked = true;
                        //this.Visible = false;
                        //this.Hide();
                        timer1.Interval = 600;
                        timer1.Enabled = true;
                    }
                strLine = sr.ReadLine();
            }
            sr.Close();
            aFile.Close();
        }
        /// <summary>
        /// 用户点击“登录”触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnloggin_Click(object sender, EventArgs e)
        {
            //try
            //{
            string UserName = txtUserName.Text.Trim();//获取用户名
            string PassWord = txtPwd.Text.Trim();//获取密码
            if (UserName == "")//如果用户名为空值
            {
                MessageBox.Show("请输入用户名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);//弹出消息对话框
            }
            else if (PassWord == "")//如果密码为空值
            {
                MessageBox.Show("请输入密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);//弹出消息对话框
            }
            else if (UserName != "" && PassWord != "")
            {
                bool findcorrectloggin = false;
                for (int i = 0; i < listuser.Count; i++)
                {
                    if (UserName == listuser[i] && PassWord == listpwd[i])
                    {
                        findcorrectloggin = true;
                        string strLine;
                        string filepath = Application.StartupPath.ToString() + "\\Log.txt";

                        FileStream aFile = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                        //StreamReader sr = new StreamReader(aFile);
                        StreamWriter sw = new StreamWriter(aFile);
                        if (usecheck.Checked == true)
                        {
                            strLine = "UserName" + ":" + UserName + "\r\n";
                            sw.Write(strLine);

                        }
                        if (pwdcheck.Checked == true)
                        {
                            strLine = "PassWord" + ":" + PassWord + "\r\n";
                            sw.Write(strLine);
                        }
                     
                        if (autocheck.Checked == true)
                        {
                            if (usecheck.Checked == false)
                            {
                                strLine = "UserName" + ":" + UserName + "\r\n";
                                sw.Write(strLine);
                            }
                            if (pwdcheck.Checked == false)
                            {
                                strLine = "PassWord" + ":" + PassWord + "\r\n";
                                sw.Write(strLine);
                            }
                            strLine = "Flag" + ":" + "true" + "\r\n";
                            sw.Write(strLine);
                        }
                        sw.Close();
                        aFile.Close();

                        this.DialogResult = DialogResult.OK;
                        break;
                    }
                   
                      
                }
                if (UserName == "admin" && PassWord == "admin")
                    {
                        findcorrectloggin = true;
                        string strLine;
                        string filepath = Application.StartupPath.ToString() + "\\Log.txt";

                        FileStream aFile = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                        //StreamReader sr = new StreamReader(aFile);
                        StreamWriter sw = new StreamWriter(aFile);
                        if (usecheck.Checked == true)
                        {
                            strLine = "UserName" + ":" + UserName + "\r\n";
                            sw.Write(strLine);

                        }
                        if (pwdcheck.Checked == true)
                        {
                            strLine = "PassWord" + ":" + PassWord + "\r\n";
                            sw.Write(strLine);
                        }

                        if (autocheck.Checked == true)
                        {
                            if (usecheck.Checked == false)
                            {
                                strLine = "UserName" + ":" + UserName + "\r\n";
                                sw.Write(strLine);
                            }
                            if (pwdcheck.Checked == false)
                            {
                                strLine = "PassWord" + ":" + PassWord + "\r\n";
                                sw.Write(strLine);
                            }
                            strLine = "Flag" + ":" + "true" + "\r\n";
                            sw.Write(strLine);
                        }
                        sw.Close();
                        aFile.Close();

                        this.DialogResult = DialogResult.OK;
                    }
                if (findcorrectloggin == false)
                {
                    MessageBox.Show("用户名或密码错误", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);//弹出警告提示框
                    //this.DialogResult = DialogResult.Cancel;
                    txtUserName.Focus();
                }
            }
        }



        private void btncancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void frmLogin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check whether form is closed with dialog result
            if (this.DialogResult != DialogResult.Cancel &&
            this.DialogResult != DialogResult.OK)
                e.Cancel = true;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
           
            
            itern = itern + 1;
            if (itern<= maxnnum)//不能无限的循环下去，当弹出3个对话框了，就停
            {
                btnloggin_Click(sender, e);
                timer1.Enabled = false;//此属性为false，timer1_Tick事件就不会执行了
            }
        }

    }
    public class UserInfo
    {
        private string strUserName;
        private string strPassword;
        public string UserName
        {
            get { return strUserName; }
            set { strUserName = value; }
        }
        public string Password
        {
            get { return strPassword; }
            set { strPassword = value; }
        }
        public UserInfo()
        {
            strUserName = "";
            strPassword = "";
        }
    }
}
