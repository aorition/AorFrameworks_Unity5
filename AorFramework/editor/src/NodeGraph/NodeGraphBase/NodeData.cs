using System;
using System.Collections.Generic;
using AorBaseUtility;
using AorFramework.core;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class NodeData : Config, INodeData
    {

        public NodeData()
        {
        }

        public NodeData(long id)
        {
            this.ref_SetField_Inst_Public("id", id);
        }

        protected bool m_isDirty = false;
        public bool isDirty
        {
            get { return m_isDirty; }
            set { m_isDirty = value; }
        }

//        [JConfigParms()]
        public long id
        {
            get { return base.id; }
        }

        [JConfigParms()]
        protected string m_name;
        public string name
        {
            get { return m_name; }
            set { m_name = value; }
        }

//        [JConfigParms()]
//        protected Vector2 m_size = new Vector2(NodeGraphDefind.NodeGUIMinSizeX, NodeGraphDefind.NodeGUIMinSizeY);
//        public Vector2 size
//        {
//            get { return m_size;}
//            set { m_size = value; }
//        }
//
//        [JConfigParms()]
//        protected Vector2 m_position = Vector2.zero;
//        public Vector2 position
//        {
//            get { return m_position; }
//            set { m_position = value; }
//        }

//        public NodeBehaviour Behaviour;
//
//        public INodeData parentNode;
//
//        public List<INodeData> ChildNodes;

    }
}
