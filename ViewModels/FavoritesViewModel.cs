using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using QuicklyTools.Stores;

namespace QuicklyTools.ViewModels;

public sealed class FavoritesViewModel : ViewModelBase, IDisposable
{
    public ReadOnlyObservableCollection<AppItemViewModel> FavoriteApps { get; }
    public bool HasFavorites => FavoriteApps.Count > 0;
    public bool HasNoFavorites => !HasFavorites;

    public FavoritesViewModel(ApplicationStore applications)
    {
        FavoriteApps = applications.Favorites;
        ((INotifyCollectionChanged)FavoriteApps).CollectionChanged += OnFavoritesChanged;
    }

    private void OnFavoritesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasFavorites));
        OnPropertyChanged(nameof(HasNoFavorites));
    }

    public void Dispose() => ((INotifyCollectionChanged)FavoriteApps).CollectionChanged -= OnFavoritesChanged;
}
