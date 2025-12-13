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
            CreateMap<PedidosCompras, PedidosComprasDto>().ReverseMap();
            CreateMap<PedidosItems, PedidosItemsDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioActualizarDto>().ReverseMap();
            CreateMap<Accesorio, AccesorioDto>().ReverseMap();
            CreateMap<Accesorio, AccesorioCrearDto>().ReverseMap();
            CreateMap<Accesorio, AccesorioActualizarDto>().ReverseMap();
           
           
        }
    }
}