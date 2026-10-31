using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuicklyTools.Models;
using QuicklyTools.Services;

namespace QuicklyTools.ViewModels;

public partial class AppItemViewModel : ViewModelBase
{
    private readonly NavigationService navigation;

    public ApplicationDefinition Definition { get; }
    public string Title => Definition.Title;
    public string Subtitle => Definition.Subtitle;
    public string Icon => Definition.Icon;
    public string Accent => Definition.Accent;
    public Type TargetPage => Definition.TargetPage;
    public string FavoriteIcon => IsFavorite ? "\uE735" : "\uE734";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FavoriteIcon))]
    private bool isFavorite;

    public AppItemViewModel(ApplicationDefinition definition, NavigationService navigation, bool isFavorite = false)
    {
        Definition = definition;
        this.navigation = navigation;
        this.isFavorite = isFavorite;
    }

    [RelayCommand]
    private void Navigate() => navigation.NavigateTo(TargetPage, Title, Subtitle);

    [RelayCommand]
    private void ToggleFavorite() => IsFavorite = !IsFavorite;
}
