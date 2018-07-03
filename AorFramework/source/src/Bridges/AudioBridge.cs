using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework
{
    public class AudioBridge
    {

        #region Play

        public static Action<string, object[]> PlayHook;

        public static void Play(string name, params object[] parms)
        {

            if (PlayHook != null)
            {
                PlayHook(name, parms);
                return;
            }

            //default:

        }

        #endregion

        #region PlayClip

        public static Action<AudioClip, object[]> PlayClipHook;

        public static void PlayClip(AudioClip clip, params object[] parms)
        {
            if (PlayClipHook != null)
            {
                PlayClipHook(clip, parms);
                return;
            }

            //default:
            // do nothing
        }

        #endregion

    }
}
