using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace QuicklyTools.ViewModels;

public partial class NavigationItemViewModel : ViewModelBase
{
    public string Label { get; }
    public string Icon { get; }
    public ICommand Command { get; }

    [ObservableProperty]
    private bool isSelected;

    public NavigationItemViewModel(string label, string icon, ICommand command)
    {
        Label = label;
        Icon = icon;
        Command = command;
    }
}
