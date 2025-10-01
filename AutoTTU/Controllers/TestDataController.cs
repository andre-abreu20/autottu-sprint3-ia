using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoTTU.Connection;
using AutoTTU.Models;

namespace AutoTTU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestDataController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria dados de teste no banco
        /// </summary>
        [HttpPost("create-test-data")]
        public async Task<ActionResult> CreateTestData()
        {
            try
            {
                // Verificar se já existem motos
                var existingMotos = await _context.Motos.CountAsync();
                if (existingMotos > 0)
                {
                    return Ok($"Já existem {existingMotos} motos no banco. Dados de teste não foram criados.");
                }

                // Criar motos de teste
                var motos = new List<Motos>
                {
                    new Motos
                    {
                        Modelo = "CB 600F",
                        Marca = "Honda",
                        Ano = 2020,
                        Placa = "ABC-1234",
                        AtivoChar = "S",
                        FotoUrl = "https://example.com/moto1.jpg",
                        Latitude = -23.5505m,
                        Longitude = -46.6333m,
                        Endereco = "São Paulo, SP",
                        UltimaAtualizacao = DateTime.Now
                    },
                    new Motos
                    {
                        Modelo = "MT-07",
                        Marca = "Yamaha",
                        Ano = 2021,
                        Placa = "DEF-5678",
                        AtivoChar = "S",
                        FotoUrl = "https://example.com/moto2.jpg",
                        Latitude = -23.5505m,
                        Longitude = -46.6333m,
                        Endereco = "São Paulo, SP",
                        UltimaAtualizacao = DateTime.Now
                    },
                    new Motos
                    {
                        Modelo = "Z650",
                        Marca = "Kawasaki",
                        Ano = 2022,
                        Placa = "GHI-9012",
                        AtivoChar = "S",
                        FotoUrl = "https://example.com/moto3.jpg",
                        Latitude = -23.5505m,
                        Longitude = -46.6333m,
                        Endereco = "São Paulo, SP",
                        UltimaAtualizacao = DateTime.Now
                    }
                };

                _context.Motos.AddRange(motos);
                await _context.SaveChangesAsync();

                // Criar usuário de teste
                var usuario = new Usuario
                {
                    Nome = "João Silva",
                    Email = "joao@email.com",
                    Senha = "123456",
                    Telefone = "11999999999"
                };

                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync();

                // Criar slots de teste
                var slots = new List<Slot>
                {
                    new Slot { IdMoto = 1, AtivoChar = "S" },
                    new Slot { IdMoto = 2, AtivoChar = "S" },
                    new Slot { IdMoto = 3, AtivoChar = "S" }
                };

                _context.Slot.AddRange(slots);
                await _context.SaveChangesAsync();

                return Ok("Dados de teste criados com sucesso! 3 motos, 1 usuário e 3 slots foram inseridos.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar dados de teste: {ex.Message}");
            }
        }

        /// <summary>
        /// Lista todas as motos cadastradas
        /// </summary>
        [HttpGet("motos")]
        public async Task<ActionResult<IEnumerable<Motos>>> GetMotos()
        {
            try
            {
                var motos = await _context.Motos.ToListAsync();
                return Ok(motos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar motos: {ex.Message}");
            }
        }
    }
}
