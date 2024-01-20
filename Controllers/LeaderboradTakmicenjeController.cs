using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Sportsko_Takmicenje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardTakmicenjeController : ControllerBase
    {
        private readonly LeaderboardService _leaderboardService;

        public LeaderboardTakmicenjeController(LeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("{takmicenjeId}")]
        public ActionResult GetLeaderboardTakmicenje(string takmicenjeId)
        {
            try
            {
                var leaderboardEntries = _leaderboardService.GetLeaderboardCompetition(takmicenjeId);

                var result = leaderboardEntries.Select(entry => new { TimId = entry.Element.ToString(), Pobede = entry.Score });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpPost("{utakmicaId}")]
        public async Task<ActionResult<bool>> IncreaseWinsForTopTeamAsync(string utakmicaId)
        {
            try
            {
                bool success = await _leaderboardService.IncreaseWinsForTopTeam(utakmicaId);
                return Ok(success);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
