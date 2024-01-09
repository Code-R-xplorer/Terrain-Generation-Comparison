using System;
using System.Numerics;

namespace FFT
{
    public class FFT
    {
        // Assumes n is a power of 2
        public static void CooleyTukeyFFT(Complex[] samples)
        {
            int n = samples.Length;
            if (n <= 1) return;

            // Check if n is a power of 2
            if ((n & (n - 1)) != 0)
                throw new ArgumentException("Sample count must be a power of 2.");

            // Bit reversal of the given array
            Complex[] a = BitReversal(samples);

            // The Cooley-Tukey recursive FFT
            for (int s = 1; s <= Log2(n); s++)
            {
                int m = 1 << s; // m = 2^s
                Complex wm = Complex.Exp(new Complex(0, -2 * Math.PI / m)); // Principal m-th root of unity

                for (int k = 0; k < n; k += m)
                {
                    Complex w = 1;
                    for (int j = 0; j < m / 2; j++)
                    {
                        Complex t = w * a[k + j + m / 2];
                        Complex u = a[k + j];
                        a[k + j] = u + t;
                        a[k + j + m / 2] = u - t;
                        w *= wm;
                    }
                }
            }

            for (int i = 0; i < n; i++)
                samples[i] = a[i];
        }

        private static int Log2(int n)
        {
            int log = 0;
            while (n > 1)
            {
                log++;
                n /= 2;
            }
            return log;
        }

        private static Complex[] BitReversal(Complex[] array)
        {
            int n = array.Length;
            Complex[] a = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                int rev = ReverseBits(i, Log2(n));
                a[rev] = array[i];
            }
            return a;
        }

        private static int ReverseBits(int n, int bitsLength)
        {
            int reversed = 0;
            for (int i = 0; i < bitsLength; i++)
            {
                reversed <<= 1;
                reversed |= n & 1;
                n >>= 1;
            }
            return reversed;
        }
        
        public static void Perform2DFFT(Complex[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            // Apply FFT to each row
            Complex[] row = new Complex[cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    row[j] = data[i, j];

                CooleyTukeyFFT(row); // Apply 1D FFT

                for (int j = 0; j < cols; j++)
                    data[i, j] = row[j];
            }

            // Apply FFT to each column
            Complex[] col = new Complex[rows];
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                    col[i] = data[i, j];

                CooleyTukeyFFT(col); // Apply 1D FFT

                for (int i = 0; i < rows; i++)
                    data[i, j] = col[i];
            }
        }
        
        // Inverse 2D FFT
        public static void Perform2DIFFT(Complex[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            // Apply IFFT to each row
            Complex[] row = new Complex[cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    row[j] = data[i, j];

                CooleyTukeyIFFT(row); // Apply 1D IFFT

                for (int j = 0; j < cols; j++)
                    data[i, j] = row[j];
            }

            // Apply IFFT to each column
            Complex[] col = new Complex[rows];
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                    col[i] = data[i, j];

                CooleyTukeyIFFT(col); // Apply 1D IFFT

                for (int i = 0; i < rows; i++)
                    data[i, j] = col[i];
            }
        }
        
        public static void CooleyTukeyIFFT(Complex[] samples)
        {
            int n = samples.Length;

            // Conjugate the complex numbers
            for (int i = 0; i < n; i++)
                samples[i] = Complex.Conjugate(samples[i]);

            // Forward FFT
            CooleyTukeyFFT(samples);

            // Conjugate the complex numbers again
            for (int i = 0; i < n; i++)
                samples[i] = Complex.Conjugate(samples[i]);

            // Divide by the number of samples
            for (int i = 0; i < n; i++)
                samples[i] /= n;
        }
    }
}