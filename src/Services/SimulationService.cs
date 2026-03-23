using api_financiamento.src.Interfaces;
using api_financiamento.src.Models;
using api_financiamento.src.Models.Base;
using api_financiamento.src.Responses.Simulation;
using api_financiamento.src.Shared.DTOs;
using api_financiamento.src.Shared.Utils;
using api_financiamento.src.Shared.Validators;

namespace api_financiamento.src.Services
{
    public class SimulationService(ISimulationRepository simulationRepository) : ISimulationService
    {
        private readonly int[] _installmentOptions = [24, 36, 48, 60];

        #region CALCULATE (Tabela Price)
        public ResponseApi<SimulationResponse> Calculate(CalculateSimulationDTO dto)
        {
            try
            {
                if (!Validator.IsPositiveDecimal(dto.VehicleValue))
                    return new(null, 400, "Valor do veículo deve ser maior que zero.");

                if (dto.DownPayment < 0 || dto.DownPayment >= dto.VehicleValue)
                    return new(null, 400, "Entrada deve ser entre 0 e o valor do veículo.");

                if (!Validator.IsValidInstallments(dto.Installments))
                    return new(null, 400, "Número de parcelas deve ser 24, 36, 48 ou 60.");

                var result = RunPriceTable(dto.VehicleValue, dto.DownPayment, dto.MonthlyInterestRate, dto.Installments);
                return new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public ResponseApi<CompareInstallmentsResponse> CompareInstallments(decimal vehicleValue, decimal downPayment, decimal monthlyInterestRate)
        {
            try
            {
                if (!Validator.IsPositiveDecimal(vehicleValue))
                    return new(null, 400, "Valor do veículo deve ser maior que zero.");

                var options = new List<InstallmentOptionResponse>();

                foreach (var n in _installmentOptions)
                {
                    var calc = RunPriceTable(vehicleValue, downPayment, monthlyInterestRate, n);
                    options.Add(new InstallmentOptionResponse
                    {
                        Installments = n,
                        InstallmentValue = calc.InstallmentValue,
                        TotalPaid = calc.TotalPaid,
                        TotalInterest = calc.TotalInterest
                    });
                }

                var minTotalPaid = options.Min(o => o.TotalPaid);
                var best = options.First(o => o.TotalPaid == minTotalPaid);
                best.IsBest = true;

                return new(new CompareInstallmentsResponse
                {
                    Options = options,
                    BestInstallments = best.Installments
                });
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Simulation?>> CreateAsync(CreateSimulationDTO dto)
        {
            try
            {
                if (!Validator.IsPositiveDecimal(dto.VehicleValue))
                    return new(null, 400, "Valor do veículo deve ser maior que zero.");

                if (dto.DownPayment < 0 || dto.DownPayment >= dto.VehicleValue)
                    return new(null, 400, "Entrada deve ser entre 0 e o valor do veículo.");

                if (!Validator.IsValidInstallments(dto.Installments))
                    return new(null, 400, "Número de parcelas deve ser 24, 36, 48 ou 60.");

                var calc = RunPriceTable(dto.VehicleValue, dto.DownPayment, dto.MonthlyInterestRate, dto.Installments);

                Simulation simulation = new()
                {
                    VehicleValue = dto.VehicleValue,
                    DownPayment = dto.DownPayment,
                    MonthlyInterestRate = dto.MonthlyInterestRate,
                    Installments = dto.Installments,
                    FinancedAmount = calc.FinancedAmount,
                    InstallmentValue = calc.InstallmentValue,
                    TotalPaid = calc.TotalPaid,
                    TotalInterest = calc.TotalInterest,
                    VehicleName = dto.VehicleName,
                    ClientName = dto.ClientName,
                    AmortizationTable = calc.AmortizationTable.Select(r => new AmortizationRow
                    {
                        Month = r.Month,
                        Installment = r.Installment,
                        Principal = r.Principal,
                        Interest = r.Interest,
                        Balance = r.Balance
                    }).ToList(),
                    CreatedBy = dto.CreatedBy,
                    UpdatedBy = dto.CreatedBy
                };

                return await simulationRepository.CreateAsync(simulation);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Simulation> pagination = new(request.QueryParams);
                int count = await simulationRepository.GetCountDocumentsAsync(pagination);
                ResponseApi<List<dynamic>> response = await simulationRepository.GetAllAsync(pagination);

                return new PaginationApi<List<dynamic>>(
                    response.Data,
                    count,
                    pagination.PageNumber,
                    pagination.PageSize
                );
            }
            catch
            {
                return new PaginationApi<List<dynamic>>(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAsync(string id)
        {
            try
            {
                return await simulationRepository.GetByIdAggregateAsync(id);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Simulation?>> UpdateAsync(UpdateSimulationDTO dto)
        {
            try
            {
                if (!Validator.IsPositiveDecimal(dto.VehicleValue))
                    return new(null, 400, "Valor do veículo deve ser maior que zero.");

                if (dto.DownPayment < 0 || dto.DownPayment >= dto.VehicleValue)
                    return new(null, 400, "Entrada deve ser entre 0 e o valor do veículo.");

                if (!Validator.IsValidInstallments(dto.Installments))
                    return new(null, 400, "Número de parcelas deve ser 24, 36, 48 ou 60.");

                ResponseApi<Simulation?> existing = await simulationRepository.GetByIdAsync(dto.Id);
                if (existing.Data is null) return new(null, 404, "Simulação não encontrada.");

                var calc = RunPriceTable(dto.VehicleValue, dto.DownPayment, dto.MonthlyInterestRate, dto.Installments);

                existing.Data.VehicleValue = dto.VehicleValue;
                existing.Data.DownPayment = dto.DownPayment;
                existing.Data.MonthlyInterestRate = dto.MonthlyInterestRate;
                existing.Data.Installments = dto.Installments;
                existing.Data.FinancedAmount = calc.FinancedAmount;
                existing.Data.InstallmentValue = calc.InstallmentValue;
                existing.Data.TotalPaid = calc.TotalPaid;
                existing.Data.TotalInterest = calc.TotalInterest;
                existing.Data.VehicleName = dto.VehicleName;
                existing.Data.ClientName = dto.ClientName;
                existing.Data.AmortizationTable = calc.AmortizationTable.Select(r => new AmortizationRow
                {
                    Month = r.Month,
                    Installment = r.Installment,
                    Principal = r.Principal,
                    Interest = r.Interest,
                    Balance = r.Balance
                }).ToList();
                existing.Data.UpdatedBy = dto.UpdatedBy;

                return await simulationRepository.UpdateAsync(existing.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Simulation>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<Simulation?> existing = await simulationRepository.GetByIdAsync(id);
                if (existing.Data is null) return new(null!, 404, "Simulação não encontrada.");

                return await simulationRepository.DeleteAsync(id);
            }
            catch
            {
                return new(null!, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region PRIVATE - Tabela Price
        private static SimulationResponse RunPriceTable(decimal vehicleValue, decimal downPayment, decimal monthlyInterestRate, int installments)
        {
            decimal financedAmount = vehicleValue - downPayment;
            decimal monthlyRate = monthlyInterestRate / 100;
            decimal installmentValue = CalculatePriceInstallment(financedAmount, monthlyRate, installments);
            decimal totalPaid = Math.Round(installmentValue * installments + downPayment, 2);
            decimal totalInterest = Math.Round(totalPaid - vehicleValue, 2);
            var table = BuildAmortizationTable(financedAmount, monthlyRate, installmentValue, installments);

            return new SimulationResponse
            {
                FinancedAmount = Math.Round(financedAmount, 2),
                InstallmentValue = Math.Round(installmentValue, 2),
                TotalPaid = totalPaid,
                TotalInterest = totalInterest,
                AmortizationTable = table
            };
        }

        private static decimal CalculatePriceInstallment(decimal principal, decimal monthlyRate, int n)
        {
            if (monthlyRate == 0) return Math.Round(principal / n, 2);
            double r = (double)monthlyRate;
            double p = (double)principal;
            double pmt = p * (r * Math.Pow(1 + r, n)) / (Math.Pow(1 + r, n) - 1);
            return (decimal)pmt;
        }

        private static List<AmortizationRowResponse> BuildAmortizationTable(decimal principal, decimal monthlyRate, decimal installmentValue, int n)
        {
            var table = new List<AmortizationRowResponse>();
            decimal balance = principal;

            for (int i = 1; i <= n; i++)
            {
                decimal interest = Math.Round(balance * monthlyRate, 2);
                decimal principalPaid = Math.Round(installmentValue - interest, 2);
                balance = Math.Round(balance - principalPaid, 2);
                if (balance < 0) balance = 0;

                table.Add(new AmortizationRowResponse
                {
                    Month = i,
                    Installment = Math.Round(installmentValue, 2),
                    Principal = principalPaid,
                    Interest = interest,
                    Balance = balance
                });
            }

            return table;
        }
        #endregion
    }
}
