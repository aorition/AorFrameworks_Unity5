using System;
using System.Collections.Generic;
using AorBaseUtility;
using YoukiaCore;

public class GameNetBase
{
    public static string Ip;
    public static int Port;
    public static string value;
    //是否连接成功
    public static bool isCounectEd;
    public static bool isLogin;
    public static bool isReCounecting;
    //-断上断下，测试用
    public static bool isDropSend;
    public static bool isDropRecv;

    private static Dictionary<Type, GameNetBase> mNetHandlerDic = new Dictionary<Type, GameNetBase>();


    public static GameNetBase getNet(Type type)
    {
        GameNetBase netHander = null;
        if (!mNetHandlerDic.TryGetValue(type, out netHander))
        {
            netHander = (GameNetBase)type.Assembly.CreateInstance(type.FullName);
            mNetHandlerDic.Add(type, netHander);
        }
        return netHander;
    }

    public static T getNet<T>() where T : GameNetBase, new()
    {
        return (T)getNet(typeof(T));
    }


    public GameNetBase()
    {
    }

    public virtual void Access(ErlKVMessage msg, ReceiveFun receive = null, bool isReConnect=false)
    {
        if (receive == null)
        {
            receive = Receive;
        }
        ConnectManager.manager().sendMessage(Ip, Port, msg, receive, null,isReConnect);
//       ConnectManager.manager().NetEvent.DispatchEvent(eYKNetEvent.AccessMsg, msg, this);
    }

    public virtual void Send(ErlKVMessage msg)
    {
        ConnectManager.manager().sendMessage(Ip, Port, msg, null, null,false);
//       ConnectManager.manager().NetEvent.DispatchEvent(eYKNetEvent.SendMsg, msg);

    }

    public virtual void Receive(Connect c, object obj, NetDef.ReceiveFunState state)
    {
        try
        {
            Read(obj as ErlKVMessage);
        }
        catch (System.Exception ex)
        {
            string str = ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace;
            Log.Error(str);
        }
    }

    public virtual void Read(ErlKVMessage msg)
    {

    }

    public static void Dispose()
    {
        mNetHandlerDic.Clear();
    }

}

