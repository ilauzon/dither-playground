namespace DitherPlayground.Models;

public class AtkinsonDitherer() : ErrorDiffusionDitherer(AtkinsonPattern)
{
    private static readonly ErrorDiffusionPattern AtkinsonPattern = new(new[,]
    {
        { 0.0, 0.0, 1.0 / 8, 1.0 / 8 },
        { 1.0 / 8, 1.0 / 8, 1.0 / 8, 0.0 },
        { 0.0, 1.0 / 8, 0.0, 0.0 }
    }, (0, 1));
}