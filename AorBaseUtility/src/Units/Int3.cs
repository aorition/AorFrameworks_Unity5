using System;

namespace AorBaseUtility
{
    [Serializable]
    public struct Int3
    {

        public static Int3 zero
        {
            get { return new Int3(0, 0, 0); }
        }

        public bool isInit;

        public int u;
        public int v;
        public int w;

        public Int3(Int2 int2)
        {
            this.u = int2.u;
            this.v = int2.v;
            this.w = 0;
            isInit = true;
        }

        public Int3(Int4 int4)
        {
            this.u = int4.x;
            this.v = int4.y;
            this.w = int4.z;
            isInit = true;
        }

        public Int3(int u, int v)
        {
            this.u = u;
            this.v = v;
            this.w = 0;
            isInit = true;
        }

        public Int3(int u, int v, int w)
        {
            this.u = u;
            this.v = v;
            this.w = w;
            isInit = true;
        }

        public override bool Equals(object obj)
        {
            Int3 i3 = (Int3)obj;
            return i3.u == this.u && i3.v == this.v && i3.w == this.w;
        }

        public override string ToString()
        {
            return "Int3[" + u + "," + v + "," + w + "]";
        }

        public string ToShortString()
        {
            return u + "_" + v + "_" + w;
        }

    }
}
