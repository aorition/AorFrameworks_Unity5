using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Medvedya.SpriteDeformerTools
{
    public class BaseOfMaterials
    {
        private static Dictionary<Material, MaterialBaseElement> materialList = new Dictionary<Material, MaterialBaseElement>();
        public class MaterialBaseElement
        {
            public int count = 0;
            public Material refMaterial;
            public Dictionary<Texture, TextureBaseElement> materialsByTexture = new Dictionary<Texture, TextureBaseElement>();
            
            public Material GetMaterialByTexture(Texture texture)
            {
                if (!materialsByTexture.ContainsKey(texture))
                {
                    materialsByTexture[texture] = new TextureBaseElement();
                    materialsByTexture[texture].material = (Material)Material.Instantiate(refMaterial);
                    materialsByTexture[texture].material.mainTexture = texture;
                    materialsByTexture[texture].material.name += "_batch";
                }
                materialsByTexture[texture].count++;
                count++;
                return materialsByTexture[texture].material;
            }
            
        }
        public class TextureBaseElement
        {
            public int count = 0;
            public Material material;

        }
        internal static Material GetMaterial(Material referenceMaterial, Texture texture)
        {

            if (referenceMaterial == null) return null;
            Material m = null;
            if (materialList.ContainsKey(referenceMaterial))
            {
                m = materialList[referenceMaterial].GetMaterialByTexture(texture);
            }
            else
            {
                materialList[referenceMaterial] = new MaterialBaseElement();
                materialList[referenceMaterial].refMaterial = referenceMaterial;
                m = materialList[referenceMaterial].GetMaterialByTexture(texture);
            }
            return m;
        }
        public static void IDestory(Material refMaterial, Texture texture)
        {
            MaterialBaseElement mbe = null;//materialList[refMaterial];
            if (!materialList.TryGetValue(refMaterial, out mbe))
            {
                return;
            }
            mbe.materialsByTexture[texture].count--;
            if (mbe.materialsByTexture[texture].count == 0)
            {
                Object.DestroyImmediate(mbe.materialsByTexture[texture].material);
                mbe.materialsByTexture.Remove(texture);
            }
            mbe.count--;
            if (mbe.count == 0)
            {
                materialList.Remove(refMaterial);
            }
        }
        public static Material[] GetAllMaterialsByReferenceMaterial(Material referensMaterial)
        {
            List<Material> materials = new List<Material>();
            if (materialList.ContainsKey(referensMaterial))
            {
                MaterialBaseElement mbe = materialList[referensMaterial];
                foreach (TextureBaseElement tb in mbe.materialsByTexture.Values)
                {
                    materials.Add(tb.material);
                }
            }
            return materials.ToArray();
        }

    }
}
