using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FastCore.Cache;

namespace FastCore.Redis
{
    /// <summary>
    /// Redis客户端。
    /// 各种Redis命令的实现。
    /// 当设置好命令以后，系统会从连接池中取出一个RedisConnection对象将命令发送给服务器，并从服务器接收数据。
    /// </summary>
    public class RedisClient : IDisposable
    {
        #region 成员变量

        private RedisOption _option; // 选项配置

        #endregion

        #region 构造与析构

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="option">配置</param>
        public RedisClient(RedisOption option)
        {
            _option = new RedisOption()
            {
                Server = ObjectCheck.IsNullOrEmpty(option.Server) ? "127.0.0.1" : option.Server,
                Port = option.Port <= 0 ? 6379 : option.Port,
                Password = option.Password,
                Db = option.Db < 0 ? 0 : option.Db,
                ConnectTimeout = option.ConnectTimeout,
                SendTimeout = option.SendTimeout,
                ReceiveTimeout = option.ReceiveTimeout,
                RetryCount = option.RetryCount
            };
        }

        /// <summary>
        /// 卸载资源。
        /// 这个析构函数只有在Dispose方法没有被调用时才会运行。
        /// </summary>
        ~RedisClient()
        {
            // 不要在这里创建销毁(Dispose)代码清理资源。
            // 调用Dispose(false)在可读性和可维护性方面是最优的。
            Dispose(false);
        }

        /// <summary>
        /// 销毁。不要使此方法为虚方法。不应让派生类能重写此方法。
        /// </summary>
        public void Dispose()
        {
            // 不要在这里创建销毁(Dispose)代码清理资源。
            // 调用Dispose(true)在可读性和可维护性方面是最优的。
            Dispose(true);

            // 从终止队列(Finalization queue)中退出，
            // 防止此对象的终止(finalization)代码第二次执行。
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 在两个不同的场景中执行Dispose(bool Dispose)：
        /// 如果dispose = true，则该方法已被用户代码直接或间接调用。可以释放托管和非托管资源(Managed and unmanaged resources)。
        /// 如果dispose = false，则运行时已从终结器(finalizer,即析构函数)内部调用该方法，您不应再引用其他对象，只能释放非托管资源。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // 如果disposing=true，在这里销毁所有托管和非托管资源(managed and unmanaged resources)。
                if (disposing)
                {
                    Cache.Dispose();
                }

                // 如果disposing=false，只能释放非托管资源(unmanaged resources)。在这里添加代码释放非托管资源。例如：
                // CloseHandle(handle);
                // handle = IntPtr.Zero;
            }

            // 注意，这不是线程安全的。
            // 在释放标志disposed设置为true之前, 另一个线程可以在托管资源被释放后开始释放对象。
            // 如果想要线程安全(thread safe)，则必须由客户端(client)实现并保证线程安全。
            disposed = true;
        }
        private bool disposed = false;

        #endregion

        #region 属性

        /// <summary>是否已登录</summary>
        public bool IsLogined { get; private set; }

        /// <summary>登录时间</summary>
        public string LoginTime { get; private set; }

        #endregion

        #region Redis命令

        /// <summary>
        /// 关闭客户端
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>Ping服务器</summary>
        /// <returns></returns>
        public bool Ping()
        {
            IList<object> result = this.Execute(new RedisCommand(Commands.Ping, null));
            return TryConvertType<bool>(result);
        }

        /// <summary>切断服务器连接</summary>
        /// <returns></returns>
        public bool Quit()
        {
            IList<object> result = this.Execute(new RedisCommand(Commands.Quit, null));
            if (TryConvertType<bool>(result))
            {
                IsLogined = false;
                LoginTime = "";
                return true;
            }
            return false;
        }

        /// <summary>切换Db</summary>
        /// <param name="db">数据库，Redis默认内置0-15个数据库</param>
        /// <returns></returns>
        public void Select(int db)
        {
            db = db < 0 ? 0 : db;
            IList<object> result = this.Execute(new RedisCommand(Commands.Select, db));
            if (TryConvertType<bool>(result))
            {
                _option.Db = db;
            }
        }

