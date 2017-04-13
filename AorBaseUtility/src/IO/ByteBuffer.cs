/**
 * Coypright 2013 by 刘耀鑫<xiney@youkia.com>.
 */
using System;
using System.Collections;

namespace AorBaseUtility
{
/**
 * @author 刘耀鑫
 */

    public class ByteBuffer : ICloneable
    {

        /* static fields */
        /** 默认的初始容量大小 */
        public const int CAPACITY = 32;
        /** 默认的动态数据或文字的最大长度，400k */
        public const int MAX_DATA_LENGTH = 400*1024;
        /** 零长度的字节数组 */
        public static readonly byte[] EMPTY_ARRAY = {};
        /** 零长度的字符串 */
        public const string EMPTY_STRING = "";

        /* fields */
        /** 字节数组 */
        private byte[] array;
        /** 字节缓存的长度 */
        private int _top;
        /** 字节缓存的偏移量 */
        private int _position;
        public uint bytesAvailable;

        public int top
        {
            get { return _top; }
            set
            {
                _top = value;
                bytesAvailable = (uint) (Math.Abs(_top - position));
            }
        }

        public int position
        {
            get { return _position; }
            set
            {
                _position = value;
                bytesAvailable = (uint) (Math.Abs(top - _position));
            }
        }

        /* constructors */
        /** 按默认的大小构造一个字节缓存对象 */

        public ByteBuffer()
            : this(CAPACITY)
        {
        }

        /** 按指定的大小构造一个字节缓存对象 */

        public ByteBuffer(int capacity)
        {
            if (capacity < 1)
                throw new Exception(this
                                    + " <init>, invalid capatity:" + capacity);
            array = new byte[capacity];
            top = 0;
            position = 0;
        }

        /** 按指定的字节数组构造一个字节缓存对象 */

        public ByteBuffer(byte[] data)
        {
            if (data == null)
                throw new Exception(this
                                    + " <init>, null data");
            array = data;
            top = data.Length;
            position = 0;
        }

        /** 按指定的字节数组构造一个字节缓存对象 */

        public ByteBuffer(byte[] data, int index, int length)
        {
            if (data == null)
                throw new Exception(this
                                    + " <init>, null data");
            if (index < 0 || index > data.Length)
                throw new Exception(this
                                    + " <init>, invalid index:" + index);
            if (length < 0 || data.Length < index + length)
                throw new Exception(this
                                    + " <init>, invalid length:" + length);
            array = data;
            top = index + length;
            position = index;
        }

        /* properties */
        /** 得到字节缓存的容积 */

        public int capacity()
        {
            return array.Length;
        }

        /** 设置字节缓存的容积，只能扩大容积 */

        public void setCapacity(int len)
        {
            int c = array.Length;
            if (len <= c)
                return;
            for (; c < len; c = (c << 1) + 1)
                ;
            byte[] temp = new byte[c];
            Array.Copy(array, 0, temp, 0, top);
            array = temp;
        }

        /** 设置字节缓存的长度 */

        public void setTop(int top)
        {
            if (top < position)
                throw new Exception(this + " setTop, invalid top:"
                                    + top);
            if (top > array.Length)
                setCapacity(top);
            this.top = top;
        }

        /** 得到字节缓存的偏移量 */

        public int offset()
        {
            return position;
        }

        /** 设置字节缓存的偏移量 */

        public void setOffset(int offset)
        {
            if (offset < 0 || offset > top)
                throw new Exception(this
                                    + " setOffset, invalid offset:" + offset);
            this.position = offset;
        }

        /** 得到字节缓存的使用长度 */

        public int length()
        {
            return top - position;
        }

        /** 得到字节缓存的字节数组，一般使用toArray()方法 */

        public byte[] getArray()
        {
            return array;
        }

        /** 获得字节缓存的哈希码 */

        public int getHashCode()
        {
            int hash = 17;
            for (int i = top - 1; i >= 0; i--)
                hash = 65537*hash + array[i];
            return hash;
        }

        /* methods */
        /* byte methods */
        /** 得到指定偏移位置的字节 */

        public byte read(int pos)
        {
            return array[pos];
        }

        /** 设置指定偏移位置的字节 */

        public void write(int b, int pos)
        {
            array[pos] = (byte) b;
        }

        /* read methods */
        /**
     * 按当前偏移位置读入指定的字节数组
     * 
     * @param data 指定的字节数组
     * @param pos 指定的字节数组的起始位置
     * @param len 读入的长度
     */

