using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace QuicklyTools.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public ObservableCollection<AppItemViewModel> Applications { get; }

    public HomeViewModel(ObservableCollection<AppItemViewModel> applications)
    {
        Applications = applications;
     
    }
}