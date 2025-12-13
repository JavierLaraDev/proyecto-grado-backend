using ApiGrado.Modelos;
using ApiGrado.Modelos.Dtos;
using ApiGrado.Repositorio;
using ApiGrado.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiGrado.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usRepo;
        protected RespuestasAPI _respuestaApi;
        private readonly IMapper _mapper;
        public UsuariosController(IUsuarioRepositorio usRepo, IMapper mapper)
        {
            _usRepo = usRepo;
            _mapper = mapper;
            this._respuestaApi = new();
        }
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {
            bool validarNombreUsuarioUnico = _usRepo.IsUniqueUser(usuarioRegistroDto.NombreUsuario);
            if (!validarNombreUsuarioUnico)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestaApi);
            }

            var usuario = await _usRepo.Registro(usuarioRegistroDto);
            if (usuario == null)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("Error en el registro");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            return Ok(_respuestaApi);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto)
        {
            var respuestaLogin = await _usRepo.Login(usuarioLoginDto);
            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario o password son incorrectos");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            _respuestaApi.Result = respuestaLogin;
            return Ok(_respuestaApi);
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usRepo.GetUsuarios();

            var listaUsuariosDto = new List<UsuarioDto>();

            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(lista));
            }
            return Ok(listaUsuariosDto);
        }

        [HttpGet("{usuarioId:int}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsuario(int usuarioId)
        {
            var itemUsuario = _usRepo.GetUsuario(usuarioId);

            if (itemUsuario == null)
            {
                return NotFound();
            }

            var itemUsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);
            return Ok(itemUsuarioDto);
        }
        [HttpPut("{usuarioId:int}", Name = "ActualizarUsuario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarUsuario(int usuarioId, [FromBody] UsuarioActualizarDto usuarioActualizarDto)
        {
            var respuesta = new RespuestasAPI();
            if (usuarioActualizarDto == null || usuarioId != usuarioActualizarDto.Id)
            {
                respuesta.StatusCode = HttpStatusCode.BadRequest;
                respuesta.IsSuccess = false;
                respuesta.ErrorMessages.Add("Datos inválidos");
                return BadRequest(respuesta);
            }
            var usuario = _usRepo.GetUsuario(usuarioId);
            if (usuario == null)
            {
                respuesta.StatusCode = HttpStatusCode.NotFound;
                respuesta.IsSuccess = false;
                respuesta.ErrorMessages.Add("Usuario no encontrado");
                return NotFound(respuesta);
            }

            usuario.Nombre = usuarioActualizarDto.Nombre;
            usuario.Apellido = usuarioActualizarDto.Apellido;
            usuario.NumeroCelular = usuarioActualizarDto.NumeroCelular;
            usuario.Direccion = usuarioActualizarDto.Direccion;
            usuario.Email = usuarioActualizarDto.Email;

            // Actualiza la contraseña solo si se proporciona una nueva
            if (!string.IsNullOrEmpty(usuarioActualizarDto.Password))
            {
                usuario.Password = UsuarioRepositorio.obtenermd5(usuarioActualizarDto.Password);
            }

            if (!_usRepo.ActualizarUsuario(usuario))
            {
                respuesta.StatusCode = HttpStatusCode.InternalServerError;
                respuesta.IsSuccess = false;
                respuesta.ErrorMessages.Add($"Algo salió mal actualizando el usuario {usuario.NombreUsuario}");
                return StatusCode(500, respuesta);
            }
            respuesta.StatusCode = HttpStatusCode.NoContent;
            respuesta.IsSuccess = true;
            return NoContent();
        }

        [HttpDelete("{usuarioId:int}", Name = "BorrarUsuario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarUsuario(int usuarioId)
        {
            var respuesta = new RespuestasAPI();

            if (usuarioId == 0)
            {
                respuesta.StatusCode = HttpStatusCode.BadRequest;
                respuesta.IsSuccess = false;
                respuesta.ErrorMessages.Add("Datos inválidos");
                return BadRequest(respuesta);
            }

            var usuario = _usRepo.GetUsuario(usuarioId);
            if (usuario == null)
            {
                respuesta.StatusCode = HttpStatusCode.NotFound;
                respuesta.IsSuccess = false;
                respuesta.ErrorMessages.Add("Usuario no encontrado");
                return NotFound(respuesta);
            }

            if (!_usRepo.BorrarUsuario(usuario))
            {
                respuesta.StatusCode = HttpStatusCode.InternalServerError;
                respuesta.IsSuccess = false;
                respuesta.ErrorMessages.Add($"Algo salió mal borrando el usuario {usuario.NombreUsuario}");
                return StatusCode(500, respuesta);
            }

            respuesta.StatusCode = HttpStatusCode.NoContent;
            respuesta.IsSuccess = true;
            return NoContent();
        }
    }
}
