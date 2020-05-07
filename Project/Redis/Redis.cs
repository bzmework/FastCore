
namespace FastCore.Redis
{
    /// <summary>
    /// Redis工厂，创建Redis客户端、集群等
    /// </summary>
    public class Redis
    {
        #region 单例

        private static volatile Redis instance = null; //确保多线程模式下总能读取到正确值
        private static readonly object objectLock = new object();

        /// <summary>
        /// 取得Redis客户端实例
        /// </summary>
        public static Redis Instance
        {
            get
            {
                // 使用双检锁模式，保证多线程应用模式下只存在唯一实例。
                if (instance == null) // 保证了性能
                {
                    lock (objectLock) // 保证了线程安全
                    {
                        if (instance == null) // 保证了只有一个实例被创建
                        {
                            instance = new Redis();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        #region 构造与析构

        /// <summary>
        /// 私有构造函数，防止实例化
        /// </summary>
        private Redis()
        {
            //
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        ~Redis()
        {
            //
        }

        #endregion

        #region 方法

        /// <summary>
        /// 创建RedisClient
        /// </summary>
        /// <param name="server">服务器，默认为127.0.0.1</param>
        /// <param name="port">端口，默认为6379</param>
        /// <param name="db">数据库，默认为0，Redis默认内置了0-15个数据库</param>
        /// <returns>RedisClient对象</returns>
        public RedisClient Create(string server = "127.0.0.1", int port = 6379, int db = 0)
        {
            return new RedisClient(new RedisOption()
            { 
                Server = server,
                Port = port,
                Db = db
            });
        }

        /// <summary>创建RedisClient</summary>
        /// <param name="server">服务器，默认为127.0.0.1</param>
        /// <param name="port">端口，默认为6379</param>
        /// <param name="password">密码，默认为空</param>
        /// <param name="db">数据库，默认为0，Redis默认内置了0-15个数据库</param>
        /// <returns>RedisClient对象</returns>
        public RedisClient Create(string server = "127.0.0.1", int port = 6379, string password = "", int db = 0)
        {
            return new RedisClient(new RedisOption()
            {
                Server = server,
                Port = port,
                Password = password,
                Db = db
            });
        }

        /// <summary>
        /// 创建RedisClient
        /// </summary>
        /// <param name="option">配置</param>
        /// <returns></returns>
        public RedisClient Create(RedisOption option)
        {
            return new RedisClient(option);
        }

        #endregion
    }
}
