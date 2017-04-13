using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YoukiaUnity
{
    public class YKSpriteManager : MonoBehaviour
    {
        //TODO:处理顶点超过VBO上限或过多导致重置Mesh开销巨大的情况
        public const int DEFAULT_SPRITE_BUFFER_COUNT = 8;

        private class SpriteEntity
        {
            public GameObject Entity;
            public Mesh Mesh;
            public Vector3[] Vertices;
            public Vector2[] TexCoords;
            public Color[] Colors;
            public int[] Indices;

            private int _bufferCount;
            private readonly List<YKSprite> _sprites = new List<YKSprite>();

            public int Count
            {
                get { return _sprites.Count; }
            }

            public SpriteEntity(Material mat)
            {
                Entity = new GameObject("YKSpriteEntity@" + mat.name);
                Mesh = new Mesh { name = "YKSpriteMesh@" + mat.name };

                Entity.AddComponent<MeshRenderer>().material = mat;
                Entity.AddComponent<MeshFilter>().mesh = Mesh;

                _bufferCount = DEFAULT_SPRITE_BUFFER_COUNT;
                _InitAttributes();
            }

            private void _InitAttributes()
            {
                Vertices = new Vector3[_bufferCount * 4];
                TexCoords = new Vector2[_bufferCount * 4];
                Colors = new Color[_bufferCount * 4];
                Indices = new int[_bufferCount * 6];

                for (int i = 0; i < _bufferCount; i++)
                {
                    TexCoords[i * 4] = new Vector2(0, 0);
                    TexCoords[i * 4 + 1] = new Vector2(0, 1);
                    TexCoords[i * 4 + 2] = new Vector2(1, 1);
                    TexCoords[i * 4 + 3] = new Vector2(1, 0);

                    Colors[i * 4] = new Color(1, 1, 1, 1);
                    Colors[i * 4 + 1] = new Color(1, 1, 1, 1);
                    Colors[i * 4 + 2] = new Color(1, 1, 1, 1);
                    Colors[i * 4 + 3] = new Color(1, 1, 1, 1);

                    Indices[i * 6] = i * 4;
                    Indices[i * 6 + 1] = i * 4 + 1;
                    Indices[i * 6 + 2] = i * 4 + 3;
                    Indices[i * 6 + 3] = i * 4 + 3;
                    Indices[i * 6 + 4] = i * 4 + 1;
                    Indices[i * 6 + 5] = i * 4 + 2;
                }

                Mesh.Clear();
                Mesh.vertices = Vertices;
                Mesh.uv = TexCoords;
                Mesh.colors = Colors;
                Mesh.triangles = Indices;
            }

            public void Update()
            {
                for (int i = 0; i < _sprites.Count; i++)
                {
                    Matrix4x4 worldMatrix = _sprites[i].transform.localToWorldMatrix;

                    Vertices[i * 4] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[0]);
                    Vertices[i * 4 + 1] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[1]);
                    Vertices[i * 4 + 2] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[2]);
                    Vertices[i * 4 + 3] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[3]);

                    Colors[i * 4] = _sprites[i].Colors[0];
                    Colors[i * 4 + 1] = _sprites[i].Colors[1];
                    Colors[i * 4 + 2] = _sprites[i].Colors[2];
                    Colors[i * 4 + 3] = _sprites[i].Colors[3];

                    TexCoords[i * 4] = _sprites[i].TexCoords[0];
                    TexCoords[i * 4 + 1] = _sprites[i].TexCoords[1];
                    TexCoords[i * 4 + 2] = _sprites[i].TexCoords[2];
                    TexCoords[i * 4 + 3] = _sprites[i].TexCoords[3];
                }
                Mesh.vertices = Vertices;
                Mesh.uv = TexCoords;
                Mesh.colors = Colors;
                Mesh.RecalculateBounds();
            }

            public void AddSprite(YKSprite sprite)
            {
                if (_sprites.Contains(sprite))
                {
                    return;
                }

                _sprites.Add(sprite);

                if (_bufferCount < _sprites.Count)
                {
                    _bufferCount *= 2;
                    _InitAttributes();
                }
            }

            public void RemoveSprite(YKSprite sprite)
            {
                if (!_sprites.Remove(sprite))
                {
                    return;
                }
                if (_bufferCount > DEFAULT_SPRITE_BUFFER_COUNT && _bufferCount > _sprites.Count * 4)
                {
                    _bufferCount /= 2;
                    _InitAttributes();
                }
                else
                {
                    Vertices[_sprites.Count * 4] = Vector3.zero;
                    Vertices[_sprites.Count * 4 + 1] = Vector3.zero;
                    Vertices[_sprites.Count * 4 + 2] = Vector3.zero;
                    Vertices[_sprites.Count * 4 + 3] = Vector3.zero;
                }
            }

            public void Dispose()
            {
                Destroy(Entity);
                Destroy(Mesh);
                _sprites.Clear();
            }
        }

        private readonly Dictionary<Material, SpriteEntity> _entitys = new Dictionary<Material, SpriteEntity>();

        private static YKSpriteManager _instance;
        public static YKSpriteManager Instance
        {
            get { return _instance ?? (_instance = new GameObject("YKSpriteManager").AddComponent<YKSpriteManager>()); }
        }

        protected void Awake()
        {
        }

        protected void Update()
        {
            foreach (SpriteEntity entity in _entitys.Values)
            {
                entity.Update();
            }
        }

        public void AddSprite(YKSprite sprite)
        {
            SpriteEntity entity;
            if (!_entitys.TryGetValue(sprite.Material, out entity))
            {
                entity = new SpriteEntity(sprite.Material);
                entity.Entity.transform.SetParent(transform, false);
                entity.Entity.layer = sprite.gameObject.layer;
                _entitys.Add(sprite.Material, entity);
            }
            entity.AddSprite(sprite);
        }

        public void RemoveSprite(YKSprite sprite)
        {
            SpriteEntity entity;
            if (_entitys.TryGetValue(sprite.Material, out entity))
            {
                entity.RemoveSprite(sprite);
                if (entity.Count == 0)
                {
                    _entitys.Remove(sprite.Material);
                    entity.Dispose();
                }
            }
        }
    }
}
