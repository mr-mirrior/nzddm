using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DM.Forms
{
    public partial class DeckCoordInput : Form
    {
        public DeckCoordInput()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tbCoords.Text.TrimEnd().Equals(string.Empty))
            {
                Utils.MB.Warning("输入坐标不能为空！");
                return;
            }
            //if (!tbCoords.Text.TrimEnd().Substring(tbCoords.Text.TrimEnd().Length-1,1).Equals(";"))
            //{
            //    Utils.MB.Warning("您的最后一个坐标后面没有 ';'，请检查！");
            //    return;
            //}
            List<DM.Geo.Coord> deckcoords = new List<DM.Geo.Coord>();
            string[] coords = tbCoords.Text.Split(';');
            for (int i = 0; i < coords.Length;i++ )
            {
                string coord = coords[i].Trim();
                string[] cdxy=coord.Split(',');
                if (cdxy.Length < 2)
                {
                    Utils.MB.Warning("您输入的坐标不正确，请检查后重新输入！");
                    return;
                }
                DM.Geo.Coord cd = new DM.Geo.Coord(Convert.ToDouble(cdxy[0]),Convert.ToDouble(cdxy[1]));
                if (cd.XF>700||cd.YF>500||cd.YF<-500)
                {
                    Utils.MB.Warning("输入坐标超越坝轴坐标界限，请检查后重新输入！");
                }
                deckcoords.Add(cd.ToEarthCoord());
            }
            deckcoords.Add(deckcoords.First());
            
            Forms.ToolsWindow.I.CurrentLayer.deckSelectPolygon = deckcoords;
            Forms.ToolsWindow.I.CurrentLayer.isDeckInput = true;
            Forms.ToolsWindow.I.CurrentLayer.Layer.isDeckInput = true;
            this.Close();
            Forms.ToolsWindow.I.CurrentLayer.IsPolySelecting=false;
        }

        private void DeckCoordInput_Load(object sender, EventArgs e)
        {
            tbCoords.Focus();
        }

        private void tbCoords_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) ||
                e.KeyChar == '\b' ||
                e.KeyChar == Convert.ToChar(".") ||
                e.KeyChar == Convert.ToChar(",") ||
                e.KeyChar == Convert.ToChar(";")||
                e.KeyChar == Convert.ToChar("-")))
            {
                e.Handled = true;
            }
        }
    }
}
