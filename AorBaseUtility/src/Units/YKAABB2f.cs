using System;

namespace AorBaseUtility
{
    public struct YKAABB2f
    {
        public YKVector2f Max;
        public YKVector2f Min;

        public YKAABB2f(YKVector2f min, YKVector2f max)
        {
            Max = max;
            Min = min;
        }

        public YKAABB2f Reset()
        {
            Max.Set(float.NegativeInfinity, float.NegativeInfinity);
            Min.Set(float.PositiveInfinity, float.PositiveInfinity);

            return this;
        }

        public YKAABB2f AddPoint(YKVector2f v)
        {
            Max.Set(Math.Max(Max.x, v.x), Math.Max(Max.y, v.y));
            Min.Set(Math.Min(Min.x, v.x), Math.Min(Min.y, v.y));
            return this;
        }

        public YKVector2f Center
        {
            get { return (Max + Min) / 2f; }
        }

    }
}
