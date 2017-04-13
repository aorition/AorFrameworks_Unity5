using System;
using System.Collections.Generic;

namespace AorBaseUtility
{

    /// <summary>
    /// 四叉树查询
    /// </summary>
    public class UQuadTree
    {
        public const int MAX_COUNT = 10;
        public const int MAX_LEVEL = 7;

        public struct Rect
        {
            public float x;
            public float y;
            public float xMax;
            public float yMax;
            public float width;
            public float height;




            public Rect(float left, float top, float Width, float Height)

            {
                x = left;
                y = top;
                width = Width;
                height = Height;
                xMax = x + width;
                yMax = y + height;

            }


            public static bool operator !=(Rect value1, Rect value2)
            {
                if (value1.x != value2.x || value1.y != value2.y || value1.width != value2.width ||
                    value1.height != value2.height)
                    return true;
                else
                    return false;


            }

            public static bool operator ==(Rect value1, Rect value2)
            {

                if (value1.x == value2.x && value1.y == value2.y && value1.width == value2.width &&
                    value1.height == value2.height)
                    return true;
                else
                    return false;
            }




        }

        public struct Vector2
        {
            public float x;
            public float y;

        }



        public struct Leaf
        {
            public Rect Bounds;
            public int Id;

            public Leaf(Rect bound, int id)
            {
                Bounds = bound;
                Id = id;

            }

        }




        /// <summary>
        /// 当前深度
        /// </summary>
        public int level;

        /// <summary>
        /// 当前节点中的对象
        /// </summary>
        public List<Leaf> items;

        /// <summary>
        /// 子节点列表
        /// </summary>
        public UQuadTree[] nodes;

        /// <summary>
        /// 父节点
        /// </summary>
        public UQuadTree parent;

        /// <summary>
        /// 范围大小
        /// </summary>
        public Rect Bounds;

        /// <summary>
        /// 所在象限
        /// </summary>
        public int index;

        /// <summary>
        /// 元素个数
        /// </summary>
        public int num;

        /// <summary>
        /// x 坐标
        /// </summary>
        public float middleX;

        /// <summary>
        /// y 坐标
        /// </summary>
        public float middleY;

        /// <summary>
        /// 拆分状态
        /// </summary>
        private bool splitStatus;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="bounds">Bounds.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="index">Index.</param>
        public UQuadTree(int level, Rect bounds, UQuadTree parent, int index)
        {
            this.level = level;
            this.Bounds = bounds;
            this.parent = parent;
            this.index = index;

            this.middleX = bounds.x + bounds.width*0.5f;
            this.middleY = bounds.y + bounds.height*0.5f;

            this.splitStatus = false;
            this.items = new List<Leaf>();
            this.nodes = new UQuadTree[4];
        }

        public UQuadTree(Rect bounds)
        {
            this.level = 0;
            this.Bounds = bounds;
            this.parent = null;
            this.index = 0;

            this.middleX = bounds.x + bounds.width*0.5f;
            this.middleY = bounds.y + bounds.height*0.5f;

            this.splitStatus = false;
            this.items = new List<Leaf>();
            this.nodes = new UQuadTree[4];
        }


        /// <summary>
        /// 获取所有对象
        /// </summary>
        /// <returns>The all.</returns>
        public void GetAll(ref List<Leaf> resultList)
        {

            // 如果拆分过
            if (this.splitStatus)
            {
                for (int i = 0; i < 4; i++)
                {
                    nodes[i].GetAll(ref resultList);
                }

            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    resultList.Add(items[i]);
                }

            }

        }

        /// <summary>
        /// 拆分象限
        /// </summary>
        public void Split()
        {
            // 如果已经拆分过
            if (this.splitStatus) return;

            float halfWidth = this.Bounds.width*0.5f;
            float halfHeight = this.Bounds.height*0.5f;


            this.nodes[0] =
                (new UQuadTree(this.level + 1, new Rect(this.Bounds.x + halfWidth, this.Bounds.y, halfWidth, halfHeight),
                    this, 0));
            this.nodes[1] =
                (new UQuadTree(this.level + 1, new Rect(this.Bounds.x, this.Bounds.y, halfWidth, halfHeight), this, 1));
            this.nodes[2] =
                (new UQuadTree(this.level + 1,
                    new Rect(this.Bounds.x, this.Bounds.y + halfHeight, halfWidth, halfHeight), this, 2));
            this.nodes[3] =
                (new UQuadTree(this.level + 1,
                    new Rect(this.Bounds.x + halfWidth, this.Bounds.y + halfHeight, halfWidth, halfHeight), this, 3));


            this.splitStatus = true;
        }

        /// <summary>
        /// 根据点获取所在象限
        /// </summary>
        public int[] GetIndex(Vector2 pos)
        {


            return GetIndex(new Rect(pos.x - 0.05f, pos.y - 0.05f, 0.1f, 0.1f));

        }

        /// <summary>
        /// 根据范围获取所在象限
        /// </summary>
        public int[] GetIndex(Rect pRect)
        {


            int[] res = new int[4] {-1, -1, -1, -1};
            Rect zero = new Rect(0, 0, 0, 0);

            for (int i = 0; i < 4; i++)
            {
                if (GetIntsect(nodes[i].Bounds, pRect) != zero)
                {
                    res[i] = 1;
                }
            }



            return res;



        }



