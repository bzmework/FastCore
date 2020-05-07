
namespace FastCore.Redis
{
    /// <summary>
    /// Redis选项配置
    /// </summary>
    public class RedisOption
    {
        /// <summary>服务器，例如：127.0.0.1</summary>
        public string Server { get; set; } = "127.0.0.1";

        /// <summary>端口，例如：6379</summary>
        public int Port { get; set; } = 6379;

        /// <summary>密码</summary>
        public string Password { get; set; } = "";

        /// <summary>数据库，默认0。Redis默认内建0-15个数据库</summary>
        public int Db { get; set; } = 0;

        /// <summary>连接超时时间，以毫秒为单位。默认5000ms</summary>
        public int ConnectTimeout { get; set; } = 5000;

        /// <summary>发送数据超时时间，以毫秒为单位。默认3000ms</summary>
        public int SendTimeout { get; set; } = 3000;

        /// <summary>接收数据超时时间，以毫秒为单位。默认3000ms</summary>
        public int ReceiveTimeout { get; set; } = 3000;

        /// <summary>出错时重试次数，默认3次</summary>
        public int RetryCount { get; set; } = 3;

    }
}
