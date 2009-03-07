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
    public partial class OverBlock : Form
    {
        string blockName = "ED";
        string deckInfo = "";

        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }
        public string DeckInfo { get { return deckInfo; } set { deckInfo = value; } }

        List<DM.DB.Segment> elevationList = new List<DM.DB.Segment>();

        public List<DM.DB.Segment> PartitionList
        {
            get { return elevationList; }
            set { elevationList = value; }
        }

        double elevation = -1;

        public double Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }
        int deckid = -1;

        public int DeckID
        {
            get { return deckid; }
            set { deckid = value; }
        }

        public OverBlock()
        {
            InitializeComponent();
        }

        private void OverBlock_Load(object sender, EventArgs e)
        {
            lbBlock.Text = blockName;
            lbTobedone.Text = DeckInfo;

            if (elevationList.Count == 0)
            {
                cbElevation.Items.Add("未找到任何碾压记录");
                cbElevation.Enabled = false;
                btnOK.Enabled = false;
            }
            else
            {
                for (int i = 0; i < elevationList.Count; i++)
                {
                    if( cbElevation.Items.IndexOf(elevationList[i].DesignZ) == -1)
                        cbElevation.Items.Add(elevationList[i].DesignZ);
                }
//                 cbPartition.DataSource = elevationList;
//                 cbPartition.DisplayMember = "DesignZ";
                cbElevation.Enabled = true;
                btnOK.Enabled = true;
            }
            if (cbElevation.Items.Count != 0)
                cbElevation.SelectedIndex = 0;
        }
        private DB.Segment FindDeck()
        {
            if (cbDecks.Items.Count == 0)
                return null;
            string name = cbDecks.SelectedItem as string;
            foreach (DB.Segment seg in elevationList)
            {
                if (seg.SegmentName.Equals(name))
                    return seg;
            }
            return null;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if( cbElevation.SelectedItem != null )
            {
                elevation = (double)cbElevation.SelectedItem;
                if(cbDecks.SelectedItem != null )
                {
                    DB.Segment seg = FindDeck();
                    if (seg == null)
                        return;
                    deckid = seg.SegmentID;
                }
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cbPartition_SelectedIndexChanged(object sender, EventArgs e)
        {
            double elev = (double)cbElevation.SelectedItem;
            cbDecks.Items.Clear();
            cbDecks.Text = "无仓面";
            cbDecks.Enabled = false;
            foreach (DB.Segment seg in elevationList)
            {
                if (seg.DesignZ == elev)
                {
                    cbDecks.Items.Add(seg.SegmentName);
                }
            }
            if (cbDecks.Items.Count != 0)
            {
                cbDecks.Enabled = true;
                cbDecks.SelectedIndex = 0;
            }
        }
    }
}
