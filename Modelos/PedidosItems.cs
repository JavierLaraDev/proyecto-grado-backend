using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos
{
    public class PedidosItems
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AccesorioId { get; set; }
        [ForeignKey("AccesorioId")]
        public Accesorio Accesorio { get; set; }
        [Required]
        public int Cantidad { get; set; }
    }
}
