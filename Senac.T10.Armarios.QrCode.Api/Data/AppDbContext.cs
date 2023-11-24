using Microsoft.EntityFrameworkCore;
using Senac.T10.Armarios.QrCode.Api.Models;

namespace Senac.T10.Armarios.QrCode.Api.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Armario> Armarios { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opcoes) : base(opcoes)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
