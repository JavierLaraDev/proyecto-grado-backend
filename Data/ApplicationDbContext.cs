using ApiGrado.Modelos;
using Microsoft.EntityFrameworkCore;

namespace ApiGrado.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Accesorio> Accesorios { get; set; }
        public DbSet<PedidosCompras> PedidosCompras { get; set; }
        public DbSet<PedidosItems> PedidosItems { get; set; }

    }
}
