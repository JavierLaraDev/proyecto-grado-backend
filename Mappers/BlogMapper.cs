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
            // =========================
            // PEDIDOS (CREACIÓN)
            // =========================
            CreateMap<PedidoCrearDto, PedidosCompras>();
            CreateMap<PedidoItemCrearDto, PedidosItems>();

            // =========================
            // PEDIDOS (LECTURA)
            // =========================
            CreateMap<PedidosCompras, PedidosComprasDto>();
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