        /// <summary>设置单个Key</summary>
        /// <typeparam name="T">设置的类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="ttl">存活时间，秒。-1表示永不过期</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, int ttl = -1)
        {
            IList<object> result = null;
             
            ttl = ttl <= 0 ? -1 : ttl;
            if (ttl <= 0)
            {
                result = this.Execute(new RedisCommand(Commands.Set, key, value));
            }
            else
            {
                result = this.Execute(new RedisCommand(Commands.SetEx, key, ttl, value));
            }
            return TryConvertType<bool>(result);
        }

        /// <summary>设置多个Key</summary>
        /// <typeparam name="T">设置的类型</typeparam>
        /// <param name="values">字典</param>
        /// <param name="ttl">存活时间，秒。-1表示永不过期</param>
        /// <returns></returns>
        public bool Set<T>(IDictionary<string, T> values, int ttl = -1)
        {
            if (values == null)
                return false;
            if (ttl <= 0)
                ttl = -1;

            // 将字典转换成列表
            var ps = new List<object>();
            foreach (var item in values)
            {
                ps.Add(item.Key);
                ps.Add(item.Value);
            }
            IList<object> result = this.Execute(new RedisCommand(Commands.MSet, ps.ToArray()));
            if (TryConvertType<bool>(result))
            {
                // MSet命令不支持设置Key的存活时间(ttl)，因此采用Expire命令进行批量设置
                var pe = new Dictionary<string, object>();
                foreach (var item in values)
                {
                    pe.Add(item.Key, ttl);
                }
                result = this.Execute(new RedisCommand(Commands.Expire, pe));
                return TryConvertType<bool>(result);
            }
            
            return false;
        }

        /// <summary>
        /// 根据Key获取对象
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>返回T的实例</returns>
        public T Get<T>(string key)
        {
            IList<object> result = this.Execute(new RedisCommand(Commands.Get, key));
            return TryConvertType<T>(result);
        }

        /// <summary>
        /// 根据Key批量获取对象
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="keys">键</param>
        /// <returns>返回T的实例字典</returns>
        public IDictionary<string, T> Get<T>(IEnumerable<string> keys)
        { 
            var ks = keys.ToArray();
            IList<object> result = this.Execute(new RedisCommand(Commands.MGet, ks));
            var rs = result.FirstOrDefault() as object[];

            var dic = new Dictionary<string, T>();
            for (int i = 0; i < ks.Length && i < rs.Length; i++)
            {
                dic[ks[i]] = TryConvertType<T>(rs[i]);
            }
            return dic;
        }

        #endregion

        #region Redis命令执行

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private IList<object> Execute(RedisCommand cmd)
        {
            var i = 0;
            do
            {
                var conn = Cache.Get(); // 从缓存中取出连接
                try
                {
                    // 初始化连接
                    conn.Initialize(_option);

                    // 连接到数据库
                    if (!IsLogined)
                    {
                        // 关闭连接
                        conn.Close();

                        // 验证密码
                        if (!ObjectCheck.IsNullOrEmpty(_option.Password))
                        {
                            var val = conn.Execute(new RedisCommand(Commands.Auth, _option.Password));
                        }

                        // 选择数据库
                        if (_option.Db > 0)
                        {
                            var val = conn.Execute(new RedisCommand(Commands.Select, _option.Db)); 
                        }

                        // 登录成功
                        IsLogined = true;
                        LoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }

                    // 执行命令
                    return conn.Execute(cmd); 
                }
                catch (RedisServerException e)
                {
                    // 如果由RedisServerException引发异常，说明成功连接到了服务器
                    if (e.Message.IndexOf("AUTH", StringComparison.InvariantCulture) < 0)
                    {
                        // 如果服务器的应答不是登录验证问题，说明客户端操作出现了问题，此时不用再重试
                        throw new RedisServerException(e.Message); 
                    }
                    else
                    {
                        // 如果服务器的应答是登录验证问题，则此时必须变更登录状态，重新进行登录验证
                        IsLogined = false;
                        LoginTime = "";
                        if (i++ >= _option.RetryCount) // 超过重试次数
                        {
                            throw new RedisClientException(e, MethodBase.GetCurrentMethod());
                        }
                    }
                }
                catch (RedisClientException e)
                {
                    // 如果网络存已经关闭，则必须重新登录
                    if (conn != null && !conn.IsConnected()) // 连接已断开必须重新登录
                    {
                        IsLogined = false;
                        LoginTime = "";
                    }

                    // 如果服务器的应答要求登录验证，则必须重新登录
                    if (e.Message.IndexOf("NOAUTH", StringComparison.InvariantCulture) >= 0)
                    {
                        IsLogined = false;
                        LoginTime = "";
                    }

                    // 超过重试次数
                    if (i++ >= _option.RetryCount) 
                    {
                        throw new RedisClientException(e, MethodBase.GetCurrentMethod());
                    }
                }
                catch (Exception e)
                {
                    // 如果由系统引发异常，不能识别
                    throw new RedisClientException(e, MethodBase.GetCurrentMethod());
                }
                finally
                {
                    Cache.Put(conn); // 回收连接
                }
            } while (true);
        }

