using System;
using AorBaseUtility;
using YoukiaCore;

/**
 * erlang 双精度数据结构
 * 
 * @author longlingquan
 * */
public class ErlDouble:ErlType
{
	/** 数据标记 */
	public  const int TAG = 0x62;
	
	/** 双精度数据 */
	private double _value;
	
	public ErlDouble (double _value)
	{
		this._value = _value;	
	}
	
	public double  value {
		get {
			return _value;
		}
		set {
			_value = value;
		} 
	}
	
	override public bool isTag (int  tag)
	{
		return TAG == tag;
	}
	
	override public void bytesRead (ByteBuffer data)
	{
		base.bytesRead (data);
		if (isTag (_tag)) {
			_value = data.readDouble ();
		}
	}
	
	override public void bytesWrite (ByteBuffer data)
	{
		base.bytesWrite (data);
		data.writeByte (TAG);
		data.writeDouble (_value);
	}
	
	/** 将双精度数据结构写入json对象 */
	override public void writeToJson (object key, object jsonObj)
	{
		
	}
}
 

