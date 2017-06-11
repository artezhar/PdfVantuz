using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class Extensions
    {
        public static string ToHexString(this byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] ToByteArray(this string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static char[] ToCharArray(this byte[] b)
        {
            char[] ret = new char[b.LongLength];
            for (long i = 0; i < b.LongLength; i++)
            {
                ret[i] = (char)b[i];
            }
            return ret;
        }

        public static int IndexOfSubArray(this char[] arr, char[] subarr, int startPos = 0)
        {
            for (int i = startPos; i < arr.Length - subarr.Length; i++)
            {
                bool flag = true;
                for (int j = 0; j < subarr.Length; j++)
                {
                    if (arr[i + j] != subarr[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return i;
                }
            }
            return -1;
        }

        public static IEnumerable<int> ExtractIntegers(this string str)
        {
            var nsStr = str.Replace(" ", "");
            List<int> ret = new List<int>();
            for (int i = 0; i < str.Length; i++)
            {
                try
                {
                    if (str[i - 1] == ')' && (str[i] == '-' || char.IsDigit(str[i])))
                    {
                        int add = 0;
                        if (int.TryParse(str.Substring(i, str.IndexOf('(', i) - i), out add)) ret.Add(add);
                    }
                }
                catch
                {
                    continue;
                }
            }
            return ret;
        }
    }

}

