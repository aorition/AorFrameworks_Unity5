using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AorFramework.AorTile3D.runtime
{

    public enum TileOrientation
    {
        X,
        negativeX,
        Y,
        negativeY,
        Z,
        negativeZ,
    }

    /// <summary>
    /// 图块
    /// </summary>
    public class TileData
    {

        //模型id
        private ulong _modelId;
        public ulong modelId
        {
            get { return _modelId;}
        }

        private int _height;
        public int height
        {
            get { return _height; }
        }

        //朝向
        private TileOrientation _orient;
        public TileOrientation orient
        {
            get { return _orient; }
        }

        //附加物id
        private ulong[] _addOnesId;
        public ulong[] addOnesId
        {
            get { return _addOnesId; }
        }
        //

        //是否碰撞 ?

        public TileData(ulong modelId, TileOrientation orient, ulong[] addOnesId,int height = 0)
        {
            this._modelId = modelId;
            this._orient = orient;
            this._addOnesId = addOnesId;
            this._height = height;
        }

    }
}
