using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using dither_playground.Models;
using dither_playground.Services;
using dither_playground.Views;

namespace dither_playground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDialogParticipant
{
    private static readonly Bitmap DefaultSourceImage =
        new(AssetLoader.Open(new Uri("avares://dither-playground/Assets/david.png")));

    public ObservableCollection<Algorithm> Algorithms { get; } =
    [
        new("None", new NullDitherer()),
        new("Floyd-Steinberg", new FloydSteinbergDitherer()),
        new("Atkinson", new AtkinsonDitherer())
    ];

    [ObservableProperty] private CustomErrorDiffusionDitherer _customDitherer;
    [ObservableProperty] private Bitmap _ditheredBitmap;
    [ObservableProperty] private Algorithm _selectedAlgorithm;
    [ObservableProperty] private Bitmap _sourceBitmap;
    [ObservableProperty] private int _cellRows;
    [ObservableProperty] private int _cellColumns;
    [ObservableProperty] private int _anchorX;
    [ObservableProperty] private int _anchorY;
    [ObservableProperty] private bool _customDithererSelected;

    public ObservableCollection<CellViewModel> Cells { get; } = [];

    public MainWindowViewModel()
    {
        var defaultAlgorithm = Algorithms[0];
        DitheredBitmap = defaultAlgorithm.Ditherer.Dither(DefaultSourceImage);
        SelectedAlgorithm = defaultAlgorithm;
        SourceBitmap = DefaultSourceImage;

        List<CellViewModel> startingCells =
        [
            new() { Value = 0 }, new() { Value = 0, IsAnchor = true }, new() { Value = 0.1 },
            new() { Value = 0.1 }, new() { Value = 0.1 }, new() { Value = 0.1 }
        ];
        startingCells.ForEach(Cells.Add);
        CellRows = 2;
        CellColumns = 3;
        AnchorX = 1;
        AnchorY = 0;

        var customDitherPattern = GetCustomPattern();
        CustomDitherer = new CustomErrorDiffusionDitherer(customDitherPattern);
        Algorithms.Add(new Algorithm("Custom", CustomDitherer));

        foreach (var cellViewModel in Cells) cellViewModel.PropertyChanged += CustomDithererPatternChanged;
    }

    [RelayCommand]
    private void ChangeSelectedAlgorithm(Algorithm algorithm)
    {
        SelectedAlgorithm = algorithm;
        DitherSourceWithSelectedAlgorithm();
        CustomDithererSelected = SelectedAlgorithm.Ditherer == CustomDitherer;
    }

    [RelayCommand]
    private void AddRowToCustomMatrix()
    {
        for (var _ = 0; _ < CellColumns; _++) Cells.Add(new CellViewModel(CustomDithererPatternChanged));
        CellRows += 1;
        ChangeCustomDitherer(GetCustomPattern());
    }

    [RelayCommand]
    private void RemoveRowFromCustomMatrix()
    {
        if (CellRows <= 1) return;
        CellRows -= 1;
        for (var _ = 0; _ < CellColumns; _++) Cells.RemoveAt(Cells.Count - 1);
        ChangeCustomDitherer(GetCustomPattern());
    }

    [RelayCommand]
    private void AddColumnToCustomMatrix()
    {
        for (var i = CellRows; i > 0; i--)
            Cells.Insert(CellColumns * i, new CellViewModel(CustomDithererPatternChanged));
        CellColumns += 1;
        ChangeCustomDitherer(GetCustomPattern());
    }

    [RelayCommand]
    private void RemoveColumnFromCustomMatrix()
    {
        if (CellColumns <= 1) return;
        CellColumns -= 1;
        for (var i = CellRows; i > 0; i--) Cells.RemoveAt((CellColumns + 1) * i - 1);
        ChangeCustomDitherer(GetCustomPattern());
    }

    [RelayCommand]
    private void ChangeAnchor(CellViewModel newAnchor)
    {
        var cellIndex = Cells.IndexOf(newAnchor);
        if (cellIndex == -1) return;

        var oldAnchor = Cells[AnchorY * CellColumns + AnchorX];
        oldAnchor.IsAnchor = false;
        AnchorY = cellIndex / CellColumns;
        AnchorX = cellIndex % CellColumns;
        newAnchor.IsAnchor = true;
        ChangeCustomDitherer(GetCustomPattern());
    }

    [RelayCommand]
    private async Task UploadImage()
    {
        var selectedFiles = await this.OpenFileDialogAsync("Choose Image", false);

        if (selectedFiles == null || selectedFiles.Count == 0) return;

        var selectedFile = selectedFiles[0];
        await using var stream = await selectedFile.OpenReadAsync();
        SourceBitmap = new Bitmap(stream);
        DitherSourceWithSelectedAlgorithm();
    }

    private void DitherSourceWithSelectedAlgorithm()
    {
        DitheredBitmap = SelectedAlgorithm.Ditherer.Dither(SourceBitmap);
    }

    private ErrorDiffusionPattern GetCustomPattern()
    {
        var matrix = new double[CellRows, CellColumns];
        for (var y = 0; y < CellRows; y++)
        for (var x = 0; x < CellColumns; x++)
            matrix[y, x] = Cells[y * CellColumns + x].Value;
        return new ErrorDiffusionPattern(matrix, (AnchorY, AnchorX));
    }

    private void CustomDithererPatternChanged(object? sender, object args)
    {
        ChangeCustomDitherer(GetCustomPattern());
    }

    private void ChangeCustomDitherer(ErrorDiffusionPattern pattern)
    {
        CustomDitherer.ChangePattern(pattern);
        if (SelectedAlgorithm.Ditherer == CustomDitherer)
            DitherSourceWithSelectedAlgorithm();
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