using System;
using UnityEngine;
using System.Collections.Generic;
using Framework;


namespace YoukiaUnity.CinemaSystem
{

    /// <summary>
    /// 剪辑其他接口
    /// </summary>
    public interface ICinemaBridge
    {
        void OnClipStart();
        void OnClipPlay();
        void OnClipEnd();
        IGameAnimObject LoadPlayer(string playerID);
        IGameAnimObject CheckLongTao(CinemaCharacter character);
        void InitActor(CinemaCharacter character,Action finish);
        void addHidePrefab(GameObject prefab);
        void setupValues(string valuesStr);
        string ExportValuesString();
    }
}


