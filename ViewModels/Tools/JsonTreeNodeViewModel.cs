using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using QuicklyTools.Models;

namespace QuicklyTools.ViewModels.Tools;

public partial class JsonTreeNodeViewModel : ViewModelBase
{
    public string Name { get; }
    public string Value { get; }
    public IReadOnlyList<JsonTreeNodeViewModel> Children { get; }

    [ObservableProperty]
    private bool isExpanded;

    public JsonTreeNodeViewModel(JsonTreeNode node)
    {
        Name = node.Name;
        Value = node.Value;
        Children = node.Children.Select(child => new JsonTreeNodeViewModel(child)).ToArray();
    }

    public void SetExpanded(bool expanded)
    {
        IsExpanded = expanded;
        foreach (var child in Children)
            child.SetExpanded(expanded);
    }
}
