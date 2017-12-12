﻿using System;
using UnityEngine;
using System.Collections.Generic;
using YoukiaUnity.View;


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
        ObjectView LoadPlayer(string playerID);
        ObjectView CheckLongTao(CinemaCharacter character);
        void InitActor(CinemaCharacter character,Action finish);
        void addHidePrefab(GameObject prefab);
    }
}


