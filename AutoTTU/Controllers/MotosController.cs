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
    public class MotosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de todas as motos cadastradas
        /// </summary>
        // GET: api/Motos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Motos>>> GetMotos()
        {
            return await _context.Motos.ToListAsync();
        }

        /// <summary>
        /// Busca de uma moto pelo ID
        /// </summary>
        /// <param name="id">ID da moto</param>
        // GET: api/Motos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Motos>> GetMotos(int id)
        {
            var motos = await _context.Motos.FindAsync(id);

            if (motos == null)
            {
                return NotFound();
            }

            return motos;
        }

        /// <summary>
        /// Alteração de uma moto já cadastrada
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     PUT/api/Motos
        ///        {
        ///          "idMoto": 1,
        ///          "modelo": "H2",
        ///          "marca": "Honda",
        ///          "ano": 2020,
        ///          "placa": "FDP3467",
        ///          "ativoChar": "s",
        ///          "status": true,
        ///          "fotoUrl": "123456plcg"
        ///        }
        ///       
        /// </remarks>
        /// 
        /// <param name="id">ID da moto</param>


        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMotos(int id, Motos motos)
        {
            if (id != motos.IdMoto)
            {
                return BadRequest();
            }

            _context.Entry(motos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MotosExists(id))
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
        /// Cadastro de uma moto
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST/api/Motos
        ///        {
        ///          "idMoto": 1,
        ///          "modelo": "H2",
        ///          "marca": "Honda",
        ///          "ano": 2020,
        ///          "placa": "FDP3467",
        ///          "ativoChar": "s",
        ///          "status": true,
        ///          "fotoUrl": "123456plcg"
        ///        }
        ///       
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<Motos>> PostMotos(Motos motos)
        {
            _context.Motos.Add(motos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMotos", new { id = motos.IdMoto }, motos);
        }

        /// <summary>
        /// Apagar uma moto pelo ID
        /// </summary>
        /// <param name="id">ID da moto</param>
        // DELETE: api/Motos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotos(int id)
        {
            var motos = await _context.Motos.FindAsync(id);
            if (motos == null)
            {
                return NotFound();
            }

            _context.Motos.Remove(motos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MotosExists(int id)
        {
            return _context.Motos.Any(e => e.IdMoto == id);
        }

        
    }
}
