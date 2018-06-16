using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab5
{
    class Program
    {
        const int ITEM_ITEM = 0;
        const int USER_USER = 0;
        static int N, M;
        static List<List<int?>> userItemMatrix = new List<List<int?>>();
        static List<List<int?>> userItemMatrixTransposed = new List<List<int?>>();
        static List<double> itemAverages;
        static List<double> userAverages;
        static List<List<double?>> userItemMatrixNormalized = new List<List<double?>>();
        static List<List<double?>> userItemMatrixTransposedNormalized = new List<List<double?>>();
        static List<double> itemSquareSums;
        static List<double> userSquareSums;

        static void Main(string[] args)
        {
            var dimensions = Console.ReadLine().Trim().Split(' ').Select(int.Parse).ToList();
            N = dimensions[0];
            M = dimensions[1];
            
            // matrix:
            for (var i = 0; i < N; i++)
            {
                userItemMatrix.Add(Console.ReadLine().Trim().Split(' ')
                    .Select(x => x == "X" ? null : int.Parse(x) as int?).ToList());
            }
            // transposed matrix:
            for (var j = 0; j < M; j++)
            {
                userItemMatrixTransposed.Add(new List<int?>(N));
                for (var i = 0; i < N; i++) userItemMatrixTransposed[j].Add(userItemMatrix[i][j]);
            }

            // compute average, normalize and square sum:
            itemAverages = userItemMatrix.Select(row => (double)row.Average()).ToList();
            for (var i = 0; i < N; i++)
            {
                userItemMatrixNormalized.Add(userItemMatrix[i]
                    .Select(x => x == null ? null : x - itemAverages[i]).ToList());
            }
            itemSquareSums = userItemMatrixNormalized
                .Select(row => Math.Sqrt((double)row.Where(x => x != null).Select(x => x * x).Sum()))
                .ToList();

            userAverages = userItemMatrixTransposed.Select(row => (double)row.Average()).ToList();
            for (var j = 0; j < M; j++)
            {
                userItemMatrixTransposedNormalized.Add(userItemMatrixTransposed[j]
                   .Select(x => x == null ? null : x - userAverages[j]).ToList());
            }
            userSquareSums = userItemMatrixTransposedNormalized
                .Select(row => Math.Sqrt((double)row.Where(x => x != null).Select(x => x * x).Sum()))
                .ToList();

            // queries:
            var Q = int.Parse(Console.ReadLine());
            for (var q = 0; q < Q; q++)
            {
                var query = Console.ReadLine().Trim().Split(' ').Select(int.Parse).ToList();
                var I = query[0] - 1;
                var J = query[1] - 1;
                var T = query[2];
                var K = query[3];

                var isItemItem = T == ITEM_ITEM;
                var matrix = isItemItem ? userItemMatrix : userItemMatrixTransposed;
                var matrixNormalized = isItemItem ? userItemMatrixNormalized : userItemMatrixTransposedNormalized;
                var squareSums = isItemItem ? itemSquareSums : userSquareSums;
                var itemPosition = isItemItem ? I : J;
                var userPosition = isItemItem ? J : I;

                var results = new Dictionary<int, Tuple<double, int?>>(); // K: itemId, V: (result, grade)
                results[itemPosition] = new Tuple<double, int?>(1.0, matrix[itemPosition][userPosition]);

                var ownerRating = squareSums[itemPosition];
                for (var i = 0; i < matrixNormalized.Count; i++)
                {
                    if (i == itemPosition) continue;
                    var otherRating = squareSums[i];
                    var sum = (double)Enumerable.Zip(
                            matrixNormalized[itemPosition], 
                            matrixNormalized[i], 
                            (owner, other) => owner * other)
                        .Sum();
                    var result = sum / (ownerRating * otherRating);
                    var grade = matrix[i][userPosition];
                    results[i] = new Tuple<double, int?>(result, grade);
                }

                var similarites = results
                    .Where(entry => entry.Key != itemPosition && entry.Value.Item2 != null && entry.Value.Item1 > 0)
                    .Select(entry => entry.Value)
                    .OrderByDescending(value => value.Item1)
                    .Take(K);

                var similarityResult = similarites.Select(value => value.Item1).Sum();
                var similatityResultGrade = similarites.Select(value => value.Item1 * (int)value.Item2).Sum();
                var recomendation = Math.Round(similatityResultGrade / similarityResult, 3, MidpointRounding.AwayFromZero);
                Console.WriteLine(String.Format("{0:0.000}", recomendation));
            }
        }
    }
}
