using System;

namespace AorBaseUtility
{
    [Serializable]
    public struct Int4
    {

        public static Int4 zero
        {
            get { return new Int4(0, 0, 0, 0); }
        }

        public bool isInit;

        public int x;
        public int y;
        public int z;
        public int w;

        public Int4(Int2 int2)
        {
            this.x = int2.u;
            this.y = int2.v;
            this.z = 0;
            this.w = 0;
            isInit = true;
        }

        public Int4(Int3 int3)
        {
            this.x = int3.u;
            this.y = int3.v;
            this.z = int3.w;
            this.w = 0;
            isInit = true;
        }

        public Int4(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
            this.w = 0;
            isInit = true;
        }

        public Int4(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 0;
            isInit = true;
        }

        public Int4(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            isInit = true;
        }

        public override bool Equals(object obj)
        {
            Int4 i4 = (Int4)obj;
            return i4.x == this.x && i4.y == this.y && i4.z == this.z && i4.w == this.w;
        }

        public override string ToString()
        {
            return "Int4[" + x + "," + y + "," + z + "," + w + "]";
        }

        public string ToShortString()
        {
            return x + "_" + y + "_" + z + "_" + w;
        }

    }
}
