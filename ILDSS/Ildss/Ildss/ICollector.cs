﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    interface ICollector
    {
        void Collect(string path);
        void Register(string path);
    }
}
