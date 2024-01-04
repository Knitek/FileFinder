using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFinder
{
    public class FileModel(string path)
    {
        public string FileName { get; set; } = System.IO.Path.GetFileName(path);
        public string Path { get; set; } = path;
        public override string ToString()
        {
            return $"{FileName}: {Path}";
        }
    }
}
