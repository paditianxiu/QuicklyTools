using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuicklyTools.Services;

namespace QuicklyTools.ViewModels;

public partial class NavigationItemViewModel : ViewModelBase
{
    public string Label { get; }
    public string Icon { get; }
    public Type TargetPage { get; }
    public ICommand Command { get; }

    [ObservableProperty]
    private bool isSelected;

    public NavigationItemViewModel(string label, string subtitle, string icon, Type targetPage, NavigationService navigation)
    {
        Label = label;
        Icon = icon;
        TargetPage = targetPage;
        Command = new RelayCommand(() => navigation.NavigateTo(targetPage, label, subtitle));
    }
}
