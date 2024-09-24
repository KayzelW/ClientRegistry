using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ClientRegistry.Models;
using ClientRegistry.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientRegistry.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly ILogger<ClientsController> _logger;
    private readonly AppDbContext _dbContext;

    public ClientsController(ILogger<ClientsController> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost]
    [ProducesResponseType<Client>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] ClientDTO clientDto)
    {
        var client = clientDto.Client;
        var foundersInn = clientDto.FoundersInn;
        client.AddDate = DateTime.Now;
        client.UpdateDate = DateTime.Now;
        
        try
        {
            var dbFounders = await _dbContext.Founders.Where(x => foundersInn.Contains(x.Inn)).ToListAsync();
            client.Founders = dbFounders;

            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest();
        }

        return Ok(client);
    }

    [HttpGet]
    [ProducesResponseType<Client>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([StringLength(Program.MaxCharsInn), FromQuery] string? inn)
    {
        //TODO: paged list
        if (string.IsNullOrEmpty(inn))
        {
            var clients = await _dbContext.Clients.Where(x => x.Inn == inn).ToListAsync();
            return Ok(clients);
        }

        var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Inn == inn);
        return client == null ? NotFound() : Ok(client);
    }


    [HttpPut(template: "{inn}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Put([StringLength(Program.MaxCharsInn)] string inn, [FromBody] ClientDTO? clientDto)
    {
        if (clientDto?.Client == null) return BadRequest();
        var client = clientDto.Client;

        var clientToPatch = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Inn == client.Inn);
        if (clientToPatch == null) return NotFound();


        if (client?.Name != null) clientToPatch.Name = client.Name;
        if (client?.Type != null) clientToPatch.Type = client.Type;
        clientToPatch.UpdateDate = DateTime.Now;

        var foundersInn = clientDto.FoundersInn;

        if (foundersInn is { Count: > 0 } && clientToPatch.Type != ClientType.IndividualEntity)
        {
            clientToPatch.Founders = await _dbContext.Founders.Where(x => foundersInn.Contains(x.Inn)).ToListAsync();
        }

        _dbContext.Update(clientToPatch);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

        return NoContent();
    }


    [HttpDelete(template: "{inn}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([StringLength(Program.MaxCharsInn)] string inn)
    {
        var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Inn == inn);
        if (client == null) return NotFound();

        _dbContext.Clients.Remove(client);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}