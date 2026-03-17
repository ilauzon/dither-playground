using System;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaPixelSnoop;

namespace DitherPlayground.Models;

public abstract class ErrorDiffusionDitherer(
    ErrorDiffusionPattern pattern) : IDitherer
{
    protected ErrorDiffusionPattern Pattern = pattern;

    public Bitmap Dither(Bitmap bitmap)
    {
        var matrix = ReadMatrixFromBitmap(bitmap);
        for (var y = 0; y < matrix.GetLength(0); y++)
        for (var x = 0; x < matrix.GetLength(1); x++)
        {
            var oldPixel = matrix[y, x];
            var newPixel = oldPixel > 0.5 ? 1.0 : 0.0;
            var quantizationError = oldPixel - newPixel;
            matrix[y, x] = newPixel;

            for (var z = 0; z < Pattern.Matrix.GetLength(0); z++)
            for (var w = 0; w < Pattern.Matrix.GetLength(1); w++)
            {
                var multiplier = Pattern.Matrix[z, w];
                try
                {
                    matrix[y + z - Pattern.AnchorYX.Item1, x + w - Pattern.AnchorYX.Item2] +=
                        quantizationError * multiplier;
                }
                catch (IndexOutOfRangeException)
                {
                }
            }
        }

        return WriteMatrixToBitmap(matrix);
    }

    private double[,] ReadMatrixFromBitmap(Bitmap bitmap)
    {
        var matrix = new double[(int)bitmap.Size.Height, (int)bitmap.Size.Width];

        WriteableBitmap writeable;
        using (var ms = new MemoryStream())
        {
            bitmap.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            writeable = WriteableBitmap.Decode(ms);
        }

        using var snoop = new BmpPixelSnoop(writeable);
        for (var y = 0; y < matrix.GetLength(0); y++)
        for (var x = 0; x < matrix.GetLength(1); x++)
            matrix[y, x] = (double)snoop.GetPixel(x, y).B / 255;

        return matrix;
    }

    private Bitmap WriteMatrixToBitmap(double[,] matrix)
    {
        var bitmap = new WriteableBitmap(new PixelSize(
            matrix.GetLength(1),
            matrix.GetLength(0)
        ), new Vector(96, 96), PixelFormats.Bgra8888);

        using (var snoop = new BmpPixelSnoop(bitmap))
        {
            for (var y = 0; y < matrix.GetLength(0); y++)
            for (var x = 0; x < matrix.GetLength(1); x++)
            {
                var color = (byte)(matrix[y, x] * 255);
                snoop.SetPixel(x, y, new Color(255, color, color, color));
            }
        }

        return bitmap;
    }
}