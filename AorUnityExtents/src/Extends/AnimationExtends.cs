using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Extends
{
    public static class AnimationExtends
    {

        public static AnimationState GetCurrentState(this Animation anim)
        {
            //TODO:注意,多通道播放时(比如使用CrossFade)可能同时有多个动画在不同通道上播放
            if (anim == null)
            {
                return null;
            }
            foreach (AnimationState state in anim)
            {
                if (anim.IsPlaying(state.name))
                {
                    return state;
                }
            }
            return null;
        }

    }
}
