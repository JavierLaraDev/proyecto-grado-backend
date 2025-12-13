using ApiGrado.Modelos;
using ApiGrado.Modelos.Dtos;
using ApiGrado.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGrado.Controllers
{
    [Route("api/accesorios")]
    [ApiController]
    public class AccesoriosController : ControllerBase
    {
        private readonly IAccesorioRepositorio _postRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<AccesoriosController> _logger;

        public AccesoriosController(IAccesorioRepositorio postRepo, IMapper mapper, ILogger<AccesoriosController> logger)
        {
            _postRepo = postRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAccesorios()
        {
            var lista = _postRepo.GetAccesorios();
            return Ok(_mapper.Map<IEnumerable<AccesorioDto>>(lista));
        }

        [AllowAnonymous]
        [HttpGet("{postId:int}", Name = "GetAccesorio")]
        public IActionResult GetAccesorio(int postId)
        {
            var item = _postRepo.GetAccesorio(postId);
            if (item == null)
                return NotFound();

            return Ok(_mapper.Map<AccesorioDto>(item));
        }

        [HttpPost]
        public IActionResult CrearAccesorio([FromBody] AccesorioCrearDto crearDto)
        {
            if (crearDto == null)
                return BadRequest();

            if (_postRepo.ExisteAccesorio(crearDto.Descripcion))
                return BadRequest("El accesorio ya existe");

            var accesorio = _mapper.Map<Accesorio>(crearDto);
            accesorio.RutaImagen = crearDto.RutaImagen;

            if (!_postRepo.CrearAccesorio(accesorio))
                return StatusCode(500, "Error al guardar el accesorio");

            return CreatedAtRoute("GetAccesorio", new { postId = accesorio.Id }, accesorio);
        }

        [HttpPatch("{accesorioId:int}")]
        public IActionResult ActualizarPatchAccesorio(int accesorioId, [FromBody] AccesorioActualizarDto actualizarDto)
        {
            if (actualizarDto == null || accesorioId != actualizarDto.Id)
                return BadRequest();

            var accesorioExistente = _postRepo.GetAccesorio(accesorioId);
            if (accesorioExistente == null)
                return NotFound();

            _mapper.Map(actualizarDto, accesorioExistente);

            if (!string.IsNullOrEmpty(actualizarDto.RutaImagen))
                accesorioExistente.RutaImagen = actualizarDto.RutaImagen;

            if (!_postRepo.ActualizarAccesorio(accesorioExistente))
                return StatusCode(500, "Error al actualizar");

            return NoContent();
        }

        [HttpDelete("{accesorioId:int}")]
        public IActionResult BorrarAccesorio(int accesorioId)
        {
            var accesorio = _postRepo.GetAccesorio(accesorioId);
            if (accesorio == null)
                return NotFound();

            if (!_postRepo.BorrarAccesorio(accesorio))
                return StatusCode(500, "Error al borrar");

            return NoContent();
        }
    }
}
