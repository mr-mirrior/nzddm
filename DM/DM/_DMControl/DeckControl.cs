using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DM.Models;
using System.Windows.Forms;

namespace DM.DMControl
{
    public class DeckControl : IDisposable
    {
        public DeckControl(Layer _owner) { owner = _owner; if (owner != null) { partition = owner.Partition; elevation = owner.Elevation; } }
        List<Deck> decks = new List<Deck>();
        Layer owner = null;

        Partition partition;
        Elevation elevation;

        public Layer Owner
        {
            get { return owner; }
            set { owner = value; partition = owner.Partition; elevation = owner.Elevation; }
        }

        public List<Deck> Decks
        {
            get { return decks; }
            set { decks = value; }
        }

        public void Dispose()
        {
            foreach (Deck dk in decks)
            {
                dk.Dispose();
            }
            GC.SuppressFinalize(this);
        }
        private Deck FindDeckByName(string name)
        {
            foreach (Deck dk in decks)
            {
                if (dk.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return dk;
            }
            return null;
        }
        public Deck FindDeckByIndex(int id)
        {
            foreach (Deck dk in decks)
            {
                if (dk.ID == id)
                    return dk;
            }
            return null;
        }
        /*int lastID = 0;*/
        private Deck FindDeck(Deck deck)
        {
            return FindDeckByName(deck.Name);
        }
        private int FindFreeIndex()
        {
            int idx = 0;
            bool occupied = false;
            while(true)
            {
                occupied = false;
                for (int i = 0; i < decks.Count; i++)
                {
                    if (decks[i].ID == idx)
                    {
                        occupied = true;
                        break;
                    }
                }
                if (!occupied)
                    return idx;
                idx++;
            }
        }
        public void AddDeck(Deck deck)
        {
            int id = FindFreeIndex();
            deck.ID = id;

//             if( deck.Name == null )
//                 deck.Name = deck.ToString();

            //decks.SetVertex(deck);
            if (ModifyDeck(deck))
            {
                deck.VehicleControl.AssignVehicle(deck);
            }
            else
            {
                //Utils.MB.Warning("新仓面未添加");
                if (!Utils.MB.OKCancelQ("新仓面信息未更改，要保存吗？\n\n按<确定>保存，<取消>抛弃"))
                {
//                     if( decks.Count != 0 )
//                         decks.RemoveAt(decks.Count - 1);
                    return;
                }
                decks.Add(deck);
                return;
            }

            if( SubmitDB() )
            {
                Forms.ToolsWindow.I.UpdateMode();
                Utils.MB.OKI("添加仓面成功，已保存至数据库");
            }
        }
        public void DeleteDeck(Deck deck)
        {
            if (deck.IsWorking || deck.WorkState== DM.DB.SegmentWorkState.END)
            {
                Utils.MB.Warning("该仓面处于开仓状态或者已经工作完成，无法删除。");
                return;
            }

            if (!Utils.MB.OKCancelQ("确定删除该仓面吗？\n\n" + deck.Name))
                return;
            
            deck.VehicleControl.DeleteAll();

            decks.Remove(deck);
            if( SubmitDB() )
            {
                Forms.ToolsWindow.I.UpdateMode();
                Utils.MB.OKI("仓面已从数据库删除");
            }
        }
        private bool SubmitDB()
        {
            try
            {
                return DB.UpdateSegmentResult.SUCCESS == DB.SegmentDAO.getInstance().updateSegments(Translate(decks), partition.ID, elevation.Height);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToString());
                return false;
            }
            finally
            {
                VehicleControl.LoadCarDistribute();
            }
        }
        public void Clear()
        {
            foreach (Deck dk in decks)
            {
                dk.Dispose();
            }
            decks.Clear();
        }
        private int FindActive()
        {
            for (int i = 0; i < decks.Count; i++ )
            {
                if (decks[i].IsVisible)
                    return i;
            }
            return -1;
        }
        public void LoadDB(Deck old)
        {
            if (owner == null)
                return;

            List<DB.Segment> lst= DB.SegmentDAO.getInstance().getSegments(owner.Partition.ID, owner.Elevation.Height);
            if (lst == null )
            {
                return;
            }
            // TODO
            decks = Translate(lst);

            if (old == null)
                return;

            Deck dk = FindDeck(old);
            if( dk != null )
            {
                dk.DrawingComponent = old.DrawingComponent;
                if (old.IsVisible)
                    this.SetVisibleDeck(dk);
            }
        }
        public void LoadDBVehicle(Deck dk)
        {
//             for (int i = 0; i < decks.Count; i++)
//             {
//                 decks[i].Partition = decks[i].Partition;
//                 decks[i].VehicleControl.LoadDB();
//             }
            if (dk.IsVisible)
            {
                dk.Partition = dk.Partition;
                dk.VehicleControl.LoadDB();
            }
        }
        private List<Deck> Translate(List<DB.Segment> lst)
        {
            List<Deck> dks = new List<Deck>();
            foreach (DB.Segment seg in lst)
            {
                Deck dk = new Deck(seg);
                dk.Owner = this.owner;
                dk.NRs = NotRolling.ReadFromDB(dk);
                dks.Add(dk);
            }
            return dks;
        }
        private List<DB.Segment> Translate(List<Deck> lst)
        {
            List<DB.Segment> dks = new List<DM.DB.Segment>();
            foreach (Deck seg in lst)
            {
                dks.Add(seg.DeckInfo);
            }
            return dks;
        }

