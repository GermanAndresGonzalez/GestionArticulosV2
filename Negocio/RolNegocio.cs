using GestionArticulos.Dominio;
using System.Collections.Generic;

namespace GestionArticulos.Negocio
{
    public class RolNegocio
    {
        public List<Rol> listar()
        {
            List<Rol> lista = new List<Rol>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT IdRol, NombreRol, Descripcion FROM ROLES WHERE Activo = 1 ORDER BY NombreRol");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(new Rol
                    {
                        Id = (int)datos.Lector["IdRol"],
                        Name = datos.Lector["NombreRol"].ToString(),
                        Description = datos.Lector["Descripcion"] == System.DBNull.Value
                                        ? "" : datos.Lector["Descripcion"].ToString()
                    });
                }
                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}