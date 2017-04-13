using AorBaseUtility;
using YoukiaCore;

public class ChecksumUtil
{

    /**
		 * @private
		 */
    private static uint[] crcTable = makeCRCTable();

    /**
	 * @private
	 */
    private static uint[] makeCRCTable()
    {
        uint[] table = new uint[256];

        uint i;
        uint j;
        uint c;
        for (i = 0; i < 256; i++)
        {
            c = i;
            for (j = 0; j < 8; j++)
            {
                if ((c & 1) == 1)
                {
                    c = 0xEDB88320 ^ (c >> 1);
                }
                else
                {
                    c >>= 1;
                }
            }

            table[i] = c;
        }
        return table;
    }

    /**
		 * Calculates a CRC-32 checksum over a ByteArray
		 * 
		 * @see http://www.w3.org/TR/PNG/#D-CRCAppendix
		 * 
		 * @param _data 
		 * @param len
		 * @param start
		 * @return CRC-32 checksum
		 */
    public static uint CRC32(ByteBuffer data, uint start, uint len)
    {
        if (start >= data.top)
        {
            start = (uint)data.top;
        }
        if (len == 0)
        {
            len = (uint)data.top - start;
        }
        if (len + start > (uint)data.top)
        {
            len = (uint)data.top - start;
        }
        uint i;
        uint c = 0xffffffff;
        for (i = start; i < len; i++)
        {
            c = (uint)crcTable[(c ^ data.getArray()[i]) & 0xff] ^ (c >> 8);
        }
        return (c ^ 0xffffffff);
    }

    /**
		 * Calculates an Adler-32 checksum over a ByteArray
		 * 
		 * @see http://en.wikipedia.org/wiki/Adler-32#Example_implementation
		 * 
		 * @param _data 
		 * @param len
		 * @param start
		 * @return Adler-32 checksum
		 */
    public static uint Adler32(ByteBuffer data)
    {
        return Adler32(data, 0, 0);
    }

    public static uint Adler32(ByteBuffer data, uint start, uint len)
    {
        if (start >= (uint)data.length())
        {
            start = (uint)data.length();
        }
        if (len == 0)
        {
            len = (uint)data.length() - start;
        }
        if (len + start > (uint)data.length())
        {
            len = (uint)data.length() - start;
        }
        uint i = start;
        uint a = 1;
        uint b = 0;

        while (i < (start + len))
        {
            a = (a + data.toArray()[i]) % 65521;
            b = (a + b) % 65521;
            i++;
        }
        return (b << 16) | a;
    }
}


