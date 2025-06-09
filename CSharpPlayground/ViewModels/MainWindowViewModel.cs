using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CSharpPlayground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // public string Greeting { get; } = "Welcome to Avalonia!";
    [ObservableProperty] private string _code = "";
    [ObservableProperty] private string _scriptOutput = "";
    [ObservableProperty] private bool _isCopilotExpanded = false;

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var files =  await OpenFilePicker();

            var path = files[0].TryGetLocalPath();
            if (path is not null)
            {
                Code = await File.ReadAllTextAsync(path);
            }
    }

    [RelayCommand]
    private async Task SwitchCopilotExpander()
    {
        IsCopilotExpanded = !IsCopilotExpanded;
    }
    
    private async Task<IReadOnlyList<IStorageFile>> OpenFilePicker()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow != null)
            {
                var files = await desktop.MainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    Title = "打开脚本文件",
                    AllowMultiple = false,
                    FileTypeFilter = [new FilePickerFileType("C#代码文件"){Patterns = ["*.cs"] }]
                });
                return files;
            }
        }
        return null;
    }
}