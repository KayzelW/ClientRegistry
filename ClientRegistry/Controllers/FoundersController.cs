using System.ComponentModel.DataAnnotations;
using ClientRegistry.Models;
using ClientRegistry.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientRegistry.Controllers;

[ApiController, Route("api/founders")]
public class FoundersController : ControllerBase
{
    private readonly ILogger<ClientsController> _logger;
    private readonly AppDbContext _dbContext;

    public FoundersController(ILogger<ClientsController> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost(template: "{isIndividual}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(bool? isIndividual, [FromBody] FounderDto founderDto)
    {
        var founder = founderDto.Founder;
        var clientsInn = founderDto.ClientsInn;
        var individualInn = founderDto.IndividualInn;
        
        if (string.IsNullOrEmpty(founder.Inn) || string.IsNullOrEmpty(founder.Name))
        {
            return BadRequest();
        }

        founder.AddDate = DateTime.Now;
        founder.UpdateDate = DateTime.Now;
        
        _dbContext.Founders.Add(founder);

        if (isIndividual is null or false)
        {
            if (clientsInn is { Count: > 0 })
            {
                founder.Clients = await _dbContext.Clients.Where(c => clientsInn.Contains(c.Inn)).ToListAsync();
                _dbContext.Update(founder);
            }

            await _dbContext.SaveChangesAsync();
            return Ok(founder);
        }

        #region IndividualEntity

        if (individualInn != null)
        {
            var client = new Client()
            {
                Inn = individualInn, //i don't know how Individual is working
                Name = founder.Name,
                Type = ClientType.IndividualEntity,
                Founders = new List<Founder>() { founder }
            };

            _dbContext.Clients.Add(client);

            founder.Clients.Add(client);
        }

        #endregion


        await _dbContext.SaveChangesAsync();
        return Ok(founder);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([StringLength(Program.MaxCharsInn), FromQuery] string? inn)
    {
        if (string.IsNullOrEmpty(inn))
        {
            var founders = _dbContext.Founders.Where(founder => founder.Inn == inn).ToListAsync();
            return Ok(founders);
        }

        var founder = await _dbContext.Founders.FirstOrDefaultAsync(founder => founder.Inn == inn);
        return founder == null ? NotFound() : Ok(founder);
    }

    [HttpPut(template: "{inn}")]
    public async Task<IActionResult> Put([StringLength(Program.MaxCharsInn)] string inn, [FromBody] FounderPutDto founderPutDto)
    {
        var founder = founderPutDto.Founder;
        var removeClientsInn = founderPutDto.removeClientsInn;
        var addClientsInn = founderPutDto.addClientsInn;

        var founderToPatch = await _dbContext.Founders
            .Include(fnd => fnd.Clients)
            .FirstOrDefaultAsync(fnd => fnd.Inn == inn);
        if (founderToPatch == null) return NotFound();

        if (!string.IsNullOrEmpty(founder.Name)) founderToPatch.Name = founder.Name;
        founderToPatch.UpdateDate = DateTime.Now;

        if (removeClientsInn != null && removeClientsInn.Any(x => x.Length == 12))
        {
            foreach (var client in founderToPatch.Clients.ToArray())
            {
                if (removeClientsInn.Contains(client.Inn)) founderToPatch.Clients.Remove(client);
            }

            _dbContext.Update(founderToPatch);
        }

        if (addClientsInn != null && addClientsInn.Any(x => x.Length == 12))
        {
            var clientsToAdd =
                await _dbContext.Clients.Where(client => addClientsInn.Contains(client.Inn)).ToListAsync();

            foreach (var client in clientsToAdd)
            {
                if (!founderToPatch.Clients.Contains(client)) founderToPatch.Clients.Add(client);
            }

            _dbContext.Update(founderToPatch);
        }

        return Ok(founderToPatch);
    }

    [HttpDelete(template: "{inn}")]
    public async Task<IActionResult> Delete([StringLength(Program.MaxCharsInn)] string? inn)
    {
        var founder = await _dbContext.Founders.FirstOrDefaultAsync(founder => founder.Inn == inn);
        return founder == null ? NotFound() : Ok(founder);
    }
}