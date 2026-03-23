using api_financiamento.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_financiamento.src.Models
{
    public class Simulation : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("vehicleValue")]
        public decimal VehicleValue { get; set; }

        [BsonElement("downPayment")]
        public decimal DownPayment { get; set; }

        [BsonElement("monthlyInterestRate")]
        public decimal MonthlyInterestRate { get; set; }

        [BsonElement("installments")]
        public int Installments { get; set; }

        [BsonElement("financedAmount")]
        public decimal FinancedAmount { get; set; }

        [BsonElement("installmentValue")]
        public decimal InstallmentValue { get; set; }

        [BsonElement("totalPaid")]
        public decimal TotalPaid { get; set; }

        [BsonElement("totalInterest")]
        public decimal TotalInterest { get; set; }

        [BsonElement("vehicleName")]
        public string VehicleName { get; set; } = string.Empty;

        [BsonElement("clientName")]
        public string ClientName { get; set; } = string.Empty;

        [BsonElement("amortizationTable")]
        public List<AmortizationRow> AmortizationTable { get; set; } = [];
    }

    public class AmortizationRow
    {
        [BsonElement("month")]
        public int Month { get; set; }

        [BsonElement("installment")]
        public decimal Installment { get; set; }

        [BsonElement("principal")]
        public decimal Principal { get; set; }

        [BsonElement("interest")]
        public decimal Interest { get; set; }

        [BsonElement("balance")]
        public decimal Balance { get; set; }
    }
}
