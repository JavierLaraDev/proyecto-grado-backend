using ApiGrado.Data;
using ApiGrado.Modelos.Dtos;
using ApiGrado.Modelos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;
using ApiGrado.Repositorio.IRepositorio;
using Microsoft.Extensions.Hosting;

namespace ApiGrado.Repositorio
{
    public class UsuarioRepositorio: IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _bd;
        private string claveSecreta;
        public UsuarioRepositorio(ApplicationDbContext bd, IConfiguration config)
        {
            _bd = bd;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");

        }
        public Usuario GetUsuario(int usuarioId)
        {
            return _bd.Usuarios.FirstOrDefault(c => c.Id == usuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _bd.Usuarios.OrderBy(c => c.Id).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuariobd = _bd.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuario);
            if (usuariobd == null)
            {
                return true;
            }

            return false;
        }
        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);
            var usuario = _bd.Usuarios.FirstOrDefault(
                u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                && u.Password == passwordEncriptado
                );
            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Usuario = null,
                    Token = ""
                };
            }    
            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = manejadorToken.CreateToken(tokenDescriptor);
            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Usuario = usuario,
                Token = manejadorToken.WriteToken(token)
            };

            return usuarioLoginRespuestaDto;
        }

        public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);
            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Nombre = usuarioRegistroDto.Nombre,
                Apellido = usuarioRegistroDto.Apellido,
                NumeroCelular = usuarioRegistroDto.NumeroCelular,
                Direccion = usuarioRegistroDto.Direccion,
                Email = usuarioRegistroDto.Email,
                Password = passwordEncriptado,  // Encriptado aquí
                Rol = usuarioRegistroDto.Rol
            };
            _bd.Usuarios.Add(usuario);
            await _bd.SaveChangesAsync();
            return usuario;
        }
        public static string obtenermd5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }
        public bool ActualizarUsuario(Usuario usuario)
        {
            _bd.Usuarios.Update(usuario);
            return Guardar();
        }

        public bool BorrarUsuario(Usuario usuario)
        {
            _bd.Usuarios.Remove(usuario);
            return Guardar();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }
    }
}
