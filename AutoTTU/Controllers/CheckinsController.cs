using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoTTU.Connection;
using AutoTTU.Models;

namespace AutoTTU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckinsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CheckinsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de todos os CheckIn's realizados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Checkin>>> GetCheckin()
        {
            return await _context.Checkin.ToListAsync();
        }

        /// <summary>
        /// Busca de CheckIn pelo ID
        /// </summary>
        /// <param name="id">ID do CheckIn</param>
        // GET: api/Checkins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Checkin>> GetCheckin(int id)
        {
            var checkin = await _context.Checkin.FindAsync(id);

            if (checkin == null)
            {
                return NotFound();
            }

            return checkin;
        }

        /// <summary>
        /// Alteração de CheckIn já realizado
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     PUT/api/CheckIn
        ///        {
        ///             "idCheckin": 1,
        ///             "idMoto": 1,
        ///             "idUsuario": 1,
        ///             "ativoChar": "s",
        ///             "violada": true,
        ///             "observacao": "Moto danificada",
        ///             "timeStamp": "2025-09-18",
        ///             "imagensUrl": "string"
        ///         }
        ///       
        /// </remarks>
        /// 
        /// <param name="id">ID do CheckIn</param>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCheckin(int id, Checkin checkin)
        {
            if (id != checkin.IdCheckin)
            {
                return BadRequest();
            }

            _context.Entry(checkin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckinExists(id))
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

        /// <summary>
        /// Realização de CheckIn
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST/api/CheckIn
        ///        {
        ///             "idCheckin": 1,
        ///             "idMoto": 1,
        ///             "idUsuario": 1,
        ///             "ativoChar": "s",
        ///             "violada": true,
        ///             "observacao": "Moto danificada",
        ///             "timeStamp": "2025-09-18",
        ///             "imagensUrl": "string"
        ///         }
        ///       
        /// </remarks>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Checkin>> PostCheckin(Checkin checkin)
        {
            _context.Checkin.Add(checkin);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCheckin", new { id = checkin.IdCheckin }, checkin);
        }


        /// <summary>
        /// Apagar CheckIn pelo ID
        /// </summary>
        /// <param name="id">ID do CheckIn</param>
        // DELETE: api/Checkins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCheckin(int id)
        {
            var checkin = await _context.Checkin.FindAsync(id);
            if (checkin == null)
            {
                return NotFound();
            }

            _context.Checkin.Remove(checkin);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CheckinExists(int id)
        {
            return _context.Checkin.Any(e => e.IdCheckin == id);
        }
    }
}
