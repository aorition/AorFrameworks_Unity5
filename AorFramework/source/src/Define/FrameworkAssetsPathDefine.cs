using System;
using System.Collections.Generic;

namespace Framework
{

    /// <summary>
    /// 定义Framework在Unity项目中必要的路径(Assets/) 
    /// </summary>
    public class FrameworkAssetsPathDefine
    {

        //-- NodeGraph 依赖资源路径
        //-------------------------------

        #region NodeGraph 依赖资源root路径

        public const string NodeGraphRoot = "Assets/NodeGraphAssets";

        #endregion
        
        #region NodeGraph 依赖Bitmap资源路径

        public const string NodeGraphBitmaps = "Assets/NodeGraphAssets/Bitmaps";

        #endregion

        //-------------------------------

        //-- 框架依赖资源
        //-------------------------------

        #region 框架依赖资源root路径

        public const string BaseRes = "Assets/Resources/RuntimeBaseRes";

        #endregion

        #region 框架依赖Shader资源路径

        public const string BaseShaderRes = "Assets/Resources/RuntimeBaseRes/Shaders";

        #endregion

        //-------------------------------

        //-- 基础UI依赖资源路径
        //-------------------------------

        #region 基础UI依赖资源路径

        public const string BaseUIRes = "Assets/Resources/RuntimeUIBaseRes";

        #endregion

        //-------------------------------

        //-- 框架Editor依赖Shader资源路径  
        //-------------------------------

        #region 框架Editor依赖Shader资源路径  

        public const string EditorShader = "Assets/FrameworkEditorSupport/Shader";

        #endregion
        
        //-------------------------------

    }
}
