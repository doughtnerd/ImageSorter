using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetadataExtractor;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace ImageSorter
{
    public class Sorter
    {
        /// <summary>
        /// Dictionary indicating which files belong to which date.
        /// </summary>
        private ConcurrentDictionary<DateTime, List<string>> dateMap = new ConcurrentDictionary<DateTime, List<string>>();

        /// <summary>
        /// Dictionary where the source image path is the key, the target sorted path is the value.
        /// </summary>
        private ConcurrentDictionary<string, FileInfo> pathMap = new ConcurrentDictionary<string, FileInfo>();

        /// <summary>
        /// Dictionary indicating renaming of any duplicate file names that were found.
        /// </summary>
        private ConcurrentDictionary<string, List<string>> duplicateMap = new ConcurrentDictionary<string, List<string>>();

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
            this.Extensions = new string[] { ".jpg", ".png", ".arw", ".cr2", ".dng"};
        }

        /// <summary>
        /// Uses multiple threads to sort all images in the rootPath into the targetPath directory using the
        /// specified number of threads.
        /// </summary>
        /// <param name="rootPath">The root folder where sorting should begin</param>
        /// <param name="targetPath">The root folder where sorted images should be placed</param>
        /// <param name="nTasks">Number of Tasks to use to sort the images</param>
        public void SortFolderThreadSafe(string rootPath, string targetPath, int nTasks)
        {
            DirectoryInfo rootInfo = System.IO.Directory.CreateDirectory(rootPath);
            DirectoryInfo targetInfo = System.IO.Directory.CreateDirectory(targetPath);

            SortAllFiles(rootInfo, targetInfo, nTasks);
            WriteAllFiles(targetInfo, nTasks);

            WriteTrackersToFile(targetPath);
            ClearTrackers();
        }

        /// <summary>
        /// Requires that valid file paths are passed to the method.
        /// Handles sorting the image files in the root folder and all sub-folders 
        /// and producing the sorted results in the target folder.
        /// </summary>
        /// <param name="rootPath">The source folder to be sorted</param>
        /// <param name="targetPath">The target folder where all sorted images should be placed</param>
        public void SortFolder(string rootPath, string targetPath)
        {
            SortFolder(rootPath, targetPath, true);
        }

        /// <summary>
        /// Requires that valid file paths are passed to the method.
        /// Handles sorting the image files in the root folder (and optionally all sub-folders)
        /// and producing the sorted results in the target folder.
        /// </summary>
        /// <param name="rootPath">The source folder to be sorted</param>
        /// <param name="targetPath">The target folder where all sorted images should be placed</param>
        /// <param name="searchChildren">True if subfolders should be included in the sort, false otherwise</param>
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
            }
            else
            {
                foreach (FileInfo file in root.EnumerateFiles())
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

        private void WriteTrackersToFile(string rootFolderPath)
        {
            WriteDictionaryToFile(rootFolderPath + @"\PathMapping.txt", pathMap);
            WriteDictionaryToFile(rootFolderPath + @"\DateMapping.txt", this.dateMap);
            WriteDictionaryToFile(rootFolderPath + @"\DuplicatesMapping.txt", this.duplicateMap);
            unableToSort.Sort();
            WriteListToFile(rootFolderPath + @"\UnableToSort.txt", this.unableToSort);
        }

        private void WriteAllFiles(DirectoryInfo targetInfo, int nTasks)
        {
            List<Task> tasks = new List<Task>();
            Queue<string> keys =new Queue<string>(pathMap.Keys);
            while (keys.Count > 0)
            {
                for (int i = 0; i < nTasks; i++)
                {
                    if (keys.Count > 0)
                    {
                        string key = keys.Dequeue();
                        FileInfo file;
                        pathMap.TryGetValue(key, out file);
                        tasks.Add(Task.Run(() => TransferFile(new FileInfo(key), file)));
                    }
                }
                Task.WaitAll(tasks.ToArray());
                tasks.Clear();
            }
        }

        private void SortAllFiles(DirectoryInfo rootInfo, DirectoryInfo targetInfo, int nTasks)
        {
            Queue<DirectoryInfo> dirs = new Queue<DirectoryInfo>(GetAllDirectoriesInDirectory(rootInfo));
            dirs.Enqueue(rootInfo);
            List<Task> tasks = new List<Task>();
            while (dirs.Count != 0)
            {
                for (int i = 0; i < nTasks; i++)
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

        private void ClearTrackers()
        {
            this.pathMap.Clear();
            this.dateMap.Clear();
            this.duplicateMap.Clear();
            this.unableToSort.Clear();
        }

        private void WriteListToFile(string path, List<string> list)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                foreach (string s in list)
                {
                    stream.WriteLine(s);
                }
                stream.Flush();
            }
        }

        private void WriteDictionaryToFile(string path, IDictionary<string, FileInfo> dic)
        {
            using(StreamWriter stream = new StreamWriter(path))
            {
                foreach(KeyValuePair<string, FileInfo> pair in dic)
                {
                    stream.WriteLine(pair.Value.FullName + " -> " + pair.Key);
                }
                stream.Flush();
            }
        }

        private void WriteDictionaryToFile(string path, IDictionary<DateTime, List<string>> dic)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                foreach (KeyValuePair<DateTime, List<string>> pair in dic)
                {
                    stream.WriteLine(pair.Key.ToString());
                    foreach(string s in pair.Value)
                    {
                        stream.WriteLine("\t" + s);
                    }
                }
                stream.Flush();
            }
        }

        private void WriteDictionaryToFile(string path, IDictionary<string, List<string>> dic)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                foreach (KeyValuePair<string, List<string>> pair in dic)
                {
                    stream.WriteLine(pair.Key);
                    foreach (string s in pair.Value)
                    {
                        stream.WriteLine("\t" + s);
                    }
                }
                stream.Flush();
            }
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
                try
                {
                    RegisterFileDestination(GetDestFolderFromDate(file, targetInfo), file);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Encountered a file with no metadata.");
                    this.unableToSort.Add(file.FullName);
                }
            }
        }

        /// <summary>
        /// Transfers an image file to the target location, based on the sorting established by the image's
        /// metadata date.
        /// </summary>
        /// <param name="image">Image to sort.</param>
        /// <param name="target">Target folder to sort into.</param>
        private void SortImage(FileInfo image, DirectoryInfo target)
        {
            TransferFile(GetDestFolderFromDate(image, target), image);
        }

        /// <summary>
        /// Creates a simple filter regex, each string passed will represent a string
        /// that is an acceptable group in the regex.
        /// </summary>
        /// <param name="optionals">String groups to use in the regex.</param>
        /// <returns>A Regex composed of the passed strings separated by |</returns>
        private Regex CreateRegex(params string[] optionals)
        {
            string reg = "^";
            for(int i = 0; i < optionals.Length-1; i++)
            {
                string s = optionals[i];
                reg += "(" + s + ")|";
            }
            reg += "("+ optionals[optionals.Length - 1] + ")$";
            return new Regex(reg, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Returns the folder that the file should be sorted into based on Metadata date/time value inside of targetRoot.
        /// Also adds the mapping of the file -> date to the dateMap.
        /// </summary>
        private DirectoryInfo GetDestFolderFromDate(FileInfo file, DirectoryInfo targetRoot)
        {
            DateTime date = ExtractDateFromImage(file.FullName);
            ManageDictionary(dateMap, date, file.FullName);
            DirectoryInfo year = targetRoot.CreateSubdirectory(date.Year.ToString());
            DirectoryInfo month = year.CreateSubdirectory(date.Month.ToString());
            return month;
        }

        /// <summary>
        /// Handles the transfer logic for the file and adds the mapping of the file to this object's pathMap.
        /// </summary>
        private void TransferFile(DirectoryInfo destFolder, FileInfo file)
        {
            string newFilePath = destFolder.FullName + "\\" + file.Name;

            file.CopyTo(newFilePath, true);

            if (!CopyFiles)
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Transfers file to the location specified by the destFile.
        /// </summary>
        /// <param name="destFile">The new file/location</param>
        /// <param name="file">The file to copy</param>
        private void TransferFile(FileInfo destFile, FileInfo file)
        {
            file.CopyTo(destFile.FullName, true);

            if (!CopyFiles)
            {
                file.Delete();
            }
        }

        private void RegisterFileDestination(DirectoryInfo destFolder, FileInfo file)
        {
            string newFilePath = destFolder.FullName + "\\" + file.Name;
            if (AllowDuplicates)
            {
                string rename;
                lock (this)
                {
                    if (DuplicateExists(newFilePath, out rename))
                    {
                        ManageDictionary(duplicateMap, newFilePath, rename);
                        newFilePath = rename;
                    }
                
                    if(!pathMap.TryAdd(newFilePath, file))
                    {
                        Console.Error.WriteLine("Failed to add file" + file.FullName + " to pathMap");
                    } else
                    {
                        Console.WriteLine("Addition of file: " + file.FullName + " successful.");
                    }
                }
            } else
            {
                pathMap.AddOrUpdate(newFilePath, file, (o, n) => { return n; });
            }
        }

        private void ManageDictionary(IDictionary<string, List<string>> dic, string key, string value)
        {
            lock (this)
            {
                if (dic.ContainsKey(key))
                {
                    List<string> list = dic[key];
                    list.Add(value);
                }
                else
                {
                    List<string> list = new List<string>();
                    list.Add(value);
                    dic.Add(key, list);
                }
            }
        }

        private void ManageDictionary(IDictionary<DateTime, List<string>> dic, DateTime key, string value)
        {
            lock (this)
            {
                if (dic.ContainsKey(key))
                {
                    List<string> list = dic[key];
                    list.Add(value);
                }
                else
                {
                    List<string> list = new List<string>();
                    list.Add(value);
                    dic.Add(key, list);
                }
            }
        }

        /// <summary>
        /// Checks for a duplicate file name and if one is found, attempts to increment the file's name until it is no longer a duplicate.
        /// Returns the new file name.
        /// </summary>
        private bool DuplicateExists(string path, out string suggestedName)
        {
            int increment = 1;
            bool flag = false;
            string pathNoExtension;
            while (pathMap.ContainsKey(path))
            {
                flag = true;
                pathNoExtension = Regex.Replace(path, Path.GetFileName(path), Path.GetFileNameWithoutExtension(path));
                if(Regex.IsMatch(pathNoExtension, @"^*_\d$"))
                {
                    pathNoExtension = Regex.Replace(pathNoExtension, @"^*_\d$", "_" + increment);
                } else
                {
                    pathNoExtension += "_" + increment;
                }
                path = pathNoExtension + Path.GetExtension(path);
                increment++;
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
                        //TODO: Maybe add error catch here?
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

            //TODO: Maybe add error catch here too?
            foreach (MetadataExtractor.Directory dir in ImageMetadataReader.ReadMetadata(imagePath))
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
