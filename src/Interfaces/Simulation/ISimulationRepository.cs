using api_financiamento.src.Models;
using api_financiamento.src.Models.Base;
using api_financiamento.src.Shared.Utils;

namespace api_financiamento.src.Interfaces
{
    public interface ISimulationRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Simulation> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Simulation?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Simulation> pagination);
        Task<ResponseApi<Simulation?>> CreateAsync(Simulation simulation);
        Task<ResponseApi<Simulation?>> UpdateAsync(Simulation simulation);
        Task<ResponseApi<Simulation>> DeleteAsync(string id);
    }
}
