using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DMControl
{
    public class LayerControl
    {
        static LayerControl ctl = new LayerControl();

        public static LayerControl Instance
        {
            get { return LayerControl.ctl; }
            //set { LayerControl.ctl = value; }
        }
        public event EventHandler OnWorkingLayersChange;

        List<DB.Segment> workingLayers = new List<DM.DB.Segment>();

        public List<DB.Segment> WorkingLayers
        {
            get { return workingLayers; }
            //set { workingLayers = value; }
        }

        public void LoadWorkingLayer()
        {
            WorkingLayers.Clear();
            List<DB.CarDistribute> lst = DB.CarDistributeDAO.getInstance().getInusedCarDistributes();
            if (lst == null)
                return;
            DB.SegmentDAO dao = DB.SegmentDAO.getInstance();
            foreach (DB.CarDistribute cd in lst)
            {
                try
                {
                    List<DB.Segment> working = dao.getSegment(cd.Blockid, cd.DesignZ, cd.Segmentid);
                    if (working != null)
                        WorkingLayers.AddRange(working);
                }
                catch
                {
                    continue;
                }
            }
            if (null != OnWorkingLayersChange)
                OnWorkingLayersChange.Invoke(null, null);
        }

        // 已经打开视图的层列表
        List<Views.LayerView> layerviews = new List<DM.Views.LayerView>();

        #region - 查找 层、视图 -
        public Models.Layer FindLayerByPE(Models.Partition partition, Models.Elevation elevation)
        {
            Views.LayerView view = FindViewByPE(partition, elevation);
            if (view == null)
                return null;
            return view.Layer;
        }
        public Models.Layer FindLayerByPE(string partition, float elevation)
        {
            Views.LayerView view = FindViewByPE(partition, elevation);
            if (view == null)
                return null;
            return view.Layer;
        }
        public Views.LayerView FindViewByPE(Models.Partition partition, Models.Elevation elevation)
        {
            return FindViewByPE(partition.Name, elevation.Height);
        }
        public Views.LayerView FindViewByPE(string partition, double elevation)
        {
            foreach (Views.LayerView view in layerviews)
            {
                if (view.Layer.Partition.Equals(partition) &&
                    view.Layer.Elevation.Height == elevation)
                    return view;
            }
            return null;
        }
        // 根据名字查找层，name=Layer.ToString();
        public Models.Layer FindLayerByName(string name)
        {
            Views.LayerView layer = FindViewByName(name);
            if (layer == null)
                return null;
            return layer.Layer;
        }
        // 根据名字查找层视图，name=Layer.ToString();
        public Views.LayerView FindViewByName(string name)
        {
            foreach (Views.LayerView view in layerviews)
            {
                if (view.Layer.IsEqual(name))
                    return view;
            }
            return null;
        }
        // 根据完整路径名查找层
        private Views.LayerView FindView(string fullpath)
        {
            foreach (Views.LayerView view in layerviews)
            {
                if (view.IsEqual(fullpath))
                    return view;
            }
            return null;
        }
        #endregion
        // 返回true：成功

        // 返回false：已打开该层
        private Views.LayerView current = null;
        public Views.LayerView OpenLayer(Models.PartitionDirectory p, Models.ElevationFile e)
        {
            if (p == null || e == null)
                return null;

            //Utils.MB.OK(p.Name + " " + e.Height);

            Views.LayerView view = FindView(e.FullName);
            if (view != null)
            {
                if (Utils.MB.OKCancelQ("已经打开该层，要转到该层吗？"))
                    Forms.Main.MainWindow.GoLayer(view);
                return null;
            }

            view = Forms.Main.MainWindow.OpenLayer(p, e);
            if (view == null)
                return null;

            layerviews.Add(view);
            if (current != null)
                current.OnLostTab();
            current = view;
            view.OnActiveTab();
            return view;
        }
        // 关闭层

        public void CloseLayer(Views.LayerView view)
        {
            if (view == null)
                return;
            if (layerviews.IndexOf(view) == -1)
                return;

            if (Forms.Main.MainWindow.CloseWnd(typeof(LayerControl), view))
            {
                layerviews.Remove(view);
                view.Dispose();
            }
        }
        public void ChangeCurrentLayer(Views.LayerView view)
        {
            if (view == null)
                return;
            if (current != null && current != view)
            {
                current.OnLostTab();
            }
            current = view;
            current.OnActiveTab();
        }

        public DB.Segment FindDeck(int carid)
        {
            try
            {
                DB.CarDistribute cd = VehicleControl.FindVehicleInUse(carid);
                Models.Partition part = PartitionControl.FromID(cd.Blockid);
                Models.Elevation elev = new DM.Models.Elevation(cd.DesignZ);

                List<DB.Segment> seg = DB.SegmentDAO.getInstance().getSegment(cd.Blockid, cd.DesignZ, cd.Segmentid);
                if (seg.Count == 0)
                    return null;
                return seg.First();
                //Models.Layer layer = this.FindLayerByPE(part, elev);
                //return layer.DeckControl.FindDeckByIndex(cd.Segmentid);
            }
            catch
            {
                return null;
            }
        }
    }
}
