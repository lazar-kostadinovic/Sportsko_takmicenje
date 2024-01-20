using StackExchange.Redis;
using Newtonsoft.Json.Linq;
using Sportsko_Takmicenje;

public partial class LeaderboardService
{
    public async Task UpdatePointsAsync(string utakmicaId, string timId)
    {

        var poeniTask = _neo4jService.GetPointsForMatchTeamAsync(utakmicaId, timId);
        var poeni = await poeniTask;
        if (poeni != null)
        {
            double poeniDouble = (double)poeni;
            var hashKey = $"utakmica:{utakmicaId}:poeni";
            _redisService.SetHashValue(hashKey, timId, poeni.ToString());

            var leaderboardKey = $"leaderboardU:{utakmicaId}";
            _redisService.AddToSortedSet(leaderboardKey, timId, poeniDouble);
        }
    }

    public double GetPoints(string utakmicaId, string timId)
    {
        var hashKey = $"utakmica:{utakmicaId}:poeni";
        var pointsString = _redisService.GetHashValue(hashKey, timId);

        if (double.TryParse(pointsString, out var poeni))
        {
            return poeni;
        }

        return 0.0;
    }

    public SortedSetEntry[] GetLeaderboardMatch(string utakmicaId)
    {
        var leaderboardKey = $"leaderboardU:{utakmicaId}";
        return _redisService.GetSortedSetRangeByRankWithScores(leaderboardKey, order: Order.Descending);
    }



}