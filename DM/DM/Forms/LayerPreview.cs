using System;
using System.Drawing;
using System.Windows.Forms;

namespace DM.Forms
{
    public partial class LayerPreview : Form
    {
        public event EventHandler OnHide;
        public LayerPreview()
        {
            preview.IsPreview = true;
            InitializeComponent();
            OnHide += Dummy;
        }
        private void Dummy(object sender, EventArgs e) {}
        public void Clear()
        {
            preview.Clear();
        }
        Views.LayerView preview = new DM.Views.LayerView();
        public void OpenLayer(Models.PartitionDirectory p, Models.ElevationFile e)
        {
            //Clear();
            if (p == null || e == null)
                return;
            if( p.Partition == null )
            {
                Utils.MB.Warning("数据库中未找到该分区信息："+p.Name);
                return;
            }

//             Views.LayerView lv = new DM.Views.LayerView();
//             pl.Controls.SetVertex(lv);
//             lv.Dock = DockStyle.Fill;
//             lv.Padding = new Padding(0, 0, 0, 0);
//             lv.Margin = new Padding(0, 0, 0, 0);
//             lv.BackColor = Color.White;
//             lv.OpenLayer(p, e);
//             lv.Visible = true;
            preview.OpenLayer(p, e);
            preview.UpdateGraphics();

            this.Text = "预览 " + preview.ToString();
        }
        private void HideThis()
        {
            Hide();
            OnHide.Invoke(this, null);
        }
        private void LayerPreview_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Escape )
            {
                HideThis();
                e.SuppressKeyPress = true;
                return;
            }
            Main.MainWindow.ProcessKeys(this, e);
        }

        private void LayerPreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            HideThis();
        }

        private void LayerPreview_Load(object sender, EventArgs e)
        {
            pl.Controls.Add(preview);
            preview.Dock = DockStyle.Fill;
            preview.Padding = new Padding(0, 0, 0, 0);
            preview.Margin = new Padding(0, 0, 0, 0);
            preview.BackColor = Color.White;
            preview.Visible = true;
            preview.AlwaysFitScreen = true;
        }

        private void LayerPreview_Resize(object sender, EventArgs e)
        {
            preview.UpdateGraphics();
        }
    }
}
