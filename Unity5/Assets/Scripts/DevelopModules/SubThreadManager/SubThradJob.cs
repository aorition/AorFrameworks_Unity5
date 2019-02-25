
using System;
using System.Collections;
using System.Threading;

public class SubThradJob
{

    public SubThradJob() {}

    public SubThradJob(Action threadFunc)
    {
        m_threadFunc = threadFunc;
    }

    public bool isDone
    {
        get
        {
            int val = 1;
            Interlocked.CompareExchange(ref val, 0, _isDone);
            if (val == 0)
                return true;
            return false;
        }
        set
        {
            _isDone = value ? 1 : 0;
        }
    }
    private int _isDone;

    protected bool m_isDisposed = false;
    public bool isDisposed
    {
        get { return m_isDisposed; }
    }

    protected string name {
        get {
            if (thread != null) return thread.Name;
            return "null";
        }

    }
    protected Thread thread;

    public void Start()
    {
        thread = new Thread(_run);
        thread.IsBackground = true;
        thread.Start();
    }

    public void Dispose() {

        if(thread != null)
        {
            thread.Abort();
            thread = null;
        }
        m_isDisposed = true;
    }

    private void _run()
    {
        ThreadFunction();
        isDone = true;
    }

    private Action m_threadFunc;
    protected virtual void ThreadFunction()
    {
        if(m_threadFunc != null)
        {
            lock (m_threadFunc) {
                m_threadFunc();
            }
        }

        //

    }

    public IEnumerator WaitTillDone()
    {
        while (!isDone)
            yield return null;
    }
    
}