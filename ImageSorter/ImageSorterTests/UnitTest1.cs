using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageSorter;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ImageSorterTests
{
    #region Program Model Tests

    [TestClass]
    public class ModelTests
    {
        private const string TEST_PHOTO_ROOT = "../../TestPhotos/";

        /*
        [TestMethod]
        public void TestExtractImageExIfs()
        {
            Model model = new Model();
            List<MetadataExtractor.Directory> list = new List<MetadataExtractor.Directory>(Model.ExtractExifDirectories(TEST_PHOTO_ROOT + "Apple iPhone 4.jpg"));
            Assert.AreEqual("Exif IFD0", list[0].Name);
            Assert.AreEqual("Exif SubIFD", list[1].Name);
        }
        */

        /*
        [TestMethod]
        public void TestExtractDates()
        {
            List<DateTime> list = new List<DateTime>(Model.GetDatesFromImage(TEST_PHOTO_ROOT+"Apple iPhone 4.jpg", n=> { return true; }));
            DateTime date = new DateTime(2011, 1, 13);
            Assert.IsTrue(date.Equals(list[0]));
            Assert.IsTrue(date.Equals(list[1]));
            Assert.IsTrue(date.Equals(list[2]));

            list = new List<DateTime>(Model.GetDatesFromImage(TEST_PHOTO_ROOT + "Canon EOS REBEL SL1.jpg", n => {
                Console.WriteLine(n.Name + " " + n.Description);
                return true;
            }));
            date = new DateTime(2013, 6, 30);
            Assert.IsTrue(date.Equals(list[0]));
            Assert.IsTrue(date.Equals(list[1]));
            Assert.IsTrue(date.Equals(list[2]));
        }
        */
    
        [TestMethod]
        public void TestCreateDirectory()
        {
            System.IO.Directory.CreateDirectory("test");
            Assert.IsTrue(System.IO.Directory.Exists("test"));
            System.IO.Directory.Delete("test");
            Assert.IsFalse(System.IO.Directory.Exists("test"));

            System.IO.DirectoryInfo info = System.IO.Directory.CreateDirectory("test");
            info = System.IO.Directory.CreateDirectory("test");
            Assert.IsTrue(System.IO.Directory.Exists("test"));
            System.IO.Directory.Delete("test");
            Assert.IsFalse(System.IO.Directory.Exists("test"));
        }

        [TestMethod]
        public void TestGetAllImages()
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(TEST_PHOTO_ROOT);
            List<System.IO.FileInfo> list = Sorter.GetAllFilesInDirectory(info, new Regex(@"^.((jpg)|(png)|(img)|(gif)|()|())$", RegexOptions.IgnoreCase), true);
            Assert.AreEqual("Apple iPhone 4.jpg", list[0].Name);
        }

        /*
        [TestMethod]
        public void TestGetAllDirectories()
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(TEST_PHOTO_ROOT);
            List<System.IO.DirectoryInfo> list = Model.GetAllDirectoriesInDirectory(info);
            Assert.AreEqual(3, list.Count);
        }
        */

        [TestMethod]
        public void TestSortFolder()
        {
            Sorter model = new Sorter();
            model.AllowDuplicates = true;
            model.CopyFiles = true;
            model.SortFolder("../../TestPhotos - Copy/", "C:/users/ccarlson/desktop/sorted");
        }

        [TestMethod]
        public void TestSortFolderThreaded()
        {
            Sorter model = new Sorter();
            model.AllowDuplicates = true;
            model.CopyFiles = true;
            model.SortFolderThreadSafe("../../TestPhotos - Copy/", "C:/users/ccarlson/desktop/sorted",2);
        }

    }

    #endregion
}
