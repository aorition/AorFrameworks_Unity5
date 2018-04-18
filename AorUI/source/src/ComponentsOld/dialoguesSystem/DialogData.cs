using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoticUnity.GUI.AorUI.Components {

    public enum DialogPortraitTitleType{
        Text,
        Image
    }

    public enum DialogPortraitPosition {
        PositionA,
        PositionB
    }

    public enum DialogPortraitType {
        Image,
        RawImage,
        prefab
    }

    /// <summary>
    /// 肖像描述
    /// </summary>
    public class DialogPortraitInfo {

        /// <summary>
        /// 肖像ID
        /// </summary>
        public int PortraitID;

        /// <summary>
        /// 肖像类型
        /// </summary>
        public DialogPortraitType PortraitType;

        /// <summary>
        /// 肖像资源路径
        /// </summary>
        public string PortraitPath;

        /// <summary>
        /// 肖像材质球路径
        /// </summary>
        public string PortraitMaterialPath;

        /// <summary>
        /// 肖像标题类型
        /// </summary>
        public DialogPortraitTitleType PortraitTitleType;

        /// <summary>
        /// 肖像标题数据 TilteType=0:标题内容/TilteType=1:标题资源路径
        /// </summary>
        public string PortraitTitleValue;

        /// <summary>
        /// 肖像标题材质球资源路径 仅当肖像标题类型为Image时可以用
        /// </summary>
        public string PortraitTitleMaterialPath;

    }

    public class DialogData {

        /// <summary>
        /// 肖像字典
        /// </summary>
        public Dictionary<int, DialogPortraitInfo> PortraitDic;

        /// <summary>
        /// 肖像id
        /// </summary>
        public List<int> PortraitList;

        /// <summary>
        /// 肖像位置列表 (默认情况下 )
        /// </summary>
        public List<DialogPortraitPosition> ProtraitPosList;

        /// <summary>
        /// 对话内容列表
        /// </summary>
        public List<string> DialogList;

        /// <summary>
        /// 对话内容语音资源路径(没有配置时,值为"")
        /// </summary>
        public List<string> DialogAudioPathList;

        /// <summary>
        /// 对话内容事件列表(没有配置时,值为"")
        /// </summary>
        public List<string> DialogEventList;


        
    }
}
