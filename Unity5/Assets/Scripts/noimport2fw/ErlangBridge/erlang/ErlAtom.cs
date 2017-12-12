using System;
using AorBaseUtility;
using YoukiaCore;

/**
 * erlang 原子数据结构
 * 
 * @author longlingquan
 * */
public class ErlAtom:ErlType
{
	/** 数据标记 */
	public const int TAG = 0x64;
	
	/** 原子字符串 */
	private string _value;
	
	public ErlAtom (string _value)
	{
		this._value = Value;
	} 
	
	/**getter setter*/
	public string Value {
		get {
			return _value;
		}
		set {
			_value = value;
		}
	}
	
	/** 是否是数据标记 */
	override public bool isTag (int tag)
	{
		return TAG == tag;
	}
	
	/** 从字节缓存中反序列化得到一个对象 */
	override public void bytesRead (ByteBuffer data)
	{
		base.bytesRead (data);
		if (isTag (_tag)) {
			_value = "";
			int length = data.readShort ();// 读出数据长度
			for (int i=0; i<length; i++) {
				//s_value+=String.fromCharCode(data.readUnsignedByte()); 暂时采用下面的写法
				_value += (char)(data.readUnsignedByte ());
				//_value+= new string(new char[]{(char)(data.readUnsignedByte())}); 
			}	
			//MonoBehaviour.print("ErlAtom  bytesRead  _value="+_value+"   bytesAvailable="+data.bytesAvailable);
		}
	}
	
	/** 将对象的域序列化到字节缓存中 */
	override public void bytesWrite (ByteBuffer data)
	{
		base.bytesWrite (data);
		if (_value == null || _value.Length < 1) {
			new ErlNullList ().bytesWrite (data);
		} else {
			data.writeByte (TAG);
			data.writeShort (_value.Length);
			for (int i=0; i<_value.Length; i++) {
				//这个是需要写的
				//	data.writeByte(_value.charCodeAt(i));
			}
		}
	}
	
	override public void writeToJson (object key, object jsonObj)
	{
		//	jsonObj[key]=_value;
	}
	
	override public string getString (object key)
	{
		String str = '"' + key.ToString () + '"' + ":" + '"'; 
		str += _value;
		str += '"'; 
		return str;
	}
	
	override public string getValueString ()
	{
		return _value;
	}
} 