using System;

namespace AorBaseUtility
{
    public static class YKCollision
    {
        public struct Sphere
        {
            public YKVector2f Center;
            public float Radius;

            public Sphere(YKVector2f center, float Radius)
            {
                this.Center = center;
                this.Radius = Radius;
            }
        }
    }
}
