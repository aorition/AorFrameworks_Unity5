using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ExoticUnity.GUI.AorUI.Components;

namespace ExoticUnity.GUI.AorUI.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleSpritePlayer : Simple2DPlayer
    {

        private SpriteRenderer _image;
        private bool _hasImage;

        protected override void Initialization()
        {
            
            if (_image == null)
            {
                _image = GetComponent<SpriteRenderer>();
                if (_image == null)
                {
                    _image = gameObject.AddComponent<SpriteRenderer>();
                    _hasImage = false;
                    imageHide(true);
                }
                else
                {
                    _hasImage = true;
                }
            }

            base.Initialization();

        }

        private void imageHide(bool isHide)
        {
            if (_image == null)
                return;

            if (isHide)
            {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
            }
            else
            {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
            }
        }


        protected override void updateSprite()
        {

            if (!_hasImage)
            {
                imageHide(false);
            }
            else
            {

                _image.sprite = _PimgList[_frameNum];


            }

        }

        protected override void OnStop()
        {
            base.OnStop();
            if (!_hasImage)
            {
                imageHide(true);
            }

        }

    }
}
