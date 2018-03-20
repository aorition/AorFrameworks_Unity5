using System;
using UnityEngine;
using System.Collections.Generic;
using YoukiaCore;
using YoukiaUnity.View;
using Object = UnityEngine.Object;

public class EffectManager : MonoBehaviour
{
    public Dictionary<string, Queue<GameObject>> CachedEffects { get { return _cachedEffects; } }
    private readonly Dictionary<string, Queue<GameObject>> _cachedEffects = new Dictionary<string, Queue<GameObject>>();
    //private readonly Dictionary<string, GameObject> _prototypes = new Dictionary<string, GameObject>();

    private readonly Dictionary<GameObject, CachedEffect> _cachedEffectKV = new Dictionary<GameObject, CachedEffect>();
    private readonly LinkedList<CachedEffect> _managedEffects = new LinkedList<CachedEffect>();
    private static EffectManager _instance;

    public const float MAX_EFFECT_TIME = 5;

    public static EffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("EffectManager");
                _instance = go.AddComponent<EffectManager>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }
    }

    public class CachedEffect
    {
        public string Path;
        public float LifeTime;
        public GameObject Object;

        public float TimePassed;
    }

    //新建Effect会直接放入Hide层
    public bool HidingLayerAtNewEffect = false;

    /**
    * 创建时生命周期为0的效果必须主动销毁
    */
    public void CreateEffect(string path, bool isAutoDestroy, bool userCacheEffect, CallBack<GameObject> call)
    {
        if (path.IsNullOrEmpty()) return;

        GameObject goo = null;
        Queue<GameObject> cache = null;
        if (_cachedEffects.TryGetValue(path, out cache))
        {
            if (cache.Count != 0)
            {
                goo = cache.Dequeue();
            }
        }
        else
        {
            _cachedEffects.Add(path, new Queue<GameObject>());
        }

        if (goo == null)
        {
            if (!userCacheEffect)
            {
                Utils.LoadObj(Paths.getPathSwitch(path), (go) =>
                {
                    go.transform.parent = transform;
                    EffectDescript effectArgs = go.GetComponent<EffectDescript>();
                    CachedEffect effect = new CachedEffect();
                    effect.Path = path;
                    effect.LifeTime = isAutoDestroy ? (effectArgs == null ? MAX_EFFECT_TIME : effectArgs.SurvivalTime == 0 ? MAX_EFFECT_TIME : effectArgs.SurvivalTime) : 0;
                    effect.TimePassed = 0;
                    effect.Object = go;
                    _cachedEffectKV[go] = effect;
                    _managedEffects.AddFirst(effect);
                    if (HidingLayerAtNewEffect && !effectArgs.isInHideLayer)
                    {
                        effectArgs.isInHideLayer = true;
                        effectArgs.registLayerMask = go.layer;
                        go.layer = 1 << LayerMask.NameToLayer("hide");
                    }
                    if (call != null)
                        call(go);
                });
            }
            else
            {
                Utils.LoadBattleEffectObj(Paths.getPathSwitch(path), (go) =>
                {
                    go.transform.parent = transform;
                    EffectDescript effectArgs = go.GetComponent<EffectDescript>();
                    CachedEffect effect = new CachedEffect();
                    effect.Path = path;
                    effect.LifeTime = isAutoDestroy ? (effectArgs == null ? MAX_EFFECT_TIME : effectArgs.SurvivalTime == 0 ? MAX_EFFECT_TIME : effectArgs.SurvivalTime) : 0;
                    effect.TimePassed = 0;
                    effect.Object = go;
                    _cachedEffectKV[go] = effect;
                    _managedEffects.AddFirst(effect);
                    if (HidingLayerAtNewEffect && !effectArgs.isInHideLayer)
                    {
                        effectArgs.isInHideLayer = true;
                        effectArgs.registLayerMask = go.layer;
                        go.layer = 1 << LayerMask.NameToLayer("hide");
                    }
                    if (call != null)
                        call(go);
                });
            }
        }
        else
        {
            EffectDescript effectArgs = goo.GetComponent<EffectDescript>();
            CachedEffect effect = new CachedEffect();
//            if (effect == null)
//            {
//                effect = goo.AddComponent<CachedEffect>();
//            }
            effect.Path = path;
            effect.LifeTime = isAutoDestroy ? (effectArgs == null ? MAX_EFFECT_TIME : effectArgs.SurvivalTime == 0 ? MAX_EFFECT_TIME : effectArgs.SurvivalTime) : 0;
            effect.TimePassed = 0;
            effect.Object = goo;
            _cachedEffectKV[goo] =  effect;
            _managedEffects.AddFirst(effect);
            if (HidingLayerAtNewEffect && !effectArgs.isInHideLayer)
            {
                effectArgs.isInHideLayer = true;
                effectArgs.registLayerMask = goo.layer;
                goo.transform.SetLayer(1 << LayerMask.NameToLayer("hide"));
            }
            if (call != null)
                call(goo);
        }
    }

    //获取 EffectManager管理的所有的Effect;
    public List<GameObject> GetAllEffectObjects()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (CachedEffect effect in _managedEffects)
        {
            if (effect != null)
            {
                list.Add(effect.Object);
            }
        }
        foreach (Queue<GameObject> cache in _cachedEffects.Values)
        {
            if (cache != null && cache.Count > 0)
            {
                list.AddRange(cache.ToArray());
            }
        }
        if (list.Count > 0)
        {
            return list;
        }
        return null;
    }

    public void DestroyEffect(GameObject effect)
    {
        if (!effect)
        {
            return;
        }

        CachedEffect cacheInfo = _cachedEffectKV[effect];
        if (cacheInfo == null)
        {
            return;
        }

        effect.SetActive(false);
        effect.transform.SetParent(transform, false);
        _cachedEffects[cacheInfo.Path].Enqueue(effect);
        _managedEffects.Remove(cacheInfo);
//        _cachedEffectKV.Remove(effect);

        EffectDescript ed = effect.GetComponent<EffectDescript>();

        if (ed && ed.trails != null)
        {
            for (int i = 0; i < ed.trails.Length; i++)
            {
                if(ed.trails[i] == null) continue;
                ed.trails[i].ClearSystem(true);
            }
        }
    }

    /// <summary>
    /// 全局控制 非挂载到角色身上的特效 隐藏/显示
    /// </summary>
    /// <param name="visible">是否显示</param>
    public void SetAllNoneRoleEffectsVisible(bool visible)
    {
        //获取当前所有的特效对象
        List<GameObject> elist = GetAllEffectObjects();
        if (elist != null)
        {
            int i, len = elist.Count;
            for (i = 0; i < len; i++)
            {
                //过滤掉 为null 和 没激活的
                //                if (elist[i] && elist[i].activeInHierarchy)
                //过滤掉为激活会出Bug
                if (elist[i])
                {
                    //抓取特效对象标识
                    EffectDescript ed = elist[i].GetComponent<EffectDescript>();
                    if (ed)
                    {
                        //过滤掉 挂载在角色身上的特效对象
                        if (!ed.EffectRootGameObject)
                        {
                            if (visible)
                            {
                                //显示
                                if (ed.isInHideLayer)
                                {
                                    ed.isInHideLayer = false;
                                    ed.transform.SetLayer(ed.registLayerMask);
                                }
                            }
                            else
                            {
                                //隐藏
                                if (!ed.isInHideLayer)
                                {
                                    ed.isInHideLayer = true;
                                    ed.registLayerMask = ed.gameObject.layer;
                                    ed.transform.SetLayer(LayerMask.NameToLayer("hide"));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 全局控制 挂载到角色身上的特效 隐藏/显示
    /// 
    /// 特效随角色 visible 而决定是否显示
    /// 
    /// </summary>
    public void SetAllEffectWithRolesVisible()
    {
        //获取当前所有的特效对象
        List<GameObject> elist = GetAllEffectObjects();
        if (elist != null)
        {
            int i, len = elist.Count;
            for (i = 0; i < len; i++)
            {
                //过滤掉 为null 和 没激活的
                if (elist[i] && elist[i].activeInHierarchy)
                {
                    //抓取特效对象标识
                    EffectDescript ed = elist[i].GetComponent<EffectDescript>();
                    if (ed)
                    {
                        //过滤掉 不是挂载在角色身上的特效对象
                        if (ed.EffectRootGameObject)
                        {
                            ObjectView ov = ed.EffectRootGameObject.GetComponent<ObjectView>();
                            if (ov.IsVisible)
                            {
                                //显示
                                if (ed.isInHideLayer)
                                {
                                    ed.isInHideLayer = false;
                                    ed.transform.SetLayer(ed.registLayerMask);
                                }
                            }
                            else
                            {
                                //隐藏
                                if (!ed.isInHideLayer)
                                {
                                    ed.isInHideLayer = true;
                                    ed.registLayerMask = ed.gameObject.layer;
                                    ed.transform.SetLayer(LayerMask.NameToLayer("hide"));
                                }
                            }
                        }
                        

                    }
                }

            }
        }
    }

    protected void Update()
    {
        LinkedListNode<CachedEffect> node = _managedEffects.First;
        while (node != null)
        {
            LinkedListNode<CachedEffect> next = node.Next;
            CachedEffect effect = node.Value;
            effect.TimePassed += Time.deltaTime;
            if ((effect.LifeTime != 0 && effect.TimePassed >= effect.LifeTime))
            {
                if (effect.Object)
                {

                    effect.Object.SetActive(false);
                    effect.Object.transform.SetParent(transform, false);
                    _cachedEffects[effect.Path].Enqueue(effect.Object);

                    //恢复layer
                    _resetEffectLayer(effect.Object);

                }
                _managedEffects.Remove(node);
            }
            node = next;
        }
    }

    public void ClearManagedEffect()
    {
        foreach (CachedEffect effect in _managedEffects)
        {
            if (effect != null && effect.Object)
            {
                effect.Object.SetActive(false);
                effect.Object.transform.SetParent(transform, false);
                _cachedEffects[effect.Path].Enqueue(effect.Object);

                //恢复layer
                _resetEffectLayer(effect.Object);

            }
        }
        _managedEffects.Clear();
    }

    private void _resetEffectLayer(GameObject effect)
    {
        EffectDescript ed = effect.GetComponent<EffectDescript>();
        if (ed)
        {
            if (ed.isInHideLayer)
            {
                ed.isInHideLayer = false;
                ed.transform.SetLayer(ed.registLayerMask);
            }
        }
    }

    public void Clear()
    {
        ClearManagedEffect();
        _cachedEffectKV.Clear();
        foreach (Queue<GameObject> cache in _cachedEffects.Values)
        {
            while (cache.Count != 0)
            {
                GameObject go = cache.Dequeue();
                Object.Destroy(go);
            }
        }
        //_prototypes.Clear();
    }
}