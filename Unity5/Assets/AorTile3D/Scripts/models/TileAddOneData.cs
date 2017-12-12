using System;
using System.Collections.Generic;

namespace AorFramework.AorTile3D
{
    //描述tile附加物
    public class TileAddOneData
    {

        public TileAddOneData(ulong modelId, float[] scale, float[] offset, float[] rotate)
        {
            this._modelId = modelId;
            this._scale = scale;
            this._offset = offset;
            this._rotate = rotate;
        }

        private ulong _modelId;
        public ulong modelId {
            get { return _modelId; }
        }

        private float[] _scale;
        public float[] scale
        {
            get { return _scale; }
        }
        private float[] _offset;
        public float[] offset
        {
            get { return _offset; }
        }
        private float[] _rotate;
        public float[] rotate
        {
            get { return _rotate; }
        }

    }
}
