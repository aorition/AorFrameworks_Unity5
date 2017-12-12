using System.Collections;
using AorBaseUtility;
using YoukiaCore;

public class ErlNullList : ErlType
{
	public const int TAG = 0x6a;

	public ErlNullList ()
	{
	}

	override public bool isTag (int tag)
	{
		return TAG == tag;
	}

	override public void bytesWrite (ByteBuffer data)
	{
		base.bytesWrite (data);
		//? no length?
		data.writeByte (TAG);
	}

	override public void writeToJson (object key, object jsonObj)
	{
		//	jsonObj[key]=[];
	}

	override public string getString (object key)
	{
		return key.ToString () + ":";
	}

	override public string  getValueString ()
	{
		return "";
	}
}
	

