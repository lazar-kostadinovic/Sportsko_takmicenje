namespace Sportsko_Takmicenje;

public class Rezultat
{
    public  string ?RezultatId { get; set; }

    public double Poeni { get; set; }

    public Tim ?Tim{get;set;}

    public Utakmica? Utakmica { get; set; }

}