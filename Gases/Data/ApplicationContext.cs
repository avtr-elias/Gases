using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gases.Models;
using Microsoft.EntityFrameworkCore;

namespace Gases.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UploadModel> Files { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }
    }
}
