using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Sportsko_Takmicenje;

[Route("api/[controller]")]
[ApiController]
public class RezultatController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public RezultatController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet("get-rezultat/{id}")]
    public async Task<ActionResult<Rezultat>> GetRezultat(string id)
    {
        try
        {
            var rezultati = await _neo4jService.GetRezultatAsync(id);
            return Ok(rezultati);
        }
        catch (Exception ex)
        {
            
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpPost("create-rezultat")]
    public async Task<ActionResult<Rezultat>> Post(Rezultat rezultat)
    {
        try
        {
            var createdRezultat = await _neo4jService.CreateRezultatAsync(rezultat);
            return Ok(createdRezultat);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("put-rezultat/{id}")]
    public async Task<ActionResult<Takmicar>> UpdateTakmicar(string id, [FromBody] Rezultat rezultat)
    {
        try
        {
            rezultat.RezultatId = id;
            var updatedRezultat = await _neo4jService.UpdateRezultatAsync(rezultat);

            if (updatedRezultat== null)
            {
                return NotFound($"Result with ID-jem {id} not found");
            }

            return Ok(updatedRezultat);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpDelete("delete-rezultat/{id}")]
    public async Task<ActionResult> DeleteRezultat(string id)
    {
        try
        {
            var isDeleted = await _neo4jService.DeleteRezultatAsync(id);

            if (!isDeleted)
            {
                return NotFound($"Result with ID-jem {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpPost("dodaj-tim-rezultatu/{rezultatid}/{timid}")]
    public async Task<ActionResult> AddTimToRezultat(string rezultatid, string timid)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddTimToRezultatAsync(rezultatid, timid);

            if (!isSuccessful)
            {
                return NotFound($"Rezultat or tim not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("dodaj-utakmicu-rezultatu/{rezultatid}/{utakmicaid}")]
    public async Task<ActionResult> AddUtakmicaToRezultat(string rezultatid, string utakmicaid)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddUtakmicaToRezultatAsync(rezultatid, utakmicaid);

            if (!isSuccessful)
            {
                return NotFound($"Rezultat or utakmica not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpGet("poeni/{utakmicaId}/{timId}")]
    public async Task<ActionResult<double>> GetPoeniZaTimNaUtakmici(string utakmicaId, string timId)
    {
        try
        {
            double poeni = await _neo4jService.GetPointsForMatchTeamAsync(utakmicaId, timId);
            return Ok(poeni);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška pri dohvatanju poena: {ex.Message}");
        }
    }
    [HttpPost("createAndAssignToTim/{utakmicaId}/{timId}/{poeni}")]
    public async Task<IActionResult> CreateAndAssignRezultatToTim(string utakmicaId, string timId, double poeni)
    {
        try
        {
            var rezultat = await _neo4jService.CreateAndAssignRezultatToTimAsync(poeni, utakmicaId, timId);

            if (rezultat != null)
            {
                return Ok(rezultat);
            }
            else
            {
                return BadRequest("Failed to create and assign result to team.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}