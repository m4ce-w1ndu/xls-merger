using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace MergerApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private ObservableCollection<string> _selectedFiles = [];
    private string _statusMessage = "No files selected.";
    private bool _canMerge = false;

    public ObservableCollection<string> SelectedFiles
    {
        get => _selectedFiles;
        set
        {
            _selectedFiles = value;
            OnPropertyChanged(nameof(SelectedFiles));
        }
    }
    
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    public bool CanMerge
    {
        get => _canMerge;
        set
        {
            _canMerge = value;
            OnPropertyChanged(nameof(CanMerge));
        }
    }

    private async void OnSelectFilesClick(object sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select Files",
            AllowMultiple = true,
            FileTypeFilter =
            [
                new FilePickerFileType("Excel Files")
                {
                    Patterns = ["*.xls", "*.xlsx"]
                }
            ]
        });

        if (files.Count > 0)
        {
            SelectedFiles.Clear();
            foreach (var file in files)
            {
                SelectedFiles.Add(file.Name);
            }

            StatusMessage = $"{SelectedFiles.Count} file(s) selected.";
            CanMerge = true;
        }
        else
        {
            StatusMessage = "No files selected.";
            CanMerge = false;
        }
    }

    private void OnMergeFilesClick(object sender, RoutedEventArgs e)
    {
        if (SelectedFiles.Count == 0) return;
        
        StatusMessage = "Merging...";
        CanMerge = false;
    }

    private void OnExitClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    // Implement INotifyPropertyChanged for data binding.
    public new event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
}