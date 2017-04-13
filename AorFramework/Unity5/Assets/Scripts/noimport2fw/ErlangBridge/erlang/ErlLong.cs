using System;
using AorBaseUtility;
using YoukiaCore;

public class ErlLong : ErlType
{
    public const int TAG = 0x6e;
    private long _value;

    public long Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }

    public ErlLong()
    {
    }

    override public bool isTag(int tag)
    {
        return TAG == tag;
    }
    //load a int data from a ByteArraydata;
    override public void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (isTag(_tag))
        {
            int length = data.readUnsignedByte();
            int pm = data.readUnsignedByte();
            byte[] bytes = new byte[8];
            data.readBytes(bytes, 0, length);
            _value = BitConverter.ToInt64(bytes, 0);
            if (pm == 1)
            {
                _value = -_value;
            }
        }
        //		MonoBase.print ("ErlInt  bytesRead  _value=" + _value);
    }
    //write to json
    override public void writeToJson(object key, object jsonObj)
    {
        //	jsonObj[key]=_value;
    }

    override public string getString(object key)
    {
        if (key == null)
            return "empty" + ":" + _value;
        else
            return key.ToString() + ':' + _value;
    }

    override public string getValueString()
    {
        return "" + _value;
    }
}

