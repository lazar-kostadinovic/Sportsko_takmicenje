namespace Sportsko_Takmicenje
{
    public class Trener
    {
        public required string JMBG { get; set; }
        public required string Ime { get; set; }
        public required string Prezime { get; set; }
        public required string DatumRodjenja { get; set; }
        public required string BrojTelefona { get; set; }
        public required string Pol { get; set; }
        public required string Adresa { get; set; }
        public required string Drzava { get; set; }
        public  Sport ?Sport { get; set; } 
        public Tim ?Tim {get; set; }
    }
}
