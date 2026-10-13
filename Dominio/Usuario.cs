using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionArticulos.Dominio
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public int RolId { get; set; }
        public Rol Role { get; set; }                 // Propiedad de navegación
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public bool Activo { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public string FullName
        {
            get { return $"{Nombre} {Apellido}".Trim(); }
        }
    }
}
