using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;

namespace AorFramework.DataWrapers
{
    /// <summary>
    /// SpriteMetaData Config包装类
    /// 
    /// 对口Unity 版本 ： 5.5.x
    /// 
    /// </summary>
    public class SpriteMetaDataWraper : Config
    {

        public static SpriteMetaDataWraper CreateFormSpriteMetaData(SpriteMetaData SpriteMetaData)
        {
            SpriteMetaDataWraper dw = new SpriteMetaDataWraper();
            dw.updateConfigValue("name", SpriteMetaData.name);
            dw.updateConfigValue("rect", SpriteMetaData.rect);
            dw.updateConfigValue("alignment", SpriteMetaData.alignment);
            dw.updateConfigValue("pivot", SpriteMetaData.pivot);
            dw.updateConfigValue("border", SpriteMetaData.border);
            return dw;
        }

        public SpriteMetaDataWraper()
        {
        }

        public readonly string name;
        public readonly Rect rect;
        public readonly int alignment;
        public readonly Vector2 pivot;
        public readonly Vector4 border;

        public SpriteMetaData ToSpriteMetaData()
        {
            SpriteMetaData md = new SpriteMetaData();
            md.name = this.name;
            md.rect = this.rect;
            md.alignment = this.alignment;
            md.pivot = this.pivot;
            md.border = this.border;
            return md;
        }

    }
}
