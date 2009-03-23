namespace DM.Forms
{
    partial class ToolsWindow
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ckAllPart = new System.Windows.Forms.CheckBox();
            this.btnInputCoord = new DM.Utils.VistaButton();
            this.vistaButton1 = new DM.Utils.VistaButton();
            this.btnZuoan = new DM.Utils.VistaButton();
            this.btnYouan = new DM.Utils.VistaButton();
            this.btnDeckPoly = new DM.Utils.VistaButton();
            this.btnDeckRect = new DM.Utils.VistaButton();
            this.btnFitscreen = new DM.Utils.VistaButton();
            this.btnShangyou = new DM.Utils.VistaButton();
            this.btnRestore = new DM.Utils.VistaButton();
            this.btnRollCount = new DM.Utils.VistaButton();
            this.btn7 = new DM.Utils.VistaButton();
            this.btnXiayou = new DM.Utils.VistaButton();
            this.btn3 = new DM.Utils.VistaButton();
            this.btn1 = new DM.Utils.VistaButton();
            this.btn11 = new DM.Utils.VistaButton();
            this.label2 = new System.Windows.Forms.Label();
            this.cbElevations = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbPartitions = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.ckPreview = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cbMode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // ckAllPart
            // 
            this.ckAllPart.AutoSize = true;
            this.ckAllPart.ForeColor = System.Drawing.Color.Black;
            this.ckAllPart.Location = new System.Drawing.Point(39, 471);
            this.ckAllPart.Name = "ckAllPart";
            this.ckAllPart.Size = new System.Drawing.Size(51, 21);
            this.ckAllPart.TabIndex = 25;
            this.ckAllPart.Text = "所有";
            this.toolTip1.SetToolTip(this.ckAllPart, "打开该高程上的所有层");
            this.ckAllPart.UseVisualStyleBackColor = true;
            this.ckAllPart.Visible = false;
            this.ckAllPart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPartitions_MouseMove);
            // 
            // btnInputCoord
            // 
            this.btnInputCoord.BackColor = System.Drawing.Color.White;
            this.btnInputCoord.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnInputCoord.ButtonText = "输入坐标";
            this.btnInputCoord.CornerRadius = 4;
            this.btnInputCoord.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInputCoord.GlowColor = System.Drawing.Color.White;
            this.btnInputCoord.HighlightColor = System.Drawing.Color.Silver;
            this.btnInputCoord.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnInputCoord.Location = new System.Drawing.Point(7, 244);
            this.btnInputCoord.Margin = new System.Windows.Forms.Padding(0);
            this.btnInputCoord.Name = "btnInputCoord";
            this.btnInputCoord.Size = new System.Drawing.Size(36, 36);
            this.btnInputCoord.TabIndex = 36;
            this.toolTip1.SetToolTip(this.btnInputCoord, "分仓");
            this.btnInputCoord.Click += new System.EventHandler(this.vistaButton2_Click);
            // 
            // vistaButton1
            // 
            this.vistaButton1.BackColor = System.Drawing.Color.White;
            this.vistaButton1.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.vistaButton1.ButtonText = "报警查询";
            this.vistaButton1.CornerRadius = 4;
            this.vistaButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.vistaButton1.GlowColor = System.Drawing.Color.Lavender;
            this.vistaButton1.HighlightColor = System.Drawing.Color.Silver;
            this.vistaButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.vistaButton1.Location = new System.Drawing.Point(6, 352);
            this.vistaButton1.Name = "vistaButton1";
            this.vistaButton1.Size = new System.Drawing.Size(77, 19);
            this.vistaButton1.TabIndex = 30;
            this.toolTip1.SetToolTip(this.vistaButton1, "从右岸往对面看");
            this.vistaButton1.Click += new System.EventHandler(this.vistaButton1_Click);
            // 
            // btnZuoan
            // 
            this.btnZuoan.BackColor = System.Drawing.Color.White;
            this.btnZuoan.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnZuoan.ButtonText = "左岸";
            this.btnZuoan.CornerRadius = 4;
            this.btnZuoan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnZuoan.GlowColor = System.Drawing.Color.White;
            this.btnZuoan.HighlightColor = System.Drawing.Color.Silver;
            this.btnZuoan.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnZuoan.Location = new System.Drawing.Point(6, 194);
            this.btnZuoan.Name = "btnZuoan";
            this.btnZuoan.Size = new System.Drawing.Size(36, 20);
            this.btnZuoan.TabIndex = 23;
            this.toolTip1.SetToolTip(this.btnZuoan, "从左岸往对面看");
            this.btnZuoan.Click += new System.EventHandler(this.btnZuoan_Click);
            // 
            // btnYouan
            // 
            this.btnYouan.BackColor = System.Drawing.Color.White;
            this.btnYouan.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnYouan.ButtonText = "右岸";
            this.btnYouan.CornerRadius = 4;
            this.btnYouan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnYouan.GlowColor = System.Drawing.Color.Lavender;
            this.btnYouan.HighlightColor = System.Drawing.Color.Silver;
            this.btnYouan.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnYouan.Location = new System.Drawing.Point(44, 194);
            this.btnYouan.Name = "btnYouan";
            this.btnYouan.Size = new System.Drawing.Size(36, 20);
            this.btnYouan.TabIndex = 22;
            this.toolTip1.SetToolTip(this.btnYouan, "从右岸往对面看");
            this.btnYouan.Click += new System.EventHandler(this.btnYouan_Click);
            // 
            // btnDeckPoly
            // 
            this.btnDeckPoly.BackColor = System.Drawing.Color.White;
            this.btnDeckPoly.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnDeckPoly.ButtonText = "任意";
            this.btnDeckPoly.CornerRadius = 4;
            this.btnDeckPoly.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeckPoly.GlowColor = System.Drawing.Color.White;
            this.btnDeckPoly.HighlightColor = System.Drawing.Color.Silver;
            this.btnDeckPoly.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnDeckPoly.Location = new System.Drawing.Point(45, 244);
            this.btnDeckPoly.Name = "btnDeckPoly";
            this.btnDeckPoly.Size = new System.Drawing.Size(36, 36);
            this.btnDeckPoly.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btnDeckPoly, "多边形分仓");
            this.btnDeckPoly.Click += new System.EventHandler(this.btnDeckPoly_Click);
            // 
            // btnDeckRect
            // 
            this.btnDeckRect.BackColor = System.Drawing.Color.White;
            this.btnDeckRect.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnDeckRect.ButtonText = "矩形";
            this.btnDeckRect.CornerRadius = 4;
            this.btnDeckRect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeckRect.GlowColor = System.Drawing.Color.White;
            this.btnDeckRect.HighlightColor = System.Drawing.Color.Silver;
            this.btnDeckRect.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnDeckRect.Location = new System.Drawing.Point(48, 303);
            this.btnDeckRect.Margin = new System.Windows.Forms.Padding(0);
            this.btnDeckRect.Name = "btnDeckRect";
            this.btnDeckRect.Size = new System.Drawing.Size(36, 36);
            this.btnDeckRect.TabIndex = 12;
            this.toolTip1.SetToolTip(this.btnDeckRect, "分仓");
            this.btnDeckRect.Visible = false;
            this.btnDeckRect.Click += new System.EventHandler(this.btn13_Click);
            // 
            // btnFitscreen
            // 
            this.btnFitscreen.BackColor = System.Drawing.Color.White;
            this.btnFitscreen.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnFitscreen.ButtonText = "屏幕";
            this.btnFitscreen.CornerRadius = 4;
            this.btnFitscreen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFitscreen.GlowColor = System.Drawing.Color.White;
            this.btnFitscreen.HighlightColor = System.Drawing.Color.Silver;
            this.btnFitscreen.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnFitscreen.Location = new System.Drawing.Point(45, 78);
            this.btnFitscreen.Name = "btnFitscreen";
            this.btnFitscreen.Size = new System.Drawing.Size(36, 36);
            this.btnFitscreen.TabIndex = 11;
            this.toolTip1.SetToolTip(this.btnFitscreen, "缩放到适合屏幕");
            this.btnFitscreen.Click += new System.EventHandler(this.btnFitscreen_Click);
            // 
            // btnShangyou
            // 
            this.btnShangyou.BackColor = System.Drawing.Color.White;
            this.btnShangyou.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnShangyou.ButtonText = "上游";
            this.btnShangyou.CornerRadius = 4;
            this.btnShangyou.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShangyou.GlowColor = System.Drawing.Color.White;
            this.btnShangyou.HighlightColor = System.Drawing.Color.Silver;
            this.btnShangyou.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnShangyou.Location = new System.Drawing.Point(7, 173);
            this.btnShangyou.Name = "btnShangyou";
            this.btnShangyou.Size = new System.Drawing.Size(36, 20);
            this.btnShangyou.TabIndex = 9;
            this.toolTip1.SetToolTip(this.btnShangyou, "从上游往下游方向看");
            this.btnShangyou.Click += new System.EventHandler(this.btnShangyou_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.BackColor = System.Drawing.Color.White;
            this.btnRestore.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnRestore.ButtonText = "0°";
            this.btnRestore.CornerRadius = 4;
            this.btnRestore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRestore.GlowColor = System.Drawing.Color.White;
            this.btnRestore.HighlightColor = System.Drawing.Color.Silver;
            this.btnRestore.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRestore.Location = new System.Drawing.Point(45, 136);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(36, 36);
            this.btnRestore.TabIndex = 8;
            this.toolTip1.SetToolTip(this.btnRestore, "无旋转");
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnRollCount
            // 
            this.btnRollCount.BackColor = System.Drawing.Color.White;
            this.btnRollCount.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnRollCount.ButtonText = "遍数";
            this.btnRollCount.CornerRadius = 4;
            this.btnRollCount.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRollCount.GlowColor = System.Drawing.Color.White;
            this.btnRollCount.HighlightColor = System.Drawing.Color.Silver;
            this.btnRollCount.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRollCount.Location = new System.Drawing.Point(7, 303);
            this.btnRollCount.Name = "btnRollCount";
            this.btnRollCount.Size = new System.Drawing.Size(36, 36);
            this.btnRollCount.TabIndex = 7;
            this.toolTip1.SetToolTip(this.btnRollCount, "观察碾压次数");
            this.btnRollCount.Click += new System.EventHandler(this.btnRollCount_Click);
            // 
            // btn7
            // 
            this.btn7.BackColor = System.Drawing.Color.White;
            this.btn7.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btn7.ButtonText = "旋转";
            this.btn7.CornerRadius = 4;
            this.btn7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn7.GlowColor = System.Drawing.Color.White;
            this.btn7.HighlightColor = System.Drawing.Color.Silver;
            this.btn7.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btn7.Location = new System.Drawing.Point(7, 136);
            this.btn7.Name = "btn7";
            this.btn7.Size = new System.Drawing.Size(36, 36);
            this.btn7.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btn7, "鼠标左键顺时针30度旋转，右键逆时针30度旋转");
            this.btn7.Click += new System.EventHandler(this.btn7_Click);
            // 
            // btnXiayou
            // 
            this.btnXiayou.BackColor = System.Drawing.Color.White;
            this.btnXiayou.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btnXiayou.ButtonText = "下游";
            this.btnXiayou.CornerRadius = 4;
            this.btnXiayou.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXiayou.GlowColor = System.Drawing.Color.Lavender;
            this.btnXiayou.HighlightColor = System.Drawing.Color.Silver;
            this.btnXiayou.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnXiayou.Location = new System.Drawing.Point(45, 173);
            this.btnXiayou.Name = "btnXiayou";
            this.btnXiayou.Size = new System.Drawing.Size(36, 20);
            this.btnXiayou.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnXiayou, "从下游往上游方向看");
            this.btnXiayou.Click += new System.EventHandler(this.btnXiayou_Click);
            // 
            // btn3
            // 
            this.btn3.BackColor = System.Drawing.Color.White;
            this.btn3.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btn3.ButtonText = "拖动";
            this.btn3.CornerRadius = 4;
            this.btn3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn3.GlowColor = System.Drawing.Color.Lavender;
            this.btn3.HighlightColor = System.Drawing.Color.Silver;
            this.btn3.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btn3.Location = new System.Drawing.Point(7, 25);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(36, 36);
            this.btn3.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btn3, "滚动");
            this.btn3.Click += new System.EventHandler(this.btn3_Click);
            // 
            // btn1
            // 
            this.btn1.BackColor = System.Drawing.Color.White;
            this.btn1.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btn1.ButtonText = "平移";
            this.btn1.Checked = true;
            this.btn1.CornerRadius = 4;
            this.btn1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn1.GlowColor = System.Drawing.Color.Lavender;
            this.btn1.HighlightColor = System.Drawing.Color.Silver;
            this.btn1.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btn1.Location = new System.Drawing.Point(45, 25);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(36, 36);
            this.btn1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btn1, "平移");
            this.btn1.Visible = false;
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // btn11
            // 
            this.btn11.BackColor = System.Drawing.Color.White;
            this.btn11.ButtonColor = System.Drawing.Color.MidnightBlue;
            this.btn11.ButtonText = "缩放";
            this.btn11.CornerRadius = 4;
            this.btn11.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn11.GlowColor = System.Drawing.Color.White;
            this.btn11.HighlightColor = System.Drawing.Color.Silver;
            this.btn11.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btn11.Location = new System.Drawing.Point(7, 78);
            this.btn11.Name = "btn11";
            this.btn11.Size = new System.Drawing.Size(36, 36);
            this.btn11.TabIndex = 10;
            this.toolTip1.SetToolTip(this.btn11, "左键放大/右键缩小");
            this.btn11.Click += new System.EventHandler(this.btn11_Click);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(7, 220);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 2);
            this.label2.TabIndex = 17;
            // 
            // cbElevations
            // 
            this.cbElevations.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbElevations.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cbElevations.FormattingEnabled = true;
            this.cbElevations.Location = new System.Drawing.Point(4, 492);
            this.cbElevations.Name = "cbElevations";
            this.cbElevations.Size = new System.Drawing.Size(77, 25);
            this.cbElevations.Sorted = true;
            this.cbElevations.TabIndex = 20;
            this.cbElevations.SelectedIndexChanged += new System.EventHandler(this.cbElevations_SelectedIndexChanged);
            this.cbElevations.Enter += new System.EventHandler(this.cbPartitions_Enter);
            this.cbElevations.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPartitions_MouseMove);
            this.cbElevations.TextUpdate += new System.EventHandler(this.cbElevations_TextUpdate);
            this.cbElevations.Click += new System.EventHandler(this.cbPartitions_Click);
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(1, 472);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 21);
            this.label3.TabIndex = 19;
            this.label3.Text = "高程：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPartitions_MouseMove);
            // 
            // cbPartitions
            // 
            this.cbPartitions.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbPartitions.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cbPartitions.FormattingEnabled = true;
            this.cbPartitions.Location = new System.Drawing.Point(4, 445);
            this.cbPartitions.Name = "cbPartitions";
            this.cbPartitions.Size = new System.Drawing.Size(77, 25);
            this.cbPartitions.TabIndex = 18;
            this.cbPartitions.SelectedIndexChanged += new System.EventHandler(this.cbPartitions_SelectedIndexChanged);
            this.cbPartitions.Enter += new System.EventHandler(this.cbPartitions_Enter);
            this.cbPartitions.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPartitions_MouseMove);
            this.cbPartitions.TextUpdate += new System.EventHandler(this.cbPartitions_TextUpdate);
            this.cbPartitions.Click += new System.EventHandler(this.cbPartitions_Click);
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(1, 421);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 21);
            this.label4.TabIndex = 16;
            this.label4.Text = "分区：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPartitions_MouseMove);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(41, 419);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(41, 23);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "打开";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPartitions_MouseMove);
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            this.btnOpen.Enter += new System.EventHandler(this.cbPartitions_Enter);
            // 
            // ckPreview
            // 
            this.ckPreview.AutoSize = true;
            this.ckPreview.ForeColor = System.Drawing.Color.Black;
            this.ckPreview.Location = new System.Drawing.Point(4, 522);
            this.ckPreview.Name = "ckPreview";
            this.ckPreview.Size = new System.Drawing.Size(51, 21);
            this.ckPreview.TabIndex = 21;
            this.ckPreview.Text = "预览";
            this.ckPreview.UseVisualStyleBackColor = true;
            this.ckPreview.Visible = false;
            this.ckPreview.CheckedChanged += new System.EventHandler(this.ckPreview_CheckedChanged);
            this.ckPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPartitions_MouseMove);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 226);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 24;
            this.label1.Text = "分仓";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 17);
            this.label5.TabIndex = 26;
            this.label5.Text = "视角";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 17);
            this.label6.TabIndex = 27;
            this.label6.Text = "移动";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 17);
            this.label7.TabIndex = 28;
            this.label7.Text = "缩放";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 283);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 17);
            this.label8.TabIndex = 29;
            this.label8.Text = "碾压情况";
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Location = new System.Drawing.Point(6, 345);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 2);
            this.label9.TabIndex = 31;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Location = new System.Drawing.Point(6, 379);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(78, 2);
            this.label10.TabIndex = 34;
            // 
            // cbMode
            // 
            this.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMode.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbMode.FormattingEnabled = true;
            this.cbMode.Items.AddRange(new object[] {
            "设计模式",
            "查询模式"});
            this.cbMode.Location = new System.Drawing.Point(6, 386);
            this.cbMode.Name = "cbMode";
            this.cbMode.Size = new System.Drawing.Size(75, 25);
            this.cbMode.TabIndex = 35;
            this.cbMode.SelectedIndexChanged += new System.EventHandler(this.cbMode_SelectedIndexChanged);
            // 
            // ToolsWindow
            // 
            this.AcceptButton = this.btnOpen;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Lavender;
            this.ClientSize = new System.Drawing.Size(88, 524);
            this.ControlBox = false;
            this.Controls.Add(this.btnInputCoord);
            this.Controls.Add(this.cbMode);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.vistaButton1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnZuoan);
            this.Controls.Add(this.btnYouan);
            this.Controls.Add(this.ckPreview);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.cbElevations);
            this.Controls.Add(this.cbPartitions);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnDeckPoly);
            this.Controls.Add(this.btnDeckRect);
            this.Controls.Add(this.btnFitscreen);
            this.Controls.Add(this.btnShangyou);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnRollCount);
            this.Controls.Add(this.btn7);
            this.Controls.Add(this.btnXiayou);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.btn11);
            this.Controls.Add(this.ckAllPart);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(94, 550);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(94, 550);
            this.Name = "ToolsWindow";
            this.Opacity = 0.75;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = " ";
            this.Load += new System.EventHandler(this.ToolsWindow_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolsWindow_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Utils.VistaButton btn11;
        private Utils.VistaButton btn1;
        private System.Windows.Forms.ToolTip toolTip1;
        private DM.Utils.VistaButton btn3;
        private DM.Utils.VistaButton btnXiayou;
        private DM.Utils.VistaButton btnRollCount;
        private DM.Utils.VistaButton btn7;
        private DM.Utils.VistaButton btnShangyou;
        private DM.Utils.VistaButton btnRestore;
        private DM.Utils.VistaButton btnFitscreen;
        private DM.Utils.VistaButton btnDeckPoly;
        private DM.Utils.VistaButton btnDeckRect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbElevations;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbPartitions;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.CheckBox ckPreview;
        private DM.Utils.VistaButton btnZuoan;
        private DM.Utils.VistaButton btnYouan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ckAllPart;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private DM.Utils.VistaButton vistaButton1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cbMode;
        private DM.Utils.VistaButton btnInputCoord;

    }
}