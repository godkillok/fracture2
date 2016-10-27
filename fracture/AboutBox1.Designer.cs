namespace fracture
{
    partial class AboutBox
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelProductName = new DevExpress.XtraEditors.LabelControl();
            this.labelVersion = new DevExpress.XtraEditors.LabelControl();
            this.textBoxDescription = new DevExpress.XtraEditors.LabelControl();
            this.labelCopyright = new DevExpress.XtraEditors.LabelControl();
            this.labelCompanyName = new DevExpress.XtraEditors.LabelControl();
            this.btncheck = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::fracture.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(115, 213);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // labelProductName
            // 
            this.labelProductName.Location = new System.Drawing.Point(149, 36);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(70, 14);
            this.labelProductName.TabIndex = 3;
            this.labelProductName.Text = "labelControl1";
            // 
            // labelVersion
            // 
            this.labelVersion.Location = new System.Drawing.Point(149, 56);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(70, 14);
            this.labelVersion.TabIndex = 3;
            this.labelVersion.Text = "labelControl1";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.textBoxDescription.LineVisible = true;
            this.textBoxDescription.Location = new System.Drawing.Point(149, 116);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(233, 70);
            this.textBoxDescription.TabIndex = 3;
            this.textBoxDescription.Text = "本计算机程序受版权法和国际条约保护，如未经授权而擅自复制、传播、修改本程序（或者其中任何部分）将受到严厉的刑事及民事制裁，并在法律允许的范围内受到最大可能的起诉。" +
    "";
            // 
            // labelCopyright
            // 
            this.labelCopyright.Location = new System.Drawing.Point(149, 76);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(70, 14);
            this.labelCopyright.TabIndex = 3;
            this.labelCopyright.Text = "labelControl1";
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.Location = new System.Drawing.Point(149, 96);
            this.labelCompanyName.Name = "labelCompanyName";
            this.labelCompanyName.Size = new System.Drawing.Size(70, 14);
            this.labelCompanyName.TabIndex = 3;
            this.labelCompanyName.Text = "labelControl1";
            // 
            // btncheck
            // 
            this.btncheck.Location = new System.Drawing.Point(282, 208);
            this.btncheck.Name = "btncheck";
            this.btncheck.Size = new System.Drawing.Size(100, 27);
            this.btncheck.TabIndex = 4;
            this.btncheck.Text = "确定";
            this.btncheck.Click += new System.EventHandler(this.btncheck_Click);
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 261);
            this.Controls.Add(this.btncheck);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.labelCompanyName);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelProductName);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(9, 8, 9, 8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AboutBox1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraEditors.LabelControl labelProductName;
        private DevExpress.XtraEditors.LabelControl labelVersion;
        private DevExpress.XtraEditors.LabelControl textBoxDescription;
        private DevExpress.XtraEditors.LabelControl labelCopyright;
        private DevExpress.XtraEditors.LabelControl labelCompanyName;
        private DevExpress.XtraEditors.SimpleButton btncheck;

    }
}
