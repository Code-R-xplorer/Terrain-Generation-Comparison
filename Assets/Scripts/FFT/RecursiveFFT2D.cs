using System;
using System.Numerics;

namespace FFT
{
    public static class RecursiveFFT2D
    {

        public static void Perform2DRecFFT(int width, int height, Complex[,] data, int stride = 1)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            
            // Apply Recursive FFT to each row
            Complex[] row = new Complex[cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    row[j] = data[i, j];

                RecFFT2(height, row, stride); // Apply 1D FFT

                for (int j = 0; j < cols; j++)
                    data[i, j] = row[j];
            }
            
            // Apply Recursive FFT to each column
            Complex[] column = new Complex[rows];
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                    column[j] = data[i, j];

                RecFFT2(width, column, stride); // Apply 1D FFT

                for (int j = 0; j < rows; j++)
                    data[i, j] = column[j];
            }
        }
        
        public static void Perform2DRecIFFT(int width, int height, Complex[,] data, int stride = 1)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            
            // Apply Recursive FFT to each row
            Complex[] row = new Complex[cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    row[j] = data[i, j];

                RecIFFT2(height, row, stride); // Apply 1D IFFT

                for (int j = 0; j < cols; j++)
                    data[i, j] = row[j];
            }
            
            // Apply Recursive FFT to each column
            Complex[] column = new Complex[rows];
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                    column[j] = data[i, j];

                RecIFFT2(width, column, stride); // Apply 1D IFFT

                for (int j = 0; j < rows; j++)
                    data[i, j] = column[j];
            }
        }
        
        public static Complex[] RecFFT2(int n, Complex[] X, int stride)
        {
            Complex[] Y = new Complex[n];

            if (n == 1)
            {
                Y[0] = X[0];
            }
            else
            {
                var Y0 = RecFFT2(n / 2, X, 2 * stride);
                var Y1 = RecFFT2(n / 2, Offset(X, stride), 2 * stride);

                for (int k1 = 0; k1 < n / 2; k1++)
                {
                    Complex t = Y0[k1];
                    Complex twiddleFactor = Complex.Exp(new Complex(0, -2 * Math.PI * k1 / n));
                    Y[k1] = t + twiddleFactor * Y1[k1];
                    Y[k1 + n / 2] = t - twiddleFactor * Y1[k1];
                }
            }

            return Y;
        }
        
        private static Complex[] RecIFFT2(int n, Complex[] X, int stride)
        {
            // Conjugate the input
            for (int i = 0; i < X.Length; i += stride)
            {
                X[i] = Complex.Conjugate(X[i]);
            }

            Complex[] Y = new Complex[n];
            if (n == 1)
            {
                Y[0] = X[0];
            }
            else
            {
                var Y0 = RecIFFT2(n / 2, X, 2 * stride);
                var Y1 = RecIFFT2(n / 2, Offset(X, stride), 2 * stride);

                for (int k1 = 0; k1 < n / 2; k1++)
                {
                    Complex t = Y0[k1];
                    Complex twiddleFactor = Complex.Exp(new Complex(0, 2 * Math.PI * k1 / n));
                    Y[k1] = t + twiddleFactor * Y1[k1];
                    Y[k1 + n / 2] = t - twiddleFactor * Y1[k1];
                }
            }

            // Normalize and conjugate the output
            if (stride == 1)
            {
                for (int i = 0; i < n; i++)
                {
                    Y[i] = Complex.Conjugate(Y[i]) / n;
                }
            }

            return Y;
        }

        private static Complex[] Offset(Complex[] X, int offset)
        {
            Complex[] result = new Complex[X.Length - offset];
            Array.Copy(X, offset, result, 0, X.Length - offset);
            return result;
        }
    }
}