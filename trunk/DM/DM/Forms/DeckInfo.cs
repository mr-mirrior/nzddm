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
    public partial class DeckInfo : Form
    {
        Segment deck = new Segment();
        SegmentDAO Segmentdao = SegmentDAO.getInstance();
        string blockName;
        bool isWorking=false;

        public bool IsWorking
        {
            get { return isWorking; }
            set { isWorking = value; }
        }

        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }

        public Segment Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        
        public DeckInfo()
        {
            InitializeComponent();
        }
       
        private void MaxSpeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void ErrorParam_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void StratZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void SpreadZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void DesignRollCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b'))
            {
                e.Handled = true;
            }
        }

        private void OpenDeckInfo_Load(object sender, EventArgs e)
        {
            //System.Diagnostics.Debugger.Break();
            //deck.ErrorParam = 0.1;
            //deck.MaxSpeed = 1;
            //deck.SpreadZ = 1;
            //deck.startZ = 2;
            //deck.DesignRollCount = 8;
            //deck.SegmentName = "888888";

#if !DEBUG
            if (isWorking)
            {
                btnOK.Enabled = false;
                cbSpeedUnit.Enabled = false;
                lbBlockname.Enabled = false;
                lbPastion.Enabled = false;
                tbDeckName.Enabled = false;
                tbDesignRollCount.Enabled = false;
                tbMaxSpeed.Enabled = false;
                txStartZ.Enabled = false;
                txDesignDepth.Enabled = false;
                txErrorParam.Enabled = false;
            }
#endif
            cbSpeedUnit.SelectedIndex = 1;
            //if (isWorking||deck.WorkState== SegmentWorkState.END)
                cbLibrate.SelectedIndex = GetValueIdx(deck.LibrateState);
            //else
            //    cbLibrate.SelectedIndex = 2;
            lbBlockname.Text = BlockName;
            lbPastion.Text = deck.DesignZ.ToString();
            this.tbDeckName.Text = deck.SegmentName;
            this.tbDesignRollCount.Text = deck.DesignRollCount.ToString();
            this.tbMaxSpeed.Text = deck.MaxSpeed.ToString("0.00");
            this.txStartZ.Text = deck.StartZ.ToString("0.00");
            this.txDesignDepth.Text = deck.DesignDepth.ToString("0.00");
            this.txErrorParam.Text = deck.ErrorParam.ToString("0.00");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private  int GetIdxValue(int idx)
        {
            switch (idx)
            {
            case 0:
                    return 0;
            case 1:
                    return 3;
            case 2:
                    return 2;
            case 3:
                    return 1;
                default:
                    return -1;
            }
        }

        private int GetValueIdx(int idx)
        {
            switch (idx)
            {
                case 0:
                    return 0;
                case 3:
                    return 1;
                case 2:
                    return 2;
                case 1:
                    return 3;
                default:
                    return -1;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tbDeckName.Text.Equals(""))
            {
                MessageBox.Show("仓面名称不能为空！");
            }
            else if (tbDesignRollCount.Text.Equals("") || Convert.ToSingle(tbDesignRollCount.Text)==0f || Convert.ToSingle(txErrorParam.Text)==0 || txErrorParam.Text.Equals("") || Convert.ToSingle(tbMaxSpeed.Text)==0 || tbMaxSpeed.Text.Equals("") || txDesignDepth.Text.Equals("") || Convert.ToSingle(txDesignDepth.Text)==0 || txStartZ.Text.Equals("") || Convert.ToSingle(txStartZ.Text)==0)
            {
                MessageBox.Show("输入数值信息不能为0或为空！");
            }
            else if (cbLibrate.SelectedIndex<0)
            {
                MessageBox.Show("请选择仓面的击震力状态！");
            }
            else
            {
                DialogResult dr = MessageBox.Show("您确定保存仓面信息？", "确认输入", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    if (cbSpeedUnit.SelectedIndex == 0)
                    {
                        deck.MaxSpeed = Convert.ToSingle(tbMaxSpeed.Text) * 3.6;
                    }
                    else
                    {
                        deck.MaxSpeed = Convert.ToDouble(tbMaxSpeed.Text);
                    }
                    this.DialogResult = DialogResult.OK;
                    deck.DesignRollCount =Convert.ToInt32(tbDesignRollCount.Text);
                    deck.ErrorParam = Convert.ToDouble(txErrorParam.Text);
                    deck.StartZ = Convert.ToDouble(txStartZ.Text);
                    deck.DesignDepth = Convert.ToDouble(txDesignDepth.Text);
                    deck.SegmentName = tbDeckName.Text;
                    deck.LibrateState = GetIdxValue(cbLibrate.SelectedIndex);
                }
                else
                {
                    this.DialogResult = DialogResult.No;
                }
            }
        }


        float meterPerSecond = 0.0f;
        float kmPerHour = 0.0f;
        private float ToKMPerHour(float meterPerSecond)
        {
            return kmPerHour = meterPerSecond * 3.6f;

        }
        private float ToMeterPerSecond(float kmPerHour)
        {
            return meterPerSecond = kmPerHour / 3.6f;
        }
        private void cbSpeedUnit_TextChanged(object sender, EventArgs e)
        {
            if (cbSpeedUnit.SelectedIndex == 1)
            {
                tbMaxSpeed.Text = ToKMPerHour(Convert.ToSingle(tbMaxSpeed.Text)).ToString();
            }
            else
            {
                tbMaxSpeed.Text = ToMeterPerSecond(Convert.ToSingle(tbMaxSpeed.Text)).ToString();
            }
        }

        private void DeckInfo_Shown(object sender, EventArgs e)
        {
#if !DEBUG
            if (isWorking)
            {
                MessageBox.Show("仓面正在运行或者已经结束，无法修改仓面属性！");
            }
#endif
        }

        private void cbLibrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLibrate.SelectedIndex==1&&!DM.Models.Config.I.GEN_NOLIBRATE_VALID)
            {
                Utils.MB.Warning("此状态目前不是可选的振动标准。");
                cbLibrate.SelectedIndex = -1;
            }
        }
    }
}
