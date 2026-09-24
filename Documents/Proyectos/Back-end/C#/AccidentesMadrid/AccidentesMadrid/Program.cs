using AccidentesMadrid.Services;

Console.WriteLine(" 30 Consultas LINQ ");

AccidentesLinqAnalyzer analyzer = new AccidentesLinqAnalyzer();

await analyzer.TareasLinqAsync();



var analyzerDF = new AccidentesDataFrameAnalyzer();
analyzerDF.CargarYCombinarDatos();

var watch = System.Diagnostics.Stopwatch.StartNew();

Console.WriteLine("\n--- RESULTADOS DATAFRAMES ---");
Console.WriteLine(analyzerDF.TotalA());
Console.WriteLine(analyzerDF.AccidentesDistritoTop5());
Console.WriteLine(analyzerDF.ATipo());
Console.WriteLine(analyzerDF.AMeteo());
Console.WriteLine(analyzerDF.ASexo());
Console.WriteLine(analyzerDF.ARangoEdad());
Console.WriteLine(analyzerDF.PositivosAlcohol());
Console.WriteLine(analyzerDF.PositivosDroga());
Console.WriteLine(analyzerDF.ADiaSemana());
Console.WriteLine(analyzerDF.AMes());
Console.WriteLine(analyzerDF.HoraConMasA());
Console.WriteLine(analyzerDF.LesionesFrecuentes());
Console.WriteLine(analyzerDF.VehiculoMasImplicado());
Console.WriteLine(analyzerDF.AccidentesPeatones());
Console.WriteLine(analyzerDF.ProporcionHombreMujer());
Console.WriteLine(analyzerDF.DistritosMasPeatones());
Console.WriteLine(analyzerDF.FinDeSemanaVsEntreSemana());
Console.WriteLine(analyzerDF.MediaAPorDia());
Console.WriteLine(analyzerDF.AlcoholYDrogas());
Console.WriteLine(analyzerDF.RangosEdadPeatonesVulnerables());
Console.WriteLine(analyzerDF.DistritosMasPositivosAlcohol());
Console.WriteLine(analyzerDF.ACodigoDistrito());
Console.WriteLine(analyzerDF.AAno());
Console.WriteLine(analyzerDF.EvolucionMensualAno());
Console.WriteLine(analyzerDF.DistritoMasAAno());
Console.WriteLine(analyzerDF.TendenciaAlcoholAno());
Console.WriteLine(analyzerDF.ComparativaFinDeSemanaAno());
Console.WriteLine(analyzerDF.HoraPicoAno());
Console.WriteLine(analyzerDF.LesionFrecuenteAno());
Console.WriteLine(analyzerDF.EvolucionPeatonesAno());

watch.Stop();
Console.WriteLine($"\n Tiempo de ejecución total de consultas: {watch.ElapsedMilliseconds} ms");