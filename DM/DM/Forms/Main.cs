using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;

namespace DM.Forms
{
    public partial class Main : Form/*DM.Utils.Visual.AsyncBaseDialog*/, IMessageFilter
    {
        public Main()
        {
            InitializeComponent();
            Utils.Global.Windows.MainWindow = this;
            //Control.CheckForIllegalCrossThreadCalls = false;
        }
        #region 最高优先级键盘响应
        public bool PreFilterMessage(ref Message msg)
        {
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_LBUTTONUP = 0x0202;
            const int WM_LBUTTONDBLCLK = 0x0203;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_RBUTTONUP = 0x0205;
            const int WM_RBUTTONDBLCLK = 0x0206;
            const int WM_MBUTTONDOWN = 0x0207;
            const int WM_MBUTTONUP = 0x0208;
            const int WM_MBUTTONDBLCLK = 0x0209;
            const int WM_KEYDOWN = 0x0100;
            const int WM_KEYUP = 0x0101;
            //const int WM_CHAR = 0x0102;

            switch (msg.Msg)
            {
                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                case WM_LBUTTONDBLCLK:
                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                case WM_RBUTTONDBLCLK:
                case WM_MBUTTONDOWN:
                case WM_MBUTTONUP:
                case WM_MBUTTONDBLCLK:
                case WM_KEYDOWN:
                case WM_KEYUP:
                    break;
            }

            try
            {
                //if (m.WParam == (IntPtr)16)   //Shift键的状态   
                //    if (((uint)m.LParam & 0x80000000) == 0) ShiftStat = true;
                //    else ShiftStat = false;

                //if (m.WParam == (IntPtr)17)   //Ctrl键的状态   
                //    if (((uint)m.LParam & 0x80000000) == 0) CtrlStat = true;
                //    else CtrlStat = false;

                //if (m.WParam == (IntPtr)18)   //Alt键的状态   
                //    if (((uint)m.LParam & 0x80000000) == 0) AltStat = true;
                //    else AltStat = false;

                //if (ShiftStat & CtrlStat & AltStat)
                //    MessageBox.Show("Shift+Ctrl+Alt");

            }
            catch
            {
            }
            return false;
        }
        #endregion 
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        public static Main MainWindow { get { return (Main)Utils.Global.Windows.MainWindow; } }

        #region 变量
        ToolsWindow toolsWnd = new ToolsWindow();
        #endregion

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        #region 业务逻辑
        private void PlaceToolsWindow()
        {
            toolsWnd.Show(this);
            int x = this.DesktopBounds.Right - toolsWnd.Width - 20;
            int y = this.DesktopBounds.Top + 40;
            toolsWnd.Location = new Point(x, y);
        }

        private void OnLoad()
        {
            
            ToggleFullScreen();
            PlaceToolsWindow();
//             EagleEye.Me.AddLayer(new Models.Partition("ED"), new Models.Elevation(611.8f));
//             EagleEye.Me.AddLayer(new Models.Partition("RD1"), new Models.Elevation(583.7f));
//             EagleEye.Me.AddLayer(new Models.Partition("RD3"), new Models.Elevation(639.9f));
            EagleEye.Me.Show(this);
        }
        private bool IsFullScreen
        {
            get 
            {
                return this.FormBorderStyle == FormBorderStyle.None;
            }
        }
        private void ToggleFullScreen()
        {
            if(  IsFullScreen )
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
        }
        
