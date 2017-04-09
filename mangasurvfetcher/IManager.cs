using mangasurvlib.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mangasurvlib
{
    public interface IManager
    {
        void ConfigureApiController(string sToken);
    }
}
