using Sportsko_Takmicenje;
using Neo4j.Driver;

public partial class Neo4jService
{
    public async Task<IEnumerable<Takmicar>> GetTakmicariAsync()
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Takmicar) RETURN t";
            var cursor = await session.RunAsync(query);

            var result = await cursor.ToListAsync();

            return result.Select(record => new Takmicar
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

    public async Task<Takmicar> CreateTakmicarAsync(Takmicar takmicar)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "CREATE (t:Takmicar { JMBG: $jmbg, Ime: $ime, Prezime: $prezime, " +
                        "DatumRodjenja: $datumRodjenja, BrojTelefona: $brojTelefona, " +
                        "Pol: $pol, Adresa: $adresa, Drzava: $drzava }) RETURN t";

            var parameters = new
            {
                jmbg = takmicar.JMBG,
                ime = takmicar.Ime,
                prezime = takmicar.Prezime,
                datumRodjenja = takmicar.DatumRodjenja,
                brojTelefona = takmicar.BrojTelefona,
                pol = takmicar.Pol,
                adresa = takmicar.Adresa,
                drzava = takmicar.Drzava
            };

            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();

            return new Takmicar
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

    public async Task<Takmicar> UpdateTakmicarAsync(string jmbg, string novaAdresa)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Takmicar { JMBG: $jmbg }) SET t.Adresa = $novaAdresa RETURN t";

            var parameters = new
            {
                jmbg,
                novaAdresa
            };

            var cursor = await session.RunAsync(query, parameters);
            var updatedRecord = await cursor.SingleAsync();

            return new Takmicar
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

    public async Task<bool> DeleteTakmicarAsync(string jmbg)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Takmicar { JMBG: $jmbg }) DETACH DELETE t";
            var parameters = new { jmbg };

            var cursor = await session.RunAsync(query, parameters);

            return cursor.ConsumeAsync().Result.Counters.NodesDeleted > 0;
        }
    }

    public async Task<bool> TakmicarTreniraSportAsync(string takmicarJMBG, string sportId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Takmicar), (s:Sport) WHERE t.JMBG = $takmicarJMBG AND s.SportId = $sportId MERGE (t)-[:TAKMICI_SE_U]->(s)";
                var parameters = new { takmicarJMBG, sportId };

                var cursor = await session.RunAsync(query, parameters);

                if (cursor.ConsumeAsync().Result.Counters.RelationshipsCreated > 0)
                {
                    var takmicar = await GetTakmicarByIdAsync(takmicarJMBG);
                    var sport = await GetSportByIdAsync(sportId);

                    if (takmicar != null && sport != null)
                    {
                        takmicar.Sportovi.Add(sport);
                        return true;
                    }
                }

                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<Takmicar> GetTakmicarByIdAsync(string takmicarJMBG)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Takmicar) WHERE t.JMBG = $JMBG RETURN t";
            var parameters = new { takmicarJMBG };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            var takmicarProperties = result?["t"].As<INode>().Properties;
            return new Takmicar
            {
                JMBG = takmicarProperties["JMBG"]?.ToString(),
                Ime = takmicarProperties["Ime"]?.ToString(),
                Prezime = takmicarProperties["Prezime"]?.ToString(),
                DatumRodjenja = takmicarProperties["DatumRodjenja"]?.ToString(),
                BrojTelefona = takmicarProperties["BrojTelefona"]?.ToString(),
                Pol = takmicarProperties["Pol"]?.ToString(),
                Adresa = takmicarProperties["Adresa"]?.ToString(),
                Drzava = takmicarProperties["Drzava"]?.ToString()
            };
        }
    }
    public async Task<bool> DodajTimTakmicaruAsync(string takmicarJMBG, string timId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Takmicar), (tim:Tim) WHERE t.JMBG = $takmicarJMBG AND tim.Id = $timId MERGE (t)-[:TAKMICAR_U_TIMU]->(tim)";
                var parameters = new { takmicarJMBG, timId };

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

}