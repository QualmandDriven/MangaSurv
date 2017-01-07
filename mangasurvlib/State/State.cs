using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mangasurvlib
{
    public enum State : int
    {
        Downloadable = 1,
        Downloading = 2,
        Complete = 3,
    }
}
