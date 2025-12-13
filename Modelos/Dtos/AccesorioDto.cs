using System.ComponentModel.DataAnnotations;
namespace ApiGrado.Modelos.Dtos
{
    public class AccesorioDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "el nombre del accesorio es requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "el costo del accesorio es requerido")]
        public double Costo { get; set; }
        [Required(ErrorMessage = "el precio del accesorio es requerido")]
        public double Precio { get; set; }
        [Required(ErrorMessage = "la descripcion del accesorio es requerido")]
        public string Descripcion { get; set; }
        public string? RutaImagen { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActualizacion { get; set; }
    }
}