        /// <summary>
        /// 尝试将结果转换成目标类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        private T TryConvertType<T>(object result)
        {
            // 取第一项
            object value;
            if (result is IList<object> rs)
            {
                value = rs.FirstOrDefault();
            }
            else
            {
                value = result;
            }
            
            // 转换字符串
            if (value is string str)
            {
                try
                {
                    switch (str)
                    {
                        case "OK": return (T)Commands.Ok;
                        case "PONG": return (T)Commands.Ok;
                    }
                    return TypeConvert.C2Type<T>(str);
                }
                catch (Exception e)
                {
                    throw new RedisClientException($"将字符串({value})转为类型({typeof(T).Name})失败({e.Message})", MethodBase.GetCurrentMethod());
                }
            }

            // 转换字节
            if (value is byte[] byts)
            {
                try
                {
                    object retVal; 
                    TypeCode typeCode = Type.GetTypeCode(typeof(T));
                    switch (typeCode)
                    {
                        case TypeCode.Object: retVal = ObjectConvert.C2Obj<T>(byts); break;
                        case TypeCode.String: retVal = ObjectConvert.C2Str(byts); break;
                        case TypeCode.DateTime: retVal = ObjectConvert.C2Date(byts); break;
                        case TypeCode.Decimal: retVal = ObjectConvert.C2Dec(byts); break;
                        case TypeCode.Double: retVal = ObjectConvert.C2Dbl(byts); break;
                        case TypeCode.Single: retVal = ObjectConvert.C2Flt(byts); break;
                        case TypeCode.Int64: retVal = ObjectConvert.C2Lng(byts); break;
                        case TypeCode.Int32: retVal = ObjectConvert.C2Int(byts); break;
                        case TypeCode.Int16: retVal = ObjectConvert.C2Short(byts); break;
                        case TypeCode.UInt64: retVal = ObjectConvert.C2Lng(byts); break;
                        case TypeCode.UInt32: retVal = ObjectConvert.C2Int(byts); break;
                        case TypeCode.UInt16: retVal = ObjectConvert.C2Short(byts); break;
                        case TypeCode.Byte: retVal = ObjectConvert.C2Byte(byts); break;
                        case TypeCode.SByte: retVal = ObjectConvert.C2SByte(byts); break;
                        case TypeCode.Char: retVal = ObjectConvert.C2Chr(byts); break;
                        case TypeCode.Boolean: retVal = ObjectConvert.C2Bool(byts); break;
                        default: retVal = TypeConvert.C2Type<T>(byts); break;
                    }
                    return (T)retVal;
                }
                catch (Exception e)
                {
                    throw new RedisClientException($"将结果转换成类型({typeof(T).Name})失败({e.Message})", MethodBase.GetCurrentMethod());
                }
            }

            return default;
        }

        #endregion

        #region Redis连接池

        /// <summary>
        /// Redis连接缓存
        /// </summary>
        private class RedisConnCache : ObjectCache<RedisConnection> { }
        
        /// <summary>
        /// 取得Redis实例对象
        /// </summary>
        private RedisConnCache instance = null; // 确保多线程模式下总能读取到正确值
        private RedisConnCache Cache
        {
            get
            {
                // 使用双检锁模式，保证多线程应用模式下只存在唯一实例。
                if (instance == null) // 保证了性能
                {
                    lock (this) // 保证了线程安全
                    {
                        if (instance == null) // 保证了只有一个实例被创建
                        {
                            instance = new RedisConnCache()
                            {
                                MaxCount = 1000 // 默认最大连接数1000
                            };
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

    }
}
