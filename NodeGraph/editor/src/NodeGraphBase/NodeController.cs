using System;
using System.Collections.Generic;
using AorBaseUtility.Config;
using NodeGraph.SupportLib;

namespace Framework.NodeGraph
{
    //NodeData 控制器
    public class NodeController : INodeController
    {

        public NodeController(){}

        public void setup(NodeGUI node)
        {
            m_nodeGUI = node;
            _isInit = true;
        }

        protected bool _isInit = false;
        public bool isInit
        {
            get { return _isInit; }
        }

        protected NodeGUI m_nodeGUI;
        public NodeGUI nodeGUI
        {
            get { return m_nodeGUI; }
        }

        public virtual string nodeDataToString()
        {
            string output = JConfigParser.ToJSON((Config)m_nodeGUI.data);
            string tagHead = "";
            output = JConfigParser.splitJsonHeadTag(output, ref tagHead);

            return output;
        }

        public virtual void update(bool updateParentLoop = true)
        {
            _updateAfterDo();
        }

        /// <summary>
        /// update后必须做的系列操作
        /// </summary>
        private void _updateAfterDo()
        {
            if (m_nodeGUI != null)
            {
                m_nodeGUI.SetDirty(false);
            }
        }


    }
}
