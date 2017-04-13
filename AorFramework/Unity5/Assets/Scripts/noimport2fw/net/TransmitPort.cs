using System;
using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;
using YoukiaCore;

/** 连接建立并成功与后台握手后的回调函数 */
public delegate void ReceiveFun(Connect c, object obj, NetDef.ReceiveFunState state = NetDef.ReceiveFunState.Ok);

public class TransmitPort:PortHandler
{
	/**通讯端口接收数组*/ 
	private List<ReceiveFun> _receiveFunArray = new List<ReceiveFun> ();
	/**通讯端口名字数组*/ 
	private List<string> _portNameArray = new List<string> ();

	public TransmitPort ()
	{
	}

	public override void receive (Connect connect, ByteBuffer data)
	{
		String portName = data.readUTF (); 
		int index = _portNameArray.IndexOf (portName);
		if (index >= 0) {
			ReceiveFun rf = _receiveFunArray [index];
			rf (connect, data);
			//	var fun:Function=_receiveFunArray[index] as Function;
			//	fun(connect,_data);
		} else {
			//trace(connect+",is not portName="+portName);
		}
	}
	
	/** 添加通讯端口 */
	public void addPort (string portName, ReceiveFun receiveFun)
	{
		int index = _portNameArray.IndexOf (portName);
		if (index >= 0) {
			_receiveFunArray [index] = receiveFun;
		} else {
			_portNameArray.Add (portName);
			_receiveFunArray.Add (receiveFun);
		}
	}
	
	/** 得到port */
	public ReceiveFun getPort (string portName)
	{
		int index = _portNameArray.IndexOf (portName);
		if (index >= 0) {
			return _receiveFunArray [index];
		} else {
			return null;
		}
	}  
} 

