using InvataGermana.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvataGermana.Data
{
    class ApplicationDbContext : DbContext
    {
        public DbSet<Lesson> lessons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=de_ro.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
