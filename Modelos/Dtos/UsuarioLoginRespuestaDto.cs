namespace ApiGrado.Modelos.Dtos
{
    public class UsuarioLoginRespuestaDto
    {
        public Usuario Usuario { get; set; }
        public string Token { get; set; }
        public RolEnum RolEnum { get; set; }
    }
}
