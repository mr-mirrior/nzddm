namespace DM.Forms
{
    partial class DMLogin
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbuserName = new System.Windows.Forms.ComboBox();
            this.PassWord = new System.Windows.Forms.TextBox();
            this.RetainMe = new System.Windows.Forms.CheckBox();
            this.SavePassWord = new System.Windows.Forms.CheckBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cbServer = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.lbregister = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(427, 74);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DM.Properties.Resources.Locks;
            this.pictureBox1.Location = new System.Drawing.Point(20, 6);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(69, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(122, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "请输入用户名和密码：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(44, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "用户名：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(44, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 22);
            this.label3.TabIndex = 2;
            this.label3.Text = "密码：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbuserName
            // 
            this.cbuserName.BackColor = System.Drawing.SystemColors.Info;
            this.cbuserName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbuserName.FormattingEnabled = true;
            this.cbuserName.Location = new System.Drawing.Point(144, 99);
            this.cbuserName.Name = "cbuserName";
            this.cbuserName.Size = new System.Drawing.Size(142, 25);
            this.cbuserName.TabIndex = 3;
            this.cbuserName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UserName_KeyUp);
            this.cbuserName.TextChanged += new System.EventHandler(this.UserName_TextChanged);
            // 
            // PassWord
            // 
            this.PassWord.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PassWord.Location = new System.Drawing.Point(144, 142);
            this.PassWord.Name = "PassWord";
            this.PassWord.PasswordChar = '*';
            this.PassWord.Size = new System.Drawing.Size(143, 23);
            this.PassWord.TabIndex = 4;
            this.PassWord.TextChanged += new System.EventHandler(this.PassWord_TextChanged);
            this.PassWord.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PassWord_KeyUp);
            // 
            // RetainMe
            // 
            this.RetainMe.AutoSize = true;
            this.RetainMe.Location = new System.Drawing.Point(301, 101);
            this.RetainMe.Name = "RetainMe";
            this.RetainMe.Size = new System.Drawing.Size(63, 21);
            this.RetainMe.TabIndex = 5;
            this.RetainMe.Text = "记住我";
            this.RetainMe.UseVisualStyleBackColor = true;
            this.RetainMe.CheckedChanged += new System.EventHandler(this.RetainMe_CheckedChanged);
            // 
            // SavePassWord
            // 
            this.SavePassWord.AutoSize = true;
            this.SavePassWord.Enabled = false;
            this.SavePassWord.Location = new System.Drawing.Point(301, 144);
            this.SavePassWord.Name = "SavePassWord";
            this.SavePassWord.Size = new System.Drawing.Size(75, 21);
            this.SavePassWord.TabIndex = 6;
            this.SavePassWord.Text = "保存密码";
            this.SavePassWord.UseVisualStyleBackColor = true;
            this.SavePassWord.Visible = false;
            // 
            // btnLogin
            // 
            this.btnLogin.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnLogin.Enabled = false;
            this.btnLogin.Location = new System.Drawing.Point(135, 202);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 27);
            this.btnLogin.TabIndex = 7;
            this.btnLogin.Text = "登录(&O)";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.loginbtn_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(216, 202);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(44, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 22);
            this.label4.TabIndex = 9;
            this.label4.Text = "服务器：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label4.Visible = false;
            // 
            // cbServer
            // 
            this.cbServer.FormattingEnabled = true;
            this.cbServer.Location = new System.Drawing.Point(144, 75);
            this.cbServer.Name = "cbServer";
            this.cbServer.Size = new System.Drawing.Size(142, 25);
            this.cbServer.TabIndex = 10;
            this.cbServer.Text = "172.23.225.223";
            this.cbServer.Visible = false;
            this.cbServer.Leave += new System.EventHandler(this.cbServer_Leave);
            this.cbServer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cbServer_KeyUp);
            this.cbServer.TextChanged += new System.EventHandler(this.cbServer_TextChanged);
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(0, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(428, 2);
            this.label5.TabIndex = 11;
            // 
            // lbregister
            // 
            this.lbregister.Location = new System.Drawing.Point(47, 172);
            this.lbregister.Name = "lbregister";
            this.lbregister.Size = new System.Drawing.Size(346, 15);
            this.lbregister.TabIndex = 12;
            this.lbregister.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DMLogin
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(427, 250);
            this.Controls.Add(this.lbregister);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbServer);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.SavePassWord);
            this.Controls.Add(this.RetainMe);
            this.Controls.Add(this.PassWord);
            this.Controls.Add(this.cbuserName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DMLogin";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "用户登录";
            this.Load += new System.EventHandler(this.DMLogin_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DMLogin_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DMLogin_FormClosing);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbuserName;
        private System.Windows.Forms.TextBox PassWord;
        private System.Windows.Forms.CheckBox RetainMe;
        private System.Windows.Forms.CheckBox SavePassWord;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbServer;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbregister;
    }
}

