using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Sportsko_Takmicenje;

[Route("api/[controller]")]
[ApiController]
public class TrenerController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public TrenerController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet("get-all-trener")]
    public async Task<ActionResult<IEnumerable<Trener>>> GetTrenere()
    {
        try
        {
            var trener = await _neo4jService.GetTreneriAsync();
            return Ok(trener);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }
    [HttpGet("get-trener/{jmbg}")]
    public async Task<ActionResult<Trener>> GetTrener(string jmbg)
    {
        try
        {
            var trener = await _neo4jService.GetTrenerByIdAsync(jmbg);

            if (trener == null)
            {
                return NotFound($"Trener with JMBG {jmbg} not found");
            }

            return Ok(trener);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpPost("create-trener")]
    public async Task<ActionResult<Trener>> Post(Trener trener)
    {
        try
        {
            var createdTrener = await _neo4jService.CreateTrenerAsync(trener);
            return Ok(createdTrener);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }




    [HttpPut("put-trener/{jmbg}/{adresa}")]
    public async Task<ActionResult<Trener>> UpdateTrener(string jmbg, string adresa)
    {
        try
        {
            var updatedTrener = await _neo4jService.UpdateTrenerAsync(jmbg, adresa);

            if (updatedTrener == null)
            {
                return NotFound($"Trener with JMBG {jmbg} not found");
            }

            return Ok(updatedTrener);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpDelete("delete-trener/{jmbg}")]
    public async Task<ActionResult> DeleteTrener(string jmbg)
    {
        try
        {
            var isDeleted = await _neo4jService.DeleteTrenerAsync(jmbg);

            if (!isDeleted)
            {
                return NotFound($"trener with JMBG {jmbg} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

   
    [HttpPost("dodaj-sport-treneru/{trenerJMBG}/{sportId}")]
    public async Task<ActionResult> AddSportToCoach(string trenerJMBG, string sportId)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddSportToCoachAsync(trenerJMBG, sportId);

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

    [HttpPost("dodaj-tim-treneru/{trenerJMBG}/{timId}")]
    public async Task<ActionResult> AddTeamToCoach(string trenerJMBG, string timId)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddTeamToCoachAsync(trenerJMBG, timId);

            if (!isSuccessful)
            {
                return NotFound($"Trener or team not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}
