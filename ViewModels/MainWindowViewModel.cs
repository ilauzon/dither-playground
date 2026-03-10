using System;
using System.Linq;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using dither_playground.Models;

namespace dither_playground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public static Algorithm[] Algorithms { get; } =
    [
        new("None", new NullDitherer()),
        new("Floyd-Steinberg", new FloydSteinbergDitherer()),
        new("Atkinson", new AtkinsonDitherer())
    ];

    private static readonly Algorithm DefaultAlgorithm = Algorithms[1];

    private static readonly Bitmap DefaultSourceImage =
        new(AssetLoader.Open(new Uri("avares://dither-playground/Assets/david.png")));

    [ObservableProperty] private Bitmap _ditheredBitmap = DefaultAlgorithm.Ditherer.Dither(DefaultSourceImage);

    [ObservableProperty] private string _selectedAlgorithm = DefaultAlgorithm.DisplayName;

    [ObservableProperty] private Bitmap _sourceBitmap = DefaultSourceImage;

    [RelayCommand]
    private void ChangeSelectedAlgorithm(Algorithm algorithm)
    {
        if (Algorithms.FirstOrDefault(a => a == algorithm, null) == null) return;
        SelectedAlgorithm = algorithm.DisplayName;
        DitheredBitmap = algorithm.Ditherer.Dither(SourceBitmap);
    }

    private void ChangeSourceImage()
    {
    }

    private Bitmap LoadImage(string uri)
    {
        return new Bitmap(AssetLoader.Open(new Uri(uri)));
    }
}

public class Algorithm(string displayName, IDitherer ditherer)
{
    public string DisplayName { get; } = displayName;
    public IDitherer Ditherer { get; } = ditherer;
}