using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneCastModelo
{
   public class Pago
    {
        public int Id { get; set; }  // Primary Key
        public DateTime FechaPago { get; set; }
        public double Monto { get; set; }
        public string MetodoPago { get; set; }  // Ejemplo: Tarjeta de crédito, PayPal, etc.
        // Relaciones
        public int UsuarioId { get; set; }  // Foreign Key a Usuario
        public int SuscripcionId { get; set; }  // Foreign Key a Suscripcion
        public  Usuario? Usuario { get; set; }  // Relación con Usuario
        public Suscripcion? Suscripcion { get; set; }  // Relación con Suscripción
    }
}
