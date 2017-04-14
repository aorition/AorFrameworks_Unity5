using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeToolItemAttribute : Attribute
    {
        public int sortId;
        public string labelDefine;
        public string reflectionClass;
        public string reflectionMethodName;
        public string collectionTag;
        public bool defaultIntoShortcutMenu;

        public NodeToolItemAttribute(string labelDefine,string reflectionClassFullName,string reflectionMethodName,string collectionTag = "default", int sortId = 0, bool defaultIntoShortcutMenu = false)
        {

            this.labelDefine = labelDefine;
            this.reflectionClass = reflectionClassFullName;
            this.reflectionMethodName = reflectionMethodName;
            this.collectionTag = collectionTag;
            this.sortId = sortId;
            this.defaultIntoShortcutMenu = defaultIntoShortcutMenu;
        }

    }
}
