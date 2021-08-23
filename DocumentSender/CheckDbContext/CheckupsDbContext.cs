using DocumentSender.Models.General;
using DocumentSender.Models.Lab;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSender.CheckDbContext
{
    public class CheckupsDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PostgreSQL uses the public schema by default - not dbo.
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);

            //Rename Identity tables to lowercase
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var currentTableName = modelBuilder.Entity(entity.Name).Metadata.GetDefaultTableName();
                modelBuilder.Entity(entity.Name).ToTable(currentTableName.ToLower());
            }
            base.OnModelCreating(modelBuilder);

        }
        public DbSet<LabCertDetails> LabCertDetails { get; set; }
        public DbSet<LabTestParticulars> LabTestParticulars { get; set; }
        public DbSet<DocumentsLog> DocumentsLog { get; set; }
        public DbSet<Cycle_idVM> Cycle_Ids { get; set; }
        public void SetCommandTimeOut(int timeOut)
        {
            Database.SetCommandTimeout(timeOut);
        }
        //
        public CheckupsDbContext(DbContextOptions<CheckupsDbContext> options)
           : base(options)
        {
            SetCommandTimeOut(1000);
        }
    }
}
