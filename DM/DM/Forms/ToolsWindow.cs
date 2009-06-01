using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DM.Forms
{
    public partial class ToolsWindow : Form
    {
        public ToolsWindow()
        {
            me = this;
            InitializeComponent();
            Init();
        }
        #region - 变量 -
        WarningList warnForm;
        static ToolsWindow me;
        public static ToolsWindow I {get{return me;}}
        Views.LayerView currentLayer = null;
        public Views.LayerView CurrentLayer { get { return currentLayer; } set { if (value == CurrentLayer) return;  currentLayer = value; UpdateLayer(); } }
        #endregion

        #region - 初始化 -
        private void Init()
        {
            InitButtons();
            preview.OnHide += OnPreviewHide;

        }
        private Models.PartitionDirectory FindPartition(string part)
        {
            foreach (Models.PartitionDirectory pd in partitions.Directories)
            {
                if (pd.Name.Equals(part))
                    return pd;
            }
            return null;
        }
        private void OnPreviewHide(object sender, EventArgs e)
        {
            ckPreview.Checked = false;
        }
        private void InitButtons()
        {
            foreach (Control c in this.Controls)
            {
                if( c is Utils.VistaButton )
                {
                    c.Click += OnClickBtn;
                }
            }
        }
        #endregion

        #region - 业务逻辑 -
        private void UpdateLayer()
        {
            if (CurrentLayer == null)
            {
                this.Text = "无";
                return;
            }

        }
        private void OpenLayer()
        {
            if( IsInitializing() )
            {
                Utils.MB.Warning("初始化尚未完成，请稍候再试一次。");
                return;
            }
            /*Views.LayerView layer = Main.MainWindow.OpenLayer(CurrentDir, CurrentFile);*/
            Views.LayerView layer = DMControl.LayerControl.Instance.OpenLayer(CurrentDir, CurrentFile);
            if (layer != null)
                CurrentLayer = layer;
            else
                return;
            ckPreview.Checked = false;

            if (ckAllPart.Checked)
            {
                for (int i = 0; i < partitions.Directories.Count; i++)
                {
                    Models.PartitionDirectory dir = partitions.Directories[i];
                    if (dir.Name.Equals(CurrentDir))
                        continue;
                    foreach (Models.ElevationFile ef in dir.Heights)
                    {
                        if (ef.HeightF == CurrentFile.HeightF)
                        {
                            layer.OpenLayer(dir, ef);
                            break;
                        }
                    }
                }
            }
            layer.UpdateGraphics();
            layer.FullPath = CurrentFile.FullName;
            layer.FitScreenOnce();
        }
        private void ShowPreview()
        {
            if (preview.Visible)
                return;
            preview.Show(this);

            Rectangle rc = this.DesktopBounds;
            int x, y;
            x = rc.Left - preview.Width;
            y = rc.Bottom - preview.Height;
            if (x < 0 || y < 0)
            {
                preview.Hide();
                return;
            }
            preview.Location = new Point(x, y);
            ckPreview.CheckState = CheckState.Checked;
        }
        public void HidePreview()
        {
            preview.Hide();
            ckPreview.CheckState = CheckState.Unchecked;
        }
        // 146.568450927734;
        //33.431549072266
        const double SHANGYOU = -146.568450927734;
        const double XIAYOU = SHANGYOU - 180;
        const double ZUOAN = SHANGYOU - 90;
        const double YOUAN = XIAYOU - 90;
        private void RotateLayer(double angle)
        {
            if (CurrentLayer != null)
            {
                CurrentLayer.RotateDegrees = angle;
                CurrentLayer.UpdateGraphics();
            }
        }
        #endregion
        #region - 打开层操作 -
        Models.PartitionDirectories partitions = new DM.Models.PartitionDirectories();
        public Models.PartitionDirectory CurrentDir
        {
            get { return (Models.PartitionDirectory)cbPartitions.SelectedItem; }
            set { cbPartitions.SelectedItem = value; }
        }
        public Models.ElevationFile CurrentFile { get { return (Models.ElevationFile)cbElevations.SelectedItem; } set { cbElevations.SelectedItem = value; } }

        private void UpdatePartition()
        {
            if (cbPartitions.Text == null)
                return;
            if (cbPartitions.Text.Length == 0)
                return;
            string typed = cbPartitions.Text;

            foreach (Models.PartitionDirectory pd in partitions.Directories)
            {
                if (pd.Name.Equals(typed, StringComparison.OrdinalIgnoreCase))
                {
                    cbPartitions.SelectedItem = pd;
                    UpdateCombo();
                    break;
                }
            }
        }
        private int FindElevation(string elevation)
        {
            if (cbElevations.DataSource == null)
                return 0;
            List<Models.ElevationFile> lst = (List<Models.ElevationFile>)cbElevations.DataSource;
            for(int i=0; i<lst.Count; i++)
            {
                if (lst[i].Height.Equals(elevation))
                    return i;
            }
            return 0;
        }
        private void UpdateCombo()
        {
            if (cbPartitions.SelectedItem == null)
                return;
            string oldEle = cbElevations.Text;
            Models.PartitionDirectory dir = (Models.PartitionDirectory)cbPartitions.SelectedItem;
            cbElevations.DataSource = dir.Heights;
            cbElevations.DisplayMember = "Height";
            int idx = FindElevation(oldEle);
            if (idx >= 0 && idx < cbElevations.Items.Count)
                cbElevations.SelectedIndex = idx;

//             AutoCompleteStringCollection col = new AutoCompleteStringCollection();
//             foreach (Models.ElevationFile ef in dir.Heights)
//             {
//                 col.SetVertex(ef.Height);
//             }
            //cbElevations.AutoCompleteCustomSource = col;
        }
        LayerPreview preview = new LayerPreview();
        private void UpdateFile()
        {
            //tbFilename.Text = "请选择……";
            if (CurrentDir == null)
                return;
            if (cbElevations.SelectedItem == null)
            {
                string height = cbElevations.Text;
                if (height == null)
                    return;
                if (height.Length == 0)
                    return;
                foreach (Models.ElevationFile ef in CurrentDir.Heights)
                {
                    if (ef.Height.Equals(height, StringComparison.OrdinalIgnoreCase))
                    {
                        cbElevations.SelectedItem = ef;
                        break;
                    }
                }
            }
            OpenPreviewLayer();
        }
        private void OpenPreviewLayer()
        {
            if (CurrentDir != null && CurrentFile != null)
            {
                preview.Clear();
                preview.OpenLayer(CurrentDir, CurrentFile);
            }
        }
        private void FilterPartitions()
        {
            if (!IsReviewing)
                return;

            // 已经存在的仓面
            DB.SegmentDAO dao = DB.SegmentDAO.getInstance();
            List<DB.Segment> lst = dao.getAllSegments();
            if (lst == null)
                return;

            // 筛选正在工作的分区
            List<Models.PartitionDirectory> pdlst = new List<DM.Models.PartitionDirectory>();
            
            for (int i = 0; i < partitions.Directories.Count; i++ )
            {
                Models.PartitionDirectory pd = partitions.Directories[i];
                for (int j = 0; j < lst.Count; j++ )
                {
                    if (pd.Partition == null)
                        continue;
                    if (lst[j].BlockID == pd.Partition.ID)
                    {
                        pdlst.Add(pd);
                        break;
                    }
                }
            }
            partitions.Directories = pdlst;

            // 筛选正在工作的高程
            for (int i = 0; i < partitions.Directories.Count; i++)
            {
                Models.PartitionDirectory pd = partitions.Directories[i];
                List<Models.ElevationFile> ef = new List<DM.Models.ElevationFile>();
                for (int j = 0; j < lst.Count; j++)
                {
                    if (lst[j].BlockID != pd.Partition.ID)
                        continue;
                    foreach (Models.ElevationFile ef2 in pd.Heights)
                    {
                        if( ef2.Elevation.Height == lst[j].DesignZ &&
                            -1 == ef.IndexOf(ef2))
                        {
                            ef.Add(ef2);
                            break;
                        }
                    }
                }
                //ef.Sort(Models.ElevationFile.Greater);
                pd.Heights = ef;
            }
        }
        private bool SearchPartitions(bool review)
        {
            //this.Enabled = false;
            //Application.DoEvents();
            if (partitions.Search(null))
            {
                //AutoCompleteStringCollection names = new AutoCompleteStringCollection();

                FilterPartitions();
                //foreach (Models.PartitionDirectory di in partitions.Directories)
                //{
                //    names.SetVertex(di.Name);
                //}
                cbPartitions.DataSource = partitions.Directories;
                //cbPartitions.AutoCompleteCustomSource = names;
                cbPartitions.DisplayMember = "Display";
                if( cbPartitions.Items.Count != 0 )
                    cbPartitions.SelectedIndex = 0;
                UpdateCombo();
            }
            else
            {
                Utils.MB.Warning("分区、高程信息无法读取！" + "\n" + "请检查磁盘状态或者网络连接是否正常。");
            }

// #if DEBUG
            //             CurrentDir = new DM.Models.PartitionDirectory(@"C:\DAMDATA\ED");
            //             CurrentFile = new DM.Models.ElevationFile(@"C:\DAMDATA\ED\569_2.txt");
//             cbPartitions.SelectedItem = FindPartition("ED");
//             cbElevations.SelectedIndex = FindElevation("569.2");
//             cbPartitions_SelectedIndexChanged(null, null);
//             OpenLayer();
//             cbPartitions.SelectedItem = FindPartition("RU2");
//             cbElevations.SelectedIndex = FindElevation("615");
//             cbPartitions.SelectedItem = FindPartition("yingdi");
//             cbElevations.SelectedIndex = FindElevation("731");
//             cbPartitions_SelectedIndexChanged(null, null);
//             cbPartitions.SelectedItem = FindPartition("LeftTop");
//             cbElevations.SelectedIndex = FindElevation("821.5");
//             cbPartitions_SelectedIndexChanged(null, null);
//             OpenLayer();
// #endif
            return true;
        }
        private void OnLoadUpdateLayer()
        {
            Cursor = Main.MainWindow.Cursor = Cursors.WaitCursor;
            ckPreview.Checked = true;
            SearchPartitions(false);

            //this.Enabled = true;
            Cursor = Main.MainWindow.Cursor = Cursors.Default;
        }
        Timer timerUpdatePartition = new Timer();
        private void OnTickUpdatePartition(object sender, EventArgs e)
        {
            timerUpdatePartition.Stop();
            UpdatePartition();
        }
        private void cbPartitions_TextUpdate(object sender, EventArgs e)
        {
            timerUpdatePartition.Stop();
            timerUpdatePartition.Tick -= OnTickUpdatePartition;
            timerUpdatePartition.Tick += OnTickUpdatePartition;
            timerUpdatePartition.Interval = 2000;
            timerUpdatePartition.Start();
        }

        private void OnTickUpdateElevation(object sender, EventArgs e)
        {
            timerUpdateElevation.Stop();
            UpdateFile();
        }
        Timer timerUpdateElevation = new Timer();
        private void cbElevations_TextUpdate(object sender, EventArgs e)
        {
            timerUpdateElevation.Stop();
            timerUpdateElevation.Tick -= OnTickUpdateElevation;
            timerUpdateElevation.Tick += OnTickUpdateElevation;
            timerUpdateElevation.Interval = 2000;
            timerUpdateElevation.Start();
        }
        #endregion

        #region - 事件处理 -
        private void OnClickBtn(object sender, EventArgs e)
        {
            if (sender == null)
                return;
            if( sender is Utils.VistaButton )
            {
                Utils.VistaButton btn = (Utils.VistaButton)sender;
                this.Text = btn.ButtonText;
            }
        }

        private void ToolsWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Escape )
            {
                if( preview.Visible )
                {
                    HidePreview();
                    e.SuppressKeyPress = true;
                    return;
                }
            }
            Main.MainWindow.ProcessKeys(sender, e);
        }
        Waiting waitDlg = new Waiting();
        //private void OnTickOnLoad(object sender, EventArgs e)
        private delegate void OnLoadOp();
        private void OnTickOnLoad()
        {
            //timerOnLoad.Stop();
            cbMode.SelectedIndex = 0;
            OnLoadUpdateLayer();
            if( this.IsHandleCreated )
                this.BeginInvoke(new OnLoadOp(EndThread));
        }
        private void EndThread()
        {
            //OpenLayer();
            waitDlg.Hide();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;

            Operation = DM.Views.Operations.SCROLL_FREE;
        }
        Timer timerOnLoad = new Timer();
        System.Threading.Thread thrd;
        private void ToolsWindow_Load(object sender, EventArgs e)
        {
            if (DM.DMControl.LoginControl.loginResult == DM.DB.LoginResult.VIEW)                                       
            {
                btnDeckRect.Enabled = false;
                btnDeckPoly.Enabled = false;
                btnInputCoord.Enabled = false;
                btnDeckRect.ForeColor = Color.Gray;
                btnDeckPoly.ForeColor = Color.Gray;
            }
            foreach (Control c in this.Controls)
            {
                if (c is Utils.VistaButton)
                {
                    Utils.VistaButton btn = (Utils.VistaButton)c;
                    if (btn.Checked)
                        OnClickBtn(btn, null);
                }
            }
            ShowPreview();

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            waitDlg.Prompt = "正在读取数据……";
            waitDlg.Show(this);
            thrd = new System.Threading.Thread(OnTickOnLoad);
            thrd.Start();
//             timerOnLoad.Interval = 100;
//             timerOnLoad.Tick += OnTickOnLoad;
//             timerOnLoad.Start();
        }
        // 打开层

        private bool IsInitializing()
        {
            if (thrd == null) return false;
            return thrd.IsAlive;
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenLayer();
        }
        private void cbPartitions_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCombo();
        }

        private void cbElevations_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFile();
        }

        private void ckPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (ckPreview.Checked)
                ShowPreview();
            else
                HidePreview();
        }

        private void cbPartitions_Enter(object sender, EventArgs e)
        {
            if (this.Location.X == 0 || this.Location.Y == 0)
                return;
            if (!ckPreview.Checked)
                ckPreview.Checked = true;
        }
        public Views.Operations Operation 
        {
            get
            {
                if (CurrentLayer != null)
                    return CurrentLayer.Operation;
                return Views.Operations.NONE;
            }
            set
            {
                if (CurrentLayer != null)
                    CurrentLayer.Operation = value;
                UpdateLayer();
            }
        }
        private void btn1_Click(object sender, EventArgs e)
        {
            
        }
        private void btn11_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.ZOOM;
        }
        private void btn3_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.SCROLL_FREE;
        }
        private void btn7_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.ROTATE;
        }
        private void btn13_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.DECK_RECT;
        }

        private void btnShangyou_Click(object sender, EventArgs e)
        {
            RotateLayer(SHANGYOU);
        }

        private void btnXiayou_Click(object sender, EventArgs e)
        {
            RotateLayer(XIAYOU);
        }

        private void btnZuoan_Click(object sender, EventArgs e)
        {
            RotateLayer(ZUOAN);
        }

        private void btnYouan_Click(object sender, EventArgs e)
        {
            RotateLayer(YOUAN);
        }
        public void ResetRotate()
        {
            RotateLayer(0);
        }
        private void btnRestore_Click(object sender, EventArgs e)
        {
            ResetRotate();
        }
        public void FitScreen()
        {
            if (CurrentLayer != null)
            {
                CurrentLayer.FitScreenOnce();
            }

        }
        private void btnFitscreen_Click(object sender, EventArgs e)
        {
            FitScreen();
        }
        private void cbPartitions_MouseMove(object sender, MouseEventArgs e)
        {
            Control c = (Control)sender;
            if( !c.Focused )
                c.Focus();
            ShowPreview();
        }
        private void btnDeckPoly_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.DECK_POLYGON;
        }

        private void cbPartitions_Click(object sender, EventArgs e)
        {
            ShowPreview();
        }

        #endregion

        private void btnRollCount_Click(object sender, EventArgs e)
        {
            Operation = DM.Views.Operations.ROOL_COUNT;
        }

        private void vistaButton1_Click(object sender, EventArgs e)
        {
            //new WarningListForm
            if (warnForm!=null)
            {
                warnForm.Close();
            }
            
             warnForm= new WarningList();
            if (warnForm.Visible)
                return;

            Rectangle rc = this.DesktopBounds;
            int x, y;
            x = rc.Left - warnForm.Width;
            y = rc.Bottom - warnForm.Height;
            warnForm.Location = new Point(x, y);
            warnForm.Show();
        }
        public void OpenLayer(string part, double elev)
        {
            //Utils.MB.OK(part + " " + elev.ToString());
            List<DM.Models.PartitionDirectory> dirs = (List<DM.Models.PartitionDirectory>)cbPartitions.DataSource;
            foreach (DM.Models.PartitionDirectory pd in dirs)
            {
                if( pd.Name.Equals(part, StringComparison.OrdinalIgnoreCase))
                {
                    //cbPartitions.SelectedItem = pd;
                    CurrentDir = pd;
                    foreach (DM.Models.ElevationFile ef in pd.Heights)
                    {
                        if( elev == ef.Elevation.Height )
                        {
                            CurrentFile = ef;
                            OpenLayer();
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private bool IsDesigning { get { return cbMode.SelectedIndex == 0; } }
        private bool IsReviewing { get { return cbMode.SelectedIndex == 1; } }
        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsDesigning)
            {
                SearchPartitions(false);
            }
            else
                SearchPartitions(true);
        }
        public void UpdateMode()
        {
            cbMode_SelectedIndexChanged(null, null);
        }

        private void vistaButton2_Click(object sender, EventArgs e)
        {
            Forms.DeckCoordInput dlg = new DeckCoordInput();
            if (CurrentLayer== null)
                return;
            dlg.Show(this.Owner);
            dlg.tbCoords.Text = string.Empty;
        }
    }
}
