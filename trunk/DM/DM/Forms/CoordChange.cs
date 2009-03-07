using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DM.Geo;

namespace DM.Forms
{
    public partial class CoordChange : Form
    {
        Coord coord;
        public Coord Coord
        {
            set { coord = value; }
            get { return coord; }
        }

        public CoordChange()
        {
            InitializeComponent();
        }

        private void CoordChange_Load(object sender, EventArgs e)
        {
            tbXValue.Text = coord.X.ToString();
            tbYValue.Text = coord.Y.ToString();  
        }

        private void tbXValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void tbYValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (tbXValue.Text.Equals("")||tbYValue.Text.Equals(""))
            {
                MessageBox.Show("修改值不能为空！");
            }
            else
            {
                double x;
                double y;
                x = Convert.ToDouble(tbXValue.Text);
                y = Convert.ToDouble(tbYValue.Text);
                retrunCoord(x, y);
            }

        }
     /// <summary>
    /// 返回修改完的Coord
    /// </summary>
        public Coord retrunCoord(double x,double y)
        {
            return new Coord(x, y);
        }
    }
}
