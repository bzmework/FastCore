using System;
using System.Collections.Generic;
using System.Text;

namespace FastCore.Json
{
    /// <summary>
    /// 数值转换函数，性能大大优于System.Convert
    /// </summary>
    /// <remarks>
    /// 代码来源：https://github.com/bzmework/TLib
    /// </remarks>
    public unsafe static class QuickConvert
    {
        private const int DBL_MIN_EXP = (-1021);  // min binary exponent
        private const int DBL_MAX_EXP = 1024;  // max binary exponent

        private const double _HUGE_ENUF = 1e+300;  // _HUGE_ENUF*_HUGE_ENUF must overflow
        private const float INFINITY = ((float)(_HUGE_ENUF * _HUGE_ENUF));
        private const double HUGE_VAL = ((double)INFINITY);

        /// <summary>
        /// 转换int类型值为字符串
        /// </summary>
        /// <param name="val">要转换的int类型值</param>
        /// <returns></returns>
        public static string C2Str(int val)
        {
            char* outVal = stackalloc char[12]; // 缓存转换后的字符。容纳int类型值的缓存，其大小至少是12*sizeof(char)个字节。
            char* left = outVal;  // 左边位置
            char* p = outVal;     // 当前位置
            int n;            // 当前数字
            char c;            // 当前字符

            // 负数须转换成正数
            if (val < 0)
            {
                val = -val; // 转换成正数
                *p = '-'; // 在输出缓存写入负号
                left = ++p; // 向前移动一个位置
            }

            // 将数字转换成字符
            do
            {
                n = val % 10; // 取得个位
                val /= 10; // 去掉个位
                *p++ = (char)(n + '0'); // 转换数字为'0'-'9'之间的字符
            } while (val > 0);
            *p-- = '\0'; // 必须添加结束符(NUL)并后移指针

            // 反转'-'到'\0'之间的数字为正确的顺序
            do
            {
                c = *left;
                *left++ = *p;
                *p-- = c;
            } while (left < p);

            // 返回
            return new string(outVal);
        }

        /// <summary>
        /// 转换unsigned int类型值为字符串
        /// </summary>
        /// <param name="val">要转换的unsigned int类型值</param>
        /// <returns></returns>
        public static string C2Str(uint val)
        {
            char* outVal = stackalloc char[20]; // 缓存转换后的字符。容纳int类型值的缓存，其大小至少是20*sizeof(char)个字节。
            char* left = outVal;  // 左边位置
            char* p = outVal;     // 当前位置
            uint n;           // 当前数字
            char c;            // 当前字符

            // 将数字转换成字符
            do
            {
                n = val % 10; //取得个位
                val /= 10; //去掉个位
                *p++ = (char)(n + '0');// 转换数字为'0'-'9'之间的字符
            } while (val > 0);
            *p-- = '\0'; // 必须添加结束符(NUL)并后移指针

            // 反转'-'到'\0'之间的数字为正确的顺序
            do
            {
                c = *left;
                *left++ = *p;
                *p-- = c;
            } while (left < p);

            // 返回
            return new string(outVal);
        }

        /// <summary>
        /// 转换long类型值为字符串
        /// </summary>
        /// <param name="val">要转换的long类型值</param>
        /// <returns></returns>
        public static string C2Str(long val)
        {
            char* outVal = stackalloc char[20]; // 缓存转换后的字符。容纳int类型值的缓存，其大小至少是20*sizeof(char)个字节。
            char* left = outVal;  // 左边位置
            char* p = outVal;     // 当前位置
            long n;             // 当前数字
            char c;            // 当前字符

            //负数须转换成正数
            if (val < 0)
            {
                val = -val; //转换成正数
                *p = '-'; //在输出缓存写入负号
                left = ++p; //向前移动一个位置
            }

            //将数字转换成字符
            do
            {
                n = val % 10; //取得个位
                val /= 10; //去掉个位
                *p++ = (char)(n + '0');// 转换数字为'0'-'9'之间的字符
            } while (val > 0);
            *p-- = '\0'; // 必须添加结束符(NUL)并后移指针

            // 反转'-'到'\0'之间的数字为正确的顺序
            do
            {
                c = *left;
                *left++ = *p;
                *p-- = c;
            } while (left < p);

            // 返回
            return new string(outVal);
        }

        /// <summary>
        /// 转换unsigned long类型值为字符串
        /// </summary>
        /// <param name="val">要转换的unsigned long类型值</param>
        /// <returns></returns>
        public static string C2Str(ulong val)
        {
            char* outVal = stackalloc char[21]; // 缓存转换后的字符。容纳int类型值的缓存，其大小至少是21*sizeof(char)个字节。
            char* left = outVal;  // 左边位置
            char* p = outVal;     // 当前位置
            ulong n;            // 当前数字
            char c;            // 当前字符

            // 将数字转换成字符
            do
            {
                n = val % 10; // 取得个位
                val /= 10; // 去掉个位
                *p++ = (char)(n + '0');// 转换数字为'0'-'9'之间的字符
            } while (val > 0);
            *p-- = '\0'; // 必须添加结束符(NUL)并后移指针

            // 反转'-'到'\0'之间的数字为正确的顺序
            do
            {
                c = *left;
                *left++ = *p;
                *p-- = c;
            } while (left < p);

            // 返回
            return new string(outVal);
        }

