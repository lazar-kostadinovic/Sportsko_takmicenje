namespace Sportsko_Takmicenje
{
    public class Takmicenje
    {
        public required string TakmicenjeId { get; set; }
        public required string MestoOdrzavanja { get; set; }
        public required string Naziv { get; set; }
        public required string DatumOd { get; set; }
        public required string DatumDo { get; set; }
        public  List<Utakmica> ?ListaUtakmica { get; set; }
    }
}
