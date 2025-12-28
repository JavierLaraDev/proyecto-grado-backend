using ApiGrado.Modelos;
using ApiGrado.Modelos.Dtos;
using AutoMapper;
using Microsoft.Extensions.Hosting;

namespace ApiGrado.Mappers
{
    public class BlogMapper:Profile
    {
        public BlogMapper()
        {
            // Mapeo de PedidoCrearDto a PedidosCompras
            CreateMap<PedidoCrearDto, PedidosCompras>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // Mapeo de PedidosCompras a PedidosComprasDto
            CreateMap<PedidosCompras, PedidosComprasDto>();

            // Mapeo de PedidoItemCrearDto a PedidosItems
            CreateMap<PedidoItemCrearDto, PedidosItems>();

            // Mapeo de PedidosItems a PedidosItemsDto
            CreateMap<PedidosItems, PedidosItemsDto>();

            // =========================
            // USUARIOS
            // =========================
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioActualizarDto>().ReverseMap();

            // =========================
            // ACCESORIOS
            // =========================
            CreateMap<Accesorio, AccesorioDto>().ReverseMap();
            CreateMap<Accesorio, AccesorioCrearDto>().ReverseMap();
            CreateMap<Accesorio, AccesorioActualizarDto>().ReverseMap();


        }
    }
}