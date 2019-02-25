using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 概率随机池
/// 
/// 区别于标准概率算法的对象池模拟算法
/// 
/// </summary>
public class RandomPool
{

    private class RBpbbItem {
        public int pbb;
        public int idxRangeMin;
        public int idxRangeMax;
    }

    //--------------------------------------

    /// <summary>
    /// 基础概率快速算法 (常见的概率算法)
    /// </summary>
    /// <param name="probabilities">概率数组</param>
    /// <returns>某概率索引号</returns>
    public static int FastRandomWithProbabilities(float[] probabilities)
    {
        float total = 0;
        //首先计算出概率的总值，用来计算随机范围
        for (int i = 0; i < probabilities.Length; i++)
        {
            total += probabilities[i];
        }
        float value = UnityEngine.Random.Range(0, total);
        for (int i = 0; i < probabilities.Length; i++)
        {
            if (value < probabilities[i])
            {
                return i;
            }
            else
            {
                value -= probabilities[i];
            }
        }
        return probabilities.Length - 1;
    }
    private static void _cutShuffle(IList ins, int cutInt, int times = 1)
    {
        int t = 0;
        object x;
        cutInt = cutInt % ins.Count;
        for (int i = 0; i < times; i++)
        {
            for (int j = 0; j < ins.Count; j++)
            {
                t = (j + cutInt) % ins.Count;
                x = ins[t];
                ins[t] = ins[j];
                ins[j] = x;
            }
        }
    }
    private static void _randomShuffle(IList ins, int times = 1)
    {
        int t = 0;
        object x;
        for (int i = 0; i < times; i++)
        {
            for (int j = 0; j < ins.Count; j++)
            {
                t = _randomDif(t, 0, ins.Count - 1);
                x = ins[t];
                ins[t] = ins[j];
                ins[j] = x;
            }
        }
    }
    private static void _perfectShuffle(IList ins, int times = 1)
    {
        for (int i = 0; i < times; i++)
        {
            _perfectShuffle(ins, 0, ins.Count - 1);
        }
    }
    private static void _perfectShuffle(IList ins, int s, int t)
    {
        if (s > t) return;
        int gap = t - s + 1;
        //判断数组长度是不是3^k-1
        int k = 1, p = 3;
        while (p - 1 <= gap) { k++; p *= 3; }
        //循环右移,确定m,n长度
        int n = gap / 2, m = n;
        if (p != gap)
        {
            m = (p / 3 - 1) / 2;
            _leftRotate(ins, s + m, s + n - 1, s + m + n - 1);
        }
        //前3^k-1个元素进行环操作
        int start = 1;
        while (--k <= 0)
        {
            _cycle(ins, start, s, 2 * m);
            start *= 3;
        }
        //最后剩余进行递归操作
        _perfectShuffle(ins, s + 2 * m, t);
    }
    private static void _cycle(IList ins, int s, int begin, int length)
    {
        object pre = ins[begin + s - 1];
        int mod = length + 1;
        int next = s * 2 % mod;
        object tmp, tmp2;
        while (next != s)
        {
            //swap
            tmp = pre;
            tmp2 = ins[begin + next - 1];
            ins[begin + next - 1] = tmp;
            pre = tmp2;

            next = 2 * next % mod;
        }
        ins[begin + s - 1] = pre;
    }
    private static void _reverse(IList ins, int s, int t) {
        while (s < t) _swap(ins, s++, t--);
    }
    private static void _swap(IList ins, int a, int b) {
        object x = ins[a];
        ins[a] = ins[b];
        ins[b] = x;
    }
    private static void _leftRotate(IList ins, int s, int m, int t)
    {
        _reverse(ins, s, m);
        _reverse(ins, m + 1, t);
        _reverse(ins, s, t);
    }
    private static int _randomDif(int ck, int min, int max)
    {
        int r = UnityEngine.Random.Range(min, max);
        if (r == ck) return _randomDif(ck, min, max);
        return r;
    }


    //--------------------------------------

