namespace dither_playground.Models;

public class FloydSteinbergDitherer() : ErrorDiffusionDitherer(Pattern)
{
    private static readonly ErrorDiffusionPattern Pattern = new(new[,]
    {
        { 0.0, 0.0, 7.0 / 16 },
        { 3.0 / 16, 5.0 / 16, 1.0 / 16 }
    }, (0, 1));
}