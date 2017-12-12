using System;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity;

namespace YoukiaUnity
{
    public class YKSprite : MonoBehaviour
    {
        [SerializeField]
        private Material _material;

        public Vector3[] Vertices;
        public Color[] Colors;
        public Vector2[] TexCoords;

        public Material Material
        {
            get { return _material; }
            set
            {
                bool isAdd = _material != null;
                _material = value;
                if (!isAdd && isActiveAndEnabled)
                {
                    OnEnable();
                }
            }
        }

        protected void Awake()
        {
            Vertices = new[] 
            {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(-0.5f, 0.5f, 0),
                new Vector3(0.5f, 0.5f, 0),
                new Vector3(0.5f, -0.5f, 0)
            };
            TexCoords = new[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
            };
            Colors = new[]
            {
                new Color(1, 1, 1, 1), 
                new Color(1, 1, 1, 1), 
                new Color(1, 1, 1, 1), 
                new Color(1, 1, 1, 1)
            };
        }

        protected virtual void OnEnable()
        {
            if (Material != null)
            {
                YKSpriteManager.Instance.AddSprite(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (Material != null)
            {
                YKSpriteManager.Instance.RemoveSprite(this);
            }
        }

        protected virtual void OnDestroy()
        {
            OnDisable();
        }
    }
}
