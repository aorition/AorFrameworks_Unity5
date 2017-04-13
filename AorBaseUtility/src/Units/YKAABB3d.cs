using System;

namespace AorBaseUtility
{
    public struct YKAABB3d
    {
        public YKVector3d Max;
        public YKVector3d Min;

        public YKAABB3d(YKVector3d min, YKVector3d max)
        {
            Max = max;
            Min = min;
        }

        public YKAABB3d Reset()
        {
            Max.Set(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            Min.Set(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);

            return this;
        }

        public YKAABB3d AddPoint(YKVector3d v)
        {
            Max.Set(Math.Max(Max.x, v.x), Math.Max(Max.y, v.y), Math.Max(Max.z, v.z));
            Min.Set(Math.Min(Min.x, v.x), Math.Min(Min.y, v.y), Math.Min(Min.z, v.z));
            return this;
        }

        public YKVector3d Center
        {
            get { return (Max + Min) / 2; }
        }

    }
}
