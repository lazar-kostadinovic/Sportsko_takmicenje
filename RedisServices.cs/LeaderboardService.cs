using StackExchange.Redis;
using Newtonsoft.Json.Linq;
using Sportsko_Takmicenje;

public partial class LeaderboardService
{
    private readonly RedisService _redisService;
    private readonly Neo4jService _neo4jService;


    public LeaderboardService(RedisService redisService, Neo4jService neo4JService)
    {
        _redisService = redisService;
        _neo4jService = neo4JService;
    }

}