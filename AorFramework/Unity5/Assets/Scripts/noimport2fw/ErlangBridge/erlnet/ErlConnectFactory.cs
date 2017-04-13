using System;
using YoukiaCore;

public class ErlConnectFactory : ConnectFactory
{
    //打开指定地址的连接
    override public Connect openConnect(string localAddress, int localPort)
    {
        ErlConnect c = new ErlConnect();
        c.open(localAddress, localPort);
        c.portHandler = DataAccess.getInstance();
        return c;
    }
    /**ping方法*/
    override public void ping()
    {
        //Log.Info("PING ===========================");
        long time = TimeKit.CurrentTimeMillis(); //链接活动时间不用做时间修正
        ErlConnect c;
        Connect[] array = connectArray.ToArray();
        //bool haveActiveConnect = false;
        for (int i = array.Length - 1; i >= 0; i--)
        {
            c = array[i] as ErlConnect;
            if (c.Active)
            {
                //if (DataAccess.getInstance().checkPing())
                //{
                    c.PingTime = time;
                    //发送ping值
                    ErlKVMessage message = new ErlKVMessage("echo");
                    DataAccess.getInstance().access(c, message, pingHandle, null, true,false);
//                    ConnectManager.manager().NetEvent.DispatchEvent(eYKNetEvent.AccessMsg);
                //}
                //else
                //{
                //    c.close();
                //    Log.Warning("断开连接");
                //}
                //haveActiveConnect = true;
            }
        }
        //if (haveActiveConnect)
        //    return;        
        //reConnet();
    }

    /**执行ping通信返回消息的执行方法
        * */
    protected void pingHandle(Connect erlConnect, object erlMessage, NetDef.ReceiveFunState state)
    {
        long time = TimeKit.CurrentTimeMillis(); //链接活动时间不用做时间修正
        erlConnect.ping = time - erlConnect.PingTime;
    }
}

