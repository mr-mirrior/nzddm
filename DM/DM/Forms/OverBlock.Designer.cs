namespace DM.Forms
{
    partial class OverBlock
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
            this.cbElevation = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbBlock = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbTobedone = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbDecks = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbElevation
            // 
            this.cbElevation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbElevation.FormattingEnabled = true;
            this.cbElevation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbElevation.Location = new System.Drawing.Point(106, 104);
            this.cbElevation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbElevation.Name = "cbElevation";
            this.cbElevation.Size = new System.Drawing.Size(169, 25);
            this.cbElevation.TabIndex = 22;
            this.cbElevation.SelectedIndexChanged += new System.EventHandler(this.cbPartition_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 28);
            this.label1.TabIndex = 23;
            this.label1.Text = "分区：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbBlock
            // 
            this.lbBlock.Location = new System.Drawing.Point(106, 84);
            this.lbBlock.Name = "lbBlock";
            this.lbBlock.Size = new System.Drawing.Size(169, 17);
            this.lbBlock.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(15, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 24);
            this.label2.TabIndex = 25;
            this.label2.Text = "高程：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Location = new System.Drawing.Point(63, 222);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(91, 27);
            this.btnOK.TabIndex = 26;
            this.btnOK.Text = "确认(&O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(182, 222);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 27);
            this.btnCancel.TabIndex = 27;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(15, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(310, 27);
            this.label3.TabIndex = 28;
            this.label3.Text = "请选择合适的仓面作为厚度监控依据：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(15, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 27);
            this.label4.TabIndex = 29;
            this.label4.Text = "待施工仓面：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(4, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(328, 2);
            this.label5.TabIndex = 31;
            // 
            // lbTobedone
            // 
            this.lbTobedone.AutoEllipsis = true;
            this.lbTobedone.Location = new System.Drawing.Point(106, 9);
            this.lbTobedone.Name = "lbTobedone";
            this.lbTobedone.Size = new System.Drawing.Size(219, 17);
            this.lbTobedone.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(15, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 24);
            this.label6.TabIndex = 34;
            this.label6.Text = "仓面：";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbDecks
            // 
            this.cbDecks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDecks.FormattingEnabled = true;
            this.cbDecks.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbDecks.Location = new System.Drawing.Point(106, 132);
            this.cbDecks.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbDecks.Name = "cbDecks";
            this.cbDecks.Size = new System.Drawing.Size(169, 25);
            this.cbDecks.TabIndex = 33;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(276, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 28);
            this.label7.TabIndex = 35;
            this.label7.Text = "米";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.ForeColor = System.Drawing.Color.Blue;
            this.label8.Location = new System.Drawing.Point(106, 172);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(169, 17);
            this.label8.TabIndex = 37;
            this.label8.Text = "565";
            this.label8.Visible = false;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(12, 166);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 28);
            this.label9.TabIndex = 36;
            this.label9.Text = "平均高程：";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label9.Visible = false;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(276, 166);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 28);
            this.label10.TabIndex = 38;
            this.label10.Text = "米";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label10.Visible = false;
            // 
            // OverBlock
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(337, 264);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbDecks);
            this.Controls.Add(this.lbTobedone);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbBlock);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbElevation);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OverBlock";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "碾压厚度监控";
            this.Load += new System.EventHandler(this.OverBlock_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbElevation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbBlock;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbTobedone;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbDecks;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;

    }
}