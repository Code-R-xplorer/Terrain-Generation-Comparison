using System.Numerics;
using System;
using Mesh_Gen;
using UnityEngine;
using NoiseTypes = FFT.Utils.NoiseTypes;

namespace FFT
{
    [RequireComponent(typeof(MeshGenerator))]
    public class TerrainGenerator : MonoBehaviour
    {
        [Header("Noise Generation")]
        public NoiseTypes noiseType;
        [Header("Gaussian Noise")]
        public double mean;
        public double stdDev = 1;
        
        [Header("White Noise")]
        public float scale;
        
        [Space]
        [Header("Terrain Size")]
        [Tooltip("Must be a power of 2!")]
        public int mapSize;
        public float mapScale = 1;

        [Space]
        [Header("Power Law Filter")]
        [Range(0f,3f)]
        public double alpha;

        private MeshGenerator _meshGenerator;

        private void Start()
        {
            _meshGenerator = GetComponent<MeshGenerator>();
        }

        public void GenerateTerrain()
        {
            // Noise.SetRandomSeed(100);
            
            // Generate the noise grid based on user input
            double[,] noiseGrid = noiseType switch
            {
                NoiseTypes.Gaussian => Noise.GenerateGaussianNoiseGrid(mapSize, mean, stdDev),
                NoiseTypes.White => Noise.GenerateTileableWhiteNoiseGrid(mapSize, scale),
                _ => throw new ArgumentOutOfRangeException()
            };

            // Convert to complex numbers to use in the FFT algorithm
            Complex[,] data = Utils.ConvertRealToComplex(noiseGrid, mapSize);
            
            // Apply Fast Fourier transform (FFT)
            FFT.Perform2DFFT(data);

            // Shift the data
            var shiftData = Utils.Shift(data);

            // Apply power law filter
            var filteredData = Filter.ApplyPowerLawFilter(shiftData, alpha);
            
            // Reverse the shift
            var unShiftedData = Utils.Shift(filteredData);
            
            // Apply Inverse Fast Fourier Transform (IFFT)
            FFT.Perform2DIFFT(unShiftedData);
            
            // Convert the filtered data back into real numbers to be used in the mesh generation
            float[,] noiseMap = Utils.ConvertComplexToReal(unShiftedData);
            
            // Generate the terrain mesh
            _meshGenerator.GenerateTerrain(noiseMap,mapSize, mapScale);
        }

    }
}