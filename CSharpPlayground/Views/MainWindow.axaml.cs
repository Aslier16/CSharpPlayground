using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (DataContext is MainWindowViewModel vm)
        {
            // Ctrl + +
            if (e.KeyModifiers == KeyModifiers.Control && (e.Key == Key.Add || e.Key == Key.OemPlus))
            {
                vm.FontSize += 1;
                e.Handled = true;
            }
            // Ctrl + -
            else if (e.KeyModifiers == KeyModifiers.Control && (e.Key == Key.Subtract || e.Key == Key.OemMinus))
            {
                vm.FontSize -= 1;
                e.Handled = true;
            }
        }
    }
}