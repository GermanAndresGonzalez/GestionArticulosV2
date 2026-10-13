using GestionArticulos.Dominio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace GestionArticulos.Negocio
{
    public class UserData
    {
        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    SELECT U.IdUsuario, U.NombreUsuario, U.ContrasenaHash, U.Nombre, U.Apellido, 
                           U.Email, U.IdRol, U.FechaAlta, U.FechaUltimoLogin, U.IntentosFallidos, 
                           U.BloqueadoHasta, U.Activo, U.FechaModificacion,
                           R.IdRol, R.NombreRol, R.Descripcion, R.Activo AS RolActivo
                    FROM USUARIOS U
                    INNER JOIN ROLES R ON U.IdRol = R.IdRol");

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Usuario user = new Usuario();
                    MapearUsuario(datos.Lector, user);
                    lista.Add(user);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public Usuario BuscarPorId(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    SELECT U.IdUsuario, U.NombreUsuario, U.ContrasenaHash, U.Nombre, U.Apellido, 
                           U.Email, U.IdRol, U.FechaAlta, U.FechaUltimoLogin, U.IntentosFallidos, 
                           U.BloqueadoHasta, U.Activo, U.FechaModificacion,
                           R.IdRol, R.NombreRol, R.Descripcion, R.Activo AS RolActivo
                    FROM USUARIOS U
                    INNER JOIN ROLES R ON U.IdRol = R.IdRol
                    WHERE U.IdUsuario = @id");

                datos.setearParametro("@id", id);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    Usuario user = new Usuario();
                    MapearUsuario(datos.Lector, user);
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public Usuario BuscarPorUsername(string username)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    SELECT U.IdUsuario, U.NombreUsuario, U.ContrasenaHash, U.Nombre, U.Apellido, 
                           U.Email, U.IdRol, U.FechaAlta, U.FechaUltimoLogin, U.IntentosFallidos, 
                           U.BloqueadoHasta, U.Activo, U.FechaModificacion,
                           R.IdRol, R.NombreRol, R.Descripcion, R.Activo AS RolActivo
                    FROM USUARIOS U
                    INNER JOIN ROLES R ON U.IdRol = R.IdRol
                    WHERE U.NombreUsuario = @username");

                datos.setearParametro("@username", username);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    Usuario user = new Usuario();
                    MapearUsuario(datos.Lector, user);
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void Agregar(Usuario user, string passwordPlano)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                // Hashear la contraseña antes de guardar
                string hash = PasswordHelper.HashPassword(passwordPlano);

                datos.setearConsulta(@"
                    INSERT INTO USUARIOS 
                    (NombreUsuario, ContrasenaHash, Nombre, Apellido, Email, IdRol, Activo)
                    VALUES 
                    (@username, @passwordHash, @firstName, @lastName, @email, @roleId, @active)");

                datos.setearParametro("@username", user.NombreUsuario);
                datos.setearParametro("@passwordHash", hash);
                datos.setearParametro("@firstName", (object)user.Nombre ?? DBNull.Value);
                datos.setearParametro("@lastName", (object)user.Apellido ?? DBNull.Value);
                datos.setearParametro("@email", user.Email);
                datos.setearParametro("@roleId", user.RolId);
                datos.setearParametro("@active", user.Activo);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void Modificar(Usuario user)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    UPDATE USUARIOS SET
                        NombreUsuario = @username,
                        Nombre = @firstName,
                        Apellido = @lastName,
                        Email = @email,
                        IdRol = @roleId,
                        Activo = @active,
                        FechaModificacion = GETDATE()
                    WHERE IdUsuario = @id");

                datos.setearParametro("@username", user.NombreUsuario);
                datos.setearParametro("@firstName", (object)user.Nombre ?? DBNull.Value);
                datos.setearParametro("@lastName", (object)user.Apellido ?? DBNull.Value);
                datos.setearParametro("@email", user.Email);
                datos.setearParametro("@roleId", user.RolId);
                datos.setearParametro("@active", user.Activo);
                datos.setearParametro("@id", user.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void CambiarPassword(int userId, string nuevaPasswordPlano)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string hash = PasswordHelper.HashPassword(nuevaPasswordPlano);

                datos.setearConsulta(@"
                    UPDATE USUARIOS SET
                        ContrasenaHash = @passwordHash,
                        FechaModificacion = GETDATE()
                    WHERE IdUsuario = @id");

                datos.setearParametro("@passwordHash", hash);
                datos.setearParametro("@id", userId);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void Eliminar(int id)   // Soft delete
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("UPDATE USUARIOS SET Activo = 0, FechaModificacion = GETDATE() WHERE IdUsuario = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // ---------- Método privado de mapeo ----------
        private void MapearUsuario(SqlDataReader lector, Usuario user)
        {
            user.Id = (int)lector["IdUsuario"];
            user.NombreUsuario = (string)lector["NombreUsuario"];
            user.PasswordHash = (string)lector["ContrasenaHash"];
            user.Nombre = lector["Nombre"] is DBNull ? null : (string)lector["Nombre"];
            user.Apellido = lector["Apellido"] is DBNull ? null : (string)lector["Apellido"];
            user.Email = (string)lector["Email"];
            user.RolId = (int)lector["IdRol"];
            user.CreatedAt = (DateTime)lector["FechaAlta"];
            user.LastLogin = lector["FechaUltimoLogin"] is DBNull ? (DateTime?)null : (DateTime)lector["FechaUltimoLogin"];
            user.FailedAttempts = (int)lector["IntentosFallidos"];
            user.LockedUntil = lector["BloqueadoHasta"] is DBNull ? (DateTime?)null : (DateTime)lector["BloqueadoHasta"];
            user.Activo = (bool)lector["Activo"];
            user.ModifiedAt = lector["FechaModificacion"] is DBNull ? (DateTime?)null : (DateTime)lector["FechaModificacion"];

            // Mapear el Rol
            user.Role = new Rol
            {
                Id = (int)lector["IdRol"],
                Name = (string)lector["NombreRol"],
                Description = lector["Descripcion"] is DBNull ? null : (string)lector["Descripcion"],
                Active = (bool)lector["RolActivo"]
            };
        }
    }
}