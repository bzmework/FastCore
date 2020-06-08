using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCore.HashAlg;

namespace FastCore.Bloom
{
    /// <summary>
    /// 布隆过滤器
    /// </summary>
    /// <remarks>
    /// 布隆过滤器的特点：
    /// 1.布隆过滤器说某个元素存在，那么元素不一定存在。
    /// 2.布隆过滤器说某个元素不存在，那么这个元素一定不存在。
    /// 参考：
    /// https://www.jasondavies.com/bloomfilter/
    /// https://github.com/jasondavies/bloomfilter.js
    /// https://hackernoon.com/probabilistic-data-structures-bloom-filter-5374112a7832
    /// https://github.com/joeyrobert/bloomfilter
    /// https://github.com/vla/BloomFilter.NetCore
    /// https://www.eecs.harvard.edu/~michaelm/postscripts/tr-02-05.pdf
    /// </remarks>
    public class BloomFilter<T>
    {
        private Murmur3KirschMitzenmacher _hashFunc = new Murmur3KirschMitzenmacher();
        private readonly BitArray _hashTable;
        private readonly object sync = new object(); // 同步锁

        private int _elementCount; // 元素数量
        private double _errorRate; // 误判率
        private int _capacity; // 实际容量
        private int _hashes; // 哈希函数数量

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="elementCount">元素数量</param>
        /// <param name="errorRate">误判率</param>
        public BloomFilter(int elementCount, double errorRate = 0.01)
        {
            if (elementCount < 1)
                throw new ArgumentOutOfRangeException("elementCount", elementCount, "元素数量必须大于0");
            if (errorRate >= 1 || errorRate <= 0)
                throw new ArgumentOutOfRangeException("errorRate", errorRate, "误判率必须介于0-1之间");
            
            var type = typeof(T);
            var typeCode = Type.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type);
            switch (typeCode)
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.String:
                case TypeCode.DateTime:
                    break; // OK

                default:
                    throw new NotSupportedException("不支持元素类型 " + type.Name);
            }

            _elementCount = elementCount;
            _errorRate = errorRate;

            _capacity = CalcBitCapacity(elementCount, errorRate);
            _hashes = CalcHashes(elementCount, _capacity);
            _hashTable = new BitArray(_capacity);

        }

        /// <summary>
        /// 增加一个元素到布隆过滤器
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns></returns>
        public bool Add(T element)
        {
            bool added = false;
            var data = Encoding.UTF8.GetBytes(Convert.ToString(element, CultureInfo.InvariantCulture)); // 将元素转换为UTF8字节
            var positions = _hashFunc.ComputeHash(data, _capacity, _hashes);
            lock (sync)
            {
                foreach (int position in positions)
                {
                    if (!_hashTable.Get(position))
                    {
                        added = true;
                        _hashTable.Set(position, true);
                    }
                }
            }
            return added;
        }

        /// <summary>
        /// 异步增加一个元素到布隆过滤器
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns></returns>
        public Task<bool> AddAsync(T element)
        {
            return Task.FromResult(Add(element));
        }

        /// <summary>
        /// 增加多个元素到布隆过滤器
        /// </summary>
        /// <param name="elements">元素集合</param>
        /// <returns></returns>
        public IList<bool> Add(IEnumerable<T> elements)
        {
            return elements.Select(e => Add(e)).ToList();
        }

        /// <summary>
        /// 异步增加多个元素到布隆过滤器
        /// </summary>
        /// <param name="elements">元素集合</param>
        /// <returns></returns>
        public async Task<IList<bool>> AddAsync(IEnumerable<T> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await AddAsync(el));
            }
            return result;
        }

        /// <summary>
        /// 元素是否存在于布隆过滤器中
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns></returns>
        public bool Contains(T element)
        {
            var data = Encoding.UTF8.GetBytes(Convert.ToString(element, CultureInfo.InvariantCulture)); // 将元素转换为UTF8字节
            var positions = _hashFunc.ComputeHash(data, _capacity, _hashes);
            lock (sync)
            {
                foreach (int position in positions)
                {
                    if (!_hashTable.Get(position))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 异步检查元素是否存在于布隆过滤器中
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns></returns>
        public Task<bool> ContainsAsync(T element)
        {
            return Task.FromResult(Contains(element));
        }

        /// <summary>
        /// 从布隆过滤器中删除所有元素
        /// </summary>
        public void Clear()
        {
            lock (sync)
            {
                _hashTable.SetAll(false);
            }
        }

        /// <summary>
        /// 异步从布隆过滤器中删除所有元素
        /// </summary>
        public Task ClearAsync()
        {
            Clear();
            return Task.FromResult(0); // Empty
        }

        /// <summary>
        /// 根据元素数量和误判率计算出布隆过滤器实际需要的位容量
        /// </summary>
        /// <remarks>
        /// 公式：m = - n*ln(p) / (ln2)^2
        /// m为布隆过滤器长度(以位为单位)，n为插入的元素个数，p为误报率。
        /// </remarks>
        /// <param name="n">元素数量</param>
        /// <param name="p">误判率</param>
        /// <returns>布隆过滤器实际需要的位容量</returns>
        private static int CalcBitCapacity(long n, double p)
        {
            return (int)Math.Ceiling(-1 * (n * Math.Log(p)) / Math.Pow(Math.Log(2), 2));
        }

        /// <summary>
        /// 根据布隆过滤器长度和元素个数计算出Hash函数个数
        /// </summary>
        /// 公式：k = m/n * ln2
        /// k为哈希函数个数，m为布隆过滤器长度，n为插入的元素个数
        /// <param name="n">插入的元素个数</param>
        /// <param name="m">布隆过滤器长度</param>
        /// <returns>哈希函数个数</returns>
        private static int CalcHashes(long n, long m)
        {
            return (int)Math.Ceiling((Math.Log(2) * m) / n);
        }
    }
}
