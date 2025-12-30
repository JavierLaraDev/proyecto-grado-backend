using ApiGrado.Modelos;
using ApiGrado.Modelos.Dtos;
using AutoMapper;

namespace ApiGrado.Mappers
{
    public class BlogMapper : Profile
    {
        public BlogMapper()
        {
            // ========================================
            // ✅ MAPEO PARA PEDIDOS (CREAR DESDE FRONTEND)
            // ========================================

            // De PedidosComprasDto (lo que envía el frontend) a PedidosCompras (entidad)
            CreateMap<PedidosComprasDto, PedidosCompras>()
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // De PedidosItemsDto (lo que envía el frontend) a PedidosItems (entidad)
            CreateMap<PedidosItemsDto, PedidosItems>()
                .ForMember(dest => dest.Accesorio, opt => opt.Ignore())
                .ForMember(dest => dest.AccesorioId, opt => opt.MapFrom(src => src.Accesorio.Id));
            // ☝️ CLAVE: Mapear AccesorioId desde src.Accesorio.Id

            // ========================================
            // ✅ MAPEO PARA PEDIDOS (LEER DESDE BD)
            // ========================================

            // De PedidosCompras (entidad) a PedidosComprasDto (respuesta)
            CreateMap<PedidosCompras, PedidosComprasDto>();

            // De PedidosItems (entidad) a PedidosItemsDto (respuesta)
            CreateMap<PedidosItems, PedidosItemsDto>()
                .ForMember(dest => dest.Accesorio, opt => opt.MapFrom(src => src.Accesorio));

            // ========================================
            // ✅ MAPEO ALTERNATIVO (si usas PedidoCrearDto)
            // ========================================

            CreateMap<PedidoCrearDto, PedidosCompras>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<PedidoItemCrearDto, PedidosItems>();

            // ========================================
            // USUARIOS
            // ========================================
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioActualizarDto>().ReverseMap();

            // ========================================
            // ACCESORIOS
            // ========================================
            CreateMap<Accesorio, AccesorioDto>().ReverseMap();
            CreateMap<Accesorio, AccesorioCrearDto>().ReverseMap();
            CreateMap<Accesorio, AccesorioActualizarDto>().ReverseMap();
        }
    }
}