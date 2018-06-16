﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Lab1.B
{
    class Program
    {
        static readonly MD5 _md5 = MD5.Create();
        const int k = 128;
        const int b = 8;
        const int r = k / b;

        static void Main(string[] args)
        {
            var N = int.Parse(Console.ReadLine());
            var shingles = new List<List<bool>>(N);
            for (var i = 0; i < N; i++)
            {
                shingles.Add(SimHash(Console.ReadLine().Trim()));
            }

            var candidates = LSH(shingles);

            var Q = int.Parse(Console.ReadLine());
            var queries = new List<Tuple<int, int>>(Q);
            for (var i = 0; i < Q; i++)
            {
                var input = Console.ReadLine().Split(' ');
                var query = new Tuple<int, int>(int.Parse(input[0]), int.Parse(input[1]));
                queries.Add(query);
            }

            foreach (var tuple in queries)
            {
                var count = 0;
                var testShingle = shingles[tuple.Item1];
                var testCandidates = candidates[tuple.Item1];
                if (testCandidates == null)
                {
                    Console.WriteLine(0);
                    continue;
                }
                foreach (var candidateId in testCandidates)
                {
                    if (IsHammingDistanceAtMost(testShingle, shingles[candidateId], tuple.Item2))
                    {
                        count++;
                    }
                }
                Console.WriteLine(count);
            }
        }

        static Dictionary<int, HashSet<int>> LSH(List<List<bool>> simHashes)
        {
            var candidates = new Dictionary<int, HashSet<int>>();

            for (var pojas = 0; pojas < b; pojas++)
            {
                var buckets = new Dictionary<int, HashSet<int>>();
                for (var currentId = 0; currentId < simHashes.Count; currentId++)
                {
                    var hash = simHashes[currentId];
                    var val = HashToInt(pojas, hash);

                    HashSet<int> textsInBucket = null;
                    if (buckets.ContainsKey(val))
                    {
                        textsInBucket = buckets[val];   // set reference
                        foreach (var textId in textsInBucket)
                        {
                            if (!candidates.ContainsKey(currentId)) candidates[currentId] = new HashSet<int>();
                            candidates[currentId].Add(textId);

                            if (!candidates.ContainsKey(textId)) candidates[textId] = new HashSet<int>();
                            candidates[textId].Add(currentId);
                        }
                    }
                    else
                    {
                        buckets[val] = new HashSet<int>();
                        textsInBucket = buckets[val];   // set reference
                    }
                    textsInBucket.Add(currentId);
                }
            }

            return candidates;
        }
        
        static Func<int, bool, int> _simhashStep = (shi, bit) => bit ? shi + 1 : shi - 1;

        static List<bool> SimHash(string text)
        {
            var sh = new int[k];
            var inidividuals = text.Split(' ');
            foreach (var individual in inidividuals)
            {
                var individualBytes = Encoding.ASCII.GetBytes(individual);
                var hash = _md5.ComputeHash(individualBytes);
                var bits = hash.ToBits();
                sh = Enumerable.Zip(sh, bits, _simhashStep).ToArray();
            }
            var shingle = sh.Select(shi => shi >= 0);
            return shingle.ToList();
        }

        static int HashToInt(int band, List<bool> hash)
        {
            var subset = hash.Skip(band * r).Take(r).ToBytes();
            return BitConverter.ToUInt16(subset.Reverse().ToArray(), 0);
        }

        static bool IsHammingDistanceAtMost(List<bool> testShingle, List<bool> nextShingle, int distance)
        {
            if (testShingle == nextShingle)
            {
                return false;
            }
            var diff = 0;
            for (var i = 0; i < testShingle.Count; i++)
            {
                if (testShingle[i] != nextShingle[i])
                {
                    diff++;
                }
                if (diff > distance)
                {
                    return false;
                }
            }
            return diff <= distance;
        }
    }

    static class Extensions
    {
        public static IEnumerable<bool> ToBits(this IEnumerable<byte> bytes)
        {
            return bytes.SelectMany(GetBits);
        }

        private static IEnumerable<bool> GetBits(byte b)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (b & 0x80) != 0;
                b *= 2;
            }
        }

        public static IEnumerable<byte> ToBytes(this IEnumerable<bool> source)
        {
            var index = 0;
            var byteArray = new byte[source.Count() / 8];
            while (index < source.Count())
            {
                byte result = 0;
                var segement = source.Skip(index).Take(8);
                int i = 8 - segement.Count();
                index += 8;

                foreach (bool b in segement)
                {
                    // if the element is 'true' set the bit at that position
                    if (b) result |= (byte)(1 << (7 - i));

                    i++;
                }

                byteArray[index / 8 - 1] = result;
            }
            return byteArray;
        }
    }
}