﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ba2Tools
{
    public interface IBA2FileEntry
    {
        int Index { get; set; }

        char[] Extension { get; set; }
    }
}
