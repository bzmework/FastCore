using System;
using System.Diagnostics;
using System.Collections.Generic;
using FastCore.Cache;
using System.Text;
using FastCore.Redis;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using FastCore.Json;
using Newtonsoft.Json;
using FastCore;
using FastCore.Security;
using FastCore.Jwt;
using FastCore.HashAlg;
using FastCore.UniqueID;
using System.Reflection;
using System.IO;
using System.Reflection.Emit;
using System.Data.SqlClient;

namespace Test
{
    [Serializable]
    public class Organization
    {
        public long OrganizationID { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
    }

    public class Department
    {
        public long DepartmentID { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentFullName { get; set; }
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            // 配置日志
            Log.Configure(LogTarget.Console | LogTarget.Debug);

            Test1();

            Int测试();

            Json测试();

            UID测试();

            哈希测试();

            加密解密测试();

            JWT测试();

            //消息处理测试();

            数据缓存测试();

            //Redis测试();
             
            // 内存占用测试
            //Test2(); // 占用约234MB
            //Test3(); // 占用约367MB。原因是Test2实际上只存储了一个对象。可以看出Newtonsoft.Json序列化后占用内存较低。

            // 对比测试
            //Test4(); // Newtonsoft.Json占用约367MB
            //Test5(); // System.Text.Json占用约537MB，从结果可以看出System.Text.Json虽然速度快但内存占用非常高，这不利用数据传输和持久化。

            Console.WriteLine("");
            Console.WriteLine("完成");
            Console.ReadLine();
        }


