using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ildss;
using System.IO;

namespace IldssTest
{
    [TestClass]
    public class CompressionTest
    {
        [TestMethod]
        public void CompressFile()
        {
            var file = new FileInfo(@"F:\compress.txt.gz");
            CompressionGZIP.Compress(file);
        }

        [TestMethod]
        public void DecompressFile()
        {
            var file = new FileInfo(@"F:\compress.txt.gz");
            CompressionGZIP.Decompress(file);
        }
    }
}
