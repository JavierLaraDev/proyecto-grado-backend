using System.ComponentModel.DataAnnotations;

namespace ApiGrado.Modelos.Dtos
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "El Usuario es obligatorio")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "La Contraseña es obligatoria")]
        public string Password { get; set; }
        public RolEnum Rol { get; set; }
    }
}
