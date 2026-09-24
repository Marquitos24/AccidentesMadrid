

using System.Globalization;
using System.Text;
using Microsoft.Data.Analysis;

namespace AccidentesMadrid.Services;

public class AccidentesDataFrameAnalyzer : IAccidentesAnalyzer
{
    private DataFrame? _df;

    /// <summary>
    /// Lee los 3 ficheros CSV utilizando Microsoft.Data.Analysis y los combina en un único DataFrame.
    /// </summary>
    public void CargarYCombinarDatos()
    {
        string basePath = Path.Combine(AppContext.BaseDirectory, "../../../data");
        
        string file2024 = Path.Combine(basePath, "2024-accidentes-trafico-detalle-csv.csv");
        string file2025 = Path.Combine(basePath, "2025-accidentes-trafico-detalle-csv.csv");
        string file2026 = Path.Combine(basePath, "2026-accidentes-trafico-detalle.csv");
        
        // Cargamos cada fichero  todas las columnas a string y asi evito fallos 
        _df = CargarCsvComoTexto(file2024);
        var df2025 = CargarCsvComoTexto(file2025);
        var df2026 = CargarCsvComoTexto(file2026);

        _df.Append(df2025.Rows, inPlace: true);
        _df.Append(df2026.Rows, inPlace: true);

        Console.WriteLine($"[DataFrame] Datos cargados y combinados correctamente. Total de filas: {_df.Rows.Count}");
    }

// Para cargar cualquier CSV evitando errores de tipos de datos
    private DataFrame CargarCsvComoTexto(string rutaArchivo)
    {
        // Leemos la cabecera para saber cuántas columnas tiene el archivo CSV
        string primeraLinea = File.ReadLines(rutaArchivo).First();
        string[] columnas = primeraLinea.Split(';');
    
        // Creo un array de tipos donde todas las columnas son string
        Type[] tipos = new Type[columnas.Length];
        Array.Fill(tipos, typeof(string));

        // Cargamos el CSV aplicando este esquema seguro
        return DataFrame.LoadCsv(rutaArchivo, separator: ';', header: true, dataTypes: tipos);
    }

    /// <summary>
    /// Ejecución de las consultas mediante DataFrame para replicar el análisis.
    /// </summary>
      public string TotalA()
    {
        if (_df == null) return "DataFrame no cargado.";
        long total = _df.Rows.Count;
        return $"1.Total de accidentes: {total} \n";
    }
    
   // 2. Accidentes por distrito (top 5)
    public string AccidentesDistritoTop5()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var valueCounts = _df["distrito"].ValueCounts();
        var colCat = (StringDataFrameColumn)valueCounts.Columns[0];
        var colCount = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
        
        var sb = new StringBuilder("2. Accidentes por distrito (Top 5):\n");
        
