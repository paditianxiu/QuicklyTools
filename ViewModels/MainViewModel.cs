using System;
using System.Collections.Generic;
using System.ComponentModel;
using QuicklyTools.Services;
using QuicklyTools.ViewModels.Tools;
using CommunityToolkit.Mvvm.Input;

namespace QuicklyTools.ViewModels;

public sealed class MainViewModel : ViewModelBase, IDisposable
{
    public NavigationService Navigation { get; }
    public IReadOnlyList<NavigationItemViewModel> NavigationItems { get; }
    public IRelayCommand GoBackCommand { get; }

    public MainViewModel(NavigationService navigation)
    {
        Navigation = navigation;
        GoBackCommand = new RelayCommand(navigation.GoBack, () => navigation.CanGoBack);
        NavigationItems = Array.AsReadOnly(new[]
        {
            new NavigationItemViewModel("首页", "快速访问常用工具", "\uE80F", typeof(HomeViewModel), navigation),
            new NavigationItemViewModel("收藏", "你标记的常用工具", "\uE734", typeof(FavoritesViewModel), navigation),
            new NavigationItemViewModel("设置", "调整 QuicklyTools 的使用方式", "\uE713", typeof(SettingsViewModel), navigation),
        });

        Navigation.PropertyChanged += OnNavigationPropertyChanged;
        NavigationItems[0].Command.Execute(null);
        UpdateSelection();
    }

    private void OnNavigationPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NavigationService.CurrentPage))
            UpdateSelection();
        if (e.PropertyName == nameof(NavigationService.CanGoBack))
            GoBackCommand.NotifyCanExecuteChanged();
    }

    private void UpdateSelection()
    {
        foreach (var item in NavigationItems)
            item.IsSelected = item.TargetPage == Navigation.CurrentPage?.GetType()
                || item.TargetPage == typeof(HomeViewModel) && Navigation.CurrentPage is ToolViewModelBase;
    }

    public void Dispose() => Navigation.PropertyChanged -= OnNavigationPropertyChanged;
}
