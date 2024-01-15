using System;
using System.Numerics;

namespace FFT
{
    public static class Utils
    {
        public static Complex[,] Shift(Complex[,] data)
        {
            // Get the dimensions of the input data array
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            // Create a new array of the same dimensions to hold the shifted data
            Complex[,] shiftedData = new Complex[width, height];

            // Loop over each element in the input data array
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate the new x-coordinate by shifting it to the right by half the width
                    // and wrapping around using the modulo operator
                    int newX = (x + width / 2) % width;

                    // Calculate the new y-coordinate by shifting it down by half the height
                    // and wrapping around using the modulo operator
                    int newY = (y + height / 2) % height;

                    // Assign the value from the original data array to the new shifted position
                    // in the shiftedData array
                    shiftedData[newX, newY] = data[x, y];
                }
            }

            // Return the shifted data array
            return shiftedData;
        }

        public static Complex[,] ConvertRealToComplex(double[,] noiseGrid, int mapSize)
        {
            // Assuming 'mapSize' is a predefined size of the grid.
            Complex[,] grid = new Complex[mapSize, mapSize];

            // Loop through each element in the noise grid
            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    // Convert each real number in the noise grid to a complex number
                    // The real part is the value from the noise grid, and the imaginary part is set to 0.0
                    grid[x, y] = new Complex(noiseGrid[x, y], 0.0);
                }
            }

            // Return the complex number grid
            return grid;
        }

        public static float[,] ConvertComplexToReal(Complex[,] complexData)
        {
            // Get the number of rows and columns from the complex data array
            int rows = complexData.GetLength(0);
            int cols = complexData.GetLength(1);

            // Initialize an array to store the real part of the complex data
            float[,] realData = new float[rows, cols];

            // Loop through each element in the complex data array
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Extract the real part of each complex number and convert it to float
                    realData[i, j] = Convert.ToSingle(complexData[i, j].Real);
                }
            }

            // Return the array containing only the real parts of the complex data
            return realData;
        }
        
        
        public static int Log2(int n)
        {
            int log = 0;
            while (n > 1)
            {
                log++;
                n /= 2;
            }
            return log;
        }

        public static Complex[] BitReversal(Complex[] array)
        {
            // n is the total number of elements in the array
            int n = array.Length;

            // Create a new array to hold the bit-reversed elements
            Complex[] a = new Complex[n];

            for (int i = 0; i < n; i++)
            {
                // Calculate the bit-reversed index for the current element
                int rev = ReverseBits(i, Log2(n));

                // Place the element in the bit-reversed position
                a[rev] = array[i];
            }

            // Return the re-ordered array
            return a;
        }

        private static int ReverseBits(int n, int bitsLength)
        {
            // Initialize the reversed number
            int reversed = 0;

            for (int i = 0; i < bitsLength; i++)
            {
                // Shift the reversed number to make room for the next bit
                reversed <<= 1;

                // Add the least significant bit of n to reversed
                reversed |= n & 1;

                // Shift n to the right, discarding its least significant bit
                n >>= 1;
            }

            // Return the bit-reversed integer
            return reversed;
        }
        
        
        public enum NoiseTypes
        {
            Gaussian,
            White
        }
    }
}