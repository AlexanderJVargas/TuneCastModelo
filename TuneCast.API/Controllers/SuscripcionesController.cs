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
    public class SuscripcionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuscripcionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Suscripciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Suscripcion>>> GetSuscripcion()
        {
            return await _context.Suscripciones.ToListAsync();
        }

        // GET: api/Suscripciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Suscripcion>> GetSuscripcion(int id)
        {
            var suscripcion = await _context.Suscripciones.FindAsync(id);

            if (suscripcion == null)
            {
                return NotFound();
            }

            return suscripcion;
        }

        // PUT: api/Suscripciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuscripcion(int id, Suscripcion suscripcion)
        {
            if (id != suscripcion.Id)
            {
                return BadRequest();
            }

            _context.Entry(suscripcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuscripcionExists(id))
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

        // POST: api/Suscripciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Suscripcion>> PostSuscripcion(Suscripcion suscripcion)
        {
            _context.Suscripciones.Add(suscripcion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSuscripcion", new { id = suscripcion.Id }, suscripcion);
        }

        // DELETE: api/Suscripciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuscripcion(int id)
        {
            var suscripcion = await _context.Suscripciones.FindAsync(id);
            if (suscripcion == null)
            {
                return NotFound();
            }

            _context.Suscripciones.Remove(suscripcion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SuscripcionExists(int id)
        {
            return _context.Suscripciones.Any(e => e.Id == id);
        }
    }
}
