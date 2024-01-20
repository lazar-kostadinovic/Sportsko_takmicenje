using Sportsko_Takmicenje;
using Neo4j.Driver;
using Neo4jClient.Cypher;

public partial class Neo4jService
{
    public async Task<IEnumerable<Trener>> GetTreneriAsync()
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Trener) RETURN t";
            var cursor = await session.RunAsync(query);

            var result = await cursor.ToListAsync();

            return result.Select(record => new Trener
            {
                JMBG = record["t"].As<INode>().Properties["JMBG"]?.As<string>(),
                Ime = record["t"].As<INode>().Properties["Ime"]?.As<string>(),
                Prezime = record["t"].As<INode>().Properties["Prezime"]?.As<string>(),
                DatumRodjenja = record["t"].As<INode>().Properties["DatumRodjenja"]?.As<string>(),
                BrojTelefona = record["t"].As<INode>().Properties["BrojTelefona"]?.As<string>(),
                Pol = record["t"].As<INode>().Properties["Pol"]?.As<string>(),
                Adresa = record["t"].As<INode>().Properties["Adresa"]?.As<string>(),
                Drzava = record["t"].As<INode>().Properties["Drzava"]?.As<string>()
            });
        }
    }

    public async Task<Trener> CreateTrenerAsync(Trener trener)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "CREATE (t:Trener { JMBG: $jmbg, Ime: $ime, Prezime: $prezime, " +
                        "DatumRodjenja: $datumRodjenja, BrojTelefona: $brojTelefona, " +
                        "Pol: $pol, Adresa: $adresa, Drzava: $drzava }) RETURN t";

            var parameters = new
            {
                jmbg = trener.JMBG,
                ime = trener.Ime,
                prezime = trener.Prezime,
                datumRodjenja = trener.DatumRodjenja,
                brojTelefona = trener.BrojTelefona,
                pol = trener.Pol,
                adresa = trener.Adresa,
                drzava = trener.Drzava
            };

            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();

            return new Trener
            {
                JMBG = createdRecord["t"].As<INode>().Properties["JMBG"].As<string>(),
                Ime = createdRecord["t"].As<INode>().Properties["Ime"].As<string>(),
                Prezime = createdRecord["t"].As<INode>().Properties["Prezime"].As<string>(),
                DatumRodjenja = createdRecord["t"].As<INode>().Properties["DatumRodjenja"].As<string>(),
                BrojTelefona = createdRecord["t"].As<INode>().Properties["BrojTelefona"].As<string>(),
                Pol = createdRecord["t"].As<INode>().Properties["Pol"].As<string>(),
                Adresa = createdRecord["t"].As<INode>().Properties["Adresa"].As<string>(),
                Drzava = createdRecord["t"].As<INode>().Properties["Drzava"].As<string>()
            };
        }
    }




    public async Task<Trener> UpdateTrenerAsync(string jmbg, string novaAdresa)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Trener { JMBG: $jmbg }) SET t.Adresa = $novaAdresa RETURN t";

            var parameters = new
            {
                jmbg,
                novaAdresa
            };

            var cursor = await session.RunAsync(query, parameters);
            var updatedRecord = await cursor.SingleAsync();

            return new Trener
            {
                JMBG = updatedRecord["t"].As<INode>().Properties["JMBG"].As<string>(),
                Ime = updatedRecord["t"].As<INode>().Properties["Ime"].As<string>(),
                Prezime = updatedRecord["t"].As<INode>().Properties["Prezime"].As<string>(),
                DatumRodjenja = updatedRecord["t"].As<INode>().Properties["DatumRodjenja"].As<string>(),
                BrojTelefona = updatedRecord["t"].As<INode>().Properties["BrojTelefona"].As<string>(),
                Pol = updatedRecord["t"].As<INode>().Properties["Pol"].As<string>(),
                Adresa = updatedRecord["t"].As<INode>().Properties["Adresa"].As<string>(),
                Drzava = updatedRecord["t"].As<INode>().Properties["Drzava"].As<string>()
            };
        }
    }


    public async Task<bool> DeleteTrenerAsync(string jmbg)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Trener { JMBG: $jmbg }) DETACH DELETE t";
            var parameters = new { jmbg };

            var cursor = await session.RunAsync(query, parameters);

            return cursor.ConsumeAsync().Result.Counters.NodesDeleted > 0;
        }
    }

    public async Task<Trener> GetTrenerByIdAsync(string JMBG)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Trener) WHERE t.JMBG = $JMBG RETURN t";
            var parameters = new { JMBG };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            var trenerProperties = result?["t"].As<INode>().Properties;
            return new Trener
            {
                JMBG = trenerProperties["JMBG"]?.ToString(),
                Ime = trenerProperties["Ime"]?.ToString(),
                Prezime = trenerProperties["Prezime"]?.ToString(),
                DatumRodjenja = trenerProperties["DatumRodjenja"]?.ToString(),
                BrojTelefona = trenerProperties["BrojTelefona"]?.ToString(),
                Pol = trenerProperties["Pol"]?.ToString(),
                Adresa = trenerProperties["Adresa"]?.ToString(),
                Drzava = trenerProperties["Drzava"]?.ToString()
            };
        }
    }

 
    public async Task<bool> AddSportToCoachAsync(string trenerJMBG, string sportId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Trener), (sport:Sport) WHERE t.JMBG = $trenerJMBG AND sport.SportId = $sportId MERGE (t)-[:TRENER_ZA_SPORT]->(sport)";
                var parameters = new { trenerJMBG, sportId };

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

    public async Task<bool> AddTeamToCoachAsync(string trenerJMBG, string timId)
    {
        try
        {
            using(var session =_driver.AsyncSession())
            {
                var query = "MATCH (t:Trener),(tim:Tim) WHERE t.JMBG = $trenerJMBG AND tim.Id = $timId MERGE (t)-[:TRENER_ZA_TIM]->(tim)";
                var parameters = new { trenerJMBG, timId };

                var cursor = await session.RunAsync(query,parameters);
                return cursor.ConsumeAsync().Result.Counters.RelationshipsCreated>0;
            }

        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error:{ex.Message}");
            return false;
        }
    }

}