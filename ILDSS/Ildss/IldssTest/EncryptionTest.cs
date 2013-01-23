using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ildss;

namespace IldssTest
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void TestEncryptFileWithoutCompression()
        {
            // Create two identical files (for the test)
            string filePath = @"F:\test.txt";
            string newFile = @"F:\test_enc.txt";
            //System.IO.StreamWriter file = new System.IO.StreamWriter(filePath);
            //file.WriteLine("Test text");
            //file.Close();

            

            Ildss.EncryptionAES enc = new Ildss.EncryptionAES();
            enc.EncryptFile(filePath, newFile, false);

        }

        [TestMethod]
        public void TestEncryptFileWithCompression()
        {
           // Ildss.EncryptionAES enc = new Ildss.EncryptionAES();
           // enc.EncryptFile(@"F:\TestDir\New.txt", @"F:\TestDir\New(encrypted).txt", false);
        }
    }
}
