using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataExtractor;
using System.IO;
using System.Text.RegularExpressions;

namespace ImageSorter
{
    public class Model
    {
        /// <summary>
        /// Dictionary indicating which files belong to which date.
        /// </summary>
        private Dictionary<DateTime, string> dateMap;

        /// <summary>
        /// Dictionary where the source image path is the key, the target sorted path is the value.
        /// </summary>
        private Dictionary<string, string> pathMap;

        /// <summary>
        /// Dictionary indicating renaming of any duplicate file names that were found.
        /// </summary>
        private Dictionary<string, string> duplicateMap;

        /// <summary>
        /// List of images that could not be sorted.
        /// </summary>
        private List<string> unableToSort;

        private ImageSortOptions sortOptions = 0x0000;

        public ImageSortOptions SortOptions
        {
            get
            {
                return sortOptions;
            }
            set
            {
                sortOptions = value;
            }
        }

        public Model()
        {
            dateMap = new Dictionary<DateTime, string>();
            pathMap = new Dictionary<string, string>();
            duplicateMap = new Dictionary<string, string>();
            unableToSort = new List<string>();
        }

        /// <summary>
        /// Handles sorting the image files in the root folder and producing the sorted results in the target.
        /// </summary>
        public void SortFolder(string rootPath, string targetPath)
        {
            DirectoryInfo rootInfo = System.IO.Directory.CreateDirectory(rootPath);
            DirectoryInfo targetInfo = System.IO.Directory.CreateDirectory(targetPath);
        }

        /// <summary>
        /// Returns the exif date data in the given image.
        /// </summary>
        public IEnumerable<DateTime> GetDatesFromImage(string imagePath)
        {
            List<DateTime> dates = new List<DateTime>();
            IEnumerable<MetadataExtractor.Directory> dirs = ExtractExIfDirectories(imagePath);
            foreach(MetadataExtractor.Directory dir in dirs)
            {
                foreach(Tag tag in ExtractExIfDateTags(dir))
                {
                    dates.Add(ConvertTagToDate(tag));
                }
            }
            return dates;
        }

        private DateTime ConvertTagToDate(Tag tag)
        {
            string dateMatch = @"[1-9][0-9]{3}:[0-9]{2}:[0-9]{2}";
            string match = Regex.Match(tag.Description, dateMatch).ToString();
            string[] arr = match.Split(':');
            return new DateTime(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
        }

        public IEnumerable<Tag> ExtractExIfDateTags(MetadataExtractor.Directory dir)
        {
            List<Tag> tags = new List<Tag>();
            foreach(Tag t in dir.Tags)
            {
                if (t.Name.ToLower().Contains("date")){
                    tags.Add(t);
                }
            }
            return tags;
        }

        public IEnumerable<MetadataExtractor.Directory> ExtractExIfDirectories(string imagePath)
        {
            List<MetadataExtractor.Directory> list = new List<MetadataExtractor.Directory>();
            foreach(MetadataExtractor.Directory dir in ImageMetadataReader.ReadMetadata(imagePath))
            {
                if (dir.Name.ToLower().Contains("exif"))
                {
                    list.Add(dir);
                }
            }
            return list;
        }

        private IEnumerable<MetadataExtractor.Directory> ExtractMetaDataDirectories(string imagePath)
        {
            return ImageMetadataReader.ReadMetadata(imagePath);
        }

        public struct ImageSortOptions
        {
            private byte value;

            public byte Value {
                get
                {
                    return this.value;
                }
                private set
                {
                    this.value = value;
                }
            }

            public ImageSortOptions(byte value)
            {
                this.value = value;
            }

            public static implicit operator ImageSortOptions(byte value)
            {
                return new ImageSortOptions(value);
            }

            public static implicit operator byte(ImageSortOptions options)
            {
                return options.Value;
            }

            public static bool operator ==(ImageSortOptions first, ImageSortOptions second)
            {
                return first.Value == second.Value;
            }

            public static bool operator !=(ImageSortOptions first, ImageSortOptions second)
            {
                return first.Value != second.Value;
            }

            /// <summary>
            /// Enum representing the possible options this class is capable of handling.
            /// </summary>
            public enum Options : byte
            {
                COPY_FILES = 0x0001,
                ALLOW_DUPLICATES = 0x0010
            }
        }
    }
}
