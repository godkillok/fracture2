namespace fracture
{
    partial class frmLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtUserName = new DevExpress.XtraEditors.TextEdit();
            this.txtPwd = new DevExpress.XtraEditors.TextEdit();
            this.btnloggin = new DevExpress.XtraEditors.SimpleButton();
            this.btncancel = new DevExpress.XtraEditors.SimpleButton();
            this.usecheck = new DevExpress.XtraEditors.CheckEdit();
            this.pwdcheck = new DevExpress.XtraEditors.CheckEdit();
            this.autocheck = new DevExpress.XtraEditors.CheckEdit();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPwd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usecheck.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pwdcheck.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.autocheck.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(89, 37);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(36, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "用户名";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(89, 83);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(24, 14);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "密码";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(89, 57);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(237, 20);
            this.txtUserName.TabIndex = 2;
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(89, 103);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Size = new System.Drawing.Size(237, 20);
            this.txtPwd.TabIndex = 2;
            // 
            // btnloggin
            // 
            this.btnloggin.Location = new System.Drawing.Point(89, 188);
            this.btnloggin.Name = "btnloggin";
            this.btnloggin.Size = new System.Drawing.Size(88, 34);
            this.btnloggin.TabIndex = 3;
            this.btnloggin.Text = "登录";
            this.btnloggin.Click += new System.EventHandler(this.btnloggin_Click);
            // 
            // btncancel
            // 
            this.btncancel.Location = new System.Drawing.Point(238, 188);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(88, 34);
            this.btncancel.TabIndex = 3;
            this.btncancel.Text = "取消";
            this.btncancel.Click += new System.EventHandler(this.btncancel_Click);
            // 
            // usecheck
            // 
            this.usecheck.Location = new System.Drawing.Point(89, 144);
            this.usecheck.Name = "usecheck";
            this.usecheck.Properties.Caption = "记住账户";
            this.usecheck.Size = new System.Drawing.Size(93, 19);
            this.usecheck.TabIndex = 4;
            // 
            // pwdcheck
            // 
            this.pwdcheck.Location = new System.Drawing.Point(170, 144);
            this.pwdcheck.Name = "pwdcheck";
            this.pwdcheck.Properties.Caption = "保存密码";
            this.pwdcheck.Size = new System.Drawing.Size(93, 19);
            this.pwdcheck.TabIndex = 4;
            // 
            // autocheck
            // 
            this.autocheck.Location = new System.Drawing.Point(251, 144);
            this.autocheck.Name = "autocheck";
            this.autocheck.Properties.Caption = "自动登录";
            this.autocheck.Size = new System.Drawing.Size(93, 19);
            this.autocheck.TabIndex = 4;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 250);
            this.Controls.Add(this.autocheck);
            this.Controls.Add(this.pwdcheck);
            this.Controls.Add(this.usecheck);
            this.Controls.Add(this.btncancel);
            this.Controls.Add(this.btnloggin);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Name = "frmLogin";
            this.Text = "用户登录";
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPwd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usecheck.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pwdcheck.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.autocheck.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtUserName;
        private DevExpress.XtraEditors.TextEdit txtPwd;
        private DevExpress.XtraEditors.SimpleButton btnloggin;
        private DevExpress.XtraEditors.SimpleButton btncancel;
        private DevExpress.XtraEditors.CheckEdit usecheck;
        private DevExpress.XtraEditors.CheckEdit pwdcheck;
        private DevExpress.XtraEditors.CheckEdit autocheck;
        private System.Windows.Forms.Timer timer1;
    }
}