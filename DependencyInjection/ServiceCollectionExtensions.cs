using Microsoft.Extensions.DependencyInjection;
using QuicklyTools.Services;
using QuicklyTools.Stores;
using QuicklyTools.ViewModels;
using QuicklyTools.Views;
using QuicklyTools.Models;
using QuicklyTools.ViewModels.Tools;
using Avalonia.Controls;

namespace QuicklyTools.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuicklyTools(this IServiceCollection services)
    {
        services.AddSingleton<NavigationService>();
        services.AddSingleton<ApplicationStore>();
        services.AddSingleton<IWifiProfileService, WindowsWifiProfileService>();
        services.AddSingleton<JsonTreeService>();
        services.AddSingleton<ViewLocator>();

        // Keep page state when switching tabs. All pages share the same application store.
        services.AddPage<HomeViewModel, HomeView>();
        services.AddPage<FavoritesViewModel, FavoritesView>();
        services.AddPage<SettingsViewModel, SettingsView>();
        services.AddTool<WifiPasswordViewModel, WifiPasswordView>("WiFi 密码查看器", "查看本机保存的无线网络和密码", "\uE701", "#E8F1FF");
        services.AddTool<JsonTreeViewModel, JsonTreeView>("JSON 可视化", "格式化 JSON，以树形结构浏览数据", "\uE943", "#EAE8FF");
        services.AddSingleton<MainViewModel>();
        services.AddSingleton(provider => new MainWindow
        {
            DataContext = provider.GetRequiredService<MainViewModel>(),
        });

        return services;
    }

    public static IServiceCollection AddPage<TViewModel, TView>(this IServiceCollection services)
        where TViewModel : ViewModelBase
        where TView : Control
    {
        services.AddSingleton<TViewModel>();
        services.AddTransient<TView>();
        services.AddSingleton(new ViewRegistration(typeof(TViewModel), typeof(TView)));
        return services;
    }

    public static IServiceCollection AddTool<TViewModel, TView>(this IServiceCollection services,
        string title, string subtitle, string icon, string accent)
        where TViewModel : ToolViewModelBase
        where TView : Control
    {
        services.AddPage<TViewModel, TView>();
        services.AddSingleton(new ApplicationDefinition(title, subtitle, icon, accent, typeof(TViewModel)));
        return services;
    }
}
