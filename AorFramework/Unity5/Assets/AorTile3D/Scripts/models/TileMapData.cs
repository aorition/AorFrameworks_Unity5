using System;
using System.Collections.Generic;

namespace AorFramework.AorTile3D.runtime
{

    public class TileMapData
    {

        #region 接口集合

        /// <summary>
        /// 当Data发生改变时触发
        /// </summary>
        public Action<TileMapData> onDataChange;

        #endregion

        public TileMapData(float tileSizeU, float tileSizeV, float tileSizeW, int uLength, int vLength)
        {
            _tileDataLength = new[] {uLength, vLength};
            _tileSize = new float[] {tileSizeU, tileSizeV, tileSizeW};
            _tileDatas = new TileData[uLength][];
            for (int u = 0; u < uLength; u++)
            {
                _tileDatas[u] = new TileData[vLength];
                for (int v = 0; v < vLength; v++)
                {
                    _tileDatas[u][v] = null;
                }
            }
            AorTile3DManager.ThrowError("TileMapData init finished !");
        }

        public void Dispose()
        {
            //接口注销
            onDataChange = null;
        }

        //定义tile的大小
        private float[] _tileSize;
        public float[] tileSize
        {
            get
            {
                return _tileSize;
            }
        }

        private int[] _tileDataLength;
        public int[] tileDataLength
        {
            get { return _tileDataLength; }
        }

        //tile数据
        private TileData[][] _tileDatas;

        ///TestCode
        public void setTileData(int u, int v)
        {
            TileData td = new TileData(0, TileOrientation.Z, null);
            _tileDatas[u][v] = td;
        }

        //附加物品数据
        private Dictionary<ulong, TileAddOneData> _addOneDatas = new Dictionary<ulong, TileAddOneData>();

        private Dictionary<ulong, string> _tileModelPath = new Dictionary<ulong, string>();
        private Dictionary<ulong, string> _addOneModelPath = new Dictionary<ulong, string>();

        /// <summary>
        /// 获取TileData
        /// </summary>
        public TileData GetTileData(int uIdx, int vIdx)
        {
            return _tileDatas[uIdx][vIdx];
        }

        /// <summary>
        /// 加入地形模型(路径)
        /// </summary>
        public ulong AddTileModePath(string path)
        {
            for (ulong j = 0; j < ulong.MaxValue - 1; j++)
            {
                if (!_tileModelPath.ContainsKey(j))
                {
                    _tileModelPath.Add(j, path);
                    _triggerChangeEvent();
                    return j;
                }
            }
            //不可能跑到的地方
            return ulong.MaxValue;
        }

        /// <summary>
        /// 获取地形模型(路径)
        /// </summary>
        public string GetTileModelPath(ulong id)
        {
            if (_tileModelPath.ContainsKey(id))
            {
                return _tileModelPath[id];
            }
            AorTile3DManager.ThrowError("TileMapData.GetTileMode Error :: [" + id + "] Can not Find TileModePath.");
            return null;
        }

        /// <summary>
        /// 移除地形模型(路径)
        /// </summary>
        public bool RemoveTileModelPath(ulong id)
        {
            if (_tileModelPath.ContainsKey(id))
            {
                bool rt = _tileModelPath.Remove(id);
                if (rt)
                {
                    _triggerChangeEvent();
                    return true;
                }

            }
            AorTile3DManager.ThrowError("TileMapData.GetTileMode Error :: [" + id + "] Can not Find TileModePath.");
            return false;
        }

        /// <summary>
        /// 加入附加模型(路径)
        /// </summary>
        public ulong AddAddOneModelPath(string path)
        {
            for (ulong j = 0; j < ulong.MaxValue - 1; j++)
            {
                if (!_addOneModelPath.ContainsKey(j))
                {
                    _addOneModelPath.Add(j, path);
                    _triggerChangeEvent();
                    return j;
                }
            }
            //不可能跑到的地方
            return ulong.MaxValue;
        }

        /// <summary>
        /// 获取附加模型(路径)
        /// </summary>
        public string GetAddOneModelPath(ulong id)
        {
            if (_tileModelPath.ContainsKey(id))
            {
                return _tileModelPath[id];
            }
            AorTile3DManager.ThrowError("TileMapData.GetAddOneModelPath Error :: [" + id + "] Can not Find AddOneModelPath.");
            return null;
        }

        /// <summary>
        /// 移除附加模型(路径)
        /// </summary>
        public bool RemoveAddOneModePath(ulong id)
        {
            if (_addOneModelPath.ContainsKey(id))
            {
                bool rt = _addOneModelPath.Remove(id);
                if (rt)
                {
                    _triggerChangeEvent();
                    return true;
                }
            }
            AorTile3DManager.ThrowError("TileMapData.GetAddOneModelPath Error :: [" + id + "] Can not Find AddOneModelPath.");
            return false;
        }

        private void _triggerChangeEvent()
        {
            if (onDataChange != null) onDataChange(this);
        }

    }
}
