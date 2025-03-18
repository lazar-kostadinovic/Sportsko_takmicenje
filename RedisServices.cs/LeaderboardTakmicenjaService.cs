using StackExchange.Redis;
using Newtonsoft.Json.Linq;
using Sportsko_Takmicenje;

public partial class LeaderboardService
{
    public async Task<bool> IncreaseWinsForTopTeam(string utakmicaId)
    {
        var leaderboard = GetLeaderboardMatch(utakmicaId);
        if (leaderboard.Length > 0)
        {
            var topTeamId = leaderboard[0].Element;


            var tim = await _neo4jService.GetTimByIdAsync(topTeamId);
            var takmicenjeAsync =await  _neo4jService.PreuzmiIdTakmicenjaZaUtakmicu(utakmicaId);
            // string takmicenjeAsync = await takmicenje;

            if (tim != null && takmicenjeAsync != null)
            {
                await _neo4jService.PovecajBrojPobeda(topTeamId, takmicenjeAsync);


                var pobedeTask = _neo4jService.GetWinsOfTeamsForCompetitionAsync(takmicenjeAsync, topTeamId);
                var pobede = await pobedeTask;

                if (pobede != null)
                {
                    double pobedeDouble = (double)pobede;
                    var hashKey = $"takmicenje:{takmicenjeAsync}:pobede";
                    _redisService.SetHashValue(hashKey, topTeamId, pobede.ToString());

                    var leaderboardKey = $"leaderboardT:{takmicenjeAsync}";
                    _redisService.AddToSortedSet(leaderboardKey, topTeamId, pobede);

                    return true;
                }

            }

        }
        return false;
    }
    // private async Task UpdateWinsAsync(string takmicenjeId, string timId)
    // {
    //     try
    //     {
    //         var pobedeTask = _neo4jService.GetWinsOfTeamsForCompetitionAsync(takmicenjeId, timId);
    //         var pobede = await pobedeTask;

    //         if (pobede != null)
    //         {
    //             double pobedeDouble = (double)pobede;
    //             var hashKey = $"takmicenje:{takmicenjeId}:pobede";
    //             _redisService.SetHashValue(hashKey, timId, pobede.ToString());

    //             var leaderboardKey = $"leaderboardT:{takmicenjeId}";
    //             _redisService.AddToSortedSet(leaderboardKey, timId, pobedeDouble);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         // Handle exceptions (log or rethrow as appropriate)
    //         Console.WriteLine($"Error updating winning teams leaderboard: {ex.Message}");
    //     }
    // }
    public double GetWins(string takmicenjeId, string timId)
    {
        var hashKey = $"takmicenje:{takmicenjeId}:pobede";
        var pobedeString = _redisService.GetHashValue(hashKey, timId);

        if (double.TryParse(pobedeString, out var pobede))
        {
            return pobede;
        }

        return 0.0;
    }

    public SortedSetEntry[] GetLeaderboardCompetition(string takmicenjeId)
    {
        var leaderboardKey = $"leaderboardT:{takmicenjeId}";
        return _redisService.GetSortedSetRangeByRankWithScores(leaderboardKey, order: Order.Descending);
    }
}
