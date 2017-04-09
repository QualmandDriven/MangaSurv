using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mangasurvfetcher.Helper
{
    interface IArguments
    {
        bool ContainsKey(string sKey);
        string GetValue(string sKey);
    }
}
