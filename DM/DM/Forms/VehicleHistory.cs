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
            List<DB.CarInfo> carsDistributed = DB.CarDistributeDAO.getInstance().getCarInfosInThisSegment_Distributed();
        }
    }
}
