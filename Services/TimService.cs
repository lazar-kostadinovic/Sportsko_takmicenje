using Neo4j.Driver;
using Sportsko_Takmicenje;

public partial class Neo4jService
{
    public async Task<IEnumerable<Tim>> GetTimoviAsync()
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (tim:Tim) RETURN tim";
            var cursor = await session.RunAsync(query);

            var result = await cursor.ToListAsync();

            return result.Select(
                record =>
                    new Tim
                    {
                        Id = record["tim"].As<INode>().Properties["Id"].As<string>(),
                        Naziv = record["tim"].As<INode>().Properties["Naziv"].As<string>(),
                        Sport = null,
                        Trener = null,
                        Takmicenja = null,
                        Takmicari = null,
                        Klub = null
                    }
            );
        }
    }

    public async Task<Tim> GetTimByIdAsync(string timId)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (k:Tim) WHERE k.Id = $timId RETURN k";
            var parameters = new { timId };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            var timProperties = result?["k"].As<INode>().Properties;
            return new Tim
            {
                Id = timProperties["Id"]?.ToString(),
                Naziv = timProperties["Naziv"]?.ToString()
            };
        }
    }



    public async Task<Tim> CreateTimAsync(Tim tim)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "CREATE(t:Tim{Id: $id, Naziv: $naziv }) RETURN t";
            var parameters = new { id = tim.Id, naziv = tim.Naziv };
            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();

            return new Tim
            {
                Id = createdRecord["t"].As<INode>().Properties["Id"].As<string>(),
                Naziv = createdRecord["t"].As<INode>().Properties["Naziv"].As<string>()
            };
        }
    }

    public async Task<Tim> UpdateTimAsync(Tim tim)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Tim { Id: $id }) SET t.Naziv = $naziv  RETURN t";

            var parameters = new 
            { 
                id = tim.Id,
                naziv = tim.Naziv 
            };
            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();
            return new Tim
            {
                Id = createdRecord["t"].As<INode>().Properties["Id"].As<string>(),
                Naziv = createdRecord["t"].As<INode>().Properties["Naziv"].As<string>()
            };
        }
    }

    public async Task<bool> DeleteTimAsync(string id)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Tim { Id: $id }) DETACH DELETE t";
            var parameters = new { id };

            var cursor = await session.RunAsync(query, parameters);

            return cursor.ConsumeAsync().Result.Counters.NodesDeleted > 0;
        }
    }

    public async Task<bool> AddTimToSportAsync(string timId, string sportId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query =
                    "MATCH (t:Tim), (sport:Sport) WHERE t.Id = $timId AND sport.SportId = $sportId MERGE (t)-[:TIM_ZA_SPORT]->(sport)";
                var parameters = new { timId, sportId };

                var cursor = await session.RunAsync(query, parameters);

                return cursor.ConsumeAsync().Result.Counters.RelationshipsCreated > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AddTimToTrenerAsync(string timId, string trenerJMBG)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query =
                    "MATCH (t:Tim), (trener:Trener) WHERE t.Id = $timId AND trener.JMBG = $trenerjmbg MERGE (t)-[:TIM_TRENIRA_TRENER]->(trener)";
                var parameters = new { timId, trenerJMBG };

                var cursor = await session.RunAsync(query, parameters);

                return cursor.ConsumeAsync().Result.Counters.RelationshipsCreated > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AddTimToTakmicenjeAsync(string timId, string takmicenjeId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query =
                    "MATCH (t:Tim), (takmicenje:Takmicenje) WHERE t.Id = $timId AND takmicenje.TakmicenjeId = $takmicenjeid MERGE (t)-[:TIM_IMA_TAKMICENJA]->(takmicenje)";
                var parameters = new { timId, takmicenjeId };

                var cursor = await session.RunAsync(query, parameters);

                return cursor.ConsumeAsync().Result.Counters.RelationshipsCreated > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AddTimToTakmicarAsync(string timId, string takmicarjmbg)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query =
                    "MATCH (t:Tim), (takmicar:Takmicar) WHERE t.Id = $timId AND takmicar.JMBG = $takmicarjmbg MERGE (t)-[:TIM_IMA_TAKMICARE]->(takmicar)";
                var parameters = new { timId, takmicarjmbg };

                var cursor = await session.RunAsync(query, parameters);

                return cursor.ConsumeAsync().Result.Counters.RelationshipsCreated > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<Tim> GetTimByNameAsync(string name)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Tim) WHERE t.Naziv = $name RETURN t";
            var parameters = new { name };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            if (result == null)
            {
                return null;
            }

            var timProperties = result?["t"].As<INode>().Properties;
            return new Tim
            {
                Id = timProperties["Id"]?.ToString(),
                Naziv = timProperties["Naziv"]?.ToString()
            };
        }
    }
}
