using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportsko_Takmicenje;

[Route("api/[controller]")]
[ApiController]
public class SportController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public SportController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet("get-all-sportovi")]
    public async Task<ActionResult<IEnumerable<Sport>>> GetSportovi()
    {
        try
        {
            var sportovi = await _neo4jService.GetSportoviAsync();
            return Ok(sportovi);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("get-sport/{sportId}")]
    public async Task<ActionResult<Sport>> GetSport(string sportId)
    {
        try
        {
            var sport = await _neo4jService.GetSportByIdAsync(sportId);

            if (sport == null)
            {
                return NotFound($"Sport with ID {sportId} not found");
            }

            return Ok(sport);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("get-sport-by-name/{name}")]
    public async Task<ActionResult<Sport>> GetSportByName(string name)
    {
        try
        {
            var sport = await _neo4jService.GetSportByNameAsync(name);
            if (sport == null)
            {
                return NotFound($"Sport with name {name} not found");
            }
            return Ok(sport);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("create-sport")]
    public async Task<ActionResult<Sport>> CreateSport(Sport sport)
    {
        try
        {
            var createdSport = await _neo4jService.CreateSportAsync(sport);
            return Ok(createdSport);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

  
    [HttpPut("update-sport/{sportId}/{brojIgraca}")]
    public async Task<ActionResult<Sport>> UpdateSport(string sportId,int brojIgraca)
    {
        try
        {
            var updatedSport = await _neo4jService.UpdateSportAsync(sportId, brojIgraca);

            if (updatedSport == null)
            {
                return NotFound($"Sport with ID {sportId} not found");
            }

            return Ok(updatedSport);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("delete-sport/{sportId}")]
    public async Task<ActionResult> DeleteSport(string sportId)
    {
        try
        {
            var isDeleted = await _neo4jService.DeleteSportAsync(sportId);

            if (!isDeleted)
            {
                return NotFound($"Sport with ID {sportId} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("dodaj-trenera-za-sport/{sportId}/{trenerJMBG}")]
    public async Task<ActionResult> DodajTreneraZaSport(string sportId, string trenerJMBG)
    {
        try
        {
            var isSuccessful = await _neo4jService.PoveziSportTrenerAsync(sportId, trenerJMBG);

            if (!isSuccessful)
            {
                return NotFound($"Trener or sport not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("dodaj-takmicara-za-sport/{sportId}/{takmicarJMBG}")]
    public async Task<ActionResult> DodajTakmicaraZaSport(string sportId, string takmicarJMBG)
    {
        try
        {
            var isSuccessful = await _neo4jService.PoveziSportTakmicarAsync(sportId, takmicarJMBG);

            if (!isSuccessful)
            {
                return NotFound($"Trener or sport not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("dodaj-tim-za-sport/{sportId}/{timId}")]
    public async Task<ActionResult> DodajTimZaSport(string sportId, string timId)
    {
        try
        {
            var isSuccessful = await _neo4jService.PoveziSportTimAsync(sportId, timId);

            if (!isSuccessful)
            {
                return NotFound($"Trener or sport not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}
