namespace Sportsko_Takmicenje;

public class RezultatTakmicenja
{
    public  string ?RezultatTakmicenjaId { get; set; }

    public double Pobede { get; set; }

    public Tim ?Tim{get;set;}

    public Takmicenje ?Takmicenje { get; set; }

}