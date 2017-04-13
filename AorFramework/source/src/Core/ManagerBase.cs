using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework
{
    /// <summary>
    /// 基于MonoBehaviour的Manager的基类
    /// </summary>
    public abstract class ManagerBase : MonoBehaviour, IManager
    {

        protected virtual void Awake()
        {
            if (!AorFacede.Instance.RegisterManager(this))
            {
                Dispose();
            }
        }
        
        public virtual void Dispose()
        {
            GameObject.Destroy(this);
        }
    }
}
