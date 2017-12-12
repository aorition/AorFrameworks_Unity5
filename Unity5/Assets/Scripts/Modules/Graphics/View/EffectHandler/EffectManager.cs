using UnityEngine;
using System.Collections.Generic;
using System;
using YoukiaUnity.View;
using Object = UnityEngine.Object;

public class EffectManager : MonoBehaviour
{
    private readonly Dictionary<string, Queue<GameObject>> _cachedEffects = new Dictionary<string, Queue<GameObject>>();
    private readonly Dictionary<string, GameObject> _prototypes = new Dictionary<string, GameObject>();

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

    public class CachedEffect : MonoBehaviour
    {
        public string Path;
        public float LifeTime;

        public float TimePassed;
    }

    /**
    * 创建时生命周期为0的效果必须主动销毁
    */
    public GameObject CreateEffect(string path, bool isAutoDestroy)
    {
        GameObject go = null;
        Queue<GameObject> cache = null;
        if (_cachedEffects.TryGetValue(path, out cache))
        {
            if (cache.Count != 0)
            {
                go = cache.Dequeue();
            }
        }
        else
        {
            _cachedEffects.Add(path, new Queue<GameObject>());
        }

        if (go == null)
        {
            GameObject prototype = null;
            if (!_prototypes.TryGetValue(path, out prototype))
            {
                //TODO:预加载预制体
                prototype = Resources.Load<GameObject>(path);
                prototype.SetActive(false);
                _prototypes.Add(path, prototype);
            }

            go = Instantiate(prototype);

            go.transform.parent = transform;
        }

        EffectDescript effectArgs = go.GetComponent<EffectDescript>();
        CachedEffect effect = go.GetComponent<CachedEffect>();
        if (effect == null)
        {
            effect = go.AddComponent<CachedEffect>();
        }

        effect.Path = path;
        effect.LifeTime = isAutoDestroy ? (effectArgs == null ? MAX_EFFECT_TIME : effectArgs.SurvivalTime == 0 ? MAX_EFFECT_TIME : effectArgs.SurvivalTime) : 0;
        effect.TimePassed = 0;
        _managedEffects.AddFirst(effect);

        return go;
    }

    public void DestroyEffect(GameObject effect)
    {
        CachedEffect cacheInfo = effect.GetComponent<CachedEffect>();
        if (cacheInfo == null)
        {
            return;
        }

        effect.SetActive(false);
        effect.transform.SetParent(transform, false);
        _cachedEffects[cacheInfo.Path].Enqueue(effect);
        _managedEffects.Remove(cacheInfo);

        EffectDescript ed = effect.GetComponent<EffectDescript>();

        if (ed.trails != null)
        {
            for (int i = 0; i < ed.trails.Length; i++)
            {
                ed.trails[i].ClearSystem(true);
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
                if (effect.gameObject != null)
                {
                    effect.gameObject.SetActive(false);
                    effect.gameObject.transform.parent = transform;
                    _cachedEffects[effect.Path].Enqueue(effect.gameObject);
                }
                _managedEffects.Remove(node);
            }
            node = next;
        }
    }

    public void Clear()
    {
        foreach (CachedEffect effect in _managedEffects)
        {
            if (effect != null)
            {
                effect.gameObject.SetActive(false);
                effect.gameObject.transform.parent = transform;
                _cachedEffects[effect.Path].Enqueue(effect.gameObject);
            }
        }
        _managedEffects.Clear();

        foreach (Queue<GameObject> cache in _cachedEffects.Values)
        {
            while (cache.Count != 0)
            {
                GameObject go = cache.Dequeue();
                Object.Destroy(go);
            }
        }
        _prototypes.Clear();
    }
}