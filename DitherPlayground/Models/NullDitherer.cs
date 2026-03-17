using Avalonia.Media.Imaging;

namespace DitherPlayground.Models;

public class NullDitherer : IDitherer
{
    public Bitmap Dither(Bitmap matrix)
    {
        return matrix;
    }
}