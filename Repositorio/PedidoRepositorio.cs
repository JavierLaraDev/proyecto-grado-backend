using ApiGrado.Data;
using ApiGrado.Modelos;
using ApiGrado.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace ApiGrado.Repositorio
{
    public class PedidoRepositorio : IPedidosRepositorio
    {
        private readonly ApplicationDbContext _bd;
        public PedidoRepositorio(ApplicationDbContext bd)
        {
            _bd = bd;
        }
        public bool AgregarPedido(PedidosCompras pedido)
        {
            if (!_bd.Usuarios.Any(u => u.Id == pedido.UsuarioId))
            {
                throw new ArgumentException($"El usuario con Id {pedido.UsuarioId} no existe.");
            }

            // No necesitamos establecer PedidosComprasId manualmente
            // Entity Framework se encargará de esto automáticamente

            _bd.PedidosCompras.Add(pedido);
            return Guardar();
        }
        public PedidosCompras GetPedido(int usuarioId)
        {
            return _bd.PedidosCompras
                .Include(c => c.Items)
                .ThenInclude(i => i.Accesorio)
                .FirstOrDefault(c => c.UsuarioId == usuarioId);
        }

        public PedidosCompras GetPedidoPorId(int carritoId)
        {
            return _bd.PedidosCompras
                .Include(c => c.Items)
                .ThenInclude(i => i.Accesorio)
                .FirstOrDefault(c => c.Id == carritoId);
        }
        public bool ActualizarPedido(PedidosCompras pedido)
        {
            var pedidoExistente = _bd.PedidosCompras
            .Include(c => c.Items)
            .FirstOrDefault(c => c.Id == pedido.Id);

            if (pedidoExistente == null)
            {
                return false;
            }

            // Actualizar solo los campos que deben cambiar
            pedidoExistente.Estado = pedido.Estado;
            pedidoExistente.PrecioTotal = pedido.PrecioTotal;

            // No modificamos los Items existentes
            // Si hay nuevos Items, los agregamos sin eliminar los existentes
            if (pedido.Items != null && pedido.Items.Any())
            {
                foreach (var nuevoItem in pedido.Items)
                {
                    if (!pedidoExistente.Items.Any(i => i.AccesorioId == nuevoItem.AccesorioId))
                    {
                        pedidoExistente.Items.Add(nuevoItem);
                    }
                    else
                    {
                        // Actualizar cantidad si el item ya existe
                        var itemExistente = pedidoExistente.Items.First(i => i.AccesorioId == nuevoItem.AccesorioId);
                        itemExistente.Cantidad = nuevoItem.Cantidad;
                    }
                }
            }

            _bd.PedidosCompras.Update(pedidoExistente);
            return Guardar();
        }
        public List<PedidosCompras> GetPedidos()
        {
            return _bd.PedidosCompras
                .Include(c => c.Items)
                .ThenInclude(i => i.Accesorio)
                .Include(c => c.Usuario)
                .ToList();
        }
        public ICollection<PedidosCompras> GetPedidosUsuario(int usuarioId)
        {
            return _bd.PedidosCompras
                .Include(p => p.Items)
                    .ThenInclude(i => i.Accesorio)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Id)
                .ToList();
        }
        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0;
        }
    }
}
