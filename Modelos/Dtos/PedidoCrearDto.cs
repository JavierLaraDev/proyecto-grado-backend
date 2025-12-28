using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos.Dtos
{
    public class PedidoCrearDto
    {
        [Required]
        public int UsuarioId { get; set; }

        public List<PedidoItemCrearDto> Items { get; set; } = new List<PedidoItemCrearDto>();

        public DateTime FechaCreacion { get; set; }

        public double PrecioTotal { get; set; }

        public EstadoPedido Estado { get; set; }

        [Required]
        public string ColorBicicleta { get; set; }
    }
}
