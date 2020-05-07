using System;

namespace FastCore.UniqueID
{
    /// <summary>
    /// NHibernate改进的UUD UniqueID生成算法。如果想要生成字符串UID可以采用此算法。
    /// </summary>
    /// <remarks>
    /// https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/Id/GuidCombGenerator.cs
    /// using a strategy suggested Jimmy Nilsson's(使用Jimmy Nilsson建议的策略)
    /// http://www.informit.com/articles/article.asp?p=25862
    /// http://www.informit.com
    /// </remarks>
    public class GuidCombGenerator
    {
        private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

        /// <summary>
        /// Generate a new Guid using the comb algorithm.
        /// 使用comb算法生成一个新的Guid。
        /// </summary>
        public string NextId()
        {
            // 生成GUID
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            // 采用Utc时间
            DateTime now = DateTime.UtcNow;

            // Get the days and milliseconds which will be used to build the byte string 
            // 获取用于生成字节字符串的天数和毫秒数 
            TimeSpan days = new TimeSpan(now.Ticks - BaseDateTicks);
            TimeSpan msecs = now.TimeOfDay;

            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            // 转换为字节数组 
            // 注意，SQL Server精确到1/300毫秒，所以我们除以3.333333
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            // 反转字节以匹配SQL服务器顺序 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid
            // 将字节复制到GUID中 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            // 返回有序的GUID
            return new Guid(guidArray).ToString();
        }
    }
}
