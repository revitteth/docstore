using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudInterface
{
    public interface ICloudManager
    {
        void CreateBucketIfNotExists();
        void CreateBucketIfNotExists(string name);
    }
}
