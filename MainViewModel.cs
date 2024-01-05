using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;

using System;

namespace FileFinder
{
    public class MainViewModel : ViewModelBase
    {
        private double _progress;
        private FileModel _selectedFile;
        private readonly FileFinderModel _model;
        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
        public FileModel SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }
        public ObservableCollection<FileModel> Files { get => files; set => files = value; }

        public ICommand FindFilesCommand { get; }
        public ICommand BrowseSourceCommand { get; }
        public ICommand MoveFilesCommand { get; }
        public ICommand BrowseDestinationCommand { get; }
        public ICommand IgnoreFileCommand { get; }
        public ICommand IgnoreFolderCommand { get; }
        public MainViewModel()
        {
            _model = new FileFinderModel();
            MoveFilesCommand = new RelayCommand(MoveFiles);
            FindFilesCommand = new RelayCommand(FindFiles);
            BrowseSourceCommand = new RelayCommand(BrowseSource);
            BrowseDestinationCommand = new RelayCommand(BrowseDestination);
            IgnoreFileCommand = new RelayCommand(IgnoreFile);
            IgnoreFolderCommand = new RelayCommand(IgnoreFolder);
            Files = new ObservableCollection<FileModel>();

            SetDefaults();
        }
        private void IgnoreFile()
        {
            Files.Remove(SelectedFile);
            OnPropertyChanged(nameof(Files));
        }
        private void IgnoreFolder()
        {
            var folder = string.Join(System.IO.Path.DirectorySeparatorChar, SelectedFile.RelativePath.Split(System.IO.Path.DirectorySeparatorChar).SkipLast(1));
            if (string.IsNullOrWhiteSpace(folder)) return;
            Files = new ObservableCollection<FileModel>(Files.Where(x => x.RelativePath.Contains(folder) is false));
            OnPropertyChanged(nameof(Files));
        }
        private void SetDefaults()
        {
            ExtensionList = "jpg";
        }

        private string _sourceFolderPath;
        string extensionList;
        public bool CopyFilesIsEnabled
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DestinationFolderPath))
                    return false;
                else
                    return true;
            }
        }
        public string ExtensionList
        {
            get => extensionList;
            set
            {
                extensionList = value;
                OnPropertyChanged(nameof(ExtensionList));
            }
        }
        public bool SearchFilesIsEnabled
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SourceFolderPath))
                    return false;
                else
                    return true;
            }
        }

        public string SourceFolderPath
        {
            get => _sourceFolderPath;
            set
            {
                _sourceFolderPath = value;
                OnPropertyChanged(nameof(SourceFolderPath));
                OnPropertyChanged(nameof(SearchFilesIsEnabled));
            }
        }

        private string _destinationFolderPath;
        private ObservableCollection<FileModel> files;

        public string DestinationFolderPath
        {
            get => _destinationFolderPath;
            set
            {
                _destinationFolderPath = value;
                OnPropertyChanged(nameof(DestinationFolderPath));
                OnPropertyChanged(nameof(CopyFilesIsEnabled));
            }
        }

        //public ObservableCollection<string> Files { get; set; }
        

        private void FindFiles()
        {
            Files.Clear();

            if (Directory.Exists(SourceFolderPath) && (string.IsNullOrWhiteSpace(ExtensionList) is false))
            {
                var tmp = _model.SearchFiles(SourceFolderPath, ExtensionList);
                Files = new ObservableCollection<FileModel>(FileFinderModel.ToFileModel(tmp));
                foreach(var file in Files) { file.RelativePath = file.Path.Remove(0,SourceFolderPath.Length+1); }
            }
            OnPropertyChanged(nameof(Files));
        }

        private void BrowseSource()
        {
            SourceFolderPath = SelectFolderPath();
        }

        private void BrowseDestination()
        {
            DestinationFolderPath = SelectFolderPath();
        }

        private string SelectFolderPath()
        {
            var folderDialog = new FolderBrowserDialog();
            var result = folderDialog.ShowDialog();

            return result != DialogResult.Abort ? folderDialog.SelectedPath : string.Empty;

        }
        private void MoveFiles()
        {
            if (Directory.Exists(DestinationFolderPath))
            {
                var totalFiles = Files.Count;
                var currentFileIndex = 0;
                foreach (var file in Files)
                {
                    try
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        DateTime creationDate = File.GetCreationTime(file.Path);

                        string yearMonthFolder = Path.Combine(DestinationFolderPath, creationDate.ToString("yyyy-MM"));
                        string destinationPath = Path.Combine(yearMonthFolder, fileName);

                        if (!Directory.Exists(yearMonthFolder))
                        {
                            Directory.CreateDirectory(yearMonthFolder);
                        }

                        File.Copy(file.Path, destinationPath);
                        currentFileIndex++;
                        Progress = (double)currentFileIndex / totalFiles * 100;
                    }
                    catch (Exception ex)
                    {
                        // Obsługa błędów
                    }
                }

                Files.Clear();
                System.Windows.MessageBox.Show("Pliki zostały przeniesione.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Podany katalog docelowy nie istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
