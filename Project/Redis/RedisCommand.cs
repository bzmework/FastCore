using System;
using System.Collections.Generic;
using System.Text;
using p = FastCore.ObjectConvert;

namespace FastCore.Redis
{
    /// <summary>
    /// Redis支持的命令
    /// </summary>
    internal static class Commands
    {
        public readonly static object Ok = true; // true对象
        public readonly static object NullObject = null; // null对象

        public readonly static byte[] RESPBulkStringsPrefix = p.C2Bytes("$"); // RESP大容量字符串前缀
        public readonly static byte[] RESPArraysPrefix = p.C2Bytes("*"); // RESP数组前缀
        public readonly static byte[] NewLine = p.C2Bytes("\r\n"); // 回车换行
        public readonly static byte[] NullChar  = p.C2Bytes("\0"); // 空(Nul)字符
        public readonly static byte[] NullCharLen = p.C2Bytes("-1"); // 空(Null)字符串长度
        public readonly static byte[] EmptyCharLen = p.C2Bytes("0"); // 空(Empty)字符串长度
        public readonly static byte[] OneElement = p.C2Bytes("1"); // 1个元素
        public readonly static byte[] ThreeElement = p.C2Bytes("3"); // 3个元素

        public readonly static byte[] Quit = p.C2Bytes("QUIT");
        public readonly static byte[] Auth = p.C2Bytes("AUTH");
        public readonly static byte[] Exists = p.C2Bytes("EXISTS");
        public readonly static byte[] Del = p.C2Bytes("DEL");
        public readonly static byte[] Type = p.C2Bytes("TYPE");
        public readonly static byte[] Keys = p.C2Bytes("KEYS");
        public readonly static byte[] RandomKey = p.C2Bytes("RANDOMKEY");
        public readonly static byte[] Rename = p.C2Bytes("RENAME");
        public readonly static byte[] RenameNx = p.C2Bytes("RENAMENX");
        public readonly static byte[] PExpire = p.C2Bytes("PEXPIRE");
        public readonly static byte[] PExpireAt = p.C2Bytes("PEXPIREAT");
        public readonly static byte[] DbSize = p.C2Bytes("DBSIZE");
        public readonly static byte[] Expire = p.C2Bytes("EXPIRE");
        public readonly static byte[] ExpireAt = p.C2Bytes("EXPIREAT");
        public readonly static byte[] Ttl = p.C2Bytes("TTL");
        public readonly static byte[] PTtl = p.C2Bytes("PTTL");
        public readonly static byte[] Select = p.C2Bytes("SELECT");
        public readonly static byte[] FlushDb = p.C2Bytes("FLUSHDB");
        public readonly static byte[] FlushAll = p.C2Bytes("FLUSHALL");
        public readonly static byte[] Ping = p.C2Bytes("PING");
        public readonly static byte[] Echo = p.C2Bytes("ECHO");

        public readonly static byte[] Save = p.C2Bytes("SAVE");
        public readonly static byte[] BgSave = p.C2Bytes("BGSAVE");
        public readonly static byte[] LastSave = p.C2Bytes("LASTSAVE");
        public readonly static byte[] Shutdown = p.C2Bytes("SHUTDOWN");
        public readonly static byte[] NoSave = p.C2Bytes("NOSAVE");
        public readonly static byte[] BgRewriteAof = p.C2Bytes("BGREWRITEAOF");

