using TuneCastModelo;

namespace TuneCast.MVC.Models
{
    public class ReportesViewModel
    {
        public List<Cancion> Canciones { get; set; }
        public List<Usuario> Usuarios { get; set; }
        public List<Pago> Pagos { get; set; }
        public List<Playlist> Playlists { get; set; }
        //public List<Artista> Artistas { get; set; }
        public List<Plan> Planes { get; set; }
    }

}
