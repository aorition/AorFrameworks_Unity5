using System;
 
public class ErlTransmitPort:TransmitPort
{
	/** 处理接收消息的回调函数 */
	private ReceiveFun receiveFunction;
	public ReceiveFun ReceiveFunction
	{
		get
		{
			return receiveFunction;
		}
		set
		{
			this.receiveFunction = value;
		}
	}
	
	public ErlTransmitPort ()
	{
	}
	
	override public void erlReceive(Connect connect,ErlKVMessage message)
	{
		 
		if(receiveFunction!=null)
		{
            receiveFunction(connect, message, NetDef.ReceiveFunState.Ok);
		}
		else
		{
			 
		}
	}	 
} 

