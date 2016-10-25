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
    public partial class frmaccount : Form
    {
        CspParameters param;
        public frmaccount()
        {
            InitializeComponent();
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            string strLine;
            string filepath = Application.StartupPath.ToString() + "\\confidential.info";
            FileStream aFile;
           
            if (File.Exists(filepath))
            {

                aFile = new FileStream(filepath, FileMode.Append, FileAccess.Write);
            }
            else
            {
                aFile = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
            }
           
            
            //StreamReader sr = new StreamReader(aFile);
            StreamWriter sw = new StreamWriter(aFile);
            strLine = "username:" + txtuser.Text.ToString()+ ":";
            strLine =strLine+ "password:" + txtpwd.Text.ToString();
            strLine = Encodermd5(strLine);
            strLine = strLine + "\r\n";
            sw.Write(strLine);
            sw.Close();
            aFile.Close();
        }
        private string Encodermd5(string strLine)
        {
            string strLine2;
             param = new CspParameters();
            param.KeyContainerName = "Tgp";//密匙容器的名称，保持加密解密一致才能解密成功
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                byte[] plaindata = Encoding.Default.GetBytes(strLine);//将要加密的字符串转换为字节数组
                byte[] encryptdata = rsa.Encrypt(plaindata, false);//将加密后的字节数据转换为新的加密字节数组
                strLine2 = Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
            }
            return strLine2;
        }
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
        private void btnsavepwd_Click(object sender, EventArgs e)
        {

        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            txtuser.Text = "";
            txtpwd.Text = "";
            txtuser.Focus();
           

        }

      

   


    }
}
