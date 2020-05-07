using System;
using System.Reflection;

namespace FastCore.Redis
{
    //private const string WrongTypeErr = "-WRONGTYPE Operation against a key holding the wrong kind of value";
    //private const string NokeyErr = "-ERR no such key";
    //private const string SyntaxErr = "-ERR syntax error";
    //private const string SameObjectErr = "-ERR source and destination objects are the same";
    //private const string OutOfRangeErr = "-ERR index out of range";
    //private const string NoscriptErr = "-NOSCRIPT No matching script. Please use EVAL.";
    //private const string LoadingErr = "-LOADING Redis is loading the dataset in memory";
    //private const string SlowScriptErr = "-BUSY Redis is busy running a script. You can only call SCRIPT KILL or SHUTDOWN NOSAVE.";
    //private const string MasterDownErr = "-MASTERDOWN Link with MASTER is down and replica-serve-stale-data is set to 'no'.";
    //private const string BgSaveErr = "-MISCONF Redis is configured to save RDB snapshots, but it is currently not able to persist on disk. Commands that may modify the data set are disabled, because this instance is configured to report errors during writes if RDB snapshotting fails (stop-writes-on-bgsave-error option). Please check the Redis logs for details about the RDB error.";
    //private const string RoSlaveErr = "-READONLY You can't write against a read only replica.";
    //private const string NoAuthErr = "-NOAUTH Authentication required.";
    //private const string OomErr = "-OOM command not allowed when used memory > 'maxmemory'.";
    //private const string ExecabortErr = "-EXECABORT Transaction discarded because of previous errors.";
    //private const string NoReplicasErr = "-NOREPLICAS Not enough good replicas to write.";
    //private const string BusykeyErr = "-BUSYKEY Target key name already exists.";
    //private const string CrossSlotErr = "-CROSSSLOT Keys in request don't hash to the same slot";
    //private const string ClusterDownStateErr = "-CLUSTERDOWN The cluster is down";
    //private const string ClusterDownUnBoundErr = "-CLUSTERDOWN Hash slot not served";
    //private const string InvalidHllErr = "-INVALIDOBJ Corrupted HLL object detected";
    //private const string MaxClientsErr = "-ERR max number of clients reached";
    //private const string NoMasterLinkErr = "-NOMASTERLINK Can't SYNC while not connected with my master";
    //private const string MisConfigErr = "-MISCONF Errors writing to the AOF file";
    //private const string NotBusyErr = "-NOTBUSY No scripts in execution right now.";
    //private const string UnKillableBusyErr = "-UNKILLABLE The busy script was sent by a master instance in the context of replication and cannot be killed.";
    //private const string UnKillableExecutedErr = "-UNKILLABLE Sorry the script already executed write commands against the dataset. You can either wait the script termination or kill";
    //private const string InProgErr = "-INPROG Failover already in progress";
    //private const string NoGoodSlaveErr = "-NOGOODSLAVE No suitable replica to promote";

    /// <summary>
    /// Redis服务器异常
    /// </summary>
    public class RedisServerException : Exception
    {
        private string _message;

        /// <summary>
        /// 实例化
        /// </summary>
        public RedisServerException()
        {
            //
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="message"></param>
        public RedisServerException(string message) : base(message)
        {
            _message = message;
            base.Source = "RedisServer";
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public RedisServerException(string message, Exception exception) : base(message, exception)
        {
            _message = message;
            base.Source = "RedisServer";
        }

        /// <summary>
        /// 消息
        /// </summary>
        public override string Message 
        {
            get { return _message; }
        }

        /// <summary>
        /// 跟踪
        /// </summary>
        public override string StackTrace
        {
            get { return base.StackTrace; }
        }
    }

    /// <summary>
    /// Redis客户端异常
    /// </summary>
    public class RedisClientException : Exception
    {
        private string _message;
        private string _source;
        private string _trace;

        /// <summary>
        /// 实例化
        /// </summary>
        public RedisClientException()
        {
            //
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="message">消息</param>
        public RedisClientException(string message) : base(message)
        {
            _message = message;
            base.Source = "RedisClient";
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="exception">异常</param>
        public RedisClientException(string message, Exception exception) : base(message, exception)
        {
            _message = message;
            _source = exception.Source;
            _trace = exception.StackTrace;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">消息来源</param>
        /// <param name="extraData">附加数据</param>
        public RedisClientException(string message, MethodBase source = null, string extraData = "") : base(message)
        {
            _message = message;
            _source = source == null ? "" : $"{source.ReflectedType.FullName}.{source.Name}";
            _trace = string.IsNullOrEmpty(extraData) ? "" : extraData + "\r\n";
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">异常来源</param>
        /// <param name="extraData">附加数据</param>
        public RedisClientException(Exception exception, MethodBase source = null, string extraData = "") : base(exception.Message, exception)
        {
            _message = exception.Message;
            _source = source == null ? exception.Source : $"{source.ReflectedType.FullName}.{source.Name}";
            _trace = exception.StackTrace + "\r\n" + (string.IsNullOrEmpty(extraData) ? "" : extraData + "\r\n");
        }

        /// <summary>
        /// 消息
        /// </summary>
        public override string Message
        {
            get { return _message; }
        }

        /// <summary>
        /// 来源
        /// </summary>
        public override string Source
        {
            get { return _source; }
        }

        /// <summary>
        /// 跟踪
        /// </summary>
        public override string StackTrace
        {
            get { return _trace; }
        }
    }

}
