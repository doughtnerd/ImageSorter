using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetadataExtractor;
using System.IO;
using System.Text.RegularExpressions;

namespace ImageSorter
{
    public class Sorter
    {
        /// <summary>
        /// Dictionary indicating which files belong to which date.
        /// </summary>
        private Dictionary<DateTime, string> dateMap = new Dictionary<DateTime, string>();

        /// <summary>
        /// Dictionary where the source image path is the key, the target sorted path is the value.
        /// </summary>
        private Dictionary<string, string> pathMap = new Dictionary<string, string>();

        /// <summary>
        /// Dictionary indicating renaming of any duplicate file names that were found.
        /// </summary>
        private Dictionary<string, string> duplicateMap = new Dictionary<string, string>();

        /// <summary>
        /// List of images that could not be sorted.
        /// </summary>
        private List<string> unableToSort = new List<string>();

        /// <summary>
        /// Types of image extensions that this sorter will allow sorting on when ran.
        /// </summary>
        public string[] Extensions { get; set; }

        /// <summary>
        /// Whether or not to allow duplicate files. If so, duplicates will be renamed incrementally.
        /// </summary>
        public bool AllowDuplicates { get; set; }

        /// <summary>
        /// Whether or not to copy files from source to destination or to move the file.
        /// </summary>
        public bool CopyFiles { get; set; }

        /// <summary>
        /// Creates a new sorter with default allowed image extensions and default multithreading task count.
        /// </summary>
        public Sorter()
        {
            this.Extensions = new string[] { ".jpg", ".png" };
        }

        /// <summary>
        /// Uses multiple threads to sort a folder and it's images.
        /// </summary>
        public void SortFolderThreadSafe(string rootPath, string targetPath, int nTasks)
        {
            DirectoryInfo rootInfo = System.IO.Directory.CreateDirectory(rootPath);
            DirectoryInfo targetInfo = System.IO.Directory.CreateDirectory(targetPath);

            Queue<DirectoryInfo> dirs = new Queue<DirectoryInfo>(GetAllDirectoriesInDirectory(rootInfo));
            dirs.Enqueue(rootInfo);
            List<Task> tasks = new List<Task>();
            while (dirs.Count != 0)
            {
                for(int i = 0; i < nTasks; i++)
                {
                    if (dirs.Count > 0)
                    {
                        DirectoryInfo dir = dirs.Dequeue();
                        tasks.Add(Task.Run(() => SortFolder(dir, targetInfo, false)));
                    }
                }
                Task.WaitAll(tasks.ToArray());
                tasks.Clear();
            }
        }

        public void WriteTrackersToFile(string rootFolderPath)
        {
            WriteDictionaryToFile(rootFolderPath +"/Paths.txt", (IDictionary<object, object>)pathMap);
            WriteDictionaryToFile(rootFolderPath +"/Dates.txt", (IDictionary<object, object>)this.dateMap);
            WriteDictionaryToFile(rootFolderPath +"/Duplicates.txt", (IDictionary<object, object>)this.duplicateMap);
        }

        private void WriteDictionaryToFile(string path, IDictionary<object, object> dic)
        {
            using(StreamWriter stream = new StreamWriter(path))
            {
                foreach(KeyValuePair<object, object> pair in dic)
                {
                    stream.WriteLine(pair.Key.ToString() + " : " + pair.Value.ToString());
                }
                stream.Flush();
            }
        }

        /// <summary>
        /// Requires that valid file paths are passed to the method.
        /// Handles sorting the image files in the root folder and producing the sorted results in the target.
        /// </summary>
        public void SortFolder(string rootPath, string targetPath)
        {
            SortFolder(rootPath, targetPath, true);
        }

        public void SortFolder(string rootPath, string targetPath, bool searchChildren)
        {
            DirectoryInfo rootInfo = System.IO.Directory.CreateDirectory(rootPath);
            DirectoryInfo targetInfo = System.IO.Directory.CreateDirectory(targetPath);
            SortFolder(rootInfo, targetInfo, searchChildren);
        }

