using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneCastModelo
{
    public class Playlist
    {

        [Key] public int Id { get; set; }  // Primary Key

        public string Nombre { get; set; }

        // Relaciones
       
        public int UsuarioId { get; set; }  // Foreign Key a Usuario
        
        public virtual Usuario? Usuario { get; set; }

        public List<Cancion>? Canciones { get; set; }  // Playlist puede contener muchas canciones
    }

}
