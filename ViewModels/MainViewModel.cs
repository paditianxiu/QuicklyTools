using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace QuicklyTools.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<AppItemViewModel> Applications { get; } = new();
    public ObservableCollection<NavigationItemViewModel> NavigationItems { get; } = new();

    public HomeViewModel Home { get; }
    public FavoritesViewModel Favorites { get; }
    public SettingsViewModel Settings { get; }

    [ObservableProperty]
    private ViewModelBase currentPage;

    [ObservableProperty]
    private string pageTitle = "首页";

    [ObservableProperty]
    private string pageSubtitle = "快速访问常用工具";

    [ObservableProperty]
    private bool canAddApplication = true;

    public ICommand MinimizeSelfCommand { get; }
    public ICommand MaximizeSelfCommand { get; }
    public ICommand CloseSelfCommand { get; }

    public MainViewModel()
    {
        Home = new HomeViewModel(Applications);
        Favorites = new FavoritesViewModel(Applications);
        Settings = new SettingsViewModel();

        foreach (var application in Applications)
            application.PropertyChanged += ApplicationOnPropertyChanged;

        NavigationItems.Add(new NavigationItemViewModel("首页", "\uE80F", new RelayCommand(() => SelectPage(Home, "首页", "快速访问常用工具"))));
        NavigationItems.Add(new NavigationItemViewModel("收藏", "\uE734", new RelayCommand(() => SelectPage(Favorites, "收藏", "你标记的常用工具"))));
        NavigationItems.Add(new NavigationItemViewModel("设置", "\uE713", new RelayCommand(() => SelectPage(Settings, "设置", "调整 QuicklyTools 的使用方式"))));

        NavigationItems[0].IsSelected = true;
        CurrentPage = Home;

        MinimizeSelfCommand = new RelayCommand(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow is not null)
                desktop.MainWindow.WindowState = WindowState.Minimized;
        });

        MaximizeSelfCommand = new RelayCommand(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow is not null)
            {
                var window = desktop.MainWindow;
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
        });

        CloseSelfCommand = new RelayCommand(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow?.Close();
        });
    }

    private void SelectPage(ViewModelBase page, string title, string subtitle)
    {
        CurrentPage = page;
        PageTitle = title;
        PageSubtitle = subtitle;
        CanAddApplication = page == Home;

        foreach (var item in NavigationItems)
            item.IsSelected = item.Label == title;
    }

 

    private void ApplicationOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        
    }
}

public partial class AppItemViewModel : ViewModelBase
{
    public string Name { get; }
    public string Description { get; }
    public string Icon { get; }
    public string Accent { get; }
    public string Executable { get; }
    public string? Arguments { get; }
    public ICommand LaunchCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }

    [ObservableProperty]
    private bool isFavorite;

    public AppItemViewModel(
        string name,
        string description,
        string icon,
        string accent,
        string executable,
        bool isFavorite = false,
        string? arguments = null)
    {
        Name = name;
        Description = description;
        Icon = icon;
        Accent = accent;
        Executable = executable;
        Arguments = arguments;
        IsFavorite = isFavorite;
        LaunchCommand = new RelayCommand(Launch);
        ToggleFavoriteCommand = new RelayCommand(() => IsFavorite = !IsFavorite);
    }

    private void Launch()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Executable,
                Arguments = Arguments ?? string.Empty,
                UseShellExecute = true,
            });
        }
        catch (Exception)
        {
            // A launcher card should remain usable when an optional Windows tool is unavailable.
        }
    }
}


