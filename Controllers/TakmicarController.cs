using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Sportsko_Takmicenje;

[Route("api/[controller]")]
[ApiController]
public class TakmicarController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public TakmicarController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet("get-all-takmicar")]
    public async Task<ActionResult<IEnumerable<Takmicar>>> GetTakmicare()
    {
        try
        {
            var takmicar = await _neo4jService.GetTakmicariAsync();
            return Ok(takmicar);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("{takmicarJMBG}")]
    public async Task<IActionResult> GetTakmicarById(string takmicarJMBG)
    {
        try
        {
            var takmicar = await _neo4jService.GetTakmicarByIdAsync(takmicarJMBG);

            if (takmicar == null)
            {
                return NotFound();
            }

            return Ok(takmicar);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("create-takmicar")]
    public async Task<ActionResult<Takmicar>> Post(Takmicar takmicar)
    {
        try
        {
            var createdTakmicar = await _neo4jService.CreateTakmicarAsync(takmicar);
            return Ok(createdTakmicar);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("put-takmicar/{jmbg}/{adresa}")]
    public async Task<ActionResult<Takmicar>> UpdateTakmicar(string jmbg, string adresa)
    {
        try
        {
            var updatedTakmicar = await _neo4jService.UpdateTakmicarAsync(jmbg, adresa);

            if (updatedTakmicar == null)
            {
                return NotFound($"Takmicar with JMBG {jmbg} not found");
            }

            return Ok(updatedTakmicar);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("delete-takmicar/{jmbg}")]
    public async Task<ActionResult> DeleteTakmicar(string jmbg)
    {
        try
        {
            var isDeleted = await _neo4jService.DeleteTakmicarAsync(jmbg);

            if (!isDeleted)
            {
                return NotFound($"Takmicar with JMBG {jmbg} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("takmicar-trenira-sport/{takmicarJMBG}/{sportId}")]
    public async Task<ActionResult> TakmicarTreniraSport(string takmicarJMBG, string sportId)
    {
        try
        {
            var isSuccessful = await _neo4jService.TakmicarTreniraSportAsync(takmicarJMBG, sportId);

            if (!isSuccessful)
            {
                return NotFound($"Takmicar or Sport not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpPost("dodaj-tim-takmicaru/{takmicarJMBG}/{timId}")]
    public async Task<ActionResult> DodajTimTakmicaru(string takmicarJMBG, string timId)
    {
        try
        {
            var isSuccessful = await _neo4jService.DodajTimTakmicaruAsync(takmicarJMBG, timId);

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
