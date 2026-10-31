using System.Collections.ObjectModel;
using QuicklyTools.Stores;

namespace QuicklyTools.ViewModels;

public sealed class HomeViewModel(ApplicationStore applications) : ViewModelBase
{
    public ReadOnlyObservableCollection<AppItemViewModel> Applications { get; } = applications.Applications;
}
