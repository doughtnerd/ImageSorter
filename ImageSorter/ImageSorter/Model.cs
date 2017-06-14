using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter
{
    class Model
    {
        /// <summary>
        /// The sorter object that will control all sorting operations.
        /// </summary>
        private Sorter sorter;

        /// <summary>
        /// The Source folder that contains the images that should be sorted.
        /// </summary>
        public string SourceFolderLocation { get; set; }

        /// <summary>
        /// The target folder that should contain the sorted images.
        /// </summary>
        public string TargetFolderLocation { get; set; }

        /// <summary>
        /// The number of threads to use to sort the images.
        /// </summary>
        public int NumberOfThreads { get; set; }

        public Model()
        {
            sorter = new Sorter();
        }

        /// <summary>
        /// Whether or not duplicate that are found between various subfolders of the root folder should be renamed
        /// or simply ignored.
        /// </summary>
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

        /// <summary>
        /// If true, the sorter will not delete the files from the source folder after sorting.
        /// If false, the sorter will delete any source files it is able to successfully transfer
        /// to the target folder.
        /// </summary>
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

        /// <summary>
        /// Starts the sorting process.
        /// </summary>
        public void StartSorting()
        {
            sorter.SortFolderThreadSafe(this.SourceFolderLocation, this.TargetFolderLocation, this.NumberOfThreads);
        }
    }
}
