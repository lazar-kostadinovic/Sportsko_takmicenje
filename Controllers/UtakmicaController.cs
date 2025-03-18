using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Sportsko_Takmicenje;

[Route("api/[controller]")]
[ApiController]
public class UtakmicaController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public UtakmicaController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet("get-all-utakmice")]
    public async Task<ActionResult<Utakmica>> GetAllUtakmice()
    {
        try
        {

            var utakmice = await _neo4jService.GetUtakmiceAsync();
            return Ok(utakmice);
        }
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("get-utakmica/{id}")]
    public async Task<ActionResult<Utakmica>> GetUtakmica(string id)
    {
        try
        {

            var utakmice = await _neo4jService.GetUtakmicaAsync(id);
            return Ok(utakmice);
        }
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpGet("get-utakmica-by-name/{name}")]
    public async Task<ActionResult<Utakmica>> GetUtakmicaByName(string name)
    {
        try
        {
            var utakmica = await _neo4jService.GetUtakmicaByNameAsync(name);
            if (utakmica == null)
            {
                return NotFound($"Utakmica with name {name} not found");
            }
            return Ok(utakmica);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpPost("create-utakmica")]
    public async Task<ActionResult<Utakmica>> Post(Utakmica utakmica)
    {
        try
        {
            var createdUtakmica = await _neo4jService.CreateUtakmicaAsync(utakmica);
            return Ok(createdUtakmica);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("put-utakmica/{utakmicaId}")]
    public async Task<ActionResult<Utakmica>> UpdateUtakmica(string utakmicaId, [FromBody] Utakmica utakmica)
    {
        try
        {
            utakmica.UtakmicaId = utakmicaId;
            var updatedUtakmica = await _neo4jService.UpdateUtakmicaAsync(utakmica);

            if (updatedUtakmica == null)
            {
                return NotFound($"Utakmica with ID {utakmicaId} not found");
            }

            return Ok(updatedUtakmica);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpDelete("delete-utakmica/{naziv}")]
    public async Task<ActionResult> DeleteUtakmica(string naziv)
    {
        try
        {
            var isDeleted = await _neo4jService.DeleteUtakmicaAsync(naziv);

            if (!isDeleted)
            {
                return NotFound($"Utakmica with  {naziv} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpPost("dodaj-timove-utakmici/{utakmicaid}/{timid}")]
    public async Task<ActionResult> AddTimToUtakmica(string utakmicaid, string timid)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddTimToUtakmicaAsync(utakmicaid, timid);

            if (!isSuccessful)
            {
                return NotFound($"Utakmica or tim not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpPost("dodaj-takmicenje-utakmici/{utakmicaid}/{takmicenjeid}")]
    public async Task<ActionResult> AddTakmicenjeToUtakmica(string utakmicaid, string takmicenjeid)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddTakmicenjeToUtakmicaAsync(utakmicaid, takmicenjeid);

            if (!isSuccessful)
            {
                return NotFound($"Utakmica or takmicenje not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpPost("dodaj-rezultate-utakmici/{utakmicaid}/{rezultatid}")]
    public async Task<ActionResult> AddRezultatToUtakmica(string utakmicaid, string rezultatid)
    {
        try
        {
            bool postojiRezultat = await _neo4jService.UtakmicaImaRezultatAsync(utakmicaid);
            if (postojiRezultat)
            {
                return BadRequest("Utakmica već ima rezultat i ne može se dodati novi.");
            }

            var isSuccessful = await _neo4jService.AddRezultatToUtakmicaAsync(utakmicaid, rezultatid);

            if (!isSuccessful)
            {
                return NotFound($"Utakmica ili rezultat nisu pronađeni ili je veza već uspostavljena.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }



    [HttpPost("dodaj-sport-utakmici/{utakmicaid}/{sportid}")]
    public async Task<ActionResult> AddSportToUtakmica(string utakmicaid, string sportid)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddSportToUtakmicaAsync(utakmicaid, sportid);

            if (!isSuccessful)
            {
                return NotFound($"Utakmica or sport not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("pobede/{utakmicaId}/{timId}")]
    public async Task<ActionResult<string>> PreuzmiIdTakmicenjaZaUtakmicuAsync(string utakmicaId)
    {
        try
        {
            string takmicenjeId = await _neo4jService.PreuzmiIdTakmicenjaZaUtakmicu(utakmicaId);

            return Ok(takmicenjeId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška pri dohvatanju broja pobeda: {ex.Message}");
        }
    }

    [HttpGet("teamIds/{utakmicaId}")]
    public async Task<IActionResult> GetTeamIdsByUtakmicaId(string utakmicaId)
    {
        try
        {
            var teamIds = await _neo4jService.GetTeamIdsByUtakmicaIdAsync(utakmicaId);
            return Ok(teamIds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("teamNames/{utakmicaId}")]
    public async Task<IActionResult> GetTeamNamesByUtakmicaId(string utakmicaId)
    {
        try
        {
            var teamIds = await _neo4jService.GetTeamNamesForUtakmicaAsync(utakmicaId);
            return Ok(teamIds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("imaRezultat/{utakmicaId}")]
    public async Task<ActionResult<bool>> ImaRezultat(string utakmicaId)
    {
        try
        {
            bool postojiRezultat = await _neo4jService.UtakmicaImaRezultatAsync(utakmicaId);
            return Ok(postojiRezultat);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška na serveru: {ex.Message}");
        }
    }



}