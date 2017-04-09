using MangaSurvWebApi.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Service
{
    public static class ApplicationDependencies
    {
        public static MangaSurvContext GetMangaSurvContext()
        {
            DbContextOptionsBuilder<MangaSurvContext> opt = new DbContextOptionsBuilder<MangaSurvContext>();
            opt.UseNpgsql(ApplicationConfiguration.GetApplicationConfiguration().PostgresConString);

            return new MangaSurvContext(opt.Options);
        }
    }
}
