using Sportsko_Takmicenje;
using Neo4j.Driver;

public partial class Neo4jService
{  
    public async Task<IEnumerable<Utakmica>> GetUtakmiceAsync()
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (u:Utakmica) RETURN u";
            var cursor = await session.RunAsync(query);

            var result = await cursor.ToListAsync();

            return result.Select(record => new Utakmica
            {
                UtakmicaId = record["u"].As<INode>().Properties["UtakmicaId"].As<string>(),
                Naziv = record["u"].As<INode>().Properties["Naziv"].As<string>(),
                Datum = DateTime.Parse(record["u"].As<INode>().Properties["Datum"].As<string>()),
                Kolo = record["u"].As<INode>().Properties["Kolo"].As<string>(),
                Timovi = new List<Tim>(), // Popunite ovo prema potrebi
                Takmicenje = null, // Popunite ovo prema potrebi
                Rezultati = new List<Rezultat>(), // Popunite ovo prema potrebi
                Sport = null // Popunite ovo prema potrebi
            });
        }
    }

    public async Task<Utakmica> GetUtakmicaAsync(string Id)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (u:Utakmica) WHERE u.UtakmicaId = $Id RETURN u ";
            var parameters = new { Id };
            var cursor = await session.RunAsync(query, parameters);

            var result = await cursor.SingleAsync();

            var utakmicaProperties = result?["u"].As<INode>().Properties;
            return new Utakmica
            {
                UtakmicaId = utakmicaProperties["UtakmicaId"].As<string>(),
                Naziv = utakmicaProperties["Naziv"].As<string>(),
                Datum = utakmicaProperties["Datum"].As<DateTime>(),
                Kolo = utakmicaProperties["Kolo"].As<string>(),

            };

        }
    }

    public async Task<Utakmica> GetUtakmicaByNameAsync(string name)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (u:Utakmica) WHERE u.Naziv = $name RETURN u ";
            var parameters = new { name };
            var cursor = await session.RunAsync(query, parameters);

            var result = await cursor.SingleAsync();

            var utakmicaProperties = result?["u"].As<INode>().Properties;
            return new Utakmica
            {
                UtakmicaId = utakmicaProperties["UtakmicaId"].As<string>(),
                Naziv = utakmicaProperties["Naziv"].As<string>(),
                Datum = utakmicaProperties["Datum"].As<DateTime>(),
                Kolo = utakmicaProperties["Kolo"].As<string>(),

            };

        }
    }

    public async Task<Utakmica> CreateUtakmicaAsync(Utakmica utakmica)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "CREATE(u:Utakmica{UtakmicaId: $id, Naziv: $naziv, Datum: $datum, Kolo: $kolo }) RETURN u";
            var parameters = new
            {
                id = utakmica.UtakmicaId,
                naziv = utakmica.Naziv,
                datum = utakmica.Datum,
                kolo = utakmica.Kolo

            };
            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();

            return new Utakmica
            {
                UtakmicaId = createdRecord["u"].As<INode>().Properties["UtakmicaId"].As<string>(),
                Naziv = createdRecord["u"].As<INode>().Properties["Naziv"].As<string>(),
                Datum = createdRecord["u"].As<INode>().Properties["Datum"].As<DateTime>(),
                Kolo = createdRecord["u"].As<INode>().Properties["Kolo"].As<string>(),
            };
        }
    }

    public async Task<Utakmica> UpdateUtakmicaAsync(Utakmica utakmica)
    {
        using (var session = _driver.AsyncSession())
        {        
            var query = "MATCH (u:Utakmica { UtakmicaId: $utakmicaId }) SET u.Naziv = $naziv, " +
                        "u.Datum = $datum, u.Kolo = $kolo RETURN u";

            var parameters = new
            {
                    utakmicaId = utakmica.UtakmicaId,
                    naziv = utakmica.Naziv,
                    datum = utakmica.Datum,
                    kolo = utakmica.Kolo
            };
            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();
            return new Utakmica
            {
                UtakmicaId = createdRecord["u"].As<INode>().Properties["UtakmicaId"].As<string>(),
                Naziv = createdRecord["u"].As<INode>().Properties["Naziv"].As<string>(),
                Datum = createdRecord["u"].As<INode>().Properties["Datum"].As<DateTime>(),
                Kolo = createdRecord["u"].As<INode>().Properties["Kolo"].As<string>(),
            };

        }
    }


    public async Task<bool> DeleteUtakmicaAsync(string utakmicaId)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (u:Utakmica { UtakmicaId: $utakmicaId }) DETACH DELETE u";
            var parameters = new { utakmicaId };

            var cursor = await session.RunAsync(query, parameters);

            return cursor.ConsumeAsync().Result.Counters.NodesDeleted > 0;
        }
    }

    public async Task<bool> AddTimToUtakmicaAsync(string utakmicaid, string timid)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (u:Utakmica), (t:Tim) WHERE u.UtakmicaId = $utakmicaid AND t.Id = $timid MERGE (u)-[:UTAKMICA_IMA_TIMOVE]->(t)";
                var parameters = new { utakmicaid, timid };

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


    public async Task<bool> AddTakmicenjeToUtakmicaAsync(string utakmicaid, string takmicenjeid)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (u:Utakmica), (t:Takmicenje) WHERE u.UtakmicaId = $utakmicaid AND t.TakmicenjeId = $takmicenjeid MERGE (t)-[:ODRZAVA_SE]->(u)";
                var parameters = new { utakmicaid, takmicenjeid };

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

    public async Task<bool> AddRezultatToUtakmicaAsync(string utakmicaid, string rezultatid)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (u:Utakmica), (r:Rezultat) WHERE u.UtakmicaId = $utakmicaid AND r.RezultatId = $rezultatid  MERGE (u)-[:UTAKMICA_IMA_REZULTATE]->(r)";
                var parameters = new { utakmicaid, rezultatid };

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

    public async Task<bool> AddSportToUtakmicaAsync(string utakmicaid, string sportid)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (u:Utakmica), (s:Sport) WHERE u.UtakmicaId = $utakmicaid AND s.SportId = $sportid MERGE (u)-[:UTAKMICA_IMA_SPORT]->(s)";
                var parameters = new { utakmicaid, sportid };

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

    public async Task<Dictionary<string, double>> GetTeamPointsByUtakmicaIdAsync(string utakmicaId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (u:Utakmica {UtakmicaId: $utakmicaId})-[:UTAKMICA_IMA_REZULTATE]->(r:Rezultat)-[:REZULTAT_ZA_TIM]->(t:Tim) RETURN t.Naziv AS NazivTima, r.Poeni AS Poeni";
                var parameters = new { utakmicaId };

                var cursor = await session.RunAsync(query, parameters);

                var result = await cursor.ToListAsync();

                return result.ToDictionary(
                    record => record["NazivTima"].As<string>(),
                    record => record["Poeni"].As<double>()
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new Dictionary<string, double>();
        }
    }

    public async Task<List<Rezultat>> GetRezultatiForUtakmicaAsync(string utakmicaId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (u:Utakmica {UtakmicaId: $utakmicaId})-[:UTAKMICA_IMA_REZULTATE]->(r:Rezultat) RETURN r";
                var parameters = new { utakmicaId };

                var cursor = await session.RunAsync(query, parameters);

                var result = await cursor.ToListAsync();

                return result.Select(record => new Rezultat
                {
                    RezultatId = record["r"].As<INode>().Properties["RezultatId"].As<string>(),
                    Poeni = record["r"].As<INode>().Properties["Poeni"].As<double>(),
                    // Add other properties of Rezultat as needed
                }).ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new List<Rezultat>();
        }
    }

    public async Task<string> PreuzmiIdTakmicenjaZaUtakmicu(string utakmicaId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (u:Utakmica)-[:ODRZAVA_SE]-(takmicenje:Takmicenje) WHERE u.UtakmicaId = $utakmicaId RETURN takmicenje";
                var parameters = new { utakmicaId };
                var cursor = await session.RunAsync(query, parameters);

                var result = await cursor.SingleAsync();
                     
                var utakmicaProperties = result?["takmicenje"].As<INode>().Properties;
                string takmicenjeid = utakmicaProperties["TakmicenjeId"].As<string>();
                return takmicenjeid;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return " ";
        }
    }

    public async Task<List<string>> GetTeamIdsByUtakmicaIdAsync(string utakmicaId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (u:Utakmica {UtakmicaId: $utakmicaId})-[:UTAKMICA_IMA_TIMOVE]->(t:Tim) RETURN t.Id AS TeamId";
                var parameters = new { utakmicaId };

                var cursor = await session.RunAsync(query, parameters);
                var result = await cursor.ToListAsync();

                return result.Select(record => record["TeamId"].As<string>()).ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new List<string>();
        }
    }

}
