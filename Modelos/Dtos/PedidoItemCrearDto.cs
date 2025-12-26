using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos.Dtos
{
    public class PedidoItemCrearDto
    {
        [Required]
        public int AccesorioId { get; set; }

        [Required]
        public int Cantidad { get; set; }
    }
}
