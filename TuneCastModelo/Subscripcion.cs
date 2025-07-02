using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneCastModelo
{
    public class Suscripcion
    {
        [Key]
        public int Id { get; set; }  // Primary Key

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activa { get; set; }  // Indica si la suscripción está activa o no

        // Relaciones
        public int UsuarioId { get; set; }  // Foreign Key a Usuario
      
        public int PlanId { get; set; }  // Foreign Key a Plan
       

        public virtual Usuario? Usuario { get; set; }
       
        public virtual Plan? Plan { get; set; }
    }

}
