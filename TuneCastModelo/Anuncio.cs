using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TuneCastModelo
{
    public class Anuncio
    {
       
        public int Id { get; set; }  // Primary Key

        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime horaInicio { get; set; }
        public DateTime HoraFin { get; set; }

        // Relaciones
        [ForeignKey("PlanId")]
        public int PlanId { get; set; }  // Foreign Key a Plan (opcional, si los anuncios son relacionados con un plan)
     
        public  Plan? Plan { get; set; }
    }

}
