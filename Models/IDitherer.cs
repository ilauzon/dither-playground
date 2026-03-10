using Avalonia.Media.Imaging;

namespace dither_playground.Models;

public interface IDitherer
{
    public Bitmap Dither(Bitmap matrix);
}