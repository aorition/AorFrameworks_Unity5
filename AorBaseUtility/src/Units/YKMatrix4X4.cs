using System;

namespace AorBaseUtility
{
    /**
     * 本文件定义了4*4的矩阵及其相关的处理函数
     * 注意：在这里是用列向量相乘的矩阵，
     * 矩阵m和向量v相乘：m * v，v是列向量:
     * 
     * [ m[0][0]  m[0][1]  m[0][2]  m[0][3] ]   {v[0]}
     * | m[1][0]  m[1][1]  m[1][2]  m[1][3] | * {v[1]}
     * | m[2][0]  m[2][1]  m[2][2]  m[2][3] |   {v[2]}
     * [ m[3][0]  m[3][1]  m[3][2]  m[3][3] ]   {1}
     *
     * 转OpenGL矩阵的时候，需要将m转置过来(OpenGL虽然也是和列向量相乘，不过OpenGL的矩阵也是列优先)
     * 转D3D矩阵的时候，也需要将m转置过来（D3D矩阵是行优先，不过是行向量和矩阵相乘）
     * 这里用的是右手坐标系，和OpenGL相同，和D3D相反
     *
     * 另，CVV用的是OpenGL的范围：-1 <= x <= 1, -1 <= y <= 1, -1 <= z <= 1
     */

    public struct YKMatrix4X4
    {
        public float m00;

        public float m10;

        public float m20;

        public float m30;

        public float m01;

        public float m11;

        public float m21;

        public float m31;

        public float m02;

        public float m12;

        public float m22;

        public float m32;

        public float m03;

        public float m13;

        public float m23;

        public float m33;

        public float this[int row, int column]
        {
            get { return this[row + column * 4]; }
            set { this[row + column * 4] = value; }
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.m00;
                    case 1:
                        return this.m10;
                    case 2:
                        return this.m20;
                    case 3:
                        return this.m30;
                    case 4:
                        return this.m01;
                    case 5:
                        return this.m11;
                    case 6:
                        return this.m21;
                    case 7:
                        return this.m31;
                    case 8:
                        return this.m02;
                    case 9:
                        return this.m12;
                    case 10:
                        return this.m22;
                    case 11:
                        return this.m32;
                    case 12:
                        return this.m03;
                    case 13:
                        return this.m13;
                    case 14:
                        return this.m23;
                    case 15:
                        return this.m33;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.m00 = value;
                        break;
                    case 1:
                        this.m10 = value;
                        break;
                    case 2:
                        this.m20 = value;
                        break;
                    case 3:
                        this.m30 = value;
                        break;
                    case 4:
                        this.m01 = value;
                        break;
                    case 5:
                        this.m11 = value;
                        break;
                    case 6:
                        this.m21 = value;
                        break;
                    case 7:
                        this.m31 = value;
                        break;
                    case 8:
                        this.m02 = value;
                        break;
                    case 9:
                        this.m12 = value;
                        break;
                    case 10:
                        this.m22 = value;
                        break;
                    case 11:
                        this.m32 = value;
                        break;
                    case 12:
                        this.m03 = value;
                        break;
                    case 13:
                        this.m13 = value;
                        break;
                    case 14:
                        this.m23 = value;
                        break;
                    case 15:
                        this.m33 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }
    }
}
