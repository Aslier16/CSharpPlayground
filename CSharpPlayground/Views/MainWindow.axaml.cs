using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CSharpPlayground.ViewModels;

namespace CSharpPlayground.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }


    #region Code Editor 内容绑定

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

    #endregion
    
    /// <summary>
    /// 全局快捷键处理
    /// </summary>
    /// <param name="e"></param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (DataContext is MainWindowViewModel vm)
        {
            #region 字体缩放快捷键

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

            #endregion
        }
    }
    
    protected void OpenSettings(object? sender, RoutedEventArgs e)
    {
        var settingWindow = new Setting()
        {
            DataContext = this.DataContext
        };
        settingWindow.Show();
        e.Handled = true;
    }
}