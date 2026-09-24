using System.Globalization;
using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;
using CsvHelper.Configuration;
using Microsoft.Data.Analysis;

namespace AccidentesMadrid.Repositories;

public class AccidentesRepository : IAccidentesRepository
{
    /*
     * Calse donde se deben cojer los datos de los csv y combinarlos en una sola coleccion
     * Lectura CSV:  // Lectura
            using var reader = new StreamReader("personas.csv");
            using var csvReader = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
            
            var personasLeidas = csvReader.GetRecords<Persona>().ToList();
     */
    public async Task<List<Accidente>> GetListaAccidentesLinq()
    {
        var culturaEspana = new CultureInfo("es-ES");
        // 1. Configurar CsvHelper para que use el punto y coma (";") como separador
        var separador = new CsvHelper.Configuration.CsvConfiguration(culturaEspana)
        {
            Delimiter = ";",
        };
        string basePath = Path.Combine(AppContext.BaseDirectory, "../../../data");
        
        // 2. Conectamos con los ficheros, que no es lo mismo que leerlos
        using var reader1 = new StreamReader(Path.Combine(basePath,"2024-accidentes-trafico-detalle-csv.csv")); 
        using var reader2 = new StreamReader(Path.Combine(basePath,"2025-accidentes-trafico-detalle-csv.csv"));
        using var reader3 = new StreamReader(Path.Combine(basePath, "2026-accidentes-trafico-detalle.csv"));
        
        // 3. Decimos que haga la separacion por ; 
        using var csvReader1 = new CsvHelper.CsvReader(reader1, separador);
        using var csvReader2 = new CsvHelper.CsvReader(reader2, separador);
        using var csvReader3 = new CsvHelper.CsvReader(reader3, separador);
            
        // Para que se comunique con el mapper y pueda mapear los datos del csv (num_expediente) al modelo (NumExpediente)
        //se añade antes de la combinacion total porque se debe haeer nada mas leer el fichero
        csvReader1.Context.RegisterClassMap<AccidenteMapper>();
        csvReader2.Context.RegisterClassMap<AccidenteMapper>();
        csvReader3.Context.RegisterClassMap<AccidenteMapper>();
        
        // 4. Creamnos las listas de cada uno
        List<Accidente> accidentes1 = new();
        List<Accidente> accidentes2 = new();
        List<Accidente> accidentes3 = new();
        
        //IMPORTANTE. Ahora convertimos las listas anteriores y les pasamos la data. 
        // Como ya hemos enviado los datos al Mapper para que pase al modelo y hemos usado StreamReader, ya los datos pasan a estar con la CPU, no perteneciendo a operaciones de I/O
        // Por ello utilizamos Task.Run() y .Result
        try
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var tarea1 = Task.Run(() => csvReader1.GetRecords<Accidente>().ToList());
            var tarea2 = Task.Run(() => csvReader2.GetRecords<Accidente>().ToList());
            var tarea3 = Task.Run(() => csvReader3.GetRecords<Accidente>().ToList());

            
            // Procesamos las tres tareas en paralelo
            await Task.WhenAll(tarea1, tarea2, tarea3);
            
            watch.Stop();
            
            accidentes1 = tarea1.Result;
            accidentes2 = tarea2.Result;
            accidentes3 = tarea3.Result;
            
            Console.WriteLine($" Tiempo de lectura de ficheros: {watch.ElapsedMilliseconds} ms\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.GetType().Name}");
            Console.WriteLine(ex.Message);
            return new List<Accidente>();
        }

        // 5. Y aqui ya las combinamos
        var combinancion = accidentes1.Concat(accidentes2).Concat(accidentes3).ToList();
        
        //6. Revision
        Console.WriteLine(combinancion.Count);
        
        return combinancion;
    }

    public string GetListaAccidentesFrame()
    {
        var f = "f";
        
        return f;
    }

   
}