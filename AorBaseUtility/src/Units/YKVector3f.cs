using System;

namespace AorBaseUtility
{
    public struct YKVector3f
    {
        public const float kEpsilon = 1E-05f;
        public const float equalNum = 9.999999E-11f;
        public float x;
        public float y;
        public float z;

        public static YKVector3f UnitX
        {
            get
            {
                return new YKVector3f(1, 0, 0);
            }
        }

        public static YKVector3f UnitY
        {
            get
            {
                return new YKVector3f(0, 1, 0);
            }
        }

        public static YKVector3f UnitZ
        {
            get
            {
                return new YKVector3f(0, 0, 1);
            }
        }

        public static YKVector3f Zero
        {
            get
            {
                return new YKVector3f(0, 0, 0);
            }
        }

        public static YKVector3f One
        {
            get
            {
                return new YKVector3f(1, 1, 1);
            }
        }

        public static YKVector3f Lerp(YKVector3f from, YKVector3f to, float t)
        {
            t = YKMath.Clamp01(t);
            return new YKVector3f(from.x + ((to.x - from.x) * t), from.y + ((to.y - from.y) * t), from.z + ((to.z - from.z) * t));
        }

        public static YKVector3f SmoothDamp(YKVector3f current, YKVector3f target, ref YKVector3f currentVelocity, float smoothTime, float maxSpeed = 1, float deltaTime = 0.02f)
        {
            smoothTime = Math.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (((1f + num2) + ((0.48f * num2) * num2)) + (((0.235f * num2) * num2) * num2));
            YKVector3f vector = current - target;
            YKVector3f vector2 = target;
            float maxLength = maxSpeed * smoothTime;
            vector = ClampMagnitude(vector, maxLength);
            target = current - vector;
            YKVector3f ykVector3F = ((currentVelocity + (num * vector)) * deltaTime);
            currentVelocity = ((currentVelocity - (num * ykVector3F)) * num3);
            YKVector3f vector4 = target + (((vector + ykVector3F) * num3));
            if (Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = ((vector4 - vector2) / deltaTime);
            }
            return vector4;
        }

        public static YKVector3f ClampMagnitude(YKVector3f vector, float maxLength)
        {
            if (vector.SqrMagnitude > (maxLength * maxLength))
            {
                return vector.Normalized * maxLength;
            }
            return vector;
        }

        public float SqrMagnitude
        {
            get
            {
                return (((x * x) + (y * y)) + (z * z));
            }
        }

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(((x * x) + (y * y)) + (z * z));
            }
        }

        public YKVector3f Normalized
        {
            get
            {
                float num = Magnitude;
                if (num > kEpsilon)
                {
                    return (this / num);
                }
                return Zero;
            }
        }

        public void Normalize()
        {
            float num = Magnitude;
            if (num > kEpsilon)
            {
                x /= num;
                y /= num;
                z /= num;
            }
            else
            {
                x = 0;
                y = 0;
                z = 0;
            }
        }

        public void Inverse()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        public YKVector3f Inversed
        {
            get
            {
                return new YKVector3f(-x, -y, -z);
            }
        }

        public static float Dot(YKVector3f lhs, YKVector3f rhs)
        {
            return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
        }

        public float Dot(YKVector3f rhs)
        {
            return (((x * rhs.x) + (y * rhs.y)) + (z * rhs.z));
        }

        public static YKVector3f Scale(YKVector3f a, YKVector3f b)
        {
            return new YKVector3f(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static YKVector3f Cross(YKVector3f lhs, YKVector3f rhs)
        {
            return new YKVector3f((lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.x * rhs.y) - (lhs.y * rhs.x));
        }

        public static YKVector3f Project(YKVector3f vector, YKVector3f onNormal)
        {
            float num = Dot(onNormal, onNormal);
            if (num < float.Epsilon)
            {
                return Zero;
            }
            return ((onNormal * Dot(vector, onNormal)) / num);
        }

        public static YKVector3f Exclude(YKVector3f excludeThis, YKVector3f fromThat)
        {
            return (fromThat - Project(fromThat, excludeThis));
        }

        public static float Angle(YKVector3f from, YKVector3f to)
        {
            return (float)(Math.Acos(YKMath.Clamp(Dot(from.Normalized, to.Normalized), -1f, 1f)) * 57.29578f);
        }

        public static float Distance(YKVector3f a, YKVector3f b)
        {
            YKVector3f vector = new YKVector3f(a.x - b.x, a.y - b.y, a.z - b.z);
            return (float)Math.Sqrt(((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z));
        }

        public static YKVector3f Min(YKVector3f lhs, YKVector3f rhs)
        {
            return new YKVector3f(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }

        public static YKVector3f Max(YKVector3f lhs, YKVector3f rhs)
        {
            return new YKVector3f(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }

        public static YKVector3f MoveTowards(YKVector3f current, YKVector3f target, float maxDistanceDelta)
        {
            YKVector3f vector = target - current;
            float magnitude = vector.Magnitude;
            if ((magnitude > maxDistanceDelta) && (magnitude != 0f))
            {
                return (current + ((YKVector3f)((vector / magnitude) * maxDistanceDelta)));
            }
            return target;
        }

        public YKVector3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public YKVector3f(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0f;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;

                    case 1:
                        return this.y;

                    case 2:
                        return this.z;
                }
                throw new IndexOutOfRangeException("Invalid Vector3 index!");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;

                    case 1:
                        this.y = value;
                        break;

                    case 2:
                        this.z = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        public void Set(float new_x, float new_y, float new_z)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
        }

        public void Scale(YKVector3f scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        public void Add(YKVector3f v)
        {
            x += v.x;
            y += v.y;
            z += v.z;
        }

        public void Sub(YKVector3f v)
        {
            x -= v.x;
            y -= v.y;
            z -= v.z;
        }

        public void Mul(YKVector3f v)
        {
            x *= v.x;
            y *= v.y;
            z *= v.z;
        }

        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format), this.z.ToString(format) };
            return string.Format("({0}, {1}, {2})", args);
        }

        #region override funtion

        public override int GetHashCode()
        {
            return ((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2));
        }

        public override bool Equals(object other)
        {
            if (!(other is YKVector3f))
            {
                return false;
            }
            YKVector3f vector = (YKVector3f)other;
            return ((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z));
        }

        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y, this.z };
            return string.Format("({0:F}, {1:F}, {2:F})", args);
        }
        #endregion

        #region reload function
        public static YKVector3f operator +(YKVector3f a, YKVector3f b)
        {
            return new YKVector3f(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static YKVector3f operator +(YKVector3f a, float b)
        {
            return new YKVector3f(a.x + b, a.y + b, a.z + b);
        }

        public static YKVector3f operator -(YKVector3f a, YKVector3f b)
        {
            return new YKVector3f(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static YKVector3f operator -(YKVector3f a)
        {
            return new YKVector3f(-a.x, -a.y, -a.z);
        }

        public static YKVector3f operator *(YKVector3f a, float d)
        {
            return new YKVector3f(a.x * d, a.y * d, a.z * d);
        }

        public static YKVector3f operator *(float d, YKVector3f a)
        {
            return new YKVector3f(a.x * d, a.y * d, a.z * d);
        }

        public static YKVector3f operator /(YKVector3f a, float d)
        {
            return new YKVector3f(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(YKVector3f lhs, YKVector3f rhs)
        {
            return ((lhs - rhs).SqrMagnitude < equalNum);
        }

        public static bool operator !=(YKVector3f lhs, YKVector3f rhs)
        {
            return ((lhs - rhs).SqrMagnitude >= equalNum);
        }
        #endregion
    }
}
