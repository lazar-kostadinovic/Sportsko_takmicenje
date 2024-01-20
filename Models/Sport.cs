namespace Sportsko_Takmicenje;

public class Sport
{
    public required string SportId { get; set; }
    public required string Naziv { get; set; }
    public int BrojIgraca { get; set; }
    public List<Takmicar> ?Takmicari { get; set; }
    public List<Trener> ?Treneri { get; set; }
    public List<Tim> ?Timovi { get; set; }
}
