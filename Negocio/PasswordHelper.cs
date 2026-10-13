using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCrypt.Net;

namespace GestionArticulos.Negocio
{
    public static class PasswordHelper
    {
        // Genera el hash de la contraseña (usar al crear o cambiar contraseña)
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // Verifica si la contraseña ingresada coincide con el hash guardado
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}