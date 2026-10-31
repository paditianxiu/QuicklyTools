using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuicklyTools.Services;

namespace QuicklyTools.ViewModels.Tools;

public partial class WifiPasswordViewModel(IWifiProfileService wifiProfiles) : ToolViewModelBase
{
    public ObservableCollection<WifiProfileViewModel> Profiles { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "正在读取本机已保存的 WiFi 网络…";

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public override void OnNavigatedTo()
    {
        if (!IsBusy)
            _ = RefreshCommand.ExecuteAsync(null);
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        Profiles.Clear();
        try
        {
            var profiles = await wifiProfiles.GetProfilesAsync(cancellationToken);
            foreach (var profile in profiles)
                Profiles.Add(new WifiProfileViewModel(profile));
            StatusMessage = profiles.Count == 0 ? "没有找到已保存的 WiFi 网络。" : $"找到 {profiles.Count} 个已保存的网络，密码默认隐藏。";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "已取消读取。";
        }
        catch (Exception error)
        {
            StatusMessage = "读取失败。";
            ErrorMessage = error.Message;
        }
        finally { IsBusy = false; }
    }
}
