using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lab4.A
{
    class Program
    {
        static int N;
        static double beta;
        static List<List<int>> adjacencyMarix = new List<List<int>>();

        static int Q;
        static List<Tuple<int, int>> queries = new List<Tuple<int, int>>();

        static int maxIterations = 0;
        static Dictionary<int, List<double>> rankPerIteration = new Dictionary<int, List<double>>();
        
        static void Main(string[] args)
        {
            Input();
            RunIterations();
            Output();
        }

        static void Input()
        {
            var input = Console.ReadLine().Trim().Split(' ');
            N = int.Parse(input[0]);
            beta = double.Parse(input[1], CultureInfo.InvariantCulture);
            for (int i = 0; i < N; i++)
            {
                var nodes = Console.ReadLine().Trim().Split(' ').Select(int.Parse).ToList();
                adjacencyMarix.Add(nodes);
            }

            Q = int.Parse(Console.ReadLine().Trim());
            for (int i = 0; i < Q; i++)
            {
                input = Console.ReadLine().Trim().Split(' ');
                var n = int.Parse(input[0]);
                var t = int.Parse(input[1]);
                if (t > maxIterations) maxIterations = t;
                queries.Add(new Tuple<int, int>(n, t));
            }
        }

        static void RunIterations()
        {
            var initialRank = (1 - beta) / N;
            rankPerIteration[0] = Enumerable.Repeat(1.0 / N, N).ToList();

            for (int k = 0; k < maxIterations; k++)
            {
                rankPerIteration[k + 1] = Enumerable.Repeat(initialRank, N).ToList();

                for (int i = 0; i < N; i++)
                {
                    var value = beta * rankPerIteration[k][i] / adjacencyMarix[i].Count;
                    foreach (var j in adjacencyMarix[i])
                    {
                        rankPerIteration[k + 1][j] += value;
                    }
                }
            }
        }

        static void Output()
        {
            foreach (var query in queries)
            {
                var output = rankPerIteration[query.Item2][query.Item1];
                Console.WriteLine(output.ToString("F10"));
            }
        }
    }
    
}
