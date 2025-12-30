using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos.Dtos
{
    public class PedidosComprasDto
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public double PrecioTotal { get; set; }

        public EstadoPedido Estado { get; set; }

        public string ColorBicicleta { get; set; }

        public List<PedidosItemsDto> Items { get; set; } = new();
    }
}