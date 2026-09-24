namespace AccidentesMadrid.Models;

public class Accidente
{
    public string NumExpediente { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime Hora { get; set; }
    public string Localizacion { get; set; }
    public string NumeroLocalizacion { get; set; }
    public string CodDistrito { get; set; }
    public string Distrito { get; set; }
    public string TipoAccidente { get; set; }
    public string EstadoMeteorologico { get; set; }
    public string TipoVehiculo { get; set; }
    public TipoPersona TipoPersona { get; set; }
    public string RengoEdad { get; set; }
    public Sexo Sexo { get; set; }
    public string CodLesividad  { get; set; }
    public string Lesividad { get; set; }
    public string CordenadaX  { get; set; }
    public string CordenadaY { get; set; }
    
    public bool Alchol { get; set; }
    // Propiedad que devuelve texto según el bool
    public string AlcholTexto => Alchol ? "Positivo" : "Negativo";
    
    public bool Drogas { get; set; }
    public string DrogasTexto => Drogas ? "Positivo" : "Negativo";
}