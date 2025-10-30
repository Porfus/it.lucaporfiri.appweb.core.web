using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using it.lucaporfiri.appweb.core.web.Models;

namespace it.lucaporfiri.appweb.core.web.Data
{
    public class ContestoApp : DbContext
    {
        public ContestoApp (DbContextOptions<ContestoApp> options)
            : base(options)
        {
        }

        public DbSet<Abbonamento> Abbonamento { get; set; } = default!;
        public DbSet<Scheda> Scheda { get; set; } = default!;
        public DbSet<Atleta> Atleta { get; set; } = default!;
        public DbSet<Eventi> Eventi { get; set; } = default!;
    }
}
