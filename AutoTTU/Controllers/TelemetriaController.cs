using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoTTU.Connection;
using AutoTTU.Models;

namespace AutoTTU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TelemetriaController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Recebe dados de telemetria de uma moto
        /// </summary>
        /// <param name="telemetria">Dados de telemetria da moto</param>
        /// <returns>Dados salvos</returns>
        [HttpPost]
        public async Task<ActionResult<Telemetria>> PostTelemetria(Telemetria telemetria)
        {
            try
            {
                // Verificar se a moto existe
                var moto = await _context.Motos.FindAsync(telemetria.DeviceId);
                if (moto == null)
                {
                    return BadRequest("Moto não encontrada");
                }

                // Atualizar localização da moto (comentado temporariamente até as colunas serem criadas)
                // moto.Latitude = telemetria.Latitude;
                // moto.Longitude = telemetria.Longitude;
                // moto.UltimaAtualizacao = DateTime.Now;

                // Salvar telemetria
                telemetria.Timestamp = DateTime.Now;
                _context.Telemetria.Add(telemetria);
                
                await _context.SaveChangesAsync();

                // Retornar apenas os dados essenciais para evitar referência circular
                var response = new
                {
                    Id = telemetria.Id,
                    DeviceId = telemetria.DeviceId,
                    Timestamp = telemetria.Timestamp,
                    Latitude = telemetria.Latitude,
                    Longitude = telemetria.Longitude,
                    Status = telemetria.Status,
                    Temperatura = telemetria.Temperatura,
                    Velocidade = telemetria.Velocidade,
                    Quilometragem = telemetria.Quilometragem,
                    NivelCombustivel = telemetria.NivelCombustivel,
                    RotacaoMotor = telemetria.RotacaoMotor,
                    Observacoes = telemetria.Observacoes
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao salvar telemetria: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca as últimas telemetrias para o dashboard
        /// </summary>
        /// <param name="limit">Número máximo de registros (padrão: 50)</param>
        /// <returns>Lista das últimas telemetrias</returns>
        [HttpGet("ultimas")]
        public async Task<ActionResult<IEnumerable<object>>> GetUltimasTelemetrias([FromQuery] int limit = 50)
        {
            try
            {
                var ultimasTelemetrias = await _context.Telemetria
                    .OrderByDescending(t => t.Timestamp)
                    .Take(limit)
                    .Select(t => new
                    {
                        t.Id,
                        t.DeviceId,
                        t.Timestamp,
                        t.Latitude,
                        t.Longitude,
                        t.Status,
                        t.Temperatura,
                        t.Velocidade,
                        t.Quilometragem,
                        t.NivelCombustivel,
                        t.RotacaoMotor,
                        t.Observacoes
                    })
                    .ToListAsync();

                return Ok(ultimasTelemetrias);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar telemetrias: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca telemetrias de uma moto específica
        /// </summary>
        /// <param name="deviceId">ID da moto</param>
        /// <param name="limit">Número máximo de registros</param>
        /// <returns>Lista de telemetrias da moto</returns>
        [HttpGet("moto/{deviceId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTelemetriasPorMoto(int deviceId, [FromQuery] int limit = 20)
        {
            try
            {
                var telemetrias = await _context.Telemetria
                    .Where(t => t.DeviceId == deviceId)
                    .OrderByDescending(t => t.Timestamp)
                    .Take(limit)
                    .Select(t => new
                    {
                        t.Id,
                        t.Timestamp,
                        t.Latitude,
                        t.Longitude,
                        t.Status,
                        t.Temperatura,
                        t.Velocidade,
                        t.Quilometragem,
                        t.NivelCombustivel,
                        t.RotacaoMotor,
                        t.Observacoes
                    })
                    .ToListAsync();

                return Ok(telemetrias);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar telemetrias da moto: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca todas as motos com suas últimas localizações
        /// </summary>
        /// <returns>Lista de motos com localização atual</returns>
        [HttpGet("motos-localizacao")]
        public async Task<ActionResult<IEnumerable<object>>> GetMotosComLocalizacao()
        {
            try
            {
                // Buscar todas as motos e suas últimas telemetrias
                var motos = await _context.Motos.ToListAsync();
                var motosComLocalizacao = new List<object>();
                
                foreach (var moto in motos)
                {
                    var ultimaTelemetria = await _context.Telemetria
                        .Where(t => t.DeviceId == moto.IdMoto)
                        .OrderByDescending(t => t.Timestamp)
                        .FirstOrDefaultAsync();
                    
                    motosComLocalizacao.Add(new
                    {
                        IdMoto = moto.IdMoto,
                        Modelo = moto.Modelo,
                        Marca = moto.Marca,
                        Ano = moto.Ano,
                        Placa = moto.Placa,
                        Status = moto.Status,
                        Latitude = ultimaTelemetria?.Latitude,
                        Longitude = ultimaTelemetria?.Longitude,
                        Endereco = (string)null, // Coluna não existe ainda
                        UltimaAtualizacao = ultimaTelemetria?.Timestamp,
                        UltimaTelemetria = ultimaTelemetria != null ? new
                        {
                            ultimaTelemetria.Status,
                            ultimaTelemetria.Temperatura,
                            ultimaTelemetria.Velocidade,
                            ultimaTelemetria.Quilometragem,
                            ultimaTelemetria.NivelCombustivel,
                            ultimaTelemetria.RotacaoMotor,
                            ultimaTelemetria.Timestamp
                        } : null
                    });
                }

                return Ok(motosComLocalizacao);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar motos com localização: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca uma telemetria específica
        /// </summary>
        /// <param name="id">ID da telemetria</param>
        /// <returns>Dados da telemetria</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Telemetria>> GetTelemetria(int id)
        {
            var telemetria = await _context.Telemetria
                .Include(t => t.Moto)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (telemetria == null)
            {
                return NotFound();
            }

            return telemetria;
        }
    }
}
