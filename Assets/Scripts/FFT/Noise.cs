using System;
using Random = System.Random;

namespace FFT
{
    public static class Noise
    {
        private static Random random = new();

        public static void SetRandomSeed(int seed)
        {
            random = new Random(seed);
        }

        // Generates a random number with Gaussian distribution
        public static double GenerateGaussianNoise(double mean, double stdDev)
        {
            double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }

        public static double[,] GenerateGaussianNoiseGrid(int mapSize ,double mean, double stdDev)
        {
            
            double[,] noiseMap = new double[mapSize, mapSize];

            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    noiseMap[x, y] = GenerateGaussianNoise(mean, stdDev);
                }
            }
            
            return noiseMap;
        }
        
        public static double[,] GenerateTileableWhiteNoiseGrid(int mapSize, float scale)
        {
            double[,] noiseMap = new double[mapSize, mapSize];

            // Generate internal noise
            for (int y = 0; y < mapSize - 1; y++)
            {
                for (int x = 0; x < mapSize - 1; x++)
                {
                    noiseMap[x, y] = UnityEngine.Random.Range(-1f, 1f) / scale;
                }
            }

            // Mirror the edges
            for (int y = 0; y < mapSize; y++)
            {
                noiseMap[mapSize - 1, y] = noiseMap[0, y]; // Mirror right edge to the left edge
            }

            for (int x = 0; x < mapSize; x++)
            {
                noiseMap[x, mapSize - 1] = noiseMap[x, 0]; // Mirror bottom edge to the top edge
            }

            return noiseMap;
        }
    }
}