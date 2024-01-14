using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static System.Net.WebRequestMethods;
using System.Windows;

namespace FileFinder
{
    public class FileFinderModel
    {
        public List<string> SearchFiles(string folderPath, string extensions)
        {
            List<string> files = new List<string>();
            try
            {
                foreach (var ext in extensions.Split(','))
                {
                    string[] tmp = Directory.GetFiles(folderPath, "*." + ext, SearchOption.AllDirectories);
                    files.AddRange(tmp);
                }
            }
            catch { }
            return files;
        }

        public static IEnumerable<FileModel> ToFileModel(List<string> files)
        {
            foreach(var file in files)
            {
                yield return new FileModel(file);
            } 
        }
        public async Task CopyFilesAsync(string destinationFolderPath,ObservableCollection<FileModel> Files, Action<double> ProgressUpdate)
        {
            if (Directory.Exists(destinationFolderPath))
            {
                var errorFiles = new List<FileModel>();
                var totalFiles = Files.Count;
                var currentFileIndex = 0;
                foreach (var file in Files)
                {
                    try
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        DateTime creationDate = File.GetCreationTime(file.Path);

                        string yearMonthFolder = Path.Combine(destinationFolderPath, creationDate.ToString("yyyy-MM"));
                        string destinationPath = Path.Combine(yearMonthFolder, fileName);

                        int count = 1;
                        while (File.Exists(destinationPath))
                        {
                            string newName = $"{Path.GetFileNameWithoutExtension(fileName)}_({count}){Path.GetExtension(fileName)}";
                            destinationPath = Path.Combine(yearMonthFolder, newName);
                            count++;
                        }

                        if (!Directory.Exists(yearMonthFolder))
                        {
                            Directory.CreateDirectory(yearMonthFolder);
                        }
                        await Task.Run(() => File.Copy(file.Path, destinationPath));
                        currentFileIndex++;
                        ProgressUpdate(currentFileIndex);
                    }
                    catch (Exception ex)
                    {
                        errorFiles.Add(file);

                        // Obsługa błędów
                    }
                }
                Files.Clear();
                errorFiles.ForEach(x => Files.Add(x));
                System.Windows.MessageBox.Show("Pliki zostały przeniesione.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Podany katalog docelowy nie istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
