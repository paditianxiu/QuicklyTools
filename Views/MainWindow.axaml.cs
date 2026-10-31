using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace QuicklyTools.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    // Window chrome is view behavior and does not belong in application ViewModels.
    private void Close_Click(object? sender, RoutedEventArgs e) => Close();

    private void Minimize_Click(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void Maximize_Click(object? sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }
}
