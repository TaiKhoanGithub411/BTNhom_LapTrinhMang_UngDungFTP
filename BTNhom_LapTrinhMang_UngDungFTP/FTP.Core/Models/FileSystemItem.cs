using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTP.Core.Models
{
    public enum FileType
    {
        File,
        Directory
    }
    public class FileSystemItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public FileType ItemType { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public FileSystemItem()
        {
            LastModified = DateTime.Now;
            Size = 0;
        }

        public FileSystemItem(string name, string path, FileType itemType, long size = 0)
        {
            Name = name;
            Path = path;
            ItemType = itemType;
            Size = size;
            LastModified = DateTime.Now;
        }
    }
}
