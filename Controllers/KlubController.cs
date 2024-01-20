using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportsko_Takmicenje;

[Route("api/[controller]")]
[ApiController]
public class KlubController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public KlubController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet("get-all-klub")]
    public async Task<ActionResult<IEnumerable<Klub>>> GetKlubovi()
    {
        try
        {
            var klubovi = await _neo4jService.GetKluboviAsync();
            return Ok(klubovi);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("get-klub/{id}")]
    public async Task<ActionResult<Klub>> GetKlub(string id)
    {
        try
        {
            var klub = await _neo4jService.GetKlubByIdAsync(id);
            return Ok(klub);
        }
        catch (Exception ex)
        {
            
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpGet("get-klub-by-name/{name}")]
    public async Task<ActionResult<Klub>> GetKlubByName(string name)
    {
        try
        {
            var klub = await _neo4jService.GetKlubByNameAsync(name);
            if (klub == null)
            {
                return NotFound($"Klub with name {name} not found");
            }
            return Ok(klub);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpPost("create-klub")]
    public async Task<ActionResult<Klub>> Post(Klub klub)
    {
        try
        {
            var createdKlub = await _neo4jService.CreateKlubAsync(klub);
            return Ok(createdKlub);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("put-klub/{klubId}")]
    public async Task<ActionResult<Klub>> UpdateKlub(string klubId, [FromBody] Klub klub)
    {
        try
        {
            klub.KlubId = klubId;
            var updatedKlub = await _neo4jService.UpdateKlubAsync(klub);
            
            if (updatedKlub == null)
            {
                return NotFound($"Klub with ID {klubId} not found");
            }

            return Ok(updatedKlub);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpDelete("delete-klub/{klubId}")]
    public async Task<ActionResult> DeleteKlub(string klubId)
    {
        try
        {
            var isDeleted = await _neo4jService.DeleteKlubAsync(klubId);

            if (!isDeleted)
            {
                return NotFound($"Klub with ID {klubId} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("dodaj-tim-klubu/{klubId}/{timId}")]
    public async Task<ActionResult> DodajTimKlubu(string klubId, string timId)
    {
        try
        {
            var isSuccessful = await _neo4jService.DodajTimKlubuAsync(klubId, timId);

            if (!isSuccessful)
            {
                return NotFound($"Takmicar or Tim not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}