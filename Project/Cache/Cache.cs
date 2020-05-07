using FastCore.Redis;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FastCore.Cache
{
    /// <summary>
    /// 缓存。采用内存和Redis缓存数据。
    /// </summary>
    /// <remarks>
    /// 缓存逻辑是：优先使用Redis缓存数据，如果Redis不可用则进行降级采用内存缓存数据，这样就可以保证缓存总是有效。
    /// </remarks>
    public static class Cache
    {
        #region 成员变量

        /// <summary>取消清理令牌</summary>
        private static CancellationTokenSource _cancelToken = null;

        /// <summary>n内存缓存</summary>
        private static MemoryCache _memCache;

        /// <summary>Redis缓存项目</summary>
        private static RedisClient _redisCache;

        /// <summary>Redis缓存是否可用</summary>
        private static volatile bool _redisValid = false;

        #endregion

        #region 方法

        /// <summary>
        /// 配置缓存 
        /// </summary>
        /// <param name="option">Redis选项配置</param>
        /// <returns></returns>
        public static void Config(RedisOption option = null)
        {
            _redisValid = false;
            _memCache = new MemoryCache(true, 60, 0, true);
            if (option != null)
            {
                _redisCache = new RedisClient(option);
                CreateMonitorTask(); // 创建监控任务
            }
        }

        /// <summary>尝试设置缓存项目</summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="ttl">项目的存活时间，以秒为单位。默认-1表示永不过期。</param>
        /// <returns></returns>
        public static bool TrySet<T>(string key, T value, int ttl = -1)
        {   
            // 优先写入Redis中
            if (_redisValid && _redisCache != null)
            {
                try
                {
                    if(_redisCache.Set<T>(key, value, ttl))
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    _redisValid = false;
                    Log.Error(e.Message, MethodBase.GetCurrentMethod());
                }
            }
            
            // 降级写入内存缓存
            try
            {
                return _memCache.Set<T>(key, value, ttl);
            }
            catch (Exception e)
            {
                _redisValid = false;
                Log.Error(e.Message, MethodBase.GetCurrentMethod());
            }

            return false;
        }

        /// <summary>尝试获取缓存项</summary>
        /// <param name="key">键</param>
        /// <param name="value">返回值</param>
        /// <returns></returns>
        public static bool TryGet<T>(string key, out T value)
        {
            value = default;

            // 优先从Redis中读取
            if (_redisValid && _redisCache != null)
            {
                try
                {
                    value = _redisCache.Get<T>(key);
                    return true;
                }
                catch (Exception e)
                {
                    _redisValid = false;
                    Log.Error(e.Message, MethodBase.GetCurrentMethod());
                }
            }

            // 降级从内存缓存读取
            try
            {
                value = _memCache.Get<T>(key);
                return true;
            }
            catch (Exception e)
            {
                _redisValid = false;
                Log.Error(e.Message, MethodBase.GetCurrentMethod());
            }

            return false;
        }

        /// <summary>
        /// 销毁缓存
        /// </summary>
        public static void Dispose()
        {
            // 关闭监控任务
            if (_cancelToken != null)
            {
                _cancelToken.Cancel();
                Task.WaitAll();
                _cancelToken.Dispose();
                _cancelToken = null;
            }

            // 销毁缓存
            if (_redisCache != null)
            {
                _redisCache.Dispose();
                _redisCache = null;
            }
            if (_memCache != null)
            {
                _memCache.Dispose();
                _memCache = null;
            }
        }

        #endregion

        #region 监控

        /// <summary>创建监控任务</summary>
        private static void CreateMonitorTask()
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
                        break; // 退出清理任务
                    }
                    else
                    {
                        // 检查Redis服务器是否可用
                        if (_redisCache != null)
                        {
                            try
                            {
                                _redisValid = _redisCache.Ping();
                                Log.Info("Redis缓存可用");
                            }
                            catch(Exception e)
                            {
                                _redisValid = false;
                                Log.Info($"Redis缓存不可用({e.Message})");
                            }
                        }
                    }
                    Thread.Sleep(5000);
                }
            });

            // 启动任务
            t.Start();
        }

        #endregion

    }
}
