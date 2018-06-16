using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4.B
{
    class Program
    {
        static int N;
        static int E;
        static List<bool> isNodeBlack = new List<bool>();
        static List<List<int>> adjacencyMarix = new List<List<int>>();
        static HashSet<int> deadEnds = new HashSet<int>();

        static void Main(string[] args)
        {
            Input();
            for (var node = 0; node < N; node++)
            {
                var blackNode = FindNearestBlack(new HashSet<int>(), new SortedSet<int> { node }, 0);
                if (blackNode.Item1 == -1)
                {
                    deadEnds.Add(node);
                }
                Console.WriteLine(blackNode.Item1 + " " + blackNode.Item2);
            }
        }

        static void Input()
        {
            var input = Console.ReadLine().Trim().Split(' ');
            N = int.Parse(input[0]);
            E = int.Parse(input[1]);
            for (var i = 0; i < N; i++)
            {
                adjacencyMarix.Add(new List<int>());
                var t = Console.ReadLine().Trim();
                isNodeBlack.Add(t == "1");
            }
            for (var i = 0; i < E; i++)
            {
                input = Console.ReadLine().Trim().Split(' ');
                var s = int.Parse(input[0]);
                var d = int.Parse(input[1]);
                adjacencyMarix[s].Add(d);
                adjacencyMarix[d].Add(s);
            }
        }

        static Tuple<int, int> FindNearestBlack(HashSet<int> visited, SortedSet<int> open, int distance)
        {
            if (distance > 10 || open.Count == 0)
            {
                return new Tuple<int, int>(-1, -1);
            }
            var next = new SortedSet<int>();
            visited.UnionWith(open);    // will visit them now
            foreach (var node in open)
            {
                if (isNodeBlack[node])
                {
                    return new Tuple<int, int>(node, distance);
                }
                next.UnionWith(adjacencyMarix[node].Where(i => !visited.Contains(i) && !deadEnds.Contains(i)));
            }
            return FindNearestBlack(visited, next, distance + 1);
        }
    }
}
