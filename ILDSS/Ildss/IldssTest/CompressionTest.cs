using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using Ildss;
using Ildss.Compression;

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
