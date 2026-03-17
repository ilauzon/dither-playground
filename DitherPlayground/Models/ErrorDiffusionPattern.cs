namespace DitherPlayground.Models;

public class ErrorDiffusionPattern(double[,] matrix, (int, int) anchorYX)
{
    public readonly (int, int) AnchorYX = anchorYX;
    public readonly double[,] Matrix = matrix;
}