﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gases.Models;

namespace Gases.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Gases.Models.Gase> Gase { get; set; }

        public DbSet<Gases.Models.NetCDF> NetCDF { get; set; }

        public DbSet<Gases.Models.Layer> Layer { get; set; }

        public DbSet<Gases.Models.GeoTiffFile> GeoTiffFile { get; set; }

        public DbSet<Gases.Models.GDataType> GDataType { get; set; }

        public DbSet<Gases.Models.Region> Region { get; set; }

        public DbSet<Gases.Models.GData> GData { get; set; }
    }
}
