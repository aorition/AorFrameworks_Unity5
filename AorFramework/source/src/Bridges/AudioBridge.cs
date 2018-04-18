using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework
{
    public class AudioBridge
    {

        #region Play

        public static Action<string> CustomPlay;
        public static Action<string, object[]> CustomPlayWithParams;

        public static void Play(string name)
        {

            if (CustomPlay != null)
            {
                CustomPlay(name);
                return;
            }

            //default:
        }

        public static void Play(string name, params object[] parms)
        {

            if (CustomPlayWithParams != null)
            {
                CustomPlayWithParams(name, parms);
                return;
            }

            //default:
        }

        #endregion

        #region PlayClip

        public static Action<AudioClip> CustomPlayClip;
        public static Action<AudioClip, object[]> CustomPlayClipWithParams;

        public static void PlayClip(AudioClip clip)
        {

            if (CustomPlayClip != null)
            {
                CustomPlayClip(clip);
                return;
            }

            //default:
            // do nothing
        }

        public static void PlayClip(AudioClip clip, params object[] parms)
        {

            if (CustomPlayClipWithParams != null)
            {
                CustomPlayClipWithParams(clip, parms);
                return;
            }

            //default:
            // do nothing
        }

        #endregion

    }
}
