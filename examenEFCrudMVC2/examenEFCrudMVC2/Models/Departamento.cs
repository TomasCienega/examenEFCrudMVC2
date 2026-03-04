using System;
using System.Collections.Generic;

namespace examenEFCrudMVC2.Models
{
    public partial class Departamento
    {
        public Departamento()
        {
            Empleados = new HashSet<Empleado>();
        }

        public int IdDepartamento { get; set; }
        public string NombreDepartamento { get; set; } = null!;

        public virtual ICollection<Empleado> Empleados { get; set; }
    }
}
