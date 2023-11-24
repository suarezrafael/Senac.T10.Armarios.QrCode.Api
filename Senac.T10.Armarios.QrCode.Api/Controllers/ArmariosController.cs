using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senac.T10.Armarios.QrCode.Api.Data;
using Senac.T10.Armarios.QrCode.Api.Dtos;
using Senac.T10.Armarios.QrCode.Api.Models;

namespace Senac.T10.Armarios.QrCode.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArmariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArmariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Armarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Armario>>> GetArmarios()
        {
          if (_context.Armarios == null)
          {
              return NotFound();
          }
            return await _context.Armarios.ToListAsync();
        }

        // GET: api/Armarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Armario>> GetArmario(int id)
        {
          if (_context.Armarios == null)
          {
              return NotFound();
          }
            var armario = await _context.Armarios.FindAsync(id);

            if (armario == null)
            {
                return NotFound();
            }

            return armario;
        }

        // PUT: api/Armarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArmario(int id, ArmarioUpdateRequest armarioRequest)
        {
            if (id != armarioRequest.Id)
            {
                return BadRequest();
            }

            var armario = await _context.Armarios.FindAsync(id);

            if (armario == null)
            {
                return NotFound();
            }
            armario.Descricao = armarioRequest.Descricao;

#pragma warning disable CS8601 // Possível atribuição de referência nula.
            armario.QrCodeBase64 = armarioRequest.QrCodeBase64;
#pragma warning restore CS8601 // Possível atribuição de referência nula.


            _context.Entry(armario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArmarioExists(id))
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

        // POST: api/Armarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Armario>> PostArmario(ArmarioRequest armario)
        {
          if (_context.Armarios == null)
          {
              return Problem("Entity set 'AppDbContext.Armarios'  is null.");
            }
            var newArmario = new Armario() 
            { 
                Descricao = armario.Descricao,
                QrCodeBase64 = "QrCode",
                Url = "url"
            };

            _context.Armarios.Add(newArmario);
            await _context.SaveChangesAsync();

            var  result = CreatedAtAction("GetArmario", new { id = newArmario.Id }, armario);
            string uri = $"http://www.armariosapi.somee.com/api/Armarios/{newArmario.Id}";
            var updateArmario = await _context.Armarios.FirstOrDefaultAsync(x=> x.Id == newArmario.Id);
            updateArmario.Url = uri;
            _context.Entry(updateArmario).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return result;
        }

        // DELETE: api/Armarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArmario(int id)
        {
            if (_context.Armarios == null)
            {
                return NotFound();
            }
            var armario = await _context.Armarios.FindAsync(id);
            if (armario == null)
            {
                return NotFound();
            }

            _context.Armarios.Remove(armario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArmarioExists(int id)
        {
            return (_context.Armarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
