using System.Collections;
using System.Collections.Generic;
using System;
using AorBaseUtility;
using YoukiaCore;
/**
 * @author longlingquan
 * */
public class ErlKVMessage
{
    /** 版本号 */
    public const int VER = 0x83;
    /** cmd*/
    private string _cmd;
    /** key */
    private List<string> _key;
    /** value */
    private List<object> _value;
    /** jsonString */
    private string jsonString = "";
    /// <summary>
    /// 是否错误提示
    /// </summary>
    private bool _isErrorTips = true;

    //-------------------------------------------------get set start-----------------
    public string Cmd
    {
        get
        {
            return _cmd;
        }
        set
        {
            _cmd = value;
        }
    }

    public List<string> Key
    {
        get
        {
            return _key;
        }
    }

    public List<object> Value
    {
        get
        {
            return _value;
        }
    }

    public bool IsErrorTips
    {
        get { return _isErrorTips; }
        set { _isErrorTips = value; }
    }
    //-------------------------------------------------get set over-----------------

    public ErlKVMessage(string cmd)
    {
        _cmd = cmd;
        _key = new List<string>();
        _value = new List<object>();
    }

    /** 添加value，有相同项执行替换，没有就添加到末尾 */
    public void addValue(string key, object Value)
    {
        int index = _key.IndexOf(key);
        if (index >= 0)
        {
            _value[index] = Value;
        }
        else
        {
            _key.Add(key);
            _value.Add(Value);
        }
    }

    /** 通过key得到value中kv的value */
    public object getValue(string key)
    {
        int index = _key.IndexOf(key);
        if (index >= 0)
        {
            return _value[index];
        }
        else
        {
            return null;
        }
    }

    /** 得到通讯返回port */
    public int getPort()
    {
        ErlInt erlInt = getValue(null) as ErlInt;
        if (erlInt != null)
        {
            return erlInt.Value;
        }

        ErlByte erlByte = getValue(null) as ErlByte;
        if (erlByte != null)
        {
            return erlByte.Value;
        }
        //throw new IOError("port is err");

        return 0;
    }
    /** 从字节缓存中反序列化得到一个对象 */
    public void bytesRead(ByteBuffer data)
    {
        _cmd = bytesReadKey(data);
        bytesReadInfo(data);
    }

    public string bytesReadKey(ByteBuffer data)
    {
        int length = data.readUnsignedByte();
        string key = null;
        for (int i = 0; i < length; i++)
        {
            if (key == null)
            {
                key = "";
            }
            key += (char)data.readUnsignedByte();
        }
        // if(!MiniConnectManager.IsRobot)
        //MonoBehaviour.print ("bytesReadKey   key=" + key);
        return key;
    }

    public object bytesReadValue(ByteBuffer data)
    {
        uint len = (uint)data.readUnsignedShort();
        uint p = (uint)data.position;
        uint tag = (uint)data.readUnsignedByte();
        //if(!MiniConnectManager.IsRobot)
        //MonoBehaviour.print ("bytesReadValue  len=" + len + "  p=" + p + "  tag=" + tag);
        if (tag == VER)
        {  // 复杂数据结构，列表，元组
            return ErlByteKit.complexAnalyse(data);
        }
        else
        { // 否则是简单数据结构，byte,int,string,byteArray 
            data.position = (int)p;
            return ErlByteKit.simpleAnalyse(data);
        }
    }

    public void bytesReadInfo(ByteBuffer data)
    {
        try
        {
            while (data.position < data.top)
            {
                addValue(bytesReadKey(data), bytesReadValue(data));
            }
        }
        catch (Exception e)
        {
            Log.Warning(e);
        }
    }

    public void bytesWrite(ByteBuffer data)
    {
        //添加流水号 暂时定义key为 serial 
        addValue(null, new ErlInt(ConnectCount.getInstance().number));

        bytesWriteKey(data, _cmd);
        bytesWriteInfo(data);
    }

    public void bytesWriteKey(ByteBuffer data, string key)
    {
        if (key != null)
        {
            byte[] bText = DefaultToUTF8Byte(key);
            data.writeByte(bText.Length);
            data.writeBytes(bText);

        }
        else
        {
            data.writeByte(0);
        }
    }

    public void bytesWriteValue(ByteBuffer sc_data, object value)
    {
        ErlType erlType = value as ErlType;
        if (erlType == null)
        {
            erlType = new ErlNullList();
        }
        ByteBuffer data = new ByteBuffer();
        erlType.bytesWrite(data);
        sc_data.writeBytes(data, 0, (uint)data.bytesAvailable);
    }

    public void bytesWriteInfo(ByteBuffer data)
    {
        for (int i = 0; i < _key.Count; i++)
        {
            bytesWriteKey(data, _key[i]);
            bytesWriteValue(data, _value[i]);
        }
    }

    public string toString()
    {
        return "[_cmd=" + _cmd + ",_key=" + _key + ",_value=" + _value + "]";
    }

    public string toJsonString()
    {
        if (jsonString == "")
        {
            jsonString = "{";
            ErlType erlType;
            for (int i = 0; i < this.Key.Count; i++)
            {
                erlType = this.Value[i] as ErlType;
                if (erlType == null)
                    continue;
                erlType.getString(this._key[i]);
                //MonoBehaviour.print ("===========" + erlType + " " + erlType.getString (this.Key [i]));
                jsonString += erlType.getString(this.Key[i]);
                if (i < (this.Key.Count - 1))
                    jsonString += ",";
            }
            jsonString += "}";
        }
        return jsonString;
    }

    public string getLogStr()
    {
        return "Cmd : " + _cmd + "\n\n" + toJsonString() + "\n";
    }

    public static byte[] DefaultToUTF8Byte(string p_str)
    {
        try
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(p_str);
            byte[] c = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF8, b);
            return c;
        }
        catch
        {
            return null;
        }
    }
}