        int limit = Math.Min(5, (int)valueCounts.Rows.Count);
        for (int i = 0; i < limit; i++)
        {
            sb.AppendLine($"   - {colCat[i]}: {colCount[i]}");
        }
        return sb.ToString().TrimEnd();
    }

    // 3. Accidentes por tipo
    public string ATipo()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var valueCounts = _df["tipo_accidente"].ValueCounts();
        var colCat = (StringDataFrameColumn)valueCounts.Columns[0];
        var colCount = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
        
        var sb = new StringBuilder("\n3. Accidentes por tipo:\n");
        
        for (int i = 0; i < valueCounts.Rows.Count; i++)
        {
            sb.AppendLine($"   - {colCat[i]}: {colCount[i]}");
        }
        return sb.ToString().TrimEnd();
    }

    // 4. Accidentes por estado meteorológico
    public string AMeteo()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var valueCounts = _df["estado_meteorológico"].ValueCounts();
        var colCat = (StringDataFrameColumn)valueCounts.Columns[0];
        var colCount = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
        
        var sb = new StringBuilder("\n4. Accidentes por estado meteorológico:\n");
        
        for (int i = 0; i < valueCounts.Rows.Count; i++)
        {
            sb.AppendLine($"   - {colCat[i]}: {colCount[i]}");
        }
        return sb.ToString().TrimEnd();
    }

    // 5. Accidentes por sexo
    public string ASexo()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var valueCounts = _df["sexo"].ValueCounts();
        var colCat = (StringDataFrameColumn)valueCounts.Columns[0];
        var colCount = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
        
        var sb = new StringBuilder("\n5. Accidentes por sexo:\n");
        
        for (int i = 0; i < valueCounts.Rows.Count; i++)
        {
            sb.AppendLine($"   - {colCat[i]}: {colCount[i]}");
        }
        return sb.ToString().TrimEnd();
    }

    // 6. Accidentes por rango de edad
    public string ARangoEdad()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var valueCounts = _df["rango_edad"].ValueCounts();
        var colCat = (StringDataFrameColumn)valueCounts.Columns[0];
        var colCount = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
        
        var sb = new StringBuilder("\n6. Accidentes por rango de edad:\n");
        
        for (int i = 0; i < valueCounts.Rows.Count; i++)
        {
            sb.AppendLine($"   - {colCat[i]}: {colCount[i]}");
        }
        return sb.ToString().TrimEnd();
    }
    // 7. Positivos en alcohol
    public string PositivosAlcohol()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colAlcohol = (StringDataFrameColumn)_df["positiva_alcohol"];
        long positivos = _df.Filter(colAlcohol.ElementwiseEquals("S")).Rows.Count;
        
        return $"7. Positivos en alcohol: {positivos} \n";
    }

    // 8. Positivos en drogas
    public string PositivosDroga()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colDroga = (StringDataFrameColumn)_df["positiva_droga"];
        long positivos = _df.Filter(colDroga.ElementwiseEquals("S")).Rows.Count;
        
        return $"8. Positivos en drogas: {positivos}\n";
    }

    // 9. Accidentes por día de la semana
    public string ADiaSemana()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var culturaEspana = new CultureInfo("es-ES");
        var diasSemana = new Dictionary<string, int>();

        foreach (var fechaStr in colFecha)
        {
            if (fechaStr != null && DateTime.TryParseExact(fechaStr.ToString(), "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string dia = dt.ToString("dddd", culturaEspana);
                // Capitalizar primera letra
                dia = char.ToUpper(dia[0]) + dia.Substring(1);
                diasSemana[dia] = diasSemana.GetValueOrDefault(dia, 0) + 1;
            }
        }

        var sb = new StringBuilder("9. Accidentes por día de la semana:\n");
        foreach (var kvp in diasSemana)
        {
            sb.AppendLine($"   - {kvp.Key}: {kvp.Value}");
        }
        return sb.ToString().TrimEnd();
    }

    // 10. Accidentes por mes
    public string AMes()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var culturaEspana = new CultureInfo("es-ES");
        var meses = new Dictionary<string, int>();

        foreach (var fechaStr in colFecha)
        {
            if (fechaStr != null && DateTime.TryParseExact(fechaStr.ToString(), "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string mes = dt.ToString("MMMM", culturaEspana);
                mes = char.ToUpper(mes[0]) + mes.Substring(1);
                meses[mes] = meses.GetValueOrDefault(mes, 0) + 1;
            }
        }

        var sb = new StringBuilder("\n10. Accidentes por mes:\n");
        foreach (var kvp in meses)
        {
            sb.AppendLine($"   - {kvp.Key}: {kvp.Value}");
        }
        return sb.ToString().TrimEnd();
    }

    // 11. Hora con más accidentes
    public string HoraConMasA()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colHora = (StringDataFrameColumn)_df["hora"];
        var horas = new Dictionary<string, int>();

        foreach (var horaStr in colHora)
        {
            if (horaStr != null)
            {
                string horaTexto = horaStr.ToString()!;
                // Las horas suelen venir como "HH:mm:ss" o "HH". Extraemos la hora principal (primeros 2 caracteres)
                string horaKey = horaTexto.Length >= 2 ? horaTexto.Substring(0, 2) : horaTexto;
                horas[horaKey] = horas.GetValueOrDefault(horaKey, 0) + 1;
            }
        }

        var horaTop = horas.OrderByDescending(x => x.Value).FirstOrDefault();
        return $"11. Hora con más accidentes: {horaTop.Key}:00 horas ({horaTop.Value} accidentes)\n";
    }

    // 12. Lesiones más frecuentes
    public string LesionesFrecuentes()
    {
        if (_df == null) return "DataFrame no cargado.";
    
        var valueCounts = _df["lesividad"].ValueCounts();
        var colCat = (StringDataFrameColumn)valueCounts.Columns[0];
        var colCount = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
    
        var sb = new StringBuilder("12. Lesiones más frecuentes:\n");
    
        int limit = Math.Min(5, (int)valueCounts.Rows.Count);
        for (int i = 0; i < limit; i++)
        {
            sb.AppendLine($"   - {colCat[i]}: {colCount[i]}");
        }
        return sb.ToString().TrimEnd();
    }

