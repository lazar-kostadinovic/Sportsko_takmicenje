using Microsoft.AspNetCore.Mvc;
using Sportsko_Takmicenje;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class RezultatTakmicenjaController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public RezultatTakmicenjaController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RezultatTakmicenja>> GetRezultatTakmicenjaAsync(string id)
    {
        var rezultat = await _neo4jService.GetRezultatTakmicenjaAsync(id);

        if (rezultat == null)
        {
            return NotFound();
        }

        return rezultat;
    }

    [HttpPut("{id}UpdatePobede")]
    public async Task<ActionResult<double>> UpdatePobedeAsync(string id, double novaPobeda)
    {
        var updatedPobede = await _neo4jService.UpdatePobedeAsync(id, novaPobeda);

        return Ok(updatedPobede);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRezultatTakmicenjaAsync(string id)
    {
        var success = await _neo4jService.DeleteRezultatTakmicenjaAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<RezultatTakmicenja>> CreateRezultatTakmicenjaAsync(RezultatTakmicenja rezultat)
    {
        var createdRezultat = await _neo4jService.CreateRezultatTakmicenjaAsync(rezultat);

        return CreatedAtAction(nameof(GetRezultatTakmicenjaAsync), new { id = createdRezultat.RezultatTakmicenjaId }, createdRezultat);
    }
    [HttpPost("AddTim")]
    public async Task<IActionResult> AddTimToRezultatTakmicenjaAsync(string rezultatId, string timId)
    {
        var success = await _neo4jService.AddTimToRezultatTakmicenjaAsync(rezultatId, timId);

        if (!success)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpPost("AddTakmicenje")]
    public async Task<IActionResult> AddTakmicenjeToRezultatAsync(string rezultatId, string takmicenjeId)
    {
        var success = await _neo4jService.AddTakmicenjeToRezultatAsync(rezultatId, takmicenjeId);

        if (!success)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpPost("povecajPobede")]
    public async Task<ActionResult<bool>> PovecajBrojPobeda(string timId, string takmicenjeId)
    {
        try
        {
            bool success = await _neo4jService.PovecajBrojPobeda(timId, takmicenjeId);
            return Ok(success);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška pri povećanju broja pobeda: {ex.Message}");
        }
    }

    [HttpGet("pobede/{takmicenjeId}/{timId}")]
    public async Task<ActionResult<double>> GetPobedeTimaNaTakmicenju(string takmicenjeId, string timId)
    {
        try
        {
            double pobede = await _neo4jService.GetWinsOfTeamsForCompetitionAsync(takmicenjeId, timId);
            return Ok(pobede);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška pri dohvatanju broja pobeda: {ex.Message}");
        }
    }
}

