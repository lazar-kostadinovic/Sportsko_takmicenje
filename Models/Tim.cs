namespace Sportsko_Takmicenje;

public class Tim
{
    public required string Id { get; set; }
    public required string Naziv { get; set; }
    
    public Sport ?Sport{ get; set; }

    public Trener ?Trener { get; set; }

    public List<Takmicenje> ?Takmicenja{ get; set; }

    public List<Takmicar>? Takmicari { get; set; }

    public Klub ?Klub { get; set; }
}