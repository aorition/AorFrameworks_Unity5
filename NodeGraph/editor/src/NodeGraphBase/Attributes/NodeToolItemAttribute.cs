using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.NodeGraph
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeToolItemAttribute : Attribute
    {
        public int sortId;
        public string labelDefine;
        public string reflectionNs;
        public string reflectionParams;
        public string collectionTag;
        public bool defaultIntoShortcutMenu;

        public NodeToolItemAttribute(string labelDefine,string reflectionNs, string reflectionParams,string collectionTag = "default", int sortId = 0, bool defaultIntoShortcutMenu = false)
        {

            this.labelDefine = labelDefine;
            this.reflectionNs = reflectionNs;
            this.reflectionParams = reflectionParams;
            this.collectionTag = collectionTag;
            this.sortId = sortId;
            this.defaultIntoShortcutMenu = defaultIntoShortcutMenu;
        }
        
        public string getFullReflectionParams()
        {
            if (reflectionParams.Contains("|"))
            {
                string o = string.Empty;
                string[] ps = reflectionParams.Split('|');
                int i, len = ps.Length;
                for (i = 0; i < len; i++)
                {
                    if (i > 0)
                    {
                        o += "|";
                    }

                    o += reflectionNs + "." + ps[i];
                }
                return o;
            }
            return reflectionNs + "." + reflectionParams;
        }

    }
}
