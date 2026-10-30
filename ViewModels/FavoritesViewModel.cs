using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace QuicklyTools.ViewModels;

public partial class FavoritesViewModel : ViewModelBase
{
    public ObservableCollection<AppItemViewModel> FavoriteApps { get; } = new();

    [ObservableProperty]
    private bool hasFavorites;

    [ObservableProperty]
    private bool hasNoFavorites;

    private readonly ObservableCollection<AppItemViewModel> allApplications;

    public FavoritesViewModel(ObservableCollection<AppItemViewModel> allApplications)
    {
        this.allApplications = allApplications;
    }


}