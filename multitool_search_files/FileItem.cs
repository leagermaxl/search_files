using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multitool_search_files
{
    public class FileItem
    {
        private List<FileInfo> fileInfoListItem = new List<FileInfo>();
        private string[] hashSum;

        public FileItem(FileInfo f1, FileInfo f2, string[] hash)
        {
            fileInfoListItem.Add(f1);
            fileInfoListItem.Add(f2);
            hashSum = hash;
        }
        public string[] HashSum
        {
            get { return hashSum; }
        }

        public List<FileInfo> FileInfoListItem
        {
            get { return fileInfoListItem; }
        }
        public void FileInfoListItemAdd(FileInfo fileInfo)
        {
            fileInfoListItem.Add(fileInfo);
        }
    }
}