        public void read(byte[] data, int pos, int len)
        {
            Array.Copy(array, position, data, pos, len);
            position += len;
        }

        /** 读出一个布尔值 */

        public bool readBoolean()
        {
            return (array[position++] != 0);
        }

        /** 读出一个字节 */

        public sbyte readByte()
        {
            return unchecked((sbyte) array[position++]);
        }

        /** 读出一个无符号字节 */

        public int readUnsignedByte()
        {
            return array[position++] & 0xff;
        }

        /** 读出一个字符 */

        public char readChar()
        {
            return (char) readUnsignedShort();
        }

        /** 读出一个短整型数值 */

        public short readShort()
        {
            return (short) readUnsignedShort();
        }

        /** 读出一个无符号的短整型数值 */

        public int readUnsignedShort()
        {
            int pos = position;
            position += 2;
            return (array[pos + 1] & 0xff) + ((array[pos] & 0xff) << 8);
        }

        /** 读出一个整型数值 */

        public int readInt()
        {
            int pos = position;
            position += 4;
            return (array[pos + 3] & 0xff) + ((array[pos + 2] & 0xff) << 8)
                   + ((array[pos + 1] & 0xff) << 16) + ((array[pos] & 0xff) << 24);
        }

        /** 读出一个浮点数值 */

