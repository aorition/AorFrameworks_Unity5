using System;
using System.Collections.Generic;

namespace AorFramework.AorTile3D.runtime
{
    public class AorTile3DScene
    {

        #region 接口集合

        /// <summary>
        /// 当边框发生改变时触发
        /// </summary>
        public Action<AorTile3DScene> onBorderChange;

        /// <summary>
        /// 当地图数据发生改变时触发
        /// </summary>
        public Action<TileMapData> onMapDataChange;
        private void _onDataChange(TileMapData data)
        {
            if (onMapDataChange != null) onMapDataChange(data);
        }

        #endregion

        /// <summary>
        /// AorTile3D场景
        /// </summary>
        public AorTile3DScene (TileMapData mapData, int[] borderCenter, int[] borderHalfSize)
        {
            _mapData = mapData;
            _mapData.onDataChange += _onDataChange;
            AorTile3DUtils.Int3Set(borderHalfSize, _borderHalfSize);
            SetBorderCenter(borderCenter);
        }

        ~AorTile3DScene()
        {
            if (_mapData != null)
            {
                _mapData.onDataChange -= _onDataChange;
                _mapData = null;
            }
        }

        private bool _isDirty = false;
        private bool _isBorderDirty = false;

        public void Update()
        {
            if (_isDirty)
            {
                if (_isBorderDirty)
                {
                    //触发Border改变事件
                    if (onBorderChange != null) onBorderChange(this);
                    _isBorderDirty = false;
                }
                _isDirty = false;
            }
        }

        private TileMapData _mapData;
        public TileMapData mapData
        {
            get { return _mapData; }
        }

        public void Dispose()
        {
            if (_mapData != null)
            {
                _mapData.onDataChange = null;
                _mapData = null;
            }
            //接口注销
            onMapDataChange = null;
            onBorderChange = null;
        }

        private readonly int[] _borderCenter = {-1, -1, -1};
        public int[] borderCenter
        {
            get { return _borderCenter; }
        }

        public void SetBorderCenter(int[] int3)
        {
            SetBorderCenter(int3[0], int3[1], int3[2]);
        }
        public void SetBorderCenter(int x, int y, int z)
        {
            if (_borderCenter[0] == x
                && _borderCenter[1] == y
                && _borderCenter[2] == z) return;

            //边界判定逻辑 (有计算误差问题)
            x = _mapData.tileDataLength[0] < _borderHalfSize[0] * 2 + 1 ? _mapData.tileDataLength[0] / 2 : x - _borderHalfSize[0] < 0 ? _borderHalfSize[0] : x + _borderHalfSize[0] > _mapData.tileDataLength[0] ? _mapData.tileDataLength[0] - _borderHalfSize[0] : x;
            y = _mapData.tileDataLength[1] < _borderHalfSize[1] * 2 + 1 ? _mapData.tileDataLength[1] / 2 : y - _borderHalfSize[1] < 0 ? _borderHalfSize[1] : y + _borderHalfSize[1] > _mapData.tileDataLength[1] ? _mapData.tileDataLength[1] - _borderHalfSize[1] : y;
            // z = _mapData.tileDataLength[2] < _borderHalfSize[2] * 2 + 1 ? _mapData.tileDataLength[2] / 2 : z - _borderHalfSize[2] < 0 ? _borderHalfSize[2] : z + _borderHalfSize[2] > _mapData.tileDataLength[2] ? _mapData.tileDataLength[2] - _borderHalfSize[2] - 1 : z;

            //边界检查后再判定一次是否相等
            if (_borderCenter[0] == x
                && _borderCenter[1] == y
                && _borderCenter[2] == z) return;

            _borderCenter[0] = x;
            _borderCenter[1] = y;
            _borderCenter[2] = z;
            _isDirty = true;
            _isBorderDirty = true;
        }

        private readonly int[] _borderHalfSize = {-1,-1,-1};
        public int[] borderHalfSize
        {
            get { return _borderHalfSize; }
        }

        public void setborderSize(int x, int y, int z)
        {

            if (_borderHalfSize[0] == x
                && _borderHalfSize[1] == y
                && _borderHalfSize[2] == z) return;

            _borderHalfSize[0] = x;
            _borderHalfSize[1] = y;
            _borderHalfSize[2] = z;
            _isDirty = true;
            _isBorderDirty = true;
        }

        public int[] borderMin
        {
            get
            {
                return new int[]
                {
                    _borderCenter[0] - _borderHalfSize[0],
                    _borderCenter[1] - _borderHalfSize[1],
                    _borderCenter[2] - _borderHalfSize[2]
                };
            }
        }

        public int[] borderMax
        {
            get
            {
                return new int[]
                {
                    _borderCenter[0] + _borderHalfSize[0],
                    _borderCenter[1] + _borderHalfSize[1],
                    _borderCenter[2] + _borderHalfSize[2]
                };
            }
        }

    }
}