        private void Exit()
        {
#if !DEBUG
            if(Utils.MB.OKCancelQ("您确定要退出系统吗？"))
#endif
            this.Close();
        }
        public bool CloseWnd(Type t, Views.LayerView view)
        {
            if( t != typeof(DMControl.LayerControl) )
            {
                return false;
            }
            if (!Utils.MB.OKCancelQ("您确定要关闭窗口吗？" + "\n\n" + view.Layer.Name))
                return false;
            tab.CloseWindow(view);
            return true;
        }
        private void ToggleToolbar()
        {
            toolsWnd.Visible = !toolsWnd.Visible;
        }
        /// <summary>
        /// 处理键盘消息，接受来自其他窗口的键盘消息
        /// </summary>
        /// <param name="e">按下的键</param>
        /// <returns>如果接受按键处理返回true（"抢夺"），否则返回false（不"抢夺"）</returns>
        public bool ProcessKeys(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            switch (e.KeyCode)
            {
                case Keys.P:
                    toolsWnd.FitScreen();
                    break;
                case Keys.H:
                    toolsWnd.ResetRotate();
                    break;
                case Keys.M:
                    toolsWnd.Operation = Views.Operations.SCROLL_FREE;
                    break;
                case Keys.X:
                    toolsWnd.Operation = Views.Operations.ROTATE;
                    break;
                case Keys.F:
                    toolsWnd.Operation = Views.Operations.ZOOM;
                    break;
                case Keys.W:
                    if( e.Control )
                    {
                        if( tab.SelectedItem != null )
                            DMControl.LayerControl.Instance.CloseLayer((Views.LayerView)tab.SelectedItem.Tag);
                        return true;
                    }
                    break;
                case Keys.Escape:
                    System.Diagnostics.Debug.Print(sender.ToString());
                    Exit();
                    return true;
                case Keys.F11:
                    ToggleFullScreen();
                    return true;
                case Keys.G:
                    ToggleToolbar();
                    return true;
                case Keys.Tab:
                    if( e.Control )
                    {
                        if( e.Shift )
                        {
                            tab.PrevItem();  // CTRL+SHIFT+TAB
                            return true;
                        }
                        else
                        {
                            tab.NextItem(); // CTRL+TAB
                            return true;
                        }
                    }
                    else
                    {
                    }
                    break;
                case Keys.F1:
                    toolsWnd.Visible = !toolsWnd.Visible;
                    EagleEye.Me.Visible = toolsWnd.Visible;
                    if (toolsWnd.CurrentLayer != null)
                        toolsWnd.CurrentLayer.ShowLandscape(toolsWnd.Visible);
                    System.Diagnostics.Debug.Print("F1 pressed");
                    return true;
                case Keys.F12:
                    //Warning dlg = new Warning();
                    //dlg.Show(this);
                    DMControl.GPSServer.test();
                    return true;
                default:
                    break;
            }
            e.SuppressKeyPress = false;
            return false;
        }
        public void GoLayer(Views.LayerView view)
        {
            FarsiLibrary.Win.FATabStripItem item = tab.Exist(view);
            if (item == null)
                return;
            tab.SelectedItem = item;
            DMControl.LayerControl.Instance.ChangeCurrentLayer(view);
        }
        public Views.LayerView OpenLayer(Models.PartitionDirectory p, Models.ElevationFile e)
        {
            //FarsiLibrary.Win.FATabStripItem item = tab.Exist(e.FullName);
//             if (item != null)
//             {
//                 if (Utils.MB.OKCancelQ("已经打开该层，要转到该层吗？"))
//                     tab.SelectedItem = item;
//                 return null;/*(Views.LayerView)item.Tag*/ ;
//             }
            FarsiLibrary.Win.FATabStripItem item;

            Views.LayerView lv = new DM.Views.LayerView();
            item = new FarsiLibrary.Win.FATabStripItem("New Layer", lv);
            tab.AddTab(item);
            lv.Dock = DockStyle.Fill;
            lv.Padding = new Padding(0, 0, 0, 0);
            lv.Margin = new Padding(0, 0, 0, 0);
            lv.BackColor = Color.White;
            
            if( !lv.OpenLayer(p, e) )
            {
                tab.RemoveTab(item);
                Utils.MB.Warning("打开层失败！");
                return null;
            }
            lv.Visible = true;
            item.Title = lv.Text;
            item.Tag = lv;
            lv.Init();

            tab.SelectedItem = item;

            return lv;
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        #region 事件响应
        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }
        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeys(sender, e);
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            OnLoad();
        }
        private void tab_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        private void tab_TabCloseButtonClicked(object sender, EventArgs e)
        {
            Exit();
        }
        private void tab_TabStripItemDBClicked(object sender, EventArgs e)
        {
            if( sender != null )
            {
                FarsiLibrary.Win.FATabStripItem item = (FarsiLibrary.Win.FATabStripItem)sender;
                //CloseWnd(item);
                DMControl.LayerControl.Instance.CloseLayer((Views.LayerView)item.Tag);
            }
            else
            {
                    ToggleFullScreen();
            }
        }

        private void tab_TabStripItemClosing(FarsiLibrary.Win.TabStripItemClosingEventArgs e)
        {
//             Exit();
//             bool result = false;
//             CloseWnd(e.Item.Title, out result);
//             e.Cancel = result;
        }
        private void Main_Deactivate(object sender, EventArgs e)
        {
        }
        private void Main_Leave(object sender, EventArgs e)
        {
            if (IsFullScreen)
                ToggleFullScreen();
        }
        private void tab_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeys(sender, e);
        }
        private void tab_TabStripItemSelectionChanged(FarsiLibrary.Win.TabStripItemChangedEventArgs e)
        {
            DMControl.LayerControl.Instance.ChangeCurrentLayer((Views.LayerView)e.Item.Tag);
        }

        #endregion

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            //DM.DMControl.GPSReceiver.socket.Shutdown(SocketShutdown.Both);
            //DM.DMControl.GPSReceiver.socket.Close();
            Program.Exiting = true;
        }
        private void internalShowWarningDlg(Warning dlg)
        {
            dlg.Show(this);
        }
        public delegate void InvokeDelegate(Forms.Warning w);
        public void ShowWarningDlg(Warning dlg)
        {
            this.BeginInvoke(new InvokeDelegate(internalShowWarningDlg), dlg);
        }
    }
}
