using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter
{
    class Model
    {
        private Sorter sorter;
        public string SourceFolderLocation { get; set; }
        public string TargetFolderLocation { get; set; }
        public int NumberOfThreads { get; set; }

        public Model()
        {
            sorter = new Sorter();
        }

        public bool AllowDuplicates
        {
            get
            {
                return sorter.AllowDuplicates;
            }
            set
            {
                sorter.AllowDuplicates = value;
            }
        }

        public bool CopyFiles
        {
            get
            {
                return sorter.CopyFiles;
            }
            set
            {
                sorter.CopyFiles = value;
            }
        }

        public void StartSorting()
        {
            sorter.SortFolderThreadSafe(this.SourceFolderLocation, this.TargetFolderLocation, this.NumberOfThreads);
        }
    }
}
