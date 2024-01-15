using System;
using System.Numerics;

namespace FFT
{
    public static class Filter
    {
        public static Complex[,] ApplyPowerLawFilter(Complex[,] fftData, double a)
        {
            // Get the dimensions of the Fourier transform data array
            int width = fftData.GetLength(0);
            int height = fftData.GetLength(1);

            // Generate a power-law filter based on the specified dimensions and alpha value
            double[,] filter = GeneratePowerLawFilter(width, height, a);

            // Apply the power-law filter to the Fourier transform data
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Multiply each element in the Fourier transform data by the corresponding filter value
                    fftData[x, y] *= filter[x, y];
                }
            }

            // Return the filtered Fourier transform data
            return fftData;
        }
        
        private static double[,] GeneratePowerLawFilter(int width, int height, double a) {
            // Initialize the filter array
            double[,] filter = new double[width, height];

            // Determine the center of the filter
            int centerX = width / 2;
            int centerY = height / 2;

            // Generate the filter based on the power law
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate the squared distance from the center of the filter
                    double distanceSquared = Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2);
                    // Calculate the actual distance
                    double distance = Math.Sqrt(distanceSquared);

                    // Avoid division by zero at the center
                    if (distance == 0)
                        filter[x, y] = 1;
                    else
                        // Apply the power law to calculate the filter value
                        filter[x, y] = Math.Pow(distance, -a);
                }
            }

            // Return the generated filter
            return filter;
        }
    }
}