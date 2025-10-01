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
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de todos os usuários cadastrados
        /// </summary>
        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuario()
        {
            return await _context.Usuario.ToListAsync();
        }

        /// <summary>
        /// Busca de um usuário pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        /// <summary>
        /// Alteração de um usuário já cadastrado
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     PUT/api/Usuarios
        ///        {
        ///            "idUsuario": 1,
        ///            "nome": "Gabriel Silva",
        ///            "email": "gabriel@silva.com",
        ///            "senha": "12345",
        ///            "telefone": "11987654321"
        ///         }
        ///       
        /// </remarks>
        /// 
        /// <param name="id">ID do usuário</param>
        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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
        /// Cadastro um usuário
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST/api/Usuarios
        ///        {
        ///            "idUsuario": 1,
        ///            "nome": "Gabriel Silva",
        ///            "email": "gabriel@silva.com",
        ///            "senha": "12345",
        ///            "telefone": "11987654321"
        ///         }
        ///       
        /// </remarks>
        /// 
        /// <param name="id">ID do usuário</param>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.IdUsuario }, usuario);
        }

        /// <summary>
        /// Apagar um usuário pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.IdUsuario == id);
        }
    }
}
