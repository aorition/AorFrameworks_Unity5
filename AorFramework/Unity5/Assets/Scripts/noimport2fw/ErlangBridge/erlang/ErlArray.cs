using System;
using System.Collections.Generic;
using AorBaseUtility;
using YoukiaCore;

/**
 * erlang 元组数据结构{}
 * 
 * @author longlingquan
 * */
public class ErlArray : ErlType
{
    public static int[] TAG = new int[2] { 0x68, 0x69 };
    private ErlType[] _value;

    public ErlArray(ErlType[] array)
    {
        if (array == null)
        {
        }
        else
        {
            _value = array;
        }
    }

    public ErlArray(int length)
    {
        _value = new ErlType[length];
    }

    public ErlArray()
    {
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
        return TAG[0] == tag || TAG[1] == tag;
    }

    override public void bytesRead(ByteBuffer data)
    {
        base.bytesRead(data);
        if (_tag == TAG[0])
        {// 小元组 
            int length = data.readUnsignedByte();
            //MonoBase.print ("small array bytesRead length=" + length);
            _value = new ErlType[length];
            for (int i = 0; i < length; i++)
            {
                _value[i] = ErlByteKit.natureAnalyse(data);
            }
        }
        else if (_tag == TAG[1])
        {// 大元组
            int length = data.readInt();
            //MonoBase.print ("big array bytesRead length=" + length);
            _value = new ErlType[length];
            for (int i = 0; i < length; i++)
            {
                _value[i] = ErlByteKit.natureAnalyse(data);
            }
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
            if (_value.Length > 0xff)
            {
                data.writeByte(TAG[1]);
                data.writeInt(_value.Length);
            }
            else
            {
                data.writeByte(TAG[0]);
                data.writeByte(_value.Length);

            }
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
        }
    }

    override public void writeToJson(object key, object jsonObj)
    {

    }

    /** 将数据结构写入string对象(平铺为数组结构) */
    override public string getString(object key)
    {
        ErlType erlType;
        String str = '"' + key.ToString() + '"' + ":";

        String sample_str = checkSample(_value);// 模板样式检查
        if (sample_str != null)
            return str + sample_str;
        str += '[';
        for (int i = 0; i < _value.Length; i++)
        {
            erlType = _value[i] as ErlType;
            if (erlType == null)
                continue;
            str += erlType.getValueString();
            if (i < (_value.Length - 1))
                str += ",";
        }
        str += "]";
        return str;
    }

    /** 检查是否为模板格式，如果是返回对应字符串描述，否则返回null */
    public string checkSample(ErlType[] array)
    {
        String str = null;
        if (_value.Length == 3 && _value[0] as ErlAtom != null && (_value[0] as ErlAtom).Value == "sid"
            && (_value[1] as ErlByte != null || _value[1] as ErlInt != null)
            && (_value[2] as ErlNullList != null || _value[2] as ErlList != null || _value[2] as ErlString != null))
        {
            int num = 0;
            if (_value[1] as ErlByte != null)
                num = (_value[1] as ErlByte).Value;
            else
                num = (_value[1] as ErlInt).Value;
            if (_value[2] as ErlNullList != null)
                str = "[" + "sid" + "," + num + "," + (_value[2] as ErlNullList).getValueString() + "]";
            else if (_value[2] as ErlList != null)
                str = "[" + "sid" + "," + num + "," + (_value[2] as ErlList).getListString() + "]";
            else
                str = "[" + "sid" + "," + num + "," + (_value[2] as ErlString).getASCII(true) + "]";
        }
        return str;
    }

    /** 处理包含在列表内的元组的字符串描述(列表内存储的第一层元组结构用KV结构进行解析) */
    public string getListArray()
    {
        //ErlType erlType;
        String str = "";
        uint len = (uint)_value.Length;

        str = checkSample(_value);// 模板样式检查
        if (str != null)
            return str;

        // 空列表
        if (len == 2 && (_value[0] as ErlByte) != null && (_value[0] as ErlByte).Value == 1 &&
            (_value[1] as ErlString) != null && (_value[1] as ErlString).Value == "nil")
        {
            str = "[]";
        }
        else if (len > 0 && len % 2 == 0)
        {// kv结构解析
            str = "{";
            for (int i = 0; i < len - 1; i += 2)
            {
                //if (_value[i] is ErlAtom || _value[i] is ErlList || _value[i] is ErlByteArray || _value[i] is ErlString || _value[i] is ErlInt)
                //{
                if (_value[i + 1] != null)
                    str += (_value[i] as ErlType).getValueString() + ':' + (_value[i + 1] as ErlType).getValueString();
                else
                    str += (_value[i] as ErlType).getValueString() + ":null";
                if (i < len - 2)
                    str += ",";
                //}
                //else
                //{
                //    throw new Exception("ErlArray function:getListArray  i=" + i + " len=" + len + " _value[i]=" + _value[i]);
                //}
            }
            str += '}';
        }
        else
        {// 格式错误，抛出错误异常 
            throw new Exception("ErlArray function:getListArray len=" + len);
        }
        return str;
    }

    public override string getValueString()
    {
        String str = "{";
        ErlType erlKey;
        for (int i = 0; i < _value.Length; i++)
        {
            erlKey = _value[i] as ErlType;
            if (erlKey == null)
                continue;
            str += erlKey.getValueString();
            if (i < (_value.Length - 1))
                str += ",";
        }
        str += "}";
        return str;
    }


    public static string erlArrToStr(ErlArray arr)
    {
        if (arr == null)
            return "";
        string str = "";
        str += '[';
        for (int i = 0; i < arr.Value.Length; i++)
        {
            ErlType erlType = arr.Value[i] as ErlType;
            if (erlType == null)
                continue;
            if (erlType is ErlArray)
                str += "[\"" + erlType.GetType().Name + "\"," + erlArrToStr(erlType as ErlArray) + "]";
            else
                str += "[\"" + erlType.GetType().Name + "\"," + erlType.getValueString() + "]";
            if (i < (arr.Value.Length - 1))
                str += ",";
        }
        str += "]";
        return str;
    }
}

