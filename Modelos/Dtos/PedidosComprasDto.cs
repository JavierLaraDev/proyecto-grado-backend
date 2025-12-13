using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos.Dtos
{
    public class PedidosComprasDto
    {
        public int Id { get; set; }
        [Required]
        public int UsuarioId { get; set; }
        public List<PedidosItemsDto> Items { get; set; } = new List<PedidosItemsDto>();
        public DateTime FechaCreacion { get; set; }
        public double PrecioTotal { get; set; }
        public EstadoPedido Estado { get; set; }
        [Required]
        public string ColorBicicleta { get; set; }
    }
}