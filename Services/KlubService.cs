using Sportsko_Takmicenje;
using Neo4j.Driver;

public partial class Neo4jService
{
    public async Task<IEnumerable<Klub>> GetKluboviAsync()
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (k:Klub) OPTIONAL MATCH (k)-[:IMA_TIM]->(t:Tim) RETURN k, COLLECT(t) as timovi";
            var cursor = await session.RunAsync(query);

            var result = await cursor.ToListAsync();

            return result.Select(record =>
            {
                var klubNode = record["k"].As<INode>();
                var timovi = record["timovi"].As<List<INode>>().Select(timNode => new Tim
                {
                    Id = timNode.Properties["Id"]?.As<string>(),
                    Naziv = timNode.Properties["Naziv"]?.As<string>()
                }).ToList();

                return new Klub
                {
                    KlubId = klubNode.Properties["KlubId"]?.As<string>(),
                    Naziv = klubNode.Properties["Naziv"]?.As<string>(),
                    Adresa = klubNode.Properties["Adresa"]?.As<string>(),
                    GodinaOsnivanja = klubNode.Properties["GodinaOsnivanja"]?.As<int>() ?? 0,
                    BrojTelefona = klubNode.Properties["BrojTelefona"]?.As<string>(),
                    Email = klubNode.Properties["Email"]?.As<string>(),
                    Timovi = timovi
                };
            });
        }
    }
    public async Task<Klub> GetKlubByIdAsync(string klubId)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (k:Klub)-[:IMA_TIM]->(t:Tim) WHERE k.KlubId = $klubId RETURN k, COLLECT(t) as timovi";
            var parameters = new { klubId };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            var klubNode = result?["k"].As<INode>();
            var timovi = result?["timovi"].As<List<INode>>()?.Select(timNode => new Tim
            {
                Id = timNode.Properties["Id"]?.As<string>(),
                Naziv = timNode.Properties["Naziv"]?.As<string>()
            }).ToList() ?? new List<Tim>();

            return new Klub
            {
                KlubId = klubNode.Properties["KlubId"]?.As<string>(),
                Naziv = klubNode.Properties["Naziv"]?.As<string>(),
                Adresa = klubNode.Properties["Adresa"]?.As<string>(),
                GodinaOsnivanja = klubNode.Properties["GodinaOsnivanja"]?.As<int>() ?? 0,
                BrojTelefona = klubNode.Properties["BrojTelefona"]?.As<string>(),
                Email = klubNode.Properties["Email"]?.As<string>(),
                Timovi = timovi
            };
        }
    }


    public async Task<Klub> CreateKlubAsync(Klub klub)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "CREATE (k:Klub { KlubId: $klubId, Naziv: $naziv, Adresa: $adresa, " +
                        "GodinaOsnivanja: $godinaOsnivanja, BrojTelefona: $brojTelefona, " +
                        "Email: $email}) RETURN k";

            var parameters = new
            {
                klubId = klub.KlubId,
                naziv = klub.Naziv,
                adresa = klub.Adresa,
                godinaOsnivanja = klub.GodinaOsnivanja,
                brojTelefona = klub.BrojTelefona,
                email = klub.Email
            };

            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();

            return new Klub
            {
                KlubId = createdRecord["k"].As<INode>().Properties["KlubId"].As<string>(),
                Naziv = createdRecord["k"].As<INode>().Properties["Naziv"].As<string>(),
                Adresa = createdRecord["k"].As<INode>().Properties["Adresa"].As<string>(),
                GodinaOsnivanja = createdRecord["k"].As<INode>().Properties["GodinaOsnivanja"].As<int>(),
                BrojTelefona = createdRecord["k"].As<INode>().Properties["BrojTelefona"].As<string>(),
                Email = createdRecord["k"].As<INode>().Properties["Email"].As<string>()
            };
        }
    }

    public async Task<Klub> UpdateKlubAsync(Klub klub)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (k:Klub { KlubId: $klubId }) SET k = $k RETURN k";

            var parameters = new
            {
                klubId = klub.KlubId,
                k = new
                {
                    KlubId = klub.KlubId,
                    Naziv = klub.Naziv,
                    Adresa = klub.Adresa,
                    GodinaOsnivanja = klub.GodinaOsnivanja,
                    BrojTelefona = klub.BrojTelefona,
                    Email = klub.Email
                }
            };

            var cursor = await session.RunAsync(query, parameters);
            var updatedRecord = await cursor.SingleAsync();

            return new Klub
            {
                KlubId = updatedRecord["k"].As<INode>().Properties["KlubId"].As<string>(),
                Naziv = updatedRecord["k"].As<INode>().Properties["Naziv"].As<string>(),
                Adresa = updatedRecord["k"].As<INode>().Properties["Adresa"].As<string>(),
                GodinaOsnivanja = updatedRecord["k"].As<INode>().Properties["GodinaOsnivanja"].As<int>(),
                BrojTelefona = updatedRecord["k"].As<INode>().Properties["BrojTelefona"].As<string>(),
                Email = updatedRecord["k"].As<INode>().Properties["Email"].As<string>()
            };
        }
    }

    public async Task<bool> DeleteKlubAsync(string klubId)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (k:Klub { KlubId: $klubId }) DETACH DELETE k";
            var parameters = new { klubId };

            var cursor = await session.RunAsync(query, parameters);


            return cursor.ConsumeAsync().Result.Counters.NodesDeleted > 0;
        }
    }
    public async Task<bool> DodajTimKlubuAsync(string klubId, string timId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (k:Klub), (t:Tim) WHERE k.KlubId = $klubId AND t.Id = $timId MERGE (k)-[:IMA_TIM]->(t)";
                var parameters = new { klubId, timId };

                var cursor = await session.RunAsync(query, parameters);

                if (cursor.ConsumeAsync().Result.Counters.RelationshipsCreated > 0)
                {
                    var klub = await GetKlubByIdAsync(klubId);
                    var tim = await GetTimByIdAsync(timId);

                    if (klub != null && tim != null)
                    {
                        klub.Timovi.Add(tim);
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

    public async Task<Klub> GetKlubByNameAsync(string name)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (k:Klub) WHERE k.Naziv = $name RETURN k";
            var parameters = new { name };
            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();
            var klubNode = result?["k"].As<INode>();

            if (klubNode == null)
            {
                return null;
            }

            return new Klub
            {
                KlubId = klubNode.Properties["KlubId"]?.As<string>(),
                Naziv = klubNode.Properties["Naziv"]?.As<string>(),
                Adresa = klubNode.Properties["Adresa"]?.As<string>(),
                GodinaOsnivanja = klubNode.Properties["GodinaOsnivanja"]?.As<int>() ?? 0,
                BrojTelefona = klubNode.Properties["BrojTelefona"]?.As<string>(),
                Email = klubNode.Properties["Email"]?.As<string>()
            };
        }
    }
        

}