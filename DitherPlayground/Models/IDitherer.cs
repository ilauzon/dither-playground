using Avalonia.Media.Imaging;

namespace DitherPlayground.Models;

public interface IDitherer
{
    public Bitmap Dither(Bitmap matrix);
}