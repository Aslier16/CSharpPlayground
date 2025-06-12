using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using CSharpPlayground.ViewModels;

namespace CSharpPlayground.Views;

public partial class Setting : Window
{
    public Setting()
    {
        InitializeComponent();
    }

    private void RangeBase_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        
    }

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var text = ImagePathTextBox.Text;
        if(text is not null && text != "")
        {
            
            if (DataContext is MainWindowViewModel vm)
                if (File.Exists(text))
                {
                    vm.BackgroundImagePath = text;
                }
        }
    }
}