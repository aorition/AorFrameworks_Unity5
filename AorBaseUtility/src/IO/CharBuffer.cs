using System;

namespace AorBaseUtility
{
    public class CharBuffer
    {

        /* static fields */
        public const int CAPACITY = 32;
        public const string NULL = "null";

        /* fields */
        private char[] array;
        private int _top;
        private int _offset;

        /* constructors */

        public CharBuffer() : this(CAPACITY)
        {
        }

        public CharBuffer(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentException(this
                                            + " <init>, invalid capatity:" + capacity);
            array = new char[capacity];
            _top = 0;
            _offset = 0;
        }

        /** 构造一个指定的字符数组的字符缓存对象 */

        public CharBuffer(char[] data)
        {
            if (data == null)
                throw new ArgumentException(this
                                            + " <init>, null data");
            array = data;
            _top = data.Length;
            _offset = 0;
        }

        /** 构造一个指定的字符数组的字符缓存对象 */

        public CharBuffer(char[] data, int index, int length)
        {
            if (data == null)
                throw new ArgumentException(this
                                            + " <init>, null data");
            if (index < 0 || index > data.Length)
                throw new ArgumentException(this
                                            + " <init>, invalid index:" + index);
            if (length < 0 || data.Length < index + length)
                throw new ArgumentException(this
                                            + " <init>, invalid length:" + length);
            array = data;
            _top = index + length;
            _offset = index;
        }

        /** 构造一个指定的字符串的字符缓存对象 */

        public CharBuffer(string str)
        {
            if (str == null)
                throw new ArgumentException(this
                                            + " <init>, null str");
            int len = str.Length;
            array = new char[len + CAPACITY];
            str.CopyTo(0, array, 0, len);
            _top = len;
            _offset = 0;
        }

        /* properties */
        /** 得到字符缓存的容积 */

        public int capacity()
        {
            return array.Length;
        }

        /** 设置字符缓存的容积，只能扩大容积 */

        public void setCapacity(int len)
        {
            int c = array.Length;
            if (len <= c)
                return;
            for (; c < len; c = (c << 1) + 1)
                ;
            char[] temp = new char[c];
            Array.Copy(array, 0, temp, 0, _top);
            array = temp;
        }

        /** 得到字符缓存的长度 */

        public int top()
        {
            return _top;
        }

        /** 设置字符缓存的长度 */

        public void setTop(int top)
        {
            if (top < _offset)
                throw new ArgumentException(this + " setTop, invalid top:"
                                            + top);
            if (top > array.Length)
                setCapacity(top);
            this._top = top;
        }

        /** 得到字符缓存的偏移量 */

        public int offset()
        {
            return _offset;
        }

        /** 设置字符缓存的偏移量 */

        public void setOffset(int offset)
        {
            if (offset < 0 || offset > _top)
                throw new ArgumentException(this
                                            + " setOffset, invalid offset:" + offset);
            this._offset = offset;
        }

        /** 得到字符缓存的使用长度 */

        public int length()
        {
            return _top - _offset;
        }

        /** 得到字符缓存的字符数组，一般使用toArray()方法 */

        public char[] getArray()
        {
            return array;
        }

        /* methods */
        /** 得到当前位置的字符 */

        public char read()
        {
            return array[_offset++];
        }

        /** 得到指定位置的字符 */

        public char read(int pos)
        {
            return array[pos];
        }

        /** 设置当前位置的字符 */

        public void write(char c)
        {
            array[_top++] = c;
        }

        /** 设置指定偏移位置的字符 */

        public void write(char c, int pos)
        {
            array[pos] = c;
        }

        /**
	 * 根据当前偏移位置读入指定的字符数组
	 * 
	 * @param data 指定的字符数组
	 * @param pos 指定的字符数组的起始位置
	 * @param len 读入的长度
	 */

        public void read(char[] data, int pos, int len)
        {
            Array.Copy(array, _offset, data, pos, len);
            _offset += len;
        }

        /**
	 * 写入指定字符数组
	 * 
	 * @param data 指定的字符数组
	 * @param pos 指定的字符数组的起始位置
	 * @param len 写入的长度
	 */

        public void write(char[] data, int pos, int len)
        {
            if (array.Length < _top + len)
                setCapacity(_top + len);
            Array.Copy(data, pos, array, _top, len);
            _top += len;
        }

        /** 追加指定对象 */

        public CharBuffer append(Object obj)
        {
            return append(obj != null ? obj.ToString() : NULL);
        }

        /** 追加指定字符串 */

