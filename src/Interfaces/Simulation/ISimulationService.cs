using api_financiamento.src.Models;
using api_financiamento.src.Models.Base;
using api_financiamento.src.Responses.Simulation;
using api_financiamento.src.Shared.DTOs;

namespace api_financiamento.src.Interfaces
{
    public interface ISimulationService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAsync(string id);
        Task<ResponseApi<Simulation?>> CreateAsync(CreateSimulationDTO dto);
        Task<ResponseApi<Simulation?>> UpdateAsync(UpdateSimulationDTO dto);
        Task<ResponseApi<Simulation>> DeleteAsync(string id);
        ResponseApi<SimulationResponse> Calculate(CalculateSimulationDTO dto);
        ResponseApi<CompareInstallmentsResponse> CompareInstallments(decimal vehicleValue, decimal downPayment, decimal monthlyInterestRate);
    }
}
