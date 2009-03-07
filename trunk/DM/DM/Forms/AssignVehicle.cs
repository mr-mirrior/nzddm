using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DM.DB;
using System.Data.SqlClient;

namespace DM.Forms
{
    public partial class AssignVehicle : Form
    {
        bool Dresult;

        //引用数据库类的实例
        CarDistributeDAO cardisDAO = CarDistributeDAO.getInstance();
        CarInfoDAO carInfoDAO = CarInfoDAO.getInstance();
        CarDistribute carDis = new CarDistribute();
        List<int> Cars = new List<int>();
        List<CarInfo> allCars = CarInfoDAO.getInstance().getAllCarInfo();


        Segment segment = new Segment();
        Segment deck = new Segment();

        public Segment Deck
        {
            get { return deck; }
            set { deck = value; }
        }
        string blockName;

        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }

        Button[] buttons;
        public AssignVehicle()
        {
            InitializeComponent();
            buttons = new Button[] { btnStop0, btnStop1, btnStop2, btnStop3, btnStop4, 
            btnStop5, btnStop6, btnStop7 };
            //btnStop0.Click += new EventHandler(btnStop_Click);
            btnStop1.Click += new EventHandler(btnStop_Click);
            btnStop2.Click += new EventHandler(btnStop_Click);
            btnStop3.Click += new EventHandler(btnStop_Click);
            btnStop4.Click += new EventHandler(btnStop_Click);
            btnStop5.Click += new EventHandler(btnStop_Click);
            btnStop6.Click += new EventHandler(btnStop_Click);
            btnStop7.Click += new EventHandler(btnStop_Click);

        }
        public AssignVehicle(int blockid, double designZ, int segmentid)
        {
            InitializeComponent();
            deck.BlockID = blockid;
            deck.DesignZ = designZ;
            deck.SegmentID = segmentid;
        }

