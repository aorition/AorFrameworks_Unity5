using System;
using System.Collections.Generic;
using System.Text;


namespace Framework.Editor.Utility
{
    
    public class ShaderWriter
    {
        private StringBuilder _tabStringBuilder;
        private StringBuilder _shaderStringBuilder;

        public string ShaderName;

        public ShaderWriter(string shaderName)
        {
            ShaderName = shaderName;
            init();
        }

        private void init()
        {
            _shaderStringBuilder = new StringBuilder();
            _tabStringBuilder = new StringBuilder();
        }

        private List<SwPropertyItem> _ProppertyList;

        private int _tabNum = 0;
        private string Core_get_tabs(int tabs)
        {

            if (tabs <= 0)
            {
                _tabNum = 0;
                _tabStringBuilder.Length = 0;
                return "";
            }

            if (_tabNum != tabs)
            {
                _tabNum = tabs;
                
                int i, len = _tabNum;
                _tabStringBuilder.Length = 0;
                for (i = 0; i < len; i++)
                {
                    _tabStringBuilder.Append("\t");
                }
            }

            return _tabStringBuilder.ToString();
        }

        private void Core_ShaderStr_AppendLine(string appendSt, int tabs = 0)
        {
            _shaderStringBuilder.AppendLine(Core_get_tabs(tabs) + appendSt);
        }

        //--------------------------------------- 
        
        /// <summary>
        /// 创建 Shader描述
        /// </summary>
        private void Gen_description(string description)
        {
            _shaderStringBuilder.AppendLine(@"//@@@DynamicShaderInfoStart");
            _shaderStringBuilder.AppendLine(@"//" + description);
            _shaderStringBuilder.AppendLine(@"//@@@DynamicShaderInfoEnd");
        }

        /// <summary>
        /// 创建 Fallback
        /// </summary>
        private void Gen_T_Fallback(string fallbackName, int tabs = 1)
        {
            if (string.IsNullOrEmpty(fallbackName))
            {
                Gen_T_FallbackOFF(tabs);
                return;
            }
            Core_ShaderStr_AppendLine("Fallback \"" + fallbackName + "\"", tabs);
        }

        /// <summary>
        /// 创建 Fallback Off
        /// </summary>
        private void Gen_T_FallbackOFF(int tabs = 1)
        {
            Core_ShaderStr_AppendLine("Fallback Off", tabs);
        }

        /// <summary>
        /// 创建 Shader{}
        /// </summary>
        private void Gen_T_Shader(string shaderName, Action InnerDo)
        {
            _shaderStringBuilder.AppendLine("Shader \"" + shaderName + "\" {");
            InnerDo();
            _shaderStringBuilder.AppendLine("}");
        }

        /// <summary>
        /// 创建 Properties{}
        /// </summary>
        private void Gen_T_Properties(List<SwPropertyItem> ProppertyList,  int tabs = 1)
        {
            Core_ShaderStr_AppendLine("Properties {", tabs);
            int i, len = ProppertyList.Count;
            for (i = 0; i < len; i++)
            {
                Core_ShaderStr_AppendLine(ProppertyList[i].toString(), tabs + 1);
            }
            Core_ShaderStr_AppendLine("}", tabs);
        }

        /// <summary>
        /// 创建 Category{}
        /// </summary>
        private void Gen_T_Category(Action InnerDo, int tabs = 1)
        {
            Core_ShaderStr_AppendLine("Category {", tabs);
            InnerDo();
            Core_ShaderStr_AppendLine("}", tabs);
        }

        /// <summary>
        /// 创建 SubShader{}
        /// </summary>
        private void Gen_T_SubShader(Action InnerDo, int tabs = 1)
        {
            Core_ShaderStr_AppendLine("SubShader {", tabs);
            InnerDo();
            Core_ShaderStr_AppendLine("}", tabs);
        }

        private void Gen_T_SubShaderTags()
        {
            //s
        }

        /// <summary>
        /// 创建 Pass{}
        /// </summary>
        private void Gen_T_Pass(Action InnerDo, string passName = "", int tabs = 2)
        {
            Core_ShaderStr_AppendLine("Pass {", tabs);
            if (!string.IsNullOrEmpty(passName))
            {
                Core_ShaderStr_AppendLine("Name \"" + passName + "\"", tabs + 1);
            }
            InnerDo();
            Core_ShaderStr_AppendLine("}", tabs);
        }

    }
}
