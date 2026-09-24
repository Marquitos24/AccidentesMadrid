using AccidentesMadrid.Models;

namespace AccidentesMadrid.Repositories;

public interface IAccidentesRepository
{
    /// <summary>
    /// Metodo para obtener y leer los csv para luego mapearlos
    /// </summary>
    /// <returns>Lista de accidentes</returns>
    Task<List<Accidente>> GetListaAccidentesLinq();
    //string GetListaAccidentesDataFrame();
}