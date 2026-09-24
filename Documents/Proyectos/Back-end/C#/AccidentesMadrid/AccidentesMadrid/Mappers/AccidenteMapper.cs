using AccidentesMadrid.Models;
using AccidentesMadrid.Repositories;
using CsvHelper.Configuration;

namespace AccidentesMadrid.Mappers;

public class AccidenteMapper : ClassMap<Accidente>
{
    /// <summary>
    /// Mapeamos los datos de los csv a el model.
    /// </summary>
    public AccidenteMapper()
    {
        Map(m => m.NumExpediente).Name("num_expediente");
        Map(m => m.Fecha).Name("fecha").TypeConverterOption.Format("dd/MM/yyyy");;
      

        Map(m => m.Hora).Name("hora").TypeConverterOption.Format("H:mm:ss");;
        Map(m => m.Localizacion).Name("localizacion");
        Map(m => m.NumeroLocalizacion).Name("numero");
        Map(m => m.CodDistrito).Name("cod_distrito");
        Map(m => m.Distrito).Name("distrito");
        Map(m => m.TipoAccidente).Convert(row => 
        {
            // Obtenemos el texto de la columna del CSV
            string valorFila = row.Row.GetField("tipo_accidente");

            // Si está vacío o nulo, devolvemos un valor por defecto
            if (string.IsNullOrWhiteSpace(valorFila))
            {
                return "Desconocido"; 
            }
            // Si tiene contenido, devolvemos  valor original
            return valorFila.Trim();
        });
        Map(m => m.EstadoMeteorologico).Convert(row => 
        {
            string valorFila = row.Row.GetField("estado_meteorológico");
            
            if (string.IsNullOrWhiteSpace(valorFila))
            {
                return "Desconocido"; 
            }
            return valorFila.Trim();
        });
        
        Map(m => m.TipoVehiculo).Name("tipo_vehiculo");
        Map(m => m.TipoPersona).Convert(row => 
        {
            // 'row.Row.GetField("NombreColumnaEnCSV")' obtiene el texto crudo del CSV
            string valorFila = row.Row.GetField("tipo_persona");

            return valorFila switch
            {
                "Conductor" => TipoPersona.Conductor,
                "Pasajero" => TipoPersona.Pasajero,
                "Peatón" => TipoPersona.Peaton,
                "Peatón (atropello sc)" => TipoPersona.PeatonSc,
                _ => TipoPersona.Peaton
            };
        });
        
        Map(m => m.RengoEdad).Name("rango_edad");
        
        Map(m => m.Sexo).Convert(row => 
        { 
            string valorFila = row.Row.GetField("sexo");

            return valorFila switch
            {
                "Hombre" => Sexo.Hombre,
                "Mujer" => Sexo.Mujer,
                "Desconocido" => Sexo.Desconocido,
                _ => Sexo.Desconocido
            };
        });
        
        Map(m => m.CodLesividad).Name("cod_lesividad");
        Map(m => m.Lesividad).Convert(row => 
        {
            string valorFila = row.Row.GetField("lesividad");
            
            if (string.IsNullOrWhiteSpace(valorFila))
            {
                return "Desconocida"; 
            }
            return valorFila.Trim();
        });
        
        Map(m => m.CordenadaX).Name("coordenada_x_utm");
        Map(m => m.CordenadaY).Name("coordenada_y_utm");
        Map(m => m.Alchol).Convert(row => 
        {
            // 'row.Row.GetField("NombreColumnaEnCSV")' obtiene el texto crudo del CSV
            string valorFila = row.Row.GetField("positiva_alcohol")?.Trim();
            
            return valorFila switch
            {
                "S" => true,
                "N" => false,
                _ => false
            };
           
        });
        Map(m => m.Drogas).Convert(row => 
        {
            // 'row.Row.GetField("NombreColumnaEnCSV")' obtiene el texto crudo del CSV
            string valorFila = row.Row.GetField("positiva_droga")?.Trim();

            return valorFila switch
            {
                "S" => true,
                "N" => false,
                _ => false
            };
        });
    }
}