using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using QuicklyTools.ViewModels;
using QuicklyTools.ViewModels.Tools;

namespace QuicklyTools.Services;

public sealed class NavigationService(IServiceProvider services) : ObservableObject
{
    private ViewModelBase? currentPage;
    private string pageTitle = string.Empty;
    private string pageSubtitle = string.Empty;
    private readonly Stack<NavigationEntry> history = new();
    public bool CanGoBack => history.Count > 0;

    public ViewModelBase? CurrentPage
    {
        get => currentPage;
        private set => SetProperty(ref currentPage, value);
    }

    public string PageTitle
    {
        get => pageTitle;
        private set => SetProperty(ref pageTitle, value);
    }

    public string PageSubtitle
    {
        get => pageSubtitle;
        private set => SetProperty(ref pageSubtitle, value);
    }

    public void NavigateTo(Type viewModelType, string title, string subtitle)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);
        if (!typeof(ViewModelBase).IsAssignableFrom(viewModelType) || viewModelType.IsAbstract)
            throw new ArgumentException("导航目标必须是具体的 ViewModel 类型。", nameof(viewModelType));

        // Only the routing layer resolves page types; ViewModels use constructor injection.
        var page = (ViewModelBase)services.GetRequiredService(viewModelType);
        if (CurrentPage is not null && !ReferenceEquals(CurrentPage, page))
            history.Push(new(CurrentPage, PageTitle, PageSubtitle));
        ShowPage(page, title, subtitle);
    }

    public void GoBack()
    {
        if (history.TryPop(out var entry))
            ShowPage(entry.Page, entry.Title, entry.Subtitle);
    }

    private void ShowPage(ViewModelBase page, string title, string subtitle)
    {
        if (page is ToolViewModelBase toolPage)
            toolPage.OnNavigatedTo();

        PageTitle = title;
        PageSubtitle = subtitle;
        CurrentPage = page;
        OnPropertyChanged(nameof(CanGoBack));
    }

    private sealed record NavigationEntry(ViewModelBase Page, string Title, string Subtitle);
}
