using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos
{
    public class PedidosCompras
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
        public List<PedidosItems> Items { get; set; } = new List<PedidosItems>();
        public double PrecioTotal { get; set; }
        public EstadoPedido Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Required]
        public string ColorBicicleta { get; set; }
    }
}
