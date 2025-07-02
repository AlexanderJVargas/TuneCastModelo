using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TuneCastModelo;

    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TuneCastModelo.Anuncio> Anuncios { get; set; } = default!;

public DbSet<TuneCastModelo.Cancion> Canciones { get; set; } = default!;

public DbSet<TuneCastModelo.Pago> Pagos { get; set; } = default!;

public DbSet<TuneCastModelo.Plan> Planes { get; set; } = default!;

public DbSet<TuneCastModelo.Playlist> Playlists { get; set; } = default!;

public DbSet<TuneCastModelo.Suscripcion> Suscripciones { get; set; } = default!;

public DbSet<TuneCastModelo.Usuario> Usuarios { get; set; } = default!;
    }
