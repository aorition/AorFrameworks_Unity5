using System.Collections;
using AorBaseUtility;

namespace YoukiaCore
{
    public class YKRandom : RandomBase
    {

        /* static fields */
        private const int A = 16807;
        private const int M = 2147483647;
        private const int Q = 127773;
        private const int R = 2836;
        private const int MASK = 123459876;

        /* constructors */

        public YKRandom() : base()
        {
        }

        /** 以指定的种子构造随机数生成器 */

        public YKRandom(int seed)
            : base(seed)
        {
        }

        /* methods */
        /** 获得随机正整数 */

        public override int randomInt()
        {
            int r = seed;
            // 防止种子为0
            r ^= MASK;
            int k = r/Q;
            r = A*(r - k*Q) - R*k;
            if (r < 0) r += M;
            seed = r;
            return r;
        }
    }
}