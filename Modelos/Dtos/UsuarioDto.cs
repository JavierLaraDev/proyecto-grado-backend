namespace ApiGrado.Modelos.Dtos
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int NumeroCelular { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public RolEnum Rol { get; set; }
    }
}
