using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using FastCore.Cache;

namespace FastCore.Redis
{
    /// <summary>
    /// Redis连接。
    /// 仅用于将命令打包发送给服务器，从服务器接收返回数据，不要增加其它功能。
    /// </summary>
    public class RedisConnection : IDisposable
    {
        #region 成员变量

        private StringBuilder _sb; // 快速字符串
        private RedisOption _option; // 选项配置
        private TcpClient _conn; // 当前连接

        #endregion

        #region 构造与析构

        /// <summary>
        /// 实例化
        /// </summary>
        public RedisConnection()
        {
            _sb = new StringBuilder();
        }

        /// <summary>
        /// 卸载资源。
        /// 这个析构函数只有在Dispose方法没有被调用时才会运行。
        /// </summary>
        ~RedisConnection()
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
                    if (_sb != null)
                    {
                        _sb.Clear();
                        _sb = null;
                    }
                    if (_conn != null)
                    {
                        _conn.Dispose();
                        _conn = null;
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

        #region 方法

        /// <summary>
        /// 是否已连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return _conn != null && _conn.Connected;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if( _conn != null && _conn.Connected)
            {
                _conn.Close(); // Close调用的是Dispose
                _conn = null;
            }
        }

        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <param name="option">配置</param>
        public void Initialize(RedisOption option)
        {
            _option = option;
            this.Clear();
        }

        /// <summary>执行命令</summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public IList<object> Execute(RedisCommand cmd)
        {
            // 取得连接对象
            var ns = this.Connect();
            if (ns == null) return null;

            // 发送请求(Send Request)
            Send(ns, cmd);

            // 接收响应(Receive Response)
            return Receive(ns, 1);
        }

        /// <summary>
        /// 发送命令给服务器
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private bool Send(NetworkStream ns, RedisCommand cmd)
        {
            // 取得内存流对象
            var ms = Cache.Get();
            if (ms == null)
            {
                throw new RedisClientException(Cache.Message, MethodBase.GetCurrentMethod());
            }

            // 制作命令包并将其写入内存流
            // 说明：Redis的管道(pipelining)技术就是将不同命令打包在一起批量发送。
            // 参考：https://redis.io/topics/pipelining
            try
            {
                // 取得并写入命令和参数，示例：*2\r\n$4\r\nLLEN\r\n$6\r\nmylist\r\n
                foreach (var obj in cmd.Value)
                {
                    if (obj is byte[] byts)
                    {
                        ms.Write(byts, 0, byts.Length);
                    }
                }
            }
            catch (Exception e)
            {
                throw new RedisClientException($"创建命令{ObjectConvert.C2Str(cmd.Command)}失败({e.Message})", MethodBase.GetCurrentMethod(), ObjectConvert.C2Str(ms.ToArray()));
            }

            // 尝试将内存流写入到网络流发送给服务器
            try
            {
                ms.WriteTo(ns);
            }
            catch(Exception e)
            {
                throw new RedisClientException($"发送命令{ObjectConvert.C2Str(cmd.Command)}失败({e.Message})", MethodBase.GetCurrentMethod(), ObjectConvert.C2Str(ms.ToArray()));
            }
            finally
            {
                // 归还内存流对象
                Cache.Put(ms);
            }

            // 返回
            return true;
        }

