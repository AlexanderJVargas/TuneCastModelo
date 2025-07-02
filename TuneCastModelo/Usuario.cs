using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneCastModelo
{
    public class Usuario
    {

        [Key] public int Id { get; set; }  // Primary Key

        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }  // Ejemplo: Administrador, Cliente,artista

        // Relaciones
        public Suscripcion? Suscripcion { get; set; }  // Relación con Suscripción
        public List<Playlist>? Playlists { get; set; }  // Un usuario puede tener muchas playlists
        public  List<Cancion>? Canciones { get; set; }  // Un usuario puede tener muchas canciones
    }

}
