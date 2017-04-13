using System;

namespace AorBaseUtility
{
    public abstract class RandomBase
    {

        /* static fields */
//        protected const float FLOAT_MAX = 1.0f - 1.192092896e-07f;
//        protected const float FLOAT_AM = 1.0f/2147483563.0f;
//        protected const double DOUBLE_MAX = 1.0d - 1.192092896e-07f;
//        protected const double DOUBLE_AM = 1.0d/2147483563.0d;

        /* fields */
        // 随机种子
        protected int seed = 0;

        /* constructors */

        public RandomBase()
        {
            seed = (int) ((new TimeSpan(DateTime.Now.Ticks).TotalSeconds));
        }

        /** 以指定的种子构造随机数生成器 */

        public RandomBase(int seed)
        {
            this.seed = seed;
        }

        /* properties */
        /** 获得随机种子 */

        public int getSeed()
        {
            return seed;
        }

        /** 设置随机种子 */

        public void setSeed(int seed)
        {
            this.seed = seed;
        }

        /* abstract methods */
        public abstract int randomInt();

        /* methods */
        /** 获得随机浮点数，范围0～1之间 */

//        public float randomFloat()
//        {
//            int r = randomInt();
//            float tmp = FLOAT_AM*r;
//            return (tmp > FLOAT_MAX) ? FLOAT_MAX : tmp;
//        }

        public float randomFloat()
        {
            int r = randomInt();
            return r / (float)int.MaxValue;
        }

        public double randomDouble()
        {
            int r = randomInt();
            return r / (double)int.MaxValue;
        }

        /** 获得指定范围的随机整数 */

        public int randomValue(int v1, int v2)
        {
            if (v2 > v1)
            {
                v2 += 1;
                return randomInt()%(v2 - v1) + v1;
            }
            else if (v1 > v2)
            {
                v1 += 1;
                return randomInt()%(v1 - v2) + v2;
            }
            else
                return v1;
        }

        /** 获得指定范围的随机浮点数 */

        public float randomValue(float v1, float v2)
        {
            if (v2 > v1)
                return randomFloat()*(v2 - v1) + v1;
            else if (v1 > v2)
                return randomFloat()*(v1 - v2) + v2;
            else
                return v1;
        }

        public double randomValue(double v1, double v2)
        {
            if (v2 > v1)
                return randomDouble() * (v2 - v1) + v1;
            else if (v1 > v2)
                return randomDouble() * (v1 - v2) + v2;
            else
                return v1;
        }
    }
}