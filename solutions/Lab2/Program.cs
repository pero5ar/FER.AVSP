using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            PCY();
        }

        static void PCY()
        {
            var N = int.Parse(Console.ReadLine());
            var s = double.Parse(Console.ReadLine());
            var b = int.Parse(Console.ReadLine());

            var prag = Convert.ToInt32(Math.Floor(s * N));

            var baskets = new List<List<int>>(N);
            var itemCounts = new Dictionary<int, int>();
            
            // 1st pass and read
            for (var i = 0; i < N; i++)
            {
                var basketItems = Console.ReadLine().Trim().Split(' ').Select(str => int.Parse(str)).ToList();
                foreach (var item in basketItems)
                {
                    if (itemCounts.ContainsKey(item))
                    {
                        itemCounts[item]++;
                        continue;
                    }
                    itemCounts.Add(item, 1);
                }
                baskets.Add(basketItems);
            }

            var m = itemCounts.Values.Where(count => count >= prag).Count();
            var A = m * (m - 1) / 2;

            var pretinci = Enumerable.Repeat(0, b).ToList();
            //var pairsToCheck = new Dictionary<int, List<long>>();

            // 2nd pass
            foreach (var basket in baskets)
            {
                for (var i = 0; i < basket.Count - 1; i++)
                {
                    for (var j = i + 1; j < basket.Count; j++)
                    {
                        var item_i = basket[i];
                        var item_j = basket[j];
                        if (itemCounts[item_i] >= prag && itemCounts[item_j] >= prag)
                        {
                            var k = ((item_i * itemCounts.Count) + item_j) % b;
                            pretinci[k]++;

                            //long x = item_i;
                            //long y = item_j;
                            //var key = (x + y) * (x + y + 1) / 2 + y;   // Cantor pairing function
                            //if (!pairsToCheck.ContainsKey(k))
                            //{
                            //    pairsToCheck[k] = new List<long>();
                            //}
                            //pairsToCheck[k].Add(key);
                        }
                    }
                }
            }

            // 3rd pass
            var pairs = new Dictionary<long, int>();

            foreach (var basket in baskets)
            {
                for (var i = 0; i < basket.Count - 1; i++)
                {
                    for (var j = i + 1; j < basket.Count; j++)
                    {
                        var item_i = basket[i];
                        var item_j = basket[j];
                        if (itemCounts[item_i] >= prag && itemCounts[item_j] >= prag)
                        {
                            var k = ((item_i * itemCounts.Count) + item_j) % b;
                            if (pretinci[k] >= prag)
                            {
                                long x = item_i;
                                long y = item_j;
                                var key = (x + y) * (x + y + 1) / 2 + y;   // Cantor pairing function
                                if (pairs.ContainsKey(key))
                                {
                                    pairs[key]++;
                                    continue;
                                }
                                pairs.Add(key, 1);
                            }
                        }
                    }
                }
            }

            //foreach (var check in pairsToCheck)
            //{
            //    if (pretinci[check.Key] >= prag)
            //    {
            //        var keys = check.Value;
            //        foreach (var key in keys)
            //        {
            //            if (pairs.ContainsKey(key))
            //            {
            //                pairs[key]++;
            //                continue;
            //            }
            //            pairs.Add(key, 1);
            //        }
            //    }
            //}

            var P = pairs.Count();

            // output
            Console.WriteLine(A);
            Console.WriteLine(P);
            var pairsCounts = pairs.Values.OrderByDescending(count => count);
            foreach (var count in pairsCounts)
            {
                Console.WriteLine(count);
            }
        }
    }
}
