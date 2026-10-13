using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionArticulos.Dominio
{
    public class Rol
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}