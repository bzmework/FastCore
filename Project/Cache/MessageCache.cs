using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FastCore.Cache
{
    /// <summary>
    /// 消息缓存，用于缓存消息数据、Socket连接数据等
    /// </summary>
    /// <remarks>
    /// 由于系统利用了任务排队+线程池并行处理消息，因此处理上百万消息都没有问题，响应速度取决于[CPU数量]和['消息处理函数'处理消息的速度]。
    /// </remarks>
    public class MessageCache<T> : IDisposable
    {
        #region 成员变量

        /// <summary>最大数量(最高水位线)。默认1000，0表示无上限</summary>
        private int _maxCount;

        /// <summary>取消清理令牌</summary>
        private CancellationTokenSource _cancelToken = null;

        /// <summary>排队项目</summary>
        private ConcurrentQueue<T> _enqueueItems;

        /// <summary>消息处理</summary>
        private Action<T> _messageAction;

        #endregion

        #region 构造与析构

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="maxCount">最大消息数量，0表示无上限</param>
        /// <param name="messageAction">消息处理函数</param>
        public MessageCache(Action<T> messageAction, int maxCount = 0)
        {
            _maxCount = maxCount < 0 ? 1000 : maxCount;
            _messageAction = messageAction;
            _enqueueItems = new ConcurrentQueue<T>();
            this.CreateProcessTask(); // 创建消息处理任务
        }

        /// <summary>
        /// 卸载资源。
        /// 这个析构函数只有在Dispose方法没有被调用时才会运行。
        /// </summary>
        ~MessageCache()
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
                    // 关闭消息处理任务
                    if (_cancelToken != null)
                    {
                        _cancelToken.Cancel();
                        Task.WaitAll();
                        _cancelToken.Dispose();
                        _cancelToken = null;
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
        /// 最大消息数量。默认1000，0表示无上限
        /// </summary>
        public int MaxCount
        {
            get { return _maxCount; }
            set { _maxCount = value < 0 ? 1000 : value; }
        }

        #endregion

        #region 方法
 
        /// <summary>消息排队</summary>
        /// <param name="value"></param>
        public virtual bool Enqueue(T value)
        {
            if (value == null) 
                return false;

            if(_maxCount > 0 && _enqueueItems.Count > _maxCount) // 已经超过允许的最大消息数量
                return false;

            _enqueueItems.Enqueue(value); // 放入队列排队

            return true;
        }

        /// <summary>创建消息处理任务</summary>
        private void CreateProcessTask()
        {
            // 初始化取消令牌
            _cancelToken = new CancellationTokenSource();

            // 创建任务
            Task t = new Task(delegate
            {
                while (_cancelToken != null)
                {
                    if (_cancelToken.IsCancellationRequested == true)
                    {
                        break; // 退出处理任务
                    }
                    else
                    {
                        if (_enqueueItems.Count == 0) // 队列为空
                        {
                            //Console.WriteLine($"队列中无消息");
                            Thread.Sleep(500); // 等待
                        }
                        else
                        {
                            while (_enqueueItems.TryDequeue(out T message)) 
                            {
                                ProcessMessage(message); // 处理消息
                            }
                        }
                    }
                }
            });

            // 启动任务
            t.Start();
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="value">消息</param>
        private void ProcessMessage(T value)
        {
            // 系统会将Task放入线程池中排队
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _messageAction(value);
                }
                catch (Exception e)
                {
                    Log.Error($"处理消息{value}失败, 任务ID:{Task.CurrentId}, 线程ID:{Thread.CurrentThread.ManagedThreadId}, 错误: {e.Message}");
                }
            });
        }

        #endregion
    }
}
