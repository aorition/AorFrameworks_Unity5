using System;
using AorBaseUtility;
using YoukiaCore;

/**
 * erlang 字节数组数据结构
 * 
 * @author longlingquan
 * */
public class ErlByteArray:ErlType
{
	public const int TAG = 0x6d;
	
	/** 数组数据 */ 
	private ByteBuffer _value;
	
	public ErlByteArray (ByteBuffer buffer)
	{
		if (buffer == null) {
		} else {
			_value = buffer;
		}
	}
	
	public ByteBuffer Value {
		get {
			_value.setOffset (0);
			return _value;
		}
		set {
			_value = value;
		}
	}
	
	public string toUTFString ()
	{ 
		_value.setOffset (0);
		return _value.readUTF ();
	}
	
	public byte[] toArray ()
	{
		if (_value == null) {
			return null;
		}
		_value.setOffset (0);
		int length = _value.readLength ();
		byte[] array = new byte[length];
		for (int i=0; i<length; i++) {
			array [i] = _value.read (i);//   _value.readByte();
		}
		return array;
	}
	
	override public bool isTag (int  tag)
	{
		return TAG == tag;
	}
	
	override public void  bytesRead (ByteBuffer data)
	{
		base.bytesRead (data);
		if (isTag (_tag)) {
			data.position -= 3;
			int length = data.readUnsignedShort () - 1;
			data.readByte ();
			_value = new ByteBuffer ();
			_value.writeBytes (data, 0, (uint)length);
			_value.position = 0;
			data.position += length;
		} 
	}
	
	override public void bytesWrite (ByteBuffer data)
	{
		base.bytesWrite (data);
		if (_value == null || _value.length() < 1) {
			new ErlNullList ().bytesWrite (data);
		} else {
			data.writeShort (_value.length() + 1);
			data.writeByte (TAG);
			data.writeBytes (_value, 0, (uint)_value.length());
		}
	}
	
	override public void writeToJson (object key, object jsonObj)
	{
	 
	}
	
	override public string  getValueString ()
	{
			string str="[";
			for(int i=0;i<_value.length();i++)
			{
				int temp=_value.readByte();
				str+=((temp>>4)&0x0f).ToString();
				str+=(temp&0x0f).ToString();
				if(i<(_value.length()-1)) str+=",";
			}
			str+=']';
			return str;
	}
} 
