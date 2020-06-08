﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FastCore.Bloom
{
    /// <summary>
    /// Building a Better Bloom Filter" by Adam Kirsch and Michael Mitzenmacher,
    /// https://www.eecs.harvard.edu/~michaelm/postscripts/tr-02-05.pdf
    /// </summary>
    public class Murmur3KirschMitzenmacher : Murmur3
    {
        /// <summary>
        /// 计算哈希
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="m">输出范围</param>
        /// <param name="k">要计算的哈希数量</param>
        /// <returns></returns>
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            int[] positions = new int[k];
            long hash1 = MurmurHash3_32(0, data, 0, data.Length);
            long hash2 = MurmurHash3_32((uint)hash1, data, 0, data.Length);
            for (int i = 0; i < k; i++)
            {
                positions[i] = (int)((hash1 + i * hash2) % m);
            }
            return positions;
        }
    }

    /// <summary>
    /// Murmur3哈希算法
    /// </summary>
    public class Murmur3
    {
        private const int IntMax = 2147483647;

        /// <summary>
        /// 计算哈希
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="m">输出范围</param>
        /// <param name="k">要计算的哈希数量</param>
        /// <returns></returns>
        public virtual int[] ComputeHash(byte[] data, int m, int k)
        {
            int[] positions = new int[k];
            uint seed = 0;
            int hashes = 0;
            while (hashes < k)
            {
                seed = MurmurHash3_32(seed, data, 0, data.Length);
                int hash = Rejection(seed, m);
                if (hash != -1)
                {
                    positions[hashes++] = hash;
                }
            }
            return positions;
        }

        protected static uint MurmurHash3_32(uint seed, byte[] data, int offset, int count)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            int length = count;
            int blocks = length / 4;
            uint hash = seed;

            uint k1;
            // body
            for (int i = 0; i < blocks; ++i)
            {
                k1 = BitConverter.ToUInt32(data, offset + i * 4);

                k1 *= c1;
                k1 = RotateLeft(k1, 15);
                k1 *= c2;

                hash ^= k1;
                hash = RotateLeft(hash, 13);
                hash = hash * 5 + 0xe6546b64;
            }

            // tail
            k1 = 0;
            var tailIdx = offset + blocks * 4;
            switch (length & 3)
            {
                case 3:
                    k1 ^= (uint)(data[tailIdx + 2]) << 16;
                    goto case 2;
                case 2:
                    k1 ^= (uint)(data[tailIdx + 1]) << 8;
                    goto case 1;
                case 1:
                    k1 ^= data[tailIdx + 0];
                    k1 *= c1; k1 = RotateLeft(k1, 15);
                    k1 *= c2;
                    hash ^= k1;
                    break;
            };

            // finalization
            hash ^= (uint)length;

            hash ^= hash >> 16;
            hash *= 0x85ebca6b;
            hash ^= hash >> 13;
            hash *= 0xc2b2ae35;
            hash ^= hash >> 16;

            return hash;
        }

        /// <summary>
        /// Perform rejection sampling on a 32-bit,
        ///https://en.wikipedia.org/wiki/Rejection_sampling
        /// </summary>
        /// <param name="random">The random.</param>
        /// <param name="m">integer output range.</param>
        /// <returns></returns>
        private int Rejection(uint random, int m)
        {
            return Rejection((int)random, m);
        }

        /// <summary>
        /// Perform rejection sampling on a 32-bit,
        /// https://en.wikipedia.org/wiki/Rejection_sampling
        /// </summary>
        /// <param name="random">The random.</param>
        /// <param name="m">integer output range.</param>
        /// <returns></returns>
        private int Rejection(int random, int m)
        {
            random = Math.Abs(random);
            if (random > (IntMax - IntMax % m) || random == IntMax)
                return -1;
            return random % m;
        }

        private static uint RotateLeft(uint original, int bits)
        {
            return (original << bits) | (original >> (32 - bits));
        }

        private static uint RotateRight(uint original, int bits)
        {
            return (original >> bits) | (original << (32 - bits));
        }

    }
}
