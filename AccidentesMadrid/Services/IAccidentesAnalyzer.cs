namespace AccidentesMadrid.Services;

public interface IAccidentesAnalyzer
{
    void CargarYCombinarDatos();

    string TotalA();
    string AccidentesDistritoTop5();
    string ATipo();
    string AMeteo();
    string ASexo();
    string ARangoEdad();
    string PositivosAlcohol();
    string PositivosDroga();
    string ADiaSemana();
    string AMes();
    string HoraConMasA();
    string LesionesFrecuentes();
    string VehiculoMasImplicado();
    string AccidentesPeatones();
    string ProporcionHombreMujer();
    string DistritosMasPeatones();
    string FinDeSemanaVsEntreSemana();
    string MediaAPorDia();
    string AlcoholYDrogas();
    string RangosEdadPeatonesVulnerables();
    string DistritosMasPositivosAlcohol();
    string ACodigoDistrito();
    string AAno();
    string EvolucionMensualAno();
    string DistritoMasAAno();
    string TendenciaAlcoholAno();
    string ComparativaFinDeSemanaAno();
    string HoraPicoAno();
    string LesionFrecuenteAno();
    string EvolucionPeatonesAno();
}