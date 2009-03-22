using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DM.Models;

namespace DM.Forms
{
    public partial class VehicleHistory : Form
    {
        public VehicleHistory()
        {
            InitializeComponent();
        }

        Deck deck;

        public Deck Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VehicleHistory_Load(object sender, EventArgs e)
        {
            lstVehicle.Items.Clear();
            if (deck == null)
                return;
            lbBlockname.Text = deck.DeckInfo.BlockName;
            lbDeckName.Text = deck.Name;
            lbPastion.Text = deck.DeckInfo.DesignZ.ToString("0.0");
            List<DB.CarDistribute> carsDistributed = DB.CarDistributeDAO.getInstance().getCarInfosInThisSegment_Distributed(deck.DeckInfo.BlockID,deck.DeckInfo.DesignZ,deck.DeckInfo.SegmentID);
            if (carsDistributed.Count == 0)
                return;
            string start, end;
            foreach (DB.CarDistribute info in carsDistributed)
            {
                ListViewItem item=lstVehicle.Items.Add(DM.DMControl.VehicleControl.FindVechicle(info.Carid).CarName);
                if (info.DTStart == DateTime.MinValue)
                    start = "尚未开始";
                else
                    start = info.DTStart.ToString();
                if (info.DTEnd == DateTime.MinValue)
                    end = "尚未结束";
                else
                    end = info.DTEnd.ToString();
                item.SubItems.AddRange(new string[] { start, end });
            }
        }
    }
}
