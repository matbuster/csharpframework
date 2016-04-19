using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csharpFramework.Tools
{
    class Conversion
    {
        public static byte[] getBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static String getStringFromHexa(byte[] bytes)
        {
            String str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += String.Format("{0:X} ", bytes[i]);
            }
            return str;
        }

        public static string getString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string getString(char[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
