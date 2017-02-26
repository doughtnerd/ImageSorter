using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageSorter;
using System.Collections.Generic;

namespace ImageSorterTests
{
    #region Program Model Tests

    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void TestImageSortOptions()
        {
            Model model = new Model();
            Assert.AreEqual(new Model.ImageSortOptions(0) , model.SortOptions);

            model.SortOptions = 0x1;
            Assert.AreEqual(new Model.ImageSortOptions(1), model.SortOptions);

            model.SortOptions = 0x10;
            Assert.AreEqual(new Model.ImageSortOptions(16), model.SortOptions);

            model.SortOptions = 0x11;
            Assert.AreEqual(new Model.ImageSortOptions(17), model.SortOptions);

            model.SortOptions = 0x1 | 0x10;
            Assert.AreEqual(new Model.ImageSortOptions(17), model.SortOptions);

            model.SortOptions = 0x1 & 0x11;
            Assert.AreEqual(new Model.ImageSortOptions(1), model.SortOptions);

            model.SortOptions = 0x1 & 0x12;
            Assert.AreEqual(new Model.ImageSortOptions(0), model.SortOptions);
        }

        [TestMethod]
        public void TestExtractImageExIfs()
        {
            Model model = new Model();
            List<MetadataExtractor.Directory> list = new List<MetadataExtractor.Directory>(model.ExtractExIfDirectories("../../Apple iPhone 4.jpg"));
            Assert.AreEqual("Exif IFD0", list[0].Name);
            Assert.AreEqual("Exif SubIFD", list[1].Name);
        }

    }

    #endregion
}
