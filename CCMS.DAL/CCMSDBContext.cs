using System;
using System.Collections.Generic;
using System.Linq;
using CCMS.DAL.Entities;
using CCMS.DAL.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace CCMS.DAL
{
    public class CCMSDBContext : DbContext
    {
        public CCMSDBContext(DbContextOptions<CCMSDBContext> options)
            : base(options)
        {
        }

        public DbSet<RepositoryEntity> Repositories { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<ConventionEntity> Conventions { get; set; }
        public DbSet<UpdatedTextEntity> ConventionTexts { get; set; }
        public DbSet<PatchEntity> Patches { get; set; }
        public DbSet<CommentEntity> Comment { get; set; }
        public DbSet<RepositoryCheckEntity> RepositoryChecks { get; set; }
        public DbSet<ConventionCheckEntity> ConventionChecks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
        }
    }
}