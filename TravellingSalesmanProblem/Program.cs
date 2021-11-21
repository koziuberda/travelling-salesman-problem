using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TravellingSalesmanProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose the way to input data: ");
            Console.WriteLine("0 - randomly generate data");
            Console.WriteLine("1 - input weight matrix from .json file");

            var c = Convert.ToChar(Console.ReadLine() ?? string.Empty);
            
            int[,] matrix;

            switch (c)
            {
                case '0':
                {
                    matrix = RandomWeightMatrix();

                    var json = JsonConvert.SerializeObject(matrix);
                    File.WriteAllText("randomMatrix.json", json);
                
                    Console.WriteLine("The matrix is saved as randomMatrix.json");
                    break;
                }
                case '1':
                {
                    Console.WriteLine("Enter file path: ");
                    string path = Console.ReadLine();
                    var json = File.ReadAllText(path ?? throw new InvalidOperationException());
                    matrix = JsonConvert.DeserializeObject<int[,]>(json);
                    break;
                }
                default:
                    Console.WriteLine("Incorrect character");
                    return;
            }

            var citiesData = new CitiesData(matrix);

            var hive = new BeeColony(citiesData, 150, 20, 30, 
                20000, 30);
            var listSolution = hive.Solve(out var dist);
            
            Console.WriteLine($"Solution distance: {dist}");
            
            StringBuilder way = new StringBuilder();
            foreach (int cityNumber in listSolution)
            {
                way.Append("-->" + cityNumber);
            }
            
            Console.WriteLine(way);
        }

        private static int[,] RandomWeightMatrix()
        {
            var random = new Random();
            var matrix = new int[300, 300];

            for (int i = 0; i < 300; i++)
            {
                for (int j = 0; j < 300; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                        continue;
                    }

                    matrix[i, j] = random.Next(5, 151);
                }
            }

            return matrix;
        }
    }
}