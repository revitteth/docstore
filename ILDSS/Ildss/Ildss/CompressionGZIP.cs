using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace Ildss
{
    class CompressionGZIP : ICompression
    {
        public void Compress(string inFile, string outFile)
        {
            Stream gs = null, infs = null, outfs = null;

            try
            {
                infs = new FileStream(inFile, FileMode.Open);
                outfs = new FileStream(inFile + ".gz", FileMode.Create);
                gs = new GZipStream(outfs, CompressionMode.Compress);
                infs.CopyTo(gs);
            }
            finally
            {
                if (gs != null)
                    gs.Close();
                if (outfs != null)
                    outfs.Close();
                if (infs != null)
                    infs.Close();
            }
        }

        public void Decompress(string inFile, string outFile)
        {
            // Decompress file
            Stream gs = null, infs = null, outfs = null;

            try
            {
                infs = new FileStream(inFile, FileMode.Open);
                outfs = new FileStream(inFile.Replace(".gz", ""), FileMode.Create);
                gs = new GZipStream(outfs, CompressionMode.Decompress);
                infs.CopyTo(gs);
            }
            finally
            {
                if (gs != null)
                    gs.Close();
                if (outfs != null)
                    outfs.Close();
                if (infs != null)
                    infs.Close();
            }
        }
    }
}