        /// <summary>
        /// 从服务器接收响应
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private IList<object> Receive(NetworkStream ns, int count)
        {
            /*
            * 响应格式
            * 1: 简单字符串(Simple Strings)，非二进制安全字符串，一般是状态回复。  以+开头，例如：+OK\r\n 
            * 2: 错误信息(Errors)。以-开头， 例如：-ERR unknown command 'mush'\r\n
            * 3: 整型数值(Integers)。以:开头， 例如：:1\r\n
            * 4：大容量字符串(Bulk Strings)，最大512M。以$开头， 例如：$4\r\mush\r\n
            * 5：数组(Arrays)。以*开头， 例如：*2\r\n$3\r\nfoo\r\n$3\r\nbar\r\n
            */

            try
            {
                // 获得网络数据流
                var bs = new BufferedStream(ns);

                // 解析返回的数据
                var list = new List<object>();
                for (var i = 0; i < count; i++)
                {
                    var head = (char)bs.ReadByte();
                    if (head == '$') // 大容量字符串(Bulk Strings)
                    {
                        list.Add(ReadBlock(bs));
                    }
                    else if (head == '*') // 数组(Arrays)
                    {
                        list.Add(ReadBlocks(bs));
                    }
                    else
                    {
                        // 字符串以换行为结束符
                        var str = ReadLine(bs);
                        if (head == '+' || head == ':') // 简单字符串和整型数值具有共性
                        {
                            list.Add(str);
                        }
                        else if (head == '-') // 按照官方建议如果应答是错误信息客户端应该抛出异常
                        {
                            throw new RedisServerException(str);
                        }
                        else
                        {
                            throw new RedisClientException($"不能解析响应({head})", MethodBase.GetCurrentMethod());
                        }
                    }
                }

                // 返回解析结果
                return list;
            }
            catch (Exception e)
            {
                throw new RedisClientException($"接收数据失败({e.Message})", MethodBase.GetCurrentMethod());
            }
        }

        /// <summary>
        /// 连接到Redis服务器
        /// </summary>
        /// <returns></returns>
        private NetworkStream Connect()
        {
            NetworkStream ns = null;

            // 检查连接
            var connected = _conn != null && _conn.Connected; // 是否已连接
            try
            {
                if (connected)
                {
                    ns = _conn?.GetStream(); // 获得NetworkStream，用于发送和接收数据
                    connected = ns != null && _conn != null && _conn.Connected && ns != null && ns.CanWrite && ns.CanRead; // 连接是否可用
                }
            }
            catch(Exception e)
            {
                Log.Warn($"取连接信息失败: {e.Message}", MethodBase.GetCurrentMethod());
            }

            // 重建连接
            if (!connected)
            {
                // 关闭连接
                if(_conn != null)
                {
                    _conn.Close(); // Close调用的是Dispose
                    _conn = null;
                }

                // 创建连接对象
                _conn = new TcpClient
                {
                    SendTimeout = _option.SendTimeout,
                    ReceiveTimeout = _option.ReceiveTimeout
                };

                // 连接到服务器
                try
                {
                    if (_option.ConnectTimeout <= 0)
                    {
                        // 未设置连接超时则直接连接
                        _conn.Connect(_option.Server, _option.Port);
                    }
                    else
                    {
                        // 采用异步来解决连接超时问题
                        IPAddress.TryParse(_option.Server, out var ip);
                        var bc = _conn.BeginConnect(ip, _option.Port, null, null);
                        if (!bc.AsyncWaitHandle.WaitOne(_option.ConnectTimeout, true))
                        {
                            _conn.Close();
                            throw new RedisClientException($"连接服务器{_option.Server}超时({_option.ConnectTimeout}ms)", MethodBase.GetCurrentMethod());
                        }
                        _conn.EndConnect(bc);
                    }

                    // 连接成功，创建NetworkStream用于发送和接收数据
                    ns = _conn.GetStream();
                }
                catch(Exception e)
                {
                    throw new RedisClientException($"连接到服务器{_option.Server}: {_option.Port}失败", MethodBase.GetCurrentMethod());
                }
                finally
                {
                    // 未连接成功则销毁
                    if (!_conn.Connected)
                    {
                        _conn.Dispose();
                        _conn = null;
                    }
                }
            }

            // 返回
            return ns;
        }

