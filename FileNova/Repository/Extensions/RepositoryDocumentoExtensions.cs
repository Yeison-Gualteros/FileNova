using Entities.Models;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Repository.Extensions
{
    public static class RepositoryDocumentoExtensions
    {
        public static IQueryable<Documento> FilterDocumento(this IQueryable<Documento> documentos, DateTime minFecha, DateTime maxFecha)=>
            documentos.Where(d=> (d.Fecha_Creacion >= minFecha && d.Fecha_Creacion <= maxFecha));

        public static IQueryable<Documento> Search(this IQueryable<Documento> documentos, string? busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
                return documentos;

            var terminoEnMinuscula = busqueda.Trim().ToLower();
            return documentos.Where(d => d.Nombre!.ToLower().Contains(terminoEnMinuscula) || (d.Descripcion != null && d.Descripcion.ToLower().Contains(terminoEnMinuscula)));
        }

        public static IQueryable<Documento> Sort(this IQueryable<Documento> documentos, string ordenPorCadenaDeConsulta)
        {
            if (string.IsNullOrWhiteSpace(ordenPorCadenaDeConsulta))
                return documentos.OrderBy(e => e.Nombre).ThenByDescending(f => f.Fecha_Creacion);

            var ordenParams = ordenPorCadenaDeConsulta.Trim().Split(',');
            var propiedadInfos = typeof(Documento).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var ordenPorCadena = new StringBuilder();

            foreach (var parametro in ordenParams)
            {
                if(string.IsNullOrWhiteSpace(parametro))
                    continue;

                var propiedadDeConsultaNombre = parametro.Split(" ")[0];
                var objetoPropiedad = propiedadInfos.FirstOrDefault(pi => pi.Name.Equals(propiedadDeConsultaNombre, StringComparison.InvariantCultureIgnoreCase));

                if (objetoPropiedad == null)
                    continue;

                var direccionDeOrden = parametro.EndsWith(" desc") ? "descending" : "ascending";
                ordenPorCadena.Append($"{objetoPropiedad.Name} {direccionDeOrden}, ");

            }

            var ordenPorCadenFinal = ordenPorCadena.ToString().TrimEnd(',', ' ');

            if(string.IsNullOrWhiteSpace(ordenPorCadenFinal))
                return documentos.OrderBy(e => e.Nombre).ThenByDescending(f => f.Fecha_Creacion);

            return documentos.OrderBy(ordenPorCadenFinal);


        }
    }
}
