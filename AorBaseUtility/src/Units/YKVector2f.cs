//-----------------------------------------------------------------------
//| by:Qcbf                                                             |
//-----------------------------------------------------------------------
using System;

namespace AorBaseUtility
{
    public struct YKVector2f
    {
        public const double kEpsilon = 1E-5d;
        public const double equalNum = 9.999999E-11d;

        public float x;
        public float y;

        public static YKVector2f Zero
        {
            get
            {
                return new YKVector2f(0, 0);
            }
        }

        public static YKVector2f One
        {
            get
            {
                return new YKVector2f(1, 1);
            }
        }

        public static YKVector2f Max
        {
            get
            {
                return new YKVector2f(float.MaxValue, float.MaxValue);
            }
        }

        public static YKVector2f UnitX
        {
            get
            {
                return new YKVector2f(1, 0);
            }
        }

        public static YKVector2f UnitY
        {
            get
            {
                return new YKVector2f(0, 1);
            }
        }

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }

        public float SqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }


        public YKVector2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public YKVector2f(YKVector2f vec2)
        {
            this.x = vec2.x;
            this.y = vec2.y;
        }

        public YKVector2f Normalized
        {
            get
            {

                YKVector2f res = new YKVector2f(this);
                float num = res.Magnitude;
                if (num <= 1e-05f)
                {
                    res.x = 0;
                    res.y = 0;
                }
                else
                {
                    res.x /= num;
                    res.y /= num;
                }

                return res;
            }
        }

        public YKVector2f Inversed
        {
            get
            {
                return new YKVector2f(-x, -y);
            }
        }

        public void Inverse()
        {
            x = -x;
            y = -y;
        }

        public void Normalize()
        {
            float num = Magnitude;
            if (num <= 1e-05f)
            {
                x = 0;
                y = 0;
            }
            else
            {
                x /= num;
                y /= num;
            }
        }

        public void Set(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public void Add(YKVector2f a)
        {
            x += a.x;
            y += a.y;
        }

        public void Add(float _x, float _y)
        {
            x += _x;
            y += _y;
        }

        public void Sub(YKVector2f a)
        {
            x -= a.x;
            y -= a.y;
        }
        public void Sub(float _x, float _y)
        {
            x -= _x;
            y -= _y;
        }

        public void Mul(float _x, float _y)
        {
            x *= _x;
            y *= _y;
        }

        public float Dot(YKVector2f a)
        {
            return x * a.x + y * a.y;
        }

        public void MoveForward(float angle, float speed)
        {
            x += speed * (float)Math.Cos(angle);
            y += speed * (float)Math.Sin(angle);
        }

        public float Distance(YKVector2f b)
        {
            return (float)Math.Sqrt(DistanceSquared(b));
        }

        public float DistanceSquared(YKVector2f b)
        {
            return (float)((x - b.x) * (x - b.x) + (y - b.y) * (y - b.y));
        }

        public float Angle(YKVector2f b)
        {
            return (float)Math.Atan2(b.y - this.y, b.x - this.x) * 180f / (float)Math.PI;
        }

        public override string ToString()
        {
            string dx = x.ToString("f4");
            string dy = y.ToString("f4");
            return string.Format("SimpleVector2({0}, {1})", dx, dy);
        }


        public static YKVector2f Lerp(YKVector2f from, YKVector2f to, float t)
        {
            return new YKVector2f(from.x + ((to.x - from.x) * t), from.y + ((to.y - from.y) * t));
        }


        public static float Distance(YKVector2f a, YKVector2f b)
        {
            return a.Distance(b);
        }

        public static float Angle(YKVector2f from, YKVector2f to)
        {
            return from.Angle(to);
        }

        public static YKVector2f operator +(YKVector2f a, YKVector2f b)
        {
            return new YKVector2f(a.x + b.x, a.y + b.y);
        }
        public static YKVector2f operator +(YKVector2f a, float[] b)
        {
            return new YKVector2f(a.x + b[0], a.y + b[1]);
        }

        public static YKVector2f operator -(YKVector2f a, YKVector2f b)
        {
            return new YKVector2f(a.x - b.x, a.y - b.y);
        }
        public static YKVector2f operator -(YKVector2f a, float[] b)
        {
            return new YKVector2f(a.x - b[0], a.y - b[1]);
        }

        public static YKVector2f operator *(YKVector2f a, int b)
        {
            return new YKVector2f(a.x * b, a.y * b);
        }

        public static YKVector2f operator *(YKVector2f a, float b)
        {
            return new YKVector2f(a.x * b, a.y * b);
        }

        public static YKVector2f operator /(YKVector2f a, int b)
        {
            return new YKVector2f(a.x / b, a.y / b);
        }

        public static YKVector2f operator /(YKVector2f a, float b)
        {
            return new YKVector2f(a.x / b, a.y / b);
        }

        public static bool operator ==(YKVector2f lhs, YKVector2f rhs)
        {
            return ((lhs - rhs).SqrMagnitude < equalNum);
        }

        public static bool operator !=(YKVector2f lhs, YKVector2f rhs)
        {
            return ((lhs - rhs).SqrMagnitude >= equalNum);
        }

        public override bool Equals(object obj)
        {
            return obj == null ? false : (x == ((YKVector2f)obj).x && y == ((YKVector2f)obj).y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
