using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastCore.Json;

namespace FastCore.Cache
{
    /// <summary>
    /// 缓存清理策略
    /// </summary>
    public enum CacheClearPolicy
    {
        /// <summary>
        /// 清理所有可能的缓存项目
        /// </summary>
        ClearAll,
        /// <summary>
        /// 仅清理已经过期的缓存项目
        /// </summary>
        OnlyExpired
    }

    /// <summary>
    /// 高速内存缓存
    /// </summary>
    public class MemoryCache: IDisposable
    {
        #region MemoryCache结构

        // 
        // |        |
        // |--------|<- 最大缓存数量(无限制，因为每隔一定时间会清理过期缓存，不用担心内存消耗过快)
        // |        |
        // |        |
        // |########|
        // |########|
        // |########|<- 清理容量(Capacity)，默认为0表示每次都要清理整个缓存中的过期项目，>0表示每次找出指定容量的过期缓存进行清理(能减少扫描整个缓存的频率)。
        // |########|   默认60秒清理一次。
        // |########|
        // |########|
        // |########|
        // |--------|
        //

        #endregion

        #region 成员变量

        /// <summary>并行缓存字典，适用于多线程高并发</summary>
        private ConcurrentDictionary<string, CacheItem> _cache;

        /// <summary>是否启用Json序列化</summary>
        private bool _jsonSerialize = false;

        /// <summary>是否自动清理缓存</summary>
        private bool _autoClearCache = true;

        /// <summary>清理间隔时间，以秒为单位，默认60秒清理一次</summary>
        private int _clearInterval = 60;

        /// <summary>清理容量，默认为0表示每次都要清理整个缓存中的过期项目</summary>
        private int _clearCapacity = 0;

        /// <summary>取消清理令牌</summary>
        private CancellationTokenSource _cancelToken = null; 

        /// <summary>缓存项目数量</summary>
        private int _count;

        #endregion

        #region 构造与析构

        /// <summary>
        /// 实例化缓存
        /// </summary>
        /// <param name="autoClearCache">是否启用自动清理缓存，默认开启</param>
        /// <param name="clearInterval">清理间隔时间，以秒为单位，默认60秒清理一次</param>
        /// <param name="clearCapacity">清理容量，默认为0表示每次都要清理整个缓存中的过期项目</param>
        /// <param name="jsonSerialize">是否进行Json序列化</param>
        public MemoryCache(bool autoClearCache = true, int clearInterval = 60, int clearCapacity = 0, bool jsonSerialize = false)
        {
            // 创建缓存
            _cache = new ConcurrentDictionary<string, CacheItem>();

            // 创建清理任务
            _autoClearCache = autoClearCache;
            _clearInterval = clearInterval <= 0 ? 60 : clearInterval;
            _clearCapacity = clearCapacity < 0 ? 0 : clearCapacity;
            _jsonSerialize = jsonSerialize;
            if (_autoClearCache)
            {
                this.CreateClearTask();
            }
        }

        /// <summary>
        /// 卸载资源。
        /// 这个析构函数只有在Dispose方法没有被调用时才会运行。
        /// </summary>
        ~MemoryCache()
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
        /// 在两个不同的场景中执行Dispose(bool Dispose)。
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
                    // 关闭清理任务
                    if (_autoClearCache && _cancelToken != null)
                    {
                        _cancelToken.Cancel();
                        Task.WaitAll();
                        _cancelToken.Dispose();
                        _cancelToken = null;
                    }
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

        /// <summary>
        /// 缓存名称
        /// </summary>
        public string Name { get; set; } = "Memory";

        /// <summary>
        /// 缓存清理策略
        /// </summary>
        public CacheClearPolicy Policy { get; set; } = CacheClearPolicy.ClearAll;

        /// <summary>清理间隔，以秒为单位，默认60秒，表示每隔60秒清理一次缓存</summary>
        public int Interval 
        { 
            get { return _clearInterval; }
            set { _clearInterval = value <= 0 ? 60 : value; }
        }

        /// <summary>
        /// 清理容量，默认为0表示每次都要清理整个缓存中的过期项目，>0表示每次找出指定容量的过期缓存进行清理(能减少扫描整个缓存的频率)。
        /// </summary>
        public int Capacity
        { 
            get { return _clearCapacity; }
            set { _clearCapacity = value < 0 ? 0 : value; }
        }

