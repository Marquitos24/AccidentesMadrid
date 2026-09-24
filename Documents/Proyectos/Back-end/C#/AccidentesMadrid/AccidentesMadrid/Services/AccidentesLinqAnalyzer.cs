using AccidentesMadrid.Models;
using AccidentesMadrid.Repositories;
using System.Linq;

namespace AccidentesMadrid.Services;

public class AccidentesLinqAnalyzer
{
    private AccidentesRepository repo = new AccidentesRepository();
    
    // Aquí guardaremos la lista ya combinada para usarla en todas las consultas
    private List<Accidente> accidentes = new();

    // Método principal que arranca el análisis
    public async Task TareasLinqAsync()
    {
        // 1. Cargamos los datos una sola vez
        accidentes = await repo.GetListaAccidentesLinq();

        if (accidentes.Count == 0)
        {
            Console.WriteLine("No se han podido cargar los datos.");
            return;
        }

        var watch = System.Diagnostics.Stopwatch.StartNew();
        // Llamamos a cada consulta
        Console.WriteLine(ObtenerTotalAccidentes() + "\n");
        Console.WriteLine(Top5AccidentesDistrito() + "\n");
        Console.WriteLine(TipoAccidente() + "\n");
        Console.WriteLine(EstadoMeteo() + "\n");
        Console.WriteLine(ASexo() + "\n");
        Console.WriteLine(AEdad() + "\n");
        Console.WriteLine(AAlchol() + "\n");
        Console.WriteLine(ADrogas() + "\n");
        Console.WriteLine(ADiasSemana() + "\n");
        Console.WriteLine(AMes() + "\n");
        Console.WriteLine(AHora() + "\n");
        Console.WriteLine(LesionesFrecuentes() + "\n");
        Console.WriteLine(TipoV() + "\n");
        Console.WriteLine(APeatones() + "\n");
        Console.WriteLine(ProporcionHombreMujer() + "\n");
        
        watch.Stop();
        Console.WriteLine($"⏱️ Tiempo de ejecución (LINQ/PLINQ): {watch.ElapsedMilliseconds} ms\n");
    }
    // Tarea 1: Método independiente y limpio
    public string ObtenerTotalAccidentes()
    {
        Console.WriteLine("============ Accidentes total registrados del año 2024-2026 ============");

        int total = accidentes.Count;
        return $"Total: {total} accidentes";
    }

    public string Top5AccidentesDistrito()
    {
        Console.WriteLine("============ Top 5 distritos con mas accidentes del año 2024-2026 ============");


        var distritos = accidentes.GroupBy(a => a.Distrito)
            .Select(g => new
        {
            Distrito = g.Key,
            Accidentes = g.ToList(),
            Total = g.Count(),
        }).OrderByDescending(g => g.Total).Take(5);

        var result = "";
        
        foreach (var distrito in distritos)
        {
            result += $"Distrito: {distrito.Distrito} -> Total: {distrito.Total} accidentes. \n";
        }

        return result;
    }

    public string TipoAccidente()
    {
        Console.WriteLine("============ ACCIDENTES POR SUS TIPOS ============");

        var tipoAccidente = accidentes.GroupBy(a => a.TipoAccidente)
            .Select(g => new
            {
                TipoAccidente = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            }).OrderByDescending(g => g.TipoAccidente);

        var result = "";
        
        foreach (var tipos in tipoAccidente)
        {
            result += $"Tipo de accidente: {tipos.TipoAccidente} -> Total: {tipos.Total} accidentes. \n";
        }
        return result;
    }
    
    public string EstadoMeteo()
    {
        Console.WriteLine("============ ACCIDENTES POR ESTADO METEROOLOGICO ============");

        var estadoMete = accidentes.GroupBy(a => a.EstadoMeteorologico)
            .Select(g => new
            {
                EstadoMeteorologico = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            }).OrderByDescending(g => g.EstadoMeteorologico);

        var result = "";
        
        foreach (var estados in estadoMete)
        {
            result += $"Estado meteorologico: {estados.EstadoMeteorologico} -> Total: {estados.Total} accidentes. \n";
        }
        return result;
    }
    public string ASexo()
    {
        Console.WriteLine("============ ACCIDENTES POR SEXO ============");

        var sexoo = accidentes.GroupBy(a => a.Sexo)
            .Select(g => new
            {
                Sexo = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            }).OrderByDescending(g => g.Sexo);

        var result = "";
        
        foreach (var sexos in sexoo)
        {
            result += $"Sexo: {sexos.Sexo} -> Total: {sexos.Total} accidentes. \n";
        }
        return result;
    }
    
    public string AEdad()
    {
        Console.WriteLine("============ ACCIDENTES POR RANGO DE EDAD ============");

        var rango = accidentes.GroupBy(a => a.RengoEdad)
            .Select(g => new
            {
                RengoEdad = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            }).OrderByDescending(g => g.RengoEdad);

        var result = "";
        
        foreach (var edades in rango)
        {
            result += $"Rango de edad: {edades.RengoEdad} -> Total: {edades.Total} accidentes. \n";
        }
        return result;
    }
    public string AAlchol()
    {
        Console.WriteLine("============ ACCIDENTES POR POSITIVOS EN ALCHOL ============");

        var config = accidentes.Where(a => a.Alchol.Equals(true)).ToList().GroupBy(a => a.AlcholTexto)
            .Select(g => new
            {
                AlcholTexto = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            }).OrderByDescending(g => g.AlcholTexto);

        var result = "";
        
        foreach (var positivos in config)
        {
            result = $"Alchol: {positivos.AlcholTexto} -> Total: {positivos.Total} accidentes. \n";
        }
        return result;
    }
    public string ADrogas()
    {
        Console.WriteLine("============ ACCIDENTES POR POSITIVOS EN DROGAS ============");

        var config = accidentes.Where(a => a.Drogas.Equals(true)).ToList().GroupBy(a => a.DrogasTexto)
            .Select(g => new
            {
                DrogasTexto = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            }).OrderByDescending(g => g.DrogasTexto);

        var result = "";
        
        foreach (var positivos in config)
        {
            result = $"Drogas: {positivos.DrogasTexto} -> Total: {positivos.Total} accidentes. \n";
        }
        return result;
    }
    
