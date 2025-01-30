using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace MergerApp;

public partial class MainWindow : Window, INotifyPropertyChanged
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
    
    #region Fields
    
    /// <summary>
    /// Collection of selected files
    /// </summary>
    private ObservableCollection<string> _selectedFiles = [];
    
    /// <summary>
    /// Current status message
    /// </summary>
    private string _statusMessage = "No files selected.";
    
    /// <summary>
    /// Merge availability
    /// </summary>
    private bool _canMerge = false;
    
    #endregion

    #region Properties
    
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
    
    #endregion

    #region Event Handlers
    
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
                SelectedFiles.Add(file.Path.AbsolutePath);
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

    private async void OnMergeFilesClick(object sender, RoutedEventArgs e)
    {
        if (SelectedFiles.Count == 0) return;
        
        StatusMessage = "Merging...";
        CanMerge = false;

        try
        {
            var outputFile = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Save File",
                FileTypeChoices =
                [
                    new FilePickerFileType("Excel Files")
                    {
                        Patterns = ["*.xls", "*.xlsx"]
                    },
                ],
                SuggestedFileName = "MergedFile.xlsx"
            });

            if (outputFile != null)
            {
                var merger = new Merger.Merger();
                await Task.Run(() => merger.MergeFiles(SelectedFiles, outputFile.Path.AbsolutePath));

                StatusMessage = "Files merged successfully.";
            }
            else
            {
                StatusMessage = "Merge canceled.";
            }
        }
        catch (Exception exception)
        {
            StatusMessage = $"Error: {exception.Message}";
        }
        finally
        {
            CanMerge = true;
        }
    }

    private void OnExitClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
    
    #endregion
}