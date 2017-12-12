using System;

using UnityEngine;

namespace YoukiaUnity.CinemaSystem
{
    /// <summary>
    /// Cinema 龙套 
    /// 
    /// （不解释 = =b）
    /// 
    /// </summary>
    public class CinemaLongtao : MonoBehaviour, IEditorOnlyScript
    {

        public bool IsDestroyed = false;

        private void Awake()
        {
            if (Application.isPlaying)
            {

                if (transform.parent)
                {
                    CinemaCharacter character = transform.parent.GetComponent<CinemaCharacter>();
                    if (character)
                    {
                        if (!string.IsNullOrEmpty(character.ActorLoadPath) || character.UseActorIDPreBinding)
                        {
                            IsDestroyed = true;
                            GameObject.DestroyObject(this.gameObject);
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                IsDestroyed = true;
                GameObject.DestroyObject(this.gameObject);
            }
        }
    }
}
