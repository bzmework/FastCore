using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FastCore.Cache
{
    /// <summary>
    /// 高速数据缓存，用于缓存高频使用的内存数据流等。
    /// </summary>
    /// <remarks>
    /// 只能缓存实现了IDisposable接口的对象，以保证缓存池销毁的时候能成功销毁被缓存的对象。
    /// </remarks>
    public class DataCache<T>: IDisposable where T : class, IDisposable
    {
        #region DataCache结构

        // 
        // |        |
        // |--------|<- 最大缓存数量(最高水位线)(MaxCount)(这里无上限，除非恶意地只取出不归还，导致不断地分配内存直到内存耗尽)
        // |        |
        // |        |
        // |########|
        // |########|
        // |########|<- 后备存储(BackStores)，根据负荷自动增减，当活动插槽中无可用项目时从后备存储中取，使用完成后放在活动插槽，并不立即回收。
        // |########|
        // |########|
        // |--------|<- 最小可缓存对象数量(最低水位线)(MinCount)(根据CPU数量决定)
        // |########|
        // |########|<- 活动插槽(ActiveSlots)，保证总是能以最快的速度命中缓存，如果活动插槽不能满足需求就从后备存储取。
        // |########|
        // |--------|
        // |########|<- 活动项目(ActiveItem)，默认总是从这里获取缓存项目，被他人取走后才从活动插槽中去取。
        // |--------|
        //
        // 说明：与ObjectCache的不同之处是：DataCache的活动插槽用数组来代替了堆栈和队列，并维持在小范围数量(CPU*n)，保证能以最快速度查找到缓存项目。
        //

        #endregion

        #region 成员变量

        /// <summary>错误消息</summary>
        private string _errMsg;

        /// <summary>活动插槽</summary>
        private CacheItem[] _actSlots;

        /// <summary>后备存储</summary>
        private ConcurrentStack<CacheItem> _bakStores;

        /// <summary>当前使用中项目</summary>
        private ConcurrentDictionary<T, CacheItem> _useItems;

        /// <summary>当前活动的项目</summary>
        private T _actItem;

        #endregion

        #region 构造与析构

        /// <summary>
        /// 实例化
        /// </summary>
        public DataCache()
        {
            var count = Environment.ProcessorCount * 2 - 1;
            count = count < 7 ? 3 : count; // 最低4个高速缓存插槽
            _actSlots = new CacheItem[count];
            _bakStores = new ConcurrentStack<CacheItem>();
            _useItems = new ConcurrentDictionary<T, CacheItem>();
            _actItem = null;
        }

        /// <summary>
        /// 卸载资源。
        /// 这个析构函数只有在Dispose方法没有被调用时才会运行。
        /// </summary>
        ~DataCache()
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
                    //
                    // 清理活动插槽
                    //
                    if (_actSlots != null)
                    {
                        lock (_actSlots)
                        {
                            if (_actItem != null) // 使用中的
                            {
                                if (_actItem is IDisposable obj)
                                {
                                    try { obj.Dispose(); }
                                    finally { }
                                }
                                _actItem = null;
                            }

                            var items = _actSlots;
                            for (var i = 0; i < items.Length; ++i) // 空闲中的
                            {
                                if (items[i].Value != null)
                                {
                                    if (items[i].Value is IDisposable obj)
                                    {
                                        try { obj.Dispose(); }
                                        finally { }
                                    }
                                    items[i].Value = null;
                                }
                            }
                        }
                        _actSlots = null;
                    }

                    //
                    // 清理后备存储
                    //

                    if (_useItems != null) // 使用中的
                    {
                        lock (_useItems)
                        {
                            foreach (var ci in _useItems)
                            {
                                if (ci.Value is IDisposable obj)
                                {
                                    try { obj.Dispose(); }
                                    finally { }
                                }
                            }
                            _useItems.Clear();
                        }
                        _useItems = null;
                    }

                    if (_bakStores != null) // 空闲中的
                    {
                        lock (_bakStores)
                        {
                            while (_bakStores.TryPop(out var ci))
                            {
                                if (ci.Value is IDisposable obj)
                                {
                                    try { obj.Dispose(); }
                                    finally { }
                                }
                            }
                        }
                        _bakStores = null;
                    }
                }

                // 如果disposing=false，只能释放非托管资源(unmanaged resources)。在这里添加释放非托管资源。例如：
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
        /// <returns></returns>
        public virtual T Get()
        {
            // 从最近位置取
            var val = _actItem;
            if (val != null && Interlocked.CompareExchange(ref _actItem, null, val) == val)
            {
                return val;
            }
            
            // 从活动插槽中取
            var items = _actSlots;
            for (var i = 0; i < items.Length; i++)
            {
                val = items[i].Value;
                if (val == null) // 由于每次都是按照顺序归还，因此有一个插槽是空的，后面的插槽都是空的。
                {
                    break;
                }
                else if (Interlocked.CompareExchange(ref items[i].Value, null, val) == val)
                {
                    return val;
                }
            }

            // 从后备存储中取
            CacheItem ci;
            do
            {
                if (_bakStores.TryPop(out ci))
                {
                    break;
                }
                else
                {
                    try
                    {
                        // 没有取到则尝试新建
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
            _useItems.TryAdd(ci.Value, ci); // 加入使用中
            return ci.Value;
        }

        /// <summary>将对象归还回缓存</summary>
        /// <param name="value"></param>
        public virtual bool Put(T value)
        {
            if (value == null) 
                return false;

            // 放在最近位置
            if (_actItem == null && Interlocked.CompareExchange(ref _actItem, value, null) == null) // 空位置上
            {
                return true;
            }
                
             // 放在插槽中
            var items = _actSlots;
            for (var i = 0; i < items.Length; ++i)
            {
                if (Interlocked.CompareExchange(ref items[i].Value, value, null) == null) // 空插槽中
                {
                    return true;
                } 
            }

            // 放在后备存储
            if (_useItems.TryRemove(value, out var ci)) 
            {
                _bakStores.Push(ci);
                return true;
            }
            
            // 归还失败
            return false;

        }

        #endregion

        #region 缓存项目

        /// <summary>
        /// 缓存项目，与ObjectCache不一样，这里用结构实现CacheItem，
        /// 因为结构存储在堆栈上，可以避免被垃圾回收器清理掉。
        /// </summary>
        private struct CacheItem
        {
            /// <summary>
            /// 缓存值
            /// </summary>
            public T Value;
        }

        #endregion
    }
}