        /// <summary>
        /// 插入数据
        /// </summary>
        public void Insert(Leaf quad)
        {



            // 如果是拆分
            if (splitStatus)
            {
                int[] indexs = GetIndex(quad.Bounds);

                for (int i = 0; i < 4; i++)
                {
                    if (indexs[i] != -1)
                    {
                        //   Debug.Log("!!枝干:" + level + "[" + index + "]" + "下放叶子" + quad.Id + "到" + nodes[i].level + "[" + i + "]");
                        nodes[i].Insert(quad);
                    }
                }


            }
            else
            {


                items.Add(quad);


                if (items.Count > MAX_COUNT && level < MAX_LEVEL)
                {
                    Split();

                    //所有节点往下放
                    foreach (Leaf deleteQuad in items)
                    {

                        int[] indexs = GetIndex(deleteQuad.Bounds);


                        for (int i = 0; i < 4; i++)
                        {
                            if (indexs[i] != -1)
                            {
                                //   Debug.Log("枝干:" + level + "[" + index + "]" + "下放叶子" + deleteQuad.Id + "到" + nodes[i].level + "[" + i + "]");
                                nodes[i].Insert(deleteQuad);
                            }
                        }


                    }

                    items.Clear();

                }

                //   Debug.Log("枝干:" + level + "[" + index + "]" + "增加叶子" + quad.Id);

            }
        }

        /// <summary>
        /// 移动添加
        /// </summary>
        /// <param name="quad">Quad.</param>
        public void MoveInsert(Leaf quad)
        {
            //        int index = this.GetIndex(quad.Bounds);
            //        if (index != -1)
            //        {
            //            this.nodes[index].Insert(quad);
            //        }
            //        else
            //        {
            //             
            //            if (this.parent != null) this.parent.MoveInsert(quad);
            //        }
        }


        public List<Leaf> Select(Rect rect)
        {

            List<UQuadTree> Trees = new List<UQuadTree>();

            SelectTrees(rect, ref Trees);


            List<Leaf> leafs = new List<Leaf>();
            if (Trees.Count > 0)
            {
                GetLeafs(ref Trees, ref leafs);
            }

            return leafs;

        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="">.</param>
        private void SelectTrees(Rect rect, ref List<UQuadTree> Trees)
        {


            if (splitStatus)
            {

                //四个象限是否包含
                int[] indexs = GetIndex(rect);

                for (int i = 0; i < indexs.Length; i++)
                {
                    if (indexs[i] != -1)
                    {
                        //如果有交集
                        nodes[i].SelectTrees(rect, ref Trees);

                    }

                }
            }
            else
            {

                Trees.Add(this);
            }

        }



        /// <summary>
        /// 查询数据列表
        /// </summary>
        private void GetLeafs(ref List<UQuadTree> trees, ref List<Leaf> AllLeafs)
        {


            for (int i = 0; i < trees.Count; i++)
            {


                for (int j = 0; j < trees[i].items.Count; j++)
                {
                    //获得所有叶子
                    AllLeafs.Add(trees[i].items[j]);
                }

            }


        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="quad">Quad.</param>
        public void Remove(Leaf quad)
        {
            this.items.Remove(quad);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="">.</param>
        public void Move(Leaf quad)
        {
            if (
                !(quad.Bounds.x > this.Bounds.x && quad.Bounds.y > this.Bounds.y &&
                  quad.Bounds.x < this.Bounds.x + this.Bounds.width &&
                  quad.Bounds.y < this.Bounds.y + this.Bounds.height))
            {
                this.Remove(quad);
                if (this.parent != null) this.parent.MoveInsert(quad);
            }
        }

        private Rect GetIntsect(Rect rect1, Rect rect2)
        {

//        rect.min.x = max(rect1.min.x, rect2.min.x); //从最小x中找最大的x
//rect.min.y = max(rect1.min.y, rect2.min.y); //从最小的y中找最大的y
//rect.max.x = min(rect1.max.x, rect2.max.x); //从最大的x中找最小的x
//rect.max.y = min(rect1.max.y, rect2.max.y); //从最大的y中找最小的y
//if ( rect.min.x<rect.max.x && rect.min.y <rect.max.y ) //这样才有面积的交集



            Rect rect = new Rect(0, 0, 0, 0);
            rect.x = Math.Max(rect1.x, rect2.x); //从最小x中找最大的x
            rect.y = Math.Max(rect1.y, rect2.y); //从最小的y中找最大的y
            rect.xMax = Math.Min(rect1.xMax, rect2.xMax); //从最大的x中找最小的x
            rect.width = rect.xMax - rect.x;

            rect.yMax = Math.Min(rect1.yMax, rect2.yMax); //从最大的y中找最小的y
            rect.height = rect.yMax - rect.y;
            if (rect.x < rect.xMax && rect.y < rect.yMax) //这样才有交集
                return rect;
            else
                return new Rect(0, 0, 0, 0);
        }
    }
}