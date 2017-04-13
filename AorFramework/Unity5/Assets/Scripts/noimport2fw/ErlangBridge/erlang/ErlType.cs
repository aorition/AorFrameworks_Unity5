using System.Collections;
using AorBaseUtility;
using YoukiaCore;

public class ErlType : ErlBytesReader, ErlBytesWriter
{
 
	protected int _tag = 0;

	virtual	public bool isTag (int tag)
	{
		return _tag == tag;
	}
	
	/** 从字节缓存中反序列化得到一个对象 */
	virtual public void bytesRead (ByteBuffer data)
	{
		int position = data.position;
		_tag = data.readByte ();// 读出erlang类型标记
		if (!isTag (_tag)) {
			data.position = position;
			return;
		}
	}
	/** 将对象的域序列化到字节缓存中 */
	virtual	public void bytesWrite (ByteBuffer data)
	{
		
	}
	/** 将数据结构写入json对象 */
	virtual	public void writeToJson (object key, object jsonObj)
	{
			
	}
	/** 获得erlang对象的KV结构的字符串描述 */
	virtual	public string getString (object key)
	{
		return "";
	}
	/** 获得erlang对象的value字符串描述 */  
	virtual	public string getValueString ()
	{
		return "";
	}
}