// 13. Tipo de vehículo más implicado
    public string VehiculoMasImplicado()
    {
        if (_df == null) return "DataFrame no cargado.";
    
        var valueCounts = _df["tipo_vehiculo"].ValueCounts();
        var colCat = (StringDataFrameColumn)valueCounts.Columns[0];
        var colCount = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
    
        var sb = new StringBuilder("\n13. Tipo de vehículo más implicado (Top 5):\n");
    
        int limit = Math.Min(5, (int)valueCounts.Rows.Count);
        for (int i = 0; i < limit; i++)
        {
            sb.AppendLine($"   - {colCat[i]}: {colCount[i]}");
        }
        return sb.ToString().TrimEnd();
    }

    // 14. Accidentes con peatones
    public string AccidentesPeatones()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colPersona = (StringDataFrameColumn)_df["tipo_persona"];
        long peatones = _df.Filter(colPersona.ElementwiseEquals("Peatón")).Rows.Count;
        
        return $"\n14. Accidentes con peatones: {peatones}\n";
    }

    // 15. Proporción hombre/mujer
    public string ProporcionHombreMujer()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colSexo = (StringDataFrameColumn)_df["sexo"];
        long hombres = _df.Filter(colSexo.ElementwiseEquals("Hombre")).Rows.Count;
        long mujeres = _df.Filter(colSexo.ElementwiseEquals("Mujer")).Rows.Count;
        
        return $"15. Proporción hombre/mujer -> Hombres: {hombres} | Mujeres: {mujeres}\n";
    }

    // 16. Distritos con más peatones
    public string DistritosMasPeatones()
    {
        if (_df == null) return "DataFrame no cargado.";
    
        var colPersona = (StringDataFrameColumn)_df["tipo_persona"];
        var dfPeatones = _df.Filter(colPersona.ElementwiseEquals("Peatón"));
    
        var valueCounts = dfPeatones["distrito"].ValueCounts();
        var colCat = (StringDataFrameColumn)valueCounts.Columns[0];
        var colCount = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
    
        var sb = new StringBuilder("16. Distritos con más peatones implicados (Top 5):\n");
    
        int limit = Math.Min(5, (int)valueCounts.Rows.Count);
        for (int i = 0; i < limit; i++)
        {
            sb.AppendLine($"   - {colCat[i]}: {colCount[i]} peatones");
        }
        return sb.ToString().TrimEnd();
    }

    // 17. Fin de semana vs entre semana
    public string FinDeSemanaVsEntreSemana()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var culturaEspana = new CultureInfo("es-ES");
        long finDeSemana = 0;
        long entreSemana = 0;

        foreach (var fechaStr in colFecha)
        {
            if (fechaStr != null && DateTime.TryParseExact(fechaStr.ToString(), "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    finDeSemana++;
                }
                else
                {
                    entreSemana++;
                }
            }
        }

        return $"\n17. Fin de semana vs Entre semana -> Fin de semana: {finDeSemana} | Entre semana: {entreSemana} \n";
    }

    // 18. Media de accidentes por día
    public string MediaAPorDia()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var culturaEspana = new CultureInfo("es-ES");
        var diasUnicos = new HashSet<string>();

        foreach (var fechaStr in colFecha)
        {
            if (fechaStr != null && DateTime.TryParseExact(fechaStr.ToString(), "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                diasUnicos.Add(dt.ToString("yyyy-MM-dd"));
            }
        }

        double media = diasUnicos.Count > 0 ? (double)_df.Rows.Count / diasUnicos.Count : 0;
        return $"18. Media de accidentes por día: {media:F2} accidentes/día (sobre {diasUnicos.Count} días distintos)\n";
    }

    // 19. Accidentes con alcohol + droga
    public string AlcoholYDrogas()
    {
        if (_df == null) return "DataFrame no cargado.";
    
        var colAlcohol = (StringDataFrameColumn)_df["positiva_alcohol"];
        var colDroga = (StringDataFrameColumn)_df["positiva_droga"];
    
        long ambos = 0;
        for (long i = 0; i < _df.Rows.Count; i++)
        {
            if (colAlcohol[i]?.ToString() == "S" && colDroga[i]?.ToString() == "S")
            {
                ambos++;
            }
        }
    
        return $"19. Accidentes con alcohol y droga: {ambos}\n";
    }

    // 20. Rangos de edad más vulnerables (peatones)
    public string RangosEdadPeatonesVulnerables()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colPersona = (StringDataFrameColumn)_df["tipo_persona"];
        var dfPeatones = _df.Filter(colPersona.ElementwiseEquals("Peatón"));
        
        var valueCounts = dfPeatones["rango_edad"].ValueCounts();
        
        var dfTones = (StringDataFrameColumn)valueCounts.Columns[0];
        var colTones = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
        
        var sb = new StringBuilder("20. Rangos de edad más vulnerables (Peatones):\n");
        
        int limit = Math.Min(5, (int)valueCounts.Rows.Count);
        for (int i = 0; i < limit; i++)
        {
            var distritoVal = dfTones[i];
            var conteoVal = colTones[i];
            sb.AppendLine($"   - {distritoVal}: {conteoVal} peatones");
        }
        return sb.ToString().TrimEnd();
    }
