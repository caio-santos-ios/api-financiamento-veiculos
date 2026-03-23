namespace api_financiamento.src.Shared.DTOs
{
    public class CreateSimulationDTO : RequestDTO
    {
        public decimal VehicleValue { get; set; }
        public decimal DownPayment { get; set; }
        public decimal MonthlyInterestRate { get; set; }
        public int Installments { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
    }

    public class UpdateSimulationDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public decimal VehicleValue { get; set; }
        public decimal DownPayment { get; set; }
        public decimal MonthlyInterestRate { get; set; }
        public int Installments { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
    }

    public class CalculateSimulationDTO
    {
        public decimal VehicleValue { get; set; }
        public decimal DownPayment { get; set; }
        public decimal MonthlyInterestRate { get; set; }
        public int Installments { get; set; }
    }
}
