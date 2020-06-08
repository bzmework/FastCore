# FastCore
 FastCore是一个极简高效的.NET Core类库

FastCore的设计目标是：简洁、稳定、高效。 在代码编写上尽量做到简洁，运行速度上尽量做到稳定且高效。   
设计FastCore的目的是：其将其运用到.Net Core WebAPI项目中作为核心类库使用。   
   
FastCore目前实现的功能模块如下：   
```
Cache：包括数据缓存，内存缓存，消息缓存，对象缓存。其中Cache类是高速内存缓存和Redis缓存的统一，保证任何时候缓存都有效(参见测试样例)。   
Hash：包括Md5、Sha256、MurmurHash3等散列算法。MurmurHash3是谷歌采用的哈希算法，通常用于搜索引擎中为网址生成唯一的散列值，高效率且低碰闯率(参见测试样例)。   
Json：基于NetJson重构，性能优于Newtonsoft.Json和System.Text.Json(参见测试样例)。  
Jwt：JSON Web Token (JWT)，简洁易用(参见测试样例)。学习文档参见“Jwt.docx”。   
Log：日志管理，放弃了log4net，能输出到文件、邮件、控制台、调试窗口即可(参见测试样例)。   
Redis：Redis数据库客户端，性能优于其它C#实现的Redis客户端(参见测试样例)。学习文档参见“Session(含Redis).docx”。   
Security：加密解密，包含主流常用的Aes、Des、Tea等(参见测试样例)。
UniqueID：唯一ID生成器，包含主流的GuidCombGenerator和SnowflakeGenerator(参见测试样例)。学习文档参见“UniqueID生成算法.docx”。   
Bloom：布隆过滤器，用于防止缓存穿透、击穿、雪崩等等。学习文档参见“布隆过滤器.docx”。
Utility：功能模块，主要包含各种数据类型之间的相互转换。其中QuickConvert采用了不安全代码(即C++代码)，性能虽然优于系统函数，但不建议采用，仅供学习参考。
```

重要文档说明：   
```
Jwt.docx：详细描述了JSON Web Token (JWT)的结构以及应用场景。   
service.docx：这是一篇重要的文档，分两章详细描述了依赖注入和控制反转及其应用，第一章从设计模式的角度阐述了”依赖注入和控制反转“的起源和设计，这是其它千篇一律的各种书籍和文章中让你对”依赖注入和控制反转“一头雾水所不具备的。   
在你明白了依赖注入和控制反转“的理论依据以后，第二章则介绍了依赖注入在Asp.Net Core中的应用。通过阅读这篇文档希望对你能有所帮助，别动不动就”依赖注入和控制反转“炫技，它并不是那么完美。   
Session(含Redis).docx：介绍了Asp.Net Core中的各种数据缓存方式，在介绍分布式缓存的时候，着重介绍了Redis数据库的基础重点知识及其应用场景，建议你阅读以后，研读源码。
UniqueID生成算法.docx：详细阐述唯一ID生成器GuidCombGenerator和SnowflakeGenerator。GuidCombGenerator用于生成字符串UID，SnowflakeGenerator用于生成数值UID。  
```
   
测试样例：   

![image](https://github.com/bzmework/fastcore/blob/master/test.png)     
https://img.wenhairu.com/image/YUnAG

编译环境：   
Windows 10   
Visual Studio 2019   

QQ讨论群：```948127686```   