        /// <summary>
        /// 查看仓面车辆分配历史
        /// </summary>
        /// <param name="deck">要查看的仓面</param>
        /// <returns></returns>
        public bool LookVehicleHistory(Deck deck)
        {
            DM.Forms.VehicleHistory dlg = new DM.Forms.VehicleHistory();
            dlg.Deck = deck;
            dlg.ShowDialog();
            return true;
        }



        public bool ModifyDeck(Deck deck)
        {
//             DB.SegmentDAO dao = DB.SegmentDAO.getInstance();
//             List<DB.Segment> lst = dao.getSegments(deck.Partition.ID, deck.Elevation.Height);
//             if (lst == null)
//                 lst = new List<DM.DB.Segment>();
            if (decks == null)
                decks = new List<Deck>();
            Partition p = DMControl.PartitionControl.FromName(deck.Partition.Name);
            // 不存在该分区，返回

            if (p == null)
            {
                //throw new ArgumentException("错误：仓面为空");
                return false;
            }

            int idx = -1;
            Deck dkFound = null;
            foreach (Deck dk in decks)
            {
                idx++;
                if (dk.IsEqual(deck))
                {
                    dkFound = dk;
                    break;
                }
            }

            Forms.DeckInfo dlg = new DM.Forms.DeckInfo();

            // 如果是新增仓面

            if (dkFound == null || idx == -1)
            {
                dlg.Deck = deck.DeckInfo;
                dlg.IsWorking = false;
            }
            else
            {
                dlg.Deck = decks[idx].DeckInfo;
                dlg.IsWorking = (dkFound.IsWorking || dkFound.WorkState == DM.DB.SegmentWorkState.END);
            }
            dlg.BlockName = partition.Name;

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                //Utils.MB.OKI("操作取消");
                return false;
            }

            Deck tobemodified = new Deck(dlg.Deck);
            tobemodified.Owner = this.Owner;
            tobemodified.Polygon = deck.Polygon;

            if (dkFound == null || idx==-1)
                decks.Add(tobemodified);
            else
                decks[idx] = tobemodified;

            //DB.SegmentDAO.getInstance().updateSegments(Translate(decks), deck.Partition.ID, deck.Elevation.Height);
            if (SubmitDB())
            {
            }
            else
            {
                Utils.MB.Warning("修改仓面信息失败！");
                return false;
            }

            LoadDB(deck);
            UpdateGraphics();

            Utils.MB.OKI("修改仓面信息成功");

            return true;
        }

        // 计算该仓面某点的碾压次数，按照屏幕坐标

        public int RollCount(PointF pt)
        {
            int count = 0;
            foreach (Deck dk in decks)
            {
                count += dk.RollCount(pt);
            }
            return count;
        }

