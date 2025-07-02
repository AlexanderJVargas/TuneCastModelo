using Microsoft.AspNetCore.Authentication.Cookies;
using TuneCastAPIConsumer;
using TuneCastModelo;

namespace TuneCast.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Crud<Anuncio>.EndPoint = "https://localhost:7194/api/Anuncios";
            Crud<Cancion>.EndPoint = "https://localhost:7194/api/Canciones";
            Crud<Pago>.EndPoint = "https://localhost:7194/api/Pagos";
            Crud<Plan>.EndPoint = "https://localhost:7194/api/Planes";
            Crud<Playlist>.EndPoint = "https://localhost:7194/api/Playlists";
            Crud<Suscripcion>.EndPoint = "https://localhost:7194/api/Subscripciones";
            Crud<Usuario>.EndPoint = "https://localhost:7194/api/Usuarios";




            // Configuración de autenticación
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                   // options.ExpireTimeSpan = TimeSpan.FromDays(30);
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
               pattern: "{controller=Home}/{action=Index}/{id?}");
              // pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