        /// <summary>
        /// 尝试清理掉历史残留数据
        /// </summary>
        private void Clear()
        {
            try
            {
                if (_conn != null && _conn.Connected) // 是否已连接
                {
                    var ns = _conn?.GetStream(); 
                    if (ns != null)
                    {
                        var count = 0;
                        if (ns is NetworkStream nss && nss.DataAvailable)
                        {
                            var buf = new byte[1024];
                            do
                            {
                                count = ns.Read(buf, 0, buf.Length);
                            } while (count > 0 && nss.DataAvailable);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn($"尝试清理连接中的残留数据失败: {e.Message}", MethodBase.GetCurrentMethod());
            }
        }

        /// <summary>
        /// 读取一行
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        private string ReadLine(Stream bs)
        {
            lock (_sb)
            {
                // 清理干净
                _sb.Clear();

                // 读取一行数据，以\r\n结束
                while (true)
                {
                    var b = bs.ReadByte();
                    if (b < 0) break;

                    if (b == '\r')
                    {
                        var l = bs.ReadByte();
                        if (l < 0) break;
                        if (l == '\n') break;
                        _sb.Append((char)b);
                        _sb.Append((char)l);
                    }
                    else
                    {
                        _sb.Append((char)b);
                    }
                }
                var val = _sb.ToString();

                // 返回
                return val;
            }
        }

        /// <summary>
        /// 读取一个大容量字符串(Bulk Strings)
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        private byte[] ReadBlock(Stream bs)
        {
            /*
             * 示例：
             * $6\r\n
             * foobar\r\n
             */
            
            var len = ObjectConvert.C2Int(ReadLine(bs)); // 读取长度
            if (len > 0)
            {
                var buf = new byte[len];
                var count = bs.Read(buf, 0, len); // 读取数据
                if (count > 0)
                {
                    bs.ReadByte(); // 丢弃"\r"
                    bs.ReadByte(); // 丢弃"\n"
                    return buf;
                }
            }

            return default;
        }

        /// <summary>
        /// 读取一个数组(Arrays)
        /// 说明：按照官方文档数组中的元素可以是混合的RESP类型，但根据官方文档对客户端-服务器的交互方式，应该总是：客户端向Redis服务器发送一个由大容量字符串组成的RESP数组。
        ///      因此这里读取数组时只读取数组中的BulkStrings和嵌套的数组中数据。
        /// 参考：https://redis.io/topics/protocol (RESP protocol description 和 Sending commands to a Redis Server)
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        private object[] ReadBlocks(Stream bs)
        {
            /*
             * 示例：
             * *2\r\n
             * $4\r\n
             * LLEN\r\n
             * $6\r\n
             * mylist\r\n
             */

            var n = ObjectConvert.C2Int(ReadLine(bs)); // 读出数组中元素的数量
            if (n > 0)
            {
                var arr = new object[n];
                for (var i = 0; i < n; i++)
                {
                    var head = (char)bs.ReadByte();
                    if (head == '$') // 读BulkStrings
                    {
                        arr[i] = ReadBlock(bs);
                    }
                    else if (head == '*') // 读嵌套数组
                    {
                        arr[i] = ReadBlocks(bs);
                    }
                }
                return arr;
            }

            return default;
        }

        #endregion

        #region 内存流缓存池

        /// <summary>
        /// Redis内存流缓存池
        /// </summary>
        private class RedisMemoryStreamCache : DataCache<MemoryStream> 
        {
            /// <summary>缓存容量, 以字节为单位。默认1024字节</summary>
            public int Capacity { get; set; } = 1024;

            /// <summary>
            /// 重写Get，取得缓存流
            /// </summary>
            /// <returns></returns>
            public override MemoryStream Get()
            {
                MemoryStream ms = base.Get();
                if (ms.Capacity < Capacity) // 设置缓存容量
                {
                    ms.Capacity = Capacity;
                }
                return ms;
            }

            /// <summary>
            /// 重写Put，归还缓存流
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public override bool Put(MemoryStream value)
            {
                //
                //MemoryStream的使用参见：NetworkStream.txt
                //if (value.Length > 0) // 由于每次只写入Length长度的数据到NetworkStream因此不必清理垃圾数据。
                //{
                //    Array.Clear(value.GetBuffer(), 0, (int)value.Length); // 清空残留的垃圾数据。
                //}
                // 
                value.SetLength(0); // 取消流长度限定，使得每次都能从头开始读写数据。
                value.Position = 0; // 将读写位置清零，防止不断地扩容，引起性能问题。
                return base.Put(value);
            }

        }

        /// <summary>
        /// 取得实例对象
        /// </summary>
        private RedisMemoryStreamCache instance = null; // 确保多线程模式下总能读取到正确值
        private RedisMemoryStreamCache Cache
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
                            instance = new RedisMemoryStreamCache()
                            {
                                Capacity = 1024 // 默认缓存容量
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
