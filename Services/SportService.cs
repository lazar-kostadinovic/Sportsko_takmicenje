using Sportsko_Takmicenje;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Neo4jService
{
    public async Task<IEnumerable<Sport>> GetSportoviAsync()
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (s:Sport) RETURN s";
            var cursor = await session.RunAsync(query);

            var result = await cursor.ToListAsync();

            return result.Select(record => new Sport
            {
                SportId = record["s"].As<INode>().Properties["SportId"].As<string>(),
                Naziv = record["s"].As<INode>().Properties["Naziv"].As<string>(),
                BrojIgraca = record["s"].As<INode>().Properties["BrojIgraca"].As<int>(),
                Takmicari = new List<Takmicar>(),
                Treneri = new List<Trener>(),
                Timovi = new List<Tim>()
            });
        }
    }
    
    public async Task<Sport> GetSportByIdAsync(string sportId)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (s:Sport) WHERE s.SportId = $sportId RETURN s";
            var parameters = new { sportId };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            var sportProperties = result?["s"].As<INode>().Properties;
            return new Sport
            {
                SportId = sportProperties["SportId"].As<string>(),
                Naziv = sportProperties["Naziv"].As<string>(),
                BrojIgraca = sportProperties["BrojIgraca"].As<int>(),
            };
        }
    }

    public async Task<Sport> CreateSportAsync(Sport sport)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "CREATE (s:Sport { SportId: $sportId, Naziv: $naziv, BrojIgraca: $brojIgraca }) RETURN s";

            var parameters = new
            {
                sportId = sport.SportId,
                naziv = sport.Naziv,
                brojIgraca = sport.BrojIgraca
            };

            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();

            return new Sport
            {
                SportId = createdRecord["s"].As<INode>().Properties["SportId"].As<string>(),
                Naziv = createdRecord["s"].As<INode>().Properties["Naziv"].As<string>(),
                BrojIgraca = createdRecord["s"].As<INode>().Properties["BrojIgraca"].As<int>()
            };
        }
    }

    public async Task<Sport> UpdateSportAsync(string sportId, int brojIgraca)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (s:Sport { SportId: $sportId }) SET s.BrojIgraca = $brojIgraca RETURN s";

            var parameters = new
            {
                sportId,
                brojIgraca
            };

            var cursor = await session.RunAsync(query, parameters);
            var updatedRecord = await cursor.SingleAsync();

            return new Sport
            {
                SportId = updatedRecord["s"].As<INode>().Properties["SportId"].As<string>(),
                Naziv = updatedRecord["s"].As<INode>().Properties["Naziv"].As<string>(),
                BrojIgraca = updatedRecord["s"].As<INode>().Properties["BrojIgraca"].As<int>()
            };
        }
    }

    public async Task<bool> DeleteSportAsync(string sportId)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (s:Sport { SportId: $sportId }) DETACH DELETE s";
            var parameters = new { sportId };

            var cursor = await session.RunAsync(query, parameters);

            return cursor.ConsumeAsync().Result.Counters.NodesDeleted > 0;
        }
    }

    public async Task<bool> PoveziSportTakmicarAsync(string sportId, string takmicarJMBG)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (sport:Sport), (takmicar:Takmicar) WHERE sport.SportId = $sportId AND takmicar.JMBG = $takmicarJMBG MERGE (sport)-[:UKLJUCUJE]->(takmicar)";
                var parameters = new { sportId, takmicarJMBG };

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
    public async Task<bool> PoveziSportTrenerAsync(string sportId, string trenerJMBG)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (sport:Sport), (trener:Trener) WHERE sport.SportId = $sportId AND trener.JMBG = $trenerJMBG MERGE (sport)-[:IMA_TRENERA]->(trener)";
                var parameters = new { sportId, trenerJMBG };

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
    public async Task<bool> PoveziSportTimAsync(string sportId, string timId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (sport:Sport), (tim:Tim) WHERE sport.SportId = $sportId AND tim.Id = $timId MERGE (sport)-[:UKLJUCUJE_TIM]->(tim)";
                var parameters = new { sportId, timId };

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
    public async Task<Sport> GetSportByNameAsync(string naziv)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (s:Sport) WHERE s.Naziv = $naziv RETURN s";
            var parameters = new { naziv };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            var sportProperties = result?["s"].As<INode>().Properties;
            return new Sport
            {
                SportId = sportProperties["SportId"].As<string>(),
                Naziv = sportProperties["Naziv"].As<string>(),
                BrojIgraca = sportProperties["BrojIgraca"].As<int>(),
            };
        }
    }
}
