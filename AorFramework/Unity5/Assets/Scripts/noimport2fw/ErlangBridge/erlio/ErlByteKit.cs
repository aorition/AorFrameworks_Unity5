using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;

public class ErlByteKit
{

    public static ErlType simpleAnalyse(ByteBuffer data)
    {
        int position = data.position;
        int tag = data.readByte();
        data.position = position;
        if (tag == ErlByte.TAG)
        {
            ErlByte erlByte = new ErlByte(0);
            erlByte.bytesRead(data);
            return erlByte;
        }
        else if (tag == ErlByteArray.TAG)
        {
            ErlByteArray erlByteArray = new ErlByteArray(null);
            erlByteArray.bytesRead(data);
            return erlByteArray;
        }
        else if (tag == ErlInt.TAG)
        {
            ErlInt erlInt = new ErlInt(0);
            erlInt.bytesRead(data);
            return erlInt;
        }
        else if (tag == ErlString.TAG)
        {
            ErlString erlString = new ErlString("");
            erlString.bytesRead(data);
            return erlString;
        }
        else if (tag == ErlLong.TAG)
        {
            ErlLong erlLong = new ErlLong();
            erlLong.bytesRead(data);
            return erlLong;
        }
        else
        {
            return null;
        }
    }

    public static ErlType complexAnalyse(ByteBuffer data)
    {
        int position = data.position;
        int tag = data.readByte();
        data.position = position;
        //			 MonoBehaviour.print("----------complexAnalyse--------------"+tag);
        if (tag == ErlArray.TAG[0] || tag == ErlArray.TAG[1])
        {
            ErlArray erlArray = new ErlArray(null);
            erlArray.bytesRead(data);
            return erlArray;
        }
        else if (tag == ErlNullList.TAG)
        {
            ErlNullList erlNullList = new ErlNullList();
            erlNullList.bytesRead(data);
            return erlNullList;
        }
        else if (tag == ErlList.TAG)
        {
            ErlList erlList = new ErlList(null);
            erlList.bytesRead(data);
            return erlList;
        }
        else if (tag == ErlAtom.TAG)
        {
            ErlAtom erlAtom = new ErlAtom(null);
            erlAtom.bytesRead(data);
            return erlAtom;
        }
        else if (tag == ErlString.TAG)
        {
            ErlString erlString = new ErlString(null);
            erlString.sampleBytesRead(data);
            return erlString;
        }
        else if (tag == ErlLong.TAG)
        {
            ErlLong erlLong = new ErlLong();
            erlLong.bytesRead(data);
            return erlLong;
        }
        else if (tag == ErlByteArray.TAG)
        {
            ErlByteArray erlByteArray = new ErlByteArray(null);
            erlByteArray.bytesRead(data);
            return erlByteArray;
        }
        else
        {
            return null;
        }
    }

    public static ErlType natureAnalyse(ByteBuffer data)
    {
        uint p = (uint)data.position;
        uint tag = (uint)data.readUnsignedByte();
        //			MonoBehaviour.print("=======natureAnalyse======= tag="+tag);
        if (tag == ErlKVMessage.VER)//ErlKVMessage.VER)
        {
            return complexAnalyse(data);
        }
        else
        {
            data.position = (int)p;
            if (tag == ErlArray.TAG[0] || tag == ErlArray.TAG[1] || tag == ErlNullList.TAG || tag == ErlList.TAG || tag == ErlAtom.TAG || tag == ErlByteArray.TAG)
            {
                return complexAnalyse(data);
            }
            else
            {
                return natureSampleAnalyse(data);
            }
        }
    }

    public static ErlType natureSampleAnalyse(ByteBuffer data)
    {
        int position = data.position;
        int tag = data.readUnsignedByte();
        data.position = position;
        if (tag == ErlByte.TAG)
        {
            ErlByte erlByte = new ErlByte(0);
            erlByte.bytesRead(data);
            return erlByte;
        }
        else if (tag == ErlByteArray.TAG)
        {
            ErlByteArray erlByteArray = new ErlByteArray(null);
            erlByteArray.bytesRead(data);
            return erlByteArray;
        }
        else if (tag == ErlInt.TAG)
        {
            ErlInt erlInt = new ErlInt(0);
            erlInt.bytesRead(data);
            return erlInt;
        }
        else if (tag == ErlString.TAG)
        {
            ErlString erlString = new ErlString("");
            erlString.sampleBytesRead(data);
            return erlString;
        }
        else if (tag == ErlLong.TAG)
        {
            ErlLong erlLong = new ErlLong();
            erlLong.bytesRead(data);
            return erlLong;
        }
        else return null;
    }

}
