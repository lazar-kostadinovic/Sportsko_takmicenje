using Sportsko_Takmicenje;
using Neo4j.Driver;

public partial class Neo4jService
{
    public async Task<RezultatTakmicenja> GetRezultatTakmicenjaAsync(string id)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (r:RezultatTakmicenja) WHERE r.RezultatTakmicenjaId = $id RETURN r";
            var parameters = new { id };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            var rezTakmicenjaProperties = result?["r"].As<INode>().Properties;
            return new RezultatTakmicenja
            {
                RezultatTakmicenjaId = rezTakmicenjaProperties["RezultatTakmicenjaId"]?.As<string>(),
                Pobede = (double)(rezTakmicenjaProperties["Pobede"]?.As<double>())
            };

        }

    }

    public async Task<RezultatTakmicenja> CreateRezultatTakmicenjaAsync(RezultatTakmicenja rezultat)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "CREATE(r:RezultatTakmicenja{RezultatTakmicenjaId: $id, Pobede: $pobede }) RETURN r";
            var parameters = new
            {
                id = rezultat.RezultatTakmicenjaId,
                pobede = rezultat.Pobede
            };
            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();

            return new RezultatTakmicenja
            {
                RezultatTakmicenjaId = createdRecord["r"].As<INode>().Properties["RezultatTakmicenjaId"].As<string>(),
                Pobede = createdRecord["r"].As<INode>().Properties["Pobede"].As<double>()
            };


        }
    }


    public async Task<double> UpdatePobedeAsync(string rezultatId, double novaPobeda)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (r:RezultatTakmicenja {RezultatTakmicenjaId: $id}) SET r.Pobede = $novaPobeda RETURN r.Pobede";

            var parameters = new
            {
                id = rezultatId,
                novaPobeda
            };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            return result?["r.Pobede"].As<double>() ?? 0.0;
        }
    }


    public async Task<bool> DeleteRezultatTakmicenjaAsync(string Id)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (r:RezultatTakmicenja { RezultatTakmicenjaId: $id }) DETACH DELETE r";
            var parameters = new { id = Id };

            var cursor = await session.RunAsync(query, parameters);

            return cursor.ConsumeAsync().Result.Counters.NodesDeleted > 0;
        }
    }

    public async Task<bool> AddTimToRezultatTakmicenjaAsync(string rezultatid, string timid)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (r:RezultatTakmicenja), (t:Tim) WHERE r.RezultatTakmicenjaId = $rezultatid AND t.Id = $timid MERGE (r)-[:POBEDE_ZA_TIM]->(t)";
                var parameters = new { rezultatid, timid };

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

    public async Task<bool> AddTakmicenjeToRezultatAsync(string rezultatid, string takmicenjeId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (r:RezultatTakmicenja), (t:Takmicenje) WHERE r.RezultatTakmicenjaId = $rezultatid AND t.TakmicenjeId = $takmicenjeId MERGE (r)-[:POBEDE_TIMA_NA_TAKMICENJU]->(t)";
                var parameters = new { rezultatid, takmicenjeId };

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



    public async Task<bool> PovecajBrojPobeda(string timId, string takmicenjeId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (r:RezultatTakmicenja)-[:POBEDE_ZA_TIM]->(t:Tim { Id: $timId }) MATCH (r)-[:POBEDE_TIMA_NA_TAKMICENJU]->(takmicenje:Takmicenje { TakmicenjeId: $takmicenjeId }) RETURN r";
                var parameters = new { timId, takmicenjeId };

                var cursor = await session.RunAsync(query, parameters);
                var result = await cursor.SingleAsync();
              
                var rezTakmicenjaProperties = result?["r"].As<INode>().Properties;
              
                string RezultatTakmicenjaId = rezTakmicenjaProperties["RezultatTakmicenjaId"]?.As<string>();

                var query2 = "MATCH (r:RezultatTakmicenja {RezultatTakmicenjaId: $idRezultataTakmicenja}) SET r.Pobede = r.Pobede + 1.0 RETURN r;";
                var parameters2 = new { idRezultataTakmicenja = RezultatTakmicenjaId };

                var cursor2 = await session.RunAsync(query2, parameters2); 
                var createdRecord = await cursor2.SingleAsync();

                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public async Task<double> GetWinsOfTeamsForCompetitionAsync(string takmicenjeId, string timId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (r:RezultatTakmicenja)-[:POBEDE_ZA_TIM]->(tim:Tim)-[:TIM_IMA_TAKMICENJA]-(t:Takmicenje) WHERE t.TakmicenjeId = $takmicenjeId AND tim.Id = $timId RETURN r";
                var parameters = new { takmicenjeId, timId };

                var cursor = await session.RunAsync(query, parameters);

                var result = await cursor.SingleAsync();

                var rezTakmicenjaProperties = result?["r"].As<INode>().Properties;

                double brPobeda= rezTakmicenjaProperties["Pobede"].As<double>();
                
                return brPobeda;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 0.0;
        }
    }
}
