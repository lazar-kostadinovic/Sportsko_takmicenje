using Sportsko_Takmicenje;
using Neo4j.Driver;

public partial class Neo4jService
{
   public async Task<Rezultat> GetRezultatAsync(string id)
    {
        using (var session=_driver.AsyncSession())
        {
            var query = "MATCH (r:Rezultat) WHERE r.RezultatId = $id RETURN r";
            var parameters = new { id };

            var cursor = await session.RunAsync(query, parameters);
            var result = await cursor.SingleAsync();

            var rezultatProperties = result?["r"].As<INode>().Properties;
            return new Rezultat
            {
              RezultatId = rezultatProperties["RezultatId"].As<string>(),
              Poeni= rezultatProperties["Poeni"].As<double>()
            };
        }  
    }

    
    
    public async Task<Rezultat> CreateRezultatAsync(Rezultat rezultat) 
    {
       using(var session = _driver.AsyncSession())
       {
            var query ="CREATE(r:Rezultat{RezultatId: $id, Poeni: $poeni }) RETURN r";
            var parameters= new
            {
                id=rezultat.RezultatId,
                poeni=rezultat.Poeni

            };
            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();

            return new Rezultat
            {
                RezultatId = createdRecord["r"].As<INode>().Properties["RezultatId"].As<string>(),
                Poeni = createdRecord["r"].As<INode>().Properties["Poeni"].As<double>()
            };
            

       }
    }
    

    public async Task<Rezultat> UpdateRezultatAsync(Rezultat rezultat)
    {
        using(var session = _driver.AsyncSession())
        {
            var query = "MATCH (r:Rezultat { RezultatId: $rezultatId }) SET r.Poeni = $poeni  RETURN r";

            var parameters = new
            {
                    rezultatId = rezultat.RezultatId,
                    poeni = rezultat.Poeni  
            };
            var cursor = await session.RunAsync(query, parameters);
            var createdRecord = await cursor.SingleAsync();
        return new Rezultat
        {
            RezultatId = createdRecord["r"].As<INode>().Properties["RezultatId"].As<string>(),
            Poeni = createdRecord["r"].As<INode>().Properties["Poeni"].As<double>()
        };
        }
    }

    public async Task<bool> DeleteRezultatAsync(string Id)
    {
        using (var session = _driver.AsyncSession())
        {
            var query = "MATCH (r:Rezultat { RezultatId: $id }) DETACH DELETE r";
            var parameters = new { id=Id };

            var cursor = await session.RunAsync(query, parameters);

            return cursor.ConsumeAsync().Result.Counters.NodesDeleted > 0;
        }
    }

         public async Task<bool> AddTimToRezultatAsync(string rezultatid, string timid)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (r:Rezultat), (t:Tim) WHERE r.RezultatId = $rezultatid AND t.Id = $timid MERGE (r)-[:REZULTAT_IMA_TIM]->(t)";
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

         public async Task<bool> AddUtakmicaToRezultatAsync(string rezultatid, string utakmicaId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = "MATCH (r:Rezultat), (u:Utakmica) WHERE r.RezultatId = $rezultatid AND u.UtakmicaId = $utakmicaid MERGE (r)-[:REZULTAT_IMA_UTAKMICA]->(u)";
                    var parameters = new { rezultatid, utakmicaId };

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




                
    public async Task<double> GetPointsForMatchTeamAsync(string utakmicaId, string timId)
    {
        try
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (r:Rezultat)-[:REZULTAT_IMA_TIM]->(t:Tim)<-[:UTAKMICA_IMA_TIMOVE]-(u:Utakmica) WHERE u.UtakmicaId = $utakmicaId AND t.Id = $timId RETURN r";
                var parameters = new { utakmicaId, timId };

                var cursor = await session.RunAsync(query, parameters);

                var result = await cursor.SingleAsync();

                var rezultatProperties = result?["r"].As<INode>().Properties;
                
                 double poeni= rezultatProperties["Poeni"].As<double>();
                 return poeni;
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 0;
        }
    }

    public async Task<Rezultat> CreateAndAssignRezultatToTimAsync(double poeni, string utakmicaId, string timId)
    {
        try
        {
            string rezultatId = IdGenerator.GenerateUniqueId();

            var rezultat = new Rezultat
            {
                RezultatId = rezultatId,
                Poeni = poeni
            };

            rezultat = await CreateRezultatAsync(rezultat);

            bool success = await AddTimToRezultatAsync(rezultat.RezultatId, timId);

            bool utakmicaSuccess = await AddUtakmicaToRezultatAsync(rezultat.RezultatId, utakmicaId);

            if (success)
            {
                return rezultat;
            }
            else
            {
                Console.WriteLine("Failed to add result to team.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
    

}
