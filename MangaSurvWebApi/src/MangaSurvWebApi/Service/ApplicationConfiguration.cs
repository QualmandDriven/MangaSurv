using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Service
{
    public class ApplicationConfiguration
    {
        private static ApplicationConfiguration _appConfig = new ApplicationConfiguration();

        public static ApplicationConfiguration GetApplicationConfiguration()
        {
            return _appConfig;
        }

        public string PostgresConString { get; set; }
    }
}
