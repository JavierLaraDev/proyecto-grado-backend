using ApiGrado.Modelos;
using ApiGrado.Modelos.Dtos;

namespace ApiGrado.Repositorio.IRepositorio
{
    public interface IPedidosRepositorio
    {
        bool AgregarPedido(PedidosCompras carrito);
        PedidosCompras GetPedido(int usuarioId);
        PedidosCompras GetPedidoPorId(int carritoId);
        bool ActualizarPedido(PedidosCompras carrito);
        List<PedidosCompras> GetPedidos();
        ICollection<PedidosCompras> GetPedidosUsuario(int usuarioId);
        bool Guardar();
    }
}