        public readonly static byte[] Info = p.C2Bytes("INFO");
        public readonly static byte[] SlaveOf = p.C2Bytes("SLAVEOF");
        public readonly static byte[] No = p.C2Bytes("NO");
        public readonly static byte[] One = p.C2Bytes("ONE");
        public readonly static byte[] ResetStat = p.C2Bytes("RESETSTAT");
        public readonly static byte[] Rewrite = p.C2Bytes("REWRITE");
        public readonly static byte[] Time = p.C2Bytes("TIME");
        public readonly static byte[] Segfault = p.C2Bytes("SEGFAULT");
        public readonly static byte[] Sleep = p.C2Bytes("SLEEP");
        public readonly static byte[] Dump = p.C2Bytes("DUMP");
        public readonly static byte[] Restore = p.C2Bytes("RESTORE");
        public readonly static byte[] Migrate = p.C2Bytes("MIGRATE");
        public readonly static byte[] Move = p.C2Bytes("MOVE");
        public readonly static byte[] Object = p.C2Bytes("OBJECT");
        public readonly static byte[] IdleTime = p.C2Bytes("IDLETIME");
        public readonly static byte[] Monitor = p.C2Bytes("MONITOR");		//missing
        public readonly static byte[] Debug = p.C2Bytes("DEBUG");			//missing
        public readonly static byte[] Config = p.C2Bytes("CONFIG");			//missing
        public readonly static byte[] Client = p.C2Bytes("CLIENT");
        public readonly static byte[] List = p.C2Bytes("LIST");
        public readonly static byte[] Kill = p.C2Bytes("KILL");
        public readonly static byte[] Addr = p.C2Bytes("ADDR");
        public readonly static byte[] Id = p.C2Bytes("ID");
        public readonly static byte[] SkipMe = p.C2Bytes("SKIPME");
        public readonly static byte[] SetName = p.C2Bytes("SETNAME");
        public readonly static byte[] GetName = p.C2Bytes("GETNAME");
        public readonly static byte[] Pause = p.C2Bytes("PAUSE");
        public readonly static byte[] Role = p.C2Bytes("ROLE");
 
        public readonly static byte[] StrLen = p.C2Bytes("STRLEN");
        public readonly static byte[] Set = p.C2Bytes("SET");
        public readonly static byte[] Get = p.C2Bytes("GET");
        public readonly static byte[] GetSet = p.C2Bytes("GETSET");
        public readonly static byte[] MGet = p.C2Bytes("MGET");
        public readonly static byte[] SetNx = p.C2Bytes("SETNX");
        public readonly static byte[] SetEx = p.C2Bytes("SETEX");
        public readonly static byte[] Persist = p.C2Bytes("PERSIST");
        public readonly static byte[] PSetEx = p.C2Bytes("PSETEX");
        public readonly static byte[] MSet = p.C2Bytes("MSET");
        public readonly static byte[] MSetNx = p.C2Bytes("MSETNX");
        public readonly static byte[] Incr = p.C2Bytes("INCR");
        public readonly static byte[] IncrBy = p.C2Bytes("INCRBY");
        public readonly static byte[] IncrByFloat = p.C2Bytes("INCRBYFLOAT");
        public readonly static byte[] Decr = p.C2Bytes("DECR");
        public readonly static byte[] DecrBy = p.C2Bytes("DECRBY");
        public readonly static byte[] Append = p.C2Bytes("APPEND");
        public readonly static byte[] GetRange = p.C2Bytes("GETRANGE");
        public readonly static byte[] SetRange = p.C2Bytes("SETRANGE");
        public readonly static byte[] GetBit = p.C2Bytes("GETBIT");
        public readonly static byte[] SetBit = p.C2Bytes("SETBIT");
        public readonly static byte[] BitCount = p.C2Bytes("BITCOUNT");

        public readonly static byte[] Scan = p.C2Bytes("SCAN");
        public readonly static byte[] SScan = p.C2Bytes("SSCAN");
        public readonly static byte[] HScan = p.C2Bytes("HSCAN");
        public readonly static byte[] ZScan = p.C2Bytes("ZSCAN");
        public readonly static byte[] Match = p.C2Bytes("MATCH");
        public readonly static byte[] Count = p.C2Bytes("COUNT");

        public readonly static byte[] PfAdd = p.C2Bytes("PFADD");
        public readonly static byte[] PfCount = p.C2Bytes("PFCOUNT");
        public readonly static byte[] PfMerge = p.C2Bytes("PFMERGE");

