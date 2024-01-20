namespace Sportsko_Takmicenje;

public class Klub
{
    public required string KlubId { get; set; }
    public required string Naziv { get; set; }
    public required string Adresa { get; set; }
    public required int GodinaOsnivanja { get; set; }
    public required string BrojTelefona { get; set; }
    public required string Email { get; set; }
    public List<Tim> ?Timovi {get; set;}
}
