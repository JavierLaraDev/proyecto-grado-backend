using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos.Dtos
{
    public class PedidoCrearDto
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public string ColorBicicleta { get; set; }

        public double PrecioTotal { get; set; }

        public List<PedidoItemCrearDto> Items { get; set; }
    }
}
