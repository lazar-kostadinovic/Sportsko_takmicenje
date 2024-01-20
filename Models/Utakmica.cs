using System.Data;

namespace Sportsko_Takmicenje;

public class Utakmica
{
   public  string ?UtakmicaId { get; set; }

   public required string Naziv{get;set;}

   public required DateTime Datum { get; set; }

   public required string Kolo { get; set; }

   public List<Tim> ?Timovi{ get;set; }

   public Takmicenje ?Takmicenje { get; set; }

   public List<Rezultat> ?Rezultati{ get; set; }

   public Sport ?Sport { get; set; }

}