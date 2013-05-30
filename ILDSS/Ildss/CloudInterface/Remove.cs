using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudInterface
{
    public static class Remove
    {
        public static Task RemoveLocal(List<string> files)
        {
            return Task.Run(() =>
                {
                    // remove the list of files from local disk
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                });
        }

        public static void RemoveCloud(List<string> files)
        {
            // not needed for project implementation (beyond scope)
        }

    }
}
