using System;
using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;
using YoukiaCore;

public class ErlEntry
{
	public Connect connect;
    /// <summary>
    /// </summary>
    public int number;
    /// <summary>
    /// �Ƿ���Ping��Ϣ
    /// </summary>
    public bool isPing;
    /// <summary>
    /// �Ƿ��Ƕ���������Ϣ
    /// </summary>
    public bool isReConnect;
	/// <summary>
	/// </summary>
	public ReceiveFun receiveFun;
	
	public List<Object> argus;

    public ByteBuffer data;

    public ErlEntry(Connect connect, int number, ReceiveFun receiveFun, List<Object> argus, bool isPing, ByteBuffer data, bool isReConnect)
	{
        this.isReConnect = isReConnect;
		this.connect=connect;
		this.number=number;
		this.receiveFun=receiveFun;
		this.argus=argus;
        this.isPing = isPing;
        this.data = data;
	}
}

