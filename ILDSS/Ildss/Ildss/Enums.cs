using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    public class Enums
    {
        public enum DocStatus { Indexed, Current, Archived, Permanent };
        public enum EventType { Read, Write, Create, Rename };
    }
}