        public readonly static byte[] RPush = p.C2Bytes("RPUSH");
        public readonly static byte[] LPush = p.C2Bytes("LPUSH");
        public readonly static byte[] RPushX = p.C2Bytes("RPUSHX");
        public readonly static byte[] LPushX = p.C2Bytes("LPUSHX");
        public readonly static byte[] LLen = p.C2Bytes("LLEN");
        public readonly static byte[] LRange = p.C2Bytes("LRANGE");
        public readonly static byte[] LTrim = p.C2Bytes("LTRIM");
        public readonly static byte[] LIndex = p.C2Bytes("LINDEX");
        public readonly static byte[] LInsert = p.C2Bytes("LINSERT");
        public readonly static byte[] Before = p.C2Bytes("BEFORE");
        public readonly static byte[] After = p.C2Bytes("AFTER");
        public readonly static byte[] LSet = p.C2Bytes("LSET");
        public readonly static byte[] LRem = p.C2Bytes("LREM");
        public readonly static byte[] LPop = p.C2Bytes("LPOP");
        public readonly static byte[] RPop = p.C2Bytes("RPOP");
        public readonly static byte[] BLPop = p.C2Bytes("BLPOP");
        public readonly static byte[] BRPop = p.C2Bytes("BRPOP");
        public readonly static byte[] RPopLPush = p.C2Bytes("RPOPLPUSH");
        public readonly static byte[] BRPopLPush = p.C2Bytes("BRPOPLPUSH");

        public readonly static byte[] SAdd = p.C2Bytes("SADD");
        public readonly static byte[] SRem = p.C2Bytes("SREM");
        public readonly static byte[] SPop = p.C2Bytes("SPOP");
        public readonly static byte[] SMove = p.C2Bytes("SMOVE");
        public readonly static byte[] SCard = p.C2Bytes("SCARD");
        public readonly static byte[] SIsMember = p.C2Bytes("SISMEMBER");
        public readonly static byte[] SInter = p.C2Bytes("SINTER");
        public readonly static byte[] SInterStore = p.C2Bytes("SINTERSTORE");
        public readonly static byte[] SUnion = p.C2Bytes("SUNION");
        public readonly static byte[] SUnionStore = p.C2Bytes("SUNIONSTORE");
        public readonly static byte[] SDiff = p.C2Bytes("SDIFF");
        public readonly static byte[] SDiffStore = p.C2Bytes("SDIFFSTORE");
        public readonly static byte[] SMembers = p.C2Bytes("SMEMBERS");
        public readonly static byte[] SRandMember = p.C2Bytes("SRANDMEMBER");

        public readonly static byte[] ZAdd = p.C2Bytes("ZADD");
        public readonly static byte[] ZRem = p.C2Bytes("ZREM");
        public readonly static byte[] ZIncrBy = p.C2Bytes("ZINCRBY");
        public readonly static byte[] ZRank = p.C2Bytes("ZRANK");
        public readonly static byte[] ZRevRank = p.C2Bytes("ZREVRANK");
        public readonly static byte[] ZRange = p.C2Bytes("ZRANGE");
        public readonly static byte[] ZRevRange = p.C2Bytes("ZREVRANGE");
        public readonly static byte[] ZRangeByScore = p.C2Bytes("ZRANGEBYSCORE");
        public readonly static byte[] ZRevRangeByScore = p.C2Bytes("ZREVRANGEBYSCORE");
        public readonly static byte[] ZCard = p.C2Bytes("ZCARD");
        public readonly static byte[] ZScore = p.C2Bytes("ZSCORE");
        public readonly static byte[] ZCount = p.C2Bytes("ZCOUNT");
        public readonly static byte[] ZRemRangeByRank = p.C2Bytes("ZREMRANGEBYRANK");
        public readonly static byte[] ZRemRangeByScore = p.C2Bytes("ZREMRANGEBYSCORE");
        public readonly static byte[] ZUnionStore = p.C2Bytes("ZUNIONSTORE");
        public readonly static byte[] ZInterStore = p.C2Bytes("ZINTERSTORE");
        public static readonly byte[] ZRangeByLex = p.C2Bytes("ZRANGEBYLEX");
        public static readonly byte[] ZLexCount = p.C2Bytes("ZLEXCOUNT");
        public static readonly byte[] ZRemRangeByLex = p.C2Bytes("ZREMRANGEBYLEX");

