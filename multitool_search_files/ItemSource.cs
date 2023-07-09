using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multitool_search_files
{
    internal class ItemSource
    {
        private bool select;
        private string name;
        private long length;
        private string directory;
        private string creationTime;
        private string extension;
        private string fullName;

        public ItemSource(FileInfo fileInfo, bool select)
        {
            name = fileInfo.Name;
            length = fileInfo.Length;
            directory = fileInfo.DirectoryName;
            creationTime = fileInfo.CreationTime.ToString();
            extension = fileInfo.Extension;
            fullName = fileInfo.FullName;
            this.select = select;
        }

        public bool Select
        {
            get { return select; }
            set { select = value; }
        }
        public string Name
        {
            get { return name; }
        }
        public long Length
        {
            get { return length; }
        }
        public string Directory
        {
            get { return directory; }
        }
        public string CreationTime
        {
            get { return creationTime; }
        }
        public string Extension
        {
            get { return extension; }
        }
        public string FullName
        {
            get { return fullName; }
        }
    }
}
