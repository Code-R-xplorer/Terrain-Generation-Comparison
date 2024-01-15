using System;
using System.Numerics;

namespace FFT
{
    public class FFT
    {
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
        
        
        private static void CooleyTukeyFFT(Complex[] samples) {
            // n is the total number of samples in the array
            int n = samples.Length;

            // Return immediately if there's only one sample, as no FFT is needed
            if (n <= 1) return;
    
            // Check if n is a power of 2, which is required for the Cooley-Tukey FFT
            if ((n & (n - 1)) != 0) throw new ArgumentException("Sample count must be a power of 2.");
    
            // Apply the bit reversal method to the samples, reordering them
            Complex[] a = Utils.BitReversal(samples);
    
            // Main FFT computation loop
            for (int s = 1; s <= Utils.Log2(n); s++)
            {
                int m = 1 << s; // m is set to 2^s, used for segmenting the sample array
                Complex wm = Complex.Exp(new Complex(0, -2 * Math.PI / m)); // Twiddle factor

                for (int k = 0; k < n; k += m)
                {
                    Complex w = 1;
                    for (int j = 0; j < m / 2; j++)
                    {
                        // Compute the FFT for the pair of elements
                        Complex t = w * a[k + j + m / 2]; // Twiddle multiplication
                        Complex u = a[k + j];
                
                        // Update the elements with their new values
                        a[k + j] = u + t;
                        a[k + j + m / 2] = u - t;
                
                        // Update the twiddle factor for next iteration
                        w *= wm;
                    }
                }
            }

            // Copy the computed FFT values back to the original samples array
            for (int i = 0; i < n; i++)
                samples[i] = a[i];
        }
        
        // Inverse 2D FFT
        public static void Perform2DIFFT(Complex[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

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
        }

        private static void CooleyTukeyIFFT(Complex[] samples)
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