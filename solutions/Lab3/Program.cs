using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3
{
    class Program
    {
        class Bucket
        {
            public int Timestamp;
            public int Size;
            public Bucket() { Timestamp = 1; Size = 1; }
        }

        static void Main(string[] args)
        {
            var N = int.Parse(Console.ReadLine());

            var buckets = new LinkedList<Bucket>();

            string line;
            while (!string.IsNullOrWhiteSpace( line = Console.ReadLine() ))
            {
                line.Trim();

                if (line.StartsWith("q"))
                {
                    var k = int.Parse(line.Split(' ')[1]);
                    var bound = buckets.Where(b => b.Timestamp <= k);
                    var first = bound.FirstOrDefault() != null ? (bound.First().Size / 2) : 0;
                    var result = first + bound.Skip(1).Select(b => b.Size).Sum();
                    
                    Console.WriteLine(result);
                }
                else
                {
                    foreach (var bit in line)
                    {
                        foreach (var b in buckets) b.Timestamp++;

                        if (buckets.Count > 0 && buckets.First.Value.Timestamp > N) buckets.RemoveFirst();

                        if (bit == '1')
                        {
                            buckets.AddLast(new Bucket());

                            var hasBucketsToMerge = buckets.Count >= 3;
                            var checkFrom = buckets.Count - 1;
                            while (hasBucketsToMerge)
                            {
                                hasBucketsToMerge = false;
                                var counter = 1;    // checkElem is the first
                                var i = buckets.Count;
                                Bucket checkElem = null;
                                foreach (var elem in buckets.Reverse())
                                {
                                    i--;
                                    if (i == checkFrom) checkElem = elem;
                                    if (i >= checkFrom) continue;

                                    if (elem.Size == checkElem.Size)
                                    {
                                        counter++;
                                        if (counter == 3)
                                        {
                                            hasBucketsToMerge = true;
                                            break;
                                        }
                                    }
                                }

                                if (hasBucketsToMerge)
                                {
                                    checkFrom -= 2;

                                    buckets.Remove(buckets.ElementAt(checkFrom));
                                    buckets.ElementAt(checkFrom).Size *= 2;

                                    hasBucketsToMerge = hasBucketsToMerge && checkFrom >= 2;
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
