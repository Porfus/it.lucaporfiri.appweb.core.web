using it.lucaporfiri.appweb.core.web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace it.lucaporfiri.appweb.core.web.Data
{
    public class ContestoApp : IdentityDbContext
    {
        public ContestoApp(DbContextOptions<ContestoApp> options)
            : base(options)
        {
        }

        public DbSet<Abbonamento> Abbonamento { get; set; } = default!;
        public DbSet<Scheda> Scheda { get; set; } = default!;
        public DbSet<Atleta> Atleta { get; set; } = default!;
        public DbSet<Evento> Eventi { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Atleta)             
                .WithOne(a => a.ApplicationUser)   
                .HasForeignKey<Atleta>(a => a.ApplicationUserId); 
        }
    }
}
