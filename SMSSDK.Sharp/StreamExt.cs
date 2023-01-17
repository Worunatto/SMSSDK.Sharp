using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CN.SMSSDK.Sharp
{
    public static class StreamExt
    {
        public static int ReadInt(this Stream s)
        {
            byte[] buf = new byte[4];
            s.Read(buf, 0, 4);
            return BitConverter.ToInt32(buf.Reverse().ToArray(), 0);
        }
        
        public static void WriteInt(this Stream s, int data)
        {
            var b = GetBytes(data);
            s.Write(b);
        }

        public static void Write(this Stream s, byte[] data)
        {
            s.Write(data, 0, data.Length);
        }

        public static void Write(this Stream s, byte[] data, int offset)
        {
            s.Write(data, offset, data.Length - offset);
        }

        public static byte[] GetBytes(int i)
        {
            byte[] result = new byte[4];

            result[0] = (byte)(i >> 24);
            result[1] = (byte)(i >> 16);
            result[2] = (byte)(i >> 8);
            result[3] = (byte)(i /*>> 0*/);

            return result;
        }
    }
}
