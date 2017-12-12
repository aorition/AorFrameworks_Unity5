using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AorBaseUtility;
using YoukiaCore;

public class ConnectFactory 
{
    // 连接数组  
    public List<Connect> connectArray = new List<Connect>();

    // ping计时器时间间隔 单位帧
    private long pingDelay = 600;
    // 整理计时器时间间隔  
    private long collateDelay = 900;
    //private YKTimer pingTimer;
    //private YKTimer collateTimer;
    //private bool startPing;


    public ConnectFactory()
    {
      //  NetEvent = new YKEvent();
    }

    //检查是否存在到指定地址的连接(如果发现有死连接也一并清空)  
    public Connect checkInstance(string localAddress, int localPort)
    {
        Connect c;
        Connect[] array = connectArray.ToArray();

        for (int i = array.Length - 1; i >= 0; i--)
        {
            c = array[i];
            if (!c.Active)
            {
                connectArray.Remove(c);
                continue;
            }
            if (c.isSameConnect(localAddress, localPort))
                return c;
        }
        return null;
    }

    // 获得指定地址的连接，并保存该连接
    public Connect getConnect(string localAddress, int localPort, CallBackHandle callBack)
    {
        Connect c = checkInstance(localAddress, localPort);
        if (c != null)
            return c;
        //if(!MiniConnectManager.IsRobot)
        //  Debug.LogWarning (localAddress + "   " + localPort);
        c = openConnect(localAddress, localPort);
        c.CallBack = callBack;
        connectArray.Add(c);
        return c;
    }

    //从连接数组中移除指定连接 
    public void removeConnect(Connect connect)
    {
        connectArray.Remove(connect);
    }

    //打开指定地址的连接 
    public virtual Connect openConnect(string localAddress, int localPort)
    {
        Connect c = new Connect();
        c.open(localAddress, localPort);
        return c;
    }
    //计时器监听
    public void run()
    {
        //TimerManager.Instance.update();
        try
        {
            receive();
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
        pingtime++;
        if (pingtime % pingDelay == 0)
        {
            ping();
        }
        else if (pingtime % collateDelay == 0)
        {
            collate();
        }
        if (pingtime == int.MaxValue)
            pingtime = 0;
    }
    int pingtime;
    private void receive()
    {
        Connect c;
        Connect[] array = connectArray.ToArray();
        for (int i = array.Length - 1; i >= 0; i--)
        {

            c = array[i] as Connect;
            c.receive();
        }

    }

    public void startTime()
    {

        //pingTimer = TimerManager.Instance.getTimer (pingDelay);
        //pingTimer.addOnTimer (ping);
        //pingTimer.start ();


        //collateTimer = TimerManager.Instance.getTimer (collateDelay);
        //collateTimer.addOnTimer (collate);
        //collateTimer.start ();


    }

    void Update()
    {
        run();
    }

    //ping方法
    public virtual void ping()
    {
        long time = TimeKit.CurrentTimeMillis(); //链接活动时间不用做时间修正
        Connect c;
        Connect[] array = connectArray.ToArray();
        for (int i = array.Length - 1; i >= 0; i--)
        {
            c = array[i];
            if (c.Active)
            {
                if (c.PingTime == 0)
                {
                    c.PingTime = time;
                    //发送ping值
                    ByteBuffer data = new ByteBuffer();
                    data.writeShort(1);
                    data.writeByte(1);
                    c.send(data);
                }
                else
                {
                    c.ping = time - c.PingTime;
                }
            }
        }
    }

    //执行ping通信返回消息的执行方法 
    protected virtual void pingHandle(ErlConnect connect, ErlKVMessage erlMessage)
    {

    }

    //整理方法 
    protected void collate()
    {
        long time = TimeKit.CurrentTimeMillis(); //链接活动时间不用做时间修正
        Connect c;
        Connect[] array = connectArray.ToArray();
        for (int i = array.Length - 1; i >= 0; i--)
        {
            c = array[i];
            if (c.Active)
            {
                if (time < c.TimeOut + c.ActiveTime)
                    continue;
                c.Dispose();
                connectArray.Remove(c);
            }
        }
    }

    // 关闭并清空指定连接(如果发现有死连接也一并清空) 
    public void closeConnect(string localAddress, int localPort)
    {
        Connect c;
        Connect[] array = connectArray.ToArray();
        for (int i = array.Length - 1; i >= 0; i--)
        {
            c = array[i];
            if (!c.Active)
            {
                connectArray.Remove(c);
                continue;
            }
            if (c.isSameConnect(localAddress, localPort))
            {
                c.Dispose();
                connectArray.Remove(c);
            }
        }
    }

    // 关闭并清空所有连接  
    public void closeAllConnects()
    {
        Connect c;
        Connect[] array = connectArray.ToArray();
        for (int i = array.Length - 1; i >= 0; i--)
        {
            c = array[i];
            c.Dispose();
        }
        connectArray.Clear();
    }
}