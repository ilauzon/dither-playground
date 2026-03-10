namespace dither_playground.Models;

public class AtkinsonDitherer() : ErrorDiffusionDitherer(Pattern)
{
    private static readonly ErrorDiffusionPattern Pattern = new(new[,]
    {
        { 0.0, 0.0, 1.0 / 8, 1.0 / 8 },
        { 1.0 / 8, 1.0 / 8, 1.0 / 8, 0.0 },
        { 0.0, 1.0 / 8, 0.0, 0.0 }
    }, (0, 1));
}