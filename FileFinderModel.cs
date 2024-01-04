using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
