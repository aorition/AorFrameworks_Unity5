using System;
using UnityEngine;

namespace Framework.AnimLinkage
{
    public interface ISimulatableAnimator
    {
        void SampleAnimation(AnimationClip clip, float time);
    }
}
