using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuicklyTools.Services;

namespace QuicklyTools.ViewModels.Tools;

public partial class JsonTreeViewModel(JsonTreeService jsonTree) : ToolViewModelBase
{
    public ObservableCollection<JsonTreeNodeViewModel> Roots { get; } = new();

    [ObservableProperty]
    private string jsonText = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string statusMessage = "输入 JSON 后点击“生成树”。";

    [RelayCommand(IncludeCancelCommand = true)]
    private Task VisualizeAsync(CancellationToken cancellationToken) => ParseAsync(false, cancellationToken);

    [RelayCommand]
    private Task FormatAsync() => ParseAsync(true, CancellationToken.None);

    private async Task ParseAsync(bool format, CancellationToken cancellationToken)
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        Roots.Clear();
        try
        {
            var input = JsonText;
            if (string.IsNullOrWhiteSpace(input))
                throw new InvalidOperationException("请先输入 JSON 文本。");

            var result = await Task.Run(() => jsonTree.Parse(input, cancellationToken), cancellationToken);
            Roots.Add(new JsonTreeNodeViewModel(result.Root) { IsExpanded = true });
            if (format)
                JsonText = result.FormattedJson;
            StatusMessage = $"解析成功，共 {result.NodeCount} 个节点。";
        }
        catch (JsonException error)
        {
            ErrorMessage = $"JSON 格式错误：第 {(error.LineNumber ?? 0) + 1} 行，第 {(error.BytePositionInLine ?? 0) + 1} 字节。{error.Message}";
            StatusMessage = "解析失败，请检查输入。";
        }
        catch (OperationCanceledException) { StatusMessage = "已取消解析。"; }
        catch (Exception error)
        {
            ErrorMessage = error.Message;
            StatusMessage = "未生成树。";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void ExpandAll()
    {
        foreach (var root in Roots)
            root.SetExpanded(true);
    }

    [RelayCommand]
    private void CollapseAll()
    {
        foreach (var root in Roots)
            root.SetExpanded(false);
    }
}
