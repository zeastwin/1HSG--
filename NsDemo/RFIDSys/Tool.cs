using System;
using System.Collections.Generic;
using System.Text;

namespace NsDemo.Utility
{
    class Tool
    {
        //将十六进制的字符串转化为ushort
        public static ushort HexString2Ushort(string s)
        {
            ushort value = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != ' ')
                {
                    value = (ushort)(value * 16 + HexStringToHex(s, i));
                }
            }

            return value;
        }

        //将字节数组形式的mac地址转化为对应的字符串
        public static string MacToString(byte[] mac)
        {
            string MacString = "";

            for (int i = 0; i < 6; i++)
            {
                MacString += ByteToHexString(mac[i]);
                if (i < 5)
                {
                    MacString += ":";
                }
            }

            return MacString;
        }

        //将字符串形式的mac地址转化为对应的字节数组
        public static byte[] StringToMac(string str)
        {
            string temp = "";

            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] != ' ') && (str[i] != ':'))
                {
                    temp += str[i];
                }
            }

            return HexStringToByte(temp, 0, 6);//6字节长度
        }

        //判断字符串是否是十六进制字符串
        public static bool ValidHexString(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!
                    (((str[i] >= '0') && (str[i] <= '9')) ||
                    ((str[i] >= 'a') && (str[i] <= 'f')) ||
                    ((str[i] >= 'A') && (str[i] <= 'F')))
                    )
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetStringWithoutSpace(string str, int pos)
        {
            string temp = "";
            for (int i = pos; i < str.Length; i++)
            {
                if ((str[i] != ' ') && (str[i] != ':'))
                {
                    temp += str[i];
                }
            }
            return temp;
        }

        //将单个十六进制字符(4 bits)转化为byte
        private static byte HexStringToHex(string str, int pos)
        {
            byte value = 0;

            if (str.Length <= pos)
            {
                return value;
            }
            if ((str[pos] >= '0') && (str[pos] <= '9'))
            {
                value = (byte)(str[pos] - '0');
            }
            else if ((str[pos] >= 'a') && (str[pos] <= 'f'))
            {
                value = (byte)(str[pos] - 'a' + 10);
            }
            else if ((str[pos] >= 'A') && (str[pos] <= 'F'))
            {
                value = (byte)(str[pos] - 'A' + 10);
            }

            return value;
        }

        //将字符串的pos位置开始，转化为数组，转化后数组的长度为cnt
        public static byte[] HexStringToByte(string str, int pos, int cnt)
        {
            if ((!ValidHexString(str)) || ((str.Length - pos + 1) >> 1 < cnt))
            {
                return null;
            }

            if (str.Length % 2 != 0)
            {
                str = "0" + str;
            }
            byte[] data = new byte[cnt];

            for (int i = 0; i < cnt; i++)
            {
                data[i] = (byte)(HexStringToHex(str, 2 * i + pos) * 16 + HexStringToHex(str, 2 * i + pos + 1));
            }

            return data;
        }

        public static string bytes2String(byte[] data, int offset, int len)
        {
            string outString = "";

            for (int i = offset; i < len + offset; i++)
            {
                outString += (char)data[i];
            }

            return outString;
        }

        //将字符串的pos位置开始，转化为数组，转化后数组的长度为cnt
        public static byte[] HexStringToByte(string str, int pos)
        {
            string tempStr = GetStringWithoutSpace(str, pos);

            return HexStringToByte(tempStr, 0, (tempStr.Length + 1) >> 1);
        }

        public static byte HexStringToSingleByte(string str, int pos)
        {
            byte temp = 0;
            int len = 2;
            string tempStr = GetStringWithoutSpace(str, pos);

            if (tempStr.Length == 0)
            {
                return 0;
            }
            else if (tempStr.Length < 2)
            {
                len = tempStr.Length;
            }
            else
            {
                len = 2;
            }

            for (int i = 0; i < len; i++)
            {
                temp = (byte)(temp * 16 + HexStringToHex(tempStr, i));
            }

            return temp;
        }

        //将字符串的pos位置开始，转化int
        public static int HexStringToInt(string s, int pos)
        {
            string str = "";
            int len = (s.Length - pos) > 8 ? 8 : s.Length - pos;
            for (int i = pos; i < len; i++)
            {
                str += s[i];
            }
            for (int i = len; i < 8; i++)
            {
                str = "0" + str;
            }

            if (!ValidHexString(str))
            {
                return 0;
            }

            int result = 0;
            byte[] data = HexStringToByte(str, 0, 4);

            for (int i = 0; i < 4; i++)
            {
                result = (result << 8) + data[i];
            }

            return result;
        }

        //将字节类型的数据转化为十六进制字符串
        public static string ByteToHexString(byte data)
        {
            string outString = "";

            if (data < 16)
            {
                outString += "0";
            }
            outString += data.ToString("X");

            return outString;
        }

        //将字节类型的数据转化为十六进制字符串
        public static string ByteToHexString(byte[] data, int pos, int length, string space)
        {
            string outString = "";

            for (int i = pos; i < pos + length; i++)
            {
                outString += ByteToHexString(data[i]);
                if (i != pos + length - 1)
                {
                    outString += space;
                }
            }

            return outString;
        }

        //将ushort类型的数据转化为十六进制字符串
        public static string ushortToHexString(ushort[] data, int pos, int length)
        {
            string outString = "";

            for (int i = pos; i < pos + length; i++)
            {
                outString += ByteToHexString((byte)(data[i] >> 8));
                outString += ByteToHexString((byte)(data[i] & 0xFF));
            }

            return outString;
        }
    }
}