        private void ThicknessMonitor(Deck dk)
        {
            DM.Forms.OverBlock ob = new DM.Forms.OverBlock();
            List<DB.Segment> lst = DB.SegmentDAO.getInstance().findBeneathSegments(dk.DeckInfo.BlockID, dk.DeckInfo.DesignZ);
            if( lst.Count == 0 )
            {
                Utils.MB.Warning("无法监控厚度：找不到厚度依据！");
                return;
            }
            ob.BlockName = dk.Partition.Description + " (" + dk.Partition.Name + ")";
            ob.DeckInfo = string.Format("{0} ({1}，仓面{2})", dk.Name, dk.Owner.Name, dk.ID);
            ob.PartitionList = lst;
            if (ob.ShowDialog() != DialogResult.OK)
            {
                dk.Thickness = -1;
                return;
            }
            Deck dkfrom = new Deck();
            dkfrom.DeckInfo.BlockID = dk.DeckInfo.BlockID;
            dkfrom.DeckInfo.DesignZ = ob.Elevation;
            dkfrom.DeckInfo.SegmentID = ob.DeckID;

            dk.Thickness = dkfrom.AverageHeightFromDataMap();

            Utils.MB.OKI("经计算，所选依据仓面的平均高程为：" + dk.Thickness.ToString("0.00")+"米");
        }
        // 开仓
        public bool Start(Deck dk)
        {
            if (null == FindDeck(dk))
            {
                Utils.MB.Warning("该仓面不存在");
                return false;
            }
            if( !dk.IsVisible )
            {
                Utils.MB.Warning("该仓面现在不是可见仓面，无法开仓。请设置为可见仓面再试一次。");
                return false;
            }
            if (!Utils.MB.OKCancelQ("您确认要开仓吗？\n仓面信息：" + dk.Name))
                return false;

//             ThicknessMonitor(dk);

            // 0) 检查操作权限

            // 1) 仓面工作状态检查

            if (dk.IsWorking)
            {
                Utils.MB.Warning("该仓面已经在工作中，请关仓后再试一次");
                return false;
            }
            // 2) 检查车辆安排情况

            DB.CarDistributeDAO daoCar = DB.CarDistributeDAO.getInstance();
            List<DB.CarInfo> info = daoCar.getInusedCars();
            List<DB.CarDistribute> dist = daoCar.getCarDistributeInThisSegment_all(partition.ID, elevation.Height, dk.ID);
            int distCount = 0;
            foreach (DB.CarDistribute cd in dist)
            {
                if (cd.IsFinished())
                    continue;
                distCount++;
                foreach (DB.CarInfo inf in info)
                {
                    if( cd.Carid == inf.CarID)
                    {
                        Utils.MB.Warning("开仓失败：车辆已被占用：\""+ inf.CarName+"\"");
                        return false;
                    }
                }
            }
            if (distCount == 0)
            {
                Utils.MB.Warning("开仓失败：尚未安排任何车辆");
                return false;
            }

            // 3) 更新数据库仓面项中的起始时间
            // 4) 更新车辆安排表项中的起始、结束时间

            DB.SegmentDAO daoSeg = DB.SegmentDAO.getInstance();
            try
            {
                DM.DB.SegmentVehicleResult result = daoSeg.startSegment(partition.ID, elevation.Height, dk.ID, dk.DeckInfo.MaxSpeed,dk.WorkState);
                if( result == DM.DB.SegmentVehicleResult.CARS_FAIL )
                    Utils.MB.Warning("开仓失败：车辆错误");
                if (result == DM.DB.SegmentVehicleResult.SEGMENT_FAIL)
                    Utils.MB.Warning("开仓失败：仓面错误");
                if (result != DM.DB.SegmentVehicleResult.SUCCESS)
                    return false;
            }
            catch
            {
                return false;
            }

            VehicleControl.LoadCarDistribute();
            DMControl.LayerControl.Instance.LoadWorkingLayer();
            LoadDB(dk);
            UpdateGraphics();
            GPSServer.OpenDeck();
            Utils.MB.OKI("\""+ dk.Name +"\""+ "已经开仓！");

            return true;
        }

