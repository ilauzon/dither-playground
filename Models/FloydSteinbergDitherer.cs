namespace dither_playground.Models;

public class FloydSteinbergDitherer() : ErrorDiffusionDitherer(FloydSteinbergPattern)
{
    private static readonly ErrorDiffusionPattern FloydSteinbergPattern = new(new[,]
    {
        { 0.0, 0.0, 7.0 / 16 },
        { 3.0 / 16, 5.0 / 16, 1.0 / 16 }
    }, (0, 1));
}