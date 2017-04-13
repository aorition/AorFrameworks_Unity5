using System;
using System.Collections.Generic;

namespace AorBaseUtility
{

    /// <summary>
    /// 树状节点数据模型
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class TreeNode<T> where T : class
    {
        /// <summary>
        /// 检查交叉引用
        /// （检查 参考节点 是否在树模型中存在重复引用）
        /// </summary>
        /// <param name="treeNode">已有的树模型</param>
        /// <param name="target">参考节点</param>
        public static bool CheckCrossReferences(TreeNode<T> treeNode, TreeNode<T> target)
        {
            bool result = false;
            //向上查找
            if (treeNode.m_parent != null)
            {
                result = findCrossReferencesUpper(treeNode.m_parent, target.value);
            }

            if (result) return true;

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

        private static bool findCrossReferencesUpper(TreeNode<T> node, T value)
        {
            if (node.value == value) return true;

            if (node.m_parent != null)
            {
                return findCrossReferencesUpper(node.m_parent, value);
            }
            else
            {
                return false;
            }
        }

        private static bool findCrossReferencesDowner(TreeNode<T> node, T value)
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

        private TreeNode<T> m_parent;
        private List<TreeNode<T>> m_children;

        private T m_value;

        public T value
        {
            get { return m_value; }
        }

        public TreeNode(T value)
        {

            m_value = value;

            if (m_parent != null)
            {
                m_parent.AddChild(this);
            }

        }

        public TreeNode(T value, TreeNode<T> parent, ICollection<TreeNode<T>> children)
        {
            m_value = value;
            m_parent = parent;
            if (m_children == null) m_children = new List<TreeNode<T>>(children);

            if (m_parent != null)
            {
                m_parent.AddChild(this);
            }
        }

        //析构
        ~TreeNode()
        {
            if (m_parent != null)
            {
                m_parent.RemoveChild(this);
            }
        }

        /// <summary>
        /// 设置上级节点
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(TreeNode<T> parent)
        {
            if (parent != null)
            {
                m_parent = parent;
                m_parent.AddChild(this);
            }
            else
            {
                m_parent = null;
            }
        }

        /// <summary>
        /// 获取上级节点
        /// </summary>
        public TreeNode<T> GetParent()
        {
            return m_parent;
        }

        /// <summary>
        /// 获取子级节点
        /// </summary>
        public List<TreeNode<T>> GetChildren()
        {
            return (m_children == null ? null : m_children);
        }

        /// <summary>
        /// 添加Child
        /// </summary>
        public void AddChild(TreeNode<T> child)
        {
            if (m_children == null) m_children = new List<TreeNode<T>>();

            if (!m_children.Contains(child))
            {
                m_children.Add(child);
            }
        }

        /// <summary>
        /// 添加children
        /// </summary>
        public void AddChildren(ICollection<TreeNode<T>> children)
        {
            if (m_children == null) m_children = new List<TreeNode<T>>();

            if (children != null && children.Count > 0)
            {
                foreach (TreeNode<T> child in children)
                {
                    if (!m_children.Contains(child))
                    {
                        m_children.Add(child);
                    }
                }
            }
        }

        /// <summary>
        /// 移除一个子级节点
        /// </summary>
        public bool RemoveChild(TreeNode<T> child)
        {
            if (m_children == null) return false;

            if (m_children.Contains(child))
            {
                if (child.GetParent() != null)
                {
                    child.SetParent(null);
                }
                return m_children.Remove(child);
            }

            return false;
        }

        /// <summary>
        /// 移除子级节点
        /// </summary>
        public void RemoveChildren(ICollection<TreeNode<T>> children)
        {
            if (m_children == null) return;

            if (children != null && children.Count > 0)
            {
                List<TreeNode<T>> dels = new List<TreeNode<T>>();
                foreach (TreeNode<T> child in children)
                {
                    if (!m_children.Contains(child))
                    {
                        dels.Add(child);
                    }
                }

                int i, len = dels.Count;
                for (i = 0; i < len; i++)
                {
                    m_children.Remove(dels[i]);
                }
            }
        }

    }

}


