using ApiGrado.Modelos.Dtos;
using ApiGrado.Modelos;
using Microsoft.Extensions.Hosting;

namespace ApiGrado.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        ICollection<Usuario> GetUsuarios();
        Usuario GetUsuario(int usuarioId);
        bool IsUniqueUser(string usuario);
        Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto);
        bool ActualizarUsuario(Usuario usuario);
        bool BorrarUsuario(Usuario usuario);
        bool Guardar();
    }
}
