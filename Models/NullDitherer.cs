using Avalonia.Media.Imaging;

namespace dither_playground.Models;

public class NullDitherer : IDitherer
{
    public Bitmap Dither(Bitmap matrix)
    {
        return matrix;
    }
}