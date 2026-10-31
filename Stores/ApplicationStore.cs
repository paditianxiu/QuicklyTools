using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using QuicklyTools.Models;
using QuicklyTools.Services;
using QuicklyTools.ViewModels;

namespace QuicklyTools.Stores;

/// <summary>Shared presentation state for the home and favorites pages.</summary>
public sealed class ApplicationStore
{
    private readonly NavigationService navigation;
    private readonly ObservableCollection<AppItemViewModel> applications = new();
    private readonly ObservableCollection<AppItemViewModel> favorites = new();

    public ReadOnlyObservableCollection<AppItemViewModel> Applications { get; }
    public ReadOnlyObservableCollection<AppItemViewModel> Favorites { get; }

    public ApplicationStore(NavigationService navigation, IEnumerable<ApplicationDefinition> definitions)
    {
        this.navigation = navigation;
        Applications = new(applications);
        Favorites = new(favorites);
        foreach (var definition in definitions)
            Add(definition);
    }

    public AppItemViewModel Add(ApplicationDefinition definition, bool isFavorite = false)
    {
        var item = new AppItemViewModel(definition, navigation, isFavorite);
        item.PropertyChanged += OnApplicationPropertyChanged;
        applications.Add(item);
        UpdateFavorite(item);
        return item;
    }

    public bool Remove(AppItemViewModel item)
    {
        if (!applications.Remove(item))
            return false;

        item.PropertyChanged -= OnApplicationPropertyChanged;
        favorites.Remove(item);
        return true;
    }

    private void OnApplicationPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppItemViewModel.IsFavorite) && sender is AppItemViewModel item)
            UpdateFavorite(item);
    }

    private void UpdateFavorite(AppItemViewModel item)
    {
        if (item.IsFavorite)
        {
            if (!favorites.Contains(item))
                favorites.Add(item);
        }
        else
        {
            favorites.Remove(item);
        }
    }
}