        /// <summary>
        /// 性能测试
        /// </summary>
        static void Test1()
        {
            int i;
            string key;
            string value;
            string str = "士大夫石帆胜丰石帆胜丰是否给嘀咕嘀咕嘀咕嘀咕高手高高手公司的公司士大夫石帆胜丰石帆胜丰石帆胜丰石帆胜丰士大夫石帆胜丰石帆胜丰是否给嘀咕嘀咕嘀咕嘀咕高手高高手公司的公司士大夫石帆胜丰石帆胜丰石帆胜丰石帆胜丰";
            bool ok;

            var org = new Organization()
            {
                OrganizationID = 100,
                OrganizationCode = "001",
                OrganizationName = "集团"
            };

            Console.WriteLine("");
            Console.WriteLine("--------关闭Json序列化---------");
            Console.WriteLine("");

            using (MemoryCache mc = new MemoryCache())
            {
                mc.JsonSerialize = false; // 关闭序列化

                Stopwatch t = new Stopwatch();
                t.Start();

                key = "A001";
                ok = mc.Set(key, str);
                Console.WriteLine($"新增单笔记录, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Restart();
                str = mc.Get<string>(key);
                Console.WriteLine($"读取单笔记录, 消耗{t.ElapsedMilliseconds}毫秒");
                Console.WriteLine("");

                t.Restart();
                i = 0;
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    org.OrganizationID = i;
                    ok = mc.Set(key, org);
                }
                Console.WriteLine($"增加{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Restart();
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    Organization val = mc.Get<Organization>(key);
                }
                Console.WriteLine($"读取{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                // 随机取出一个项目，验证正确性
                // 显示的对象信息相同，说明增加的是同一对象
                key = "k10000";
                Organization org1 = mc.Get<Organization>(key);
                Console.WriteLine($"{org1.OrganizationID}/{org1.OrganizationCode}/{org1.OrganizationName}");
                key = "k20000";
                Organization org2 = mc.Get<Organization>(key);
                Console.WriteLine($"{org2.OrganizationID}/{org2.OrganizationCode}/{org2.OrganizationName}");

                t.Stop();

                // 阻止结束
                //Console.ReadLine();
            }

            Console.WriteLine("");
            Console.WriteLine("--------开启Json序列化---------");
            Console.WriteLine("");

            using (MemoryCache mc2 = new MemoryCache())
            {
                mc2.JsonSerialize = true; // 关闭序列化

                Stopwatch t = new Stopwatch();
                t.Start();
           
                key = "A001";
                ok = mc2.Set(key, str);
                Console.WriteLine($"新增单笔记录, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Restart();
                str = mc2.Get<string>(key);
                Console.WriteLine($"读取单笔记录, 消耗{t.ElapsedMilliseconds}毫秒");
                Console.WriteLine("");

                t.Restart();
                i = 0;
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    org.OrganizationID = i;
                    ok = mc2.Set(key, org);
                }
                Console.WriteLine($"增加{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Restart();
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    Organization val = mc2.Get<Organization>(key);
                }
                Console.WriteLine($"读取{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                // 随机取出一个项目，验证正确性
                // 显示的对象信息相同，说明增加的是同一对象
                key = "k10000";
                Organization org1 = mc2.Get<Organization>(key);
                Console.WriteLine($"{org1.OrganizationID}/{org1.OrganizationCode}/{org1.OrganizationName}");
                key = "k20000";
                Organization org2 = mc2.Get<Organization>(key);
                Console.WriteLine($"{org2.OrganizationID}/{org2.OrganizationCode}/{org2.OrganizationName}");

                t.Stop();

                // 阻止结束
                //Console.ReadLine();
            }

        }

        /// <summary>
        /// 关闭Json序列化后的内存占用
        /// </summary>
        static void Test2()
        {
            int i;
            string key;
            string value;
            bool ok;

            var org = new Organization()
            {
                OrganizationID = 100,
                OrganizationCode = "001",
                OrganizationName = "集团"
            };

            Console.WriteLine("");
            Console.WriteLine("--------关闭Json序列化---------");

            using (MemoryCache mc = new MemoryCache())
            {
                mc.JsonSerialize = false; // 关闭序列化

                Stopwatch t = new Stopwatch();
                t.Start();

                i = 0;
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    org.OrganizationID = i;
                    ok = mc.Set(key, org);
                }
                Console.WriteLine($"增加{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Restart();
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    Organization val = mc.Get<Organization>(key);
                }
                Console.WriteLine($"读取{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                // 随机取出一个项目，验证正确性
                // 显示的对象信息相同，说明增加的是同一对象(引用类型按地址传递)
                key = "k10000";
                Organization org1 = mc.Get<Organization>(key);
                Console.WriteLine($"{org1.OrganizationID}/{org1.OrganizationCode}/{org1.OrganizationName}");
                key = "k20000";
                Organization org2 = mc.Get<Organization>(key);
                Console.WriteLine($"{org2.OrganizationID}/{org2.OrganizationCode}/{org2.OrganizationName}");

                t.Stop();

                // 阻止结束
                //Console.ReadLine();
            }
        }

        /// <summary>
        /// 开启Json序列化后的内存占用
        /// </summary>
        static void Test3()
        {
            int i;
            string key;
            string value;
            bool ok;

            var org = new Organization()
            {
                OrganizationID = 100,
                OrganizationCode = "001",
                OrganizationName = "集团"
            };

            Console.WriteLine("");
            Console.WriteLine("--------开启Json序列化---------");

            using (MemoryCache mc = new MemoryCache())
            {
                mc.JsonSerialize = true; // 开启序列化

                Stopwatch t = new Stopwatch();
                t.Start();

                i = 0;
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    org.OrganizationID = i;
                    ok = mc.Set(key, org);
                }
                Console.WriteLine($"增加{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Restart();
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    Organization val = mc.Get<Organization>(key);
                }
                Console.WriteLine($"读取{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                // 随机取出一个项目，验证正确性
                // 显示的对象信息相同，说明增加的是同一对象
                key = "k10000";
                Organization org1 = mc.Get<Organization>(key);
                Console.WriteLine($"{org1.OrganizationID}/{org1.OrganizationCode}/{org1.OrganizationName}");
                key = "k20000";
                Organization org2 = mc.Get<Organization>(key);
                Console.WriteLine($"{org2.OrganizationID}/{org2.OrganizationCode}/{org2.OrganizationName}");

                t.Stop();

                // 阻止结束
                //Console.ReadLine();
            }
        }

        /// <summary>
        /// 对比测试 Newtonsoft.Json
        /// </summary>
        static void Test4()
        {
            int i;
            string key;
            string value;
            bool ok;

            var org = new Organization()
            {
                OrganizationID = 100,
                OrganizationCode = "001",
                OrganizationName = "集团"
            };

            Console.WriteLine("");
            Console.WriteLine("--------测试Newtonsoft.Json序列化---------");

            using (MemoryCache mc = new MemoryCache())
            {
                mc.JsonSerialize = false; // 关闭序列化

                Stopwatch t = new Stopwatch();
                t.Start();

                i = 0;
                for (i = 0; i < 1000000; i++)
                {
                    org.OrganizationID = i;

                    key = "k" + i;
                    value = Newtonsoft.Json.JsonConvert.SerializeObject(org);
                    ok = mc.Set(key, value);
                }
                Console.WriteLine($"增加{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Restart();
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    value = mc.Get<string>(key);
                    org = Newtonsoft.Json.JsonConvert.DeserializeObject<Organization>(value);
                }
                Console.WriteLine($"读取{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Stop();

                // 随机取出一个项目，验证正确性
                // 显示的对象信息相同，说明增加的是同一对象
                key = "k10000";
                value = mc.Get<string>(key);
                Organization org1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Organization>(value);
                Console.WriteLine($"{org1.OrganizationID}/{org1.OrganizationCode}/{org1.OrganizationName}");

                key = "k20000";
                value = mc.Get<string>(key);
                Organization org2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Organization>(value);
                Console.WriteLine($"{org2.OrganizationID}/{org2.OrganizationCode}/{org2.OrganizationName}");

                // 阻止结束
                //Console.ReadLine();
            }

        }

        /// <summary>
        /// 对比测试 System.Text.Json
        /// </summary>
        static void Test5()
        {
            int i;
            string key;
            string value;
            bool ok;

            var org = new Organization()
            {
                OrganizationID = 100,
                OrganizationCode = "001",
                OrganizationName = "集团"
            };

            // 注意：System.Text.Json只能序列化属性，不能序列化字段
            // https://www.cnblogs.com/waku/p/11026630.html

            Console.WriteLine("");
            Console.WriteLine("--------测试System.Text.Json序列化---------");

            using (MemoryCache mc = new MemoryCache())
            {
                mc.JsonSerialize = false; // 关闭序列化

                Stopwatch t = new Stopwatch();
                t.Start();

                i = 0;
                for (i = 0; i < 1000000; i++)
                {
                    org.OrganizationID = i;

                    key = "k" + i;
                    value = System.Text.Json.JsonSerializer.Serialize<Organization>(org, JsonSerializerOptionsProvider.Options);
                    ok = mc.Set(key, value);
                }
                Console.WriteLine($"增加{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Restart();
                for (i = 0; i < 1000000; i++)
                {
                    key = "k" + i;
                    value = mc.Get<string>(key);
                    org = System.Text.Json.JsonSerializer.Deserialize<Organization>(value, JsonSerializerOptionsProvider.Options);
                }
                Console.WriteLine($"读取{i}条, 消耗{t.ElapsedMilliseconds}毫秒");

                t.Stop();

                // 随机取出一个项目，验证正确性
                // 显示的对象信息相同，说明增加的是同一对象
                key = "k10000";
                value = mc.Get<string>(key);
                Organization org1 = System.Text.Json.JsonSerializer.Deserialize<Organization>(value, JsonSerializerOptionsProvider.Options);
                Console.WriteLine($"{org1.OrganizationID}/{org1.OrganizationCode}/{org1.OrganizationName}");

                key = "k20000";
                value = mc.Get<string>(key);
                Organization org2 = System.Text.Json.JsonSerializer.Deserialize<Organization>(value, JsonSerializerOptionsProvider.Options);
                Console.WriteLine($"{org2.OrganizationID}/{org2.OrganizationCode}/{org2.OrganizationName}");

                // 阻止结束
                //Console.ReadLine();
            }

        }

        /// <summary>
        /// 测试Redis
        /// </summary>
        static void Redis测试()
        {
            Console.WriteLine("--------测试Redis---------");
            
            //var redis = Redis.Instance.Create("127.0.0.1", 6379, "clouderpwebappcache", 4);
            var redis = Redis.Instance.Create(new RedisOption()
            {
                Server = "127.0.0.1",
                Port = 6379,
                Password = "clouderpwebappcache",
                Db = 4
            });

            var org = new Organization()
            {
                OrganizationID = 100,
                OrganizationCode = "001",
                OrganizationName = "集团"
            };

            IList<Department> dpts = new List<Department>()
            {
                new Department()
                {
                    DepartmentID = 100,
                    DepartmentCode = "1001",
                    DepartmentName = "财务部"
                },
                new Department()
                {
                    DepartmentID = 200,
                    DepartmentCode = "1002",
                    DepartmentName = "研发部"
                }
            };

            bool ok;

            try
            {

                ok = redis.Ping();
                if (!ok)
                {
                    Log.Fatal("Redis服务器不可用");
                }

                ok = redis.Quit();

                ok = redis.Set("name", dpts);
                var val = redis.Get<IList<Department>>("name");

                //redis.Set("time", DateTime.Now, 5);
                //Console.WriteLine(redis.Get<DateTime>("time"));
                //Thread.Sleep(10000);
                //Console.WriteLine(redis.Get<DateTime>("time"));

                var dicall = new Dictionary<string, object>
                {
                    ["name"] = "hello",
                    ["time"] = DateTime.Now,
                    ["count"] = 1234
                };
                redis.Set(dicall, 300);

                var keys = new List<string>
                {
                    "name",
                    "time",
                    "count"
                };
                var dicGet = redis.Get<object>(keys);
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            // 模拟随机读写
            Task.Factory.StartNew(() =>
            {
                Stopwatch t = new Stopwatch();
                t.Start();

                for (int i = 0; i < 100; i++)
                {
                    int count = 1000;

                    t.Restart();
                    for (int n = 0; n < count; n++)
                    {
                        try
                        {
                            redis.Set("name", "张珊");
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Set 失败: {e.Message}");
                        }
                    }
                    Console.WriteLine($"第{i}次，Set {count}条记录，消耗{t.ElapsedMilliseconds}毫秒");

                    t.Restart();
                    for (int n = 0; n < count; n++)
                    {
                        try
                        {
                            redis.Get<string>("name");
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Get 失败: {e.Message}");
                        }
                    }
                    Console.WriteLine($"第{i}次，Get {count}条记录，消耗{t.ElapsedMilliseconds}毫秒");

                    Thread.Sleep(50);
                }

                t.Stop();

                // 关闭Redis客户端，清理资源，也可以调用Dispose()销毁，功能一样。
                redis.Close();

            });

        }

        /// <summary>
        /// 测试Redis性能
        /// </summary>
        static void Test8()
        {
            Console.WriteLine("--------测试Redis性能---------");

            //var redis = Redis.Instance.Create("127.0.0.1", 6379, "clouderpwebappcache", 4);
            var redis = Redis.Instance.Create(new RedisOption()
            {
                Server = "127.0.0.1",
                Port = 6379,
                Password = "clouderpwebappcache",
                Db = 4
            });

            Random rnd = new Random();
            bool rand = false;
            int threads = 1; // 线程数量
            int times = 10000; // 次数
            string key = "id";

            Console.WriteLine($"测试 {times:n0} 项，{threads,3:n0} 线程");

            var sw = Stopwatch.StartNew();
            if (rand)
            {
                // 随机操作，每个线程每次操作不同key，跳跃式
                Parallel.For(0, threads, k =>
                {
                    var val = rnd.Next().ToString();
                    for (var i = k; i < times; i += threads)
                    {
                        redis.Set(key + i, val);
                    }

                });
            }
            else
            {
                // 顺序操作，每个线程多次操作同一个key
                Parallel.For(0, threads, k =>
                {
                    var mykey = key + k;
                    var val = rnd.Next().ToString();
                    var count = times / threads;
                    for (var i = 0; i < count; i++)
                    {
                        redis.Set(mykey, val);
                    }
                });
            }
            sw.Stop();

            var speed = times * 1000 / sw.ElapsedMilliseconds;
            Console.WriteLine($"赋值 耗时 {sw.ElapsedMilliseconds,7:n0}ms 速度 {speed,9:n0} ops\r\n");
             
        }

        private static void 加密解密测试()
        {
            Console.WriteLine("--------加密解密测试---------");

            string key, pwd;
            key = KeyGen.GenerateRandomKey();
            key = KeyGen.GenerateAesKey();
            key = KeyGen.GenerateDesKey();
            key = KeyGen.GenerateTeaKey();

            key = "_elong.tech@2020_"; // 自己指定Key
            pwd = "hello12345你好";

            var p1 = Aes.Encrypt(pwd, key);
            var p2 = Aes.Decrypt(p1, key);
            Console.WriteLine($"Aes加密: {p1}, Aes解密: {p2}");

            var p3 = Rc4.Encrypt(pwd, key);
            var p4 = Rc4.Decrypt(p3, key);
            Console.WriteLine($"Rc4加密: {p3}, Rc4解密: {p4}");

            var p5 = Des.Encrypt(pwd, key);
            var p6 = Des.Decrypt(p5, key);
            Console.WriteLine($"Des加密: {p5}, Des解密:{p6}");

            var p7 = Tea.Encrypt(pwd, key);
            var p8 = Tea.Decrypt(p7, key);
            Console.WriteLine($"Tea加密: {p7}, Tea解密: {p8}");

            var p9 = Hash.Md5(pwd);
            Console.WriteLine($"MD5哈希: {p9}");
            Console.WriteLine("");
        }

        private static void JWT测试()
        {
            Console.WriteLine("--------JWT测试---------");

            // 申请一个密钥
            var key = JwtBuilder.GenerateKey("_elong.tech@2020_");

            Stopwatch t = new Stopwatch();
            t.Start();

            // 颁发令牌
            var token = new JwtBuilder()
                .WithAes(true)
                .WithAlgorithm(JwtSecurityAlgorithms.HmacSha256)
                .WithSecret(key)
                .AddClaim(JwtClaimNames.ExpirationTime, 60) // 60秒过期
                .AddClaim(nameof(Organization.OrganizationID), 100)
                .AddClaim(nameof(Organization.OrganizationName), "集团")
                .Build();

            Console.WriteLine($"颁发令牌，消耗{t.ElapsedMilliseconds}毫秒");
            t.Restart();

            // 解码令牌，返回字典
            var payload = new JwtBuilder()
                .WithAes(true)
                .WithAlgorithm(JwtSecurityAlgorithms.HmacSha256)
                .WithSecret(key)
                .Decode(token);
            
            Console.WriteLine($"解码令牌，返回字典，消耗{t.ElapsedMilliseconds}毫秒");
            t.Restart();

            // 解码令牌，返回对象
            var org = new JwtBuilder()
                .WithAes(true)
                .WithAlgorithm(JwtSecurityAlgorithms.HmacSha256)
                .WithSecret(key)
                .Decode<Organization>(token);

            Console.WriteLine($"解码令牌，返回对象，消耗{t.ElapsedMilliseconds}毫秒");
            t.Restart();

            // 解析令牌，返回指定的Claim
            var claim = new JwtBuilder()
                .WithAes(true)
                .WithAlgorithm(JwtSecurityAlgorithms.HmacSha256)
                .WithSecret(key)
                .Parse(token, JwtClaimNames.JwtId);
            
            Console.WriteLine($"解析令牌，返回Claim，JwtId: {claim}, 消耗{t.ElapsedMilliseconds}毫秒");
            t.Restart();

            // 验证令牌
            var msg = new JwtBuilder()
                .WithAes(true)
                .WithAlgorithm(JwtSecurityAlgorithms.HmacSha256)
                .WithSecret(key)
                .Verify(token);

            Console.WriteLine($"验证令牌，消耗{t.ElapsedMilliseconds}毫秒");
            t.Stop();
            Console.WriteLine("");
        }

        private static void 消息处理测试()
        {
            Console.WriteLine("--------消息处理测试---------");

            var msgCache = new MessageCache<string>((message) =>
            {
                Console.WriteLine($"处理消息{message}, 任务ID:{Task.CurrentId}, 线程ID:{Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(50); // 故意延迟
                //throw new Exception("无效消息"); // 模拟错误
            });

            int i;
            for (i = 0; i < 100; i++) // 由于系统采用线程池并行处理消息，因此处理上百万消息都没有问题，响应速度取决于CPU数量和消息处理函数处理消息的速度。
            {
                msgCache.Enqueue(i.ToString());
            }
            Console.WriteLine($"消息总数{i}");
            Thread.Sleep(1000); 

        }

        private static void 数据缓存测试()
        {
            Console.WriteLine("--------数据缓存测试---------");

            var org = new Organization()
            {
                OrganizationID = 100,
                OrganizationCode = "集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团",
                OrganizationName = "集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团集团"
            };

            // 配置缓存
            Cache.Config(new RedisOption()
            {
                Server = "127.0.0.1",
                Port = 6379,
                Password = "clouderpwebappcache",
                Db = 4,
                RetryCount = 0
            });

            // 模拟随机读写
            // 测试说明：关闭Redis服务器，设置1个极大的数(例如1千万)，不断读写给内存造成极大压力(必须设置Key的TTL防止崩溃)。注意观察内存在一段时间以后是否被自动清理。
            Console.Write($"50000万数据缓存测试...");
            int count = 100;
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    var rnd = new Random();
                    var ttl = rnd.Next(1, 3600);

                    string key = "k-" + i.ToString() /*+ rnd.Next(1, 10000000).ToString()*/;

                    org.OrganizationID = i;
                    Console.Write($"缓存数据{key}...");
                    if (Cache.TrySet<Organization>(key, org, ttl)) // 1个小时以内失效
                    {
                        Console.Write($"成功");
                    }
                    else
                    {
                        Console.Write($"......失败");
                    }
                    Console.WriteLine("");

                    Console.Write($"读取数据{key}...");
                    if (Cache.TryGet<Organization>(key, out Organization org2))
                    {
                        Console.Write($"成功");
                    }
                    else
                    {
                        Console.Write($"......失败");
                    }
                    Console.WriteLine("");

                    //Thread.Sleep(100);
                    i++;
                }
            });

            //Cache.Dispose(); // 销毁
            Console.WriteLine("");
        }

        private static void 哈希测试()
        {
            Console.WriteLine("--------哈希测试---------");
            int i;

            string str = "https://www.baidu.com/";

            Stopwatch t = new Stopwatch();
            t.Start();
            for ( i = 0; i < 100000; i++)
            {
                var md5 = Hash.Md5(str);
            }
            Console.WriteLine($"Md5生成字符串{str}的Hash值{i}次，消耗{t.ElapsedMilliseconds}毫秒");
            
            t.Restart();
            for (i = 0; i < 100000; i++)
            {
                var mur3 = Hash.Mur3(str);
            }
            Console.WriteLine($"Mur3生成字符串{str}的Hash值{i}次，消耗{t.ElapsedMilliseconds}毫秒");
            t.Stop();
            Console.WriteLine("");
        }

        private static void UID测试()
        {
            Console.WriteLine("--------UID测试---------");
            int i;

            Stopwatch t = new Stopwatch();
            t.Start();
            var uidGen1 = new GuidCombGenerator();
            for (i = 0; i < 100000; i++)
            {
                var uid1 = uidGen1.NextId();
            }
            Console.WriteLine($"GuidCombGenerator生成UID{i}次，消耗{t.ElapsedMilliseconds}毫秒");

            t.Restart();
            var uidGen2 = new SnowflakeGenerator(1, 2);
            for (i = 0; i < 100000; i++)
            {
                var uid2 = uidGen2.NextId();
            }
            Console.WriteLine($"SnowflakeGenerator生成UID{i}次，消耗{t.ElapsedMilliseconds}毫秒");
            t.Stop();
            Console.WriteLine("");
        }

        /// <summary>
        /// Json测试
        /// </summary>
        static void Json测试()
        {
            int i;
            int n = 100000;

            var org = new Organization()
            {
                OrganizationID = 100,
                OrganizationCode = "001",
                OrganizationName = "集团"
            };

            var jsonx = Json.Serialize(org);
            var orgx = Json.Deserialize<Organization>(jsonx);

            Console.WriteLine("");
            Console.WriteLine("--------Json测试---------");
           
            Stopwatch t = new Stopwatch();
            t.Start();

            i = 0;
            for (i = 0; i < n; i++)
            {
                var json = Json.Serialize(org);
            }
            Console.WriteLine($"FastCore.Json序列化{n}条, 消耗{t.ElapsedMilliseconds}毫秒");

            t.Restart();
            for (i = 0; i < n; i++)
            {
                var json = JsonConvert.SerializeObject(org);
            }
            Console.WriteLine($"Newtonsoft.Json序列化{n}条, 消耗{t.ElapsedMilliseconds}毫秒");

            t.Restart();
            for (i = 0; i < n; i++)
            {
                var json = System.Text.Json.JsonSerializer.Serialize<Organization>(org, JsonSerializerOptionsProvider.Options);
            }
            Console.WriteLine($"System.Text.Json序列化{n}条, 消耗{t.ElapsedMilliseconds}毫秒");

            t.Restart();
            for (i = 0; i < n; i++)
            {
                var org2 = Json.Deserialize<Organization>(jsonx);
            }
            Console.WriteLine($"FastCore.Json反序列化{n}条, 消耗{t.ElapsedMilliseconds}毫秒");

            t.Restart();
            for (i = 0; i < n; i++)
            {
                var org3 = JsonConvert.DeserializeObject<Organization>(jsonx);
            }
            Console.WriteLine($"Newtonsoft.Json反序列化{n}条, 消耗{t.ElapsedMilliseconds}毫秒");

            t.Restart();
            for (i = 0; i < n; i++)
            {
                var org4 = System.Text.Json.JsonSerializer.Deserialize<Organization>(jsonx, JsonSerializerOptionsProvider.Options);
            }
            Console.WriteLine($"System.Text.Json反序列化{n}条, 消耗{t.ElapsedMilliseconds}毫秒");

            t.Stop();
            Console.WriteLine("");

        }

        /// <summary>
        /// Int测试
        /// </summary>
        static void Int测试()
        {
            int i;
            int n = 10000000;

            Console.WriteLine("");
            Console.WriteLine("--------Int测试---------");

            Stopwatch t = new Stopwatch();

            t.Start();
            //for (i = 0; i < n; i++)
            //{
            //    var str1 = JsonInteger.itoa(1000);
            //}
            //Console.WriteLine($"{n}条, 消耗{t.ElapsedMilliseconds}毫秒");

            t.Restart();
            for (i = 0; i < n; i++)
            {
                var str2 = Convert.ToString(1000);
            }
            Console.WriteLine($"Convert.ToString {n}条, 消耗{t.ElapsedMilliseconds}毫秒");
            
            t.Restart();
            for (i = 0; i < n; i++)
            {
                var str3 = QuickConvert.C2Str(1000);
            }
            Console.WriteLine($"C2Str {n}条, 消耗{t.ElapsedMilliseconds}毫秒");

            t.Restart();
            for (i = 0; i < n; i++)
            {
                var str2 = Convert.ToDouble("1.23");
            }
            Console.WriteLine($"Convert.ToDouble {n}条, 消耗{t.ElapsedMilliseconds}毫秒");

            t.Restart();
            for (i = 0; i < n; i++)
            {
                var str3 = QuickConvert.C2Dbl("1.23");
            }
            Console.WriteLine($"C2Dbl {n}条, 消耗{t.ElapsedMilliseconds}毫秒");



            t.Stop();
            Console.WriteLine("");

        }
    }
}

