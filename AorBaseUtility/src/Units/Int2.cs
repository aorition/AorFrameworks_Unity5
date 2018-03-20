using System;

namespace AorBaseUtility
{
    [Serializable]
    public struct Int2
    {

        public static Int2 zero
        {
            get { return new Int2(0, 0); }
        }

        public bool isInit;

        public int u;
        public int v;

        public Int2(int u, int v)
        {
            this.u = u;
            this.v = v;
            isInit = true;
        }

        public Int2(Int3 int3)
        {
            this.u = int3.u;
            this.v = int3.v;
            isInit = true;
        }

        public Int2(Int4 int4)
        {
            this.u = int4.x;
            this.v = int4.y;
            isInit = true;
        }

        public override bool Equals(object obj)
        {
            Int2 i2 = (Int2)obj;
            return i2.u == this.u && i2.v == this.v;
        }

        public override string ToString()
        {
            return "Int2[" + u + "," + v + "]";
        }

        public string ToShortString()
        {
            return u + "_" + v;
        }

    }
}
