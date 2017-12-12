namespace YoukiaCore
{

    public delegate void CallBack();
    public delegate void CallBack<T>(T arg0);
    public delegate void Action<T, T1>(T arg0, T1 arg1);
    public delegate void CallBack<T, T1, T2>(T arg0, T1 arg1, T2 args2);
    public delegate void CallBack<T, T1, T2, T3>(T arg0, T1 arg1, T2 args2, T3 args3);
    
    public delegate bool CallbackBool();
    public delegate bool CallbackBool<T>(T arg0);
    public delegate bool CallbackBool<T, T1>(T arg0, T1 arg1);

    public delegate object CallbackObj();
    public delegate object CallbackObj<T>(T arg0);
    public delegate object CallbackObj<T, T1>(T arg0, T1 arg1);
}



