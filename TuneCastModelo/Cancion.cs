using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneCastModelo
{
    public class Cancion
    {
        [Key] public int Id { get; set; }  // Primary Key

        public string Titulo { get; set; }
        public string Artista { get; set; }
        public string Genero { get; set; }
        public bool Aprobada { get; set; }  // Si la canción está aprobada por el administrador
        public string DerechosAutor { get; set; }
        public int numeroReproducciones { get; set; }  // Número de reproducciones de la canción
        public string Licencia { get; set; }
        public string RutaArchivo { get; set; }

        // Relaciones
        public int UsuarioId { get; set; }  // Foreign Key a Usuario
      
        public virtual Usuario? Usuario { get; set; }

    }

}
