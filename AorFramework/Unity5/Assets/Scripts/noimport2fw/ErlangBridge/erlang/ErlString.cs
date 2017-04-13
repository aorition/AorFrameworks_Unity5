using System.Collections;
using AorBaseUtility;
using YoukiaCore;

/** 
 * @author longlingquan
 */ 
public class ErlString : ErlType
{  
	public const int TAG = 0x6b;
	protected string  _value;
	
	public string Value {
		get {
			return _value;
		}
		set {
			_value = value;
		}
	}

	private ByteBuffer  _byteArray = new ByteBuffer ();

	public ErlString (string _value)
	{
		this._value = _value;
	}

	override public bool isTag (int tag)
	{
		return TAG == tag;
	}
	
	/// <summary>
	/// 解析字符串 utf-8编码的数据解析 107简单数据
	/// </summary>
	/// <param name="data">utf-8编码</param>
	override public void bytesRead (ByteBuffer data)
	{
		base.bytesRead (data);
		if (isTag (_tag)) {
			data.position -= 3;//-1bit tag and -2bit length
			int length = data.readUnsignedShort () - 1; // -1bit tag 
			data.readByte (); //drop the tag
			uint p = (uint)data.position;

			_byteArray.clear ();
			data.readBytes (_byteArray, 0, length); 
			//data.position = (int)p+length;

			//flash中是:data..readUTFBytes (length);然后data的游标下滑了,这里_byteArray.readUTFBytes不回导致data.position下滑,所以	这里	//data.position = (int)p回退不对
			_value = _byteArray.readUTFBytes (length);
		} 
	}
	
	public void sampleBytesRead (ByteBuffer data)
	{   
		base.bytesRead (data);
		
		if (isTag (_tag)) {
			uint len = (uint)data.readUnsignedShort ();
			_value = "";
			_byteArray.clear ();
			data.readBytes (_byteArray, 0, (int)len);
			_byteArray.position = 0;
			while (_byteArray.bytesAvailable>0) {
				_value += (char)_byteArray.readUnsignedByte ();
			}
		}
	}
	
	//output a string_ByteArray to the new netnode ByteArray;
	override public void bytesWrite (ByteBuffer data)
	{ 
		base.bytesWrite (data);
		if (_value == null || _value.Length < 1) {
			data.writeShort (1); //数据值为空，只需要传标示到后台就行了
			data.writeByte (TAG);
		} else {
			ByteBuffer bt = new ByteBuffer ();
			bt.writeUTFBytes (_value);
			 
			data.writeShort (bt.top + 1);
			data.writeByte (TAG);
			data.writeBytes (bt, 0, (uint)bt.bytesAvailable); 
		}
	}

	override public void writeToJson (object key, object jsonObj)
	{
		//	jsonObj[key]=_value;
	}

	override public string getString (object key)
	{
		_byteArray.position = 0;  
		if (_byteArray.bytesAvailable > 0 && _byteArray.readUnsignedByte () == 0) {
			return key.ToString () + ':' + getASCII ();
		}
		if (_value == null || _value.Length <= 0)
			return key.ToString () + ':' + " ";
		return key.ToString () + ':' + _value + '"';
	}

	override public string getValueString ()
	{
		_byteArray.position = 0;
		if (_byteArray.bytesAvailable > 0 && _byteArray.readUnsignedByte () == 0) {
			return getASCII ();
		}
		if (_value == null || _value.Length <= 0)
			return "";
//		return '"' + _value + '"'; 去掉引号
		return _value;
	}

	public string getASCII ()
	{
		return	getASCII (false);
	}
	
	public string getASCII (bool resetPosition)
	{
//			if(resetPosition) 
		_byteArray.position = 0;
		string str = "[";
		while (_byteArray.bytesAvailable>0) {
			str += _byteArray.readUnsignedByte ();
			if (_byteArray.bytesAvailable > 0)
				str += ",";
		}
		str += "]";  
		return str;
	}
} 