    public string ADiasSemana()
    {
        Console.WriteLine("============ ACCIDENTES POR DIAS DE LA SEMANA ============");
      
        var culturaEspanol = new System.Globalization.CultureInfo("es-ES");  // Devuelva los nombres de los días en español

        var resultado = accidentes
            .GroupBy(a => a.Fecha.DayOfWeek)
            .Select(g => new
            {
                // g.Key te da el DayOfWeek (Monday, Tuesday...), y con la cultura lo pasamos a texto en español
                Dia = culturaEspanol.DateTimeFormat.GetDayName(g.Key),
                Total = g.Count()
            });

        // Construimos un texto legible para mostrarlo
        var mensaje = "Accidentes por día de la semana:\n";
        foreach (var item in resultado)
        {
            // Capitalizamos la primera letra (Lunes, Martes...) para que quede profesional
            string diaFormateado = char.ToUpper(item.Dia[0]) + item.Dia.Substring(1);
            mensaje += $"- {diaFormateado}: {item.Total} accidentes\n";
        }
        return mensaje;
    }
    
    public string AMes()
    {
        Console.WriteLine("============ ACCIDENTES POR MES ============");
      
        var culturaEspanol = new System.Globalization.CultureInfo("es-ES");  // Devuelva los nombres de los días en español

        var resultado = accidentes
            .GroupBy(a => a.Fecha.Month)
            .Select(g => new
            {
                // g.Key te da el DayOfWeek (Monday, Tuesday...), y con la cultura lo pasamos a texto en español
                Mes = culturaEspanol.DateTimeFormat.GetMonthName(g.Key),
                Total = g.Count()
            });

        // Construimos un texto legible para mostrarlo
        var mensaje = "Accidentes por mes:\n";
        foreach (var item in resultado)
        {
            // Capitalizamos la primera letra (Lunes, Martes...) para que quede profesional
            string mesFormateado = char.ToUpper(item.Mes[0]) + item.Mes.Substring(1);
            mensaje += $"- {mesFormateado}: {item.Total} accidentes\n";
        }
        return mensaje;
    }
    public string AHora()
    {
        Console.WriteLine("============ HORA CON MÁS ACCIDENTES ============");
        
        var hora = accidentes.GroupBy(a => a.Hora.Hour)
            .Select(g => new
            {
                Hora = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            });

        var result = "";
        
        foreach (var horas in hora)
        {
            result = $"Hora con más accidentes: {horas.Hora}:00 -> Total: {horas.Total} accidentes. \n";
        }
        return result;
    }
    
    public string LesionesFrecuentes()
    {
        Console.WriteLine("============ LESIONES FRECUENTES ============");
        
        var config = accidentes.GroupBy(a => a.Lesividad)
            .Select(g => new
            {
                Lesividad = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            }).OrderByDescending(g => g.Total);

        var result = "Lesiones más frecuentes:\n";
        var contador = 1;
        
        foreach (var lesiones in config)
        {
            result += $"{contador}. {lesiones.Lesividad} -> Total: {lesiones.Total} accidentes. \n";
            contador++;
        }
        return result;
    }
    
    public string TipoV()
    {
        Console.WriteLine("============ TIPO DE VEHICULO MAS IMPLICADO EN ACCIDENTES ============");
        
        var config = accidentes.GroupBy(a => a.TipoVehiculo)
            .Select(g => new
            {
                Vehiculo = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            }).OrderByDescending(g => g.Total);

        var result = "Tipos de vehiculos más implicados en accidentes:\n";
        var contador = 1;
        
        foreach (var tipos in config)
        {
            result += $"{contador}. {tipos.Vehiculo} -> Total: {tipos.Total} accidentes. \n";
            contador++;
        }
        return result;
    }
    
    public string APeatones()
    {
        Console.WriteLine("============ ACCIDENTES CON PEATONES ============");
        var config = accidentes.GroupBy(a => a.TipoPersona == TipoPersona.Peaton || a.TipoPersona == TipoPersona.PeatonSc)
            .Select(g => new
            {
                TipoPersona = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            });

        var result = "";
        foreach (var tipos in config)
        {
            result = $"Accidentes con peatones -> Total: {tipos.Total} accidentes. \n";
        }
        return result;
    }
    
    public string ProporcionHombreMujer()
    {
        Console.WriteLine("============ PROPORCIÓN DE HOMBRES/MUJERES DE ACCIDENTES ============");
        var configMan = accidentes.GroupBy(a => a.Sexo == Sexo.Hombre)
            .Select(g => new
            {
                Hombre = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            });
        
        var configWoman = accidentes.GroupBy(a => a.Sexo == Sexo.Mujer)
            .Select(g => new
            {
                Mujer = g.Key,
                Accidentes = g.ToList(),
                Total = g.Count(),
            });
    // Son X hombres y X mijeres EJ: 55 H y 10 M. 
        int menA = 0;
        int womanA = 0;

        
        foreach (var hombres in configMan){menA = hombres.Total;}
        foreach (var mujeres in configWoman){womanA = mujeres.Total;}

        return $"Proporción de accidentes: Hombre: {menA} accidentes / Mujer:  {womanA} accidentes.";
    }
}







    