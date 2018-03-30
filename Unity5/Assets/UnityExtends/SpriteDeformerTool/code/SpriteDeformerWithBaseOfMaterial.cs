using UnityEngine;
using System.Collections;
namespace Medvedya.SpriteDeformerTools
{
    public abstract class SpriteDeformerWithBaseOfMaterial:SpriteDeformer
    {
        public Material referenceMaterial 
        {
            get {
                return _referenceMaterial;
            }
            set
            {
                if (value == referenceMaterial) return;
                if (Application.isPlaying)
                {
                    if (sprite != null && referenceMaterial != null)
                    {
                        BaseOfMaterials.IDestory(referenceMaterial, sprite.texture);
                    }
                    if (sprite != null && value != null)
                    {
                        meshRender.sharedMaterial = BaseOfMaterials.GetMaterial(value, sprite.texture);
                    }
                }
               _referenceMaterial = value;
               if (!Application.isPlaying)
               {
                   //if (meshRender.sharedMaterial!=null)
                  // DestroyImmediate(meshRender.sharedMaterial);
                   // = null;
                   meshRender.sharedMaterial = null;
                   Update();
                  // return;
                   getEditorMaterial();
                 
               }
            }
        }
        [SerializeField]
        private Material _referenceMaterial;

        protected override void Update()
        {
            if (!Application.isPlaying && GetComponent<Renderer>().isVisible)
            {
                
                getEditorMaterial();
                //Debug.Log(referenceMaterial);
            }
            base.Update();
        }
        private void getEditorMaterial()
        {
              if (referenceMaterial == null || sprite == null) return;
                if (meshRender.sharedMaterial == null)
                {
                    meshRender.sharedMaterial = (Material)Material.Instantiate(referenceMaterial);
                    meshRender.sharedMaterial.name = referenceMaterial.name + "_editor_" + Random.Range(0, 1000).ToString();
                }
                meshRender.sharedMaterial.shader = referenceMaterial.shader;
                meshRender.sharedMaterial.CopyPropertiesFromMaterial(referenceMaterial);
                meshRender.sharedMaterial.mainTexture = sprite.texture;
                
        }
        protected override void Awake()
        {
            if (!Application.isPlaying && meshRender!=null)
            {
                
                meshRender.sharedMaterial = null;
                getEditorMaterial();
            }
            base.Awake();
            if (Application.isPlaying && referenceMaterial != null && sprite != null)
            {
                meshRender.sharedMaterial = BaseOfMaterials.GetMaterial(referenceMaterial, sprite.texture);
            }
           
        }
        protected override void onSpriteChange(Sprite lastSprite, Sprite currentSprite)
        {
            if (Application.isPlaying)
            {
                if (lastSprite != null && currentSprite != null)
                {
                    if (lastSprite.texture != currentSprite.texture)
                    {

                        BaseOfMaterials.IDestory(referenceMaterial, lastSprite.texture);
                        meshRender.sharedMaterial = BaseOfMaterials.GetMaterial(referenceMaterial, currentSprite.texture);
                    }
                }
                else
                {
                    if (lastSprite != null)
                    {
                        BaseOfMaterials.IDestory(referenceMaterial, lastSprite.texture);
                    }
                    if (currentSprite != null)
                    {
                        meshRender.sharedMaterial = BaseOfMaterials.GetMaterial(referenceMaterial, currentSprite.texture);

                    }
                }
            }
            base.onSpriteChange(lastSprite, currentSprite);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!Application.isPlaying)
            {
                if (meshRender.sharedMaterial != null)
                {
                    Object.DestroyImmediate(meshRender.sharedMaterial);
                }
            }
            if (Application.isPlaying)
            {

                if (sprite!=null)
                BaseOfMaterials.IDestory(referenceMaterial, sprite.texture);
            }
        }
    }
}
