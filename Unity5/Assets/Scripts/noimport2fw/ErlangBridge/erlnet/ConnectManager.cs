using System;
using System.Collections;
using System.Collections.Generic;
using YoukiaCore;

/** 
  * 连接管理器 
  * @author longlingquan 
  * */
public class ConnectManager
{
    /** 连接管理器的唯一实例 */
    private static ConnectManager _manager = null;
    public ErlConnectFactory factory;
    public ReceiveFun messageHandle;
    //内部消息
//    public YKEvent NetEvent;
    public float RealtimeSinceStartup;
    
    public static ConnectManager manager()
    {
        if (_manager == null)
        {
            _manager = new ConnectManager();

        }
        return _manager;
    }

    public ConnectManager()
    {
//        NetEvent = new YKEvent();
    }

    public void init()
    {

        if (factory == null)
            factory = new ErlConnectFactory();

        factory.startTime();
    }


    /** 获得指定连接当前状态(返回0表示连接正常，1连接不存在，2连接存在但不可用) */
    public int getConnectStatus(string address, int port)
    {
        ErlConnect erlConnect = factory.checkInstance(address, port) as ErlConnect;
        if (erlConnect == null)
            return 1;
        if (!erlConnect.isActive)
            return 2;
        return 0;
    }

    /** 打开指定地址和端口号的连接 */
    public Connect getInstance(string address, int port, CallBackHandle handlel)
    {
        return factory.getConnect(address, port, handlel);
    }

    public void Update(float time)
    {
        RealtimeSinceStartup = time;

        if (factory!=null)
            factory.run();
    }


    ///** 关闭并清空指定连接(如果发现有死连接也一并清空) */
    //public void closeConnect (string address, int port)
    //{
    //    factory.closeConnect (address, port);
    //}
    ///** 指定连接是否可用 */
    //public Boolean isActive (string address, int port)
    //{
    //    ErlConnect erlConnect = factory.checkInstance (address, port) as ErlConnect;
    //    if (erlConnect == null)
    //        return false;
    //    return erlConnect.isActive;
    //}
    ///** 获取指定连接的ping延迟时间 */
    //public int getPing (string address, int port)
    //{
    //    ErlConnect erlConnect = factory.checkInstance (address, port) as ErlConnect;
    //    if (erlConnect == null)
    //        return 0;
    //    return (int)erlConnect.ping;
    //}

    //public void ping()
    //{
    //    factory.ping();
    //}

    /** 关闭并清空所有连接 */
    public void closeAllConnects()
    {
        factory.closeAllConnects();
    }

    /** 
	 * 向指定地址和端口的连接发送消息
	 * @param address-消息发送地址
	 * @param port-消息发送端口
	 * @param handle-执行回调的函数
	 * @param argus-执行回调的参数数组
	 * @param _data-消息发送数据 ErlKVMessage对象
	 **/
    public void sendMessage(string address, int port, ErlKVMessage message, ReceiveFun handle, List<object> argus, bool isReConnect)
    {
        //        if(!MiniConnectManager.IsRobot)
        //        Debug.Log(message.Cmd + "," + message.toJsonString());

        ErlConnect connect = factory.getConnect(address, port, null) as ErlConnect;
        if (connect == null)
        {
            return;
        }
        if (handle != null)
        {
            DataAccess.getInstance().access(connect, message, handle, argus, false, isReConnect);
        }
        else
            DataAccess.getInstance().access(connect, message, messageHandle, argus, false, isReConnect);

    }
}

