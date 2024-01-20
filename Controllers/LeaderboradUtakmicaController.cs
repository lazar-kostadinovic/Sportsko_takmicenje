using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Sportsko_Takmicenje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardUtakmicaController : ControllerBase
    {
        private readonly LeaderboardService _leaderboardService;

        public LeaderboardUtakmicaController(LeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("{utakmicaId}")]
        public IActionResult GetLeaderboard(string utakmicaId)
        {
            try
            {
                var leaderboardEntries = _leaderboardService.GetLeaderboardMatch(utakmicaId);
                
                var result = leaderboardEntries.Select(entry => new { TimId = entry.Element.ToString(), Poeni = entry.Score });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("{utakmicaId}/{timId}")]
        public ActionResult UpdatePoints(string utakmicaId, string timId)
        {
            try
            {
                _leaderboardService.UpdatePointsAsync(utakmicaId, timId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