        /// <summary>返回缓存项目数量</summary>
        public int Count 
        { 
            get { return _count; } 
        }

        /// <summary>返回所有键。实际返回所有Key的集合，数据量较大时注意性能</summary>
        public ICollection<string> Keys 
        { 
            get { return _cache.Keys; } 
        }

        /// <summary>
        /// 是否启用Json序列化，默认开启。
        /// 当启用Json序列化时对性能会有所损耗，经过测试对性能影响不是很大。
        /// 开启Json序列化用来缓存对象特别有用，由于序列化会把对象转换成字符串存储，因此即使被缓存对象被销毁了也能找回。
        /// </summary>
        public bool JsonSerialize
        {
            // 通过对比测试System.Text.Json和Newtonsoft.Json，发现：
            // System.Text.Json只能序列化属性，不能序列化字段。
            // System.Text.Json比Newtonsoft.Json速度快，但占用的内存高。
            // System.Text.Json没有Newtonsoft.Json稳定。
            // 最后，在性能影响不大的情况下，决定选择Newtonsoft.Json作为序列化组件。
            get { return _jsonSerialize; }
            set { _jsonSerialize = value; }
        }

        #endregion

        #region 方法

        /// <summary>是否包含缓存项</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _cache.ContainsKey(key);
        }

        /// <summary>设置缓存项目</summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="ttl">项目的存活时间，以秒为单位。默认-1表示永不过期。</param>
        /// <param name="replace">当项目已经存在时是否替换。如果设置成false，则已存在时不替换，此时可用于锁争夺</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, int ttl = -1, bool replace = true)
        {
            // 调整存活时间
            if (ttl < 0) 
                ttl = -1;

            // 尝试增加缓存项目，直到成功为止
            CacheItem ci = null;
            do
            {
                // 缓存项目已存在, 已存在则修改
                if (_cache.TryGetValue(key, out var item) || item != null) 
                {
                    if (replace)
                    {
                        if(_jsonSerialize) // 启用了Json序列化
                        {
                            var json = Json.Json.Serialize(value);
                            item.Set(json, ttl); 
                        }
                        else
                        {
                            item.Set(value, ttl); 
                        }
                    }
                    return replace;
                }

                // 创建缓存项目
                if (ci == null)
                {
                    if (_jsonSerialize) // 启用了Json序列化
                    {
                        var json = Json.Json.Serialize(value);
                        ci = new CacheItem(json, ttl);
                    }
                    else
                    {
                        ci = new CacheItem(value, ttl);
                    }
                } 
            } while (!_cache.TryAdd(key, ci)); 

            // 计数
            Interlocked.Increment(ref _count);

            return true;
        }

        /// <summary>修改缓存项目</summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Update<T>(string key, T value)
        {
            if (_cache.TryGetValue(key, out var item) || item != null)
            {
                int ttl = item.TTL; // 继承存活时间
                if (_jsonSerialize) // 启用了Json序列化
                {
                    var json = Json.Json.Serialize(value);
                    item.Set(json, ttl);
                }
                else
                {
                    item.Set(value, ttl);
                }
                return true;
            }
            return false;
        }

        /// <summary>删除缓存项目，支持批量删除</summary>
        /// <param name="keys">键集合</param>
        /// <returns>实际删除的缓存项目个数</returns>
        public int Remove(params string[] keys)
        {
            var count = 0;
            foreach (var key in keys)
            {
                if (_cache.TryRemove(key, out var item))
                {
                    count++;
                    Interlocked.Decrement(ref _count);
                }
            }
            return count;
        }

