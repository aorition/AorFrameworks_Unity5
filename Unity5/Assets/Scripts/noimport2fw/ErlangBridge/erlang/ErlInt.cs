using System.Collections;
using AorBaseUtility;
using YoukiaCore;

public class ErlInt : ErlType
{
	public  const int TAG = 0x62;
	protected int  _value;
		
	public int  Value {
		get {
			return _value;
		}
		set {
			_value = value;
		} 
	}
	 
	public ErlInt (int value)
	{
		_value = value;
	}
 
	override public bool isTag (int  tag)
	{
		return TAG == tag;
	}
	//load a int data from a ByteArraydata;
	override public void  bytesRead (ByteBuffer data)
	{
		base.bytesRead (data);
        BytesReadValue(data);
//		MonoBase.print ("ErlInt  bytesRead  _value=" + _value);
	}
    protected virtual void BytesReadValue(ByteBuffer data)
    {
        if (isTag (_tag)) {
			_value = data.readInt (); 
		}
    }
	//output a int_ByteArray  data ;
	override public void bytesWrite (ByteBuffer data)
	{
		base.bytesWrite (data);
		data.writeShort (5);
		data.writeByte (TAG);
		data.writeInt (_value);
	}
	//write to json
	override public void writeToJson (object key, object jsonObj)
	{
		//	jsonObj[key]=_value;
	}
	  
	override public string  getString (object key)
	{
		if (key == null)
			return "empty" + ":" + _value;
		else
			return key.ToString () + ':' + _value;
	}
		 
	override public string  getValueString ()
	{
		return "" + _value;
	}
}
