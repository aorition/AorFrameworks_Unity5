using System;

namespace AorFramework.AssetsNodeGraph
{
    /// <summary>
    /// AssetsNodeGraph自定义脚本描述
    /// </summary>
    public class CustomScriptDescribeAttribute : Attribute
    {
        public readonly string Describe;
        public CustomScriptDescribeAttribute(string describe)
        {
            this.Describe = describe;
        }
    }
}
