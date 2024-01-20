using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sportsko_Takmicenje;

[Route("api/[controller]")]
[ApiController]
public class TimController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public TimController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }
    [HttpGet("get-all-teams")]
    public async Task<ActionResult<IEnumerable<Tim>>> GetTimovi()
    {
        try
        {
            var timovi = await _neo4jService.GetTimoviAsync();
            return Ok(timovi);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("get-tim/{id}")]
    public async Task<ActionResult<Tim>> GetTim(string id)
    {
        try
        {
            var timovi = await _neo4jService.GetTimByIdAsync(id);
            return Ok(timovi);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("get-tim-by-name/{name}")]
    public async Task<ActionResult<Tim>> GetTimByName(string name)
    {
        try
        {
            var tim = await _neo4jService.GetTimByNameAsync(name);

            if (tim == null)
            {
                return NotFound($"Tim with name {name} not found");
            }

            return Ok(tim);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("create-tim")]
    public async Task<ActionResult<Takmicar>> Post(Tim tim)
    {
        try
        {
            var createdTim = await _neo4jService.CreateTimAsync(tim);
            return Ok(createdTim);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("put-tim/{id}")]
    public async Task<ActionResult<Tim>> UpdateTim(string id, [FromBody] Tim tim)
    {
        try
        {
            tim.Id = id;
            var updatedTim = await _neo4jService.UpdateTimAsync(tim);

            if (updatedTim == null)
            {
                return NotFound($"Tim with JMBG {id} not found");
            }

            return Ok(updatedTim);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("delete-tim/{Id}")]
    public async Task<ActionResult> DeleteTim(string Id)
    {
        try
        {
            var isDeleted = await _neo4jService.DeleteTimAsync(Id);

            if (!isDeleted)
            {
                return NotFound($"Tim with id {Id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("delete-tim-by-name/{name}")]
    public async Task<ActionResult> DeleteTimByName(string name)
    {
        try
        {
            var tim = await _neo4jService.GetTimByNameAsync(name);

            if (tim == null)
            {
                return NotFound($"Tim with name {name} not found");
            }

            var isDeleted = await _neo4jService.DeleteTimAsync(tim.Id);

            if (!isDeleted)
            {
                return StatusCode(500, $"Failed to delete Tim with name {name}");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("dodaj-sport-timu/{timId}/{sportId}")]
    public async Task<ActionResult> AddSportToTim(string timId, string sportId)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddTimToSportAsync(timId, sportId);

            if (!isSuccessful)
            {
                return NotFound($"Tim or sport not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("dodaj-trenera-timu/{timId}/{trenerJMBG}")]
    public async Task<ActionResult> AddTrenerToTim(string timId, string trenerJMBG)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddTimToTrenerAsync(timId, trenerJMBG);

            if (!isSuccessful)
            {
                return NotFound($"Tim or trener not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("dodaj-takmicenje-timu/{timId}/{takmicenjeId}")]
    public async Task<ActionResult> AddTakmicenjeToTim(string timId, string takmicenjeId)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddTimToTakmicenjeAsync(timId, takmicenjeId);

            if (!isSuccessful)
            {
                return NotFound($"Tim or takmicenje not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("dodaj-takmicara-timu/{timId}/{takmicarjmbg}")]
    public async Task<ActionResult> AddTakmicarToTim(string timId, string takmicarjmbg)
    {
        try
        {
            var isSuccessful = await _neo4jService.AddTimToTakmicarAsync(timId, takmicarjmbg);

            if (!isSuccessful)
            {
                return NotFound($"Tim or takmicar not found, or the relationship already exists");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}
