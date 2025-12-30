using ApiGrado.Modelos;
using ApiGrado.Modelos.Dtos;
using ApiGrado.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiGrado.Controllers
{
    [Route("api/pedido")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidosRepositorio _pedidoRepo;
        private readonly IMapper _mapper;
        public PedidosController(IPedidosRepositorio pedidoRepo, IMapper mapper)
        {
            _pedidoRepo = pedidoRepo;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AgregarPedido([FromBody] PedidoCrearDto dto)
        {
            try
            {
                Console.WriteLine("============================================================");
                Console.WriteLine("📥 PETICIÓN RECIBIDA: POST /api/pedido");
                Console.WriteLine("============================================================");

                if (dto == null)
                {
                    Console.WriteLine("❌ DTO ES NULL");
                    return BadRequest(new ApiErrorResponse { ErrorMessage = "El cuerpo de la petición está vacío" });
                }

                Console.WriteLine($"✅ DTO recibido:");
                Console.WriteLine($"   - UsuarioId: {dto.UsuarioId}");
                Console.WriteLine($"   - PrecioTotal: {dto.PrecioTotal}");
                Console.WriteLine($"   - ColorBicicleta: '{dto.ColorBicicleta}'");
                Console.WriteLine($"   - Items: {dto.Items?.Count ?? 0}");

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("❌ ModelState INVÁLIDO:");
                    foreach (var error in ModelState)
                    {
                        Console.WriteLine($"   Campo: {error.Key}");
                        foreach (var err in error.Value.Errors)
                        {
                            Console.WriteLine($"      - Error: {err.ErrorMessage}");
                        }
                    }

                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiErrorResponse
                    {
                        ErrorMessage = "Datos inválidos: " + string.Join("; ", errors)
                    });
                }

                if (dto.UsuarioId <= 0)
                {
                    Console.WriteLine($"❌ UsuarioId inválido: {dto.UsuarioId}");
                    return BadRequest(new ApiErrorResponse
                    {
                        ErrorMessage = "El ID de usuario debe ser mayor a 0"
                    });
                }

                Console.WriteLine($"✅ Validaciones pasadas. Mapeando DTO...");

                // Mapear de PedidoCrearDto a PedidosCompras
                var pedido = _mapper.Map<PedidosCompras>(dto);

                pedido.FechaCreacion = DateTime.UtcNow;
                pedido.Estado = EstadoPedido.Pendiente;

                Console.WriteLine($"💾 Guardando en repositorio...");

                _pedidoRepo.AgregarPedido(pedido);

                Console.WriteLine($"✅ Pedido guardado exitosamente con ID: {pedido.Id}");

                var resultado = _mapper.Map<PedidosComprasDto>(pedido);
                return CreatedAtRoute("GetPedidoPorId", new { pedidoId = pedido.Id }, resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine("============================================================");
                Console.WriteLine($"❌ EXCEPCIÓN EN AgregarPedido:");
                Console.WriteLine($"   Mensaje: {ex.Message}");
                Console.WriteLine($"   Inner Exception: {ex.InnerException?.Message ?? "N/A"}");
                Console.WriteLine($"   StackTrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("============================================================");

                return StatusCode(500, new ApiErrorResponse
                {
                    ErrorMessage = ex.InnerException?.Message ?? ex.Message
                });
            }
        }



        [HttpGet("{pedidoId:int}", Name = "GetPedidoPorId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPedidoPorId(int pedidoId)
        {
            var pedido = _pedidoRepo.GetPedidoPorId(pedidoId);
            if (pedido == null)
            {
                return NotFound();
            }
            var pedidoDto = _mapper.Map<PedidosComprasDto>(pedido);
            return Ok(pedidoDto);
        }

        [HttpGet("usuario/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPedidoPorUsuario(int userId)
        {
            var pedidos = _pedidoRepo.GetPedidosUsuario(userId);
            if (pedidos == null || !pedidos.Any())
            {
                return NotFound();
            }
            var pedidosDto = _mapper.Map<List<PedidosComprasDto>>(pedidos);
            return Ok(pedidosDto);
        }

        [HttpDelete("{pedidoId}/items/{accesorioId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult EliminarItemPedido(int pedidoId, int accesorioId)
        {
            try
            {
                var pedido = _pedidoRepo.GetPedidoPorId(pedidoId);
                if (pedido == null)
                {
                    return NotFound();
                }
                var item = pedido.Items.FirstOrDefault(i => i.AccesorioId == accesorioId);
                if (item == null)
                {
                    return NotFound();
                }
                pedido.Items.Remove(item);
                if (_pedidoRepo.ActualizarPedido(pedido))
                {
                    return NoContent();
                }
                else
                {
                    ModelState.AddModelError("", $"Algo salió mal al eliminar el item del carrito");
                    return StatusCode(500, ModelState);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error inesperado: {ex.Message}");
                return StatusCode(500, ModelState);
            }
        }

        [HttpDelete("{pedidoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult VaciarPedido(int pedidoId)
        {
            try
            {
                var pedido = _pedidoRepo.GetPedidoPorId(pedidoId);
                if (pedido == null)
                {
                    return NotFound();
                }
                pedido.Items.Clear();
                if (_pedidoRepo.ActualizarPedido(pedido))
                {
                    return NoContent();
                }
                else
                {
                    ModelState.AddModelError("", $"Algo salió mal al vaciar el carrito");
                    return StatusCode(500, ModelState);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error inesperado: {ex.Message}");
                return StatusCode(500, ModelState);
            }
        }

        [HttpGet("pedidos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPedidos()
        {
            var pedidos = _pedidoRepo.GetPedidos();
            var pedidosDto = _mapper.Map<List<PedidosComprasDto>>(pedidos);
            return Ok(pedidosDto);
        }

        [HttpPatch("{pedidoId}/estado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ActualizarEstadoPedido(int pedidoId, [FromBody] string nuevoEstadoString)
        {
            if (string.IsNullOrEmpty(nuevoEstadoString))
            {
                return BadRequest("El estado del pedido no puede estar vacío");
            }
            if (!Enum.TryParse<EstadoPedido>(nuevoEstadoString, out var nuevoEstado))
            {
                return BadRequest("Estado de pedido no válido");
            }
            var pedido = _pedidoRepo.GetPedidoPorId(pedidoId);
            if (pedido == null)
            {
                return NotFound();
            }
            pedido.Estado = nuevoEstado;
            if (_pedidoRepo.ActualizarPedido(pedido))
            {
                return NoContent();
            }
            else
            {
                return BadRequest("No se pudo actualizar el estado del pedido");
            }
        }


    }
}
