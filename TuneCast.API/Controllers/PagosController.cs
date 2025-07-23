using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuneCastModelo;

namespace TuneCast.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PagosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Pagos - CORREGIDO PARA EVITAR REFERENCIAS CIRCULARES
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPago()
        {
            var pagos = await _context.Pagos.ToListAsync();

            // DEVOLVER SOLO DATOS SIMPLES SIN NAVEGACIÓN
            var pagosResponse = pagos.Select(pago => new
            {
                Id = pago.Id,
                FechaPago = pago.FechaPago,
                Monto = pago.Monto,
                MetodoPago = pago.MetodoPago,
                UsuarioId = pago.UsuarioId,
                SuscripcionId = pago.SuscripcionId
            });

            return Ok(pagosResponse);
        }

        // GET: api/Pagos/5 - CORREGIDO PARA EVITAR REFERENCIAS CIRCULARES
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPago(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }

            //DEVOLVER SOLO DATOS SIMPLES SIN NAVEGACIÓN
            var pagoResponse = new
            {
                Id = pago.Id,
                FechaPago = pago.FechaPago,
                Monto = pago.Monto,
                MetodoPago = pago.MetodoPago,
                UsuarioId = pago.UsuarioId,
                SuscripcionId = pago.SuscripcionId
            };

            return pagoResponse;
        }

        // PUT: api/Pagos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPago(int id, Pago pago)
        {
            if (id != pago.Id)
            {
                return BadRequest();
            }

            _context.Entry(pago).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PagoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Pagos - MÉTODO CORREGIDO SIN REFERENCIAS CIRCULARES
        [HttpPost]
        public async Task<ActionResult<object>> PostPago(Pago pago)
        {
            try
            {
                // VALIDAR QUE EL USUARIO EXISTE PRIMERO
                var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == pago.UsuarioId);
                if (!usuarioExiste)
                {
                    return BadRequest($"El usuario con ID {pago.UsuarioId} no existe.");
                }

                // Si SuscripcionId es 0 o nulo, crear una suscripción automática
                if (pago.SuscripcionId == null || pago.SuscripcionId <= 0)
                {
                    // Buscar el primer plan disponible
                    var primerPlan = await _context.Planes.FirstOrDefaultAsync();
                    if (primerPlan != null)
                    {
                        // Crear suscripción automática 
                        var nuevaSuscripcion = new Suscripcion
                        {
                            UsuarioId = pago.UsuarioId,
                            PlanId = primerPlan.Id,
                            FechaInicio = DateTime.UtcNow,
                            FechaFin = DateTime.UtcNow.AddMonths(1),
                            Activa = true
                        };

                        _context.Suscripciones.Add(nuevaSuscripcion);
                        await _context.SaveChangesAsync();

                        // Asignar la suscripción al pago
                        pago.SuscripcionId = nuevaSuscripcion.Id;
                    }
                    else
                    {
                        return BadRequest("No hay planes disponibles para crear una suscripción.");
                    }
                }

                // Crear el pago
                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                // DEVOLVER SOLO DATOS SIMPLES SIN NAVEGACIÓN
                var pagoResponse = new
                {
                    Id = pago.Id,
                    FechaPago = pago.FechaPago,
                    Monto = pago.Monto,
                    MetodoPago = pago.MetodoPago,
                    UsuarioId = pago.UsuarioId,
                    SuscripcionId = pago.SuscripcionId
                };

                return CreatedAtAction("GetPago", new { id = pago.Id }, pagoResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // DELETE: api/Pagos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePago(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }

            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }
    }
}