        private void SendVehicle_Load(object sender, EventArgs e)
        {
            //初始化button
            for (int i = 0; i < 8; i++)
            {
                lstVehicle.Items.Add("");
                buttons[i].Location = new Point(buttons[i].Location.X, lstVehicle.Items[i].Position.Y + lstVehicle.Location.Y);
            }


            //初始化信息条
            lbBlockname.Text = blockName;
            lbPastion.Text = deck.DesignZ.ToString();
            lbDeckName.Text = deck.SegmentName;
            //在Vehical listView 中显示闲置车辆
            SegmentDAO segmentDAO = SegmentDAO.getInstance();
            //Segment segment=new Segment();
            List<Segment> segments = new List<Segment>();
            segments = segmentDAO.getSegment(deck.BlockID, deck.DesignZ, deck.SegmentID);


            foreach (Segment s in segments)
            {
                segment = s;
            }

            if (segments.Count == 0)
            {
                MessageBox.Show("无此仓面!");
                return;
            }



            if (segment.WorkState == SegmentWorkState.WORK)
            {
                UpdateData();
            }
            else if (segment.WorkState == SegmentWorkState.WAIT)
            {
                UpdateAssignData();
            }
            else if (segment.WorkState == SegmentWorkState.END)
            {
                UpdateAssignData();
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            if (lstVehicle.SelectedItems.Count == 0)
            {
                MessageBox.Show("您没选中空闲车辆！");
            }
            else
            {

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //int a = WorkVehical.CheckedItems.Count;
            ////将选中的被派遣的车辆撤销回到闲置列表
            //for (int i=0;i<a;i++)
            //{
            //    string name = WorkVehical.CheckedItems[0].Text;
            //    Cars=Cars+name;
            //    lstVehicle.Items.SetVertex(name);
            //    WorkVehical.Items.Remove(WorkVehical.CheckedItems[0]);
            //}

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstVehicle.CheckedItems.Count<1)
            {
                MessageBox.Show("请勾选要派遣的车辆！");
                return;
            }

            if (segment.WorkState == SegmentWorkState.WORK)
            {
                DialogResult result = MessageBox.Show(
           "仓面正在运行中，\n" +
           "添加到此仓面的车辆将立刻开始工作！\n\n" +
           "按\"是\"确认添加该车，按\"否\"取消操作",
           "添加车辆", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach (ListViewItem item in lstVehicle.CheckedItems)
                    {
                        Int32 carid = carInfoDAO.getCarNameByCarID(allCars, item.Text);
                        Cars.Add(carid);
                        CarDistribute thisCardis = new CarDistribute();
                        thisCardis.Blockid= deck.BlockID;
                        thisCardis.DesignZ = deck.DesignZ;
                        thisCardis.Segmentid = deck.SegmentID;
                        thisCardis.Carid = carid;

                        Dresult = cardisDAO.startCar(thisCardis, deck.MaxSpeed);
                        DMControl.GPSServer.UpdateDeck();
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else if (segment.WorkState == SegmentWorkState.WAIT)
            {
                DialogResult dr = MessageBox.Show("请确认操作", "确认派遣", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    foreach (ListViewItem item in lstVehicle.CheckedItems)
                    {
                        Int32 carid = carInfoDAO.getCarNameByCarID(allCars, item.Text);
                        Cars.Add(carid);
                        item.Tag = CarDistribute_Status.ASSIGNED;
                    }
                    Dresult=cardisDAO.distributeCars(deck.BlockID, deck.DesignZ, deck.SegmentID, Cars);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else if (segment.WorkState == SegmentWorkState.END)
            {
                DialogResult dr = MessageBox.Show("请确认操作", "确认派遣", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    foreach (ListViewItem item in lstVehicle.CheckedItems)
                    {
                        int carid = carInfoDAO.getCarNameByCarID(allCars, item.Text);
                        Cars.Add(carid);
                        item.Tag = CarDistribute_Status.ASSIGNED;
                    }
                    Dresult=cardisDAO.distributeCars(deck.BlockID, deck.DesignZ, deck.SegmentID, Cars);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }


        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Vehical_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if ((CarDistribute_Status)lstVehicle.Items[e.Index].Tag == CarDistribute_Status.WORK)
            {
                e.NewValue = e.CurrentValue;
            }
            else if(((CarDistribute_Status)lstVehicle.Items[e.Index].Tag)!= CarDistribute_Status.FREE)
            {
                e.NewValue =e.CurrentValue;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

            SegmentDAO segmentDAO = SegmentDAO.getInstance();
            Segment segment = new Segment();
            List<Segment> segments = new List<Segment>();
            segments = segmentDAO.getSegment(deck.BlockID, deck.DesignZ, deck.SegmentID);

            int i;
            if (lstVehicle.Items.Count<1)
            {
                return;
            }
            for (i = 0; i < buttons.Length; i++)
            {
                if (((Button)sender) == buttons[i])
                {
                    lstVehicle.Items[i].Selected = true;
                }
            }

            CarDistribute endCardis = new CarDistribute();
            Control c = (sender as Control);
            if ((CarDistribute_Status)c.Tag == CarDistribute_Status.WORK)
            {
                DialogResult result = MessageBox.Show(
                "车辆正在运行，\n" +
                "强制停止可能影响计算结果！\n\n" +
                "按\"是\"确认强制停止该车，按\"否\"取消操作",
                "强制停止车辆", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {

                    ListViewItem item = lstVehicle.SelectedItems[0];
                    item.Tag = CarDistribute_Status.FREE;
                     c.Tag = CarDistribute_Status.FREE;
                     endCardis.Carid = carInfoDAO.getCarNameByCarID(allCars, item.Text);
                     endCardis.Blockid = deck.BlockID;
                     endCardis.DesignZ = deck.DesignZ;
                     endCardis.Segmentid = deck.SegmentID;
                     cardisDAO.endCar(endCardis);
                    foreach (Segment s in segments)
                    {
                        segment = s;
                    }

                    if (segments.Count == 0)
                    {
                        MessageBox.Show("无此仓面!");
                        return;
                    }
                    DMControl.GPSServer.UpdateDeck();
                    this.Close();

                    //if (segment.WorkState == SegmentWorkState.WORK)
                    //{
                    //    UpdateData();
                    //}
                    //else if (segment.WorkState == SegmentWorkState.WAIT)
                    //{
                    //    UpdateAssignData();
                    //}
                    //else if (segment.WorkState == SegmentWorkState.END)
                    //{
                    //    UpdateAssignData();
                    //}
                }




            }
            else if ((CarDistribute_Status)c.Tag == CarDistribute_Status.ASSIGNED)
            {
                DialogResult result = MessageBox.Show(
               "车辆已经在之前分配过，\n" +
               "取消分配可以去掉之前的分配记录！\n\n" +
               "按\"是\"确认取消分配，按\"否\"取消操作",
               "取消分配操作车辆", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    endCardis.Carid = carInfoDAO.getCarNameByCarID(allCars, lstVehicle.SelectedItems[0].Text);
                    endCardis.Blockid = deck.BlockID;
                    endCardis.DesignZ = deck.DesignZ;
                    endCardis.Segmentid = deck.SegmentID;
                    cardisDAO.removeCar(endCardis);
                }
                foreach (Segment s in segments)
                {
                    segment = s;
                }

                if (segments.Count == 0)
                {
                    MessageBox.Show("无此仓面!");
                    return;
                }



                if (segment.WorkState == SegmentWorkState.WORK)
                {
                    UpdateData();
                }
                else if (segment.WorkState == SegmentWorkState.WAIT)
                {
                    UpdateAssignData();
                }
                else if (segment.WorkState == SegmentWorkState.END)
                {
                    UpdateAssignData();
                }
            }

        }
        private bool IsOccupied(int idx)
        {
            if (idx < 0 || idx >= lstVehicle.Items.Count)
                return false;
            return lstVehicle.Items[idx].Tag != null;
        }
        private void CheckStopButtons()
        {
            //if (lstVehicle.SelectedItems.Count != 1)
            //    return;

            //int idx = lstVehicle.SelectedItems[0].Index;
            //for (int i = 0; i < buttons.Length; i++)
            //{
            //    if (idx == i)
            //    {
            //        if ((CarDistribute_Status)buttons[i].Tag == CarDistribute_Status.WORK)
            //        {
            //            buttons[i].Enabled = true;
            //            break;
            //        }
            //        else if ((CarDistribute_Status)buttons[i].Tag == CarDistribute_Status.ASSIGNED)
            //        {
            //            buttons[i].Enabled = true;
            //            break;
            //        }
            //        else if ((CarDistribute_Status)buttons[i].Tag == CarDistribute_Status.ENDWORK)
            //        {
            //            buttons[i].Visible = false;
            //            break;
            //        }
            //        else if ((CarDistribute_Status)buttons[i].Tag == CarDistribute_Status.FREE)
            //        {
            //            buttons[i].Visible = false;
            //            break;
            //        }
            //        ////     }
            //        //buttons[i].Visible = true;
            //        //if (IsOccupied(i))
            //        //    buttons[i].Enabled = true;
            //        //else
            //        //    buttons[i].Enabled = false;
            //    }
            //}
        }
        private void Vehical_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckStopButtons();
        }
        private void UpdateData()
        {

            lstVehicle.Items.Clear();
            for (int j = 0; j < buttons.Length; j++)
            {
                buttons[j].Visible = false;
            }


            //lbBlockname.Text = blockName;
            //lbPastion.Text = deck.DesignZ.ToString();
            //lbDeckName.Text = deck.SegmentName;


            List<CarInfo> inUsedAtThisDeck = cardisDAO.getCarInfosInThisSegment_inuse(deck.BlockID, deck.DesignZ, deck.SegmentID);
            List<CarDistribute> cds = cardisDAO.getCarDistributeInThisSegment_all(deck.BlockID, deck.DesignZ, deck.SegmentID);
            List<CarInfo> unUsedCars = new List<CarInfo>();
            List<CarInfo> InUsedNotAtThisDeck = new List<CarInfo>();
            List<CarInfo> allInUsedCars = CarDistributeDAO.getInstance().getInusedCars();

            //获取没在工作的所有车辆
            foreach (CarInfo ci in allCars)
            {
                int j = 0;
                for (int k = 0; k < allInUsedCars.Count; k++)
                {
                    if (ci.CarID != allInUsedCars[k].CarID)
                    {
                        j++;
                    }
                }
                if (j == allInUsedCars.Count)
                {
                    unUsedCars.Add(ci);
                }

            }
            //获取没有工作在此仓面的车辆信息
            foreach (CarInfo ci in allInUsedCars)
            {
                int j = 0;
                for (int k = 0; k < inUsedAtThisDeck.Count; k++)
                {
                    if (ci.CarID != inUsedAtThisDeck[k].CarID)
                    {
                        j++;
                    }
                }
                if (j == inUsedAtThisDeck.Count)
                {
                    InUsedNotAtThisDeck.Add(ci);
                }
            }

            //添加车辆信息
            int i = 0;
            foreach (CarInfo ci in unUsedCars)
            {
                lstVehicle.Items.Add(ci.CarName);
                buttons[i].Tag = CarDistribute_Status.FREE;
                string status = "可分配";
                ListViewItem item = lstVehicle.Items[i];
                item.Tag = CarDistribute_Status.FREE;
                string info = ci.ScrollWidth.ToString();
#if DEBUG
                info = ci.CarID.ToString();
#endif
                item.SubItems.AddRange(new string[] { info/* + "|" + ci.ScrollWidth.ToString()*/, ci.GPSHeight.ToString(), status });
                i++;
            }

            foreach (CarInfo ci in inUsedAtThisDeck)
            {

                lstVehicle.Items.Add(ci.CarName);
                ListViewItem item = lstVehicle.Items[i];
                buttons[i].Tag = CarDistribute_Status.WORK;
                item.Tag = CarDistribute_Status.WORK;
                buttons[i].Visible = true;
                string status = "此占用";
                string info = ci.ScrollWidth.ToString();
#if DEBUG
                info = ci.CarID.ToString();
#endif
                item.SubItems.AddRange(new string[] { info/*+ "|" + ci.ScrollWidth.ToString()*/, ci.GPSHeight.ToString(), status });
                item.Checked = true;
                item.ForeColor = Color.DarkGray;
                item.BackColor = Color.WhiteSmoke;
                item.Font = new Font(this.Font, FontStyle.Bold);
                i++;
            }

            foreach (CarInfo ci in InUsedNotAtThisDeck)
            {
                lstVehicle.Items.Add(ci.CarName);
                ListViewItem item = lstVehicle.Items[i];
                buttons[i].Tag = CarDistribute_Status.WORK;
                buttons[i].Visible = true;
                buttons[i].Enabled = false;
                string status = "已占用";
                item.Tag = CarDistribute_Status.WORK;
                string info = ci.ScrollWidth.ToString();
#if DEBUG
                info = ci.CarID.ToString();
#endif
                item.SubItems.AddRange(new string[] { info/* + "|" + ci.ScrollWidth.ToString()*/, ci.GPSHeight.ToString(), status });
                item.ForeColor = Color.DarkGray;
                item.BackColor = Color.WhiteSmoke;
                item.Font = new Font(this.Font, FontStyle.Bold);
                i++;
            }
            lstVehicle.Items[0].Selected = true;
            CheckStopButtons();

        }
        //lstVehicle.Items[0].Selected = true;
        //CheckStopButtons();
        //}

        private void UpdateAssignData()
        {
            List<CarInfo> inUsedAtThisDeck = cardisDAO.getCarInfosInThisSegment_inuse(deck.BlockID, deck.DesignZ, deck.SegmentID);
            List<CarDistribute> cds = cardisDAO.getCarDistributeInThisSegment_all(deck.BlockID, deck.DesignZ, deck.SegmentID);
            List<CarInfo> unUsedCars = new List<CarInfo>();
            List<CarInfo> InUsedNotAtThisDeck = new List<CarInfo>();
            List<CarInfo> allInUsedCars = CarDistributeDAO.getInstance().getInusedCars();
            List<CarDistribute> allDistributeCars = cardisDAO.getCarDistributeInThisSegment_all_except_end(deck.BlockID, deck.DesignZ, deck.SegmentID);
            List<int> other = new List<int>();

            lstVehicle.Items.Clear();
            for (int j = 0; j < buttons.Length; j++)
            {
                buttons[j].Visible = false;
            }



            
            //获取没有工作在此仓面的车辆信息
            foreach (CarInfo ci in allInUsedCars)
            {
                int j = 0;
                for (int k = 0; k < inUsedAtThisDeck.Count; k++)
                {
                    if (ci.CarID != inUsedAtThisDeck[k].CarID)
                    {
                        j++;
                    }
                }
                if (j == inUsedAtThisDeck.Count)
                {
                    InUsedNotAtThisDeck.Add(ci);
                }
            }

            

            string status = "可分配";
            int i = 0;
            foreach (CarDistribute cd in allDistributeCars)
            {
                foreach (CarInfo ci in allCars)
                {
                    if (ci.CarID==cd.Carid&&cd.IsAssigned())
                    {
                            lstVehicle.Items.Add(ci.CarName);
                            ListViewItem item = lstVehicle.Items[i];
                            item.Tag = CarDistribute_Status.ASSIGNED;
                             status = "已分配";
                             buttons[i].Text = "取消分配(&S)";
                             buttons[i].Tag = CarDistribute_Status.ASSIGNED;
                             buttons[i].Visible = true;
                             string info = ci.ScrollWidth.ToString(); 
#if DEBUG
                             info = ci.CarID.ToString();
#endif
                             item.SubItems.AddRange(new string[] { info/*+"|"+CarI.ScrollWidth.ToString()*/, ci.GPSHeight.ToString(), status });
                             i++;
                             break;
                    }
                   
                }
                other.Add(cd.Carid);
            }
            
            bool add=false;
            foreach (CarInfo ci in allInUsedCars)
            {
                add = false;
                foreach (CarDistribute cI in allDistributeCars)
                {
                    if (ci.CarID == (cI.Carid))
                    {
                        add = true;
                        break;
                    }
                }
                if (!add)
                {
                    lstVehicle.Items.Add(ci.CarName);
                    ListViewItem item = lstVehicle.Items[i];
                    status = "正在工作，可分配";
                    buttons[i].Tag = CarDistribute_Status.FREE;
                    item.Tag = CarDistribute_Status.FREE;
                    buttons[i].Visible = false;
                    item.ForeColor = Color.DarkGray;
                    item.BackColor = Color.WhiteSmoke;
                    item.Font = new Font(this.Font, FontStyle.Bold);
                    string info = ci.ScrollWidth.ToString();
                    item.SubItems.AddRange(new string[] { info/*+"|"+CarI.ScrollWidth.ToString()*/, ci.GPSHeight.ToString(), status });
                    other.Add(ci.CarID);
                    i++;
                }
                }

            List<CarInfo> getOther = cardisDAO.getOthers(other);
            foreach(CarInfo ci in getOther)
            {
                lstVehicle.Items.Add(ci.CarName);
                ListViewItem item = lstVehicle.Items[i];
                status = "可分配";
                buttons[i].Tag = CarDistribute_Status.FREE;
                item.Tag = CarDistribute_Status.FREE;
                buttons[i].Visible = false;
                string info = ci.ScrollWidth.ToString();
#if DEBUG
                info=ci.CarID.ToString();
#endif
                item.SubItems.AddRange(new string[] { info, ci.GPSHeight.ToString(), status });
                i++;
            }
            lstVehicle.Items[0].Selected = true;

            //添加车辆信息
            //int i = 0;
            //foreach (CarInfo CarI in allCars)
            //{

            //    lstVehicle.Items.SetVertex(CarI.CarName);
            //    ListViewItem item = lstVehicle.Items[i];
            //    string status = "可分配";
            //    buttons[i].Tag = CarDistribute_Status.FREE;
            //    foreach (CarDistribute cd in cds)
            //    {
            //        if (cd.Carid == CarI.CarID)
            //        {
            //            if (cd.IsAssigned())
            //            {
            //                status = "已分配";
            //                buttons[i].Text = "取消分配(&S)";
            //                buttons[i].Tag = CarDistribute_Status.ASSIGNED;
            //                buttons[i].Visible = true;
            //                break;
            //            }

            //            else if (cd.IsFinished())
            //            {
            //                status = "可分配";
            //                buttons[i].Tag = CarDistribute_Status.ENDWORK;
            //                buttons[i].Visible = false;
            //            }
            //            else if (cd.IsWorking())
            //            {
            //                status = "正在工作，可分配";
            //                buttons[i].Tag = CarDistribute_Status.WORK;
            //                buttons[i].Visible = false;
            //                item.ForeColor = Color.DarkGray;
            //                item.BackColor = Color.WhiteSmoke;
            //                item.Font = new Font(this.Font, FontStyle.Bold);
            //            }
            //        }
            //    }
            //    item.SubItems.AddRange(new string[] { CarI.CarID.ToString()/*+"|"+CarI.ScrollWidth.ToString()*/, CarI.GPSHeight.ToString(), status });
            //    i++;
            //}
            //lstVehicle.Items[0].Selected = true;
            //CheckStopButtons();
        }
        private void AssignVehicle_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Dresult==true&&segment.WorkState==SegmentWorkState.WORK)
            {
                MessageBox.Show("添加车辆成功，车辆已经投入碾压中！");
            }
            else if (Dresult == true && segment.WorkState == SegmentWorkState.WAIT)
            {
                MessageBox.Show("分配车辆成功！");
            }
            else if (Dresult == true && segment.WorkState == SegmentWorkState.END)
            {
                MessageBox.Show("分配车辆成功！");
            }
        }
    }

}