        /// <summary>
        /// 转换字符串为double类型值
        /// </summary>
        /// <remarks>
        /// c2dbl()会扫描参数str字符串，跳过前面的空格字符，直到遇上数字或正负符号才开始做转换，
        /// 到出现非数字或字符串结束时('\0')才结束转换，并将结果返回。
        /// 参数str字符串可包含正负号、小数点或E(e)来表示指数部分。如123.456或123e-2或123.456e2。
        /// </remarks>
        /// <param name="str">要转换的字符串</param>
        /// <returns>返回double值</returns>
        public static double C2Dbl(string str)
        {
	        double number = 0.0;
	        int num_digits = 0;
	        int num_decimals = 0;	// 小数位数
	        int exponent = 0;		// 指数
	        bool negative;			// 负号 
	        double p10;
	        int n;

            fixed (char* pd = str.ToCharArray())
            {
                char* p = pd;

                // 跳过前导空格等
                while (IsSpace(*p))
                {
                    p++;
                }

                // 处理正负符号
                negative = false;
                if (*p == '-')
                {
                    negative = true;
                    p++; // 跳过负号
                }
                else if (*p == '+')
                {
                    p++; // 跳过正号
                }

                // 转换字符为数字
                // number*10: 将number扩大10倍，目的是加上个位数，为什么不扩大100倍，因为只需要加两位数的0-9。
                // 例如我们要将2019-05转换成201905，只需要将年*100加上月即可，即2019*100+5，既满足了转换又起到了格式化的效果。
                // 将数扩大n倍+x(0<x<n),这是转换字符串为数字的常用算法和技巧。
                while (IsDigit(*p))
                {
                    number = number * 10.0 + ((double)*p - '0');  // 转换'0'-'9'字符为数字
                    p++;
                    num_digits++;
                }

                // 处理小数部分
                // 例如：123.456
                if (*p == '.')
                {
                    p++;
                    while (IsDigit(*p))
                    {
                        number = number * 10.0 + ((double)*p - '0');  // 转换'0'-'9'字符为数字
                        p++;
                        num_digits++;
                        num_decimals++; //记录小数位数
                    }

                    exponent -= num_decimals;
                }

                // 无任何数字
                if (num_digits == 0)
                {
                    return 0.0;
                }

                // 加上负号
                if (negative)
                {
                    number = -number;
                }

                // 处理指数
                // 例如：123e-2
                if (*p == 'e' || *p == 'E')
                {
                    // 处理正负符号
                    negative = false;
                    if (*p == '-')
                    {
                        negative = true;
                        p++; // 跳过负号
                    }
                    else if (*p == '+')
                    {
                        p++; // 跳过正号
                    }

                    // 转换字符为数字
                    n = 0;
                    while (IsDigit(*p))
                    {
                        n = n * 10 + ((int)*p - '0');  // 转换'0'-'9'字符为数字
                        p++;
                    }

                    // 处理指数
                    if (negative)
                    {
                        exponent -= n;
                    }
                    else
                    {
                        exponent += n;
                    }
                }

                // 指数溢出
                if (exponent < DBL_MIN_EXP || exponent > DBL_MAX_EXP)
                {
                    return HUGE_VAL;
                }

                // 将指数部分附加到number上
                p10 = 10.0;
                n = exponent;
                if (n < 0)
                    n = -n;
                while (n != 0)
                {
                    if ((n & 1) != 0) // 求余，相当于：n % 2；
                    {
                        if (exponent < 0)
                        {
                            number /= p10;
                        }
                        else
                        {
                            number *= p10;
                        }
                    }
                    n >>= 1; // 整除，相当于：(int)(n / 2);
                    p10 *= p10;
                }
            }

            // 溢出
            if (number == HUGE_VAL)
                return HUGE_VAL;

            return number;
        }

        // 字符是否为空格符、制表符、回车、换行、纵向制表符和换页符
        // 0x20,空格符;0x0d(\r),回车;0x0a(\n),换行;0x09(\t),制表符;0x0b(\v),纵向制表符;0x0c(\f),换页符.
        private static bool IsSpace(char c)
        {
            return c == 0x20 || c == 0x0d || c == 0x0a || c == 0x09 || c == 0x0b || c == 0x0c;
        }

        // 字符是否为0-9之间的数字
        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

    }
}
