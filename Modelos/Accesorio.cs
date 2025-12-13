using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos
{
    public class Accesorio
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public double Costo { get; set; }
        [Required]
        public double Precio { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public string? RutaImagen { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActualizacion { get; set; }
    }
}