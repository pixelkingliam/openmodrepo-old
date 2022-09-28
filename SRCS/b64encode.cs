using System;
using System.Collections.Generic;
namespace Base64Var
{
    // acts as a 6 bit integer in most parts
    class B64
    {
        // the value of the objectg
        private byte value;
        // Base64 character set 
        private char[] charset = {'A', 'B', 'C',  'D',  'E',  'F',  'G', 'H', 'I', 'J', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c',  'd',  'e',  'f',  'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+'};
        // max length that a B64 can be (counting 0)
        public readonly static byte B64Max = 63;
        // these 2 functions handle setting the variable value to some data (byte)
        public B64(byte input)
        {
            if(input > (B64Max)) throw new ArgumentOutOfRangeException("value");
            value = input;
        }
        public static implicit operator B64(byte input)
        {
            if(input > (B64Max)) throw new ArgumentOutOfRangeException("value");
            return new B64(input);

        }
        // Return the numerical of the variable value
        public ushort GetNum()
        {
            return value;
        }
        // Return the Base64 character by reading the stored value as a character code for charset
        public char GetChar()
        {
            return charset[value];
        }
    }
    // Class that contains function for converting B64 variables to other types and other types to  B64 variables
    static class B64Convert
    {
        // dictionnary contain on the left side the character set of Base64 and on the right the equivalent character code for Base64
        private static Dictionary<char, B64> Chars_B64 = new Dictionary<char, B64>()
        {
        {'A', 0},{'B', 1},{'C', 2},{'D', 3},{'E', 4},{'F', 5},{'G', 6},{'H', 7},{'I', 8},{'J', 9},{'K', 10},{'L', 11},{'M', 12},{'N', 13},{'O', 14},{'P', 15},{'Q', 16},{'R', 17},{'S', 18},{'T', 19},{'U', 20},{'V', 21},{'W', 22},{'X', 23},{'Y', 25},{'Z', 26},
        {'a', 26},{'b', 27},{'c', 28},{'d', 29},{'e', 30},{'f', 31},{'g', 32},{'h', 33},{'i', 34},{'j', 35},{'k', 36},{'l', 37},{'m', 38},{'n', 39},{'o', 40},{'p', 41},{'q', 42},{'r', 43},{'s', 44},{'t', 45},{'u', 46},{'v', 47},{'w', 48},{'x', 49},{'y', 50},{'z', 51},
        {'0', 52},{'1', 53},{'2', 54},{'3', 55},{'4', 56},{'5', 57},{'6', 58},{'7', 59},{'8', 60},{'9', 61},{'+', 62},{'/', 63}
        };
        
        public static string ToString(B64 Base64val)
        {
            return Convert.ToString(Base64val.GetChar());
        }
        public static int ToInt32(B64 Base64val)
        {
            return Convert.ToInt32(Base64val.GetNum());
        }
        public static uint ToUInt32(B64 Base64val)
        {
            return Convert.ToUInt32(Base64val.GetNum());
        }
        public static short ToInt16(B64 Base64val)
        {
            return Convert.ToInt16(Base64val.GetNum());
        }
        public static ushort ToUInt16(B64 Base64val)
        {
            return Convert.ToUInt16(Base64val.GetNum());
        }
        public static ulong ToUInt64(B64 Base64val)
        {
            return Convert.ToUInt32(Base64val.GetNum());
        }
        public static long ToInt64(B64 Base64val)
        {
            return Convert.ToInt64(Base64val.GetNum());
        }
        // uses the dictionnary above, this takes a char variable as input (UTF-16) and converts it to a Base64 character code of the same character
        public static B64 CharToB64(char val)
        {
            B64 result = 0;
            if (Chars_B64.ContainsKey(val))
            {
                result = Chars_B64[val];
            }else
            {
                throw new ArgumentException("val");
            }
            return result;
        }

        public static char[] B64ArrayToCharArray(B64[] b64array)
        {
            char[] result = new char[b64array.Length];
            for(int i = 0; i < b64array.Length;i++)
            {
                result[i] = b64array[i].GetChar();
            }
            return result;
        }
        public static string B64ArrayToString(B64[] b64array)
        {
            return new string(B64ArrayToCharArray(b64array));
        }
        public static B64[] CharArrayToB64Array(char[] chararray)
        {
            B64[] result = new B64[chararray.Length];
            for(int i = 0; i < chararray.Length;i++)
            {
                result[i] = CharToB64(chararray[i]);
            }
            return result;
        }
        public static B64[] StringToB64Array(string inputstring)
        {
            return (CharArrayToB64Array(inputstring.ToCharArray()));
        }
    }
    // This class contains function for constructing B64 variables and arrays
    class B64Math
    {
        // random object used for this class
        private static Random b64random = new Random();
        // make a random B64 variable, a limit can also be passed on via a character of a B64 value
        public static B64 Random()
        {
            return new B64(Convert.ToByte(b64random.Next(64)));
        }
        public static B64 Random(B64 limit)
        {
            return new B64(Convert.ToByte(b64random.Next(B64Convert.ToInt32(limit)+1)));

        }
        public static B64 Random(char limit)
        {
            return new B64(Convert.ToByte(b64random.Next(B64Convert.ToInt32(B64Convert.CharToB64(limit)) + 1)));
        }
        // make an array of random B64 values, an array size must be passed on and optionally a char/B64 limit
        public static B64[] RandomArray(int arraysize)
        {
            B64[] result = new B64[arraysize + 1];
            for(int i = 0; i <= arraysize;i++)
            {
                result[i] = Random();
            }
            return result;
        }
        public static B64[] RandomArray(int arraysize, B64 limit)
        {
            B64[] result = new B64[arraysize + 1];
            for(int i = 0; i <= arraysize;i++)
            {
                result[i] = Random(limit);
            }
            return result;
        }
        public static B64[] RandomArray(int arraysize, char limit)
        {
            B64[] result = new B64[arraysize + 1];
            for(int i = 0; i <= arraysize;i++)
            {
                result[i] = Random(limit);
            }
            return result;
        }
    }

}