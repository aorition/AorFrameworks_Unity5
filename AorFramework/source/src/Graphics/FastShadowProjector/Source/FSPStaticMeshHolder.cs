﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Graphic.FastShadowProjector
{
    public class FSPStaticMeshHolder : MonoBehaviour
    {

        static FSPStaticMeshHolder _instance;
        Dictionary<int, Mesh> _meshLinks;
        [UnityEngine.SerializeField]
        public int receiverID;

        public static FSPStaticMeshHolder Get()
        {
            return _instance;
        }

        void Awake()
        {


            _instance = this;

            GameObject staticHolder = (GameObject)GameObject.Find("_FSPStaticHolder");
            if (staticHolder != null && staticHolder.GetInstanceID() != gameObject.GetInstanceID())
            {
                GameObject.Destroy(staticHolder);
            }
            //	print (gameObject.name);
            LinkMeshes();




        }

        public Mesh GetMesh(int receiverID)
        {
            return _meshLinks[receiverID];
        }

        void LinkMeshes()
        {
            _meshLinks = new Dictionary<int, Mesh>();

            Mesh mesh;
            Transform link;

            for (int n = 0; n < transform.childCount; n++)
            {
                link = transform.GetChild(n);
                mesh = link.GetComponent<MeshFilter>().mesh;
                int receiverID = int.Parse(link.name);
                _meshLinks[receiverID] = mesh;
            }
        }
    }
}


