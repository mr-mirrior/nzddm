using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using DM.DB;
using System.Security.Cryptography;
using System.Security;
using System.Threading;
using DM.Utils.Visual;
using DM.DMControl;

namespace DM.Forms
{
    public partial class DMLogin : Form
    {
        List<USERINFO> listUserInfo;
        List<string> listServerString = new List<string>();
        UserInfo uI;
        USERINFO ui = new USERINFO();
        LoginResult result;

        public DMLogin()
        {
            InitializeComponent();
        }

        string username, password;


     
        private void loginbtn_Click(object sender, EventArgs e)
        {
            //账号密码不能为空
            if (cbuserName.Text.Equals("") || PassWord.Text.Equals(""))
            {
                lbregister.Text = "用户名密码不能为空！";
            }
            //用户名密码输入错误

            else
            {
                username = cbuserName.Text;
                password = PassWord.Text;
                //RunAsyncOperation(DoLogin);
                UserDAO UD = UserDAO.getInstance();
                LoginControl.loginResult = UD.login(username, password);
                result = LoginControl.loginResult;



                //登录判断
                if (result == LoginResult.INVALID_PASSWORD ||
                    result == LoginResult.INVALID_USER)
                {
                    lbregister.Text = "用户名密码输入错误！请重试。";
                    //base.EndAsyncIndication();
                    //return;
                }
//                 else if (result == LoginResult.INVALID_USER)
//                 {
// 
//                     lbregister.Text = "用户名无效！请重试";
//                     //base.EndAsyncIndication();
//                     //return;
//                 }

                else if (result == LoginResult.ERROR)
                {
                    lbregister.Text = /*Utils.MB.Warning*/("数据库访问错误！请检查网络连接或服务器状态。");
                }
                else
                {
                    DialogResult = DialogResult.OK;
                }
                //base.EndAsyncIndication();
                if (DialogResult == DialogResult.OK)
                {
                    this.Close();
                }

                if (!LoginOKorNO(result))
                {
                    Thread.Sleep(1000);
                }
                //根据是否勾选复选框保存数据
                else
                {
                    if (RetainMe.Checked)
                    {
                        bool allN = true;
                        foreach (USERINFO UI in uI.ListUserInfo)
                        {
                            if (UI.UserName.Equals(username))
                            {
                                allN = false;
                            }
                        }
                        if (allN)
                        {
                            cbuserName.Items.Add(username);
                        }
                        ui.UserName = cbuserName.Text;
                        if (SavePassWord.Checked)
                        {
                            ui.Password = Utils.Security.EncryptDES(password, "tjdxtjdx");
                        }
                        bool allNo = true;
                        if (uI.ListUserInfo.Count > 0)
                        {
                            foreach (USERINFO server in uI.ListUserInfo)
                            {
                                if (server.UserName.Equals(username))
                                {
                                    allNo = false;
                                }
                            }

                        }
                        if (allNo)
                        {
                            uI.ListUserInfo.Add(ui);
                        }
                        //uI.ListUserInfo = listUserInfo;
                        Utils.Xml.XMLUtil<UserInfo>.SaveXml("UserInfo.xml", uI);
                    }
                }


                
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #region  //控制btnLogin Enable
        private void cbServer_KeyUp(object sender, KeyEventArgs e)
        {
            if (cbServer.Text.Equals(""))
            {
                btnLogin.Enabled = false;
            }
        }
        private void cbServer_TextChanged(object sender, EventArgs e)
        {
            if (!cbServer.Text.Equals("") && !cbuserName.Text.Equals("") && !PassWord.Text.Equals(""))
            {
                btnLogin.Enabled = true;
            }
        }

        private void PassWord_TextChanged(object sender, EventArgs e)
        {
            if (!cbServer.Text.Equals("") && !cbuserName.Text.Equals("") && !PassWord.Text.Equals(""))
            {
                btnLogin.Enabled = true;
            }
        }
        private void UserName_KeyUp(object sender, KeyEventArgs e)
        {
            if (cbuserName.Text.Equals(""))
            {
                btnLogin.Enabled = false;
            }
        }

        private void PassWord_KeyUp(object sender, KeyEventArgs e)
        {
            if (PassWord.Text.Equals(""))
            {
                btnLogin.Enabled = false;
            }
        }
        #endregion
        //控制checkBox
        private void RetainMe_CheckedChanged(object sender, EventArgs e)
        {
            if (RetainMe.Checked)
            {
                SavePassWord.Enabled = true;
            }
            else
            {
                SavePassWord.Checked = false;
                SavePassWord.Enabled = false;
            }
        }

        private void DMLogin_Load(object sender, EventArgs e)
        {
       
            toolTip1.SetToolTip(SavePassWord, "密码以密文形式加密");
            uI = Utils.Xml.XMLUtil<UserInfo>.LoadXml("UserInfo.xml");
            if (uI == null)
                return;

            RetainMe.Checked = uI.RetainMe;
            SavePassWord.Checked = uI.SavePassword;
            listUserInfo = uI.ListUserInfo;

            if (uI == null)
            {
                uI = new UserInfo();
            }


            foreach (USERINFO ui in uI.ListUserInfo)
            {
                if (ui.UserName != null)
                {
                    cbuserName.Items.Add(ui.UserName);
                }
            }
            foreach (string server in uI.ListServerString)
            {
                cbServer.Items.Add(server);
            }
            if (uI.ListUserInfo.Count > 0)
            {
                cbuserName.SelectedIndex = uI.Index;
            }

        }

        private void UserName_TextChanged(object sender, EventArgs e)
        {
            if (uI == null)
            {
                uI = new UserInfo();
            }
            if (uI.ListUserInfo.Count > 0)
            {
                listUserInfo = uI.ListUserInfo;
                foreach (USERINFO ui in listUserInfo)
                {
                    if (ui.UserName.Equals(cbuserName.Text))
                    {
                        PassWord.Text = Utils.Security.DecryptDES(ui.Password, "tjdxtjdx");
                        break;
                    }
                }
            }
            if (!cbServer.Text.Equals("") && !cbuserName.Text.Equals("") && !PassWord.Text.Equals(""))
            {
                btnLogin.Enabled = true;
            }
        }

        private void cbServer_Leave(object sender, EventArgs e)
        {
            bool allNo = true;
            if (!cbServer.Text.Equals("172.23.225.223"))
            {
                foreach (string server in listServerString)
                {
                    if (server.Equals(cbServer.Text))
                    {
                        allNo = false;
                    }
                }
                if (allNo)
                {
                    string serverString = cbServer.Text;
                    listServerString.Add(serverString);
                    uI.ListServerString = listServerString;
                }
            }

        }

        private void DMLogin_FormClosed(object sender, FormClosedEventArgs e)
        {

            
        }
        private bool LoginOKorNO(LoginResult LResult)
        {

            if (LResult == LoginResult.ERROR || LResult == LoginResult.INVALID_PASSWORD || LResult == LoginResult.INVALID_USER)
            {
                return false;

            }
            else
            {
                return true;
            }
        }

        private void DMLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (result == LoginResult.ADMIN || result == LoginResult.DISWARNING || result == LoginResult.OPERATOR || result == LoginResult.VIEW)
            {
                if (uI == null)
                    return;
                uI.RetainMe = RetainMe.Checked;
                uI.SavePassword = SavePassWord.Checked;
                if (cbuserName.SelectedIndex != -1)
                {
                    uI.Index = cbuserName.SelectedIndex;
                }
                Utils.Xml.XMLUtil<UserInfo>.SaveXml("UserInfo.xml", uI);
            }
            if(result==LoginResult.ERROR||result==LoginResult.INVALID_PASSWORD||result==LoginResult.INVALID_USER)
            {
                if (e.CloseReason== CloseReason.UserClosing)
                {
                    //this.Close();
                }
                else
                {
                    e.Cancel = true;
                } 
            }
        }

    }
}
