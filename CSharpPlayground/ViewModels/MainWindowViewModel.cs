using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpPlayground.Models;

namespace CSharpPlayground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // public string Greeting { get; } = "Welcome to Avalonia!";
    [ObservableProperty] private string _code = """
                                                Console.WriteLine("Hello, World!");
                                                1.Dump();
                                                var list = new List<int> { 1, 2, 3, 4, 5 };
                                                list.Dump();
                                                """;
    [ObservableProperty] private string _lineNumberString = "1\n2\n3\n4";
    [ObservableProperty] private string _scriptOutput = "";
    [ObservableProperty] private bool _isCopilotExpanded = false;
    [ObservableProperty] private double _timeConsumed = 0;
    [ObservableProperty] private double _fontSize = 14;
    
    partial void OnCodeChanged(string value)
    {
        // 计算行号
        var lines = value.Split('\n');
        LineNumberString = string.Join("\n", Enumerable.Range(1, lines.Length).Select(i => i.ToString()));
    }

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var files = await OpenFilePicker();

        var path = files[0].TryGetLocalPath();
        if (path is not null)
        {
            Code = await File.ReadAllTextAsync(path);
        }
    }

    [RelayCommand]
    private async Task SaveFileAsync()
    {
        var file = await SaveFilePicker();

        if (file != null)
        {
            var path = file.Path.LocalPath;
            await File.WriteAllTextAsync(path, Code);
        }
    }

    private async Task<IStorageFile> SaveFilePicker()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow != null)
            {
                var file = await desktop.MainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
                {
                    Title = "保存脚本文件",
                    DefaultExtension = "cs",
                    SuggestedFileName = "NewScript",
                    FileTypeChoices = [new FilePickerFileType("C#代码") { Patterns = ["*.cs"] }]
                });
                return file;
            }
        }
        return null;
    }


    [RelayCommand]
    private void SwitchCopilotExpander()
    {
        IsCopilotExpanded = !IsCopilotExpanded;
    }

    [RelayCommand]
    private async Task ExecuteScriptAsync()
    {
        ScriptOutput = "正在执行脚本...";
        Stopwatch stopwatch = Stopwatch.StartNew();
        ScriptOutput = await ScriptExecutor.ExecuteScript(Code);
        stopwatch.Stop();
        TimeConsumed = stopwatch.Elapsed.TotalSeconds;
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
                    FileTypeFilter = [new FilePickerFileType("C#代码") { Patterns = ["*.cs"] }]
                });
                return files;
            }
        }

        return null;
    }
}
