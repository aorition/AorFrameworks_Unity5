using System;
using YoukiaCore;

public class ErlPingPort:PortHandler
{
	override public void erlReceive(Connect connect,ErlKVMessage message)
	{
		long time=TimeKit.CurrentTimeMillis(); //链接活动时间不用做时间修正
		connect.ping=time-connect.PingTime;
		connect.PingTime=0;
	}
} 

