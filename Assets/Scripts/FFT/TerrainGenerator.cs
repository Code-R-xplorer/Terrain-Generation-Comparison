using System.Numerics;
using System;
using System.Linq;
using Mesh_Gen;
using UnityEngine;
using Random = System.Random;

namespace FFT
{
    public class TerrainGenerator : MonoBehaviour
    {
        public double mean = 0;
        public double stdDev = 1;
        public int mapWidth, mapDepth;
        public double roughness;
        public float scale;

        public void GenerateTerrain()
        {
            // Create the random noise grid
            double[,] gaussianNoiseGrid = Noise.GenerateNoiseGrid(mapWidth, mapDepth, mean, stdDev);

            // double[,] perlinNoiseGrid = Noise.GeneratePerlinNoiseGrid(mapWidth, mapDepth, scale);
            
            // double[,] simplexNoiseGrid = Noise.GenerateSimpleNoiseGrid(mapWidth, mapDepth, scale);
            
            // Convert to complex numbers to use in the FFT algorithm
            Complex[,] data = ConvertRealToComplex(gaussianNoiseGrid);
            
            // Apply Fast Fourier transform (FFT)
            // FFT.Perform2DFFT(data);
            RecursiveFFT2D.Perform2DRecFFT(mapWidth, mapDepth, data);
            
            ApplyFrequencyFilter(data, roughness);
            
            RecursiveFFT2D.Perform2DRecIFFT(mapWidth, mapDepth, data);

            float[,] noiseMap = ConvertComplexToReal(data);

            Debug.Log(noiseMap);
            
            GetComponent<MeshGenerator>().GenerateTerrain(noiseMap, mapWidth, mapDepth);
        }

        private Complex[,] ConvertRealToComplex(double[,] noiseGrid)
        {
            Complex[,] grid = new Complex[mapWidth,mapDepth];

            // float[,] noiseGrid = Noise.GenerateNoiseGrid(mapWidth, mapHeight, mean, stdDev);

            for (int y = 0; y < mapDepth; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    grid[x, y] = new Complex(noiseGrid[x, y], 0.0);
                }
            }

            return grid;
        }
        
        private float[,] ConvertComplexToReal(Complex[,] complexData)
        {
            int rows = complexData.GetLength(0);
            int cols = complexData.GetLength(1);
            float[,] realData = new float[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Take the real part of the complex number
                    realData[i, j] = Convert.ToSingle(complexData[i, j].Real);
                }
            }

            return realData;
        }
        
        private void ApplyFrequencyFilter(Complex[,] data, double r)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            Complex[,] filtered = new Complex[width, height];

            for (int u = 0; u < width; u++)
            {
                for (int v = 0; v < height; v++)
                {
                    // Compute frequency magnitude (distance from the center of the frequency domain)
                    // double uShifted = u - width / 2;
                    // double vShifted = v - height / 2;
                    // double distance = Math.Sqrt(uShifted * uShifted + vShifted * vShifted);
                    // double frequency = distance / Math.Sqrt(width * width + height * height);

                    double frequency = data[u, v].Real;
                    
                    // Avoid division by zero
                    if (frequency == 0) continue;

                    // Compute filter value
                    double filter = 1 / Math.Pow(frequency, r);

                    // Apply the filter
                    filtered[u, v] = new Complex(filter, data[u, v].Imaginary);
                }
            }
        }


    }
}