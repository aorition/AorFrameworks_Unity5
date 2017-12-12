using System;
using AorBaseUtility;
using YoukiaCore;

/**
 * erlang 列表数据结构
 * 规定格式:[{K,V},...] 或者[integer,....]=string 这个列表的结尾肯定是106 ErlString没有106结尾符
 * @author longlingquan
 * */
public class ErlList : ErlType
{
    public const int TAG = 0x6c;
    private ErlType[] _value;
    private bool isString = true;

    public ErlList(ErlType[] array)
    {
        if (array == null)
        {
        }
        else
        {
            _value = array;
        }
    }

    public ErlType[] Value
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

    override public bool isTag(int tag)
    {
        return TAG == tag;
    }

    override public void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (isTag(_tag))
        {
            int length = data.readInt();
            _value = new ErlType[length];
            ErlType erl;
            for (int i = 0; i < length; i++)
            {
                // length==3  ErlList的循环
                erl = ErlByteKit.natureAnalyse(data);
                if (!(erl is ErlByte || erl is ErlInt))
                    isString = false;
                _value[i] = erl;
            }
            data.readUnsignedByte();// 读取列表结尾的空列表的tag标记 
        }
    }

    override public void bytesWrite(ByteBuffer data)
    {
        base.bytesWrite(data);
        if (_value == null || _value.Length < 1)
        {
            new ErlNullList().bytesWrite(data);
        }
        else
        {
            data.writeByte(TAG);
            data.writeInt(_value.Length);
            ErlBytesWriter erlBW;
            for (int i = 0; i < _value.Length; i++)
            {
                erlBW = _value[i] as ErlBytesWriter;
                if (erlBW == null)
                {
                    erlBW = new ErlNullList();
                }
                erlBW.bytesWrite(data);
            }
            new ErlNullList().bytesWrite(data);
        }
    }

    override public void writeToJson(object key, object jsonObj)
    {

    }

    override public string getString(object key)
    {
        string str = '"' + key.ToString() + '"' + ':';
        ErlType erlType;
        if (_value.Length == 0)
        {
            return "[]";
        }
        if (isString)
        {
            str += '"' + getValueString() + '"';
            return str;
        }
        else
        {
            str += "[";
            for (int i = 0; i < _value.Length; i++)
            {
                erlType = _value[i] as ErlType;
                if (erlType == null)
                    continue;
                if (erlType as ErlArray != null)
                {
                    str += (erlType as ErlArray).getValueString();// 针对直接嵌套在列表内的元组的特殊处理
                }
                else
                    str += erlType.getValueString();
                if (i < (_value.Length - 1))
                    str += ",";
            }
            str += "]";
            return str;
        }
    }

    public override string getValueString()
    {
        if (isString)
        {

            return System.Text.UnicodeEncoding.Unicode.GetString(ToBytes());
        }
        else
        {
            return getListString();
        }
    }

    public string getListString()
    {
        string str = "[";
        ErlType erlType;
        for (int i = 0; i < _value.Length; i++)
        {
            erlType = _value[i] as ErlType;
            if (erlType == null)
                continue;
            if (erlType as ErlArray != null)
            {
                str += (erlType as ErlArray).getValueString();// 针对直接嵌套在列表内的元组的特殊处理
            }
            else
                str += erlType.getValueString();
            if (i < (_value.Length - 1))
            {
                str += ",";
            }
        }
        str += "]";
        return str;
    }

    private byte[] ToBytes()
    {
        ByteBuffer data = new ByteBuffer();
        int value = 0;
        byte[] temps = null;
        for (int i = 0; i < _value.Length; i++)
        {
            if (_value[i] is ErlInt)
            {
                value = (_value[i] as ErlInt).Value;
                if (value <= sbyte.MaxValue)
                {
                    temps = BitConverter.GetBytes((sbyte)value);
                }
                else if (value <= ushort.MaxValue)
                {
                    temps = BitConverter.GetBytes((ushort)value);
                }
                else
                {
                    temps = BitConverter.GetBytes(value);
                }
                data.writeBytes(temps);
            }
        }
        return data.toArray();
    }
}

