﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D; 


namespace DM
{
    public partial class RoundPanel : Panel
    {
        private int mMatrixRound = 8;
        private Color mBack;

        public Color Back
        {
            get { return mBack; }
            set
            {
                if (value == null)
                {
                    mBack = Control.DefaultBackColor;
                }
                else
                {
                    mBack = value;
                }
                base.Refresh();
            }
        }

        public int MatrixRound
        {
            get { return mMatrixRound; }
            set
            {
                mMatrixRound = value;
                base.Refresh();
            }
        }

        public RoundPanel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private GraphicsPath CreateRound(Rectangle rect, int radius)
        {
            GraphicsPath roundRect = new GraphicsPath();
            //顶端 
            roundRect.AddLine(rect.Left + radius - 1, rect.Top - 1, rect.Right - radius, rect.Top - 1);
            //右上角 
            roundRect.AddArc(rect.Right - radius, rect.Top - 1, radius, radius, 270, 90);
            //右边 
            roundRect.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom - radius);
            //右下角

            roundRect.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            //底边 
            roundRect.AddLine(rect.Right - radius, rect.Bottom, rect.Left + radius, rect.Bottom);
            //左下角 
            roundRect.AddArc(rect.Left - 1, rect.Bottom - radius, radius, radius, 90, 90);
            //左边 
            roundRect.AddLine(rect.Left - 1, rect.Top + radius, rect.Left - 1, rect.Bottom - radius);
            //左上角 
            roundRect.AddArc(rect.Left - 1, rect.Top - 1, radius, radius, 180, 90);
            return roundRect;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            int width = base.Width - base.Margin.Left - base.Margin.Right;
            int height = base.Height - base.Margin.Top - base.Margin.Bottom;
            Rectangle rec = new Rectangle(base.Margin.Left, base.Margin.Top, width, height);
            GraphicsPath round = CreateRound(rec, mMatrixRound);
            //GraphicsPath round = new GraphicsPath();
            //round.AddEllipse(this.ClientRectangle);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using(Brush b = new SolidBrush(this.ForeColor))
                e.Graphics.FillPath(b, round);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.Refresh();
        } 


    }
}
