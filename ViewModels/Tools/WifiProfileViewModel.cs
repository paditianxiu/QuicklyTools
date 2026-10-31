using CommunityToolkit.Mvvm.ComponentModel;
using QuicklyTools.Models;

namespace QuicklyTools.ViewModels.Tools;

public partial class WifiProfileViewModel(WifiProfile profile) : ViewModelBase
{
    public string ProfileName => profile.ProfileName;
    public string Ssid => profile.Ssid;
    public string Authentication => profile.Authentication;
    public string Status => profile.Status;
    public bool HasPassword => profile.Password is not null;
    public string PasswordText => HasPassword  ? profile.Password! : "—";
}
