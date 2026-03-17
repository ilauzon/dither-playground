using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace dither_playground.ViewModels;

public partial class CellViewModel() : ViewModelBase
{
    [ObservableProperty] private double _value;
    [ObservableProperty] private bool _isAnchor = false;

    public CellViewModel(PropertyChangedEventHandler handler) : this()
    {
        PropertyChanged += handler;
    }
}