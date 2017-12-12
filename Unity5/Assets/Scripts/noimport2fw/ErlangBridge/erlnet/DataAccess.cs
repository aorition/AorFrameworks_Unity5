using System;
using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;
using YoukiaCore;

/**
 * 同步通讯类，需要等待返回，有超时
 * 
 * @author longlingquan
 * */
public class DataAccess : PortHandler
{
    /** 实例化对象 */
    private static DataAccess _dataAccess;
    /** 接收后台广播的默认处理函数 */
    public ReceiveFun defaultHandle = null;

    /** 条目列表 */
    private List<ErlEntry> _list = new List<ErlEntry>();

    public DataAccess()
    {
    }

    /** 获得实例 */
    public static DataAccess getInstance()
    {
        if (_dataAccess == null)
        {
            _dataAccess = new DataAccess();
        }
        return _dataAccess;
    }
    //public bool checkPing()
    //{
    //    ErlEntry entry;
    //    ErlEntry[] array = _list.ToArray();
    //    for (int i = 0; i < array.Length; i++)
    //    {
    //        entry = array[i] as ErlEntry;
    //        if (entry.isPing)
    //        {
    //            _list.Remove(entry);
    //            return false;
    //        }
    //    }
    //    return true;
    //}
    public void clear()
    {
        _list.Clear();
    }
    public void reaccess(ErlConnect c)
    {
        removeReConnect();
        ErlEntry entry;
        ErlEntry[] array = _list.ToArray();
        //Log.Info("有几个消息没处理" + array.Length);
        for (int i = 0; i < array.Length; i++)
        {
            entry = array[i] as ErlEntry;
            if (entry.isPing)
            {
                //Log.Info("多余的PING消息");
                continue;
            }
            if(entry.isReConnect)
            {
                //Log.Info("多余的重连消息");
                continue;
            }
            //Log.Info("再次发送 ================================= 流水号 " + entry.number);
            c.sendErl(entry.data, ErlConnect.ENCRYPTION, ErlConnect.CRC, ErlConnect.COMPRESS, ErlConnect.KV);
        }
        Log.Info("重连通知，有需要主动取数据的关注下");
//        ConnectManager.manager().NetEvent.DispatchEvent(eYKNetEvent.Reconnect);
    }
    public void access(ErlConnect connect, ErlKVMessage message, ReceiveFun receiveFun, List<object> argus, bool isPing , bool isReConnect)
    {
        ByteBuffer data = new ByteBuffer();
        message.bytesWrite(data);
        // 链接不用服务器时间
        //if (_list.Count > 0)
        //    Log.Warning("同时不能发送2个消息到服务器!");
        _list.Add(new ErlEntry(connect, message.getPort(), receiveFun, argus, isPing, data,isReConnect));
        if (GameNetBase.isDropSend || GameNetBase.isReCounecting)
            return;
        //if(!isPing&&!isReConnect)
        //    Log.Info("发送 ================================= 流水号 " + message.getPort());
        connect.sendErl(data, ErlConnect.ENCRYPTION, ErlConnect.CRC, ErlConnect.COMPRESS, ErlConnect.KV);
    }

    override public void erlReceive(Connect connect, ErlKVMessage message)
    {
        if (GameNetBase.isDropRecv)
            return;
        string cmd = message.Cmd;
        //Log.Info("接收 ================================= 流水号 " + message.getPort());
        if (cmd == "r_ok" || cmd == "r_err")
        {
            int port = message.getPort();// 获取流水号
            ErlEntry entry = removeReciveFun(port);
            //Debug.Log("接收---------------------" + port);
            //Debug.Log("流水号：" + port);
            if (entry == null || entry.receiveFun == null)
            {
                return;
            }
            NetDef.ReceiveFunState state = NetDef.ReceiveFunState.Ok;
            string error = null;
            if (cmd == "r_ok")
            {
                ErlString str = message.getValue("msg") as ErlString;
                if (str != null && str.getValueString() != "ok")
                {
                    state = NetDef.ReceiveFunState.Error;
                    error = str.getValueString();
                    Log.Debug(entry.receiveFun.Target.ToString(), error);
                }
            }
            else if (cmd == "r_err")
            {
                state = NetDef.ReceiveFunState.SystemError;
                error = "SystemError";
                Log.Debug(entry.receiveFun.Target != null ? entry.receiveFun.Target.ToString() : "", message.getLogStr());
            }
//           ConnectManager.manager().NetEvent.DispatchEvent(eYKNetEvent.ReceiveMsg, entry, message);
            entry.receiveFun(connect, message, state);
            if (error != null && message.IsErrorTips)
            {
                // GlobalEvent.dispatch(eEventName.UnLockView);
//               ConnectManager.manager().NetEvent.DispatchEvent(eYKNetEvent.ReceiveMsgError, error, state);
            }
        }
        else
        {// 服务器的广播消息
            message.addValue("cmd", new ErlString(cmd));// 为js服务的代码 
            defaultHandle(connect, message, NetDef.ReceiveFunState.Ok);
        }
    }

    /** 获取返回方法 */
    private ErlEntry removeReciveFun(int port)
    {
        ErlEntry entry;
        for (int i = 0; i < _list.Count; i++)
        {
            entry = _list[i] as ErlEntry;
            if (entry.number == port)
            {
                _list.Remove(entry);
                return entry;
            }
        }
        return null;
    }
    void removeReConnect()
    {
        ErlEntry entry;
        for (int i = _list.Count-1; i >=0; i--)
        {
            entry = _list[i] as ErlEntry;
                
            if (entry.isReConnect||entry.isPing)
            {
                _list.Remove(entry);
            }
        }
    }
}