        public CharBuffer append(string str)
        {
            if (str == null)
                str = NULL;
            int len = str.Length;
            if (len <= 0)
                return this;
            if (array.Length < _top + len)
                setCapacity(_top + len);
            str.CopyTo(0, array, _top, len);
            _top += len;
            return this;
        }

        /** 追加指定字符数组 */

        public CharBuffer append(char[] data)
        {
            if (data == null)
                return append(NULL);
            return append(data, 0, data.Length);
        }

        /** 追加指定字符数组 */

        public CharBuffer append(char[] data, int pos, int len)
        {
            if (data == null)
                return append(NULL);
            write(data, pos, len);
            return this;
        }

        /** 追加指定布尔值 */

        public CharBuffer append(bool b)
        {
            int pos = _top;
            if (b)
            {
                if (array.Length < pos + 4)
                    setCapacity(pos + CAPACITY);
                array[pos] = 't';
                array[pos + 1] = 'r';
                array[pos + 2] = 'u';
                array[pos + 3] = 'e';
                _top += 4;
            }
            else
            {
                if (array.Length < pos + 5)
                    setCapacity(pos + CAPACITY);
                array[pos] = 'f';
                array[pos + 1] = 'a';
                array[pos + 2] = 'l';
                array[pos + 3] = 's';
                array[pos + 4] = 'e';
                _top += 5;
            }
            return this;
        }

        /** 追加指定字符 */

        public CharBuffer append(char c)
        {
            if (array.Length < _top + 1)
                setCapacity(_top + CAPACITY);
            array[_top++] = c;
            return this;
        }

        /** 追加指定整数 */

        public CharBuffer append(int i)
        {
            if (i == int.MinValue)
            {
                append("-2147483648");
                return this;
            }
            int pos = _top;
            int len = 0, n = 0, j;
            if (i < 0)
            {
                i = -i;
                for (n = 0,j = i; (j /= 10) > 0; n++)
                    ;
                len = n + 2;
                if (array.Length < pos + len)
                    setCapacity(pos + len);
                array[pos++] = '-';
            }
            else
            {
                for (n = 0,j = i; (j /= 10) > 0; n++)
                    ;
                len = n + 1;
                if (array.Length < pos + len)
                    setCapacity(pos + len);
            }
            while (n >= 0)
            {
                array[pos + n] = (char) ('0' + (i%10));
                i /= 10;
                n--;
            }
            _top += len;
            return this;
        }

        /** 追加指定长整数 */

        public CharBuffer append(long i)
        {
            if (i == long.MinValue)
            {
                append("-9223372036854775808");
                return this;
            }
            int pos = _top;
            int len = 0, n = 0;
            long j;
            if (i < 0)
            {
                i = -i;
                for (n = 0,j = i; (j /= 10) > 0; n++)
                    ;
                len = n + 2;
                if (array.Length < pos + len)
                    setCapacity(pos + len);
                array[pos++] = '-';
            }
            else
            {
                for (n = 0,j = i; (j /= 10) > 0; n++)
                    ;
                len = n + 1;
                if (array.Length < pos + len)
                    setCapacity(pos + len);
            }
            while (n >= 0)
            {
                array[pos + n] = (char) ('0' + (i%10));
                i /= 10;
                n--;
            }
            _top += len;
            return this;
        }

        /** 追加指定浮点数 */

        public CharBuffer append(float f)
        {
            return append(Convert.ToString(f));
        }

        /** 追加指定浮点数 */

        public CharBuffer append(double d)
        {
            return append(Convert.ToString(d));
        }

        /** 得到字符缓存当前长度的字符数组 */

        public char[] toArray()
        {
            char[] data = new char[_top - _offset];
            Array.Copy(array, _offset, data, 0, data.Length);
            return data;
        }

        /** 清除字符缓存对象 */

        public void clear()
        {
            _top = 0;
            _offset = 0;
        }

        /** 获得字符串 */

        public string getString()
        {
            return new string(array, _offset, _top - _offset);
        }

        /* common methods */

        public int hashCode()
        {
            int hash = 0;
            char[] temp = array;
            int len = _top, i = _offset;
            for (; i < len; i++)
                hash = 31*hash + temp[i];
            return hash;
        }

        public bool equals(Object obj)
        {
            if (this == obj)
                return true;
            if (!(obj is CharBuffer))
                return false;
            CharBuffer cb = (CharBuffer) obj;
            if (cb._top != _top)
                return false;
            if (cb._offset != _offset)
                return false;
            for (int i = _top - 1; i >= 0; i--)
            {
                if (cb.array[i] != array[i])
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return base.ToString() + "[" + _top + "," + _offset + "," + array.Length + "]";
        }

    }
}
