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
    public class SlotsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SlotsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de todos os slots (espaços de estacionamento) cadastrados
        /// </summary>
        // GET: api/Slots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slot>>> GetSlot()
        {
            return await _context.Slot.ToListAsync();
        }

        /// <summary>
        /// Busca de um slot pelo ID
        /// </summary>
        /// <param name="id">ID do slot</param>
        // GET: api/Slots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Slot>> GetSlot(int id)
        {
            var slot = await _context.Slot.FindAsync(id);

            if (slot == null)
            {
                return NotFound();
            }

            return slot;
        }

        /// <summary>
        /// Alteração de um slot já cadastrado
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     PUT/api/Slots
        ///        {
        ///             "idSlot": 1,
        ///             "idMoto": 1,
        ///             "ativoChar": "s",
        ///             "ocupado": true
        ///        }
        ///       
        /// </remarks>
        /// 
        /// <param name="id">ID do usuário</param>
        // PUT: api/Slots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSlot(int id, Slot slot)
        {
            
            
            if (id != slot.IdSlot)
            {
                return BadRequest();
            }

            _context.Entry(slot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlotExists(id))
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
        /// Cadastro de um slot
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST/api/Slots
        ///        {
        ///             "idSlot": 1,
        ///             "idMoto": 1,
        ///             "ativoChar": "s",
        ///             "ocupado": true
        ///        }
        ///       
        /// </remarks>
        /// 
        // POST: api/Slots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Slot>> PostSlot(Slot slot)
        {

            _context.Slot.Add(slot);
            await _context.SaveChangesAsync();

           


            return CreatedAtAction("GetSlot", new { id = slot.IdSlot }, slot);
        }


        /// <summary>
        /// Apagar um slot pelo ID
        /// </summary>
        /// <param name="id">ID do slot</param>
        // DELETE: api/Slots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var slot = await _context.Slot.FindAsync(id);
            if (slot == null)
            {
                return NotFound();
            }

            _context.Slot.Remove(slot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SlotExists(int id)
        {
            return _context.Slot.Any(e => e.IdSlot == id);
        }


     
       
    }
}
