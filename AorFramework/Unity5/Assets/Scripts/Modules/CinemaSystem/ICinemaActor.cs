using UnityEngine;


namespace YoukiaUnity.CinemaSystem
{
    /// <summary>
    /// 演员接口
    /// </summary>
    public interface ICinemaActor
    {
        void ActorPlay(string AnimName);
        GameObject gameObject { get; }
        void Initialization(CinemaCharacter character,ICinemaBridge _bridge);
        void OnUpdate(CinemaCharacter character);
        void SetVisble(bool bo);
    }
}