        /// <summary>
        /// Requires that valid file paths are passed to the method.
        /// Sorts an image to a folder on the targetPath based on the image's
        /// metadata.
        /// </summary>
        public void SortImage(string imagePath, string targetPath)
        {
            FileInfo image = new FileInfo(imagePath);
            DirectoryInfo target = System.IO.Directory.CreateDirectory(targetPath);
            SortImage(image, target);
           
        }

        /// <summary>
        /// Recursively gets all files in a directory (and sub-directories) that match the given extension filter
        /// and returns the results as a list of files.
        /// </summary>
        public static List<FileInfo> GetAllFilesInDirectory(DirectoryInfo root, Regex extensionFilter, bool searchChildren)
        {
            List<FileInfo> list = new List<FileInfo>();
            if (searchChildren)
            {
                GetAllFilesInDirectory(root, list, extensionFilter);
            } else
            {
                foreach(FileInfo file in root.EnumerateFiles())
                {
                    if (extensionFilter.IsMatch(Path.GetExtension(file.FullName)))
                    {
                        list.Add(file);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Extract a single date from an image.
        /// </summary>
        public static DateTime ExtractDateFromImage(string imagePath)
        {
            return new List<DateTime>(GetDatesFromImage(imagePath, t => { return true; }))[0];
        }

        /// <summary>
        /// Recursively gets all directories in a root directory as well as child directories.
        /// </summary>
        private static List<DirectoryInfo> GetAllDirectoriesInDirectory(DirectoryInfo root)
        {
            List<DirectoryInfo> list = new List<DirectoryInfo>();
            GetAllDirectoriesInDirectory(root, list);
            return list;
        }

        private void SortFolder(DirectoryInfo rootInfo, DirectoryInfo targetInfo, bool searchChildren)
        {
            foreach (FileInfo file in GetAllFilesInDirectory(rootInfo, CreateRegex(this.Extensions), searchChildren))
            {
                ///TODO: Improve this method by simply returning null date if there is no metadata.
                try
                {
                    TransferFile(GetDestFolderFromDate(file, targetInfo), file);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.Error.WriteLine("Encountered a file with no metadata.");
                    this.unableToSort.Add(file.FullName);
                }
            }
        }

        private void SortImage(FileInfo image, DirectoryInfo target)
        {
            TransferFile(GetDestFolderFromDate(image, target), image);
        }

        private Regex CreateRegex(params string[] optionals)
        {
            string reg = "^";
            for(int i = 0; i < optionals.Length-1; i++)
            {
                string s = optionals[i];
                reg += "(" + s + ")|";
            }
            reg += optionals[optionals.Length - 1] + ")$";
            return new Regex(reg, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Returns the folder that the file should be sorted into based on Metadata date/time value inside of targetRoot.
        /// Also adds the mapping of the file -> date to this object's dateMap.
        /// </summary>
        private DirectoryInfo GetDestFolderFromDate(FileInfo file, DirectoryInfo targetRoot)
        {
            DateTime date = ExtractDateFromImage(file.FullName);
            this.dateMap.Add(date, file.FullName);
            DirectoryInfo year = targetRoot.CreateSubdirectory(date.Year.ToString());
            DirectoryInfo month = year.CreateSubdirectory(date.Month.ToString());
            return month;
        }

        /// <summary>
        /// Handles the transfer logic for the file and adds the mapping of the file to this object's pathMap.
        /// </summary>
        private void TransferFile(DirectoryInfo destFolder, FileInfo file)
        {
            string newFilePath = destFolder.FullName + "/" + file.Name;

            if(AllowDuplicates)
            {
                string rename;
                if(DuplicateExists(newFilePath, out rename))
                {
                    this.duplicateMap.Add(newFilePath, rename);
                    newFilePath = rename;
                }
            }

            file.CopyTo(newFilePath, true);

            if (!CopyFiles)
            {
                file.Delete();
            }

            this.pathMap.Add(file.FullName, newFilePath);
        }

        /// <summary>
        /// Checks for a duplicate file name and if one is found, attempts to increment the file's name until it is no longer a duplicate.
        /// Returns the new file name.
        /// </summary>
        private bool DuplicateExists(string path, out string suggestedName)
        {
            int increment = 1;
            bool flag = false;
            while (File.Exists(path))
            {
                flag = true;
                path = Regex.Replace(path, Path.GetFileName(path), Path.GetFileNameWithoutExtension(path) + "_" + increment + Path.GetExtension(path));

            }
            suggestedName = path;
            return flag;
        }

        /// <summary>
        /// Recursively finds all directories contained in the root, including children.
        /// </summary>
        private static void GetAllDirectoriesInDirectory(DirectoryInfo root, List<DirectoryInfo> container)
        {
            foreach(DirectoryInfo dir in root.EnumerateDirectories())
            {
                container.Add(dir);
                GetAllDirectoriesInDirectory(dir, container);
            }
        }

        /// <summary>
        /// Recursive method, adds any file matching the extension filter to the container.
        /// </summary>
        private static void GetAllFilesInDirectory(DirectoryInfo root, List<FileInfo> container, Regex extensionFilter)
        {
            foreach (FileInfo file in root.EnumerateFiles())
            {
                if (extensionFilter.IsMatch(Path.GetExtension(file.FullName)))
                {
                    container.Add(file);
                }
            }

            foreach (DirectoryInfo dir in root.EnumerateDirectories())
            {
                GetAllFilesInDirectory(dir, container, extensionFilter);
            }
        }

        /// <summary>
        /// Returns the exif date data in the given image.
        /// </summary>
        private static IEnumerable<DateTime> GetDatesFromImage(string imagePath, Func<Tag, bool> filter)
        {
            List<DateTime> dates = new List<DateTime>();
            IEnumerable<MetadataExtractor.Directory> dirs = ExtractExifDirectories(imagePath);
            foreach(MetadataExtractor.Directory dir in dirs)
            {
                foreach(Tag tag in ExtractExIfDateTags(dir))
                {
                    if (filter(tag))
                    {
                        dates.Add(ConvertTagToDate(tag));
                    }
                }
            }
            return dates;
        }

        /// <summary>
        /// Converts a tag containing a date into a DateTime value.
        /// </summary>
        private static DateTime ConvertTagToDate(Tag tag)
        {
            string dateMatch = @"[1-9][0-9]{3}:[0-9]{2}:[0-9]{2}";
            string match = Regex.Match(tag.Description, dateMatch).ToString();
            string[] arr = match.Split(':');
            return new DateTime(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
        }

        /// <summary>
        /// Requires exifDirectory is an actual metadata exif directory.
        /// Attempts to return the tags that have 'date' in their names
        /// </summary>
        private static IEnumerable<Tag> ExtractExIfDateTags(MetadataExtractor.Directory exifDirectory)
        {
            return ExtractDirectoryTags(exifDirectory, n => { return n.Name.ToLower().Contains("date"); });
        }

        /// <summary>
        /// Extracts all tags with names matching the filter from the directory and returns an enumerable of those tags.
        /// </summary>
        private static IEnumerable<Tag> ExtractDirectoryTags(MetadataExtractor.Directory dir, Func<Tag, bool> filter)
        {
            List<Tag> tags = new List<Tag>();
            foreach (Tag t in dir.Tags)
            {
                if (filter(t))
                {
                    tags.Add(t);
                }
            }
            return tags;
        }

        private static IEnumerable<MetadataExtractor.Directory> ExtractExifDirectories(string imagePath)
        {
            return ExtractDirectories(imagePath, d => { return d.Name.ToLower().Contains("exif"); });
        }

        private static IEnumerable<MetadataExtractor.Directory> ExtractDirectories(string imagePath, Func<MetadataExtractor.Directory, bool> filter)
        {
            List<MetadataExtractor.Directory> list = new List<MetadataExtractor.Directory>();
            foreach(MetadataExtractor.Directory dir in ImageMetadataReader.ReadMetadata(imagePath))
            {
                if (filter(dir))
                {
                    list.Add(dir);
                }
            }
            return list;
        }
    }
}
