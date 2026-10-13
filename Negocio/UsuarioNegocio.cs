using BCrypt.Net;
using GestionArticulos.Dominio;
using System;
using System.Collections.Generic;

namespace GestionArticulos.Negocio
{
    public class UsuarioNegocio
    {
        // ---------- ALTA ----------
        public void agregar(Usuario u, string passwordPlano)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(passwordPlano);

                datos.setearConsulta(@"
                    INSERT INTO USUARIOS
                        (NombreUsuario, ContrasenaHash, Nombre, Apellido, Email,
                         IdRol, FechaAlta, IntentosFallidos, Activo)
                    VALUES
                        (@NombreUsuario, @ContrasenaHash, @Nombre, @Apellido, @Email,
                         @IdRol, GETDATE(), 0, @Activo)");

                datos.setearParametro("@NombreUsuario", u.NombreUsuario);
                datos.setearParametro("@ContrasenaHash", hash);
                datos.setearParametro("@Nombre", (object)u.Nombre ?? DBNull.Value);
                datos.setearParametro("@Apellido", (object)u.Apellido ?? DBNull.Value);
                datos.setearParametro("@Email", u.Email);
                datos.setearParametro("@IdRol", u.RolId);
                datos.setearParametro("@Activo", u.Activo);

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ---------- EDICIÓN ----------
        public void modificar(Usuario u, string passwordPlano = null)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string sql = @"
                    UPDATE USUARIOS SET
                        NombreUsuario = @NombreUsuario,
                        Nombre        = @Nombre,
                        Apellido      = @Apellido,
                        Email         = @Email,
                        IdRol         = @IdRol,
                        Activo        = @Activo,
                        FechaModificacion = GETDATE()";

                if (!string.IsNullOrWhiteSpace(passwordPlano))
                    sql += ", ContrasenaHash = @ContrasenaHash";

                sql += " WHERE IdUsuario = @IdUsuario";

                datos.setearConsulta(sql);
                datos.setearParametro("@NombreUsuario", u.NombreUsuario);
                datos.setearParametro("@Nombre", (object)u.Nombre ?? DBNull.Value);
                datos.setearParametro("@Apellido", (object)u.Apellido ?? DBNull.Value);
                datos.setearParametro("@Email", u.Email);
                datos.setearParametro("@IdRol", u.RolId);
                datos.setearParametro("@Activo", u.Activo);
                datos.setearParametro("@IdUsuario", u.Id);

                if (!string.IsNullOrWhiteSpace(passwordPlano))
                {
                    string hash = BCrypt.Net.BCrypt.HashPassword(passwordPlano);
                    datos.setearParametro("@ContrasenaHash", hash);
                }

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ---------- BAJA LÓGICA ----------
        public void eliminarLogico(int idUsuario)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    UPDATE USUARIOS
                       SET Activo = 0,
                           FechaModificacion = GETDATE()
                     WHERE IdUsuario = @IdUsuario");
                datos.setearParametro("@IdUsuario", idUsuario);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ---------- LISTAR ----------
        public List<Usuario> listar()
        {
            List<Usuario> lista = new List<Usuario>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT IdUsuario, NombreUsuario, Nombre, Apellido, Email, IdRol,
                           FechaAlta, FechaUltimoLogin, IntentosFallidos,
                           BloqueadoHasta, Activo, FechaModificacion
                      FROM USUARIOS
                     ORDER BY NombreUsuario");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(mapear(datos));
                }
                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public Usuario obtenerPorId(int idUsuario)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT IdUsuario, NombreUsuario, Nombre, Apellido, Email, IdRol,
                           FechaAlta, FechaUltimoLogin, IntentosFallidos,
                           BloqueadoHasta, Activo, FechaModificacion
                      FROM USUARIOS
                     WHERE IdUsuario = @IdUsuario");
                datos.setearParametro("@IdUsuario", idUsuario);
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                    return mapear(datos);
                return null;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        private Usuario mapear(AccesoDatos datos)
        {
            Usuario u = new Usuario();
            u.Id = (int)datos.Lector["IdUsuario"];
            u.NombreUsuario = datos.Lector["NombreUsuario"].ToString();
            u.Nombre = datos.Lector["Nombre"] == DBNull.Value ? null : datos.Lector["Nombre"].ToString();
            u.Apellido = datos.Lector["Apellido"] == DBNull.Value ? null : datos.Lector["Apellido"].ToString();
            u.Email = datos.Lector["Email"].ToString();
            u.RolId = (int)datos.Lector["IdRol"];
            u.CreatedAt = (DateTime)datos.Lector["FechaAlta"];
            u.LastLogin = datos.Lector["FechaUltimoLogin"] == DBNull.Value
                            ? (DateTime?)null : (DateTime)datos.Lector["FechaUltimoLogin"];
            u.FailedAttempts = (int)datos.Lector["IntentosFallidos"];
            u.LockedUntil = datos.Lector["BloqueadoHasta"] == DBNull.Value
                            ? (DateTime?)null : (DateTime)datos.Lector["BloqueadoHasta"];
            u.Activo = (bool)datos.Lector["Activo"];
            u.ModifiedAt = datos.Lector["FechaModificacion"] == DBNull.Value
                            ? (DateTime?)null : (DateTime)datos.Lector["FechaModificacion"];
            return u;
        }
    }
}