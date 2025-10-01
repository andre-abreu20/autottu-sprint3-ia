using Microsoft.EntityFrameworkCore;
using AutoTTU.Models;

namespace AutoTTU.Connection
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Motos> Motos { get; set; }
        public DbSet<Slot> Slot { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Checkin> Checkin { get; set; }
        public DbSet<Telemetria> Telemetria { get; set; }

    }
}