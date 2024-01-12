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
        // public double mean = 0;
        // public double stdDev = 1;
        public int mapWidth, mapDepth;
        public double roughness;
        public float scale;

        public float scaleX = 1, scaleZ = 1;

        [Header("Filters")] 
        public double sigma;
        public double cutoffFrequencyLow;
        public double cutoffFrequencyHigh;
        public int kernelSize;

        // public bool useGaussian, usePerlin, useSimplex, useWhite;

        public void GenerateTerrain()
        {

            // double[,] noiseGrid = new double[mapWidth, mapDepth];
            
            double[,] noiseGrid = Noise.GenerateTileableWhiteNoiseGrid(mapWidth, mapDepth, scale);
            // Create the random noise grid
            // double[,] gaussianNoiseGrid = Noise.GenerateNoiseGrid(mapWidth, mapDepth, mean, stdDev);

            // double[,] perlinNoiseGrid = Noise.GeneratePerlinNoiseGrid(mapWidth, mapDepth, scale);
            
            // double[,] simplexNoiseGrid = Noise.GenerateSimpleNoiseGrid(mapWidth, mapDepth, scale);

            // double[,] whiteNoiseGrid = Noise.GenerateWhiteNoiseGrid(mapWidth, mapDepth);
            
            // Convert to complex numbers to use in the FFT algorithm
            Complex[,] data = ConvertRealToComplex(noiseGrid);
            
            // Apply Fast Fourier transform (FFT)
            FFT.Perform2DFFT(data);
            // RecursiveFFT2D.Perform2DRecFFT(mapWidth, mapDepth, data);
            
            // data = ApplyFrequencyFilter(data, roughness);
            
            ApplyGaussianBlur2(data, sigma);
            
            var lowData = ApplyLowPassFilter2(data, kernelSize);
            
            var highData = ApplyHighPassFilter2(data, cutoffFrequencyHigh);

            int width = data.GetLength(0);
            int height = data.GetLength(1);
            
            Complex[,] combinedData = new Complex[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var real = (data[x, y].Real + lowData[x, y].Real + highData[x, y].Real) / 3;
                    var imaginary = (data[x, y].Imaginary + lowData[x, y].Imaginary + highData[x, y].Imaginary) / 3;
                    combinedData[x, y] = new Complex(real, imaginary);
                }
            }
            
            // RecursiveFFT2D.Perform2DRecIFFT(mapWidth, mapDepth, data);
            FFT.Perform2DIFFT(combinedData);

            float[,] noiseMap = ConvertComplexToReal(combinedData);

            // Debug.Log(noiseMap);
            
            GetComponent<MeshGenerator>().GenerateTerrain(noiseMap, mapWidth, mapDepth, scaleX, scaleZ);
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
        
        private Complex[,] ApplyFrequencyFilter(Complex[,] data, double r)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            Complex[,] filtered = new Complex[width, height];

            for (int u = 0; u < width; u++)
            {
                for (int v = 0; v < height; v++)
                {
                    // Compute frequency magnitude (distance from the center of the frequency domain)
                    double uShifted = u - width / 2;
                    double vShifted = v - height / 2;
                    double distance = Math.Sqrt(uShifted * uShifted + vShifted * vShifted);
                    double frequency = distance / Math.Sqrt(width * width + height * height);

                    // double frequency = data[u, v].Real;
                    
                    // Avoid division by zero
                    if (frequency == 0) continue;

                    // Compute filter value
                    double filter = 1 / Math.Pow(frequency, r);
                    
                    // double b0 = 0, b1 = 0, b2 = 0, b3 = 0, b4 = 0, b5 = 0, b6 = 0, pink = 0;
                    // b0 = 0.99886 * b0 + frequency * 0.0555179;
                    // b1 = 0.99332 * b1 + frequency * 0.0750759;
                    // b2 = 0.96900 * b2 + frequency * 0.1538520;
                    // b3 = 0.86650 * b3 + frequency * 0.3104856;
                    // b4 = 0.55000 * b4 + frequency * 0.5329522;
                    // b5 = -0.7616 * b5 - frequency * 0.0168980;
                    // b6 = frequency * 0.115926;
                    // pink = b0 + b1 + b2 + b3 + b4 + b5 + b6 + frequency * 0.5362;
                    

                    // Apply the filter
                    filtered[u, v] = new Complex(filter, data[u, v].Imaginary);
                }
            }

            return filtered;    
        }
        
        public Complex[,] ApplyLowPassFilter2(Complex[,] data, int kernelSize)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            Complex[,] result = new Complex[width, height];
            int k = kernelSize / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sum = 0.0;
                    int count = 0;

                    for (int dy = -k; dy <= k; dy++)
                    {
                        for (int dx = -k; dx <= k; dx++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                sum += data[nx, ny].Real;
                                count++;
                            }
                        }
                    }

                    result[x, y] = new Complex(sum / count, data[x,y].Imaginary);
                }
            }

            return result;
        }
        
        public Complex[,] ApplyHighPassFilter2(Complex[,] data, double cutoff)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            
            Complex[,] lowPass = ApplyLowPassFilter(data, 3);
            Complex[,] result = new Complex[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[x, y] = data[x, y] - lowPass[x, y];

                    if (Math.Abs(result[x, y].Real) < cutoff)
                    {
                        result[x, y] = new Complex(0, 0);
                    }
                }
            }

            return result;
        }


        
        public Complex[,] ApplyLowPassFilter(Complex[,] data, double cutoffFrequency)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            
            Complex[,] filtered = new Complex[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double frequency = Math.Sqrt(x * x + y * y);
                    if (frequency > cutoffFrequency)
                    {
                        filtered[x, y] = Complex.Zero;
                    }
                }
            }

            return filtered;
        }

        public Complex[,] ApplyHighPassFilter(Complex[,] data, double cutoffFrequency)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            
            Complex[,] filtered = new Complex[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double frequency = Math.Sqrt(x * x + y * y);
                    if (frequency < cutoffFrequency)
                    {
                        filtered[x, y] = Complex.Zero;
                    }
                }
            }

            return filtered;
        }
        
        public static void ApplyGaussianBlurFilter(Complex[,] data, double sigma)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            double twoSigmaSquared = 2 * sigma * sigma;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double distanceSquared = x * x + y * y;
                    double factor = Math.Exp(-distanceSquared / twoSigmaSquared);
                    data[x, y] *= factor;
                }
            }
        }
        
        public Complex[,] ApplyGaussianBlur2(Complex[,] data, double sigma)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            
            int kernelS = (int)(6 * sigma + 1);
            int k = kernelS / 2;
            double[,] kernel = GenerateGaussianKernel(kernelS, sigma);
            Complex[,] result = new Complex[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sum = 0.0;
                    double weightSum = 0.0;

                    for (int dy = -k; dy <= k; dy++)
                    {
                        for (int dx = -k; dx <= k; dx++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                double weight = kernel[dx + k, dy + k];
                                sum += data[nx, ny].Real * weight;
                                weightSum += weight;
                            }
                        }
                    }

                    result[x, y] = new Complex( sum / weightSum, data[x,y].Imaginary);
                }
            }

            return result;
        }

        private double[,] GenerateGaussianKernel(int size, double sigma)
        {
            double[,] kernel = new double[size, size];
            double sum = 0.0;
            int k = size / 2;

            for (int y = -k; y <= k; y++)
            {
                for (int x = -k; x <= k; x++)
                {
                    kernel[x + k, y + k] = Math.Exp(-(x * x + y * y) / (2 * sigma * sigma));
                    sum += kernel[x + k, y + k];
                }
            }

            // Normalize the kernel
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    kernel[x, y] /= sum;
                }
            }

            return kernel;
        }





    }
}