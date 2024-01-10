using System;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace FFT
{
    public static class Noise
    {
        private static Random random = new();

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

        public static double[,] GenerateNoiseGrid(int mapWidth, int mapHeight, double mean, double stdDev)
        {
            double[,] noiseMap = new double[mapWidth, mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = GenerateGaussianNoise(mean, stdDev);
                }
            }
            
            return noiseMap;
        }
        
        public static double[,] GeneratePerlinNoiseGrid(int mapWidth, int mapHeight, float scale)
        {
            double[,] noiseMap = new double[mapWidth, mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float sampleX = x / scale;
                    float sampleY = y / scale;

                    noiseMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
                }
            }
            
            return noiseMap;
        }

        public static double[,] GenerateSimpleNoiseGrid(int mapWidth, int mapDepth, float scale)
        {
            double[,] noiseMap = new double[mapWidth, mapDepth];
            
            for (int y = 0; y < mapDepth; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = noise.snoise(new float2(x / scale,y / scale));
                }
            }

            return noiseMap;
        }
    }
}