        /// <summary>获取缓存项，不存在时返回默认值</summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            // 尝试获得值
            if (_cache.TryGetValue(key, out var item) || item != null)
            {
                item.Alive = DateTime.Now.Ticks; // 设置活跃时间
                if (_jsonSerialize) // 启用了Json序列化
                {
                    return Json.Json.Deserialize<T>(item.Value.ToString()); // 尝试转换回指定类型
                }
                else
                {
                    return TypeConvert.C2Type<T>(item.Value); // 尝试转换回指定类型
                }
            }
            return default;
        }

        /// <summary>
        /// 清空所有缓存项目
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
            _count = 0;
        }

        /// <summary>设置缓存项目的存活时间</summary>
        /// <param name="key">键</param>
        /// <param name="ttl">存活时间(Time to live)，以秒为单位，-1表示永不过期</param>
        /// <returns>设置是否成功</returns>
        public bool SetTTL(string key, int ttl)
        {
            if (_cache.TryGetValue(key, out var item) || item != null)
            {
                item.Set(ttl);
                return true;
            }
            return false;
        }

        /// <summary>获取缓存项目剩余的存活时间(Time to live)，以秒为单位，-1表示永不过期</summary>
        /// <param name="key">键</param>
        /// <returns>缓存项目不存在或已经过期时返回0</returns>
        public int GetTTL(string key)
        {
            if (_cache.TryGetValue(key, out var item) || item != null)
            {
                int ttl = (int)(item.Expire - DateTime.Now.Ticks); // 如果<=0说明已经过期
                return ttl <= 0 ? 0 : ttl;
            }
            return 0;
        }

        #endregion

        #region 清理缓存

        /// <summary>创建清理缓存任务</summary>
        private void CreateClearTask()
        {
            // 初始化取消令牌
            _cancelToken = new CancellationTokenSource();

            // 创建任务
            Task t = new Task(delegate
            {
                var policy = this.Policy;
                int interval = this.Interval;
                int i = 0;
                while(_cancelToken != null)
                {
                    if (_cancelToken.IsCancellationRequested == true)
                    {
                        break; // 退出清理任务
                    }
                    else
                    {
                        if (i >= interval) // 到达清理间隔
                        {
                            ClearCacheItem(policy); // 清理
                            i = 0;
                        }
                    }
                    i += 1;
                    Thread.Sleep(1000);
                }
            });

            // 启动任务
            t.Start(); 
        }

        /// <summary>
        /// 清理缓存项目。
        /// </summary>
        private void ClearCacheItem(CacheClearPolicy policy)
        {
            // 缓存中无项目不必清理
            var dic = _cache;
            if (dic.Count == 0)
            {
                return;
            }

            // 根据不同策略清理
            int total = 0;
            if (policy == CacheClearPolicy.OnlyExpired) // 只清理已过期的缓存项目
            {
                // 统计已过期的缓存项目
                int i = 0;
                int celarCount = Capacity > 0 ? Capacity : _count; // 当清理数量<=0则默认清理所有过期缓存项目；当清理数量>0时则清理指定数量的过期缓存。
                long now = DateTime.Now.Ticks;
                var list = new List<string>();

                foreach (var item in dic)
                {
                    // 已经超过清理数量则结束统计
                    if (i >= celarCount)
                    {
                        break;
                    }

                    // 缓存已过期项目到列表
                    var value = item.Value;
                    if (value != null)
                    {
                        var ttl = value.TTL;
                        var expire = value.Expire;
                        if (ttl != -1 && expire <= now) // 已过期
                        {
                            list.Add(item.Key);
                            i += 1;
                        }
                    }
                }

                // 清理过期的缓存项目
                if (list.Count > 0)
                {
                    Log.Info($"清理过期的缓存项目...");

                    int c = 0;
                    foreach (var item in list)
                    {
                        if (_cache.TryRemove(item, out _)) // 下划线表示丢弃输出变量
                        {
                            c += 1;
                        }
                    }
                    total += c;
                    _count -= c;

                    Log.Info($"过期的缓存项目清理完成, 本次清理数量: {c}");
                }
            }
            else
            {
                // 统计已过期和即将过期的缓存项目
                int i = 0;
                int celarCount = Capacity > 0 ? Capacity : _count; // 当清理数量<=0则默认清理所有过期缓存项目；当清理数量>0时则清理指定数量的过期缓存。
                long now = DateTime.Now.Ticks;
                long exp = DateTime.Now.Ticks + 36_000_000_000L; // 一定时间之内(默认1个小时内)即将过期的缓存项目
                var list = new List<string>();
                var sdic = new SortedDictionary<long, string>();

                foreach (var item in dic)
                {
                    // 已经超过清理数量就结束统计
                    if (i >= celarCount)
                    {
                        break;
                    }

                    // 缓存已过期项目到列表
                    var value = item.Value;
                    if (value != null)
                    {
                        var ttl = value.TTL;
                        if (ttl != -1) // 设置了存活时间
                        {
                            var expire = value.Expire;
                            if (expire <= now) // 已过期
                            {
                                list.Add(item.Key);
                                i += 1;
                            }
                            else
                            {
                                if (ttl > 3600 && expire < exp) // 即将过期(存活时间超过1小时的才需要处理)
                                {
                                    var alive = value.Alive;
                                    if (!sdic.ContainsKey(alive)) // 由于采用高精度计时，alive不可能重复。
                                    {
                                        sdic.Add(alive, item.Key);
                                        i += 1;
                                    }
                                }
                            }
                        }
                    }
                }

                // 清理过期的缓存项目
                if (list.Count > 0)
                {
                    Log.Info($"开始清理过期的缓存项目...");
                    int c = 0;
                    foreach (var item in list)
                    {
                        if (_cache.TryRemove(item, out _)) // 下划线表示丢弃输出变量
                        {
                            c += 1;
                        }
                    }
                    total += c;
                    _count -= c;
                    Log.Info($"过期的缓存项目清理完成, 本次清理数量: {c}");
                }

                // 清理即将过期的缓存项目，采用LRU(最近最少使用)原则清除
                if (sdic.Count > 0)
                {
                    Log.Info($"开始清理即将过期的缓存项目...");
                    int c = 0;
                    foreach (var item in sdic)
                    {
                        if (_cache.TryRemove(item.Value, out _)) // 下划线表示丢弃输出变量
                        {
                            c += 1;
                        }
                    }
                    total += c;
                    _count -= c;
                    Log.Info($"即将过期的缓存项目清理完成, 本次清理数量: {c}");
                }

                // 清理数据
                list.Clear();
                sdic.Clear();
            }

            // 强制一次垃圾回收，清理内存
            if (total > 0)
            {
                GC.Collect();
            }
            
            Log.Info($"缓存项目清理完成, 总计清理: {total}, 剩余缓存项目数量: {_count}");
        }

        #endregion

        #region 缓存项目

        /// <summary>
        /// 缓存项目
        /// </summary>
        private class CacheItem
        {
            /// <summary>
            /// 项目值
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// 存活时间(Time To Live)，以秒为单位，-1表示永不过期
            /// </summary>
            public int TTL { get; set; }

            /// <summary>
            /// 到期时间(Expire Time)，以纳秒为单位
            /// </summary>
            public long Expire { get; set; }

            /// <summary>
            /// 活跃时间(Alive Time)，以纳秒为单位
            /// </summary>
            public long Alive { get; set; }

            /// <summary>构造缓存项目</summary>
            /// <param name="value"></param>
            /// <param name="ttl"></param>
            public CacheItem(object value, int ttl)
            {
                // 1秒 = 1000毫秒，1毫秒 = 1000微妙，1微秒 = 1000纳秒。因此：
                // 1秒 = 1000000000纳秒，1纳秒 = 0.000000001秒
                // 1毫秒 = 1000000纳秒，1纳秒 = 0.000001毫秒
                // 
                // DateTime.Ticks：表示0001年1月1日午夜12:00:00以来所经历的100纳秒数。因此：
                // 1秒 = 1000000000/100 = 10000000纳秒，1纳秒 = 0.0000001秒
                // 1毫秒 = 1000000/100 = 10000纳秒，1纳秒 = 0.0001毫秒

                long elapsed = DateTime.Now.Ticks;
                Value = value;
                TTL = ttl < 0 ? -1 : ttl;
                Expire = elapsed + ttl * 10_000_000L; // 转换成纳秒
                Alive = elapsed;
            }

            /// <summary>设置项目的值和存活时间</summary>
            /// <param name="value"></param>
            /// <param name="ttl"></param>
            public void Set(object value, int ttl)
            {
                long elapsed = DateTime.Now.Ticks;
                Value = value;
                TTL = ttl < 0 ? -1 : ttl;
                Expire = elapsed + ttl * 10_000_000L;
                Alive = elapsed;
            }

            /// <summary>设置项目的存活时间</summary>
            /// <param name="ttl"></param>
            public void Set(int ttl)
            {
                long elapsed = DateTime.Now.Ticks;
                TTL = ttl < 0 ? -1 : ttl;
                Expire = elapsed + ttl * 10_000_000L;
                Alive = elapsed;
            }

        }

        #endregion
    }
}
