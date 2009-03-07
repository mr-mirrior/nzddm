using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace DM.Forms
{
    public partial class Landscape : Form
    {
//         [DllImport("gdi32.dll")]
//         private static extern bool StretchBlt(
//           IntPtr hdcDest,      // handle to destination DC
//           int nXOriginDest, // x-coord of destination upper-left corner
//           int nYOriginDest, // y-coord of destination upper-left corner
//           int nWidthDest,   // width of destination rectangle
//           int nHeightDest,  // height of destination rectangle
//           IntPtr hdcSrc,       // handle to source DC
//           int nXOriginSrc,  // x-coord of source upper-left corner
//           int nYOriginSrc,  // y-coord of source upper-left corner
//           int nWidthSrc,    // width of source rectangle
//           int nHeightSrc,   // height of source rectangle
//           Int32 dwRop       // raster operation code
//         );
//         private static extern bool BitBlt(
//             IntPtr hdcDest, // handle to destination DC 
//             int nXDest,     // x-coord of destination upper-left corner 
//             int nYDest,     // y-coord of destination upper-left corner 
//             int nWidth,     // width of destination rectangle 
//             int nHeight,    // height of destination rectangle 
//             IntPtr hdcSrc,  // handle to source DC 
//             int nXSrc,      // x-coordinate of source upper-left corner 
//             int nYSrc,      // y-coordinate of source upper-left corner 
//             Int32 dwRop     // raster operation code 
//         );
        Bitmap bmp;
        Views.LayerView layerview = null;

        public Views.LayerView LayerView
        {
            get { return layerview; }
            set { layerview = value; }
        }

        //public Bitmap BMP { get { return bmp; } set { bmp = value; UpdateSize(); } }
        public Landscape()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
        }
        private void CreateBMP()
        {
            if (ClientRectangle.Width == 0 || ClientRectangle.Height == 0)
                return;
            if( bmp == null )
            {
                bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            }
            if( !bmp.Size.Equals(this.ClientRectangle.Size) )
            {
                bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            }

//             //Bitmap bmp = new Bitmap(DisplayRectangle.Width, DisplayRectangle.Height, g);
//             Graphics gbmp = Graphics.FromImage(bmp);
//             IntPtr srcDc = g.GetHdc();
//             IntPtr bmpDc = gbmp.GetHdc();
// //             StretchBlt(bmpDc, 0, 0, bmp.Width, bmp.Height, srcDc, 0, 0, display.Width, display.Height, 0x00CC0020 /* SRCCOPY */);
// 
//             g.ReleaseHdc(srcDc);
//             gbmp.ReleaseHdc(bmpDc);
//             gbmp.Dispose();
// 
//             Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (bmp != null)
            {
                e.Graphics.Clear(Color.White);
                e.Graphics.DrawImage(bmp, this.ClientRectangle);
            }
        }
        float zoom = 1.0f;
        //float zoomy = 1.0f;
        Rectangle layerviewDisplay = new Rectangle();
        public void UpdateSize(Rectangle display)
        {
//             if (bmp == null)
//                 return;
//             Rectangle display = this.ClientRectangle;
            if (display.Width == 0 || display.Height == 0)
                return;

            layerviewDisplay = display;
            float ratio = (float)display.Width / display.Height;
            int dh = this.Height - this.ClientRectangle.Height;
            int dw = this.Width - this.ClientRectangle.Width;
            int h = this.ClientRectangle.Height;
            int w = (int)(ratio * h);
//             if( ClientRectangle.Width < ClientRectangle.Height )
//             {
//                 w = this.ClientRectangle.Width;
//                 h = (int)(ratio * w);
//             }
            zoom = (float)w / display.Width;
//             float zoomy = (float)h / display.Height;
//             zoom = Math.Min(zoomx, zoomy);

            w += dw;
            h += dh;

            Size s = new Size(w, h);
            if (!this.Size.Equals(s))
            {
                this.Size = s;

            }

            CreateBMP();
        }
        public void UpdateLocation()
        {
            Rectangle desktop = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            int x = desktop.Right - this.Width - 20;
            int y = desktop.Bottom - this.Height - 20;
            this.Location = new Point(x, y);
        }
        public Graphics GetGraphics() 
        {
            if (zoom < 0.001)
                return null;
            if (bmp == null)
                return null;
            if (!this.Visible)
                return null;

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.ScaleTransform(zoom, zoom);
            //g.PageUnit = GraphicsUnit.Display;
            //g.SmoothingMode = SmoothingMode.AntiAlias;

            return g;
        }
        Rectangle layerclient;
        public void ReleaseGraphics(Graphics gbmp, Point scrollpos, Rectangle client)
        {
            if (gbmp == null)
                return;

            gbmp.ResetTransform();
            gbmp.ScaleTransform(zoom, zoom);
            scrollpos = new Point(-scrollpos.X, -scrollpos.Y);
            client.Location = scrollpos;
            using (Pen p = new Pen(Color.Black))
                gbmp.DrawRectangle(p, client);

            if (gbmp != null)
                gbmp.Dispose();

            layerclient = client;
            this.Refresh();
        }

        private void Landscape_Resize(object sender, EventArgs e)
        {
            this.SuspendLayout();
            UpdateSize(layerviewDisplay);
            this.ResumeLayout();
            if (layerview != null)
            {
                layerview.Refresh();
                //CreateBMP();
            }
        }

        private void Landscape_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( e.CloseReason == CloseReason.UserClosing )
            {
                e.Cancel = true;
                layerview.OnClosedLandscape();
                this.Hide();
            }
        }

        bool isDown = false;
        PointF cursor;
//         Point down;
        private void Landscape_MouseDown(object sender, MouseEventArgs e)
        {
            isDown = true;
//             down = e.Location;
            UpdatePosition();
        }

        private void Landscape_MouseUp(object sender, MouseEventArgs e)
        {
            isDown = false;
        }
        private void UpdatePosition()
        {
            if (layerview == null)
                return;

            if (isDown)
            {
                float x = cursor.X;
                float y = cursor.Y;
                float width = layerclient.Width;
                float height = layerclient.Height;
                x = x / zoom;
                y = y / zoom;
                layerview.MyScrollX = (int)(width / 2 - x);
                layerview.MyScrollY = (int)(height / 2 - y);
                layerview.MyRefresh();
//                 down = cursor;
            }
        }
        private void CheckFocus()
        {
            if (this.Focused == false)
                this.Focus();
        }
        private void Landscape_MouseMove(object sender, MouseEventArgs e)
        {
            CheckFocus();
            cursor = e.Location;
            UpdatePosition();
        }

        private void Landscape_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1.0;
        }

        private void Landscape_Deactivate(object sender, EventArgs e)
        {
            this.Opacity = 0.5;
        }

        private void Landscape_KeyDown(object sender, KeyEventArgs e)
        {
            layerview.ProcessKeys(e);
        }

    }
}
