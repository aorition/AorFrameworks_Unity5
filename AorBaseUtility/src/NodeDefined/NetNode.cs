using System;
using System.Collections.Generic;

namespace AorBaseUtility
{

    /// <summary>
    /// 网络节点数据模型
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class NetNode<T> where T : class
    {
        /// <summary>
        /// 检查交叉引用
        /// （检查 参考节点 是否在网络模型中存在循环引用）
        /// </summary>
        /// <param name="treeNode">已有的树模型</param>
        /// <param name="target">参考节点</param>
        public static bool CheckCrossReferences(NetNode<T> treeNode, NetNode<T> target)
        {
            bool result = false;
            //向上查找
            if (treeNode.m_parents != null)
            {
                int i, len = treeNode.m_parents.Count;
                for (i = 0; i < len; i++)
                {
                    result = findCrossReferencesUpper(treeNode.m_parents[i], target.value);
                    if (result) return true;
                }
            }

            //向下查找
            if (treeNode.m_children != null)
            {
                int i, len = treeNode.m_children.Count;
                for (i = 0; i < len; i++)
                {
                    result = findCrossReferencesDowner(treeNode.m_children[i], target.value);
                    if (result) return true;
                }
            }

            return false;
        }

        private static bool findCrossReferencesUpper(NetNode<T> node, T value)
        {
            if (node.value == value) return true;

            if (node.m_parents != null)
            {
                bool r;
                int i, len = node.m_parents.Count;
                for (i = 0; i < len; i++)
                {
                    r = findCrossReferencesUpper(node.m_parents[i], value);
                    if (r) return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private static bool findCrossReferencesDowner(NetNode<T> node, T value)
        {
            if (node.value == value) return true;

            bool result = false;
            if (node.m_children != null)
            {
                int i, len = node.m_children.Count;
                for (i = 0; i < len; i++)
                {
                    result = findCrossReferencesDowner(node.m_children[i], value);
                    if (result) return true;
                }
            }

            return result;
        }

        private List<NetNode<T>> m_parents;
        private List<NetNode<T>> m_children;

        private T m_value;

        public T value
        {
            get { return m_value; }
        }

        public NetNode(T value)
        {

            m_value = value;

            if (m_parents != null)
            {
                int i, len = m_parents.Count;
                for (i = 0; i < len; i++)
                {
                    m_parents[i].AddChild(this);
                }
            }

        }

        public NetNode(T value, ICollection<NetNode<T>> parents, ICollection<NetNode<T>> children)
        {
            m_value = value;

            if (m_children == null) m_children = new List<NetNode<T>>(children);

            if (m_parents == null) m_parents = new List<NetNode<T>>(parents);
            if (m_parents != null)
            {
                int i, len = m_parents.Count;
                for (i = 0; i < len; i++)
                {
                    m_parents[i].AddChild(this);
                }
            }
        }

        //析构
        ~NetNode()
        {
            if (m_parents != null)
            {
                int i, len = m_parents.Count;
                for (i = 0; i < len; i++)
                {
                    m_parents[i].RemoveChild(this);
                }
                
            }
        }

        /// <summary>
        /// 设置上级节点
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(NetNode<T> parent)
        {
            if (m_parents == null) m_parents = new List<NetNode<T>>();

            if (!m_parents.Contains(parent))
            {
                m_parents.Add(parent);
                parent.AddChild(this);
            }
        }

        public void RemoveParent(NetNode<T> parent)
        {
            if (m_parents == null) return;
            if (m_parents.Contains(parent))
            {
                m_parents.Remove(parent);
                parent.RemoveChild(this);
            }
        }

        /// <summary>
        /// 获取上级节点
        /// </summary>
        public List<NetNode<T>> GetParents()
        {
            return (m_parents == null ? null : m_parents);
        }

        /// <summary>
        /// 获取子级节点
        /// </summary>
        public List<NetNode<T>> GetChildren()
        {
            return (m_children == null ? null : m_children);
        }

        /// <summary>
        /// 添加Child
        /// </summary>
        public void AddChild(NetNode<T> child)
        {
            if (m_children == null) m_children = new List<NetNode<T>>();

            if (!m_children.Contains(child))
            {
                m_children.Add(child);
                child.SetParent(this);
            }
        }

        /// <summary>
        /// 添加children
        /// </summary>
        public void AddChildren(ICollection<NetNode<T>> children)
        {
            if (m_children == null) m_children = new List<NetNode<T>>();

            if (children != null && children.Count > 0)
            {
                foreach (NetNode<T> child in children)
                {
                    AddChild(child);
                }
            }
        }

        /// <summary>
        /// 移除一个子级节点
        /// </summary>
        public bool RemoveChild(NetNode<T> child)
        {
            if (m_children == null) return false;

            if (m_children.Contains(child))
            {

                if (m_children.Remove(child))
                {
                    child.RemoveParent(this);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除子级节点
        /// </summary>
        public void RemoveChildren(ICollection<NetNode<T>> children)
        {
            if (m_children == null) return;

            if (children != null && children.Count > 0)
            {
                List<NetNode<T>> dels = new List<NetNode<T>>();
                foreach (NetNode<T> child in children)
                {
                    RemoveChild(child);
                }
            }
        }

    }

}


