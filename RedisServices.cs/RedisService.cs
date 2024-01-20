using StackExchange.Redis;

public class RedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    public void SetHashValue(string hashKey, string field, string value)
    {
        _db.HashSet(hashKey, field, value);
    }

    public string GetHashValue(string hashKey, string field)
    {
        return _db.HashGet(hashKey, field);
    }

    public void AddToSortedSet(string key, string member, double score)
    {
        _db.SortedSetAdd(key, member, score);
    }
    

    public SortedSetEntry[] GetSortedSetRangeByRankWithScores(string key, long start = 0, long stop = -1, Order order = Order.Ascending)
    {
        return _db.SortedSetRangeByRankWithScores(key, start, stop, order);
    }
}
