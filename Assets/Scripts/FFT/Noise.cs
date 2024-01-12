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

        public static double[,] GenerateWhiteNoiseGrid(int mapWidth, int mapDepth, float scale)
        {
            double[,] noiseMap = new double[mapWidth, mapDepth];
            
            for (int y = 0; y < mapDepth; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = UnityEngine.Random.Range(-1f,1f) / scale;
                }
            }

            return noiseMap;
        }
        
        public static double[,] GenerateTileablePerlinNoiseGrid(int mapWidth, int mapHeight, float scale)
        {
            double[,] noiseMap = new double[mapWidth, mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    // Wrap the sample coordinates
                    float sampleX = (x / scale) % mapWidth;
                    float sampleY = (y / scale) % mapHeight;

                    // Use Mathf.PerlinNoise with wrapped coordinates
                    noiseMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
                }
            }

            return noiseMap;
        }
        
        public static double[,] GenerateTileableWhiteNoiseGrid(int mapWidth, int mapDepth, float scale)
        {
            double[,] noiseMap = new double[mapWidth, mapDepth];

            // Generate internal noise
            for (int y = 0; y < mapDepth - 1; y++)
            {
                for (int x = 0; x < mapWidth - 1; x++)
                {
                    noiseMap[x, y] = UnityEngine.Random.Range(-1f, 1f) / scale;
                }
            }

            // Mirror the edges
            for (int y = 0; y < mapDepth; y++)
            {
                noiseMap[mapWidth - 1, y] = noiseMap[0, y]; // Mirror right edge to the left edge
            }

            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, mapDepth - 1] = noiseMap[x, 0]; // Mirror bottom edge to the top edge
            }

            return noiseMap;
        }
        
        public static double[,] GenerateTileableSimpleNoiseGrid(int mapWidth, int mapDepth, float scale)
        {
            double[,] noiseMap = new double[mapWidth, mapDepth];
            
            for (int y = 0; y < mapDepth; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = noise.snoise(new float2((x / scale) % mapWidth, (y / scale) % mapDepth));
                }
            }

            return noiseMap;
        }
    }
}