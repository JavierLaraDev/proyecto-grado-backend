using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos.Dtos
{
    public class PedidosItemsDto
    {
        public int Id { get; set; }
        [Required]
        public int AccesorioId { get; set; }
        public AccesorioDto Accesorio { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }
        
    }
}