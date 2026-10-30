using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace QuicklyTools.ViewModels;
public partial class SettingsViewModel : ViewModelBase
{
    public ObservableCollection<string> ThemeOptions { get; } = new() { "跟随系统", "浅色", "深色" };

    [ObservableProperty]
    private bool launchOnStartup;

    [ObservableProperty]
    private bool minimizeToTray = true;

    [ObservableProperty]
    private bool compactCards;

    [ObservableProperty]
    private string selectedTheme = "跟随系统";
}

