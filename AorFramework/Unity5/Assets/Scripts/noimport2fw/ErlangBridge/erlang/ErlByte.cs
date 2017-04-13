using System.Collections;
using AorBaseUtility;
using YoukiaCore;

/**
	 * erlang 短整数数据结构
	 * 
	 * @author xiney
	 * */
public class ErlByte : ErlInt
{
	/* static fields */
	/** 数据标记 */
	public new const int TAG = 0x61;
		
	/* fields */
		
	/* construct */
	public  ErlByte (int value):base(value)
	{
	}
		
	/* properties */
	/* methods */
	/** 是否是数据标记 */
	override public bool isTag (int tag)
	{
		return TAG == tag;
	}
	/** 从字节缓存中反序列化得到一个对象 */
	override public void bytesRead (ByteBuffer data)
	{
		base.bytesRead (data);
		
	}
    protected override void BytesReadValue(ByteBuffer data)
    {
        if (isTag(_tag))
        {
            _value = data.readUnsignedByte();// 读出byte数据
        } 
    }
	/** 将对象的域序列化到字节缓存中 */
	override public void bytesWrite (ByteBuffer data)
	{
		base.bytesWrite (data);
		data.writeShort (2);// 长度
		data.writeByte (TAG);
		data.writeByte (_value);
	}
	/** 将短整数数据结构写入json对象 */
	override public void writeToJson (object  key, object jsonObj)
	{
		//	jsonObj[key]=_value;
	}
	/** 将数据结构写入string对象 */
	override public string getString (object key)
	{
		if(key==null)
			return '"'+"empty"+'"'+":"+_value;
		return '"'+key.ToString()+'"' + ":" + _value;
	}
	/** 获得erlang对象的value字符串描述 */
	override public string  getValueString ()
	{
		return "" + _value;
	}
}
