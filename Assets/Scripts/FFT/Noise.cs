using System;

namespace FFT
{
    public static class Noise
    {
        private static Random random = new();

        // Generates a random number with Gaussian distribution
        private static float GenerateGaussianNoise(double mean, double stdDev)
        {
            double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return (float)randNormal;
        }

        public static float[,] GenerateNoiseGrid(int mapWidth, int mapHeight, double mean, double stdDev)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = GenerateGaussianNoise(mean, stdDev);
                }
            }
            
            return noiseMap;
        }
    }
}