// 21. Distritos con más positivos en alcohol
    public string DistritosMasPositivosAlcohol()
    {
        if (_df == null) return "DataFrame no cargado.";
    
        var colAlcohol = (StringDataFrameColumn)_df["positiva_alcohol"];
        var dfPositivos = _df.Filter(colAlcohol.ElementwiseEquals("S"));
    
        var valueCounts = dfPositivos["distrito"].ValueCounts();
    
        // Acceso correcto a las columnas del DataFrame resultante mediante .Columns[...]
        var distritoCol = (StringDataFrameColumn)valueCounts.Columns[0];
        var countCol = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];

        var sb = new StringBuilder("\n21. Distritos con más positivos en alcohol (Top 5):\n");
    
        int limit = Math.Min(5, (int)valueCounts.Rows.Count);
        for (int i = 0; i < limit; i++)
        {
            var distritoVal = distritoCol[i];
            var conteoVal = countCol[i];
            sb.AppendLine($"   - {distritoVal}: {conteoVal} positivos");
        }
        return sb.ToString().TrimEnd();
    }

    // 22. Accidentes por código de distrito
    public string ACodigoDistrito()
    {
        if (_df == null) return "DataFrame no cargado.";
    
        var valueCounts = _df["cod_distrito"].ValueCounts();
        var colCat = (StringDataFrameColumn)valueCounts.Columns[0];
        var colCount = (PrimitiveDataFrameColumn<long>)valueCounts.Columns[1];
    
        var sb = new StringBuilder("\n22. Accidentes por código de distrito:\n");
    
        for (int i = 0; i < valueCounts.Rows.Count; i++)
        {
            sb.AppendLine($"   - Código {colCat[i]}: {colCount[i]} accidentes");
        }
        return sb.ToString().TrimEnd();
    }

    // 23. Accidentes por año
    public string AAno()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var culturaEspana = new CultureInfo("es-ES");
        var anios = new Dictionary<string, int>();

        foreach (var fechaStr in colFecha)
        {
            if (fechaStr != null && DateTime.TryParseExact(fechaStr.ToString(), "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string anio = dt.Year.ToString();
                anios[anio] = anios.GetValueOrDefault(anio, 0) + 1;
            }
        }

        var sb = new StringBuilder("\n23. Accidentes por año:\n");
        foreach (var kvp in anios.OrderBy(x => x.Key))
        {
            sb.AppendLine($"   - Año {kvp.Key}: {kvp.Value} accidentes");
        }
        return sb.ToString().TrimEnd();
    }

    // 24. Evolución mensual por año
    public string EvolucionMensualAno()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var culturaEspana = new CultureInfo("es-ES");
        var evolucion = new Dictionary<string, int>(); // Clave: "YYYY-MM"

        foreach (var fechaStr in colFecha)
        {
            if (fechaStr != null && DateTime.TryParseExact(fechaStr.ToString(), "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string mesAnio = dt.ToString("yyyy-MM", culturaEspana);
                evolucion[mesAnio] = evolucion.GetValueOrDefault(mesAnio, 0) + 1;
            }
        }

        var sb = new StringBuilder("\n24. Evolución mensual por año:\n");
        foreach (var kvp in evolucion.OrderBy(x => x.Key))
        {
            sb.AppendLine($"   - {kvp.Key}: {kvp.Value} accidentes");
        }
        return sb.ToString().TrimEnd();
    }

    // 25. Distrito con más accidentes por año
    public string DistritoMasAAno()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var colDistrito = (StringDataFrameColumn)_df["distrito"];
        var culturaEspana = new CultureInfo("es-ES");
        
        // Diccionario de Anio -> (Diccionario de Distrito -> Conteo)
        var anioDistrito = new Dictionary<string, Dictionary<string, int>>();

        for (long i = 0; i < _df.Rows.Count; i++)
        {
            var fechaStr = colFecha[i]?.ToString();
            var distrito = colDistrito[i]?.ToString() ?? "Desconocido";

            if (fechaStr != null && DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string anio = dt.Year.ToString();
                if (!anioDistrito.ContainsKey(anio)) anioDistrito[anio] = new Dictionary<string, int>();
                
                anioDistrito[anio][distrito] = anioDistrito[anio].GetValueOrDefault(distrito, 0) + 1;
            }
        }

        var sb = new StringBuilder("\n25. Distrito con más accidentes por año:\n");
        foreach (var anioKvp in anioDistrito.OrderBy(x => x.Key))
        {
            var topDistrito = anioKvp.Value.OrderByDescending(x => x.Value).FirstOrDefault();
            sb.AppendLine($"   - Año {anioKvp.Key}: {topDistrito.Key} ({topDistrito.Value} accidentes)");
        }
        return sb.ToString().TrimEnd();
    }

    // 26. Tendencia de alcohol por año
    public string TendenciaAlcoholAno()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var colAlcohol = (StringDataFrameColumn)_df["positiva_alcohol"];
        var culturaEspana = new CultureInfo("es-ES");
        var alcoholPorAnio = new Dictionary<string, int>();

        for (long i = 0; i < _df.Rows.Count; i++)
        {
            var fechaStr = colFecha[i]?.ToString();
            var alcohol = colAlcohol[i]?.ToString();

            if (alcohol == "S" && fechaStr != null && DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string anio = dt.Year.ToString();
                alcoholPorAnio[anio] = alcoholPorAnio.GetValueOrDefault(anio, 0) + 1;
            }
        }

        var sb = new StringBuilder("\n26. Tendencia de positivos en alcohol por año:\n");
        foreach (var kvp in alcoholPorAnio.OrderBy(x => x.Key))
        {
            sb.AppendLine($"   - Año {kvp.Key}: {kvp.Value} positivos");
        }
        return sb.ToString().TrimEnd();
    }

    // 27. Comparativa fin de semana vs entre semana por año
    public string ComparativaFinDeSemanaAno()
    {
        if (_df == null) return "DataFrame no cargado.";
    
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var culturaEspana = new CultureInfo("es-ES");
        var statsAnio = new Dictionary<string, (int FinDeSemana, int EntreSemana)>();

        for (long i = 0; i < _df.Rows.Count; i++)
        {
            var fechaStr = colFecha[i]?.ToString();
            if (fechaStr != null && DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string anio = dt.Year.ToString();
            
                // Inicializamos la tupla para el año si no existe
                if (!statsAnio.ContainsKey(anio))
                {
                    statsAnio[anio] = (0, 0);
                }
            
                var current = statsAnio[anio];

                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    statsAnio[anio] = (current.FinDeSemana + 1, current.EntreSemana);
                }
                else
                {
                    statsAnio[anio] = (current.FinDeSemana, current.EntreSemana + 1);
                }
            }
        }

        var sb = new StringBuilder("\n27. Comparativa fin de semana vs entre semana por año:\n");
        foreach (var kvp in statsAnio.OrderBy(x => x.Key))
        {
            sb.AppendLine($"   - Año {kvp.Key} -> Fin de semana: {kvp.Value.FinDeSemana} | Entre semana: {kvp.Value.EntreSemana}");
        }
        return sb.ToString().TrimEnd();
    }

    // 28. Hora pico por año
    public string HoraPicoAno()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var colHora = (StringDataFrameColumn)_df["hora"];
        var culturaEspana = new CultureInfo("es-ES");
        
        // Anio -> (Hora -> Conteo)
        var horasPorAnio = new Dictionary<string, Dictionary<string, int>>();

        for (long i = 0; i < _df.Rows.Count; i++)
        {
            var fechaStr = colFecha[i]?.ToString();
            var horaStr = colHora[i]?.ToString();

            if (fechaStr != null && horaStr != null && DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string anio = dt.Year.ToString();
                string horaKey = horaStr.Length >= 2 ? horaStr.Substring(0, 2) : horaStr;

                if (!horasPorAnio.ContainsKey(anio)) horasPorAnio[anio] = new Dictionary<string, int>();
                horasPorAnio[anio][horaKey] = horasPorAnio[anio].GetValueOrDefault(horaKey, 0) + 1;
            }
        }

        var sb = new StringBuilder("\n28. Hora pico por año:\n");
        foreach (var anioKvp in horasPorAnio.OrderBy(x => x.Key))
        {
            var horaPico = anioKvp.Value.OrderByDescending(x => x.Value).FirstOrDefault();
            sb.AppendLine($"   - Año {anioKvp.Key}: {horaPico.Key}:00 horas ({horaPico.Value} accidentes)");
        }
        return sb.ToString().TrimEnd();
    }

    // 29. Lesión más frecuente por año
    public string LesionFrecuenteAno()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var colLesividad = (StringDataFrameColumn)_df["lesividad"];
        var culturaEspana = new CultureInfo("es-ES");
        
        // Anio -> (Lesividad -> Conteo)
        var lesionesPorAnio = new Dictionary<string, Dictionary<string, int>>();

        for (long i = 0; i < _df.Rows.Count; i++)
        {
            var fechaStr = colFecha[i]?.ToString();
            var lesividad = colLesividad[i]?.ToString() ?? "Desconocido";

            if (fechaStr != null && DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string anio = dt.Year.ToString();
                if (!lesionesPorAnio.ContainsKey(anio)) lesionesPorAnio[anio] = new Dictionary<string, int>();
                
                lesionesPorAnio[anio][lesividad] = lesionesPorAnio[anio].GetValueOrDefault(lesividad, 0) + 1;
            }
        }

        var sb = new StringBuilder("\n29. Lesión más frecuente por año:\n");
        foreach (var anioKvp in lesionesPorAnio.OrderBy(x => x.Key))
        {
            var lesionTop = anioKvp.Value.OrderByDescending(x => x.Value).FirstOrDefault();
            sb.AppendLine($"   - Año {anioKvp.Key}: {lesionTop.Key} ({lesionTop.Value} casos)");
        }
        return sb.ToString().TrimEnd();
    }

    // 30. Evolución de peatones por año
    public string EvolucionPeatonesAno()
    {
        if (_df == null) return "DataFrame no cargado.";
        
        var colFecha = (StringDataFrameColumn)_df["fecha"];
        var colPersona = (StringDataFrameColumn)_df["tipo_persona"];
        var culturaEspana = new CultureInfo("es-ES");
        var peatonesPorAnio = new Dictionary<string, int>();

        for (long i = 0; i < _df.Rows.Count; i++)
        {
            var fechaStr = colFecha[i]?.ToString();
            var tipoPersona = colPersona[i]?.ToString();

            if (tipoPersona == "Peatón" && fechaStr != null && DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", culturaEspana, DateTimeStyles.None, out var dt))
            {
                string anio = dt.Year.ToString();
                peatonesPorAnio[anio] = peatonesPorAnio.GetValueOrDefault(anio, 0) + 1;
            }
        }

        var sb = new StringBuilder("\n30. Evolución de peatones implicados por año:\n");
        foreach (var kvp in peatonesPorAnio.OrderBy(x => x.Key))
        {
            sb.AppendLine($"   - Año {kvp.Key}: {kvp.Value} peatones");
        }
        return sb.ToString().TrimEnd();
    }
}
