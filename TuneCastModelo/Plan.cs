using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneCastModelo
{
    public class Plan
    {

        [Key] public int Id { get; set; }  // Primary Key

        public string Nombre { get; set; }
        public double Precio { get; set; }
        public string Descripcion { get; set; }

        // Relaciones
        public List<Suscripcion>? Suscripciones { get; set; }  // Un plan puede tener muchas suscripciones
    }

}