        public float readFloat()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(readInt()), 0);
        }

        /** 读出一个长整型数值 */

        public long readLong()
        {
            int pos = position;
            position += 8;
            return (array[pos + 7] & 0xffL) + ((array[pos + 6] & 0xffL) << 8)
                   + ((array[pos + 5] & 0xffL) << 16) + ((array[pos + 4] & 0xffL) << 24)
                   + ((array[pos + 3] & 0xffL) << 32) + ((array[pos + 2] & 0xffL) << 40)
                   + ((array[pos + 1] & 0xffL) << 48) + ((array[pos] & 0xffL) << 56);
        }

        /** 读出一个双浮点数值 */

        public double readDouble()
        {
            return BitConverter.Int64BitsToDouble(readLong());
        }

        /**
     * 读出动态长度， 数据大小采用动态长度，整数类型下，最大为512M
     * <li>1xxx,xxxx表示（0~0x80） 0~128B</li>
     * <li>01xx,xxxx,xxxx,xxxx表示（0~0x4000）0~16K</li>
     * <li>001x,xxxx,xxxx,xxxx,xxxx,xxxx,xxxx,xxxx表示（0~0x20000000）0~512M</li>
     */

        public int readLength()
        {
            int n = array[position] & 0xff;
            if (n >= 0x80)
            {
                position++;
                return n - 0x80;
            }
            if (n >= 0x40)
                return readUnsignedShort() - 0x4000;
            if (n >= 0x20)
                return readInt() - 0x20000000;
            throw new Exception(this
                                + " readLength, invalid number:" + n);
        }

        /** 读出一个指定长度的字节数组，可以为null */

        public byte[] readData()
        {
            int len = readLength() - 1;
            if (len < 0)
                return null;
            if (len > MAX_DATA_LENGTH)
                throw new Exception(this
                                    + " readData, data overflow:" + len);
            if (len == 0)
                return EMPTY_ARRAY;
            byte[] data = new byte[len];
            read(data, 0, len);
            return data;
        }

        /** 读出一个指定长度的字符串 */

        public string readString()
        {
            return readString(null);
        }

        /** 读出一个指定长度和编码类型的字符串 */

        public string readString(string charsetName)
        {
            int len = readLength() - 1;
            if (len < 0)
                return null;
            if (len > MAX_DATA_LENGTH)
                throw new Exception(this
                                    + " readString, data overflow:" + len);
            if (len == 0)
                return EMPTY_STRING;
            byte[] data = new byte[len];
            read(data, 0, len);
            if (charsetName == null)
                return System.Text.Encoding.Default.GetString(data);
            try
            {
                return System.Text.Encoding.GetEncoding(charsetName).GetString(data);
            }
            catch (Exception e)
            {
                throw new Exception(this
                                    + " readString, invalid charsetName:" + charsetName + " " + e.ToString());
            }
        }

        /** 读出一个指定长度的utf字符串 */

        public string readUTF()
        {
            int len = readLength() - 1;
            if (len < 0)
                return null;
            if (len == 0)
                return EMPTY_STRING;
            if (len > MAX_DATA_LENGTH)
                throw new Exception(this
                                    + " readUTF, data overflow:" + len);
            char[] temp = new char[len];
            int n = ByteKit.readUTF(array, position, len, temp);
            if (n < 0)
                throw new Exception(this
                                    + " readUTF, format err, len=" + len);
            position += len;
            return new String(temp, 0, n);
        }

        /* write methods */
        /**
     * 写入指定字节数组
     * 
     * @param data 指定的字节数组
     * @param pos 指定的字节数组的起始位置
     * @param len 写入的长度
     */

        public void write(byte[] data, int pos, int len)
        {
            if (len <= 0)
                return;
            if (array.Length < top + len)
                setCapacity(top + len);
            Array.Copy(data, pos, array, top, len);
            top += len;
        }

        /** 写入一个布尔值 */

        public void writeBoolean(bool b)
        {
            if (array.Length < top + 1)
                setCapacity(top + CAPACITY);
            array[top++] = (byte) (b ? 1 : 0);
        }

        /** 写入一个字节 */

        public void writeByte(int b)
        {
            if (array.Length < top + 1)
                setCapacity(top + CAPACITY);
            array[top++] = (byte) b;
        }

        /** 写入一个字符 */

        public void writeChar(int c)
        {
            writeShort(c);
        }

        /** 写入一个短整型数值 */

        public void writeShort(int s)
        {
            int pos = top;
            if (array.Length < pos + 2)
                setCapacity(pos + CAPACITY);
            byte[] temp = BitConverter.GetBytes(s);
            Array.Reverse(temp);
            Array.Copy(temp, 2, array, pos, 2);
            top += 2;
        }

        /** 写入一个整型数值 */

        public void writeInt(int i)
        {
            int pos = top;
            if (array.Length < pos + 4)
                setCapacity(pos + CAPACITY);
            byte[] temp = BitConverter.GetBytes(i);
            Array.Reverse(temp);
            Array.Copy(temp, 0, array, pos, 4);
            top += 4;
        }

        /** 写入一个浮点数值 */

        public void writeFloat(float f)
        {
            writeInt(BitConverter.ToInt32(BitConverter.GetBytes(f), 0));
        }

        /** 写入一个长整型数值 */

        public void writeLong(long l)
        {
            int pos = top;
            if (array.Length < pos + 8)
                setCapacity(pos + CAPACITY);
            byte[] temp = BitConverter.GetBytes(l);
            Array.Reverse(temp);
            Array.Copy(temp, 0, array, pos, 8);
            top += 8;
        }

        /** 写入一个双浮点数值 */

        public void writeDouble(double d)
        {
            writeLong(BitConverter.DoubleToInt64Bits(d));
        }

        /** 写入动态长度 */

        public void writeLength(int len)
        {
            if (len >= 0x20000000 || len < 0)
                throw new Exception(this
                                    + " writeLength, invalid len:" + len);
            if (len < 0x80)
                writeByte(len + 0x80);
            else if (len < 0x4000)
                writeShort(len + 0x4000);
            else
                writeInt(len + 0x20000000);
        }

        /** 写入一个字节数组，可以为null */

        public void writeData(byte[] data)
        {
            writeData(data, 0, (data != null) ? data.Length : 0);
        }

        /** 写入一个字节数组，可以为null */

        public void writeData(byte[] data, int pos, int len)
        {
            if (data == null)
            {
                writeLength(0);
                return;
            }
            writeLength(len + 1);
            write(data, pos, len);
        }

        /** 写入一个字符串，可以为null */

        public void writeString(string str)
        {
            writeString(str, null);
        }

        /** 写入一个字符串，以指定的字符进行编码 */

        public void writeString(string str, string charsetName)
        {
            if (str == null)
            {
                writeLength(0);
                return;
            }
            if (str.Length <= 0)
            {
                writeLength(1);
                return;
            }
            byte[] data;
            if (charsetName != null)
            {
                try
                {
                    data = System.Text.Encoding.GetEncoding(charsetName).GetBytes(str);
                }
                catch (Exception e)
                {
                    //Log.error(null,e);
                    throw new Exception(this
                                        + " writeString, invalid charsetName:" + charsetName + " " + e.ToString());
                }
            }
            else
                data = System.Text.Encoding.Default.GetBytes(str);
            writeLength(data.Length + 1);
            write(data, 0, data.Length);
        }

        /** 写入一个utf字符串，可以为null */

        public void writeUTF(string str)
        {
            writeUTF(str, 0, (str != null) ? str.Length : 0);
        }

        /** 写入一个utf字符串中指定的部分，可以为null */

        public void writeUTF(string str, int index, int length)
        {
            if (str == null)
            {
                writeLength(0);
                return;
            }
            int len = ByteKit.getUTFLength(str, index, length);
            writeLength(len + 1);
            if (len <= 0)
                return;
            int pos = top;
            if (array.Length < pos + len)
                setCapacity(pos + len);
            ByteKit.writeUTF(str, index, length, array, pos);
            top += len;
        }

        /** 归零偏移量 */

        public void zeroOffset()
        {
            int pos = position;
            if (pos <= 0)
                return;
            int t = top - pos;
            Array.Copy(array, pos, array, 0, t);
            top = t;
            position = 0;
        }

        /** 检查是否为相同类型的实例 */

        public bool checkClass(object obj)
        {
            return (obj is ByteBuffer);
        }

        /** 得到字节缓存当前长度的字节数组 */

        public byte[] toArray()
        {
            byte[] data = new byte[top - position];
            Array.Copy(array, position, data, 0, data.Length);
            return data;
        }

        /** 清除字节缓存对象 */

        public void clear()
        {
            top = 0;
            position = 0;
        }

        /** 从字节缓存中反序列化获得对象的域 */

        public object bytesRead(ByteBuffer data)
        {
            int len = data.readLength() - 1;
            if (len < 0)
                return null;
            if (len > MAX_DATA_LENGTH)
                throw new Exception(this
                                    + " bytesRead, data overflow:" + len);
            if (array.Length < len)
                array = new byte[len];
            if (len > 0)
                data.read(array, 0, len);
            top = len;
            position = 0;
            return this;
        }

        /** 将对象的域序列化到字节缓存中 */

        public void bytesWrite(ByteBuffer data)
        {
            data.writeData(array, position, top - position);
        }

        /* common methods */

        public object Clone()
        {
            try
            {
                ByteBuffer bb = (ByteBuffer) base.MemberwiseClone();
                byte[] array = bb.array;
                bb.array = new byte[bb.top];
                Array.Copy(array, 0, bb.array, 0, bb.top);
                return bb;
            }
            catch (Exception e)
            {
                //Log.error(null,e);
                throw new Exception(this
                                    + " clone, capacity=" + array.Length, e);
            }
        }

        public bool equals(object obj)
        {
            if (this == obj)
                return true;
            if (!checkClass(obj))
                return false;
            ByteBuffer bb = (ByteBuffer) obj;
            if (bb.top != top)
                return false;
            if (bb.position != position)
                return false;
            for (int i = top - 1; i >= 0; i--)
            {
                if (bb.array[i] != array[i])
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return base.ToString() + "[" + top + "," + position + "," + array.Length + "]";
        }

        //---------------------------------------------add 2013.6.7 longlingquan-----------------------------------  
        /** 读出一个指定长度的utf字符串 */

        public string readUTFBytes(int len)
        {
            if (len < 0)
                return null;
            if (len == 0)
                return EMPTY_STRING;
            if (len > MAX_DATA_LENGTH)
                throw new Exception(this
                                    + " readUTF, data overflow:" + len);
            byte[] data = new byte[len];
            read(data, 0, len);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        public void writeUTFBytes(string str)
        {
            if (str == null)
            {
                //writeLength(0);
                return;
            }
            if (str.Length <= 0)
            {
                //writeLength(1);
                return;
            }
            //int pos = top; 
            byte[] srcData;
            srcData = System.Text.Encoding.UTF8.GetBytes(str);
            writeBytes(srcData);

            //Array.Reverse (srcData);
            //Array.Copy (srcData, 0, array, pos, srcData.Length);
            //top += srcData.Length; 
        }

        public void writeBytes(byte[] b)
        {
            write(b, 0, b.Length);
        }

        public void writeBytes(byte[] b, int offset, int len)
        {
            write(b, offset, len);
        }

        public void writeBytes(ByteBuffer b, uint offset, uint len)
        {
            write(b.toArray(), (int) offset, (int) len);
        }

        public void readBytes(ByteBuffer data, int pos, int len)
        {
            //如果被写入的数据过多 则扩大容器
            if (data.array.Length < pos + len + 1)
                data.setCapacity(data.top + CAPACITY + len);
            Array.Copy(array, position, data.array, pos, len);
            position += len;
            data.top = len;
            data.position = pos;
        }

        public void readBytes(byte[] bytes, int pos, int len)
        {
            Array.Copy(array, position, bytes, pos, len);
            position += len;
        }


        //---------------------------------------------add over---------------------------------------------------------
    }
}
