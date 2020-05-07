using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FastCore.Cache
{
    /// <summary>
    /// 对象缓存，用于缓存数据库连接、网络连接等。
    /// </summary>
    /// <remarks>
    /// 只能缓存实现了IDisposable接口的对象，以保证缓存池销毁的时候能成功销毁被缓存的对象。
    /// </remarks>
    public class ObjectCache<T>: IDisposable where T : class, IDisposable
    {
        #region ObjectCache结构

        // 
        // |        |
        // |--------|<- 最大缓存数量(最高水位线)(MaxCount)
        // |        |
        // |        |
        // |########|
        // |########|
        // |########|<- 空闲缓存项目(FreeItems)，根据负荷自动增减，到了一定时间后自动清理，保证不占用过多的内存。
        // |########|
        // |########|
        // |--------|<- 最小可缓存对象数量(最低水位线)(MinCount)
        // |########|
        // |########|<- 基础缓存项目(BaseItems)，保证总是有缓存项目可用
        // |########|
        // |--------|
        //

        #endregion

        #region 成员变量

        /// <summary>错误消息</summary>
        private string _errMsg;

        /// <summary>空闲对象数量(最低水位线以上)</summary>
        private int _freeCount;

        /// <summary>繁忙对象数量</summary>
        private int _busyCount;

        /// <summary>最大可缓存对象数量(最高水位线)。默认163，0表示无上限</summary>
        private int _maxCount;

        /// <summary>最小可缓存对象数量(最低水位线)。默认3</summary>
        private int _minCount;

        /// <summary>缓存清理间隔，以秒为单位。最低水位线之上的资源超过空闲时间时被自动清理，默认30s, 0表示永不清理</summary>
        private int _Interval;

        /// <summary>取消清理令牌</summary>
        private CancellationTokenSource _cancelToken = null;

        /// <summary>基础缓存项目(最常使用的项目用堆栈缓存, 后进先出)</summary>
        private ConcurrentStack<CacheItem> _baseItems;

        /// <summary>空闲缓存项目(空闲使用的项目用队列缓存, 先进先出)</summary>
        private ConcurrentQueue<CacheItem> _freeItems;

        /// <summary>繁忙缓存项目</summary>
        private ConcurrentDictionary<T, CacheItem> _busyItems;
      
        #endregion

        #region 构造与析构

        /// <summary>
        /// 实例化
        /// </summary>
        public ObjectCache()
        {
            _freeCount = 0;
            _busyCount = 0;
            _maxCount = 163;
            _minCount = 3;
            _Interval = 30;
            _baseItems = new ConcurrentStack<CacheItem>();
            _freeItems = new ConcurrentQueue<CacheItem>();
            _busyItems = new ConcurrentDictionary<T, CacheItem>();
            this.CreateClearTask(); // 创建清理任务
        }

        /// <summary>
        /// 卸载资源。
        /// 这个析构函数只有在Dispose方法没有被调用时才会运行。
        /// </summary>
        ~ObjectCache()
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
                    if (_cancelToken != null)
                    {
                        _cancelToken.Cancel();
                        Task.WaitAll();
                        _cancelToken.Dispose();
                        _cancelToken = null;
                    }

                    // 清理基础缓存项目
                    if (_baseItems != null)
                    {
                        lock (_baseItems)
                        {
                            while (_baseItems.TryPop(out var ci))
                            {
                                if (ci.Value is IDisposable obj)
                                {
                                    try { obj.Dispose(); }
                                    finally { }
                                }
                            }
                        }
                        _baseItems = null;
                    }

                    // 清理空闲缓存项目
                    if (_freeItems != null)
                    {
                        lock (_freeItems)
                        {
                            while (_freeItems.TryDequeue(out var ci))
                            {
                                if (ci.Value is IDisposable obj)
                                {
                                    try { obj.Dispose(); }
                                    finally { }
                                }
                            }
                        }
                        _freeItems = null;
                    }

                    // 清理繁忙缓存项目
                    if (_busyItems != null)
                    {
                        lock (_busyItems)
                        {
                            foreach (var ci in _busyItems)
                            {
                                if (ci.Value is IDisposable obj)
                                {
                                    try { obj.Dispose(); }
                                    finally { }
                                }
                            }
                            _busyItems.Clear();
                        }
                        _busyItems = null;
                    }
                }

                // 如果disposing=false，只能释放非托管资源(unmanaged resources)。在这里添加释代码放非托管资源。例如：
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
        /// 空闲对象数量
        /// </summary>
        public int FreeCount
        {
            get { return _freeCount; }
        }

        /// <summary>
        /// 繁忙对象数量
        /// </summary>
        public int BusyCount
        {
            get { return _busyCount; }
        }

        /// <summary>
        /// 最大可缓存对象数量。默认100，0表示无上限
        /// </summary>
        public int MaxCount
        {
            get { return _maxCount; }
            set { _maxCount = value < 0 ? 163 : value; }
        }

        /// <summary>
        /// 最小可缓存对象数量(最低水位线)。默认3
        /// </summary>
        public int MinCount
        {
            get { return _minCount; }
            set { _minCount = value <= 0 ? 3 : value; }
        }

        /// <summary>
        /// 缓存清理间隔，以秒为单位，默认30s, 0表示永不清理。
        /// </summary>
        public int Interval
        {
            get { return _Interval; }
            set { _Interval = value < 0 ? 30 : value; }
        }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message
        {
            get { return _errMsg; }
            private set { _errMsg = value; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 取出一个空闲的对象
        /// </summary>
        /// <param name="ttl">存活时间，以秒为单位，默认10秒(超过以后还不归还，缓存清理时会被自动回收)，-1表示一直占用(缓存清理时不进行自动回收)</param>
        /// <returns></returns>
        public virtual T Get(int ttl = 10)
        {
            if (ttl == 0 || ttl < -1)
                ttl = 10;

            CacheItem ci = null;
            do
            {
                if (_baseItems.TryPop(out ci) || _freeItems.TryDequeue(out ci))
                {
                    // 从空闲队列取出，先从最常使用中取，再从空闲队列取
                    Interlocked.Decrement(ref _freeCount);
                    break;
                }
                else
                {
                    // 超出最大缓存数量后不能再分配
                    var count = _busyCount;
                    if (_maxCount > 0 && count >= _maxCount)
                    {
                        _errMsg = $"不能再申请{typeof(T).Name}缓存对象, 原因: 缓存已满({_maxCount})";
                        return default;
                    }

                    // 没有取到则尝试新建
                    try
                    {
                        ci = new CacheItem
                        {
                            Value = Activator.CreateInstance(typeof(T), true) as T
                        };
                    }
                    catch(Exception e)
                    {
                        _errMsg = $"创建对象{typeof(T).Name}失败, 原因: {e.Message}";
                        return default;
                    }
                    break;
                }
            } while (true);

            // 设置存活时间
            ci.TTL = ttl;

            // 设置最后时间
            ci.LastTime = DateTime.Now;

            // 加入繁忙队列
            _busyItems.TryAdd(ci.Value, ci);
            Interlocked.Increment(ref _busyCount);

            // 返回取到的对象
            return ci.Value;
        }

        /// <summary>将对象归还回缓存</summary>
        /// <param name="value"></param>
        public virtual bool Put(T value)
        {
            if (value == null) 
                return false;

            // 从繁忙队列查找归还的项目
            if (!_busyItems.TryRemove(value, out var ci))
            {
                return false;
            }

            // 设置最后时间，让其在空闲队列保留一段时间，防止被立即清理掉
            ci.LastTime = DateTime.Now;

            // 放入空闲队列
            if (_baseItems.Count < _minCount) 
            {
                _baseItems.Push(ci); // 优先放入最常使用的基础缓存
            }
            else
            {
                _freeItems.Enqueue(ci); // 基础缓存已满则放入空闲队列排队
            }
            
            // 更新闲忙状态
            Interlocked.Decrement(ref _busyCount);
            Interlocked.Increment(ref _freeCount);

            return true;
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
                int interval = this._Interval;
                int i = 0;
                while (_cancelToken != null)
                {
                    if (_cancelToken.IsCancellationRequested == true)
                    {
                        break; // 退出清理任务
                    }
                    else
                    {
                        if (interval > 0 && i >= interval) // 到达清理间隔
                        {
                            ClearCacheItem(); // 清理
                            i = 0;
                        }
                    }
                    if (interval > 0) // 防止无限计数
                    {
                        i += 1;
                    }
                    Thread.Sleep(1000);
                }
            });

            // 启动任务
            t.Start();
        }

        /// <summary>
        /// 清理缓存项目。
        /// </summary>
        private void ClearCacheItem()
        {
            int count = 0;

            // 清理使用不还的缓存
            if (!_busyItems.IsEmpty)
            {
                foreach (var item in _busyItems)
                {
                    var obj = item.Value;
                    if (obj.TTL > 0)
                    {
                        var esp = obj.LastTime.AddSeconds(obj.TTL);
                        var exp = obj.LastTime.AddSeconds(60); 
                        if (DateTime.Now > esp) // 已经超过存活时间，将其清理掉(注意并没有将其销毁，因为有可能仍然在活跃)。
                        {
                            if (_busyItems.TryRemove(item.Key, out var v))
                            {
                                Interlocked.Decrement(ref _busyCount);
                                count++;
                            }
                        }
                        else if(DateTime.Now > exp) // 申请使用超过1小时仍然占用，不归还将其踢出(注意并没有将其销毁，因为有可能仍然在活跃)。
                        {
                            if (_busyItems.TryRemove(item.Key, out var v))
                            {
                                Interlocked.Decrement(ref _busyCount);
                                count++;
                            }
                        }
                    }
                }
            }

            // 清理空闲缓存中没有使用的缓存项目
            if (_freeItems.IsEmpty)
            {
                while (_freeItems.TryPeek(out var ci))
                {
                    if (ci.TTL > 0)
                    {
                        var exp = ci.LastTime.AddSeconds(ci.TTL);
                        if (DateTime.Now > exp) // 已经超过存活时间，没有被使用则将其清理
                        {
                            if (_freeItems.TryDequeue(out ci))
                            {
                                IDisposable obj = ci.Value as IDisposable;
                                if (obj != null)
                                {
                                    try 
                                    { 
                                        obj.Dispose(); // 尝试将其销毁
                                    }
                                    catch(Exception e) 
                                    { 
                                        _errMsg = $"清理缓存对象{typeof(T).Name}失败, 原因: {e.Message}"; 
                                    }
                                }
                                Interlocked.Decrement(ref _freeCount);
                                count++;
                            }
                        }
                    }
                }
            }

            // 强制进行垃圾回收
            if (count > 0)
            {
                GC.Collect();
            }
        }

        #endregion

        #region 缓存项目

        /// <summary>
        /// 缓存项目
        /// </summary>
        private class CacheItem
        {
            /// <summary>
            /// 缓存值
            /// </summary>
            public T Value { get; set; }

            /// <summary>
            /// 存活时间，以秒为单位，默认10秒，-1表示一直占用
            /// </summary>
            public int TTL { get; set; }

            /// <summary>
            /// 最近访问时间
            /// </summary>
            public DateTime LastTime { get; set; }
        }

        #endregion
    }
}
