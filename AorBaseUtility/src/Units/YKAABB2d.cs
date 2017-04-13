using System;

namespace AorBaseUtility
{
    public struct YKAABB2d
    {
        public YKVector2d Max;
        public YKVector2d Min;

        public YKAABB2d(YKVector2d min, YKVector2d max)
        {
            Max = max;
            Min = min;
        }

        public YKAABB2d Reset()
        {
            Max.Set(double.NegativeInfinity, double.NegativeInfinity);
            Min.Set(double.PositiveInfinity, double.PositiveInfinity);

            return this;
        }

        public YKAABB2d AddPoint(YKVector2d v)
        {
            Max.Set(Math.Max(Max.x, v.x), Math.Max(Max.y, v.y));
            Min.Set(Math.Min(Min.x, v.x), Math.Min(Min.y, v.y));
            return this;
        }

        public YKVector2d Center
        {
            get { return (Max + Min) / 2; }
        }

    }
}