    /// <summary>
    /// 概率随机池
    /// </summary>
    /// <param name="probabilities">概率组(其内float值表示一个概率值(百分比),取值范围为大于0f并小于等于1f, 注意概率组内成员值的总和应该为1f)</param>
    /// <param name="values">对象组(匹配概率组索引号)</param>
    /// <param name="poolSize">池大小(默认值为100.注意 1.这个值越大开销越大. 2.实际生成池大小不一定等于这个值, 具体要看概率组概率总是否等于1f)</param>
    /// <param name="initShuffleTimes">初始池洗牌次数</param>
    public RandomPool(float[] probabilities, object[] values, int  poolSize = 100, int initShuffleTimes = 1)
    {
        if(probabilities != null && values != null && probabilities.Length > 0 && probabilities.Length == values.Length)
        {

            this.m_probabilities = probabilities;
            this.m_values = values;

            int i, len = probabilities.Length;
            List<RandomPool.RBpbbItem> RBpbbList = new List<RBpbbItem>();
            for (i = 0; i < len; i++)
            {
                RBpbbItem rBpbbItem = new RBpbbItem();
                float p = probabilities[i];
                rBpbbItem.pbb = UnityEngine.Mathf.RoundToInt(p * poolSize);
                RBpbbList.Add(rBpbbItem);
            }

            ////Todo 从大到小,必须验证sort是否正确
            //RBpbbList.Sort((a, b) => {
            //    if(a.pbb < b.pbb)
            //    {
            //        return 1;
            //    }
            //    else
            //    {
            //        return -1;
            //    }
            //});
            
            int total = 0;
            for (i = 0; i < len; i++)
            {
                RBpbbList[i].idxRangeMin = total;
                total += RBpbbList[i].pbb;
                RBpbbList[i].idxRangeMax = total;
            }

            _rdIdxBox = new int[total];
            for (int u = 0; u < RBpbbList.Count; u++)
            {
                RBpbbItem item = RBpbbList[u];
                for (int j = item.idxRangeMin; j < item.idxRangeMax; j++)
                {
                    _rdIdxBox[j] = u;
                }
            }

            RBpbbList.Clear();

            //初始化概率数组固定洗牌
            _shuffleRandomPool(_rdIdxBox, initShuffleTimes);

        }
        else
        {
            //Error
            throw new Exception("*** RandomPool.Exception :: 指定形参无效 !");
        }
    }

    /// <summary>
    /// 原生随机池
    /// </summary>
    private int[] _rdIdxBox;

    /// <summary>
    /// 输入概率组
    /// </summary>
    private float[] m_probabilities;
    /// <summary>
    /// 输入池对象组
    /// </summary>
    private object[] m_values;

    /// <summary>
    /// 随机池
    /// </summary>
    private List<int> m_activedRdIdxBox = new List<int>();
    protected List<int> _activedRdIdxBox {
        get {
            if(m_activedRdIdxBox.Count == 0)
            {
                ReloadRandomPool();
            }
            return m_activedRdIdxBox;
        }
    }
    
    /// <summary>
    /// 重置随机池
    /// <param name="shuffleRandomPoolTimes">重置随机池后,洗牌的次数(默认:2,洗牌两次)</param>
    /// </summary>
    public void ReloadRandomPool(int shuffleRandomPoolTimes = 1) {
        m_activedRdIdxBox.Clear();
        for (int i = 0; i < _rdIdxBox.Length; i++)
        {
            int t = _rdIdxBox[i];
            m_activedRdIdxBox.Add(t);
        }
        ShuffleRandomPool(shuffleRandomPoolTimes);
    }
    
    /// <summary>
    /// 随机池洗牌
    /// </summary>
    /// <param name="times">洗牌次数</param>
    public void ShuffleRandomPool(int times = 1) {
        _shuffleRandomPool(m_activedRdIdxBox, times);
    }

    protected void _shuffleRandomPool(IList list, int times = 1)
    {
        for (int i = 0; i < times; i++)
        {
            _cutShuffle(list, UnityEngine.Random.Range(0, list.Count - 1));
            //完美洗牌
            _perfectShuffle(list);
            //随机洗牌
            _randomShuffle(list);
        }
    }

    /// <summary>
    /// 从随机池中获取一个对象,并且将该对象移除随机池
    /// </summary>
    /// <param name="randomIndex">是否在随机池中的随机位置抽取对象(默认:false,抽取随机池中第一个对象)</param>
    /// <param name="shuffleRandomPoolTimes">如果发生自动重置, 重置随机池后,洗牌的次数(默认:2,洗牌两次)</param>
    /// <returns></returns>
    public object PopValueFromRandomPool(int index = 0, int shuffleRandomPoolTimes = 1) {
        if(m_activedRdIdxBox.Count == 0) ReloadRandomPool(shuffleRandomPoolTimes);
        int r = index % m_activedRdIdxBox.Count;
        int idx = m_activedRdIdxBox[r];
        m_activedRdIdxBox.RemoveAt(r);
        return m_values[idx];
    }

    /// <summary>
    /// 获取当前随机池的对象个数
    /// </summary>
    public int GetRandomPoolLength {
        get { return m_activedRdIdxBox.Count; }
    }
    
    /// <summary>
    /// 从随机池中获取一个对象
    /// </summary>
    /// <param name="index">随机池索引号</param>
    public object GetValueFromRandomPool(int index)
    {
        if(index >= 0 && index < m_activedRdIdxBox.Count)
        {
            return m_values[m_activedRdIdxBox[index]];
        }
        else
        {
            //Error
            return null;
        }
    }
    
    /// <summary>
    /// 交换随机池中的两个对象位置
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void SwapRandomPoolValue(int index1, int index2) {
        if (index1 >= 0 && index1 < m_activedRdIdxBox.Count && index2 >= 0 && index2 < m_activedRdIdxBox.Count && index1 != index2)
        {
            _swap(m_activedRdIdxBox, index1, index2);
        }
        else
        {
            //Error
        }
    }


}

