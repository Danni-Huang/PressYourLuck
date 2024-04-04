using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PressYourLuck.Helpers;

namespace PressYourLuck.Models
{
    public class AuditContext : DbContext
    {
        public AuditContext(DbContextOptions options) : base(options)
        {
        }
        /// <summary>
        /// A way of accessing the AuditType table in DB
        /// </summary>
        public DbSet<AuditType> AuditTypes { get; set; }

        /// <summary>
        /// A way of accessing the Audit table in DB
        /// </summary>
        public DbSet<Audit> Audits { get; set; }

        /// <summary>
        /// Doing some DB initialization
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //add some Audit types
            modelBuilder.Entity<AuditType>().HasData(
                new AuditType() { AuditTypeId = 1, Name = "Cash In" },
                new AuditType() { AuditTypeId = 2, Name = "Cash Out" },
                new AuditType() { AuditTypeId = 3, Name = "Win" },
                new AuditType() { AuditTypeId = 4, Name = "Lose" }
                );
        }

        private static List<Audit> _audits = new List<Audit>();
       
        /// <summary>
        /// the method is getting audits by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Audit> GetAuditsByType(string type)
        {
            return _audits.Where(t => t.AuditType.Name == type).ToList();
        }
    }
}
