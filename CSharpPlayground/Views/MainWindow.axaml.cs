using System;
using Avalonia;
using Avalonia.Controls;
using CSharpPlayground.ViewModels;

namespace CSharpPlayground.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CodeEditor_OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            CodeEditor.Text = vm.Code;
        }
    }

    private void CodeEditor_OnTextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.Code = CodeEditor.Text;
        }
    }
}