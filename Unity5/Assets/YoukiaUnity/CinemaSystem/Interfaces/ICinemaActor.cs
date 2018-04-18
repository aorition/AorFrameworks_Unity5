using System;
using Framework;
using UnityEngine;

namespace YoukiaUnity.CinemaSystem
{
    /// <summary>
    /// 演员接口
    /// </summary>
    public interface ICinemaActor
    {
        IGameAnimObject View { get; set; }
        void ActorPlay(string AnimName);
        GameObject gameObject { get; }
        void Initialization(CinemaCharacter character,ICinemaBridge _bridge, Action finish = null);
        void OnUpdate(CinemaCharacter character);
        void SetVisible(bool bo);
    }
}


