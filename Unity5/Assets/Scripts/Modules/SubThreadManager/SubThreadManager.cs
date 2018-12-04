using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubThreadManager : MonoBehaviour
{
    private static SubThreadManager _instance;
    public static SubThreadManager Instance {
        get { return _instance; }
    }

    public static bool HasInstance {
        get {
            return _instance != null;
        }
    }

    public static SubThreadManager CreateInstance(GameObject target) {
        if (!_instance)
        {
            if (!target) target = new GameObject("SubThreadManager");
            _instance = target.GetComponent<SubThreadManager>();
            if (!_instance) _instance = target.AddComponent<SubThreadManager>();
        }
        return _instance;
    }

    //-------------------------------------------------

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }else if(_instance != this)
        {
            GameObject.Destroy(this);
        }
    }

    //public int MaxSubThradPipelineLimit = 8;
    //private Dictionary<string, SubThradJob> _pipelineDic = new Dictionary<string, SubThradJob>();
    
    public void StartSubThreadJob( SubThradJob job, Action onJobFinish = null) {
        job.Start();
        StartCoroutine(STJListenFunc(job, onJobFinish));
    }

    private IEnumerator STJListenFunc(SubThradJob job, Action onJobFinish) {
        yield return job.WaitTillDone();
        if (!job.isDisposed)
        {
            if (onJobFinish != null) onJobFinish();
        }
        job.Dispose();
    }

    private Queue<Action> _callbackQueue = new Queue<Action>();
    public void AddThreadCallback(Action callback)
    {
        lock (_callbackQueue)
        {
            _callbackQueue.Enqueue(callback);
        }
    }
    // Update is called once per frame
    void Update()
    {
        lock (_callbackQueue)
        {
            while (_callbackQueue.Count > 0)
            {
                _callbackQueue.Dequeue()();
            }
        }
    }

}