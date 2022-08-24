﻿using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Navigation;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace WSLDiskShrinker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private bool _isScanning = false;
    private bool _isProcessing = false;
    private WSLDistro? _currentShrinkingDistro;
    private int _currentShrinkingIndex = 1;
    private string _status = "Idle";
    public bool IsProcessing
    {
        get => _isProcessing;
        private set
        {
            if (value == _isProcessing) return;
            _isProcessing = value;
            OnPropertyChanged();
        }
    }

    public bool IsScanning
    {
        get => _isScanning;
        private set
        {
            if (value == _isScanning) return;
            _isScanning = value;
            OnPropertyChanged();
        }
    }

    public int CurrentShrinkingIndex
    {
        get => _currentShrinkingIndex;
        private set
        {
            if (value == _currentShrinkingIndex) return;
            _currentShrinkingIndex = value;
            OnPropertyChanged();
        }
    }

    public string Status
    {
        get => _status;
        private set
        {
            if (value == _status) return;
            _status = value;
            OnPropertyChanged();
        }
    }

    public WSLDistro? CurrentShrinkingDistro
    {
        get => _currentShrinkingDistro;
        private set
        {
            if (value == _currentShrinkingDistro) return;
            _currentShrinkingDistro = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<WSLDistro> Distros { get; private set; } = new();

    public ObservableCollection<WSLDistro> DistrosToShrink { get; private set; } = new();
    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += RefreshButton_OnClick;
    }

    private void LoadMockObjects()
    {
        Distros.Add(new("?????", "Ubuntu") { Size = 2321412343 });
        Distros.Add(new("?????", "Linux") { Size = 232121323212 });
        Distros.Add(new("?????", "ArchLinux") { Size = 232122313 });
        DistrosToShrink.Add(Distros[0]);
    }

    private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }

    private async void RefreshButton_OnClick(object sender, RoutedEventArgs e) /* Called during initialization. */
    {
        var btn = sender as Button;
        DisableButton(btn);
        IsScanning = true;
        Distros.Clear();
        await Task.Delay(200);
        var distros = Scanner.Scan();
        foreach (var distro in distros) Distros.Add(distro);
        IsScanning = false;
        EnableButton(btn);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void ShrinkAllButton_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        DisableButton(btn);
        foreach (var distro in Distros) DistrosToShrink.Add(distro);
        await dialogHost.ShowDialog(dialogHost.DialogContent ?? "n/a"); // Keep compiler happy...
        EnableButton(btn);
    }

    private void OpenFolderButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.GetDirectoryName(((WSLDistro)((Button)sender).DataContext).Path) ?? "n/a", // Keep compiler happy...
                UseShellExecute = true
            });
        }
        catch
        {
            MessageBox.Show("Error occurred while opening the containing directory.", "Oops!", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private static void EnableButton(Button? btn)
    {
        if (btn is not null) btn.IsEnabled = true;
    }

    private static void DisableButton(Button? btn)
    {
        if (btn is not null) btn.IsEnabled = false;
    }

    private async void ShrinkButton_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        DisableButton(btn);
        DistrosToShrink.Add((WSLDistro)((Button)sender).DataContext);
        await dialogHost.ShowDialog(dialogHost.DialogContent ?? "n/a"); // Keep compiler happy...
        EnableButton(btn);
    }

    private async void ProceedButton_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        DisableButton(btn);
        IsProcessing = true;
        await Shrink();
        IsProcessing = false;
        EnableButton(btn);
    }

    private async Task Shrink()
    {
        CurrentShrinkingIndex = 0;
        foreach (var distro in DistrosToShrink)
        {
            CurrentShrinkingIndex++;
            CurrentShrinkingDistro = distro;
            if (distro.Name != "Custom VHDX")
            {
                Status = "Terminating the distro...";
                var terpr = Process.Start("wsl.exe", $"-t {distro.Name}");
#if NET5_0_OR_GREATER
                await terpr.WaitForExitAsync();
#else
                await terpr.CustomWaitForExitAsync();
#endif

                if (terpr.ExitCode != 0)
                {
                    Status = "Error while terminating!";
                    MessageBox.Show(
                        $"While shrinking {distro.Name}," +
                        $"We encountered an error while terminating the WSL distro.\r\n" +
                        "The virtual disk of this distro is left unshrunk.\r\n" +
                        $"Technical details:\r\nExit code:{terpr.ExitCode}.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }
            }

            Status = "Writing diskpart script...";
            FileInfo script;
            try
            {
                script = await Shrinker.GenerateDiskpartScript(distro.Path);
            }
            catch (Exception e)
            {
                Status = "Error while writing diskpart script!";
                MessageBox.Show(
                    $"While shrinking {distro.Name}," +
                    $"We encountered an error while writing diskpart script.\r\n" +
                    "The virtual disk of this distro is left unshrunk.\r\n" +
                    $"Technical details:\r\nStackTrace: {e.StackTrace}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                continue;
            }

            Status = "Shrinking ...";
            try
            {
                await Shrinker.ShrinkUsingDiskpart(script);
            }
            catch (CommandFailedException e)
            {
                Status = "Error while executing diskpart script!";
                MessageBox.Show(
                    $"While shrinking {distro.Name}," +
                    $"We encountered an error while executing diskpart script.\r\n" +
                    "The virtual disk of this distro is in an unknown state (probably unchanged). :(\r\n" +
                    $"Technical details:\r\n" +
                    $"Command Stdout: {await e.FailedProcess.StandardOutput.ReadToEndAsync()}\r\n" +
                    $"Command Stderr: {await e.FailedProcess.StandardError.ReadToEndAsync()}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                continue;
            }

            var newSize = new FileInfo(distro.Path).Length;
            Status = $"Success! Saved {FileSizeConverter.Convert(distro.Size - newSize)}.";
            distro.Size = newSize;
            await Task.Delay(1500);
        }

        dialogHost.IsOpen = false;
        await Task.Delay(500);
        DistrosToShrink.Clear();
    }

    private async void ShrinkCustomButton_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        DisableButton(btn);
        OpenFileDialog dlg = new()
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = "Virtual Disk Files (*.vhdx)|*.vhdx"
        };
        if (dlg.ShowDialog() is true)
        {
            DistrosToShrink.Add(new WSLDistro(dlg.FileName, "Custom VHDX"));
            await dialogHost.ShowDialog(dialogHost.DialogContent ?? "n/a"); // Keep compiler happy...
        }
        EnableButton(btn);
    }

    private void DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventargs)
    {
        if (eventargs.Parameter as string == "cancelled")
        {
            DistrosToShrink.Clear();
        }
    }
}