        public readonly static byte[] HSet = p.C2Bytes("HSET");
        public readonly static byte[] HSetNx = p.C2Bytes("HSETNX");
        public readonly static byte[] HGet = p.C2Bytes("HGET");
        public readonly static byte[] HMSet = p.C2Bytes("HMSET");
        public readonly static byte[] HMGet = p.C2Bytes("HMGET");
        public readonly static byte[] HIncrBy = p.C2Bytes("HINCRBY");
        public readonly static byte[] HIncrByFloat = p.C2Bytes("HINCRBYFLOAT");
        public readonly static byte[] HExists = p.C2Bytes("HEXISTS");
        public readonly static byte[] HDel = p.C2Bytes("HDEL");
        public readonly static byte[] HLen = p.C2Bytes("HLEN");
        public readonly static byte[] HKeys = p.C2Bytes("HKEYS");
        public readonly static byte[] HVals = p.C2Bytes("HVALS");
        public readonly static byte[] HGetAll = p.C2Bytes("HGETALL");

        public readonly static byte[] Sort = p.C2Bytes("SORT");

        public readonly static byte[] Watch = p.C2Bytes("WATCH");
        public readonly static byte[] UnWatch = p.C2Bytes("UNWATCH");
        public readonly static byte[] Multi = p.C2Bytes("MULTI");
        public readonly static byte[] Exec = p.C2Bytes("EXEC");
        public readonly static byte[] Discard = p.C2Bytes("DISCARD");

        public readonly static byte[] Subscribe = p.C2Bytes("SUBSCRIBE");
        public readonly static byte[] UnSubscribe = p.C2Bytes("UNSUBSCRIBE");
        public readonly static byte[] PSubscribe = p.C2Bytes("PSUBSCRIBE");
        public readonly static byte[] PUnSubscribe = p.C2Bytes("PUNSUBSCRIBE");
        public readonly static byte[] Publish = p.C2Bytes("PUBLISH");


        public readonly static byte[] WithScores = p.C2Bytes("WITHSCORES");
        public readonly static byte[] Limit = p.C2Bytes("LIMIT");
        public readonly static byte[] By = p.C2Bytes("BY");
        public readonly static byte[] Asc = p.C2Bytes("ASC");
        public readonly static byte[] Desc = p.C2Bytes("DESC");
        public readonly static byte[] Alpha = p.C2Bytes("ALPHA");
        public readonly static byte[] Store = p.C2Bytes("STORE");

        public readonly static byte[] Eval = p.C2Bytes("EVAL");
        public readonly static byte[] EvalSha = p.C2Bytes("EVALSHA");
        public readonly static byte[] Script = p.C2Bytes("SCRIPT");
        public readonly static byte[] Load = p.C2Bytes("LOAD");
        //public readonly static byte[] Exists = "EXISTS".ToUtf8Bytes();
        public readonly static byte[] Flush = p.C2Bytes("FLUSH");
        public readonly static byte[] Slowlog = p.C2Bytes("SLOWLOG");

        public readonly static byte[] Ex = p.C2Bytes("EX");
        public readonly static byte[] Px = p.C2Bytes("PX");
        public readonly static byte[] Nx = p.C2Bytes("NX");
        public readonly static byte[] Xx = p.C2Bytes("XX");

        // Sentinel commands
        public readonly static byte[] Sentinel = p.C2Bytes("SENTINEL");
        public readonly static byte[] Masters = p.C2Bytes("masters");
        public readonly static byte[] Sentinels = p.C2Bytes("sentinels");
        public readonly static byte[] Master = p.C2Bytes("master");
        public readonly static byte[] Slaves = p.C2Bytes("slaves");
        public readonly static byte[] Failover = p.C2Bytes("failover");
        public readonly static byte[] GetMasterAddrByName = p.C2Bytes("get-master-addr-by-name");

