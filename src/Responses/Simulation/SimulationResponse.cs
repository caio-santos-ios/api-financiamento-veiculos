namespace api_financiamento.src.Responses.Simulation
{
    public class SimulationResponse
    {
        public string Id { get; set; } = string.Empty;
        public decimal VehicleValue { get; set; }
        public decimal DownPayment { get; set; }
        public decimal MonthlyInterestRate { get; set; }
        public int Installments { get; set; }
        public decimal FinancedAmount { get; set; }
        public decimal InstallmentValue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalInterest { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public List<AmortizationRowResponse> AmortizationTable { get; set; } = [];
        public DateTime CreatedAt { get; set; }
    }

    public class AmortizationRowResponse
    {
        public int Month { get; set; }
        public decimal Installment { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Balance { get; set; }
    }

    public class CompareInstallmentsResponse
    {
        public List<InstallmentOptionResponse> Options { get; set; } = [];
        public int BestInstallments { get; set; }
    }

    public class InstallmentOptionResponse
    {
        public int Installments { get; set; }
        public decimal InstallmentValue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalInterest { get; set; }
        public bool IsBest { get; set; }
    }
}
