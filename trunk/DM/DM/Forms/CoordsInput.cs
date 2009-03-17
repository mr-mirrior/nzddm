using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DM.DB;

namespace DM.Forms
{
    public partial class CoordsInput : Form
    {
/*
/// //////////////////////////////////////////////////////大坝坐标范围
        /// x<700;
        /// 
        /// y<+-500
*/

        Segment deck = new Segment();

        public Segment Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        public CoordsInput()
        {
            InitializeComponent();
        }
        public string Coords { get { return tbCoords.Text; } set { tbCoords.Text = value; UpdateCoords(); TranslateToAxis(); } }
        public string CoordsNoadd { get { return tbCoords.Text; } set { tbCoords.Text = value; UpdateCoordsNoAdd(); TranslateToAxis(); } }
        public string Comments { get { return tbMark.Text; } set { tbMark.Text = value; } }

        private void NotRolling_Load(object sender, EventArgs e)
        {
            lbBlockname.Text = deck.BlockName;
            lbDeckName.Text = deck.SegmentName;
            lbPastion.Text = deck.DesignZ.ToString("0.0");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tbCoords.Text.Equals(string.Empty) || tbMark.Text.Equals(string.Empty))
            {
                Utils.MB.Warning("坐标和备注都不允许为空。");
                return;
            }

            UpdateCoords();
            //CheckCoordsClosed();
            string result = FormatCoords();
            string areas = CalcAreas();
            SegmentDAO dao = DB.SegmentDAO.getInstance();
            if( !dao.UpdateSegmentNotRolling(deck.BlockID, deck.DesignZ, deck.SegmentID, result, tbMark.Text, areas) )
            {
                Utils.MB.Warning("保存数据库失败！");
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                Utils.MB.OK("保存成功！");
                DialogResult = DialogResult.OK;
            }
        }

        private void tbCoord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || 
                e.KeyChar == '\b' || 
                e.KeyChar == Convert.ToChar(".") || 
                e.KeyChar == Convert.ToChar(",") || 
                e.KeyChar == Convert.ToChar(";")||
                e.KeyChar==Convert.ToChar("|")||
                e.KeyChar == Convert.ToChar("-")))
            {
                e.Handled = true;
            }
        }
        List<List<Geo.Coord>> nrs = new List<List<DM.Geo.Coord>>();

        public List<List<Geo.Coord>> NRs
        {
            get { return nrs; }
            set { nrs = value; }
        }
        private void UpdateCoords()
        {
            nrs.Clear();
            if (tbCoords.Text == null ||
                tbCoords.Text.Length == 0)
                return;

            string[] s0 = tbCoords.Text.Split(new char[] { '|' });
            foreach(string batch in s0)
            {
                List<Geo.Coord> v = new List<DM.Geo.Coord>();
                string[] s1;
                s1 = batch.Split(new char[] { ';' });
                foreach (string s2 in s1)
                {
                    s2.Trim();
                    string[] s3;
                    s3 = s2.Split(new char[] { ',' });
                    if (s3.Length != 2)
                    {
                        continue;
                    }
                    s3[0].Trim();
                    s3[1].Trim();

                    float x, y;
                    if (float.TryParse(s3[0], out x) &&
                        float.TryParse(s3[1], out y))
                    {
                        v.Add(new DM.Geo.Coord(x, y));
                    }
                }
                if (v.Count != 0)
                {
                    if (v.First().Equals(v.Last()) == false)
                        v.Add(v.First());
                    nrs.Add(v);
                }
            }
        }
        private void UpdateCoordsNoAdd()
        {
            nrs.Clear();
            if (tbCoords.Text == null ||
                tbCoords.Text.Length == 0)
                return;

            string[] s0 = tbCoords.Text.Split(new char[] { '|' });
            foreach (string batch in s0)
            {
                List<Geo.Coord> v = new List<DM.Geo.Coord>();
                string[] s1;
                s1 = batch.Split(new char[] { ';' });
                foreach (string s2 in s1)
                {
                    s2.Trim();
                    string[] s3;
                    s3 = s2.Split(new char[] { ',' });
                    if (s3.Length != 2)
                    {
                        continue;
                    }
                    s3[0].Trim();
                    s3[1].Trim();

                    float x, y;
                    if (float.TryParse(s3[0], out x) &&
                        float.TryParse(s3[1], out y))
                    {
                        v.Add(new DM.Geo.Coord(x, y));
                    }
                }
                nrs.Add(v);
                //if (v.Count != 0)
                //{
                //    if (v.First().Equals(v.Last()) == false)
                //        v.Add(v.First());
                //    nrs.Add(v);
                //}
            }
        }
        private void TranslateToAxis()
        {
            if (nrs.Count == 0)
                return;
            string r = "";
            for(int j=0; j<nrs.Count; j++)
            {
                List<Geo.Coord> lst = nrs[j];
                for (int i = 0; i < lst.Count; i++ )
                {
                    lst[i] = lst[i].ToDamAxisCoord();
                    r += lst[i].XF.ToString();
                    r += ", ";
                    r += lst[i].YF.ToString();
                    r += "; ";
                }
                if( j<(nrs.Count-1) )
                    r += "| ";
            }
            tbCoords.Text = r;
        }
        private void CheckCoordsClosed()
        {
//             UpdateCoords();
//             if (nrs.Count < 2)
//                 return;
//             if (nrs.First().Equals(nrs.Last()))
//                 return;
//             tbX.Text = nrs.First().XF.ToString();
//             tbY.Text = nrs.First().YF.ToString();
//             btnAddCoord_Click(null, null);
// 
//             UpdateCoords();
        }
        string result = "";
        private string FormatCoords()
        {
            for (int i = 0; i < nrs.Count; i++ )
            {
                for (int j = 0; j < nrs[i].Count; j++ )
                {
                    Geo.Coord c = nrs[i][j].ToEarthCoord();
                    //nr[i][j] = c;
                    result += c.XF.ToString();
                    result += ", ";
                    result += c.YF.ToString();
                    result += "; ";
                }
                if (i < (nrs.Count - 1))
                    result += "| ";
            }
            return result;
        }
        private string CalcAreas()
        {
            string areas = "";
            foreach (List<Geo.Coord> lst in nrs)
            {
                Models.Polygon pl = new DM.Models.Polygon(lst);
                areas += pl.ActualArea.ToString();
                areas += ", ";
            }

            return areas;
        }
        private void btnAddCoord_Click(object sender, EventArgs e)
        {
            float x, y;
            if( !float.TryParse(tbX.Text, out x) ||
                !float.TryParse(tbY.Text, out y ))
            {
                Utils.MB.Warning("坐标输入有误，请重试。");
                return;
            }
            string pair = tbX.Text +", "+ tbY.Text + "; ";
            tbCoords.Text += pair;
            //nrs.SetVertex(new DM.Geo.Coord(x, y));
            UpdateCoords();

            tbX.Focus();
        }

        private void tbX_Enter(object sender, EventArgs e)
        {
            tbX.SelectAll();
        }

        private void tbY_Enter(object sender, EventArgs e)
        {
            tbY.SelectAll();
        }
    }
}