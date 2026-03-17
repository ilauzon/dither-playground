namespace DitherPlayground.Models;

public class CustomErrorDiffusionDitherer(ErrorDiffusionPattern pattern)
    : ErrorDiffusionDitherer(pattern)
{
    public void ChangePattern(ErrorDiffusionPattern newPattern)
    {
        Pattern = newPattern;
    }
}