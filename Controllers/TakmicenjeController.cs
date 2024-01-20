using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportsko_Takmicenje;

[Route("api/[controller]")]
[ApiController]
public class TakmicenjeController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public TakmicenjeController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet("get-all-takmicenja")]
    public async Task<ActionResult<IEnumerable<Takmicenje>>> GetTakmicenja()
    {
        try
        {
            var takmicenja = await _neo4jService.GetTakmicenjaAsync();
            return Ok(takmicenja);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("get-takmicenje/{takmicenjeId}")]
    public async Task<ActionResult<Takmicenje>> GetTakmicenje(string takmicenjeId)
    {
        try
        {
            var takmicenje = await _neo4jService.GetTakmicenjeByIdAsync(takmicenjeId);

            if (takmicenje == null)
            {
                return NotFound($"Sport with ID {takmicenjeId} not found");
            }

            return Ok(takmicenje);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("get-takmicenje-by-name/{name}")]
    public async Task<ActionResult<Takmicenje>> GetTakmicenjeByName(string name)
    {
        try
        {
            var takmicenje = await _neo4jService.GetTakmicenjeByNameAsync(name);
            if (takmicenje == null)
            {
                return NotFound($"Takmicenje with name {name} not found");
            }
            return Ok(takmicenje);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
   

    [HttpPost("create-takmicenje")]
    public async Task<ActionResult<Takmicenje>> Post(Takmicenje takmicenje)
    {
        try
        {
            var createdTakmicenje = await _neo4jService.CreateTakmicenjeAsync(takmicenje);
            return Ok(createdTakmicenje);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("update-takmicenje/{takmicenjeId}")]
    public async Task<ActionResult<Takmicenje>> UpdateTakmicenje(string takmicenjeId, [FromBody] Takmicenje takmicenje)
    {
        try
        {
            takmicenje.TakmicenjeId = takmicenjeId;

            var updatedTakmicenje = await _neo4jService.UpdateTakmicenjeAsync(takmicenje);

            if (updatedTakmicenje == null)
            {
                return NotFound($"Takmicenje with ID {takmicenjeId} not found");
            }

            return Ok(updatedTakmicenje);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("delete-takmicenje/{takmicenjeId}")]
    public async Task<ActionResult> DeleteTakmicenje(string takmicenjeId)
    {
        try
        {
            var isDeleted = await _neo4jService.DeleteTakmicenjeAsync(takmicenjeId);

            if (!isDeleted)
            {
                return NotFound($"Takmicenje with ID {takmicenjeId} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    
    [HttpPost("povezi-takmicenje-utakmica/{takmicenjeId}/{utakmicaId}")]
    public async Task<IActionResult> PoveziTakmicenjeUtakmica(string takmicenjeId, string utakmicaId)
    {
        try
        {
            var uspesnoPovezano = await _neo4jService.PoveziTakmicenjeUtakmicaAsync(takmicenjeId, utakmicaId);

            if (uspesnoPovezano)
            {
                return Ok("Takmičenje uspešno povezano sa utakmicom.");
            }
            else
            {
                return BadRequest("Povezivanje takmičenja i utakmice nije uspelo.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška prilikom povezivanja takmičenja i utakmice: {ex.Message}");
        }
    }

    [HttpGet("get-utakmice-by-takmicenje/{takmicenjeName}")]
    public async Task<ActionResult<IEnumerable<Utakmica>>> GetUtakmiceByTakmicenjeName(string takmicenjeName)
    {
        try
        {
            var utakmice = await _neo4jService.GetUtakmiceByTakmicenjeNameAsync(takmicenjeName);

            if (utakmice == null || !utakmice.Any())
            {
                return NotFound($"No utakmice found for takmicenje with name {takmicenjeName}");
            }

            return Ok(utakmice);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpGet("get-utakmiceID/{takmicenjeId}")]
    public async Task<IActionResult> GetUtakmiceByTakmicenjeIdAsync(string takmicenjeId)
    {
        try
        {
            var takmicenje = await _neo4jService.GetTakmicenjeByIdAsync(takmicenjeId);

            if (takmicenje == null)
            {
                return NotFound($"Takmičenje sa ID-jem {takmicenjeId} nije pronađeno.");
            }

            var utakmice = await _neo4jService.GetUtakmiceByTakmicenjeIdAsync(takmicenjeId);

            if (utakmice == null)
            {
                return NotFound($"Nema utakmica za takmičenje sa ID-jem {takmicenjeId}.");
            }

            var utakmicaIds = utakmice.Select(utakmica => utakmica.UtakmicaId);

            return Ok(utakmicaIds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška prilikom preuzimanja utakmica: {ex.Message}");
        }
    }
}
