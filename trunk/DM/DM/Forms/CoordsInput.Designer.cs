namespace DM.Forms
{
    partial class CoordsInput
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbMark = new System.Windows.Forms.TextBox();
            this.lbDeckName = new System.Windows.Forms.Label();
            this.dada = new System.Windows.Forms.Label();
            this.lbPastion = new System.Windows.Forms.Label();
            this.lbPastiondsa = new System.Windows.Forms.Label();
            this.lbBlockname = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tbY = new System.Windows.Forms.TextBox();
            this.tbCoords = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnAddCoord = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(35, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "坐标点(&C)：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbX
            // 
            this.tbX.Location = new System.Drawing.Point(125, 79);
            this.tbX.Name = "tbX";
            this.tbX.Size = new System.Drawing.Size(119, 23);
            this.tbX.TabIndex = 7;
            this.toolTip1.SetToolTip(this.tbX, "施工坐标，输入单个X坐标");
            this.tbX.Visible = false;
            this.tbX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCoord_KeyPress);
            this.tbX.Enter += new System.EventHandler(this.tbX_Enter);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(57, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "备注(&R)：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbMark
            // 
            this.tbMark.AcceptsReturn = true;
            this.tbMark.Location = new System.Drawing.Point(125, 138);
            this.tbMark.Multiline = true;
            this.tbMark.Name = "tbMark";
            this.tbMark.Size = new System.Drawing.Size(407, 58);
            this.tbMark.TabIndex = 14;
            this.tbMark.Text = "测试";
            // 
            // lbDeckName
            // 
            this.lbDeckName.AutoEllipsis = true;
            this.lbDeckName.AutoSize = true;
            this.lbDeckName.Location = new System.Drawing.Point(388, 15);
            this.lbDeckName.Name = "lbDeckName";
            this.lbDeckName.Size = new System.Drawing.Size(83, 17);
            this.lbDeckName.TabIndex = 5;
            this.lbDeckName.Text = "lbDeckName";
            // 
            // dada
            // 
            this.dada.AutoSize = true;
            this.dada.Location = new System.Drawing.Point(347, 15);
            this.dada.Name = "dada";
            this.dada.Size = new System.Drawing.Size(44, 17);
            this.dada.TabIndex = 4;
            this.dada.Text = "仓面：";
            // 
            // lbPastion
            // 
            this.lbPastion.AutoEllipsis = true;
            this.lbPastion.AutoSize = true;
            this.lbPastion.Location = new System.Drawing.Point(277, 15);
            this.lbPastion.Name = "lbPastion";
            this.lbPastion.Size = new System.Drawing.Size(61, 17);
            this.lbPastion.TabIndex = 3;
            this.lbPastion.Text = "lbPastion";
            // 
            // lbPastiondsa
            // 
            this.lbPastiondsa.AutoSize = true;
            this.lbPastiondsa.Location = new System.Drawing.Point(240, 15);
            this.lbPastiondsa.Name = "lbPastiondsa";
            this.lbPastiondsa.Size = new System.Drawing.Size(44, 17);
            this.lbPastiondsa.TabIndex = 2;
            this.lbPastiondsa.Text = "高程：";
            // 
            // lbBlockname
            // 
            this.lbBlockname.AutoEllipsis = true;
            this.lbBlockname.Location = new System.Drawing.Point(122, 15);
            this.lbBlockname.Name = "lbBlockname";
            this.lbBlockname.Size = new System.Drawing.Size(112, 17);
            this.lbBlockname.TabIndex = 1;
            this.lbBlockname.Text = "lbBlockID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(78, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "分区：";
            // 
            // tbY
            // 
            this.tbY.Location = new System.Drawing.Point(289, 66);
            this.tbY.Name = "tbY";
            this.tbY.Size = new System.Drawing.Size(119, 23);
            this.tbY.TabIndex = 9;
            this.toolTip1.SetToolTip(this.tbY, "施工坐标，输入单个Y坐标");
            this.tbY.Visible = false;
            this.tbY.Enter += new System.EventHandler(this.tbY_Enter);
            // 
            // tbCoords
            // 
            this.tbCoords.AcceptsReturn = true;
            this.tbCoords.Location = new System.Drawing.Point(125, 47);
            this.tbCoords.Multiline = true;
            this.tbCoords.Name = "tbCoords";
            this.tbCoords.Size = new System.Drawing.Size(407, 85);
            this.tbCoords.TabIndex = 12;
            this.tbCoords.Text = "312.95, -259.26; 315.03, -257.96; 321.55, -254.21; 308.03, -256.99;";
            this.toolTip1.SetToolTip(this.tbCoords, "施工坐标，坐标以x1,y1;x2,y2;...方式输入");
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(317, 282);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 27);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Location = new System.Drawing.Point(202, 282);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(91, 27);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "确认(&O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(159, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "&X：";
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(266, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "&Y：";
            this.label5.Visible = false;
            // 
            // btnAddCoord
            // 
            this.btnAddCoord.Location = new System.Drawing.Point(447, 65);
            this.btnAddCoord.Name = "btnAddCoord";
            this.btnAddCoord.Size = new System.Drawing.Size(75, 25);
            this.btnAddCoord.TabIndex = 10;
            this.btnAddCoord.Text = "添加坐标";
            this.btnAddCoord.UseVisualStyleBackColor = true;
            this.btnAddCoord.Visible = false;
            this.btnAddCoord.Click += new System.EventHandler(this.btnAddCoord_Click);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(22, 199);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(533, 79);
            this.label6.TabIndex = 17;
            this.label6.Text = "排除碾压区域边界输入法：形如x11,y11;x12,y12;x13,y13|x21,y21;x22,y22;x23,y23;x24,y24 ，即逗号隔离某一点的坝" +
                "横坐标与坝纵坐标，分号隔离两个控制点，竖杠隔离两个排除碾压区域（坐标点请按照顺时针或逆时针依次输入）。排除碾压区域备注输入法：形如 监测仪器埋设|修路 ，竖杠隔" +
                "离两个排除碾压区域备注，标点请使用英文半角输入法！";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CoordsInput
            // 
            this.AcceptButton = this.btnAddCoord;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(577, 318);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbCoords);
            this.Controls.Add(this.btnAddCoord);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbY);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lbDeckName);
            this.Controls.Add(this.dada);
            this.Controls.Add(this.lbPastion);
            this.Controls.Add(this.lbPastiondsa);
            this.Controls.Add(this.lbBlockname);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbMark);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbX);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoordsInput";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "输入坐标";
            this.Load += new System.EventHandler(this.NotRolling_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbMark;
        private System.Windows.Forms.Label lbDeckName;
        private System.Windows.Forms.Label dada;
        private System.Windows.Forms.Label lbPastion;
        private System.Windows.Forms.Label lbPastiondsa;
        private System.Windows.Forms.Label lbBlockname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbY;
        private System.Windows.Forms.Button btnAddCoord;
        private System.Windows.Forms.TextBox tbCoords;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.Button btnOK;
    }
}