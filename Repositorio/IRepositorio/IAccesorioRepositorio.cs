using ApiGrado.Modelos;
using Microsoft.Extensions.Hosting;

namespace ApiGrado.Repositorio.IRepositorio
{
    public interface IAccesorioRepositorio
    {
        ICollection<Accesorio> GetAccesorios();
        Accesorio GetAccesorio(int postId);
        bool ExisteAccesorio(string nombre);
        bool ExisteAccesorio(int id);
        bool CrearAccesorio(Accesorio accesorio);
        bool ActualizarAccesorio(Accesorio accesorio);
        bool BorrarAccesorio(Accesorio accesorio);
        bool Guardar();
    }
}
