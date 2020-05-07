using System;

namespace FastCore.UniqueID
{
    /// <summary>
    /// Twitter Snowflake UniqueID生成算法。如果想要生成数值UID可以采用此算法。
    /// From: https://github.com/twitter/snowflake
    /// An object that generates IDs.
    /// This is broken into a separate class in case
    /// we ever want to support multiple worker threads
    /// per process
    /// </summary>
    public class SnowflakeGenerator
    {
        private long workerId; // 工作节点ID(0~31)
        private long datacenterId; // 数据中心ID(0~31)
        private long sequence = 0L; // 毫秒内序列(0~4095)
        private long lastTimestamp = -1L; // 上次生成ID的时间截
        private static object syncRoot = new object(); // 同步锁

        //private static long twepoch = 1288834974657L; // 开始时间截(毫秒) Thu, 04 Nov 2010 01:42:54 GMT
        private static long twepoch = 63397900800000L; // 开始时间截(毫秒) = (long)new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks * 0.0001; 
        private static long workerIdBits = 5L; // 机器id所占的位数
        private static long datacenterIdBits = 5L; // 数据中心id所占的位数
        private static long maxWorkerId = -1L ^ (-1L << (int)workerIdBits); // 支持的最大机器id，结果是31 (这个移位算法可以很快的计算出几位二进制数所能表示的最大十进制数)
        private static long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits); //支持的最大数据中心id，结果是31
        private static long sequenceBits = 12L; // 序列id所占的位数

        private long workerIdOffset = sequenceBits; // 机器ID的偏移量(12)
        private long datacenterIdOffset = sequenceBits + workerIdBits; // 数据中心ID的偏移量(12+5)
        private long timestampOffset = sequenceBits + workerIdBits + datacenterIdBits; // 时间截的偏移量(5+5+12)
        private long sequenceMask = -1L ^ (-1L << (int)sequenceBits); // 生成序列的掩码，这里为4095 (0b111111111111=0xfff=4095)

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="workerId">工作ID, 取值范围: 0~31</param>
        /// <param name="datacenterId">数据中心ID, 取值范围: 0~31</param>
        public SnowflakeGenerator(long workerId, long datacenterId)
        {
            // 检查workerId是否正常
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"工作Id不能大于{maxWorkerId}或小于0");
            }
            if (datacenterId > maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"数据中心Id不能大于{maxDatacenterId}或小于0");
            }
            
            this.workerId = workerId;
            this.datacenterId = datacenterId;
        }

        /// <summary>
        /// 获得下一个ID
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            lock (syncRoot) // 同步锁保证线程安全
            {
                long timestamp = timeGen();

                // 系统时钟不允许回退
                // 说明：这种回拨机制判断仅限于运行时检查，如果关闭以后回拨系统时间，则生成的ID就同样存在风险。
                if (timestamp < lastTimestamp)
                {
                    throw new ApplicationException($"时钟回退过，拒绝在{lastTimestamp - timestamp}毫秒内生成id");
                }

                // 如果是同一时间生成的，则进行毫秒内序列生成
                if (lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & sequenceMask;
                    if (sequence == 0) // sequence等于0说明毫秒内序列已经增长到最大值
                    {
                        // 阻塞到下一个毫秒,获得新的时间戳
                        timestamp = tilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    sequence = 0L; // 时间戳改变，毫秒内序列重置
                }

                // 上次生成ID的时间截
                lastTimestamp = timestamp;

                // 移位并通过或运算拼到一起组成64位的ID
                return ((timestamp - twepoch) << (int)timestampOffset)  // 时间戳部分
                    | (datacenterId << (int)datacenterIdOffset)         // 数据中心部分
                    | (workerId << (int)workerIdOffset)                 // 机器标识部分
                    | sequence;                                         // 序列号部分
            }
        }

        /// <summary>
        /// 阻塞到下一个毫秒，直到获得新的时间戳
        /// </summary>
        /// <param name="lastTimestamp">上次生成ID的时间截</param>
        /// <returns>当前时间戳</returns>
        private long tilNextMillis(long lastTimestamp)
        {
            long timestamp = timeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = timeGen();
            }
            return timestamp;
        }

        /// <summary>
        /// 生成时间戳(毫秒)
        /// </summary>
        /// <returns></returns>
        private long timeGen()
        {
            //return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return (long)(DateTime.UtcNow.Ticks * 0.0001); // 转换成毫秒数
        }

    }
}
