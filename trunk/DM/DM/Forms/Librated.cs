using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DM.Forms
{
    struct LibratedInfo
    {
        int _CarID;

        public int CarID
        {
            get { return _CarID; }
        }

        string _CarName;

        public string CarName
        {
            get { return _CarName; }
            set { _CarName = value; }
        }

        int _LibrateStaus;

        public int LibrateStaus
        {
            get { return _LibrateStaus; }
            set { _LibrateStaus = value; }
        }

        public LibratedInfo(int id,string carname,int librate)
        {
            _CarID = id;
            _CarName = carname;
            _LibrateStaus = librate;
        }
    }


    public partial class Librated : Form
    {
        public Librated()
        {
            InitializeComponent();
        }
        Timer _Timer ;
        const int EACHHEIGHT = 25;
        private List<LibratedInfo> _LibratedInfos;
        private Font _MyFont = new Font("微软雅黑",9f);
        private Brush _NoLibratedBrush=Brushes.Gray;
        private Brush _LowLibratedBrush = Brushes.Blue;
        private Brush _HighLibratedBrush = Brushes.Red;


        static Librated _FrmLibrated;

        public static Librated GetInstance
        {
            get
            {
                if (_FrmLibrated == null)
                    _FrmLibrated = new Librated();
                return _FrmLibrated;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            int i = 0;
            Graphics g = e.Graphics;

            if (_LibratedInfos == null)
                return;

            foreach (LibratedInfo info in _LibratedInfos)
            {
                g.DrawString(info.CarName, _MyFont, Brushes.Black, e.ClipRectangle.Left,e.ClipRectangle.Top+25*i);
               
                using (Pen p = new Pen(Color.White, 3))
                    g.DrawLine(p, 0, e.ClipRectangle.Top + 25 * (i + 1) - 4, e.ClipRectangle.Width, e.ClipRectangle.Top + 25 * (i + 1) - 4);
                using (Pen p = new Pen(Color.Black))
                {
                    p.DashStyle = DashStyle.Dash;
                    p.DashPattern = new float[] { 3, 3 };
                    g.DrawLine(p, 0, e.ClipRectangle.Top + 25 * (i + 1) - 4, e.ClipRectangle.Width, e.ClipRectangle.Top + 25 * (i + 1) -4);
                }

                SizeF size=g.MeasureString("一号碾压机",_MyFont);
                
                switch (info.LibrateStaus)
                {
                    case 0:
                        g.FillRectangle(_NoLibratedBrush, size.Width, e.ClipRectangle.Top + 25 * i+4,140,10);
                	    break;
                    case 1:
                        g.FillRectangle(_LowLibratedBrush, size.Width, e.ClipRectangle.Top + 25 * i+4, 140, 10);
                        break;
                    case 2:
                        g.FillRectangle(_HighLibratedBrush, size.Width, e.ClipRectangle.Top + 25 *i+4, 140, 10);
                        break;
                }
                
                i++;
            }

            g.DrawString("不振:", _MyFont, Brushes.Black, 0, e.ClipRectangle.Bottom-16);
            g.FillRectangle(Brushes.Gray, 40, e.ClipRectangle.Bottom - EACHHEIGHT/2,20,10);
            g.DrawString("低振:", _MyFont, Brushes.Black, 70, e.ClipRectangle.Bottom -16);
            g.FillRectangle(Brushes.Blue, 110, e.ClipRectangle.Bottom - EACHHEIGHT/2, 20, 10);
            g.DrawString("高振:", _MyFont, Brushes.Black, 140, e.ClipRectangle.Bottom-16 );
            g.FillRectangle(Brushes.Red, 180, e.ClipRectangle.Bottom - EACHHEIGHT/2, 20, 10);
           
        }

        private void Librated_Load(object sender, EventArgs e)
        {
            _Timer = new Timer();
            _Timer.Interval = 1000;
            _Timer.Tick += new EventHandler(_Timer_Tick);
            _Timer.Start();
            FillLibratedInfos();
            this.Location = new Point(3, Main.MainWindow.Height - this.Height - 3);
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.Refresh();
        }

        void _Timer_Tick(object sender, EventArgs e)
        {
           if(_NoLibratedBrush==Brushes.Gray)
           {
               _NoLibratedBrush = Brushes.White;
               _LowLibratedBrush = Brushes.White;
               _HighLibratedBrush = Brushes.White;
           }
           else
           {
               _NoLibratedBrush = Brushes.Gray;
               _LowLibratedBrush = Brushes.Blue;
               _HighLibratedBrush = Brushes.Red;
           }
           this.Refresh();
        }

        private void FillLibratedInfos()
        {
           int[] carIDs=Utils.FileHelper.FileHelper.ReaderLibratedIDS();
           this.Height = EACHHEIGHT * (carIDs.Length + 2);
           _LibratedInfos = new List<LibratedInfo>();

            for (int i=0;i<carIDs.Length;i++)
            {
                for (int j=0;j<DMControl.VehicleControl.carIDs.Length;j++)
                {
                    if(carIDs[i]==DMControl.VehicleControl.carIDs[j])
                    {
                        _LibratedInfos.Add(new LibratedInfo(carIDs[i],DB.CarInfoDAO.getInstance().getCarName(carIDs[i]),DMControl.VehicleControl.carLibratedStates[j]));
                        break;
                    }
                }
            }
        }

        public void ChangeLibratedInfos(int carid,int libratedinfo)
        {
            LibratedInfo info;
            for(int i=0;i<_LibratedInfos.Count;i++)
            {
                if (_LibratedInfos[i].CarID == carid)
                {
                    info = _LibratedInfos[i];
                    info.LibrateStaus = libratedinfo;
                    _LibratedInfos[i] = info;
                    this.Refresh();
                    break;
                }
            }
        }

        private void Librated_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void Librated_Deactivate(object sender, EventArgs e)
        {
            this.Opacity = 0.75;
        }

        private void Librated_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}