        //Geo commands
        public readonly static byte[] GeoAdd = p.C2Bytes("GEOADD");
        public readonly static byte[] GeoDist = p.C2Bytes("GEODIST");
        public readonly static byte[] GeoHash = p.C2Bytes("GEOHASH");
        public readonly static byte[] GeoPos = p.C2Bytes("GEOPOS");
        public readonly static byte[] GeoRadius = p.C2Bytes("GEORADIUS");
        public readonly static byte[] GeoRadiusByMember = p.C2Bytes("GEORADIUSBYMEMBER");

        public readonly static byte[] WithCoord = p.C2Bytes("WITHCOORD");
        public readonly static byte[] WithDist = p.C2Bytes("WITHDIST");
        public readonly static byte[] WithHash = p.C2Bytes("WITHHASH");

        public readonly static byte[] Meters = p.C2Bytes("m");
        public readonly static byte[] Kilometers = p.C2Bytes("km");
        public readonly static byte[] Miles = p.C2Bytes("mi");
        public readonly static byte[] Feet = p.C2Bytes("ft");

        public static byte[] GetUnit(string unit)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));

            switch (unit)
            {
                case "m":
                    return Meters;
                case "km":
                    return Kilometers;
                case "mi":
                    return Miles;
                case "ft":
                    return Feet;
                default:
                    throw new NotSupportedException("Unit '{0}' is not a valid unit" + unit);
            }
        }
    }

    /// <summary>
    /// 命令类型
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// 单条命令
        /// </summary>
        One,

        /// <summary>
        /// 批量命令
        /// </summary>
        Batch,

        /// <summary>
        /// 多条命令
        /// </summary>
        Multiple
    }

    /// <summary>
    /// Redis命令
    /// </summary>
    public class RedisCommand
    {
        private CommandType _type;
        private byte[] _cmd;
        private object _value;

        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// 命令
        /// </summary>
        public byte[] Command
        {
            get { return _cmd; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public object[] Value 
        {
            get
            {
                if (this.Type == CommandType.Multiple)
                {
                    // 多条命令，命令格式：
                    // 命令A Key Value [Key1 Value1 Key2 Value2 ...]
                    // 命令B Key Value [Key1 Value1 Key2 Value2 ...]
                    // 命令C Key Value [Key1 Value1 Key2 Value2 ...]
                    // ....
                    // 示例：
                    // SET key1 "Hello"
                    // MSET key1 "Hello" key2 "World"
                    // GET key1
                    // ...

                    // 注意：
                    // 每个RESP Array数组应该是一条命令，因此多条命令(管道命令)，应该一组一组地生成，例如：
                    // *3\r\n$6\r\nEXPIRE\r\n$4\r\nTime\r\n$2\r\n60\r\n
                    // *3\r\n$3\r\nSET\r\n$4\r\nName\r\n$2\r\nMo\r\n 
                    // *3\r\n$4\r\nMSET\r\n$4\r\nName\r\n$2\r\nMo\r\n 
                    // 下面将多条命令合并在一个数组是错误的：
                    // *9\r\n$6\r\nEXPIRE\r\n$4\r\nTime\r\n$2\r\n60\r\n$3\r\nSET\r\n$4\r\nName\r\n$2\r\nMo\r\n$4\r\nMSET\r\n$4\r\nName\r\n$2\r\nMo\r\n 
                    // 

                    IDictionary<byte[], IDictionary<string, object>> cmdargs = _value as IDictionary<byte[], IDictionary<string, object>>;
                    var ps = new List<object>();
                    foreach (var item in cmdargs)
                    {
                        var cmd = item.Key;
                        var args = item.Value;
                        if (args == null) // 无参数
                        {
                            // 头部，示例：*1\r\n
                            ps.Add(Commands.RESPArraysPrefix); // 数组标志"*"
                            ps.Add(Commands.OneElement); // 数组元素个数
                            ps.Add(Commands.NewLine);

                            // 命令，示例：$4\r\nLLEN\r\n
                            ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                            ps.Add(ObjectConvert.C2Bytes($"{cmd.Length}")); // 命令长度
                            ps.Add(Commands.NewLine);
                            ps.Add(cmd); // 命令
                            ps.Add(Commands.NewLine);
                        }
                        else
                        {
                            // 头部，示例：*2\r\n
                            ps.Add(Commands.RESPArraysPrefix); // 数组标志"*"
                            ps.Add(ObjectConvert.C2Bytes($"{args.Count + 1}")); // 数组元素个数
                            ps.Add(Commands.NewLine);

                            // 命令，示例：$4\r\nLLEN\r\n
                            ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                            ps.Add(ObjectConvert.C2Bytes($"{cmd.Length}")); // 命令长度
                            ps.Add(Commands.NewLine);
                            ps.Add(cmd); // 命令
                            ps.Add(Commands.NewLine);

                            foreach (var arg in args)
                            {
                                // 键，示例：$8\r\nUserName\r\n
                                var key = ObjectConvert.C2Bytes(arg.Key); // 转换成字节数组
                                ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                ps.Add(ObjectConvert.C2Bytes($"{key.Length}")); // 键长度
                                ps.Add(Commands.NewLine);
                                ps.Add(key); // 键
                                ps.Add(Commands.NewLine);

                                // 值，示例：$2\r\nMo\r\n
                                var val = arg.Value;
                                if (val == null) // 空值(null), 示例：$-1\r\n
                                {
                                    ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                    ps.Add(Commands.NullCharLen); // 值长度
                                    ps.Add(Commands.NewLine);
                                }
                                else if (val is string && val.ToString().Length <= 0) // 空字符串(empty string), 示例：$0\r\n\r\n
                                {
                                    ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                    ps.Add(Commands.EmptyCharLen); // 值长度
                                    ps.Add(Commands.NewLine);
                                    ps.Add(Commands.NewLine);
                                }
                                else
                                {
                                    var str = ObjectConvert.C2Bytes(arg.Value); // 转换成字节数组
                                    ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                    ps.Add(ObjectConvert.C2Bytes($"{str.Length}")); // 值长度
                                    ps.Add(Commands.NewLine);
                                    ps.Add(str); // 值
                                    ps.Add(Commands.NewLine);
                                }
                            }
                        }
                    }
                    return ps.ToArray();
                }
                else if (this.Type == CommandType.Batch)
                {
                    //
                    // 批量命令，命令格式：
                    // 命令A Key Value 
                    // 命令A Key Value
                    // ....
                    //
                    // 示例：
                    // SET key1 "Hello"
                    // SET key2 "World"
                    // ...
                    //
                    // 注意：
                    // 每个RESP Array数组应该是一条命令，因此批量命令(管道命令)，应该一组一组地生成，例如：
                    // *3\r\n$6\r\nEXPIRE\r\n$4\r\nname\r\n$2\r\n60\r\n
                    // *3\r\n$6\r\nEXPIRE\r\n$4\r\ntime\r\n$2\r\n60\r\n 
                    // *3\r\n$6\r\nEXPIRE\r\n$5\r\ncount\r\n$2\r\n60\r\n
                    // 下面将多条命令合并在一个数组是错误的：
                    // *9\r\n$6\r\nEXPIRE\r\n$4\r\nname\r\n$2\r\n60\r\n$6\r\nEXPIRE\r\n$4\r\ntime\r\n$2\r\n60\r\n$6\r\nEXPIRE\r\n$5\r\ncount\r\n$2\r\n60\r\n
                    // 

                    IDictionary<string, object> args = _value as IDictionary<string, object>;
                    var ps = new List<object>();
                    if (args == null) // 无参数
                    {
                        // 头部，示例：*1\r\n
                        ps.Add(Commands.RESPArraysPrefix); // 数组标志"*"
                        ps.Add(Commands.OneElement); // 数组元素个数
                        ps.Add(Commands.NewLine);

                        // 命令，示例：$4\r\nLLEN\r\n
                        ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                        ps.Add(ObjectConvert.C2Bytes($"{_cmd.Length}")); // 命令长度
                        ps.Add(Commands.NewLine);
                        ps.Add(_cmd); // 命令
                        ps.Add(Commands.NewLine);
                    }
                    else
                    {
                        foreach (var item in args) 
                        {
                            // 头部，示例：*2\r\n
                            ps.Add(Commands.RESPArraysPrefix); // 数组标志"*"
                            ps.Add(Commands.ThreeElement); // 数组元素个数
                            ps.Add(Commands.NewLine);

                            // 命令，示例：$4\r\nLLEN\r\n
                            ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                            ps.Add(ObjectConvert.C2Bytes($"{_cmd.Length}")); // 命令长度
                            ps.Add(Commands.NewLine);
                            ps.Add(_cmd); // 命令
                            ps.Add(Commands.NewLine);

                            // 键，示例：$8\r\nUserName\r\n
                            var key = ObjectConvert.C2Bytes(item.Key); // 转换成字节数组
                            ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                            ps.Add(ObjectConvert.C2Bytes($"{key.Length}")); // 键长度
                            ps.Add(Commands.NewLine);
                            ps.Add(key); // 键
                            ps.Add(Commands.NewLine);

                            // 值，示例：$2\r\nMo\r\n
                            if (item.Value == null) // 空值(null), 示例：$-1\r\n
                            {
                                ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                ps.Add(Commands.NullCharLen); // 值长度
                                ps.Add(Commands.NewLine);
                            }
                            else if (item.Value is string && item.Value.ToString().Length <= 0) // 空字符串(empty string), 示例：$0\r\n\r\n
                            {
                                ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                ps.Add(Commands.EmptyCharLen); // 值长度
                                ps.Add(Commands.NewLine);
                                ps.Add(Commands.NewLine);
                            }
                            else
                            {
                                var val = ObjectConvert.C2Bytes(item.Value); // 转换成字节数组
                                ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                ps.Add(ObjectConvert.C2Bytes($"{val.Length}")); // 值长度
                                ps.Add(Commands.NewLine);
                                ps.Add(val); // 值
                                ps.Add(Commands.NewLine);
                            }
                        }
                    }
                    return ps.ToArray();

                }
                else
                {
                    //
                    // 单条命令，命令格式：
                    // 命令 Key Value [Key1 Value1 Key2 Value2 ...]
                    // 示例：
                    // SET key1 "Hello"
                    // MSET key1 "Hello" key2 "World" key3 "China"
                    //
                    // 一个RESP Array数组应该是一条命令，例如：
                    // *3\r\n$6\r\nEXPIRE\r\n$4\r\nTime\r\n$2\r\n60\r\n
                    // *3\r\n$3\r\nSET\r\n$4\r\nName\r\n$2\r\nMo\r\n 
                    // 下面将多条命令合并在一个数组是错误的：
                    // *6\r\n$6\r\nEXPIRE\r\n$4\r\nTime\r\n$2\r\n60\r\n$3\r\nSET\r\n$4\r\nName\r\n$2\r\nMo\r\n 
                    // 

                    object[] args = _value as object[];
                    var ps = new List<object>();
                    if (args == null) // 无参数
                    {
                        // 头部，示例：*1\r\n
                        ps.Add(Commands.RESPArraysPrefix); // 数组标志"*"
                        ps.Add(ObjectConvert.C2Bytes("1")); // 数组元素个数
                        ps.Add(Commands.NewLine);

                        // 命令，示例：$4\r\nQUIT\r\n
                        ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                        ps.Add(ObjectConvert.C2Bytes($"{_cmd.Length}")); // 命令长度
                        ps.Add(Commands.NewLine);
                        ps.Add(_cmd); // 命令
                        ps.Add(Commands.NewLine);
                    }
                    else
                    {
                        // 头部，示例：*2\r\n
                        ps.Add(Commands.RESPArraysPrefix); // 数组标志"*"
                        ps.Add(ObjectConvert.C2Bytes($"{args.Length + 1}")); // 数组元素个数
                        ps.Add(Commands.NewLine);

                        // 命令，示例：$4\r\nLLEN\r\n
                        ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                        ps.Add(ObjectConvert.C2Bytes($"{_cmd.Length}")); // 命令长度
                        ps.Add(Commands.NewLine);
                        ps.Add(_cmd); // 命令
                        ps.Add(Commands.NewLine);

                        foreach (var item in args)
                        {
                            // 键/值，示例：$8\r\nUserName\r\n
                            if (item == null) // 空值(null), 示例：$-1\r\n
                            {
                                ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                ps.Add(Commands.NullCharLen); // 值长度
                                ps.Add(Commands.NewLine);
                            }
                            else if (item is string && item.ToString().Length <= 0) // 空字符串(empty string), 示例：$0\r\n\r\n
                            {
                                ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                ps.Add(Commands.EmptyCharLen); // 值长度
                                ps.Add(Commands.NewLine);
                                ps.Add(Commands.NewLine);
                            }
                            else
                            {
                                var val = ObjectConvert.C2Bytes(item); // 转换成字节数组
                                ps.Add(Commands.RESPBulkStringsPrefix); // 字符串标志"$"
                                ps.Add(ObjectConvert.C2Bytes($"{val.Length}")); // 值长度
                                ps.Add(Commands.NewLine);
                                ps.Add(val); // 值
                                ps.Add(Commands.NewLine);
                            }
                        }
                    }
                    return ps.ToArray();
                }
            }
        }

        /// <summary>
        /// 命令文本
        /// </summary>
        public string Text
        {
            get 
            {
                StringBuilder sb = new StringBuilder();
                var value = this.Value;
                foreach(var item in value)
                {
                    byte[] str = item as byte[];
                    sb.Append(ObjectConvert.C2Str(str));
                }
                return sb.ToString(); 
            }
        }

        /// <summary>
        /// 单条命令，命令格式：
        /// 命令 Key Value [Key1 Value1 Key2 Value2 ...]
        /// 示例：
        /// SET key1 "Hello"
        /// MSET key1 "Hello" key2 "World" key3 "China"
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">Key,Value</param>
        public RedisCommand(byte[] cmd, params object[] args)
        {
            _type = CommandType.One;
            _cmd = cmd;
            _value = args;
        }

        /// <summary>
        /// 批量命令，命令格式：
        /// 命令A Key Value 
        /// 命令A Key Value
        /// ....
        /// 示例：
        /// SET key1 "Hello"
        /// SET key2 "World"
        /// ...
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">Key-Value</param>
        /// <param name="_batch">批量标志，仅占位用，防止new RedisCommand(Commands.Ping, null)这类构造函数报错</param>
        public RedisCommand(byte[] cmd, IDictionary<string, object> args, bool _batch = true)
        {
            _type = CommandType.Batch;
            _cmd = cmd;
            _value = args;
        }

        /// <summary>
        /// 多条命令，命令格式：
        /// 命令A Key Value [Key1 Value1 Key2 Value2 ...]
        /// 命令B Key Value [Key1 Value1 Key2 Value2 ...]
        /// 命令C Key Value [Key1 Value1 Key2 Value2 ...]
        /// ....
        /// 示例：
        /// SET key1 "Hello"
        /// MSET key1 "Hello" key2 "World"
        /// GET key1
        /// ...
        /// </summary>
        /// <param name="cmdargs">命令-Key-Value</param>
        public RedisCommand(IDictionary<byte[], IDictionary<string, object>> cmdargs)
        {
            _type = CommandType.Multiple;
            _value = cmdargs;
        }

    }

}