        // 关仓
        public bool Stop(Deck dk)
        {
            if( null == FindDeck(dk))
            {
                Utils.MB.Warning("该仓面不存在");
                return false;
            }
            if (!dk.IsVisible)
            {
                Utils.MB.Warning("该仓面现在不是可见仓面，无法关仓。请设置为可见仓面再试一次。");
                return false;
            }
            if (!Utils.MB.OKCancelQ("您确认要关仓吗？\n仓面信息：" + dk.Name))
                return false;

            // 0) 检查操作权限

            // 1) 仓面工作状态检查

            if (!dk.IsWorking)
            {
                Utils.MB.Warning("该仓面未开仓，请开仓后再试一次");
                return false;
            }
            // 2) 检查车辆安排情况

            // 3) 更新数据库仓面项中的起始时间
            // 4) 更新车辆安排表项中的起始、结束时间

            DB.SegmentDAO dao = DB.SegmentDAO.getInstance();

            try
            {
                DM.DB.SegmentVehicleResult result = dao.endSegment(partition.ID, elevation.Height, dk.ID);
                if (result == DM.DB.SegmentVehicleResult.CARS_FAIL)
                    Utils.MB.Warning("开仓失败：车辆错误");
                if (result == DM.DB.SegmentVehicleResult.SEGMENT_FAIL)
                    Utils.MB.Warning("开仓失败：仓面错误");
                if (result!= DM.DB.SegmentVehicleResult.SUCCESS)
                    return false;
            }
            catch
            {
                return false;
            }

            isStoppingDeck = true;
            dk.VehicleControl.Clear();
            DMControl.LayerControl.Instance.LoadWorkingLayer();
            VehicleControl.LoadCarDistribute();
            LoadDB(dk);
            //UpdateGraphics();
            GPSServer.CloseDeck();

            //if (CreateDataMap(dk))
            //{
            //    //Utils.MB.OK("数据图更新成功！");
            //}
            //else
            //{
            //    //Utils.MB.OK("数据图更新失败！");
            //}


            //ReportRolling(dk);

            Utils.MB.OKI("\"" + dk.Name + "\"" + "关仓完毕！");
            return true;
        }
        volatile bool isStoppingDeck = false;
        private void ReportRolling(Deck dk)
        {
            if (!isStoppingDeck)
                return;
            Forms.Warning warndlg = new Forms.Warning();
            isStoppingDeck = false;

            int[] areas = null;
            Bitmap bmp = dk.CreateRollCountImage(out areas);
            bmp.Dispose();

            int expected = dk.DeckInfo.DesignRollCount;
            double[] area_ratio = Models.Deck.AreaRatio(areas, dk);
            double ok = 0;
            double nok = 0;
            if (area_ratio == null || area_ratio.Length == 0)
            {
                Utils.MB.Warning("标准遍数百分比计算异常！");
            }
            else
            {
                ok = area_ratio[expected];
                for (int i = 0; i < expected; i++)
                {
                    nok += area_ratio[i];
                }
            }

            
            if (area_ratio!=null && area_ratio.Length != 0)
                dk.DeckInfo.POP = ok;
            else
                dk.DeckInfo.POP = -1;
            string warning = string.Format("碾压简报：{0}-{1}米 仓面：{2}，碾压标准：{3}遍，碾压合格：{4:P}",
                    dk.DeckInfo.BlockName,
                    dk.DeckInfo.DesignZ, 
                    dk.DeckInfo.SegmentName,
                    dk.DeckInfo.DesignRollCount,
                    dk.DeckInfo.POP);
            DB.SegmentDAO.getInstance().setSegmentPOP(dk.Partition.ID, dk.Elevation.Height, dk.ID, dk.DeckInfo.POP);

            DMControl.WarningControl.SendMessage(WarningType.ROLLINGLESS,dk.DeckInfo.BlockID,warning);
            
            dk.UpdateName();
            warndlg.BlockName = dk.DeckInfo.BlockName;
            warndlg.DeckName = dk.DeckInfo.SegmentName;
            warndlg.DesignZ = dk.DeckInfo.DesignZ;
            warndlg.ShortRollerArea = nok;
            warndlg.TotalAreaRatio = nok + ok;
            warndlg.ActualArea = dk.Polygon.ActualArea;
            warndlg.WarningDate = DB.DBCommon.getDate().Date.ToString("D");
            warndlg.WarningTime = DB.DBCommon.getDate().ToString("T");
            warndlg.WarningType = WarningType.ROLLINGLESS;
            warndlg.FillForms();
            Forms.Main.MainWindow.ShowWarningDlg(warndlg);
        }
        private void UpdateGraphics()
        {
            Owner.Owner.UpdateGraphics();
        }
        public void UnvisibleDeck(Deck dk)
        {
            if (dk == null)
                return;
            dk.IsVisible = false;
            dk.VehicleControl.Dispose();
        }
        private void thrdSetVisibleDeck()
        {
            if (tobesetvisible == null)
                return;
            foreach (Deck dk in decks)
            {
                if (dk.IsEqual(tobesetvisible))
                {
                    dk.IsVisible = true;
                    LoadDBVehicle(dk);
                }
                else
                {
                    UnvisibleDeck(dk);
                }
            }
            TrackGPS.SetTime = DB.DBCommon.getDate()-TimeSpan.FromSeconds(2);
            TrackGPS.hasReadCar.Clear();
            TrackGPS.alltimes.Clear();
            UpdateGraphics();
            ReportRolling(tobesetvisible);
            tobesetvisible = null;

        }
        Deck tobesetvisible = null;
        Forms.Waiting dlg;
        public void SetVisibleDeck(Deck deck)
        {
            tobesetvisible = deck;
            dlg =  new DM.Forms.Waiting();
            dlg.Start(Forms.Main.MainWindow, "正在计算轨迹，请稍候……", thrdSetVisibleDeck, 1000);
        }
        public Deck GetVisibleDeck()
        {
            foreach (Deck dk in decks)
            {
                if (dk.IsVisible)
                    return dk;
            }
            return null;
        }
        public bool CreateDataMap(Deck dk)
        {
            byte[] map = dk.CreateDatamap();
            if( map != null )
            {
                DB.SegmentDAO dao = DB.SegmentDAO.getInstance();
                if( dao.updateDatamap(dk.Partition.ID, dk.Elevation.Height, dk.ID, map) > 0 )
                {
                    return true;
                }
                else
                {
                    //Utils.MB.OK("数据图更新失败！");
                    return false;
                }
            }
            return false;
        }
    }
}
