using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace AorBaseUtility
{

    public class Config
    {

        [ConfigComment("ID")]
        public readonly long id;
    }

    /// <summary>
    /// TConfig 序列化格式未Table文本数据config
    /// </summary>
    public class TConfig : Config
    {

        [ConfigComment("描述")]
        public readonly string describe;

        private HashSet<string> _assets = new HashSet<string>();

        public HashSet<string> Assets
        {
            get { return _assets; }
        }
    }

    public class ConfigCommentAttribute : Attribute
    {
        public readonly string comment;
        public ConfigCommentAttribute(string comment)
        {
            this.comment = comment;
        }
    }

    public class ConfigAssetAttribute : Attribute { }
    public class ConfigPathAttribute : Attribute { }

    /// <summary>
    /// 用于附加等级计算的专用类，使用格式为@level:id_index(@level:id)，作用类型为double
    /// </summary>
    public class ConfigLevelVar : TConfig
    {
        [ConfigComment("首项")]
        public readonly double firstItem;
        [ConfigComment("公差")]
        public readonly double tolerance;
        [ConfigComment("附加数组")]
        public readonly double[] extendArray;

        public double GetLevelVar(int index)
        {
            if (index < 0) return firstItem;    
 
            double result = firstItem + tolerance * index;

            if (extendArray.Length == 0) return result;

            return index >= extendArray.Length
                ? result + extendArray[extendArray.Length - 1]
                : result + extendArray[index];
        }
    }
}
