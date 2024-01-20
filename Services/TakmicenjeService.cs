using Sportsko_Takmicenje;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Neo4jService
{
    public async Task<IEnumerable<Takmicenje>> GetTakmicenjaAsync()
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Takmicenje) RETURN t";
            var cursor = await session.RunAsync(query);

            var result = await cursor.ToListAsync();

            return result.Select(record => new Takmicenje
            {
                TakmicenjeId = record["t"].As<INode>().Properties["TakmicenjeId"].As<string>(),
                MestoOdrzavanja = record["t"].As<INode>().Properties["MestoOdrzavanja"].As<string>(),
                Naziv = record["t"].As<INode>().Properties["Naziv"].As<string>(),
                DatumOd = record["t"].As<INode>().Properties["DatumOd"].As<string>(),
                DatumDo = record["t"].As<INode>().Properties["DatumDo"].As<string>(),
                ListaUtakmica = new List<Utakmica>() 
            });
        }
    }
    public async Task<Takmicenje> GetTakmicenjeByIdAsync(string takmicenjeId)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Takmicenje) WHERE t.TakmicenjeId = $takmicenjeId RETURN t";
            var parameters = new { takmicenjeId };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            var takmicenjeProperties = result?["t"].As<INode>().Properties;
            return new Takmicenje
            {
                TakmicenjeId = takmicenjeProperties["TakmicenjeId"].As<string>(),
                MestoOdrzavanja = takmicenjeProperties["MestoOdrzavanja"].As<string>(),
                Naziv = takmicenjeProperties["Naziv"].As<string>(),
                DatumOd = takmicenjeProperties["DatumOd"].As<string>(),
                DatumDo = takmicenjeProperties["DatumDo"].As<string>(),
                ListaUtakmica = new List<Utakmica>() 
            };
        }
    }



    public async Task<Takmicenje> CreateTakmicenjeAsync(Takmicenje takmicenje)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "CREATE (t:Takmicenje { TakmicenjeId: $takmicenjeId, MestoOdrzavanja: $mestoOdrzavanja, " +
                        "Naziv: $naziv, DatumOd: $datumOd, DatumDo: $datumDo }) RETURN t";

            var parameters = new
            {
                takmicenjeId = takmicenje.TakmicenjeId,
                mestoOdrzavanja = takmicenje.MestoOdrzavanja,
                naziv = takmicenje.Naziv,
                datumOd = takmicenje.DatumOd,
                datumDo = takmicenje.DatumDo
            };

            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();

            return new Takmicenje
            {
                TakmicenjeId = createdRecord["t"].As<INode>().Properties["TakmicenjeId"].As<string>(),
                MestoOdrzavanja = createdRecord["t"].As<INode>().Properties["MestoOdrzavanja"].As<string>(),
                Naziv = createdRecord["t"].As<INode>().Properties["Naziv"].As<string>(),
                DatumOd = createdRecord["t"].As<INode>().Properties["DatumOd"].As<string>(),
                DatumDo = createdRecord["t"].As<INode>().Properties["DatumDo"].As<string>()
            };
        }
    }

    public async Task<Takmicenje> UpdateTakmicenjeAsync(Takmicenje takmicenje)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Takmicenje { TakmicenjeId: $takmicenjeId }) SET t.MestoOdrzavanja = $mestoOdrzavanja, " +
                        "t.Naziv = $naziv, t.DatumOd = $datumOd, t.DatumDo = $datumDo RETURN t";

            var parameters = new
            {
                takmicenjeId = takmicenje.TakmicenjeId,
                mestoOdrzavanja = takmicenje.MestoOdrzavanja,
                naziv = takmicenje.Naziv,
                datumOd = takmicenje.DatumOd,
                datumDo = takmicenje.DatumDo
            };

            var cursor = await session.RunAsync(query, parameters);
            var updatedRecord = await cursor.SingleAsync();

            return new Takmicenje
            {
                TakmicenjeId = updatedRecord["t"].As<INode>().Properties["TakmicenjeId"].As<string>(),
                MestoOdrzavanja = updatedRecord["t"].As<INode>().Properties["MestoOdrzavanja"].As<string>(),
                Naziv = updatedRecord["t"].As<INode>().Properties["Naziv"].As<string>(),
                DatumOd = updatedRecord["t"].As<INode>().Properties["DatumOd"].As<string>(),
                DatumDo = updatedRecord["t"].As<INode>().Properties["DatumDo"].As<string>(),
                ListaUtakmica = new List<Utakmica>()  
            };
        }
    }

    public async Task<bool> DeleteTakmicenjeAsync(string takmicenjeId)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Takmicenje { TakmicenjeId: $takmicenjeId }) DETACH DELETE t";
            var parameters = new { takmicenjeId };

            var cursor = await session.RunAsync(query, parameters);

            return cursor.ConsumeAsync().Result.Counters.NodesDeleted > 0;
        }
    }
    public async Task<bool> PoveziTakmicenjeUtakmicaAsync(string takmicenjeId, string utakmicaId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (takmicenje:Takmicenje), (utakmica:Utakmica) WHERE takmicenje.TakmicenjeId = $takmicenjeId AND utakmica.UtakmicaId = $utakmicaId MERGE (takmicenje)-[:ODRZAVA_SE]->(utakmica)";
                var parameters = new { takmicenjeId, utakmicaId };

                var cursor = await session.RunAsync(query, parameters);

                if (cursor.ConsumeAsync().Result.Counters.RelationshipsCreated > 0)
                {
                    var takmicenje = await GetTakmicenjeByIdAsync(takmicenjeId);
                    var utakmica = await GetUtakmicaAsync(utakmicaId);

                    if (takmicenje != null && utakmica != null)
                    {
                        takmicenje.ListaUtakmica.Add(utakmica);
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

    public async Task<Takmicenje> GetTakmicenjeByNameAsync(string name)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (t:Takmicenje) WHERE t.Naziv = $name RETURN t";
            var parameters = new { name };
            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();
            var takmicenjeNode = result?["t"].As<INode>();

            if (takmicenjeNode == null)
            {
                return null;
            }

            return new Takmicenje
            {
                TakmicenjeId = takmicenjeNode.Properties["TakmicenjeId"]?.As<string>(),
                MestoOdrzavanja = takmicenjeNode.Properties["MestoOdrzavanja"]?.As<string>(),
                Naziv = takmicenjeNode.Properties["Naziv"]?.As<string>(),
                DatumOd = takmicenjeNode.Properties["DatumOd"]?.As<string>(),
                DatumDo = takmicenjeNode.Properties["DatumDo"]?.As<string>(),
            };
        }
    }



    public async Task<IEnumerable<Utakmica>> GetUtakmiceByTakmicenjeNameAsync(string takmicenjeName)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Takmicenje)-[:ODRZAVA_SE]->(utakmica:Utakmica) WHERE t.Naziv = $takmicenjeName RETURN utakmica";
                var parameters = new { takmicenjeName };

                var cursor = await session.RunAsync(query, parameters);
                var result = await cursor.ToListAsync();

                return result.Select(record => new Utakmica
                {
                    UtakmicaId = record["utakmica"].As<INode>().Properties["UtakmicaId"].As<string>(),
                    Naziv = record["utakmica"].As<INode>().Properties["Naziv"].As<string>(),
                    Datum = record["utakmica"].As<INode>().Properties["Datum"].As<DateTime>(),
                    Kolo = record["utakmica"].As<INode>().Properties["Kolo"].As<string>(),
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<Utakmica>> GetUtakmiceByTakmicenjeIdAsync(string takmicenjeId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Takmicenje)-[:ODRZAVA_SE]->(utakmica:Utakmica) WHERE t.TakmicenjeId = $takmicenjeId RETURN utakmica";
                var parameters = new { takmicenjeId };

                var cursor = await session.RunAsync(query, parameters);
                var result = await cursor.ToListAsync();

                return result.Select(record => new Utakmica
                {
                    UtakmicaId = record["utakmica"].As<INode>().Properties["UtakmicaId"].As<string>(),
                    Naziv = record["utakmica"].As<INode>().Properties["Naziv"].As<string>(),
                    Datum = record["utakmica"].As<INode>().Properties["Datum"].As<DateTime>(),
                    Kolo = record["utakmica"].As<INode>().Properties["Kolo"].As<string>(),
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }



}
