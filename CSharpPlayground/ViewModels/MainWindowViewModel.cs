using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpPlayground.Models;
using CSharpPlayground.Views;

namespace CSharpPlayground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // public string Greeting { get; } = "Welcome to Avalonia!";
    [ObservableProperty] private string _code = """
                                                Console.WriteLine("Hello, World!");
                                                "Hello, World!".Dump();
                                                1.Dump();
                                                Enumerable.Range(1, 10).Select(x => x * x).Dump();
                                                """;
    [ObservableProperty] private string _scriptOutput = "";
    [ObservableProperty] private bool _isCopilotExpanded = false;
    [ObservableProperty] private double _timeConsumed = 0;

    
    [ObservableProperty] private double _fontSize = 20;
    [ObservableProperty] private double _windowOpacity = 1;
    [ObservableProperty] private IBrush? _windowOpacityBrush = new SolidColorBrush(Colors.White);
    [ObservableProperty] private double _opacity = 0.2;

    [ObservableProperty] private string _backgroundImagePath = "";
    [ObservableProperty] private Bitmap? _backgroundImage;

    [ObservableProperty] private string _copilotInput = string.Empty;
    [ObservableProperty] private string _copilotOutput = string.Empty;
    [ObservableProperty] private string _deepseekApiToken = string.Empty;

    public MainWindowViewModel()
    {
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "UserConfig.json");
        if (File.Exists(configPath))
        {
            var config = JsonSerializer.Deserialize<UserConfig>(File.ReadAllText(configPath));
            if (config is not null)
            {
                SetUserConfig(config);
            }
        }
    }
    
    [RelayCommand]
    private async Task ChooseBackgroundImage()
    {
        var files = await OpenFilePicker(new FilePickerOpenOptions()
        {
            Title = "选择背景图片",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        var path = files[0].TryGetLocalPath();
        if(path is not null && File.Exists(path))
            BackgroundImagePath = path;
    }

    partial void OnBackgroundImagePathChanged(string value)
    {
        if (File.Exists(value))
        {
            BackgroundImage = new Bitmap(value);
        }
        else
        {
            BackgroundImage = null;
        }
    }
    
    partial void OnWindowOpacityChanged(double value)
    {
        WindowOpacityBrush = new SolidColorBrush(Color.FromArgb((byte)(value * 255),255,255,255));
    }

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var files = await OpenFilePicker(new FilePickerOpenOptions()
        {
            Title = "打开脚本文件",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("C#代码") { Patterns = ["*.cs"] }]
        });

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
    
    [RelayCommand]
    private async Task SendToCopilotAsync()
    {
        if (string.IsNullOrWhiteSpace(CopilotInput))
        {
            // CopilotOutput = "请输入内容";
            return;
        }
        // CopilotOutput = "正在请求 Copilot...";
        try
        {
            var deepseek = new DeepseekRequest();
            var responseJson = await deepseek.SendRequest(CopilotInput, DeepseekApiToken);
            var response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response != null && response.choices != null && response.choices.Length > 0)
            {
                CopilotOutput += Environment.NewLine+Environment.NewLine+response.choices[0].message.content;
            }
            else
            {
                CopilotOutput += Environment.NewLine+Environment.NewLine+"未获取到有效回复。";
            }
        }
        catch (Exception ex)
        {
            CopilotOutput = $"{Environment.NewLine+Environment.NewLine}请求失败: {ex.Message}";
        }
        finally
        {
            CopilotInput = string.Empty;
        }
    }

    private async Task<IReadOnlyList<IStorageFile>> OpenFilePicker(FilePickerOpenOptions options)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow != null)
            {
                var files = await desktop.MainWindow.StorageProvider.OpenFilePickerAsync(options);
                return files;
            }
        }

        return null;
    }

    public UserConfig SaveUserConfig()
    {
        return new UserConfig()
        {
            FontSize = FontSize,
            WindowOpacity = WindowOpacity,
            BackgroundImagePath = BackgroundImagePath,
            Opacity = Opacity,
            DeepseekApiToken = DeepseekApiToken
        };
    }
    
    private void SetUserConfig(UserConfig config)
    {
        FontSize = config.FontSize;
        WindowOpacity = config.WindowOpacity;
        BackgroundImagePath = config.BackgroundImagePath;
        Opacity = config.Opacity;
        DeepseekApiToken = config.DeepseekApiToken;
    }
}

public class UserConfig
{
    public double FontSize { get; set; } = 20;
    public double WindowOpacity { get; set; } = 1;
    public string BackgroundImagePath { get; set; } = "";
    public double Opacity { get; set; } = 0.2;
    
    public string DeepseekApiToken { get; set; } = string.Empty;
}
